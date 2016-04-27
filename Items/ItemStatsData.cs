namespace Trinity.Items
{
    public class ItemStatsData
    {
        public float HatredRegen { get; set; }
        public float MaxDiscipline { get; set; }
        public float MaxArcanePower { get; set; }
        public float MaxMana { get; set; }
        public float MaxFury { get; set; }
        public float MaxSpirit { get; set; }
        public float ManaRegen { get; set; }
        public float SpiritRegen { get; set; }
        public float ArcaneOnCrit { get; set; }
        public float HealthPerSpiritSpent { get; set; }
        public float AttackSpeedPercent { get; set; }
        public float AttackSpeedPercentBonus { get; set; }
        public string Quality { get; set; }
        public int Level { get; set; }
        public int ItemLevelRequirementReduction { get; set; }
        public int RequiredLevel { get; set; }
        public float CritPercent { get; set; }
        public float CritDamagePercent { get; set; }
        public float BlockChance { get; set; }
        public float BlockChanceBonus { get; set; }
        public float HighestPrimaryAttribute { get; set; }
        public float Intelligence { get; set; }
        public float Vitality { get; set; }
        public float Strength { get; set; }
        public float Dexterity { get; set; }
        public float Armor { get; set; }
        public float ArmorBonus { get; set; }
        public float ArmorTotal { get; set; }
        public int Sockets { get; set; }
        public float LifeSteal { get; set; }
        public float LifeOnHit { get; set; }
        public float LifeOnKill { get; set; }
        public float MagicFind { get; set; }
        public float GoldFind { get; set; }
        public float ExperienceBonus { get; set; }
        public float WeaponOnHitSlowProcChance { get; set; }
        public float WeaponOnHitBlindProcChance { get; set; }
        public float WeaponOnHitChillProcChance { get; set; }
        public float WeaponOnHitFearProcChance { get; set; }
        public float WeaponOnHitFreezeProcChance { get; set; }
        public float WeaponOnHitImmobilizeProcChance { get; set; }
        public float WeaponOnHitKnockbackProcChance { get; set; }
        public float WeaponOnHitBleedProcChance { get; set; }
        public float WeaponDamagePercent { get; set; }
        public float WeaponAttacksPerSecond { get; set; }
        public float WeaponMinDamage { get; set; }
        public float WeaponMaxDamage { get; set; }
        public float WeaponDamagePerSecond { get; set; }
        public string WeaponDamageType { get; set; }
        public float MaxDamageElemental { get; set; }
        public float MinDamageElemental { get; set; }
        public float MinDamageFire { get; set; }
        public float MaxDamageFire { get; set; }
        public float MinDamageLightning { get; set; }
        public float MaxDamageLightning { get; set; }
        public float MinDamageCold { get; set; }
        public float MaxDamageCold { get; set; }
        public float MinDamagePoison { get; set; }
        public float MaxDamagePoison { get; set; }
        public float MinDamageArcane { get; set; }
        public float MaxDamageArcane { get; set; }
        public float MinDamageHoly { get; set; }
        public float MaxDamageHoly { get; set; }
        public float OnHitAreaDamageProcChance { get; set; }
        public float PowerCooldownReductionPercent { get; set; }
        public float ResourceCostReductionPercent { get; set; }
        public float PickUpRadius { get; set; }
        public float MovementSpeed { get; set; }
        public float HealthGlobeBonus { get; set; }
        public float HealthPerSecond { get; set; }
        public float LifePercent { get; set; }
        public float DamagePercentBonusVsElites { get; set; }
        public float DamagePercentReductionFromElites { get; set; }
        public float Thorns { get; set; }
        public float ResistAll { get; set; }
        public float ResistArcane { get; set; }
        public float ResistCold { get; set; }
        public float ResistFire { get; set; }
        public float ResistHoly { get; set; }
        public float ResistLightning { get; set; }
        public float ResistPhysical { get; set; }
        public float ResistPoison { get; set; }
        public float DamageReductionPhysicalPercent { get; set; }
        public float SkillDamagePercentBonus { get; set; }
        public float ArcaneSkillDamagePercentBonus { get; set; }
        public float ColdSkillDamagePercentBonus { get; set; }
        public float FireSkillDamagePercentBonus { get; set; }
        public float HolySkillDamagePercentBonus { get; set; }
        public float LightningSkillDamagePercentBonus { get; set; }
        public float PosionSkillDamagePercentBonus { get; set; }
        public float MinDamage { get; set; }
        public float MaxDamage { get; set; }
        public string BaseType { get; set; }
        public string ItemType { get; set; }

        public override string ToString()
        {
            return ""
                + "HatredRegen: " + HatredRegen + ", "
                + "MaxDiscipline: " + MaxDiscipline + ", "
                + "MaxArcanePower: " + MaxArcanePower + ", "
                + "MaxMana: " + MaxMana + ", "
                + "MaxFury: " + MaxFury + ", "
                + "MaxSpirit: " + MaxSpirit + ", "
                + "ManaRegen: " + ManaRegen + ", "
                + "SpiritRegen: " + SpiritRegen + ", "
                + "ArcaneOnCrit: " + ArcaneOnCrit + ", "
                + "HealthPerSpiritSpent: " + HealthPerSpiritSpent + ", "
                + "AttackSpeedPercent: " + AttackSpeedPercent + ", "
                + "AttackSpeedPercentBonus: " + AttackSpeedPercentBonus + ", "
                + "Quality: " + Quality + ", "
                + "Level: " + Level + ", "
                + "ItemLevelRequirementReduction: " + ItemLevelRequirementReduction + ", "
                + "RequiredLevel: " + RequiredLevel + ", "
                + "CritPercent: " + CritPercent + ", "
                + "CritDamagePercent: " + CritDamagePercent + ", "
                + "BlockChance: " + BlockChance + ", "
                + "BlockChanceBonus: " + BlockChanceBonus + ", "
                + "HighestPrimaryAttribute: " + HighestPrimaryAttribute + ", "
                + "Intelligence: " + Intelligence + ", "
                + "Vitality: " + Vitality + ", "
                + "Strength: " + Strength + ", "
                + "Dexterity: " + Dexterity + ", "
                + "Armor: " + Armor + ", "
                + "ArmorBonus: " + ArmorBonus + ", "
                + "ArmorTotal: " + ArmorTotal + ", "
                + "Sockets: " + Sockets + ", "
                + "LifeSteal: " + LifeSteal + ", "
                + "LifeOnHit: " + LifeOnHit + ", "
                + "LifeOnKill: " + LifeOnKill + ", "
                + "MagicFind: " + MagicFind + ", "
                + "GoldFind: " + GoldFind + ", "
                + "ExperienceBonus: " + ExperienceBonus + ", "
                + "WeaponOnHitSlowProcChance: " + WeaponOnHitSlowProcChance + ", "
                + "WeaponOnHitBlindProcChance: " + WeaponOnHitBlindProcChance + ", "
                + "WeaponOnHitChillProcChance: " + WeaponOnHitChillProcChance + ", "
                + "WeaponOnHitFearProcChance: " + WeaponOnHitFearProcChance + ", "
                + "WeaponOnHitFreezeProcChance: " + WeaponOnHitFreezeProcChance + ", "
                + "WeaponOnHitImmobilizeProcChance: " + WeaponOnHitImmobilizeProcChance + ", "
                + "WeaponOnHitKnockbackProcChance: " + WeaponOnHitKnockbackProcChance + ", "
                + "WeaponOnHitBleedProcChance: " + WeaponOnHitBleedProcChance + ", "
                + "WeaponDamagePercent: " + WeaponDamagePercent + ", "
                + "WeaponAttacksPerSecond: " + WeaponAttacksPerSecond + ", "
                + "WeaponMinDamage: " + WeaponMinDamage + ", "
                + "WeaponMaxDamage: " + WeaponMaxDamage + ", "
                + "WeaponDamagePerSecond: " + WeaponDamagePerSecond + ", "
                + "WeaponDamageType: " + WeaponDamageType + ", "
                + "MaxDamageElemental: " + MaxDamageElemental + ", "
                + "MinDamageElemental: " + MinDamageElemental + ", "
                + "MinDamageFire: " + MinDamageFire + ", "
                + "MaxDamageFire: " + MaxDamageFire + ", "
                + "MinDamageLightning: " + MinDamageLightning + ", "
                + "MaxDamageLightning: " + MaxDamageLightning + ", "
                + "MinDamageCold: " + MinDamageCold + ", "
                + "MaxDamageCold: " + MaxDamageCold + ", "
                + "MinDamagePoison: " + MinDamagePoison + ", "
                + "MaxDamagePoison: " + MaxDamagePoison + ", "
                + "MinDamageArcane: " + MinDamageArcane + ", "
                + "MaxDamageArcane: " + MaxDamageArcane + ", "
                + "MinDamageHoly: " + MinDamageHoly + ", "
                + "MaxDamageHoly: " + MaxDamageHoly + ", "
                + "OnHitAreaDamageProcChance: " + OnHitAreaDamageProcChance + ", "
                + "PowerCooldownReductionPercent: " + PowerCooldownReductionPercent + ", "
                + "ResourceCostReductionPercent: " + ResourceCostReductionPercent + ", "
                + "PickUpRadius: " + PickUpRadius + ", "
                + "MovementSpeed: " + MovementSpeed + ", "
                + "HealthGlobeBonus: " + HealthGlobeBonus + ", "
                + "HealthPerSecond: " + HealthPerSecond + ", "
                + "LifePercent: " + LifePercent + ", "
                + "DamagePercentBonusVsElites: " + DamagePercentBonusVsElites + ", "
                + "DamagePercentReductionFromElites: " + DamagePercentReductionFromElites + ", "
                + "Thorns: " + Thorns + ", "
                + "ResistAll: " + ResistAll + ", "
                + "ResistArcane: " + ResistArcane + ", "
                + "ResistCold: " + ResistCold + ", "
                + "ResistFire: " + ResistFire + ", "
                + "ResistHoly: " + ResistHoly + ", "
                + "ResistLightning: " + ResistLightning + ", "
                + "ResistPhysical: " + ResistPhysical + ", "
                + "ResistPoison: " + ResistPoison + ", "
                + "DamageReductionPhysicalPercent: " + DamageReductionPhysicalPercent + ", "
                + "SkillDamagePercentBonus: " + SkillDamagePercentBonus + ", "
                + "ArcaneSkillDamagePercentBonus: " + ArcaneSkillDamagePercentBonus + ", "
                + "ColdSkillDamagePercentBonus: " + ColdSkillDamagePercentBonus + ", "
                + "FireSkillDamagePercentBonus: " + FireSkillDamagePercentBonus + ", "
                + "HolySkillDamagePercentBonus: " + HolySkillDamagePercentBonus + ", "
                + "LightningSkillDamagePercentBonus: " + LightningSkillDamagePercentBonus + ", "
                + "PosionSkillDamagePercentBonus: " + PosionSkillDamagePercentBonus + ", "
                + "MinDamage: " + MinDamage + ", "
                + "MaxDamage: " + MaxDamage + ", "
                + "BaseType: " + BaseType + ", "
                + "ItemType: " + ItemType + ", ";
        }
    }
}
