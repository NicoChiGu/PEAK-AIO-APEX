# PEAK-AIO-APEX 开发者与 AI 智能体协同指南 (AGENTS.md)

欢迎来到 **PEAK-AIO-APEX** 代码库。本文件是面向 AI 智能体（如 Google Antigravity、Claude Code、Cursor 等）及人类开发者的核心规范手册。在阅读、编写、重构或调试本项目代码之前，**必须完整阅读并严格遵守**本指南中定义的技术栈红线、架构模式与工程规范。

---

## 1. 项目定位与核心技术全貌 (Project Overview & Tech Stack)

### 1.1 项目定位
- **项目名称**：`PEAK AIO APEX Mod`
- **Mod 唯一标识 (GUID)**：`com.onigremlin.peakaio`
- **输出程序集**：`PEAK-AIO.dll`（主命名空间：`PEAK_AIO`）
- **目标宿主游戏**：Steam 平台多人合作探险攀登游戏 **《PEAK》**（Steam AppID: 3527290）
- **项目性质**：挂载于 Unity/Mono 运行时的游戏内辅助与多人大厅管理模组。通过 BepInEx 插件体系注入、原生 IMGUI 渲染操作界面，结合 Harmony 字节码拦截与 Photon (PUN2) 网络封包调度，为玩家提供高自由度控制与网络同步修复。

### 1.2 技术栈基准表
| 维度 | 规范与版本 | 强制说明 |
| :--- | :--- | :--- |
| **开发语言** | **C#** (语言版本 7.3 / 8.0 兼容子集) | 允许非安全代码块 (`<AllowUnsafeBlocks>true</AllowUnsafeBlocks>`)。 |
| **目标框架** | **.NET Framework 4.7.2** (`net472`) | 经典 Windows .NET Framework，与 Unity 2021/2022 Mono 运行时深度兼容。 |
| **Mod 加载器** | **BepInEx 5.4.23.3 (x64)** | **【严格禁止使用 BepInEx 6.x】**。插件继承自 `BaseUnityPlugin`。 |
| **字节码补丁** | **HarmonyX / 0Harmony 2.9.0.0** | 负责运行时方法的 Prefix/Postfix 拦截与私有成员反射访问。 |
| **UI 渲染引擎** | **Unity Native IMGUI** (`OnGUI`) | **完全摒弃 DearImGuiInjection 等第三方 DX Hook**，使用原生 `GUILayout` 绘制，消除 DX11/DX12/Vulkan 崩溃。 |
| **网络协议** | **Photon Unity Networking (PUN2)**<br/>**Zorro.Core.Runtime** | 游戏底层采用 PUN2 进行 RPC 调度与状态流同步，Zorro 序列化负责二进制数据包。 |
| **依赖管理模式** | **物理本地二进制库 (`References/`)** | **【严禁使用 NuGet 管理核心依赖】**。全部 19 个程序集均位于 `PEAK-AIO/References/`。 |

---

## 2. 核心铁律与安全红线 (Strict Constraints & Red Lines)

AI 智能体在处理本项目时，必须将以下条款视为**绝对不可逾越的红线**：

1. **运行时环境不可变更**：
   - 严禁将 `.csproj` 升级或尝试迁移到 .NET Core / .NET 6/8/9 或 SDK-style 格式。
   - 严禁修改 Target Framework（必须固定为 `v4.7.2`），否则将导致 BepInEx 5 加载器符号绑定失败。
2. **依赖隔离铁律**：
   - 严禁通过 `dotnet add package` 或 NuGet 引入游戏运行库替代品。
   - 所有 UnityEngine、Photon、BepInEx 相关依赖必须通过 `PEAK-AIO/References/*.dll` 进行相对路径引用。
