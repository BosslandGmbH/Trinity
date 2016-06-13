using System;
using System.Linq;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// PropertyLoader that are specific to monsters
    /// </summary>
    public class MonsterProperties : PropertyLoader.IPropertyCollection
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(100);

        public void ApplyTo(TrinityCacheObject target)
        {
            if (DateTime.UtcNow.Subtract(_lastUpdated).TotalMilliseconds > UpdateInterval.TotalMilliseconds && target.IsValid)
                Update(target);

            target.IsBoss = this.IsBoss;
            target.MonsterType = this.MonsterType;
            target.MonsterSize = this.MonsterSize;
            target.MonsterRace = this.MonsterRace;
            target.MonsterQuality = this.MonsterQuality;
            target.Rotation = this.Rotation;
            target.DirectionVector = this.DirectionVector;
            target.IsSummoned = this.IsSummoned;
            target.IsHostile = this.IsHostile;
            target.RiftValuePct = this.RiftValuePct;
            target.NPCIsOperable = this.NPCIsOperatable;
            target.HitPoints = this.HitPoints;
            target.HitPointsMax = this.HitPointsMax;
            target.HitPointsPct = this.HitPointsPct;
            target.HasDotDPS = this.HasDotDps;
            target.MonsterAffixes = this.MonsterAffixes;
            target.IsTreasureGoblin = this.IsGoblin;
            target.IsAlly = this.IsAlly;
            target.IsIllusion = this.IsIllusion;
            target.IsNPC = this.IsNPC;
            target.IsSameTeam = this.IsSameTeam;
            target.IsQuestGiver = this.IsQuestGiver;
            target.IsUntargetable = this.IsUntargetable;
            target.IsInvulnerable = this.IsInvulnerable;
            target.IsElite = this.IsElite;
            target.IsUnique = this.IsUnique;
            target.IsMinion = this.IsMinion;
            target.IsRare = this.IsRare;
            target.IsSummoner = this.IsSummoner;
            target.SummonedByACDId = this.SummonedByACDId;
            target.SummonerId = this.SummonerId;
            target.IsSummonedByPlayer = this.IsSummonedByPlayer;
            target.IsSummoned = this.IsSummoned;
            target.IsReflectingDamage = this.IsReflectingDamage;
        }

        public void OnCreate(TrinityCacheObject source)
        {
            if (source.ActorType != ActorType.Monster || !source.IsValid)
                return;

            var unit = source.Unit;
            if (unit == null || !unit.IsValid)
                return;

            var commonData = source.CommonData;
            var monsterInfo = source.Unit.MonsterInfo;
            this.MonsterSize = monsterInfo.MonsterSize;
            this.MonsterRace = monsterInfo.MonsterRace;
            this.MonsterType = monsterInfo.MonsterType;
            this.MonsterQuality = commonData.MonsterQualityLevel;
            this.IsBoss = this.MonsterQuality == MonsterQuality.Boss;
            this.MonsterAffixes = MonsterPropertyUtils.GetMonsterAffixes(source);
            this.IsHostile = unit.IsHostile;
            this.IsSummoned = commonData.SummonedByACDId > 0;
            this.RiftValuePct = RiftProgression.GetRiftValue(source);
            this.IsGoblin = MonsterRace == MonsterRace.TreasureGoblin || source.InternalNameLowerCase.Contains("goblin");
            this.IsIllusion = this.MonsterAffixes.HasFlag(MonsterAffixes.Illusionist) && source.ActorAttributes.IsIllusion;
            this.IsElite = this.MonsterAffixes.HasFlag(MonsterAffixes.Elite);
            this.IsRare = this.MonsterAffixes.HasFlag(MonsterAffixes.Rare);
            this.IsUnique = this.MonsterAffixes.HasFlag(MonsterAffixes.Unique);
            this.IsMinion = this.MonsterAffixes.HasFlag(MonsterAffixes.Minion);

            if (source.ActorSNO == 86624) // Jondar is not an ally.
                this.MonsterType = MonsterType.Undead;

            if (this.IsHostile && !this.IsSameTeam)
            {
                this.SummonedByACDId = unit.SummonedByACDId;
                this.IsSummoned = this.SummonedByACDId > 0;

                if (this.IsSummoned)
                {
                    this.IsSummonedByPlayer = this.SummonedByACDId == TrinityPlugin.Player.MyDynamicID;
                }
                else
                {
                    this.SummonerId = unit.SummonerId;
                    this.IsSummoner = this.SummonerId > 0;
                }
            }

        }

        public void Update(TrinityCacheObject source)
        {
            _lastUpdated = DateTime.UtcNow;

            if (source.ActorType != ActorType.Monster || !source.IsValid)
                return;

            var unit = source.Unit;
            if (unit == null || !unit.IsValid)
                return;

            this.HitPoints = unit.HitpointsCurrent;
            this.HitPointsMax = unit.HitpointsMax;
            this.HitPointsPct = this.HitPoints/this.HitPointsMax;
            this.HasDotDps = source.ActorAttributes.HasDotDps;
            this.IsReflectingDamage = this.MonsterAffixes.HasFlag(MonsterAffixes.ReflectsDamage) && source.ActorAttributes.IsReflecting;
            this.IsNPC = unit.IsNPC;
            this.IsQuestGiver = unit.IsQuestGiver;
            this.NPCIsOperatable = source.ActorAttributes.NPCIsOperatable;
            this.IsUntargetable = source.ActorAttributes.IsUntargetable && !DataDictionary.IgnoreUntargettableAttribute.Contains(source.ActorSNO);
            this.IsInvulnerable = source.ActorAttributes.IsInvulnerable;
            this.TeamId = source.TeamId;
            this.IsSameTeam = this.TeamId == 1 || this.TeamId == 2 || this.TeamId == 17 || this.TeamId == TrinityPlugin.Player.TeamId || DataDictionary.AllyMonsterTypes.Contains(this.MonsterType);

            var movement = unit.Movement;
            if (movement != null && movement.IsValid)
            {
                this.Rotation = movement.Rotation;
                this.DirectionVector = movement.DirectionVector;
            }
        }

        public bool IsReflectingDamage { get; set; }
        public bool IsSummonedByPlayer { get; set; }
        public bool IsSummoner { get; set; }
        public int SummonerId { get; set; }
        public int SummonedByACDId { get; set; }
        public bool IsMinion { get; set; }
        public bool IsUnique { get; set; }
        public bool IsRare { get; set; }
        public bool IsElite { get; set; }
        public bool IsInvulnerable { get; set; }
        public bool IsUntargetable { get; set; }
        public bool IsBurrowed { get; set; }
        public bool IsQuestGiver { get; set; }
        public bool IsSameTeam { get; set; }
        public int TeamId { get; set; }
        public bool IsNPC { get; set; }
        public bool IsIllusion { get; set; }
        public bool IsAlly { get; set; }
        public bool IsGoblin { get; set; }
        public MonsterAffixes MonsterAffixes { get; set; }
        public bool HasDotDps { get; set; }
        public float HitPointsMax { get; set; }
        public double HitPointsPct { get; set; }
        public double HitPoints { get; set; }
        public bool NPCIsOperatable { get; set; }
        public double RiftValuePct { get; set; }
        public bool IsSummoned { get; set; }
        public bool IsHostile { get; set; }
        public Vector2 DirectionVector { get; set; }
        public float Rotation { get; set; }
        public bool IsBoss { get; set; }
        public MonsterQuality MonsterQuality { get; set; }
        public MonsterType MonsterType { get; set; }
        public MonsterRace MonsterRace { get; set; }
        public MonsterSize MonsterSize { get; set; }

    }

    public class MonsterPropertyUtils
    {
        public bool IsDead(TrinityCacheObject monster)
        {
            if (monster.ActorType == ActorType.Monster)
            {
                if (monster.AnimationState == AnimationState.Dead)
                {
                    return true;
                }

                var lowerAnim = monster.AnimationNameLowerCase;
                if (lowerAnim != null && (lowerAnim.Contains("_dead") || (lowerAnim.Contains("_death") && !lowerAnim.Contains("deathmaiden") && !lowerAnim.Contains("death_orb"))))
                {
                    return true;
                }

                ////if (CurrentCacheObject.CommonData.GetAttribute<int>(ActorAttributeType.DeletedOnServer) > 0)
                //if (CurrentCacheObject.CommonData.DeletedOnServer > 0)                    
                //{
                //    c_IgnoreSubStep = "DeletedOnServer";
                //    return false;
                //}
            }
            return false;
        }

        public static MonsterAffixes GetMonsterAffixes(TrinityCacheObject source)
        {
            // because DiaUnit.MonsterAffixes is very very slow

            var result = MonsterAffixes.None;
            foreach (var affix in source.CommonData.Affixes)
            {
                if (affix < 0) continue;

                switch ((TrinityMonsterAffix)affix)
                {
                    case TrinityMonsterAffix.ArcaneEnchanted:
                        result |= MonsterAffixes.ArcaneEnchanted;
                        continue;
                    case TrinityMonsterAffix.Avenger:
                        result |= MonsterAffixes.Avenger;
                        continue;
                    case TrinityMonsterAffix.Desecrator:
                        result |= MonsterAffixes.Desecrator;
                        continue;
                    case TrinityMonsterAffix.Electrified:
                        result |= MonsterAffixes.Electrified;
                        continue;
                    case TrinityMonsterAffix.ExtraHealth:
                        result |= MonsterAffixes.ExtraHealth;
                        continue;
                    case TrinityMonsterAffix.Fast:
                        result |= MonsterAffixes.Fast;
                        continue;
                    case TrinityMonsterAffix.FireChains:
                        result |= MonsterAffixes.FireChains;
                        continue;
                    case TrinityMonsterAffix.Frozen:
                        result |= MonsterAffixes.Frozen;
                        continue;
                    case TrinityMonsterAffix.FrozenPulse:
                        result |= MonsterAffixes.FrozenPulse;
                        continue;
                    case TrinityMonsterAffix.HealthLink:
                        result |= MonsterAffixes.HealthLink;
                        continue;
                    case TrinityMonsterAffix.Horde:
                        result |= MonsterAffixes.Horde;
                        continue;
                    case TrinityMonsterAffix.Illusionist:
                        result |= MonsterAffixes.Illusionist;
                        continue;
                    case TrinityMonsterAffix.Jailer:
                        result |= MonsterAffixes.Jailer;
                        continue;
                    case TrinityMonsterAffix.Knockback:
                        result |= MonsterAffixes.Knockback;
                        continue;
                    case TrinityMonsterAffix.MissileDampening:
                        result |= MonsterAffixes.MissileDampening;
                        continue;
                    case TrinityMonsterAffix.Molten:
                        result |= MonsterAffixes.Molten;
                        continue;
                    case TrinityMonsterAffix.Mortar:
                        result |= MonsterAffixes.Mortar;
                        continue;
                    case TrinityMonsterAffix.Nightmarish:
                        result |= MonsterAffixes.Nightmarish;
                        continue;
                    case TrinityMonsterAffix.Orbiter:
                        result |= MonsterAffixes.Orbiter;
                        continue;
                    case TrinityMonsterAffix.Plagued:
                        result |= MonsterAffixes.Plagued;
                        continue;
                    case TrinityMonsterAffix.PoisonEnchanted:
                        result |= MonsterAffixes.PoisonEnchanted;
                        continue;
                    case TrinityMonsterAffix.ReflectsDamage:
                        result |= MonsterAffixes.ReflectsDamage;
                        continue;
                    case TrinityMonsterAffix.Shielding:
                        result |= MonsterAffixes.Shielding;
                        continue;
                    case TrinityMonsterAffix.Teleporter:
                        result |= MonsterAffixes.Teleporter;
                        continue;
                    case TrinityMonsterAffix.Thunderstorm:
                        result |= MonsterAffixes.Thunderstorm;
                        continue;
                    case TrinityMonsterAffix.Vampiric:
                        result |= MonsterAffixes.Vampiric;
                        continue;
                    case TrinityMonsterAffix.Vortex:
                        result |= MonsterAffixes.Vortex;
                        continue;
                    case TrinityMonsterAffix.Waller:
                        result |= MonsterAffixes.Waller;
                        continue;
                    case TrinityMonsterAffix.Wormhole:
                        result |= MonsterAffixes.Wormhole;
                        continue;
                    case TrinityMonsterAffix.Champion:
                    case TrinityMonsterAffix.Rare:
                        result |= MonsterAffixes.Rare;
                        continue;
                    case TrinityMonsterAffix.Unique:
                        result |= MonsterAffixes.Unique;
                        continue;
                    case TrinityMonsterAffix.Minion:
                        result |= MonsterAffixes.Minion;
                        continue;
                    case TrinityMonsterAffix.Elite:
                        result |= MonsterAffixes.Elite;
                        continue;
                    default:
                        var entry = source.CommonData.MonsterAffixEntries.FirstOrDefault(a => a.Gbid == affix);
                        Trinity.Technicals.Logger.LogNormal($"Unknown AffixId={affix} Name={entry.Name} Type={entry.AffixType} Element={entry.Resistance}");
                        break;
                }
            }
            return result;
        }


    }
}



