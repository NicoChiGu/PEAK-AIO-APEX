# 《PEAK》高频状态同步流规范 (IPunObservable & 二进制流)

本文档深入剖析游戏《PEAK》中基于 PUN2 `IPunObservable` 接口实现的 **30Hz 二进制状态同步流**。包含了角色位置、布娃娃刚体、道具物理以及绳索分段同步的精细字节流布局与优化节流机制。

---

## 一、高频流同步架构与全局配置

### 1. 全局同步频次

在 `Peak.Network.NetworkingUtilities.ConnectToNetwork()` 中统一硬编码设置：
- **发包频率 (`PhotonNetwork.SendRate`)**: `30` Hz（每秒向底层网络发送 30 次数据包）
- **序列化频率 (`PhotonNetwork.SerializationRate`)**: `30` Hz（每 33.33ms 执行一次 `OnPhotonSerializeView`）

### 2. 核心基类抽象：`PhotonBinaryStreamSerializer<T>`

为了极大降低 PUN 反射和封包开销，游戏彻底摒弃了传统在 `PhotonStream` 中反复调用 `SendNext(float)` / `SendNext(Vector3)` 的做法，而是定义了如下泛型基类：

```csharp
public abstract class PhotonBinaryStreamSerializer<T> : MonoBehaviourPunCallbacks, IPunObservable 
    where T : struct, IBinarySerializable
```

#### 工作机制：
- **写入端 (Local Authority)**:
  1. 调用 `ShouldSendData()` 判断当前帧是否需发包（若静止或休眠则直接略过发包，实现静默节流）。
  2. 获取泛型结构体 `T data = GetDataToWrite()`。
  3. 创建 `Zorro.Core.Serizalization.BinarySerializer`，将 `data` 打包至紧凑字节缓冲区。
  4. 提取为 `byte[]`，并在 `PhotonStream` 中**仅执行一次**：
     ```csharp
     stream.SendNext(byteArray);
     ```
- **读取端 (Remote Client)**:
  1. 从流中接收原生字节数组：`byte[] raw = (byte[])stream.ReceiveNext();`
  2. 使用 `BinaryDeserializer` 反序列化为结构体 `T`。
  3. 缓存至 `this.RemoteValue`，并在 `Update` 或 `FixedUpdate` 中进行时间插值平滑（Lerp / Slerp）。

---

## 二、5 大状态同步器与字节布局深度剖析

---

### 1. `CharacterSyncer` (玩家角色状态流)

- **实现类**: `CharacterSyncer : PhotonBinaryStreamSerializer<CharacterSyncData>`
- **数据结构**: `CharacterSyncData` (结构体)
- **发包条件**: 作为本地控制端（Local Owner）时始终返回 `true`，以 **30Hz** 持续同步。
- **插值算法**:
  - `Update()` 中对视角朝向 `lookValues` 进行 `Vector2.Lerp` 平滑。
  - `FixedUpdate()` 中对布娃娃各刚体骨骼应用线性速度补偿与位置平滑。

#### 二进制流字节序列与字段布局 (单包 33 ~ 41 字节)

| 顺序 | 字段名 | C# 类型 | 序列化方法 | 物理类型 | 占用字节 | 业务说明与取值范围 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **1** | `hipLocation` | `Vector3` | `WriteFloat3` | 3 x Float32 | 12 B | 角色臀部核心刚体的世界坐标（高精度） |
| **2** | `lookValues` | `Vector2` | `WriteHalf2` | 2 x Float16 | 4 B | 视角俯仰角与偏航角（半精度浮点压缩） |
| **3** | `flags` | `Flags` (enum) | `WriteByte` | UInt8 | 1 B | 角色行为动作状态位掩码（定义见下表） |
| **4** | `sinceGrounded` | `float` | `WriteHalf` | Float16 | 2 B | 角色离开地面的滞空秒数 |
| **5** | `ropePercent` | `float` | `WriteHalf` | Float16 | **2 B (条件)** | **【条件写入】** 仅当 `flags & ROPE_CLIMBING` 为真时写入 |
| **6** | `averageVelocity` | `Vector3` | `WriteHalf3` | 3 x Float16 | 6 B | 布娃娃整体平均线性速度向量（半精度压缩） |
| **7** | `climbPos` | `Vector3` | `WriteHalf3` | 3 x Float16 | **6 B (条件)** | **【条件写入】** 仅当 `flags & CLIMBING` 为真时写入 |
| **8** | `stammina` | `float` | `WriteHalf` | Float16 | 2 B | 角色当前主耐力数值（半精度浮点） |
| **9** | `extraStammina` | `float` | `WriteHalf` | Float16 | 2 B | 角色额外耐力（黄条）数值 |
| **10** | `spectateZoom` | `float` | `WriteHalf` | Float16 | 2 B | 观战模式缩放系数 |
| **11** | `isChargingThrow`| `float` | `WriteHalf` | Float16 | 2 B | 是否处于投掷蓄力状态（0.0f 或 1.0f） |

