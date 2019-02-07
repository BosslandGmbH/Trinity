using System;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Routines
{
    public interface IRoutine
    {
        string DisplayName { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
        ActorClass Class { get; }
        string Url { get; }
        Build BuildRequirements { get; }
        IDynamicSetting RoutineSettings { get; }

        // Kiting
        KiteMode KiteMode { get; }
        float KiteDistance { get; }
        int KiteStutterDuration { get; }
        int KiteStutterDelay { get; }
        int KiteHealthPct { get; }               

        // Range
        float TrashRange { get; }
        float EliteRange { get; }
        float HealthGlobeRange { get; }
        float ShrineRange { get; }        

        // Cluster
        float ClusterRadius { get; }
        int ClusterSize { get; }

        // Misc
        int PrimaryEnergyReserve { get; }
        int SecondaryEnergyReserve { get; }

        float PotionHealthPct { get; }
        float EmergencyHealthPct { get; }

        // Power Selection
        TrinityPower GetOffensivePower();
        TrinityPower GetDefensivePower();
        TrinityPower GetBuffPower();
        TrinityPower GetDestructiblePower();
        TrinityPower GetMovementPower(Vector3 destination);

        /// <summary>
        /// Called for every actor in order to get a weight - highest will become 'CurrentTarget'
        /// Determines which actor (if any) should be the objective. Switch/Door/Globe/Monster etc
        /// If true: it has been handled for that actor only, false: default weighting is used.
        /// </summary>
        bool SetWeight(TrinityActor cacheObject);

        /// <summary>
        /// Must return a power for use on current target, doors/switches == something to interact, monsters == somethnign to attack
        /// The power is then passed along to the SpellProvider which reads the instructions from TrinityPower and performs the required 
        /// checks, movement and casting and logging.
        /// </summary>
        TrinityPower GetPowerForTarget(TrinityActor target);

        // Listed in order of execution.
        Task<bool> HandleBeforeCombat();
        Task<bool> HandleAvoiding();
        Task<bool> HandleKiting();
        Task<bool> HandleTarget(TrinityActor target);
        Task<bool> HandleOutsideCombat();

        // Temporary Overrides     
        Func<bool> ShouldIgnoreNonUnits { get; }
        Func<bool> ShouldIgnorePackSize { get; }
        Func<bool> ShouldIgnoreAvoidance { get; }
        Func<bool> ShouldIgnoreKiting { get; }
        Func<bool> ShouldIgnoreFollowing { get; }
    }
}

