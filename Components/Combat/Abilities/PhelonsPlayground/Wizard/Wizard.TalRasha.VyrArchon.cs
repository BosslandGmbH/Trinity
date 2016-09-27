//using Trinity.Reference;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Wizard
//{
//    internal partial class Wizard
//    {
//        partial class TalRasha
//        {
//            public class VyrArchon
//            {
//                public static TrinityPower PowerSelector()
//                {
//                    //if (Player.IsIncapacitated) return null;

//                    if (Archon.ShouldArchon())
//                        return Archon.CastArchon;

//                    if (ShouldFrostNova)
//                        return CastFrostNova;

//                    if (ShouldExplosiveBlast)
//                        return CastExplosiveBlast;

//                    if (ShouldBlackHole)
//                        return CastBlackHole;

//                    var power = Archon.PowerSelector();
//                    if (power != null)
//                        return power;

//                    if (ShouldArcaneTorrent)
//                        return CastArcaneTorrent;

//                    return null;
//                }

//                private static bool ShouldArcaneTorrent
//                {
//                    get
//                    {
//                        return CanCast(SNOPower.Wizard_ArcaneTorrent) && TargetUtil.BestAoeUnit(45, true) != null;
//                    }
//                }

//                private static TrinityPower CastExplosiveBlast
//                {
//                    get { return new TrinityPower(SNOPower.Wizard_ExplosiveBlast); }
//                }

//                private static bool ShouldExplosiveBlast
//                    =>
//                        Skills.Wizard.ExplosiveBlast.CanCast() && TargetUtil.AnyMobsInRange(12f, false) &&
//                        (Skills.Wizard.Archon.CanCast() || TimeSincePowerUse(SNOPower.Wizard_ExplosiveBlast) > 4000 ||
//                         Player.PrimaryResourcePct > 0.90);

//                private static TrinityPower CastArcaneTorrent
//                {
//                    get
//                    {
//                        return new TrinityPower(SNOPower.Wizard_ArcaneTorrent, 55f,
//                            TargetUtil.BestAoeUnit(45, true).Position);
//                    }
//                }

//                private static bool ShouldBlackHole
//                {
//                    get
//                    {
//                        return Skills.Wizard.BlackHole.CanCast() && TargetUtil.BestAoeUnit(45, true) != null &&
//                               (Skills.Wizard.Archon.CanCast() ||
//                                TimeSincePowerUse(Skills.Wizard.BlackHole.SNOPower) > 15000 && TalRashasCount < 4 ||
//                                Player.PrimaryResourcePct > 0.90);
//                    }
//                }

//                private static TrinityPower CastBlackHole
//                {
//                    get
//                    {
//                        return new TrinityPower(Skills.Wizard.BlackHole.SNOPower, 55f,
//                            TargetUtil.BestAoeUnit(45, true).Position);
//                    }
//                }

//                private static bool ShouldFrostNova
//                {
//                    get
//                    {
//                        return Skills.Wizard.FrostNova.CanCast() && TargetUtil.AnyMobsInRange(12f, false) &&
//                               (Skills.Wizard.Archon.CanCast() ||
//                                TimeSincePowerUse(Skills.Wizard.FrostNova.SNOPower) > 8000);
//                    }
//                }

//                private static TrinityPower CastFrostNova
//                {
//                    get { return new TrinityPower(Skills.Wizard.FrostNova.SNOPower); }
//                }

//                #region Archon Skills

//                #endregion
//            }
//        }
//    }
//}