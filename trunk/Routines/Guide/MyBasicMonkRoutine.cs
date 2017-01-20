//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Trinity.Components.Combat;
//using Trinity.Components.Combat.Resources;
//using Trinity.DbProvider;
//using Trinity.Framework;
//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Framework.Avoidance.Structures;
//using Trinity.Framework.Objects;
//using Trinity.Reference;
//using Trinity.Routines.Monk;
//using Trinity.Settings;
//using Zeta.Bot;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.SNO;
//using Logger = Trinity.Framework.Helpers.Logger;

//namespace Trinity.Routines.Guide
//{
//    public class MyBasicMonkRoutine : MonkBase, IRoutine
//    {
//        public string DisplayName => "Example Monk Routine with no settings";
//        public string Description => "A very simple example build ";
//        public string Author => "xzjv";
//        public string Version => "0.1";
//        public string Url => string.Empty;
//        public Build BuildRequirements => null;
//        public IDynamicSetting RoutineSettings => null;

//        /// <summary>
//        /// Only cast in combat and the target is a unit
//        /// </summary>
//        public TrinityPower GetOffensivePower()
//        {
//            TrinityPower power;

//            if (TrySpecialPower(out power))
//                return power;

//            if (TrySecondaryPower(out power))
//                return power;

//            if (TryPrimaryPower(out power))
//                return power;

//            if (IsNoPrimary)
//                return Walk(CurrentTarget);

//            return null;
//        }

//        /// <summary>
//        /// Only cast when avoiding.
//        /// </summary>
//        public TrinityPower GetDefensivePower()
//        {
//            return GetBuffPower();
//        }

//        /// <summary>
//        /// Cast always, in and out of combat.
//        /// </summary>
//        public TrinityPower GetBuffPower()
//        {
//            return DefaultBuffPower();
//        }

//        /// <summary>
//        /// Only cast on destructibles/barricades
//        /// </summary>
//        public TrinityPower GetDestructiblePower()
//        {
//            return DefaultDestructiblePower();
//        }

//        /// <summary>
//        /// Cast by all plugins for all movement.        
//        /// </summary>
//        public TrinityPower GetMovementPower(Vector3 destination)
//        {
//            return Walk(destination);
//        }


//    }

//}

using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Routines.Monk;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Guide
{
    public class MyBasicMonkRoutine : MonkBase, IRoutine
    {
        public string DisplayName => "Example Monk Routine with no settings";
        public string Description => "A very simple example build ";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => string.Empty;
        public Build BuildRequirements => null;
        public IDynamicSetting RoutineSettings => null;

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
                return Walk(CurrentTarget);

            return null;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetBuffPower() => DefaultBuffPower();

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination) => Walk(destination);

    }

}

