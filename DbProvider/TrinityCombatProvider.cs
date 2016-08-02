using System.Collections.Generic;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.DbProvider
{
    public class TrinityCombatProvider : ITargetingProvider
    {
        public List<DiaObject> GetObjectsByWeight() => new List<DiaObject> { ZetaDia.Actors.GetActorById(CurrentTargetRActorId) };

        public float CurrentCastRange => CurrentPower?.MinimumRange ?? CombatBase.DefaultWeaponDistance;

        public TrinityPower CurrentPower => CombatBase.CurrentPower;

        public TrinityActor CurrentTarget => CombatBase.CurrentTarget;

        public int CurrentTargetRActorId => CurrentTarget?.RActorId ?? -1;

        public bool IsAvoiding => CombatBase.IsCurrentlyAvoiding;

        public bool IsKiting => CombatBase.IsCurrentlyKiting;
    }
}
