using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

[HarmonyPatch(typeof(PointPinger), "ReceivePoint_Rpc")]
public class PointPingPatch
{
    static void Postfix(Vector3 point, Vector3 hitNormal, PointPinger __instance)
    {
        try
        {
            if (!ConfigManager.TeleportToPing.Value)
                return;

            Photon.Realtime.Player owner = null;
            if (__instance.character != null && __instance.character.photonView != null)
            {
                owner = __instance.character.photonView.Owner;
            }

            if (owner != null && owner == PhotonNetwork.LocalPlayer)
            {
                if (Character.localCharacter != null && !Character.localCharacter.data.dead)
                {
                    Vector3 safePoint = Utilities.ResolveSafeGroundPosition(point);

                    Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                        safePoint, true
                    });

                    ConfigManager.Logger.LogInfo("[Patch] Teleported to ping!");
                }
            }
        }
        catch (Exception ex)
        {
            ConfigManager.Logger.LogError("[Patch] Exception: " + ex);
        }
    }
}

[HarmonyPatch(typeof(Character), "Update")]
public class FlyPatch
{
    private static bool isFlying = false;
    private static Vector3 flyVelocity = Vector3.zero;

    public static void SetFlying(bool enable)
    {
        isFlying = enable;
        flyVelocity = Vector3.zero;

        ConfigManager.Logger.LogInfo(string.Format("[FlyMod] Flight {0}.", enable ? "enabled" : "disabled"));
    }

    public static bool IsFlying
    {
        get { return isFlying; }
    }

    static void Postfix(Character __instance)
    {
        if (!ConfigManager.FlyMod.Value && !isFlying)
            return;

        if (!__instance.IsLocal)
            return;

        if (!ConfigManager.FlyMod.Value)
        {
            if (isFlying)
            {
                isFlying = false;
                flyVelocity = Vector3.zero;
                ConfigManager.Logger.LogInfo("[FlyMod] Flight disabled.");
            }
            return;
        }

        if (!isFlying)
        {
            isFlying = true;
            ConfigManager.Logger.LogInfo("[FlyMod] Flight enabled.");
        }

        __instance.data.isGrounded = true;
        __instance.data.sinceGrounded = 0f;
        __instance.data.sinceJump = 0f;

        Vector3 input = __instance.input.movementInput;
        Vector3 forward = __instance.data.lookDirection_Flat.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 moveVec = forward * input.y + right * input.x;

        if (__instance.input.jumpIsPressed)
            moveVec += Vector3.up;

        if (__instance.input.crouchIsPressed)
            moveVec += Vector3.down;

        float speed = ConfigManager.FlySpeed.Value;
        float accel = ConfigManager.FlyAcceleration.Value;

        flyVelocity = Vector3.Lerp(flyVelocity, moveVec.normalized * speed, Time.deltaTime * accel);

        var partList = __instance.refs.ragdoll.partList;
        for (int i = 0; i < partList.Count; i++)
        {
            if (partList[i] != null && partList[i].Rig != null)
            {
                partList[i].Rig.linearVelocity = flyVelocity;
            }
        }
    }
}

[HarmonyPatch(typeof(CharacterAfflictions), "UpdateWeight")]
public class Patch_UpdateWeight
{
    private static CharacterAfflictions cachedLocalAfflictions;
    private static Character cachedLocalCharacter;

    static void Postfix(CharacterAfflictions __instance)
    {
        if (!ConfigManager.NoWeight.Value)
            return;

        var localChar = Character.localCharacter;
        if (ReferenceEquals(localChar, null))
            return;

        if (!ReferenceEquals(localChar, cachedLocalCharacter))
        {
            cachedLocalCharacter = localChar;
            cachedLocalAfflictions = (localChar.refs != null) ? localChar.refs.afflictions : localChar.GetComponent<CharacterAfflictions>();
        }

        if (ReferenceEquals(__instance, cachedLocalAfflictions))
        {
            __instance.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, 0f, false);
        }
    }
}