#### `Flags` 状态位掩码对照表 (`[Flags] public enum Flags : byte`)

| 位权 (Bit Value) | 掩码十六进制 | 枚举常量名 | 动作语义 |
| :--- | :--- | :--- | :--- |
| `1` | `0x01` | `SPRINT` | 疾跑冲刺状态 |
| `2` | `0x02` | `ROPE_CLIMBING` | 正在攀爬绳索（触发字段 5 写入） |
| `4` | `0x04` | `WALK_RIGHT` | 向右侧向位移 |
| `8` | `0x08` | `WALK_LEFT` | 向左侧向位移 |
| `16` | `0x10` | `WALK_FORWARD` | 向前直行 |
| `32` | `0x20` | `WALK_BACKWARD`| 向后倒退 |
| `64` | `0x40` | `CLIMBING` | 正在岩壁攀爬（触发字段 7 写入） |
| `128` | `0x80` | `IS_GROUNDED` | 角色双脚处于地面着地状态 |

> [!TIP]
> **Mod 数据伪造提示**：
> - 只要持续向流中写入 `flags |= IS_GROUNDED`，游戏各端逻辑便会始终判定角色处于站立着地状态，从而免疫下坠滑落！
> - 将 `stammina` 与 `extraStammina` 持续定格在最大值，即可向其他玩家广播“无限耐力”。

---

### 2. `ItemPhysicsSyncer` & `MobItemPhysicsSyncer` (物理道具与掉落物)

- **实现类**:
  - `ItemPhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>`
  - `MobItemPhysicsSyncer : ItemPhysicsSyncer` (重写最大角速度阈值为 180°/s)
- **发包条件与节流策略 (`ShouldSendData`)**:
  - 必须同时满足：`shouldSync && !rig.isKinematic && !rig.IsSleeping() && itemState == ItemState.Ground`。
  - **节流关键**：当物体刚体进入休眠（Sleeping）、变为运动学刚体（Kinematic）或被收入玩家背包（非地面）时，**立即停发网络封包**。
  - 唤醒保底：若 `forceSyncFrames > 0`，强制同步指定帧数后休眠。

#### 二进制流字节布局 (固定 40 字节)

| 顺序 | 字段名 | C# 类型 | 序列化方法 | 物理格式 | 占用字节 | 说明 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **1** | `position` | `Vector3` | `WriteFloat3` | 3 x Float32 | 12 B | 掉落物世界坐标（单精度） |
| **2** | `rotation` | `Quaternion` | `WriteQuaternion`| 4 x Float32 | 16 B | 物体旋转四元数 $(x, y, z, w)$ |
| **3** | `linearVelocity` | `Vector3` | `WriteHalf3` | 3 x Float16 | 6 B | 线性速度矢量 |
| **4** | `angularVelocity`| `Vector3` | `WriteHalf3` | 3 x Float16 | 6 B | 角速度矢量 |

---

### 3. `PhysicsSyncer` (通用场景刚体)

- **实现类**: `PhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>`
- **应用范围**: 场景活动机关门、落石、可移动障碍物。
- **数据结构**: 与 `ItemPhysicsSyncData` 完全一致（40 字节）。`ShouldSendData()` 默认常开。

---

