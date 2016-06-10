using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Trinity.Technicals;

namespace Trinity.Reference
{
    public class PassiveUtils
    {

        /// <summary>
        /// Fast lookup for a Skill by SNOPower
        /// </summary>
        public static Passive ById(SNOPower power)
        {
            if (!_allPassiveBySnoPower.Any())
                _allPassiveBySnoPower = All.ToDictionary(s => s.SNOPower, s => s);

            Passive passive;
            var result = _allPassiveBySnoPower.TryGetValue(power, out passive);
            if (!result)
            {
                Logger.LogDebug("Unable to find passive for power {0}", power);
            }
            return result ? passive : new Passive();
        }
        private static Dictionary<SNOPower, Passive> _allPassiveBySnoPower = new Dictionary<SNOPower, Passive>();

        /// <summary>
        /// All passives that are currently active
        /// </summary>
        public static List<Passive> Active
        {
            get
            {
                if (ZetaDia.PlayerData.IsValid && ZetaDia.IsInGame && (!_active.Any() || DateTime.UtcNow.Subtract(_lastUpdatedActivePassives) > TimeSpan.FromSeconds(3)))
                {
                    _lastUpdatedActivePassives = DateTime.UtcNow;
                    _active.Clear();
                    _active = CurrentClass.Where(p => p.IsActive).ToList();
                }
                return _active;
            }
        }
        private static List<Passive> _active = new List<Passive>();
        private static DateTime _lastUpdatedActivePassives = DateTime.MinValue;

        /// <summary>
        /// All passives
        /// </summary>        
        public static List<Passive> All
        {
            get
            {
                if (!_all.Any())
                {
                    _all.AddRange(Passives.Barbarian.ToList());
                    _all.AddRange(Passives.WitchDoctor.ToList());
                    _all.AddRange(Passives.DemonHunter.ToList());
                    _all.AddRange(Passives.Wizard.ToList());
                    _all.AddRange(Passives.Crusader.ToList());
                    _all.AddRange(Passives.Monk.ToList());
                }
                return _all;
            }
        }
        private static List<Passive> _all = new List<Passive>();


        /// <summary>
        /// All passives for the specified class
        /// </summary>
        public static List<Passive> ByActorClass(ActorClass Class)
        {
            if (ZetaDia.Me.IsValid)
            {
                switch (ZetaDia.Me.ActorClass)
                {
                    case ActorClass.Barbarian:
                        return Passives.Barbarian.ToList();
                    case ActorClass.Crusader:
                        return Passives.Crusader.ToList();
                    case ActorClass.DemonHunter:
                        return Passives.DemonHunter.ToList();
                    case ActorClass.Monk:
                        return Passives.Monk.ToList();
                    case ActorClass.Witchdoctor:
                        return Passives.WitchDoctor.ToList();
                    case ActorClass.Wizard:
                        return Passives.Wizard.ToList();
                }
            }
            return new List<Passive>();
        }

        /// <summary>
        /// Passives for the current class
        /// </summary>
        public static IEnumerable<Passive> CurrentClass
        {
            get { return ZetaDia.Me.IsValid ? ByActorClass(ZetaDia.Me.ActorClass) : new List<Passive>(); }
        }
    }
}
