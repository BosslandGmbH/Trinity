using System;
using System.Threading.Tasks;
using Trinity.Components.Combat;
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

        float EmergencyHealthPct { get; }

        // Power Selection
        TrinityPower GetOffensivePower();
        TrinityPower GetDefensivePower();
        TrinityPower GetBuffPower();
        TrinityPower GetDestructiblePower();
        TrinityPower GetMovementPower(Vector3 destination);

        // Hardcore Overrides        
        Task<bool> HandleKiting();
        Task<bool> HandleAvoiding();
        Task<bool> HandleTargetInRange();
        Task<bool> MoveToTarget();
        bool SetWeight(TrinityActor cacheObject);

        // Temporary Overrides        
        Func<bool> ShouldIgnoreNonUnits { get; }
        Func<bool> ShouldIgnorePackSize { get; }
        Func<bool> ShouldIgnoreAvoidance { get; }
        Func<bool> ShouldIgnoreKiting { get; }
        Func<bool> ShouldIgnoreFollowing { get; }

    }

}





