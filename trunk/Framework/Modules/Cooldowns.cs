using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Modules
{
    /// <summary>
    /// Keep track of cooldowns for buffs and skills
    /// </summary>
    public sealed class Cooldowns : Module
    {
        public class CooldownData
        {
            /// <summary>
            /// SNOPower as an integer
            /// </summary>
            public int SnoId { get; set; }

            /// <summary>
            /// Game time when the buff started
            /// </summary>
            public int StartGameTime { get; set; }

            /// <summary>
            /// Game time when the buff will end
            /// </summary>
            public int EndGameTime { get; set; }

            /// <summary>
            /// Total time of the cooldown from start to end.
            /// </summary>
            public int DurationGameTime { get; set; }

            /// <summary>
            /// Start attribute this buff is currently stored in.
            /// </summary>
            public ActorAttributeType StartAttribute { get; set; }

            /// <summary>
            /// End attribute this buff is currently stored in.
            /// </summary>
            public ActorAttributeType EndAttribute { get; set; }

            /// <summary>
            /// CurrentTime remaining in game ticks
            /// </summary>
            public int RemainingGameTime => IsFinished ? 0 : Offset - EndOffset;

            /// <summary>
            /// Total duration of the cooldown
            /// </summary>
            public TimeSpan Duration => TimeSpan.FromMilliseconds((double)DurationGameTime * 100 / 6);

            /// <summary>
            /// Time remaining before cooldown ends
            /// </summary>
            public TimeSpan Remaining => IsFinished ? TimeSpan.Zero : TimeSpan.FromMilliseconds((double)RemainingGameTime*100/6);

            /// <summary>
            /// Time elapsed since cooldown started
            /// </summary>
            public TimeSpan Elapsed => IsFinished ? Duration : Duration - Remaining;

            /// <summary>
            /// Cooldown has finished.
            /// </summary>
            public bool IsFinished => CurrentTime > EndCurrentTime;

            /// <summary>
            /// Percentage of cooldown remaining
            /// </summary>
            public double Percent
            {
                get { return DurationGameTime > 0 ? RemainingGameTime / (double)DurationGameTime : 0; }
            }

            public int AttributeIndex { get; set; }
            public int Offset { get; set; }
            public int EndOffset { get; set; }
            public int StorageKey { get; set; }
            public int EndCurrentTime { get; set; }

            public override string ToString()
            {
                return String.Format("Power={0} ({1}) Start={2} End={3} Duration={4} ({5}s) StartAttr={6} Remaining={7}s ({8:00.00}%) Finished={9} Offset={10} EndOffset={11} CurrentTime={12} EndCurrentTime={13} Key={14}",
                    (SNOPower)SnoId, SnoId, StartGameTime, EndGameTime, DurationGameTime, Duration.TotalSeconds, StartAttribute, Remaining.TotalSeconds, Percent*100, IsFinished, Offset, EndOffset, ZetaDia.CurrentTime, EndCurrentTime, StorageKey);
            }
        }

        /// <summary>
        /// Storage for cooldowns of a SNOPower.
        /// Every SNOPower may have up to 31 cooldowns, most only use one.
        /// </summary>
        public class CooldownGroup
        {
            public Dictionary<int, CooldownData> Cooldowns = new Dictionary<int, CooldownData>();
        }

        public static readonly Dictionary<int, CooldownGroup> CooldownStore = new Dictionary<int, CooldownGroup>();

        public static int CurrentTime;

        public static bool IsLogging;

        protected override int UpdateIntervalMs => 0;

        protected override void OnPulse()
        {
            using (new PerformanceLogger("Utility.Cooldowns.Pulse"))
            {
                CurrentTime = ZetaDia.CurrentTime;
                IsLogging = TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.Cooldowns);

                foreach (var buff in ZetaDia.Me.GetAllBuffs()) //CacheData.Buffs.AllBuffs)
                {
                    var cBuff = new CachedBuff(buff);
                    RecordBuffData(cBuff.Id, cBuff.BuffAttributeSlot, cBuff.VariantName);
                }

                // Convention full duration buff.
                RecordBuffData((int)SNOPower.P2_ItemPassive_Unique_Ring_038, 8, "AllElements");

                foreach (var power in SkillUtils.ActiveIds)
                {
                    RecordSkillData(power);
                }
            }
        }


        public CooldownData GetBuffCooldown(SNOPower power, int variant = NoAttributeKey)
        {
            var key = (int)power;
            if (!CooldownStore.ContainsKey(key))
                return null;

            var group = CooldownStore[key];
            if (variant == NoAttributeKey)
            {
                return group.Cooldowns.FirstOrDefault(cd => cd.Key >= 0 && !cd.Value.IsFinished).Value;
            }

            if (!group.Cooldowns.ContainsKey(variant))
                return null;

            return group.Cooldowns[variant];
        }

        public TimeSpan GetBuffCooldownRemaining(SNOPower power, int variant = NoAttributeKey)
        {
            var cd = GetBuffCooldown(power);
            return cd != null ? cd.Remaining : TimeSpan.Zero;
        }

        public TimeSpan GetBuffCooldownElapsed(SNOPower power, int variant = NoAttributeKey)
        {
            var cd = GetBuffCooldown(power);
            return cd != null ? cd.Elapsed : TimeSpan.Zero;
        }

        public CooldownData GetSkillCooldown(SNOPower power)
        {
            var key = (int)power;
            if (CooldownStore.ContainsKey(key))
            {
                var group = CooldownStore[key];
                if (group.Cooldowns.ContainsKey(SkillAttributeKey))
                {
                    return group.Cooldowns[SkillAttributeKey];
                }
            }
            return null;
        }

        public TimeSpan GetSkillCooldownRemaining(SNOPower power)
        {
            var cd = GetSkillCooldown(power);
            return cd != null ? cd.Remaining : TimeSpan.Zero;
        }

        public TimeSpan GetSkillCooldownElapsed(SNOPower power)
        {
            var cd = GetSkillCooldown(power);            
            return cd != null ? cd.Elapsed : TimeSpan.Zero;
        }

        private const int NoAttributeKey = -2;
        private const int SkillAttributeKey = -1;

        internal void RecordBuffData(int powerId, int attrKey, string name)
        {
            var startAttr = _buffStartAttributes[attrKey];
            var endAttr = _buffEndAttributes[attrKey];

            var startTime = ZetaDia.Me.CommonData.GetAttribute<int>(startAttr & ((1 << 12) - 1) | (powerId << 12));
            if (startTime <= 1) return;

            var endTime = ZetaDia.Me.CommonData.GetAttribute<int>(endAttr & ((1 << 12) - 1) | (powerId << 12));
            if (endTime <= 1) return;

            var data = UpdateCooldownData(powerId, attrKey, startTime, (ActorAttributeType)startAttr, endTime, (ActorAttributeType)endAttr);

            if (IsLogging)
                Logger.Log("Buff Cooldown: {0} Variant={1}", data, name);
        }

        internal void RecordSkillData(SNOPower power)
        {
            var startAttr = ActorAttributeType.PowerCooldownStart;
            var endAttr = ActorAttributeType.PowerCooldown;
            var storageKey = SkillAttributeKey;

            var startTime = ZetaDia.Me.CommonData.GetAttribute<int>((int)startAttr & ((1 << 12) - 1) | ((int)power << 12));
            if (startTime <= 1) return;

            var endTime = ZetaDia.Me.CommonData.GetAttribute<int>((int)endAttr & ((1 << 12) - 1) | ((int)power << 12));
            if (endTime <= 1) return;

            var data = UpdateCooldownData((int)power, storageKey, startTime, startAttr, endTime, endAttr);

            if (IsLogging)
                Logger.Log("Skill Cooldown: {0}", data);
        }

        private static CooldownData UpdateCooldownData(int snoId, int storageKey, int startTime, ActorAttributeType startAttr, int endTime, ActorAttributeType endAttr)
        {
            // Attribute time returned (game time) is not the same as ZetaDia.CurrentTime and the difference between the two vary over time.
            // When the buff starts a comparison to CurrentTime is recorded. This can then be used to work out know how much time has elapsed.
            // Offset will count down until it reaches EndOffset, then it has ended.

            Action<CooldownData> updateBuffDataObject = d =>
            {
                d.SnoId = snoId;
                d.StartGameTime = startTime;
                d.StartAttribute = startAttr;
                d.EndGameTime = endTime;
                d.EndAttribute = endAttr;                
                d.StorageKey = storageKey;
                d.DurationGameTime = endTime - startTime;
                d.EndCurrentTime = ZetaDia.CurrentTime + d.DurationGameTime;
                d.EndOffset = (startTime - ZetaDia.CurrentTime) - d.DurationGameTime;
            };

            CooldownGroup group;
            CooldownData data;
            if (!CooldownStore.ContainsKey(snoId))
            {
                group = new CooldownGroup();
                CooldownStore.Add(snoId, group);
            }
            else
            {
                group = CooldownStore[snoId];
            }

            var cooldowns = group.Cooldowns;
            if (!cooldowns.ContainsKey(storageKey))
            {
                data = new CooldownData();
                updateBuffDataObject(data);
                cooldowns.Add(storageKey, data);
            }
            else
            {
                data = cooldowns[storageKey];
                if (data.StartGameTime != startTime)
                {
                    updateBuffDataObject(data);
                }
            }

            data.Offset = startTime - ZetaDia.CurrentTime;
            //data.IsFinished = data.Offset <= data.EndOffset;
            return data;
        }

        public enum Power
        {
            None = 0,
            ConventionOfElements = SNOPower.P2_ItemPassive_Unique_Ring_038,
            BastianOfWill = SNOPower.ItemPassive_Unique_Ring_735_x1,
        }

        private readonly int[] _buffStartAttributes = new int[]
        {
            (int)ActorAttributeType.BuffIconStartTick0,
            (int)ActorAttributeType.BuffIconStartTick1,
            (int)ActorAttributeType.BuffIconStartTick2,
            (int)ActorAttributeType.BuffIconStartTick3,
            (int)ActorAttributeType.BuffIconStartTick4,
            (int)ActorAttributeType.BuffIconStartTick5,
            (int)ActorAttributeType.BuffIconStartTick6,
            (int)ActorAttributeType.BuffIconStartTick7,
            (int)ActorAttributeType.BuffIconStartTick8,
            (int)ActorAttributeType.BuffIconStartTick9,
            (int)ActorAttributeType.BuffIconStartTick10,
            (int)ActorAttributeType.BuffIconStartTick11,
            (int)ActorAttributeType.BuffIconStartTick12,
            (int)ActorAttributeType.BuffIconStartTick13,
            (int)ActorAttributeType.BuffIconStartTick14,
            (int)ActorAttributeType.BuffIconStartTick15,
            (int)ActorAttributeType.BuffIconStartTick16,
            (int)ActorAttributeType.BuffIconStartTick17,
            (int)ActorAttributeType.BuffIconStartTick18,
            (int)ActorAttributeType.BuffIconStartTick19,
            (int)ActorAttributeType.BuffIconStartTick20,
            (int)ActorAttributeType.BuffIconStartTick21,
            (int)ActorAttributeType.BuffIconStartTick22,
            (int)ActorAttributeType.BuffIconStartTick23,
            (int)ActorAttributeType.BuffIconStartTick24,
            (int)ActorAttributeType.BuffIconStartTick25,
            (int)ActorAttributeType.BuffIconStartTick26,
            (int)ActorAttributeType.BuffIconStartTick27,
            (int)ActorAttributeType.BuffIconStartTick28,
            (int)ActorAttributeType.BuffIconStartTick29,
            (int)ActorAttributeType.BuffIconStartTick30,
            (int)ActorAttributeType.BuffIconStartTick31,
        };

        private readonly int[] _buffEndAttributes = new int[]
        {
            (int)ActorAttributeType.BuffIconEndTick0,
            (int)ActorAttributeType.BuffIconEndTick1,
            (int)ActorAttributeType.BuffIconEndTick2,
            (int)ActorAttributeType.BuffIconEndTick3,
            (int)ActorAttributeType.BuffIconEndTick4,
            (int)ActorAttributeType.BuffIconEndTick5,
            (int)ActorAttributeType.BuffIconEndTick6,
            (int)ActorAttributeType.BuffIconEndTick7,
            (int)ActorAttributeType.BuffIconEndTick8,
            (int)ActorAttributeType.BuffIconEndTick9,
            (int)ActorAttributeType.BuffIconEndTick10,
            (int)ActorAttributeType.BuffIconEndTick11,
            (int)ActorAttributeType.BuffIconEndTick12,
            (int)ActorAttributeType.BuffIconEndTick13,
            (int)ActorAttributeType.BuffIconEndTick14,
            (int)ActorAttributeType.BuffIconEndTick15,
            (int)ActorAttributeType.BuffIconEndTick16,
            (int)ActorAttributeType.BuffIconEndTick17,
            (int)ActorAttributeType.BuffIconEndTick18,
            (int)ActorAttributeType.BuffIconEndTick19,
            (int)ActorAttributeType.BuffIconEndTick20,
            (int)ActorAttributeType.BuffIconEndTick21,
            (int)ActorAttributeType.BuffIconEndTick22,
            (int)ActorAttributeType.BuffIconEndTick23,
            (int)ActorAttributeType.BuffIconEndTick24,
            (int)ActorAttributeType.BuffIconEndTick25,
            (int)ActorAttributeType.BuffIconEndTick26,
            (int)ActorAttributeType.BuffIconEndTick27,
            (int)ActorAttributeType.BuffIconEndTick28,
            (int)ActorAttributeType.BuffIconEndTick29,
            (int)ActorAttributeType.BuffIconEndTick30,
            (int)ActorAttributeType.BuffIconEndTick31,
        };
    }


}
