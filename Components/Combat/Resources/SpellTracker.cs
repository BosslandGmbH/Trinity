using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Resources
{
    /// <summary>
    /// Track DoT and duration/expiration spells on monsters
    /// This is still useful for expensive spells that apply a debuff and we dont want to
    /// cast it again before the projectile actually hits the unit.
    /// </summary>
    public class SpellTracker : IEquatable<SpellTracker>
    {
        private const int AnyRune = -999;

        public int AcdId { get; set; }
        public SNOPower Power { get; set; }
        public DateTime Expiration { get; set; }

        internal static HashSet<SpellTracker> TrackedUnits { get; set; }
        private static readonly Thread MaintenanceThread;

        internal static void TrackSpellOnUnit(SpellTracker trackedUnit)
        {
            if (!TrackedUnits.Any(t => t.Equals(trackedUnit)))
            {
                TrackedUnits.Add(trackedUnit);
            }
        }

        internal static void TrackSpellOnUnit(int AcdId, SNOPower power)
        {
            try
            {
                if (AcdId == 0)
                    return;

                float duration = -1;
                if (Core.Hotbar.ActivePowers.Contains(power))
                {
                    // Can't track a spell that isn't equipped
                    var skill = Core.Hotbar.GetHotbarSkill(power);
                    var spell = TrackedSpells.FirstOrDefault(s => s.Equals(new TrackedSpell(power, skill.RuneIndex)) || s.Equals(new TrackedSpell(power, AnyRune)));
                    if (spell != null)
                        duration = spell.Duration;
                }

                var anyRune = TrackedSpells.FirstOrDefault(s => s.Power == power && s.RuneIndex == AnyRune);

                if (duration == -1 && anyRune != null)
                    duration = anyRune.Duration;

                if (duration > 0)
                {
                    Core.Logger.Verbose(LogCategory.Behavior, "Tracking unit {0} with power {1} for duration {2:0.00}", AcdId, power, duration);
                    TrackSpellOnUnit(new SpellTracker()
                    {
                        AcdId = AcdId,
                        Power = power,
                        Expiration = DateTime.UtcNow.AddMilliseconds(duration)
                    });
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in TrackSpellOnUnit: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Checks if a unit is currently being tracked with a given SNOPower. When the spell is properly configured, this can be used to set a "timer" on a DoT re-cast, for example.
        /// </summary>
        /// <param name="AcdId"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static bool IsUnitTracked(int AcdId, SNOPower power)
        {
            return TrackedUnits.Any(t => t.AcdId == AcdId && t.Power == power);
        }

        /// <summary>
        /// Checks if a unit is currently being tracked with a given SNOPower. When the spell is properly configured, this can be used to set a "timer" on a DoT re-cast, for example.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static bool IsUnitTracked(TrinityActor unit, SNOPower power)
        {
            if (unit == null)
                return false;

            if (unit.Type != TrinityObjectType.Unit)
                return false;
            bool result = TrackedUnits.Any(t => t.AcdId == unit.AcdId && t.Power == power);
            //if (result)
            //    Technicals.Core.Logger.Log("Unit {0} is tracked with power {1}", unit.AcdId, power);
            //else
            //    Technicals.Core.Logger.Log("Unit {0} is NOT tracked with power {1}", unit.AcdId, power);
            return result;
        }

        #region Static Constructor

        static SpellTracker()
        {
            TrackedUnits = new HashSet<SpellTracker>();
            PopulateTrackedSpells();

            MaintenanceThread = new Thread(RunMaintenance)
            {
                Name = "TrinityPlugin SpellTracker",
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            MaintenanceThread.Start();
        }

        #endregion Static Constructor

        #region Background Maintenance Thread

        private static void RunMaintenance()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(100);
                    if (TrackedUnits.Any())
                    {
                        lock (TrackedUnits)
                        {
                            //TrackedUnits.RemoveWhere(t => t.Expiration < DateTime.UtcNow);
                            var units = TrackedUnits.Where(t => t.Expiration < DateTime.UtcNow);
                            foreach (var unit in units.ToList())
                            {
                                //Technicals.Core.Logger.Log("Removing unit {0} from TrackedUnits ({1}, {2})", unit.AcdId, unit.Expiration.Ticks, DateTime.UtcNow.Ticks);
                                TrackedUnits.Remove(unit);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Log("Exception in SpellTracker Maintenance: {0}", ex.ToString());
                }
            }
        }

        #endregion Background Maintenance Thread

        #region IEquatable Implimentation

        public bool Equals(SpellTracker other)
        {
            return this.AcdId == other.AcdId && this.Power == other.Power;
        }

        #endregion IEquatable Implimentation

        private static readonly HashSet<TrackedSpell> TrackedSpells = new HashSet<TrackedSpell>();

        /// <summary>
        /// Populates the static TrackedSpells Hashset. Called once from Static Constructor. Do not call anywhere else...
        /// </summary>
        private static void PopulateTrackedSpells()
        {
            // new TrackedSpell(SNOPower, RuneIndex, MillsecondsDuration)
            // Use RuneIndex = AnyRune for "default" or any rune index

            TrackedSpells.Clear();

            // Barbarian
            // TBD, maybe Rend is a good candidate?

            // Monk
            TrackedSpells.Add(new TrackedSpell(SNOPower.Monk_ExplodingPalm, AnyRune, 9000f));

            TrackedSpells.Add(new TrackedSpell(SNOPower.Monk_FistsofThunder, 0, 6000f)); // Static Charge

            // Wizard
            // TBD

            // Witch Doctor
            TrackedSpells.Add(new TrackedSpell(SNOPower.Witchdoctor_Haunt, 0, 6000f));
            TrackedSpells.Add(new TrackedSpell(SNOPower.Witchdoctor_Haunt, 1, 6000f));
            TrackedSpells.Add(new TrackedSpell(SNOPower.Witchdoctor_Haunt, 2, 6000f));
            TrackedSpells.Add(new TrackedSpell(SNOPower.Witchdoctor_Haunt, 3, 6000f));
            TrackedSpells.Add(new TrackedSpell(SNOPower.Witchdoctor_Haunt, 4, 2000f)); // WD, Resentful Spirit

            TrackedSpells.Add(new TrackedSpell(SNOPower.Witchdoctor_Locust_Swarm, AnyRune, 8000f));

            TrackedSpells.Add(new TrackedSpell(SNOPower.DemonHunter_MarkedForDeath, AnyRune, 10f));

            // Demon Hunter
            TrackedSpells.Add(new TrackedSpell(SNOPower.DemonHunter_MarkedForDeath, AnyRune, 30000f));
            // TBD
        }

        public class TrackedSpell : IEquatable<TrackedSpell>
        {
            /// <summary>
            /// The SNO Power
            /// </summary>
            public SNOPower Power { get; set; }

            /// <summary>
            /// The rune index
            /// </summary>
            public int RuneIndex { get; set; }

            /// <summary>
            /// Duration of Tracked Spell in Millseconds
            /// </summary>
            public float Duration { get; set; }

            public TrackedSpell(SNOPower power, int runeIndex, float duration)
            {
                Power = power;
                RuneIndex = runeIndex;
                Duration = duration;
            }

            public TrackedSpell(SNOPower power, int runeIndex)
            {
                Power = power;
                RuneIndex = runeIndex;
            }

            public bool Equals(TrackedSpell other)
            {
                return Power == other.Power && (RuneIndex == other.RuneIndex || RuneIndex == AnyRune || other.RuneIndex == AnyRune);
            }
        }
    }
}