using System.Collections.Generic;
using Trinity.Combat;
using Trinity.Combat.Abilities;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;

namespace Trinity.DbProvider
{
    /// <summary>Combat Targeting Provider </summary>
    public class TrinityCombatProvider : ITargetingProvider
    {
        private static readonly List<DiaObject> targetList = new List<DiaObject>();

        /// <summary> Gets list of target in range by weight.</summary>
        /// <returns>Blank list of target, TrinityPlugin don't use this Db process.</returns>
        public List<DiaObject> GetObjectsByWeight()
        {
            var list = new List<DiaObject>();
            if (TrinityPlugin.CurrentTarget != null && TrinityPlugin.CurrentTarget.Object != null && TrinityPlugin.CurrentTarget.Object.IsValid)
                list.Add(TrinityPlugin.CurrentTarget.Object);
            return list;
        }

        public float CurrentCastRange
        {
            get { return CurrentPower != null ? CurrentPower.MinimumRange : CombatBase.DefaultWeaponDistance; }
        }

        public TrinityPower CurrentPower
        {
            get { return CombatBase.CurrentPower; }
        }

        public TrinityCacheObject CurrentTarget
        {
            get { return CombatBase.CurrentTarget; }
        }

        public bool IsAvoiding
        {
            get { return CombatBase.IsCurrentlyAvoiding; }
        }

        public bool IsKiting
        {
            get { return CombatBase.IsCurrentlyKiting; }
        }
    }
}
