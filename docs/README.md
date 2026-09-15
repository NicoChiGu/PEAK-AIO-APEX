# 《PEAK》网络通信与多人协议开发参考手册

本文档为基于 `20260915-Assembly-CSharp.dll` 逆向反编译提炼的完整网络通信协议参考，专门用于制作游戏 Mod、Harmony 补丁、联机功能扩展及辅助工具。

---

## 目录索引

| 文档编号 | 文档名称 | 核心内容 |
| :--- | :--- | :--- |
| **[01. RPC 远程调用手册](./01_Photon_RPC_Reference.md)** | `01_Photon_RPC_Reference.md` | 整理了 94 个类中的 **234 个 `[PunRPC]`** 详细通信接口、调用方与接收方、数据类型签名及网络行为。 |
| **[02. 网络事件与自定义命令](./02_Network_Events_and_Custom_Commands.md)** | `02_Network_Events_and_Custom_Commands.md` | `PhotonNetwork.RaiseEvent`、Zorro `CustomCommands` 二进制网络包（外观/地图传送/岩浆）、Steam Lobby 握手发现与房间管理。 |
| **[03. 实时状态同步流](./03_State_Synchronization_Stream.md)** | `03_State_Synchronization_Stream.md` | `IPunObservable` 30Hz 高频二进制同步流，包含角色位置动作、刚体物理、绳索剔除降频机制及 Room/Player Properties 说明。 |
| **[04. Mod 网络开发实战指南](./04_Modding_Practical_Guide.md)** | `04_Modding_Practical_Guide.md` | 基于 Harmony 的常用 RPC 拦截、封包注入、上帝模式、无限耐力、全图瞬移、房主权限接管等实战代码模板。 |

---

## 游戏多人网络架构总览

游戏采用典型的 **Client-Server-Host (PUN2 房主架构) + Steam P2P Lobby 握手** 模式：

```mermaid
graph TD
    subgraph 发现与握手阶段 (Steamworks P2P)
        A[Client] -->|SteamMatchmaking.GetLobbyData| B[Steam Lobby]
        A -->|SendLobbyChatMsg: RequestRoomID| C[Host]
        C -->|SendLobbyChatMsg: RoomID| A
    end

    subgraph 房间与会话层 (Photon Realtime / PUN2)
        A -->|JoinRoom / ConnectToRegion| D[Photon Cloud Server]
        C -->|CreateRoom / MasterClient| D
    end

    subgraph 业务通讯通道
        D --> E[PUN RPC 远程过程调用<br/>234 个 PunRPC, 负责离散玩法与指令]
        D --> F[Zorro CustomCommands<br/>RaiseEvent 二进制可靠包, 同步外观与宏观状态]
        D --> G[IPunObservable 状态流<br/>30Hz 二进制压缩流, 同步角色与刚体物理]
    end
```

---

## 全局网络基础参数

在 `Peak.Network.NetworkingUtilities` 中定义的关键网络配置：

- **发包速率 (SendRate)**: `30` Hz（每秒向网络层发包 30 次）
- **序列化速率 (SerializationRate)**: `30` Hz（状态流序列化间隔约 33.33ms）
- **房间人数上限 (MaxPlayers)**: `4`（默认最大 4 人，可由 Mod 扩展）
- **房间可见性 (IsVisible)**: `false`（默认不公开在 Photon 公共房间大厅，依赖 Steam 房间号互联）
- **用户 ID 广播 (PublishUserId)**: `true`
- **默认场景**: 初始场景为 `"Airport"`（值机机场）

---

## Mod 开发者速查导引

1. **如果你想修改角色状态（防跌倒、防眩晕、免死、锁耐力）**：
   - 参考 `01_Photon_RPC_Reference.md` 中的 `Character` 与 `CharacterAfflictions`。
2. **如果你想实现物品无限刷取或远程拾取**：
   - 参考 `01_Photon_RPC_Reference.md` 中的 `Item` 与 `GameUtils`。
3. **如果你想同步自定义游戏数据或拦截踢人指令**：
   - 参考 `02_Network_Events_and_Custom_Commands.md` 中的 `CustomCommands` 与 `KickPlayer (EventCode 18)`。
4. **如果你想开发穿墙、飞天、平滑移动外挂或解除绳索长度限制**：
   - 参考 `03_State_Synchronization_Stream.md` 中的 `CharacterSyncer` 与 `RopeSyncer`。
