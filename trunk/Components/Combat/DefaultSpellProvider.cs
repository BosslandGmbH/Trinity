﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat.Resources;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.ProfileTags;
using Trinity.Reference;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Components.Combat
{
    public interface ISpellProvider
    {
        bool CanCast(SNOPower power);
        bool CanCast(Skill power);
        Task<bool> CastTrinityPower(TrinityPower power, string type = "");
        bool CastPower(SNOPower power, Vector3 clickPosition, int targetAcdId);
    }

    public class DefaultSpellProvider : ISpellProvider
    {
        public async Task<bool> CastTrinityPower(TrinityPower power, string type = "")
        {
            if (power == null || power.SNOPower == SNOPower.None)
            {
                Logger.Log(LogCategory.Spells, $"Power was null or SNOPower.None");
                return false;
            }

            if (!CanCast(power.SNOPower))
            {
                if (!Core.Player.IsPowerUseDisabled)
                {
                    Logger.Log(LogCategory.Spells, $"CanCast failed for {power.SNOPower}");
                }
                return false;
            }

            if (power.AssignedInDifferentWorld)
            {
                Logger.Log(LogCategory.Spells, $"World has changed since power was created");
                return false;
            }

            var distance = power.TargetPosition.Distance(Core.Player.Position);
            var castInfo = $"{type} {power}".Trim();

            var target = Core.Actors.GetActorByAcdId<TrinityActor>(power.TargetAcdId);
            if (target != null)
            {
                castInfo += $" on {target}";

                // Store found unit's position for SpellHistory queries.
                power.TargetPosition = target.Position;

                if (!Combat.Targeting.IsInRange(target, power))
                {
                    Logger.Log(LogCategory.Movement, $"Moving to {castInfo}");
                    PlayerMover.MoveTo(target.Position);
                    return true;
                }
            }
            else if (power.TargetPosition != Vector3.Zero)
            {
                if (distance > 200f)
                {
                    Logger.Log(LogCategory.Spells, $"Target is way too far away ({distance})");
                    return false;
                }

                castInfo += $" Dist:{Core.Player.Position.Distance(power.TargetPosition)}";
                if (!Combat.Targeting.IsInRange(power.TargetPosition, power))
                {
                    Logger.Log(LogCategory.Movement, $"Moving to position for {castInfo}");
                    PlayerMover.MoveTo(power.TargetPosition);
                    return true;
                }
            }

            if (power.ShouldWaitBeforeUse)
            {
                Logger.LogVerbose($"Waiting before power for {power.WaitTimeBeforeRemaining}");
                await Coroutine.Sleep((int)power.WaitTimeBeforeRemaining);
            }

            if (power.SNOPower == SNOPower.Walk)
            {
                Logger.LogVerbose(LogCategory.Movement, $"Walk - arrived at Destination doing nothing {castInfo}");
                return true;
            }

            if (!CastPower(power.SNOPower, power.TargetPosition, power.TargetAcdId))
            {
                Logger.LogVerbose(LogCategory.Spells, $"Failed to cast {castInfo}");
                return false;
            }
            
            Logger.Warn(LogCategory.Spells, $"Cast {castInfo}");

            if (power.ShouldWaitAfterUse)
            {
                Logger.LogVerbose(LogCategory.Spells, $"Waiting after power for {power.WaitTimeAfterRemaining}");
                await Coroutine.Sleep((int)power.WaitTimeAfterRemaining);
            }

            if (power.ShouldWaitForAttackToFinish)
            {
                Logger.Log(LogCategory.Spells, $"Waiting for Attack to Finish");
                await Coroutine.Wait(1000, () => Core.Player.IsCasting);
            }
            return true;
        }

        public bool CanCast(SNOPower power)
        {
            if (GameData.AlwaysCanCastPowers.Contains(power))
                return true;

            var skill = SkillUtils.GetSkillByPower(power);
            return skill != null && skill.CanCast();
        }

        public bool CanCast(Skill skill)
        {
            if (Core.Player.IsIncapacitated)
                return false;

            if (!Core.Hotbar.ActivePowers.Contains(skill.SNOPower))
                return false;

            if (!HasEnoughCharges(skill))
                return false;

            if (!PowerManager.CanCast(skill.SNOPower))
                return false;

            if (!HasEnoughResource(skill))
                return false;

            return true;
        }

        public bool HasEnoughCharges(Skill skill)
        {
            if (skill == Skills.Barbarian.Avalanche && Runes.Barbarian.TectonicRift.IsActive)
                return Core.Hotbar.GetSkillCharges(skill.SNOPower) > 0;

            if (GameData.ChargeBasedPowers.Contains(skill.SNOPower))
                return Core.Hotbar.GetSkillCharges(skill.SNOPower) > 0;
   
            return true;
        }

        public bool HasEnoughResource(Skill skill)
        {
            var resourceCost = skill.Cost * (1 - Core.Player.ResourceCostReductionPct);
            if (resourceCost > 0 && !skill.IsGeneratorOrPrimary)
            {
                var actualResource = (skill.Resource == Resource.Discipline) ? Core.Player.SecondaryResource : Core.Player.PrimaryResource;
                if (actualResource < resourceCost)
                    return false;
            }

            return true;
        }

        public bool CastPower(TrinityPower power)
        {
            if (power.SNOPower != SNOPower.None && Core.GameIsReady)
            {
                if (GameData.InteractPowers.Contains(power.SNOPower))
                {
                    power.TargetPosition = Vector3.Zero;
                }
                else if (power.TargetPosition == Vector3.Zero)
                {
                    power.TargetPosition = Core.Player.Position;
                }

                if (ZetaDia.Me.UsePower(power.SNOPower, power.TargetPosition, Core.Player.WorldDynamicId, power.TargetAcdId))
                {
                    if (GameData.ResetNavigationPowers.Contains(power.SNOPower))
                    {
                        Navigator.Clear();
                    }

                    SpellHistory.RecordSpell(power);
                    return true;
                }
            }
            return false;
        }



        public bool CastPower(SNOPower power, Vector3 clickPosition, int targetAcdId)
        {
            if (power != SNOPower.None && Core.GameIsReady)
            {
                if (GameData.InteractPowers.Contains(power))
                {
                    clickPosition = Vector3.Zero;
                }
                else if (clickPosition == Vector3.Zero)
                {
                    clickPosition = Core.Player.Position;
                }

                if (GameData.ResetNavigationPowers.Contains(power))
                {
                    Navigator.Clear();
                }

                if (ZetaDia.Me.UsePower(power, clickPosition, Core.Player.WorldDynamicId, targetAcdId))
                {
                    SpellHistory.RecordSpell(power, clickPosition, targetAcdId);
                    return true;
                }


            }
            return false;
        }

    }
}