### 4. `PositionSyncer` (轻量级物体位置同步)

- **实现类**: `PositionSyncer : PhotonBinaryStreamSerializer<PositionSyncer.Pos>`
- **数据结构**: 仅包含一个 `Vector3 Position`。
- **发包条件与差分节流**:
  - 对比当前坐标与上一帧发送坐标：
    ```csharp
    !Mathf.Approximately(last.x, n.x) || !Mathf.Approximately(last.y, n.y) || !Mathf.Approximately(last.z, n.z)
    ```
  - **静止状态下零发包**，仅在位移变化时发包。新玩家加入时，通过 `forceSyncFrames = 10` 强制补发 10 帧。

#### 二进制流字节布局 (固定 6 字节)

| 顺序 | 字段名 | 序列化方法 | 物理格式 | 占用字节 |
| :--- | :--- | :--- | :--- | :--- |
| **1** | `Position` | `WriteHalf3` | 3 x Float16 (半精度) | **6 B** |

---

### 5. `RopeSyncer` (多节点攀爬绳索同步)

- **实现类**: `RopeSyncer : PhotonBinaryStreamSerializer<RopeSyncData>`
- **数据结构**: `RopeSyncData` 嵌套包含 $N$ 个 `SegmentData`。
- **数据包动态大小**: $4 + N \times 28$ 字节（如 10 节点绳索为 284 字节）。

#### 极具特色的 AOI 剔除与超长降频机制：
1. **距离剔除 (AOI)**: 若场景中所有玩家与绳索顶点距离超过 **100 米**，彻底停止发包。
2. **稳定降频节流**:
   - 当绳索生成超过 60 秒后（`Time.realtimeSinceStartup - startSyncTime > 60f`），物理晃动基本静止。
   - 此时内部激活 `syncIndex` 累加计数器，**每累加到 600 才同步 1 帧**！
   - 即在 30Hz 频率下，由每秒发 30 次暴降为**每 20 秒仅发 1 次**！

#### 二进制流字段布局

| 顺序 | 字段名 | 格式 | 字节 | 业务说明 |
| :--- | :--- | :--- | :--- | :--- |
| **1** | `isVisible` | `WriteBool` | 1 B | 绳索网格是否可见 |
| **2** | `updateVisualizerManually` | `WriteBool` | 1 B | 是否开启手动渲染管线刷新 |
| **3** | `segments.Length` | `WriteUshort` | 2 B | 绳索节点段数 $N$ (UInt16) |
| **4 ~ 4+N** | `segments[i]` | 循环结构体 | 每个 28 B | 每个分段包含：<br>• `position`: `WriteFloat3` (12 B)<br>• `rotation`: `WriteQuaternion` (16 B) |

---

## 三、Room Properties 与 Player Properties 使用情况

经对 Assembly-CSharp 全工程的检索：

### 1. 核心游戏业务：完全未使用 PUN 原生属性
- `Peak.Network.PhotonRoomEvents` 中：
  ```csharp
  public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
  public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
  ```
  回调函数均保留为空实现，游戏没有在房主或玩家属性表中存储任何业务状态。
- 原生持久化与外观改为使用前述的 `CustomCommands` 二进制包替代。

### 2. 仅内置语音演示模块 (Photon Voice Demo) 使用了 Player Properties：

| 属性 Key | 属性类型 | 对应读写方法 | 用途说明 |
| :--- | :--- | :--- | :--- |
| `"mu"` | `bool` | `Mute(true)` / `IsMuted()` | 玩家是否在房间内被静音 |
| `"pv"` | `bool` | `SetPhotonVAD(bool)` | 开启 Photon 自带的语音活动检测 |
| `"wv"` | `bool` | `SetWebRTCVAD(bool)` | 开启 WebRTC 语音活动检测 |
| `"ec"` | `bool` | `SetAEC(bool)` | 开启回声消除 (AEC) |
| `"gc"` | `object[]` | `SetAGC(enabled, gain, level)` | 麦克风自动增益控制设置 |
| `"m"` | `int (enum)` | `SetMic(MicType)` | 当前选择的麦克风硬件设备索引 |
