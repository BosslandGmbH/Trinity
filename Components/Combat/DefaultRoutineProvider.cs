using Trinity.Framework;
using Trinity.Routines;

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