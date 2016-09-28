using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Objects;
using Trinity.Reference;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Trinity.Technicals;
using Logger = Trinity.Technicals.Logger;

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
                Logger.Log($"Power was null or SNOPower.None");
                return false;
            }

            if (!CanCast(power.SNOPower))
            {
                if (!Core.Player.IsPowerUseDisabled)
                {
                    Logger.Log($"CanCast failed for {power.SNOPower}");
                }
                return false;
            }

            if (power.TargetDynamicWorldId != ZetaDia.WorldId)
            {
                Logger.Log($"World has changed since power was created");
                return false;
            }

            var castInfo = $"{type} {power}".Trim();

            var target = Core.Actors.GetActorByAcdId<TrinityActor>(power.TargetAcdId);
            if (target != null)
            {
                castInfo += $" on {target}";

                // Store found unit's position for SpellHistory queries.
                power.TargetPosition = target.Position;

                if (!Combat.Targeting.IsTargetInRange(target, power))
                {
                    Logger.Log($"Moving to {castInfo}");
                    PlayerMover.MoveTo(target.Position);
                    return true;
                }
            }
            else if (power.TargetPosition != Vector3.Zero)
            {
                castInfo += $" Dist:{Core.Player.Position.Distance(power.TargetPosition)}";

                if (!Combat.Targeting.IsPositionInRange(power.TargetPosition, power))
                {
                    Logger.Log($"Moving to position for {castInfo}");
                    PlayerMover.MoveTo(power.TargetPosition);
                    return true;
                }
            }

            if (power.ShouldWaitBeforeUse)
            {
                Logger.LogVerbose($"Waiting for before power for {power.WaitTimeBeforeRemaining}");
                await Coroutine.Sleep((int)power.WaitTimeBeforeRemaining);
            }

            if (power.SNOPower == SNOPower.Walk)
            {
                Logger.LogVerbose($"Walk - arrived at Destination doing nothing {castInfo}");
                return true;
            }

            if (!CastPower(power.SNOPower, power.TargetPosition, power.TargetAcdId))
            {
                Logger.LogVerbose($"Failed to cast {castInfo}");
                return false;
            }
            
            Logger.Warn($"Cast {castInfo}");

            if (power.ShouldWaitAfterUse)
            {
                Logger.LogVerbose($"Waiting after power for {power.WaitTimeAfterRemaining}");
                await Coroutine.Sleep((int)power.WaitTimeAfterRemaining);
            }

            if (power.ShouldWaitForAttackToFinish)
            {
                Logger.Log($"Waiting for Attack to Finish");
                await Coroutine.Wait(1000, () => Core.Player.IsCasting);
            }
            return true;
        }

        public bool CanCast(SNOPower power)
        {
            if (DataDictionary.AlwaysCanCastPowers.Contains(power))
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

            if (DataDictionary.ChargeBasedPowers.Contains(skill.SNOPower))
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
                if (DataDictionary.InteractPowers.Contains(power.SNOPower))
                {
                    power.TargetPosition = Vector3.Zero;
                }
                else if (power.TargetPosition == Vector3.Zero)
                {
                    power.TargetPosition = Core.Player.Position;
                }

                if (ZetaDia.Me.UsePower(power.SNOPower, power.TargetPosition, Core.Player.WorldDynamicId, power.TargetAcdId))
                {
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
                if (DataDictionary.InteractPowers.Contains(power))
                {
                    clickPosition = Vector3.Zero;
                }
                else if (clickPosition == Vector3.Zero)
                {
                    clickPosition = Core.Player.Position;
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
