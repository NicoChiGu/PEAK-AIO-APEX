using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[BepInPlugin("com.onigremlin.peakaio", "PEAK AIO Mod", "1.2.0")]
public class PeakMod : BaseUnityPlugin
{
    public static bool IsMenuOpen = false;
    private bool showMenu = false;
    private int selectedTab = 1;
    private int pendingTab = 1;

    // Coordinate inputs
    private string coordXStr = "0";
    private string coordYStr = "0";
    private string coordZStr = "0";

    // GUI Styling
    private bool skinInitialized = false;
    private GUISkin customSkin;
    private GUIStyle windowStyle;
    private GUIStyle sidebarBtnStyle;
    private GUIStyle sidebarActiveBtnStyle;
    private GUIStyle cardBoxStyle;
    private GUIStyle primaryBtnStyle;
    private GUIStyle dangerBtnStyle;
    private GUIStyle regularBtnStyle;
    private GUIStyle sectionHeaderStyle;
    private GUIStyle subHeaderStyle;
    private GUIStyle labelStyle;
    private GUIStyle boldLabelStyle;
    private GUIStyle tipLabelStyle;
    private GUIStyle textInputStyle;
    private GUIStyle itemSelectableStyle;
    private GUIStyle itemSelectedStyle;
    private GUIStyle toggleStyle;
    private GUIStyle ribbonCardStyle;
    private GUIStyle badgeCurrentStyle;
    private GUIStyle badgeCompletedStyle;
    private GUIStyle badgePendingStyle;
    private GUIStyle flowArrowStyle;
    private GUIStyle stageTagStyle;
    private GUIStyle stageTagActiveStyle;

    // Solid Color Textures
    private Texture2D texCanvasTan;
    private Texture2D texBadgeBrown;
    private Texture2D texLogInk;
    private Texture2D texSidebarGreen;
    private Texture2D texTrailDust;
    private Texture2D texTrailDustHover;
    private Texture2D texTrailDustActive;
    private Texture2D texRopeBrown;
    private Texture2D texLightGreen;
    private Texture2D texScoutRed;
    private Texture2D texCardBg;
    private Texture2D texRibbonCard;
    private Texture2D texBadgeCur;
    private Texture2D texBadgeComp;
    private Texture2D texBadgePend;

    private static Texture2D MakeSolidTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void Awake()
    {
        Logger.LogInfo("[PEAK AIO] Initializing Mod (Native Unity GUI)...");
        this.gameObject.AddComponent<EventComponent>();
        this.gameObject.AddComponent<AutoReconnectService>();
    }

    private void OnEnable()
    {
        Logger.LogInfo("[PEAK AIO] OnEnable called");

        Globals.itemSearchBuffers = new string[3] { "", "", "" };
        ConfigManager.Init(Config, Logger);

        try
        {
            var harmony = new Harmony("com.onigremlin.peakaio");
            harmony.PatchAll();
            Logger.LogInfo("[PEAK AIO] Harmony patches applied successfully.");
        }
        catch (Exception ex)
        {
            Logger.LogError("[PEAK AIO] Harmony PatchAll failed: " + ex);
        }
    }

    private void OnDisable()
    {
        Logger.LogInfo("[PEAK AIO] OnDisable called");
        if (showMenu)
        {
            showMenu = false;
            IsMenuOpen = false;
            RestoreCursor();
        }
    }

    private void Update()
    {
        UnityMainThreadDispatcher.Update();

        if (UnityEngine.Input.GetKeyDown(ConfigManager.MenuToggleKey.Value))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        showMenu = !showMenu;
        IsMenuOpen = showMenu;
        if (showMenu)
        {
            CaptureCursor();
        }
        else
        {
            RestoreCursor();
        }
    }

    private void CaptureCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitStyles()
    {
        if (skinInitialized) return;

        // Colors
        Color canvasTan = new Color(0.953f, 0.941f, 0.902f, 1.0f);
        Color badgeBrown = new Color(0.361f, 0.294f, 0.231f, 1.0f);
        Color logInk = new Color(0.18f, 0.18f, 0.18f, 1.0f);
        Color sidebarGreen = new Color(0.18f, 0.28f, 0.22f, 1.0f);
        Color trailDust = new Color(0.866f, 0.827f, 0.741f, 1.0f);
        Color trailDustHover = new Color(0.80f, 0.78f, 0.65f, 1.0f);
        Color trailDustActive = new Color(0.75f, 0.72f, 0.61f, 1.0f);
        Color ropeBrown = new Color(0.55f, 0.42f, 0.28f, 1.0f);
        Color lightGreen = new Color(0.318f, 0.569f, 0.384f, 1.0f);
        Color scoutRed = new Color(0.76f, 0.44f, 0.39f, 1.0f);
        Color cardBg = new Color(0.91f, 0.88f, 0.82f, 1.0f);

        // Textures
        texCanvasTan = MakeSolidTex(2, 2, canvasTan);
        texBadgeBrown = MakeSolidTex(2, 2, badgeBrown);
        texLogInk = MakeSolidTex(2, 2, logInk);
        texSidebarGreen = MakeSolidTex(2, 2, sidebarGreen);
        texTrailDust = MakeSolidTex(2, 2, trailDust);
        texTrailDustHover = MakeSolidTex(2, 2, trailDustHover);
        texTrailDustActive = MakeSolidTex(2, 2, trailDustActive);
        texRopeBrown = MakeSolidTex(2, 2, ropeBrown);
        texLightGreen = MakeSolidTex(2, 2, lightGreen);
        texScoutRed = MakeSolidTex(2, 2, scoutRed);
        texCardBg = MakeSolidTex(2, 2, cardBg);

        Color ribbonBg = new Color(0.95f, 0.93f, 0.88f, 1.0f);
        Color badgeCurCol = new Color(0.22f, 0.52f, 0.32f, 1.0f);
        Color badgeCompCol = new Color(0.42f, 0.62f, 0.46f, 0.92f);
        Color badgePendCol = new Color(0.86f, 0.82f, 0.75f, 1.0f);

        texRibbonCard = MakeSolidTex(2, 2, ribbonBg);
        texBadgeCur = MakeSolidTex(2, 2, badgeCurCol);
        texBadgeComp = MakeSolidTex(2, 2, badgeCompCol);
        texBadgePend = MakeSolidTex(2, 2, badgePendCol);

        // Clone base skin to retain all default controls and scrollbars
        customSkin = Instantiate(GUI.skin);
        try
        {
            string[] fontCandidates = new string[] {
                "Microsoft YaHei", "PingFang SC", "Noto Sans CJK SC", "Malgun Gothic", "Meiryo", "Arial"
            };
            Font dynamicFont = Font.CreateDynamicFontFromOSFont(fontCandidates, 12);
            if (dynamicFont != null)
            {
                customSkin.font = dynamicFont;
            }
        }
        catch { }

        // Window style
        windowStyle = new GUIStyle(customSkin.window);
        windowStyle.normal.background = texCanvasTan;
        windowStyle.onNormal.background = texCanvasTan;
        windowStyle.normal.textColor = logInk;
        windowStyle.onNormal.textColor = logInk;
        windowStyle.fontSize = 14;
        windowStyle.fontStyle = FontStyle.Bold;
        windowStyle.padding = new RectOffset(8, 8, 24, 8);
        windowStyle.border = new RectOffset(0, 0, 0, 0);
        customSkin.window = windowStyle;

        // Default button style
        GUIStyle defaultBtn = new GUIStyle(customSkin.button);
        defaultBtn.normal.background = texTrailDust;
        defaultBtn.hover.background = texTrailDustHover;
        defaultBtn.active.background = texTrailDustActive;
        defaultBtn.normal.textColor = logInk;
        defaultBtn.hover.textColor = logInk;
        defaultBtn.active.textColor = logInk;
        defaultBtn.fontSize = 12;
        defaultBtn.alignment = TextAnchor.MiddleCenter;
        defaultBtn.margin = new RectOffset(2, 2, 2, 2);
        defaultBtn.padding = new RectOffset(6, 6, 4, 4);
        customSkin.button = defaultBtn;
        regularBtnStyle = defaultBtn;

        // Primary Button
        primaryBtnStyle = new GUIStyle(defaultBtn);
        primaryBtnStyle.normal.background = texLightGreen;
        primaryBtnStyle.hover.background = MakeSolidTex(2, 2, new Color(0.36f, 0.63f, 0.43f, 1.0f));
        primaryBtnStyle.active.background = MakeSolidTex(2, 2, new Color(0.28f, 0.50f, 0.34f, 1.0f));
        primaryBtnStyle.normal.textColor = Color.white;
        primaryBtnStyle.hover.textColor = Color.white;
        primaryBtnStyle.active.textColor = Color.white;
        primaryBtnStyle.fontStyle = FontStyle.Bold;

        // Danger Button
        dangerBtnStyle = new GUIStyle(defaultBtn);
        dangerBtnStyle.normal.background = texScoutRed;
        dangerBtnStyle.hover.background = MakeSolidTex(2, 2, new Color(0.82f, 0.50f, 0.45f, 1.0f));
        dangerBtnStyle.active.background = MakeSolidTex(2, 2, new Color(0.68f, 0.38f, 0.33f, 1.0f));
        dangerBtnStyle.normal.textColor = Color.white;
        dangerBtnStyle.hover.textColor = Color.white;
        dangerBtnStyle.active.textColor = Color.white;
        dangerBtnStyle.fontStyle = FontStyle.Bold;

        // Sidebar Buttons
        sidebarBtnStyle = new GUIStyle(defaultBtn);
        sidebarBtnStyle.normal.background = texTrailDust;
        sidebarBtnStyle.hover.background = texTrailDustHover;
        sidebarBtnStyle.normal.textColor = logInk;
        sidebarBtnStyle.fontSize = 11;
        sidebarBtnStyle.fixedHeight = 32;
        sidebarBtnStyle.fontStyle = FontStyle.Bold;

        sidebarActiveBtnStyle = new GUIStyle(sidebarBtnStyle);
        sidebarActiveBtnStyle.normal.background = texSidebarGreen;
        sidebarActiveBtnStyle.hover.background = texSidebarGreen;
        sidebarActiveBtnStyle.active.background = texSidebarGreen;
        sidebarActiveBtnStyle.normal.textColor = Color.white;
        sidebarActiveBtnStyle.hover.textColor = Color.white;
        sidebarActiveBtnStyle.active.textColor = Color.white;

        // Card Box
        cardBoxStyle = new GUIStyle(customSkin.box);
        cardBoxStyle.normal.background = texCardBg;
        cardBoxStyle.normal.textColor = logInk;
        cardBoxStyle.padding = new RectOffset(8, 8, 8, 8);
        cardBoxStyle.margin = new RectOffset(2, 2, 2, 2);
        cardBoxStyle.border = new RectOffset(0, 0, 0, 0);
        customSkin.box = cardBoxStyle;

        // Section Header
        sectionHeaderStyle = new GUIStyle(customSkin.label);
        sectionHeaderStyle.normal.textColor = sidebarGreen;
        sectionHeaderStyle.fontSize = 13;
        sectionHeaderStyle.fontStyle = FontStyle.Bold;
        sectionHeaderStyle.margin = new RectOffset(0, 0, 4, 2);

        // Subheader
        subHeaderStyle = new GUIStyle(customSkin.label);
        subHeaderStyle.normal.textColor = badgeBrown;
        subHeaderStyle.fontSize = 12;
        subHeaderStyle.fontStyle = FontStyle.Bold;
        subHeaderStyle.margin = new RectOffset(0, 0, 2, 2);

        // Standard Label
        labelStyle = new GUIStyle(customSkin.label);
        labelStyle.normal.textColor = logInk;
        labelStyle.fontSize = 12;
        labelStyle.wordWrap = true;
        customSkin.label = labelStyle;

        boldLabelStyle = new GUIStyle(labelStyle);
        boldLabelStyle.fontStyle = FontStyle.Bold;

        tipLabelStyle = new GUIStyle(labelStyle);
        tipLabelStyle.normal.textColor = ropeBrown;
        tipLabelStyle.fontSize = 10;

        // Text input
        textInputStyle = new GUIStyle(customSkin.textField);
        textInputStyle.normal.background = texTrailDust;
        textInputStyle.focused.background = texTrailDustHover;
        textInputStyle.normal.textColor = logInk;
        textInputStyle.focused.textColor = logInk;
        textInputStyle.fontSize = 12;
        textInputStyle.padding = new RectOffset(4, 4, 3, 3);
        customSkin.textField = textInputStyle;

        // Selectables
        itemSelectableStyle = new GUIStyle(customSkin.button);
        itemSelectableStyle.normal.background = texTrailDust;
        itemSelectableStyle.hover.background = texTrailDustHover;
        itemSelectableStyle.normal.textColor = logInk;
        itemSelectableStyle.fontSize = 11;
        itemSelectableStyle.alignment = TextAnchor.MiddleLeft;
        itemSelectableStyle.padding = new RectOffset(6, 4, 3, 3);
        itemSelectableStyle.margin = new RectOffset(1, 1, 1, 1);
        itemSelectableStyle.fixedHeight = 22;

        itemSelectedStyle = new GUIStyle(itemSelectableStyle);
        itemSelectedStyle.normal.background = texLightGreen;
        itemSelectedStyle.hover.background = texLightGreen;
        itemSelectedStyle.normal.textColor = Color.white;
        itemSelectedStyle.hover.textColor = Color.white;

        // Toggle
        toggleStyle = new GUIStyle(customSkin.toggle);
        toggleStyle.normal.textColor = logInk;
        toggleStyle.hover.textColor = logInk;
        toggleStyle.active.textColor = logInk;
        toggleStyle.onNormal.textColor = sidebarGreen;
        toggleStyle.onHover.textColor = sidebarGreen;
        toggleStyle.onActive.textColor = sidebarGreen;
        toggleStyle.fontSize = 12;
        toggleStyle.fontStyle = FontStyle.Normal;
        customSkin.toggle = toggleStyle;

        // Horizontal Slider
        GUIStyle sliderTrack = new GUIStyle(customSkin.horizontalSlider);
        sliderTrack.normal.background = texTrailDust;
        sliderTrack.fixedHeight = 8;
        customSkin.horizontalSlider = sliderTrack;

        GUIStyle sliderThumb = new GUIStyle(customSkin.horizontalSliderThumb);
        sliderThumb.normal.background = texSidebarGreen;
        sliderThumb.hover.background = texLightGreen;
        sliderThumb.active.background = texSidebarGreen;
        sliderThumb.fixedWidth = 14;
        sliderThumb.fixedHeight = 14;
        customSkin.horizontalSliderThumb = sliderThumb;

        // Scrollbars
        GUIStyle vScroll = new GUIStyle(customSkin.verticalScrollbar);
        vScroll.normal.background = texBadgeBrown;
        vScroll.fixedWidth = 10;
        customSkin.verticalScrollbar = vScroll;

        GUIStyle vScrollThumb = new GUIStyle(customSkin.verticalScrollbarThumb);
        vScrollThumb.normal.background = texSidebarGreen;
        vScrollThumb.hover.background = texLightGreen;
        vScrollThumb.active.background = texSidebarGreen;
        vScrollThumb.fixedWidth = 10;
        customSkin.verticalScrollbarThumb = vScrollThumb;

        // Ribbon Card Box (Paper Ticket style)
        ribbonCardStyle = new GUIStyle(customSkin.box);
        ribbonCardStyle.normal.background = texRibbonCard;
        ribbonCardStyle.normal.textColor = logInk;
        ribbonCardStyle.padding = new RectOffset(6, 6, 6, 6);
        ribbonCardStyle.margin = new RectOffset(2, 2, 2, 2);

        // Badges for Flight Route Ribbon
        badgeCurrentStyle = new GUIStyle(customSkin.label);
        badgeCurrentStyle.normal.background = texBadgeCur;
        badgeCurrentStyle.normal.textColor = Color.white;
        badgeCurrentStyle.fontSize = 11;
        badgeCurrentStyle.fontStyle = FontStyle.Bold;
        badgeCurrentStyle.alignment = TextAnchor.MiddleCenter;
        badgeCurrentStyle.padding = new RectOffset(7, 7, 3, 3);
        badgeCurrentStyle.margin = new RectOffset(1, 1, 1, 1);

        badgeCompletedStyle = new GUIStyle(customSkin.label);
        badgeCompletedStyle.normal.background = texBadgeComp;
        badgeCompletedStyle.normal.textColor = Color.white;
        badgeCompletedStyle.fontSize = 11;
        badgeCompletedStyle.alignment = TextAnchor.MiddleCenter;
        badgeCompletedStyle.padding = new RectOffset(6, 6, 3, 3);
        badgeCompletedStyle.margin = new RectOffset(1, 1, 1, 1);

        badgePendingStyle = new GUIStyle(customSkin.label);
        badgePendingStyle.normal.background = texBadgePend;
        badgePendingStyle.normal.textColor = logInk;
        badgePendingStyle.fontSize = 11;
        badgePendingStyle.alignment = TextAnchor.MiddleCenter;
        badgePendingStyle.padding = new RectOffset(6, 6, 3, 3);
        badgePendingStyle.margin = new RectOffset(1, 1, 1, 1);

        flowArrowStyle = new GUIStyle(customSkin.label);
        flowArrowStyle.normal.textColor = ropeBrown;
        flowArrowStyle.fontSize = 12;
        flowArrowStyle.fontStyle = FontStyle.Bold;
        flowArrowStyle.alignment = TextAnchor.MiddleCenter;
        flowArrowStyle.padding = new RectOffset(0, 0, 2, 0);

        stageTagStyle = new GUIStyle(badgePendingStyle);
        stageTagStyle.fontStyle = FontStyle.Bold;
        stageTagStyle.fontSize = 10;
        stageTagStyle.normal.textColor = ropeBrown;

        stageTagActiveStyle = new GUIStyle(badgeCurrentStyle);
        stageTagActiveStyle.fontSize = 10;

        skinInitialized = true;
    }

