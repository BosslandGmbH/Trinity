using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Objects;

namespace Trinity.Modules
{
    public class RoutineSelector : Module
    {
        protected override int UpdateIntervalMs => 1000;
        
        protected override void OnPulse()
        {
            Core.Routines.SelectRoutine();
        }
    }
}
