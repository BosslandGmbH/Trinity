using System;
using System.Linq;
using Trinity.Framework;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    internal partial class Wizard
    {
        partial class TalRasha
        {
            public class VyrArchon
            {
                public static TrinityPower PowerSelector()
                {
                    if (Player.IsIncapacitated) return null;

                    if (ShouldFrostNova)
                        return CastFrostNova;

                    if (ShouldBlackHole)
                        return CastBlackHole;

                    if (ShouldExplosiveBlast)
                        return CastExplosiveBlast;

                    if (ShouldArchon())
                        return CastArchon;

                    var power = Archon.PowerSelector();
                    if (power != null)
                        return power;

                    if (ShouldArcaneTorrent)
                        return CastArcaneTorrent;

                    return null;
                }

                private static bool ShouldArcaneTorrent
                {
                    get
                    {
                        return CanCast(SNOPower.Wizard_ArcaneTorrent) && PhelonTargeting.BestAoeUnit(45, true) != null;
                    }
                }

                private static TrinityPower CastExplosiveBlast
                {
                    get { return new TrinityPower(SNOPower.Wizard_ExplosiveBlast); }
                }

                private static bool ShouldExplosiveBlast
                    =>
                        Skills.Wizard.ExplosiveBlast.CanCast() && TargetUtil.AnyMobsInRange(12f, false) &&
                        (Skills.Wizard.Archon.CanCast() || TimeSincePowerUse(SNOPower.Wizard_ExplosiveBlast) > 4000);

                private static TrinityPower CastArcaneTorrent
                {
                    get
                    {
                        return new TrinityPower(SNOPower.Wizard_ArcaneTorrent, 55f,
                            PhelonTargeting.BestAoeUnit(45, true).Position);
                    }
                }

                private static bool ShouldBlackHole
                {
                    get
                    {
                        return Skills.Wizard.BlackHole.CanCast() && PhelonTargeting.BestAoeUnit(45, true) != null && 
                            Skills.Wizard.Archon.CanCast();
                    }
                }

                private static TrinityPower CastBlackHole
                {
                    get
                    {
                        return new TrinityPower(Skills.Wizard.BlackHole.SNOPower, 55f,
                            PhelonTargeting.BestAoeUnit(45, true).Position);
                    }
                }

                private static bool ShouldFrostNova
                {
                    get
                    {
                        return Skills.Wizard.FrostNova.CanCast() && TargetUtil.AnyMobsInRange(12f, false) &&
                               (Skills.Wizard.Archon.CanCast() || TimeSincePowerUse(Skills.Wizard.FrostNova.SNOPower) > 8000);
                    }
                }

                private static TrinityPower CastFrostNova
                {
                    get { return new TrinityPower(Skills.Wizard.FrostNova.SNOPower); }
                }

                #region Archon Skills

                public static bool ShouldArchon()
                {
                    if (!Skills.Wizard.Archon.CanCast() || Skills.Wizard.ArchonBlast.CanCast())
                        return false;

                    if (Player.PrimaryResourcePct < 0.20 ||
                        CacheData.Buffs.GetBuffTimeRemainingMilliseconds(SNOPower.Wizard_Archon) < 1500)
                        return true;

                    if (Sets.ChantodosResolve.IsFullyEquipped)
                    {
                        return GetHasBuff(SNOPower.P3_ItemPassive_Unique_Ring_021) &&
                               GetBuffStacks(SNOPower.P3_ItemPassive_Unique_Ring_021) > 19;
                    }

                    return true;
                }

                public static TrinityPower CastArchon
                {
                    get
                    {
                        Archon.NeedSlowTime = true;
                        return new TrinityPower(Skills.Wizard.Archon.SNOPower);
                    }
                }

                #endregion

            }
        }
    }
}