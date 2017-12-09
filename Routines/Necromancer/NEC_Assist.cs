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
using Zeta.Bot;
using Zeta.Bot.Navigation;
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
    public sealed class NEC_Assist : NecroMancerBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Necromancer Pestilence CDR";
        public string Description => "Necromancer Pestilence CDR Routine";
        public string Author => "crazyjay1982";
        public string Version => "1.1.2e";
        public string Url => "https://www.thebuddyforum.com/threads/necromancer-pestilence-cdr-routine-gr-110.413734/";
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
                { Skills.Necromancer.LandOfTheDead, Runes.Necromancer.FrozenLands },
                { Skills.Necromancer.Devour, null },
            },
            // Items Requirements
            Items = new List<Item>
            {
            }
        };

        #endregion

        public bool IsReallyBlocked => IsBlocked || IsStuck;

        public override Func<bool> ShouldIgnoreAvoidance => IgnoreAvoidanceCondition;
        public override Func<bool> ShouldIgnoreNonUnits => IgnoreNonUnitsCondition;

        private static List<TrinityActor> ObjectCache => Core.Targets.Entries;
        private bool IsPartyGame => ZetaDia.Service.Party.NumPartyMembers > 1;

        private TrinityPower MyDecrepify(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Decrepify, 60f, position);

        private TrinityPower MyFrailty(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Frailty, 60f, position);

        private TrinityPower MyLeech(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Leech, 60f, position);

        private double MyGetTickCount()
        {
            TimeSpan current = DateTime.Now - (new DateTime(1970, 1, 1, 0, 0, 0));
            return current.TotalMilliseconds;
        }

        private bool IgnoreAvoidanceCondition()
        {
            if (!Core.Rift.IsGreaterRift)
                return true;
            
            return Core.Buffs.HasInvulnerableShrine;
        }

        private bool IgnoreNonUnitsCondition()
        {
            return false;
        }

        public TrinityPower MyTryCurseSkills()
        {
            Vector3 position = Vector3.Zero;

            if (ShouldDecrepify(out position))
                return MyDecrepify(position);

            if (ShouldFrailty(out position))
                return MyFrailty(position);

            if (ShouldLeech(out position))
                return MyLeech(position);

            return null;
        }

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;
            Vector3 position;

            TryZeroPower();

            if (TryRescureByBloodRush(out power))
                return power;

            if (TryCloseToTarget(out power))
                return power;

            if (TryGoOcculus(out power))
                return power;

            // 先于亡者领域放双分，让双大的重叠范围更大
            if (ShouldSimulacrum(out position))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用血魂双分，目标距离我{position.Distance(Player.Position)}");
                return Simulacrum(position);
            }

            if (ShouldLandOfTheDead(out target))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用亡者领域");
                return LandOfTheDead();
            }

            // 优先诅咒减伤
            power = MyTryCurseSkills();
            if (power != null)
                return power;

            // 亡者大军，我自己也没用过，随便放放
            if (ShouldArmyOfTheDead(out target))
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用亡者大军，目标距离我{target.Distance}");
                return ArmyOfTheDead(target);
            }

            if (ShouldBoneArmor())
            {
                if (IsDebugSkill)
                    Core.Logger.Warn($"使用骨甲");
                return BoneArmor();
            }

            if (ShouldCommandGolem(out target))
                return CommandGolem(target);

            // if (ShouldUseGrimScytheInAdvance())
            // {
            //     if (ShouldGrimScythe(out target))
            //         return GrimScythe(target);
            // }

            if (ShouldCorpseLance(out target))
                return CorpseLance(target);

            if (ShouldBoneSpear(out target))
                return BoneSpear(target);

            if (ShouldDeathNova(out target))
                return DeathNova(target);

            if (ShouldGrimScythe(out target))
                return GrimScythe(target);

            // 骨刺是范围控制法术，应该攻击最密集的怪群
            if (ShouldBoneSpikes(out target))
                return BoneSpikes(target);

            // 使用鲜血虹吸
            if (ShouldSiphonBlood(out target))
                return SiphonBlood(target);

            if (UseBloodRushAsOffensive(out power))
                return power;

            // if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
            // {
            //     return Devour();
            // }

            Vector3 safepot = Vector3.Zero;
            
            if (ShouldStand() || Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
            {
                safepot = Player.Position;
            }
            else
            {
                safepot = SafePotForAvoid();
                if (IsDebugMove)
                    Core.Logger.Warn($"OffensivePower中，使用步行等待CD，目标距离我{safepot.Distance(Player.Position)}");
            }
            return ToDestAny(safepot);
        }

        public TrinityPower GetMiniOffensive()
        {
            TrinityActor target = null;

            if (Skills.Necromancer.LandOfTheDead.CanCast())
                return null;

            if (ShouldBoneSpear(out target))
                return BoneSpear(target);

            if (ShouldDeathNova(out target))
                return DeathNova(target);

            if (ShouldCorpseLance(out target))
                return CorpseLance(target);

            return null;
        }

        public TrinityPower GetDefensivePower()
        {
            return null;//GetBuffPower();
        }
        
        private int worldid = 0;

        public TrinityPower GetBuffPower()
        {
            TrinityActor target;
            TrinityPower power;

            TryChangeSkills();

            if (Player.IsInTown)
                return null;

            if (ZetaDia.Storage.RiftCompleted)
            {
                return null;
            }

            TryZeroPower();

            if (false)
            {
                FindBoss();

                if (Player.WorldSnoId != worldid)
                {
                    worldid = Player.WorldSnoId;

                    if (!hasBoss && (!ZetaDia.Storage.RiftCompleted || !Core.Rift.IsGreaterRift))
                    {
                        if (IgnoreShrine)
                        {
                            Core.Settings.Weighting.ShrineWeighting = SettingMode.Disabled;
                        }
                        else 
                        {
                            Core.Settings.Weighting.ShrineWeighting = SettingMode.Enabled;
                        }
                    }
                }

                if ((ZetaDia.Storage.RiftCompleted && Core.Rift.IsGreaterRift) || hasBoss)
                {
                    Core.Settings.Weighting.ShrineWeighting = SettingMode.Disabled;
                }
            }

            // if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000)
            // {
            //     power = GetMiniOffensive();
            //     if (power != null)
            //         return power;
            // }

            return null;
        }

        public TrinityPower GetDestructiblePower()
        {
            if (CurrentTarget == null)
                return null;

            if (Skills.Necromancer.BoneSpikes.CanCast())
                return BoneSpikes(CurrentTarget);

            if (Skills.Necromancer.BoneSpear.CanCast())
                return BoneSpear(CurrentTarget);

            if (Skills.Necromancer.DeathNova.CanCast())
            {
                return DeathNova(CurrentTarget);
            }

            if (CanCorpseLance())
            {
                return CorpseLance(CurrentTarget);
            }

            return DefaultPower;
        }

        TrinityPower ToDestInPath(Vector3 dest)
        {
            TrinityPower power;

            if (Player.CurrentHealthPct > 0.3 && !IsInCombat)
            {
                if (TryBloodrushMovement(dest, out power))
                    return power;
            }

            if (Skills.Necromancer.BloodRush.CanCast() && Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000 && Runes.Necromancer.Molting.IsActive)
            {
                return BloodRush(dest);
            }

            if (dest.Distance(Player.Position) > 10f)
                return Walk(dest, 10f);
            else
                return Walk(dest);
        }

        TrinityPower ToDestAny(Vector3 dest)
        {
            // TrinityPower power;
            // var distance = dest.Distance(Player.Position);

            // if (CanBloodRush() && (distance > 15f || (IsBlocked && distance > 5f)))
            // {
            //     return BloodRush(dest);
            // }

            if (dest.Distance(Player.Position) > 10f)
                return Walk(dest, 10f);
            else
                return Walk(dest);
        }

        TrinityPower ToDestByWalk(Vector3 dest)
        {
            if (dest.Distance(Player.Position) > 10f && Core.Rift.IsGreaterRift)
                return Walk(dest, 10f);
            else
                return Walk(dest);
        }

        private bool ShouldGoTo(Vector3 destination)
        {
            // if (Core.Buffs.HasInvulnerableShrine)
            //     return true;

            // if (Core.Avoidance.Grid.IsIntersectedByFlags(destination, Core.Player.Position, AvoidanceFlags.CriticalAvoidance))
            //     return false;

            return true;
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            TrinityPower power = null;
            TrinityActor target = null;
            Vector3 position = Vector3.Zero;
            TrinityActor closetManster = ClosestTarget(Player.Position, 60f);
            TrinityPlayer leader = AvoidLost?teamMad:null;

            if (Player.IsInTown)
                return Walk(destination);

            TryZeroPower();

            if (ZetaDia.Storage.RiftCompleted && !Core.Rift.IsNephalemRift)
            {
                return ToDestInPath(destination);
            }

            if (CurrentTarget != null && CurrentTarget.Type == TrinityObjectType.ProgressionGlobe)
            {
                return ToDestInPath(destination);
            }

            CheckFollow();

            if (isChasing && Skills.Necromancer.BloodRush.CanCast() &&
                TargetUtil.NumMobsInRangeOfPosition(Player.Position, 10f) > 0)
            {
                position = SafePotForBloodRush();
                if (position != Vector3.Zero)
                    return BloodRush(position);
            }

            if (TryRescureByBloodRush(out power))
                return power;

            if (TryGoOcculus(out power))
                return power;

            power = MyTryCurseSkills();
            if (power != null)
                return power;

            float distanceFromLeader = leader==null?0:leader.Position.Distance(Player.Position);
            if (
                !isChasing && StillMode && closetManster != null && closetManster.Distance < FightDistance &&
                distanceFromLeader < 40f && 
                !Skills.Necromancer.LandOfTheDead.CanCast())
            {
                power = GetMiniOffensive();
                if (power != null)
                    return power;
            }

            if (IsPartyGame && Core.Rift.IsGreaterRift && AnyCriticalInRange() && IsValidTarget(ClosestTarget(Player.Position)) && !Core.Avoidance.InAvoidance(Player.Position))
            {
                Core.Logger.Warn($"有可能在熔火附近鬼畜，强制攻击");
                power = GetMiniOffensive();
                if (power != null)
                    return power;
            }

            if (ShouldStand())
            {
                power = GetMiniOffensive();
                if (power != null)
                    return power;
            }

            if (IsReallyBlocked)
            {
                if (!TargetUtil.AnyMobsInRange(15f))
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

                            if (Skills.Necromancer.BoneSpikes.CanCast())
                                return BoneSpikes(actor);

                            if (Skills.Necromancer.BoneSpear.CanCast())
                                return BoneSpear(actor);

                            if (Skills.Necromancer.DeathNova.CanCast())
                                return DeathNova(actor);
                        }
                    }
                }
                else 
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"被卡住了，强制攻击");

                    power = GetMiniOffensive();
                    if (power != null)
                        return power;
                }
            }

            if (!ShouldGoTo(destination))
            {
                power = GetMiniOffensive();
                if (power != null)
                    return power;
            }

            if (IsDebugMove)
                Core.Logger.Warn($"按照Trinity的指示，走向{destination}");

            return ToDestInPath(destination);
        }

        #region DecisionHelpers

        private Vector3 bossPos = Vector3.Zero;
        private bool hasBoss = false;
        private float bossDistance = 0;

        private double lastFindBossTick = 0;
        private double findBossInterval = 2000;

        private bool FindBoss()
        {
            if (!IsPartyGame)
                return false;
            
            if (MyGetTickCount() - lastFindBossTick < findBossInterval)
                return hasBoss;
            
            lastFindBossTick = MyGetTickCount();
            
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

        
		private bool __avoider(AvoidanceNode n)
        {
            return !Core.Avoidance.InAvoidance(n.NavigableCenter);
        }

        private bool __avoider_skillonly(AvoidanceNode n)
        {
            return !Core.Avoidance.InAvoidance(n.NavigableCenter) && Core.Grids.CanRayWalk(Player.Position, n.NavigableCenter);
        }

        private int __avoider_dist = 0;
        private int __avoider_num = 0;

        private bool __avoider_generic(AvoidanceNode n)
        {
            return !Core.Avoidance.InAvoidance(n.NavigableCenter) && 
                Core.Grids.CanRayWalk(Player.Position, n.NavigableCenter) &&
                TargetUtil.NumMobsInRangeOfPosition(n.NavigableCenter, __avoider_dist) < __avoider_num;
        }
        
		private Vector3 SafePotForBloodRush()
		{
			Vector3 safePot;

			Func<AvoidanceNode, bool> myf = new Func<AvoidanceNode, bool>(__avoider);

            Vector3 projectedPosition = IsBlocked
                    ? Core.Grids.Avoidance.GetPathCastPosition(50f, true)
                    : Core.Grids.Avoidance.GetPathWalkPosition(50f, true);

			Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 15f, 50f, Player.Position, myf);
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
                    ? Core.Grids.Avoidance.GetPathCastPosition(40f, true)
                    : Core.Grids.Avoidance.GetPathWalkPosition(40f, true);

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

			if (safePot == Vector3.Zero)
			{
				Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 20f, 40f, Player.Position, myf_skill);
			}
            if (safePot == Vector3.Zero)
			{
				Core.Avoidance.Avoider.TryGetSafeSpot(out safePot, 20f, 40f, Player.Position, null);
			}
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

        internal bool TryCloseToTarget(out TrinityPower power)
        {
            power = null;

            // if (!IsPartyGame)
            //     return false;
            if (ShouldStand())
                return false;

            int fightD = Skills.Necromancer.DeathNova.IsActive?20:FightDistance;
            TrinityPlayer leader = AvoidLost?teamMad:null;
            TrinityActor closestMonsterFromLeader = ClosestTarget(leader==null?Player.Position:leader.Position);
            TrinityActor tagetToClose = null;
            float distFromLeader = leader==null?0:leader.Position.Distance(ZetaDia.Me.Position);

            if (IsDebugMove)
                Core.Logger.Warn($"与队长的距离{distFromLeader}");

            bool shouldCloseToLeader = false;
            if (PartyLeader != ZetaDia.Me && leader != null && leader.WorldDynamicId == Core.Player.WorldDynamicId)
            {
                if (distFromLeader > 60f && distFromLeader < 120f && 
                    closestMonsterFromLeader != null && closestMonsterFromLeader.Position.Distance(leader.Position) < fightD)
                {
                    if (IsDebugParty)
                        Core.Logger.Warn($"离队长太远了，准备靠近");
                    shouldCloseToLeader = true;
                }
            }

            if (!shouldCloseToLeader && TargetUtil.AnyMobsInRange(fightD))
                return false;

            TrinityActor target = closestMonsterFromLeader??ClosestTarget(Player.Position)??CurrentTarget;
            if (!IsValidTarget(target))
                return false;

            if (IsDebugMove)
                Core.Logger.Warn($"TryCloseToTarget，目标距离我{target.Distance}");

            power = ToDestAny(target.Position); // TODO

            return true;
        }

        internal bool GetSkillInfo(SNOPower snoPower, out int slot, out int runeIndex)
		{
			bool isOverrideActive = false;
            try
            {
                isOverrideActive = ZetaDia.Me.SkillOverrideActive;
            }
            catch (ArgumentException ex)
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

        internal double lastChangeTime = 0;
        internal int timeForSkillChange = 500;

        internal void TryChangeSkills()
        {
            if (!Settings.AutoChangeSkill)
                return;

            // 鲜血奔行
            int slotOfBloodRush = 0;
            int runeIndexOfBloodRush = 0;
            bool hasBloodRush = false;

            // 在城里
            if (Player.IsInTown && !(Core.Rift.IsGreaterRift && !ZetaDia.Storage.RiftCompleted))
            {
                double lastTime = MyGetTickCount()-lastChangeTime;
                if (lastTime > timeForSkillChange)
                {
                    hasBloodRush = GetSkillInfo(SNOPower.P6_Necro_BloodRush, out slotOfBloodRush, out runeIndexOfBloodRush);

                    if (hasBloodRush && runeIndexOfBloodRush != 3)
                    {
                        if (Skills.Necromancer.BloodRush.CanCast())
                        {
                            ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BloodRush.SNOPower, 3, (HotbarSlot)slotOfBloodRush);
                            lastChangeTime = MyGetTickCount();
                            Core.Logger.Warn($"在城镇中，鲜血奔行更换符文->鲜血潜能");
                        }
                    }

                    return;
                }
            }

            // 在小米
            if (!Player.IsInTown && Core.Rift.IsNephalemRift && !ZetaDia.Me.IsInCombat)
            {
                double lastTime = MyGetTickCount();
                if (lastTime > timeForSkillChange)
                {
                    hasBloodRush = GetSkillInfo(SNOPower.P6_Necro_BloodRush, out slotOfBloodRush, out runeIndexOfBloodRush);

                    if (hasBloodRush && runeIndexOfBloodRush != 2)
                    {
                        if (Skills.Necromancer.BloodRush.CanCast())
                        {
                            ZetaDia.Me.SetActiveSkill(Skills.Necromancer.BloodRush.SNOPower, 2, (HotbarSlot)slotOfBloodRush);
                            lastChangeTime = MyGetTickCount();
                            Core.Logger.Warn($"在城镇中，鲜血奔行更换符文-鲜血代谢");
                        }
                    }

                    return;
                }
            }
            
        }

        internal static TrinityActor ClosestBossInRange(float range)
        {
            return (from u in TargetUtil.SafeList(true)
                    where u.IsUnit &&
                          (u.IsBoss || u.ActorSnoId == 360636) && u.IsValid && u.Weight > 0 &&
                          u.IsInLineOfSight &&
                          u.Distance <= range
                    orderby
                          u.Distance
                    select u).FirstOrDefault();
        }

        internal static TrinityActor BestEliteInRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {				
			return (from u in TargetUtil.SafeList(true)
                where u.IsUnit &&
                    // && u.EliteType != EliteTypes.Minion
					((u.IsElite || u.ActorSnoId == 360636)) && u.IsValid && u.Weight > 0 &&
                    !u.IsSafeSpot && u.IsInLineOfSight && u != exclude &&
                    (!canRayWalk || (canRayWalk && Core.Grids.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }
		
		internal static TrinityActor BestTrashInRange(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return (from u in TargetUtil.SafeList(true)
                where u.IsUnit && u.IsValid && u.Weight > 0 && !u.IsSafeSpot && !u.IsElite && 
                    u.IsInLineOfSight && u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    (!canRayWalk || (canRayWalk && Core.Grids.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        internal static TrinityActor ClosestTarget(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return (from u in TargetUtil.SafeList(true)
                where u.IsUnit && u.IsValid && u.Weight > 0 && !u.IsSafeSpot && 
                    u.IsInLineOfSight && u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    (!canRayWalk || (canRayWalk && Core.Grids.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        internal static TrinityActor BestClusterUnit(Vector3 pos, float range = 60f, bool canRayWalk = false, TrinityActor exclude = null)
        {	
			return (from u in TargetUtil.SafeList(true)
                where u.IsUnit && u.IsValid && u.Weight > 0 && !u.IsSafeSpot && 
                    u.IsInLineOfSight && u != exclude &&
                    u.Type != TrinityObjectType.Shrine &&
                    (!canRayWalk || (canRayWalk && Core.Grids.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                orderby
                    u.NearbyUnitsWithinDistance(15f) descending,
					u.Position.Distance(pos),
					u.HitPointsPct descending
                select u).FirstOrDefault();
        }

        internal static bool AnyMobsInRange(Vector3 pos, float range = 60f, bool canRayWalk = true, TrinityActor exclude = null)
        {				
			return (from u in TargetUtil.SafeList(true)
                where u.IsUnit &&
					u.IsValid && u.Weight > 0 &&
                    !u.IsSafeSpot && u != exclude && //u.IsInLineOfSight && 
                    (!canRayWalk || (canRayWalk && Core.Grids.CanRayWalk(pos, u.Position))) &&
					u.Position.Distance(pos) <= range
                select u).Any();
        }

        private static TrinityActor BestTargetWithoutDebuff(float range, SNOPower debuff, Vector3 position = default(Vector3))
        {
            return BestTargetWithoutDebuffs(range, new List<SNOPower> { debuff }, position);
        }


        private static TrinityActor BestTargetWithoutDebuffs(float range, IEnumerable<SNOPower> debuffs, Vector3 position = default(Vector3))
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityActor target;
            var unitsByWeight = (from u in TargetUtil.SafeList(true)
                                 where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range &&
                                        !debuffs.All(u.HasDebuff)
                                 orderby 
                                    u.NearbyUnitsWithinDistance(15f) descending
                                 select u).ToList();

            if (unitsByWeight.Any())
                target = unitsByWeight.FirstOrDefault();
            else
                target = null;

            return target;
        }

        private bool AnyTargetWithDebuff(float range, SNOPower debuff)
        {
            TrinityActor target;
            var units = (from u in TargetUtil.SafeList(true)
                            where u.IsUnit && u.IsValid &&
                                u.Weight > 0 &&
                                u.Distance <= range &&
                                u.HasDebuff(debuff)
                            select u);

            return units.Any();
        }

        private static bool AnyTargetWithDebuffs(float range, IEnumerable<SNOPower> debuffs, Vector3 position = default(Vector3))
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityActor target;
            return (from u in TargetUtil.SafeList(true)
                where u.IsUnit && u.IsValid &&
                    u.Weight > 0 &&
                    u.Position.Distance(position) <= range &&
                    debuffs.Any(u.HasDebuff)
                select u).Any();
        }

        internal static TrinityActor GetClosestUnitUnSafe(float maxDistance = 100f)
        {
            var result =
                (from u in TargetUtil.SafeList(true)
                where 
                    u.IsUnit && u.IsValid && u.Weight > 0 && u.RadiusDistance <= maxDistance &&
                    u.Type != TrinityObjectType.Shrine &&
                    u.Type != TrinityObjectType.ProgressionGlobe &&
                    u.Type != TrinityObjectType.HealthGlobe &&
                    u.ActorSnoId != 454066
                orderby
                    u.Distance
                select u).FirstOrDefault();

            return result;
        }

        private static TrinityActor GetBestTarget(Vector3 pos, float range = 60f, bool canRayWalk = true, TrinityActor exclude = null)
        {
            return BestEliteInRange(pos, range, canRayWalk, exclude)??
                BestTrashInRange(pos, range, canRayWalk, exclude)??
                TargetUtil.GetClosestUnit(range>30?30:range);
        }

        private static TrinityActor GetBestTarget2(Vector3 pos, float range = 60f, bool canRayWalk = true, TrinityActor exclude = null)
        {
            return BestEliteInRange(pos, range, canRayWalk, exclude)??
                BestTrashInRange(pos, range, canRayWalk, exclude);
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
        		
		internal static TrinityActor BestDestructibleInRange(Vector3 pos, float range = 60f, bool canReach = true)
        {	
			return (from u in TargetUtil.SafeList(true)
                where u.IsInLineOfSight &&
                    u.Type == TrinityObjectType.Destructible &&
                    (!canReach || (canReach && Core.Grids.CanRayWalk(pos, u.Position))) &&
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
            bool shouldAvoid = false, bool shouldAvoidCritical = true)
        {
            return (from u in TargetUtil.SafeList()
                where 
                    u?.Type == TrinityObjectType.HealthGlobe && u.Distance < range &&
                    TargetUtil.NumMobsInRangeOfPosition(u.Position, maxDistanceFromMob) > 0 &&
                    Core.Grids.CanRayWalk(Player.Position, u.Position) &&
                    (!combating || (combating && TargetUtil.NumMobsInRangeOfPosition(u.Position, minDistanceFromMob) < ClusterSize)) &&
                    (!shouldAvoid || !Core.Avoidance.InAvoidance(u.Position)) &&
                    (!shouldAvoidCritical || !Core.Avoidance.InCriticalAvoidance(u.Position))
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
                    Core.Grids.CanRayWalk(Player.Position, u.Position) &&
                    (!combating || (combating && TargetUtil.NumMobsInRangeOfPosition(u.Position, minDistanceFromMob) < ClusterSize)) &&
                    (!shouldAvoid || !Core.Avoidance.InAvoidance(u.Position)) &&
                    (!shouldAvoidCritical || !Core.Avoidance.InCriticalAvoidance(u.Position))
                orderby
                    NumHealthGlobesInPosOfRange(u.Position, 10f) descending
                select u).FirstOrDefault();
        }

        private bool GetOculusPosition(out Vector3 position, float range, Vector3 fromLocation)
        {
            position = Vector3.Zero;

            TrinityActor actor = 
                (from u in TargetUtil.SafeList(false)
                 where fromLocation.Distance2D(u.Position) <= range &&
                       u.ActorSnoId == 433966
                 orderby u.Distance
                 select u).ToList().FirstOrDefault();

            if (actor == null)
                return false;

            position = actor.Position;
            position.Z = Player.Position.Z; // Z坐标修正，他妈的你神目为什么出现在天上？

            return true;
        }

        private static bool IsValidTarget(TrinityActor target)
        {
            if (target == null)
                return false;

            if (!(target.IsUnit && target.IsValid && target.Weight > 0))
                return false;
            
            return target.IsBoss || target.IsElite || target.IsTrashMob || target.IsTreasureGoblin;
        }

        internal bool CanBloodRush(bool forRescure = false)
        {
            if (!Skills.Necromancer.BloodRush.CanCast())
                return false;

            if (forRescure && Player.CurrentHealthPct > 0.11)
                return true;
            
            if (Player.CurrentHealthPct < 0.3)
                return false;

            // if (Skills.Necromancer.BloodRush.CooldownRemaining > 0 && !HasInfiniteCasting)
            //     return false;

            return true;
        }

        private bool IsInOcculus()
        {
            Vector3 occulusPos;
            GetOculusPosition(out occulusPos, 60f, Player.Position);
            if (occulusPos == Vector3.Zero)
                return false;
            
            return occulusPos.Distance(Player.Position) < OcculusRadius;
        }

        private bool IsSafePosition(Vector3 position)
        {
            if (Core.Buffs.HasInvulnerableShrine)
                return true;

            if (Core.Avoidance.InAvoidance(position))
                return false;

            return true;
        }


        private int OcculusRadius => 8;
        private int OcculusInterval => 5000; // 10秒踩一次
        private double LastOcculusTick = 0f;
        private bool TryGoOcculus(out TrinityPower power)
        {
            power = null;

            if (!UseOcculus)
                return false;

            // 小米不踩神目
            if (!Core.Rift.IsGreaterRift)
                return false;

            if (!IsInCombat)
                return false;

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000)
                return false;

            if (IsPartyGame && MyGetTickCount() - LastOcculusTick < OcculusInterval)
                return false;
            
            // bool canBloodRush = CanBloodRush();

            // if (!canBloodRush)
            //     return false;

            // if (IsPartyGame)
            TrinityPlayer nearestParty = MyLeader();
            if (nearestParty == null && IsPartyGame)
                return false;
            
            Vector3 occulusPos = Vector3.Zero;
            bool ret = GetOculusPosition(out occulusPos, 60f, Player.Position);
            if (!ret || occulusPos == Vector3.Zero)
                return false;

            float distance = occulusPos.Distance(Player.Position);
            
            if (IsDebugMove)
                Core.Logger.Warn($"发现神目，距离我{distance}");

            // 只要50码内有怪，我就敢去踩。输出也会判断同样的状态
            TrinityActor target = ClosestTarget(occulusPos, 35f);
            if (!IsValidTarget(target))
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"神目周围35码内没有可以攻击的怪物，放弃踩神目");
                return false;
            }

            // if (nearestParty.Position.Distance(occulusPos) > 58f)
            // {
            //     if (IsDebugMove)
            //         Core.Logger.Warn($"神目离队友太远，放弃神目");
            //     return false;
            // }

            if (distance < OcculusRadius)
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"已经在神目中，无需移动");
                return false;
            }
            else if (CanBloodRush())
            {
                if (IsDebugMove)
                    Core.Logger.Warn($"飞向神目");
                power = BloodRush(occulusPos);
            }
            else if (distance < 30f && !IsPartyGame)
            {
                Vector3 closePos = MathEx.CalculatePointFrom(occulusPos, Player.Position, 10f);
                if (Core.Grids.CanRayWalk(Player.Position, closePos))
                {
                    if (IsDebugMove)
                        Core.Logger.Warn($"走向神目");
                    power = Walk(occulusPos, 10f);
                }
            }
            else
            {
            }

            LastOcculusTick = MyGetTickCount();
            return true;
        }

        bool ShouldStand()
        {
            if (!IsInCombat)
                return false; 

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000)
                return false;
            
            if (!IsSafePosition(Player.Position))
                return false;
            
            Vector3 occulusPos;
            GetOculusPosition(out occulusPos, 58f, Player.Position);

            if (occulusPos == Vector3.Zero)
                return false;

            // 这里故意使用10码而不是OcculusRadius，也是对反复进出神目的一个保护
            if (occulusPos.Distance(Player.Position) > 10f)
                return false;

            if (Skills.Necromancer.GrimScythe.IsActive)
            {
                if (!IsValidTarget(ClosestTarget(occulusPos, 15f)))
                {
                    return false;
                }
            }
            else 
            {
                if (!IsValidTarget(ClosestTarget(occulusPos, 40f)))
                {
                    return false;
                }
            }

            if (IsDebugMove)
                Core.Logger.Warn($"在神目中，不再强制移动");
            return true;
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
            
            bool needAvoid = false;

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

            int mobs = TargetUtil.NumMobsInRange(10f);;
            if (mobs > 4)
            {
                if (IsDebugAvoid)
                    Core.Logger.Warn($"被包围了，放弃闪避，死磕到底！");
                return false;
            }
            
            if (!needAvoid)
                return false;

            // if (ShouldMoveInCombat(out power))
            // {
            //     if (IsDebugAvoid)
            //         Core.Logger.Warn($"通过吃球进行闪避");

            //     return true;
            // }
            
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

        private bool UseBloodRushAsOffensive(out TrinityPower power)
        {
            power = null;

            if (!Runes.Necromancer.Molting.IsActive)
            {
                if (Player.PrimaryResource > 25)
                    return false;

                if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
                    return false;
            }

            if (!Skills.Necromancer.BloodRush.CanCast())
                return false;

            Vector3 pos = Vector3.Zero;
            if (ShouldStand())
            {
                pos = Player.Position;
            }
            else
            {
                pos = SafePotForBloodRush();
                if (pos == Vector3.Zero)
                    return false;
            }

            if (IsDebugMove)
                Core.Logger.Warn($"OffensivePower中，使用血步等待CD，目标距离我{pos.Distance(Player.Position)}");
            power = BloodRush(pos);

            return power != null;
        }

        private bool TryRescureByBloodRush(out TrinityPower power)
        {
            power = null;
            TrinityActor healthGlobe = null;
            Vector3 pos = Vector3.Zero;

            if (IsPartyGame)
                return false;

            if (!IsInCombat)
                return false;

            if (!Core.Rift.IsGreaterRift)
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

            // if (!needRescure)
            // {
            //     if (BloodRushAvoidElite && _AnyElitesInSafeRange && !IsInOcculus())
            //     {
            //         if (IsDebugAvoid)
            //             Core.Logger.Warn($"附近有精英，尝试使用血步闪避");
            //         needRescure = true;
            //     }
            // }

            if (!needRescure)
                return false;

            if ((IsInOcculus() && Player.CurrentHealthPct < 0.4) || (!IsInOcculus() && Player.CurrentHealthPct < 0.6))
            {
                // 尝试找到一个血球最密集的点
                healthGlobe = FindHealthGlobeHeap(Player.Position, 10f, 50f, 8, true, 60f)??
                    FindHealthGlobeHeap(Player.Position, 10f, 50f, 8, false, 70f)??
                    FindHealthGlobeHeap(Player.Position, 10f, 50f, 8, true, 60f, false)??
                    FindHealthGlobeHeap(Player.Position, 10f, 50f, 8, false, 70f, false);

                if (healthGlobe != null)
                {
                    if (IsDebugAvoid)
                        Core.Logger.Warn($"找到了血球堆，距离我{healthGlobe.Distance}，用血步飞");
                    power = BloodRush(healthGlobe.Position);

                    // 已经使用血步进行闪避，进行中的闪避取消
                    isAvoiding = false;
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

            return true;
        }

        protected override bool ShouldLandOfTheDead(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.LandOfTheDead.CanCast())
                return false;

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 9900)
                return false;

            // target = GetBestTarget(Player.Position);
            target = TargetUtil.GetClosestUnit();
            if (!IsValidTarget(target))
                return false;

            return true;
        }

        protected override bool ShouldDevour()
        {
            if (!Skills.Necromancer.Devour.CanCast())
                return false;

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
            {
                if (DurationInLOTD == 0)
                    return true;

                return Skills.Necromancer.Devour.TimeSinceUse > DurationInLOTD*10;
            }
            else 
            {
                return Skills.Necromancer.Devour.TimeSinceUse > 1000 && TargetUtil.CorpseCount(60f) > 0;
            }

            return false;
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
                int intevalForCurse = CurseInterval*1000;

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
                int intevalForCurse = CurseInterval*1000;

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
                int intevalForCurse = CurseInterval*1000;

                if (Skills.Necromancer.Leech.TimeSinceUse < intevalForCurse) 
                    return false;
            }

            target = BestTargetWithoutDebuff(60f, SNOPower.P6_Necro_PassiveManager_Leech);
            if  (!IsValidTarget(target))
                return false;

            position = target.Position;
            return true;
        }

        TrinityActor lastCSTarget = null;
        double lastCSCast = 0;
        protected override bool ShouldCommandSkeletons(out TrinityActor target)
        {
            target = null;

            if (MyGetTickCount() - lastCSCast > 2000)
                lastCSTarget = null;
            
            if (!Skills.Necromancer.CommandSkeletons.CanCast())
                return false;

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
            {
                if (Skills.Necromancer.LandOfTheDead.CanCast())
                    return false;
                
                if (CastCSInterval !=0 && Skills.Necromancer.CommandSkeletons.TimeSinceUse < CastCSInterval*10)
                    return false;
            }
            else 
            {
                int interval = 250;
                if (Skills.Necromancer.LandOfTheDead.CanCast())
                    interval = 500;
                if (Skills.Necromancer.CommandSkeletons.TimeSinceUse < interval)
                    return false;
            }

            if (false)
                target = GetBestTarget2(Player.Position, 60f, true, lastCSTarget)??GetBestTarget2(Player.Position, 60f, true)??GetBestTarget2(Player.Position, 60f, false)??TargetUtil.GetClosestUnit(30f);
            else 
                target = GetBestTarget(Player.Position, 60f, true);

            if (!IsValidTarget(target))
                return false;

            // Core.Logger.Warn($"使用号令骸骨刷CD，目标距离我{target.Distance}");

            lastCSTarget = target;
            lastCSCast = MyGetTickCount();

            return true;
        }  

        protected override bool ShouldBoneSpikes(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.BoneSpikes.CanCast())
                return false;

            target = BestClusterUnit(Player.Position, 60f, true)??TargetUtil.GetClosestUnit(30f);
            return IsValidTarget(target);
        }

        protected override bool ShouldSiphonBlood(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.SiphonBlood.CanCast())
                return false;

            target = GetBestTarget(Player.Position, 50f);
            return IsValidTarget(target);
        }

        protected override bool ShouldCommandGolem(out TrinityActor target)
        {
            target = null;

            // note: this is returning PowerManager.CanCast true even when active is on cooldown.

            if (!Skills.Necromancer.CommandGolem.CanCast())
                return false;

            if (Skills.Necromancer.CommandGolem.TimeSinceUse < 4000)
                return false;

            var rune = Skills.Necromancer.CommandGolem.CurrentRune;
            if (rune == Runes.Necromancer.FleshGolem)
            {
                //
            }
            if (rune == Runes.Necromancer.IceGolem)
            {
                //
            }
            if (rune == Runes.Necromancer.BoneGolem)
            {
                //
            }
            if (rune == Runes.Necromancer.DecayGolem)
            {
                if (TargetUtil.CorpseCount(20f) == 0)
                    return false;
            }
            if (rune == Runes.Necromancer.BloodGolem)
            {
                //
            }

            if (Core.Cooldowns.GetSkillCooldownRemaining(SNOPower.P6_Necro_RaiseGolem).TotalMilliseconds > 0)
                return false;

            target = GetBestTarget(Player.Position, 60f);
            return IsValidTarget(target);
        }

        protected bool ShouldDeathNova(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.DeathNova.CanCast())
                return false;

            if (Player.PrimaryResource < 30)
                return false;

            target = ClosestTarget(Player.Position, 20f);

            return IsValidTarget(target);
        }

        bool ShouldUseGrimScytheInAdvance()
        {
            if (!Skills.Necromancer.GrimScythe.IsActive)
                return false;
            
            var buffs = new List<SNOPower> { SNOPower.P6_Necro_PassiveManager_Decrepify, SNOPower.P6_Necro_PassiveManager_Frailty, SNOPower.P6_Necro_PassiveManager_Leech };
            return !AnyTargetWithDebuffs(60f, buffs);
        }

        protected override bool ShouldGrimScythe(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.GrimScythe.CanCast())
                return false;

            target = TargetUtil.GetClosestUnit(60f);
            return IsValidTarget(target);
        }

        protected override bool ShouldBoneArmor()
        {
            if (!Skills.Necromancer.BoneArmor.CanCast())
                return false;

            if (TargetUtil.AnyMobsInRange(30f))
            {
                if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
                {
                    return Skills.Necromancer.BoneArmor.TimeSinceUse > 4000;
                }
                else 
                {
                    return Skills.Necromancer.BoneArmor.TimeSinceUse > 2000;
                }
            }

            return false;
        }

        protected override bool ShouldBoneSpear(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.BoneSpear.CanCast())
                return false;

            if (Player.PrimaryResource < 20)
                return false;

            // target = BestClusterUnit(ZetaDia.Me.Position, 60f);
            target = GetBestTarget(Player.Position, 60f);
            return IsValidTarget(target);
        }

        protected override bool ShouldSimulacrum(out Vector3 position)
        {
            position = Player.Position;

            if (!Skills.Necromancer.Simulacrum.CanCast())
                return false;

            if (Player.CurrentHealthPct < 0.5)
                return false;

            TrinityActor bestUnit = GetBestTarget(Player.Position);
            if (bestUnit != null)
                position = bestUnit.Position;
            else
                position = Player.Position;

            return true;
        }

        private bool CanCorpseLance()
        {
            if (!Skills.Necromancer.CorpseLance.CanCast())
                return false;

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
                return true;

            if (TargetUtil.CorpseCount(60f) > 0)
                return true;

            return false;
        }

        protected override bool ShouldCorpseLance(out TrinityActor target)
        {
            target = null;
            
            if (!Skills.Necromancer.CorpseLance.CanCast())
                return false;

            // 只在死地中主动使用尸矛
            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000)
                return false;

            // if (!Skills.Necromancer.LandOfTheDead.CanCast())
            //     return false;

            target = GetBestTarget(Player.Position, 60f);

            return IsValidTarget(target);
        }

        private void MyCastPower(TrinityPower power)
        {
            if (ZetaDia.Me.UsePower(power.SNOPower, power.TargetPosition, Core.Player.WorldDynamicId, power.TargetAcdId))
            {
                if (GameData.ResetNavigationPowers.Contains(power.SNOPower))
                {
                    Navigator.Clear();
                }

                SpellHistory.RecordSpell(power);
            }
        }

        private void TryZeroPower()
        {
            TrinityActor target = null;

            if (ShouldCommandSkeletons(out target))
            {
                MyCastPower(CommandSkeletons(target));
            }

            if (ShouldDevour())
            {
                MyCastPower(Devour());
            }
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
            TrinityPlayer me = null;
            foreach (TrinityPlayer u in Players)
            {
                // if (IsDebugParty)
                //     Core.Logger.Warn($"是我{u.IsMe}，是死灵{u.ActorClass == ActorClass.Necromancer}，距离我{u.Position.Distance(Player.Position), 类型{u.ActorClass}}");

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
                else 
                {
                    me = u;
                }
            }

            return leader;
        }

        void CheckFollow()
        {
            if (!AvoidLost)
                return;

            if (!IsPartyGame)
                return;

            if (PartyLeader == ZetaDia.Me)
                return;

            double currentTick = MyGetTickCount();

            if (currentTick - lastCheckFollow < checkFollowInterval)
                return;

            TrinityPlayer leader = MyLeader();
            teamMad = leader;
            lastCheckFollow = currentTick;

            if (leader == null)
            {
                isChasing = true;
                if (IsDebugParty)
                    Core.Logger.Warn($"附近没有队友，我肯定被抛弃了");
                return;
            }

            if (IsDebugParty)
                Core.Logger.Warn($"输出地图{leader.WorldDynamicId}, 我的地图{Core.Player.WorldDynamicId}，与输出距离{leader.Position.Distance(Player.Position)}");

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

        //public override int ClusterSize => Settings.ClusterSize;
        // Core.Settings.Weighting.EliteWeighting
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
                        // if (IsDebugParty)
                        //     Core.Logger.Warn($"追踪中，禁止打精英");
                        Core.Settings.Weighting.EliteWeighting = SettingMode.Disabled;
                        return 60;
                    }
                    else 
                    {
                        Core.Settings.Weighting.EliteWeighting = SettingMode.Enabled;
                    }
                }
                else 
                {
                    isChasing = false;
                    teamMad = null;
                }

                if (!IsPartyGame && Skills.Necromancer.LandOfTheDead.TimeSinceUse < 10000)
                {
                    return 1;
                }

                // if (Skills.Necromancer.LandOfTheDead.TimeSinceUse > 10000 && !Skills.Necromancer.LandOfTheDead.CanCast())
                // {
                //     return 40;
                // }

                if (IsReallyBlocked)
                {
                    return 1;
                }

                if (Core.Rift.IsNephalemRift)
                {
                    return Settings.ClusterSizeN;
                }
                else
                {  
                    return Settings.ClusterSize;
                }
			}
    	}
        public override float ClusterRadius => (int)Settings.ClusterRadius;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public override float ShrineRange => 120f;
        public override float TrashRange => 75f;
        public override float EliteRange => 75f;

        public bool AutoChangeSkill => Settings.AutoChangeSkill;
        public int FightDistance => Settings.FightDistance;
        public bool IgnoreShrine => Settings.IgnoreShrine;
        public int DurationInLOTD => Settings.DurationInLOTD;
        public bool StillMode => Settings.StillMode;
        public bool AvoidLost
        {
            get 
            {
                if (PartyLeader == ZetaDia.Me)
                    return false;

                return Settings.AvoidLost;
            }
        }
        public bool UseOcculus => Settings.UseOcculus;
        public int CurseInterval => Settings.CurseInterval;

        public int CastCSInterval => Settings.CastCSInterval;

        private bool IsDebugMove => Settings.IsDebugMove;
        private bool IsDebugSkill => Settings.IsDebugSkill;
        private bool IsDebugAvoid => Settings.IsDebugAvoid;
        private bool IsDebugParty => Settings.IsDebugParty;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public NEC_AssistSettings Settings { get; } = new NEC_AssistSettings();

        public sealed class NEC_AssistSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private int _clusterSizeN;
            private int _clusterRadius;
            private float _emergencyHealthPct;

            private bool _autoChangeSkill;
            private int _fightDistance;
            private bool _ignoreShrine;
            private int _durationInLOTD;
            private bool _stillMode;
            private bool _avoidLost;
            private bool _useOcculus;
            private int _curseInterval;

            private int _castCSInterval;

            private bool _isDebugMove;
            private bool _isDebugSkill;
            private bool _isDebugAvoid;
            private bool _isDebugParty;

            [DefaultValue(10)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(10)]
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
            public bool AutoChangeSkill
            {
                get { return _autoChangeSkill; }
                set { SetField(ref _autoChangeSkill, value); }
            }

            [DefaultValue(35)]
            public int FightDistance
            {
                get { return _fightDistance; }
                set { SetField(ref _fightDistance, value); }
            }

            [DefaultValue(false)]
            public bool IgnoreShrine
            {
                get { return _ignoreShrine; }
                set { SetField(ref _ignoreShrine, value); }
            }

            [DefaultValue(false)]
            public bool StillMode
            {
                get { return _stillMode; }
                set { SetField(ref _stillMode, value); }
            }

            [DefaultValue(false)]
            public bool AvoidLost
            {
                get { return _avoidLost; }
                set { SetField(ref _avoidLost, value); }
            }

            [DefaultValue(false)]
            public bool UseOcculus
            {
                get { return _useOcculus; }
                set { SetField(ref _useOcculus, value); }
            }

            [DefaultValue(2)]
            public int CurseInterval
            {
                get { return _curseInterval; }
                set { SetField(ref _curseInterval, value); }
            }

            [DefaultValue(5)]
            public int CastCSInterval
            {
                get { return _castCSInterval; }
                set { SetField(ref _castCSInterval, value); }
            }

            [DefaultValue(25)]
            public int DurationInLOTD
            {
                get { return _durationInLOTD; }
                set { SetField(ref _durationInLOTD, value); }
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
            public bool IsDebugParty
            {
                get { return _isDebugParty; }
                set { SetField(ref _isDebugParty, value); }
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


