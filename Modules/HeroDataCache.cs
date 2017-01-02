using System.Collections.Generic;
using Trinity.Framework.Objects;
using Zeta.Game;

namespace Trinity.Modules
{
    public enum HeroDataSnoId
    {
        None = 0,
        DemonHunter = -930376119,
        Barbarian = 1337532130,
        Wizard = 491159985,
        WitchDoctor = 54772266,
        Monk = 4041749,
        Crusader = -1104684007,
    }

    public class HeroDataCache : Module
    {
        public class HeroData
        {
            public int Strength { get; set; }
            public int Dexterity { get; set; }
            public int Intelligence { get; set; }
            public int Vitality { get; set; }
            public AttributeType PrimaryAttribute { get; set; }
            public ResourceType SecondaryResourceType { get; set; }
            public ResourceType PrimaryResourceType { get; set; }
            public List<int> ActorSnoIds { get; set; }
            public string Name { get; set; }
            public int SkillKitSno { get; set; }
            public float DamageReduction { get; set; }
            public int PrimaryResourceMax { get; set; }
            public int PrimaryResourceRegen { get; set; }
            public float BaseCritChance { get; set; }
            public float BaseCritDamage { get; set; }
            public float MaxCritChance { get; set; }
            public int DefaultPowerSnoId { get; set; }
            public int TreasureClassSnoId { get; set; }
            public int GameBalanceId { get; set; }
            public ActorClass ActorClass { get; set; }
            public override string ToString() => $"{GetType().Name}: {Name}";
        }

        public ActorClass GetActorClass(int gameBalanceId)
        {
            switch ((HeroDataSnoId)gameBalanceId)
            {
                case HeroDataSnoId.DemonHunter: return ActorClass.DemonHunter;
                case HeroDataSnoId.Barbarian: return ActorClass.Barbarian;
                case HeroDataSnoId.Wizard: return ActorClass.Wizard;
                case HeroDataSnoId.WitchDoctor: return ActorClass.Witchdoctor;
                case HeroDataSnoId.Monk: return ActorClass.Monk;
                case HeroDataSnoId.Crusader: return ActorClass.Crusader;
            }
            return ActorClass.Invalid;
        }

        private readonly Dictionary<ActorClass, HeroData> _entries = new Dictionary<ActorClass, HeroData>();

        public HeroData Monk => _entries[ActorClass.Monk];
        public HeroData Barbarian => _entries[ActorClass.Barbarian];
        public HeroData DemonHunter => _entries[ActorClass.DemonHunter];
        public HeroData Witchdoctor => _entries[ActorClass.Witchdoctor];
        public HeroData Crusader => _entries[ActorClass.Crusader];
        public HeroData Wizard => _entries[ActorClass.Wizard];

        public HeroDataCache()
        {
            //foreach (var native in Core.MemoryModel.GameBalanceHelper.GetRecords<NativeHeroData>(SnoGameBalanceType.Heroes))
            //{
            //    var actorClass = GetActorClass(native._2_0x100_int);
            //    var heroData = new HeroData
            //    {
            //        Name = native._1_0x0_String,
            //        ActorSnoIds = new List<int>
            //        {
            //            native._4_0x108_Actor_Sno,
            //            native._5_0x10C_Actor_Sno
            //        },
            //        SkillKitSno = native._10_0x120_SkillKit_Sno,
            //        PrimaryResourceType = (ResourceType)native._15_0x134_Enum,
            //        SecondaryResourceType = (ResourceType)native._15_0x134_Enum,
            //        PrimaryAttribute = (AttributeType)native._16_0x138_Enum,
            //        PrimaryResourceMax = (int)native._28_0x168_float,
            //        PrimaryResourceRegen = (int)native._30_0x170_float,
            //        DamageReduction = native._26_0x160_float,
            //        Vitality = (int)native._22_0x150_float,
            //        Intelligence = (int)native._21_0x14C_float,
            //        Dexterity = (int)native._20_0x148_float,
            //        Strength = (int)native._19_0x144_float,
            //        BaseCritChance = native._41_0x19C_float,
            //        BaseCritDamage = native._54_0x1D0_float,
            //        MaxCritChance = native._42_0x1A0_float,
            //        DefaultPowerSnoId = native._9_0x11C_Power_Sno,
            //        TreasureClassSnoId = native._6_0x110_TreasureClass_Sno,
            //        GameBalanceId = native._2_0x100_int,
            //        ActorClass = actorClass
            //    };
            //    _entries.Add(actorClass, heroData);
            //}
        }
    }

}


