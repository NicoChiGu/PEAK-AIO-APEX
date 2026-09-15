# 《PEAK》全量 Photon RPC 远程过程调用速查手册

本文档整合自反编译程序集 `20260915-Assembly-CSharp.dll` 中的所有 `[PunRPC]` 定义与调用。全工程共检索出 **94 个类**、**234 处 RPC 调用与处理函数**。

---

## 目录
- [一、PUN2 RpcTarget 映射基准](#一pun2-rpctarget-映射基准)
- [二、角色核心与状态系统 (Player & Character)](#二角色核心与状态系统-player--character)
- [三、物品与装备系统 (Items & Equipment)](#三物品与装备系统-items--equipment)
- [四、道具动作卡系统 (Action Cards)](#四道具动作卡系统-action-cards)
- [五、环境、关卡与灾害设施 (Environment & Hazards)](#五环境关卡与灾害设施-environment--hazards)
- [六、怪物与生物实体系统 (Monsters & Bosses)](#六怪物与生物实体系统-monsters--bosses)
- [七、神力与特殊机制 (BingBong Powers)](#七神力与特殊机制-bingbong-powers)
- [八、全局网络流程与工具服务 (GameUtils & Flow)](#八全局网络流程与工具服务-gameutils--flow)

---

## 一、PUN2 RpcTarget 映射基准

在 Assembly-CSharp 源代码中，`photonView.RPC(...)` 的目标通常为整数枚举值或直接传入 `Player` 实例：

| 目标代码 / 形式 | Photon 枚举映射 | 网络行为与范围 |
| :--- | :--- | :--- |
| `0` | `RpcTarget.All` | 向房间内所有玩家（包括发起者本地）立即分发执行 |
| `1` | `RpcTarget.Others` | 仅向房间内其他客户端分发，本地不触发（本地由前置逻辑预测执行） |
| `2` | `RpcTarget.MasterClient` | 定向分发给房主（主机），用于仲裁验证、物品拾取批准、关卡跳转 |
| `3` | `RpcTarget.AllBuffered` | 发给全员并存储在 Photon 服务器缓存池，新入场玩家重放执行 |
| `4` | `RpcTarget.OthersBuffered` | 发给除自身外全员并缓存 |
| `Player` 实例 | `Target Player` | 点对点单播（如 `photonView.Owner`、`newPlayer`） |

---

## 二、角色核心与状态系统 (Player & Character)

### 1. `Character.cs` (核心受力、生命周期与动作)

| RPC 方法签名 | 目标 | 触发场景与业务逻辑 | 参数类型及说明 | Mod 实用 Hook 价值 |
| :--- | :--- | :--- | :--- | :--- |
| `RPCA_AddForceAtPosition(Vector3 force, Vector3 point, float radius)` | `All (0)` | 爆炸/冲击在特定三维坐标对布娃娃骨骼产生范围冲量 | `force` (冲量向量)<br>`point` (爆点坐标)<br>`radius` (影响半径) | 拦截可实现无视爆炸冲击波 |
| `RegainItems(PhotonView regainingCharacterView)` | `MasterClient (2)` | 角色复活后向主机请求吸附收回散落的私人物品 | `regainingCharacterView` (复活角色网络视图) | 可用于任意时刻远程自动吸取掉落物 |
| `RPCA_SetDead()` | `All (0)` | 设置角色 `dead = true`, `fullyPassedOut = true` 并重置倒计时 | 无 | 阻止该 RPC 广播可防止进入死亡态 |
| `RPCA_Die(Vector3 itemSpawnPoint)` | `All (0)` | 角色彻底死亡广播，卸载所有随身道具并掉落到地面 | `itemSpawnPoint` (遗物掉落点) | 阻止此 RPC 可避免死亡掉落背包与装备 |
| `RPCA_Zombify(Vector3 itemSpawnPoint)` | `All (0)` | 玩家尸体转变为真菌丧尸，主机端生成丧尸实体 | `itemSpawnPoint` (生成坐标) | 拦截可免疫丧尸化转变 |
| `RPCEndGame_ForceWin()` | `All (0)` | 强制触发全员胜利并播放撤离结算动画 | 无 | **高危/调试**：直接调用可秒通关 |
| `RPCEndGame()` | `All (0)` | 终局判定，检查各玩家是否在救援梯/山顶并结算输赢 | 无 | 用于自定义登顶触发规则 |
| `RPCA_UnPassOut()` | `All (0)` | 唤醒昏迷角色，本地玩家触发淡入黑屏恢复视线 | 无 | 任意时刻调用可瞬间解救昏睡队友 |
| `RPCA_PassOut()` | `All (0)` | 饥饿、缺氧或体力透支耗尽导致玩家昏睡倒地 | 无 | 拦截可实现“绝对不昏睡”功能 |
| `MoraleBoost(float staminaAdd, int scoutCount)` | `All (0)` | 团队士气振奋，增加全员额外耐力并播放特效 | `staminaAdd` (耐力恢复量)<br>`scoutCount` (童军人数) | 可用于制作全队耐力激励光环 |
| `GetFedItemRPC(int itemPhotonID)` | `Target Player` | 队友强行手持食物/药水喂食本地玩家，强制触发物品使用 | `itemPhotonID` (物品 PhotonView ID) | 自动吃药/防被队友恶意喂毒 |
| `RPCA_Fall(float seconds)` | `All (0)` | 角色失足滑落，同步布娃娃失控下坠时长 | `seconds` (下落滑行时间) | 拦截可彻底消除失足滑落硬直 |
| `RPCA_FallWithScreenShake(float seconds, float shake)` | `All (0)` | 伴随剧烈屏幕震动的严重滑坠同步 | `seconds` (秒数)<br>`shake` (晃动强度) | 拦截可消除坠落眩晕与镜头震荡 |
| `RPCA_UnFall()` | `All (0)` | 坠落着地恢复站立姿态，清空滑坠状态机 | 无 | 失足时主动触发可立即刹车恢复 |
| `RPCA_Revive(bool applyStatus)` | `All (0)` | 原地复活角色，清除倒地昏厥并可附带复活负面状态 | `applyStatus` (是否附加复活虚弱/虚脱) | **核心技能**：无限免费原地满血复活 |
| `RPCA_ReviveAtPosition(Vector3 position, bool applyStatus, int statueSegment)` | `All (0)` | 在指定雕像/坐标处重生物理躯体并复活 | `position` (复活点)<br>`applyStatus` (附加负面)<br>`statueSegment` (段落) | 任意坐标定向复活重生 |
| `WarpPlayerRPC(Vector3 position, bool poof)` | `All (0)` | 强行将角色重定位至指定三维坐标，带烟雾特效 | `position` (目标坐标)<br>`poof` (是否产生烟雾) | **核心功能**：全图定点瞬间传送 |
| `RPCA_AddForceToBodyPart(BodypartType bodypartType, Vector3 force, Vector3 wholeBodyForce)` | `All (0)` | 定向对头部/四肢单个骨骼施加外力并联动躯干 | `bodypartType` (骨骼枚举)<br>`force` (单部位力)<br>`wholeBodyForce` (全身力) | 消除受击击退反馈 |
| `RPCA_Stick(BodypartType bodypartType, Vector3 pos, Vector3 stickAnchor, CharacterAfflictions.STATUSTYPE statusType, float statusAmount)` | `All (0)` | 身体特定部位被蛛网/黏胶吸附，挂载物理关节并减速 | `bodypartType` (部位)<br>`pos` (黏附点)<br>`stickAnchor` (锚点)<br>`statusType` (附加状态)<br>`statusAmount` (强度) | 免疫蛛网缠绕与黏附陷阱 |
| `RPCA_ClearStickData()` / `RPCA_ClearJoint(...)` / `RPCA_Unstick()` | `All (0)` | 挣脱或清除身上的黏附物理关节 | `bodypartType` (部位枚举) | 自动瞬间挣脱任何束缚 |

---

### 2. `CharacterAfflictions.cs` (疾病、负面状态与荆棘)

| RPC 方法签名 | 目标 | 触发场景与业务逻辑 | 参数类型及说明 | Mod 实用 Hook 价值 |
| :--- | :--- | :--- | :--- | :--- |
| `SyncStatusesRPC(byte[] data)` | `Others (1)` | 广播耐力上限削减百分比数组（寒冷、中毒、诅咒等） | `data` (二进制压缩的状态数组) | 伪造全 0 封包可实现永久锁满耐力 |
| `SyncThornsRPC_Remote(byte[] data)` | `Others (1)` | 广播扎在角色肉体上的尖刺物理索引集合 | `data` (ThornSyncData 二进制流) | 拦截或清除远程荆棘显示 |
| `RPC_ApplyStatusesFromFloatArray(float[] data, PhotonMessageInfo info)` | 房主验证执行 | 主机下发各状态增减浮点数（寒冷、毒素等） | `data` (浮点数组)<br>`info` (发送者信息) | 过滤扣血/扣耐力负面参数 |
| `SyncAfflictionsRPC(byte[] data)` | `Others (1)` | 同步当前角色附带的疾病列表 | `data` (疾病数据流) | 净化角色负面疾病状态 |
| `RPC_EnableThorn(int thornIndex)` | `All (0)` | 显示身上某根荆棘刺模型并增加负重 | `thornIndex` (尖刺插槽索引) | 拦截可使荆棘无法在身上附着 |
| `RPC_DisableThorn(int thornIndex)` | `All (0)` | 拔出身上特定荆棘并减轻负重 | `thornIndex` (尖刺插槽索引) | 自动一键拔除全身荆棘 |
| `RemoveThornRPC(int index)` | `Target Player` | 队友交互帮忙拔刺，定向发给受害者客户端 | `index` (尖刺索引) | 远程帮队友瞬拔所有荆棘 |
| `StartWhirlwindRPC()` / `WarnStopWhirlwindRPC()` / `StopWhirlwindRPC()` | `All (0)` | 角色周围旋风/防护力场的启动、预警与熄灭 | 无 | 自定义旋风护盾特效 |

---

### 3. 角色动作控制与协同交互

#### `CharacterCarrying.cs` (背人/搬运队友)
- `RPCA_StartCarry(PhotonView targetView)` (`RpcTarget.All`): 扛起倒地或疲惫的队友。若背部有背包则自动脱落至地面，建立物理刚体父子连接。
- `RPCA_Drop(PhotonView targetView)` (`RpcTarget.All`): 放下背上的玩家，解除物理刚体绑定。

#### `CharacterClimbing.cs` (岩壁攀爬)
- `StartClimbRpc(Vector3 climbPos, Vector3 climbNormal)` (`RpcTarget.All`): 抓附岩壁开始攀爬，同步攀爬锚点与法线，扣除初次攀抓耐力。
- `StopClimbingRpc(float setFall)` (`RpcTarget.All`): 脱离岩壁进入自由落体或站立。
- `RPCA_ClimbJump()` (`RpcTarget.All`): 攀爬时执行向上蹬墙跳，消耗 20% 耐力并伴随镜头抖动。

#### `CharacterCustomization.cs` & `CharacterData.cs`
- `SetCharacterIdle_RPC(int index)` (`RpcTarget.AllBuffered`): 设置同步玩家个性化待机姿势。
- `CharacterDied()` / `CharacterPassedOut()` / `OnRevive_RPC()` (`RpcTarget.AllBuffered`): 切换眼睛贴图（死亡死鱼眼、眩晕螺旋眼、复活正常眼）。
- `CharacterData.RPC_SyncOnJoin(...)` (`Target Player (newPlayer)`): 新玩家加入时下发 19 项全量状态（是否昏厥、食人许可、骷髅皮肤、攀爬百分比等）。
- `CharacterData.RPCA_SyncCanBeCannibalized(bool canBeCannibalized)` (`RpcTarget.All`): 同步是否允许被队友食用开关。
- `CharacterData.RPC_SyncSkeleton(bool active)` (`RpcTarget.All`): 变为骷髅人外观。

---

## 三、物品与装备系统 (Items & Equipment)

### 1. `Item.cs` 核心基类 (生命周期与拾取仲裁)

| RPC 方法签名 | 目标 | 触发场景与业务逻辑 | 参数说明 | Mod 实用 Hook 价值 |
| :--- | :--- | :--- | :--- | :--- |
| `RequestPickup(PhotonView interactorView)` | `MasterClient (2)` | 玩家尝试拾取地面道具，向主机发起权威仲裁请求 | `interactorView` (请求者网络视图) | 可实现全图超远距离自动拾取 |
| `OnPickupAccepted(byte targetSlot)` | `Target Player` | 主机批准拾取，通知发起者放入指定装备槽 | `targetSlot` (目标背包/手持槽) | 拦截或直接调用可强行拿取物品 |
| `DenyPickupRPC()` | `Target Player` | 主机拒绝拾取（距离过远或已被他人抢先拾起） | 无 | 绕过拒绝判定可防止被抢夺 |
| `SetItemInstanceDataRPC(ItemInstanceData data)` | `Others (1)` | 同步物品内部动态数据（耐久、新鲜度、数值修饰） | `data` (物品结构体) | 伪造耐久度可实现无限耐久 |
| `PutInBackpackRPC(int parentSlot, int childSlot)` | `All (0)` | 物品放入背包，禁用场景碰撞盒并挂载到背包节点 | `parentSlot`<br>`childSlot` | 背包无限收纳与自动整理 |
| `SetKinematicRPC(bool kinematic, Vector3 pos, Quaternion rot)` | `AllBuffered (3)` | 设置物品刚体是否受物理重力影响或固定 | `kinematic`<br>`pos`<br>`rot` | 物体空中悬浮与隔空抓物 |
| `Consume(int consumerID)` | `All (0)` | 物品消耗事件，广播吃食物特效或扣除耐久 | `consumerID` (消费者角色 ID) | 拦截可实现吃食物不消耗物品 |
| `SendFeedDataRPC(...)` / `RemoveFeedDataRPC(int id)` | `All (0)` | 手持食物喂队友时的引导进度与交互 | 喂食数据结构 | 自动持续给队友喂食药水 |

---

### 2. 特殊物品机制

| 组件类名 | RPC 方法签名 | 目标 | 业务说明与应用 |
| :--- | :--- | :--- | :--- |
| `ItemCooking.cs` | `EnableCookingSmokeRPC(bool enable)` | `All` | 烤肉/烹饪过程烟雾产生 |
| | `FinishCookingRPC()` | `All` | 烹饪完成，物品属性与模型变为熟食 |
| | `RPC_CookingExplode()` | `All` | 烧烤过热炸毁并点燃周围场景 |
| `Dynamite.cs` | `RPC_Explode()` | `All` | 炸药起爆，炸碎周围易碎岩石并击飞角色 |
| | `SetFlareLitRPC()` | `All` | 点燃炸药导火索视觉效果 |
| `Flare.cs` | `TriggerHelicopter()` | `AllBuffered` | 发射信号弹后，在指定空域呼叫直升机提早撤离 |
| `Lantern.cs` | `LightLanternRPC(bool lit)` | `All` | 点亮/熄灭随身马灯光源 |
| `Luggage.cs` | `OpenLuggageRPC(bool open)` | `All` | 行李箱开盖，爆出内部掉落物 |
| `MagicBean.cs` | `GrowVineRPC(Vector3 start, Vector3 end)` | `All` | 魔豆萌发，凭空生成直通天际的攀爬粗藤蔓 |
| `Mandrake.cs` | `RPC_Scream()` | `All` | 拔起曼德拉草，向全地图广播高分贝尖叫致晕全员 |
| `RescueHook.cs` | `RPCA_RescueCharacter(PhotonView targetView)` | `All` | 救援爪钩射中倒地队友，发射收卷缆绳将其拖回 |
| | `RPCA_RescueWall(bool hit, Vector3 point)` | `All` | 抓钩锚定岩壁并飞速拉扯自身（立体机动装置） |
| `Rope.cs` | `AttachToAnchor_Rpc(...)` | `AllBuffered` | 绳索挂接到锚点上 |
| | `Detach_Rpc(int segments)` | `AllBuffered` | 绳索超重绷断，断开分段 |
| `ShittyPiton.cs` | `RPCA_StartBreaking()` | `All` | 劣质岩钉开裂受损 |
| | `RPCA_Break()` | `All` | 岩钉彻底崩落脱落 |
| `FakeItemManager.cs` | `RPC_RequestFakeItemPickup(int index)` | `MasterClient` | 向主机申请捡起假道具 |
| | `RPC_FakeItemPickupSuccess(int index)` | `All` | 审批通过，同步销毁全网假物品 |

---

## 四、道具动作卡系统 (Action Cards)

| 动作卡组件 | RPC 方法签名 | 目标 | 业务说明 |
| :--- | :--- | :--- | :--- |
| `Action_ApplyMassAffliction.cs` | `RPC_ApplyAffliction(...)` | `All` | 卷轴/道具向范围内所有玩家施加群体增益或负面状态 |
| `Action_AskBingBong.cs` | `RPC_AskBingBong(...)` | `All` | 使用神秘占卜球向 BingBong 祈愿或获取神谕 |
| `Action_BookOfBonesAnim.cs` | `RPC_PlayBookAnim()` | `All` | 骸骨之书翻页咒语动画广播 |
| `Action_RaycastDart.cs` | `RPC_ShootDart(...)` | `All` | 吹箭筒向射线落点发射毒针/昏睡针 |
| `Action_ReduceUses.cs` | `RPC_ReduceUses()` | `All` | 道具使用次数扣减 1 次 |
| `Action_ShowBinocularOverlay.cs` | `RPC_ShowOverlay(bool show)` | `All` | 望远镜侦查视野全屏 UI 覆盖 |
| `Action_Spawn.cs` | `RPC_SpawnActionItem(...)` | `MasterClient` | 道具动作卡生成专属衍生实体道具 |
| `Action_WarpToRandomPlayer.cs` | `RPC_WarpToPlayer(...)` | `All` | 瞬移卷轴：立即随机传送到一名存活队友身边 |

---

## 五、环境、关卡与灾害设施 (Environment & Hazards)

| 场景设施组件 | RPC 方法签名 | 目标 | 业务说明与 Hook 场景 |
| :--- | :--- | :--- | :--- |
| `AirportCheckInKiosk.cs` | `LoadIslandMaster(int ascent)` | `MasterClient` | 玩家在前台柜台选定难度，向主机请求开始登岛 |
| | `BeginIslandLoadRPC(string sceneName, int ascent)` | `All` | 主机命令全员异步加载目标大地图场景 |
| `Campfire.cs` | `Light_Rpc()` | `All` | 点燃营火，提供保暖与恢复力场 |
| | `Extinguish_Rpc()` | `AllBuffered` | 暴风雪熄灭营火 |
| `BreakableBridge.cs` | `SyncHoldsPeopleRPC(bool holds)` | `All` | 同步吊桥是否已无法承重 |
| | `ShakeBridge_Rpc()` | `All` | 踏上脆弱吊桥时同步剧烈摇晃与碎裂音效 |
| | `Fall_Rpc()` | `All` | 吊桥彻底崩塌坠毁 |
| `DayNightManager.cs` | `RPCA_SyncTime(float timeOfDay)` | `All` | 局内白天/黑夜昼夜更替时间同步 |
| `Fog.cs` / `OrbFogHandler.cs` | `Fog.RPCA_SyncFog(float height)` | `Others` | 剧毒迷雾海平面上升高度同步 |
| | `OrbFogHandler.StartMovingRPC()` | `All` | 巡航迷雾毒球开始移动推进 |
| `MovingLava.cs` | `RPCA_StartLavaRise()` | `All` | 火山喷发，熔岩开始向上漫延 |
| | `RPCA_SyncLavaHeight(float height)` | `All` | 熔岩绝对高度校准对齐 |
| `ScoutCannon.cs` (童军大炮) | `RPCA_SetTarget(int characterViewID)` | `All` | 将指定玩家塞入大炮炮筒 |
| | `RPCA_Light()` | `All` | 点燃大炮引信呲呲冒火花 |
| | `RPCA_LaunchTarget(int characterViewID)` | `All` | 开炮将玩家超高速轰向山顶或高空 |
| | `RPCA_LaunchItem(int itemID)` | `All` | 发射塞在大炮中的物资箱 |
| `Tornado.cs` (龙卷风) | `RPCA_SyncTornado(Vector3 vel)` | `All` | 龙卷风风眼移动速度向量同步 |
| | `RPCA_CaptureCharacter(int characterViewID)` | `All` | 龙卷风将经过的玩家卷入半空失控旋转 |
| | `RPCA_ThrowPlayer(int characterViewID)` | `All` | 龙卷风将玩家向远处高空暴力抛射 |
| | `RPCA_TornadoDie()` | `All` | 龙卷风自然消散 |
| `WindChillZone.cs` | `RPCA_ToggleWind(bool active)` | `All` | 极寒暴风雪区域开启/关闭，体温暴跌 |

---

## 六、怪物与生物实体系统 (Monsters & Bosses)

| 怪物组件 | RPC 方法签名 | 目标 | 业务说明 |
| :--- | :--- | :--- | :--- |
| `Antlion.cs` (沙坑蚁狮) | `RPCA_SetClosestTarget(int characterViewID)` | `All` | 锁定落入流沙凹坑的猎物玩家 |
| | `RPCA_Attack(int characterViewID)` | `All` | 从流沙破土跃出扑咬击退 |
| `ArrowShooter.cs` (毒箭机关) | `WarningArrows_RPC(int count)` | `AllBuffered` | 机关触发前先射出警告性钝头箭 |
| | `FireArrow_RPC(Vector3 targetPos)` | `AllBuffered` | 陷阱连环向目标坐标射出剧毒箭矢 |
| `BeeSwarm.cs` (杀人蜂群) | `SetBeesAngryRPC(bool flag)` | `AllBuffered` | 蜂巢破坏，蜂群暴怒死咬追逐玩家 |
| | `DisperseRPC()` | `All` | 遇到火把或跃入水中时蜂群退散 |
| `Mob.cs` (通用野生动物) | `RPC_SyncMobState(int state)` | `Others` | 怪物警戒、巡逻、追击状态机同步 |
| | `RPC_SyncTargetCharacter(int characterID)` | `Others` | 仇恨目标切换 |
| | `RPC_StartAttack()` | `All` | 触发普通攻击起手前摇动画 |
| `MushroomZombie.cs` (真菌丧尸) | `RPC_Arise(int characterViewID)` | `All` | 倒地尸体变异为真菌丧尸破土爬起 |
| | `RPCA_SetCurrentTarget(int characterViewID)` | `All` | 丧尸锁定活体人类仇恨 |
| | `RPC_PlaySFX(int sfxIndex)` | `All` | 播放阶段嘶吼与狂暴音效 |
| `Scoutmaster.cs` (教官 BOSS) | `RPCA_SetCurrentTarget(...)` | `All` | 锁定违反团队纪律的队员 |
| | `WarpPlayerRPC(...)` | `All` | 隔空瞬移强行抓捕乱跑玩家 |
| | `RPCA_Throw()` | `All` | 抓住玩家并向前狠狠摔飞 |
| `Spider.cs` (巨型蜘蛛) | `RPCA_GrabCharacter(PhotonView characterView)` | `All` | 蛛丝缠绕并拖拽玩家进入洞穴 |
| | `RPCA_LetGo()` | `All` | 松开猎物 |
| | `BonkRPC()` | `All` | 蜘蛛头部受重击产生击晕硬直 |
| `Looker.cs` (石化眼球) | `RPCA_Switch(bool active)` | `All` | 眼球开启/闭合待机状态 |
| | `RPCA_CodeRed()` | `AllBuffered` | 探测到玩家移动，触发红色全域石化致死警报 |

---

## 七、神力与特殊机制 (BingBong Powers)

| 特效组件 | RPC 方法签名 | 目标 | 业务说明与应用 |
| :--- | :--- | :--- | :--- |
| `BingBongTimeControl.cs` | `RPCA_SyncTime(float timeScale)` | `All (0)` | **时间操纵**：全图时间膨胀缩放（如 `0.1f` 子弹时间或极速倍速） |
| `BingBongForceAbilities.cs` | `RPCA_ApplyForceField(Vector3 center, float force)` | `All (0)` | 在指定坐标爆开斥力/引力神力力场，击飞所有实体 |
| `BingBongStatus.cs` | `RPCA_GrantPowerStatus(int characterViewID, int powerID)` | `All (0)` | 随机赋予某玩家无敌、飞天或超速等神力祝福 |

---

## 八、全局网络流程与工具服务 (GameUtils & Flow)

### 1. `GameOverHandler.cs` (结算与返回)
- `PlayerHasClosedEndScreen()` (`RpcTarget.All`): 记录该客户端已关闭结算界面。
- `LoadAirportMaster()` (`RpcTarget.MasterClient`): 向主机发起返航请求。
- `BeginAirportLoadRPC()` (`RpcTarget.All`): 主机命令全员切回机场大厅场景。
- `ForceEveryPlayerDoneWithEndScreenRPC()` (`RpcTarget.All`): 主机强制关闭所有人的结算面板跳回大厅。

### 2. `GameUtils.cs` (全局通用网络工具单例)
- `RPC_SyncAscent(int ascent)` (`RpcTarget.All`): 同步当前游戏难度等阶。
- `InstantiateAndGrabRPC(string path, Vector3 pos, PhotonView playerView, byte slotID)` (`RpcTarget.MasterClient`):
  - **物品生成器**：向主机请求在指定坐标生成预制体道具，并直接塞入目标玩家的手持槽位中。
- `RPC_SpawnResourceAtPosition(string resourcePath, Vector3 position)` (`Target Player`): 在指定位置生成资源实体。
- `IncrementFriendHealingRpc(int amt)` (`Target Player`): 跨端累加目标的救治队友统计。
- `IncrementPoisonHealedStat(int amt)` (`Target Player`): 跨端累加目标的解毒统计。

### 3. 其它辅助网络组件
- `PointPinger.cs` -> `ReceivePoint_Rpc(Vector3 point, Vector3 normal)` (`RpcTarget.All`): 队伍战术标点，在碰撞表面生成 3D 标点与光柱。
- `ReconnectHandler.cs` -> `RefreshReconnectDataTable(string playerSteamID, byte[] reconnectData)` (`Target Player`): 断线重连时房主下发该玩家掉线前的状态数据镜像。
- `RunManager.cs` -> `SyncRunSeedRPC(int seed)` (`RpcTarget.AllBuffered`): 关卡随机生成种子全员对齐。
