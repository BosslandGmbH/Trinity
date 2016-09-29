using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Routines.Crusader;
using Trinity.Settings.Combat;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardFirebirdArchon : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Wizard Firebird Archon";
        public string Description => "Specialized combat routine for firebird archon S7 Meta";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/wizard-firebird-archon-build-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.FirebirdsFinery, SetBonus.Third },
                { Sets.ChantodosResolve, SetBonus.First }
            },  
        };

        public override KiteMode KiteMode => KiteMode.Always;
        public override float KiteDistance => 15f;

        public override Func<bool> ShouldIgnoreAvoidance => () => IsArchonActive;
        public override Func<bool> ShouldIgnoreKiting => () => IsArchonActive || Player.CurrentHealthPct > 0.3f;

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            var bestCluster = TargetUtil.GetBestClusterPoint();
            var energyLevelReady = !Legendary.AquilaCuirass.IsEquipped || Player.PrimaryResourcePct > 0.95;
            if(bestCluster.Distance(Player.Position) > 15f && CanTeleportTo(bestCluster) && energyLevelReady)
                return Teleport(bestCluster);

            if (IsArchonActive)
            {                
                if (CanTeleport)
                    Teleport(TargetUtil.GetBestClusterPoint());

                if (Skills.Wizard.ArchonDisintegrationWave.CanCast())
                {
                    // Use Wave to pull and ignite monsters that are lined up nicely and are not burning.
                    var pierceUnits = Units.Where(u => u.Distance < 50f && !u.Attributes.HasFirebirdPermanent && !u.Attributes.HasFirebirdTemporary && (u.CountUnitsInFront() + u.CountUnitsBehind(15f)) > 5).ToList();
                    var bestPierceUnit = pierceUnits.OrderBy(u => u.Distance).FirstOrDefault(u => u.Distance <= 15f);
                    if (bestPierceUnit != null)
                        return ArchonDisintegrationWave(bestPierceUnit);
                }

                if (ShouldArchonStrike(out target))
                    return ArchonStrike(target);

                return null;
            }

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

        protected override TrinityPower ArchonDisintegrationWave(TrinityActor target)
            => new TrinityPower(Skills.Wizard.ArchonDisintegrationWave, 15f, target.AcdId, 25, 25);


        protected override bool ShouldExplosiveBlast(out Vector3 position)
        {
            // being cast as a buff instead. 
            position = Vector3.Zero;
            return false;
        }

        protected override bool ShouldArchonStrike(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArchonStrike.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected override bool ShouldArchon()
        {
            if (!Skills.Wizard.Archon.CanCast())
                return false;

            if (Sets.ChantodosResolve.IsFullyEquipped && ChantodosStacks < 20)
                return false;

            if (!TargetUtil.AnyMobsInRange(30f))
                return false;

            return true;
        }

        protected override bool ShouldTeleport(out Vector3 position)
        {
            // Keep the buff up for safe passage.
            // Jump into clusters.

            position = Vector3.Zero;

            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (Skills.Wizard.Teleport.TimeSinceUse < 2000)
                return false;

            if (Legendary.AquilaCuirass.IsEquipped && Player.PrimaryResourcePct < 0.95)
                return false;
    
            var needSafePassageBuff = IsInCombat && Runes.Wizard.SafePassage.IsActive && !Skills.Wizard.Teleport.IsBuffActive;
            var closeEnoughToBestCluster = TargetUtil.GetBestClusterUnit(15f, 50f)?.Distance < 15f;
            if (!needSafePassageBuff && closeEnoughToBestCluster)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetBuffPower()
        {
            if (IsArchonActive && Skills.Wizard.ArchonBlast.CanCast())
                return ArchonBlast();

            if (Skills.Wizard.ExplosiveBlast.CanCast())
                return ExplosiveBlast();

            if (ShouldArchon())
            {
                // Make sure we're in the best spot when entering archon form.
                var explosionPoint = TargetUtil.GetBestClusterPoint();
                var explosionPointDistance = explosionPoint.Distance(Player.Position);
                if (explosionPointDistance > 10f)
                    return Teleport(explosionPoint);

                return Archon();
            }
                          
            return DefaultBuffPower();
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            return ShouldMovementTeleport(destination) ? Teleport(destination) : Walk(destination);
        }

        private bool ShouldMovementTeleport(Vector3 destination)
        {
            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (IsStuck || IsBlocked)
                return true;

            if (!CanTeleportTo(destination))
                return false;

            if (Legendary.AquilaCuirass.IsEquipped && Player.PrimaryResourcePct < 0.95)
                return false;

            if (Skills.Wizard.Teleport.TimeSinceUse < 1000)
                return false;

            return true;
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardFirebirdArchonSettings Settings { get; } = new WizardFirebirdArchonSettings();

        public sealed class WizardFirebirdArchonSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(8)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


