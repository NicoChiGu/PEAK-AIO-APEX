using HarmonyLib;
using ImGuiNET;
using Photon.Pun;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[HarmonyPatch(typeof(PointPinger), "ReceivePoint_Rpc")]
public class PointPingPatch
{
    static void Postfix(Vector3 point, Vector3 hitNormal, PointPinger __instance)
    {
        try
        {
            if (!ConfigManager.TeleportToPing.Value)
                return;

            var owner = __instance.character?.photonView?.Owner;
            if (owner != null && owner == PhotonNetwork.LocalPlayer)
            {
                if (Character.localCharacter != null && !Character.localCharacter.data.dead)
                {
                    Vector3 safePoint = point + Vector3.up;
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

        ConfigManager.Logger.LogInfo($"[FlyMod] Flight {(enable ? "enabled" : "disabled")}.");
    }

    public static bool IsFlying => isFlying;

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
            var rig = partList[i]?.Rig;
            if (rig != null)
                rig.linearVelocity = flyVelocity;
        }
    }
}

public static class CJKFontPatch
{
    private static bool fontsLoaded = false;
    private static GCHandle cjkRangesHandle;
    private static GCHandle koreanRangesHandle;

    public static ImFontPtr CjkFont;
    public static ImFontPtr KoreanFont;
    public static bool HasCjkFont = false;
    public static bool HasKoreanFont = false;

    private static readonly ushort[] CjkRanges = {
        0x0020, 0x00FF, // Basic Latin + Latin-1 Supplement
        0x2000, 0x206F, // General Punctuation
        0x3000, 0x30FF, // CJK Symbols, Hiragana, Katakana
        0x31F0, 0x31FF, // Katakana Phonetic Extensions
        0x4E00, 0x9FFF, // CJK Unified Ideographs
        0xFF00, 0xFFEF, // Halfwidth/Fullwidth Forms
        0x0000
    };

    private static readonly ushort[] KoreanRanges = {
        0x0020, 0x00FF, // Basic Latin + Latin-1 Supplement
        0x2000, 0x206F, // General Punctuation
        0x3131, 0x3163, // Korean Alphabets (Jamo)
        0xAC00, 0xD7A3, // Hangul Syllables
        0xFF00, 0xFFEF, // Halfwidth/Fullwidth Forms
        0x0000          // Null terminator
    };

    private static readonly string[] CjkFontCandidates = {
        @"C:\Windows\Fonts\msyh.ttc",
        @"C:\Windows\Fonts\meiryo.ttc",
        @"C:\Windows\Fonts\yugothm.ttc",
    };

    private static readonly string[] KoreanFontCandidates = {
        @"C:\Windows\Fonts\malgun.ttf",
        @"C:\Windows\Fonts\malgunsl.ttf",
    };

    private static string FindFirst(string[] paths)
    {
        foreach (var p in paths)
            if (System.IO.File.Exists(p)) return p;
        return null;
    }

