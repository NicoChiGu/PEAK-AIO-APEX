# 《PEAK》Mod 网络开发实战指南与 Harmony 补丁模板

本文档专为《PEAK》Mod 开发者编写，结合前述网络通信协议，提供基于 **BepInEx 5 / Harmony** 的通用网络 Hook 模板、RPC 伪造与状态拦截实战代码。

---

## 一、基础开发环境准备

推荐工程配置：
- **目标框架**: .NET Framework 4.7.2 或 .NET Standard 2.0 / 2.1
- **核心依赖**:
  - `0Harmony.dll` (Harmony 2.x)
  - `BepInEx.Core.dll`
  - `Assembly-CSharp.dll` (引用 `./dll/20260915-Assembly-CSharp.dll`)
  - `PhotonUnityNetworking.dll` & `PhotonRealtime.dll`
  - `UnityEngine.CoreModule.dll`

---

## 二、实用实战代码模板

### 1. 无敌免死与免昏厥 (God Mode & Anti-PassOut)

通过 Harmony 前置补丁（Prefix）阻断本地角色向网络广播昏迷与死亡 RPC：

```csharp
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace PeakMod.Cheats
{
    [HarmonyPatch(typeof(Character))]
    public static class GodModePatch
    {
        // 拦截昏迷倒地
        [HarmonyPatch("RPCA_PassOut")]
        [HarmonyPrefix]
        public static bool Prefix_PassOut(Character __instance)
        {
            if (__instance.photonView.IsMine)
            {
                Debug.Log("[PeakMod] 已阻断本地角色昏厥事件广播！");
                return false; // 阻止原版逻辑执行与 RPC 发送
            }
            return true;
        }

        // 拦截失足坠落硬直
        [HarmonyPatch("RPCA_Fall")]
        [HarmonyPrefix]
        public static bool Prefix_Fall(Character __instance)
        {
            if (__instance.photonView.IsMine)
            {
                return false; // 免疫滑落与失控状态
            }
            return true;
        }

        // 拦截彻底死亡与掉落装备
        [HarmonyPatch("RPCA_Die")]
        [HarmonyPrefix]
        public static bool Prefix_Die(Character __instance)
        {
            if (__instance.photonView.IsMine)
            {
                Debug.Log("[PeakMod] 濒死保护生效：就地原地满血复活！");
                // 调用原生 RPC 免费复活自身
                __instance.photonView.RPC("RPCA_Revive", RpcTarget.All, false);
                return false;
            }
            return true;
        }
    }
}
```

---

### 2. 全图定点瞬间传送 (Instant Teleport)

通过封装原生 `WarpPlayerRPC` 实现无限制瞬间移动：

```csharp
using Photon.Pun;
using UnityEngine;

namespace PeakMod.Utilities
{
    public static class TeleportHelper
    {
        /// <summary>
        /// 将本地角色瞬间传送到目标世界坐标
        /// </summary>
        /// <param name="targetPosition">目标三维位置</param>
        /// <param name="showPoofEffect">是否产生烟雾粒子</param>
        public static void TeleportTo(Vector3 targetPosition, bool showPoofEffect = true)
        {
            if (Character.localCharacter != null && Character.localCharacter.photonView != null)
            {
                // RpcTarget.All 为 0
                Character.localCharacter.photonView.RPC(
                    "WarpPlayerRPC", 
                    RpcTarget.All, 
                    targetPosition, 
                    showPoofEffect
                );
            }
        }

        /// <summary>
        /// 瞬间传送至指定队友身边
        /// </summary>
        public static void TeleportToTeammate(Character targetTeammate)
        {
            if (targetTeammate != null)
            {
                TeleportTo(targetTeammate.transform.position + Vector3.up * 0.5f);
            }
        }
    }
}
```

---

### 3. 防房主恶意踢出 (Anti-Kick Patch)

游戏踢人依赖 `PhotonShim.RaiseGenericEvent` 发送 `EventCode = 18`。在客户端拦截此事件即可实现“防踢”：