[HarmonyPatch(typeof(Character), "CanDoInput")]
public class Patch_Character_CanDoInput
{
    static bool Prefix(ref bool __result)
    {
        if (PeakMod.IsMenuOpen)
        {
            __result = false;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CharacterMovement), "CanMoveCamera")]
public class Patch_CharacterMovement_CanMoveCamera
{
    static bool Prefix(ref bool __result)
    {
        if (PeakMod.IsMenuOpen)
        {
            __result = false;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CursorHandler), "Update")]
public class Patch_CursorHandler_Update
{
    static void Postfix()
    {
        if (PeakMod.IsMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

[HarmonyPatch(typeof(Character), "RPCA_Die")]
public class Patch_CharacterDie
{
    static void Prefix(Character __instance)
    {
        try
        {
            if (__instance != null)
            {
                Utilities.CaptureInventorySnapshot(__instance);

                if (__instance.photonView != null)
                {
                    int viewId = __instance.photonView.ViewID;
                    // 在尸体被传送到 (0, 5000, -5000) 之前，捕获真实世界坐标
                    Vector3 rawDeathPos = Utilities.GetCharacterPosition(__instance);
                    if (rawDeathPos.y > 4000f)
                    {
                        rawDeathPos = __instance.LastLivingPosition;
                    }

                    // 贴地安全检测，防止死在虚空深渊中
                    Vector3 safeGround = Utilities.ResolveSafeGroundPosition(rawDeathPos);
                    if (safeGround.y < -30f || float.IsNaN(safeGround.y))
                    {
                        Globals.PlayerLocationSnapshot snap;
                        if (Globals.playerSafeLocations.TryGetValue(viewId, out snap))
                            safeGround = snap.safePosition;
                        else
                            safeGround = __instance.LastLivingPosition;
                    }

                    Globals.playerDeathLocations[viewId] = safeGround;
                    if (ConfigManager.Logger != null)
                        ConfigManager.Logger.LogInfo(string.Format("[Patch_CharacterDie] Recorded death pos for {0} (ViewID {1}): {2}", __instance.characterName, viewId, safeGround));
                }
            }
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogError("[Patch_CharacterDie] Error: " + ex);
        }
    }
}

[HarmonyPatch(typeof(AirportCheckInKiosk), "BeginIslandLoadRPC")]
public class Patch_AirportCheckInKiosk_BeginIslandLoadRPC
{
    static void Prefix(string sceneName, int ascent)
    {
        try
        {
            Utilities.WorldDataCache.OnBeginIslandLoadAnnounced(sceneName, ascent);
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogError("[Patch_AirportCheckInKiosk_BeginIslandLoadRPC] Error: " + ex);
        }
    }
}

[HarmonyPatch(typeof(MapHandler), "JumpToSegmentLogic")]
public class Patch_MapHandler_JumpToSegmentLogic
{
    private static readonly System.Reflection.FieldInfo s_currentSegmentField = AccessTools.Field(typeof(MapHandler), "currentSegment");
    private static readonly System.Reflection.FieldInfo s_lastRevivedSegmentField = AccessTools.Field(typeof(MapHandler), "_lastRevivedSegment");

    static bool Prefix(Segment segment, HashSet<int> playersToTeleport, bool sendToEveryone, bool updateFog)
    {
        try
        {
            var mh = Singleton<MapHandler>.Instance;
            if (mh == null || mh.segments == null || mh.segments.Length == 0)
                return true;

            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogInfo(string.Format("[Harmony] Safeguarded JumpToSegmentLogic: {0}", segment));

            // 1. 安全禁用所有段落的基础与变体对象
            for (int i = 0; i < mh.segments.Length; i++)
            {
                var mapSeg = mh.segments[i];
                if (mapSeg == null) continue;

                if (mapSeg.segmentParent != null)
                    mapSeg.segmentParent.SetActive(false);
                if (mapSeg.segmentCampfire != null)
                    mapSeg.segmentCampfire.SetActive(false);
                if (mapSeg.wallNext != null)
                    mapSeg.wallNext.gameObject.SetActive(false);
                if (mapSeg.wallPrevious != null)
                    mapSeg.wallPrevious.gameObject.SetActive(false);

                MapHandler.MapSegment vSeg;
                if (Utilities.TryGetVariantSegment(mh, mapSeg, out vSeg) && vSeg != null)
                {
                    if (vSeg.segmentParent != null)
                        vSeg.segmentParent.SetActive(false);
                    if (vSeg.segmentCampfire != null)
                        vSeg.segmentCampfire.SetActive(false);
                    if (vSeg.wallNext != null)
                        vSeg.wallNext.gameObject.SetActive(false);
                    if (vSeg.wallPrevious != null)
                        vSeg.wallPrevious.gameObject.SetActive(false);
                }
            }

            // 2. 更新 MapHandler 当前段落号
            if (s_currentSegmentField != null)
            {
                s_currentSegmentField.SetValue(mh, (int)segment);
            }

            // 3. 计算主段落索引 num
            int num = (segment == Segment.Peak) ? Mathf.Min(4, mh.segments.Length - 1) : Mathf.Clamp((int)segment, 0, mh.segments.Length - 1);
            var activeSeg = mh.segments[num];

            // 4. 激活当前段落与变体段落
            if (activeSeg != null)
            {
                MapHandler.MapSegment vSeg;
                if (Utilities.TryGetVariantSegment(mh, activeSeg, out vSeg) && vSeg != null)
                {
                    if (vSeg.segmentParent != null && !vSeg.segmentParent.activeSelf)
                        vSeg.segmentParent.SetActive(true);
                    if (vSeg.segmentCampfire != null && !vSeg.segmentCampfire.activeSelf)
                        vSeg.segmentCampfire.SetActive(true);
                    if (vSeg.wallNext != null && !vSeg.wallNext.activeSelf)
                        vSeg.wallNext.SetActive(true);
                    if (vSeg.wallPrevious != null && !vSeg.wallPrevious.activeSelf)
                        vSeg.wallPrevious.SetActive(true);

                    if (vSeg.reconnectSpawnPos != null)
                    {
                        activeSeg.reconnectSpawnPos = vSeg.reconnectSpawnPos;
                    }
                }

                if (activeSeg.segmentParent != null && !activeSeg.segmentParent.activeSelf)
                    activeSeg.segmentParent.SetActive(true);
                if (activeSeg.segmentCampfire != null && !activeSeg.segmentCampfire.activeSelf)
                    activeSeg.segmentCampfire.SetActive(true);
                if (activeSeg.wallNext != null && !activeSeg.wallNext.activeSelf)
                    activeSeg.wallNext.SetActive(true);
                if (activeSeg.wallPrevious != null && !activeSeg.wallPrevious.activeSelf)
                    activeSeg.wallPrevious.SetActive(true);
            }

            // 特殊关卡激活支持：TheKiln 或 The Citadel (城塞)
            if (segment == Segment.TheKiln)
            {
                if (mh.segments.Length > 4 && mh.segments[4] != null)
                {
                    var kSeg = mh.segments[4];
                    MapHandler.MapSegment vSeg;
                    if (Utilities.TryGetVariantSegment(mh, kSeg, out vSeg) && vSeg != null)
                    {
                        if (vSeg.segmentParent != null && !vSeg.segmentParent.activeSelf)
                            vSeg.segmentParent.SetActive(true);
                        if (vSeg.segmentCampfire != null && !vSeg.segmentCampfire.activeSelf)
                            vSeg.segmentCampfire.SetActive(true);
                        if (vSeg.wallNext != null && !vSeg.wallNext.activeSelf)
                            vSeg.wallNext.SetActive(true);
                        if (vSeg.wallPrevious != null && !vSeg.wallPrevious.activeSelf)
                            vSeg.wallPrevious.SetActive(true);
                        if (vSeg.reconnectSpawnPos != null)
                            kSeg.reconnectSpawnPos = vSeg.reconnectSpawnPos;
                    }

                    if (kSeg.segmentParent != null && !kSeg.segmentParent.activeSelf)
                        kSeg.segmentParent.SetActive(true);
                    if (kSeg.reconnectSpawnPos == null)
                    {
                        Transform kilnTf = Utilities.GetRespawnTheKiln(mh);
                        if (kilnTf != null) kSeg.reconnectSpawnPos = kilnTf;
                    }
                }
                // 关键保活：保持 Caldera / 雾沼(段落3) 地表与相连通道激活，防止玩家从交界缝隙跌入虚空
                if (mh.segments.Length > 3 && mh.segments[3] != null)
                {
                    var cSeg = mh.segments[3];
                    MapHandler.MapSegment vSeg3;
                    if (Utilities.TryGetVariantSegment(mh, cSeg, out vSeg3) && vSeg3 != null)
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
            }

            // 特殊关卡激活支持：Peak
            if (segment == Segment.Peak)
            {
                if (mh.segments.Length > 4 && mh.segments[4] != null && mh.segments[4].segmentParent != null)
                {
                    mh.segments[4].segmentParent.SetActive(true);
                }
                if (Singleton<PeakHandler>.Instance != null)
                {
                    if (!Singleton<PeakHandler>.Instance.gameObject.activeSelf)
                        Singleton<PeakHandler>.Instance.gameObject.SetActive(true);
                    // 注意：绝不激活 peakSequence 避免提前触发直升机救援序列
                }
            }

            // 激活上一段落营火（官方逻辑）
            if (num > 0 && mh.segments != null && num - 1 < mh.segments.Length)
            {
                var prevSeg = mh.segments[num - 1];
                if (prevSeg != null && prevSeg.segmentCampfire != null)
                {
                    prevSeg.segmentCampfire.SetActive(true);
                }
            }

            // 关键：强制刷新 Unity PhysX BVH 结构，确保刚激活的切片碰撞体即刻对射线生效
            Physics.SyncTransforms();

            // 5. 获取经过 100% 安全校验的绝对坐标（绝不飞天，绝不掉入虚空）
            Vector3 safeVector;
            if (!Utilities.TryGetSegmentSpawnPosition(segment, out safeVector))
            {
                Transform kilnTf = Utilities.GetRespawnTheKiln(mh);
                if (segment == Segment.TheKiln && kilnTf != null)
                    safeVector = Utilities.ResolveSafeGroundPosition(kilnTf.position);
                else if (segment == Segment.Peak && mh.respawnThePeak != null)
                    safeVector = Utilities.ResolveSafeGroundPosition(mh.respawnThePeak.position);
                else if (activeSeg != null && activeSeg.reconnectSpawnPos != null)
                    safeVector = Utilities.ResolveSafeGroundPosition(activeSeg.reconnectSpawnPos.position);
                else if (Character.localCharacter != null)
                    safeVector = Utilities.ResolveSafeGroundPosition(Utilities.GetCharacterPosition(Character.localCharacter));
                else
                    safeVector = (activeSeg != null && activeSeg.segmentParent != null) ? Utilities.ResolveSafeGroundPosition(activeSeg.segmentParent.transform.position) : Vector3.zero;

                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogWarning(string.Format("[Harmony] TryGetSegmentSpawnPosition fallback for {0}: {1}", segment, safeVector));
            }

            // 6. MasterClient 触发物品 Spawner
            if (PhotonNetwork.IsMasterClient && activeSeg != null)
            {
                if (activeSeg.segmentParent != null)
                {
                    ISpawner[] spawners = activeSeg.segmentParent.GetComponentsInChildren<ISpawner>(true);
                    for (int i = 0; i < spawners.Length; i++)
                    {
                        try { spawners[i].TrySpawnItems(); } catch { }
                    }
                }
                if (activeSeg.segmentCampfire != null)
                {
                    ISpawner[] spawners = activeSeg.segmentCampfire.GetComponentsInChildren<ISpawner>(true);
                    for (int i = 0; i < spawners.Length; i++)
                    {
                        try { spawners[i].TrySpawnItems(); } catch { }
                    }
                }
            }

            // 7. 迷雾与日夜管理
            if (updateFog && Singleton<OrbFogHandler>.Instance != null)
            {
                try { Singleton<OrbFogHandler>.Instance.SetFogOrigin(num); } catch { }
            }
            if (activeSeg != null && activeSeg.dayNightProfile != null && DayNightManager.instance != null)
            {
                try { DayNightManager.instance.BlendProfiles(activeSeg.dayNightProfile); } catch { }
            }

            // 8. 传送玩家（使用安全的 safeVector 坐标）
            if (PhotonNetwork.IsMasterClient && playersToTeleport != null)
            {
                foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
                {
                    if (character != null && character.photonView != null && character.photonView.Owner != null)
                    {
                        if (playersToTeleport.Contains(character.photonView.Owner.ActorNumber))
                        {
                            character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] { safeVector, true });
                        }
                    }
                }
            }

            // 9. 广播网络调试包（使用安全反射调用，解耦缺失的程序集强引用）
            if (sendToEveryone)
            {
                try
                {
                    Type pkgType = Type.GetType("SyncMapHandlerDebugCommandPackage, Assembly-CSharp");
                    if (pkgType != null)
                    {
                        object pkg = Activator.CreateInstance(pkgType, new object[] { segment, new int[0] });
                        Type cmdType = Type.GetType("CustomCommands`1, Assembly-CSharp");
                        Type cmdTypeEnum = Type.GetType("CustomCommandType, Assembly-CSharp");
                        if (cmdType != null && cmdTypeEnum != null)
                        {
                            Type genericCmd = cmdType.MakeGenericType(cmdTypeEnum);
                            System.Reflection.MethodInfo sendMethod = genericCmd.GetMethod("SendPackage", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            if (sendMethod != null)
                            {
                                sendMethod.Invoke(null, new object[] { pkg, (byte)0 });
                            }
                        }
                    }
                }
                catch { }
            }

            // 10. 复活石雕像同步
            try
            {
                if (MapHandler.CurrentScoutStatue != null)
                {
                    MapHandler.CurrentScoutStatue.SegmentNumber = MapHandler.CurrentSegmentNumber;
                    MapHandler.CurrentScoutStatue.ReviveUsed += delegate(RespawnChest statue)
                    {
                        var inst = Singleton<MapHandler>.Instance;
                        if (inst != null && s_lastRevivedSegmentField != null)
                            s_lastRevivedSegmentField.SetValue(inst, (int)statue.SegmentNumber);
                    };
                }
            }
            catch { }

            // 11. 刷新 Mod 数据缓存
            Utilities.WorldDataCache.Invalidate();

            return false; // 拦截官方充满 Bug 的实现
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogError("[Patch_MapHandler_JumpToSegmentLogic] Exception: " + ex);
            return true; // 异常时回退给官方
        }
    }
}

// ==========================================
// Blowgun Dart Ammunition Enchantment Patches
// ==========================================
[HarmonyPatch(typeof(Action_RaycastDart), "FireDart")]
public class Patch_Action_RaycastDart_FireDart
{
    static void Prefix(Action_RaycastDart __instance)
    {
        try
        {
            if (!Globals.dartAmmoEnabled)
                return;

            var ammoType = Globals.selectedDartAmmoType;
            if (ammoType == Globals.DartAmmoType.Chaos)
            {
                int count = Enum.GetValues(typeof(Globals.DartAmmoType)).Length - 1;
                ammoType = (Globals.DartAmmoType)UnityEngine.Random.Range(0, count);
            }

            var affList = new List<Peak.Afflictions.Affliction>();
            switch (ammoType)
            {
                case Globals.DartAmmoType.Invincibility:
                    affList.Add(new Peak.Afflictions.Affliction_Invincibility());
                    break;
                case Globals.DartAmmoType.SpeedBoost:
                    affList.Add(new Peak.Afflictions.Affliction_FasterBoi());
                    break;
                case Globals.DartAmmoType.InfiniteStamina:
                    affList.Add(new Peak.Afflictions.Affliction_InfiniteStamina(60f));
                    break;
                case Globals.DartAmmoType.FullCleanse:
                    affList.Add(new Peak.Afflictions.Affliction_ClearAllStatus());
                    break;
                case Globals.DartAmmoType.LowGravity:
                    var lowGrav = new Peak.Afflictions.Affliction_LowGravity();
                    lowGrav.lowGravAmount = 3;
                    affList.Add(lowGrav);
                    break;
                case Globals.DartAmmoType.Glow:
                    affList.Add(new Peak.Afflictions.Affliction_Glowing());
                    break;
                case Globals.DartAmmoType.Poison:
                    var poison = new Peak.Afflictions.Affliction_PoisonOverTime(0f, 10f, 30f);
                    affList.Add(poison);
                    break;
                case Globals.DartAmmoType.Starvation:
                    var hunger = new Peak.Afflictions.Affliction_AdjustStatus();
                    hunger.statusType = CharacterAfflictions.STATUSTYPE.Hunger;
                    hunger.statusAmount = 60f;
                    affList.Add(hunger);
                    break;
                case Globals.DartAmmoType.Sleep:
                    var drowsy = new Peak.Afflictions.Affliction_AdjustDrowsyOverTime();
                    drowsy.statusPerSecond = 20f;
                    affList.Add(drowsy);
                    break;
                case Globals.DartAmmoType.Thorns:
                    var thorns = new Peak.Afflictions.Affliction_AdjustStatus();
                    thorns.statusType = CharacterAfflictions.STATUSTYPE.Thorns;
                    thorns.statusAmount = 50f;
                    affList.Add(thorns);
                    break;
                case Globals.DartAmmoType.Spores:
                    var spores = new Peak.Afflictions.Affliction_AdjustStatus();
                    spores.statusType = CharacterAfflictions.STATUSTYPE.Spores;
                    spores.statusAmount = 50f;
                    affList.Add(spores);
                    break;
                case Globals.DartAmmoType.Blind:
                    affList.Add(new Peak.Afflictions.Affliction_Blind());
                    break;
                case Globals.DartAmmoType.Numb:
                    affList.Add(new Peak.Afflictions.Affliction_Numb());
                    break;
                default:
                    break;
            }

            if (affList.Count > 0)
            {
                __instance.afflictionsOnHit = affList.ToArray();
            }
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogError("[Patch_Action_RaycastDart_FireDart] Exception: " + ex);
        }
    }
}

[HarmonyPatch(typeof(Action_RaycastDart), "DartImpact")]
public class Patch_Action_RaycastDart_DartImpact
{
    static void Postfix(Character hitCharacter, Vector3 origin, Vector3 endpoint)
    {
        try
        {
            if (!Globals.dartAmmoEnabled || hitCharacter == null || hitCharacter.photonView == null)
                return;

            var ammoType = Globals.selectedDartAmmoType;
            if (ammoType == Globals.DartAmmoType.Chaos)
            {
                int count = Enum.GetValues(typeof(Globals.DartAmmoType)).Length - 1;
                ammoType = (Globals.DartAmmoType)UnityEngine.Random.Range(0, count);
            }

            if (!hitCharacter.photonView.IsMine)
            {
                switch (ammoType)
                {
                    case Globals.DartAmmoType.TripFall:
                        hitCharacter.photonView.RPC("RPCA_Fall", RpcTarget.All, new object[] { 3.5f });
                        break;

                    case Globals.DartAmmoType.Revive:
                        hitCharacter.photonView.RPC("RPCA_Revive", RpcTarget.All, new object[] { false });
                        break;

                    case Globals.DartAmmoType.SpeedBoost:
                    case Globals.DartAmmoType.InfiniteStamina:
                        hitCharacter.photonView.RPC("MoraleBoost", RpcTarget.All, new object[] { 100f, 1 });
                        break;

                    case Globals.DartAmmoType.Poison:
                        hitCharacter.photonView.RPC("RPCA_Stick", RpcTarget.All, new object[] {
                            BodypartType.Torso, endpoint, endpoint, CharacterAfflictions.STATUSTYPE.Poison, 50f
                        });
                        break;

                    case Globals.DartAmmoType.Starvation:
                        hitCharacter.photonView.RPC("RPCA_Stick", RpcTarget.All, new object[] {
                            BodypartType.Torso, endpoint, endpoint, CharacterAfflictions.STATUSTYPE.Hunger, 50f
                        });
                        break;

                    case Globals.DartAmmoType.Sleep:
                        hitCharacter.photonView.RPC("RPCA_Stick", RpcTarget.All, new object[] {
                            BodypartType.Torso, endpoint, endpoint, CharacterAfflictions.STATUSTYPE.Drowsy, 50f
                        });
                        break;

                    case Globals.DartAmmoType.Thorns:
                        hitCharacter.photonView.RPC("RPCA_Stick", RpcTarget.All, new object[] {
                            BodypartType.Torso, endpoint, endpoint, CharacterAfflictions.STATUSTYPE.Thorns, 50f
                        });
                        break;

                    case Globals.DartAmmoType.Spores:
                        hitCharacter.photonView.RPC("RPCA_Stick", RpcTarget.All, new object[] {
                            BodypartType.Torso, endpoint, endpoint, CharacterAfflictions.STATUSTYPE.Spores, 50f
                        });
                        break;
                }
            }
            else
            {
                if (ammoType == Globals.DartAmmoType.TripFall)
                {
                    hitCharacter.photonView.RPC("RPCA_Fall", RpcTarget.All, new object[] { 3.5f });
                }
                else if (ammoType == Globals.DartAmmoType.Revive)
                {
                    hitCharacter.photonView.RPC("RPCA_Revive", RpcTarget.All, new object[] { false });
                }
            }
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogError("[Patch_Action_RaycastDart_DartImpact] Exception: " + ex);
        }
    }
}

// ==========================================
// Food Poison Immunity Patches
// ==========================================
[HarmonyPatch(typeof(Action_InflictPoison), "RunAction")]
public class Patch_Action_InflictPoison
{
    static bool Prefix()
    {
        if (Globals.foodPoisonImmunity)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogInfo("[FoodPoisonImmunity] Blocked poison infliction.");
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CookingBehavior_AddPoisonOnUse), "TriggerBehaviour")]
public class Patch_CookingBehavior_AddPoisonOnUse
{
    static bool Prefix()
    {
        if (Globals.foodPoisonImmunity)
        {
            return false;
        }
        return true;
    }
}

// ==========================================
// Anti-Host Malicious Kick Patch
// ==========================================
[HarmonyPatch(typeof(PlayerHandler), "Kick")]
public class Patch_PlayerHandler_Kick
{
    static bool Prefix(int actorNumber)
    {
        try
        {
            if (!Globals.enableAntiKick) return true;

            if (PhotonNetwork.LocalPlayer != null && actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                if (ConfigManager.Logger != null)
                    ConfigManager.Logger.LogWarning(string.Format("[AntiKick] Intercepted kick command targeting local player (ActorNumber={0})!", actorNumber));

                Globals.GlobalNotifier.ShowError(Localization.T("network.antikick_toast"), 4.5f);
                return false; // 阻断踢人逻辑执行
            }
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
                ConfigManager.Logger.LogError("[AntiKick] Error in Kick patch: " + ex);
        }
        return true;
    }
}

[HarmonyPatch(typeof(PlayerHandler), "KickRoutine")]
public class Patch_PlayerHandler_KickRoutine
{
    static bool Prefix(int actorNumber)
    {
        if (Globals.enableAntiKick && PhotonNetwork.LocalPlayer != null && actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return false;
        }
        return true;
    }
}



