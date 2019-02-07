using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardManaldCoE : WizardBase, IRoutine
    {
        #region RoutineID
        public string DisplayName => "Wizard Manald CoE";
        public string Description => "Vyr-Rasha Manald Wizard with CoE Support";
        public string Author => "Sequence";
        public string Version => "0.2.1b";
        public string Url => "http://www.diablofans.com/builds/89144-10-2-5-gr-push-coe-manald-archon-wizard-gr112";
        #endregion

        #region Build
        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.VyrsAmazingArcana, SetBonus.Second },
                { Sets.TalRashasElements, SetBonus.Third }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Wizard.MagicMissile, Runes.Wizard.GlacialSpike },
                { Skills.Wizard.BlackHole, Runes.Wizard.Spellsteal },
                { Skills.Wizard.Archon, Runes.Wizard.Combustion },
                { Skills.Wizard.EnergyArmor, Runes.Wizard.ForceArmor },
                { Skills.Wizard.Teleport, Runes.Wizard.Calamity },
                { Skills.Wizard.MagicWeapon, Runes.Wizard.Deflection }
            },
            Passives = new List<Passive>
            {
                { Passives.Wizard.Evocation },
                { Passives.Wizard.UnstableAnomaly },
                { Passives.Wizard.Paralysis }
            },
            Items = new List<Item>
            {
                { Legendary.ManaldHeal },
                { Legendary.ConventionOfElements },
                { Legendary.Starfire },
                { Legendary.AetherWalker },
                { Legendary.TheSwami },
                { Legendary.RingOfRoyalGrandeur },
                { Legendary.FazulasImprobableChain }
            },
        };
        #endregion

        #region Fine Tuning
        public override KiteMode KiteMode => KiteMode.Never;
        /*public override float KiteDistance => IsArchonActive ? 25f : 45f;*/
        /*public override int KiteHealthPct => IsArchonActive ? 45 : 100;*/
        public override Func<bool> ShouldIgnoreAvoidance => () => false;
        #endregion

        #region BuffPower
        public TrinityPower GetBuffPower()
        {
            var archonST = Skills.Wizard.ArchonSlowTime;
            if (!IsArchonSlowTimeActive && archonST.IsActive && archonST.CanCast())
            {
                Core.Logger.Log(LogCategory.Spells, $"Buff ArchonSlowTime");
                return new TrinityPower(archonST);
            }

            var energyArmor = Skills.Wizard.EnergyArmor;
            var magicWeapon = Skills.Wizard.MagicWeapon;
            if (energyArmor.IsActive && !energyArmor.IsBuffActive && energyArmor.CanCast() && Core.Player.PrimaryResourcePct > 0.5f)
            {
                Core.Logger.Log(LogCategory.Spells, $"Buff EnergyArmor={energyArmor.IsBuffActive}");
                return new TrinityPower(energyArmor);
            }
            if (magicWeapon.IsActive && !magicWeapon.IsBuffActive && magicWeapon.CanCast() && Core.Player.PrimaryResourcePct > 0.5f)
            {
                Core.Logger.Log(LogCategory.Spells, $"Buff MagicWeapon={magicWeapon.IsBuffActive}");
                return new TrinityPower(magicWeapon);
            }

            return null;
        }

        protected override bool ShouldArchonBlast()
        {
            var archonBlast = Skills.Wizard.ArchonBlast;
            if (!archonBlast.CanCast())
                return false;
            if (archonBlast.CanCast())
            {
                if (!Player.IsInTown)
                {
                    if (Player.IsMoving)
                        return true;
                    if (Skills.Wizard.ArchonDisintegrationWave.IsDamaging && Skills.Wizard.ArchonDisintegrationWave.TimeSinceUse > 1000)
                        return false;
                    if (!Skills.Wizard.ArchonDisintegrationWave.IsDamaging && TargetUtil.AnyMobsInRange(10, 3, true))
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region DefensivePower
        public TrinityPower GetDefensivePower() => null;
        #endregion

        #region DestructiblePower
        public TrinityPower GetDestructiblePower()
        {
            if (CurrentTarget.IsCorruptGrowth)
            {
                Core.Logger.Log(LogCategory.Targetting, $"It's a {CurrentTarget.ActorSnoId}!");
                return GetOffensivePower();
            }
            return DefaultDestructiblePower();
        }
        #endregion

        #region MovementPower
        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport) && CanTeleportTo(destination) && destination.Distance(Core.Player.Position) > 15)
            {
                Core.Logger.Log(LogCategory.Movement, $"Moving method <Coord:{destination}>, Reason: Movement");
                return Teleport(destination);
            }

            return Walk(destination);
        }
        #endregion

        #region OffensivePower
        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            var target = CurrentTarget;
            Vector3 position = Vector3.Zero;

                if (Skills.Wizard.ArchonTeleport.CanCast() || Skills.Wizard.Teleport.CanCast())
                {
                    if (!Core.Buffs.HasInvulnerableShrine && Core.Avoidance.InAvoidance(Core.Player.Position))
                    {
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, Player.Position);

                        if (position == Vector3.Zero)
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15, 50, Player.Position);
                    }
                if (!Core.Buffs.HasInvulnerableShrine)
                {
                    if (IsArchonActive)
                    {
                        if (TargetUtil.AnyElitesInRange(10) && Skills.Wizard.ArchonTeleport.TimeSinceUse > 2000)
                        {
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, CurrentTarget.Position);

                            if (position == Vector3.Zero)
                                Core.Avoidance.Avoider.TryGetSafeSpot(out position, 20, 50, CurrentTarget.Position);
                        }
                        if (TargetUtil.AnyMobsInRange(15, 3) && Skills.Wizard.ArchonTeleport.TimeSinceUse > 3000)
                        {
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, CurrentTarget.Position);

                            if (position == Vector3.Zero)
                                Core.Avoidance.Avoider.TryGetSafeSpot(out position, 20, 50, CurrentTarget.Position);
                        }
                        if (Player.CurrentHealthPct < 1 && Skills.Wizard.ArchonTeleport.TimeSinceUse > 2000)
                        {
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, Player.Position);

                            if (position == Vector3.Zero)
                                Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15, 50, Player.Position);
                        }
                    }
                    if (!IsArchonActive)
                    {
                        if (TargetUtil.AnyElitesInRange(10) && Skills.Wizard.Teleport.TimeSinceUse > 1200)
                        {
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, CurrentTarget.Position);

                            if (position == Vector3.Zero)
                                Core.Avoidance.Avoider.TryGetSafeSpot(out position, 20, 50, CurrentTarget.Position);
                        }

                        if (TargetUtil.AnyMobsInRange(15, 3) && Skills.Wizard.Teleport.TimeSinceUse > 1200)
                        {
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, CurrentTarget.Position);

                            if (position == Vector3.Zero)
                                Core.Avoidance.Avoider.TryGetSafeSpot(out position, 20, 50, CurrentTarget.Position);
                        }
                        if (Player.CurrentHealthPct < 1 && Skills.Wizard.Teleport.TimeSinceUse > 1200)
                        {
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, Player.Position);

                            if (position == Vector3.Zero)
                                Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15, 50, Player.Position);
                        }
                    }
                }

                if (target.IsBoss && target.Distance < 25 && (Skills.Wizard.ArchonTeleport.TimeSinceUse > 2000 || Skills.Wizard.Teleport.TimeSinceUse > 2000))
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 40, 50, CurrentTarget.Position);

                if (position != Vector3.Zero && !TrinityGrid.Instance.IsIntersectedByFlags(ZetaDia.Me.Position, position, AvoidanceFlags.Combat, AvoidanceFlags.CriticalAvoidance))
                        return Teleport(position);
                }

            if (!IsArchonActive)
            {
                if (Player.CurrentHealthPct <= 0.7f && Skills.Wizard.Archon.CanCast())
                {
                    Core.Logger.Log(LogCategory.Spells, $"Emergency Archon, LOW ON HP {Player.CurrentHealthPct}%!");
                    return Archon();
                }
                if (Legendary.ConventionOfElements.IsEquipped)
                {
                    if (Skills.Wizard.Archon.CanCast())
                        {
                            if (Core.Buffs.ConventionElement == Element.Fire && TalRashaStacks >= 3)
                            {
                                Core.Logger.Log(LogCategory.Spells, $"Archon conditions met (TalRasha {TalRashaStacks} stacks && We are on {Core.Buffs.ConventionElement}!)");
                                return Archon();
                            }

                            if (Core.Buffs.ConventionElement == Element.Fire && TalRashaStacks >= 2)
                            {
                                Core.Logger.Log(LogCategory.Spells, $"Archon conditions met (TalRasha {TalRashaStacks} stacks && We are on {Core.Buffs.ConventionElement}!)");
                                return Archon();
                            }
                    } 
                }
                if (!Legendary.ConventionOfElements.IsEquipped)
                {
                    if (Skills.Wizard.Archon.CanCast() && TalRashaStacks >= 2)
                    {
                        Core.Logger.Log(LogCategory.Spells, $"Archon conditions met (TalRasha {TalRashaStacks} stacks!)");
                        return Archon();
                    }
                }
            }

            if (IsArchonActive)
            {
                target = null;
                var targetElite = TargetUtil.BestEliteInRange(75);
                var targett = TargetUtil.BestRangedAoeUnit(30, 75, Settings.ClusterSize, true, true);
                var targettt = TargetUtil.GetBestClusterUnit(30, 75, true, true, false, true) ?? CurrentTarget;
                if (targetElite != null)
                {
                    Core.Logger.Log(LogCategory.Targetting, $"Ladies first!");
                    return ArchonDisintegrationWave(targetElite);
                }
                else if (targett != null)
                {
                    Core.Logger.Log(LogCategory.Targetting, $"Good hunts are second!");
                    return ArchonDisintegrationWave(targett);
                }
                else
                {
                    Core.Logger.Log(LogCategory.Targetting, $"Archon Disintegration: Pew Pew Pew!");
                    return ArchonDisintegrationWave(targettt);
                }
            }

            var lastBlackHolePosition = Vector3.Zero;
                power = null;
                if (Skills.Wizard.BlackHole.CanCast() && Core.Player.PrimaryResource > 18)
                {
                    if (Skills.Wizard.BlackHole.CurrentRune == Runes.Wizard.Spellsteal || Skills.Wizard.BlackHole.CurrentRune == Runes.Wizard.AbsoluteZero)
                    {
                        if (TargetUtil.NumMobsInRangeOfPosition(target.Position, 15) > Skills.Wizard.BlackHole.BuffStacks)
                        {
                            lastBlackHolePosition = target.Position;
                            {
                                Core.Logger.Log(LogCategory.Targetting, $"Placing BlackHole: Good place for stacks!");
                                return BlackHole(target);
                            }
                        }
                    }

                    if (target.Position.Distance2D(lastBlackHolePosition) > 10)
                    {
                        lastBlackHolePosition = target.Position;
                        {
                            Core.Logger.Log(LogCategory.Targetting, $"Placing BlackHole: The bomb has been planted!");
                            return BlackHole(target);
                        }
                    }

                    if (Skills.Wizard.BlackHole.TimeSinceUse > 2000)
                    {
                        lastBlackHolePosition = target.Position;
                        {
                            Core.Logger.Log(LogCategory.Targetting, $"Placing BlackHole: Well placed.");
                            return BlackHole(target);
                        }
                    }
                }

                if (TrySecondaryPower(out power))
                {
                    Core.Logger.Log(LogCategory.Targetting, $"OffensivePower SecondaryPower");
                    return power;
                }

                if (TryPrimaryPower(out power))
                {
                    Core.Logger.Log(LogCategory.Targetting, $"OffensivePower PrimaryPower");
                    return power;
                }
            return Walk(CurrentTarget.Position);
        }
        #endregion

        #region Some more settings

        #endregion

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardManaldCoESettings Settings { get; } = new WizardManaldCoESettings();

        public sealed class WizardManaldCoESettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(8)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(0.7f)]
            public float EmergencyHealthPct
            {
                get => _emergencyHealthPct;
                set => SetField(ref _emergencyHealthPct, value);
            }

            public SkillSettings Teleport
            {
                get => _teleport;
                set => SetField(ref _teleport, value);
            }

            #region Skill Defaults

            private static readonly SkillSettings TeleportDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 200,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Teleport = TeleportDefaults.Clone();
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion

    }
}

