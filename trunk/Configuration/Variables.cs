using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Trinity.Components.Combat;
using Trinity.Config;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Modules;
using Trinity.ItemRules;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public static class TrinityPluginSettings
    {
        public static TrinitySetting Settings = new TrinitySetting();
    }

    public partial class TrinityPlugin
    {
        /// <summary>
        /// Settings of the plugin
        /// </summary>
        public static TrinitySetting Settings => TrinityPluginSettings.Settings;

        /// <summary>
        /// Used for letting noobs know they started the bot without TrinityPlugin enabled in the plugins tab.
        /// </summary>
        public static bool IsPluginEnabled
        {
            get { return _isPluginEnabled; }
            set { _isPluginEnabled = value; }
        }
        public static bool _isPluginEnabled;

        /// <summary>
        /// Used for a global bot-pause
        /// </summary>
        public const bool MainBotPaused = false;

        /// <summary>
        /// Used to force-refresh dia objects at least once every XX milliseconds
        /// </summary>
        public static DateTime LastRefreshedCache = DateTime.MinValue;

        //intell
        public static DateTime TimeToRunFromPoison = DateTime.MinValue;
        public static DateTime LogTest = DateTime.MinValue;
        public static bool RunFromPoison = false;
        public static bool GotFrenzyShrine = false;
        public static bool GotBlessedShrine = false;
        public static bool PrioritizeCloseRangeUnits = false;

        /// <summary>
        /// This object is used for the main handling - the "current target" etc. as selected by the target-selecter, whether it be a unit, an item, a shrine, anything. 
        /// It's cached data using my own class, so I never need to hit D3 memory to "re-check" the data or to call an interact request or anything
        /// </summary>
        internal static TrinityActor CurrentTarget
        {
            get { return _currentTarget; }
            set
            {
                _currentTarget = value;
            }
        }

        /// <summary>
        /// A flag to indicate if we should pick a new power/ability to use or not
        /// </summary>
        public static bool _shouldPickNewAbilities;



        /// <summary>
        /// Status text for DB main window status
        /// </summary>
        public static string _statusText = "";

        /// <summary>
        /// Timestamp of when our position was last measured as changed
        /// </summary>
        public static DateTime _lastMovedDuringCombat = DateTime.MinValue;

        /// <summary>
        /// Used to ignore a specific RActor for <see cref="_ignoreTargetForLoops"/> ticks
        /// </summary>
        public static int _ignoreRactorGuid;

        /// <summary>
        /// Ignore <see cref=" _ignoreRactorGuid"/> for this many ticks
        /// </summary>
        public static int _ignoreTargetForLoops;

        /// <summary>
        /// Holds all of the player's current info handily cached, updated once per loop with a minimum timer on updates to save D3 memory hits
        /// </summary>
        public static PlayerCache Player => Core.Player;

        public static PlayerMover PlayerMover => PlayerMover.Instance;


        // Also storing a list of all profiles, for experimental reasons/incase I want to use them down the line
        public static List<string> ProfileHistory = new List<string>();
        public static string CurrentProfile = "";
        public static string CurrentProfileName = "";
        public static string FirstProfile = "";

        // A list of small areas covering zones we move through while fighting to help our custom move-handler skip ahead waypoints
        //internal static HashSet<CacheObstacleObject> SkipAheadAreaCache = new HashSet<CacheObstacleObject>();
        public static DateTime LastAddedLocationCache = DateTime.MinValue;
        public static Vector3 LastRecordedPosition = Vector3.Zero;
        public static bool SkipAheadAGo = false;


        public static DateTime _lastClearedAvoidanceBlackspots = DateTime.MinValue;

        // A count for player mystic ally, gargantuans, and zombie dogs
        //internal static int PlayerOwnedMysticAllyCount = 0;
        //internal static int PlayerOwnedGargantuanCount = 0;
        //internal static int PlayerOwnedZombieDogCount = 0;
        //internal static int PlayerOwnedFetishArmyCount = 0;
        //internal static int PlayerOwnedDHPetsCount = 0;
        //internal static int PlayerOwnedDHSentryCount = 0;
        //internal static int PlayerOwnedHydraCount = 0;
        //internal static int PlayerOwnedAncientCount = 0;       

        // These are a bunch of safety counters for how many times in a row we register having *NO* ability to select when we need one (eg all off cooldown)

        // After so many, give the player a friendly warning to check their skill/build setup
        public static int NoAbilitiesAvailableInARow = 0;
        public static DateTime lastRemindedAboutAbilities = DateTime.MinValue;

        // Last had any mob in range, for loot-waiting
        internal static DateTime lastHadUnitInSights = DateTime.MinValue;

        // When we last saw a boss/elite etc.
        internal static DateTime lastHadEliteUnitInSights = DateTime.MinValue;
        internal static DateTime lastHadBossUnitInSights = DateTime.MinValue;

        internal static DateTime lastHadContainerInSights = DateTime.MinValue;

        // Do we need to reset the debug bar after combat handling?
        public static bool _resetStatusText;

        // Death counts
        public static int DeathsThisRun = 0;

        // Force a target update after certain interactions
        public static bool _forceTargetUpdate;

        /// <summary>
        /// This holds whether or not we want to prioritize a close-target, used when we might be body-blocked by monsters
        /// </summary>
        public static bool _forceCloseRangeTarget;

        // How many times a movement fails because of being "blocked"
        public static int _timesBlockedMoving;

        // how long to force close-range targets for
        public const int ForceCloseRangeForMilliseconds = 0;

        // Date time we were last told to stick to close range targets
        public static DateTime _lastForcedKeepCloseRange = DateTime.MinValue;


        // Caching of the current primary target's health, to detect if we AREN'T damaging it for a period of time
        public static double _targetLastHealth;

        // This is used so we don't use certain skills until we "top up" our primary resource by enough
        internal static double MinEnergyReserve = 0d;



        // These values below are set on a per-class basis later on, so don't bother changing them here! These are the old default values
        public static double PlayerEmergencyHealthPotionLimit = 0.35;
        public static double _playerEmergencyHealthGlobeLimit = 0.35;
        public static double _playerHealthGlobeResource = 0.35;

        /*
         *  Blacklists
         */
        internal static bool NeedToClearBlacklist3 = false;
        internal static DateTime Blacklist1LastClear = DateTime.MinValue;
        internal static DateTime Blacklist3LastClear = DateTime.MinValue;
        internal static DateTime Blacklist15LastClear = DateTime.MinValue;
        internal static DateTime Blacklist60LastClear = DateTime.MinValue;
        internal static DateTime Blacklist90LastClear = DateTime.MinValue;

        /// <summary>
        /// Use RActorGUID to blacklist an object/monster for 1 seconds
        /// </summary>
        internal static HashSet<int> Blacklist1Second = new HashSet<int>();
        /// <summary>
        /// Use RActorGUID to blacklist an object/monster for 3 seconds
        /// </summary>
        internal static HashSet<int> Blacklist3Seconds = new HashSet<int>();
        /// <summary>
        /// Use RActorGUID to blacklist an object/monster for 15 seconds
        /// </summary>
        internal static HashSet<int> Blacklist15Seconds = new HashSet<int>();
        /// <summary>
        /// Use RActorGUID to blacklist an object/monster for 60 seconds
        /// </summary>
        internal static HashSet<int> Blacklist60Seconds = new HashSet<int>();
        /// <summary>
        /// Use RActorGUID to blacklist an object/monster for 90 seconds
        /// </summary>
        internal static HashSet<int> Blacklist90Seconds = new HashSet<int>();

        // This is a blacklist that is cleared within 3 seconds of last attacking a destructible
        public static HashSet<int> _destructible3SecBlacklist = new HashSet<int>();
        public static DateTime _lastDestroyedDestructible = DateTime.MinValue;
        public static bool _needClearDestructibles;

        // The number of loops to extend kill range for after a fight to try to maximize kill bonus exp etc.
        public static int _keepKillRadiusExtendedForSeconds;
        public static DateTime _timeKeepKillRadiusExtendedUntil = DateTime.MinValue;

        // The number of loops to extend loot range for after a fight to try to stop missing loot
        public static int _keepLootRadiusExtendedForSeconds;

        // Some avoidance related variables

        /// <summary>
        /// Whether or not we need avoidance this target-search-loop
        /// </summary>
        public static bool _standingInAvoidance;

        public static string _currentAvoidanceName;

        public static TrinityActor _currentAvoidance;

        /// <summary>
        /// This lets us know if there is a target but it's in avoidance so we can just "stay put" until avoidance goes
        /// </summary>
        public static bool _shouldStayPutDuringAvoidance;

        /// <summary>
        /// This force-prevents avoidance for XX loops incase we get stuck trying to avoid stuff
        /// </summary>
        public static readonly DateTime timeCancelledEmergencyMove = DateTime.MinValue;
        public static int cancelledEmergencyMoveForMilliseconds = 0;

        /// <summary>
        /// Prevent spam-kiting too much - allow fighting between each kite movement
        /// </summary>
        public static DateTime timeCancelledKiteMove = DateTime.MinValue;
        public static int cancelledKiteMoveForMilliseconds = 0;

        // Variable to let us force new target creations immediately after a root
        public static bool wasRootedLastTick = false;

        // Variables used to actually hold powers the power-selector has picked to use, for buffing and main power use
        public static TrinityPower powerBuff;

        public static SNOPower lastPowerUsed = SNOPower.None;
        public static SNOPower LastPowerUsed
        {
            get { return TrinityPlugin.lastPowerUsed; }
            set { TrinityPlugin.lastPowerUsed = value; }
        }

        public static bool DisableOutofCombatSprint = false;
        public static bool OnlyTarget = false;

        // Target provider and core routine variables
        //public static bool AnyElitesPresent = false;
        public static bool AnyTreasureGoblinsPresent = false;
        public static bool AnyMobsInRange = false;
        public static float CurrentBotKillRange = 0f;
        public static float CurrentBotLootRange = 0f;
        internal static bool MaintainTempestRush = false;


        // Goblinney things
        public static int TotalNumberGoblins = 0;
        public static DateTime lastGoblinTime = DateTime.MinValue;


        public static bool IsAlreadyMoving = false;
        public static Vector3 LastMoveToTarget;
        public static float LastDistanceFromTarget;
        public static DateTime lastMovementCommand = DateTime.MinValue;

        // Contains our apparent *CURRENT* hotbar abilities, cached in a fast hash
        public static List<SNOPower> Hotbar = new List<SNOPower>();

        // Contains a hash of our LAST hotbar abilities before we transformed into archon (for quick and safe hotbar restoration)
        internal static List<SNOPower> hashCachedPowerHotbarAbilities = new List<SNOPower>();

        // A list and a dictionary for quick buff checking and buff references
        internal static Dictionary<int, int> PlayerBuffs = new Dictionary<int, int>();

        // For "position-shifting" to navigate around obstacle SNO's
        internal static Vector3 ShiftedPosition = Vector3.Zero;
        internal static DateTime lastShiftedPosition = DateTime.MinValue;
        internal static int ShiftPositionFor = 0;
        internal static Vector3 CurrentDestination;
        public static int CurrentWorldDynamicId = -1;
        public static int CurrentWorldId = -1; // worldId from profiles, used in persistent stats

        /// <summary>
        /// Do not wait for animation after using, spam the power (false)
        /// </summary>
        public const bool NO_WAIT_ANIM = false;
        /// <summary>
        /// Wait for animation after using, do not spam the power (true)
        /// </summary>
        public const bool WAIT_FOR_ANIM = true;




        // Whether to try forcing a vendor-run for custom reasons
        public static bool ForceVendorRunASAP = false;
        public static bool _wantToTownRun;
        public static bool WantToTownRun { get { return _wantToTownRun; } set { _wantToTownRun = value; } }

        // Stash mapper - it's an array representing every slot in your stash, true or false dictating if the slot is free or not
        public static bool[,] StashSlotBlocked = new bool[7, 30];

        /*
         * From RefreshDiaObject
         *
         * Bunch of temporary variables that get used when creating the current object/target list - this was just a nicer way for me to handle it code wise at first
         * Even if it looks a bit messy and probably should have just used it's own object instance of the cache-class instead! :D
         * c_ variables are all used in the caching mechanism
         */

        /// <summary>
        /// This contains the active cache of valid objects 
        /// </summary>
        internal static List<TrinityActor> Targets => Core.Targets.Items;

        // From main RefreshDiaobjects
        /// <summary>
        /// The position of the last CurrentTarget (Primary Target)
        /// </summary>
        internal static Vector3 LastTargetPosition;
        public static Vector3 KiteAvoidDestination;
        /// <summary>
        /// The RActorGUID of the last CurrentTarget (PrimaryTarget)
        /// </summary>
        public static int LastTargetRactorGUID;

        internal static int LastTargetAcdId;
        /// <summary>
        /// The number of monsters within melee range distance of the player
        /// </summary>
        public static double HighestWeightFound;

        public static bool NeedToKite = false;

        /// <summary>
        /// Used for trimming off numbers from object names in RefreshDiaObject
        /// </summary>
        internal static Regex NameNumberTrimRegex = new Regex(@"-\d+$", RegexOptions.Compiled);

        // The following 2 variables are used to clear the dictionaries out - clearing one dictionary out per maximum every 2 seconds, working through in sequential order
        public static DateTime lastClearedCacheDictionary = DateTime.MinValue;

        // On death, clear the timers for all abilities
        internal static DateTime LastDeathTime = DateTime.MinValue;
        public static int _totalDeaths = 0;

        internal static int TotalDeaths
        {
            get { return TrinityPlugin._totalDeaths; }
            set { TrinityPlugin._totalDeaths = value; }
        }

        // When did we last send a move-power command?
        public static DateTime lastSentMovePower = DateTime.MinValue;


        /// <summary>
        /// If we should force movement
        /// </summary>
        public static bool ForceNewMovement = false;

        /// <summary>
        /// Store player current position
        /// </summary>
        Vector3 CurrentPlayerPosition = Vector3.Zero;

        // For path finding
        /// <summary>
        /// The Grid Provider for Navigation checks
        /// </summary>
        internal static MainGridProvider MainGridProvider
        {
            get
            {
                return (MainGridProvider)Navigator.SearchGridProvider;

                //if (Navigator.SearchGridProvider.GetType() == typeof(MainGridProvider))
                //    return (MainGridProvider)Navigator.SearchGridProvider;
                //else if (Navigator.SearchGridProvider.GetType() == typeof(DbProvider.SearchAreaProvider))
                //    return (DbProvider.SearchAreaProvider)Navigator.SearchGridProvider;
                //else
                //    return Navigator.SearchGridProvider;
            }
        }
        /// <summary>
        /// The PathFinder for Navigation checks
        /// </summary>
        //internal static PathFinder pf;




        // Darkfriend's Looting Rule
        public static Interpreter StashRule = null; // = new Interpreter();

        // Tesslerc - used for using combination strike
        // ForesightFirstHit is used to track the 30 second buff from deadly reach.
        public static DateTime ForeSightFirstHit = new DateTime(1996, 6, 3, 22, 15, 0);
        // Foresight2 is used to track combination strike buff.
        public static DateTime ForeSight2 = DateTime.MinValue;
        // Otherthandeadlyreach is used for other spirit generators to track for combination strike buff.
        public static DateTime OtherThanDeadlyReach = DateTime.MinValue;

        /// <summary>
        /// And a "global cooldown" to prevent non-signature-spells being used too fast
        /// </summary>
        public static DateTime lastGlobalCooldownUse = DateTime.MinValue;

        // track bounties completed
        public static int TotalBountyCachesOpened = 0;

        // Xp Counter
        public static Vector3 eventStartPosition = Vector3.Zero;
        public static Vector3 EventStartPosition
        {
            get { return TrinityPlugin.eventStartPosition; }
            set { TrinityPlugin.eventStartPosition = value; }
        }

        public static DateTime eventStartTime = DateTime.MinValue;
        public static TrinityActor _currentTarget;

        public static DateTime EventStartTime
        {
            get { return TrinityPlugin.eventStartTime; }
            set { TrinityPlugin.eventStartTime = value; }
        }

        public static int PlayerOwnedSpiderPetsCount { get; set; }
        public static bool LastTargetIsSafeSpot { get; set; }

        public static DateTime LastWorldChangeTime = DateTime.MinValue;
    }
}
