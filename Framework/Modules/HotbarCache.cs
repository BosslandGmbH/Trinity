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
    public class HotbarCache : Module
    {
        public class HotbarSkill
        {
            public Skill Skill { get; set; }
            public HotbarSlot Slot { get; set; }
            public SNOPower Power { get; set; }
            public int RuneIndex { get; set; }
            public bool HasRuneEquipped { get; set; }

            public Rune Rune
            {
                get { return Skill.CurrentRune; }
            }

            public int Charges
            {
                get
                {
                    try
                    {
                        return ZetaDia.Me.CommonData.GetAttribute<int>(((int) Power << 12) + ((int) ActorAttributeType.SkillCharges & 0xFFF));
                    }
                    catch
                    {
                        Logger.LogError("Exception getting Charges for Power {0}", Power);
                        return 0;
                    }
                }
            }

            public override string ToString()
            {
                return $"Power: {Power}, SRune: {Rune}, Charge:{Charges}, Slot:{Slot}";
            }
        }


        public HashSet<SNOPower> ActivePowers { get; private set; }
        public List<HotbarSkill> ActiveSkills { get; private set; }
        public HashSet<SNOPower> PassiveSkills { get; private set; }

        private Dictionary<SNOPower, HotbarSkill> _skillBySnoPower = new Dictionary<SNOPower, HotbarSkill>();

        private Dictionary<HotbarSlot, HotbarSkill> _skillBySlot = new Dictionary<HotbarSlot, HotbarSkill>();

        public bool IsArchonActive
        {
            get { return ActivePowers.Any(p => DataDictionary.ArchonSkillIds.Contains((int) p)); }
        }

        protected override void OnPulse()
        {
            Update();
        }

        internal void Update()
        {
            Clear();

            var cPlayer = ZetaDia.PlayerData;

            PassiveSkills = new HashSet<SNOPower>(cPlayer.PassiveSkills);

            for (int i = 0; i <= 5; i++)
            {
                var diaActiveSkill = cPlayer.GetActiveSkillByIndex(i, ZetaDia.Me.SkillOverrideActive);
                if (diaActiveSkill == null || diaActiveSkill.Power == SNOPower.None)
                    continue;

                var power = diaActiveSkill.Power;
                var runeIndex = diaActiveSkill.RuneIndex;

                var hotbarskill = new HotbarSkill
                {
                    Power = diaActiveSkill.Power,
                    Slot = (HotbarSlot) i,
                    RuneIndex = runeIndex,
                    HasRuneEquipped = diaActiveSkill.HasRuneEquipped,
                    Skill = SkillUtils.ById(power),
                };

                if (!ActivePowers.Contains(power))
                    ActivePowers.Add(power);
                ActiveSkills.Add(hotbarskill);
                _skillBySnoPower.Add(power, hotbarskill);
                _skillBySlot.Add((HotbarSlot) i, hotbarskill);

                if (!DataDictionary.LastUseAbilityTimeDefaults.ContainsKey(power))
                    DataDictionary.LastUseAbilityTimeDefaults.Add(power, DateTime.MinValue);
            }

            Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                "Refreshed Hotbar: ActiveSkills={0} PassiveSkills={1}",
                ActiveSkills.Count,
                PassiveSkills.Count);
        }

        internal HotbarSkill GetSkill(SNOPower power)
        {
            HotbarSkill skill;
            return _skillBySnoPower.TryGetValue(power, out skill) ? skill : new HotbarSkill();
        }

        internal HotbarSkill GetSkill(HotbarSlot slot)
        {
            HotbarSkill skill;
            return _skillBySlot.TryGetValue(slot, out skill) ? skill : new HotbarSkill();
        }

        public int GetSkillStacks(int id)
        {
            return GetSkill((SNOPower) id).Charges;
        }

        public int GetSkillCharges(SNOPower id)
        {
            return GetSkillStacks((int) id);
        }

        public void Clear()
        {
            ActivePowers = new HashSet<SNOPower>();
            ActiveSkills = new List<HotbarSkill>();
            PassiveSkills = new HashSet<SNOPower>();
            _skillBySnoPower = new Dictionary<SNOPower, HotbarSkill>();
            _skillBySlot = new Dictionary<HotbarSlot, HotbarSkill>();
        }
    }
}