    private void OnGUI()
    {
        InitStyles();

        DrawGlobalErrorToast();

        if (!showMenu)
            return;

        GUISkin prevSkin = GUI.skin;
        try
        {
            GUI.skin = customSkin;

            // Ensure window stays within screen bounds
            Globals.windowRect.x = Mathf.Clamp(Globals.windowRect.x, 0, Mathf.Max(0, Screen.width - Globals.windowRect.width));
            Globals.windowRect.y = Mathf.Clamp(Globals.windowRect.y, 0, Mathf.Max(0, Screen.height - Globals.windowRect.height));

            Globals.windowRect = GUILayout.Window(9999, Globals.windowRect, DrawWindow, "PEAK AIO [APEX Edition]", GUILayout.Width(780), GUILayout.Height(520));
        }
        finally
        {
            GUI.skin = prevSkin;
        }
    }

    private void DrawGlobalErrorToast()
    {
        if (string.IsNullOrEmpty(Globals.GlobalNotifier.CurrentErrorMessage))
            return;

        float remaining = Globals.GlobalNotifier.ExpireTime - Time.unscaledTime;
        if (remaining <= 0f)
        {
            Globals.GlobalNotifier.Clear();
            return;
        }

        float toastW = 460f;
        float toastH = 70f;
        float toastX = (Screen.width - toastW) * 0.5f;
        float toastY = 30f;
        Rect toastRect = new Rect(toastX, toastY, toastW, toastH);

        GUISkin prevSkin = GUI.skin;
        int prevDepth = GUI.depth;
        try
        {
            GUI.skin = customSkin;
            GUI.depth = -10000;

            GUI.Box(toastRect, GUIContent.none, cardBoxStyle);

            GUILayout.BeginArea(toastRect);
            try
            {
                GUILayout.BeginHorizontal();
                bool isSuccess = (Globals.GlobalNotifier.CurrentNotificationType == "SUCCESS");
                string color = isSuccess ? "#4ea359" : "#c24338";
                string prefix = isSuccess ? "[SUCCESS]" : string.Format("[{0}]", Localization.T("error.title"));
                string title = string.Format("<color={0}><b>{1}</b></color> ({2}: {3:F1}s)",
                    color,
                    prefix,
                    Localization.T("error.auto_close"),
                    remaining);
                GUILayout.Label(title, boldLabelStyle);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("✕", dangerBtnStyle, GUILayout.Width(24), GUILayout.Height(20)))
                {
                    Globals.GlobalNotifier.Clear();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                GUILayout.Label(Globals.GlobalNotifier.CurrentErrorMessage, labelStyle);
            }
            finally
            {
                GUILayout.EndArea();
            }
        }
        finally
        {
            GUI.depth = prevDepth;
            GUI.skin = prevSkin;
        }
    }

    private void DrawWindow(int windowId)
    {
        if (Event.current.type == EventType.Layout)
        {
            if (selectedTab != pendingTab)
            {
                selectedTab = pendingTab;
                Globals.mainScroll = Vector2.zero;
            }
        }

        // Close button at top right
        if (GUI.Button(new Rect(Globals.windowRect.width - 26, 3, 22, 20), "X", dangerBtnStyle))
        {
            ToggleMenu();
            return;
        }

        GUILayout.BeginHorizontal();

        // 1. Left Sidebar
        GUILayout.BeginVertical(GUILayout.Width(115));
        try
        {
            DrawSidebar();
        }
        catch (Exception ex)
        {
            GUILayout.Label("Sidebar Error: " + ex.Message, tipLabelStyle);
            if (Logger != null)
                Logger.LogError("[PEAK AIO] Error in DrawSidebar: " + ex);
        }
        finally
        {
            GUILayout.EndVertical();
        }

        GUILayout.Space(6);

        // 2. Main Content Area
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        try
        {
            DrawMainArea();
        }
        catch (Exception ex)
        {
            GUILayout.Label("Error rendering tab: " + ex.Message, labelStyle);
            if (Logger != null)
                Logger.LogError("[PEAK AIO] Error in DrawMainArea: " + ex);
        }
        finally
        {
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();

        // Drag bar across the top header
        GUI.DragWindow(new Rect(0, 0, Globals.windowRect.width - 28, 24));
    }

    private void DrawSidebar()
    {
        string[] sidebarKeys = new string[] {
            "tab.player", "tab.items", "tab.lobby", "tab.world", "tab.creatures", "tab.achievements", "tab.about", "tab.language", "tab.debug"
        };

        for (int i = 0; i < sidebarKeys.Length; i++)
        {
            int tabIndex = i + 1;
            bool isSelected = (pendingTab == tabIndex);
            string label = Localization.T(sidebarKeys[i]);

            GUIStyle style = isSelected ? sidebarActiveBtnStyle : sidebarBtnStyle;
            if (GUILayout.Button(label, style))
            {
                if (pendingTab != tabIndex)
                {
                    pendingTab = tabIndex;
                }
            }
            GUILayout.Space(2);
        }

        GUILayout.FlexibleSpace();

        // Menu Hotkey indicator
        GUILayout.Label(string.Format("[{0}]", ConfigManager.MenuToggleKey.Value), tipLabelStyle);
    }

    private void DrawMainArea()
    {
        switch (selectedTab)
        {
            case 1:
                DrawPlayerTab();
                break;
            case 2:
                DrawItemsTab();
                break;
            case 3:
                DrawLobbyTab();
                break;
            case 4:
                DrawWorldTab();
                break;
            case 5:
                DrawCreaturesTab();
                break;
            case 6:
                DrawAchievementsTab();
                break;
            case 7:
                DrawAboutTab();
                break;
            case 8:
                DrawLanguageTab();
                break;
            case 9:
                DrawDebugTab();
                break;
            default:
                DrawPlayerTab();
                break;
        }
    }

    // ==========================================
    // TAB 1: PLAYER
    // ==========================================
    private void DrawPlayerTab()
    {
        Globals.mainScroll = GUILayout.BeginScrollView(Globals.mainScroll);

        GUILayout.BeginHorizontal();

        // Left Column: Self Mods & Teleport
        GUILayout.BeginVertical(GUILayout.Width(310));

        GUILayout.Label(Localization.T("player.selfmods"), sectionHeaderStyle);

        DrawCheckbox(ConfigManager.InfiniteStamina, Localization.T("player.infinite_stamina"), (val) =>
        {
            var character = GameHelpers.GetCharacterComponent();
            var prop = ConstantFields.GetInfiniteStaminaProperty();
            if (character != null && prop != null)
                prop.SetValue(character, val);
        });

        DrawCheckbox(ConfigManager.LockStatus, Localization.T("player.freeze_afflictions"), (val) =>
        {
            var character = GameHelpers.GetCharacterComponent();
            var prop = ConstantFields.GetStatusLockProperty();
            if (character != null && prop != null)
                prop.SetValue(character, val);
        });

        // Clear All Afflictions button
        if (GUILayout.Button(Localization.T("player.clear_afflictions"), primaryBtnStyle, GUILayout.Height(24)))
        {
            Utilities.ClearAllAfflictions();
        }
        GUILayout.Label(Localization.T("tip.clear_afflictions"), tipLabelStyle);
        GUILayout.Space(4);

        DrawCheckbox(ConfigManager.NoWeight, Localization.T("player.no_weight"));

        DrawCheckbox(ConfigManager.SpeedMod, Localization.T("player.change_speed"), (val) =>
        {
            var movement = GameHelpers.GetMovementComponent();
            var field = ConstantFields.GetMovementModifierField();
            if (movement != null && field != null)
                field.SetValue(movement, ConfigManager.SpeedAmount.Value);
        });

        DrawCheckbox(ConfigManager.JumpMod, Localization.T("player.change_jump"), (val) =>
        {
            var movement = GameHelpers.GetMovementComponent();
            var jumpField = ConstantFields.GetJumpGravityField();
            var fallField = ConstantFields.GetFallDamageTimeField();
            if (movement != null && jumpField != null)
                jumpField.SetValue(movement, ConfigManager.JumpAmount.Value);
            if (movement != null && fallField != null)
                fallField.SetValue(movement, ConfigManager.NoFallDmg.Value ? 999f : 1.5f);
        });

        DrawCheckbox(ConfigManager.ClimbMod, Localization.T("player.change_climb"), (val) =>
        {
            var climb = GameHelpers.GetClimbingComponent();
            var field = ConstantFields.GetClimbSpeedModField();
            if (climb != null && field != null)
                field.SetValue(climb, ConfigManager.ClimbAmount.Value);
        });

        DrawCheckbox(ConfigManager.VineClimbMod, Localization.T("player.change_vine_climb"), (val) =>
        {
            var vine = GameHelpers.GetVineClimbComponent();
            var field = ConstantFields.GetVineClimbSpeedModField();
            if (vine != null && field != null)
                field.SetValue(vine, ConfigManager.VineClimbAmount.Value);
        });

        DrawCheckbox(ConfigManager.RopeClimbMod, Localization.T("player.change_rope_climb"), (val) =>
        {
            var rope = GameHelpers.GetRopeClimbComponent();
            var field = ConstantFields.GetRopeClimbSpeedModField();
            if (rope != null && field != null)
                field.SetValue(rope, ConfigManager.RopeClimbAmount.Value);
        });

        DrawCheckbox(ConfigManager.TeleportToPing, Localization.T("player.teleport_to_ping"));

        DrawCheckbox(ConfigManager.FlyMod, Localization.T("player.fly_mode"), FlyPatch.SetFlying);

        GUILayout.Space(6);
        if (GUILayout.Button(Localization.T("player.spawn_backpack"), GUILayout.Height(24)))
        {
            Utilities.GivePlayerBackpack(Player.localPlayer);
        }

        GUILayout.Space(8);
        GUILayout.Label(Localization.T("player.teleport"), sectionHeaderStyle);

        GUILayout.BeginHorizontal();
        GUILayout.Label("X:", GUILayout.Width(18));
        coordXStr = GUILayout.TextField(coordXStr, GUILayout.Width(70));
        GUILayout.Label("Y:", GUILayout.Width(18));
        coordYStr = GUILayout.TextField(coordYStr, GUILayout.Width(70));
        GUILayout.Label("Z:", GUILayout.Width(18));
        coordZStr = GUILayout.TextField(coordZStr, GUILayout.Width(70));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Localization.T("player.teleport_to_coords"), GUILayout.Height(24)))
        {
            float x, y, z;
            float.TryParse(coordXStr, out x);
            float.TryParse(coordYStr, out y);
            float.TryParse(coordZStr, out z);
            Utilities.TeleportToCoords(x, y, z);
        }