```csharp
using HarmonyLib;
using Peak.Network;
using Photon.Pun;
using UnityEngine;

namespace PeakMod.Security
{
    [HarmonyPatch(typeof(PlayerHandler), "OnNetworkEvent")]
    public static class AntiKickPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(INetworkEventData eventData)
        {
            // EventCode 18 代表 KickPlayer 指令
            if (eventData.EventCode == 18)
            {
                string kickedUserId = eventData.CustomData as string;
                string localUserId = PhotonNetwork.LocalPlayer.UserId;

                if (kickedUserId == localUserId)
                {
                    Debug.LogWarning("[PeakMod] 检测到房主尝试踢出本地玩家，已强制拦截！");
                    return false; // 阻断踢人逻辑执行
                }
            }
            return true;
        }
    }
}
```

---

### 4. 远程物品生成与隔空拾取 (Item Spawner & Force Pickup)

利用 `GameUtils` 的主机权威 RPC 远程生成任意装备并直接塞入手持槽：

```csharp
using Photon.Pun;
using UnityEngine;

namespace PeakMod.Items
{
    public static class ItemHelper
    {
        /// <summary>
        /// 远程召唤指定物品预制体并自动放入背包/手中
        /// </summary>
        /// <param name="prefabResourcePath">Resources 资源路径，如 "Items/Flare"</param>
        /// <param name="targetSlotID">装备槽编号 (0: 手持)</param>
        public static void SpawnItemToHand(string prefabResourcePath, byte targetSlotID = 0)
        {
            if (GameUtils.instance != null && Character.localCharacter != null)
            {
                // 发送给 MasterClient (2) 请求生成
                GameUtils.instance.photonView.RPC(
                    "InstantiateAndGrabRPC",
                    RpcTarget.MasterClient,
                    prefabResourcePath,
                    Character.localCharacter.transform.position,
                    Character.localCharacter.photonView,
                    targetSlotID
                );
            }
        }

        /// <summary>
        /// 绕过距离与所有权检查，隔空强行拾取地面道具
        /// </summary>
        public static void ForcePickupGroundItem(Item groundItem, byte targetSlot = 0)
        {
            if (groundItem != null && Character.localCharacter != null)
            {
                // 直接本地触发拾取成功，并通知各端挂载
                groundItem.photonView.RPC("OnPickupAccepted", PhotonNetwork.LocalPlayer, targetSlot);
            }
        }
    }
}
```

---

### 5. 状态流数据注入：无限耐力与锁体力 (Stamina Stream Injector)

在 `CharacterSyncer.GetDataToWrite` 发送前修改 `CharacterSyncData`，让其他玩家眼中本地角色的耐力始终为满格：

```csharp
using HarmonyLib;
using UnityEngine;

namespace PeakMod.Sync
{
    [HarmonyPatch(typeof(CharacterSyncer), "GetDataToWrite")]
    public static class StaminaSyncPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref CharacterSyncData __result)
        {
            // 锁定发送给网络的主耐力为满值
            __result.stammina = 100f;
            __result.extraStammina = 50f;
            
            // 确保着地标志位开启，防止远程客户端误判为滑落
            __result.flags |= CharacterSyncData.Flags.IS_GROUNDED;
        }
    }
}
```

---

### 6. 一键关卡跳转数据包 (Custom Command Dispatcher)

构造并广播 `SyncMapHandlerDebugCommandPackage`，实现全员集体跃迁：

```csharp
using Zorro.PhotonUtility;

namespace PeakMod.Map
{
    public static class LevelSkipHelper
    {
        /// <summary>
        /// 广播关卡段落跳转包，传送指定玩家至目标段落
        /// </summary>
        /// <param name="targetSegment">目标地图段落枚举 (例如 2: Alpine)</param>
        /// <param name="playerActorNumbers">需要传送的玩家 ActorNumber 列表</param>
        public static void JumpToLevelSegment(byte targetSegment, int[] playerActorNumbers)
        {
            var package = new SyncMapHandlerDebugCommandPackage
            {
                Segment = targetSegment,
                Length = (byte)playerActorNumbers.Length,
                PlayerToTeleport = playerActorNumbers
            };

            // 广播自定义网络包
            CustomCommands<CustomCommandType>.SendPackage(package, 0); // 0 为全员广播
        }
    }
}
```
