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

    public static bool IsItemToxic(Item item)
    {
        if (item == null) return false;
        try
        {
            if (item.GetComponent<Action_InflictPoison>() != null)
                return true;
        }
        catch { }
        try
        {
            string n = item.name;
            if (!string.IsNullOrEmpty(n) && (n.IndexOf("toxic", StringComparison.OrdinalIgnoreCase) >= 0 || n.IndexOf("poison", StringComparison.OrdinalIgnoreCase) >= 0))
                return true;
        }
        catch { }
        return false;
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

            Action<Item, bool> tryAddItem = null;
            tryAddItem = (item, requireAssetOnly) =>
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

                string prefabKey = item.name;
                if (!string.IsNullOrEmpty(prefabKey))
                {
                    if (prefabKey.EndsWith("(Clone)"))
                        prefabKey = prefabKey.Substring(0, prefabKey.Length - 7);

                    bool isToxic = IsItemToxic(item);
                    string uniqueKey = prefabKey + (isToxic ? "_toxic" : "_safe");

                    if (!itemSet.Contains(uniqueKey))
                    {
                        itemSet.Add(uniqueKey);
                        collectedItems.Add(item);
                    }
                }

                try
                {
                    if (item.isSecretlyOtherItemPrefab != null)
                    {
                        tryAddItem(item.isSecretlyOtherItemPrefab, requireAssetOnly);
                    }
                }
                catch { }
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

            // Pass 1: Identify items that have both toxic and non-toxic variants (e.g. Button, Bugle, Cluster mushrooms).
            // Items with Action_RandomMushroomEffect (blind-box shroomberries) are excluded so they keep their clean name.
            var toxicItemsByName = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            var safeItemsByName = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < collectedItems.Count; i++)
            {
                var item = collectedItems[i];
                if (item == null) continue;

                bool isRandom = false;
                try { isRandom = item.GetComponent<Action_RandomMushroomEffect>() != null; } catch { }
                if (isRandom) continue;

                string baseName = null;
                try { baseName = item.GetName(); } catch { }
                if (string.IsNullOrEmpty(baseName)) baseName = item.name;
                if (string.IsNullOrEmpty(baseName)) continue;

                bool isToxic = IsItemToxic(item);
                if (isToxic)
                    toxicItemsByName[baseName] = true;
                else
                    safeItemsByName[baseName] = true;
            }

            // Pass 2: Extract display names.
            // If and only if the item has both toxic and safe variants, append (Safe) / (Toxic).
            // Other random or naturally non-toxic items (e.g. Chubby Shroom, tools) will NOT have any suffixes appended!
            var itemEntries = new List<KeyValuePair<Item, string>>(collectedItems.Count);
            for (int i = 0; i < collectedItems.Count; i++)
            {
                var item = collectedItems[i];
                if (item == null) continue;

                string displayName = null;
                try { displayName = item.GetName(); } catch { }
                if (string.IsNullOrEmpty(displayName)) displayName = item.name;
                if (string.IsNullOrEmpty(displayName)) displayName = "Unknown Item";

                bool isRandom = false;
                try { isRandom = item.GetComponent<Action_RandomMushroomEffect>() != null; } catch { }

                if (!isRandom && toxicItemsByName.ContainsKey(displayName) && safeItemsByName.ContainsKey(displayName))
                {
                    bool isToxic = IsItemToxic(item);
                    if (isToxic)
                    {
                        displayName += Localization.T("items.suffix_toxic");
                    }
                    else
                    {
                        displayName += Localization.T("items.suffix_nontoxic");
                    }
                }

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

                    if (Globals.mushroomSpawnMode != Globals.MushroomSpawnMode.Vanilla && Globals.items[itemIndex] != null)
                    {
                        var comp = Globals.items[itemIndex].GetComponent<Action_RandomMushroomEffect>();
                        if (comp != null && Globals.playerObj != null)
                        {
                            var playerMushroomComps = Globals.playerObj.GetComponentsInChildren<Action_RandomMushroomEffect>(true);
                            if (playerMushroomComps != null)
                            {
                                int effectToSet = 0;
                                switch (Globals.mushroomSpawnMode)
                                {
                                    case Globals.MushroomSpawnMode.Purified:
                                        int[] goods = Action_RandomMushroomEffect.GoodEffects ?? new int[] { 0, 1, 2, 3, 4 };
                                        effectToSet = goods[UnityEngine.Random.Range(0, goods.Length)];
                                        break;
                                    case Globals.MushroomSpawnMode.Toxic:
                                        int[] bads = Action_RandomMushroomEffect.BadEffects ?? new int[] { 5, 6, 7, 8, 9 };
                                        effectToSet = bads[UnityEngine.Random.Range(0, bads.Length)];
                                        break;
                                    case Globals.MushroomSpawnMode.Specific:
                                        effectToSet = Mathf.Clamp(Globals.selectedMushroomEffect, 0, 9);
                                        break;
                                }
                                for (int pm = 0; pm < playerMushroomComps.Length; pm++)
                                {
                                    playerMushroomComps[pm].useDebugEffect = true;
                                    playerMushroomComps[pm].debugEffect = effectToSet;
                                }
                            }
                        }
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

    /// <summary>
    /// 安全获取角色（本地或远端）当前真实的物理世界中心坐标。
    /// 彻底规避《PEAK》中布娃娃角色 Character.transform.position 永远静止在开局出生篝火点的引擎特性。
    /// </summary>
    public static Vector3 GetCharacterPosition(Character character)
    {
        if (character == null) return Vector3.zero;

        // 1. 若角色已死，优先返回生前最后存活坐标或幽灵坐标（防止获取到被移至太空中的尸体）
        if (character.data != null && character.data.dead)
        {
            Vector3 lastLive = character.LastLivingPosition;
            if (lastLive.sqrMagnitude > 1f && lastLive.y < 4000f && lastLive.y > -30f)
                return lastLive;

            if (character.Ghost != null)
            {
                Vector3 gPos = character.Ghost.transform.position;
                if (gPos.sqrMagnitude > 1f && gPos.y < 4000f && gPos.y > -30f)
                    return gPos;
            }
        }

        // 2. 优先使用游戏原生的 VirtualCenter（内部自动处理 alive/dead/warping 状态）
        try
        {
            Vector3 vc = character.VirtualCenter;
            if (vc.sqrMagnitude > 1f && vc.y < 4000f && vc.y > -30f)
                return vc;
        }
        catch { }

        // 3. 备选：原生 Center（躯干 Torso 变换坐标）
        try
        {
            Vector3 center = character.Center;
            if (center.sqrMagnitude > 1f && center.y < 4000f && center.y > -30f)
                return center;
        }
        catch { }

        // 4. 备选：臀部核心刚体物理坐标（与网络同步流 1:1 对应）
        try
        {
            if (character.refs != null && character.refs.hip != null && character.refs.hip.Rig != null)
            {
                Vector3 hip = character.refs.hip.Rig.position;
                if (hip.sqrMagnitude > 1f && hip.y < 4000f && hip.y > -30f)
                    return hip;
            }
        }
        catch { }

        // 5. 备选：头部坐标
        try
        {
            Vector3 head = character.Head;
            if (head.sqrMagnitude > 1f && head.y < 4000f && head.y > -30f)
                return head;
        }
        catch { }

        // 6. 极端保底（若骨骼完全尚未初始化）
        return character.transform.position;
    }

    /// <summary>
    /// 获取角色水平面平视朝向向量（规避布娃娃 Character.transform.forward 不随视角旋转的问题）
    /// </summary>
    public static Vector3 GetCharacterForward(Character character)
    {
        if (character == null) return Vector3.forward;

        if (character.data != null)
        {
            Vector3 dir = character.data.lookDirection_Flat;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                return dir.normalized;
        }

        if (character.refs != null && character.refs.rigCreator != null)
        {
            Vector3 rigFwd = character.refs.rigCreator.transform.forward;
            rigFwd.y = 0f;
            if (rigFwd.sqrMagnitude > 0.001f)
                return rigFwd.normalized;
        }

        Vector3 tfFwd = character.transform.forward;
        tfFwd.y = 0f;
        if (tfFwd.sqrMagnitude > 0.001f)
            return tfFwd.normalized;

        return Vector3.forward;
    }

    public static Vector3 CalculateGroundSpawnPosition(Character character, float forwardDist = 2.0f)
    {
        if (character == null) return Vector3.zero;

        Vector3 lookDir = GetCharacterForward(character);
        Vector3 center = GetCharacterPosition(character);
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
        bool isDowned = (target.data != null && (target.data.passedOut || target.data.fullyPassedOut));

        // 1. 如果没有死亡（存活站立或仅倒地昏迷）：复活在玩家当前原本坐标（贴地对齐），绝不拉回出生点或营火
        if (!isDead)
        {
            Vector3 currentPos = GetCharacterPosition(target);
            return ResolveSafeGroundPosition(currentPos);
        }

        // 2. 玩家已死亡：优先复活在他所在幽灵 (PlayerGhost) 的旁边
        PlayerGhost ghost = target.Ghost;
        if (ghost == null)
        {
            // 防御性搜索：场景中可能存在刚生成但引用尚未绑定的 PlayerGhost
            var allGhosts = UnityEngine.Object.FindObjectsOfType<PlayerGhost>();
            if (allGhosts != null)
            {
                for (int i = 0; i < allGhosts.Length; i++)
                {
                    var g = allGhosts[i];
                    if (g != null)
                    {
                        if (g.m_owner == target)
                        {
                            ghost = g;
                            break;
                        }
                        var gView = g.GetComponent<Photon.Pun.PhotonView>();
                        if (gView != null && target.photonView != null && gView.Owner == target.photonView.Owner)
                        {
                            ghost = g;
                            break;
                        }
                    }
                }
            }
        }

        if (ghost != null && ghost.gameObject != null)
        {
            Vector3 ghostPos = ghost.transform.position;
            // 幽灵漂浮在空中，先探测幽灵正下方的安全坚实地面
            Vector3 groundPos = ResolveSafeGroundPosition(ghostPos);

            // 校验幽灵正下方地面是否有效：高度合理且不是虚空未命中，垂直落差不过度悬殊（< 30m）
            bool groundValid = groundPos.sqrMagnitude > 1f && groundPos.y > -30f && groundPos.y < 4000f &&
                               Mathf.Abs(groundPos.y - ghostPos.y) > 0.05f && (ghostPos.y - groundPos.y) < 30f;
            if (groundValid)
            {
                return groundPos;
            }

            // 若幽灵正下方是无底悬崖/虚空深渊，但幽灵正在观战跟随存活队友 (m_target)
            if (ghost.m_target != null && ghost.m_target.data != null && !ghost.m_target.data.dead)
            {
                Vector3 teammatePos = GetCharacterPosition(ghost.m_target) + GetCharacterForward(ghost.m_target) * 1.0f;
                Vector3 teammateGround = ResolveSafeGroundPosition(teammatePos);
                if (teammateGround.sqrMagnitude > 1f && teammateGround.y > -30f && teammateGround.y < 4000f)
                {
                    return teammateGround;
                }
            }

            // 若幽灵正下方地面稍高/稍低但仍是合法陆地，作为次选
            if (groundPos.sqrMagnitude > 1f && groundPos.y > -30f && groundPos.y < 4000f && Mathf.Abs(groundPos.y - ghostPos.y) > 0.05f)
            {
                return groundPos;
            }
        }

        // 3. 兜底分级链（若幽灵尚未生成，如死亡动画期间，或单人全灭无观战）：
        // 3.1 优先读取死亡发生瞬间记录的现场物理世界坐标
        int viewId = (target.photonView != null) ? target.photonView.ViewID : target.GetInstanceID();
        Vector3 deathPos;
        if (Globals.playerDeathLocations.TryGetValue(viewId, out deathPos) && deathPos.sqrMagnitude > 1f && deathPos.y > -30f && deathPos.y < 4000f)
        {
            return ResolveSafeGroundPosition(deathPos);
        }

        // 3.2 备选：生前最后一次安全地面快照
        Globals.PlayerLocationSnapshot snapshot;
        if (target.photonView != null && Globals.playerSafeLocations.TryGetValue(target.photonView.ViewID, out snapshot))
        {
            if (snapshot.safePosition.sqrMagnitude > 1f && snapshot.safePosition.y > -30f && snapshot.safePosition.y < 4000f)
            {
                return ResolveSafeGroundPosition(snapshot.safePosition);
            }
        }

        // 3.3 备选：角色属性 LastLivingPosition
        if (target.LastLivingPosition.sqrMagnitude > 1f && target.LastLivingPosition.y > -30f && target.LastLivingPosition.y < 4000f)
        {
            return ResolveSafeGroundPosition(target.LastLivingPosition);
        }

        // 3.4 备选：存活队友身边安全地面（优先本地玩家，否则遍历任意存活队友）
        if (Character.localCharacter != null && Character.localCharacter != target && Character.localCharacter.data != null && !Character.localCharacter.data.dead)
        {
            return ResolveSafeGroundPosition(GetCharacterPosition(Character.localCharacter) + GetCharacterForward(Character.localCharacter) * 1.2f);
        }
        var characters = Character.AllCharacters;
        if (characters != null)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                var c = characters[i];
                if (c != null && c != target && c.data != null && !c.data.dead)
                {
                    return ResolveSafeGroundPosition(GetCharacterPosition(c) + GetCharacterForward(c) * 1.2f);
                }
            }
        }

        // 3.5 关卡段落出生点保底（绝对禁止对死亡角色的 target.transform.position 发射射线，因为尸体在 (0, 5000, -5000) 太空）
        Vector3 segmentSpawn;
        Segment curSeg = WorldDataCache.currentSegment;
        if (TryGetSegmentSpawnPosition(curSeg, out segmentSpawn))
        {
            return ResolveSafeGroundPosition(segmentSpawn);
        }

        if (SpawnPoint.LocalSpawnPoint != null)
        {
            return ResolveSafeGroundPosition(SpawnPoint.LocalSpawnPoint.transform.position);
        }

        return Vector3.zero;
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
                    Vector3 spawnPos = Vector3.zero;
                    if (Character.localCharacter != null)
                    {
                        spawnPos = CalculateGroundSpawnPosition(Character.localCharacter, 2.0f);
                        ItemDatabase.Add(item, spawnPos);
                    }
                    else
                    {
                        ItemDatabase.Add(item);
                    }

                    // Mushroom custom attribute injection (Only when spawning via mod)
                    if (Globals.mushroomSpawnMode != Globals.MushroomSpawnMode.Vanilla)
                    {
                        InjectMushroomCustomization(spawnPos);
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

    public static void InjectMushroomCustomization(Vector3 spawnPos)
    {
        try
        {
            Action_RandomMushroomEffect targetComp = null;
            if (spawnPos != Vector3.zero)
            {
                var colliders = Physics.OverlapSphere(spawnPos, 3.5f);
                if (colliders != null)
                {
                    for (int c = 0; c < colliders.Length; c++)
                    {
                        var comp = colliders[c].GetComponentInParent<Action_RandomMushroomEffect>();
                        if (comp != null)
                        {
                            targetComp = comp;
                            break;
                        }
                    }
                }
            }

            if (targetComp == null)
            {
                var allMushrooms = UnityEngine.Object.FindObjectsOfType<Action_RandomMushroomEffect>();
                float closestDist = float.MaxValue;
                if (allMushrooms != null)
                {
                    for (int m = 0; m < allMushrooms.Length; m++)
                    {
                        float d = (spawnPos != Vector3.zero) ? Vector3.Distance(allMushrooms[m].transform.position, spawnPos) : 0f;
                        if (d < closestDist && d < 10f)
                        {
                            closestDist = d;
                            targetComp = allMushrooms[m];
                        }
                    }
                }
            }

            if (targetComp != null)
            {
                int effectToSet = 0;
                switch (Globals.mushroomSpawnMode)
                {
                    case Globals.MushroomSpawnMode.Purified:
                        int[] goods = Action_RandomMushroomEffect.GoodEffects;
                        if (goods == null || goods.Length == 0) goods = new int[] { 0, 1, 2, 3, 4 };
                        effectToSet = goods[UnityEngine.Random.Range(0, goods.Length)];
                        break;
                    case Globals.MushroomSpawnMode.Toxic:
                        int[] bads = Action_RandomMushroomEffect.BadEffects;
                        if (bads == null || bads.Length == 0) bads = new int[] { 5, 6, 7, 8, 9 };
                        effectToSet = bads[UnityEngine.Random.Range(0, bads.Length)];
                        break;
                    case Globals.MushroomSpawnMode.Specific:
                        effectToSet = Mathf.Clamp(Globals.selectedMushroomEffect, 0, 9);
                        break;
                }

                targetComp.useDebugEffect = true;
                targetComp.debugEffect = effectToSet;

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Mushroom] Injected custom effect {0} (mode: {1}) into spawned mushroom.", effectToSet, Globals.mushroomSpawnMode));
            }
        }
        catch (Exception ex)
        {
            if (Logger != null)
                Logger.LogWarning("[Mushroom] InjectMushroomCustomization exception: " + ex);
        }
    }

    private static float _lastToolChargeSyncTime = 0f;

    /// <summary>
    /// 安全高效地充能前三个工具槽位：具备脏标记校验与网络发包节流，消除无意义的 RPC 洪泛。
    /// </summary>
    public static void SafeRechargeToolSlots()
    {
        GetPlayer();
        if (Globals.playerObj == null || Globals.playerObj.itemSlots == null) return;

        bool anyModified = false;
        int maxSlots = Math.Min(3, Globals.playerObj.itemSlots.Length);

        for (int s = 0; s < maxSlots; s++)
        {
            var itemSlot = Globals.playerObj.itemSlots[s];
            if (itemSlot == null || itemSlot.data == null || itemSlot.data.data == null) continue;

            foreach (var kvp in itemSlot.data.data)
            {
                if (kvp.Key == DataEntryKey.PetterItemUses)
                {
                    var intData = kvp.Value as IntItemData;
                    if (intData != null && intData.Value < 100)
                    {
                        intData.Value = 100;
                        anyModified = true;
                    }
                }
                else if (kvp.Key == DataEntryKey.Fuel)
                {
                    var floatData = kvp.Value as FloatItemData;
                    if (floatData != null && floatData.Value < 99.9f)
                    {
                        floatData.Value = 100f;
                        anyModified = true;
                    }
                }
                else if (kvp.Key == DataEntryKey.UseRemainingPercentage)
                {
                    var floatData = kvp.Value as FloatItemData;
                    if (floatData != null && floatData.Value < 99.9f)
                    {
                        floatData.Value = 100f;
                        anyModified = true;
                    }
                }
                else if (kvp.Key == DataEntryKey.ItemUses)
                {
                    var intData = kvp.Value as OptionableIntItemData;
                    if (intData != null && intData.Value < 100)
                    {
                        intData.Value = 100;
                        anyModified = true;
                    }
                }
            }
        }

        if (anyModified && (Time.time - _lastToolChargeSyncTime >= 2.0f))
        {
            _lastToolChargeSyncTime = Time.time;
            if (Globals.playerObj.photonView != null && PhotonNetwork.InRoom)
            {
                var syncObj = new InventorySyncData(
                    Globals.playerObj.itemSlots,
                    Globals.playerObj.backpackSlot,
                    Globals.playerObj.tempFullSlot
                );
                byte[] syncData = SerializeSyncData(syncObj);
                Globals.playerObj.photonView.SafeRPC("SyncInventoryRPC", RpcTarget.Others, syncData, true);
            }
        }
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
                        bool modified = false;
                        foreach (var kvp in itemSlot.data.data)
                        {
                            if (kvp.Key == DataEntryKey.PetterItemUses)
                            {
                                var intData = kvp.Value as IntItemData;
                                if (intData != null && intData.Value != (int)rechargeValue)
                                {
                                    intData.Value = (int)rechargeValue;
                                    modified = true;
                                }
                            }
                            else if (kvp.Key == DataEntryKey.Fuel)
                            {
                                var floatData = kvp.Value as FloatItemData;
                                if (floatData != null && Math.Abs(floatData.Value - rechargeValue) > 0.1f)
                                {
                                    floatData.Value = rechargeValue;
                                    modified = true;
                                }
                            }
                            else if (kvp.Key == DataEntryKey.UseRemainingPercentage)
                            {
                                var floatData = kvp.Value as FloatItemData;
                                if (floatData != null && Math.Abs(floatData.Value - rechargeValue) > 0.1f)
                                {
                                    floatData.Value = rechargeValue;
                                    modified = true;
                                }
                            }
                            else if (kvp.Key == DataEntryKey.ItemUses)
                            {
                                var intData = kvp.Value as OptionableIntItemData;
                                if (intData != null && intData.Value != (int)rechargeValue)
                                {
                                    intData.Value = (int)rechargeValue;
                                    modified = true;
                                }
                            }
                        }

                        // 仅在真实发生数据修改时才同步网络
                        if (modified && Globals.playerObj.photonView != null && PhotonNetwork.InRoom)
                        {
                            var syncObj = new InventorySyncData(
                                Globals.playerObj.itemSlots,
                                Globals.playerObj.backpackSlot,
                                Globals.playerObj.tempFullSlot
                            );
                            byte[] syncData = SerializeSyncData(syncObj);
                            Globals.playerObj.photonView.SafeRPC("SyncInventoryRPC", RpcTarget.Others, syncData, true);
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

    /// <summary>
    /// 向指定角色施加状态效果（负面或属性衰减），支持本地角色直接穿透免疫、远端房主原生RPC与全网广播。
    /// </summary>
    public static void AddStatusEffect(Character target, CharacterAfflictions.STATUSTYPE statusType, float amount)
    {
        if (target == null) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                string targetName = target.photonView != null && target.photonView.Owner != null
                    ? target.photonView.Owner.NickName
                    : (target.IsLocal ? "Self" : "Player");

                if (target.IsLocal)
                {
                    if (target.refs != null && target.refs.afflictions != null)
                    {
                        // 使用底层 SetStatus 穿透 statusesLocked 与 m_inAirport 免疫（原生 AddStatus 会在 airport 或 locked 时静默失败）
                        float cur = target.refs.afflictions.GetCurrentStatus(statusType);
                        float next = Mathf.Clamp(cur + amount, 0f, 1f);
                        target.refs.afflictions.SetStatus(statusType, next, true);
                        target.ClampStamina();
                        if (GUIManager.instance != null && GUIManager.instance.bar != null)
                        {
                            GUIManager.instance.bar.ChangeBar();
                        }
                    }
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        float[] data = new float[12];
                        int idx = (int)statusType;
                        if (idx >= 0 && idx < 12)
                        {
                            data[idx] = amount;
                            if (target.refs != null && target.refs.afflictions != null && target.refs.afflictions.photonView != null)
                            {
                                target.refs.afflictions.photonView.RPC("RPC_ApplyStatusesFromFloatArray", target.photonView.Owner, new object[] { data });
                            }
                        }

                        // 房主直接更新本地镜像并向全房间（Others）广播 SyncStatusesRPC，解决远端不会回传导致UI不更新的问题
                        if (target.refs != null && target.refs.afflictions != null && target.refs.afflictions.currentStatuses != null)
                        {
                            int typeIdx = (int)statusType;
                            if (typeIdx >= 0 && typeIdx < target.refs.afflictions.currentStatuses.Length)
                            {
                                target.refs.afflictions.currentStatuses[typeIdx] = Mathf.Clamp(target.refs.afflictions.currentStatuses[typeIdx] + amount, 0f, 1f);
                                byte[] syncArray = SerializeSyncData(new StatusSyncData
                                {
                                    statusList = new List<float>(target.refs.afflictions.currentStatuses)
                                });
                                target.photonView.RPC("SyncStatusesRPC", RpcTarget.Others, new object[] { syncArray });
                            }
                        }
                    }
                    else
                    {
                        Vector3 center = target.Center;
                        target.photonView.RPC("RPCA_Stick", RpcTarget.All, new object[] {
                            BodypartType.Torso, center, center, statusType, amount
                        });
                        target.photonView.RPC("RPCA_Unstick", RpcTarget.All, Array.Empty<object>());
                    }
                }

                string statusName = Localization.GetStatusTypeName(statusType);
                if (Logger != null)
                    Logger.LogInfo(string.Format("[PEAK AIO] Applied status {0} ({1:F2}) to {2}.", statusType, amount, targetName));
                Globals.GlobalNotifier.ShowError(Localization.T("status.toast_applied", statusName, amount, targetName), 3.5f);
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] AddStatusEffect error: " + ex);
                Globals.GlobalNotifier.ShowError("AddStatusEffect Failed: " + ex.Message);
            }
        });
    }

    /// <summary>
    /// 减轻指定角色的状态效果数值。
    /// </summary>
    public static void SubtractStatusEffect(Character target, CharacterAfflictions.STATUSTYPE statusType, float amount)
    {
        if (target == null) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                string targetName = target.photonView != null && target.photonView.Owner != null
                    ? target.photonView.Owner.NickName
                    : (target.IsLocal ? "Self" : "Player");

                if (target.IsLocal)
                {
                    if (target.refs != null && target.refs.afflictions != null)
                    {
                        float cur = target.refs.afflictions.GetCurrentStatus(statusType);
                        float next = Mathf.Clamp(cur - amount, 0f, 1f);
                        target.refs.afflictions.SetStatus(statusType, next, true);
                        target.ClampStamina();
                        if (GUIManager.instance != null && GUIManager.instance.bar != null)
                        {
                            GUIManager.instance.bar.ChangeBar();
                        }
                    }
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        float[] data = new float[12];
                        int idx = (int)statusType;
                        if (idx >= 0 && idx < 12)
                        {
                            data[idx] = -amount;
                            if (target.refs != null && target.refs.afflictions != null && target.refs.afflictions.photonView != null)
                            {
                                target.refs.afflictions.photonView.RPC("RPC_ApplyStatusesFromFloatArray", target.photonView.Owner, new object[] { data });
                            }
                        }

                        if (target.refs != null && target.refs.afflictions != null && target.refs.afflictions.currentStatuses != null)
                        {
                            int typeIdx = (int)statusType;
                            if (typeIdx >= 0 && typeIdx < target.refs.afflictions.currentStatuses.Length)
                            {
                                target.refs.afflictions.currentStatuses[typeIdx] = Mathf.Clamp(target.refs.afflictions.currentStatuses[typeIdx] - amount, 0f, 1f);
                                byte[] syncArray = SerializeSyncData(new StatusSyncData
                                {
                                    statusList = new List<float>(target.refs.afflictions.currentStatuses)
                                });
                                target.photonView.RPC("SyncStatusesRPC", RpcTarget.Others, new object[] { syncArray });
                            }
                        }
                    }
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[PEAK AIO] Subtracted status {0} ({1:F2}) from {2}.", statusType, amount, targetName));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] SubtractStatusEffect error: " + ex);
            }
        });
    }

    /// <summary>
    /// 清除指定角色的所有负面状态（支持本地自身、远端房主全网清除、非房主拔刺与士气提振）。
    /// </summary>
    public static void ClearAllAfflictionsForCharacter(Character target)
    {
        if (target == null) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                string targetName = target.photonView != null && target.photonView.Owner != null
                    ? target.photonView.Owner.NickName
                    : (target.IsLocal ? "Self" : "Player");

                if (target.IsLocal)
                {
                    ClearAllAfflictions();
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        float[] clearData = new float[12];
                        for (int i = 0; i < 12; i++)
                        {
                            clearData[i] = -2.0f;
                        }
                        if (target.refs != null && target.refs.afflictions != null && target.refs.afflictions.photonView != null)
                        {
                            target.refs.afflictions.photonView.SafeRPC("RPC_ApplyStatusesFromFloatArray", target.photonView.Owner, clearData);

                            // 精确拔除实际扎在目标身上的荆棘，避免无脑 20 次空包发送
                            float thornVal = target.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns);
                            if (thornVal > 0.001f)
                            {
                                int thornCount = Mathf.Clamp(Mathf.CeilToInt(thornVal / 20f), 1, 5);
                                for (int t = 0; t < thornCount; t++)
                                {
                                    target.refs.afflictions.photonView.SafeRPC("RemoveThornRPC", target.photonView.Owner, t);
                                }
                            }
                        }

                        // 提振耐力
                        target.photonView.SafeRPC("MoraleBoost", RpcTarget.All, 100f, 1);
                    }
                    else
                    {
                        if (target.refs != null && target.refs.afflictions != null && target.refs.afflictions.photonView != null)
                        {
                            float thornVal = target.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns);
                            if (thornVal > 0.001f)
                            {
                                int thornCount = Mathf.Clamp(Mathf.CeilToInt(thornVal / 20f), 1, 5);
                                for (int t = 0; t < thornCount; t++)
                                {
                                    target.refs.afflictions.photonView.SafeRPC("RemoveThornRPC", target.photonView.Owner, t);
                                }
                            }
                        }
                        target.photonView.SafeRPC("MoraleBoost", RpcTarget.All, 100f, 1);
                        Globals.GlobalNotifier.ShowError(Localization.T("status.non_host_hint"), 4f);
                    }
                }

                Globals.GlobalNotifier.ShowError(Localization.T("status.toast_cleared", targetName), 3.5f);
                if (Logger != null)
                    Logger.LogInfo(string.Format("[PEAK AIO] Cleared all afflictions for {0}.", targetName));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] ClearAllAfflictionsForCharacter error: " + ex);
                Globals.GlobalNotifier.ShowError("ClearAllAfflictions Failed: " + ex.Message);
            }
        });
    }

    /// <summary>
    /// 一键清除全房间所有玩家的负面状态（可选排除自己）。
    /// </summary>
    public static void ClearAllAfflictionsForAllPlayers()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (Character.AllCharacters == null || Character.AllCharacters.Count == 0)
                {
                    RefreshPlayerList();
                }

                var list = Character.AllCharacters ?? new List<Character>();
                int count = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    var c = list[i];
                    if (c == null) continue;
                    if (Globals.excludeSelfFromAllActions && c.IsLocal) continue;

                    ClearAllAfflictionsForCharacter(c);
                    count++;
                }

                Globals.GlobalNotifier.ShowError(string.Format("已为 {0} 位玩家清除负面状态", count), 3.5f);
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[PEAK AIO] ClearAllAfflictionsForAllPlayers error: " + ex);
            }
        });
    }

    /// <summary>
    /// 恶搞/折磨预设：同时施加中毒 + 寒冷 + 饥饿 + 嗜睡 + 蛛网
    /// </summary>
    public static void ApplyTorturePreset(Character target)
    {
        if (target == null) return;
        AddStatusEffect(target, CharacterAfflictions.STATUSTYPE.Poison, 0.4f);
        AddStatusEffect(target, CharacterAfflictions.STATUSTYPE.Cold, 0.4f);
        AddStatusEffect(target, CharacterAfflictions.STATUSTYPE.Hunger, 0.4f);
        AddStatusEffect(target, CharacterAfflictions.STATUSTYPE.Drowsy, 0.4f);
        AddStatusEffect(target, CharacterAfflictions.STATUSTYPE.Web, 0.5f);
    }

    /// <summary>
    /// 濒死重伤预设：直接施加 90% 负伤
    /// </summary>
    public static void ApplyCriticalInjury(Character target)
    {
        if (target == null) return;
        AddStatusEffect(target, CharacterAfflictions.STATUSTYPE.Injury, 0.9f);
    }

    /// <summary>
    /// 圣光净化预设：清空负面状态并完全补满额外耐力
    /// </summary>
    public static void ApplyDivinePurify(Character target)
    {
        if (target == null) return;
        ClearAllAfflictionsForCharacter(target);
        if (target.photonView != null)
        {
            target.photonView.RPC("MoraleBoost", RpcTarget.All, new object[] { 100f, 1 });
        }
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
                        bool isDowned = (character.data != null && (character.data.passedOut || character.data.fullyPassedOut));

                        if (!isDead)
                        {
                            // 存活或仅倒地：复活在当前位置，原地唤醒，严禁调用 RPCA_ReviveAtPosition，杜绝掉装
                            if (isDowned)
                            {
                                character.photonView.RPC("RPCA_UnPassOut", RpcTarget.All, Array.Empty<object>());
                            }
                            character.photonView.RPC("RPCA_Revive", RpcTarget.All, new object[] { false });

                            Vector3 safePos = GetSafeRevivePosition(character);
                            if (safePos.sqrMagnitude > 1f && safePos.y > -30f && safePos.y < 4000f)
                            {
                                character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { safePos, false });
                            }

                            if (character.refs != null && character.refs.afflictions != null)
                            {
                                character.refs.afflictions.ClearAllStatus(true);
                                character.refs.afflictions.RemoveAllThorns();
                                character.refs.afflictions.ClearAllAfflictions();
                            }
                            if (character.data != null)
                            {
                                character.data.passedOut = false;
                                character.data.fullyPassedOut = false;
                                character.data.deathTimer = 0f;
                                character.data.fallSeconds = 0f;
                                var stamF = ConstantFields.GetStaminaField();
                                if (stamF != null) stamF.SetValue(character.data, 1f);
                            }
                            character.ClampStamina();
                            if (character.IsLocal)
                            {
                                character.SetExtraStamina(0f);
                            }
                            continue;
                        }

                        // 彻底死亡玩家：在其所在幽灵身旁复活
                        Vector3 revivePos = GetSafeRevivePosition(character);

                        character.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] {
                            revivePos, false, -1
                        });

                        // 延迟清除负面诅咒/饥饿
                        var cRef = character;
                        EventComponent.QueueDelayedAction(() =>
                        {
                            if (cRef != null && cRef.refs != null && cRef.refs.afflictions != null)
                            {
                                cRef.refs.afflictions.ClearAllStatus(true);
                                cRef.refs.afflictions.RemoveAllThorns();
                                cRef.refs.afflictions.ClearAllAfflictions();
                            }
                        }, 0.2f);

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
                    Logger.LogInfo(string.Format("[Lobby] Revive All triggered. RestoreItems: {0}", restoreItems));
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

                int count = 0;
                for (int i = 0; i < characters.Count; i++)
                {
                    try
                    {
                        var character = characters[i];
                        if (character == null || character.photonView == null) continue;
                        if (Globals.excludeSelfFromAllActions && character.IsLocal)
                            continue;

                        var statusLockProp = ConstantFields.GetStatusLockProperty();
                        if (character.statusesLocked && statusLockProp != null)
                        {
                            statusLockProp.SetValue(character, false, null);
                        }

                        Vector3 pos = GetCharacterPosition(character);
                        if (pos.y > 4000f || pos.sqrMagnitude < 0.1f)
                            pos = character.LastLivingPosition;

                        CaptureInventorySnapshot(character);
                        int viewId = character.photonView.ViewID;
                        Globals.playerDeathLocations[viewId] = ResolveSafeGroundPosition(pos);

                        if (character.IsLocal)
                        {
                            if (FlyPatch.IsFlying)
                            {
                                FlyPatch.SetFlying(false);
                            }
                            if (character.refs != null && character.refs.items != null)
                            {
                                character.refs.items.EquipSlot(Optionable<byte>.None);
                            }
                            character.photonView.RPC("RPCA_SetDead", RpcTarget.All, Array.Empty<object>());
                            Character.Die();
                        }
                        else
                        {
                            // 广播双重死亡 RPC：先同步状态机与倒计时对齐，再广播彻底死亡掉落
                            character.photonView.RPC("RPCA_SetDead", RpcTarget.All, Array.Empty<object>());
                            character.photonView.RPC("RPCA_Die", RpcTarget.All, new object[] { pos });
                            if (character.data != null)
                            {
                                character.data.dead = true;
                                character.data.fullyPassedOut = true;
                            }
                        }
                        count++;
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                            Logger.LogError(string.Format("[Lobby] Kill failed for a character: {0}", ex.Message));
                    }
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Kill All triggered ({0} players). ExcludeSelf: {1}", count, Globals.excludeSelfFromAllActions));
                Globals.GlobalNotifier.ShowError(string.Format("全员击杀已触发，共处理 {0} 名玩家", count), 3.0f);
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
                Globals.GlobalNotifier.ShowError("全员击杀异常: " + ex.Message);
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

                Vector3 myPos = GetCharacterPosition(Character.localCharacter);
                Vector3 myFwd = GetCharacterForward(Character.localCharacter);
                float baseAngle = Mathf.Atan2(myFwd.x, myFwd.z) * Mathf.Rad2Deg;

                List<Character> toWarp = new List<Character>();
                for (int i = 0; i < characters.Count; i++)
                {
                    var ch = characters[i];
                    if (ch == null || ch.photonView == null) continue;
                    if (Globals.excludeSelfFromAllActions && ch.IsLocal)
                        continue;
                    toWarp.Add(ch);
                }

                int count = toWarp.Count;
                float angleStep = count > 1 ? 360f / count : 0f;
                float radius = 1.5f;

                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        var character = toWarp[i];
                        if (character == null || character.photonView == null) continue;
                        if (character.IsLocal) continue; // 本地角色作为中心基准，无需位移

                        Vector3 targetPos;
                        if (count <= 1)
                        {
                            targetPos = myPos + myFwd * 1.0f;
                        }
                        else
                        {
                            float currentAngle = (baseAngle + i * angleStep) * Mathf.Deg2Rad;
                            Vector3 offset = new Vector3(Mathf.Sin(currentAngle), 0f, Mathf.Cos(currentAngle)) * radius;
                            targetPos = myPos + offset;
                        }

                        Vector3 safeTarget = ResolveSafeGroundPosition(targetPos);
                        if (safeTarget.sqrMagnitude < 1f || safeTarget.y <= -30f || safeTarget.y >= 4000f)
                        {
                            safeTarget = ResolveSafeGroundPosition(myPos);
                        }

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
                    Logger.LogInfo(string.Format("[Lobby] Warp All To Me triggered. ExcludeSelf: {0}, Total: {1}", Globals.excludeSelfFromAllActions, count));
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
                bool isDowned = (target.data != null && (target.data.passedOut || target.data.fullyPassedOut));

                if (!isDead)
                {
                    // 1. 玩家未死亡（存活站立或仅倒地昏迷）：复活在玩家当前位置，绝不能调用 RPCA_ReviveAtPosition，防止全身装备爆落到地上！
                    if (isDowned)
                    {
                        // 原地唤醒倒地玩家
                        target.photonView.RPC("RPCA_UnPassOut", RpcTarget.All, Array.Empty<object>());
                    }

                    // 原地安全复活刷新状态（清除诅咒与异常，不掉装）
                    target.photonView.RPC("RPCA_Revive", RpcTarget.All, new object[] { false });

                    // 计算当前位置的安全地面，并安全微调托举（防卡进地形/缝隙中）
                    Vector3 safePos = GetSafeRevivePosition(target);
                    if (safePos.sqrMagnitude > 1f && safePos.y > -30f && safePos.y < 4000f)
                    {
                        target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { safePos, false });
                    }

                    // 原地恢复状态，彻底清除诅咒、饥饿、中毒等负面
                    if (target.refs != null && target.refs.afflictions != null)
                    {
                        target.refs.afflictions.ClearAllStatus(true);
                        target.refs.afflictions.RemoveAllThorns();
                        target.refs.afflictions.ClearAllAfflictions();
                    }
                    if (target.data != null)
                    {
                        target.data.passedOut = false;
                        target.data.fullyPassedOut = false;
                        target.data.deathTimer = 0f;
                        target.data.fallSeconds = 0f;
                        var stamF = ConstantFields.GetStaminaField();
                        if (stamF != null) stamF.SetValue(target.data, 1f);
                    }
                    target.ClampStamina();
                    if (target.IsLocal)
                    {
                        target.SetExtraStamina(0f);
                    }

                    if (Logger != null)
                        Logger.LogInfo(string.Format("[Lobby] Player {0} is alive/downed; revived at current position without dropping items.", target.characterName));
                    return;
                }

                // 2. 玩家彻底死亡：传送到其所在幽灵身旁复活（绝不传送到出生点）
                Vector3 revivePos = GetSafeRevivePosition(target);

                // 发送 RPCA_ReviveAtPosition (applyStatus = false, 绝不施加诅咒/饥饿)
                target.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] {
                    revivePos, false, -1
                });

                // 复活后延迟清除可能带有的任何负面
                EventComponent.QueueDelayedAction(() =>
                {
                    if (target != null && target.refs != null && target.refs.afflictions != null)
                    {
                        target.refs.afflictions.ClearAllStatus(true);
                        target.refs.afflictions.RemoveAllThorns();
                        target.refs.afflictions.ClearAllAfflictions();
                    }
                }, 0.2f);

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
                    Logger.LogInfo(string.Format("[Lobby] Revive requested for player index {0} at death pos. RestoreItems: {1}", Globals.selectedPlayer, restoreItems));
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

                // 1. 若目标开启了状态锁定（免伤），临时解锁
                var targetStatusProp = ConstantFields.GetStatusLockProperty();
                if (target.statusesLocked && targetStatusProp != null)
                {
                    targetStatusProp.SetValue(target, false, null);
                }

                Vector3 pos = GetCharacterPosition(target);
                if (pos.y > 4000f || pos.sqrMagnitude < 0.1f)
                    pos = target.LastLivingPosition;

                // 2. 捕获物品快照和死亡位置
                CaptureInventorySnapshot(target);
                int viewId = target.photonView.ViewID;
                Globals.playerDeathLocations[viewId] = ResolveSafeGroundPosition(pos);

                // 3. 执行死亡处理
                if (target.IsLocal)
                {
                    if (FlyPatch.IsFlying)
                    {
                        FlyPatch.SetFlying(false);
                    }
                    if (target.refs != null && target.refs.items != null)
                    {
                        target.refs.items.EquipSlot(Optionable<byte>.None);
                    }
                    target.photonView.RPC("RPCA_SetDead", RpcTarget.All, Array.Empty<object>());
                    Character.Die();
                }
                else
                {
                    // 广播双重死亡 RPC：先同步状态机与倒计时对齐，再广播彻底死亡掉落
                    target.photonView.RPC("RPCA_SetDead", RpcTarget.All, Array.Empty<object>());
                    target.photonView.RPC("RPCA_Die", RpcTarget.All, new object[] { pos });
                    if (target.data != null)
                    {
                        target.data.dead = true;
                        target.data.fullyPassedOut = true;
                    }
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Kill executed for player index {0} ({1})", Globals.selectedPlayer, target.characterName));
                Globals.GlobalNotifier.ShowError(string.Format("已击杀玩家: {0}", target.characterName), 3.0f);
            }
            catch (Exception ex)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogError(ex);
                Globals.GlobalNotifier.ShowError("击杀玩家异常: " + ex.Message);
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
                if (target == null || Character.localCharacter == null) return;

                // 1. 获取目标玩家真实物理坐标与朝向
                Vector3 targetPos = GetCharacterPosition(target);
                Vector3 targetFwd = GetCharacterForward(target);

                // 2. 防穿模偏移：计算目标身旁约 0.8 米位置，彻底避免两人刚体完全重合产生物理互斥弹飞
                Vector3 sideOffset = Vector3.Cross(targetFwd, Vector3.up).normalized * 0.8f;
                Vector3 desiredPos = targetPos + sideOffset;
                Vector3 safePos = ResolveSafeGroundPosition(desiredPos);

                // 若侧方探测到悬崖或无效地面，优雅回退到目标正中心安全地面
                if (safePos.sqrMagnitude < 1f || safePos.y <= -30f || safePos.y >= 4000f)
                {
                    safePos = ResolveSafeGroundPosition(targetPos);
                }

                Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                    safePos, true
                });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Warp to player requested for index {0} at {1}", Globals.selectedPlayer, safePos));
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
                if (target == null || Character.localCharacter == null) return;

                // 1. 获取本地玩家真实物理世界坐标与朝向
                Vector3 myPos = GetCharacterPosition(Character.localCharacter);
                Vector3 myFwd = GetCharacterForward(Character.localCharacter);

                // 2. 防穿模偏移：将目标传送到自己正前方 1.0 米处，自然面迎目标且避免刚体挤压
                Vector3 desiredPos = myPos + myFwd * 1.0f;
                Vector3 safePos = ResolveSafeGroundPosition(desiredPos);

                // 若前方落空，优雅回退到自己脚下安全地面
                if (safePos.sqrMagnitude < 1f || safePos.y <= -30f || safePos.y >= 4000f)
                {
                    safePos = ResolveSafeGroundPosition(myPos);
                }

                target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                    safePos, true
                });

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Warp to me requested for player index {0} to {1}", Globals.selectedPlayer, safePos));
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

    private static readonly FieldInfo s_respawnTheKilnField = typeof(MapHandler).GetField("respawnTheKiln", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly PropertyInfo s_respawnTheKilnProp = typeof(MapHandler).GetProperty("respawnTheKiln", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo s_lavaRisingField = typeof(MapHandler).GetField("lavaRising", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly PropertyInfo s_lavaRisingProp = typeof(MapHandler).GetProperty("lavaRising", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo s_variantBiomeField = typeof(MapHandler.MapSegment).GetField("variantBiome", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        ?? typeof(MapHandler.MapSegment).GetField("_variantBiome", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly PropertyInfo s_variantBiomeProp = typeof(MapHandler.MapSegment).GetProperty("variantBiome", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    public static Transform GetRespawnTheKiln(MapHandler mh)
    {
        if (mh == null) return null;
        if (s_respawnTheKilnField != null)
            return s_respawnTheKilnField.GetValue(mh) as Transform;
        if (s_respawnTheKilnProp != null)
            return s_respawnTheKilnProp.GetValue(mh, null) as Transform;
        return null;
    }

    public static LavaRising GetLavaRising(MapHandler mh)
    {
        if (mh != null)
        {
            if (s_lavaRisingField != null)
            {
                var lr = s_lavaRisingField.GetValue(mh) as LavaRising;
                if (lr != null) return lr;
            }
            if (s_lavaRisingProp != null)
            {
                var lr = s_lavaRisingProp.GetValue(mh, null) as LavaRising;
                if (lr != null) return lr;
            }
        }
        return UnityEngine.Object.FindObjectOfType<LavaRising>();
    }

    public static bool TryGetVariantSegment(MapHandler mh, MapHandler.MapSegment seg, out MapHandler.MapSegment variantSeg)
    {
        variantSeg = null;
        if (mh == null || seg == null || !seg.hasVariant) return false;

        try
        {
            Biome.BiomeType vBiome = (Biome.BiomeType)(-1);
            if (s_variantBiomeProp != null)
                vBiome = (Biome.BiomeType)s_variantBiomeProp.GetValue(seg, null);
            else if (s_variantBiomeField != null)
                vBiome = (Biome.BiomeType)s_variantBiomeField.GetValue(seg);

            if (vBiome != (Biome.BiomeType)(-1) && mh.BiomeIsPresent(vBiome))
            {
                if (mh.variantSegments != null)
                {
                    for (int i = 0; i < mh.variantSegments.Length; i++)
                    {
                        var v = mh.variantSegments[i];
                        if (v != null && v.biome == vBiome)
                        {
                            variantSeg = v;
                            return true;
                        }
                    }
                }
            }
        }
        catch { }

        return false;
    }

    public static Vector3 ResolveSafeGroundPosition(Vector3 rawPos)
    {
        if (float.IsNaN(rawPos.x) || float.IsInfinity(rawPos.x) ||
            float.IsNaN(rawPos.y) || float.IsInfinity(rawPos.y) ||
            float.IsNaN(rawPos.z) || float.IsInfinity(rawPos.z))
        {
            return rawPos;
        }

        // 关键：必须显式包含 Default 物理层，因为火山浮石、窑炉平台、山顶停机坪神殿网格均在 Default 层
        int terrainMapMask = LayerMask.GetMask("Terrain", "Map", "Default");
        if (terrainMapMask == 0)
            terrainMapMask = HelperFunctions.AllPhysicalExceptCharacter.value;

        // 刷新 PhysX 碰撞加速结构，确保新激活物体的 MeshCollider 即刻对射线有效
        Physics.SyncTransforms();

        // 1. 向上探测是否有岩壁屋顶、洞穴穹顶或拱桥天花板（避免从洞穴/拱门上方下落射线导致传送到山顶/穹顶极高处）
        RaycastHit ceilHit;
        bool hasCeiling = Physics.Raycast(
            rawPos + Vector3.up * 0.2f,
            Vector3.up,
            out ceilHit,
            3.5f,
            terrainMapMask,
            QueryTriggerInteraction.Ignore
        );

        // 如果头顶有天花板，将向上发射起点限制在天花板距离的一半且不超过0.8m；若无天花板，仅向上偏移1.2m
        float castOffset = hasCeiling ? Mathf.Clamp(ceilHit.distance * 0.5f, 0.15f, 0.8f) : 1.2f;
        Vector3 rayStart = rawPos + Vector3.up * castOffset;

        // 2. 向下发射近距离高精度射线
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, 20.0f, terrainMapMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.point.y <= rayStart.y && hit.normal.y > 0.25f)
            {
                return hit.point + Vector3.up * 0.15f;
            }
        }

        // 3. 中距球体投射（半径0.25m，深度35m，防止射线正好穿过拼接缝隙）
        RaycastHit sphereHit;
        if (Physics.SphereCast(rayStart, 0.25f, Vector3.down, out sphereHit, 35.0f, terrainMapMask, QueryTriggerInteraction.Ignore))
        {
            if (sphereHit.point.y <= rayStart.y && sphereHit.normal.y > 0.25f)
            {
                return sphereHit.point + Vector3.up * 0.15f;
            }
        }

        // 4. 深探 60m 寻找真实地面（防止悬空跌落虚空）
        RaycastHit deepHit;
        if (Physics.Raycast(rawPos + Vector3.up * 0.5f, Vector3.down, out deepHit, 60.0f, terrainMapMask, QueryTriggerInteraction.Ignore))
        {
            if (deepHit.point.y <= rawPos.y + 1.0f && deepHit.normal.y > 0.25f)
            {
                return deepHit.point + Vector3.up * 0.15f;
            }
        }

        // 5. 超深 500m 探底（针对山顶/高空出生点，穿透高空将落点精准吸附至地表停机坪或石台，彻底消除半空悬空）
        RaycastHit ultraHit;
        if (Physics.Raycast(rawPos + Vector3.up * 2.0f, Vector3.down, out ultraHit, 500.0f, terrainMapMask, QueryTriggerInteraction.Ignore))
        {
            if (ultraHit.normal.y > 0.25f)
            {
                return ultraHit.point + Vector3.up * 0.15f;
            }
        }

        return rawPos + Vector3.up * 0.15f;
    }

    public static bool TryGetSegmentSpawnPosition(Segment segment, out Vector3 safePos)
    {
        safePos = Vector3.zero;
        if (!MapHandler.Exists || MapHandler.Instance == null)
            return false;

        var mh = MapHandler.Instance;
        int segIdx = (int)segment;
        Vector3 rawPos = Vector3.zero;
        bool foundPos = false;

        if (segIdx >= 5) // Level 6: Peak
        {
            // 1. 确保第 4 段地形及网格被激活
            if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null)
            {
                if (mh.segments[4].segmentParent != null && !mh.segments[4].segmentParent.activeSelf)
                    mh.segments[4].segmentParent.SetActive(true);
            }
            if (mh.segments != null && mh.segments.Length > 5 && mh.segments[5] != null)
            {
                if (mh.segments[5].segmentParent != null && !mh.segments[5].segmentParent.activeSelf)
                    mh.segments[5].segmentParent.SetActive(true);
            }

            // 2. 确保 PeakHandler 根对象处于激活状态（注意：绝不激活 peakSequence 避免提前触发直升机救援序列）
            if (Singleton<PeakHandler>.Instance != null)
            {
                if (!Singleton<PeakHandler>.Instance.gameObject.activeSelf)
                    Singleton<PeakHandler>.Instance.gameObject.SetActive(true);
            }

            Physics.SyncTransforms();

            // 3. 优先取通关童子军站立点（绝对地表坚实网格）
            if (Singleton<PeakHandler>.Instance != null && Singleton<PeakHandler>.Instance.firstCutsceneScout != null)
            {
                rawPos = Singleton<PeakHandler>.Instance.firstCutsceneScout.transform.position;
                foundPos = true;
            }
            // 4. 次选：官方山顶出生点 respawnThePeak（配合超深探地射线精准吸附停机坪地表）
            else if (mh.respawnThePeak != null && mh.respawnThePeak.position.sqrMagnitude > 1f && mh.respawnThePeak.position.y > 10f && mh.respawnThePeak.position.y < 800f)
            {
                rawPos = mh.respawnThePeak.position;
                foundPos = true;
            }
            else if (Singleton<PeakHandler>.Instance != null && Singleton<PeakHandler>.Instance.transform.position.sqrMagnitude > 1f && Singleton<PeakHandler>.Instance.transform.position.y < 800f)
            {
                rawPos = Singleton<PeakHandler>.Instance.transform.position;
                foundPos = true;
            }
            else if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null && mh.segments[4].reconnectSpawnPos != null)
            {
                rawPos = mh.segments[4].reconnectSpawnPos.position;
                foundPos = true;
            }
        }
        else if (segIdx == 4) // Level 5: The Kiln or The Citadel
        {
            MapHandler.MapSegment activeSeg = null;
            if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null)
            {
                var kSeg = mh.segments[4];
                activeSeg = kSeg;

                // 核心：探测并激活变体切片（如 Citadel 城塞）
                MapHandler.MapSegment vSeg;
                if (TryGetVariantSegment(mh, kSeg, out vSeg) && vSeg != null)
                {
                    activeSeg = vSeg;
                    if (vSeg.segmentParent != null && !vSeg.segmentParent.activeSelf)
                        vSeg.segmentParent.SetActive(true);
                    if (vSeg.segmentCampfire != null && !vSeg.segmentCampfire.activeSelf)
                        vSeg.segmentCampfire.SetActive(true);
                    if (vSeg.wallNext != null && !vSeg.wallNext.activeSelf)
                        vSeg.wallNext.SetActive(true);
                    if (vSeg.wallPrevious != null && !vSeg.wallPrevious.activeSelf)
                        vSeg.wallPrevious.SetActive(true);
                }

                if (kSeg.segmentParent != null && !kSeg.segmentParent.activeSelf)
                    kSeg.segmentParent.SetActive(true);
                if (kSeg.segmentCampfire != null && !kSeg.segmentCampfire.activeSelf)
                    kSeg.segmentCampfire.SetActive(true);
                if (kSeg.wallNext != null && !kSeg.wallNext.activeSelf)
                    kSeg.wallNext.SetActive(true);
                if (kSeg.wallPrevious != null && !kSeg.wallPrevious.activeSelf)
                    kSeg.wallPrevious.SetActive(true);
            }

            // 关键保活：保持第 3 段（Caldera 或变体 Gloom 雾沼）的地表及连接通道激活，防止玩家从交界缝隙跌入虚空
            if (mh.segments != null && mh.segments.Length > 3 && mh.segments[3] != null)
            {
                var cSeg = mh.segments[3];
                MapHandler.MapSegment vSeg3;
                if (TryGetVariantSegment(mh, cSeg, out vSeg3) && vSeg3 != null)
                {
                    if (vSeg3.segmentParent != null && !vSeg3.segmentParent.activeSelf)
                        vSeg3.segmentParent.SetActive(true);
                    if (vSeg3.wallNext != null && !vSeg3.wallNext.activeSelf)
                        vSeg3.wallNext.SetActive(true);
                }
                if (cSeg.segmentParent != null && !cSeg.segmentParent.activeSelf)
                    cSeg.segmentParent.SetActive(true);
                if (cSeg.wallNext != null && !cSeg.wallNext.activeSelf)
                    cSeg.wallNext.SetActive(true);
            }

            Physics.SyncTransforms();

            // 若激活的是变体切片（如 Citadel 城塞），优先取变体的 reconnectSpawnPos 或 RespawnChest
            if (activeSeg != null && activeSeg != (mh.segments.Length > 4 ? mh.segments[4] : null))
            {
                if (activeSeg.reconnectSpawnPos != null && activeSeg.reconnectSpawnPos.position.sqrMagnitude > 1f && activeSeg.reconnectSpawnPos.position.y > -50f)
                {
                    rawPos = activeSeg.reconnectSpawnPos.position;
                    foundPos = true;
                }
                else if (activeSeg.segmentParent != null)
                {
                    RespawnChest chest = activeSeg.segmentParent.GetComponentInChildren<RespawnChest>(true);
                    if (chest != null && chest.transform.position.sqrMagnitude > 1f && chest.transform.position.y > -50f)
                    {
                        rawPos = chest.transform.position + chest.transform.forward * 1.5f;
                        foundPos = true;
                    }
                }
            }

            // 若不是变体或变体未找到点位，走经典熔炉查找
            if (!foundPos)
            {
                Transform kilnSpawn = GetRespawnTheKiln(mh);
                if (kilnSpawn != null && kilnSpawn.position.sqrMagnitude > 1f && kilnSpawn.position.y > -10f)
                {
                    rawPos = kilnSpawn.position;
                    foundPos = true;
                    if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null && mh.segments[4].reconnectSpawnPos == null)
                    {
                        mh.segments[4].reconnectSpawnPos = kilnSpawn;
                    }
                }
                else if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null && mh.segments[4].reconnectSpawnPos != null && mh.segments[4].reconnectSpawnPos.position.sqrMagnitude > 1f)
                {
                    rawPos = mh.segments[4].reconnectSpawnPos.position;
                    foundPos = true;
                }
                else if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null && mh.segments[4].segmentParent != null)
                {
                    RespawnChest chest = mh.segments[4].segmentParent.GetComponentInChildren<RespawnChest>(true);
                    if (chest != null && chest.transform.position.sqrMagnitude > 1f)
                    {
                        rawPos = chest.transform.position + chest.transform.forward * 1.5f;
                        foundPos = true;
                    }
                }
                else
                {
                    LavaRising lr = GetLavaRising(mh);
                    if (lr != null && lr.transform.position.sqrMagnitude > 1f)
                    {
                        rawPos = lr.transform.position + Vector3.up * 8f;
                        foundPos = true;
                    }
                }
            }
        }
        else if (mh.segments != null && segIdx >= 0 && segIdx < mh.segments.Length)
        {
            var seg = mh.segments[segIdx];
            if (seg != null)
            {
                MapHandler.MapSegment activeSeg = seg;
                // 核心：处理 Caldera（第 4 关）等含有变体地形（Volcano/Mesa/Swamp/Roots）的段落重定向
                MapHandler.MapSegment vSeg;
                if (TryGetVariantSegment(mh, seg, out vSeg) && vSeg != null)
                {
                    activeSeg = vSeg;
                    if (vSeg.segmentParent != null && !vSeg.segmentParent.activeSelf)
                        vSeg.segmentParent.SetActive(true);
                    if (vSeg.segmentCampfire != null && !vSeg.segmentCampfire.activeSelf)
                        vSeg.segmentCampfire.SetActive(true);
                    if (vSeg.wallNext != null && !vSeg.wallNext.activeSelf)
                        vSeg.wallNext.SetActive(true);
                    if (vSeg.wallPrevious != null && !vSeg.wallPrevious.activeSelf)
                        vSeg.wallPrevious.SetActive(true);

                    if (vSeg.reconnectSpawnPos != null && (seg.reconnectSpawnPos == null || seg.reconnectSpawnPos.position.sqrMagnitude < 1f))
                    {
                        seg.reconnectSpawnPos = vSeg.reconnectSpawnPos;
                    }
                }

                if (seg.segmentParent != null && !seg.segmentParent.activeSelf)
                    seg.segmentParent.SetActive(true);
                if (seg.segmentCampfire != null && !seg.segmentCampfire.activeSelf)
                    seg.segmentCampfire.SetActive(true);
                if (seg.wallNext != null && !seg.wallNext.activeSelf)
                    seg.wallNext.SetActive(true);
                if (seg.wallPrevious != null && !seg.wallPrevious.activeSelf)
                    seg.wallPrevious.SetActive(true);

                Physics.SyncTransforms();

                // 候选 0：海滩起点专属优先判定
                if (segIdx == 0 && SpawnPoint.LocalSpawnPoint != null && SpawnPoint.LocalSpawnPoint.transform.position.sqrMagnitude > 1f)
                {
                    rawPos = SpawnPoint.LocalSpawnPoint.transform.position;
                    foundPos = true;
                }

                // 候选 1：生效变体段落的 reconnectSpawnPos
                if (!foundPos && activeSeg.reconnectSpawnPos != null && activeSeg.reconnectSpawnPos.position.sqrMagnitude > 1f && activeSeg.reconnectSpawnPos.position.y > -50f)
                {
                    rawPos = activeSeg.reconnectSpawnPos.position;
                    foundPos = true;
                }

                // 候选 2：基础段落的 reconnectSpawnPos
                if (!foundPos && seg.reconnectSpawnPos != null && seg.reconnectSpawnPos.position.sqrMagnitude > 1f && seg.reconnectSpawnPos.position.y > -50f)
                {
                    rawPos = seg.reconnectSpawnPos.position;
                    foundPos = true;
                }

                // 候选 3：MountainProgressHandler 权威进度点 Transform
                if (!foundPos && MountainProgressHandler.Instance != null && MountainProgressHandler.Instance.progressPoints != null)
                {
                    var points = MountainProgressHandler.Instance.progressPoints;
                    if (segIdx >= 0 && segIdx < points.Length && points[segIdx] != null && points[segIdx].transform != null)
                    {
                        var ptTf = points[segIdx].transform;
                        if (ptTf.position.sqrMagnitude > 1f && ptTf.position.y > -50f)
                        {
                            rawPos = ptTf.position;
                            foundPos = true;
                        }
                    }
                }

                // 候选 4：RespawnChest 石雕像（Caldera 等段落的重生标志）
                if (!foundPos && activeSeg.segmentParent != null)
                {
                    RespawnChest chest = activeSeg.segmentParent.GetComponentInChildren<RespawnChest>(true);
                    if (chest != null && chest.transform.position.sqrMagnitude > 1f && chest.transform.position.y > -50f)
                    {
                        rawPos = chest.transform.position + chest.transform.forward * 1.5f;
                        foundPos = true;
                    }
                }
                if (!foundPos && seg.segmentParent != null)
                {
                    RespawnChest chest = seg.segmentParent.GetComponentInChildren<RespawnChest>(true);
                    if (chest != null && chest.transform.position.sqrMagnitude > 1f && chest.transform.position.y > -50f)
                    {
                        rawPos = chest.transform.position + chest.transform.forward * 1.5f;
                        foundPos = true;
                    }
                }

                // 候选 5（最后兜底）：段落营火位置
                if (!foundPos && activeSeg.segmentCampfire != null)
                {
                    Campfire cf = activeSeg.segmentCampfire.GetComponentInChildren<Campfire>(true);
                    if (cf != null && cf.transform.position.sqrMagnitude > 1f && cf.transform.position.y > -50f)
                    {
                        rawPos = cf.transform.position + cf.transform.forward * 1.5f;
                        foundPos = true;
                    }
                }
                if (!foundPos && seg.segmentCampfire != null)
                {
                    Campfire cf = seg.segmentCampfire.GetComponentInChildren<Campfire>(true);
                    if (cf != null && cf.transform.position.sqrMagnitude > 1f && cf.transform.position.y > -50f)
                    {
                        rawPos = cf.transform.position + cf.transform.forward * 1.5f;
                        foundPos = true;
                    }
                }
            }
        }

        if (!foundPos || rawPos.sqrMagnitude < 0.1f || rawPos.y < -50f)
        {
            if (Character.localCharacter != null)
            {
                safePos = ResolveSafeGroundPosition(Character.localCharacter.transform.position);
                return true;
            }
            return false;
        }

        safePos = ResolveSafeGroundPosition(rawPos);
        return true;
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

                Vector3 finalPos;
                if (!TryGetSegmentSpawnPosition(segment, out finalPos))
                {
                    Globals.GlobalNotifier.ShowError(string.Format("未找到区域 {0} 的安全出生点！", segment));
                    return;
                }

                int segIdx = (int)segment;

                // 若房主跳转，同步官方段落切换
                if (Photon.Pun.PhotonNetwork.IsMasterClient && (int)MapHandler.CurrentSegmentNumber != segIdx)
                {
                    try
                    {
                        MapHandler.JumpToSegment(segment);
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                            Logger.LogWarning("[PEAK AIO] MapHandler.JumpToSegment error: " + ex.Message);
                    }
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

    public class RouteNodeBadge
    {
        public int stepIndex;
        public string icon = "📍";
        public string name = "";
        public char biomeChar = ' ';
        public Segment segment = Segment.Beach;
        public bool hasSegment = false;
        public bool isCompleted = false;
        public bool isCurrent = false;
        public bool isPending = true;
        public float altitude = 0f;
        public bool hasCampfire = false;
        public bool isCampfireLit = false;
    }

    public class PlaylistItemInfo
    {
        public int index;
        public int mapIndex = -1;
        public string sceneName = "";
        public string displayName = "";
        public string biomeId = "";
        public string formattedRoute = "";
        public bool isCurrent = false;
        public List<RouteNodeBadge> nodes = new List<RouteNodeBadge>();
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

    public static bool IsCitadelActive(MapHandler mh = null)
    {
        if (mh == null && MapHandler.Exists) mh = MapHandler.Instance;
        try
        {
            if (!string.IsNullOrEmpty(WorldDataCache.todayBiomeID))
            {
                string b = WorldDataCache.todayBiomeID.ToUpper();
                if (b.Length >= 5 && b[4] == 'C') return true;
                if (b.Length >= 4 && (b[3] == 'S' || b[3] == 'G')) return true;
            }
            if (mh != null)
            {
                if (mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null)
                {
                    MapHandler.MapSegment vSeg;
                    if (TryGetVariantSegment(mh, mh.segments[4], out vSeg) && vSeg != null)
                        return true;
                }
                if (mh.variantSegments != null)
                {
                    for (int i = 0; i < mh.variantSegments.Length; i++)
                    {
                        var v = mh.variantSegments[i];
                        if (v != null && v.segmentParent != null && v.segmentParent.name.IndexOf("Citadel", StringComparison.OrdinalIgnoreCase) >= 0 && v.segmentParent.activeSelf)
                            return true;
                    }
                }
            }
        }
        catch { }
        return false;
    }

    public static string GetBiomeIcon(char c)
    {
        switch (char.ToUpper(c))
        {
            case 'S': return "🏝️"; // Shore / Beach
            case 'T': return "🌴"; // Tropics
            case 'R': return "🌿"; // Roots
            case 'A': return "🏔️"; // Alpine
            case 'M': return "🏜️"; // Mesa
            case 'V': return "🌋"; // Volcano / Caldera
            case 'G': return "🌫️"; // Gloom / Swamp
            case 'C': return "🏰"; // Citadel
            case 'K': return "🔥"; // Kiln
            case 'P': return "🚩"; // Peak
            default: return "📍";
        }
    }

    public static string GetBiomeIcon(Biome.BiomeType bt, Segment seg)
    {
        if (seg == Segment.TheKiln) return IsCitadelActive() ? "🏰" : "🔥";
        if (seg == Segment.Peak) return "🚩";
        switch (bt)
        {
            case Biome.BiomeType.Shore: return "🏝️";
            case Biome.BiomeType.Tropics: return "🌴";
            case Biome.BiomeType.Roots: return "🌿";
            case Biome.BiomeType.Alpine: return "🏔️";
            case Biome.BiomeType.Volcano: return "🌋";
            case Biome.BiomeType.Mesa: return "🏜️";
            case Biome.BiomeType.Peak: return "🚩";
            default:
                switch (seg)
                {
                    case Segment.Beach: return "🏝️";
                    case Segment.Tropics: return "🌴";
                    case Segment.Alpine: return "🏔️";
                    case Segment.Caldera: return "🌋";
                    case Segment.TheKiln: return IsCitadelActive() ? "🏰" : "🔥";
                    case Segment.Peak: return "🚩";
                    default: return "📍";
                }
        }
    }

    public static List<RouteNodeBadge> BuildRouteBadges(string biomeId, Segment currentSeg, bool isCurrentMap, bool isInAirport, List<RouteSegmentInfo> liveRoute = null)
    {
        var badges = new List<RouteNodeBadge>();

        // 1. If liveRoute is valid and we have active segments, use liveRoute for high fidelity
        if (liveRoute != null && liveRoute.Count > 0 && !isInAirport)
        {
            for (int i = 0; i < liveRoute.Count; i++)
            {
                var r = liveRoute[i];
                char bChar = ' ';
                if (!string.IsNullOrEmpty(biomeId) && i < biomeId.Length)
                {
                    bChar = biomeId[i];
                }

                bool isCur = isCurrentMap && r.isCurrent;
                bool isComp = isCurrentMap && ((int)r.segment < (int)currentSeg || r.isCampfireLit);
                bool isPend = !isCur && !isComp;

                badges.Add(new RouteNodeBadge
                {
                    stepIndex = r.level,
                    icon = GetBiomeIcon(r.biomeType, r.segment),
                    name = r.displayName,
                    biomeChar = bChar,
                    segment = r.segment,
                    isCompleted = isComp,
                    isCurrent = isCur,
                    isPending = isPend,
                    altitude = r.altitude,
                    hasCampfire = r.hasCampfire,
                    isCampfireLit = r.isCampfireLit
                });
            }
            return badges;
        }

        // 2. Otherwise build from biomeId string
        List<char> chars = new List<char>();
        if (!string.IsNullOrEmpty(biomeId))
        {
            for (int i = 0; i < biomeId.Length; i++)
            {
                chars.Add(biomeId[i]);
            }
        }
        else
        {
            chars.AddRange(new char[] { 'S', 'T', 'A', 'V' });
        }

        if (chars.Count < 5)
        {
            char seg4Char = chars.Count > 3 ? char.ToUpper(chars[3]) : 'V';
            if (seg4Char == 'S' || seg4Char == 'G')
                chars.Add('C');
            else
                chars.Add('K');
        }
        if (chars.Count < 6)
        {
            chars.Add('P');
        }

        Segment[] defSegs = new Segment[] {
            Segment.Beach, Segment.Tropics, Segment.Alpine, Segment.Caldera, Segment.TheKiln, Segment.Peak
        };

        for (int i = 0; i < chars.Count; i++)
        {
            char c = chars[i];
            bool hasSeg = (i < defSegs.Length);
            Segment s = hasSeg ? defSegs[i] : Segment.Beach;
            string name;
            if (i == 3 && (char.ToUpper(c) == 'S' || char.ToUpper(c) == 'G'))
                name = Localization.T("world.segment_swamp");
            else
                name = DecodeBiomeCharToName(c);

            bool isCur = false;
            bool isComp = false;

            if (isCurrentMap && !isInAirport)
            {
                int curSegIdx = (int)currentSeg;
                if (i == curSegIdx)
                {
                    isCur = true;
                }
                else if (i < curSegIdx)
                {
                    isComp = true;
                }
            }

            badges.Add(new RouteNodeBadge
            {
                stepIndex = i + 1,
                icon = GetBiomeIcon(c),
                name = name,
                biomeChar = c,
                segment = s,
                hasSegment = hasSeg,
                isCompleted = isComp,
                isCurrent = isCur,
                isPending = !isCur && !isComp,
                altitude = 0f,
                hasCampfire = (i < chars.Count - 1),
                isCampfireLit = isComp
            });
        }

        return badges;
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
            case 'G': return Localization.T("world.segment_swamp");
            case 'C': return Localization.T("world.segment_citadel");
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
            if (i == 3 && (char.ToUpper(c) == 'S' || char.ToUpper(c) == 'G'))
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
            char seg4 = char.ToUpper(biomeId[3]);
            if (seg4 == 'S' || seg4 == 'G')
                parts.Add(Localization.T("world.segment_citadel"));
            else
                parts.Add(Localization.T("world.segment_thekiln"));
            parts.Add(Localization.T("world.segment_peak"));
        }
        else if (parts.Count == 5)
        {
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
        // 只有 0..3 段落（Beach, Tropics, Alpine, Caldera）可能存在营火，熔炉与山顶绝无营火
        if (segmentIndex < 0 || segmentIndex >= 4)
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
            if (seg != null)
            {
                // 1. 优先从生效的变体段落查找营火（特别是 Caldera 各种变体，如火山/沼泽等）
                MapHandler.MapSegment vSeg;
                if (TryGetVariantSegment(mh, seg, out vSeg) && vSeg != null && vSeg.segmentCampfire != null)
                {
                    targetCampfire = vSeg.segmentCampfire.GetComponentInChildren<Campfire>(true);
                }

                // 2. 从基础段落查找营火
                if (targetCampfire == null && seg.segmentCampfire != null)
                {
                    targetCampfire = seg.segmentCampfire.GetComponentInChildren<Campfire>(true);
                }
            }
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

        return Vector3.Distance(GetCharacterPosition(local), cf.transform.position) <= maxDist;
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

    public static bool TryGetCustomMapOrPlaylist(
        out int customMapIndex,
        out string sceneName,
        out string biomeId,
        out string playlistInfo,
        out string sourceName,
        out List<PlaylistItemInfo> playlistQueue,
        out int activePlaylistIndex)
    {
        customMapIndex = -1;
        sceneName = "";
        biomeId = "";
        playlistInfo = "";
        sourceName = "";
        playlistQueue = new List<PlaylistItemInfo>();
        activePlaylistIndex = 0;

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

                                    PropertyInfo curPlIdxProp = type.GetProperty("CurrentPlaylistIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("PlaylistIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("CurrentMapIndexInPlaylist", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                    if (curPlIdxProp != null)
                                    {
                                        object val = curPlIdxProp.GetValue(instance, null);
                                        if (val is int) activePlaylistIndex = (int)val;
                                    }

                                    var baker = SingletonAsset<MapBaker>.Instance;
                                    if (baker != null && customMapIndex >= 0 && string.IsNullOrEmpty(sceneName))
                                    {
                                        sceneName = baker.GetLevel(customMapIndex);
                                        if (string.IsNullOrEmpty(biomeId))
                                            biomeId = baker.GetBiomeID(customMapIndex);
                                    }

                                    // Parse Playlist elements
                                    PropertyInfo playlistProp = type.GetProperty("Playlist", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                        ?? type.GetProperty("MapList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                    if (playlistProp != null)
                                    {
                                        object pVal = playlistProp.GetValue(instance, null);
                                        System.Collections.IEnumerable enumerable = pVal as System.Collections.IEnumerable;
                                        if (enumerable != null)
                                        {
                                            int k = 0;
                                            foreach (object item in enumerable)
                                            {
                                                if (item == null) continue;
                                                PlaylistItemInfo itemInfo = new PlaylistItemInfo();
                                                itemInfo.index = k + 1;

                                                if (item is int)
                                                {
                                                    int mapIdx = (int)item;
                                                    itemInfo.mapIndex = mapIdx;
                                                    if (baker != null)
                                                    {
                                                        itemInfo.sceneName = baker.GetLevel(mapIdx);
                                                        itemInfo.biomeId = baker.GetBiomeID(mapIdx);
                                                    }
                                                    itemInfo.displayName = !string.IsNullOrEmpty(itemInfo.sceneName) ? itemInfo.sceneName : string.Format("Map #{0}", mapIdx);
                                                }
                                                else if (item is string)
                                                {
                                                    string str = (string)item;
                                                    itemInfo.sceneName = str;
                                                    itemInfo.displayName = str;
                                                    if (baker != null && baker.ScenePaths != null)
                                                    {
                                                        for (int b = 0; b < baker.ScenePaths.Length; b++)
                                                        {
                                                            if (baker.GetLevel(b).Equals(str, StringComparison.OrdinalIgnoreCase))
                                                            {
                                                                itemInfo.mapIndex = b;
                                                                itemInfo.biomeId = baker.GetBiomeID(b);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Type itemType = item.GetType();
                                                    PropertyInfo sProp = itemType.GetProperty("SceneName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                                        ?? itemType.GetProperty("CustomSceneName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                                        ?? itemType.GetProperty("Name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                                    if (sProp != null)
                                                    {
                                                        object sv = sProp.GetValue(item, null);
                                                        if (sv != null) itemInfo.sceneName = sv.ToString();
                                                    }

                                                    PropertyInfo bProp = itemType.GetProperty("BiomeID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                                        ?? itemType.GetProperty("CustomBiomeID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                                    if (bProp != null)
                                                    {
                                                        object bv = bProp.GetValue(item, null);
                                                        if (bv != null) itemInfo.biomeId = bv.ToString();
                                                    }

                                                    PropertyInfo mProp = itemType.GetProperty("MapIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                                        ?? itemType.GetProperty("CustomMapIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)
                                                        ?? itemType.GetProperty("LevelIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                                    if (mProp != null)
                                                    {
                                                        object mv = mProp.GetValue(item, null);
                                                        if (mv is int) itemInfo.mapIndex = (int)mv;
                                                    }

                                                    if (baker != null && itemInfo.mapIndex >= 0)
                                                    {
                                                        if (string.IsNullOrEmpty(itemInfo.sceneName)) itemInfo.sceneName = baker.GetLevel(itemInfo.mapIndex);
                                                        if (string.IsNullOrEmpty(itemInfo.biomeId)) itemInfo.biomeId = baker.GetBiomeID(itemInfo.mapIndex);
                                                    }

                                                    itemInfo.displayName = !string.IsNullOrEmpty(itemInfo.sceneName) ? itemInfo.sceneName : string.Format("Map #{0}", k + 1);
                                                }

                                                if (string.IsNullOrEmpty(itemInfo.displayName))
                                                {
                                                    itemInfo.displayName = string.Format("Map #{0}", k + 1);
                                                }

                                                itemInfo.formattedRoute = FormatBiomeIDRoute(itemInfo.biomeId);
                                                itemInfo.isCurrent = (k == activePlaylistIndex) ||
                                                    (!string.IsNullOrEmpty(sceneName) && sceneName.Equals(itemInfo.sceneName, StringComparison.OrdinalIgnoreCase));
                                                itemInfo.nodes = BuildRouteBadges(itemInfo.biomeId, Segment.Beach, itemInfo.isCurrent, true, null);

                                                playlistQueue.Add(itemInfo);
                                                k++;
                                            }

                                            if (playlistQueue.Count > 0)
                                            {
                                                playlistInfo = string.Format("{0} maps", playlistQueue.Count);
                                            }
                                        }
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

    public static bool TryGetCustomMapOrPlaylist(out int customMapIndex, out string sceneName, out string biomeId, out string playlistInfo, out string sourceName)
    {
        List<PlaylistItemInfo> queue;
        int activeIdx;
        return TryGetCustomMapOrPlaylist(out customMapIndex, out sceneName, out biomeId, out playlistInfo, out sourceName, out queue, out activeIdx);
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

        Vector3 pPos = GetCharacterPosition(local);
        var mh = MapHandler.Instance;
        Segment officialSeg = MapHandler.CurrentSegmentNumber;

        try
        {
            // 1. 检查是否在 Peak 山顶（高度和距离双重判定）
            if (officialSeg == Segment.Peak)
            {
                return Segment.Peak;
            }
            if (mh.respawnThePeak != null && mh.respawnThePeak.position.y > 50f)
            {
                if (pPos.y >= mh.respawnThePeak.position.y - 25f && Vector3.Distance(pPos, mh.respawnThePeak.position) < 200f)
                {
                    return Segment.Peak;
                }
            }

            // 1.5. 检查是否在 The Kiln 熔炉
            if (officialSeg == Segment.TheKiln)
            {
                return Segment.TheKiln;
            }
            Transform kilnRespawn = GetRespawnTheKiln(mh);
            if (kilnRespawn != null && kilnRespawn.position.y > 10f)
            {
                if (Mathf.Abs(pPos.y - kilnRespawn.position.y) < 50f && Vector3.Distance(pPos, kilnRespawn.position) < 150f)
                {
                    return Segment.TheKiln;
                }
            }

            // 2. 检查玩家是否正站在某个营火附近（15m内），营火点燃则已迈入下一区域
            for (int i = 0; i < 4; i++)
            {
                if (IsPlayerNearCampfire(i, 15f))
                {
                    if (IsCampfireLit(i))
                    {
                        return (Segment)(i + 1);
                    }
                    return (Segment)i;
                }
            }

            // 3. 参考 MountainProgressHandler 达到的权威最大里程碑
            if (MountainProgressHandler.Instance != null)
            {
                int maxProgress = MountainProgressHandler.Instance.maxProgressPointReached;
                if (maxProgress > (int)officialSeg && maxProgress <= (int)Segment.Peak)
                {
                    return (Segment)maxProgress;
                }
            }

            return officialSeg;
        }
        catch { }

        return officialSeg;
    }

    public static float GetCurrentPlayerAltitude()
    {
        Character local = Character.localCharacter;
        if (local != null)
        {
            return GetCharacterPosition(local).y;
        }
        return 0f;
    }

    public static List<RouteSegmentInfo> GetAirportPredictedRoute(string biomeId)
    {
        var route = new List<RouteSegmentInfo>(6);
        Segment[] defaultSegments = new Segment[] {
            Segment.Beach, Segment.Tropics, Segment.Alpine, Segment.Caldera, Segment.TheKiln, Segment.Peak
        };

        List<char> chars = new List<char>(6);
        if (!string.IsNullOrEmpty(biomeId))
        {
            for (int k = 0; k < biomeId.Length; k++) chars.Add(biomeId[k]);
        }
        else
        {
            chars.AddRange(new char[] { 'S', 'T', 'A', 'V' });
        }
        if (chars.Count < 5)
        {
            char seg4Char = chars.Count > 3 ? char.ToUpper(chars[3]) : 'V';
            if (seg4Char == 'S' || seg4Char == 'G')
                chars.Add('C');
            else
                chars.Add('K');
        }
        if (chars.Count < 6)
        {
            chars.Add('P');
        }

        for (int i = 0; i < 6; i++)
        {
            string name;
            char c = i < chars.Count ? chars[i] : ' ';
            if (i == 3 && (char.ToUpper(c) == 'S' || char.ToUpper(c) == 'G'))
                name = Localization.T("world.segment_swamp");
            else
                name = DecodeBiomeCharToName(c);

            // 仅前三关（Beach, Tropics, Alpine）默认必然有营火；Caldera 绝大部分无营火；熔炉与山顶绝无营火
            bool hasCamp = (i < 3);

            route.Add(new RouteSegmentInfo
            {
                level = i + 1,
                segment = defaultSegments[i],
                biomeType = (Biome.BiomeType)(-1),
                displayName = name,
                isCurrent = (i == 0),
                hasCampfire = hasCamp,
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
            string displayName = "";
            bool hasCamp = false;

            if (i == 4)
            {
                // Level 5: The Kiln 或 The Citadel (城塞) - 绝无营火，独立重生点
                seg = Segment.TheKiln;
                bt = (Biome.BiomeType)(-1);
                bool isCitadel = IsCitadelActive(mh);
                displayName = isCitadel ? Localization.T("world.segment_citadel") : Localization.T("world.segment_thekiln");
                hasCamp = false;
                if (mapExists && mh.segments != null && mh.segments.Length > 4 && mh.segments[4] != null)
                {
                    MapHandler.MapSegment vSeg;
                    if (TryGetVariantSegment(mh, mh.segments[4], out vSeg) && vSeg != null && vSeg.reconnectSpawnPos != null)
                    {
                        altitude = vSeg.reconnectSpawnPos.position.y;
                    }
                    else
                    {
                        Transform kilnRespawn = GetRespawnTheKiln(mh);
                        if (kilnRespawn != null)
                            altitude = kilnRespawn.position.y;
                        else if (mh.segments[4].reconnectSpawnPos != null)
                            altitude = mh.segments[4].reconnectSpawnPos.position.y;
                    }
                }
            }
            else if (i == 5)
            {
                // Level 6: The Peak (顶峰) - 绝无营火，停机坪安全点
                seg = Segment.Peak;
                bt = Biome.BiomeType.Peak;
                displayName = Localization.T("world.segment_peak");
                hasCamp = false;
                if (mapExists && mh.respawnThePeak != null)
                {
                    altitude = mh.respawnThePeak.position.y;
                }
            }
            else
            {
                // Level 1..4: Beach, Tropics, Alpine, Caldera
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

                        Transform spawnTf = mapSeg.reconnectSpawnPos;
                        MapHandler.MapSegment vSeg;
                        if (TryGetVariantSegment(mh, mapSeg, out vSeg) && vSeg != null && vSeg.reconnectSpawnPos != null)
                        {
                            spawnTf = vSeg.reconnectSpawnPos;
                        }

                        if (spawnTf != null)
                        {
                            altitude = spawnTf.position.y;
                        }
                    }
                }

                // 若为第 4 关沼泽（当天轮换 BiomeID 第 4 位为 'S'）
                if (i == 3 && !string.IsNullOrEmpty(WorldDataCache.todayBiomeID) && WorldDataCache.todayBiomeID.Length > 3 && char.ToUpper(WorldDataCache.todayBiomeID[3]) == 'S')
                {
                    displayName = Localization.T("world.segment_swamp");
                }
                else
                {
                    displayName = GetBiomeDisplayName(bt, seg);
                }

                hasCamp = (i < 3) || (i == 3 && GetSegmentCampfire(3) != null);
            }

            bool isCurrent = mapExists && (currentSeg == seg);
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

    public static bool IsInAirport()
    {
        try
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene != null && !string.IsNullOrEmpty(scene.name) && string.Equals(scene.name, "Airport", StringComparison.OrdinalIgnoreCase))
                return true;
        }
        catch { }

        if (!GameHandler.IsOnIsland)
            return true;

        if (!MapHandler.Exists || MapHandler.Instance == null)
            return true;

        return false;
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

        public static bool hasDeterminedRoute
        {
            get
            {
                return !isInAirport && route != null && route.Count == 6;
            }
        }

        // Custom Map & Playlist Support
        public static bool isCustomScene = false;
        public static string customMapSourceName = "";
        public static string playlistInfo = "";
        public static string pendingAnnouncedScene = "";
        public static int pendingAnnouncedAscent = -1;
        public static float pendingAnnouncedTime = -100f;

        // Playlist & Route Badges Model
        public static List<PlaylistItemInfo> playlistQueue = new List<PlaylistItemInfo>();
        public static int currentPlaylistIndex = 0;
        public static int totalPlaylistCount = 0;
        public static List<RouteNodeBadge> currentMapBadges = new List<RouteNodeBadge>();

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
            isInAirport = IsInAirport();

            var baker = SingletonAsset<MapBaker>.Instance;
            int customMapIdx = -1;
            string customSceneName = "";
            string customBiomeId = "";
            string extPlaylistInfo = "";
            string extSourceName = "";
            List<PlaylistItemInfo> extQueue;
            int extActivePlIdx;
            bool hasCustomMap = TryGetCustomMapOrPlaylist(out customMapIdx, out customSceneName, out customBiomeId, out extPlaylistInfo, out extSourceName, out extQueue, out extActivePlIdx);

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

                if (extQueue != null && extQueue.Count > 0)
                {
                    playlistQueue = extQueue;
                    currentPlaylistIndex = extActivePlIdx;
                    totalPlaylistCount = extQueue.Count;
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
                currentSegDisplayName = (route != null && route.Count > 0) ? route[0].displayName : Localization.T("world.segment_beach");
                nextSegDisplayName = (route != null && route.Count > 1) ? route[1].displayName : "";
                nextLevelNumber = 2;
            }

            // Sync or generate playlistQueue fallback
            if (playlistQueue == null || playlistQueue.Count == 0)
            {
                playlistQueue = new List<PlaylistItemInfo>();
                PlaylistItemInfo single = new PlaylistItemInfo
                {
                    index = 1,
                    mapIndex = todayLevelIndex,
                    sceneName = todaySceneName,
                    displayName = !string.IsNullOrEmpty(todaySceneName) ? todaySceneName : (isCustomScene ? "Custom Island" : "Daily Island"),
                    biomeId = todayBiomeID,
                    formattedRoute = todayBiomeRoute,
                    isCurrent = true,
                    nodes = BuildRouteBadges(todayBiomeID, currentSegment, true, isInAirport, route)
                };
                playlistQueue.Add(single);
                currentPlaylistIndex = 0;
                totalPlaylistCount = 1;
            }
            else
            {
                // Sync current states of playlist queue items
                for (int p = 0; p < playlistQueue.Count; p++)
                {
                    var pItem = playlistQueue[p];
                    bool isCur = (p == currentPlaylistIndex) ||
                        (!string.IsNullOrEmpty(todaySceneName) && todaySceneName.Equals(pItem.sceneName, StringComparison.OrdinalIgnoreCase));
                    pItem.isCurrent = isCur;
                    pItem.nodes = BuildRouteBadges(pItem.biomeId, currentSegment, isCur, isInAirport, isCur ? route : null);
                }
            }

            currentMapBadges = BuildRouteBadges(todayBiomeID, currentSegment, true, isInAirport, route);
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

        // 如果处于第 5 关 TheKiln，下一关必为第 6 关 The Peak（TheKiln 无营火）
        if (curIdx == 4)
        {
            JumpToSegmentStartSafe(Segment.Peak);
            return true;
        }

        // 如果处于第 4 关 Caldera
        if (curIdx == 3)
        {
            // Caldera 变体通常无常规营火，或者营火已点燃/已在营火旁，直接前往第 5 关 TheKiln
            Campfire c3 = GetSegmentCampfire(3);
            if (c3 == null || IsCampfireLit(3) || IsPlayerNearCampfire(3, 12f))
            {
                JumpToSegmentStartSafe(Segment.TheKiln);
                return true;
            }
            return TeleportToCampfire(3);
        }

        // 前三关（Beach, Tropics, Alpine）：
        // 若当前营火已点燃或玩家已在营火旁，前进到下一区域
        if (IsPlayerNearCampfire(curIdx, 12f) || IsCampfireLit(curIdx))
        {
            int nextIdx = curIdx + 1;
            if (nextIdx == 3)
            {
                Campfire c3 = GetSegmentCampfire(3);
                if (c3 != null)
                {
                    JumpToSegmentCampfire(Segment.Caldera);
                }
                else
                {
                    JumpToSegmentStartSafe(Segment.Caldera);
                }
                return true;
            }
            else if (nextIdx == 4)
            {
                JumpToSegmentStartSafe(Segment.TheKiln);
                return true;
            }
            else if (nextIdx >= 5)
            {
                JumpToSegmentStartSafe(Segment.Peak);
                return true;
            }
            else
            {
                JumpToSegmentCampfire((Segment)nextIdx);
                return true;
            }
        }

        // 若当前关卡营火尚未点燃且玩家尚未到达，先传送到当前关卡营火
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

            // 激活当前段落与变体段落
            if (mh.segments != null && segmentIndex >= 0 && segmentIndex < mh.segments.Length)
            {
                var seg = mh.segments[segmentIndex];
                if (seg != null)
                {
                    MapHandler.MapSegment vSeg;
                    if (TryGetVariantSegment(mh, seg, out vSeg) && vSeg != null)
                    {
                        if (vSeg.segmentParent != null && !vSeg.segmentParent.activeSelf)
                            vSeg.segmentParent.SetActive(true);
                        if (vSeg.segmentCampfire != null && !vSeg.segmentCampfire.activeSelf)
                            vSeg.segmentCampfire.SetActive(true);
                        if (vSeg.wallNext != null && !vSeg.wallNext.activeSelf)
                            vSeg.wallNext.SetActive(true);
                        if (vSeg.wallPrevious != null && !vSeg.wallPrevious.activeSelf)
                            vSeg.wallPrevious.SetActive(true);
                    }

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

            Campfire targetCampfire = GetSegmentCampfire(segmentIndex);

            if (targetCampfire == null)
            {
                if (Logger != null)
                    Logger.LogWarning(string.Format("[PEAK AIO] Campfire for segment {0} not found, safely jumping to segment start.", segmentIndex));
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
            Vector3 rawCfPos = cfPos + forward * 1.8f + Vector3.up * 0.2f;
            Vector3 safePos = ResolveSafeGroundPosition(rawCfPos);

            if (localCharacter.photonView != null)
            {
                localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { safePos, true });
            }
            else
            {
                localCharacter.WarpPlayerRPC(safePos, true);
            }

            if (Logger != null)
                Logger.LogInfo(string.Format("[PEAK AIO] Teleported to campfire for segment {0} at {1}", segmentIndex, safePos));

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
        if (segIdx == 3)
        {
            Campfire cf = GetSegmentCampfire(3);
            if (cf == null)
            {
                JumpToSegmentStartSafe(Segment.Caldera);
                return;
            }
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
                            MapHandler.MapSegment vSeg;
                            if (TryGetVariantSegment(mh, seg, out vSeg) && vSeg != null)
                            {
                                if (vSeg.segmentParent != null && !vSeg.segmentParent.activeSelf)
                                    vSeg.segmentParent.SetActive(true);
                                if (vSeg.segmentCampfire != null && !vSeg.segmentCampfire.activeSelf)
                                    vSeg.segmentCampfire.SetActive(true);
                                if (vSeg.wallNext != null && !vSeg.wallNext.activeSelf)
                                    vSeg.wallNext.SetActive(true);
                                if (vSeg.wallPrevious != null && !vSeg.wallPrevious.activeSelf)
                                    vSeg.wallPrevious.SetActive(true);
                            }

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
        if (EventComponent.Instance != null)
        {
            EventComponent.Instance.StartCoroutine(OpenNearbyLuggageRoutine());
        }
        else
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
                        view.SafeRPC("OpenLuggageRPC", RpcTarget.All, true);
                        opened++;
                    }
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Luggage] Requested open for {0} nearby containers.", opened));
            });
        }
    }

    private static System.Collections.IEnumerator OpenNearbyLuggageRoutine()
    {
        int count = Globals.luggageObject.Count;
        int opened = 0;

        for (int i = 0; i < count; i++)
        {
            if (i < Globals.luggageObject.Count)
            {
                var luggage = Globals.luggageObject[i];
                if (luggage != null)
                {
                    var view = luggage.GetComponent<PhotonView>();
                    if (view != null && PhotonNetwork.InRoom)
                    {
                        view.SafeRPC("OpenLuggageRPC", RpcTarget.All, true);
                        opened++;
                    }
                }
            }

            // 每开启 2 个箱子平滑等待 0.12 秒，防止单帧瞬发网络风暴与物理刚体流打满
            if (opened > 0 && opened % 2 == 0)
            {
                yield return new WaitForSeconds(0.12f);
            }
        }

        if (Logger != null)
            Logger.LogInfo(string.Format("[Luggage] Smoothly opened {0} nearby containers.", opened));
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
                    view.SafeRPC("OpenLuggageRPC", RpcTarget.All, true);
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

    /// <summary>
    /// Spawns a creature/monster entity with defensive ground alignment and aggro binding.
    /// </summary>
    public static void SpawnCreature(Globals.CreatureType type, Globals.CreatureSpawnAnchor anchor, float distance, int targetPlayerIndex, float ghostScale = 3.0f)
    {
        // 1.0s spamming cooldown protection
        if (Time.unscaledTime - Globals.lastCreatureSpawnTime < 1.0f)
        {
            Globals.GlobalNotifier.ShowError(Localization.T("creatures.cooldown"), 2.5f);
            return;
        }

        // Host verification for networked RoomObjects
        bool isHost = PhotonNetwork.IsMasterClient;
        if (!isHost && (type == Globals.CreatureType.Scoutmaster || type == Globals.CreatureType.MushroomZombie))
        {
            Globals.GlobalNotifier.ShowError(Localization.T("creatures.host_required"), 4f);
            if (Logger != null)
                Logger.LogWarning("[Creatures] RoomObject creation requires MasterClient authority.");
            return;
        }

        Globals.lastCreatureSpawnTime = Time.unscaledTime;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var localChar = Character.localCharacter;
                if (localChar == null)
                    return;

                Vector3 basePos = localChar.transform.position;
                Vector3 forwardDir = localChar.transform.forward;
                forwardDir.y = 0;
                if (forwardDir.sqrMagnitude > 0.001f) forwardDir.Normalize();
                else forwardDir = Vector3.forward;

                Vector3 desiredPos = basePos + forwardDir * distance;

                if (anchor == Globals.CreatureSpawnAnchor.SelectedPlayer)
                {
                    if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Character.AllCharacters.Count)
                    {
                        var selChar = Character.AllCharacters[Globals.selectedPlayer];
                        if (selChar != null)
                        {
                            basePos = selChar.transform.position;
                            forwardDir = selChar.transform.forward;
                            forwardDir.y = 0;
                            if (forwardDir.sqrMagnitude > 0.001f) forwardDir.Normalize();
                            else forwardDir = Vector3.forward;
                            desiredPos = basePos + forwardDir * distance;
                        }
                    }
                }
                else if (anchor == Globals.CreatureSpawnAnchor.Crosshair)
                {
                    Camera cam = Camera.main;
                    if (cam != null)
                    {
                        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                        RaycastHit aimHit;
                        if (Physics.Raycast(ray, out aimHit, 150f, ~0))
                        {
                            desiredPos = aimHit.point;
                            forwardDir = (desiredPos - basePos);
                            forwardDir.y = 0;
                            if (forwardDir.sqrMagnitude > 0.001f) forwardDir.Normalize();
                            else forwardDir = Vector3.forward;
                        }
                    }
                }

                // Defensive Ground Alignment
                Physics.SyncTransforms();
                Vector3 spawnPos = desiredPos;
                RaycastHit groundHit;
                if (Physics.Raycast(desiredPos + Vector3.up * 25f, Vector3.down, out groundHit, 70f, ~0))
                {
                    spawnPos = groundHit.point + Vector3.up * 0.5f;
                }
                else
                {
                    spawnPos = ResolveSafeGroundPosition(desiredPos);
                    if (spawnPos == desiredPos)
                    {
                        Globals.GlobalNotifier.ShowError(Localization.T("creatures.no_ground"), 4f);
                        if (Logger != null)
                            Logger.LogWarning("[Creatures] No valid ground found to spawn creature.");
                        return;
                    }
                }

                // Determine aggro target character
                Character targetChar = null;
                if (targetPlayerIndex == -1)
                {
                    targetChar = localChar;
                }
                else if (targetPlayerIndex >= 0 && targetPlayerIndex < Character.AllCharacters.Count)
                {
                    targetChar = Character.AllCharacters[targetPlayerIndex];
                }

                Quaternion rotation = Quaternion.LookRotation(-forwardDir);

                // Spawning Pipeline per Creature Type
                switch (type)
                {
                    case Globals.CreatureType.Scoutmaster:
                    {
                        GameObject scoutObj = PhotonNetwork.InstantiateRoomObject("Character_Scoutmaster", spawnPos, rotation, 0, null);
                        if (scoutObj != null)
                        {
                            var character = scoutObj.GetComponent<Character>();
                            if (character != null)
                                character.data.spawnPoint = scoutObj.transform;

                            var sm = scoutObj.GetComponent<Scoutmaster>();
                            if (sm != null && targetChar != null)
                            {
                                try
                                {
                                    var method = typeof(Scoutmaster).GetMethod("SetCurrentTarget", BindingFlags.Instance | BindingFlags.NonPublic);
                                    if (method != null)
                                    {
                                        method.Invoke(sm, new object[] { targetChar, 9999f });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (Logger != null)
                                        Logger.LogError("[Creatures] Scoutmaster aggro set error: " + ex);
                                }
                            }
                        }
                        break;
                    }

                    case Globals.CreatureType.BigGhost:
                    {
                        GameObject ghostObj = PhotonNetwork.Instantiate("PlayerGhost", spawnPos + Vector3.up * 1.5f, rotation, 0, null);
                        if (ghostObj != null)
                        {
                            float s = Mathf.Clamp(ghostScale, 0.5f, 10f);
                            ghostObj.transform.localScale = Vector3.one * s;

                            var pg = ghostObj.GetComponent<PlayerGhost>();
                            if (pg != null && targetChar != null && targetChar.refs != null && targetChar.refs.view != null)
                            {
                                try
                                {
                                    pg.m_view.RPC("RPCA_SetTarget", Photon.Pun.RpcTarget.All, new object[] { targetChar.refs.view });
                                }
                                catch (Exception ex)
                                {
                                    if (Logger != null)
                                        Logger.LogError("[Creatures] PlayerGhost target set error: " + ex);
                                }
                            }
                        }
                        break;
                    }

                    case Globals.CreatureType.MushroomZombie:
                    {
                        GameObject zombieObj = PhotonNetwork.InstantiateRoomObject("MushroomZombie_Player", spawnPos, rotation, 0, null);
                        if (zombieObj != null)
                        {
                            var character = zombieObj.GetComponent<Character>();
                            if (character != null)
                                character.data.spawnPoint = zombieObj.transform;

                            var mz = zombieObj.GetComponent<MushroomZombie>();
                            if (mz != null && targetChar != null)
                            {
                                mz.currentTarget = targetChar;
                                try
                                {
                                    var method = typeof(MushroomZombie).GetMethod("SetCurrentTarget", BindingFlags.Instance | BindingFlags.NonPublic);
                                    if (method != null)
                                    {
                                        method.Invoke(mz, new object[] { targetChar, 9999f });
                                    }
                                }
                                catch { }
                            }
                        }
                        break;
                    }

                    case Globals.CreatureType.Scorpion:
                    {
                        PhotonNetwork.Instantiate("0_Items/Scorpion", spawnPos + Vector3.up * 0.2f, Quaternion.identity, 0, null);
                        break;
                    }

                    case Globals.CreatureType.Beetle:
                    {
                        PhotonNetwork.Instantiate("0_Items/Beetle", spawnPos + Vector3.up * 0.2f, Quaternion.identity, 0, null);
                        break;
                    }

                    case Globals.CreatureType.BeeSwarm:
                    {
                        GameObject hiveObj = PhotonNetwork.Instantiate("0_Items/Beehive", spawnPos + Vector3.up * 0.5f, Quaternion.identity, 0, null);
                        if (hiveObj != null)
                        {
                            var beehive = hiveObj.GetComponent<Beehive>();
                            if (beehive != null && beehive.currentBees != null && beehive.currentBees.photonView != null)
                            {
                                beehive.currentBees.photonView.SafeRPC("SetBeesAngryRPC", Photon.Pun.RpcTarget.All, true);
                            }
                        }
                        break;
                    }
                }

                string typeKey = "creatures.type." + type.ToString().ToLower();
                if (type == Globals.CreatureType.BeeSwarm) typeKey = "creatures.type.bees";
                string typeName = Localization.T(typeKey);
                Globals.GlobalNotifier.ShowError(string.Format(Localization.T("creatures.success"), typeName, distance), 3.5f);

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Creatures] Successfully spawned {0} at {1} (Distance: {2:F1}m)", type, spawnPos, distance));
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError("[Creatures] SpawnCreature exception: " + ex);
                Globals.GlobalNotifier.ShowError("Spawn Creature Failed: " + ex.Message, 4f);
            }
        });
    }

    public static void SpawnScoutmasterForPlayer(int playerIndex)
    {
        SpawnCreature(Globals.CreatureType.Scoutmaster, Globals.CreatureSpawnAnchor.SelectedPlayer, 5f, playerIndex);
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
                dropPos = GetCharacterPosition(Character.localCharacter) + GetCharacterForward(Character.localCharacter) * 1.2f + Vector3.up * 0.2f;
            else if (player != null)
                dropPos = player.transform.position + Vector3.up * 0.5f;
            else
                dropPos = Vector3.zero;

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

                // 1. 如果已有背包先卸下
                if (backpackSlot != null && !backpackSlot.IsEmpty())
                {
                    DropCurrentBackpack(player);
                }

                // 2. 准备背包物品与类型
                Item bpPrefab = customItem != null ? customItem : FindBackpackPrefab(type);
                if (bpPrefab == null)
                {
                    Globals.GlobalNotifier.ShowError(string.Format("未找到对应类型的背包预制体 ({0})", type));
                    return;
                }

                string bpName = "";
                try { if (bpPrefab != null) bpName = bpPrefab.GetName(); } catch { }
                if (string.IsNullOrEmpty(bpName) && bpPrefab != null) bpName = bpPrefab.name;
                if (string.IsNullOrEmpty(bpName)) bpName = type.ToString();

                if (type == BackpackSlot.BackpackType.None && bpPrefab != null)
                {
                    type = GetBackpackTypeForItem(bpPrefab, bpName);
                }

                // 3. 原生权威生成与拾取流程（彻底解决远端玩家 InventorySyncData 丢失 prefab 无法掏出背包的问题）
                if (character.IsLocal)
                {
                    var data = new ItemInstanceData(Guid.NewGuid());
                    ItemInstanceDataHandler.AddInstanceData(data);

                    if (type == BackpackSlot.BackpackType.Jetpack)
                    {
                        var fuel = data.RegisterNewEntry<FloatItemData>(DataEntryKey.Fuel);
                        if (fuel != null) fuel.Value = 100f;
                    }

                    if (backpackSlot != null)
                    {
                        backpackSlot.backpackType = type;
                        backpackSlot.SetItem(bpPrefab, data);
                    }

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
                }
                else
                {
                    // 远端玩家：通过游戏原生权威生成器 InstantiateAndGrabRPC 生成并自动塞入目标4号背包槽
                    if (GameUtils.instance != null)
                    {
                        GameUtils.instance.InstantiateAndGrab(bpPrefab, character, 0);
                    }
                    else if (PhotonNetwork.IsMasterClient)
                    {
                        Vector3 spawnPos = GetCharacterPosition(character) + GetCharacterForward(character) * 0.5f + Vector3.up * 0.2f;
                        var spawnedGo = PhotonNetwork.InstantiateItemRoom(bpPrefab.gameObject.name, spawnPos, Quaternion.identity);
                        if (spawnedGo != null)
                        {
                            var itemComp = spawnedGo.GetComponent<Item>();
                            if (itemComp != null)
                            {
                                itemComp.Interact(character);
                            }
                        }
                    }
                    else
                    {
                        var gu = UnityEngine.Object.FindObjectOfType<GameUtils>();
                        if (gu != null && gu.photonView != null)
                        {
                            gu.photonView.SafeRPC("InstantiateAndGrabRPC", RpcTarget.MasterClient,
                                bpPrefab.gameObject.name,
                                character.transform.position,
                                character.photonView,
                                (byte)0
                            );
                        }
                    }
                }

                if (Logger != null)
                    Logger.LogInfo(string.Format("[Lobby] Equipped {0} ({1}) to Slot 4 for player '{2}'", bpName, type, character.characterName));
                Globals.GlobalNotifier.ShowError(string.Format("已为 '{0}' 装备 4 号背包: {1}", character.characterName, bpName), 3.0f);
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

    // ==========================================
    // STEAM ACHIEVEMENTS SUBSYSTEM (CACHED & SYNCHRONOUS)
    // ==========================================

    public static class AchievementCache
    {
        public static readonly ACHIEVEMENTTYPE[] ValidAchievements;
        private static readonly HashSet<ACHIEVEMENTTYPE> s_UnlockedSet = new HashSet<ACHIEVEMENTTYPE>();
        public static int UnlockedCount { get; private set; }
        public static int TotalCount
        {
            get { return ValidAchievements != null ? ValidAchievements.Length : 0; }
        }
        private static float s_LastRefreshTime = -100f;
        private const float REFRESH_INTERVAL = 1.0f;

        static AchievementCache()
        {
            var all = (ACHIEVEMENTTYPE[])Enum.GetValues(typeof(ACHIEVEMENTTYPE));
            var list = new List<ACHIEVEMENTTYPE>();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != ACHIEVEMENTTYPE.NONE)
                    list.Add(all[i]);
            }
            ValidAchievements = list.ToArray();
        }

        public static bool IsUnlocked(ACHIEVEMENTTYPE type)
        {
            return s_UnlockedSet.Contains(type);
        }

        public static void MarkUnlocked(ACHIEVEMENTTYPE type)
        {
            if (s_UnlockedSet.Add(type))
            {
                UnlockedCount++;
            }
        }

        public static void Invalidate()
        {
            s_LastRefreshTime = -100f;
        }

        public static void EnsureUpdated(bool force = false)
        {
            float now = Time.unscaledTime;
            if (!force && (now - s_LastRefreshTime < REFRESH_INTERVAL))
                return;

            s_LastRefreshTime = now;
            var instance = Singleton<AchievementManager>.Instance;
            if (instance == null) return;

            s_UnlockedSet.Clear();
            int count = 0;
            for (int i = 0; i < ValidAchievements.Length; i++)
            {
                var ach = ValidAchievements[i];
                try
                {
                    if (instance.IsAchievementUnlocked(ach))
                    {
                        s_UnlockedSet.Add(ach);
                        count++;
                    }
                }
                catch { }
            }
            UnlockedCount = count;
        }
    }

    public static bool IsAchievementUnlocked(ACHIEVEMENTTYPE type)
    {
        if (type == ACHIEVEMENTTYPE.NONE) return false;
        return AchievementCache.IsUnlocked(type);
    }

    public static bool UnlockAchievement(ACHIEVEMENTTYPE type, bool showNotify = true)
    {
        try
        {
            if (type == ACHIEVEMENTTYPE.NONE) return false;
            var instance = Singleton<AchievementManager>.Instance;
            if (instance == null)
            {
                if (showNotify)
                    Globals.GlobalNotifier.ShowError(Localization.T("achievements.not_in_game"));
                return false;
            }

            var method = ConstantFields.GetThrowAchievementMethod();
            if (method != null)
            {
                method.Invoke(instance, new object[] { type });
                AchievementCache.MarkUnlocked(type);
                if (showNotify)
                {
                    string name = Localization.GetAchievementName(type);
                    Globals.GlobalNotifier.ShowSuccess(Localization.T("achievements.unlock_success", name));
                }
                return true;
            }
            else
            {
                if (Logger != null)
                    Logger.LogWarning("[Achievements] ThrowAchievement MethodInfo not found.");
            }
        }
        catch (Exception ex)
        {
            if (Logger != null)
                Logger.LogError(string.Format("[Achievements] Failed to unlock {0}: {1}", type, ex));
            if (showNotify)
                Globals.GlobalNotifier.ShowError(string.Format("Failed to unlock {0}: {1}", type, ex.Message));
        }
        return false;
    }

    public static void UnlockAllAchievementsSync()
    {
        try
        {
            var instance = Singleton<AchievementManager>.Instance;
            if (instance == null)
            {
                Globals.GlobalNotifier.ShowError(Localization.T("achievements.not_in_game"));
                return;
            }

            // Execute game native synchronous batch unlock
            instance.DebugGetAllAchievements();

            // Refresh cached statuses immediately
            AchievementCache.EnsureUpdated(true);

            Globals.GlobalNotifier.ShowSuccess(Localization.T("achievements.unlock_all_success", AchievementCache.TotalCount));
        }
        catch (Exception ex)
        {
            if (Logger != null)
                Logger.LogError("[Achievements] Error in UnlockAllAchievementsSync: " + ex);
            Globals.GlobalNotifier.ShowError("Unlock All Error: " + ex.Message);
        }
    }
}
