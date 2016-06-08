using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Trinity.Technicals;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities
{
    public static class SpellHistory
    {
        private const int SpellHistorySize = 300;
        private static List<SpellHistoryItem> _history = new List<SpellHistoryItem>(SpellHistorySize * 2);

        private static DateTime _lastSpenderCast = DateTime.MinValue;
        public static double TimeSinceSpenderCast 
        {
            get { return DateTime.UtcNow.Subtract(_lastSpenderCast).TotalMilliseconds; }
        }

        private static DateTime _lastGeneratorCast = DateTime.MinValue;
        public static double TimeSinceGeneratorCast
        {
            get { return DateTime.UtcNow.Subtract(_lastGeneratorCast).TotalMilliseconds; }
        }

        internal static List<SpellHistoryItem> History
        {
            get { return _history; }
            set { _history = value; }
        }

        public static void RecordSpell(TrinityPower power)
        {
            if (_history.Count >= SpellHistorySize)
                _history.RemoveAt(0);

            var skill = SkillUtils.ById(power.SNOPower);
            
            if(skill.IsAttackSpender)
                _lastSpenderCast = DateTime.UtcNow;

            if (skill.IsGeneratorOrPrimary)
                _lastGeneratorCast = DateTime.UtcNow;

            _history.Add(new SpellHistoryItem
            {
                Power = power,
                UseTime = DateTime.UtcNow,
                MyPosition = TrinityPlugin.Player.Position,
                TargetPosition = power.TargetPosition
            });

            TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
            TrinityPlugin.LastPowerUsed = power.SNOPower;

            LastSpellUseTime = DateTime.UtcNow;
            LastPowerUsed = power.SNOPower;

            Logger.LogVerbose(LogCategory.Targetting, "Recorded {0}", power);
            CacheData.AbilityLastUsed[power.SNOPower] = DateTime.UtcNow;            
        }

        public static DateTime LastSpellUseTime { get; set; }

        public static SNOPower LastPowerUsed { get; set; }

        public static TrinityPower LastPower => History.OrderBy(s => s.TimeSinceUse).FirstOrDefault(s => s.Power.SNOPower != SNOPower.Walk)?.Power;

        public static void RecordSpell(SNOPower power)
        {
            RecordSpell(new TrinityPower(power));
        }

        public static TrinityPower GetLastTrinityPower()
        {
            if (History.Any())
                return _history.OrderByDescending(i => i.UseTime).FirstOrDefault().Power;
            return new TrinityPower();
        }

        public static SNOPower GetLastSNOPower()
        {
            if (History.Any())
                return _history.OrderByDescending(i => i.UseTime).FirstOrDefault().Power.SNOPower;
            return SNOPower.None;
        }

        public static DateTime GetSpellLastused(SNOPower power = SNOPower.None)
        {
            DateTime lastUsed = DateTime.MinValue;
            if (power == SNOPower.None && CacheData.AbilityLastUsed.Any())
            {
                var pair = CacheData.AbilityLastUsed.LastOrDefault();
                lastUsed = pair.Value;
            }
            else
            {
                CacheData.AbilityLastUsed.TryGetValue(power, out lastUsed);
            }
            return lastUsed;
        }

        public static TimeSpan TimeSinceUse(SNOPower power)
        {
            DateTime lastUsed = GetSpellLastused(power);
            return DateTime.UtcNow.Subtract(lastUsed);
        }

        public static int SpellUseCountInTime(SNOPower power, TimeSpan time)
        {
            if (_history.Any(i => i.Power.SNOPower == power))
            {
                var spellCount = _history.Count(i => i.Power.SNOPower == power && i.TimeSinceUse <= time);
                Logger.LogVerbose(LogCategory.Targetting, "Found {0}/{1} spells in {2} time for {3} power", spellCount, _history.Count(i => i.Power.SNOPower == power), time, power);
                return spellCount;
            }
            return 0;
        }

        public static bool HasUsedSpell(SNOPower power)
        {
            if (_history.Any() && _history.Any(i => i.Power.SNOPower == power))
                return true;
            return false;
        }

        public static Vector3 GetSpellLastTargetPosition(SNOPower power)
        {
            Vector3 lastUsed = Vector3.Zero;
            if (_history.Any(i => i.Power.SNOPower == power))
                lastUsed = _history.FirstOrDefault(i => i.Power.SNOPower == power).TargetPosition;
            return lastUsed;
        }

        public static Vector3 GetSpellLastMyPosition(SNOPower power)
        {
            Vector3 lastUsed = Vector3.Zero;
            if (_history.Any(i => i.Power.SNOPower == power))
                lastUsed = _history.FirstOrDefault(i => i.Power.SNOPower == power).MyPosition;
            return lastUsed;
        }

        public static float DistanceFromLastTarget(SNOPower power)
        {
            var lastUsed = GetSpellLastTargetPosition(power);
            return TrinityPlugin.Player.Position.Distance(lastUsed);
        }

        public static float DistanceFromLastUsePosition(SNOPower power)
        {
            var lastUsed = GetSpellLastMyPosition(power);
            return TrinityPlugin.Player.Position.Distance(lastUsed);
        }

    }
}
