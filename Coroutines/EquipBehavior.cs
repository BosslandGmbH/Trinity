using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Objects;
using Trinity.Reference;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Trinity.Technicals;
using Zeta.Game.Internals.SNO;

namespace Trinity.Coroutines
{
    public class EquipBehavior : BaseObject
    {
        public static EquipBehavior Instance { get { return _instance ?? (_instance = (new EquipBehavior())); } }

        private Dictionary<Skill, Rune> _skills;
        private List<Passive> _passives;
        private static EquipBehavior _instance;

        public EquipBehavior()
        {
            _instance = this;
        }

        public EquipBehavior(Dictionary<Skill, Rune> skills, List<Passive> passives)
        {
            _skills = skills;
            _passives = passives;
        }

        public List<Passive> Passives
        {
            get { return _passives; }
            set { SetField(ref _passives, value); }
        }

        public Dictionary<Skill, Rune> Skills
        {
            get { return _skills; }
            set { SetField(ref _skills, value); }
        }

        public async Task<bool> Execute(Dictionary<Skill, Rune> skills, List<Passive> passives)
        {
            _skills = skills;
            _passives = passives;
            return await Execute();
        }

        private DateTime _lastSkillChange = DateTime.MinValue;

        public async Task<bool> Execute()
        {
            try
            {
                if (ZetaDia.Me == null || ZetaDia.Me.IsDead || ZetaDia.Me.IsGhosted || ZetaDia.Me.IsInCombat || ZetaDia.Me.IsInConversation || ZetaDia.Me.IsInBossEncounter || ZetaDia.Me.LoopingAnimationEndTime != 0)
                {
                    Logger.Log("[Auto Skills] Cannot equip build right now");
                    return false;
                }

                if (DateTime.UtcNow.Subtract(_lastSkillChange).TotalMinutes < 1)
                    return false;

                _lastSkillChange = DateTime.UtcNow;

                if (_skills != null && _skills.Any())
                {
                    await Coroutine.Sleep(250);

                    // Only 'IsPrimary' flagged skills can be assigned to slots 0 and 1

                    KnownSkills = ZetaDia.Me.KnownSkills.ToDictionary(s => s.SNOPower, v => v);

                    CacheData.Hotbar.UpdateHotbarCache();

                    var primarySkills = _skills.Where(s => s.Key.IsPrimary).Take(2).ToList();
                    if (primarySkills.Any())
                    {
                        for (int i = 0; i < Math.Min(primarySkills.Count,2); i++)
                        {
                            var skillDefinition = primarySkills.ElementAtOrDefault(i);
                            if (CacheData.Hotbar.ActiveSkills.Any(s => s.Power == skillDefinition.Key.SNOPower && (int) s.Slot == i))
                            {
                                Logger.LogVerbose("[Auto Skills] Skipping Skill (Already Equipped): {0} ({1}) - {2} in slot {3}",
                                    skillDefinition.Key.Name,
                                    (int)skillDefinition.Key.SNOPower,
                                    skillDefinition.Value.Name,
                                    i);

                                continue;
                            }

                            await EquipSkill(skillDefinition, i);

                            
                            await Coroutine.Sleep(500);
                        }
                    }
                
                    var otherSkills = Skills.Except(primarySkills).ToList();
                    if (otherSkills.Any())
                    {
                        for (int i = 2; i < otherSkills.Count + 2; i++)
                        {
                            await EquipSkill(otherSkills.ElementAtOrDefault(i), i);
                            await Coroutine.Sleep(500);
                        }
                    }                  
                }

                if (Passives == null || !Passives.Any())
                    return false;

                var validPasives = Passives.Where(p => p.Class == ZetaDia.Me.ActorClass && TrinityPlugin.Player.Level >= p.RequiredLevel).ToList();
                var passivePowers = validPasives.Select(p => p.SNOPower).ToList();

                foreach (var passive in validPasives)
                {
                    Logger.LogVerbose("[Auto Skills] Selecting Passive: {0} {1} ({2})", passive.Name, passive.SNOPower.ToString(), (int)passive.SNOPower);
                }

                await Coroutine.Sleep(250);

                switch(validPasives.Count)
                {
                    case 1:
                        ZetaDia.Me.SetTraits(passivePowers[0]);
                        break;

                    case 2:
                        ZetaDia.Me.SetTraits(passivePowers[0], passivePowers[1]);
                        break;

                    case 3:
                        ZetaDia.Me.SetTraits(passivePowers[0], passivePowers[1], passivePowers[2]);
                        break;

                    case 4:
                        ZetaDia.Me.SetTraits(passivePowers[0], passivePowers[1], passivePowers[2], passivePowers[3]);
                        break;
                }

                await Coroutine.Sleep(2000);

            }
            catch (Exception ex)
            {
                Logger.LogError("[Auto Skills] Exception in Build.EquipBuild(). {0}", ex);
            }

            return true;

        }

