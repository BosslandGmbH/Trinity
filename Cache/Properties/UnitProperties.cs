using System;
using System.Collections.Generic;
using Trinity.Framework;
using Trinity.Framework.Objects.Memory.Attributes;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// Properties that are specific to monsters
    /// This class should update all values that are possible/reasonable/useful.
    /// DO NOT put settings or situational based exclusions in here, do that in weighting etc.
    /// </summary>
    public class UnitProperties : IPropertyCollection
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(100);

        public bool IsValid { get; set; } = true;

        public DateTime CreationTime { get; } = DateTime.UtcNow;

        public void ApplyTo(TrinityCacheObject target)
        {
            if (!target.IsFrozen && DateTime.UtcNow.Subtract(_lastUpdated) > UpdateInterval)
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
            target.IsTreasureGoblin = this.IsTreasureGoblin;
            target.IsAlly = this.IsAlly;
            target.IsIllusion = this.IsIllusion;
            target.IsNPC = this.IsNPC;
            target.IsSameTeam = this.IsSameTeam;
            target.IsQuestGiver = this.IsQuestGiver;
            target.IsUntargetable = this.IsUntargetable;
            target.IsInvulnerable = this.IsInvulnerable;
            target.IsElite = this.IsElite;
            target.IsTrashMob = this.IsTrashMob;
            target.IsUnique = this.IsUnique;
            target.IsMinion = this.IsMinion;
            target.IsRare = this.IsRare;
            target.IsChampion = this.IsChampion;
            target.IsSummoner = this.IsSummoner;
            target.SummonedByACDId = this.SummonedByACDId;
            target.SummonerId = this.SummonerId;
            target.IsSummonedByPlayer = this.IsSummonedByPlayer;
            target.IsSummoned = this.IsSummoned;
            target.IsReflectingDamage = this.IsReflectingDamage;
            target.IsDead = this.IsDead;
            target.IsHidden = this.IsHidden;
            target.IsSpawningBoss = this.IsSpawningBoss;
        }

        public void OnCreate(TrinityCacheObject source)
        {
            if (source.ActorType != ActorType.Monster || !source.IsValid)
                return;

            var unit = source.Unit;
            if (unit == null || !unit.IsValid)
                return;

            var commonData = source.CommonData;
            if (commonData == null || !commonData.IsValid)
                return;

            var monsterInfo = source.Unit.MonsterInfo;
            this.MonsterSize = monsterInfo.MonsterSize;
            this.MonsterRace = monsterInfo.MonsterRace;
            this.MonsterType = monsterInfo.MonsterType;

            this.MonsterAffixes = MonsterPropertyUtils.GetMonsterAffixes(commonData.Affixes).Flags;
            this.MonsterQuality = commonData.MonsterQualityLevel;
            this.IsBoss = this.MonsterQuality == MonsterQuality.Boss;
            this.IsHostile = unit.IsHostile;

            this.RiftValuePct = RiftProgression.GetRiftValue(source);
            this.IsTreasureGoblin = MonsterRace == MonsterRace.TreasureGoblin || source.InternalNameLowerCase.Contains("goblin");
            this.IsIllusion = this.MonsterAffixes.HasFlag(MonsterAffixes.Illusionist) && source.ActorAttributes.IsIllusion;            
            this.IsRare = this.MonsterAffixes.HasFlag(MonsterAffixes.Rare) || MonsterQuality == MonsterQuality.Rare;
            this.IsChampion = MonsterQuality == MonsterQuality.Champion;
            this.IsUnique = this.MonsterAffixes.HasFlag(MonsterAffixes.Unique) || MonsterQuality == MonsterQuality.Unique;
            this.IsMinion = this.MonsterAffixes.HasFlag(MonsterAffixes.Minion) || MonsterQuality == MonsterQuality.Minion;
            this.IsElite = IsMinion || IsRare || IsChampion || IsUnique || IsBoss;
            this.IsTrashMob = source.IsUnit && !(IsElite || IsBoss || IsTreasureGoblin || IsMinion);

            if (source.ActorSNO == 86624) // Jondar is not an ally.
                this.MonsterType = MonsterType.Undead;

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

            Update(source);
        }

        public void Update(TrinityCacheObject source)
        {
            _lastUpdated = DateTime.UtcNow;            

            if (!source.IsValid || source.ActorType != ActorType.Monster && source.ActorType != ActorType.Player)
                return;

            var unit = source.Unit;
            if (unit == null || !unit.IsValid)
            {
                this.IsDead = true;
                return;
            }

            this.IsDead = MonsterPropertyUtils.IsDead(source);
            this.HitPoints = unit.HitpointsCurrent;
            this.HitPointsMax = unit.HitpointsMax;
            this.HitPointsPct = this.HitPoints / this.HitPointsMax;
            this.HasDotDps = source.ActorAttributes.HasDotDps;
            this.IsReflectingDamage = this.MonsterAffixes.HasFlag(MonsterAffixes.ReflectsDamage) && source.ActorAttributes.IsReflecting;
            this.IsNPC = unit.IsNPC;
            this.IsQuestGiver = unit.IsQuestGiver;
            this.NPCIsOperatable = source.ActorAttributes.NPCIsOperatable;
            this.IsUntargetable = source.ActorAttributes.IsUntargetable && !DataDictionary.IgnoreUntargettableAttribute.Contains(source.ActorSNO);
            this.IsInvulnerable = source.ActorAttributes.IsInvulnerable;
            this.TeamId = unit.TeamId;
            this.IsSameTeam = this.TeamId == 1 || this.TeamId == 2 || this.TeamId == 17 || this.TeamId == TrinityPlugin.Player.TeamId || DataDictionary.AllyMonsterTypes.Contains(this.MonsterType);
            this.IsHidden = source.ActorAttributes.IsHidden || source.ActorAttributes.IsBurrowed;
            this.IsCurrentAvoidance = Core.Avoidance.ActiveAvoidanceSnoIds.Contains(source.ActorSNO);
            this.IsSpawningBoss = this.IsBoss && this.IsUntargetable;

            var movement = unit.Movement;
            if (movement != null && movement.IsValid)
            {
                this.Rotation = movement.Rotation;
                this.DirectionVector = movement.DirectionVector;
            }
        }

        public bool IsSpawningBoss { get; set; }
        public bool IsTrashMob { get; set; }
        public bool IsCurrentAvoidance { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDead { get; set; }
        public bool IsChampion { get; set; }
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
        public bool IsTreasureGoblin { get; set; }
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
        public MonsterType MonsterType { get; set; } = MonsterType.None;
        public MonsterRace MonsterRace { get; set; } = MonsterRace.Unknown;
        public MonsterSize MonsterSize { get; set; } = MonsterSize.Unknown;

    }

    public class MonsterPropertyUtils
    {
        public static bool IsDead(TrinityCacheObject monster)
        {            
            if (monster.ActorType == ActorType.Monster)
            {
                if (DataDictionary.FakeDeathMonsters.Contains(monster.ActorSNO))
                {
                    return false;
                }

                if (monster.AnimationState == AnimationState.Dead)
                {
                    return true;
                }

                if (monster.Unit.IsDead || monster.ActorAttributes.IsDead)
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

        public class FlagEnumParseResult
        {
            public MonsterAffixes Flags;
            public HashSet<MonsterAffixes> Collection;
        }

        public static FlagEnumParseResult GetMonsterAffixes(int[] affixIds)
        {
            // because DiaUnit.MonsterAffixes is very very slow
            var hash = new HashSet<MonsterAffixes>();
            var flags = MonsterAffixes.None;
            foreach (var affix in affixIds)
            {
                if (affix == -1) continue;

                switch (affix)
                {
                    case -1669589516:
                        hash.Add(MonsterAffixes.ArcaneEnchanted);
                        flags |= MonsterAffixes.ArcaneEnchanted;
                        continue;
                    case 1165197192:
                        hash.Add(MonsterAffixes.Avenger);
                        flags |= MonsterAffixes.Avenger;
                        continue;
                    case -121983956:
                        hash.Add(MonsterAffixes.Desecrator);
                        flags |= MonsterAffixes.Desecrator;
                        continue;
                    case -1752429632:
                        hash.Add(MonsterAffixes.Electrified);
                        flags |= MonsterAffixes.Electrified;
                        continue;
                    case -1512481702:
                        hash.Add(MonsterAffixes.ExtraHealth);
                        flags |= MonsterAffixes.ExtraHealth;
                        continue;
                    case 3775118:
                        hash.Add(MonsterAffixes.Fast);
                        flags |= MonsterAffixes.Fast;
                        continue;
                    case -439707236:
                        hash.Add(MonsterAffixes.FireChains);
                        flags |= MonsterAffixes.FireChains;
                        continue;
                    case -163836908:
                        hash.Add(MonsterAffixes.Frozen);
                        flags |= MonsterAffixes.Frozen;
                        continue;
                    case 1886876669:
                        hash.Add(MonsterAffixes.FrozenPulse);
                        flags |= MonsterAffixes.FrozenPulse;
                        continue;
                    case 1799201764:
                        hash.Add(MonsterAffixes.HealthLink);
                        flags |= MonsterAffixes.HealthLink;
                        continue;
                    case 127452338:
                        hash.Add(MonsterAffixes.Horde);
                        flags |= MonsterAffixes.Horde;
                        continue;
                    case 394214687:
                        hash.Add(MonsterAffixes.Illusionist);
                        flags |= MonsterAffixes.Illusionist;
                        continue;
                    case -27686857:
                        hash.Add(MonsterAffixes.Jailer);
                        flags |= MonsterAffixes.Jailer;
                        continue;
                    case -2088540441:
                        hash.Add(MonsterAffixes.Knockback);
                        flags |= MonsterAffixes.Knockback;
                        continue;
                    case -1412750743:
                        hash.Add(MonsterAffixes.MissileDampening);
                        flags |= MonsterAffixes.MissileDampening;
                        continue;
                    case 106438735:
                        hash.Add(MonsterAffixes.Molten);
                        flags |= MonsterAffixes.Molten;
                        continue;
                    case 106654229:
                        hash.Add(MonsterAffixes.Mortar);
                        flags |= MonsterAffixes.Mortar;
                        continue;
                    case -1245918914:
                        hash.Add(MonsterAffixes.Nightmarish);
                        flags |= MonsterAffixes.Nightmarish;
                        continue;
                    case 1905614711:
                        hash.Add(MonsterAffixes.Orbiter);
                        flags |= MonsterAffixes.Orbiter;
                        continue;
                    case -1333953694:
                        hash.Add(MonsterAffixes.Plagued);
                        flags |= MonsterAffixes.Plagued;
                        continue;
                    case 1929212066:
                        hash.Add(MonsterAffixes.PoisonEnchanted);
                        flags |= MonsterAffixes.PoisonEnchanted;
                        continue;
                    case -1374592233:
                        hash.Add(MonsterAffixes.ReflectsDamage);
                        flags |= MonsterAffixes.ReflectsDamage;
                        continue;
                    case -725865705:
                        hash.Add(MonsterAffixes.Shielding);
                        flags |= MonsterAffixes.Shielding;
                        continue;
                    case -507706394:
                        hash.Add(MonsterAffixes.Teleporter);
                        flags |= MonsterAffixes.Teleporter;
                        continue;
                    case -50556465:
                        hash.Add(MonsterAffixes.Thunderstorm);
                        flags |= MonsterAffixes.Thunderstorm;
                        continue;
                    case 395423867:
                        hash.Add(MonsterAffixes.Vampiric);
                        flags |= MonsterAffixes.Vampiric;
                        continue;
                    case 458872904:
                        hash.Add(MonsterAffixes.Vortex);
                        flags |= MonsterAffixes.Vortex;
                        continue;
                    case 481181063:
                        hash.Add(MonsterAffixes.Waller);
                        flags |= MonsterAffixes.Waller;
                        continue;
                    case 1156956365:
                        hash.Add(MonsterAffixes.Wormhole);
                        flags |= MonsterAffixes.Wormhole;
                        continue;
                    case 924743082: // Champion
                        hash.Add(MonsterAffixes.Elite);
                        flags |= MonsterAffixes.Elite;
                        continue;
                    case 4206314:
                        hash.Add(MonsterAffixes.Rare);
                        flags |= MonsterAffixes.Rare;
                        continue;
                    case 418225399:
                        hash.Add(MonsterAffixes.Unique);
                        flags |= MonsterAffixes.Unique;
                        continue;
                    case 99383434:
                        hash.Add(MonsterAffixes.Minion);
                        flags |= MonsterAffixes.Minion;
                        continue;
                    default:
                        Logger.LogVerbose($"Unknown AffixId={affix}");
                        break;
                }
            }

            return new FlagEnumParseResult
            {
                Collection = hash,
                Flags = flags
            };
        }


    }
}