    public static unsafe void Prefix()
    {
        if (fontsLoaded) return;
        fontsLoaded = true;

        try
        {
            var io = ImGui.GetIO();
            var fonts = io.Fonts;

            cjkRangesHandle = GCHandle.Alloc(CjkRanges, GCHandleType.Pinned);
            koreanRangesHandle = GCHandle.Alloc(KoreanRanges, GCHandleType.Pinned);
            IntPtr cjkRangesPtr = cjkRangesHandle.AddrOfPinnedObject();
            IntPtr koreanRangesPtr = koreanRangesHandle.AddrOfPinnedObject();

            string cjkPath = FindFirst(CjkFontCandidates);
            string koreanPath = FindFirst(KoreanFontCandidates);

            if (cjkPath != null)
            {
                CjkFont = fonts.AddFontFromFileTTF(cjkPath, 14.0f, default, cjkRangesPtr);
                HasCjkFont = CjkFont.NativePtr != null;
                ConfigManager.Logger.LogInfo($"[PEAK AIO] CJK font ({System.IO.Path.GetFileName(cjkPath)}): {(HasCjkFont ? "OK" : "FAILED")}");
            }

            if (koreanPath != null)
            {
                KoreanFont = fonts.AddFontFromFileTTF(koreanPath, 14.0f, default, koreanRangesPtr);
                HasKoreanFont = KoreanFont.NativePtr != null;
                ConfigManager.Logger.LogInfo($"[PEAK AIO] Korean font ({System.IO.Path.GetFileName(koreanPath)}): {(HasKoreanFont ? "OK" : "FAILED")}");
            }

            if (HasCjkFont)
                io.NativePtr->FontDefault = CjkFont.NativePtr;
            else if (HasKoreanFont)
                io.NativePtr->FontDefault = KoreanFont.NativePtr;
            else
                ConfigManager.Logger.LogWarning("[PEAK AIO] No CJK/Korean fonts found, using default font.");

            bool built = fonts.Build();
            ConfigManager.Logger.LogInfo($"[PEAK AIO] Atlas build: {(built ? "OK" : "FAILED")}, size: {fonts.TexWidth}x{fonts.TexHeight}, fonts: {fonts.Fonts.Size}");

            if (cjkRangesHandle.IsAllocated) cjkRangesHandle.Free();
            if (koreanRangesHandle.IsAllocated) koreanRangesHandle.Free();
        }
        catch (Exception ex)
        {
            if (cjkRangesHandle.IsAllocated) cjkRangesHandle.Free();
            if (koreanRangesHandle.IsAllocated) koreanRangesHandle.Free();
            ConfigManager.Logger.LogWarning("[PEAK AIO] Font loading failed: " + ex.Message);
        }
    }
}

public static class ImGuiInputPatch
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void ImGuiIO_AddMouseButtonEvent(ImGuiIO* self, int mouse_button, byte mouse_down);

