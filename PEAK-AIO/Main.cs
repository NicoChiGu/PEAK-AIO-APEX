using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
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
    private GUIStyle sectionHeaderStyle;
    private GUIStyle subHeaderStyle;
    private GUIStyle labelStyle;
    private GUIStyle boldLabelStyle;
    private GUIStyle tipLabelStyle;
    private GUIStyle textInputStyle;
    private GUIStyle itemSelectableStyle;
    private GUIStyle itemSelectedStyle;
    private GUIStyle toggleStyle;

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
        windowStyle.border = new RectOffset(4, 4, 4, 4);
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

        skinInitialized = true;
    }

    private void OnGUI()
    {
        if (!showMenu)
            return;

        InitStyles();

        GUI.skin = customSkin;

        // Ensure window stays within screen bounds
        Globals.windowRect.x = Mathf.Clamp(Globals.windowRect.x, 0, Mathf.Max(0, Screen.width - Globals.windowRect.width));
        Globals.windowRect.y = Mathf.Clamp(Globals.windowRect.y, 0, Mathf.Max(0, Screen.height - Globals.windowRect.height));

        Globals.windowRect = GUI.Window(9999, Globals.windowRect, DrawWindow, "PEAK AIO [APEX Edition]");
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
        DrawSidebar();
        GUILayout.EndVertical();

        GUILayout.Space(6);

        // 2. Main Content Area
        GUILayout.BeginVertical(cardBoxStyle);
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
            "tab.player", "tab.items", "tab.lobby", "tab.world", "tab.about", "tab.language", "tab.debug"
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
                DrawAboutTab();
                break;
            case 6:
                DrawLanguageTab();
                break;
            case 7:
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
                var pos = Character.localCharacter.transform.position;
                coordXStr = pos.x.ToString("F1");
                coordYStr = pos.y.ToString("F1");
                coordZStr = pos.z.ToString("F1");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Space(12);

        // Right Column: Modifiers / Sliders
        GUILayout.BeginVertical(GUILayout.Width(290));

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
                Utilities.UpdateItemsSync();
            }
        }

        // Safety check for search buffers
        if (Globals.itemSearchBuffers == null || Globals.itemSearchBuffers.Length < 3)
        {
            Globals.itemSearchBuffers = new string[3] { "", "", "" };
        }
        for (int s = 0; s < 3; s++)
        {
            if (Globals.itemSearchBuffers[s] == null)
                Globals.itemSearchBuffers[s] = "";
        }

        if (Globals.slotScrolls == null || Globals.slotScrolls.Length < 3)
        {
            Globals.slotScrolls = new Vector2[3] { Vector2.zero, Vector2.zero, Vector2.zero };
        }

        if (Globals.selectedItems == null || Globals.selectedItems.Length < 3)
        {
            Globals.selectedItems = new int[3] { -1, -1, -1 };
        }

        GUILayout.BeginHorizontal();
        try
        {
            GUILayout.Label(Localization.T("tab.items"), sectionHeaderStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(string.Format(Localization.T("items.loaded_count"), Globals.items.Count), tipLabelStyle);
            if (GUILayout.Button(Localization.T("items.refresh"), GUILayout.Width(120), GUILayout.Height(22)))
            {
                Utilities.pendingItemRefresh = true;
            }
        }
        finally
        {
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(4);

        // 3 columns for slots 0, 1, 2
        GUILayout.BeginHorizontal();
        try
        {
            for (int slot = 0; slot < 3; slot++)
            {
                DrawItemSlotColumn(slot);
                if (slot < 2) GUILayout.Space(4);
            }
        }
        finally
        {
            GUILayout.EndHorizontal();
        }
    }

    private void DrawItemSlotColumn(int slot)
    {
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.Width(200));
        try
        {
            // Slot header
            GUILayout.Label(string.Format("{0} {1}", Localization.T("items.slot"), slot + 1), boldLabelStyle);

            // Current item
            string currentItemName = Localization.T("items.none");
            try
            {
                if (Player.localPlayer != null && Player.localPlayer.itemSlots != null &&
                    Player.localPlayer.itemSlots.Length > slot && Player.localPlayer.itemSlots[slot] != null &&
                    Player.localPlayer.itemSlots[slot].prefab != null)
                {
                    string n = Player.localPlayer.itemSlots[slot].prefab.GetName();
                    if (string.IsNullOrEmpty(n)) n = Player.localPlayer.itemSlots[slot].prefab.name;
                    if (!string.IsNullOrEmpty(n)) currentItemName = n;
                }
            }
            catch { }
            GUILayout.Label(string.Format("{0}: {1}", Localization.T("items.current"), currentItemName), tipLabelStyle);

            GUILayout.Space(2);

            // Search filter
            string curSearch = Globals.itemSearchBuffers[slot] ?? "";
            string newSearch = GUILayout.TextField(curSearch, GUILayout.Height(20));
            Globals.itemSearchBuffers[slot] = newSearch ?? "";
            string search = Globals.itemSearchBuffers[slot];

            // Item list
            Globals.slotScrolls[slot] = GUILayout.BeginScrollView(Globals.slotScrolls[slot], GUILayout.Height(150));
            try
            {
                bool hasItem = false;
                int itemCount = Math.Min(Globals.items.Count, Globals.itemNames.Count);
                for (int i = 0; i < itemCount; i++)
                {
                    string name = Globals.itemNames[i];
                    if (string.IsNullOrEmpty(name))
                        continue;

                    if (!string.IsNullOrEmpty(search) && name.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    hasItem = true;
                    bool isSelected = (Globals.selectedItems[slot] == i);
                    GUIStyle btnStyle = isSelected ? itemSelectedStyle : itemSelectableStyle;

                    if (GUILayout.Button(name, btnStyle))
                    {
                        Globals.selectedItems[slot] = i;
                        Utilities.AssignInventoryItem(slot, i);
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

            // Recharge section
            ConfigEntry<float> rechargeConfig = (slot == 0) ? ConfigManager.RechargeAmountSlot1 :
                (slot == 1 ? ConfigManager.RechargeAmountSlot2 : ConfigManager.RechargeAmountSlot3);

            if (rechargeConfig != null)
            {
                DrawSliderFloat(rechargeConfig, Localization.T("items.recharge"), 0f, 100f, "{0:F0}", 60f, 35f);
            }

            GUILayout.BeginHorizontal();
            try
            {
                if (rechargeConfig != null && GUILayout.Button(Localization.T("items.recharge"), GUILayout.Height(22)))
                {
                    Utilities.RechargeInventorySlot(slot, rechargeConfig.Value);
                }

                bool canSpawn = (Globals.selectedItems[slot] >= 0 && Globals.selectedItems[slot] < Globals.items.Count);
                bool prevEnabled = GUI.enabled;
                GUI.enabled = canSpawn;
                if (GUILayout.Button(Localization.T("items.spawn_item"), primaryBtnStyle, GUILayout.Height(22)))
                {
                    if (canSpawn)
                    {
                        Utilities.SpawnItemInWorld(Globals.selectedItems[slot]);
                    }
                }
                GUI.enabled = prevEnabled;
            }
            finally
            {
                GUILayout.EndHorizontal();
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
            Utilities.ReviveAllPlayers();

        if (GUILayout.Button(Localization.T("lobby.kill_all"), dangerBtnStyle, GUILayout.Height(24)))
            Utilities.KillAllPlayers();
        GUILayout.EndHorizontal();

        Globals.excludeSelfFromAllActions = GUILayout.Toggle(Globals.excludeSelfFromAllActions, Localization.T("lobby.exclude_self"));

        GUILayout.Space(4);
        if (GUILayout.Button(Localization.T("lobby.warp_all_to_me"), GUILayout.Height(24)))
            Utilities.WarpAllPlayersToMe();

        GUILayout.EndVertical();

        GUILayout.Space(8);

        // Right Column: Selected Player Actions
        GUILayout.BeginVertical(cardBoxStyle, GUILayout.Width(310));

        GUILayout.Label(Localization.T("lobby.actions"), sectionHeaderStyle);

        if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Globals.allPlayers.Count)
        {
            string selName = Globals.playerNames[Globals.selectedPlayer];
            GUILayout.Label(string.Format("Target: {0}", selName), boldLabelStyle);

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.revive"), primaryBtnStyle, GUILayout.Height(26)))
                Utilities.ReviveSelectedPlayer();

            if (GUILayout.Button(Localization.T("lobby.kill"), dangerBtnStyle, GUILayout.Height(26)))
                Utilities.KillSelectedPlayer();
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localization.T("lobby.warp_to"), GUILayout.Height(26)))
                Utilities.WarpToSelectedPlayer();

            if (GUILayout.Button(Localization.T("lobby.warp_to_me"), GUILayout.Height(26)))
                Utilities.WarpSelectedPlayerToMe();
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            GUILayout.Label(Localization.T("lobby.special_actions"), subHeaderStyle);

            if (GUILayout.Button(Localization.T("lobby.spawn_scoutmaster"), dangerBtnStyle, GUILayout.Height(26)))
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
    // TAB 4: WORLD (Updated Map & Containers)
    // ==========================================
    private void DrawWorldTab()
    {
        Utilities.EnsureLuggageListInitialized();

        Globals.mainScroll = GUILayout.BeginScrollView(Globals.mainScroll);

        // --- Map / Segment Jump & Route Section ---
        GUILayout.BeginVertical(cardBoxStyle);

        // Header
        GUILayout.Label(Localization.T("world.segment_teleport"), sectionHeaderStyle);
        GUILayout.Space(4);

        // Current Area Detection
        Segment detectedSeg = Utilities.DetectCurrentPlayerSegment();
        int currentLevel = (int)detectedSeg + 1;
        float altitude = Utilities.GetCurrentPlayerAltitude();
        List<Utilities.RouteSegmentInfo> route = Utilities.GetFullRoute();
        bool isAtCampfire = (currentLevel <= 5) && Utilities.IsPlayerNearCampfire((int)detectedSeg);

        string currentSegDisplayName = "Unknown";
        for (int i = 0; i < route.Count; i++)
        {
            if (route[i].segment == detectedSeg)
            {
                currentSegDisplayName = route[i].displayName;
                break;
            }
        }

        GUILayout.BeginHorizontal();
        string statusText = string.Format("{0} {1} - {2}",
            Localization.T("world.current_segment"),
            string.Format(Localization.T("world.level_label"), currentLevel),
            currentSegDisplayName);
        if (isAtCampfire)
        {
            statusText += "  " + Localization.T("world.at_campfire_tag");
        }
        GUILayout.Label(statusText, boldLabelStyle);

        GUILayout.FlexibleSpace();
        if (altitude != 0f)
        {
            GUILayout.Label(string.Format("{0} {1:F1} m", Localization.T("world.current_altitude"), altitude), tipLabelStyle);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        // Prominent Button(s): Teleport to Campfire / Light Campfire
        if (currentLevel < 6)
        {
            int nextLevel = currentLevel + 1;
            string nextName = (currentLevel < route.Count) ? route[currentLevel].displayName : "";

            if (!isAtCampfire)
            {
                string btnText = string.Format("{0} ({1} {2})",
                    Localization.T("world.teleport_next_campfire"),
                    string.Format(Localization.T("world.level_label"), nextLevel),
                    nextName);

                if (GUILayout.Button(btnText, primaryBtnStyle, GUILayout.Height(32)))
                {
                    Utilities.TeleportToNextCampfire();
                }
            }
            else
            {
                // Player is already at the transition campfire!
                GUILayout.BeginHorizontal();
                string lightText = string.Format(Localization.T("world.light_campfire"), nextLevel);
                if (GUILayout.Button(lightText, primaryBtnStyle, GUILayout.Height(32)))
                {
                    Utilities.LightCurrentCampfire();
                }

                GUILayout.Space(6);

                int afterNextLevel = nextLevel + 1;
                string nextCampText = (nextLevel < 6)
                    ? string.Format(Localization.T("world.teleport_next_area_campfire"), afterNextLevel)
                    : Localization.T("world.teleport_to_peak");

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

        GUILayout.Space(8);

        // Full Route Visualization (Breadcrumbs)
        GUILayout.Label(Localization.T("world.route_header") + ":", subHeaderStyle);
        GUILayout.Space(2);

        GUILayout.BeginHorizontal();
        for (int i = 0; i < route.Count; i++)
        {
            var r = route[i];
            bool isCur = (r.segment == detectedSeg);
            GUIStyle stepStyle = isCur ? sidebarActiveBtnStyle : sidebarBtnStyle;
            string stepText = string.Format("{0}. {1}", r.level, r.displayName);
            if (isCur && r.isAtCampfire)
            {
                stepText += " 🔥";
            }
            if (GUILayout.Button(stepText, stepStyle, GUILayout.Height(24)))
            {
                Utilities.JumpToSegment(r.segment);
            }
            if (i < route.Count - 1)
            {
                GUILayout.Label("➔", boldLabelStyle, GUILayout.Width(16));
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8);

        // Detailed Area & Campfire Jump Controls
        GUILayout.Label(Localization.T("world.jump_to_segment") + ":", subHeaderStyle);
        GUILayout.Space(2);

        for (int i = 0; i < route.Count; i++)
        {
            var r = route[i];
            bool isCur = (r.segment == detectedSeg);

            GUILayout.BeginHorizontal();

            // Label for Level and Biome name
            string segLabel = string.Format("{0}: {1}", string.Format(Localization.T("world.level_label"), r.level), r.displayName);
            if (isCur)
            {
                segLabel += r.isAtCampfire ? "  [🔥]" : "  [✓]";
            }
            GUILayout.Label(segLabel, isCur ? boldLabelStyle : labelStyle, GUILayout.Width(200));

            // Button 1: Jump to Start of segment
            GUIStyle jumpBtnStyle = isCur ? sidebarActiveBtnStyle : primaryBtnStyle;
            string jumpStartText = string.Format("{0}", Localization.T("world.jump_to_start"));
            if (GUILayout.Button(jumpStartText, jumpBtnStyle, GUILayout.Height(24), GUILayout.Width(95)))
            {
                Utilities.JumpToSegment(r.segment);
            }

            GUILayout.Space(4);

            // Button 2: Teleport to Campfire (or Peak action)
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
            else
            {
                if (GUILayout.Button(Localization.T("world.teleport_to_peak"), primaryBtnStyle, GUILayout.Height(24), GUILayout.Width(125)))
                {
                    Utilities.JumpToSegment(r.segment);
                }
            }

            GUILayout.EndHorizontal();

            if (i < route.Count - 1)
                GUILayout.Space(3);
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
    // TAB 5: ABOUT
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