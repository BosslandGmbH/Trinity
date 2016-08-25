using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Components.Combat
{
    public class CombatManager
    {
        public static Weighting Weighting { get;  } = new Weighting();

        //public static HandleTarget TargetHandler { get; } = new HandleTarget();

        public static TargetHandler TargetHandler { get; } = new TargetHandler();

        public static AbilitySelector AbilitySelector { get; } = new AbilitySelector();

    }
}
