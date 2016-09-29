using System;
using System.Threading.Tasks;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Routines;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Components.Combat
{
    public interface IRoutineProvider
    {
        IRoutine Current { get; }
    }

    public class DefaultRoutineProvider : IRoutineProvider
    {
        public IRoutine Current => Core.Routines.CurrentRoutine;
    }
    
}
