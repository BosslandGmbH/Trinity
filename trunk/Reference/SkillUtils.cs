using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Objects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Reference
{
    public class SkillUtils
    {
        #region SkillMeta

        /// <summary>
        /// Dictionary mapping Skills to SkillMeta
        /// </summary>
        private static Dictionary<Skill, SkillMeta> _skillMetas = new Dictionary<Skill, SkillMeta>();

        /// <summary>
        /// Set skill to use an SkillMeta object
        /// </summary>
        public static void SetSkillMeta(SkillMeta newMeta)
        {
            if (newMeta.Skill == null)
            {
                Logger.Log("SkillInfo set attempt without a reference to a skill");
                return;
            }

            SkillMeta oldMeta;
            if (_skillMetas.TryGetValue(newMeta.Skill, out oldMeta))
            {
                oldMeta.Apply(newMeta);
            }
            else
            {
                _skillMetas.Add(newMeta.Skill, newMeta);
            }
        }

        /// <summary>
        /// Set skills to use SkillMeta objects
        /// </summary>
        public static void SetSkillMeta(IEnumerable<SkillMeta> metas)
        {
            metas.ForEach(SetSkillMeta);
        }

        /// <summary>
        /// Get a SkillMeta object
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static SkillMeta GetSkillMeta(Skill skill)
        {
            SkillMeta s;
            if (_skillMetas.TryGetValue(skill, out s))
                return s;

            //Logger.LogVerbose("GetSkillInfo found no SkillMeta for {0}", skill.Name);
            
            var newMeta = new SkillMeta(skill);
            SetSkillMeta(newMeta);

            return newMeta;
        }

        #endregion

        /// <summary>
        /// Fast lookup for a Skill by SNOPower
        /// </summary>
        public static Skill ById(SNOPower power)
        {
            if (!_allSkillBySnoPower.Any())
                _allSkillBySnoPower = All.ToDictionary(s => s.SNOPower, s => s);

            Skill skill;
            var result = _allSkillBySnoPower.TryGetValue(power, out skill);
            if (!result)
            {
                //Logger.LogDebug("Unable to find skill for power {0}", power);
            }
            return result ? skill : new Skill();
        }
        private static Dictionary<SNOPower, Skill> _allSkillBySnoPower = new Dictionary<SNOPower, Skill>();

        /// <summary>
        /// Fast lookup for a Skill by SNOPower
        /// </summary>
        public static Skill ByName(string name)
        {
            if (!_allSkillByName.Any())
                _allSkillByName = All.ToDictionary(s => s.Name.ToLowerInvariant(), s => s);

            Skill skill;
            var result = _allSkillByName.TryGetValue(name.ToLowerInvariant(), out skill);
            if (!result)
            {
                //Logger.LogDebug("Unable to find skill for power {0}", name);
            }
            return result ? skill : new Skill();
        }
        private static Dictionary<string, Skill> _allSkillByName = new Dictionary<string, Skill>();

        /// <summary>
        /// All SNOPowers
        /// </summary>        
        public static HashSet<SNOPower> AllIds
        {
            get { return _allSNOPowers ?? (_allSNOPowers = new HashSet<SNOPower>(All.Select(s => s.SNOPower))); }
        }

        private static HashSet<SNOPower> _allSNOPowers;

        /// <summary>
        /// All skills that are currently active
        /// </summary>
        public static List<Skill> Active
        {
            get
            {
                if (!_active.Any() || ShouldUpdateActiveSkills)
                    UpdateActiveSkills();

                return _active;
            }
        }

        private static List<Skill> _active = new List<Skill>();

        /// <summary>
        /// All skills that are currently active, as SNOPower
        /// </summary>
        public static HashSet<SNOPower> ActiveIds
        {
            get
            {
                if (!_activeIds.Any() || ShouldUpdateActiveSkills)
                    UpdateActiveSkills();

                return _activeIds;
            }
        }

        private static HashSet<SNOPower> _activeIds = new HashSet<SNOPower>();

        /// <summary>
        /// Refresh active skills collections with the latest data
        /// </summary>
        private static void UpdateActiveSkills()
        {
            _lastUpdatedActiveSkills = DateTime.UtcNow;
            _active = CurrentClass.Where(s => Core.Hotbar.ActivePowers.Contains(s.SNOPower)).ToList();
            _activeIds = Core.Hotbar.ActivePowers;
        }

        private static DateTime _lastUpdatedActiveSkills = DateTime.MinValue;

        /// <summary>
        /// Check time since last update of active skills
        /// </summary>
        private static bool ShouldUpdateActiveSkills
        {
            get { return DateTime.UtcNow.Subtract(_lastUpdatedActiveSkills) > TimeSpan.FromMilliseconds(250); }
        }

        /// <summary>
        /// All possible skills, as SNOPower
        /// </summary>        
        public static HashSet<SNOPower> CurrentClassIds
        {
            get { return new HashSet<SNOPower>(CurrentClass.Select(s => s.SNOPower)); }
        }

        /// <summary>
        /// All skills
        /// </summary>        
        public static List<Skill> All
        {
            get
            {
                if (!_all.Any())
                {
                    _all.AddRange(Skills.Barbarian.ToList());
                    _all.AddRange(Skills.WitchDoctor.ToList());
                    _all.AddRange(Skills.DemonHunter.ToList());
                    _all.AddRange(Skills.Wizard.ToList());
                    _all.AddRange(Skills.Crusader.ToList());
                    _all.AddRange(Skills.Monk.ToList());
                }
                return _all;
            }
        }
        private static List<Skill> _all = new List<Skill>();

        /// <summary>
        /// All skills for the specified class
        /// </summary>
        public static List<Skill> ByActorClass(ActorClass Class)
        {
            if (ZetaDia.Me.IsValid)
            {
                switch (ZetaDia.Me.ActorClass)
                {
                    case ActorClass.Barbarian:
                        return Skills.Barbarian.ToList();
                    case ActorClass.Crusader:
                        return Skills.Crusader.ToList();
                    case ActorClass.DemonHunter:
                        return Skills.DemonHunter.ToList();
                    case ActorClass.Monk:
                        return Skills.Monk.ToList();
                    case ActorClass.Witchdoctor:
                        return Skills.WitchDoctor.ToList();
                    case ActorClass.Wizard:
                        return Skills.Wizard.ToList();
                }
            }
            return new List<Skill>();
        }

        /// <summary>
        /// Skills for the current class
        /// </summary>
        public static IEnumerable<Skill> CurrentClass
        {
            get { return ZetaDia.Me.IsValid ? ByActorClass(ZetaDia.Me.ActorClass) : new List<Skill>(); }
        }

    }
}
