using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Reference
{
    class RuneUtils
    {
        /// <summary>
        /// All runes that are currently active
        /// </summary>
        public static List<Rune> Active
        {
            get
            {
                if (ZetaDia.PlayerData.IsValid && ZetaDia.IsInGame && (!_active.Any() || DateTime.UtcNow.Subtract(_lastUpdatedActiveRunes) > TimeSpan.FromSeconds(3)))
                {
                    _lastUpdatedActiveRunes = DateTime.UtcNow;
                    _active.Clear();
                    _active = SkillUtils.Active.SelectMany(s => s.Runes).Where(r => r.IsActive).ToList();
                }
                return _active;
            }
        }
        private static List<Rune> _active = new List<Rune>();
        private static DateTime _lastUpdatedActiveRunes = DateTime.MinValue;


        /// <summary>
        /// All runes
        /// </summary>        
        public static List<Rune> All
        {
            get
            {
                if (!_all.Any())
                {
                    _all.AddRange(Runes.Barbarian.ToList());
                    _all.AddRange(Runes.WitchDoctor.ToList());
                    _all.AddRange(Runes.DemonHunter.ToList());
                    _all.AddRange(Runes.Wizard.ToList());
                    _all.AddRange(Runes.Crusader.ToList());
                    _all.AddRange(Runes.Monk.ToList());
                }
                return _all;
            }
        }
        private static List<Rune> _all = new List<Rune>();

        /// <summary>
        /// All skills for the specified class
        /// </summary>
        public static List<Rune> ByActorClass(ActorClass Class)
        {
            if (ZetaDia.Me.IsValid)
            {
                switch (ZetaDia.Me.ActorClass)
                {
                    case ActorClass.Barbarian:
                        return Runes.Barbarian.ToList();
                    case ActorClass.Crusader:
                        return Runes.Crusader.ToList();
                    case ActorClass.DemonHunter:
                        return Runes.DemonHunter.ToList();
                    case ActorClass.Monk:
                        return Runes.Monk.ToList();
                    case ActorClass.Witchdoctor:
                        return Runes.WitchDoctor.ToList();
                    case ActorClass.Wizard:
                        return Runes.Wizard.ToList();
                }
            }
            return new List<Rune>();
        }

        /// <summary>
        /// Skills for the current class
        /// </summary>
        public static IEnumerable<Rune> CurrentClass
        {
            get { return ZetaDia.Me.IsValid ? ByActorClass(ZetaDia.Me.ActorClass) : new List<Rune>(); }
        }


        /// <summary>
        /// Convert a D3 rune index to a proper rune index (order they are listed in d3 skills UI)
        /// </summary>
        /// <returns>-999 on failure</returns> 
        public static int GetProperRuneIndex(int dbRuneIndex, SNOPower power)
        {
            var firstOrDefault = SkillUtils.ById(power).Runes.FirstOrDefault(r => r.RuneIndex == dbRuneIndex);
            if (firstOrDefault != null) return firstOrDefault.Index;
            return -999;
        }

        /// <summary>
        /// Convert a proper rune index (order they are listed in d3 skills UI) to D3 RuneIndex        
        /// </summary>
        /// <returns>-999 on failure</returns>
        public static int GetDBRuneIndex(int properRuneIndex, SNOPower power)
        {
            var firstOrDefault = SkillUtils.ById(power).Runes.FirstOrDefault(r => r.Index == properRuneIndex);
            if (firstOrDefault != null) return firstOrDefault.RuneIndex;
            return -999;
        }
    }
}
