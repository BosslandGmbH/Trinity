using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Combat.Resources
{
    public static class SpellHistory
    {
        static SpellHistory()
        {
            GameEvents.OnPlayerDied += GameEvents_OnPlayerDied;
        }

        private static void GameEvents_OnPlayerDied(object sender, EventArgs e)
        {
            _history.Clear();
        }

        public class SpellHistoryItem
        {
            public TrinityPower Power { get; set; }
            public DateTime UseTime { get; set; }
            public Vector3 MyPosition { get; set; }
            public Vector3 TargetPosition { get; set; }
            public TimeSpan TimeSinceUse => DateTime.UtcNow.Subtract(UseTime);
            public int TargetAcdId { get; set; }

            public TimeSpan TimeDistanceFrom(SpellHistoryItem other)
            {
                return other.UseTime < UseTime
                    ? other.UseTime.Subtract(UseTime)
                    : UseTime.Subtract(other.UseTime);
            }

            public override string ToString() => $"{Power.SNOPower} Age: {TimeSinceUse.ToString("mm':'ss':'fff")}";
        }

        private const int SpellHistorySize = 300;
        private static List<SpellHistoryItem> _history = new List<SpellHistoryItem>(SpellHistorySize);

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

            // todo, need a better way than looking up skill again.
            var skill = power.GetSkill();
            if (skill != null)
            {
                if (skill.IsAttackSpender)
                    _lastSpenderCast = DateTime.UtcNow;

                if (skill.IsGeneratorOrPrimary)
                    _lastGeneratorCast = DateTime.UtcNow;
            }

            _history.Add(new SpellHistoryItem
            {
                Power = power,
                UseTime = DateTime.UtcNow,
                MyPosition = Core.Player.Position,
                TargetPosition = power.TargetPosition,
                TargetAcdId = power.TargetAcdId,
            });

            LastSpellUseTime = DateTime.UtcNow;
            LastPowerUsed = power.SNOPower;
        }

        public static DateTime LastSpellUseTime { get; set; }

        public static SNOPower LastPowerUsed { get; set; }

        public static TrinityPower LastPower => History.OrderBy(s => s.TimeSinceUse).FirstOrDefault(s => s.Power.SNOPower != SNOPower.Walk)?.Power;

        public static void RecordSpell(SNOPower power)
        {
            RecordSpell(new TrinityPower(power));
        }

        public static DateTime PowerLastUsedTime(SNOPower power)
        {
            var useTime = DateTime.MinValue;
            if (History.Any())
            {
                var spellHistoryItem = GetLastUseHistoryItem(power);
                if (spellHistoryItem != null)
                {
                    useTime = spellHistoryItem.UseTime;
                }
                return useTime;
            }
            return useTime;
        }

        public static SpellHistoryItem GetLastUseHistoryItem(SNOPower power)
        {
            return _history.OrderByDescending(i => i.UseTime).FirstOrDefault(o => o.Power.SNOPower == power);
        }

        public static TimeSpan TimeSinceUse(SNOPower power)
        {
            var lastUsed = PowerLastUsedTime(power);
            if (lastUsed != DateTime.MinValue)
                return DateTime.UtcNow.Subtract(lastUsed);
            return TimeSpan.MaxValue;
        }

        internal static double MillisecondsSinceUse(SNOPower power)
        {
            var lastUsed = PowerLastUsedTime(power);
            if (lastUsed != DateTime.MinValue)
                return DateTime.UtcNow.Subtract(lastUsed).TotalMilliseconds;
            return -1;
        }

        public static int SpellUseCountInTime(SNOPower power, TimeSpan time)
        {
            if (_history.Any(i => i.Power.SNOPower == power))
            {
                var spellCount = _history.Count(i => i.Power.SNOPower == power && i.TimeSinceUse <= time);
                Core.Logger.Verbose(LogCategory.Targetting, "Found {0}/{1} spells in {2} time for {3} power", spellCount, _history.Count(i => i.Power.SNOPower == power), time, power);
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
            return Core.Player.Position.Distance(lastUsed);
        }

        public static float DistanceFromLastUsePosition(SNOPower power)
        {
            var lastUsed = GetSpellLastMyPosition(power);
            return Core.Player.Position.Distance(lastUsed);
        }

        public static IEnumerable<SpellHistoryItem> FindSpells(SNOPower withPower, Vector3 nearPosition, float withinRadius, int withinSeconds)
        {
            return History.Where(p => p.Power.SNOPower == withPower
                && p.TimeSinceUse < TimeSpan.FromSeconds(withinSeconds)
                && p.TargetPosition.Distance2DSqr(nearPosition) < withinRadius);
        }

        public static IEnumerable<SpellHistoryItem> FindSpells(SNOPower withPower, int withinSeconds)
        {
            return History.Where(p => p.Power.SNOPower == withPower
                && p.TimeSinceUse < TimeSpan.FromSeconds(withinSeconds));
        }

        public static void RecordSpell(SNOPower power, Vector3 position, int targetAcdId)
        {
            SpellTracker.TrackSpellOnUnit(targetAcdId, power);
            var p = new TrinityPower(power)
            {
                TargetAcdId = targetAcdId,
                TargetPosition = position,
            };
            RecordSpell(p);
        }

        public static void RecordSpell(SNOPower power, int targetAcdId)
        {
            SpellTracker.TrackSpellOnUnit(targetAcdId, power);
            var p = new TrinityPower(power)
            {
                TargetAcdId = targetAcdId
            };
            RecordSpell(p);
        }
    }
}