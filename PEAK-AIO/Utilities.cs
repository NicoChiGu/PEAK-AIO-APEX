using BepInEx.Logging;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zorro.Core;

public static class UnityMainThreadDispatcher
{
    private static readonly Queue<Action> executionQueue = new Queue<Action>();

    public static void Enqueue(Action action)
    {
        if (action == null) return;
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    public static void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                try
                {
                    executionQueue.Dequeue().Invoke();
                }
                catch (Exception ex)
                {
                    if (ConfigManager.Logger != null)
                        ConfigManager.Logger.LogError("Error in UnityMainThreadDispatcher: " + ex);
                }
            }
        }
    }
}

public static class Utilities
{
    private static ManualLogSource Logger
    {
        get { return ConfigManager.Logger; }
    }

    public static void GetPlayer()
    {
        if (Globals.playerObj == null)
            Globals.playerObj = Player.localPlayer;
    }

    public static void UpdateItems()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            Globals.items.Clear();
            Globals.itemNames.Clear();
            for (int i = 0; i < 3; i++) Globals.selectedItems[i] = -1;

            var itemSet = new HashSet<string>();
            var collectedItems = new List<Item>();

            // 1. Try to load from ItemDatabase singleton asset
            try
            {
                var db = SingletonAsset<ItemDatabase>.Instance;
                if (db != null && db.Objects != null && db.Objects.Count > 0)
                {
                    for (int i = 0; i < db.Objects.Count; i++)
                    {
                        var item = db.Objects[i];
                        if (item != null && !string.IsNullOrEmpty(item.name))
                        {
                            string key = item.GetName();
                            if (string.IsNullOrEmpty(key)) key = item.name;
                            if (!itemSet.Contains(key))
                            {
                                itemSet.Add(key);
                                collectedItems.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogWarning("[PEAK AIO] Could not query ItemDatabase: " + ex.Message);
            }

            // 2. Fallback or augment with FindObjectsOfTypeAll
            try
            {
                UnityEngine.Object[] allItems = Resources.FindObjectsOfTypeAll(typeof(Item));
                for (int i = 0; i < allItems.Length; i++)
                {
                    var item = allItems[i] as Item;
                    if (item != null && item.gameObject.scene.handle == 0 && string.IsNullOrEmpty(item.gameObject.scene.name))
                    {
                        string key = item.GetName();
                        if (string.IsNullOrEmpty(key)) key = item.name;
                        if (!itemSet.Contains(key))
                        {
                            itemSet.Add(key);
                            collectedItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogWarning("[PEAK AIO] Could not query Resources for Items: " + ex.Message);
            }

            // Sort alphabetically by name
            collectedItems.Sort((a, b) =>
            {
                string nameA = a.GetName();
                string nameB = b.GetName();
                return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
            });

            for (int i = 0; i < collectedItems.Count; i++)
            {
                var item = collectedItems[i];
                Globals.items.Add(item);
                Globals.itemNames.Add(item.GetName());
            }

            if (Logger != null)
                Logger.LogInfo(string.Format("[PEAK AIO] Loaded {0} unique items.", Globals.items.Count));
        });
    }

    private static MethodInfo _cachedToManagedArrayMethod;

    public static byte[] SerializeSyncData<T>(T syncObj) where T : struct
    {
        if (_cachedToManagedArrayMethod == null)
        {
            Type binType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "Zorro.Core.Runtime")
                {
                    binType = assembly.GetType("Zorro.Core.Serizalization.IBinarySerializable");
                    if (binType != null) break;
                }
            }
            if (binType == null)
            {
                try
                {
                    var asm = System.Reflection.Assembly.Load("Zorro.Core.Runtime");
                    binType = asm.GetType("Zorro.Core.Serizalization.IBinarySerializable");
                }
                catch { }
            }
            if (binType == null)
            {
                binType = Type.GetType("Zorro.Core.Serizalization.IBinarySerializable, Zorro.Core.Runtime");
            }
            if (binType != null)
            {
                _cachedToManagedArrayMethod = binType.GetMethod("ToManagedArray", BindingFlags.Public | BindingFlags.Static);
            }
        }

        if (_cachedToManagedArrayMethod == null)
        {
            throw new InvalidOperationException("Failed to locate IBinarySerializable.ToManagedArray in Zorro.Core.Runtime");
        }

        return (byte[])_cachedToManagedArrayMethod.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { syncObj });
    }

    public static void AssignInventoryItem(int slot, int itemIndex)
    {
        GetPlayer();

        if (Globals.playerObj == null)
        {
            if (Logger != null)
                Logger.LogError("[PEAK AIO] Player is null during inventory operation");
            return;
        }

        if (Globals.playerObj != null &&
            Globals.playerObj.itemSlots != null &&
            Globals.playerObj.itemSlots.Length > slot &&
            itemIndex >= 0 && itemIndex < Globals.items.Count)
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                try
                {
                    var slotData = Globals.playerObj.itemSlots[slot];
                    slotData.prefab = Globals.items[itemIndex];
                    slotData.data = new ItemInstanceData(Guid.NewGuid());
                    ItemInstanceDataHandler.AddInstanceData(slotData.data);

                    var syncObj = new InventorySyncData(
                        Globals.playerObj.itemSlots,
                        Globals.playerObj.backpackSlot,
                        Globals.playerObj.tempFullSlot
                    );
                    byte[] syncData = SerializeSyncData(syncObj);

                    Globals.playerObj.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { syncData, true });
                    if (Logger != null)
                        Logger.LogInfo(string.Format("[Inventory] Assigned {0} to slot {1}", Globals.itemNames[itemIndex], slot + 1));
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError("[PEAK AIO] AssignInventoryItem error: " + ex);
                }
            });
        }
    }

    public static void SpawnItemInWorld(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= Globals.items.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var item = Globals.items[itemIndex];
                if (item != null)
                {
                    if (Character.localCharacter != null)
                    {
                        Vector3 spawnPos = Character.localCharacter.Head + Character.localCharacter.transform.forward * 1.5f + Vector3.up * 0.2f;
                        ItemDatabase.Add(item, spawnPos);
                    }
                    else
                    {
                        ItemDatabase.Add(item);
                    }

                    if (Logger != null)
                        Logger.LogInfo(string.Format("[Inventory] Spawned {0} into world.", item.GetName()));
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Inventory] SpawnItemInWorld failed: " + ex.Message);
            }
        });
    }

    public static void RechargeInventorySlot(int slot, float rechargeValue)
    {
        GetPlayer();

        if (Globals.playerObj == null)
        {
            if (Logger != null)
                Logger.LogError("[PEAK AIO] Player is null during inventory operation");
            return;
        }

        if (Globals.playerObj != null &&
            Globals.playerObj.itemSlots != null &&
            Globals.playerObj.itemSlots.Length > slot)
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                try
                {
                    var itemSlot = Globals.playerObj.itemSlots[slot];
                    if (itemSlot != null && itemSlot.data != null && itemSlot.data.data != null)
                    {
                        foreach (var kvp in itemSlot.data.data)
                        {
                            if (kvp.Key == DataEntryKey.PetterItemUses)
                            {
                                var intData = kvp.Value as IntItemData;
                                if (intData != null) intData.Value = (int)rechargeValue;
                            }
                            else if (kvp.Key == DataEntryKey.Fuel)
                            {
                                var floatData = kvp.Value as FloatItemData;
                                if (floatData != null) floatData.Value = rechargeValue;
                            }
                            else if (kvp.Key == DataEntryKey.UseRemainingPercentage)
                            {
                                var floatData = kvp.Value as FloatItemData;
                                if (floatData != null) floatData.Value = rechargeValue;
                            }
                            else if (kvp.Key == DataEntryKey.ItemUses)
                            {
                                var intData = kvp.Value as OptionableIntItemData;
                                if (intData != null) intData.Value = (int)rechargeValue;
                            }
                        }

                        // Sync updated data over network
                        var syncObj = new InventorySyncData(
                            Globals.playerObj.itemSlots,
                            Globals.playerObj.backpackSlot,
                            Globals.playerObj.tempFullSlot
                        );
                        byte[] syncData = SerializeSyncData(syncObj);
                        Globals.playerObj.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { syncData, true });
                    }
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError("[PEAK AIO] RechargeInventorySlot error: " + ex);
                }
            });
            if (Logger != null)
                Logger.LogInfo(string.Format("[Inventory] Recharged slot {0} to {1}", slot + 1, rechargeValue));
        }
    }

    public static void ClearAllAfflictions()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var character = Character.localCharacter;
                if (character == null || character.refs == null || character.refs.afflictions == null)
                    return;

                var afflictions = character.refs.afflictions;

                // 1. Clear all active over-time affliction objects (poison, bite, etc.)
                afflictions.ClearAllAfflictions();

                // 2. Remove all physical thorns
                afflictions.RemoveAllThorns();

                // 3. Clear all statuses directly
                Array values = Enum.GetValues(typeof(CharacterAfflictions.STATUSTYPE));
                for (int i = 0; i < values.Length; i++)
                {
                    var status = (CharacterAfflictions.STATUSTYPE)values.GetValue(i);
                    afflictions.SetStatus(status, 0f, false);
                }

                // 4. Sync statuses across network
                afflictions.PushStatuses(null);

                // 5. Reset stamina bar and UI
                character.ClampStamina();
                if (GUIManager.instance != null && GUIManager.instance.bar != null)
                {
                    GUIManager.instance.bar.ChangeBar();
                }

                if (Logger != null)
                    Logger.LogInfo("[PEAK AIO] Cleared all afflictions (injury, hunger, cold, poison, crab, curse, drowsy, hot, thorns, spores, web).");
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] ClearAllAfflictions error: " + ex);
            }
        });
    }

    public static void RefreshPlayerList()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Globals.allPlayers.Clear();
                Globals.playerNames.Clear();
                Globals.selectedPlayer = -1;

                var characters = Character.AllCharacters;
                if (characters == null || characters.Count == 0)
                    return;

                for (int i = 0; i < characters.Count; i++)
                {
                    try
                    {
                        var character = characters[i];
                        if (character == null) continue;
                        string name = character.characterName;
                        if (string.IsNullOrEmpty(name)) name = "Unknown";
                        Globals.allPlayers.Add(character);
                        Globals.playerNames.Add(name);
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (Globals.allPlayers.Count > 0 && Globals.selectedPlayer == -1)
                    Globals.selectedPlayer = 0;

                string namesStr = string.Join(", ", Globals.playerNames.ToArray());
                if (Logger != null)
                {
                    Logger.LogInfo(string.Format("[PlayerList] [{0}]", namesStr));
                    Logger.LogInfo(string.Format("[PlayerList] Found {0} players.", Globals.allPlayers.Count));
                }
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void GetItemsLogs(int slot)
    {
        if (Player.localPlayer == null || Player.localPlayer.itemSlots == null || slot >= Player.localPlayer.itemSlots.Length)
        {
            if (Logger != null)
                Logger.LogInfo(string.Format("[Slots_Items] Slot {0} invalid", slot + 1));
            return;
        }

        var itemSlot = Player.localPlayer.itemSlots[slot];
        if (itemSlot == null || itemSlot.prefab == null)
        {
            if (Logger != null)
                Logger.LogInfo(string.Format("[Slots_Items] Slot {0} empty", slot + 1));
            return;
        }

        var prefab = itemSlot.prefab;
        string name = prefab.GetName();
        string unityName = prefab.name;
        int instanceID = prefab.GetInstanceID();
        int hash = prefab.GetHashCode();
        string type = prefab.GetType().Name;

        if (Logger != null)
        {
            Logger.LogInfo("[Slots_Items]");
            Logger.LogInfo(string.Format("Slot: {0}", slot + 1));
            Logger.LogInfo(string.Format("Name: {0}", name));
            Logger.LogInfo(string.Format("PrefabName: {0}", unityName));
            Logger.LogInfo(string.Format("Type: {0}", type));
            Logger.LogInfo(string.Format("InstanceID: {0}", instanceID));
            Logger.LogInfo(string.Format("Hash: {0}", hash));

            foreach (var field in Player.localPlayer.GetType().GetFields())
            {
                Logger.LogInfo(string.Format("Field: {0}", field.Name));
            }
        }
    }

    public static void ReviveAllPlayers()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var characters = Character.AllCharacters;
                if (characters == null || characters.Count == 0)
                    return;

                for (int i = 0; i < characters.Count; i++)
                {
                    try
                    {
                        var character = characters[i];
                        if (character == null || character.photonView == null) continue;

                        Vector3 revivePos = character.Ghost != null
                            ? character.Ghost.transform.position
                            : character.Head;
                        character.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] {
                            revivePos + new Vector3(0f, 4f, 0f), false, -1
                        });
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                            Logger.LogError(string.Format("[Lobby] Revive failed for a character: {0}", ex.Message));
                    }
                }
                if (Logger != null)
                    Logger.LogInfo("[Lobby] Revive All triggered.");
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void KillAllPlayers()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var characters = Character.AllCharacters;
                if (characters == null || characters.Count == 0)
                    return;

                for (int i = 0; i < characters.Count; i++)
                {
                    try
                    {
                        var character = characters[i];
                        if (character == null || character.photonView == null) continue;
                        if (Globals.excludeSelfFromAllActions && character.IsLocal)
                            continue;

                        Vector3 pos = character.transform.position;
                        character.photonView.RPC("RPCA_Die", RpcTarget.All, new object[] { pos });
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                            Logger.LogError(string.Format("[Lobby] Kill failed for a character: {0}", ex.Message));
                    }
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Kill All triggered. ExcludeSelf: {0}", Globals.excludeSelfFromAllActions));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void WarpAllPlayersToMe()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (Character.localCharacter == null)
                    return;

                var characters = Character.AllCharacters;
                if (characters == null || characters.Count == 0)
                    return;

                Vector3 target = Character.localCharacter.Head + new Vector3(0f, 4f, 0f);

                for (int i = 0; i < characters.Count; i++)
                {
                    try
                    {
                        var character = characters[i];
                        if (character == null || character.photonView == null) continue;
                        if (Globals.excludeSelfFromAllActions && character.IsLocal)
                            continue;

                        character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                            target, true
                        });
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                            Logger.LogError(string.Format("[Lobby] Warp to me failed for a character: {0}", ex.Message));
                    }
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Warp All To Me triggered. ExcludeSelf: {0}", Globals.excludeSelfFromAllActions));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void ReviveSelectedPlayer()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                if (target == null || target.photonView == null) return;

                Vector3 revivePos = target.Ghost != null
                    ? target.Ghost.transform.position
                    : target.Head;

                target.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] {
                    revivePos + new Vector3(0f, 4f, 0f), false, -1
                });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Revive requested for player index {0}", Globals.selectedPlayer));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void KillSelectedPlayer()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                if (target == null || target.photonView == null) return;

                Vector3 pos = target.transform.position;
                target.photonView.RPC("RPCA_Die", RpcTarget.All, new object[] { pos });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Kill requested for player index {0}", Globals.selectedPlayer));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void WarpToSelectedPlayer()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                if (target == null) return;

                Vector3 targetHead = target.Head + new Vector3(0f, 4f, 0f);
                Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                    targetHead, true
                });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Warp to player requested for index {0}", Globals.selectedPlayer));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void WarpSelectedPlayerToMe()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                if (target == null) return;

                Vector3 myHead = Character.localCharacter.Head + new Vector3(0f, 4f, 0f);
                target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                    myHead, true
                });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Warp to me requested for player index {0}", Globals.selectedPlayer));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void TeleportToCoords(float x, float y, float z)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Character localCharacter = Character.localCharacter;
                if (localCharacter == null || localCharacter.data.dead)
                {
                    if (Logger != null)
                        Logger.LogWarning("[Teleport] Local character is null or dead. Aborting teleport.");
                    return;
                }

                PhotonView photonView = localCharacter.photonView;
                if (photonView == null)
                    return;

                Vector3 target = new Vector3(x, y, z);
                photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
                {
                    target, true
                });

                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogInfo(string.Format("[Teleport] Teleported to {0}", target));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError("[Teleport] Exception: " + ex);
            }
        });
    }

    public static void JumpToSegment(Segment segment)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (Logger != null)
                    Logger.LogInfo(string.Format("[PEAK AIO] Jumping to segment: {0}", segment));
                MapHandler.JumpToSegment(segment);
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] JumpToSegment error: " + ex.Message);
            }
        });
    }

    public static bool hasInitializedLuggageList = false;

    public static void EnsureLuggageListInitialized()
    {
        if (!hasInitializedLuggageList && Character.localCharacter != null)
        {
            hasInitializedLuggageList = true;
            RefreshLuggageList();
        }
    }

    private struct LuggageEntry
    {
        public Luggage lug;
        public float distance;
    }

    public static void RefreshLuggageList()
    {
        try
        {
            Globals.luggageLabels.Clear();
            Globals.luggageObject.Clear();
            Globals.selectedLuggageIndex = -1;

            var localChar = Character.localCharacter;
            if (localChar == null)
                return;

            var luggageList = Luggage.ALL_LUGGAGE;
            if (luggageList == null || luggageList.Count == 0)
                return;

            var allLuggage = new List<LuggageEntry>();
            Vector3 headPos = localChar.Head;

            for (int i = 0; i < luggageList.Count; i++)
            {
                try
                {
                    var lug = luggageList[i];
                    if (lug == null) continue;

                    float distance = Vector3.Distance(headPos, lug.Center());
                    if (distance <= 300f)
                    {
                        LuggageEntry entry;
                        entry.lug = lug;
                        entry.distance = distance;
                        allLuggage.Add(entry);
                    }
                }
                catch
                {
                    continue;
                }
            }

            allLuggage.Sort((a, b) => a.distance.CompareTo(b.distance));

            for (int i = 0; i < allLuggage.Count; i++)
            {
                try
                {
                    var entry = allLuggage[i];
                    string name = entry.lug.displayName;
                    if (string.IsNullOrEmpty(name)) name = "Unnamed";
                    string typeTag = (entry.lug is LuggageCursed) ? "[Cursed] " : "";
                    Globals.luggageLabels.Add(string.Format("{0}{1} [{2:F1}m]", typeTag, name, entry.distance));
                    Globals.luggageObject.Add(entry.lug);
                }
                catch
                {
                    continue;
                }
            }

            if (Globals.luggageLabels.Count > 0)
                Globals.selectedLuggageIndex = 0;

            if (Logger != null)
                Logger.LogInfo(string.Format("[Luggage] Refreshed. Found {0} nearby.", Globals.luggageLabels.Count));
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogError(string.Format("[Luggage] RefreshLuggageList error: {0}", ex.Message));
        }
    }

    public static void OpenAllNearbyLuggage()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            int opened = 0;

            for (int i = 0; i < Globals.luggageObject.Count; i++)
            {
                var luggage = Globals.luggageObject[i];
                if (luggage == null) continue;

                var view = luggage.GetComponent<PhotonView>();
                if (view != null)
                {
                    view.RPC("OpenLuggageRPC", RpcTarget.All, new object[] { true });
                    opened++;
                }
            }

            if (Logger != null)
                Logger.LogInfo(string.Format("[Luggage] Requested open for {0} nearby containers.", opened));
        });
    }

    public static void OpenLuggage(int index)
    {
        if (index < 0 || index >= Globals.luggageObject.Count)
            return;

        var luggage = Globals.luggageObject[index];
        if (luggage == null)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                PhotonView view = luggage.GetComponent<PhotonView>();
                if (view != null)
                {
                    view.RPC("OpenLuggageRPC", RpcTarget.All, new object[] { true });
                    if (Logger != null)
                        Logger.LogInfo(string.Format("[Luggage] Sent OpenLuggageRPC for: {0}", luggage.displayName));
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Luggage] Open failed: " + ex);
            }
        });
    }

    public static void SpawnScoutmasterForPlayer(int playerIndex)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (Logger != null)
                    Logger.LogWarning("[Scoutmaster] Only the MasterClient can spawn the Scoutmaster.");
                return;
            }

            if (playerIndex < 0 || playerIndex >= Character.AllCharacters.Count)
            {
                if (Logger != null)
                    Logger.LogWarning("[Scoutmaster] Invalid player index.");
                return;
            }

            Character targetCharacter = Character.AllCharacters[playerIndex];
            Vector3 targetPos = targetCharacter.transform.position;
            Vector3 spawnOrigin = targetPos + new Vector3(UnityEngine.Random.Range(-10f, 10f), 25f, UnityEngine.Random.Range(-10f, 10f));
            Vector3 down = Vector3.down;

            RaycastHit hit;
            if (Physics.Raycast(spawnOrigin, down, out hit, 100f, ~0))
            {
                Vector3 spawnPoint = hit.point + Vector3.up * 1f;
                Quaternion rotation = Quaternion.identity;

                GameObject scoutObj = PhotonNetwork.InstantiateRoomObject("Character_Scoutmaster", spawnPoint, rotation, 0, null);
                var character = scoutObj.GetComponent<Character>();
                if (character != null)
                    character.data.spawnPoint = character.transform;

                var scoutmaster = scoutObj.GetComponent<Scoutmaster>();
                if (scoutmaster != null)
                {
                    try
                    {
                        var method = typeof(Scoutmaster).GetMethod("SetCurrentTarget", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (method != null)
                        {
                            method.Invoke(scoutmaster, new object[] { targetCharacter, 15f });
                            if (Logger != null)
                                Logger.LogInfo(string.Format("[Scoutmaster] Target set to {0}", targetCharacter.characterName));
                        }
                        else
                        {
                            if (Logger != null)
                                Logger.LogWarning("[Scoutmaster] Reflection failed — method not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                            Logger.LogError("[Scoutmaster] Reflection error: " + ex);
                    }
                }
            }
            else
            {
                if (Logger != null)
                    Logger.LogWarning("[Scoutmaster] No valid ground to spawn.");
            }
        });
    }

    public static bool PlayerHasBackpack(Player player)
    {
        if (player == null || player.itemSlots == null)
            return false;

        if (player.itemSlots.Length <= 3)
            return false;

        var backpackSlot = player.itemSlots[3] as BackpackSlot;
        return backpackSlot != null && backpackSlot.hasBackpack;
    }

    public static void GivePlayerBackpack(Player player)
    {
        if (player == null)
        {
            if (Logger != null)
                Logger.LogError("[SpawnBackpack] Player is null.");
            return;
        }

        ItemSlot slot = player.GetItemSlot(3);
        var backpackSlot = slot as BackpackSlot;
        if (backpackSlot != null)
        {
            if (backpackSlot.hasBackpack)
            {
                if (Logger != null)
                    Logger.LogInfo("[SpawnBackpack] Player already has backpack.");
                return;
            }

            var data = new ItemInstanceData(Guid.NewGuid());
            ItemInstanceDataHandler.AddInstanceData(data);

            backpackSlot.hasBackpack = true;
            backpackSlot.data = data;

            try
            {
                var syncObj = new InventorySyncData(
                    player.itemSlots,
                    backpackSlot,
                    player.tempFullSlot
                );
                byte[] syncData = SerializeSyncData(syncObj);
                if (player.photonView != null)
                {
                    player.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { syncData, true });
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogWarning("[SpawnBackpack] Could not sync backpack via RPC: " + ex.Message);
            }

            if (Logger != null)
                Logger.LogInfo("[SpawnBackpack] Backpack granted to player.");
        }
        else
        {
            if (Logger != null)
                Logger.LogError("[SpawnBackpack] Slot 3 is not BackpackSlot.");
        }
    }
}