3. **IMGUI 零 GC 内存纪律**：
   - 严禁在 `OnGUI()`、`DrawWindow()` 或任何每帧调用的 UI 渲染逻辑中执行 `new Texture2D(...)` 或 `new GUIStyle(...)`。
   - 所有的背景纹理、字体样式、调色板必须在 `Main.InitStyles()` 中一次性生成并静态持有，避免高频 GC 分配（GC Spikes）导致游戏掉帧或卡顿。
4. **主线程封送隔离 (Dispatcher Rule)**：
   - Unity 引擎限制：除数学计算与基础数据结构外，几乎所有 Unity API（如 `Transform`、`GameObject`、`Physics`、`Raycast`、`Component`）及 PUN RPC 操作均**必须在 Unity 主线程执行**。
   - 任何来自异步线程、网络回调或延迟任务的代码，必须使用 `Utilities.UnityMainThreadDispatcher.Enqueue(...)` 封送回主线程执行。
5. **防御性物理判定与防穿模**：
   - 严禁直接将玩家传送至未经验证的空间坐标。
   - 任何涉及位置突变（瞬移、切片跳转、复活）的操作，必须调用 `Physics.SyncTransforms()` 刷新刚体层次树，并通过 `Utilities.ResolveSafeGroundPosition(...)` 执行垂向射线探测坚实地面。
6. **多语言完整性 (i18n)**：
   - 严禁在 UI 界面中直接硬编码英文字符串或中文文本。
   - 所有显示文本必须使用 `Localization.T("key")`，并在 `Localization.cs` 中一次性提供 **6 种语言**（English, 简体中文, 繁体中文, 日本語, 한국어, Italiano）的完整翻译字典。
7. **外部模组解耦 (Zero Hard-Dependency)**：
   - 针对第三方模组（如 `PeakAMap`、自定义播放列表 Mod）的功能适配，严禁在工程中添加静态 DLL 引用。
   - 必须通过 `AppDomain.CurrentDomain.GetAssemblies()` 结合反射进行无侵入动态探测（参考 `Utilities.TryGetCustomMapOrPlaylist`）。

---

## 3. 代码库组织与核心模块导览 (Repository Map)

```
e:\CSharp\PEAK-AIO-APEX/
├── .github/workflows/
│   └── build-and-release.yml         # CI/CD：自动化 MSBuild 编译与 GitHub Release/Pre-release
├── dll/
│   └── 20260915-Assembly-CSharp.dll  # 游戏原生逻辑反编译参考基准
├── docs/                             # 游戏底层网络通信协议手册（逆向工程产出）
│   ├── 01_Photon_RPC_Reference.md    # 234 个 PunRPC 接口定义与入参详析
│   ├── 02_Network_Events_and_Custom_Commands.md  # Zorro 封包结构与 Steam Lobby 握手
│   ├── 03_State_Synchronization_Stream.md        # IPunObservable 30Hz 物理状态同步流
│   ├── 04_Modding_Practical_Guide.md             # Harmony 拦截与网络注入实战手册
│   └── README.md                     # 网络协议文档总览
├── PEAK-AIO/                         # 模组主源码目录
│   ├── Properties/AssemblyInfo.cs    # 程序集元数据与版本号
│   ├── References/                   # 19 个本地依赖 DLL（BepInEx, 0Harmony, UnityEngine等）
│   ├── ConfigManager.cs              # BepInEx 配置系统持久化与日志门面
│   ├── ConstantFields.cs             # 游戏私有反射成员（FieldInfo/PropertyInfo）高速单例缓存
│   ├── EventComponent.cs             # 守护 MonoBehaviour：驱动 0.1s/1.0s 轮询、延迟队列与缓存失效
│   ├── GameHelpers.cs                # 角色核心组件弱校验缓存（Character, CharacterMovement 等）
│   ├── Globals.cs                    # 跨模块共享状态、快照字典、UI 滚动条与全局报错中心
│   ├── HarmonyPatch.cs               # 核心功能切面拦截（死亡拦截、跳图补丁、飞行、负重清零）
│   ├── Localization.cs               # 六国语言本地化多层字典与操作系统 CJK 字体动态加载
│   ├── Main.cs                       # 插件生命周期入口、热键捕获、IMGUI 窗口及各大功能分页
│   ├── PEAK-AIO.csproj               # 核心 MSBuild 工程配置文件
│   └── Utilities.cs                  # 核心中台：主线程调度、物品全图扫描与同步、安全对齐、防刷复活
├── build.bat                         # Windows 批处理一键编译入口
├── build.ps1                         # PowerShell 智能探测 MSBuild 编译脚本
└── README.md                         # 面向玩家的安装与功能说明文档
```

