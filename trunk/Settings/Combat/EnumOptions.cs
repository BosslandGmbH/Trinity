
namespace Trinity.Settings.Combat
{
    public enum GoblinPriority
    {
        Ignore = 0,
        Normal = 1,
        Prioritize = 2,
        Kamikaze = 3
    }
    public enum TempestRushOption
    {
        MovementOnly = 0,
        ElitesGroupsOnly = 1,
        CombatOnly = 2,
        Always = 3,
        TrashOnly = 4,
    }
    public enum WizardKiteOption
    {
        Anytime,
        ArchonOnly,
        NormalOnly
    }

    public enum WizardArchonCancelOption
    {
        Never,
        Timer,
        RebuffArmor,
        RebuffMagicWeaponFamiliar,
    }

    public enum DestructibleIgnoreOption
    {
        ForceIgnore,
        OnlyIfStuck,
        DestroyAll
    }

    public enum CrusaderAkaratsMode
    {
        HardElitesOnly,
        Normal,
        WhenReady,
        WhenInCombat
    }

    public enum MonkEpiphanyMode
    {
        HardElitesOnly,
        Normal,
        WhenReady,
        WhenInCombat
    }

    public enum BarbarianWOTBMode
    {
        HardElitesOnly,
        Normal,
        WhenReady,
        WhenInCombat
    }

    public enum BarbarianSprintMode
    {
        Always,
        CombatOnly,
        MovementOnly
    }

    public enum DemonHunterVaultMode
    {
        Always,
        CombatOnly,
        MovementOnly
    }

    public enum KiteMode
    {
        Never,
        Always,
        Bosses,
        Elites,
    }

    public enum TrinityItemQuality
    {
        Invalid = -1,
        None = 0,
        Inferior,
        Common,
        Magic,
        Rare,
        Legendary,
        Set
    }

    public enum SettingMode
    {
        None = 0,
        Enabled,
        Disabled,
        Selective
    }
}

