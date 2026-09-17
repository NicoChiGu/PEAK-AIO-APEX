using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Globals
{
    // Boolean
    public static bool anyAfflictionEnabled;

    // Objects
    public static Character character;
    public static CharacterData characterData;

    public static FieldInfo staminaField;
    public static PropertyInfo infiniteStamProp;

    public static FieldInfo sinceFallSlideField;
    public static FieldInfo sinceGroundedField;

    public static object movementComp;
    public static FieldInfo movementModifierField;
    public static FieldInfo jumpGravityField;
    public static FieldInfo fallDamageTimeField;

    public static object characterClimb;
    public static FieldInfo climbSpeedModifierField;

    public static object characterVineClimb;
    public static FieldInfo vineClimbSpeedModifierField;

    public static object characterRopeHandling;
    public static FieldInfo ropeClimbSpeedModifierField;

    public static object afflictionsObj;
    public static MethodInfo setStatusMethod;
    public static object weightEnumValue;
    public static object poisonEnumValue;
    public static object hotEnumValue;
    public static object coldEnumValue;
    public static object curseEnumValue;
    public static object injuryEnumValue;
    public static object drowsyEnumValue;
    public static object hungerEnumValue;

    // Inventory
    public static List<Item> items = new List<Item>();
    public static List<string> itemNames = new List<string>();
    public static int[] selectedItems = new int[] { -1, -1, -1, -1 };
    public static string[] itemDisplayNames = new string[] { "None", "None", "None", "None" };
    public static string[] itemSearchBuffers = new string[4] { "", "", "", "" };

    // Player
    public static Player playerObj;

    // Lobby
    public static List<Character> allPlayers = new List<Character>();
    public static List<string> playerNames = new List<string>();
    public static int selectedPlayer = -1;
    public static bool excludeSelfFromAllActions = true;
    public static string lobbyItemSearchBuffer = "";
    public static int selectedLobbyItem = -1;
    public static Vector2 lobbyItemScroll = Vector2.zero;

    // Teleport
    public static bool teleportToPingEnabled = false;
    public static float teleportX = 0f;
    public static float teleportY = 0f;
    public static float teleportZ = 0f;

    // World
    public static int selectedLuggageIndex = -1;
    public static List<string> luggageLabels = new List<string>();
    public static List<Luggage> luggageObject = new List<Luggage>();
    public static List<Luggage> allOpenedLuggage = new List<Luggage>();
    public static int selectedSegment = 0;

    // Debug
    public static string debugSlotBuffer = "0";

    // GUI State
    public static Rect windowRect = new Rect(40f, 40f, 780f, 520f);
    public static Vector2 sidebarScroll = Vector2.zero;
    public static Vector2 mainScroll = Vector2.zero;
    public static Vector2[] slotScrolls = new Vector2[4] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public static Vector2 lobbyPlayerScroll = Vector2.zero;
    public static Vector2 luggageScroll = Vector2.zero;

    // Inventory Snapshot & Anti-Duplication Revive System
    public class ItemSlotSnapshot
    {
        public Item prefab;
        public ItemInstanceData data;
    }

    public class PlayerInventorySnapshot
    {
        public int photonId;
        public ItemSlotSnapshot[] mainSlots = new ItemSlotSnapshot[3];
        public BackpackSlot.BackpackType backpackType = BackpackSlot.BackpackType.None;
        public ItemSlotSnapshot backpackSlotItem;
        public List<ItemSlotSnapshot> innerBackpackItems = new List<ItemSlotSnapshot>();
        public bool isConsumed = false;
        public DateTime snapshotTime;
    }

    public static Dictionary<int, PlayerInventorySnapshot> deathSnapshots = new Dictionary<int, PlayerInventorySnapshot>();
    public static Dictionary<int, PlayerInventorySnapshot> liveSnapshots = new Dictionary<int, PlayerInventorySnapshot>();

    // Location Snapshot System (Prevents reviving into void/falling through map)
    public class PlayerLocationSnapshot
    {
        public Vector3 safePosition;
        public float lastRecordedTime;
    }
    public static Dictionary<int, PlayerLocationSnapshot> playerSafeLocations = new Dictionary<int, PlayerLocationSnapshot>();
    public static Dictionary<int, Vector3> playerDeathLocations = new Dictionary<int, Vector3>();

    // Global Error & Notification System
    public static class GlobalNotifier
    {
        public static string CurrentErrorMessage = null;
        public static float ExpireTime = 0f;
        public static float Duration = 5f;

        public static void ShowError(string message, float duration = 5f)
        {
            CurrentErrorMessage = message;
            Duration = duration;
            ExpireTime = Time.unscaledTime + duration;
        }

        public static void Clear()
        {
            CurrentErrorMessage = null;
        }
    }

    // Creature Spawner State
    public enum CreatureType
    {
        Scoutmaster,
        BigGhost,
        MushroomZombie,
        Scorpion,
        Beetle,
        BeeSwarm
    }

    public enum CreatureSpawnAnchor
    {
        Self,
        SelectedPlayer,
        Crosshair
    }

    public static CreatureType selectedCreatureType = CreatureType.Scoutmaster;
    public static CreatureSpawnAnchor creatureSpawnAnchor = CreatureSpawnAnchor.Self;
    public static float creatureSpawnDistance = 5.0f;
    public static int creatureAggroTargetIndex = -1; // -1: Self, >=0: Selected Player index
    public static float creatureGhostScale = 3.0f;
    public static float lastCreatureSpawnTime = 0f;
    public static Vector2 creaturesScroll = Vector2.zero;

    // ==========================================
    // Item Attributes & Enchantment States
    // ==========================================
    public enum MushroomSpawnMode
    {
        Vanilla,
        Purified,       // Random Good (0-4)
        Toxic,          // Random Bad (5-9)
        Specific        // Specific Effect (0-9)
    }

    public enum DartAmmoType
    {
        // Buffs
        Invincibility,
        SpeedBoost,
        InfiniteStamina,
        FullCleanse,
        LowGravity,
        Glow,
        Revive,

        // Debuffs
        Poison,
        Starvation,
        Sleep,
        TripFall,
        Thorns,
        Spores,
        Blind,
        Numb,

        // Chaos
        Chaos
    }

    public static MushroomSpawnMode mushroomSpawnMode = MushroomSpawnMode.Vanilla;
    public static int selectedMushroomEffect = 3; // Default: Invincibility

    public static bool dartAmmoEnabled = false;
    public static DartAmmoType selectedDartAmmoType = DartAmmoType.Invincibility;

    public static bool foodPoisonImmunity = false;
    public static bool infiniteToolCharge = false;

    // Status & Afflictions Management
    public static int selfSelectedStatusIndex = 0;
    public static float selfStatusAmount = 0.5f;
    public static int lobbySelectedStatusIndex = 0;
    public static float lobbyStatusAmount = 0.5f;

    public static readonly CharacterAfflictions.STATUSTYPE[] AllStatusTypes = new CharacterAfflictions.STATUSTYPE[]
    {
        CharacterAfflictions.STATUSTYPE.Injury,
        CharacterAfflictions.STATUSTYPE.Hunger,
        CharacterAfflictions.STATUSTYPE.Cold,
        CharacterAfflictions.STATUSTYPE.Poison,
        CharacterAfflictions.STATUSTYPE.Crab,
        CharacterAfflictions.STATUSTYPE.Curse,
        CharacterAfflictions.STATUSTYPE.Drowsy,
        CharacterAfflictions.STATUSTYPE.Weight,
        CharacterAfflictions.STATUSTYPE.Hot,
        CharacterAfflictions.STATUSTYPE.Thorns,
        CharacterAfflictions.STATUSTYPE.Spores,
        CharacterAfflictions.STATUSTYPE.Web
    };
}
