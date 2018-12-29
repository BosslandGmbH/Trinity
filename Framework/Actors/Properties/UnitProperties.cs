using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.Properties
{
    public class UnitProperties
    {
        public static void Populate(TrinityActor actor)
        {
            Update(actor);
        }

        public static void Update(TrinityActor actor)
        {
            if (!IsValidUnit(actor))
                return;

            UpdateDeath(actor);
        }

        private static bool IsValidUnit(TrinityActor actor)
        {
            if (actor.ActorType != ActorType.Monster && actor.ActorType != ActorType.Player)
                return false;

            if (!actor.IsAcdBased || !actor.IsAcdValid)
                return false;

            return true;
        }

        public static int GetTeamId(ACD acd)
        {
            if (acd == null) return -1;
            var overrideId = acd.TeamOverride;
            return overrideId == -1 ? acd.TeamId : overrideId;
        }

        private static bool IsHostile(int a1, int a2)
        {
            return a1 != a2 && a1 != 1 && a2 != 1
                && (a1 != 14 || (uint)(a2 - 10) > 3)
                && (a2 != 14 || (uint)(a1 - 10) > 3)
                && ((uint)(a1 - 15) > 7 || (uint)(a2 - 2) > 7)
                && ((uint)(a1 - 2) > 7 || (uint)(a2 - 15) > 7);
        }

        public static bool IsHostile(ACD acd, ACD againstActor)
        {
            return IsHostile(GetTeamId(acd), GetTeamId(againstActor));
        }

        private static void UpdateDeath(TrinityActor actor)
        {
            var isDead = GetIsDead(actor);
            if (isDead != actor.IsDead)
            {
                actor.IsDead = isDead;
                if (isDead && actor.IsUnit)
                {
                    actor.OnUnitDeath();
                }
            }
        }

        public static bool GetIsDead(TrinityActor monster)
        {
            if (monster.IsDead)
            {
                return true;
            }

            if (monster.ActorType == ActorType.Monster)
            {
                if (GameData.FakeDeathMonsters.Contains(monster.ActorSnoId))
                {
                    return false;
                }

                if (monster.AnimationState == AnimationState.Dead)
                {
                    return true;
                }

                var hpA = monster.Attributes.Hitpoints <= 0.01;
                var hpB = monster.CommonData.HitpointsCur <= 0.01;

                if (hpA)
                {
                    return true;
                }

                var lowerAnim = monster.AnimationNameLowerCase;
                if (lowerAnim != null)
                {
                    var mentionsDeath = lowerAnim.Contains("_dead") || lowerAnim.Contains("_death");
                    if (mentionsDeath)
                    {
                        if (lowerAnim.Contains("deathmaiden") || lowerAnim.Contains("death_orb") || lowerAnim.Contains("deathorb") || lowerAnim.Contains("raise_dead"))
                        {
                            return false;
                        }
                        return true;
                    }
                }

                if (monster.IsElite && !monster.IsBoss && monster.HitPointsPct < 0.25f)
                {
                    return monster.Attributes.IsDeletedOnServer; 
                        //monster.Attributes.GetAttributeDirectlyFromTable<bool>(ActorAttributeType.DeletedOnServer);
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
            public HashSet<MonsterAffixes> Collection = new HashSet<MonsterAffixes>();
        }

        public static FlagEnumParseResult GetMonsterAffixes(List<int> affixIds)
        {
            if (affixIds == null || !affixIds.Any())
                return new FlagEnumParseResult();

            //1747048841
            //[Trinity 2.42.156] Unknown AffixId=-1941835855
            //[Trinity 2.42.156] Unknown AffixId = -76087889

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
                    case -464468964:
                        hash.Add(MonsterAffixes.Juggernaut);
                        flags |= MonsterAffixes.Juggernaut;
                        continue;
                    default:
                        //Core.Logger.Verbose($"Unknown AffixId={affix}");
                        break;
                }
            }

            return new FlagEnumParseResult
            {
                Collection = hash,
                Flags = flags
            };
        }

        public static EliteTypes GetEliteType(TrinityActor cacheObject)
        {
            switch (cacheObject.MonsterQuality)
            {
                case MonsterQuality.Champion:
                    return EliteTypes.Champion;

                case MonsterQuality.Minion:
                    return EliteTypes.Minion;

                case MonsterQuality.Rare:
                    return EliteTypes.Rare;
            }
            return EliteTypes.None;
        }
    }
}
