using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Helpers;
using Trinity.Reference;
using Trinity.Objects;

namespace Trinity.Settings.Loot
{
    public static class PresetLists
    {
        private static HashSet<int> _patch24NewOnlyItems;
        public static HashSet<int> Patch24NewOnlyItems
        {
            get { return _patch24NewOnlyItems ?? (_patch24NewOnlyItems = new HashSet<int>(_patch24ItemsNewOnlySource.DistinctBy(i => i.Id).Select(i => i.Id))); }
        }

        private static List<Item> _patch24ItemsNewOnlySource = new List<Item>()
        {
            // Barb NEW/CHANGED
            Legendary.Standoff,
            Legendary.TheThreeHundredthSpear,
            Legendary.BladeOfTheWarlord,
            Legendary.BandOfMight,
            Legendary.SkularsSalvation,
            Legendary.Oathkeeper,
            Legendary.BladeOfTheTribes,

            // Crusader NEW/CHANGED
            Legendary.HammerJammers,
            Legendary.ShieldOfFury,
            Legendary.ShieldOfTheSteed,
            Legendary.WarhelmOfKassar,
            Legendary.AkkhansManacles,
            Legendary.BracerOfFury,
            Legendary.AkkhansLeniency,
            Legendary.AkkhansAddendum,

            //Demonhunter NEW/CHANGED
            Legendary.Manticore,
            Legendary.Dawn,
            Legendary.SinSeekers,
            Legendary.ElusiveRing,
            Legendary.KarleisPoint,
            Legendary.LordGreenstonesFan,
            Legendary.SwordOfIllWill,
            Legendary.VisageOfGiyua,
            Legendary.ChainOfShadows,
            Legendary.ZoeysSecret,
            Legendary.LiannasWings,
            Legendary.FortressBallista,

            // Monk NEW/CHANGED
            Legendary.Balance,
            Legendary.RiveraDancers,
            Legendary.LefebvresSoliloquy,
            Legendary.KyoshirosSoul,
            Legendary.KyoshirosBlade,
            Legendary.BindingsOfTheLesserGods,
            Legendary.PintosPride,
            Legendary.CesarsMemento,
            
            //WD NEW/CHANGED
            Legendary.VoosJuicer,
            Legendary.LastBreath,
            Legendary.VileHive,
            Legendary.StaffOfChiroptera,
            Legendary.ThingOfTheDeep,
            Legendary.RingOfEmptiness,
            Legendary.MordullusPromise,
            Legendary.LakumbasOrnament,
            Legendary.WilkensReach,
                        
            // Wizard NEW/CHANGED
            Legendary.TheTwistedSword,
            Legendary.Deathwish,
            Legendary.TheShameOfDelsere,
            Legendary.HergbrashsBinding,
            Legendary.UnstableScepter,
            Legendary.PrimordialSoul,
            Legendary.EtchedSigil,
            Legendary.AshnagarrsBloodBracer,

            //Neutral NEW/CHANGED
            Legendary.StringOfEars,
            Legendary.AquilaCuirass,
            Legendary.HeartOfIron,
            Legendary.JusticeLantern,
            Legendary.MantleOfChanneling,
            Legendary.VambracesOfSescheron,
            Legendary.TheWailingHost,
            Legendary.LitanyOfTheUndaunted,             
        };

        private static HashSet<int> _patch24items;
        public static HashSet<int> Patch24Items
        {
            get { return _patch24items ?? (_patch24items = new HashSet<int>(_patch24ItemsSource.DistinctBy(i => i.Id).Select(i => i.Id))); }
        }

