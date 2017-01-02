using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Modules
{
    public class HotbarCache : Module
    {
        protected override int UpdateIntervalMs => 50;

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


        public HashSet<SNOPower> ActivePowers { get; private set; } = new HashSet<SNOPower>();
        public List<HotbarSkill> ActiveSkills { get; private set; } = new List<HotbarSkill>();
        public HashSet<SNOPower> PassivePowers { get; private set; } = new HashSet<SNOPower>();

        private Dictionary<SNOPower, HotbarSkill> _hotbarSkillBySnoPower = new Dictionary<SNOPower, HotbarSkill>();

        private Dictionary<HotbarSlot, HotbarSkill> _skillBySlot = new Dictionary<HotbarSlot, HotbarSkill>();

        public bool IsArchonActive
        {
            get { return ActivePowers.Any(p => GameData.ArchonSkillIds.Contains((int) p)); }
        }

        protected override void OnPulse()
        {
            Update();
        }

        internal void Update()
        {
            Clear();

            if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                return;

            var cPlayer = ZetaDia.PlayerData;
            if (cPlayer == null || !cPlayer.IsValid)
                return;

            PassivePowers = new HashSet<SNOPower>(cPlayer.PassiveSkills);

            var activePowers = new HashSet<SNOPower>();
            var activeHotbarSkills = new List<HotbarSkill>();

            bool isOverrideActive = false;
            try
            {
                isOverrideActive = ZetaDia.Me.SkillOverrideActive;
            }
            catch (ArgumentException ex)
            {
                // nesox?

                /*[Trinity 2.55.238] Error in OnEnable: System.ArgumentException: An item with the same key has already been added.
                   at System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)
                   at System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
                   at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)
                   at Zeta.Game.Internals.FastAttribGroupsEntry.‌⁫‭⁫⁪⁮⁮⁮⁪‌‮‌‪‭‬‌‍‍‫‬‏‌⁫‏‌‏⁯⁯‬⁭‌‪⁬⁫‬⁫⁪⁭‬‭‮()
                   at Zeta.Game.PerFrameCachedValue`1.get_Value()
                   at Zeta.Game.Internals.FastAttribGroupsEntry.get_AttributeMapA()
                   at Zeta.Game.Internals.FastAttribGroupsEntry.⁯⁫‌‭⁬‫‪⁬‮⁪⁬⁭⁮‪⁮⁭‬‮‬⁪‪⁪⁭‮‏​‎⁯‌‎‎⁭⁯‬⁪⁪‫‎‫⁭‮[](Int32 , ACD , & )
                   at Zeta.Game.Internals.FastAttribGroupsEntry.‏⁫⁫​⁮‏⁮​‍⁮⁪⁯⁪⁯‍‫‏‎⁫‌‬⁫⁯⁮‮⁮​‪⁭‎‏‌⁮‎⁬‮‬‍​‪‮[](Int32 , ACD )
                   at Zeta.Game.Internals.Actors.ACD.‌‍‬‌⁭⁫‪⁭⁭⁫‍‪‪‭⁯‍⁯‏⁪‭‪‮‬‮⁫‭⁫‫‫‫‭‎⁬‫‫​‪‭⁬⁫‮()
                   at Zeta.Game.PerFrameCachedValue`1.get_Value()
                   at Zeta.Game.Internals.Actors.ACD.get_SkillOverrideActive()
                   at Zeta.Game.Internals.Actors.DiaPlayer.get_SkillOverrideActive()
                   at Trinity.Framework.Modules.HotbarCache.Update()
                   at Trinity.Framework.Core.Enable()
                   at Trinity.TrinityPlugin.OnEnabled()*/
            }

            for (int i = 0; i <= 5; i++)
            {
                var diaActiveSkill = cPlayer.GetActiveSkillByIndex(i, isOverrideActive);
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
                    Skill = SkillUtils.GetSkillByPower(power),
                };

                if (!activePowers.Contains(power))
                {
                    activePowers.Add(power);
                    activeHotbarSkills.Add(hotbarskill);
                    _hotbarSkillBySnoPower.Add(power, hotbarskill);
                    _skillBySlot.Add((HotbarSlot)i, hotbarskill);
                }

                if (!GameData.LastUseAbilityTimeDefaults.ContainsKey(power))
                    GameData.LastUseAbilityTimeDefaults.Add(power, DateTime.MinValue);
            }

            ActivePowers = activePowers;
            ActiveSkills = activeHotbarSkills;

            Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                "Refreshed Hotbar: ActiveSkills={0} PassiveSkills={1}",
                ActiveSkills.Count,
                PassivePowers.Count);
        }

        internal HotbarSkill GetHotbarSkill(SNOPower power)
        {
            HotbarSkill skill;
            return _hotbarSkillBySnoPower.TryGetValue(power, out skill) ? skill : new HotbarSkill();
        }

        internal HotbarSkill GetHotbarSkill(HotbarSlot slot)
        {
            HotbarSkill skill;
            return _skillBySlot.TryGetValue(slot, out skill) ? skill : new HotbarSkill();
        }

        public int GetSkillStacks(int id)
        {
            return GetHotbarSkill((SNOPower) id).Charges;
        }

        public int GetSkillCharges(SNOPower id)
        {
            return GetSkillStacks((int) id);
        }

        public void Clear()
        {
            ActivePowers = new HashSet<SNOPower>();
            ActiveSkills = new List<HotbarSkill>();
            PassivePowers = new HashSet<SNOPower>();
            _hotbarSkillBySnoPower = new Dictionary<SNOPower, HotbarSkill>();
            _skillBySlot = new Dictionary<HotbarSlot, HotbarSkill>();
        }
    }
}
