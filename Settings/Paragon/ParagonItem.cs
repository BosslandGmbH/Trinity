using System;
using Trinity.Framework.Helpers;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Settings.Paragon
{
    [DataContract(Namespace = "")]
    public class ParagonItem : NotifyBase
    {
        private int _limit;
        private bool _isLimited;

        public ParagonItem(TrinityParagonBonusType type)
        {
            TypeName = type.ToString();
            Type = type;
            DisplayName = GetDisplayName(TypeName);
            Category = GetCategory(Type);
            MaxLimit = GetPointLimit(Type);            
            Limit = MaxLimit;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            TrinityParagonBonusType type;
            if (Enum.TryParse(TypeName, out type))
            {
                Type = type;
                DisplayName = GetDisplayName(TypeName);
                Category = GetCategory(Type);
                MaxLimit = GetPointLimit(Type);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int Limit
        {
            get { return _limit; }
            set { SetField(ref _limit, value); }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool IsLimited
        {
            get { return _isLimited; }
            set { SetField(ref _isLimited, value); }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string TypeName { get; set; } // not storing Id in Case ParagonBonusType Enum changes.

        [IgnoreDataMember]
        public string DisplayName { get; set; }

        [IgnoreDataMember]
        public int MaxLimit { get; set; }

        [IgnoreDataMember]
        public TrinityParagonBonusType Type { get; set; }

        [IgnoreDataMember]
        public ParagonBonusType DynamicType
        {
            get { return GetDynamicType(Type); }
        }

        [IgnoreDataMember]
        public ParagonCategory Category { get; set; }

        public static int GetPointLimit(TrinityParagonBonusType type)
        {
            switch (type)
            {
                case TrinityParagonBonusType.PrimaryStat:
                case TrinityParagonBonusType.Vitality:
                    return 5000;
            }
            return 50;
        }

        public static ParagonCategory GetCategory(TrinityParagonBonusType type)
        {
            switch (type)
            {
                case TrinityParagonBonusType.MovementSpeed:
                case TrinityParagonBonusType.PrimaryStat:
                case TrinityParagonBonusType.Vitality:
                case TrinityParagonBonusType.Resource:
                    return ParagonCategory.PrimaryAttributes;

                case TrinityParagonBonusType.CriticalChance:
                case TrinityParagonBonusType.CooldownReduction:
                case TrinityParagonBonusType.CriticalDamage:
                case TrinityParagonBonusType.AttackSpeed:
                    return ParagonCategory.Offense;

                case TrinityParagonBonusType.Life:
                case TrinityParagonBonusType.Armor:
                case TrinityParagonBonusType.ResistAll:
                case TrinityParagonBonusType.LifeRegen:
                    return ParagonCategory.Defense;

                case TrinityParagonBonusType.AreaDamage:
                case TrinityParagonBonusType.ResourceCost:
                case TrinityParagonBonusType.GoldFind:
                case TrinityParagonBonusType.LifeOnHit:
                    return ParagonCategory.Defense;
            }

            return default(ParagonCategory);
        }

        public static ParagonBonusType GetDynamicType(TrinityParagonBonusType type)
        {
            switch (type)
            {
                case TrinityParagonBonusType.PrimaryStat:

                    switch (ZetaDia.Me.ActorClass)
                    {
                        case ActorClass.Crusader:
                            return ParagonBonusType.StrengthCrusader;
                        case ActorClass.Barbarian:
                            return ParagonBonusType.StrengthBarbarian;
                        case ActorClass.DemonHunter:
                            return ParagonBonusType.DexterityDemonHunter;
                        case ActorClass.Wizard:
                            return ParagonBonusType.IntelligenceWizard;
                        case ActorClass.Witchdoctor:
                            return ParagonBonusType.IntelligenceWitchdoctor;
                        case ActorClass.Monk:
                            return ParagonBonusType.DexterityMonk;
                    }

                    break;

                case TrinityParagonBonusType.Resource:

                    switch (ZetaDia.Me.ActorClass)
                    {
                        case ActorClass.Crusader:
                            return ParagonBonusType.ResourceMaxBonusFaith;
                        case ActorClass.Barbarian:
                            return ParagonBonusType.ResourceMaxBonusFury;
                        case ActorClass.DemonHunter:
                            return ParagonBonusType.ResourceMaxBonusHatred;
                        case ActorClass.Wizard:
                            return ParagonBonusType.ResourceMaxBonusArcanum;
                        case ActorClass.Witchdoctor:
                            return ParagonBonusType.ResourceMaxBonusMana;
                        case ActorClass.Monk:
                            return ParagonBonusType.ResourceMaxBonusSpirit;
                    }

                    break;

            }

            return (ParagonBonusType) ((int) type);
        }

        private static string GetDisplayName(string input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            return builder.ToString();
        }
    }
}