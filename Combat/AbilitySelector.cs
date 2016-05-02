using System;
using Trinity.Combat.Abilities;
using Trinity.Combat.Abilities.PhelonsPlayground;
using Trinity.Combat.Abilities.PhelonsPlayground.Barbarian;
using Trinity.Technicals;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public partial class TrinityPlugin : IPlugin
    {


        /// <summary>
        /// Check if a particular buff is present
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static bool GetHasBuff(SNOPower power)
        {
            return GetBuffStacks(power) > 0;
        }

        /// <summary>
        /// Returns how many stacks of a particular buff there are
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static int GetBuffStacks(SNOPower power)
        {
            return CacheData.Buffs.GetBuffStacks(power);
        }

        ///// <summary>
        ///// Check re-use timers on skills, returns true if we can use the power
        ///// </summary>
        ///// <param name="power">The power.</param>
        ///// <param name="recheck">if set to <c>true</c> check again.</param>
        ///// <returns>
        ///// Returns whether or not we can use a skill, or if it's on our own internal TrinityPlugin cooldown timer
        ///// </returns>
        //public static bool SNOPowerUseTimer(SNOPower power, bool recheck = false)
        //{
        //    if (TimeSinceUse(power) >= CombatBase.GetSNOPowerUseDelay(power))
        //        return true;
        //    if (recheck && TimeSinceUse(power) >= 150 && TimeSinceUse(power) <= 600)
        //        return true;
        //    return false;
        //}
        
        /// <summary>
        /// A default power in case we can't use anything else
        /// </summary>
        private static TrinityPower defaultPower = new TrinityPower();

        /// <summary>
        /// Returns an appropriately selected TrinityPower and related information
        /// </summary>
        /// <param name="IsCurrentlyAvoiding">Are we currently avoiding?</param>
        /// <param name="UseOOCBuff">Buff Out Of Combat</param>
        /// <param name="UseDestructiblePower">Is this for breaking destructables?</param>
        /// <returns></returns>
        internal static TrinityPower AbilitySelector(bool IsCurrentlyAvoiding = false, bool UseOOCBuff = false, bool UseDestructiblePower = false)
        {
            using (new PerformanceLogger("AbilitySelector"))
            {
                if (!UseOOCBuff && CurrentTarget == null)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "AbilitySelector CurrentTarget == null while in combat, returning empty power");
                    return new TrinityPower();
                }

                if (DateTime.UtcNow.Subtract(CombatBase.CurrentPower.PowerAssignmentTime).TotalSeconds > 5 || CurrentTarget == null)
                    CombatBase.CurrentPower = defaultPower;                

                // Switch based on the cached character class
                TrinityPower power = CombatBase.CurrentPower;

                using (new PerformanceLogger("AbilitySelector.ClassAbility"))
                {
                    switch (Player.ActorClass)
                    {
                        // Barbs
                        case ActorClass.Barbarian:
                            if (Settings.Advanced.PhelonsPlayground)
                            {
                                power = Barbarian.GetPower();
                                if (power != null)
                                    break;
                            }
                            power = BarbarianCombat.GetPower();
                            break;
                        // Crusader
                        case ActorClass.Crusader:
                            power = CrusaderCombat.GetPower();
                            break;
                        // Monks
                        case ActorClass.Monk:
                            power = MonkCombat.GetPower();
                            if (power != null && GetHasBuff(SNOPower.X1_Monk_Epiphany) && power.MinimumRange > 0)
                                power.MinimumRange = 75f;
                            break;
                        // Wizards
                        case ActorClass.Wizard:
                            power = WizardCombat.GetPower();
                            break;
                        // Witch Doctors
                        case ActorClass.Witchdoctor:
                            power = WitchDoctorCombat.GetPower();
                            break;
                        // Demon Hunters
                        case ActorClass.DemonHunter:
                            power = DemonHunterCombat.GetPower();
                            break;
                    }
                }

                // use IEquatable to check if they're equal
                if (CombatBase.CurrentPower == power)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "Keeping {0}", CombatBase.CurrentPower.ToString());
                    return CombatBase.CurrentPower;
                }
                else if (power != null && power.SNOPower != SNOPower.None)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "Selected new {0}", power.ToString());
                    return power;
                }
                else
                    return defaultPower;
            }
        }

        /// <summary>
        /// Returns true if we have the ability and the buff is up, or true if we don't have the ability in our hotbar
        /// </summary>
        /// <param name="snoPower"></param>
        /// <returns></returns>
        internal static bool CheckAbilityAndBuff(SNOPower snoPower)
        {
            return
                (!Hotbar.Contains(snoPower) || (Hotbar.Contains(snoPower) && GetHasBuff(snoPower)));

        }

        /// <summary>
        /// Gets the time in Millseconds since we've used the specified power
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        internal static double TimeSinceUse(SNOPower power)
        {
            if (CacheData.AbilityLastUsed.ContainsKey(power))
                return DateTime.UtcNow.Subtract(CacheData.AbilityLastUsed[power]).TotalMilliseconds;
            else
                return -1;
        }


    }

}