        public static Dictionary<int, ActiveSkillEntry> KnownSkills = new Dictionary<int, ActiveSkillEntry>();

        public async static Task<bool> EquipSkill(KeyValuePair<Skill, Rune> skill, int slot)
        {
            if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.IsLoadingWorld || ZetaDia.IsPlayingCutscene)
                return false;

            if (skill.Key == null || skill.Value == null || slot < 0)
                return false;

            Logger.LogVerbose("[Auto Skills] Selecting Skill: {0} ({1}) - {2} in slot {3}",
                skill.Key.Name,
                (int)skill.Key.SNOPower,
                skill.Value.Name,
                slot);

            if (skill.Key.Class != ZetaDia.Me.ActorClass || skill.Value.Class != ZetaDia.Me.ActorClass)
            {
                Logger.LogError("[Auto Skills] Attempting to equip skill/rune for the wrong class will crash the game");
                return false;
            }

            var currentLevel = ZetaDia.Me.Level;

            ActiveSkillEntry knownSkillRecord;
            if (!KnownSkills.TryGetValue((int)skill.Key.SNOPower, out knownSkillRecord))
            {
                Logger.LogError("[Auto Skills] Skill is not known", skill.Key.Name, skill.Key.RequiredLevel);
                return false;
            }

            if (currentLevel < knownSkillRecord.RequiredLevel)
            {                
                Logger.LogError("[Auto Skills] Skill {0} cannot be equipped until level {1}", skill.Key.Name, knownSkillRecord.RequiredLevel);
                return false;                
            }

            if (skill.Value.RuneIndex == -1 && currentLevel < knownSkillRecord.RuneNoneRequiredLevel)
            {
                Logger.LogError("[Auto Skills] Skill {0} with Rune {1} cannot be equipped until level {1}", skill.Key.Name, skill.Value.Name, knownSkillRecord.RuneNoneRequiredLevel);
                return false;
            }

            if (skill.Value.RuneIndex == 1 && currentLevel < knownSkillRecord.Rune1RequiredLevel)
            {
                Logger.LogError("[Auto Skills] Skill {0} with Rune {1} cannot be equipped until level {1}", skill.Key.Name, skill.Value.Name, knownSkillRecord.RuneNoneRequiredLevel);
                return false;
            }

            if (skill.Value.RuneIndex == 2 && currentLevel < knownSkillRecord.Rune2RequiredLevel)
            {
                Logger.LogError("[Auto Skills] Skill {0} with Rune {1} cannot be equipped until level {1}", skill.Key.Name, skill.Value.Name, knownSkillRecord.RuneNoneRequiredLevel);
                return false;
            }

            if (skill.Value.RuneIndex == 3 && currentLevel < knownSkillRecord.Rune3RequiredLevel)
            {
                Logger.LogError("[Auto Skills] Skill {0} with Rune {1} cannot be equipped until level {1}", skill.Key.Name, skill.Value.Name, knownSkillRecord.RuneNoneRequiredLevel);
                return false;
            }

            if (skill.Value.RuneIndex == 4 && currentLevel < knownSkillRecord.Rune4RequiredLevel)
            {
                Logger.LogError("[Auto Skills] Skill {0} with Rune {1} cannot be equipped until level {1}", skill.Key.Name, skill.Value.Name, knownSkillRecord.RuneNoneRequiredLevel);
                return false;
            }

            if (skill.Value.RuneIndex == 5 && currentLevel < knownSkillRecord.Rune5RequiredLevel)
            {
                Logger.LogError("[Auto Skills] Skill {0} with Rune {1} cannot be equipped until level {1}", skill.Key.Name, skill.Value.Name, knownSkillRecord.RuneNoneRequiredLevel);
                return false;
            }

            if (currentLevel < skill.Key.RequiredLevel)
            {
                Logger.LogError("[Auto Skills] Skill {0} cannot be equipped until level {1}", skill.Key.Name, skill.Key.RequiredLevel);
                return false;
            }

            //await Coroutine.Sleep(500);

            ZetaDia.Me.SetActiveSkill(skill.Key.SNOPower, skill.Value.RuneIndex, (HotbarSlot)slot);

            await Coroutine.Sleep(500);

            return true;// ZetaDia.PlayerData.GetActiveSkillBySlot((HotbarSlot)slot).Power == skill.Key.SNOPower;
        }


    }
}