---

## 4. 关键架构模式与工程规范 (Design Patterns & Implementations)

### 4.1 主线程调度模式 (UnityMainThreadDispatcher)
所有不在常规 `Update` 帧中触发的操作（如外部异步回调、网络监听器触发、延迟协程）必须封送回主线程：
```csharp
// 正确示范：安全封送到 Unity 主线程
Utilities.UnityMainThreadDispatcher.Enqueue(() =>
{
    if (GameHelpers.LocalCharacter != null)
    {
        // 安全调用 Unity Transform 或 Physics API
        var groundPos = Utilities.ResolveSafeGroundPosition(targetPos);
        GameHelpers.LocalCharacter.transform.position = groundPos;
    }
});
```

### 4.2 反射与实例的分级缓存 (Tiered Caching & Invalidation)
为避免在帧循环中反复执行昂贵的反射和场景树搜索，必须遵循以下缓存策略：
- **静态成员缓存 (`ConstantFields`)**：在类初始化时反射解析 `FieldInfo`、`PropertyInfo`，全周期复用。
- **动态组件弱缓存 (`GameHelpers`)**：缓存本地与远端角色引用，由 `EventComponent.FixedUpdate/Update` 定期调用 `GameHelpers.InvalidateCache()` 进行弱有效性校验（如检测 Unity Object 是否已被 `Destroy`）。
- **世界地表数据节流缓存 (`WorldDataCache`)**：以 0.5s 为节流周期缓存切片数据与篝火点；一旦发生传送或切片跳转，必须**立即显式调用 `WorldDataCache.Invalidate()`**。

### 4.3 装备防刷单次消费令牌 (Atomic Snapshot & Token Lock)
在实现“死亡恢复装备”功能时，传统盲目生成装备会导致复制 Bug。必须遵循以下时序与原子性原则：
1. **捕获快照**：在 `HarmonyPatch.Patch_CharacterDie`（玩家濒死/彻底死亡瞬间）中抓取角色的背包数据快照，存入 `Globals.PlayerInventorySnapshots[steamId]`。
2. **复活消费**：在调用 `Utilities.ReviveSelectedPlayer` 时，若启用 `restoreItems`：
   ```csharp
   if (Globals.PlayerInventorySnapshots.TryGetValue(targetSteamId, out var snapshot))
   {
       // 立即标记并原子移除，防止多次点击或重入导致多重刷取
       Globals.PlayerInventorySnapshots.Remove(targetSteamId);
       snapshot.isConsumed = true;
       // 执行二进制反序列化还原槽位道具...
   }
   ```

### 4.4 防御性物理贴地对齐机制 (Defensive Ground Alignment)
在跳转切片（JumpToSegment）或空间瞬移时，为解决 Unity 物理网格不同步导致的掉落虚空 Bug，必须按此流水线执行：
```csharp
// 1. 强制刷新物理场景变换
Physics.SyncTransforms();

// 2. 多级防御性探测落点
Vector3 safeGround = Utilities.ResolveSafeGroundPosition(rawPosition);

// 3. 将刚体线速度与角速度归零，再设置坐标
var rb = character.GetComponent<Rigidbody>();
if (rb != null)
{
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
}
character.transform.position = safeGround;
```

