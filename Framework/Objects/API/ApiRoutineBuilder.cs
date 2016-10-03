using System;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;

namespace Trinity.Framework.Objects.Api
{
    public class ApiRoutineBuilder
    {
        public static ApiRoutine CreateRoutine()
        {
            return new ApiRoutine
            {
                InternalName = Core.Routines.CurrentRoutine?.GetType().Name,
                DisplayName = Core.Routines.CurrentRoutine?.DisplayName
            };
        }
    }
}

