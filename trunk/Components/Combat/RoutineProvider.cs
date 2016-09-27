using System;
using System.Threading.Tasks;
using Trinity.Components.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Objects;
using Trinity.Routines;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Components.Combat
{
    public interface IRoutineProvider
    {
        IRoutine Current { get; }
    }

    public class RoutineProvider : IRoutineProvider
    {
        public IRoutine Current => Core.Routines.CurrentRoutine;
    }
    
}
