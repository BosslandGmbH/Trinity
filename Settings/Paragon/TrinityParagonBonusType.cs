using Zeta.Game.Internals.Actors;

namespace Trinity.Settings.Paragon
{
    /// <summary>
    /// Paragon bonus type for friendly UI Display.
    /// </summary>
    public enum TrinityParagonBonusType
    {
        None = 0,

        PrimaryStat = 1,
        Resource = 2,
        Vitality = ParagonBonusType.Vitality,
        MovementSpeed = ParagonBonusType.MovementBonusRunSpeed,

        AttackSpeed = ParagonBonusType.AttacksPerSecondBonus,
        CooldownReduction = ParagonBonusType.PowerCooldownReductionPercent,
        CriticalChance = ParagonBonusType.CritPercentBonusCapped,
        CriticalDamage = ParagonBonusType.CritDamagePercent,

        Life = ParagonBonusType.HitpointsPercent,
        Armor = ParagonBonusType.ArmorBonusPercent,
        ResistAll = ParagonBonusType.ResistanceAll,
        LifeRegen = ParagonBonusType.HitpointsRegenPerSecond,

        AreaDamage = ParagonBonusType.SplashDamageEffectPercent,
        ResourceCost = ParagonBonusType.ResourceCostReductionPercentAll,
        LifeOnHit = ParagonBonusType.HitpointsOnHit,
        GoldFind = ParagonBonusType.GoldFind,
    }
}