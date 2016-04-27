using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public partial class CacheData
    {
        /// <summary>
        /// Fast Hotbar Cache, Self-Updating, use instead of ZetaDia.PlayerData / Trinity.Hotbar
        /// </summary>
        public class HotbarCache
        {
            static HotbarCache()
            {
                Pulsator.OnPulse += (sender, args) => Instance.UpdateHotbarCache();
            }

            public HotbarCache()
            {
                UpdateHotbarCache();
            }

            private static HotbarCache _instance;
            public static HotbarCache Instance
            {
                get { return _instance ?? (_instance = new HotbarCache()); }
                set { _instance = value; }
            }

            public class HotbarSkill
            {
                public Skill Skill { get; set; }
                public HotbarSlot Slot { get; set; }
                public SNOPower Power { get; set; }
                public int RuneIndex { get; set; }
                public bool HasRuneEquipped { get; set; }
                public Rune Rune { get { return Skill.CurrentRune; } }
                public int Charges
                {
                    get
                    {
                        try
                        {
                            return ZetaDia.Me.CommonData.GetAttribute<int>(((int)Power << 12) + ((int)ActorAttributeType.SkillCharges & 0xFFF));
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
                    return String.Format("Power: {0}, SRune: {1}, Charge:{2}, Slot:{3}", Power, Rune, Charges, Slot);
                }
            }

            public HashSet<SNOPower> ActivePowers { get; private set; }
            public List<HotbarSkill> ActiveSkills { get; private set; }
            public HashSet<SNOPower> PassiveSkills { get; private set; }
            public DateTime LastUpdated = DateTime.MinValue;

            private static Dictionary<SNOPower, HotbarSkill> _skillBySNOPower = new Dictionary<SNOPower, HotbarSkill>();
            private static Dictionary<HotbarSlot, HotbarSkill> _skillBySlot = new Dictionary<HotbarSlot, HotbarSkill>();

            public static bool IsArchonActive
            {
                get { return Instance.ActivePowers.Any(p => DataDictionary.ArchonSkillIds.Contains((int)p)); }
            }

            internal void UpdateHotbarCache()
            {
                using (new PerformanceLogger("UpdateCachedHotbarData"))
                {
                    var lastUpdateMs = DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds;

                    if (lastUpdateMs <= 250)
                        return;

                    Clear();

                    try
                    {
                        RefreshHotbar();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Exception grabbing Hotbar data.{0}{1}", Environment.NewLine, ex);
                    }
                }
            }     

            private void RefreshHotbar()
            {
                using (new PerformanceLogger("RefreshHotbar"))
                {
                    var cPlayer = ZetaDia.PlayerData;

                    LastUpdated = DateTime.UtcNow;

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
                            Slot = (HotbarSlot)i,
                            RuneIndex = runeIndex,
                            HasRuneEquipped = diaActiveSkill.HasRuneEquipped,
                            Skill = SkillUtils.ById(power),
                            //Charges = ZetaDia.Me.CommonData.GetAttribute<int>(((int)diaActiveSkill.Power << 12) + ((int)ActorAttributeType.SkillCharges & 0xFFF)),
                        };

                        ActivePowers.Add(power);
                        ActiveSkills.Add(hotbarskill);
                        _skillBySNOPower.Add(power, hotbarskill);
                        _skillBySlot.Add((HotbarSlot)i, hotbarskill);



                        if (!DataDictionary.LastUseAbilityTimeDefaults.ContainsKey(power))
                            DataDictionary.LastUseAbilityTimeDefaults.Add(power, DateTime.MinValue);

                        if (!AbilityLastUsed.ContainsKey(power))
                            AbilityLastUsed.Add(power, DateTime.MinValue);

                    }

                    Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                        "Refreshed Hotbar: ActiveSkills={0} PassiveSkills={1}",
                        ActiveSkills.Count,
                        PassiveSkills.Count);

                }
            }

            internal HotbarSkill GetSkill(SNOPower power)
            {
                HotbarSkill skill;
                return _skillBySNOPower.TryGetValue(power, out skill) ? skill : new HotbarSkill();
            }

            internal HotbarSkill GetSkill(HotbarSlot slot)
            {
                HotbarSkill skill;
                return _skillBySlot.TryGetValue(slot, out skill) ? skill : new HotbarSkill();
            }

            public void Dump()
            {
                //using (new MemoryHelper())
                //{
                //    foreach (var hotbarskill in ActiveSkills.ToList())
                //    {
                //        Logger.Log("Power={0} SkillName={1} Slot={2} DBRuneIndex={3} ProperRuneIndex={4} RuneName={5}",
                //            hotbarskill.Power,
                //            hotbarskill.Skill.Name,
                //            hotbarskill.Slot,
                //            hotbarskill.RuneIndex,
                //            hotbarskill.Rune.Index,
                //            hotbarskill.Rune.Name);
                //    }
                //}
            }

            public int GetSkillStacks(int id)
            {
                return GetSkill((SNOPower)id).Charges;
            }

            public int GetSkillCharges(SNOPower id)
            {
                return GetSkillStacks((int)id);
            }
            public void Clear()
            {
                LastUpdated = DateTime.MinValue;
                ActivePowers = new HashSet<SNOPower>();
                ActiveSkills = new List<HotbarSkill>();
                PassiveSkills = new HashSet<SNOPower>();
                _skillBySNOPower = new Dictionary<SNOPower, HotbarSkill>();
                _skillBySlot = new Dictionary<HotbarSlot, HotbarSkill>();
            }
        }
    }
}