    [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void ImGuiIO_AddMouseWheelEvent(ImGuiIO* self, float wheel_x, float wheel_y);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    private const int VK_LBUTTON = 0x01;
    private const int VK_RBUTTON = 0x02;
    private const int VK_MBUTTON = 0x04;

    private static bool cachedLButton, cachedRButton, cachedMButton;
    private static float cachedScroll;
    private static float cachedMouseX, cachedMouseY;
    private static bool forceInput;
    private static int logFrames;
    private static int renderLogFrames;
    private static bool nativeApiWorks = true;
    private static IntPtr gameWindowHandle = IntPtr.Zero;

    private static IntPtr GetGameWindow()
    {
        if (gameWindowHandle == IntPtr.Zero)
        {
            try { gameWindowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle; }
            catch { }
        }
        return gameWindowHandle;
    }

    public static void SetForceInput(bool enabled) => forceInput = enabled;

    public static void ResetLogs()
    {
        logFrames = 0;
        renderLogFrames = 0;
    }

    public static void CaptureInput()
    {
        cachedLButton = (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        cachedRButton = (GetAsyncKeyState(VK_RBUTTON) & 0x8000) != 0;
        cachedMButton = (GetAsyncKeyState(VK_MBUTTON) & 0x8000) != 0;

        try { cachedScroll = UnityEngine.Input.mouseScrollDelta.y; }
        catch { cachedScroll = 0f; }

        try
        {
            if (GetCursorPos(out POINT screenPos))
            {
                POINT clientPos = screenPos;
                IntPtr hwnd = GetGameWindow();
                if (hwnd != IntPtr.Zero && ScreenToClient(hwnd, ref clientPos))
                {
                    cachedMouseX = clientPos.X;
                    cachedMouseY = clientPos.Y;
                }
            }
        }
        catch { }
    }

    public static unsafe void ApplyToImGui()
    {
        if (!forceInput) return;

        try
        {
            var io = ImGui.GetIO();

            int origFlags = (int)io.ConfigFlags;
            io.ConfigFlags &= ~ImGuiConfigFlags.NoMouse;

            io.MousePos = new System.Numerics.Vector2(cachedMouseX, cachedMouseY);

            if (nativeApiWorks)
            {
                try
                {
                    var ioPtr = io.NativePtr;
                    ImGuiIO_AddMouseButtonEvent(ioPtr, 0, cachedLButton ? (byte)1 : (byte)0);
                    ImGuiIO_AddMouseButtonEvent(ioPtr, 1, cachedRButton ? (byte)1 : (byte)0);
                    ImGuiIO_AddMouseButtonEvent(ioPtr, 2, cachedMButton ? (byte)1 : (byte)0);
                    ImGuiIO_AddMouseWheelEvent(ioPtr, 0f, cachedScroll);
                }
                catch
                {
                    nativeApiWorks = false;
                    ConfigManager.Logger.LogWarning("[InputPatch] Native event API failed, using legacy.");
                }
            }

            io.MouseDown[0] = cachedLButton;
            io.MouseDown[1] = cachedRButton;
            io.MouseDown[2] = cachedMButton;
            io.MouseWheel = cachedScroll;

            if (logFrames < 5)
            {
                logFrames++;
                bool hadNoMouse = (origFlags & (int)ImGuiConfigFlags.NoMouse) != 0;
                ConfigManager.Logger.LogInfo(
                    $"[InputPatch] nativeApi={nativeApiWorks} hadNoMouse={hadNoMouse} " +
                    $"win32Pos=({cachedMouseX:F0},{cachedMouseY:F0}) " +
                    $"imguiPos=({io.MousePos.X:F0},{io.MousePos.Y:F0}) " +
                    $"displaySize=({io.DisplaySize.X:F0},{io.DisplaySize.Y:F0}) " +
                    $"hwnd={gameWindowHandle}");
            }
        }
        catch (Exception ex)
        {
            if (logFrames < 5)
            {
                logFrames++;
                ConfigManager.Logger.LogError($"[InputPatch] Error: {ex}");
            }
        }
    }

    public static void LogPostNewFrame()
    {
        if (!forceInput) return;
        if (renderLogFrames >= 15) return;

        try
        {
            var io = ImGui.GetIO();
            var winPos = ImGui.GetWindowPos();
            var winSize = ImGui.GetWindowSize();
            bool winHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows | ImGuiHoveredFlags.AllowWhenBlockedByActiveItem);
            bool anyItemHovered = ImGui.IsAnyItemHovered();
            bool winFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);
            bool rectHover = ImGui.IsMouseHoveringRect(
                winPos,
                new System.Numerics.Vector2(winPos.X + winSize.X, winPos.Y + winSize.Y),
                false);

            if (cachedLButton)
            {
                renderLogFrames++;
                ConfigManager.Logger.LogInfo(
                    $"[InputPatch-Render] CLICK " +
                    $"winHovered={winHovered} rectHover={rectHover} winFocused={winFocused} " +
                    $"wantCapture={io.WantCaptureMouse} configFlags=0x{(int)io.ConfigFlags:X} " +
                    $"imguiPos=({io.MousePos.X:F0},{io.MousePos.Y:F0}) " +
                    $"winPos=({winPos.X:F0},{winPos.Y:F0}) winSize=({winSize.X:F0},{winSize.Y:F0})");
            }
            else if (renderLogFrames < 5)
            {
                renderLogFrames++;
                ConfigManager.Logger.LogInfo(
                    $"[InputPatch-Render] " +
                    $"winHovered={winHovered} rectHover={rectHover} winFocused={winFocused} " +
                    $"wantCapture={io.WantCaptureMouse} configFlags=0x{(int)io.ConfigFlags:X} " +
                    $"imguiPos=({io.MousePos.X:F0},{io.MousePos.Y:F0}) " +
                    $"winPos=({winPos.X:F0},{winPos.Y:F0}) winSize=({winSize.X:F0},{winSize.Y:F0})");
            }
        }
        catch { }
    }

    public static void Prefix()
    {
        ApplyToImGui();
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
            cachedLocalAfflictions = localChar.GetComponent<CharacterAfflictions>();
        }

        if (ReferenceEquals(__instance, cachedLocalAfflictions))
        {
            __instance.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, 0f);
        }
    }
}