### 4.5 异常处理与全局浮窗报错 (Global Error Toast)
对于非致命但影响操作结果的运行时异常（如找不到指定物品 Prefab、切片对象为空、RPC 同步超时）：
- 严禁直接吞掉异常；
- 严禁弹出会阻塞游戏的模态对话框；
- **标准做法**：记录到 BepInEx 日志，同时通过 `Globals.GlobalNotifier.ShowError(message)` 推送到界面右下角的自销毁 Toast 队列：
```csharp
try
{
    // 执行高风险操作...
}
catch (Exception ex)
{
    ConfigManager.Logger.LogError($"[ActionName] Failed: {ex}");
    Globals.GlobalNotifier.ShowError($"Action Failed: {ex.Message}");
}
```

---

## 5. 构建、验证与调试流水线 (Build & Verification Workflow)

### 5.1 本地编译指令
在修改代码后，必须通过命令行验证编译是否通过。代码库提供了完善的自动化构建包装：

- **方式 1：PowerShell 脚本（推荐）**
  ```powershell
  .\build.ps1 -Configuration Release
  ```
  *说明*：该脚本会自动探测环境变量、`vswhere.exe` 及 Visual Studio 路径中的 `MSBuild.exe`，执行标准编译，并在成功后将产物从 `PEAK-AIO\bin\Release\PEAK-AIO.dll` 同步复制到项目根目录。

- **方式 2：CMD 批处理**
  ```cmd
  build.bat
  ```

- **方式 3：直接调用 MSBuild**
  ```cmd
  msbuild PEAK-AIO.sln /p:Configuration=Release /p:Platform="Any CPU" /v:minimal
  ```

### 5.2 构建成功验证标准
1. MSBuild 报告 `0 错误，0 警告`（或仅包含不可避免的废弃 API 警告）。
2. 项目根目录下的 `PEAK-AIO.dll` 文件时间戳得到更新。
3. DLL 文件体积通常在 ~150KB - ~300KB 之间，无符号丢失。

### 5.3 运行时调试与日志排障
- **日志文件位置**：`游戏根目录/BepInEx/LogOutput.log`。
- **游戏内调试控制台 (Debug Tab)**：UI 包含专用的调试分页，可通过输入 Slot 索引执行 `Utilities.GetItemsLogs` 输出指定槽位的二进制序列化 Payload、GUID 与内部元数据。
- **配置文件**：`游戏根目录/BepInEx/config/com.onigremlin.peakaio.cfg`，支持在外部直接调整默认开启的作弊开关与呼出快捷键（默认 `Insert`）。

---

## 6. AI 智能体开发任务清单 (Agent Development Checklist)

当 AI 智能体被指派为本项目添加新功能、修复 Bug 或重构代码时，必须对照以下清单逐一核验：

- [ ] **框架与依赖未被破坏**：确认没有引入新的 NuGet 包，`.csproj` 依然保留 `net472` 与本地 `References`。
- [ ] **主线程安全**：确认所有对 Unity 游戏对象的直接操作与射线检测都在主线程（或通过 Dispatcher 调度）。
- [ ] **IMGUI 零开销**：若新增了 UI 控件，确认没有在 `Draw` 方法中每帧执行 `new GUIStyle` 或加载图片资源。
- [ ] **多语言覆盖**：若新增了 UI 标签或提示文本，确认在 `Localization.cs` 的所有 6 个语言字典中添加了对应键值。
- [ ] **物理碰撞与虚空防范**：若新增了位置传送或生物生成，确认使用了 `Physics.SyncTransforms` 和 `ResolveSafeGroundPosition`。
- [ ] **编译构建验证**：在完成代码修改后，务必在命令行运行 `.\build.ps1 -Configuration Release`，确保构建无 Error。
- [ ] **文档与注释保全**：维护代码既有注释与文档结构，新增的核心中台方法必须包含清晰的 XML 三斜杠文档注释。