        private static List<Item> _patch24ItemsSource = new List<Item>()
        {
            // Barb NEW/CHANGED
            Legendary.Standoff,
            Legendary.TheThreeHundredthSpear,
            Legendary.BladeOfTheWarlord,
            Legendary.BandOfMight,
            Legendary.SkularsSalvation,
            Legendary.Oathkeeper,
            Legendary.BladeOfTheTribes,

            // Raekors Immortal Charge Spear
            Legendary.RaekorsBurden,
            Legendary.RaekorsWraps,
            Legendary.RaekorsBreeches,
            Legendary.RaekorsHeart,
            Legendary.RaekorsStriders,
            Legendary.RaekorsWill,            
            Legendary.ImmortalKingsEternalReign,
            Legendary.ImmortalKingsBoulderBreaker,
            Legendary.ImmortalKingsIrons,
            Legendary.ImmortalKingsStature,
            Legendary.ImmortalKingsStride,
            Legendary.ImmortalKingsTribalBinding,
            Legendary.ImmortalKingsTriumph,
            Legendary.TheThreeHundredthSpear,

            // Earth Set
            Legendary.LutSocks,
            Legendary.EyesOfTheEarth,
            Legendary.PullOfTheEarth,
            Legendary.SpiresOfTheEarth,
            Legendary.WeightOfTheEarth,
            Legendary.SpiritOfTheEarth,
            Legendary.FoundationOfTheEarth,
            Legendary.DreadIron,
            Legendary.TheEssOfJohan,
            Legendary.StringOfEars,


            // Crusader NEW/CHANGED
            Legendary.HammerJammers,
            Legendary.ShieldOfFury,
            Legendary.ShieldOfTheSteed,
            Legendary.WarhelmOfKassar,
            Legendary.AkkhansManacles,
            Legendary.BracerOfFury,
            Legendary.AkkhansLeniency,
            Legendary.AkkhansAddendum,

            // Invoker/Thorns
            Legendary.PigSticker,
            Legendary.ZealOfTheInvoker,
            Legendary.RenewalOfTheInvoker,
            Legendary.VotoyiasSpiker,
            Legendary.BeltOfTheTrove,
            Legendary.JusticeLantern,
            Legendary.PrideOfTheInvoker,
            Legendary.ShacklesOfTheInvoker,
            Legendary.HeartOfIron,
            Legendary.CrownOfTheInvoker,
            Legendary.BurdenOfTheInvoker,
            Legendary.AkaratsAwakening,
            Legendary.BloodBrother,
            Legendary.Hack,

            //Demonhunter NEW/CHANGED
            Legendary.Manticore,
            Legendary.Dawn,
            Legendary.SinSeekers,
            Legendary.ElusiveRing,
            Legendary.KarleisPoint,
            Legendary.LordGreenstonesFan,
            Legendary.SwordOfIllWill,
            Legendary.VisageOfGiyua,
            Legendary.ChainOfShadows,
            Legendary.ZoeysSecret,
            Legendary.LiannasWings,
            Legendary.FortressBallista,

            // DH Strafe/Fan/Dagger
            Legendary.Dawn,
            Legendary.StArchewsGage,
            Legendary.WrapsOfClarity,
            Legendary.FireWalkers,
            Legendary.HexingPantsOfMrYan,
            Legendary.PridesFall,
            Legendary.ThundergodsVigor,    
            Legendary.Calamity,
            Legendary.BeckonSail,
            Legendary.IceClimbers,      

            // UE Multishot
            Legendary.UnsanctifiedShoulders,
            Legendary.FiendishGrips,
            Legendary.YangsRecurve,
            Legendary.HellWalkers,
            Legendary.DeadMansLegacy,
            Legendary.UnholyPlates,
            Legendary.AccursedVisage,
            Legendary.CageOfTheHellborn,

            // DH Shadows
            Legendary.TheShadowsBane,
            Legendary.TheShadowsBurden,
            Legendary.TheShadowsCoil,
            Legendary.TheShadowsGrasp,
            Legendary.TheShadowsHeels,
            Legendary.TheShadowsMask,
            Legendary.HolyPointShot,
            Legendary.VisageOfGunes,

            // DH Marauders
            Legendary.MaraudersCarapace,
            Legendary.MaraudersEncasement,
            Legendary.MaraudersGloves,
            Legendary.MaraudersSpines,
            Legendary.MaraudersTreads,
            Legendary.MaraudersVisage,
            Legendary.Manticore,
            Legendary.BombardiersRucksack,
            Legendary.SpinesOfSeethingHatred,

            // Monk NEW/CHANGED
            Legendary.Balance,
            Legendary.RiveraDancers,
            Legendary.LefebvresSoliloquy,
            Legendary.KyoshirosSoul,
            Legendary.KyoshirosBlade,
            Legendary.BindingsOfTheLesserGods,
            Legendary.PintosPride,
            Legendary.CesarsMemento,

            // Monk Innas/Shenlong
            Legendary.FlyingDragon,
            Legendary.ShenlongsFistOfLegend,
            Legendary.ShenlongsRelentlessAssault,
            Legendary.InnasFavor,
            Legendary.InnasHold,
            Legendary.InnasSandals,
            Legendary.InnasTemperance,
            Legendary.InnasVastExpanse,
            Legendary.MaskOfTheSearingSky,
            Legendary.MantleOfTheUpsidedownSinners,
            Legendary.InnasRadiance,
            Legendary.FistsOfThunder,
            Legendary.SpiritGuards,
            Legendary.TheCrudestBoots,
            Legendary.EightdemonBoots,

            // Monk Istvans/Cyclone
            Legendary.TheSlanderer,
            Legendary.LittleRogue,
            
            //WD NEW/CHANGED
            Legendary.VoosJuicer,
            Legendary.LastBreath,
            Legendary.VileHive,
            Legendary.StaffOfChiroptera,
            Legendary.ThingOfTheDeep,
            Legendary.RingOfEmptiness,
            Legendary.MordullusPromise,
            Legendary.LakumbasOrnament,
            Legendary.WilkensReach,

            // WD Jade Harvester Build
            Legendary.HenrisPerquisition,
            Legendary.JadeHarvestersCourage,
            Legendary.JadeHarvestersJoy,
            Legendary.JadeHarvestersMercy,
            Legendary.JadeHarvestersPeace,
            Legendary.JadeHarvestersSwiftness,
            Legendary.JadeHarvestersWisdom,
            Legendary.Quetzalcoatl,
            Legendary.SacredHarvester,
            Legendary.HauntingGirdle,
            Legendary.RechelsRingOfLarceny,
      
            // WD Pets Build
            Legendary.UhkapianSerpent,
            Legendary.IllusoryBoots,
            Legendary.BeltOfTranscendence,
            Legendary.TheTallMansFinger,
            Legendary.TaskerAndTheo,
            Legendary.StarmetalKukri,
            Legendary.MaskOfJeram,
            Legendary.CoilsOfTheFirstSpider,
            Legendary.TheDaggerOfDarts,
            Legendary.Carnevil,
            Legendary.TheShortMansFinger,
                        
            // Wizard NEW/CHANGED
            Legendary.TheTwistedSword,
            Legendary.Deathwish,
            Legendary.TheShameOfDelsere,
            Legendary.HergbrashsBinding,
            Legendary.UnstableScepter,
            Legendary.PrimordialSoul,
            Legendary.EtchedSigil,
            Legendary.AshnagarrsBloodBracer,

            // Wizard Firebirds
            Legendary.FirebirdsBreast,
            Legendary.FirebirdsDown,
            Legendary.FirebirdsEye,
            Legendary.FirebirdsPinions,
            Legendary.FirebirdsPlume,
            Legendary.FirebirdsTalons,
            Legendary.FirebirdsTarsi,

            // Wizard Delseres
            Legendary.DashingPauldronsOfDespair,
            Legendary.FierceGauntlets,
            Legendary.ShroudedMask,
            Legendary.HarnessOfTruth,
            Legendary.HaloOfArlyse,
            Legendary.LegGuardsOfMystery,
            Legendary.StridersOfDestiny,
            Legendary.Triumvirate,
            Legendary.RanslorsFolly,

            //Neutral NEW/CHANGED
            Legendary.StringOfEars,
            Legendary.AquilaCuirass,
            Legendary.HeartOfIron,
            Legendary.JusticeLantern,
            Legendary.MantleOfChanneling,
            Legendary.VambracesOfSescheron,    
            Legendary.TheWailingHost,
            Legendary.LitanyOfTheUndaunted,  
            
            // Other Useful
            Legendary.TheFurnace,
            Legendary.AncientParthanDefenders,
            Legendary.Focus,
            Legendary.Restraint,
            Legendary.Unity,
            Legendary.TheTravelersPledge,
            Legendary.TheCompassRose,
            Legendary.ConventionOfElements,
            Legendary.ObsidianRingOfTheZodiac,
            Legendary.LeoricsCrown,
            Legendary.OculusRing,
            Legendary.CountessJuliasCameo,
            Legendary.MarasKaleidoscope,
            Legendary.EyeOfEtlich,
            Legendary.TalismanOfAranoch,
            Legendary.TheStarOfAzkaranth,
            Legendary.XephirianAmulet,



        };
    }
}
