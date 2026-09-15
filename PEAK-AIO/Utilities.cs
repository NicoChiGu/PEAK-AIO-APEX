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

    public static bool hasAttemptedItemLoad = false;
    private static bool isUpdatingItems = false;
    public static bool pendingItemRefresh = false;
    private static float lastItemLoadAttemptTime = -999f;

    public static void UpdateItems(bool force = false)
    {
        if (isUpdatingItems) return;
        if (!force && hasAttemptedItemLoad && Globals.items != null && Globals.items.Count > 0) return;
        if (!force && Time.realtimeSinceStartup - lastItemLoadAttemptTime < 2.0f) return;

        if (force)
        {
            pendingItemRefresh = true;
            hasAttemptedItemLoad = false;
        }

        // If inside OnGUI, only perform synchronous update during EventType.Layout
        if (Event.current != null)
        {
            if (Event.current.type == EventType.Layout)
            {
                pendingItemRefresh = false;
                UpdateItemsSync();
            }
            else
            {
                pendingItemRefresh = true;
            }
            return;
        }

        // Outside OnGUI: if on main thread, run sync; else dispatch
        if (System.Threading.Thread.CurrentThread.ManagedThreadId == 1)
        {
            UpdateItemsSync();
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(UpdateItemsSync);
        }
    }

    public static void UpdateItemsSync()
    {
        if (isUpdatingItems) return;
        isUpdatingItems = true;
        lastItemLoadAttemptTime = Time.realtimeSinceStartup;

        try
        {
            var itemSet = new HashSet<string>();
            var collectedItems = new List<Item>();

            Action<Item, bool> tryAddItem = (item, requireAssetOnly) =>
            {
                if (item == null || item.gameObject == null) return;
                if (requireAssetOnly)
                {
                    bool isAsset = false;
                    try
                    {
                        isAsset = !item.gameObject.scene.IsValid() || string.IsNullOrEmpty(item.gameObject.scene.name);
                    }
                    catch
                    {
                        isAsset = true;
                    }
                    if (!isAsset) return;
                }

                string key = null;
                try { key = item.GetName(); } catch { }
                if (string.IsNullOrEmpty(key)) key = item.name;
                if (!string.IsNullOrEmpty(key) && !itemSet.Contains(key))
                {
                    itemSet.Add(key);
                    collectedItems.Add(item);
                }
            };

            // 1. Try to load from ItemDatabase singleton asset
            try
            {
                var db = SingletonAsset<ItemDatabase>.Instance;
                if (db != null)
                {
                    if (db.Objects == null || db.Objects.Count == 0)
                    {
                        try { db.LoadItems(); } catch { }
                    }

                    if (db.Objects != null && db.Objects.Count > 0)
                    {
                        for (int i = 0; i < db.Objects.Count; i++)
                        {
                            try
                            {
                                tryAddItem(db.Objects[i], false);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogWarning("[PEAK AIO] Could not query ItemDatabase: " + ex.Message);
            }

            // 2. Fallback to Item.ALL_ITEMS
            try
            {
                if (Item.ALL_ITEMS != null && Item.ALL_ITEMS.Count > 0)
                {
                    for (int i = 0; i < Item.ALL_ITEMS.Count; i++)
                    {
                        try
                        {
                            tryAddItem(Item.ALL_ITEMS[i], false);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogWarning("[PEAK AIO] Could not query Item.ALL_ITEMS: " + ex.Message);
            }

            // 3. Fallback to Item.ALL_ACTIVE_ITEMS
            try
            {
                if (Item.ALL_ACTIVE_ITEMS != null && Item.ALL_ACTIVE_ITEMS.Count > 0)
                {
                    for (int i = 0; i < Item.ALL_ACTIVE_ITEMS.Count; i++)
                    {
                        try
                        {
                            tryAddItem(Item.ALL_ACTIVE_ITEMS[i], false);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogWarning("[PEAK AIO] Could not query Item.ALL_ACTIVE_ITEMS: " + ex.Message);
            }

            // 4. Fallback to Resources.FindObjectsOfTypeAll only if earlier sources gave 0 items
            if (collectedItems.Count == 0)
            {
                try
                {
                    UnityEngine.Object[] allItems = Resources.FindObjectsOfTypeAll(typeof(Item));
                    if (allItems != null)
                    {
                        for (int i = 0; i < allItems.Length; i++)
                        {
                            try
                            {
                                tryAddItem(allItems[i] as Item, true);
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogWarning("[PEAK AIO] Could not query Resources for Items: " + ex.Message);
                }
            }

            // Extract display names and sort alphabetically
            var itemEntries = new List<KeyValuePair<Item, string>>(collectedItems.Count);
            for (int i = 0; i < collectedItems.Count; i++)
            {
                var item = collectedItems[i];
                if (item == null) continue;
                string displayName = null;
                try { displayName = item.GetName(); } catch { }
                if (string.IsNullOrEmpty(displayName)) displayName = item.name;
                if (string.IsNullOrEmpty(displayName)) displayName = "Unknown Item";
                itemEntries.Add(new KeyValuePair<Item, string>(item, displayName));
            }

            try
            {
                itemEntries.Sort((a, b) => string.Compare(a.Value, b.Value, StringComparison.OrdinalIgnoreCase));
            }
            catch { }

            var newItems = new List<Item>(itemEntries.Count);
            var newItemNames = new List<string>(itemEntries.Count);
            for (int i = 0; i < itemEntries.Count; i++)
            {
                newItems.Add(itemEntries[i].Key);
                newItemNames.Add(itemEntries[i].Value);
            }

            // Atomic assignment
            Globals.items = newItems;
            Globals.itemNames = newItemNames;

            // Mark load attempt
            hasAttemptedItemLoad = true;

            // Clamp selected items if out of bounds
            if (Globals.selectedItems != null)
            {
                for (int i = 0; i < Globals.selectedItems.Length; i++)
                {
                    if (Globals.selectedItems[i] >= newItems.Count)
                        Globals.selectedItems[i] = -1;
                }
            }

            if (Logger != null)
                Logger.LogInfo(string.Format("[PEAK AIO] Loaded {0} unique items.", Globals.items.Count));
        }
        catch (Exception ex)
        {
            if (Logger != null)
                Logger.LogError("[PEAK AIO] UpdateItems error: " + ex);
        }
        finally
        {
            isUpdatingItems = false;
        }
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

        if (slot == 3)
        {
            AssignBackpackItem(itemIndex);
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
                    if (slotData != null)
                    {
                        var itemData = new ItemInstanceData(Guid.NewGuid());
                        ItemInstanceDataHandler.AddInstanceData(itemData);
                        slotData.SetItem(Globals.items[itemIndex], itemData);
                    }

                    var syncObj = new InventorySyncData(
                        Globals.playerObj.itemSlots,
                        Globals.playerObj.backpackSlot,
                        Globals.playerObj.tempFullSlot
                    );
                    byte[] syncData = SerializeSyncData(syncObj);

                    if (Globals.playerObj.photonView != null)
                    {
                        Globals.playerObj.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { syncData, true });
                    }
                    if (Logger != null)
                        Logger.LogInfo(string.Format("[Inventory] Assigned {0} to slot {1}", Globals.itemNames[itemIndex], slot + 1));
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError("[PEAK AIO] AssignInventoryItem error: " + ex);
                    Globals.GlobalNotifier.ShowError("分配物品失败: " + ex.Message);
                }
            });
        }
    }

    public static Vector3 CalculateGroundSpawnPosition(Character character, float forwardDist = 2.0f)
    {
        if (character == null) return Vector3.zero;

        Vector3 lookDir = Vector3.zero;
        if (character.data != null && character.data.lookDirection_Flat.sqrMagnitude > 0.001f)
        {
            lookDir = character.data.lookDirection_Flat.normalized;
        }
        else
        {
            lookDir = character.transform.forward;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
                lookDir.Normalize();
            else
                lookDir = Vector3.forward;
        }

        Vector3 center = character.transform.position + Vector3.up * 0.5f;
        Vector3 targetHorizontal = center + lookDir * forwardDist;
        Vector3 rayStart = targetHorizontal + Vector3.up * 2.0f;

        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 6.0f, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.point + Vector3.up * 0.15f;
        }

        return targetHorizontal + Vector3.up * 0.15f;
    }

    public static Vector3 GetSafeRevivePosition(Character target)
    {
        if (target == null) return Vector3.zero;

        bool isDead = (target.data != null && target.data.dead) || target.Ghost != null;
        bool isDowned = (target.data != null && target.data.passedOut);

        // 1. 如果没有死亡且没有倒地（存活状态）或者处于倒地状态，必须【原地复活】，绝不能拉回历史死亡地点
        if (!isDead)
        {
            Vector3 currentPos = target.transform.position;
            if (isDowned && target.Head != Vector3.zero)
            {
                currentPos = target.Head;
            }

            // 在当前站立/倒地位置正上方微调打射线，紧贴当前地面
            Vector3 rayStart = currentPos + Vector3.up * 1.5f;
            RaycastHit hit;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 4.0f, ~0, QueryTriggerInteraction.Ignore))
            {
                return hit.point + Vector3.up * 0.15f;
            }
            return currentPos;
        }

        // 2. 只有在玩家彻底死亡（Dead）且掉入虚空或变为幽灵的情况下，才使用安全快照或幽灵位置
        Vector3 basePos = Vector3.zero;
        bool foundSnapshot = false;

        Globals.PlayerLocationSnapshot snapshot;
        if (target.photonView != null && Globals.playerSafeLocations.TryGetValue(target.photonView.ViewID, out snapshot))
        {
            basePos = snapshot.safePosition;
            foundSnapshot = true;
        }
        else
        {
            Globals.PlayerLocationSnapshot snapInst;
            if (Globals.playerSafeLocations.TryGetValue(target.GetInstanceID(), out snapInst))
            {
                basePos = snapInst.safePosition;
                foundSnapshot = true;
            }
        }

        if (!foundSnapshot)
        {
            basePos = target.Ghost != null ? target.Ghost.transform.position : target.Head;
            if (basePos == Vector3.zero)
                basePos = target.transform.position;
        }

        Vector3 deadRayStart = basePos + Vector3.up * 5.0f;
        RaycastHit deadHit;
        if (Physics.Raycast(deadRayStart, Vector3.down, out deadHit, 15.0f, ~0, QueryTriggerInteraction.Ignore))
        {
            return deadHit.point + Vector3.up * 1.5f;
        }

        return basePos + Vector3.up * 1.5f;
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
                        Vector3 spawnPos = CalculateGroundSpawnPosition(Character.localCharacter, 2.0f);
                        ItemDatabase.Add(item, spawnPos);
                    }
                    else
                    {
                        ItemDatabase.Add(item);
                    }

                    string itemName = null;
                    try { itemName = item.GetName(); } catch { }
                    if (string.IsNullOrEmpty(itemName)) itemName = item.name;

                    if (Logger != null)
                        Logger.LogInfo(string.Format("[Inventory] Spawned {0} into world on ground.", itemName));
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Inventory] SpawnItemInWorld failed: " + ex.Message);
                Globals.GlobalNotifier.ShowError("生成物品失败: " + ex.Message);
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
                        if (Globals.playerObj.photonView != null)
                        {
                            Globals.playerObj.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { syncData, true });
                        }
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

    public static void ReviveAllPlayers(bool restoreItems = false)
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

                        bool isDead = (character.data != null && character.data.dead) || character.Ghost != null;
                        Vector3 revivePos = GetSafeRevivePosition(character);

                        character.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] {
                            revivePos, false, -1
                        });

                        int viewId = character.photonView.ViewID;
                        Globals.playerSafeLocations[viewId] = new Globals.PlayerLocationSnapshot
                        {
                            safePosition = revivePos,
                            lastRecordedTime = Time.time
                        };

                        if (restoreItems && isDead)
                        {
                            var targetChar = character;
                            EventComponent.QueueDelayedAction(() =>
                            {
                                RestoreInventorySnapshot(targetChar);
                            }, 0.3f);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                            Logger.LogError(string.Format("[Lobby] Revive failed for a character: {0}", ex.Message));
                    }
                }
                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Revive All triggered with safe snapshots. RestoreItems: {0}", restoreItems));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
                Globals.GlobalNotifier.ShowError("全员复活异常: " + ex.Message);
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

                Vector3 myPos = Character.localCharacter.transform.position;
                Vector3 rayStart = myPos + Vector3.up * 5f;
                Vector3 safeTarget;
                RaycastHit hit;
                if (Physics.Raycast(rayStart, Vector3.down, out hit, 10f, ~0, QueryTriggerInteraction.Ignore))
                    safeTarget = hit.point + Vector3.up * 1.5f;
                else
                    safeTarget = myPos + Vector3.up * 3f;

                for (int i = 0; i < characters.Count; i++)
                {
                    try
                    {
                        var character = characters[i];
                        if (character == null || character.photonView == null) continue;
                        if (Globals.excludeSelfFromAllActions && character.IsLocal)
                            continue;

                        character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                            safeTarget, true
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
                Globals.GlobalNotifier.ShowError("全员传送到我异常: " + ex.Message);
            }
        });
    }

    public static void ReviveSelectedPlayer(bool restoreItems = false)
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                if (target == null || target.photonView == null) return;

                bool isDead = (target.data != null && target.data.dead) || target.Ghost != null;
                Vector3 revivePos = GetSafeRevivePosition(target);

                target.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] {
                    revivePos, false, -1
                });

                int viewId = target.photonView.ViewID;
                Globals.playerSafeLocations[viewId] = new Globals.PlayerLocationSnapshot
                {
                    safePosition = revivePos,
                    lastRecordedTime = Time.time
                };

                if (restoreItems && isDead)
                {
                    EventComponent.QueueDelayedAction(() =>
                    {
                        RestoreInventorySnapshot(target);
                    }, 0.3f);
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Revive requested for player index {0}. RestoreItems: {1}", Globals.selectedPlayer, restoreItems));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
                Globals.GlobalNotifier.ShowError("复活玩家异常: " + ex.Message);
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

                Vector3 targetPos = target.transform.position;
                Vector3 rayStart = targetPos + Vector3.up * 5f;
                Vector3 safePos;
                RaycastHit hit;
                if (Physics.Raycast(rayStart, Vector3.down, out hit, 10f, ~0, QueryTriggerInteraction.Ignore))
                    safePos = hit.point + Vector3.up * 1.5f;
                else
                    safePos = targetPos + Vector3.up * 3f;

                Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                    safePos, true
                });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Warp to player requested for index {0}", Globals.selectedPlayer));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
                Globals.GlobalNotifier.ShowError("传送到玩家异常: " + ex.Message);
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

                Vector3 myPos = Character.localCharacter.transform.position;
                Vector3 rayStart = myPos + Vector3.up * 5f;
                Vector3 safePos;
                RaycastHit hit;
                if (Physics.Raycast(rayStart, Vector3.down, out hit, 10f, ~0, QueryTriggerInteraction.Ignore))
                    safePos = hit.point + Vector3.up * 1.5f;
                else
                    safePos = myPos + Vector3.up * 3f;

                target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                    safePos, true
                });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Warp to me requested for player index {0}", Globals.selectedPlayer));
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
                Globals.GlobalNotifier.ShowError("传送玩家到我异常: " + ex.Message);
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
                WorldDataCache.Invalidate();
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] JumpToSegment error: " + ex.Message);
            }
        });
    }

    public static void JumpToSegmentStartSafe(Segment segment)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (!MapHandler.Exists || MapHandler.Instance == null)
                {
                    Globals.GlobalNotifier.ShowError("无法跳转：未在世界地图场景中！");
                    return;
                }

                var mh = MapHandler.Instance;
                int segIdx = (int)segment;
                Vector3 spawnPos = Vector3.zero;
                bool foundPos = false;

                if (segIdx >= 5) // Peak
                {
                    if (mh.respawnThePeak != null)
                    {
                        spawnPos = mh.respawnThePeak.position;
                        foundPos = true;
                    }
                }
                else if (segIdx == 4) // The Kiln
                {
                    if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null)
                    {
                        var kSeg = mh.segments[4];
                        if (kSeg.reconnectSpawnPos != null)
                        {
                            spawnPos = kSeg.reconnectSpawnPos.position;
                            foundPos = true;
                        }
                        else if (kSeg.segmentParent != null)
                        {
                            spawnPos = kSeg.segmentParent.transform.position;
                            foundPos = true;
                        }
                    }
                }
                else if (mh.segments != null && segIdx >= 0 && segIdx < mh.segments.Length)
                {
                    var seg = mh.segments[segIdx];
                    if (seg != null)
                    {
                        if (seg.segmentParent != null && !seg.segmentParent.activeSelf)
                            seg.segmentParent.SetActive(true);
                        if (seg.segmentCampfire != null && !seg.segmentCampfire.activeSelf)
                            seg.segmentCampfire.SetActive(true);
                        if (seg.wallNext != null && !seg.wallNext.activeSelf)
                            seg.wallNext.SetActive(true);
                        if (seg.wallPrevious != null && !seg.wallPrevious.activeSelf)
                            seg.wallPrevious.SetActive(true);

                        if (seg.reconnectSpawnPos != null)
                        {
                            spawnPos = seg.reconnectSpawnPos.position;
                            foundPos = true;
                        }
                        else if (seg.segmentParent != null)
                        {
                            spawnPos = seg.segmentParent.transform.position;
                            foundPos = true;
                        }
                    }
                }

                if (!foundPos)
                {
                    Globals.GlobalNotifier.ShowError(string.Format("未找到区域 {0} 的起点出生点！", segment));
                    return;
                }

                Vector3 rayStart = spawnPos + Vector3.up * 5.0f;
                Vector3 finalPos;
                RaycastHit hit;
                if (Physics.Raycast(rayStart, Vector3.down, out hit, 15.0f, ~0, QueryTriggerInteraction.Ignore))
                {
                    finalPos = hit.point + Vector3.up * 1.5f;
                }
                else
                {
                    finalPos = spawnPos + Vector3.up * 2.0f;
                }

                // If host, sync official segment transition
                if (Photon.Pun.PhotonNetwork.IsMasterClient && (int)MapHandler.CurrentSegmentNumber != segIdx)
                {
                    try
                    {
                        MapHandler.JumpToSegment(segment);
                    }
                    catch { }
                }

                if (Character.localCharacter != null)
                {
                    if (Character.localCharacter.photonView != null)
                    {
                        Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { finalPos, true });
                    }
                    else
                    {
                        Character.localCharacter.WarpPlayerRPC(finalPos, true);
                    }
                }

                WorldDataCache.Invalidate();
                if (Logger != null)
                    Logger.LogInfo(string.Format("[PEAK AIO] Safely jumped to segment start: {0} at {1}", segment, finalPos));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] JumpToSegmentStartSafe error: " + ex.Message);
                Globals.GlobalNotifier.ShowError("跳转到起点失败: " + ex.Message);
            }
        });
    }

    public struct RouteSegmentInfo
    {
        public int level;
        public Segment segment;
        public Biome.BiomeType biomeType;
        public string displayName;
        public bool isCurrent;
        public bool hasCampfire;
        public float altitude;
        public bool isAtCampfire;
        public bool isCampfireLit;
    }

    public static string GetBiomeDisplayName(Biome.BiomeType bt, Segment seg)
    {
        if (seg == Segment.TheKiln)
            return Localization.T("world.segment_thekiln");
        if (seg == Segment.Peak)
            return Localization.T("world.segment_peak");
        if ((int)seg == 6)
            return Localization.T("world.segment_void");

        switch (bt)
        {
            case Biome.BiomeType.Shore:
                return Localization.T("world.segment_beach");
            case Biome.BiomeType.Tropics:
                return Localization.T("world.segment_tropics");
            case Biome.BiomeType.Roots:
                return Localization.T("world.segment_roots");
            case Biome.BiomeType.Alpine:
                return Localization.T("world.segment_alpine");
            case Biome.BiomeType.Volcano:
                return Localization.T("world.segment_caldera");
            case Biome.BiomeType.Mesa:
                return Localization.T("world.segment_mesa");
            case Biome.BiomeType.Peak:
                return Localization.T("world.segment_peak");
            default:
                if ((int)bt == 8)
                    return Localization.T("world.segment_swamp");
                if ((int)bt == 16)
                    return Localization.T("world.segment_void");

                switch (seg)
                {
                    case Segment.Beach: return Localization.T("world.segment_beach");
                    case Segment.Tropics: return Localization.T("world.segment_tropics");
                    case Segment.Alpine: return Localization.T("world.segment_alpine");
                    case Segment.Caldera: return Localization.T("world.segment_caldera");
                    case Segment.TheKiln: return Localization.T("world.segment_thekiln");
                    case Segment.Peak: return Localization.T("world.segment_peak");
                    default:
                        if ((int)seg == 6) return Localization.T("world.segment_void");
                        return seg.ToString();
                }
        }
    }

    public static string DecodeBiomeCharToName(char c)
    {
        switch (char.ToUpper(c))
        {
            case 'S': return Localization.T("world.segment_beach");
            case 'T': return Localization.T("world.segment_tropics");
            case 'R': return Localization.T("world.segment_roots");
            case 'A': return Localization.T("world.segment_alpine");
            case 'M': return Localization.T("world.segment_mesa");
            case 'V': return Localization.T("world.segment_caldera");
            case 'K': return Localization.T("world.segment_thekiln");
            case 'P': return Localization.T("world.segment_peak");
            default: return c.ToString();
        }
    }

    public static string FormatBiomeIDRoute(string biomeId)
    {
        if (string.IsNullOrEmpty(biomeId)) return "";
        List<string> parts = new List<string>();
        for (int i = 0; i < biomeId.Length; i++)
        {
            char c = biomeId[i];
            if (i == 3 && char.ToUpper(c) == 'S')
            {
                parts.Add(Localization.T("world.segment_swamp"));
            }
            else
            {
                parts.Add(DecodeBiomeCharToName(c));
            }
        }
        if (parts.Count == 4)
        {
            parts.Add(Localization.T("world.segment_thekiln"));
            parts.Add(Localization.T("world.segment_peak"));
        }
        return string.Join(" ➔ ", parts.ToArray());
    }

    private static readonly Campfire[] s_CachedCampfires = new Campfire[6];

    public static void ClearCampfireCache()
    {
        for (int i = 0; i < s_CachedCampfires.Length; i++)
        {
            s_CachedCampfires[i] = null;
        }
    }

    public static Campfire GetSegmentCampfire(int segmentIndex)
    {
        if (segmentIndex < 0 || segmentIndex >= 5)
            return null;

        if (s_CachedCampfires[segmentIndex] != null && s_CachedCampfires[segmentIndex].gameObject != null)
        {
            return s_CachedCampfires[segmentIndex];
        }

        if (!MapHandler.Exists || MapHandler.Instance == null)
            return null;

        var mh = MapHandler.Instance;
        Campfire targetCampfire = null;

        if (mh.segments != null && segmentIndex < mh.segments.Length)
        {
            var seg = mh.segments[segmentIndex];
            if (seg != null && seg.segmentCampfire != null)
            {
                targetCampfire = seg.segmentCampfire.GetComponentInChildren<Campfire>(true);
            }
        }

        if (targetCampfire == null && (int)MapHandler.CurrentSegmentNumber == segmentIndex)
        {
            targetCampfire = MapHandler.CurrentCampfire;
        }

        if (targetCampfire != null)
        {
            s_CachedCampfires[segmentIndex] = targetCampfire;
        }

        return targetCampfire;
    }

    public static bool IsPlayerNearCampfire(int segmentIndex, float maxDist = 12f)
    {
        Character local = Character.localCharacter;
        if (local == null) return false;

        Campfire cf = GetSegmentCampfire(segmentIndex);
        if (cf == null) return false;

        return Vector3.Distance(local.transform.position, cf.transform.position) <= maxDist;
    }

    public static bool IsCampfireLit(int segmentIndex)
    {
        try
        {
            Campfire cf = GetSegmentCampfire(segmentIndex);
            if (cf != null)
            {
                return cf.Lit || cf.state == Campfire.FireState.Spent;
            }
        }
        catch { }
        return false;
    }

    public static bool TryGetCustomMapOrPlaylist(out int customMapIndex, out string sceneName, out string biomeId, out string playlistInfo, out string sourceName)
    {
        customMapIndex = -1;
        sceneName = "";
        biomeId = "";
        playlistInfo = "";
        sourceName = "";

        try
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < asms.Length; i++)
            {
                string asmName = asms[i].GetName().Name;
                if (asmName == "PeakAMap" ||
                    asmName.IndexOf("CustomMap", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    asmName.IndexOf("Playlist", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Type[] types;
                    try { types = asms[i].GetTypes(); } catch { continue; }

                    for (int t = 0; t < types.Length; t++)
                    {
                        Type type = types[t];
                        if (type.Name.Equals("CustomMaps", StringComparison.OrdinalIgnoreCase) ||
                            type.Name.IndexOf("Playlist", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            type.Name.IndexOf("MapSelector", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            PropertyInfo instProp = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                            FieldInfo instField = instProp == null ? type.GetField("Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy) : null;
                            object instance = instProp != null ? instProp.GetValue(null, null) : (instField != null ? instField.GetValue(null) : null);

                            if (instance != null)
                            {
                                FieldInfo modeField = type.GetField("loadMode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                PropertyInfo modeProp = modeField == null ? type.GetProperty("loadMode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) : null;
                                object modeVal = modeField != null ? modeField.GetValue(instance) : (modeProp != null ? modeProp.GetValue(instance, null) : null);

                                bool isActive = false;
                                if (modeVal != null)
                                {
                                    string modeStr = modeVal.ToString();
                                    if (!modeStr.Equals("Vanilla", StringComparison.OrdinalIgnoreCase) &&
                                        !modeStr.Equals("Default", StringComparison.OrdinalIgnoreCase) &&
                                        !modeStr.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                                        !modeStr.Equals("Disabled", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isActive = true;
                                        sourceName = asmName + " (" + modeStr + ")";
                                    }
                                }

                                FieldInfo enabledField = type.GetField("enabled", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                if (enabledField != null && enabledField.FieldType == typeof(bool))
                                {
                                    if ((bool)enabledField.GetValue(instance))
                                    {
                                        isActive = true;
                                        if (string.IsNullOrEmpty(sourceName)) sourceName = asmName;
                                    }
                                }

                                if (isActive)
                                {
                                    PropertyInfo sceneProp = type.GetProperty("CustomSceneName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("SceneName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("CurrentScene", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                    if (sceneProp != null)
                                    {
                                        object val = sceneProp.GetValue(instance, null);
                                        if (val != null) sceneName = val.ToString();
                                    }

                                    PropertyInfo indexProp = type.GetProperty("CustomMapIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("MapIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("CurrentIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                    if (indexProp != null)
                                    {
                                        object val = indexProp.GetValue(instance, null);
                                        if (val is int) customMapIndex = (int)val;
                                    }

                                    PropertyInfo biomeProp = type.GetProperty("BiomeID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("CustomBiomeID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                    if (biomeProp != null)
                                    {
                                        object val = biomeProp.GetValue(instance, null);
                                        if (val != null) biomeId = val.ToString();
                                    }

                                    PropertyInfo playlistProp = type.GetProperty("Playlist", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("MapList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                    if (playlistProp != null)
                                    {
                                        object pVal = playlistProp.GetValue(instance, null);
                                        System.Collections.ICollection coll = pVal as System.Collections.ICollection;
                                        if (coll != null)
                                        {
                                            playlistInfo = string.Format("{0} maps", coll.Count);
                                        }
                                    }

                                    var baker = SingletonAsset<MapBaker>.Instance;
                                    if (baker != null && customMapIndex >= 0 && string.IsNullOrEmpty(sceneName))
                                    {
                                        sceneName = baker.GetLevel(customMapIndex);
                                        if (string.IsNullOrEmpty(biomeId))
                                            biomeId = baker.GetBiomeID(customMapIndex);
                                    }

                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch { }

        return false;
    }

    public static bool TryGetPeakAMapCustomMap(out int customMapIndex, out string sceneName, out string biomeId)
    {
        string pInfo, srcName;
        return TryGetCustomMapOrPlaylist(out customMapIndex, out sceneName, out biomeId, out pInfo, out srcName);
    }

    public static Segment DetectCurrentPlayerSegment()
    {
        if (!MapHandler.Exists || MapHandler.Instance == null)
            return Segment.Beach;

        Character local = Character.localCharacter;
        if (local == null)
            return MapHandler.CurrentSegmentNumber;

        Vector3 pPos = local.transform.position;
        var mh = MapHandler.Instance;
        Segment officialSeg = MapHandler.CurrentSegmentNumber;

        try
        {
            // 1. Check if at The Peak
            if (officialSeg == Segment.Peak)
            {
                return Segment.Peak;
            }
            if (mh.respawnThePeak != null && mh.respawnThePeak.position.y > 50f)
            {
                if (pPos.y >= mh.respawnThePeak.position.y - 25f || Vector3.Distance(pPos, mh.respawnThePeak.position) < 80f)
                {
                    return Segment.Peak;
                }
            }

            // 2. Determine minimum segment based on lit campfires
            int minSegment = 0;
            for (int i = 3; i >= 0; i--)
            {
                if (IsCampfireLit(i))
                {
                    minSegment = i + 1;
                    break;
                }
            }

            // 3. Check if player is standing near any segment's campfire
            for (int i = 0; i < 4; i++)
            {
                if (IsPlayerNearCampfire(i, 12f))
                {
                    if (IsCampfireLit(i))
                    {
                        return (Segment)Math.Max(minSegment, i + 1);
                    }
                    return (Segment)Math.Max(minSegment, i);
                }
            }

            // 4. Check altitude bands across segments (descending from 4 down to 0)
            if (mh.segments != null && mh.segments.Length > 0)
            {
                for (int i = mh.segments.Length - 1; i >= 0; i--)
                {
                    var seg = mh.segments[i];
                    if (seg != null && seg.reconnectSpawnPos != null)
                    {
                        float spawnY = seg.reconnectSpawnPos.position.y;
                        if (i == 0 || spawnY > 10f)
                        {
                            if (pPos.y >= spawnY - 2f)
                            {
                                return (Segment)Math.Max(minSegment, i);
                            }
                        }
                    }
                }
            }

            return (Segment)Math.Max(minSegment, (int)officialSeg);
        }
        catch { }

        return officialSeg;
    }

    public static float GetCurrentPlayerAltitude()
    {
        Character local = Character.localCharacter;
        if (local != null)
        {
            return local.transform.position.y;
        }
        return 0f;
    }

    public static List<RouteSegmentInfo> GetAirportPredictedRoute(string biomeId)
    {
        var route = new List<RouteSegmentInfo>(6);
        Segment[] defaultSegments = new Segment[] {
            Segment.Beach, Segment.Tropics, Segment.Alpine, Segment.Caldera, Segment.TheKiln, Segment.Peak
        };

        for (int i = 0; i < 6; i++)
        {
            string name;
            if (i < 4 && !string.IsNullOrEmpty(biomeId) && i < biomeId.Length)
            {
                char c = biomeId[i];
                if (i == 3 && char.ToUpper(c) == 'S')
                    name = Localization.T("world.segment_swamp");
                else
                    name = DecodeBiomeCharToName(c);
            }
            else if (i == 4)
            {
                name = Localization.T("world.segment_thekiln");
            }
            else if (i == 5)
            {
                name = Localization.T("world.segment_peak");
            }
            else
            {
                name = GetBiomeDisplayName((Biome.BiomeType)(-1), defaultSegments[i]);
            }

            route.Add(new RouteSegmentInfo
            {
                level = i + 1,
                segment = defaultSegments[i],
                biomeType = (Biome.BiomeType)(-1),
                displayName = name,
                isCurrent = (i == 0),
                hasCampfire = (i < 5),
                altitude = 0f,
                isAtCampfire = false
            });
        }
        return route;
    }

    public static List<RouteSegmentInfo> GetFullRoute()
    {
        var route = new List<RouteSegmentInfo>(6);
        Segment currentSeg = DetectCurrentPlayerSegment();

        Segment[] defaultSegments = new Segment[] {
            Segment.Beach, Segment.Tropics, Segment.Alpine, Segment.Caldera, Segment.TheKiln, Segment.Peak
        };

        bool mapExists = MapHandler.Exists && MapHandler.Instance != null;
        var mh = mapExists ? MapHandler.Instance : null;

        for (int i = 0; i < 6; i++)
        {
            int levelNum = i + 1;
            Segment seg = defaultSegments[i];
            Biome.BiomeType bt = (Biome.BiomeType)(-1);
            float altitude = 0f;

            if (mapExists && mh.biomes != null && i < mh.biomes.Count)
            {
                bt = mh.biomes[i];
            }

            if (mapExists && mh.segments != null && i < mh.segments.Length)
            {
                var mapSeg = mh.segments[i];
                if (mapSeg != null)
                {
                    if (bt == (Biome.BiomeType)(-1))
                    {
                        try
                        {
                            bt = mapSeg.biome;
                        }
                        catch { }
                    }

                    if (mapSeg.reconnectSpawnPos != null)
                    {
                        altitude = mapSeg.reconnectSpawnPos.position.y;
                    }
                }
            }
            else if (i == 5)
            {
                bt = Biome.BiomeType.Peak;
                if (mapExists && mh.respawnThePeak != null)
                {
                    altitude = mh.respawnThePeak.position.y;
                }
            }

            string displayName = GetBiomeDisplayName(bt, seg);
            bool isCurrent = mapExists && (currentSeg == seg);
            bool hasCamp = (i < 4);
            bool isAtCamp = hasCamp && isCurrent && IsPlayerNearCampfire(i, 12f);
            bool isCampLit = hasCamp && IsCampfireLit(i);

            route.Add(new RouteSegmentInfo
            {
                level = levelNum,
                segment = seg,
                biomeType = bt,
                displayName = displayName,
                isCurrent = isCurrent,
                hasCampfire = hasCamp,
                altitude = altitude,
                isAtCampfire = isAtCamp,
                isCampfireLit = isCampLit
            });
        }

        return route;
    }

    public static class WorldDataCache
    {
        public static Segment currentSegment = Segment.Beach;
        public static float altitude = 0f;
        public static List<RouteSegmentInfo> route = new List<RouteSegmentInfo>();
        public static bool isAtCampfire = false;
        public static string currentSegDisplayName = "Unknown";
        public static string nextSegDisplayName = "Unknown";
        public static int currentLevelNumber = 1;
        public static int nextLevelNumber = 2;
        public static bool isInAirport = true;

        // Custom Map & Playlist Support
        public static bool isCustomScene = false;
        public static string customMapSourceName = "";
        public static string playlistInfo = "";
        public static string pendingAnnouncedScene = "";
        public static int pendingAnnouncedAscent = -1;
        public static float pendingAnnouncedTime = -100f;

        // Daily Island Info
        public static int todayLevelIndex = 0;
        public static string todaySceneName = "";
        public static string todayBiomeID = "";
        public static string todayBiomeRoute = "";

        public static int nextLevelIndex = 1;
        public static string nextSceneName = "";
        public static string nextBiomeID = "";
        public static string nextBiomeRoute = "";

        public static string countdownFormatted = "";

        private static float s_LastUpdateTime = -10f;
        public const float UPDATE_INTERVAL = 0.5f;

        public static void OnBeginIslandLoadAnnounced(string sceneName, int ascent)
        {
            pendingAnnouncedScene = sceneName;
            pendingAnnouncedAscent = ascent;
            pendingAnnouncedTime = Time.realtimeSinceStartup;
            todaySceneName = sceneName;

            var baker = SingletonAsset<MapBaker>.Instance;
            bool foundInBaker = false;
            if (baker != null && baker.ScenePaths != null)
            {
                for (int k = 0; k < baker.ScenePaths.Length; k++)
                {
                    if (baker.GetLevel(k).Equals(sceneName, StringComparison.OrdinalIgnoreCase))
                    {
                        todayLevelIndex = k;
                        todayBiomeID = baker.GetBiomeID(k);
                        todayBiomeRoute = FormatBiomeIDRoute(todayBiomeID);
                        foundInBaker = true;
                        isCustomScene = false;
                        break;
                    }
                }
            }

            if (!foundInBaker)
            {
                isCustomScene = true;
                todayLevelIndex = -1;
            }

            Invalidate();
        }

        public static void Invalidate()
        {
            s_LastUpdateTime = -10f;
            ClearCampfireCache();
            if (route != null)
                route.Clear();
        }

        public static void EnsureUpdated(bool force = false)
        {
            float now = Time.realtimeSinceStartup;
            if (!force && (now - s_LastUpdateTime < UPDATE_INTERVAL))
            {
                return;
            }
            s_LastUpdateTime = now;

            UpdateCacheInternal();
        }

        private static void UpdateCacheInternal()
        {
            isInAirport = (!MapHandler.Exists || MapHandler.Instance == null);

            var baker = SingletonAsset<MapBaker>.Instance;
            int customMapIdx = -1;
            string customSceneName = "";
            string customBiomeId = "";
            string extPlaylistInfo = "";
            string extSourceName = "";
            bool hasCustomMap = TryGetCustomMapOrPlaylist(out customMapIdx, out customSceneName, out customBiomeId, out extPlaylistInfo, out extSourceName);

            try
            {
                var nextLevelService = GameHandler.GetService<NextLevelService>();
                if (nextLevelService != null && baker != null)
                {
                    int curIdx = nextLevelService.NextLevelIndexOrFallback;
                    int offset = NextLevelService.debugLevelIndexOffset;
                    todayLevelIndex = curIdx + offset;
                    todaySceneName = baker.GetLevel(todayLevelIndex);
                    todayBiomeID = baker.GetBiomeID(todayLevelIndex);
                    todayBiomeRoute = FormatBiomeIDRoute(todayBiomeID);

                    nextLevelIndex = curIdx + offset + 1;
                    nextSceneName = baker.GetLevel(nextLevelIndex);
                    nextBiomeID = baker.GetBiomeID(nextLevelIndex);
                    nextBiomeRoute = FormatBiomeIDRoute(nextBiomeID);

                    int seconds = nextLevelService.Data.IsSome
                        ? nextLevelService.Data.Value.SecondsLeft
                        : nextLevelService.SecondsLeftFallback;

                    if (seconds >= 0)
                    {
                        int h = seconds / 3600;
                        int m = (seconds % 3600) / 60;
                        int s = seconds % 60;
                        countdownFormatted = string.Format("{0}h {1:D2}m {2:D2}s", h, m, s);
                    }
                    else
                    {
                        countdownFormatted = "--:--:--";
                    }
                }
            }
            catch { }

            // Custom map / playlist mod override
            if (hasCustomMap)
            {
                isCustomScene = true;
                customMapSourceName = extSourceName;
                playlistInfo = extPlaylistInfo;
                if (!string.IsNullOrEmpty(customSceneName))
                    todaySceneName = customSceneName;
                if (customMapIdx >= 0)
                    todayLevelIndex = customMapIdx;
                if (!string.IsNullOrEmpty(customBiomeId))
                {
                    todayBiomeID = customBiomeId;
                    todayBiomeRoute = FormatBiomeIDRoute(todayBiomeID);
                }
            }
            else if (isInAirport && Time.realtimeSinceStartup - pendingAnnouncedTime < 30f && !string.IsNullOrEmpty(pendingAnnouncedScene))
            {
                todaySceneName = pendingAnnouncedScene;
            }

            if (!isInAirport)
            {
                // In game: detect active scene in case custom map was loaded
                try
                {
                    if (MapHandler.Instance != null && baker != null)
                    {
                        string activeScene = MapHandler.Instance.gameObject.scene.name;
                        if (!string.IsNullOrEmpty(activeScene) && !activeScene.Equals("Airport", StringComparison.OrdinalIgnoreCase))
                        {
                            todaySceneName = activeScene;
                            bool matchedVanilla = false;
                            if (baker.ScenePaths != null)
                            {
                                for (int k = 0; k < baker.ScenePaths.Length; k++)
                                {
                                    if (baker.GetLevel(k).Equals(activeScene, StringComparison.OrdinalIgnoreCase))
                                    {
                                        todayLevelIndex = k;
                                        todayBiomeID = baker.GetBiomeID(k);
                                        todayBiomeRoute = FormatBiomeIDRoute(todayBiomeID);
                                        matchedVanilla = true;
                                        isCustomScene = false;
                                        break;
                                    }
                                }
                            }

                            if (!matchedVanilla)
                            {
                                isCustomScene = true;
                                todayLevelIndex = -1;
                            }
                        }
                    }
                }
                catch { }

                currentSegment = DetectCurrentPlayerSegment();
                currentLevelNumber = (int)currentSegment + 1;
                altitude = GetCurrentPlayerAltitude();
                route = GetFullRoute();
                isAtCampfire = (currentLevelNumber <= 4) && IsPlayerNearCampfire((int)currentSegment, 12f);

                // If on custom scene, construct todayBiomeRoute directly from route segments!
                if (isCustomScene && route != null && route.Count > 0)
                {
                    List<string> segNames = new List<string>(route.Count);
                    for (int i = 0; i < route.Count; i++)
                    {
                        segNames.Add(route[i].displayName);
                    }
                    todayBiomeRoute = string.Join(" ➔ ", segNames.ToArray());
                }

                currentSegDisplayName = "Unknown";
                for (int i = 0; i < route.Count; i++)
                {
                    if (route[i].segment == currentSegment)
                    {
                        currentSegDisplayName = route[i].displayName;
                        break;
                    }
                }

                if (currentLevelNumber < 6 && currentLevelNumber < route.Count)
                {
                    nextLevelNumber = currentLevelNumber + 1;
                    nextSegDisplayName = route[currentLevelNumber].displayName;
                }
                else
                {
                    nextLevelNumber = 6;
                    nextSegDisplayName = Localization.T("world.segment_peak");
                }
            }
            else
            {
                currentSegment = Segment.Beach;
                currentLevelNumber = 1;
                altitude = 0f;
                route = GetAirportPredictedRoute(todayBiomeID);
                isAtCampfire = false;
                currentSegDisplayName = (route.Count > 0) ? route[0].displayName : Localization.T("world.segment_beach");
                nextSegDisplayName = (route.Count > 1) ? route[1].displayName : "";
                nextLevelNumber = 2;
            }
        }
    }

    public static void ReturnToAirport()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
                RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(
                    LoadingScreen.LoadingScreenType.Plane,
                    null,
                    new System.Collections.IEnumerator[] {
                        RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 2f)
                    }
                );
                WorldDataCache.Invalidate();
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] ReturnToAirport error: " + ex.Message);
            }
        });
    }

    public static bool LightCampfire(int segmentIndex)
    {
        try
        {
            Campfire cf = GetSegmentCampfire(segmentIndex);
            if (cf == null)
            {
                if (Logger != null)
                    Logger.LogWarning(string.Format("[PEAK AIO] Campfire for segment {0} not found.", segmentIndex));
                return false;
            }

            if (!cf.gameObject.activeInHierarchy)
            {
                if (cf.transform.parent != null)
                    cf.transform.parent.gameObject.SetActive(true);
                cf.gameObject.SetActive(true);
            }

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                try
                {
                    cf.DebugLight();
                    if (Logger != null)
                        Logger.LogInfo(string.Format("[PEAK AIO] Lit campfire for segment {0}.", segmentIndex));
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError("[PEAK AIO] Error lighting campfire: " + ex.Message);
                }
            });

            return true;
        }
        catch (Exception ex)
        {
            if (Logger != null)
                Logger.LogError("[PEAK AIO] LightCampfire exception: " + ex.Message);
            return false;
        }
    }

    public static bool LightCurrentCampfire()
    {
        int curIdx = (int)DetectCurrentPlayerSegment();
        if (curIdx < 5)
        {
            return LightCampfire(curIdx);
        }
        return false;
    }

    public static bool TeleportToNextCampfire()
    {
        if (!MapHandler.Exists || MapHandler.Instance == null)
        {
            if (Logger != null)
                Logger.LogWarning("[PEAK AIO] MapHandler does not exist.");
            return false;
        }

        int curIdx = (int)DetectCurrentPlayerSegment();
        if (curIdx >= 5)
        {
            if (Logger != null)
                Logger.LogInfo("[PEAK AIO] Already at The Peak, no next campfire.");
            return false;
        }

        // If player is already standing at current campfire or current campfire is already lit, advance to next segment!
        if (IsPlayerNearCampfire(curIdx, 12f) || IsCampfireLit(curIdx))
        {
            int nextIdx = curIdx + 1;
            if (nextIdx < 4)
            {
                JumpToSegmentCampfire((Segment)nextIdx);
                return true;
            }
            else if (nextIdx == 4)
            {
                JumpToSegmentStartSafe(Segment.TheKiln);
                return true;
            }
            else
            {
                JumpToSegmentStartSafe(Segment.Peak);
                return true;
            }
        }

        return TeleportToCampfire(curIdx);
    }

    public static bool TeleportToCampfire(int segmentIndex)
    {
        try
        {
            Character localCharacter = Character.localCharacter;
            if (localCharacter == null || localCharacter.data.dead)
            {
                if (Logger != null)
                    Logger.LogWarning("[PEAK AIO] Local character is null or dead.");
                return false;
            }

            if (!MapHandler.Exists || MapHandler.Instance == null)
            {
                if (Logger != null)
                    Logger.LogWarning("[PEAK AIO] MapHandler does not exist.");
                return false;
            }

            if (segmentIndex == 4)
            {
                JumpToSegmentStartSafe(Segment.TheKiln);
                return true;
            }
            if (segmentIndex >= 5)
            {
                JumpToSegmentStartSafe(Segment.Peak);
                return true;
            }

            var mh = MapHandler.Instance;

            // Ensure parent and campfire GameObjects are active so colliders/transforms are fully valid
            if (mh.segments != null && segmentIndex >= 0 && segmentIndex < mh.segments.Length)
            {
                var seg = mh.segments[segmentIndex];
                if (seg != null)
                {
                    if (seg.segmentParent != null && !seg.segmentParent.activeSelf)
                        seg.segmentParent.SetActive(true);
                    if (seg.segmentCampfire != null && !seg.segmentCampfire.activeSelf)
                        seg.segmentCampfire.SetActive(true);
                }
            }

            Campfire targetCampfire = GetSegmentCampfire(segmentIndex);

            if (targetCampfire == null)
            {
                if (Logger != null)
                    Logger.LogWarning(string.Format("[PEAK AIO] Campfire before segment {0} not found, falling back to segment start.", (Segment)(segmentIndex + 1)));
                JumpToSegmentStartSafe((Segment)segmentIndex);
                return true;
            }

            if (!targetCampfire.gameObject.activeInHierarchy)
            {
                if (targetCampfire.transform.parent != null)
                    targetCampfire.transform.parent.gameObject.SetActive(true);
                targetCampfire.gameObject.SetActive(true);
            }

            Vector3 cfPos = targetCampfire.transform.position;
            Vector3 forward = targetCampfire.transform.forward;
            if (forward.sqrMagnitude < 0.01f) forward = Vector3.forward;
            Vector3 safePos = cfPos + forward * 1.8f + Vector3.up * 0.4f;

            if (localCharacter.photonView != null)
            {
                localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { safePos, true });
            }
            else
            {
                localCharacter.WarpPlayerRPC(safePos, true);
            }

            if (Logger != null)
                Logger.LogInfo(string.Format("[PEAK AIO] Teleported to campfire before segment {0} at {1}", (Segment)(segmentIndex + 1), safePos));

            WorldDataCache.Invalidate();
            return true;
        }
        catch (Exception ex)
        {
            if (Logger != null)
                Logger.LogError("[PEAK AIO] TeleportToCampfire error: " + ex.Message);
            return false;
        }
    }

    public static void JumpToSegmentCampfire(Segment segment)
    {
        int segIdx = (int)segment;
        if (segIdx == 4)
        {
            JumpToSegmentStartSafe(Segment.TheKiln);
            return;
        }
        if (segIdx >= 5)
        {
            JumpToSegmentStartSafe(Segment.Peak);
            return;
        }

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (MapHandler.Exists && MapHandler.Instance != null)
                {
                    var mh = MapHandler.Instance;
                    if (mh.segments != null && segIdx >= 0 && segIdx < mh.segments.Length)
                    {
                        var seg = mh.segments[segIdx];
                        if (seg != null)
                        {
                            if (seg.segmentParent != null && !seg.segmentParent.activeSelf)
                                seg.segmentParent.SetActive(true);
                            if (seg.segmentCampfire != null && !seg.segmentCampfire.activeSelf)
                                seg.segmentCampfire.SetActive(true);
                            if (seg.wallNext != null && !seg.wallNext.activeSelf)
                                seg.wallNext.SetActive(true);
                            if (seg.wallPrevious != null && !seg.wallPrevious.activeSelf)
                                seg.wallPrevious.SetActive(true);
                        }
                    }

                    if (Photon.Pun.PhotonNetwork.IsMasterClient && (int)MapHandler.CurrentSegmentNumber != segIdx)
                    {
                        try { MapHandler.JumpToSegment(segment); } catch { }
                    }

                    TeleportToCampfire(segIdx);
                    WorldDataCache.Invalidate();
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] JumpToSegmentCampfire error: " + ex.Message);
            }
        });
    }

    public static void SummonHelicopter()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (Singleton<PeakHandler>.Instance != null)
                {
                    Singleton<PeakHandler>.Instance.SummonHelicopter();
                    if (Logger != null)
                        Logger.LogInfo("[PEAK AIO] Helicopter summoned at Peak.");
                }
                else
                {
                    if (Logger != null)
                        Logger.LogWarning("[PEAK AIO] PeakHandler instance not found.");
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] SummonHelicopter error: " + ex.Message);
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
        if (player == null)
            return false;

        if (player.backpackSlot != null && !player.backpackSlot.IsEmpty())
            return true;

        if (player.itemSlots != null && player.itemSlots.Length > 3)
        {
            var backpackSlot = player.itemSlots[3] as BackpackSlot;
            return backpackSlot != null && !backpackSlot.IsEmpty();
        }

        return false;
    }

    public static void GivePlayerBackpack(Player player)
    {
        if (player == null)
        {
            if (Logger != null)
                Logger.LogError("[SpawnBackpack] Player is null.");
            return;
        }

        BackpackSlot backpackSlot = player.backpackSlot;
        if (backpackSlot == null && player.itemSlots != null && player.itemSlots.Length > 3)
        {
            backpackSlot = player.itemSlots[3] as BackpackSlot;
        }

        if (backpackSlot != null)
        {
            if (!backpackSlot.IsEmpty())
            {
                if (Logger != null)
                    Logger.LogInfo("[SpawnBackpack] Player already has backpack.");
                return;
            }

            var data = new ItemInstanceData(Guid.NewGuid());
            ItemInstanceDataHandler.AddInstanceData(data);

            backpackSlot.backpackType = BackpackSlot.BackpackType.Backpack;
            backpackSlot.SetItem(null, data);

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
                Logger.LogError("[SpawnBackpack] BackpackSlot is null.");
        }
    }

    public static bool IsBackpackItem(Item item, string name)
    {
        if (item == null && string.IsNullOrEmpty(name)) return false;
        string n = name ?? "";
        if (item != null && string.IsNullOrEmpty(n))
        {
            try { n = item.GetName(); } catch { }
            if (string.IsNullOrEmpty(n)) n = item.name;
        }
        if (string.IsNullOrEmpty(n)) return false;

        return n.IndexOf("backpack", StringComparison.OrdinalIgnoreCase) >= 0 ||
               n.IndexOf("jetpack", StringComparison.OrdinalIgnoreCase) >= 0 ||
               n.IndexOf("rocketpack", StringComparison.OrdinalIgnoreCase) >= 0 ||
               n.IndexOf("rocket pack", StringComparison.OrdinalIgnoreCase) >= 0 ||
               n.IndexOf("fannypack", StringComparison.OrdinalIgnoreCase) >= 0 ||
               n.IndexOf("fanny pack", StringComparison.OrdinalIgnoreCase) >= 0 ||
               n.IndexOf("背包", StringComparison.OrdinalIgnoreCase) >= 0 ||
               n.IndexOf("腰包", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static BackpackSlot.BackpackType GetBackpackTypeForItem(Item item, string name)
    {
        string n = name ?? "";
        if (item != null && string.IsNullOrEmpty(n))
        {
            try { n = item.GetName(); } catch { }
            if (string.IsNullOrEmpty(n)) n = item.name;
        }
        n = n.ToLowerInvariant();

        if (n.Contains("jetpack") || n.Contains("喷气"))
            return BackpackSlot.BackpackType.Jetpack;
        if (n.Contains("rocket") || n.Contains("火箭"))
            return BackpackSlot.BackpackType.Rocketpack;
        if (n.Contains("fanny") || n.Contains("funny") || n.Contains("腰包") || n.Contains("滑稽"))
            return BackpackSlot.BackpackType.Fannypack;

        return BackpackSlot.BackpackType.Backpack;
    }

    public static Item FindBackpackPrefab(BackpackSlot.BackpackType type)
    {
        string keyword = "backpack";
        switch (type)
        {
            case BackpackSlot.BackpackType.Jetpack: keyword = "jetpack"; break;
            case BackpackSlot.BackpackType.Rocketpack: keyword = "rocket"; break;
            case BackpackSlot.BackpackType.Fannypack: keyword = "fanny"; break;
            case BackpackSlot.BackpackType.Backpack: keyword = "backpack"; break;
        }

        for (int i = 0; i < Globals.items.Count; i++)
        {
            var item = Globals.items[i];
            if (item == null) continue;
            string n = "";
            try { n = item.GetName(); } catch { }
            if (string.IsNullOrEmpty(n)) n = item.name;
            if (!string.IsNullOrEmpty(n) && n.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                return item;
        }
        return null;
    }

    public static void DropCurrentBackpack(Player player)
    {
        if (player == null) return;
        BackpackSlot backpackSlot = player.backpackSlot;
        if (backpackSlot == null && player.itemSlots != null && player.itemSlots.Length > 3)
            backpackSlot = player.itemSlots[3] as BackpackSlot;

        if (backpackSlot != null && !backpackSlot.IsEmpty())
        {
            Vector3 dropPos;
            if (Character.localCharacter != null)
                dropPos = Character.localCharacter.Head + Character.localCharacter.transform.forward * 1.2f + Vector3.up * 0.2f;
            else
                dropPos = player.transform.position + Vector3.up * 0.5f;

            if (backpackSlot.prefab != null)
            {
                ItemDatabase.Add(backpackSlot.prefab, dropPos);
            }
            else
            {
                Item foundPrefab = FindBackpackPrefab(backpackSlot.backpackType);
                if (foundPrefab != null)
                {
                    ItemDatabase.Add(foundPrefab, dropPos);
                }
            }

            backpackSlot.EmptyOut();
            backpackSlot.backpackType = BackpackSlot.BackpackType.None;

            if (Logger != null)
                Logger.LogInfo("[Backpack] Dropped existing backpack to the ground.");
        }
    }

    public static void AssignBackpackItem(int itemIndex)
    {
        GetPlayer();
        if (Globals.playerObj == null)
        {
            if (Logger != null)
                Logger.LogError("[PEAK AIO] Player is null during backpack assignment");
            return;
        }

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var player = Globals.playerObj;
                BackpackSlot backpackSlot = player.backpackSlot;
                if (backpackSlot == null && player.itemSlots != null && player.itemSlots.Length > 3)
                    backpackSlot = player.itemSlots[3] as BackpackSlot;

                if (backpackSlot == null)
                {
                    if (Logger != null) Logger.LogError("[PEAK AIO] BackpackSlot is null");
                    return;
                }

                // 1. 如果槽位4已经有东西，先丢下背包
                if (!backpackSlot.IsEmpty())
                {
                    DropCurrentBackpack(player);
                }

                // 2. 刷出选中的新背包物品
                Item newItem = (itemIndex >= 0 && itemIndex < Globals.items.Count) ? Globals.items[itemIndex] : null;
                string itemName = (itemIndex >= 0 && itemIndex < Globals.itemNames.Count) ? Globals.itemNames[itemIndex] : "";

                var bpType = GetBackpackTypeForItem(newItem, itemName);
                var data = new ItemInstanceData(Guid.NewGuid());
                ItemInstanceDataHandler.AddInstanceData(data);

                // 如果是喷气背包，默认赋予满燃料
                if (bpType == BackpackSlot.BackpackType.Jetpack)
                {
                    var fuel = data.RegisterNewEntry<FloatItemData>(DataEntryKey.Fuel);
                    if (fuel != null) fuel.Value = 100f;
                }

                backpackSlot.backpackType = bpType;
                backpackSlot.SetItem(newItem, data);

                // 3. 全量网络同步
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

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Backpack] Equipped {0} ({1}) to Slot 4", itemName, bpType));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] AssignBackpackItem error: " + ex);
            }
        });
    }

    public static void EquipBackpackToPlayer(int playerIndex, BackpackSlot.BackpackType type, Item customItem = null)
    {
        if (playerIndex < 0 || playerIndex >= Globals.allPlayers.Count) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var character = Globals.allPlayers[playerIndex];
                if (character == null) return;

                Player player = character.player;
                if (player == null && character.IsLocal)
                    player = Player.localPlayer;

                if (player == null)
                {
                    Globals.GlobalNotifier.ShowError(string.Format("未找到玩家 '{0}' 的 Player 组件", character.characterName));
                    return;
                }

                BackpackSlot backpackSlot = player.backpackSlot;
                if (backpackSlot == null && player.itemSlots != null && player.itemSlots.Length > 3)
                    backpackSlot = player.itemSlots[3] as BackpackSlot;

                if (backpackSlot == null)
                {
                    Globals.GlobalNotifier.ShowError(string.Format("玩家 '{0}' 的4号背包槽位为空", character.characterName));
                    return;
                }

                // 1. 如果已有背包先卸下
                if (!backpackSlot.IsEmpty())
                {
                    DropCurrentBackpack(player);
                }

                // 2. 准备背包物品与类型
                Item bpPrefab = customItem != null ? customItem : FindBackpackPrefab(type);
                string bpName = "";
                try { if (bpPrefab != null) bpName = bpPrefab.GetName(); } catch { }
                if (string.IsNullOrEmpty(bpName) && bpPrefab != null) bpName = bpPrefab.name;
                if (string.IsNullOrEmpty(bpName)) bpName = type.ToString();

                if (type == BackpackSlot.BackpackType.None && bpPrefab != null)
                {
                    type = GetBackpackTypeForItem(bpPrefab, bpName);
                }

                var data = new ItemInstanceData(Guid.NewGuid());
                ItemInstanceDataHandler.AddInstanceData(data);

                if (type == BackpackSlot.BackpackType.Jetpack)
                {
                    var fuel = data.RegisterNewEntry<FloatItemData>(DataEntryKey.Fuel);
                    if (fuel != null) fuel.Value = 100f;
                }

                backpackSlot.backpackType = type;
                backpackSlot.SetItem(bpPrefab, data);

                // 3. 网络全量同步（发送给全部客户端使所有人及目标自身都能看到背包）
                var syncObj = new InventorySyncData(
                    player.itemSlots,
                    backpackSlot,
                    player.tempFullSlot
                );
                byte[] syncData = SerializeSyncData(syncObj);

                if (player.photonView != null)
                {
                    player.photonView.RPC("SyncInventoryRPC", RpcTarget.All, new object[] { syncData, true });
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Equipped {0} ({1}) to Slot 4 for player '{2}'", bpName, type, character.characterName));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Lobby] EquipBackpackToPlayer error: " + ex);
                Globals.GlobalNotifier.ShowError("装备背包到4号槽位失败: " + ex.Message);
            }
        });
    }

    public static void GiveItemToPlayer(int playerIndex, int itemIndex)
    {
        if (playerIndex < 0 || playerIndex >= Globals.allPlayers.Count) return;
        if (itemIndex < 0 || itemIndex >= Globals.items.Count) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[playerIndex];
                var item = Globals.items[itemIndex];
                if (target == null || item == null) return;

                Vector3 spawnPos = CalculateGroundSpawnPosition(target, 2.0f);
                ItemDatabase.Add(item, spawnPos);

                string itemName = null;
                try { itemName = item.GetName(); } catch { }
                if (string.IsNullOrEmpty(itemName)) itemName = item.name;

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Gave item '{0}' to player '{1}' on ground", itemName, target.characterName));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Lobby] GiveItemToPlayer error: " + ex);
                Globals.GlobalNotifier.ShowError("给予玩家物品失败: " + ex.Message);
            }
        });
    }

    public static void GiveItemToAllPlayers(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= Globals.items.Count) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var item = Globals.items[itemIndex];
                if (item == null) return;

                if (Globals.allPlayers.Count == 0)
                {
                    RefreshPlayerList();
                }

                int count = 0;
                for (int i = 0; i < Globals.allPlayers.Count; i++)
                {
                    var target = Globals.allPlayers[i];
                    if (target == null) continue;
                    if (Globals.excludeSelfFromAllActions && target.IsLocal) continue;

                    Vector3 spawnPos = CalculateGroundSpawnPosition(target, 2.0f);
                    ItemDatabase.Add(item, spawnPos);
                    count++;
                }

                if (count == 0 && Globals.allPlayers.Count > 0 && Globals.excludeSelfFromAllActions)
                {
                    Globals.GlobalNotifier.ShowError("全员发放未生效：当前勾选了'排除自己'且房间内无其他玩家！");
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Gave item to {0} players on ground. ExcludeSelf: {1}", count, Globals.excludeSelfFromAllActions));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Lobby] GiveItemToAllPlayers error: " + ex);
                Globals.GlobalNotifier.ShowError("全员发放物品失败: " + ex.Message);
            }
        });
    }

    public static void GiveQuickBackpackToPlayer(int playerIndex, BackpackSlot.BackpackType type)
    {
        if (playerIndex < 0 || playerIndex >= Globals.allPlayers.Count) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[playerIndex];
                if (target == null) return;

                Item prefab = FindBackpackPrefab(type);
                Vector3 spawnPos = CalculateGroundSpawnPosition(target, 2.0f);

                if (prefab != null)
                {
                    ItemDatabase.Add(prefab, spawnPos);
                }
                else
                {
                    if (Logger != null)
                        Logger.LogWarning(string.Format("[Lobby] Could not find prefab for backpack type {0}", type));
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Spawned {0} for player '{1}' on ground", type, target.characterName));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Lobby] GiveQuickBackpackToPlayer error: " + ex);
                Globals.GlobalNotifier.ShowError("生成背包失败: " + ex.Message);
            }
        });
    }

    public static void CaptureInventorySnapshot(Character character)
    {
        if (character == null || character.photonView == null) return;
        int photonId = character.photonView.ViewID;

        Player player = character.player;
        if (player == null && character.IsLocal)
            player = Player.localPlayer;
        if (player == null) return;

        var snapshot = new Globals.PlayerInventorySnapshot();
        snapshot.photonId = photonId;
        snapshot.snapshotTime = DateTime.UtcNow;
        snapshot.isConsumed = false;

        // 1. 记录手持槽位 0..2
        if (player.itemSlots != null)
        {
            for (int i = 0; i < Math.Min(3, player.itemSlots.Length); i++)
            {
                var slot = player.itemSlots[i];
                if (slot != null && !slot.IsEmpty() && slot.prefab != null)
                {
                    snapshot.mainSlots[i] = new Globals.ItemSlotSnapshot
                    {
                        prefab = slot.prefab,
                        data = slot.data != null ? slot.data.Copy() : null
                    };
                }
            }
        }

        // 2. 记录背包槽位
        BackpackSlot bpSlot = player.backpackSlot;
        if (bpSlot == null && player.itemSlots != null && player.itemSlots.Length > 3)
            bpSlot = player.itemSlots[3] as BackpackSlot;

        if (bpSlot != null && !bpSlot.IsEmpty())
        {
            snapshot.backpackType = bpSlot.backpackType;
            snapshot.backpackSlotItem = new Globals.ItemSlotSnapshot
            {
                prefab = bpSlot.prefab,
                data = bpSlot.data != null ? bpSlot.data.Copy() : null
            };
        }

        // 存入待恢复池与实时池
        Globals.liveSnapshots[photonId] = snapshot;
        Globals.deathSnapshots[photonId] = snapshot;

        if (Logger != null)
            Logger.LogInfo(string.Format("[InventorySnapshot] Captured snapshot for character {0} (photonId: {1})", character.characterName, photonId));
    }

    public static void RestoreInventorySnapshot(Character character)
    {
        if (character == null || character.photonView == null) return;
        int photonId = character.photonView.ViewID;

        Globals.PlayerInventorySnapshot snapshot;
        // 防重复刷新核心逻辑：单次消费锁
        if (!Globals.deathSnapshots.TryGetValue(photonId, out snapshot) || snapshot == null || snapshot.isConsumed)
        {
            if (Logger != null)
                Logger.LogInfo(string.Format("[RestoreInventory] No active snapshot or already consumed for photonId {0}", photonId));
            return;
        }

        // 立即标记为已消费并从待恢复字典移除（原子防刷）
        snapshot.isConsumed = true;
        Globals.deathSnapshots.Remove(photonId);

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Player player = character.player;
                if (player == null && character.IsLocal)
                    player = Player.localPlayer;
                if (player == null) return;

                // 1. 恢复主槽位 0..2
                if (player.itemSlots != null)
                {
                    for (int i = 0; i < Math.Min(3, player.itemSlots.Length); i++)
                    {
                        var rec = snapshot.mainSlots[i];
                        if (rec != null && rec.prefab != null)
                        {
                            var instanceData = rec.data != null ? rec.data.Copy() : new ItemInstanceData(Guid.NewGuid());
                            ItemInstanceDataHandler.AddInstanceData(instanceData);
                            player.itemSlots[i].SetItem(rec.prefab, instanceData);
                        }
                    }
                }

                // 2. 恢复背包槽位
                BackpackSlot bpSlot = player.backpackSlot;
                if (bpSlot == null && player.itemSlots != null && player.itemSlots.Length > 3)
                    bpSlot = player.itemSlots[3] as BackpackSlot;

                if (bpSlot != null && snapshot.backpackType != BackpackSlot.BackpackType.None)
                {
                    bpSlot.backpackType = snapshot.backpackType;
                    Item bpPrefab = snapshot.backpackSlotItem != null ? snapshot.backpackSlotItem.prefab : FindBackpackPrefab(snapshot.backpackType);
                    ItemInstanceData bpData = (snapshot.backpackSlotItem != null && snapshot.backpackSlotItem.data != null)
                        ? snapshot.backpackSlotItem.data.Copy()
                        : new ItemInstanceData(Guid.NewGuid());
                    ItemInstanceDataHandler.AddInstanceData(bpData);
                    bpSlot.SetItem(bpPrefab, bpData);
                }

                // 3. 网络全同步
                var syncObj = new InventorySyncData(
                    player.itemSlots,
                    bpSlot,
                    player.tempFullSlot
                );
                byte[] syncData = SerializeSyncData(syncObj);
                if (player.photonView != null)
                {
                    player.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { syncData, true });
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[RestoreInventory] Restored inventory slots and backpack for character '{0}' (photonId: {1})", character.characterName, photonId));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[RestoreInventory] Error restoring inventory: " + ex);
            }
        });
    }
}
