using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Reference
{
    public class SkillUtils
    {    
        public static Skill GetSkillByPower(SNOPower power)
        {
            if (!_allSkillBySnoPower.Any())
            {
                CreateSkillsBySnoPowerDictionary();
            }

            Skill skill;
            if (_allSkillBySnoPower.TryGetValue(power, out skill))
                return skill;

            return null;
        }

        private static void CreateSkillsBySnoPowerDictionary()
        {
            foreach (var s in All)
            {
                if (s.SNOPowers != null && s.SNOPowers.Any())
                {
                    foreach (var p in s.SNOPowers)
                    {
                        _allSkillBySnoPower.Add(p, s);
                    }
                }
                else
                {
                    _allSkillBySnoPower.Add(s.SNOPower, s);
                }
            }
        }

        private static Dictionary<SNOPower, Skill> _allSkillBySnoPower = new Dictionary<SNOPower, Skill>();

        public static Skill ByName(string name)
        {
            if (!_allSkillByName.Any())
                _allSkillByName = All.ToDictionary(s => s.Name.ToLowerInvariant(), s => s);

            Skill skill;
            var result = _allSkillByName.TryGetValue(name.ToLowerInvariant(), out skill);
            return result ? skill : new Skill();
        }

        private static Dictionary<string, Skill> _allSkillByName = new Dictionary<string, Skill>();

    
        public static HashSet<SNOPower> AllIds
        {
            get { return _allSNOPowers ?? (_allSNOPowers = new HashSet<SNOPower>(All.Select(s => s.SNOPower))); }
        }

        private static HashSet<SNOPower> _allSNOPowers;

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

        public static void UpdateActiveSkills()
        {
            //Core.Logger.Debug($"UpdateActiveSkills: BotMain.BotThread.ThreadState={BotMain.BotThread?.ThreadState} BotMain.BotThread.IsAlive={BotMain.BotThread?.IsAlive} BotMain.IsRunning={BotMain.IsRunning} BotEvents.IsBotRunning={BotEvents.IsBotRunning}");

            if (ZetaDia.Service.IsInGame)
            {
				using (ZetaDia.Memory.AcquireFrame())
                {
					//Core.Logger.Debug($"UpdateActiveSkills: Before UpdateActors: BotMain.BotThread.ThreadState={BotMain.BotThread?.ThreadState} BotMain.BotThread.IsAlive={BotMain.BotThread?.IsAlive} BotMain.IsRunning={BotMain.IsRunning} BotEvents.IsBotRunning={BotEvents.IsBotRunning}");
					if(!Zeta.Bot.BotMain.IsRunning)
					{
						ZetaDia.Actors.Update();
					}
					//Core.Logger.Debug($"UpdateActiveSkills: Before AcquireFrame: BotMain.BotThread.ThreadState={BotMain.BotThread?.ThreadState} BotMain.BotThread.IsAlive={BotMain.BotThread?.IsAlive} BotMain.IsRunning={BotMain.IsRunning} BotEvents.IsBotRunning={BotEvents.IsBotRunning}");

                    //Core.Logger.Debug($"UpdateActiveSkills: Before Hotbar.Update: BotMain.BotThread.ThreadState={BotMain.BotThread?.ThreadState} BotMain.BotThread.IsAlive={BotMain.BotThread?.IsAlive} BotMain.IsRunning={BotMain.IsRunning} BotEvents.IsBotRunning={BotEvents.IsBotRunning}");

                    Core.Hotbar.Update();
                }
            }
            _lastUpdatedActiveSkills = DateTime.UtcNow;
            _active = CurrentClass.Where(s => Core.Hotbar.ActivePowers.Contains(s.SNOPower)).ToList();
            _activeIds = Core.Hotbar.ActivePowers;
        }

        private static DateTime _lastUpdatedActiveSkills = DateTime.MinValue;

        private static bool ShouldUpdateActiveSkills
        {
            get { return DateTime.UtcNow.Subtract(_lastUpdatedActiveSkills) > TimeSpan.FromMilliseconds(100); }
        }
     
        public static HashSet<SNOPower> CurrentClassIds
        {
            get { return new HashSet<SNOPower>(CurrentClass.Select(s => s.SNOPower)); }
        }
     
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
                    _all.AddRange(Skills.Necromancer.ToList());
                }
                return _all;
            }
        }
        private static List<Skill> _all = new List<Skill>();

        public static List<Skill> ByActorClass(ActorClass Class)
        {
            switch (Class)
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
                case ActorClass.Necromancer:
                    return Skills.Necromancer.ToList();
            }
            return new List<Skill>();
        }

        public static IEnumerable<Skill> CurrentClass
        {
            get { return ZetaDia.Service?.Hero != null ? ByActorClass(ZetaDia.Service.Hero.Class) : new List<Skill>(); }
        }

    }
}