        if (GUILayout.Button("Get Coords", GUILayout.Width(85), GUILayout.Height(24)))
        {
            if (Character.localCharacter != null)
            {
                var pos = Utilities.GetCharacterPosition(Character.localCharacter);
                coordXStr = pos.x.ToString("F1");
                coordYStr = pos.y.ToString("F1");
                coordZStr = pos.z.ToString("F1");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Space(12);

        // Right Column: Status & Afflictions Management & Modifiers / Sliders
        GUILayout.BeginVertical(GUILayout.Width(300));

        // --- Status Effects & Debuffs Section ---
        GUILayout.BeginVertical(cardBoxStyle);
        GUILayout.Label(Localization.T("status.panel_title"), subHeaderStyle);

        // Clear All Statuses button
        if (GUILayout.Button(Localization.T("status.clear_all"), primaryBtnStyle, GUILayout.Height(26)))
        {
            Utilities.ClearAllAfflictions();
        }
        GUILayout.Label(Localization.T("tip.clear_afflictions"), tipLabelStyle);
        GUILayout.Space(4);

        // Status Selector with < and >
        int curStatusIdx = Mathf.Clamp(Globals.selfSelectedStatusIndex, 0, Globals.AllStatusTypes.Length - 1);
        CharacterAfflictions.STATUSTYPE curType = Globals.AllStatusTypes[curStatusIdx];
        string curTypeName = Localization.GetStatusTypeName(curType);

        GUILayout.Label(Localization.T("status.select_effect") + ":", boldLabelStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(22)))
        {
            Globals.selfSelectedStatusIndex = (curStatusIdx - 1 + Globals.AllStatusTypes.Length) % Globals.AllStatusTypes.Length;
        }
        GUILayout.Label(curTypeName, subHeaderStyle, GUILayout.ExpandWidth(true));
        if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(22)))
        {
            Globals.selfSelectedStatusIndex = (curStatusIdx + 1) % Globals.AllStatusTypes.Length;
        }
        GUILayout.EndHorizontal();

        // Intensity Slider
        GUILayout.Space(2);
        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("{0}: {1:P0}", Localization.T("status.amount"), Globals.selfStatusAmount), GUILayout.Width(110));
        Globals.selfStatusAmount = GUILayout.HorizontalSlider(Globals.selfStatusAmount, 0.1f, 1.0f);
        GUILayout.EndHorizontal();

        // Apply & Reduce Buttons
        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Localization.T("status.add_effect"), dangerBtnStyle, GUILayout.Height(24)))
        {
            var charObj = Character.localCharacter;
            if (charObj != null)
            {
                Utilities.AddStatusEffect(charObj, curType, Globals.selfStatusAmount);
            }
        }
        if (GUILayout.Button(Localization.T("status.subtract_effect"), GUILayout.Height(24)))
        {
            var charObj = Character.localCharacter;
            if (charObj != null)
            {
                Utilities.SubtractStatusEffect(charObj, curType, Globals.selfStatusAmount);
            }
        }
        GUILayout.EndHorizontal();

        // Presets: Divine Purify / Near Death / Extreme Torture
        GUILayout.Space(6);
        GUILayout.Label("快捷预设 (Presets):", boldLabelStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Localization.T("status.preset_purify"), primaryBtnStyle, GUILayout.Height(22)))
        {
            Utilities.ApplyDivinePurify(Character.localCharacter);
        }
        if (GUILayout.Button(Localization.T("status.preset_critical"), dangerBtnStyle, GUILayout.Height(22)))
        {
            Utilities.ApplyCriticalInjury(Character.localCharacter);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(2);
        if (GUILayout.Button(Localization.T("status.preset_torture"), dangerBtnStyle, GUILayout.Height(22)))
        {
            Utilities.ApplyTorturePreset(Character.localCharacter);
        }

        GUILayout.EndVertical();

        GUILayout.Space(8);

        GUILayout.Label(Localization.T("player.details"), sectionHeaderStyle);

        if (ConfigManager.JumpMod.Value)
        {
            GUILayout.BeginVertical(cardBoxStyle);
            DrawCheckbox(ConfigManager.NoFallDmg, Localization.T("player.no_fall_dmg"));
            DrawSliderFloat(ConfigManager.JumpAmount, Localization.T("player.jump_mult"), 10.0f, 500.0f, "{0:F0}");
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        if (ConfigManager.SpeedMod.Value)
        {
            GUILayout.BeginVertical(cardBoxStyle);
            DrawSliderFloat(ConfigManager.SpeedAmount, Localization.T("player.move_speed"), 1.0f, 20.0f, "{0:F1}x");
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        if (ConfigManager.ClimbMod.Value)
        {
            GUILayout.BeginVertical(cardBoxStyle);
            DrawSliderFloat(ConfigManager.ClimbAmount, Localization.T("player.climb_speed"), 1.0f, 20.0f, "{0:F1}x");
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        if (ConfigManager.VineClimbMod.Value)
        {
            GUILayout.BeginVertical(cardBoxStyle);
            DrawSliderFloat(ConfigManager.VineClimbAmount, Localization.T("player.vine_speed"), 1.0f, 20.0f, "{0:F1}x");
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        if (ConfigManager.RopeClimbMod.Value)
        {
            GUILayout.BeginVertical(cardBoxStyle);
            DrawSliderFloat(ConfigManager.RopeClimbAmount, Localization.T("player.rope_speed"), 1.0f, 20.0f, "{0:F1}x");
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        if (ConfigManager.FlyMod.Value)
        {
            GUILayout.BeginVertical(cardBoxStyle);
            DrawSliderFloat(ConfigManager.FlySpeed, Localization.T("player.fly_speed"), 10f, 100f, "{0:F0}");
            DrawSliderFloat(ConfigManager.FlyAcceleration, Localization.T("player.fly_acceleration"), 10f, 300f, "{0:F0}");
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    // ==========================================
    // TAB 2: ITEMS
    // ==========================================
    private void DrawItemsTab()
    {
        if (Event.current != null && Event.current.type == EventType.Layout)
        {
            if (Utilities.pendingItemRefresh || (Globals.items.Count == 0 && !Utilities.hasAttemptedItemLoad))
            {
                Utilities.pendingItemRefresh = false;
                try
                {
                    Utilities.UpdateItemsSync();
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError("[PEAK AIO] Error updating items in DrawItemsTab: " + ex);
                }
            }
        }

        Globals.mainScroll = GUILayout.BeginScrollView(Globals.mainScroll);
        try
        {
            // Safety check for search buffers (extended to 4 slots)
            if (Globals.itemSearchBuffers == null || Globals.itemSearchBuffers.Length < 4)
            {
                Globals.itemSearchBuffers = new string[4] { "", "", "", "" };
            }
            for (int s = 0; s < 4; s++)
            {
                if (Globals.itemSearchBuffers[s] == null)
                    Globals.itemSearchBuffers[s] = "";
            }

            if (Globals.slotScrolls == null || Globals.slotScrolls.Length < 4)
            {
                Globals.slotScrolls = new Vector2[4] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
            }

            if (Globals.selectedItems == null || Globals.selectedItems.Length < 4)
            {
                Globals.selectedItems = new int[4] { -1, -1, -1, -1 };
            }

            GUILayout.BeginHorizontal();
            try
            {
                GUILayout.Label(Localization.T("tab.items"), sectionHeaderStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(string.Format(Localization.T("items.loaded_count"), Globals.items.Count), tipLabelStyle);
                if (GUILayout.Button(Localization.T("items.refresh"), GUILayout.Width(130), GUILayout.Height(22)))
                {
                    Utilities.pendingItemRefresh = true;
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(4);

            if (Globals.items.Count == 0)
            {
                GUILayout.BeginVertical(cardBoxStyle);
                try
                {
                    GUILayout.Label(Localization.T("items.empty_notice"), labelStyle);
                    GUILayout.Space(2);
                    GUILayout.Label(Localization.T("items.empty_tip"), tipLabelStyle);
                    GUILayout.Space(6);
                    if (GUILayout.Button(Localization.T("items.refresh"), primaryBtnStyle, GUILayout.Width(160), GUILayout.Height(26)))
                    {
                        Utilities.pendingItemRefresh = true;
                    }
                }
                finally
                {
                    GUILayout.EndVertical();
                }
            }
            else
            {
                // 2x2 Grid for slots: Row 1 (Slot 0, 1), Row 2 (Slot 2, 3 [Backpack])
                GUILayout.BeginHorizontal();
                try
                {
                    DrawItemSlotColumn(0);
                    GUILayout.Space(6);
                    DrawItemSlotColumn(1);
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(6);

                GUILayout.BeginHorizontal();
                try
                {
                    DrawItemSlotColumn(2);
                    GUILayout.Space(6);
                    DrawItemSlotColumn(3);
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(6);
                DrawItemAttributesSection();
            }
        }
        finally
        {
            GUILayout.EndScrollView();
        }
    }

    private void DrawItemAttributesSection()
    {
        GUILayout.BeginVertical(cardBoxStyle);
        try
        {
            GUILayout.Label(Localization.T("items.attributes_title"), sectionHeaderStyle);
            GUILayout.Space(4);

            // 1. Mushroom Customization
            GUILayout.Label(Localization.T("items.mushroom_customization"), boldLabelStyle);
            GUILayout.BeginHorizontal();

            bool isVanilla = (Globals.mushroomSpawnMode == Globals.MushroomSpawnMode.Vanilla);
            bool isPurified = (Globals.mushroomSpawnMode == Globals.MushroomSpawnMode.Purified);
            bool isToxic = (Globals.mushroomSpawnMode == Globals.MushroomSpawnMode.Toxic);
            bool isSpecific = (Globals.mushroomSpawnMode == Globals.MushroomSpawnMode.Specific);

            if (GUILayout.Toggle(isVanilla, Localization.T("items.mushroom_mode_vanilla"), GUILayout.Width(130)) && !isVanilla)
            {
                Globals.mushroomSpawnMode = Globals.MushroomSpawnMode.Vanilla;
            }
            if (GUILayout.Toggle(isPurified, Localization.T("items.mushroom_mode_purified"), GUILayout.Width(170)) && !isPurified)
            {
                Globals.mushroomSpawnMode = Globals.MushroomSpawnMode.Purified;
            }
            if (GUILayout.Toggle(isToxic, Localization.T("items.mushroom_mode_toxic"), GUILayout.Width(170)) && !isToxic)
            {
                Globals.mushroomSpawnMode = Globals.MushroomSpawnMode.Toxic;
            }
            if (GUILayout.Toggle(isSpecific, Localization.T("items.mushroom_mode_specific"), GUILayout.Width(110)) && !isSpecific)
            {
                Globals.mushroomSpawnMode = Globals.MushroomSpawnMode.Specific;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (Globals.mushroomSpawnMode == Globals.MushroomSpawnMode.Specific)
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                for (int e = 0; e <= 4; e++)
                {
                    bool isCur = (Globals.selectedMushroomEffect == e);
                    GUIStyle bStyle = isCur ? primaryBtnStyle : regularBtnStyle;
                    if (GUILayout.Button(Localization.T("items.mushroom_effect_" + e), bStyle, GUILayout.Height(22)))
                    {
                        Globals.selectedMushroomEffect = e;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                for (int e = 5; e <= 9; e++)
                {
                    bool isCur = (Globals.selectedMushroomEffect == e);
                    GUIStyle bStyle = isCur ? dangerBtnStyle : regularBtnStyle;
                    if (GUILayout.Button(Localization.T("items.mushroom_effect_" + e), bStyle, GUILayout.Height(22)))
                    {
                        Globals.selectedMushroomEffect = e;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8);

            // 2. Blowgun Dart Ammo Enchantment
            GUILayout.Label(Localization.T("items.blowgun_enchantment"), boldLabelStyle);
            GUILayout.BeginHorizontal();
            bool prevDart = Globals.dartAmmoEnabled;
            bool newDart = GUILayout.Toggle(prevDart, Localization.T("items.dart_enable"), GUILayout.Width(240));
            if (newDart != prevDart)
            {
                Globals.dartAmmoEnabled = newDart;
                ConfigManager.DartAmmoEnabled.Value = newDart;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (Globals.dartAmmoEnabled)
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localization.T("items.dart_ammo_buffs"), tipLabelStyle, GUILayout.Width(100));
                DrawDartAmmoButton(Globals.DartAmmoType.Invincibility, "items.dart_invincible", primaryBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.SpeedBoost, "items.dart_speed", primaryBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.InfiniteStamina, "items.dart_stamina", primaryBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.FullCleanse, "items.dart_cleanse", primaryBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.LowGravity, "items.dart_lowgrav", primaryBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Glow, "items.dart_glow", primaryBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Revive, "items.dart_revive", primaryBtnStyle);
                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localization.T("items.dart_ammo_debuffs"), tipLabelStyle, GUILayout.Width(100));
                DrawDartAmmoButton(Globals.DartAmmoType.Poison, "items.dart_poison", dangerBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Starvation, "items.dart_starvation", dangerBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Sleep, "items.dart_sleep", dangerBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.TripFall, "items.dart_fall", dangerBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Thorns, "items.dart_thorns", dangerBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Spores, "items.dart_spores", dangerBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Blind, "items.dart_blind", dangerBtnStyle);
                DrawDartAmmoButton(Globals.DartAmmoType.Numb, "items.dart_numb", dangerBtnStyle);
                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(104);
                DrawDartAmmoButton(Globals.DartAmmoType.Chaos, "items.dart_chaos", regularBtnStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8);

            // 3. Extended Item Attributes
            GUILayout.Label(Localization.T("items.extended_attributes"), boldLabelStyle);
            GUILayout.BeginHorizontal();

            bool curFoodPoison = Globals.foodPoisonImmunity;
            bool newFoodPoison = GUILayout.Toggle(curFoodPoison, Localization.T("items.food_poison_immunity"), GUILayout.Width(300));
            if (newFoodPoison != curFoodPoison)
            {
                Globals.foodPoisonImmunity = newFoodPoison;
                ConfigManager.FoodPoisonImmunity.Value = newFoodPoison;
            }

            bool curTool = Globals.infiniteToolCharge;
            bool newTool = GUILayout.Toggle(curTool, Localization.T("items.infinite_tool_charge"), GUILayout.Width(280));
            if (newTool != curTool)
            {
                Globals.infiniteToolCharge = newTool;
                ConfigManager.InfiniteToolCharge.Value = newTool;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    private void DrawDartAmmoButton(Globals.DartAmmoType ammoType, string locKey, GUIStyle activeStyle)
    {
        bool isSelected = (Globals.selectedDartAmmoType == ammoType);
        GUIStyle style = isSelected ? activeStyle : regularBtnStyle;
        string text = Localization.T(locKey);
        if (GUILayout.Button(text, style, GUILayout.Height(22)))
        {
            Globals.selectedDartAmmoType = ammoType;
            ConfigManager.SelectedDartAmmoType.Value = (int)ammoType;
        }
    }

    private void DrawItemSlotColumn(int slot)
    {
        bool isBackpackSlot = (slot == 3);
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.ExpandWidth(true));
        try
        {
            // Slot header
            if (isBackpackSlot)
            {
                GUILayout.Label(Localization.T("items.slot4"), boldLabelStyle);
            }
            else
            {
                GUILayout.Label(string.Format("{0} {1}", Localization.T("items.slot"), slot + 1), boldLabelStyle);
            }

            // Current item display
            string currentItemName = Localization.T("items.none");
            try
            {
                if (Player.localPlayer != null)
                {
                    if (isBackpackSlot)
                    {
                        var bpSlot = Player.localPlayer.backpackSlot;
                        if (bpSlot == null && Player.localPlayer.itemSlots != null && Player.localPlayer.itemSlots.Length > 3)
                            bpSlot = Player.localPlayer.itemSlots[3] as BackpackSlot;

                        if (bpSlot != null && !bpSlot.IsEmpty())
                        {
                            if (bpSlot.prefab != null)
                            {
                                string n = bpSlot.prefab.GetName();
                                if (string.IsNullOrEmpty(n)) n = bpSlot.prefab.name;
                                if (!string.IsNullOrEmpty(n)) currentItemName = n;
                            }
                            else if (bpSlot.backpackType != BackpackSlot.BackpackType.None)
                            {
                                currentItemName = bpSlot.backpackType.ToString();
                            }
                        }
                    }
                    else if (Player.localPlayer.itemSlots != null && Player.localPlayer.itemSlots.Length > slot && Player.localPlayer.itemSlots[slot] != null)
                    {
                        var itemSlot = Player.localPlayer.itemSlots[slot];
                        if (!itemSlot.IsEmpty() && itemSlot.prefab != null)
                        {
                            string n = itemSlot.prefab.GetName();
                            if (string.IsNullOrEmpty(n)) n = itemSlot.prefab.name;
                            if (!string.IsNullOrEmpty(n)) currentItemName = n;
                        }
                        else if (!itemSlot.IsEmpty())
                        {
                            string n = itemSlot.GetPrefabName();
                            if (!string.IsNullOrEmpty(n)) currentItemName = n;
                        }
                    }
                }
            }
            catch { }
            GUILayout.Label(string.Format("{0}: {1}", Localization.T("items.current"), currentItemName), tipLabelStyle);

            if (isBackpackSlot)
            {
                GUILayout.Label(Localization.T("items.tip_backpack_slot"), tipLabelStyle);
            }
            else
            {
                GUILayout.Space(16);
            }

            GUILayout.Space(2);

            // Search filter
            string curSearch = Globals.itemSearchBuffers[slot] ?? "";
            string newSearch = GUILayout.TextField(curSearch, GUILayout.Height(20));
            Globals.itemSearchBuffers[slot] = newSearch ?? "";
            string search = Globals.itemSearchBuffers[slot];

            // Item list (Slot 4 only displays backpack items)
            Globals.slotScrolls[slot] = GUILayout.BeginScrollView(Globals.slotScrolls[slot], GUILayout.Height(140));
            try
            {
                bool hasItem = false;
                int itemCount = Math.Min(Globals.items.Count, Globals.itemNames.Count);
                for (int i = 0; i < itemCount; i++)
                {
                    string name = Globals.itemNames[i];
                    if (string.IsNullOrEmpty(name))
                        continue;

                    Item itemObj = Globals.items[i];

                    // Slot 4: only allow backpack items
                    if (isBackpackSlot && !Utilities.IsBackpackItem(itemObj, name))
                        continue;

                    if (!string.IsNullOrEmpty(search) && name.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    hasItem = true;
                    bool isSelected = (Globals.selectedItems[slot] == i);
                    GUIStyle btnStyle = isSelected ? itemSelectedStyle : itemSelectableStyle;

                    if (GUILayout.Button(name, btnStyle))
                    {
                        Globals.selectedItems[slot] = i;
                    }
                }

                if (!hasItem)
                {
                    GUILayout.Label(string.IsNullOrEmpty(search) ? Localization.T("items.none_available") : Localization.T("items.no_matches"), tipLabelStyle);
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }

            GUILayout.Space(4);

            if (isBackpackSlot)
            {
                // Slot 4 specific controls: Drop backpack button
                if (GUILayout.Button(Localization.T("items.drop_backpack"), dangerBtnStyle, GUILayout.Height(22)))
                {
                    Utilities.DropCurrentBackpack(Player.localPlayer);
                }

                bool canAct = (Globals.selectedItems[slot] >= 0 && Globals.selectedItems[slot] < Globals.items.Count);
                bool prevEnabled = GUI.enabled;
                GUI.enabled = canAct;

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Localization.T("items.equip_item"), primaryBtnStyle, GUILayout.Height(22)))
                {
                    if (canAct)
                    {
                        Utilities.AssignBackpackItem(Globals.selectedItems[slot]);
                    }
                }

                if (GUILayout.Button(Localization.T("items.spawn_item"), primaryBtnStyle, GUILayout.Height(22)))
                {
                    if (canAct)
                    {
                        Utilities.SpawnItemInWorld(Globals.selectedItems[slot]);
                    }
                }
                GUILayout.EndHorizontal();
                GUI.enabled = prevEnabled;
            }
            else
            {
                // Slots 0, 1, 2: Recharge section
                ConfigEntry<float> rechargeConfig = (slot == 0) ? ConfigManager.RechargeAmountSlot1 :
                    (slot == 1 ? ConfigManager.RechargeAmountSlot2 : ConfigManager.RechargeAmountSlot3);

                GUILayout.BeginHorizontal();
                if (rechargeConfig != null)
                {
                    GUILayout.Label(Localization.T("items.recharge"), GUILayout.Width(45));
                    float current = rechargeConfig.Value;
                    float next = GUILayout.HorizontalSlider(current, 0f, 100f);
                    if (Math.Abs(next - current) > 0.001f)
                    {
                        rechargeConfig.Value = next;
                    }
                    GUILayout.Space(4);
                    GUILayout.Label(string.Format("{0:F0}%", next), GUILayout.Width(35));
                    if (GUILayout.Button(Localization.T("items.recharge"), GUILayout.Width(60), GUILayout.Height(22)))
                    {
                        Utilities.RechargeInventorySlot(slot, rechargeConfig.Value);
                    }
                }
                GUILayout.EndHorizontal();

                bool canAct = (Globals.selectedItems[slot] >= 0 && Globals.selectedItems[slot] < Globals.items.Count);
                bool prevEnabled = GUI.enabled;
                GUI.enabled = canAct;

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Localization.T("items.equip_item"), primaryBtnStyle, GUILayout.Height(22)))
                {
                    if (canAct)
                    {
                        Utilities.AssignInventoryItem(slot, Globals.selectedItems[slot]);
                    }
                }

                if (GUILayout.Button(Localization.T("items.spawn_item"), primaryBtnStyle, GUILayout.Height(22)))
                {
                    if (canAct)
                    {
                        Utilities.SpawnItemInWorld(Globals.selectedItems[slot]);
                    }
                }
                GUILayout.EndHorizontal();
                GUI.enabled = prevEnabled;
            }
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    // ==========================================
    // TAB 3: LOBBY
    // ==========================================
    private void DrawLobbyTab()
    {
        if (Globals.allPlayers.Count == 0)
        {
            Utilities.RefreshPlayerList();
        }

        GUILayout.BeginHorizontal();

        // Left Column: Player List & Batch Actions
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.Width(290));

        GUILayout.BeginHorizontal();
        GUILayout.Label(Localization.T("lobby.players"), sectionHeaderStyle);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(Localization.T("lobby.refresh_players"), GUILayout.Height(20)))
        {
            Utilities.RefreshPlayerList();
        }
        GUILayout.EndHorizontal();

        // Scrollable player list
        Globals.lobbyPlayerScroll = GUILayout.BeginScrollView(Globals.lobbyPlayerScroll, GUILayout.Height(160));
        if (Globals.playerNames.Count == 0)
        {
            GUILayout.Label(Localization.T("lobby.no_player_selected"), tipLabelStyle);
        }
        else
        {
            for (int i = 0; i < Globals.playerNames.Count; i++)
            {
                bool isSelected = (Globals.selectedPlayer == i);
                GUIStyle pStyle = isSelected ? itemSelectedStyle : itemSelectableStyle;
                if (GUILayout.Button(Globals.playerNames[i], pStyle))
                {
                    Globals.selectedPlayer = i;
                }
            }
        }
        GUILayout.EndScrollView();

        GUILayout.Space(6);
        GUILayout.Label(Localization.T("lobby.all_players"), subHeaderStyle);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Localization.T("lobby.revive_all"), primaryBtnStyle, GUILayout.Height(24)))
            Utilities.ReviveAllPlayers(false);

        if (GUILayout.Button(Localization.T("lobby.revive_all_restore"), primaryBtnStyle, GUILayout.Height(24)))
            Utilities.ReviveAllPlayers(true);
        GUILayout.EndHorizontal();

        GUILayout.Space(2);
        if (GUILayout.Button(Localization.T("lobby.kill_all"), dangerBtnStyle, GUILayout.Height(24)))
            Utilities.KillAllPlayers();

        if (GUILayout.Button(Localization.T("lobby.clear_all_afflictions_all"), primaryBtnStyle, GUILayout.Height(24)))
            Utilities.ClearAllAfflictionsForAllPlayers();

        Globals.excludeSelfFromAllActions = GUILayout.Toggle(Globals.excludeSelfFromAllActions, Localization.T("lobby.exclude_self"));

        GUILayout.Space(4);
        if (GUILayout.Button(Localization.T("lobby.warp_all_to_me"), GUILayout.Height(24)))
            Utilities.WarpAllPlayersToMe();

        GUILayout.Space(4);
        bool canGiveAll = (Globals.selectedLobbyItem >= 0 && Globals.selectedLobbyItem < Globals.items.Count);
        string selAllName = canGiveAll && (Globals.selectedLobbyItem < Globals.itemNames.Count) ? Globals.itemNames[Globals.selectedLobbyItem] : "";
        string giveAllText = canGiveAll
            ? string.Format("{0} ({1})", Localization.T("lobby.give_all_item"), selAllName)
            : string.Format("{0} ({1})", Localization.T("lobby.give_all_item"), Localization.T("lobby.select_item_first"));

        bool prevGA = GUI.enabled;
        GUI.enabled = canGiveAll;
        if (GUILayout.Button(giveAllText, primaryBtnStyle, GUILayout.Height(24)))
        {
            if (canGiveAll)
            {
                Utilities.GiveItemToAllPlayers(Globals.selectedLobbyItem);
            }
        }
        GUI.enabled = prevGA;

        GUILayout.Space(6);
        GUILayout.Label(Localization.T("lobby.network_title"), subHeaderStyle);

        bool curTuning = Globals.enableNetworkTuning;
        bool newTuning = GUILayout.Toggle(curTuning, Localization.T("lobby.enable_network_tuning"));
        if (newTuning != curTuning)
        {
            Globals.enableNetworkTuning = newTuning;
            ConfigManager.EnableNetworkTuning.Value = newTuning;
            if (newTuning) NetworkTuningManager.ApplyOptimizations();
        }

        bool curAntiKick = Globals.enableAntiKick;
        bool newAntiKick = GUILayout.Toggle(curAntiKick, Localization.T("lobby.enable_antikick"));
        if (newAntiKick != curAntiKick)
        {
            Globals.enableAntiKick = newAntiKick;
            ConfigManager.EnableAntiKick.Value = newAntiKick;
        }

        bool curAutoRec = Globals.enableAutoReconnect;
        bool newAutoRec = GUILayout.Toggle(curAutoRec, Localization.T("lobby.enable_autoreconnect"));
        if (newAutoRec != curAutoRec)
        {
            Globals.enableAutoReconnect = newAutoRec;
            ConfigManager.EnableAutoReconnect.Value = newAutoRec;
        }

        GUILayout.EndVertical();

        GUILayout.Space(8);

        // Right Column: Selected Player Actions & Give Items
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.Width(340));

        GUILayout.Label(Localization.T("lobby.actions"), sectionHeaderStyle);

        if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Globals.allPlayers.Count)
        {
            string selName = Globals.playerNames[Globals.selectedPlayer];
            GUILayout.Label(string.Format("Target: {0}", selName), boldLabelStyle);

            GUILayout.Space(4);

            // Dual Revive buttons + Kill
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.revive"), primaryBtnStyle, GUILayout.Height(26)))
                Utilities.ReviveSelectedPlayer(false);

            if (GUILayout.Button(Localization.T("lobby.revive_restore"), primaryBtnStyle, GUILayout.Height(26)))
                Utilities.ReviveSelectedPlayer(true);

            if (GUILayout.Button(Localization.T("lobby.kill"), dangerBtnStyle, GUILayout.Height(26)))
                Utilities.KillSelectedPlayer();
            GUILayout.EndHorizontal();
            GUILayout.Label(Localization.T("tip.revive_restore"), tipLabelStyle);

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.warp_to"), GUILayout.Height(24)))
                Utilities.WarpToSelectedPlayer();

            if (GUILayout.Button(Localization.T("lobby.warp_to_me"), GUILayout.Height(24)))
                Utilities.WarpSelectedPlayerToMe();
            GUILayout.EndHorizontal();

            // --- Target Status Effects & Debuffs Section ---
            GUILayout.Space(6);
            GUILayout.Label(Localization.T("lobby.target_status_mgmt"), subHeaderStyle);

            if (GUILayout.Button(Localization.T("status.clear_all"), primaryBtnStyle, GUILayout.Height(24)))
            {
                if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Globals.allPlayers.Count)
                {
                    var targetChar = Globals.allPlayers[Globals.selectedPlayer];
                    if (targetChar != null)
                    {
                        Utilities.ClearAllAfflictionsForCharacter(targetChar);
                    }
                }
            }

            int lobbyStatusIdx = Mathf.Clamp(Globals.lobbySelectedStatusIndex, 0, Globals.AllStatusTypes.Length - 1);
            CharacterAfflictions.STATUSTYPE lobbyType = Globals.AllStatusTypes[lobbyStatusIdx];
            string lobbyTypeName = Localization.GetStatusTypeName(lobbyType);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", GUILayout.Width(28), GUILayout.Height(20)))
            {
                Globals.lobbySelectedStatusIndex = (lobbyStatusIdx - 1 + Globals.AllStatusTypes.Length) % Globals.AllStatusTypes.Length;
            }
            GUILayout.Label(lobbyTypeName, boldLabelStyle, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(">", GUILayout.Width(28), GUILayout.Height(20)))
            {
                Globals.lobbySelectedStatusIndex = (lobbyStatusIdx + 1) % Globals.AllStatusTypes.Length;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0}: {1:P0}", Localization.T("status.amount"), Globals.lobbyStatusAmount), GUILayout.Width(110));
            Globals.lobbyStatusAmount = GUILayout.HorizontalSlider(Globals.lobbyStatusAmount, 0.1f, 1.0f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("status.add_effect"), dangerBtnStyle, GUILayout.Height(22)))
            {
                if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Globals.allPlayers.Count)
                {
                    var targetChar = Globals.allPlayers[Globals.selectedPlayer];
                    if (targetChar != null)
                    {
                        Utilities.AddStatusEffect(targetChar, lobbyType, Globals.lobbyStatusAmount);
                    }
                }
            }
            if (GUILayout.Button(Localization.T("status.preset_torture"), dangerBtnStyle, GUILayout.Height(22)))
            {
                if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Globals.allPlayers.Count)
                {
                    var targetChar = Globals.allPlayers[Globals.selectedPlayer];
                    if (targetChar != null)
                    {
                        Utilities.ApplyTorturePreset(targetChar);
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // --- Give Items Section ---
            GUILayout.Label(Localization.T("lobby.give_items"), subHeaderStyle);

            // Item Search Bar
            string curSearch = Globals.lobbyItemSearchBuffer ?? "";
            string newSearch = GUILayout.TextField(curSearch, GUILayout.Height(20));
            Globals.lobbyItemSearchBuffer = newSearch ?? "";
            string search = Globals.lobbyItemSearchBuffer;

            // Scrollable item list
            Globals.lobbyItemScroll = GUILayout.BeginScrollView(Globals.lobbyItemScroll, GUILayout.Height(100));
            bool hasItem = false;
            int count = Math.Min(Globals.items.Count, Globals.itemNames.Count);
            for (int i = 0; i < count; i++)
            {
                string iName = Globals.itemNames[i];
                if (string.IsNullOrEmpty(iName)) continue;
                if (!string.IsNullOrEmpty(search) && iName.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                hasItem = true;
                bool isSelected = (Globals.selectedLobbyItem == i);
                GUIStyle iStyle = isSelected ? itemSelectedStyle : itemSelectableStyle;
                if (GUILayout.Button(iName, iStyle))
                {
                    Globals.selectedLobbyItem = i;
                }
            }
            if (!hasItem)
            {
                GUILayout.Label(string.IsNullOrEmpty(search) ? Localization.T("items.none_available") : Localization.T("items.no_matches"), tipLabelStyle);
            }
            GUILayout.EndScrollView();

            GUILayout.Space(4);

            // Give Selected Item: 1. Spawn in Front, 2. Equip to Slot 4 (Backpack)
            bool canGive = (Globals.selectedLobbyItem >= 0 && Globals.selectedLobbyItem < Globals.items.Count);
            bool prevE = GUI.enabled;
            GUI.enabled = canGive;
            string selDisp = canGive ? Globals.itemNames[Globals.selectedLobbyItem] : Localization.T("items.none");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(string.Format("{0}: {1}", Localization.T("lobby.spawn_in_front"), selDisp), primaryBtnStyle, GUILayout.Height(24)))
            {
                if (canGive)
                {
                    Utilities.GiveItemToPlayer(Globals.selectedPlayer, Globals.selectedLobbyItem);
                }
            }

            if (GUILayout.Button(Localization.T("lobby.give_to_slot4"), GUILayout.Height(24), GUILayout.Width(130)))
            {
                if (canGive)
                {
                    Utilities.EquipBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.None, Globals.items[Globals.selectedLobbyItem]);
                }
            }
            GUILayout.EndHorizontal();
            GUI.enabled = prevE;

            GUILayout.Space(6);

            // Quick Backpack Buttons: Row 1 = Direct Equip to Slot 4; Row 2 = Spawn in Front
            GUILayout.Label(Localization.T("items.slot4") + " (快捷给予 / 穿戴):", subHeaderStyle);

            // Direct Equip
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.equip_backpack"), primaryBtnStyle, GUILayout.Height(22)))
                Utilities.EquipBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Backpack);

            if (GUILayout.Button(Localization.T("lobby.equip_jetpack"), primaryBtnStyle, GUILayout.Height(22)))
                Utilities.EquipBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Jetpack);
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.equip_rocketpack"), primaryBtnStyle, GUILayout.Height(22)))
                Utilities.EquipBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Rocketpack);

            if (GUILayout.Button(Localization.T("lobby.equip_fannypack"), primaryBtnStyle, GUILayout.Height(22)))
                Utilities.EquipBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Fannypack);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            // Spawn on ground
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.give_backpack"), GUILayout.Height(22)))
                Utilities.GiveQuickBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Backpack);

            if (GUILayout.Button(Localization.T("lobby.give_jetpack"), GUILayout.Height(22)))
                Utilities.GiveQuickBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Jetpack);
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.give_rocketpack"), GUILayout.Height(22)))
                Utilities.GiveQuickBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Rocketpack);

            if (GUILayout.Button(Localization.T("lobby.give_fannypack"), GUILayout.Height(22)))
                Utilities.GiveQuickBackpackToPlayer(Globals.selectedPlayer, BackpackSlot.BackpackType.Fannypack);
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            GUILayout.Label(Localization.T("lobby.special_actions"), subHeaderStyle);

            if (GUILayout.Button(Localization.T("lobby.spawn_scoutmaster"), dangerBtnStyle, GUILayout.Height(24)))
            {
                Utilities.SpawnScoutmasterForPlayer(Globals.selectedPlayer);
            }
            GUILayout.Label(Localization.T("tip.spawn_scoutmaster"), tipLabelStyle);
        }
        else
        {
            GUILayout.Label(Localization.T("lobby.no_player_selected"), tipLabelStyle);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    // ==========================================
    // TAB 4: WORLD (Custom Route Map & Flight Ribbon)
    // ==========================================
    private void DrawRouteFlowRibbon(List<Utilities.RouteNodeBadge> badges, bool showDetails = true)
    {
        if (badges == null || badges.Count == 0)
            return;

        GUILayout.BeginVertical(ribbonCardStyle);
        GUILayout.BeginHorizontal();

        for (int i = 0; i < badges.Count; i++)
        {
            var b = badges[i];
            GUIStyle bStyle = b.isCurrent ? badgeCurrentStyle : (b.isCompleted ? badgeCompletedStyle : badgePendingStyle);

            string statusMarker = "";
            if (b.isCurrent)
            {
                statusMarker = " ★";
            }
            else if (b.isCompleted || b.isCampfireLit)
            {
                statusMarker = " ✓";
            }

            string badgeText = string.Format("{0}. {1}{2}", b.stepIndex, b.name, statusMarker);
            GUILayout.Label(badgeText, bStyle, GUILayout.Height(24));

            if (i < badges.Count - 1)
            {
                GUILayout.Label("➔", flowArrowStyle, GUILayout.Width(16), GUILayout.Height(24));
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (showDetails)
        {
            Utilities.RouteNodeBadge activeBadge = null;
            for (int i = 0; i < badges.Count; i++)
            {
                if (badges[i].isCurrent)
                {
                    activeBadge = badges[i];
                    break;
                }
            }

            if (activeBadge != null)
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                string curDetail = string.Format("{0}: {1}. {2}",
                    Localization.T("world.current_node_tag"),
                    activeBadge.stepIndex,
                    activeBadge.name);

                if (activeBadge.hasCampfire)
                {
                    curDetail += activeBadge.isCampfireLit ? " [✓]" : "";
                }
                if (activeBadge.altitude > 0f)
                {
                    curDetail += string.Format(" ({0:F1}m)", activeBadge.altitude);
                }
                GUILayout.Label(curDetail, tipLabelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();
    }

    private void DrawPlaylistQueueSection()
    {
        var queue = Utilities.WorldDataCache.playlistQueue;
        if (queue == null || queue.Count <= 1)
            return;

        GUILayout.Space(6);
        string overviewHeader = string.Format("{0} ({1}/{2})",
            Localization.T("world.playlist_overview"),
            Utilities.WorldDataCache.currentPlaylistIndex + 1,
            Utilities.WorldDataCache.totalPlaylistCount);
        GUILayout.Label(overviewHeader, subHeaderStyle);
        GUILayout.Space(2);

        for (int p = 0; p < queue.Count; p++)
        {
            var item = queue[p];
            bool isCur = item.isCurrent;

            GUILayout.BeginVertical(ribbonCardStyle);

            GUILayout.BeginHorizontal();
            string stageTag = string.Format(Localization.T("world.stage_tag"), p + 1, queue.Count);
            GUIStyle tagStyle = isCur ? stageTagActiveStyle : stageTagStyle;
            GUILayout.Label(stageTag, tagStyle, GUILayout.Height(18), GUILayout.Width(75));

            GUILayout.Space(4);
            string mapTitle = item.displayName;
            GUILayout.Label(mapTitle, isCur ? boldLabelStyle : labelStyle);

            GUILayout.FlexibleSpace();

            string statusText;
            GUIStyle statusStyle;
            if (isCur)
            {
                statusText = string.Format("★ {0}", Localization.T("world.status_active"));
                statusStyle = badgeCurrentStyle;
            }
            else if (p < Utilities.WorldDataCache.currentPlaylistIndex)
            {
                statusText = string.Format("✓ {0}", Localization.T("world.status_completed"));
                statusStyle = badgeCompletedStyle;
            }
            else
            {
                statusText = Localization.T("world.status_upcoming");
                statusStyle = badgePendingStyle;
            }
            GUILayout.Label(statusText, statusStyle, GUILayout.Height(18), GUILayout.Width(65));
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(item.formattedRoute))
            {
                GUILayout.Space(2);
                GUILayout.Label(string.Format("{0}", item.formattedRoute), tipLabelStyle);
            }

            if (item.nodes != null && item.nodes.Count > 0)
            {
                GUILayout.Space(2);
                DrawRouteFlowRibbon(item.nodes, false);
            }

            GUILayout.EndVertical();
            if (p < queue.Count - 1)
            {
                GUILayout.Space(3);
            }
        }
    }

    private void DrawWorldTab()
    {
        Utilities.EnsureLuggageListInitialized();
        Utilities.WorldDataCache.EnsureUpdated();

        Globals.mainScroll = GUILayout.BeginScrollView(Globals.mainScroll);

        // --- Map / Segment Jump & Route Section ---
        GUILayout.BeginVertical(cardBoxStyle);

        // Header
        GUILayout.Label(Localization.T("world.segment_teleport"), sectionHeaderStyle);
        GUILayout.Space(4);

        // Read from Cached World Data (Zero Lag)
        Segment detectedSeg = Utilities.WorldDataCache.currentSegment;
        int currentLevel = Utilities.WorldDataCache.currentLevelNumber;
        float altitude = Utilities.WorldDataCache.altitude;
        List<Utilities.RouteSegmentInfo> route = Utilities.WorldDataCache.route;
        bool isAtCampfire = Utilities.WorldDataCache.isAtCampfire;
        string currentSegDisplayName = Utilities.WorldDataCache.currentSegDisplayName;
        string nextSegDisplayName = Utilities.WorldDataCache.nextSegDisplayName;
        bool isInAirport = Utilities.WorldDataCache.isInAirport;

        if (isInAirport)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.T("world.airport_status"), boldLabelStyle);
            GUILayout.FlexibleSpace();
            if (!string.IsNullOrEmpty(Utilities.WorldDataCache.countdownFormatted))
            {
                GUILayout.Label(string.Format("{0} {1}", Localization.T("world.rotation_timer"), Utilities.WorldDataCache.countdownFormatted), tipLabelStyle);
            }
            GUILayout.EndHorizontal();

            // Check if recently announced island loading
            if (Time.realtimeSinceStartup - Utilities.WorldDataCache.pendingAnnouncedTime < 25f &&
                !string.IsNullOrEmpty(Utilities.WorldDataCache.pendingAnnouncedScene))
            {
                string announceText = string.Format("{0}: {1} (Ascent: {2})",
                    Localization.T("world.loading_announced"),
                    Utilities.WorldDataCache.pendingAnnouncedScene,
                    Utilities.WorldDataCache.pendingAnnouncedAscent);
                GUILayout.Label(announceText, boldLabelStyle);
            }

            if (Utilities.WorldDataCache.isCustomScene)
            {
                string customTitle = string.Format("[{0}] {1}",
                    Localization.T("world.custom_map_tag"),
                    Utilities.WorldDataCache.todaySceneName);
                if (!string.IsNullOrEmpty(Utilities.WorldDataCache.playlistInfo))
                {
                    customTitle += " (" + Utilities.WorldDataCache.playlistInfo + ")";
                }
                GUILayout.Label(customTitle, boldLabelStyle);

                if (!string.IsNullOrEmpty(Utilities.WorldDataCache.todayBiomeRoute))
                {
                    GUILayout.Label(string.Format("{0}: {1}", Localization.T("world.custom_route_info"), Utilities.WorldDataCache.todayBiomeRoute), subHeaderStyle);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Utilities.WorldDataCache.todayBiomeRoute))
                {
                    GUILayout.Label(string.Format("{0}: {1}", Localization.T("world.daily_route_info"), Utilities.WorldDataCache.todayBiomeRoute), subHeaderStyle);
                }
                if (!string.IsNullOrEmpty(Utilities.WorldDataCache.nextBiomeRoute))
                {
                    GUILayout.Label(string.Format("{0}: {1}", Localization.T("world.next_rotation_info"), Utilities.WorldDataCache.nextBiomeRoute), tipLabelStyle);
                }
            }

            // Route Ribbon in Airport
            GUILayout.Space(4);
            GUILayout.Label(string.Format("{0}:", Localization.T("world.route_flow_title")), subHeaderStyle);
            DrawRouteFlowRibbon(Utilities.WorldDataCache.currentMapBadges, false);
            GUILayout.Label(Localization.T("world.airport_preview_tip"), tipLabelStyle);

            // Playlist Queue (if multiple maps)
            DrawPlaylistQueueSection();

            GUILayout.Space(6);
            GUILayout.Label(Localization.T("world.airport_teleport_hidden_hint"), tipLabelStyle);
        }
        else
        {
            GUILayout.BeginHorizontal();
            string statusText = string.Format("{0} {1} - {2}",
                Localization.T("world.current_segment"),
                string.Format(Localization.T("world.level_label"), currentLevel),
                currentSegDisplayName);
            if (isAtCampfire)
            {
                statusText += "  " + Localization.T("world.at_campfire_tag");
            }
            if (Utilities.WorldDataCache.isCustomScene)
            {
                statusText += "  [" + Localization.T("world.custom_map_tag") + "]";
            }
            GUILayout.Label(statusText, boldLabelStyle);

            GUILayout.FlexibleSpace();
            if (altitude != 0f)
            {
                GUILayout.Label(string.Format("{0} {1:F1} m", Localization.T("world.current_altitude"), altitude), tipLabelStyle);
            }
            GUILayout.EndHorizontal();

            // Next Level & Daily Route info
            GUILayout.BeginHorizontal();
            if (currentLevel < 6)
            {
                GUILayout.Label(string.Format("{0} {1} - {2}",
                    Localization.T("world.next_level_target"),
                    string.Format(Localization.T("world.level_label"), Utilities.WorldDataCache.nextLevelNumber),
                    nextSegDisplayName), tipLabelStyle);
            }
            GUILayout.FlexibleSpace();
            if (Utilities.WorldDataCache.isCustomScene && !string.IsNullOrEmpty(Utilities.WorldDataCache.todaySceneName))
            {
                GUILayout.Label(string.Format(Localization.T("world.custom_scene_active"), Utilities.WorldDataCache.todaySceneName), tipLabelStyle);
            }
            else if (!string.IsNullOrEmpty(Utilities.WorldDataCache.countdownFormatted))
            {
                GUILayout.Label(string.Format("{0} {1}", Localization.T("world.rotation_timer"), Utilities.WorldDataCache.countdownFormatted), tipLabelStyle);
            }
            GUILayout.EndHorizontal();

            if (Utilities.WorldDataCache.isCustomScene && !string.IsNullOrEmpty(Utilities.WorldDataCache.todayBiomeRoute))
            {
                GUILayout.Label(string.Format("{0}: {1}", Localization.T("world.custom_route_info"), Utilities.WorldDataCache.todayBiomeRoute), tipLabelStyle);
            }

            // Route Ribbon in Island
            GUILayout.Space(4);
            GUILayout.Label(string.Format("{0}:", Localization.T("world.route_flow_title")), subHeaderStyle);
            DrawRouteFlowRibbon(Utilities.WorldDataCache.currentMapBadges, true);

            // Playlist Queue (if multiple maps)
            DrawPlaylistQueueSection();

            // 仅在进入海岛游戏且路线确认完毕后展示传送与区域跳转功能
            if (Utilities.WorldDataCache.hasDeterminedRoute)
            {
                GUILayout.Space(6);

                // Prominent Button(s): Teleport to Campfire / Light Campfire
                if (currentLevel < 6)
                {
                    int nextLevel = Utilities.WorldDataCache.nextLevelNumber;
                    string nextName = nextSegDisplayName;

                    if (!isAtCampfire)
                    {
                        string btnText;
                        if (nextLevel < 5)
                        {
                            btnText = string.Format("{0} ({1} {2})",
                                Localization.T("world.teleport_next_campfire"),
                                string.Format(Localization.T("world.level_label"), nextLevel),
                                nextName);
                        }
                        else if (nextLevel == 5)
                        {
                            btnText = Localization.T("world.teleport_kiln_safe");
                        }
                        else
                        {
                            btnText = Localization.T("world.teleport_to_peak");
                        }

                        if (GUILayout.Button(btnText, primaryBtnStyle, GUILayout.Height(32)))
                        {
                            Utilities.TeleportToNextCampfire();
                        }
                    }
                    else
                    {
                        // Player is already at the transition campfire!
                        GUILayout.BeginHorizontal();
                        string lightText = string.Format(Localization.T("world.light_campfire"), currentLevel);
                        if (GUILayout.Button(lightText, primaryBtnStyle, GUILayout.Height(32)))
                        {
                            Utilities.LightCurrentCampfire();
                        }

                        GUILayout.Space(6);

                        int afterNextLevel = nextLevel;
                        string nextCampText;
                        if (afterNextLevel < 5)
                        {
                            nextCampText = string.Format(Localization.T("world.teleport_next_area_campfire"), afterNextLevel);
                        }
                        else if (afterNextLevel == 5)
                        {
                            nextCampText = Localization.T("world.teleport_kiln_safe");
                        }
                        else
                        {
                            nextCampText = Localization.T("world.teleport_to_peak");
                        }

                        if (GUILayout.Button(nextCampText, sidebarActiveBtnStyle, GUILayout.Height(32)))
                        {
                            Utilities.TeleportToNextCampfire();
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    // At Peak
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    GUILayout.Button(Localization.T("world.at_peak"), primaryBtnStyle, GUILayout.Height(32));
                    GUI.enabled = true;
                    GUILayout.Space(6);
                    if (GUILayout.Button(Localization.T("world.summon_helicopter"), dangerBtnStyle, GUILayout.Height(32), GUILayout.Width(160)))
                    {
                        Utilities.SummonHelicopter();
                    }
                    GUILayout.EndHorizontal();
                }

                // Detailed Area & Campfire Jump Controls with Refresh Button
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localization.T("world.jump_to_segment") + ":", subHeaderStyle);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Localization.T("world.refresh_route"), GUILayout.Width(110), GUILayout.Height(22)))
                {
                    Utilities.WorldDataCache.Invalidate();
                    Utilities.WorldDataCache.EnsureUpdated(force: true);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);

                for (int i = 0; i < route.Count; i++)
                {
                    var r = route[i];
                    bool isCur = (r.segment == detectedSeg);

                    GUILayout.BeginHorizontal();

                    // Label for Level and Biome name
                    string segLabel = string.Format("{0}: {1}", string.Format(Localization.T("world.level_label"), r.level), r.displayName);
                    if (r.isCampfireLit)
                    {
                        segLabel += "  [✓]";
                    }
                    if (isCur)
                    {
                        segLabel += r.isAtCampfire
                            ? string.Format("  [{0}]", Localization.T("world.at_campfire_tag"))
                            : string.Format("  [{0}]", Localization.T("world.current_tag"));
                    }
                    GUILayout.Label(segLabel, isCur ? boldLabelStyle : labelStyle, GUILayout.Width(200));

                    // Button 1: Jump to Start of segment (Safe start jump, does not light campfire)
                    GUIStyle jumpBtnStyle = isCur ? sidebarActiveBtnStyle : primaryBtnStyle;
                    string jumpStartText = string.Format("{0}", Localization.T("world.jump_to_start"));
                    if (GUILayout.Button(jumpStartText, jumpBtnStyle, GUILayout.Height(24), GUILayout.Width(95)))
                    {
                        Utilities.JumpToSegmentStartSafe(r.segment);
                    }

                    GUILayout.Space(4);

                    // Button 2: Teleport to Campfire (or Kiln safe point / Peak)
                    if (r.hasCampfire)
                    {
                        if (GUILayout.Button(Localization.T("world.teleport_campfire"), primaryBtnStyle, GUILayout.Height(24), GUILayout.Width(125)))
                        {
                            Utilities.JumpToSegmentCampfire(r.segment);
                        }

                        GUILayout.Space(3);

                        // Button 3: Light Campfire
                        if (GUILayout.Button(Localization.T("world.light_action"), sidebarBtnStyle, GUILayout.Height(24), GUILayout.Width(50)))
                        {
                            Utilities.LightCampfire(i);
                        }
                    }
                    else if (r.segment == Segment.TheKiln)
                    {
                        bool isCitadel = Utilities.IsCitadelActive() || (!string.IsNullOrEmpty(r.displayName) && (r.displayName.IndexOf("Citadel", StringComparison.OrdinalIgnoreCase) >= 0 || r.displayName.IndexOf("城塞", StringComparison.OrdinalIgnoreCase) >= 0));
                        string btnKey = isCitadel ? "world.teleport_citadel_safe" : "world.teleport_kiln_safe";
                        if (GUILayout.Button(Localization.T(btnKey), primaryBtnStyle, GUILayout.Height(24), GUILayout.Width(178)))
                        {
                            Utilities.JumpToSegmentStartSafe(Segment.TheKiln);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(Localization.T("world.teleport_to_peak"), primaryBtnStyle, GUILayout.Height(24), GUILayout.Width(178)))
                        {
                            Utilities.JumpToSegmentStartSafe(Segment.Peak);
                        }
                    }

                    GUILayout.EndHorizontal();

                    if (i < route.Count - 1)
                        GUILayout.Space(3);
                }

                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Localization.T("world.return_airport"), sidebarBtnStyle, GUILayout.Height(24), GUILayout.Width(150)))
                {
                    Utilities.ReturnToAirport();
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();

        GUILayout.Space(8);

        // --- Containers / Luggage Section ---
        GUILayout.BeginHorizontal();

        // Left: Luggage List
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.Width(340));
        GUILayout.BeginHorizontal();
        GUILayout.Label(Localization.T("world.containers"), sectionHeaderStyle);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(Localization.T("world.refresh_luggage"), GUILayout.Height(20)))
        {
            Utilities.hasInitializedLuggageList = false;
            Utilities.RefreshLuggageList();
        }
        GUILayout.EndHorizontal();

        Globals.luggageScroll = GUILayout.BeginScrollView(Globals.luggageScroll, GUILayout.Height(130));
        if (Globals.luggageLabels.Count == 0)
        {
            GUILayout.Label(Localization.T("world.no_containers"), tipLabelStyle);
        }
        else
        {
            for (int i = 0; i < Globals.luggageLabels.Count; i++)
            {
                bool isSelected = (Globals.selectedLuggageIndex == i);
                GUIStyle lStyle = isSelected ? itemSelectedStyle : itemSelectableStyle;
                if (GUILayout.Button(Globals.luggageLabels[i], lStyle))
                {
                    Globals.selectedLuggageIndex = i;
                }
            }
        }
        GUILayout.EndScrollView();

        GUILayout.Space(4);
        if (GUILayout.Button(Localization.T("world.open_all_nearby"), dangerBtnStyle, GUILayout.Height(24)))
        {
            Utilities.OpenAllNearbyLuggage();
        }
        GUILayout.EndVertical();

        GUILayout.Space(8);

        // Right: Luggage Actions
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.Width(260));
        GUILayout.Label(Localization.T("lobby.actions"), sectionHeaderStyle);

        if (Globals.selectedLuggageIndex >= 0 && Globals.selectedLuggageIndex < Globals.luggageObject.Count)
        {
            var lug = Globals.luggageObject[Globals.selectedLuggageIndex];
            string name = (lug != null) ? lug.displayName : "None";
            GUILayout.Label(string.Format("Target: {0}", name), boldLabelStyle);

            GUILayout.Space(6);

            if (GUILayout.Button(Localization.T("world.warp_to_luggage"), primaryBtnStyle, GUILayout.Height(26)))
            {
                if (lug != null)
                {
                    Vector3 luggageCoords = lug.Center();
                    luggageCoords.y += 1.5f;
                    Utilities.TeleportToCoords(luggageCoords.x, luggageCoords.y, luggageCoords.z);
                }
            }

            GUILayout.Space(4);

            if (GUILayout.Button(Localization.T("world.open_luggage"), GUILayout.Height(26)))
            {
                Utilities.OpenLuggage(Globals.selectedLuggageIndex);
            }
        }
        else
        {
            GUILayout.Label(Localization.T("world.no_luggage_selected"), tipLabelStyle);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    // ==========================================
    // TAB 5: CREATURES (CREATURE SPAWNER)
    // ==========================================
    private void DrawCreaturesTab()
    {
        Globals.creaturesScroll = GUILayout.BeginScrollView(Globals.creaturesScroll);

        // Header and Description
        GUILayout.BeginHorizontal();
        GUILayout.Label(Localization.T("creatures.title"), sectionHeaderStyle);
        GUILayout.FlexibleSpace();

        // Host Authority Status Badge
        bool isHost = PhotonNetwork.IsMasterClient;
        GUIStyle badgeStyle = isHost ? badgeCompletedStyle : badgePendingStyle;
        string badgeText = isHost ? Localization.T("creatures.host_badge_ready") : Localization.T("creatures.host_badge_warning");
        GUILayout.Label(badgeText, badgeStyle, GUILayout.Height(22));
        GUILayout.EndHorizontal();

        GUILayout.Label(Localization.T("creatures.desc"), tipLabelStyle);
        GUILayout.Space(6);

        // --- 1. Creature Type Selection Grid ---
        GUILayout.Label(Localization.T("creatures.select_type"), subHeaderStyle);
        GUILayout.Space(2);

        Globals.CreatureType[] types = new Globals.CreatureType[] {
            Globals.CreatureType.Scoutmaster,
            Globals.CreatureType.BigGhost,
            Globals.CreatureType.MushroomZombie,
            Globals.CreatureType.Scorpion,
            Globals.CreatureType.Beetle,
            Globals.CreatureType.BeeSwarm
        };

        for (int row = 0; row < 3; row++)
        {
            GUILayout.BeginHorizontal();
            for (int col = 0; col < 2; col++)
            {
                int index = row * 2 + col;
                if (index < types.Length)
                {
                    var cType = types[index];
                    bool isSelected = (Globals.selectedCreatureType == cType);
                    string key = (cType == Globals.CreatureType.BeeSwarm) ? "creatures.type.bees" : ("creatures.type." + cType.ToString().ToLower());
                    string typeLabel = Localization.T(key);

                    GUIStyle btnStyle = isSelected ? sidebarActiveBtnStyle : primaryBtnStyle;
                    if (GUILayout.Button(typeLabel, btnStyle, GUILayout.Height(28)))
                    {
                        Globals.selectedCreatureType = cType;
                    }
                    if (col == 0) GUILayout.Space(6);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        // Selected Creature Description Card
        GUILayout.BeginVertical(cardBoxStyle);
        string curDescKey = (Globals.selectedCreatureType == Globals.CreatureType.BeeSwarm)
            ? "creatures.type.bees_desc"
            : ("creatures.type." + Globals.selectedCreatureType.ToString().ToLower() + "_desc");
        GUILayout.Label(Localization.T(curDescKey), labelStyle);
        GUILayout.EndVertical();

        GUILayout.Space(8);

        // --- 2. Spawn Anchor & Spatial Parameters ---
        GUILayout.Label(Localization.T("creatures.spawn_anchor"), subHeaderStyle);
        GUILayout.Space(2);

        GUILayout.BeginHorizontal();
        bool isAnchorSelf = (Globals.creatureSpawnAnchor == Globals.CreatureSpawnAnchor.Self);
        if (GUILayout.Button(Localization.T("creatures.anchor_self"), isAnchorSelf ? sidebarActiveBtnStyle : primaryBtnStyle, GUILayout.Height(24)))
        {
            Globals.creatureSpawnAnchor = Globals.CreatureSpawnAnchor.Self;
        }
        GUILayout.Space(4);

        bool isAnchorPlayer = (Globals.creatureSpawnAnchor == Globals.CreatureSpawnAnchor.SelectedPlayer);
        if (GUILayout.Button(Localization.T("creatures.anchor_player"), isAnchorPlayer ? sidebarActiveBtnStyle : primaryBtnStyle, GUILayout.Height(24)))
        {
            Globals.creatureSpawnAnchor = Globals.CreatureSpawnAnchor.SelectedPlayer;
        }
        GUILayout.Space(4);

        bool isAnchorCrosshair = (Globals.creatureSpawnAnchor == Globals.CreatureSpawnAnchor.Crosshair);
        if (GUILayout.Button(Localization.T("creatures.anchor_crosshair"), isAnchorCrosshair ? sidebarActiveBtnStyle : primaryBtnStyle, GUILayout.Height(24)))
        {
            Globals.creatureSpawnAnchor = Globals.CreatureSpawnAnchor.Crosshair;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        // Distance Slider
        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format(Localization.T("creatures.spawn_distance"), Globals.creatureSpawnDistance), boldLabelStyle, GUILayout.Width(170));
        Globals.creatureSpawnDistance = GUILayout.HorizontalSlider(Globals.creatureSpawnDistance, 1.0f, 50.0f);
        GUILayout.Space(8);
        if (GUILayout.Button("5m", sidebarBtnStyle, GUILayout.Width(36), GUILayout.Height(20))) Globals.creatureSpawnDistance = 5f;
        if (GUILayout.Button("10m", sidebarBtnStyle, GUILayout.Width(38), GUILayout.Height(20))) Globals.creatureSpawnDistance = 10f;
        if (GUILayout.Button("20m", sidebarBtnStyle, GUILayout.Width(38), GUILayout.Height(20))) Globals.creatureSpawnDistance = 20f;
        if (GUILayout.Button("35m", sidebarBtnStyle, GUILayout.Width(38), GUILayout.Height(20))) Globals.creatureSpawnDistance = 35f;
        GUILayout.EndHorizontal();

        // Ghost Scale Slider (Only shown if BigGhost selected)
        if (Globals.selectedCreatureType == Globals.CreatureType.BigGhost)
        {
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format(Localization.T("creatures.ghost_scale"), Globals.creatureGhostScale), boldLabelStyle, GUILayout.Width(170));
            Globals.creatureGhostScale = GUILayout.HorizontalSlider(Globals.creatureGhostScale, 1.0f, 5.0f);
            GUILayout.Space(8);
            if (GUILayout.Button("1x", sidebarBtnStyle, GUILayout.Width(32), GUILayout.Height(20))) Globals.creatureGhostScale = 1f;
            if (GUILayout.Button("2x", sidebarBtnStyle, GUILayout.Width(32), GUILayout.Height(20))) Globals.creatureGhostScale = 2f;
            if (GUILayout.Button("3x", sidebarBtnStyle, GUILayout.Width(32), GUILayout.Height(20))) Globals.creatureGhostScale = 3f;
            if (GUILayout.Button("5x", sidebarBtnStyle, GUILayout.Width(32), GUILayout.Height(20))) Globals.creatureGhostScale = 5f;
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(8);

        // --- 3. Aggro & Behavior Target Binding ---
        GUILayout.Label(Localization.T("creatures.aggro_target"), subHeaderStyle);
        GUILayout.Space(2);

        GUILayout.BeginHorizontal();
        bool isTargetSelf = (Globals.creatureAggroTargetIndex == -1);
        if (GUILayout.Button(Localization.T("creatures.target_self"), isTargetSelf ? sidebarActiveBtnStyle : primaryBtnStyle, GUILayout.Height(24)))
        {
            Globals.creatureAggroTargetIndex = -1;
        }
        GUILayout.Space(4);

        bool isTargetPlayer = (Globals.creatureAggroTargetIndex >= 0);
        string targetPlayerLabel = Localization.T("creatures.target_selected");
        if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Character.AllCharacters.Count)
        {
            var pChar = Character.AllCharacters[Globals.selectedPlayer];
            if (pChar != null && !string.IsNullOrEmpty(pChar.characterName))
                targetPlayerLabel = string.Format("{0} ({1})", Localization.T("creatures.target_selected"), pChar.characterName);
        }
        if (GUILayout.Button(targetPlayerLabel, isTargetPlayer ? sidebarActiveBtnStyle : primaryBtnStyle, GUILayout.Height(24)))
        {
            Globals.creatureAggroTargetIndex = (Globals.selectedPlayer >= 0) ? Globals.selectedPlayer : 0;
        }
        GUILayout.Space(4);

        bool isTargetNone = (Globals.creatureAggroTargetIndex == -2);
        if (GUILayout.Button(Localization.T("creatures.target_none"), isTargetNone ? sidebarActiveBtnStyle : primaryBtnStyle, GUILayout.Height(24)))
        {
            Globals.creatureAggroTargetIndex = -2;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(12);

        // --- 4. Main Spawn Button ---
        bool requiresHost = (Globals.selectedCreatureType == Globals.CreatureType.Scoutmaster || Globals.selectedCreatureType == Globals.CreatureType.MushroomZombie);
        bool canSpawn = isHost || !requiresHost;

        if (!canSpawn)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button(Localization.T("creatures.btn_spawn"), dangerBtnStyle, GUILayout.Height(32)))
        {
            Utilities.SpawnCreature(
                Globals.selectedCreatureType,
                Globals.creatureSpawnAnchor,
                Globals.creatureSpawnDistance,
                Globals.creatureAggroTargetIndex,
                Globals.creatureGhostScale
            );
        }

        if (!canSpawn)
        {
            GUI.enabled = true;
            GUILayout.Label(Localization.T("creatures.host_required"), tipLabelStyle);
        }

        GUILayout.EndScrollView();
    }

    // ==========================================
    // TAB 6: ACHIEVEMENTS
    // ==========================================
    private void DrawAchievementsTab()
    {
        // 节流刷新底层成就状态缓存（每秒最多查询 1 次 Steamworks，消除每帧 IPC 开销）
        Utilities.AchievementCache.EnsureUpdated();

        Globals.achievementsScroll = GUILayout.BeginScrollView(Globals.achievementsScroll);

        // 1. Header & Realtime Progress Badge
        GUILayout.BeginHorizontal();
        GUILayout.Label(Localization.T("achievements.title"), sectionHeaderStyle);
        GUILayout.FlexibleSpace();

        int unlockedCount = Utilities.AchievementCache.UnlockedCount;
        int totalValid = Utilities.AchievementCache.TotalCount;
        bool allDone = (totalValid > 0 && unlockedCount >= totalValid);
        GUIStyle badgeStyle = allDone ? badgeCompletedStyle : badgePendingStyle;
        GUILayout.Label(Localization.T("achievements.count_format", unlockedCount, totalValid), badgeStyle, GUILayout.Height(22));
        GUILayout.EndHorizontal();

        GUILayout.Label(Localization.T("achievements.desc"), tipLabelStyle);
        GUILayout.Space(6);

        // 2. Control Bar Card (One-click unlock all + Search)
        GUILayout.BeginVertical(cardBoxStyle);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(Localization.T("achievements.unlock_all"), primaryBtnStyle, GUILayout.Height(28), GUILayout.Width(220)))
        {
            Utilities.UnlockAllAchievementsSync();
        }

        GUILayout.Space(12);

        GUILayout.Label("🔍", boldLabelStyle, GUILayout.Width(20), GUILayout.Height(28));
        Globals.achievementSearchText = GUILayout.TextField(Globals.achievementSearchText, GUILayout.Height(26), GUILayout.ExpandWidth(true));
        if (!string.IsNullOrEmpty(Globals.achievementSearchText))
        {
            if (GUILayout.Button("✕", dangerBtnStyle, GUILayout.Width(26), GUILayout.Height(26)))
            {
                Globals.achievementSearchText = "";
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(8);

        // 3. Filtered Achievement List using cached array
        var validAchievements = Utilities.AchievementCache.ValidAchievements;
        string query = Globals.achievementSearchText != null ? Globals.achievementSearchText.Trim() : "";
        List<ACHIEVEMENTTYPE> filtered = new List<ACHIEVEMENTTYPE>(validAchievements.Length);
        for (int i = 0; i < validAchievements.Length; i++)
        {
            var type = validAchievements[i];
            if (string.IsNullOrEmpty(query))
            {
                filtered.Add(type);
            }
            else
            {
                string locName = Localization.GetAchievementName(type);
                string rawName = type.ToString();
                if (locName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    rawName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filtered.Add(type);
                }
            }
        }

        // Render in 2 columns
        for (int i = 0; i < filtered.Count; i += 2)
        {
            GUILayout.BeginHorizontal();
            DrawAchievementCard(filtered[i], 310);
            if (i + 1 < filtered.Count)
            {
                GUILayout.Space(8);
                DrawAchievementCard(filtered[i + 1], 310);
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        GUILayout.EndScrollView();
    }

    private void DrawAchievementCard(ACHIEVEMENTTYPE type, float width)
    {
        bool isUnlocked = Utilities.IsAchievementUnlocked(type);
        string locName = Localization.GetAchievementName(type);
        string rawName = type.ToString();

        GUILayout.BeginVertical(cardBoxStyle, GUILayout.Width(width));
        GUILayout.BeginHorizontal();

        // Left info column (clickable title to unlock)
        GUILayout.BeginVertical();
        if (GUILayout.Button(locName, boldLabelStyle))
        {
            Utilities.UnlockAchievement(type, true);
        }
        GUILayout.Label(rawName, tipLabelStyle);
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        // Right status column & Action button
        GUILayout.BeginVertical(GUILayout.Width(76));
        GUIStyle statusBadge = isUnlocked ? badgeCompletedStyle : badgePendingStyle;
        string statusText = isUnlocked ? Localization.T("achievements.status_unlocked") : Localization.T("achievements.status_locked");
        GUILayout.Label(statusText, statusBadge, GUILayout.Height(18));
        GUILayout.Space(2);

        if (GUILayout.Button(Localization.T("achievements.btn_unlock"), primaryBtnStyle, GUILayout.Height(22)))
        {
            Utilities.UnlockAchievement(type, true);
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    // ==========================================
    // TAB 7: ABOUT
    // ==========================================
    private void DrawAboutTab()
    {
        Globals.mainScroll = GUILayout.BeginScrollView(Globals.mainScroll);

        GUILayout.Label(Localization.T("about.title"), sectionHeaderStyle);
        GUILayout.Label(Localization.T("about.version"), boldLabelStyle);
        GUILayout.Label(Localization.T("about.author"), tipLabelStyle);

        GUILayout.Space(6);
        GUILayout.Label(Localization.T("about.description"), labelStyle);

        GUILayout.Space(8);
        GUILayout.Label(Localization.T("about.key_features"), subHeaderStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.feature1"), labelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.feature2"), labelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.feature3"), labelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.feature4"), labelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.feature5"), labelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.feature6"), labelStyle);

        GUILayout.Space(8);
        GUILayout.Label(Localization.T("about.thanks"), subHeaderStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.thanks1"), tipLabelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.thanks2"), tipLabelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.thanks3"), tipLabelStyle);
        GUILayout.Label("\u2022 " + Localization.T("about.thanks4"), tipLabelStyle);

        GUILayout.Space(8);
        GUILayout.Label(Localization.T("about.disclaimer"), tipLabelStyle);

        GUILayout.EndScrollView();
    }

    // ==========================================
    // TAB 6: LANGUAGE
    // ==========================================
    private void DrawLanguageTab()
    {
        GUILayout.Label(Localization.T("lang.title"), sectionHeaderStyle);
        GUILayout.Label(Localization.T("lang.current"), boldLabelStyle);
        GUILayout.Space(6);
        GUILayout.Label(Localization.T("lang.select"), subHeaderStyle);

        for (int i = 0; i < Localization.LanguageNames.Length; i++)
        {
            bool isActive = ((int)Localization.CurrentLanguage == i);
            GUIStyle btnStyle = isActive ? primaryBtnStyle : customSkin.button;

            if (GUILayout.Button(Localization.LanguageNames[i], btnStyle, GUILayout.Height(30)))
            {
                Localization.SetLanguage(i);
                ConfigManager.LanguageIndex.Value = i;
                Utilities.pendingItemRefresh = true;
            }
            GUILayout.Space(3);
        }
    }

    // ==========================================
    // TAB 7: DEBUG
    // ==========================================
    private void DrawDebugTab()
    {
        GUILayout.Label("Debug Utilities", sectionHeaderStyle);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Debug Slot (0-2):", GUILayout.Width(110));
        Globals.debugSlotBuffer = GUILayout.TextField(Globals.debugSlotBuffer, GUILayout.Width(60));
        if (GUILayout.Button("Inspect Slot", primaryBtnStyle, GUILayout.Width(110), GUILayout.Height(24)))
        {
            int slotNum;
            if (int.TryParse(Globals.debugSlotBuffer, out slotNum))
            {
                Utilities.GetItemsLogs(slotNum);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8);

        if (GUILayout.Button("Teleport to Origin (0, 0, 0)", GUILayout.Height(24)))
        {
            Utilities.TeleportToCoords(0f, 0f, 0f);
        }
    }

    // ==========================================
    // HELPER DRAWING METHODS
    // ==========================================
    private void DrawCheckbox(ConfigEntry<bool> config, string label, Action<bool> onChange = null)
    {
        bool current = config.Value;
        bool next = GUILayout.Toggle(current, label, toggleStyle);
        if (next != current)
        {
            config.Value = next;
            if (onChange != null)
                onChange.Invoke(next);
        }
    }

    private void DrawSliderFloat(ConfigEntry<float> config, string label, float min, float max, string format, float labelWidth = 120f, float valueWidth = 50f)
    {
        if (config == null) return;
        GUILayout.BeginHorizontal();
        if (labelWidth > 0)
            GUILayout.Label(label, GUILayout.Width(labelWidth));
        else
            GUILayout.Label(label);
        float current = config.Value;
        float next = GUILayout.HorizontalSlider(current, min, max);
        if (Math.Abs(next - current) > 0.001f)
        {
            config.Value = next;
        }
        GUILayout.Space(4);
        GUILayout.Label(string.Format(format, next), GUILayout.Width(valueWidth));
        GUILayout.EndHorizontal();
    }
}