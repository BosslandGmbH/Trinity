using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals;
using Trinity.Framework.Avoidance.Structures;
using Buddy.Coroutines;
using System.Threading;
using Trinity.DbProvider;
using Trinity.Components.Coroutines;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Grid;
using Trinity.Components.Adventurer;

namespace Trinity.Routines.Necromancer
{
    public sealed class NEC_RathmaCDRSimulacrum4 : NecroMancerBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Rathma CDR Simulacrm Routine v4";
        public string Description => "Necromancer Combat Routine" + System.Environment.NewLine
				+ "Translated by ReddeR";
        public string Author => "crazyjay1982";
        public string Version => "0.4.4d";
        public string Url => "https://www.thebuddyforum.com/threads/necromancer-rathma-cdr-simulacrm-routine-gr-100.408084/";
        public Build BuildRequirements => new Build
        {
            // Set requirements
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.BonesOfRathma, SetBonus.Third },
            },
            // Skills Requirement
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Necromancer.BoneSpikes, null },
                { Skills.Necromancer.Simulacrum, Runes.Necromancer.Reservoir},
                { Skills.Necromancer.SkeletalMage, null },
                { Skills.Necromancer.LandOfTheDead, Runes.Necromancer.FrozenLands },
                { Skills.Necromancer.Devour, null },
                { Skills.Necromancer.BoneArmor, null },
            },
            // Items Requirements
            Items = new List<Item>
            {
            }
        };

        #endregion

        public bool IsReallyBlocked => IsBlocked || IsStuck;
        // Core.BlockedCheck.BlockedTime.TotalMilliseconds
        // public override Func<bool> ShouldIgnoreKiting => () => Core.Buffs.HasInvulnerableShrine;
        public override Func<bool> ShouldIgnoreAvoidance => IgnoreAvoidanceCondition;
        public override Func<bool> ShouldIgnoreNonUnits => IgnoreNonUnitsCondition;
        private float OcculusRadius => 9f;

        private Vector3 bossPos = Vector3.Zero;
        private bool hasBoss = false;
        private float bossDistance = 0;
        public bool inMovementPower = false;

        private TrinityPower MySkeletalMage(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.SkeletalMage, 60f, target.AcdId);

        private TrinityPower MyDecrepify(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Decrepify, 60f, position);

        private TrinityPower MyFrailty(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Frailty, 60f, position);

        private TrinityPower MyLeech(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Leech, 60f, position);

        private double BuffCooldownOfBoneArmor => Core.Cooldowns.GetBuffCooldownElapsed(SNOPower.P6_Necro_BoneArmor).TotalMilliseconds;
        private bool IsPartyGame => ZetaDia.Service.Party.NumPartyMembers > 1;
        private bool IsBoneArmorFirst => Legendary.ScytheOfTheCycle.IsEquipped || Legendary.ScytheOfTheCycle.IsEquippedInCube;
        private bool IsObsidianTacked => Legendary.ObsidianRingOfTheZodiac.IsEquipped || Legendary.ObsidianRingOfTheZodiac.IsEquippedInCube;
        // ObsidianRingOfTheZodiac

        // ObjectCache可以保证所有的目标都是有效的
        private static List<TrinityActor> ObjectCache => Core.Targets.Entries;

        private bool IgnoreAvoidanceCondition()
        {
            if (Core.Buffs.HasInvulnerableShrine)
                return true;

            if (!Core.Rift.IsGreaterRift)
            {
                if (IgnoreAvoidanceInN)
                    return true;
            }
            else 
            {
                if (IgnoreAvoidanceInG)
                    return true;
            }

            return false;
        }

        // GlobalStatus Begin

        private int _g_mobsIn10 = 0;
        private bool _isvalid_g_mobsIn10 = false;
        private int _MobsIn10
        {
            get 
            {
                if (!_isvalid_g_mobsIn10)
                {       
                    _g_mobsIn10 = TargetUtil.NumMobsInRange(10f);
                    _isvalid_g_mobsIn10 = true;
                    // if (IsDebugMove)
                    //     Core.Logger.Warn($"获取全局状态，_MobsIn10={_g_mobsIn10}");
                }

                return _g_mobsIn10;
            }
        }

        private int _g_mobsIn20 = 0;
        private bool _isvalid_g_mobsIn20 = false;
        private int _MobsIn20
        {
            get 
            {
                if (!_isvalid_g_mobsIn20)
                {       
                    _g_mobsIn20 = TargetUtil.NumMobsInRange(20f);
                    _isvalid_g_mobsIn20 = true;
                    // if (IsDebugMove)
                    //     Core.Logger.Warn($"获取全局状态，_MobsIn20={_g_mobsIn20}");
                }

                return _g_mobsIn20;
            }
        }

        private int _g_mobsIn30 = 0;
        private bool _isvalid_g_mobsIn30 = false;
        private int _MobsIn30
        {
            get 
            {
                if (!_isvalid_g_mobsIn30)
                {       
                    _g_mobsIn30 = TargetUtil.NumMobsInRange(30f);
                    _isvalid_g_mobsIn30 = true;
                    // if (IsDebugMove)
                    //     Core.Logger.Warn($"获取全局状态，_MobsIn30={_g_mobsIn30}");
                }

                return _g_mobsIn30;
            }
        }

        private int _g_mobsIn50 = 0;
        private bool _isvalid_g_mobsIn50 = false;
        private int _MobsIn50
        {
            get 
            {
                if (!_isvalid_g_mobsIn50)
                {       
                    _g_mobsIn50 = TargetUtil.NumMobsInRange(50f);
                    _isvalid_g_mobsIn50 = true;
                }

                return _g_mobsIn50;
            }
        }

        private TrinityActor _g_bestTargetIn60 = null;
        private bool _isvalid_g_bestTargetIn60 = false;
        private TrinityActor _BestTargetIn60
        {
            get 
            {
                if (!_isvalid_g_bestTargetIn60)
                {       
                    _g_bestTargetIn60 = GetBestTarget(Player.Position, 60f, true);
                    _isvalid_g_bestTargetIn60 = true;
                }

                return _g_bestTargetIn60;
            }
        }

        private TrinityActor _g_bestTargetIn60_unsafe = null;
        private bool _isvalid_g_bestTargetIn60_unsafe = false;
        private TrinityActor _BestTargetIn60_unsafe
        {
            get 
            {
                if (!_isvalid_g_bestTargetIn60_unsafe)
                {       
                    _g_bestTargetIn60_unsafe = GetBestTarget(Player.Position, 60f, false);
                    _isvalid_g_bestTargetIn60_unsafe = true;
                }

                return _g_bestTargetIn60_unsafe;
            }
        }

        private TrinityActor _g_closestTargetIn60 = null;
        private bool _isvalid_g_closestTargetIn60 = false;
        private TrinityActor _ClosestTargetIn60
        {
            get 
            {
                if (!_isvalid_g_closestTargetIn60)
                {       
                    _g_closestTargetIn60 = ClosestTarget(Player.Position, 60f, true);
                    _isvalid_g_closestTargetIn60 = true;
                }

                return _g_closestTargetIn60;
            }
        }

        private TrinityActor _g_closestTargetIn60_unsafe = null;
        private bool _isvalid_g_closestTargetIn60_unsafe = false;
        private TrinityActor _ClosestTargetIn60Unsafe
        {
            get 
            {
                if (!_isvalid_g_closestTargetIn60_unsafe)
                {       
                    _g_closestTargetIn60_unsafe = ClosestTarget2(Player.Position, 60f, false);
                    _isvalid_g_closestTargetIn60_unsafe = true;
                }

                return _g_closestTargetIn60_unsafe;
            }
        }

        private TrinityActor _g_closestTarget_danger = null;
        private bool _isvalid_g_closestTarget_danger = false;
        private TrinityActor _ClosestTargetDanger
        {
            get 
            {
                if (!_isvalid_g_closestTarget_danger)
                {       
                    _g_closestTarget_danger = GetClosestUnitUnSafe();
                    _isvalid_g_closestTarget_danger = true;
                }

                return _g_closestTarget_danger;
            }
        }

        private TrinityActor _g_bestClusterIn60 = null;
        private bool _isvalid_g_bestClusterIn60 = false;
        private TrinityActor _BestClusterIn60
        {
            get 
            {
                if (!_isvalid_g_bestClusterIn60)
                {       
                    _g_bestClusterIn60 = BestClusterUnit(Player.Position, 60f, true);
                    _isvalid_g_bestClusterIn60 = true;
                }

                return _g_bestClusterIn60;
            }
        }

        private TrinityActor _g_bestClusterIn60_unsafe = null;
        private bool _isvalid_g_bestClusterIn60_unsafe = false;
        private TrinityActor _BestClusterIn60_unsafe
        {
            get 
            {
                if (!_isvalid_g_bestClusterIn60_unsafe)
                {       
                    _g_bestClusterIn60_unsafe = BestClusterUnit(Player.Position, 60f, true);
                    _isvalid_g_bestClusterIn60_unsafe = true;
                }

                return _g_bestClusterIn60_unsafe;
            }
        }

        private Vector3 _g_closestOcculusIn58 = Vector3.Zero;
        private bool _isvalid_g_closestOcculusIn58 = false;
        private Vector3 _ClosestOcculusIn58
        {
            get 
            {
                if (!_isvalid_g_closestOcculusIn58)
                {       
                    bool hasOcculus = GetOculusPosition(out _g_closestOcculusIn58, 58f, Player.Position);
                    _isvalid_g_closestOcculusIn58 = true;
                }

                return _g_closestOcculusIn58;
            }
        }

        private bool _g_anyElitesInSafeRange = false;
        private bool _isvalid_g_anyElitesInSafeRange = false;
        private bool _AnyElitesInSafeRange
        {
            get 
            {
                if (!_isvalid_g_anyElitesInSafeRange)
                {       
                    _g_anyElitesInSafeRange = AnyElitesInRangeOfPosition(Player.Position, BloodRushSafeRange);
                    _isvalid_g_anyElitesInSafeRange = true;
                }

                return _g_anyElitesInSafeRange;
            }
        }

        private int _g_magCount = 0;
        private bool _isvalid_g_magCount = false;
        private int _MagCount
        {
            get 
            {
                if (!_isvalid_g_magCount)
                {       
                    _g_magCount = Core.Actors.Actors.Count(a => a.ActorSnoId == SNOActor.p6_necro_skeletonMage_C);
                    _isvalid_g_magCount = true;
                }

                return _g_magCount;
            }
        }

        private int _g_corpseCount = 0;
        private bool _isvalid_g_corpseCount = false;
        private int _CorpseCount
        {
            get 
            {
                if (!_isvalid_g_corpseCount)
                {       
                    _g_corpseCount = TargetUtil.CorpseCount(60f);
                    _isvalid_g_corpseCount = true;
                }

                return _g_corpseCount;
            }
        }

        private TrinityPlayer _g_nearestParty = null;
        private bool _isvalid_g_nearestParty = false;
        private TrinityPlayer _NearestParty
        {
            get 
            {
                if (!_isvalid_g_nearestParty)
                {       
                    _g_nearestParty = MyLeader();
                    _isvalid_g_nearestParty = true;
                }

                return _g_nearestParty;
            }
        }

        private void ResetGlobalStatus()
        {
            _isvalid_g_mobsIn10 = false;
            _isvalid_g_mobsIn20 = false;
            _isvalid_g_mobsIn30 = false;
            _isvalid_g_mobsIn50 = false;
            _isvalid_g_bestTargetIn60 = false;
            _isvalid_g_bestTargetIn60_unsafe = false;
            _isvalid_g_closestTargetIn60 = false;
            _isvalid_g_closestTargetIn60_unsafe = false;
            _isvalid_g_closestTarget_danger = false;

            _isvalid_g_bestClusterIn60 = false;
            _isvalid_g_bestClusterIn60_unsafe = false;

            _isvalid_g_closestOcculusIn58 = false;

            _isvalid_g_anyElitesInSafeRange = false;

            _isvalid_g_magCount = false;
            _isvalid_g_corpseCount = false;
            _isvalid_g_nearestParty = false;
        }

        // GlobalStatus End

        private double lastCheckCondition = 0;
        private int mobsIn10 = 0;
        private int mobsIn50 = 0;

        private bool IgnoreNonUnitsCondition()
        {
            if (!Core.Rift.IsGreaterRift)
                return false;

            // if (Core.Rift.IsGreaterRift && Core.Rift.RiftComplete)
            //     return true;
            bool needCheckShrine = false;
            double currentTick = MyGetTickCount();
            if (currentTick - lastCheckCondition > 500)
            {
                lastCheckCondition = currentTick;
                mobsIn10 = TargetUtil.NumMobsInRange(10f);
                mobsIn50 = TargetUtil.NumMobsInRange(50f);
                needCheckShrine = true;
            }
            
            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse <= 10000 && mobsIn50 > 3)
            {
                // if (IsDebugAvoid)
                //     Core.Logger.Warn($"死地中，怪物多于3个，先杀怪");
                return true;
            }

            if (mobsIn10 > 0)
            {
                // if (IsDebugAvoid)
                //     Core.Logger.Warn($"有怪贴身，先杀怪");
                return true;
            }

            // 碰撞的概率非常低，强制吃塔
            if (needCheckShrine)
            {
                TrinityActor shrine = ClosestShrineInRange();
                if (shrine != null)
                {
                    if (ForceUseShrine)
                    {
                        shrine.Weight = 1000;
                    }
                    else 
                    {
                        if (shrine.Distance < 10f)
                        {
                            if (IsDebugAvoid)
                                Core.Logger.Warn($"10码内有塔而且很安全");
                            shrine.Weight = 1000;
                            return false;
                        }

                        // if (shrine.Distance < 25f && TargetUtil.NumMobsInRange(25f) < 2)
                        // {
                        //     if (IsDebugAvoid)
                        //         Core.Logger.Warn($"25码内有塔而且很安全");
                        //     return false;
                        // }

                        if (shrine.Distance < 35f && TargetUtil.NumMobsInRange(shrine.Distance) < 3)
                        {
                            if (IsDebugAvoid)
                                Core.Logger.Warn($"{shrine.Distance}码内有塔而且很安全");
                            shrine.Weight = 1000;
                            return false;
                        }
                    }
                }
            }

            if (mobsIn50 > ClusterSize) //6够吗？
            {
                // if (IsDebugAvoid)
                //     Core.Logger.Warn($"周围的怪多于{ClusterSize}个，先杀怪");
                return true;
            }

            // if (IsInOcculus())
            // {
            //     // if (IsDebugAvoid)
            //     //     Core.Logger.Warn($"在神目中，先杀怪");
            //     return true;
            // }



            return false;
        }

        private double MyGetTickCount()
        {
            TimeSpan current = DateTime.Now - (new DateTime(1970, 1, 1, 0, 0, 0));
            return current.TotalMilliseconds;
        }

        bool ShouldCurse(out TrinityPower power)
        {
            power = null;
            Vector3 position = Vector3.Zero;

            if (ShouldDecrepify(out position))
                power = MyDecrepify(position);

            if (ShouldFrailty(out position))
                power = MyFrailty(position);

            if (ShouldLeech(out position))
                power = MyLeech(position);

            return power != null;
        }

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power = null;
            TrinityActor target = null;
            Vector3 position = Vector3.Zero;
            bool byBoneArmor = false;

            // if (!IsInCombat)
            //     Core.Logger.Warn($"莫名其妙的GetOffensivePower");

            ResetGlobalStatus();

            FindBoss();
            CheckDangerMonster();

            // 保命技能最优先，使用诅咒触发但提特效
            if (ShouldCurse(out power))
                return power;

            if (IsBoneArmorFirst && ShouldBoneArmor())
                return BoneArmor();

            if (Player.CurrentHealthPct < HealthPctForFindHealthGlobe && AnyHealthGlobeInRange())
            {
                if (ShouldSiphonBlood(out target))
                    return SiphonBlood(target);
            }

            // 如果带了血步，优先踩神目，保证输出最大化
            // 另外，踩神目也是一种另类的闪避
            if (Skills.Necromancer.BloodRush.CanCast() && TryGoOcculus(out power))
                return power;

            // 保命技能，拥有最高的优先级
            if (TryRescureByBloodRush(out power))
                return power;

            // 关于技能优先级：
            // 双分的原则：
            // 1. 双分施放的时候必须先保证安全，否则放了双分容易死
            // 2. 如果安全，而且双分和死地都带了，则优先使用双分，保证大招的重叠率

            // 先于亡者领域放双分，让双大的重叠范围更大
            if (ShouldSimulacrum(out position))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用血魂双分，目标距离我{position.Distance(Player.Position)}");
                return Simulacrum(position);
            }

            // 大招！
            if (ShouldLandOfTheDead(out target))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用亡者领域");
                return LandOfTheDead();
            }
        
            // 如果踩了神目，或者使用了血步闪避，应该取消当前的躲闪
            if (TryAvoidToSafePot(out power))
                return power;

            // 大招中，只要安全就不停的放吞噬
            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000 && 
                Player.CurrentHealthPct > HealthPctForFindHealthGlobe &&
                !Core.Avoidance.InAvoidance(Player.Position))
            {
                if (ShouldDevour())
                {
                    if (IsDebugSkill)
                        Core.Logger.Warn($"使用吞噬");
                    return Devour();
                }
            }
            
            if (ShouldMoveInCombat(out power))
                return power;

            // 如果血步cd没好，稍微靠后一点
            if (!Skills.Necromancer.BloodRush.CanCast() && TryGoOcculus(out power))
                return power;

            if (ShouldCloseToTarget(out byBoneArmor))
            {
                TryCloseToTarget(out power, false, byBoneArmor);
                    return power;
            }

            // 优先防御性技能，开骨甲
            if (!IsBoneArmorFirst && ShouldBoneArmor())
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用骨甲");
                return BoneArmor();
            }

            // 号令骷髅
            if (ShouldCommandSkeletons(out target))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用号令骷髅，目标距离我{target.Distance}");
                return CommandSkeletons(target);
            }

            // 亡者大军，我自己也没用过，随便放放
            if (ShouldArmyOfTheDead(out target))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用亡者大军，目标距离我{target.Distance}");
                return ArmyOfTheDead(target);
            }

            // 施放骷髅法师
            if (ShouldSkeletalMage(out power))
            {
                // if (IsDebugSkill)
                //     Core.Logger.Warn($"使用骷髅法师，目标距离我{target.Distance}");
                return power;
            }

            if (ShouldBoneSpear(out target))
                return BoneSpear(target);

            // 尽情享受大招的畅快吧！优先级应该要大于吞噬，以防止大招中只吞噬不贴身
            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse <= 10000 && _ClosestTargetIn60 != null && _ClosestTargetIn60.Distance > CastDistanceWhenLandOfDeath)
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"在大招中，向怪靠近！");
                return Walk(_ClosestTargetIn60.Position, _ClosestTargetIn60.Distance - CastDistanceWhenLandOfDeath); 
            }

            // 攻击中需要使用吞噬吗？？？？回能速度比不上骨刺和血球
            if (ShouldDevour())
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用吞噬");
                return Devour();
            }

            // 捡血球
            if (ShouldMoveInCombat(out power))
                return power;

            // 靠近怪
            if (TryCloseToTarget(out power))
                return power;

            // 骨刺是范围控制法术，应该攻击最密集的怪群
            if (ShouldBoneSpikes(out target))
                return BoneSpikes(target);

            // 使用鲜血虹吸
            if (ShouldSiphonBlood(out target))
                return SiphonBlood(target);

            if (ShouldGrimScythe(out target))
                return GrimScythe(target);

            if (IsDebugSkill || IsDebugMove)
                Core.Logger.Warn($"尝试攻击失败");
            
            return null;
        }

        public TrinityPower GetDefensivePower()
        {
            // Core.Logger.Warn($"尝试防御");
            if (IsPartyGame)
                return GetBuffPower();

            return null;
        }

        public TrinityPower GetBuffPower()
        {
            TrinityActor target;
            TrinityPower power;
            Vector3 position = Vector3.Zero;

            TryChangeSkills();

            // Core.Logger.Warn($"绝命效忠buff = {Core.Buffs.HasBuff(SNOPower.P6_Necro_Passive_CheatDeath)}");
            
            if (Player.IsInTown)
                return null;

            if (Core.Rift.RiftComplete && !Core.Rift.IsNephalemRift)
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"大米结束，不再使用技能");
                return null;
            }

            ResetGlobalStatus();
            
            if (IsInCombat || (CastMageAnytime && Core.Rift.IsGreaterRift) ||
                (CastMageAnytimeN && Core.Rift.IsNephalemRift))
            {
                if (ShouldCurse(out power))
                    return power;

                if (ShouldBoneArmor())
                {
                    if (IsDebugSkill)
                        Core.Logger.Warn($"使用骨甲");
                    return BoneArmor();
                }
                if (ShouldCommandSkeletons(out target))
                    return CommandSkeletons(target);

                if (ShouldSkeletalMage(out power))
                    return power;

                if (ShouldBoneSpear(out target))
                    return BoneSpear(target);
            }

            // if (TryRescureByBloodRush(out power))
            //     return power;

            if (ShouldDevour())
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"在GetBuffPower中使用吞噬");
                return Devour();
            }

            return null;
        }

        public TrinityPower GetDestructiblePower()
        {
            if (CurrentTarget == null)
                return null;

            if (CurrentTarget.IsCorruptGrowth && Skills.Necromancer.SkeletalMage.CanCast())
                return MySkeletalMage(CurrentTarget);

            if (Skills.Necromancer.BoneSpikes.CanCast())
                return BoneSpikes(CurrentTarget);

            if (Skills.Necromancer.DeathNova.CanCast())
                return DeathNova(CurrentTarget);

            if (Skills.Necromancer.SkeletalMage.CanCast())
                return MySkeletalMage(CurrentTarget);

            return DefaultPower;
        }

        TrinityPower ToDestInPath(Vector3 dest)
        {
            TrinityPower power;

            float safeDist = GetCastDistance()<BloodRushSafeRange?GetCastDistance():BloodRushSafeRange;
            if (!Core.Rift.IsGreaterRift || !TargetUtil.AnyMobsInRangeOfPosition(dest, safeDist))
            {
                if (TryBloodrushMovement(dest, out power))
                    return power;
            }

            return ToDestByWalk(dest);
        }

        TrinityPower ToDestAny(Vector3 dest)
        {
            var distance = dest.Distance(Player.Position);
            if (CanBloodRush() && (distance > 15f || (IsBlocked && distance > 5f)))
            {
                float safeDist = GetCastDistance()<BloodRushSafeRange?GetCastDistance():BloodRushSafeRange;
                if (!Core.Rift.IsGreaterRift || !TargetUtil.AnyMobsInRangeOfPosition(dest, safeDist))
                {
                    return BloodRush(dest);
                }
            }

            return ToDestByWalk(dest);
        }

        TrinityPower ToDestByWalk(Vector3 dest)
        {
            if (dest.Distance(Player.Position) > 10f && Core.Rift.IsGreaterRift)
                return Walk(dest, 10f);
            else
                return Walk(dest);
        }

        bool ShouldStand()
        {
            if (!IsInCombat)
                return false; 
            
            if (!IsSafePosition(Player.Position))
                return false;
            
            Vector3 occulusPos = _ClosestOcculusIn58;

            if (occulusPos == Vector3.Zero)
                return false;

            // 这里故意使用10码而不是OcculusRadius，也是对反复进出神目的一个保护
            if (occulusPos.Distance(Player.Position) > 10f)
                return false;

            if (Skills.Necromancer.GrimScythe.IsActive)
            {
                if (!IsValidTarget(ClosestTarget2(occulusPos, 15f)))
                {
                    return false;
                }
            }
            else 
            {
                if (IsBoneArmorBuffFirst())
                    return false;

                if (!IsValidTarget(_ClosestTargetIn60Unsafe))
                {
                    return false;
                }
            }

            if (IsDebugMove)
                Core.Logger.Warn($"在神目中，不再强制移动");
            return true;
        }

        static double _lastDevourTickInMove = 0;
        public TrinityPower GetMovementPower(Vector3 destination)
        {
            TrinityPower power = null;
            Vector3 position;
            bool cannotOffensive = false;

            if (Player.IsInTown)
                return Walk(destination);

            if (Player.IsGhosted)
            {
                CheckDeath();

                // 向队友靠拢？？一坨屎一样
                // if (IsPartyGame)
                //     return Walk(destination);

                destination = SafePotForAvoid();
                if (destination == Vector3.Zero)
                    Core.Avoidance.Avoider.TryGetSafeSpot(out destination);
                
                if (IsDebugMove)
                    Core.Logger.Warn($"复活了，向安全点前进，距离我{destination.Distance(Player.Position)}");
                return Walk(destination);
            }

            ResetGlobalStatus();

            if (Core.Rift.RiftComplete && !Core.Rift.IsNephalemRift)
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"大米结束，不再使用技能");
                return ToDestInPath(destination);
            }

            bool chasingMode = IsPartyGame && PartyLeader != ZetaDia.Me && (_NearestParty==null || _NearestParty.Position.Distance(Player.Position) > 60f);

            if (chasingMode)
            {
                // Core.Logger.Warn($"离队友太远了，全力追赶");
                // 这里不能使用ToDestInPath，因为追赶的时候，插件的路线和T是不一样的
                return ToDestAny(destination);
            }

            // 这里不能扩大到所有的闪避，否则会出现在大规模地板技能之间发呆的问题
            if (Core.Avoidance.InCriticalAvoidance(Player.Position))
            {
                return ToDestInPath(destination);
            }

            if (IsInCombat || (CastMageAnytime && Core.Rift.IsGreaterRift) ||
                (CastMageAnytimeN && Core.Rift.IsNephalemRift))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"在buffpower中，战斗模式下使用技能");

                if (ShouldCurse(out power))
                    return power;

                if (ShouldBoneArmor())
                {
                    if (IsDebugSkill)
                        Core.Logger.Warn($"在buffpoweer中使用骨甲");
                    return BoneArmor();
                }

                if (ShouldSimulacrum(out position))
                {
                    if (IsDebugSkill)
                        Core.Logger.Warn($"在buffpower中使用双分");
                    return Simulacrum(position);
                }

                TrinityActor target = null;
                if (ShouldCommandSkeletons(out target))
                    return CommandSkeletons(target);

                if (ShouldSkeletalMage(out power))
                    return power;

                if (ShouldBoneSpear(out target))
                    return BoneSpear(target);
            }

            if (
                IsInCombat && AnyMobsInRange(CastDistanceWhenLandOfDeath) &&
                Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000 &&
                Player.CurrentHealthPct > 0.99)
            {
                if (ShouldDevour())
                {
                    if (IsDebugSkill)
                        Core.Logger.Warn($"在大招期间，移动中使用吞噬");
                    return Devour();
                }
            }

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000 && MyGetTickCount() - _lastDevourTickInMove > 2000)
            {
                _lastDevourTickInMove = MyGetTickCount();
                
                if (ShouldDevour())
                {
                    if (IsDebugSkill)
                        Core.Logger.Warn($"移动中使用吞噬");
                    return Devour();
                }
            }

            if (TryGoOcculus(out power))
                return power;

            if (TryAvoidToSafePot(out power))
                return power;

            if (!cannotOffensive && ShouldStand())
            {
                power = GetOffensivePower();
                if (power != null)
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"在神目中，强制攻击");
                    return power;
                }
                else 
                {
                    cannotOffensive = true;
                    if (IsDebugMove)
                        Core.Logger.Warn($"在神目中，但是无法攻击，尝试移动");
                }
            }

            if (ShouldMoveInPath(out power, destination))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"向血球移动");
                return power;
            }

            if (IsPartyGame && !cannotOffensive && StillMode && Core.Rift.IsGreaterRift && _MobsIn50 > 0)
            {
                if (_NearestParty != null && _NearestParty.Position.Distance(Player.Position) < 50f)
                {
                    power = GetOffensivePower();
                    if (power != null)
                    {
                        if (IsDebugMove)
                            Core.Logger.Warn($"站桩模式下，强制攻击");
                        return power;
                    }
                    else 
                    {
                        cannotOffensive = true;
                    }
                }
            }

            // TODO: 尝试血步解围
            if (IsReallyBlocked)
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"进入卡位处理，10码{_MobsIn10}, 20码{_MobsIn20}, 攻击{cannotOffensive}");
                
                if (_MobsIn20 == 0)
                {
                    if (Core.BlockedCheck.BlockedTime.TotalMilliseconds > 500)
                    {
                        if (IsDebugMove)
                            Core.Logger.Warn($"被长时间卡住了");
                        TrinityActor actor = BestDestructibleInRange(Player.Position, 20f);
                        if (null != actor)
                        {
                            if (IsDebugMove)
                                Core.Logger.Warn($"有可能是被可破坏物挡住了，摧毁它");
                            
                            if (actor.IsCorruptGrowth && Skills.Necromancer.SkeletalMage.CanCast())
                                return MySkeletalMage(actor);

                            if (Skills.Necromancer.BoneSpikes.CanCast())
                                return BoneSpikes(actor);

                            if (Skills.Necromancer.DeathNova.CanCast())
                                return DeathNova(actor);

                            if (Skills.Necromancer.SkeletalMage.CanCast())
                                return MySkeletalMage(actor);
                        }
                    }

                    if (Core.BlockedCheck.BlockedTime.TotalMilliseconds > 1500 && CanBloodRush())
                    {
                        Core.Logger.Warn($"被长时间阻挡了，尝试血步解围");
                        position = SafePotForBloodRush();
                        if (position != Vector3.Zero)
                            return BloodRush(position);
                    }
                }
                else if (!cannotOffensive)
                {
                    power = GetOffensivePower();
                    if (power != null)
                    {
                        if (IsDebugMove)
                            Core.Logger.Warn($"被卡住了，强制攻击");
                        return power;
                    }
                    else 
                    {
                        cannotOffensive = true;
                        if (IsDebugMove)
                            Core.Logger.Warn($"被卡住了，周围有怪但是无法攻击");
                    }
                }
            }



            // if (!cannotOffensive && _ClosestTargetIn60 != null && !Core.Avoidance.InAvoidance(Player.Position) && AnyCriticalInRange())
            // {
            //     Core.Logger.Warn($"有可能在熔火附近鬼畜，强制攻击");
            //     power = GetOffensivePower();
            //     if (power != null)
            //     {
            //         // if (IsDebugMove)
            //         //     Core.Logger.Warn($"有可能在熔火附近鬼畜，强制攻击");
            //         return power;
            //     }
            //     else 
            //     {
            //         // if (IsDebugMove)
            //         //     Core.Logger.Warn($"有可能在鬼畜，周围有怪但是无法攻击");
            //     }
            // }

            if (ShouldGoTo(destination))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"向目标移动，距离我{destination.Distance(Player.Position)}");
                return ToDestInPath(destination); 
            }
            else if (!cannotOffensive)
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"目标很危险，准备强制攻击");
                power = GetOffensivePower();
                if (power != null)
                {
                    return power;
                }
                else 
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"目标很危险，而且强制攻击失败");
                    cannotOffensive = true;
                }
            }
            else 
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"目标很危险，而且强制攻击，保持等待");
                return Walk(Player.Position);
            }

            if (!cannotOffensive)
            {
                power = GetOffensivePower();
                if (power != null)
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"强制攻击");
                    return power;
                }
                else 
                {
                    cannotOffensive = true;
                    if (IsDebugMove)
                        Core.Logger.Warn($"强制攻击失败，按照Trinity的指示前进");
                }
            }

            return ToDestInPath(destination);
        }

        #region DecisionHelpers

		private bool __avoider(AvoidanceNode n)
        {
            if (Core.Avoidance.InAvoidance(n.NavigableCenter))
                return false;

            if (ClosestTarget2(n.NavigableCenter, GetCastDistance()) == null)
                return false;

            // 闪避时同时向队友靠拢
            if (IsPartyGame)
            {
                TrinityActor nearestParty = _NearestParty;
                if (nearestParty != null && nearestParty.Position.Distance(Player.Position) > 40f)
                    return false;
            }

            return true;
        }

        private bool __avoider_skillonly(AvoidanceNode n)
        {
            return !Core.Avoidance.InAvoidance(n.NavigableCenter) && TrinityGrid.Instance.CanRayWalk(Player.Position, n.NavigableCenter);
        }

        private int __avoider_dist = 0;
        private int __avoider_num = 0;

        private bool __avoider_generic(AvoidanceNode n)
        {
            return !Core.Avoidance.InAvoidance(n.NavigableCenter) &&
                TrinityGrid.Instance.CanRayWalk(Player.Position, n.NavigableCenter) &&
                TargetUtil.NumMobsInRangeOfPosition(n.NavigableCenter, __avoider_dist) < __avoider_num;
        }

		private Vector3 SafePotForBloodRush()
		{
			Vector3 safePot;

			Func<AvoidanceNode, bool> myf = new Func<AvoidanceNode, bool>(__avoider);

            Vector3 projectedPosition = IsBlocked
                    ? TrinityGrid.Instance.GetPathCastPosition(50f, true)
                    : TrinityGrid.Instance.GetPathWalkPosition(50f, true);

			Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 40f, 50f, Player.Position, myf);
            if (safePot == Vector3.Zero)
			{
				Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 30f, 40f, Player.Position, myf);
			}
            if (safePot == Vector3.Zero)
			{
				Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 20f, 30f, Player.Position, myf);
			}
			if (safePot == Vector3.Zero)
			{
				Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 15f, 50f, Player.Position, null);
			}
			if (safePot == Vector3.Zero)
			{
				safePot = projectedPosition;
			}

            if (safePot == Vector3.Zero)
            {
                if (IsDebugAvoid)
                    Core.Logger.Warn($"竟然无处可逃，只能站着等死");
            }

			return safePot;
		}

        private Vector3 SafePotForAvoid()
        {
            Vector3 safePot = Vector3.Zero;

			Func<AvoidanceNode, bool> myf_safe = new Func<AvoidanceNode, bool>(__avoider_generic);
            Func<AvoidanceNode, bool> myf_skill = new Func<AvoidanceNode, bool>(__avoider_skillonly);

            Vector3 projectedPosition = IsBlocked
                    ? TrinityGrid.Instance.GetPathCastPosition(40f, true)
                    : TrinityGrid.Instance.GetPathWalkPosition(40f, true);

            // __avoider_dist = 40;
            // for (int i = 1; i <= 3; i++)
            // {
            //     __avoider_num = i;
            //     if (Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 16f, 60f, Player.Position, myf_safe))
            //         break;
            // }

            if (safePot == Vector3.Zero)
            {
                __avoider_dist = 30;
                for (int i = 1; i <= 3; i++)
                {
                    __avoider_num = i;
                    if (Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 16f, 60f, Player.Position, myf_safe))
                        break;
                }
            }

            if (safePot == Vector3.Zero)
            {
                __avoider_dist = 20;
                for (int i = 1; i <= 3; i++)
                {
                    __avoider_num = i;
                    if (Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 16f, 60f, Player.Position, myf_safe))
                        break;
                }
            }

			// if (safePot == Vector3.Zero)
			// {
			// 	Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 20f, 40f, Player.Position, myf_skill);
			// }
            // if (safePot == Vector3.Zero)
			// {
			// 	Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 20f, 40f, Player.Position, null);
			// }
			if (safePot == Vector3.Zero)
			{
				safePot = projectedPosition;
			}

            if (safePot == Vector3.Zero)
            {
                Core.Logger.Warn($"没有合适的闪避点");
            }

			return safePot;
        }

        private bool FindBoss()
        {
            TrinityActor boss = ClosestBossInRange(75f);
            if (boss == null)
            {
                hasBoss = false;
                bossDistance = 100f;
                bossPos = Vector3.Zero;
                return false;
            }

            hasBoss = true;
            bossDistance = boss.RadiusDistance;
            bossPos = boss.Position;

            return true;
        }

        private SNOWorld _worldId = SNOWorld.Invalid;
        private bool needCheckMonster = false;
        private bool hasDanger = false;

        private void CheckDangerMonster()
        {
            if (Core.Rift.IsNephalemRift || !CautiousMode)
                return;

            if (hasDanger && IsDebugAvoid)
                Core.Logger.Warn($"检测到爷，以安全方式推进");
            
            if (_worldId != Core.Player.WorldSnoId)
            {
                needCheckMonster = true;
                hasDanger = false;
                _worldId = Core.Player.WorldSnoId;
            }

            if (!needCheckMonster)
                return;

            List<TrinityActor> y = FindYe(75f);
            if (y.Any())
            {
                needCheckMonster = false;
                hasDanger = true;
            }
        }

        private int deathCount;
        private double lastDeathTick = 0;
        private double revengeTick = 0;
        private double revengeGap = 20*1000;
        bool needRevenge = false;

        private void CheckDeath()
        {
            if (!FightingRevengeMode)
                return;

            if (!Skills.Necromancer.LandOfTheDead.IsActive)
                return;
            
            if (deathCount != ZetaDia.Me.CommonData.DeathCount)
            {
                deathCount = ZetaDia.Me.CommonData.DeathCount;

                if (MyGetTickCount() - lastDeathTick < 30*1000)
                {
                    needRevenge = true;
                    revengeTick = MyGetTickCount();

                    if (IsDebugTB)
                        Core.Logger.Warn($"30秒内连挂2次，进入复仇模式");
                }
                else
                {
                    needRevenge = false;
                }
                lastDeathTick = MyGetTickCount();
            }

            if (needRevenge && MyGetTickCount() - revengeTick > revengeGap)
            {
                if (IsDebugTB)
                    Core.Logger.Warn($"距离复仇模式超过20秒，解除复仇模式");
                needRevenge = false;
            }
        }

        private bool isAvoiding = false;
        private double intervalToCheckAvoid = 2000;
        private double intervalOfAvoid = 600;
        private double lastCheckAvoidTick = 0;
        private double lastAvoidTick = 0;
        private double avoidSafeInterval = 3000;
        private Vector3 avoidPos = Vector3.Zero;

        private bool CheckAvoiding(out TrinityPower power)
        {
            power = null;

            if (isAvoiding)
            {
                if (MyGetTickCount() - lastAvoidTick > intervalOfAvoid)
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"闪避时间已过, 当前距离闪避点{avoidPos.Distance(Player.Position)}");
                    isAvoiding = false;
                    return false;
                }

                if (avoidPos.Distance(Player.Position) < 5f)
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"到达安全点附近，闪避动作结束");
                    isAvoiding = false;
                    return false;
                }
                if (avoidPos != Vector3.Zero)
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"闪避中，向安全点{avoidPos}移动");
                    power = ToDestByWalk(avoidPos);
                    return true;
                }
                else 
                {
                    isAvoiding = false;
                }
            }

            return false;
        }

        private float safeDistance = 25f;
        private bool TryAvoidToSafePot(out TrinityPower power)
        {
            power = null;
            Vector3 pos = Vector3.Zero;

            if (!IsInCombat)
                return false;

            if (!Core.Rift.IsGreaterRift)
                return false;

            if (Core.Buffs.HasInvulnerableShrine)
                return false;

            if (Skills.Necromancer.GrimScythe.IsActive)
                return false;

            if (CheckAvoiding(out power))
                return true;
            
            if (MyGetTickCount() - lastCheckAvoidTick < intervalToCheckAvoid)
                return false;          
            
            lastCheckAvoidTick = MyGetTickCount();
            if (GetCastDistance() < safeDistance)
                return false;

           // 带骨甲要注意，放了骨甲才能开始后退
            
            bool needAvoid = false;

            if (hasBoss)
            {
                if(bossPos.Distance(Player.Position) < safeDistance)
                {
                    needAvoid = true;
                }
            }
            else if (UseIntelliAvoider)
            {
                if (TargetUtil.AnyMobsInRange(safeDistance) && Player.CurrentHealthPct < 0.6)
                {
                    needAvoid = true;
                }

                if ((IsInOcculus() && Player.CurrentHealthPct < 0.4) || (!IsInOcculus() && Player.CurrentHealthPct < 0.6))
                {
                    needAvoid = true;
                }

                if (Core.Avoidance.InAvoidance(Player.Position))
                {
                    needAvoid = true;
                }

                int mobs = _MobsIn10;
                if (mobs > 4)
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"被包围了，放弃闪避，死磕到底！");
                    return false;
                }
            }
            if (!needAvoid)
                return false;

            if (ShouldMoveInCombat(out power))
            {
                if (IsDebugAvoid)
                    Core.Logger.Warn($"通过吃球进行闪避");

                return true;
            }
            
            // 如果带了血步，只用血步闪避boss
            if (Skills.Necromancer.BloodRush.IsActive && CanBloodRush() && Skills.Necromancer.BloodRush.TimeSinceUse > 1500 && hasBoss)
            {
                pos = SafePotForBloodRush();
                if (pos != Vector3.Zero)
                {
                    power = BloodRush(pos);
                    return true;
                }
            }

            pos = SafePotForAvoid();
            if (IsDebugAvoid)
                Core.Logger.Warn($"开始向{pos}闪避，新位置距离我{pos.Distance(Player.Position)}");

            if (pos == Vector3.Zero)
            {
                if (IsDebugAvoid)
                    Core.Logger.Warn($"找不到安全的闪避点，放弃闪避");
                return false;
            }
            isAvoiding = true;
            avoidPos = pos;
            lastAvoidTick = MyGetTickCount();

            power = ToDestByWalk(pos); // TODO:WALK
            return true;
        }

        private bool TryRescureByBloodRush(out TrinityPower power)
        {
            power = null;
            TrinityActor healthGlobe = null;
            Vector3 pos = Vector3.Zero;

            if (!IsInCombat)
                return false;

            if (!Core.Rift.IsGreaterRift && IgnoreAvoidanceInN)
                return false;

            if (Core.Buffs.HasInvulnerableShrine)
                return false;

            if (!CanBloodRush(true))
            {
                return false;
            }

            bool needRescure = false;

            if (!needRescure)
            {
                if (Player.CurrentHealthPct < 0.6)
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"生命值太低，尝试使用血步闪避");
                    needRescure = true;
                }
            }

            if (!needRescure)
            {
                if (Core.Avoidance.InAvoidance(Player.Position))
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"附近有地板技能，尝试使用血步闪避");
                    needRescure = true;
                }
            }

            if (!needRescure)
            {
                if (BloodRushAvoidElite && _AnyElitesInSafeRange && !IsInOcculus())
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"附近有精英，尝试使用血步闪避");
                    needRescure = true;
                }
            }

            if (!needRescure)
                return false;

            if ((IsInOcculus() && Player.CurrentHealthPct < 0.4) || (!IsInOcculus() && Player.CurrentHealthPct < 0.6))
            {
                // 尝试找到一个血球最密集的点
                healthGlobe = FindHealthGlobeHeap(Player.Position, 10f, 50f, BloodRushSafeRange, true, GetCastDistance())??
                    FindHealthGlobeHeap(Player.Position, 10f, 50f, BloodRushSafeRange, false, 70f)??
                    FindHealthGlobeHeap(Player.Position, 10f, 50f, BloodRushSafeRange, true, GetCastDistance(), false)??
                    FindHealthGlobeHeap(Player.Position, 10f, 50f, BloodRushSafeRange, false, 70f, false);

                if (healthGlobe != null)
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"找到了血球堆，距离我{healthGlobe.Distance}，用血步飞");
                    power = BloodRush(healthGlobe.Position);

                    // 已经使用血步进行闪避，进行中的闪避取消
                    isAvoiding = false;
                    if (SurviveMode)
                        lastAvoidTick = MyGetTickCount();
                    return true;
                }
            }

            // 找不到球堆，找安全点
            // 如果安全点附近没有血球，而且吃了减耗塔，会导致一直飞
            // 辅助死灵也需要修改
            if (Skills.Necromancer.BloodRush.TimeSinceUse < 1500)
                return false;
            
            pos = SafePotForBloodRush();
            if (pos == Vector3.Zero)
                return false;

            power = BloodRush(pos);

            if (IsDebugAvoid)
                Core.Logger.Warn($"找到了安全点，距离我{pos.Distance(Player.Position)}，用血步飞");

            // 已经使用血步进行闪避，进行中的闪避取消
            isAvoiding = false;
            // 使用血步触发闪避以后，也进入安全模式
            if (SurviveMode)
                lastAvoidTick = MyGetTickCount();

            return true;
        }

        private bool IsSafePosition(Vector3 position)
        {
            if (Core.Buffs.HasInvulnerableShrine)
                return true;

            if (Core.Avoidance.InAvoidance(position))
                return false;

            if (Skills.Necromancer.BloodRush.IsActive && BloodRushAvoidElite && _AnyElitesInSafeRange)
                return false;

            return true;
        }

        private bool TryGoOcculus(out TrinityPower power)
        {
            power = null;

            // 小米不踩神目
            if (!Core.Rift.IsGreaterRift)
                return false;

            if (!IsInCombat)
                return false;

            if (isAvoiding)
                return false;
            
            bool canBloodRush = CanBloodRush();

            if (!canBloodRush && !GoOcculusByFoot)
                return false;
            
            Vector3 occulusPos = _ClosestOcculusIn58;

            if (occulusPos == Vector3.Zero)
                return false;

            float distance = occulusPos.Distance(Player.Position);
            
            if (IsDebugMove)
                Core.Logger.Warn($"发现神目，距离我{distance}");

            if (!IsSafePosition(occulusPos))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"神目附近有危险区域，放弃！");
                return false;
            }

            bool isNeedClose = Skills.Necromancer.GrimScythe.IsActive;
            float mobsRange = Skills.Necromancer.GrimScythe.IsActive?15f:50f;

            // 只要50码内有怪，我就敢去踩。恐镰15码
            TrinityActor target = ClosestTarget2(occulusPos, mobsRange);
            if (!IsValidTarget(target))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"神目周围50码内没有可以攻击的怪物，放弃踩神目");
                return false;
            }

            if (IsBoneArmorBuffFirst() && !Skills.Necromancer.GrimScythe.IsActive)
            {
                target = ClosestTarget2(occulusPos, 30f);
                if (!IsValidTarget(target))
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"骨甲快断了，此时踩神目会导致无法叠骨甲，放弃神目");
                    return false;
                }
            }

            if (IsPartyGame && _NearestParty != null && _NearestParty.Position.Distance(occulusPos) > 50f)
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"神目离队友太远，放弃神目");
                return false;
            }

            // 谨慎谨慎再谨慎！！！！！
            // if (SurviveMode && !Core.Buffs.HasInvulnerableShrine && Core.Buffs.HasBuff(SNOPower.P6_Necro_Passive_CheatDeath))
            // {
            //     // 如果本身处境很安全，但是目标不安全，放弃神目
            //     if (_MobsIn10 < 2 && TargetUtil.NumMobsInRangeOfPosition(occulusPos, BloodRushSafeRange) > 2)
            //     {
            //         if (IsDebugMove)
            //             Core.Logger.Warn($"春哥都出了，踩神目谨慎点");
            //         return false;
            //     }
            //     else 
            //     {
            //         if (IsDebugMove)
            //             Core.Logger.Warn($"虽然已经出了春哥，但是神目周围状况还可以，去踩!");
            //     }
            // }

            if (distance < OcculusRadius)
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"已经在神目中，无需移动");
                return false;
            }
            else if (canBloodRush)
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"飞向神目");
                power = BloodRush(occulusPos);
            }
            else if (distance < GoOcculusByFootDistance)
            {
                Vector3 closePos = MathEx.CalculatePointFrom(occulusPos, Player.Position, 10f);
                if (TrinityGrid.Instance.CanRayWalk(Player.Position, closePos))
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"向神目步行移动，距离我{distance}");

                    power = Walk(occulusPos);
                }
                else 
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"神目不能直线到达，放弃");
                    return false;
                }
            }
            else 
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"神目距离{distance}不合适，放弃");

                return false;
            }

            isAvoiding = false;
            return power != null;
        }

        private bool ShouldMoveInCombat(out TrinityPower power)
        {
            power = null;
            bool canRecoverResource = Legendary.ReapersWraps.IsEquipped || Legendary.ReapersWraps.IsEquippedInCube;
            bool needUseHealthGlobe = (canRecoverResource && Player.PrimaryResourcePct < ResourcePctForFindHealthGlobe) || Player.CurrentHealthPct < HealthPctForFindHealthGlobe || Core.Avoidance.InAvoidance(Player.Position);

            if (!needUseHealthGlobe)
                return false;

            if (ForbidFindHealthGlobe && IsPartyGame)
                return false;
            
            if (ShouldStand())
                return false;
                        
            TrinityActor healthGlobe = null;

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000)
            {
                healthGlobe = FindHealthGlobe(40f, 15f, false, 60f);
            }
            else 
            {
                healthGlobe = FindHealthGlobe(25f, 20f, true, 20f)??
                            FindHealthGlobe(25f, 20f, true, 30f)??
                            FindHealthGlobe(40f, 20f, true, 20f)??
                            FindHealthGlobe(40f, 20f, true, 30f)??
                            FindHealthGlobe(40f, 20f, true, 40f);
            }

            if (healthGlobe == null)
            {
                // if (IsDebugAvoid)
                //     Core.Logger.Warn($"找不到可以吃的球，放弃了");
                return false;
            }

            power = ToDestAny(healthGlobe.Position);
            float safeRange = healthGlobe.Position.Distance(Player.Position);

            // 在亡者领域中无所顾忌的吃球
            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000  && !Core.Buffs.HasInvulnerableShrine)
            {
                // if (IsValidTarget(target) && target.Position.Distance(Player.Position) < safeRange)
                //     return false;

                // if (TargetUtil.NumMobsInRangeOfPosition(healthGlobe.Position, safeRange) > 0)
                //     return false;
            }

            // 如果有护盾，不考虑闪避
            if (!Core.Buffs.HasInvulnerableShrine)
            {
                if (Core.Avoidance.InAvoidance(healthGlobe.Position))
                    return false;

                if (Core.Avoidance.Grid.IsIntersectedByFlags(healthGlobe.Position, Core.Player.Position, AvoidanceFlags.CriticalAvoidance))
                    return false;
            }

            if (IsDebugAvoid || IsDebugMove)
                Core.Logger.Warn($"战斗中，向安全血球移动，距离我{healthGlobe.Distance}");
            
            return true;
        }

        private bool ShouldMoveInPath(out TrinityPower power, Vector3 dest)
        {
            power = null;

            if (_MobsIn50 > 0)
                return ShouldMoveInCombat(out power);

            bool canRecoverResource = Legendary.ReapersWraps.IsEquipped || Legendary.ReapersWraps.IsEquippedInCube;
            bool needUseHealthGlobe = (canRecoverResource && Player.PrimaryResourcePct < ResourcePctForFindHealthGlobe) || Player.CurrentHealthPct < HealthPctForFindHealthGlobe || Core.Avoidance.InAvoidance(Player.Position);

            if (!needUseHealthGlobe)
                return false;

            if (ForbidFindHealthGlobe && IsPartyGame)
                return false;
            
            if (ShouldStand())
                return false;
                        
            TrinityActor healthGlobe = FindHealthGlobeAt(dest);

            if (healthGlobe == null)
            {
                return false;
            }

            power = ToDestAny(healthGlobe.Position);
            float safeRange = healthGlobe.Position.Distance(Player.Position);

            // 在亡者领域中无所顾忌的吃球
            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000  && !Core.Buffs.HasInvulnerableShrine)
            {
                // if (IsValidTarget(target) && target.Position.Distance(Player.Position) < safeRange)
                //     return false;

                // if (TargetUtil.NumMobsInRangeOfPosition(healthGlobe.Position, safeRange) > 0)
                //     return false;
            }

            // 如果有护盾，不考虑闪避
            if (!Core.Buffs.HasInvulnerableShrine)
            {
                if (Core.Avoidance.InAvoidance(healthGlobe.Position))
                    return false;

                if (Core.Avoidance.Grid.IsIntersectedByFlags(healthGlobe.Position, Core.Player.Position, AvoidanceFlags.CriticalAvoidance))
                    return false;
            }

            if (IsDebugAvoid || IsDebugMove)
                Core.Logger.Warn($"行进中，向目标附近的血球移动，距离我{healthGlobe.Distance}");
            
            return true;
        }

        private int GetCastDistance()
        {
            if (Core.Rift.IsNephalemRift)
            {
                return CastDistanceWhenNephalemRift;
            }
            else
            {
                if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000)
                {
                    return CastDistance;
                }
                else 
                {
                    return CastDistanceWhenLandOfDeath;
                }
            }            
        }

        private bool ShouldGoTo(Vector3 destination)
        {
            return true;

            // if (!IsInCombat)
            //     return true;
            
            // if (Core.Buffs.HasInvulnerableShrine)
            //     return true;

            // if (destination.Distance(Player.Position) > 15f && Core.Avoidance.Grid.IsIntersectedByFlags(destination, Core.Player.Position, AvoidanceFlags.CriticalAvoidance))
            //     return false;

            // return true;
        }

        private bool ShouldCloseToTarget(out bool byBoneArmor)
        {
            byBoneArmor = false;
            TrinityActor target = _ClosestTargetIn60Unsafe;

            if (!IsInCombat)
                return false;

            if (!IsValidTarget(target))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"60码内没有可攻击目标，准备向前贴怪");
                return true;
            }

            if (!IsBoneArmorBuffFirst())
                return false;

            if (!TargetUtil.AnyMobsInRange(BoneArmorRange))
            {
                byBoneArmor = true;
                return true;
            }

            return false;
        }

        private bool TryCloseToTarget(out TrinityPower power, bool force = false, bool byBoneArmor = false)
        {
            power = null;
            TrinityActor closestTargetNear = ClosestTarget2(Player.Position, 60f, true);//_ClosestTargetIn60Unsafe;
            TrinityActor closestTarget = ClosestTarget(Player.Position, 75f, true); // TODO: 会贴不了？

            if (!IsInCombat)
                return false;

            // 即使是强制贴脸，也要非常谨慎，避免直接撞怪群
            if (force && IsValidTarget(closestTarget))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"强制向前贴怪，距离我{closestTarget.Distance}");
                power = Walk(closestTarget.Position, 10f);
                return true;
            }

            // 骨甲流贴脸，是可以硬踩的
            if (byBoneArmor && IsValidTarget(closestTarget))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"需要放骨甲，向前贴怪，距离我{closestTarget.Distance}");

                power = ToDestAny(closestTarget.Position);
                return true;
            }

            if (ShouldStand())
                return false;

            if (!IsValidTarget(closestTarget)) 
            {
                closestTarget = GetClosestUnitUnSafe(); //修正：可能出现不能直接到达的目标，依然要往前贴
                if (!IsValidTarget(closestTarget))
                    return false;
            }

            if (Core.Rift.IsNephalemRift || (Core.Rift.IsGreaterRift && CastDistance != 60))
            {
                int distance = Core.Rift.IsNephalemRift?CastDistanceWhenNephalemRift:CastDistance;
                if (closestTarget.Distance > distance && Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000)
                {
                    Vector3 destination = MathEx.CalculatePointFrom(Player.Position, closestTarget.Position, closestTarget.Distance - distance);
                    if (!Core.Avoidance.InCriticalAvoidance(destination) || Core.Buffs.HasInvulnerableShrine)
                    {
                        if (IsDebugMove)
                            Core.Logger.Warn($"新目标距离我太远={closestTarget.Distance},超过了设定{distance}，尝试拉近距离");
                        power = Walk(destination, 10f);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GetOculusPosition(out Vector3 position, float range, Vector3 fromLocation)
        {
            position = Vector3.Zero;

            TrinityActor actor = 
                (from u in TargetUtil.SafeList(false)
                 where fromLocation.Distance2D(u.Position) <= range &&
                       u.ActorSnoId == SNOActor.p2_itemPassive_unique_ring_017_dome
                 orderby u.Distance
                 select u).ToList().FirstOrDefault();

            if (actor == null)
                return false;

            position = actor.Position;
            position.Z = Player.Position.Z; // Z坐标修正，他妈的你神目为什么出现在天上？

            return true;
        }

        private bool IsInOcculus()
        {
            Vector3 occulusPos = _ClosestOcculusIn58;
            if (occulusPos == Vector3.Zero)
                return false;
            
            return occulusPos.Distance(Player.Position) < OcculusRadius;
        }

        private List<Vector3> GetCirclePoints(int points, double radius, Vector3 center)
        {
            var result = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var newX = (int)(center.X + radius * Math.Cos(angle));
                var newY = (int)(center.Y + radius * Math.Sin(angle));

                var newpoint = new Vector3(newX, newY, center.Z);
                result.Add(newpoint);
            }
            return result;
        }

        public float GetClosestUnitDistanceFromPoint(Vector3 point, float maxDistance = 20f)
        {
            var result =
                (from u in ObjectCache
                 where u.IsUnit && u.IsValid && u.Weight > 0 && u.RadiusDistance <= maxDistance
                 orderby u.Position.Distance(point)
                 select u).FirstOrDefault();

            TrinityActor actor = result;
            if (result == null)
                return 60f;
            else
                return actor.Position.Distance(point);
        }

        // public Vector3 GetSafeSpotPosition(float distance = 20f)
        // {
        //     Func<Vector3, bool> isValid = p => Core.Grids.CanRayCast(p) && !TargetUtil.IsPositionOnMonster(p, 20f);

        //     var circlePositions = GetCirclePoints(8, distance, Player.Position);
        //     circlePositions.AddRange(GetCirclePoints(8, distance + 10f, Player.Position));

        //     var closestMonster = GetBestTarget(Player.Position);
        //     var proximityTarget = closestMonster != null ? closestMonster.Position : Player.Position;
        //     var validPositions = circlePositions.Where(isValid).OrderByDescending(p => GetClosestUnitDistanceFromPoint(p, 20f));

        //     return validPositions.FirstOrDefault();
        // }

        private static TrinityActor ClosestBossInRange(float range)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid && u.Weight > 0 &&
                          (u.IsBoss || u.ActorSnoId == SNOActor.X1_LR_Boss_TerrorDemon_A) && u.IsValid && u.Weight > 0 &&
                          u.IsInLineOfSight &&
                          u.Distance <= range
                    orderby
                          u.Distance
                    select u).FirstOrDefault();
        }

        private static TrinityActor FindOrdnanceTower(Vector3 pos, float range = 60f, bool canRayWalk = false)
        {				
			TrinityActor actor = (from u in TargetUtil.SafeList(true)
                where
                    u.ActorSnoId == SNOActor.X1_Pand_Ext_Ordnance_Tower_Shock_A &&
                    TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                select u).FirstOrDefault();

            if (actor != null)
            {
                Core.Logger.Warn($"向电击塔发动攻击！");
            }

            return actor;
        }

        private static TrinityActor BestEliteInRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {				
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 &&
					((u.IsElite && u.EliteType != EliteTypes.Minion) || u.ActorSnoId == SNOActor.X1_LR_Boss_TerrorDemon_A) && 
                    !u.IsSafeSpot && u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                    TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        private static TrinityActor BestEliteInRangeByRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {				
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 &&
					((u.IsElite && u.EliteType != EliteTypes.Minion) || u.ActorSnoId == SNOActor.X1_LR_Boss_TerrorDemon_A) && 
                    !u.IsSafeSpot && u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                    TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.NearbyUnitsWithinDistance(15f) descending,
                    u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        private static TrinityActor BestMinionInRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {				
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 &&
                    (u.IsElite && u.EliteType == EliteTypes.Minion) &&
                    !u.IsSafeSpot && u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                      TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        private static TrinityActor BestMinionInRangeByRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {				
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 &&
                    (u.IsElite && u.EliteType == EliteTypes.Minion) &&
                    !u.IsSafeSpot && u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                      TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.NearbyUnitsWithinDistance(15f) descending,
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }
		
		private TrinityActor BestTrashInRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 && !u.IsSafeSpot && !u.IsElite && 
                    u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                      TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        private TrinityActor BestTrashInRangeByRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 && !u.IsSafeSpot && !u.IsElite && 
                    u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                      TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.NearbyUnitsWithinDistance(15f) descending,
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        private TrinityActor GetBestTarget(Vector3 pos, float range = 60f, bool canRayWalk = true, TrinityActor exclude = null)
        {
            return GetBestTarget2(pos, range, canRayWalk, exclude)??GetClosestUnitUnSafe(range>30?30:range);
        }

        private TrinityActor GetBestTarget2(Vector3 pos, float range = 60f, bool canRayWalk = true, TrinityActor exclude = null)
        {
            TrinityActor ordnanceTower = ForceAttackOrdnanceTower?FindOrdnanceTower(pos, range, canRayWalk):null;

            // 组队模式下，要充分的利用范围伤优势
            if (!RubbishPC)
            {
                return 
                    ordnanceTower??
                    BestEliteInRangeByRange(pos, range, canRayWalk, exclude)??
                    BestMinionInRangeByRange(pos, range, canRayWalk, exclude)??
                    BestTrashInRangeByRange(pos, range, canRayWalk, exclude);
            }
            else 
            {
                return 
                    ordnanceTower??
                    BestEliteInRange(pos, range, canRayWalk, exclude)??
                    BestMinionInRange(pos, range, canRayWalk, exclude)??
                    BestTrashInRange(pos, range, canRayWalk, exclude);
            }
        }

        private TrinityActor ClosestTarget(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return ClosestTarget2(pos, range, canRayWalk, exclude)??GetClosestUnitUnSafe(range>30?30:range);
        }

        private TrinityActor ClosestTarget2(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 && !u.IsSafeSpot && 
                    u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                      TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        // TODO: 为什么有时候找不到贴身的怪？？？
        private TrinityActor GetClosestUnitUnSafe(float maxDistance = 65f)
        {
            var result =
                (from u in ObjectCache
                where 
                    u.IsUnit && u.IsValid && u.Weight > 0 && u.Distance <= maxDistance &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh
                 orderby
                    u.Distance
                select u).FirstOrDefault();

            return result;
        }

        private TrinityActor BestClusterUnit(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return (from u in ObjectCache
                where u.IsUnit && u.IsValid && u.Weight > 0 && !u.IsSafeSpot && 
                    u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != SNOActor.P6_Necro_Corpse_Flesh &&
                      TrinityGrid.Instance.CanRayCast(pos, u.Position) &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
                    u.NearbyUnitsWithinDistance(10f) descending,
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        private static bool AnyElitesInRangeOfPosition(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {				
			return (from u in ObjectCache
                where u.IsUnit &&
					((u.IsElite || u.ActorSnoId == SNOActor.X1_LR_Boss_TerrorDemon_A) && u.EliteType != EliteTypes.Minion) && u.IsValid && u.Weight > 0 &&
                    !u.IsSafeSpot && u != exclude && 
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                select u).Any();
        }

        private bool AnyMobsInRange(float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {				
			return (from u in ObjectCache
                where u.IsUnit &&
					u.IsValid && u.Weight > 0 &&
                    !u.IsSafeSpot && u != exclude &&
                    (!canRayWalk || (canRayWalk && TrinityGrid.Instance.CanRayWalk(Player.Position, u.Position))) &&
					u.RadiusDistance <= range
                select u).Any();
        }

        private TrinityActor BestTargetWithoutDebuff(float range, SNOPower debuff, Vector3 position = default(Vector3))
        {
            return BestTargetWithoutDebuffs(range, new List<SNOPower> { debuff }, position);
        }

        private TrinityActor BestTargetWithoutDebuffs(float range, IEnumerable<SNOPower> debuffs, Vector3 position = default(Vector3))
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityActor target;
            var unitsByWeight = 
                (from u in ObjectCache
                where u.IsUnit && u.IsValid &&
                    u.Weight > 0 &&
                    u.Position.Distance(position) <= range &&
                      TrinityGrid.Instance.CanRayCast(position, u.Position) &&
                    !debuffs.All(u.HasDebuff)
                orderby
                u.NearbyUnitsWithinDistance(20f) descending,
                u.Distance
                select u).ToList();

            if (unitsByWeight.Any())
                target = unitsByWeight.FirstOrDefault();
            else
                target = null;

            return target;
        }

        private bool AnyTargetWithDebuff(float range, SNOPower debuff)
        {
            var units = (from u in ObjectCache
                            where u.IsUnit && u.IsValid &&
                                u.Weight > 0 &&
                                u.Distance <= range &&
                                u.HasDebuff(debuff)
                            select u);

            return units.Any();
        }

        // 你们都很牛逼，老子让让你
        private List<SNOActor> yes = new List<SNOActor> 
        {
            SNOActor.x1_SkeletonArcher_Westmarch_Ghost_A,
            SNOActor.SkeletonArcher_E,
            SNOActor.SkeletonArcher_F,
            SNOActor.SkeletonArcher_westm_A,
            SNOActor.skeletonMage_Fire_A,
            SNOActor.p1_LR_Ghost_A,
            SNOActor.p1_LR_Ghost_Dark_A,
            SNOActor.p1_LR_Ghost_B,
            SNOActor.p1_LR_Ghost_C,
            SNOActor.p1_LR_Ghost_D,
            SNOActor.Corpulent_A,
            SNOActor.LacuniFemale_A,
            SNOActor.Succubus_C,
            SNOActor.Succubus_B,
            SNOActor.SoulRipper_B,
            SNOActor.SoulRipper_A,
            SNOActor.SoulRipper_C_Despair,
            SNOActor.FallenGrunt_C,
            SNOActor.x1_BogFamily_ranged_A,
            SNOActor.GoatMutant_Ranged_A,
            SNOActor.GoatMutant_Ranged_A_Large_Aggro,
            SNOActor.x1_leaperAngel_A,
            SNOActor.QuillDemon_C,
            SNOActor.X1_Pand_Ext_Ordnance_Tower_Shock_A,
            SNOActor.Beast_B,
            SNOActor.Beast_C,
            SNOActor.Beast_D,
            SNOActor.LacuniMale_B,
            SNOActor.LacuniMale_A,
            SNOActor.LacuniFemale_B,
            SNOActor.LacuniFemale_A,
            SNOActor.LacuniFemale_B_Range,
            SNOActor.LacuniFemale_C,
            SNOActor.LacuniMale_C,
            SNOActor.x1_BogFamily_brute_A,
            SNOActor.GoatMutant_Shaman_A,
            SNOActor.x1_BogFamily_brute_A,
            SNOActor.x1_BogFamily_brute_A_eventAngryBats,
            SNOActor.x1_BogFamily_brute_A_MaggotCrew,
            SNOActor.X1_Lore_Bestiary_TuskedBogan,
            // 0
        };

        // 判断是否有爷在附近
        private List<TrinityActor> FindYe(float range = 75f)
        {
            List<TrinityActor> units = 
                (from u in TargetUtil.SafeList(true)
                where u.IsUnit && u.IsValid &&
                    u.Weight > 0 &&
                    u.Position.Distance(Player.Position) <= range &&
                    yes.Exists(o => o == u.ActorSnoId)
                select u).ToList();

            return units;
        }

        private bool AnyCriticalInRange()
        {
            foreach (var node in Core.Avoidance.GridEnricher.CurrentNodes)
            {
                if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.CriticalAvoidance))
                    return true;
            }

            return false;
        }

        private static bool AnyShrineInRange(float distance, bool objectsInAoe = false)
        {
            return
                TargetUtil.SafeList(objectsInAoe).Any(
                    m => m.Type == TrinityObjectType.Shrine && m.Distance <= distance);
        }

        private static TrinityActor ClosestShrineInRange(float range = 60f, bool canReach = true)
        {
            return (from u in TargetUtil.SafeList(false)
                where
                    u.Type == TrinityObjectType.Shrine &&                    
                    (!canReach || (canReach && TrinityGrid.Instance.CanRayWalk(Player.Position, u.Position))) &&
					u.Distance <= range
                select u).FirstOrDefault();
        }
        		
		private static TrinityActor BestDestructibleInRange(Vector3 pos, float range = 60f, bool canReach = true)
        {	
			return (from u in TargetUtil.SafeList(true)
                where
                    u.Type == TrinityObjectType.Destructible &&                    
                    (!canReach || (canReach && TrinityGrid.Instance.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos)
                select u).FirstOrDefault();
        }

        private int NumHealthGlobesInPosOfRange(Vector3 pos, float range)
        {
            return (from u in TargetUtil.SafeList()
                where 
                    u?.Type == TrinityObjectType.HealthGlobe && u.Position.Distance(pos) < range
                select u).Count();
        }

        private TrinityActor FindHealthGlobe(float range = 30f, float minDistanceFromMob = 30f, 
            bool combating = false, float maxDistanceFromMob = 75f,
            bool shouldAvoid = false)
        {
            return (from u in TargetUtil.SafeList()
                where 
                    u?.Type == TrinityObjectType.HealthGlobe && u.Distance < range &&
                    TargetUtil.NumMobsInRangeOfPosition(u.Position, maxDistanceFromMob) > 0 &&
                    TrinityGrid.Instance.CanRayWalk(Player.Position, MathEx.CalculatePointFrom(u.Position, Player.Position, 8f)) &&
                    (!combating || (combating && TargetUtil.NumMobsInRangeOfPosition(u.Position, minDistanceFromMob) <= ClusterSize)) &&
                    (!shouldAvoid || (shouldAvoid && !Core.Avoidance.InAvoidance(u.Position))) &&
                    (!BloodRushAvoidElite || (BloodRushAvoidElite && !AnyElitesInRangeOfPosition(u.Position, 25f)))
                orderby
                    u.Distance
                select u).FirstOrDefault();
        }

        private TrinityActor FindHealthGlobeAt(Vector3 pos, float range = 40f, bool shouldAvoid = false)
        {
            return (from u in TargetUtil.SafeList()
                where 
                    u?.Type == TrinityObjectType.HealthGlobe && u.Position.Distance(pos) < range &&
                    TrinityGrid.Instance.CanRayWalk(Player.Position, MathEx.CalculatePointFrom(u.Position, Player.Position, 8f)) &&
                    (!shouldAvoid || (shouldAvoid && !Core.Avoidance.InAvoidance(u.Position))) &&
                    (!BloodRushAvoidElite || (BloodRushAvoidElite && !AnyElitesInRangeOfPosition(u.Position, 25f)))
                orderby
                    u.Distance
                select u).FirstOrDefault();
        }

        private TrinityActor FindHealthGlobeHeap(Vector3 fromPos, float minRange = 10f, float maxRange = 50f, float minDistanceFromMob = 30f, 
            bool combating = false, float maxDistanceFromMob = 75f,
            bool shouldAvoid = false, bool shouldAvoidCritical = true)
        {
            return (from u in TargetUtil.SafeList()
                where 
                    u?.Type == TrinityObjectType.HealthGlobe && 
                    u.Position.Distance(fromPos) < maxRange && u.Position.Distance(fromPos) > minRange &&
                    TargetUtil.NumMobsInRangeOfPosition(u.Position, maxDistanceFromMob) > 0 &&
                    TrinityGrid.Instance.CanRayWalk(Player.Position, u.Position) &&
                    (!combating || (combating && TargetUtil.NumMobsInRangeOfPosition(u.Position, minDistanceFromMob) <= ClusterSize)) &&
                    (!shouldAvoid || !Core.Avoidance.InAvoidance(u.Position)) &&
                    (!shouldAvoidCritical || !Core.Avoidance.InCriticalAvoidance(u.Position))
                orderby
                    NumHealthGlobesInPosOfRange(u.Position, 10f) descending
                select u).FirstOrDefault();
        }

        private bool AnyHealthGlobeInRange (float range = 40f)
        {
            return (from u in TargetUtil.SafeList()
                where 
                    u?.Type == TrinityObjectType.HealthGlobe && u.Distance < range
                select u).Any();
        }

        private TrinityActor FindProgressGlobe (float range = 30f)
        {
            return (from u in TargetUtil.SafeList()
                where 
                    u?.Type == TrinityObjectType.ProgressionGlobe && u.Distance < range
                orderby
                    u.Distance
                select u).FirstOrDefault();
        }

        private static bool IsValidTarget(TrinityActor target)
        {
            if (target == null)
                return false;

            if (target.AcdId == 322194)
                return true;

            if (!(target.IsUnit && target.IsValid && target.Weight > 0))
                return false;
            
            if (target.Type == TrinityObjectType.Shrine ||
                target.Type == TrinityObjectType.ProgressionGlobe ||
                target.Type == TrinityObjectType.HealthGlobe ||
                target.ActorSnoId == SNOActor.P6_Necro_Corpse_Flesh)
                return false;

            return true;
            // return target.IsBoss || target.IsElite || target.IsTrashMob || target.IsTreasureGoblin;
        }

        private bool GetSkillInfo(SNOPower snoPower, out int slot, out int runeIndex)
		{
			bool isOverrideActive = false;
            try
            {
                isOverrideActive = ZetaDia.Me.SkillOverrideActive;
            }
            catch (ArgumentException)
            {
            }
			
			var cPlayer = ZetaDia.Storage.PlayerDataManager.ActivePlayerData;
			slot = 0;
			runeIndex = 0;
			for (int i = 0; i <= 5; i++)
            {			
                var diaActiveSkill = cPlayer.GetActiveSkillByIndex(i, isOverrideActive);
                if (diaActiveSkill == null || diaActiveSkill.Power == SNOPower.None)
                    continue;

				if (diaActiveSkill.Power == snoPower)
				{
					slot = i;
					runeIndex = diaActiveSkill.RuneIndex;
					return true;
				}
            }

			return false;
		}

        private double lastChangeTime = 0;
        private double timeForSkillChange = 500;
        private SNOWorld worldId2 = SNOWorld.Invalid;
        private double maxCheckIntervalForSkillChange = 30000;
        private double lastChangeWorldTime = 0;
        private bool needTryChangeSkill = false;

        private void TryChangeSkills()
        {
            if (!Settings.AutoChangeSkill)
                return;

            if (Core.Player.WorldSnoId != worldId2)
            {
                lastChangeWorldTime = MyGetTickCount();
                needTryChangeSkill = true;
                worldId2 = Core.Player.WorldSnoId;
            }

            if (!needTryChangeSkill)
                return;

            if (MyGetTickCount() - lastChangeWorldTime > maxCheckIntervalForSkillChange)
            {
                needTryChangeSkill = false;
            }

            // 鲜血奔行
            int slotOfBloodRush = 0;
            int runeIndexOfBloodRush = 0;
            bool hasBloodRush = false;

            // 骨刺
            int slotOfBoneSpikes = 0;
            int runeIndexOfBoneSpikes = 0;
            bool hasBoneSpikes = false;

            // 鲜血虹吸
            int slotOfSiphonBlood = 0;
            int runeIndexOfSiphonBlood = 0;
            bool hasSiphonBlood = false;     

            // 血魂双分
            int slotOfSimulacrum = 0;
			int runeIndexOfSimulacrum = 0;
			bool hasSimulacrum = false; //GetSkillInfo(SNOPower.P6_Necro_Simulacrum, out slotOfSimulacrum, out runeIndexOfSimulacrum);

            // 亡者领域
            int slotOfLOTD = 0;
			int runeIndexOfLOTD = 0;
			bool hasLOTD = false; //GetSkillInfo(SNOPower.P6_Necro_LandOfTheDead, out slotOfLOTD, out runeIndexOfLOTD);      
            
            // 衰老
            int slotOfDecrepify = 0;
            int runeIndexOfDecrepify = 0;
            bool hasDecrepify = false; 

            // Core.Logger.Warn($"换装状态{Player.IsInTown}{Core.Rift.IsGreaterRift}{Core.Rift.RiftComplete}");
            // 在城里
            if (Player.IsInTown && !(Core.Rift.IsGreaterRift && !Core.Rift.RiftComplete))
            {
                double lastTime = MyGetTickCount()-lastChangeTime;
                if (lastTime > timeForSkillChange)
                {

                    hasBloodRush = GetSkillInfo(SNOPower.P6_Necro_BloodRush, out slotOfBloodRush, out runeIndexOfBloodRush);
                    hasBoneSpikes = GetSkillInfo(SNOPower.P6_Necro_BoneSpikes, out slotOfBoneSpikes, out runeIndexOfBoneSpikes);
                    hasSiphonBlood = GetSkillInfo(SNOPower.P6_Necro_SiphonBlood, out slotOfSiphonBlood, out runeIndexOfSiphonBlood);  
                    hasSimulacrum = GetSkillInfo(SNOPower.P6_Necro_Simulacrum, out slotOfSimulacrum, out runeIndexOfSimulacrum);
                    hasLOTD = GetSkillInfo(SNOPower.P6_Necro_LandOfTheDead, out slotOfLOTD, out runeIndexOfLOTD);    
                    hasDecrepify = GetSkillInfo(SNOPower.P6_Necro_Decrepify, out slotOfDecrepify, out runeIndexOfDecrepify);

                    if (hasSiphonBlood && !hasBoneSpikes && BoneSpike2SiphonBlood)
                    {
                        ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BoneSpikes.SNOPower, 0, (HotbarSlot)slotOfSiphonBlood);
                        lastChangeTime = MyGetTickCount();
                        Core.Logger.Warn($"在城镇中，鲜血虹吸->骨刺-出其不意");
                    }

                    if (HasBloodRushInBD && hasBloodRush && runeIndexOfBloodRush != 3)
                    {
                        if (Skills.Necromancer.BloodRush.CanCast())
                        {
                            ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BloodRush.SNOPower, 3, (HotbarSlot)slotOfBloodRush);
                            lastChangeTime = MyGetTickCount();
                            Core.Logger.Warn($"在城镇中，鲜血奔行更换符文->鲜血潜能");
                        }
                    }

                    if (!HasBloodRushInBD && hasBloodRush)
                    {
                        // 如果是骨甲轮回流，换回诅咒
                        if (Simulacrum2BloodRush && !hasSimulacrum)
                        {
                            if (Skills.Necromancer.BloodRush.CanCast())
                            {
                                ZetaDia.Me.SetActiveSkill(Skills.Necromancer.Simulacrum.SNOPower, 0, (HotbarSlot)slotOfBloodRush);
                                lastChangeTime = MyGetTickCount();
                                Core.Logger.Warn($"在城镇中，鲜血奔行->血魂双分");
                            }
                        }
                        else if (LOTD2BloodRush && !hasLOTD)
                        {
                            if (Skills.Necromancer.BloodRush.CanCast())
                            {
                                ZetaDia.Me.SetActiveSkill(Skills.Necromancer.LandOfTheDead.SNOPower, 1, (HotbarSlot)slotOfBloodRush);
                                lastChangeTime = MyGetTickCount();
                                Core.Logger.Warn($"在城镇中，鲜血奔行->亡者领域-死寒之地");
                            }
                        }
                        else if (Decrepify2BloodRush && !hasDecrepify)
                        {
                            if (Skills.Necromancer.BloodRush.CanCast())
                            {
                                ZetaDia.Me.SetActiveSkill(Skills.Necromancer.Decrepify.SNOPower, 4, (HotbarSlot)slotOfBloodRush);
                                lastChangeTime = MyGetTickCount();
                                Core.Logger.Warn($"在城镇中，鲜血奔行->衰老-晕眩诅咒");
                            }
                        }
                    }

                    return;
                }
            }

            // 在小米，换成鲜奔行
            if (!Player.IsInTown && Core.Rift.IsNephalemRift && !ZetaDia.Me.IsInCombat)
            {
                double lastTime = MyGetTickCount()-lastChangeTime;
                if (lastTime > timeForSkillChange)
                {
                    hasBloodRush = GetSkillInfo(SNOPower.P6_Necro_BloodRush, out slotOfBloodRush, out runeIndexOfBloodRush);
                    hasBoneSpikes = GetSkillInfo(SNOPower.P6_Necro_BoneSpikes, out slotOfBoneSpikes, out runeIndexOfBoneSpikes);
                    hasSiphonBlood = GetSkillInfo(SNOPower.P6_Necro_SiphonBlood, out slotOfSiphonBlood, out runeIndexOfSiphonBlood);  
                    hasSimulacrum = GetSkillInfo(SNOPower.P6_Necro_Simulacrum, out slotOfSimulacrum, out runeIndexOfSimulacrum);
                    hasLOTD = GetSkillInfo(SNOPower.P6_Necro_LandOfTheDead, out slotOfLOTD, out runeIndexOfLOTD); 
                    hasDecrepify = GetSkillInfo(SNOPower.P6_Necro_Decrepify, out slotOfDecrepify, out runeIndexOfDecrepify);                 

                    if (HasBloodRushInBD && hasBloodRush && runeIndexOfBloodRush != 2)
                    {
                        if (Skills.Necromancer.BloodRush.CanCast())
                        {
                            ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BloodRush.SNOPower, 2, (HotbarSlot)slotOfBloodRush);
                            lastChangeTime = MyGetTickCount();
                            Core.Logger.Warn($"在城镇中，鲜血奔行更换符文-鲜血代谢");
                        }
                    }

                    if (hasBoneSpikes && !hasSiphonBlood && BoneSpike2SiphonBlood)
                    {
                        ZetaDia.Me.SetActiveSkill(Skills.Necromancer.SiphonBlood.SNOPower, 4, (HotbarSlot)slotOfBoneSpikes);
                        lastChangeTime = MyGetTickCount();
                        Core.Logger.Warn($"在小米中，骨刺->鲜血虹吸-鲜血饥渴");
                    }

                    if (!HasBloodRushInBD && !hasBloodRush)
                    {           
                        // 如果是骨甲轮回流，换回诅咒
                        if (Simulacrum2BloodRush)
                        {
                            if (Skills.Necromancer.Simulacrum.CanCast())
                            {
                                ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BloodRush.SNOPower, 2, (HotbarSlot)slotOfSimulacrum);
                                lastChangeTime = MyGetTickCount();
                                Core.Logger.Warn($"在小米中，血魂双分->鲜血奔行");      
                            } 
                        }
                        else if (LOTD2BloodRush)
                        {
                            if (Skills.Necromancer.LandOfTheDead.CanCast())
                            {
                                ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BloodRush.SNOPower, 2, (HotbarSlot)slotOfLOTD);
                                lastChangeTime = MyGetTickCount();
                                Core.Logger.Warn($"在小米中，亡者领域->鲜血奔行");
                            }
                        }
                        else if (Decrepify2BloodRush)
                        {
                            if (Skills.Necromancer.Decrepify.CanCast())
                            {
                                ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BloodRush.SNOPower, 2, (HotbarSlot)slotOfDecrepify);
                                lastChangeTime = MyGetTickCount();
                                Core.Logger.Warn($"在小米中，衰老->鲜血奔行");
                            }
                        }     
                    }
                    return;
                }
            }
            
        }

        private bool CanBloodRush(bool forRescure = false)
        {
            if (!Skills.Necromancer.BloodRush.CanCast())
                return false;

            if (forRescure && Player.CurrentHealthPct > 0.11)
                return true;
            
            if (Player.CurrentHealthPct < 0.5)
                return false;

            // if (Skills.Necromancer.BloodRush.CooldownRemaining > 0 && !HasInfiniteCasting)
            //     return false;

            return true;
        }

        protected override bool ShouldBloodRush(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Necromancer.BloodRush.CanCast())
                return false;

            if (Player.CurrentHealthPct < 0.5)
                return false;

            if (position.Distance(Player.Position) < 15f)
                return false;

            if (Skills.Necromancer.BloodRush.CooldownRemaining > 0 && !HasInfiniteCasting)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        private int BoneArmorStacks => Legendary.WisdomOfKalan.IsEquipped ? 15 : 10;
        private int BoneArmorRange => 30;
        private int MinBoneArmorInterval => 2000;
        private int MaxBoneArmorInterval => 12000;

        bool IsBoneArmorBuffFirst()
        {
            if (!IsBoneArmorFirst)
                return false;
            
            if (!Skills.Necromancer.BoneArmor.CanCast())
                return false;

            if (Skills.Necromancer.BoneArmor.TimeSinceUse < MinBoneArmorInterval)
                return false;

            if (Skills.Necromancer.BoneArmor.BuffStacks < 1)
                return true;
            
            // SNOPower.P6_Necro_BoneArmor
            // if (!Core.Buffs.HasBuff(SNOPower.P6_Necro_BoneArmor))
            //     return true;
            
            if (BuffCooldownOfBoneArmor > MaxBoneArmorInterval)// || Skills.Necromancer.BoneArmor.BuffStacks < MinBuffStackForBoneArmor)
            {
                return true;
            }

            return false;
        }

        protected override bool ShouldBoneArmor()
        {
            if (!Skills.Necromancer.BoneArmor.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(BoneArmorRange))
                return false;

            // if (Skills.Necromancer.BoneArmor.BuffStacks < MinBuffStackForBoneArmor && Skills.Necromancer.BoneArmor.TimeSinceUse < 1000)
            //     return true;

            // 因为轮回的原因，估计的持续时间很短，需要经常续buff
            if (Skills.Necromancer.BoneArmor.TimeSinceUse > MaxBoneArmorInterval) {
                if (IsDebugSkill)
                    Core.Logger.Warn($"Buff快没了，强制开buff");
                return true;
            }

            if (Skills.Necromancer.BoneArmor.TimeSinceUse < MinBoneArmorInterval)
                return false;

            if (Skills.Necromancer.BoneArmor.BuffStacks < BoneArmorStacks) {
                if (IsDebugSkill)
                    Core.Logger.Warn($"层数{Skills.Necromancer.BoneArmor.BuffStacks}低于{BoneArmorStacks}，强开骨甲");

                return true;
            }

            bool isBigCluster = TargetUtil.ClusterExists(BoneArmorRange, 2);
            bool isEliteInRange = TargetUtil.AnyElitesInRange(BoneArmorRange);
            if (isBigCluster || isEliteInRange) {
                if (IsDebugSkill)
                    Core.Logger.Warn($"怪群数量还可以，强开骨甲");
                return true;
            }

            return false;
        }

        private bool ShouldUseBig()
        {
            List<TrinityActor> eliteList;

            if (DontUseBigToMinion)
            {
                eliteList =  
                    (from u in ObjectCache
                    where u.IsUnit &&
                        ((u.IsElite || u.ActorSnoId == SNOActor.X1_LR_Boss_TerrorDemon_A) && u.EliteType != EliteTypes.Minion) &&
                        u.IsValid && u.Weight > 0 &&
                        !u.IsSafeSpot && 
                        u.Distance <= 50f
                    select u).ToList();
            }
            else 
            {
                eliteList =  
                    (from u in ObjectCache
                    where u.IsUnit &&
                        ((u.IsElite || u.ActorSnoId == SNOActor.X1_LR_Boss_TerrorDemon_A)) &&
                        u.IsValid && u.Weight > 0 &&
                        !u.IsSafeSpot && 
                        u.Distance <= 50f
                    select u).ToList();
            }

            if (eliteList.Count() == 0)
                return false;

            if (eliteList.Count() > 1)
            {
                if (IsDebugTB)
                    Core.Logger.Warn($"发现不止一个精英，准备放大招");
                return true;
            }

            TrinityActor elite = eliteList.FirstOrDefault();
            if (!IsValidTarget(elite))
                return false;

            if (DontUseBigToWeakElite && elite.HitPointsPct < ValidElitHealthPct)
            {
                if (IsDebugTB)
                    Core.Logger.Warn($"精英血量为{elite.HitPointsPct}，已经残血，不放大招");
                    
                return false;
            }

            if (IsDebugTB)
                Core.Logger.Warn($"精英血量为{elite.HitPointsPct}，满足条件，准备放大招");
            return true;
        }

        // private bool CanSimulacrum()
        // {
        //     if (Core.Rift.IsNephalemRift)
        //         return false;

        //     if (!Skills.Necromancer.Simulacrum.CanCast())
        //         return false;

        //     if (Player.CurrentHealthPct < CastSimulacrumHealthPct)
        //         return false;

        //     return true;
        // }

        protected override bool ShouldSimulacrum(out Vector3 position)
        {
            position = Player.Position;

            if (!Core.Rift.IsGreaterRift && ForbidSimulacrumInNR)
                return false;

            if (!Skills.Necromancer.Simulacrum.CanCast())
                return false;

            if (Player.CurrentHealthPct < CastSimulacrumHealthPct)
                return false;

            if (_BestTargetIn60 != null)
                position = _BestTargetIn60.Position;

            if (HasInfiniteCasting)
                return true;

            if (hasBoss)
                return true;

            switch (Settings.TwoBigUsage)
            {
                case BigUsage.AlwaysSimu_EliteLOTD:
                case BigUsage.AlwaysSimu_AlwaysLOTD:
                    return _MobsIn50 > 0;
                case BigUsage.EliteSimu_EliteLOTD:
                case BigUsage.EliteSimu_AlwaysLOTD:
                    return ShouldUseBig();
                case BigUsage.Always_Simu_LOTD_Loop:
                {
                    if (!Skills.Necromancer.LandOfTheDead.IsActive)
                        return true;

                    if (Skills.Necromancer.LandOfTheDead.TimeSinceUse <= 10000)
                        return false;

                    return _MobsIn50 > 0;
                }
                case BigUsage.Elite_Simu_LOTD_Loop:
                {
                    if (Skills.Necromancer.LandOfTheDead.IsActive && Skills.Necromancer.LandOfTheDead.TimeSinceUse <= 10000)
                        return false;
                    
                    if (!ShouldUseBig())
                        return false;

                    return true;
                }
                default:
                    return false;
            }
        }

        protected override bool ShouldLandOfTheDead(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.LandOfTheDead.CanCast())
                return false;

            if (HasInfiniteCasting)
                return true;

            if (!IsValidTarget(_BestTargetIn60))
                return false;

            if (hasBoss)
                return true;

            if (FightingRevengeMode && needRevenge)
            {
                if (IsDebugTB)
                    Core.Logger.Warn($"距离复仇开启，亡者领域清场！");
                needRevenge = false;
                return true;
            }

            switch (Settings.TwoBigUsage)
            {
                case BigUsage.AlwaysSimu_EliteLOTD:
                case BigUsage.EliteSimu_EliteLOTD:
                    return ShouldUseBig();
                case BigUsage.AlwaysSimu_AlwaysLOTD:
                case BigUsage.EliteSimu_AlwaysLOTD:
                    return _MobsIn50 > 0;
                case BigUsage.Always_Simu_LOTD_Loop:
                {
                    if (!Skills.Necromancer.Simulacrum.IsActive)
                        return true;

                    int timeForSimu = (Legendary.HauntedVisions.IsEquipped || Legendary.HauntedVisions.IsEquippedInCube)?30:15;
                    if (Skills.Necromancer.Simulacrum.TimeSinceUse <= timeForSimu*1000)
                        return false;

                    return _MobsIn50 > 0;
                }
                case BigUsage.Elite_Simu_LOTD_Loop:
                {
                    if (Skills.Necromancer.Simulacrum.IsActive)
                    {
                        int timeForSimu = (Legendary.HauntedVisions.IsEquipped || Legendary.HauntedVisions.IsEquippedInCube)?30:15;
                        if (Skills.Necromancer.Simulacrum.TimeSinceUse <= timeForSimu*1000)
                            return false;
                    }
                    
                    if (!ShouldUseBig())
                        return false;

                    return true;
                }
                default:
                    return false;
            }
        }

        protected override bool ShouldDevour()
        {
            if (!Skills.Necromancer.Devour.CanCast())
                return false;
            
            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000 && 
                Player.PrimaryResourcePct < 0.99 &&
                Skills.Necromancer.Devour.TimeSinceUse > DurationInLOTD*10)
            {
                return true;
            }

            if (Player.PrimaryResourcePct > 0.99 && !Passives.Necromancer.LifeFromDeath.IsActive)
                return false;

            if (TargetUtil.CorpseCount(60f) > 0 && Skills.Necromancer.Devour.TimeSinceUse > 500)
                return true;

            return false;
        }

        double lastCSCast = 0;
        protected override bool ShouldCommandSkeletons(out TrinityActor target)
        {
            target = null;
            bool needKeepSkeletonsNearby = false;
            
            if (!Skills.Necromancer.CommandSkeletons.CanCast())
                return false;

            if (MyGetTickCount() - lastCSCast < 200)
                return false;

            if (!IsObsidianTacked)
            {
                if (Core.Buffs.HasBuff(SNOPower.P6_Necro_CommandSkeletons))
                {
                    // 如果出现了绝命效忠buff，始终把骷髅保持在身边
                    if (Core.Buffs.HasBuff(SNOPower.P6_Necro_Passive_CheatDeath) && SurviveMode)
                    {
                        needKeepSkeletonsNearby = true;

                        int timecheck = 3000;
                        if (Skills.Necromancer.CommandSkeletons.TimeSinceUse < timecheck)
                            return false;
                    }
                    else if (_MobsIn10 > 0)
                    {
                        needKeepSkeletonsNearby = true;

                        int timecheck = 3000;
                        if (Skills.Necromancer.CommandSkeletons.TimeSinceUse < timecheck)
                            return false;
                    }
                    else 
                    {
                        //冰骷髅3秒一次，其他骷髅10秒一次
                        int timecheck = Runes.Necromancer.FreezingGrasp.IsActive?3000:15000;
                        // 骷髅法师不足时，适当延长号令骸骨的时间
                        int timecheck2 = Runes.Necromancer.FreezingGrasp.IsActive?5000:20000;

                        if (Skills.Necromancer.CommandSkeletons.TimeSinceUse < timecheck)
                            return false;

                        // 法师数量不够时，优先召唤法师
                        if (_MagCount < MinMageCnt && Skills.Necromancer.CommandSkeletons.TimeSinceUse < timecheck2)
                            return false;
                    }
                }
                else 
                {
                    if (IsDebugSkill)
                        Core.Logger.Warn($"遭遇战，施放号令骸骨！");
                }
            }
            else 
            {
                if (Skills.Necromancer.CommandSkeletons.TimeSinceUse < CastCSInterval*100)
                    return false;
            }

            if (needKeepSkeletonsNearby)
                target = _ClosestTargetIn60Unsafe;
            else
                target = _BestTargetIn60_unsafe;

            if (!IsValidTarget(target))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"找不到有效的目标来放号令骸骨！");
                return false;
            }

            lastCSCast = MyGetTickCount();
            return true;
        } 

        bool IsCommandSkeletonsBuffFirst()
        {
            if (!Sets.JessethArms.IsFullyEquipped)
                return false;

            if (!Skills.Necromancer.CommandSkeletons.IsActive)
                return false;

            return !Core.Buffs.HasBuff(SNOPower.P6_Necro_CommandSkeletons);
        }

        private bool ShouldDecrepify(out Vector3 position)
        {
            position = Vector3.Zero;
            TrinityActor target = null;
            if (!Skills.Necromancer.Decrepify.CanCast())
                return false;

            if (Player.PrimaryResource < 10)
                return false;

            bool hasCurseBuff = AnyTargetWithDebuff(65f, SNOPower.P6_Necro_PassiveManager_Decrepify);
            
            if (hasCurseBuff)
            {
                int intevalForCurse = CurseSkillInterval*1000;

                if (_MagCount < MinMageCnt)
                    return false;

                if (IsCommandSkeletonsBuffFirst())
                    return false;

                if (IsBoneArmorBuffFirst())
                    return false;

                if (Skills.Necromancer.Decrepify.TimeSinceUse < intevalForCurse) 
                    return false;
            }

            target = BestTargetWithoutDebuff(60f, SNOPower.P6_Necro_PassiveManager_Decrepify);
            if  (!IsValidTarget(target))
                return false;

            position = target.Position;
            return true;
        }

        private bool ShouldFrailty(out Vector3 position)
        {
            position = Vector3.Zero;
            TrinityActor target = null;
            if (!Skills.Necromancer.Frailty.CanCast())
                return false;

            if (Player.PrimaryResource < 10)
                return false;

            bool hasCurseBuff = AnyTargetWithDebuff(65f, SNOPower.P6_Necro_PassiveManager_Frailty);
            
            if (hasCurseBuff)
            {
                int intevalForCurse = CurseSkillInterval*1000;

                if (_MagCount < MinMageCnt)
                    return false;

                if (IsCommandSkeletonsBuffFirst())
                    return false;

                if (IsBoneArmorBuffFirst())
                    return false;

                if (Skills.Necromancer.Frailty.TimeSinceUse < intevalForCurse) 
                    return false;
            }

            target = BestTargetWithoutDebuff(60f, SNOPower.P6_Necro_PassiveManager_Frailty);
            if  (!IsValidTarget(target))
                return false;

            position = target.Position;
            return true;
        }

        private bool ShouldLeech(out Vector3 position)
        {
            position = Vector3.Zero;
            TrinityActor target = null;
            if (!Skills.Necromancer.Leech.CanCast())
                return false;

            if (Player.PrimaryResource < 10)
                return false;

            bool hasCurseBuff = AnyTargetWithDebuff(65f, SNOPower.P6_Necro_PassiveManager_Leech);
            
            if (hasCurseBuff)
            {
                int intevalForCurse = CurseSkillInterval*1000;

                if (_MagCount < MinMageCnt)
                    return false;

                if (IsCommandSkeletonsBuffFirst())
                    return false;

                if (IsBoneArmorBuffFirst())
                    return false;

                if (Skills.Necromancer.Leech.TimeSinceUse < intevalForCurse) 
                    return false;
            }

            target = BestTargetWithoutDebuff(60f, SNOPower.P6_Necro_PassiveManager_Leech);
            if  (!IsValidTarget(target))
                return false;

            position = target.Position;
            return true;
        }

        protected override bool ShouldBoneSpikes(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.BoneSpikes.CanCast())
                return false;

            target = _BestClusterIn60??_ClosestTargetDanger;
            return IsValidTarget(target);
        }

        protected override bool ShouldSiphonBlood(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.SiphonBlood.CanCast())
                return false;

            target = _ClosestTargetIn60Unsafe;
            return IsValidTarget(target);
        }

        protected override bool ShouldGrimScythe(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.GrimScythe.CanCast())
                return false;

            target = _ClosestTargetIn60Unsafe??_ClosestTargetDanger;
            return IsValidTarget(target);
        }

        private bool ShouldSkeletalMage(out TrinityPower power)
        {    
            power = null;
            TrinityActor target = null;
            
            if (!Skills.Necromancer.SkeletalMage.CanCast())
                return false;

            if (IsDebugParty)
                Core.Logger.Warn($"当前精魂{Player.PrimaryResource}，法师数量为{_MagCount}, 设置为{MinMageCnt}，队伍为{ZetaDia.Service.Party.NumPartyMembers}，队长{PartyLeader == ZetaDia.Me}");

            if (Player.PrimaryResource < 40)
            {
                return false;
            }

            int timeForSimu = (Legendary.HauntedVisions.IsEquipped || Legendary.HauntedVisions.IsEquippedInCube)?30:15;
            if (UseMageOnlyInSimulacrum && Skills.Necromancer.Simulacrum.TimeSinceUse > timeForSimu*1000 - 200)
            {
                return false;
            }

            // 对着精英放魂法。魂法可以无视障碍。
            target = _BestTargetIn60_unsafe;
            if (!IsValidTarget(target))
            {
                if (IsDebugParty)
                    Core.Logger.Warn($"找不到有效的目标来放骷髅法师！");
                power = SkeletalMage(Player.Position);
            }
            else 
            {
                if (IsDebugParty)
                    Core.Logger.Warn($"找到有效的目标，准备放骷髅法师！");
                power = MySkeletalMage(target);
            }

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
            {
                if (IsDebugParty)
                    Core.Logger.Warn($"在死地中，只招满魂骷髅法师！");
                // if (Skills.Necromancer.SkeletalMage.TimeSinceUse > 2000)
                //     return true;

                // 如果带了死地，肯定是单人模式，这里不用管黄道戒指的问题
                if (_MagCount >= 10 && Skills.Necromancer.SkeletalMage.TimeSinceUse < 2000)
                    return false;
                // 在亡者领域期间，召唤高精魂法师
                return Player.PrimaryResourcePct > 0.90;
            }

            // 非战斗状态，只有精魂快满的时候才招
            // 此时回能只能靠吃球
            if (!IsInCombat)
            {
                float pct = 0.71f;
                if (_MagCount >= MinMageCnt && CastMagesPct > pct)
                {
                    pct = CastMagesPct;
                }

                return Player.PrimaryResourcePct > pct;
            }

            // 数量少于最小法师数设置，强制召唤
            if (!IsPartyGame && _MagCount < MinMageCnt)
            {
                return true;
            }

            if (IsDebugParty)
                Core.Logger.Warn($"当前精魂{Player.PrimaryResourcePct}, 设置为{CastMagesPct}");
            
            if (Player.PrimaryResourcePct < CastMagesPct)
                return false;

            // 不管单人还是组队，只要不满就开始招
            if (_MagCount < 10)
                return true;

            // 非黄道组队模式立刻召唤法师
            if (IsPartyGame && !IsObsidianTacked)
                return true;

            if (IsObsidianTacked)
                return Player.PrimaryResourcePct > 0.9 && Skills.Necromancer.SkeletalMage.TimeSinceUse > MageIntervalWhenFull*100;
            else
                return Player.PrimaryResourcePct > 0.9;
        }

        protected override bool ShouldBoneSpear(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.BoneSpear.CanCast())
                return false;

            if (Player.PrimaryResource < 20)
                return false;

            target = _BestClusterIn60;
            return IsValidTarget(target);
        }

        #endregion

        #region Settings      

        bool isChasing = false;
        double lastCheckFollow = 0;
        double checkFollowInterval => 2000;
        TrinityPlayer teamMad = null;

        private IEnumerable<TrinityPlayer> Players => Core.Actors.Actors.OfType<TrinityPlayer>();

        TrinityPlayer MyLeader()
        {
            TrinityPlayer leader = null;

            foreach (TrinityPlayer u in Players)
            {
                if (!u.IsMe)// && u.IsLeader)//u.ActorClass == ActorClass.Necromancer)
                {
                    if (IsDebugParty)
                        Core.Logger.Warn($"输出死灵位置{u.Position}, 距离我{u.Position.Distance(Player.Position)}, 类型{u.ActorClass}");
                    if (leader == null)
                        leader = u;
                    else 
                    {
                        if (u.Position.Distance(Player.Position) < leader.Position.Distance(Player.Position))
                            leader = u;
                    }
                    break;
                }
            }

            return leader;
        }

        void CheckFollow()
        {
            if (!AvoidLost)
                return;

            if (PartyLeader == ZetaDia.Me)
                return;

            double currentTick = MyGetTickCount();

            if (currentTick - lastCheckFollow < checkFollowInterval)
                return;

            TrinityPlayer leader = MyLeader();
            teamMad = leader;

            if (leader == null)
            {
                isChasing = true;
                if (IsDebugParty)
                    Core.Logger.Warn($"附近没有队友，我肯定被抛弃了");
                return;
            }

            if (IsDebugParty)
                Core.Logger.Warn($"输出地图{leader.WorldDynamicId}, 我的地图{Core.Player.WorldDynamicId}，与输出距离{leader.Position.Distance(Player.Position)}");

            lastCheckFollow = currentTick;

            if (isChasing)
            {
                if (leader.WorldDynamicId != Core.Player.WorldDynamicId)
                    return;
                
                if (leader.Position.Distance(Player.Position) > 60f)
                    return;

                if (IsDebugParty)
                    Core.Logger.Warn($"追踪模式解除");
                isChasing = false;
            }
            else 
            {
                if (
                    leader.WorldDynamicId != Core.Player.WorldDynamicId ||
                    leader.Position.Distance(Player.Position) > 75f)
                {
                    if (IsDebugParty)
                        Core.Logger.Warn($"追踪模式开启");
                    isChasing = true;
                }
            }
        }

        // public override int ClusterSize => Settings.ClusterSize;
        public override int ClusterSize
    	{
			get
			{
                if (AvoidLost)
                {
                    CheckFollow();
                    if (IsDebugParty)
                        Core.Logger.Warn($"当前追踪状态{isChasing}");

                    if (isChasing)
                    {
                        Core.Settings.Weighting.EliteWeighting = SettingMode.Disabled;
                        return 60;
                    }
                    else 
                    {
                        Core.Settings.Weighting.EliteWeighting = SettingMode.Enabled;
                    }
                }

                if (Core.Rift.IsNephalemRift)
                {
                    if (IsBlocked && !IsPartyGame)
                    {
                        return 1;
                    }
                    else 
                    {
                        return Settings.ClusterSizeN;
                    }
                }
                else
                {  
                    if (Core.Buffs.HasInvulnerableShrine)
                    {
                        return Settings.ClusterSize;
                    }
                    else if (IsBlocked && !IsPartyGame)
                    {
                        return 1;
                    }
                    else if (CautiousMode && hasDanger)
                    {
                        return 1;
                    }
                    else if (MyGetTickCount() - lastAvoidTick < avoidSafeInterval)
                    {
                        return 1;
                    }
                    else if (Core.Buffs.HasBuff(SNOPower.P6_Necro_Passive_CheatDeath))
                    {
                        return 1;
                    }
                    else 
                    {   
                        return Settings.ClusterSize;
                    }
                }
			}
    	}
        public override float ClusterRadius => (float)Settings.ClusterRadius;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public override float ShrineRange => 70f;
        public override float TrashRange => 70f;
        public override float EliteRange => 70f;

        public bool RubbishPC => Settings.RubbishPC;

        public bool IgnoreAvoidanceInG => Settings.IgnoreAvoidanceInG;
        public bool IgnoreAvoidanceInN => Settings.IgnoreAvoidanceInN;
        public float CastMagesPct => Settings.CastMagesPct;

        public bool FuriousInLOTD => Settings.FuriousInLOTD;
        public int DurationInLOTD => Settings.DurationInLOTD;
        public bool FightingRevengeMode => Settings.FightingRevengeMode;
        public bool CautiousMode
        {
            get 
            {
                // if (SurviveMode)
                //     return true;
                // else
                    return Settings.CautiousMode;
            }
        }
        public bool SurviveMode => Settings.SurviveMode;
        public bool UseIntelliAvoider => Settings.UseIntelliAvoider;
        public bool AvoidLost => Settings.AvoidLost;
        public bool CastMageAnytime => Settings.CastMageAnytime;
        public bool CastMageAnytimeN => Settings.CastMageAnytimeN;
        public bool UseMageOnlyInSimulacrum => Settings.UseMageOnlyInSimulacrum;

        public BigUsage TwoBigUsage => Settings.TwoBigUsage;
        public float ValidElitHealthPct => Settings.ValidElitHealthPct;
        public bool DontUseBigToWeakElite => Settings.DontUseBigToWeakElite;
        public bool DontUseBigToMinion => Settings.DontUseBigToMinion;

        // public int CastDistance => Settings.CastDistance;
        public int CastDistance
        {
            get
            {
                if (Core.Buffs.HasInvulnerableShrine)
                {
                    return Settings.CastDistance;
                }
                else if (CautiousMode && hasDanger)
                {
                    return 60;
                }
                else if (MyGetTickCount() - lastAvoidTick < avoidSafeInterval)
                {
                    return 60;
                }
                else if (Core.Buffs.HasBuff(SNOPower.P6_Necro_Passive_CheatDeath) && SurviveMode)
                {
                    return 60;
                }
                else 
                {   
                    return Settings.CastDistance;
                }
            }
        }
        public int CastDistanceWhenNephalemRift => Settings.CastDistanceWhenNephalemRift;
        public int CastDistanceWhenLandOfDeath => Settings.CastDistanceWhenLandOfDeath;
        public bool ForbidSimulacrumInNR => Settings.ForbidSimulacrumInNR;
        public bool StillMode => Settings.StillMode;

        public int CastCSInterval => Settings.CastCSInterval;

        //public int MinMageCnt => Settings.MinMageCnt;
        public int MinMageCnt
        {
            get
			{
				if (Core.Rift.IsNephalemRift)
				{
					return 10;
				}
                else if (Core.Buffs.HasBuff(SNOPower.P6_Necro_Passive_CheatDeath) && SurviveMode)
                {
                    return 10;
                }
				else
				{  
                    return Settings.MinMageCnt;
				}
			}
        }
        public int MageIntervalWhenFull => Settings.MageIntervalWhenFull;
        public float CastSimulacrumHealthPct => Settings.CastSimulacrumHealthPct;

        public int CurseSkillInterval => Settings.CurseSkillInterval;

        // 血步选项
        public bool BloodRushRescure => Settings.BloodRushRescure;
        public float BloodRushRescureHealthPct => Settings.BloodRushRescureHealthPct;
        public int BloodRushSafeRange => Settings.BloodRushSafeRange;
        public bool BloodRushAvoidElite => Settings.BloodRushAvoidElite;

        public bool AutoChangeSkill => Settings.AutoChangeSkill;
        public bool BoneSpike2SiphonBlood => Settings.BoneSpike2SiphonBlood;
        public bool BoneArmor2BloodRush => Settings.BoneArmor2BloodRush;
        public bool HasBloodRushInBD => Settings.HasBloodRushInBD;

        public bool Simulacrum2BloodRush => Settings.Simulacrum2BloodRush;
        public bool LOTD2BloodRush => Settings.LOTD2BloodRush;
        public bool Decrepify2BloodRush => Settings.Decrepify2BloodRush;
        
        public bool ForbidFindHealthGlobe => Settings.ForbidFindHealthGlobe;
        public float HealthPctForFindHealthGlobe => Settings.HealthPctForFindHealthGlobe;
        public float ResourcePctForFindHealthGlobe => Settings.ResourcePctForFindHealthGlobe;

        // public bool GoOcculusByFoot => Settings.GoOcculusByFoot;
        public bool GoOcculusByFoot
        {
            get
            {
                // if (Skills.Necromancer.BloodRush.IsActive)
                //     return false;

                return Settings.GoOcculusByFoot;;
            }
        }
        public int GoOcculusByFootDistance => Settings.GoOcculusByFootDistance;

        private bool ForceUseShrine => Settings.ForceUseShrine;
        private bool ForceAttackOrdnanceTower => Settings.ForceAttackOrdnanceTower;

        private bool IsDebugMove => Settings.IsDebugMove;
        private bool IsDebugSkill => Settings.IsDebugSkill;
        private bool IsDebugAvoid => Settings.IsDebugAvoid;
        private bool IsDebugTB => Settings.IsDebugTB;
        private bool IsDebugParty => Settings.IsDebugParty;


        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public NEC_RathmaCDRSimulacrum4Settings Settings { get; } = new NEC_RathmaCDRSimulacrum4Settings();

        public enum BigUsage
        {
            None = 0,
            AlwaysSimu_AlwaysLOTD,
            EliteSimu_EliteLOTD,
            EliteSimu_AlwaysLOTD,
            Always_Simu_LOTD_Loop,
            Elite_Simu_LOTD_Loop,

            AlwaysSimu_EliteLOTD
        }

        public sealed class NEC_RathmaCDRSimulacrum4Settings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private int _clusterSizeN;
            private int _clusterRadius;
            private float _emergencyHealthPct;
            private bool _rubbishPC;

            private bool _ignoreAvoidanceInG;
            private bool _ignoreAvoidanceInN;
            private float _castMagesPct;

            private bool _furiousInLOTD;
            private int _durationInLOTD;
            private bool _fightingRevengeMode;
            private bool _cautiousMode;
            private bool _surviveMode;
            private bool _useIntelliAvoider;
            private bool _avoidLost;
            private bool _castMageAnytime;
            private bool _castMageAnytimeN;
            private bool _useMageOnlyInSimulacrum;

            private int _castDistance;
            private int _castDistanceWhenLandOfDeath;
            private int _castDistanceWhenNephalemRift;
            private bool _forbidSimulacrumInNR;
            private bool _stillMode;

            private int _castCSInterval;

            private int _minMageCnt;
            private int _mageIntervalWhenFull;
            private float _castSimulacrumHealthPct;
            
            private int _curseSkillInterval;

            //血步选项
            private bool _bloodRushRescure;
            private float _bloodRushRescureHealthPct;
            private int _bloodRushSafeRange;
            private bool _bloodRushAvoidElite;

            private bool _autoChangeSkill;
            private bool _boneSpike2SiphonBlood;
            private bool _boneArmor2BloodRush;
            private bool _hasBloodRushInBD;
            private bool _simulacrum2BloodRush;
            private bool _LOTD2BloodRush;
            private bool _decrepify2BloodRush;

            private bool _forbidFindHealthGlobe;
            private float _healthPctForFindHealthGlobe;
            private float _resourcePctForFindHealthGlobe;

            private BigUsage _twoBigUsage;
            private float _validElitHealthPct;
            private bool _dontUseBigToWeakElite;
            private bool _dontUseBigToMinion;

            private bool _goOcculusByFoot;
            private int _goOcculusByFootDistance;

            private bool _forceUseShrine;
            private bool _forceAttackOrdnanceTower;

            private bool _isDebugMove;
            private bool _isDebugSkill;
            private bool _isDebugAvoid;
            private bool _isDebugTB;
            private bool _isDebugParty;


            [DefaultValue(6)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(6)]
            public int ClusterSizeN
            {
                get { return _clusterSizeN; }
                set { SetField(ref _clusterSizeN, value); }
            }

            [DefaultValue(25)]
            public int ClusterRadius
            {
                get { return _clusterRadius; }
                set { SetField(ref _clusterRadius, value); }
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            [DefaultValue(false)]
            public bool RubbishPC
            {
                get { return _rubbishPC; }
                set { SetField(ref _rubbishPC, value); }
            }


            [DefaultValue(false)]
            public bool IgnoreAvoidanceInG
            {
                get { return _ignoreAvoidanceInG; }
                set { SetField(ref _ignoreAvoidanceInG, value); }
            }

            [DefaultValue(false)]
            public bool IgnoreAvoidanceInN
            {
                get { return _ignoreAvoidanceInN; }
                set { SetField(ref _ignoreAvoidanceInN, value); }
            }

            [DefaultValue(0.4f)]
            public float CastMagesPct
            {
                get { return _castMagesPct; }
                set { SetField(ref _castMagesPct, value); }
            }

            [DefaultValue(8)]
            public int MinMageCnt
            {
                get { return _minMageCnt; }
                set { SetField(ref _minMageCnt, value); }
            }

            [DefaultValue(20)]
            public int MageIntervalWhenFull
            {
                get { return _mageIntervalWhenFull; }
                set { SetField(ref _mageIntervalWhenFull, value); }
            }

            [DefaultValue(BigUsage.Elite_Simu_LOTD_Loop)]
            public BigUsage TwoBigUsage
            {
                get { return _twoBigUsage; }
                set { SetField(ref _twoBigUsage, value); }
            }

            [DefaultValue(45)]
            public int CastDistance
            {
                get { return _castDistance; }
                set { SetField(ref _castDistance, value); }
            }

            [DefaultValue(35)]
            public int CastDistanceWhenLandOfDeath
            {
                get { return _castDistanceWhenLandOfDeath; }
                set { SetField(ref _castDistanceWhenLandOfDeath, value); }
            }

            [DefaultValue(15)]
            public int CastDistanceWhenNephalemRift
            {
                get { return _castDistanceWhenNephalemRift; }
                set { SetField(ref _castDistanceWhenNephalemRift, value); }
            }

            [DefaultValue(false)]
            public bool ForbidSimulacrumInNR
            {
                get { return _forbidSimulacrumInNR; }
                set { SetField(ref _forbidSimulacrumInNR, value); }
            }

            [DefaultValue(false)]
            public bool StillMode
            {
                get { return _stillMode; }
                set { SetField(ref _stillMode, value); }
            }

            [DefaultValue(30)]
            public int CastCSInterval
            {
                get { return _castCSInterval; }
                set { SetField(ref _castCSInterval, value); }
            }

            [DefaultValue(false)]
            public bool FuriousInLOTD
            {
                get { return _furiousInLOTD; }
                set { SetField(ref _furiousInLOTD, value); }
            }

            [DefaultValue(0)]
            public int DurationInLOTD
            {
                get { return _durationInLOTD; }
                set { SetField(ref _durationInLOTD, value); }
            }

            [DefaultValue(false)]
            public bool FightingRevengeMode
            {
                get { return _fightingRevengeMode; }
                set { SetField(ref _fightingRevengeMode, value); }
            }

            [DefaultValue(false)]
            public bool CautiousMode
            {
                get { return _cautiousMode; }
                set { SetField(ref _cautiousMode, value); }
            }

            [DefaultValue(false)]
            public bool SurviveMode
            {
                get { return _surviveMode; }
                set { SetField(ref _surviveMode, value); }
            }

            [DefaultValue(false)]
            public bool UseIntelliAvoider
            {
                get { return _useIntelliAvoider; }
                set { SetField(ref _useIntelliAvoider, value); }
            }

            [DefaultValue(false)]
            public bool AvoidLost
            {
                get { return _avoidLost; }
                set { SetField(ref _avoidLost, value); }
            }

            [DefaultValue(false)]
            public bool CastMageAnytime
            {
                get { return _castMageAnytime; }
                set { SetField(ref _castMageAnytime, value); }
            }

            [DefaultValue(false)]
            public bool CastMageAnytimeN
            {
                get { return _castMageAnytimeN; }
                set { SetField(ref _castMageAnytimeN, value); }
            }

            [DefaultValue(false)]
            public bool UseMageOnlyInSimulacrum
            {
                get { return _useMageOnlyInSimulacrum; }
                set { SetField(ref _useMageOnlyInSimulacrum, value); }
            }
            
            [DefaultValue(0.5f)]
            public float CastSimulacrumHealthPct
            {
                get { return _castSimulacrumHealthPct; }
                set { SetField(ref _castSimulacrumHealthPct, value); }
            }

            [DefaultValue(8)]
            public int CurseSkillInterval
            {
                get { return _curseSkillInterval; }
                set { SetField(ref _curseSkillInterval, value); }
            }


            // 血步选项
            // 此选项不开放
            [DefaultValue(true)]
            public bool BloodRushRescure
            {
                get { return _bloodRushRescure; }
                set { SetField(ref _bloodRushRescure, value); }
            }

            [DefaultValue(0.5f)]
            public float BloodRushRescureHealthPct
            {
                get { return _bloodRushRescureHealthPct; }
                set { SetField(ref _bloodRushRescureHealthPct, value); }
            }

            [DefaultValue(25)]
            public int BloodRushSafeRange
            {
                get { return _bloodRushSafeRange; }
                set { SetField(ref _bloodRushSafeRange, value); }
            }

            // 此选项不开放
            [DefaultValue(false)]
            public bool BloodRushAvoidElite
            {
                get { return _bloodRushAvoidElite; }
                set { SetField(ref _bloodRushAvoidElite, value); }
            }

            // 换装选项

            [DefaultValue(false)]
            public bool AutoChangeSkill
            {
                get { return _autoChangeSkill; }
                set { SetField(ref _autoChangeSkill, value); }
            }
            
            [DefaultValue(false)]
            public bool HasBloodRushInBD
            {
                get { return _hasBloodRushInBD; }
                set { SetField(ref _hasBloodRushInBD, value); }
            }

            [DefaultValue(false)]
            public bool BoneSpike2SiphonBlood
            {
                get { return _boneSpike2SiphonBlood; }
                set { SetField(ref _boneSpike2SiphonBlood, value); }
            }
            
            [DefaultValue(false)]
            public bool BoneArmor2BloodRush
            {
                get { return _boneArmor2BloodRush; }
                set { SetField(ref _boneArmor2BloodRush, value); }
            }

            [DefaultValue(false)]
            public bool Simulacrum2BloodRush
            {
                get { return _simulacrum2BloodRush; }
                set { SetField(ref _simulacrum2BloodRush, value); }
            }

            [DefaultValue(false)]
            public bool LOTD2BloodRush
            {
                get { return _LOTD2BloodRush; }
                set { SetField(ref _LOTD2BloodRush, value); }
            }

            [DefaultValue(false)]
            public bool Decrepify2BloodRush
            {
                get { return _decrepify2BloodRush; }
                set { SetField(ref _decrepify2BloodRush, value); }
            }

            [DefaultValue(false)]
            public bool ForbidFindHealthGlobe
            {
                get { return _forbidFindHealthGlobe; }
                set { SetField(ref _forbidFindHealthGlobe, value); }
            }

            [DefaultValue(0.8f)]
            public float HealthPctForFindHealthGlobe
            {
                get { return _healthPctForFindHealthGlobe; }
                set { SetField(ref _healthPctForFindHealthGlobe, value); }
            }

            [DefaultValue(0.7f)]
            public float ResourcePctForFindHealthGlobe
            {
                get { return _resourcePctForFindHealthGlobe; }
                set { SetField(ref _resourcePctForFindHealthGlobe, value); }
            }

            [DefaultValue(false)]
            public bool DontUseBigToWeakElite
            {
                get { return _dontUseBigToWeakElite; }
                set { SetField(ref _dontUseBigToWeakElite, value); }
            }
            
            [DefaultValue(false)]
            public bool DontUseBigToMinion
            {
                get { return _dontUseBigToMinion; }
                set { SetField(ref _dontUseBigToMinion, value); }
            }

            [DefaultValue(0.2f)]
            public float ValidElitHealthPct
            {
                get { return _validElitHealthPct; }
                set { SetField(ref _validElitHealthPct, value); }
            }

            [DefaultValue(false)]
            public bool GoOcculusByFoot
            {
                get { return _goOcculusByFoot; }
                set { SetField(ref _goOcculusByFoot, value); }
            }

            [DefaultValue(25)]
            public int GoOcculusByFootDistance
            {
                get { return _goOcculusByFootDistance; }
                set { SetField(ref _goOcculusByFootDistance, value); }
            }
            
            [DefaultValue(false)]
            public bool ForceUseShrine
            {
                get { return _forceUseShrine; }
                set { SetField(ref _forceUseShrine, value); }
            }

            [DefaultValue(false)]
            public bool ForceAttackOrdnanceTower
            {
                get { return _forceAttackOrdnanceTower; }
                set { SetField(ref _forceAttackOrdnanceTower, value); }
            }

            [DefaultValue(false)]
            public bool IsDebugMove
            {
                get { return _isDebugMove; }
                set { SetField(ref _isDebugMove, value); }
            }

            [DefaultValue(false)]
            public bool IsDebugSkill
            {
                get { return _isDebugSkill; }
                set { SetField(ref _isDebugSkill, value); }
            }

            [DefaultValue(false)]
            public bool IsDebugAvoid
            {
                get { return _isDebugAvoid; }
                set { SetField(ref _isDebugAvoid, value); }
            }

            [DefaultValue(false)]
            public bool IsDebugTB
            {
                get { return _isDebugTB; }
                set { SetField(ref _isDebugTB, value); }
            }

            [DefaultValue(false)]
            public bool IsDebugParty
            {
                get { return _isDebugParty; }
                set { SetField(ref _isDebugParty, value); }
            }

            public override void LoadDefaults()
            {
                base.LoadDefaults();
            }


            //#endregion

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + "_en.xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


