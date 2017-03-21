using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;

namespace Trinity.Framework.Reference
{
    public class Sets : FieldCollection<Sets, Set>
    {

        public static Set ThousandStorms = new Set
        {
            Name = "Raiment of a Thousand Storms",
            Items = new List<Item>
            {
                Legendary.MaskOfTheSearingSky,
                Legendary.ScalesOfTheDancingSerpent,
                Legendary.MantleOfTheUpsidedownSinners,
                Legendary.HeartOfTheCrashingWave,
                Legendary.FistsOfThunder,
                Legendary.EightdemonBoots
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set NorvaldsFavor = new Set
        {
            Name = "Norvald's Favor",
            Items = new List<Item>
            {
                Legendary.ShieldOfTheSteed,
                Legendary.FlailOfTheCharge
            },
            FirstBonusItemCount = 2,
        };

        public static Set ThornsoftheInvoker = new Set
        {
            Name = "Thorns of the Invoker",
            Items = new List<Item>
            {
                Legendary.BurdenOfTheInvoker,
                Legendary.CrownOfTheInvoker,
                Legendary.PrideOfTheInvoker,
                Legendary.RenewalOfTheInvoker,
                Legendary.ShacklesOfTheInvoker,
                Legendary.ZealOfTheInvoker,
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set Innas = new Set
        {
            Name = "Inna's Mantra",
            Items = new List<Item>
            {
                Legendary.InnasVastExpanse,
                Legendary.InnasTemperance,
                Legendary.InnasReach,
                Legendary.InnasRadiance,
                Legendary.InnasFavor,
                Legendary.InnasHold,
                Legendary.InnasSandals
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set RaimentOfTheJadeHarvester = new Set
        {
            Name = "Raiment of the Jade Harvester",
            Items = new List<Item>
            {
                Legendary.JadeHarvestersCourage,
                Legendary.JadeHarvestersWisdom,
                Legendary.JadeHarvestersJoy,
                Legendary.JadeHarvestersMercy,
                Legendary.JadeHarvestersPeace,
                Legendary.JadeHarvestersSwiftness
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set VyrsAmazingArcana = new Set
        {
            Name = "Vyr's Amazing Arcana",
            Items = new List<Item>
            {
                Legendary.VyrsAstonishingAura,
                Legendary.VyrsFantasticFinery,
                Legendary.VyrsGraspingGauntlets,
                Legendary.VyrsSwaggeringStance,
                Legendary.VyrsProudPauldrons,
                Legendary.VyrsSightlessSkull,
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set TheShadowsMantle = new Set
        {
            Name = "The Shadow's Mantle",
            Items = new List<Item>
            {
                Legendary.TheShadowsBane,
                Legendary.TheShadowsCoil,
                Legendary.TheShadowsHeels,
                Legendary.TheShadowsGrasp,
                Legendary.TheShadowsMask,
                Legendary.TheShadowsBurden,
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set MightOfTheEarth = new Set
        {
            Name = "Might of the Earth",
            Items = new List<Item>
            {
                Legendary.EyesOfTheEarth,
                Legendary.PullOfTheEarth,
                Legendary.SpiresOfTheEarth,
                Legendary.WeightOfTheEarth,
                Legendary.SpiritOfTheEarth,
                Legendary.FoundationOfTheEarth
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        //public static Set AshearasVestments = new Set
        //{
        //    Name = "Asheara's Vestments",
        //    Items = new List<Item>
        //    {
        //        Legendary.AshearasFinders,
        //        Legendary.AshearasPace,
        //        Legendary.AshearasWard,
        //        Legendary.AshearasCustodian
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3,
        //    ThirdBonusItemCount = 4
        //};

        //public static Set CainsDestiny = new Set
        //{
        //    Name = "Cain's Destiny",
        //    Items = new List<Item>
        //    {
        //        Legendary.CainsTravelers,
        //        Legendary.CainsHabit,               
        //        Legendary.CainsInsight,
        //        Legendary.CainsScrivener
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3
        //};

        //public static Set AughildsAuthority = new Set
        //{
        //    Name = "Aughild's Authority",
        //    Items = new List<Item>
        //    {
        //        Legendary.AughildsPower,
        //        Legendary.AughildsRule,
        //        Legendary.AughildsSearch,
        //        Legendary.AughildsSpike
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3
        //};

        //public static Set SagesJourney = new Set
        //{
        //    Name = "Sage's Journey",
        //    Items = new List<Item>
        //    {
        //        Legendary.SagesApogee,
        //        Legendary.SagesPassage,
        //        Legendary.SagesPurchase
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3
        //};

        public static Set ZunimassasHaunt = new Set
        {
            Name = "Zunimassa's Haunt",
            Items = new List<Item>
            {
                Legendary.ZunimassasMarrow,
                Legendary.ZunimassasPox,
                Legendary.ZunimassasStringOfSkulls,
                Legendary.ZunimassasTrail,
                Legendary.ZunimassasVision,
                Legendary.ZunimassasCloth,
                Legendary.ZunimassasFingerWraps
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set DelseresMagnumOpus = new Set
        {
            Name = "Delsere's Magnum Opus",
            Items = new List<Item>
            {
                Legendary.DashingPauldronsOfDespair,
                Legendary.HarnessOfTruth,
                Legendary.FierceGauntlets,
                Legendary.LegGuardsOfMystery,
                Legendary.ShroudedMask,
                Legendary.StridersOfDestiny
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set NatalyasVengeance = new Set
        {
            Name = "Natalya's Vengeance",
            Items = new List<Item>
            {
                Legendary.NatalyasBloodyFootprints,
                Legendary.NatalyasEmbrace,
                Legendary.NatalyasSight,
                Legendary.NatalyasReflection,   
                Legendary.NatalyasSlayer,
                Legendary.NatalyasLeggings,
                Legendary.NatalyasTouch
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set UnhallowedEssence = new Set
        {
            Name = "Unhallowed Essence",
            Items = new List<Item>
            {
                Legendary.UnsanctifiedShoulders,
                Legendary.CageOfTheHellborn,
                Legendary.FiendishGrips,
                Legendary.UnholyPlates,   
                Legendary.AccursedVisage,
                Legendary.HellWalkers
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set BastionsOfWill = new Set
        {
            Name = "Bastions of Will",
            Items = new List<Item>
            {
                Legendary.Focus,  
                Legendary.Restraint
            },
            FirstBonusItemCount = 2
        };

        public static Set LegacyOfNightmares = new Set
        {
            Name = "Legacy of Nightmares",
            Items = new List<Item>
            {
                Legendary.LitanyOfTheUndaunted,  
                Legendary.TheWailingHost
            },
            FirstBonusItemCount = 2
        };

        public static Set ChantodosResolve = new Set
        {
            Name = "Chantodo's Resolve",
            Items = new List<Item>
            {
                Legendary.ChantodosWill,
                Legendary.ChantodosForce
            },
            FirstBonusItemCount = 2
        };

        public static Set BulKathossOath = new Set
        {
            Name = "Bul-Kathos's Oath",
            Items = new List<Item>
            {
                Legendary.BulkathossWarriorBlood,
                Legendary.BulkathossSolemnVow
            },
            FirstBonusItemCount = 2
        };

        public static Set ManajumasWay = new Set
        {
            Name = "Manajuma's Way",
            Items = new List<Item>
            {
                Legendary.ManajumasCarvingKnife,
                Legendary.ManajumasGoryFetch
            },
            FirstBonusItemCount = 2
        };

        public static Set IstvansPairedBlades = new Set
        {
            Name = "Istvan's Paired Blades",
            Items = new List<Item>
            {
                Legendary.TheSlanderer,
                Legendary.LittleRogue
            },
            FirstBonusItemCount = 2
        };

        public static Set DanettasHatred = new Set
        {
            Name = "Danetta's Hatred",
            Items = new List<Item>
            {
                Legendary.DanettasSpite,
                Legendary.DanettasRevenge
            },
            FirstBonusItemCount = 2
        };

        //public static Set GuardiansJeopardy = new Set
        //{
        //    Name = "Guardian's Jeopardy",
        //    Items = new List<Item>
        //    {           
        //        Legendary.GuardiansCase,
        //        Legendary.GuardiansAversion,
        //        Legendary.GuardiansGaze
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3
        //};

        //public static Set CaptainCrimsonTrimmings = new Set
        //{
        //    Name = "Captain Crimson Trimmings",
        //    Items = new List<Item>
        //    {
        //        Legendary.CaptainCrimsonsSilkGirdle,
        //        Legendary.CaptainCrimsonsThrust,
        //        Legendary.CaptainCrimsonsWaders
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3
        //};

        //public static Set DemonsHide = new Set
        //{
        //    Name = "Demon's Hide",
        //    Items = new List<Item>
        //    {
        //        Legendary.DemonsAileron,
        //        Legendary.DemonsMarrow,
        //        Legendary.DemonsPlate,
        //        Legendary.DemonsAnimus,
        //        Legendary.DemonsRestraint
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3,
        //    ThirdBonusItemCount = 4
        //};

        public static Set TheLegacyOfRaekor = new Set
        {
            Name = "The Legacy of Raekor",
            Items = new List<Item>
            {
                Legendary.RaekorsBurden,
                Legendary.RaekorsHeart,
                Legendary.RaekorsWraps,
                Legendary.RaekorsBreeches,  
                Legendary.RaekorsStriders,
                Legendary.RaekorsWill
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 5
        };

        public static Set EmbodimentOfTheMarauder = new Set
        {
            Name = "Embodiment of the Marauder",
            Items = new List<Item>
            {
                Legendary.MaraudersCarapace,
                Legendary.MaraudersTreads,
                Legendary.MaraudersVisage,        
                Legendary.MaraudersEncasement,
                Legendary.MaraudersGloves,
                Legendary.MaraudersSpines
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set HelltoothHarness = new Set
        {
            Name = "Helltooth Harness",
            Items = new List<Item>
            {
                Legendary.HelltoothMantle,
                Legendary.HelltoothTunic, 
                Legendary.HelltoothGauntlets,
                Legendary.HelltoothLegGuards,
                Legendary.HelltoothGreaves,
                Legendary.HelltoothMask
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set FirebirdsFinery = new Set
        {
            Name = "Firebird's Finery",
            Items = new List<Item>
            {
                Legendary.FirebirdsBreast,
                Legendary.FirebirdsDown,
                Legendary.FirebirdsEye,
                Legendary.FirebirdsPinions,
                Legendary.FirebirdsPlume,
                Legendary.FirebirdsTalons,
                Legendary.FirebirdsTarsi
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set ArmorOfAkkhan = new Set
        {
            Name = "Armor of Akkhan",
            Items = new List<Item>
            {
                Legendary.BreastplateOfAkkhan,  
                Legendary.CuissesOfAkkhan,
                Legendary.GauntletsOfAkkhan,
                Legendary.HelmOfAkkhan,
                Legendary.PauldronsOfAkkhan,
                Legendary.SabatonsOfAkkhan,
                Legendary.TalismanOfAkkhan
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set BlackthornesBattlegear = new Set
        {
            Name = "Blackthorne's Battlegear",
            Items = new List<Item>
            {
                Legendary.BlackthornesDuncraigCross,
                Legendary.BlackthornesJoustingMail,
                Legendary.BlackthornesNotchedBelt,
                Legendary.BlackthornesSpurs,
                Legendary.BlackthornesSurcoat
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 3,
            ThirdBonusItemCount = 4
        };

        public static Set TalRashasElements = new Set
        {
            Name = "Tal Rasha's Elements",
            Items = new List<Item>
            {
                Legendary.TalRashasAllegiance,
                Legendary.TalRashasGrasp,
                Legendary.TalRashasStride,
                Legendary.TalRashasBrace,
                Legendary.TalRashasGuiseOfWisdom,
                Legendary.TalRashasRelentlessPursuit,
                Legendary.TalRashasUnwaveringGlare
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set EndlessWalk = new Set
        {
            Name = "Endless Walk",
            Items = new List<Item>
            {
                Legendary.TheTravelersPledge,
                Legendary.TheCompassRose
            },
            FirstBonusItemCount = 2,
        };

        public static Set RolandsLegacy = new Set
        {
            Name = "Roland's Legacy",
            Items = new List<Item>
            {
                Legendary.RolandsBearing,
                Legendary.RolandsDetermination,
                Legendary.RolandsGrasp,
                Legendary.RolandsMantle,
                Legendary.RolandsStride,
                Legendary.RolandsVisage
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set ImmortalKingsCall = new Set
        {
            Name = "Immortal King's Call",
            Items = new List<Item>
            {
                Legendary.ImmortalKingsBoulderBreaker,
                Legendary.ImmortalKingsEternalReign,
                Legendary.ImmortalKingsIrons,
                Legendary.ImmortalKingsStride,
                Legendary.ImmortalKingsStature,
                Legendary.ImmortalKingsTribalBinding,
                Legendary.ImmortalKingsTriumph,
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set WrathOfTheWastes = new Set
        {
            Name = "Wrath of the Wastes",
            Items = new List<Item>
            {
                Legendary.CuirassOfTheWastes,
                Legendary.GauntletOfTheWastes,
                Legendary.HelmOfTheWastes,
                Legendary.PauldronsOfTheWastes,
                Legendary.SabatonOfTheWastes,
                Legendary.TassetOfTheWastes,
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set MonkeyKingsGarb = new Set
        {
            Name = "Monkey King's Garb",
            Items = new List<Item>
            {
                Legendary.SunwukosBalance,
                Legendary.SunwukosCrown,
                Legendary.SunwukosPaws,
                Legendary.SunwukosShines,
                Legendary.SunwukosLeggings,
                Legendary.SunwukosSoul
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set ShenlongsSpirit = new Set
        {
            Name = "Shenlong's Spirit",
            Items = new List<Item>
            {
                Legendary.ShenlongsFistOfLegend,  
                Legendary.ShenlongsRelentlessAssault
            },
            FirstBonusItemCount = 2
        };

        //public static Set HallowedProtectors = new Set
        //{
        //    Name = "Hallowed Protectors",
        //    Items = new List<Item>
        //    {
        //        Legendary.HallowedBreach,
        //        Legendary.HallowedNemesis,
        //        Legendary.HallowedSufferance,
        //        Legendary.HallowedCondemnation,
        //        Legendary.HallowedBaton,
        //        Legendary.HallowedHold,
        //        Legendary.HallowedBarricade
        //    },
        //    FirstBonusItemCount = 2
        //};

        public static Set UlianasStratagem = new Set
        {
            Name = "Uliana's Stratagem",
            Items = new List<Item>
            {
                Legendary.UlianasBurden,
                Legendary.UlianasDestiny,
                Legendary.UlianasFury,
                Legendary.UlianasHeart,
                Legendary.UlianasSpirit,
                Legendary.UlianasStrength, 
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set SpiritOfArachnyr = new Set
        {
            Name = "Spirit of Arachnyr",
            Items = new List<Item>
            {
                Legendary.ArachyrsCarapace,
                Legendary.ArachyrsClaws,
                Legendary.ArachyrsLegs,
                Legendary.ArachyrsMantle,
                Legendary.ArachyrsStride,
                Legendary.ArachyrsVisage, 
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        public static Set SeekerOfTheLight = new Set
        {
            Name = "Seeker of the Light",
            Items = new List<Item>
            {
                Legendary.CrownOfTheLight,
                Legendary.FoundationOfTheLight,
                Legendary.HeartOfTheLight,
                Legendary.MountainOfTheLight,
                Legendary.TowersOfTheLight,
                Legendary.WillOfTheLight, 
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6
        };

        //public static Set BornsCommand = new Set
        //{
        //    Name = "Born's Command",
        //    Items = new List<Item>
        //    {
        //        Legendary.BornsFrozenSoul,
        //        Legendary.BornsFuriousWrath,
        //        Legendary.BornsPrivilege 
        //    },
        //    FirstBonusItemCount = 2,
        //    SecondBonusItemCount = 3
        //};

        /// <summary>
        /// Gets equipped sets
        /// </summary>
        public static List<Set> Equipped
        {
            get
            {
                return ToList().Where(s => s.IsEquipped).ToList();
            }
        }

        /// <summary>
        /// Gets equipped sets
        /// </summary>
        public static List<Set> FullyEquipped
        {
            get
            {
                return ToList().Where(s => s.IsFullyEquipped).ToList();
            }
        }

        /// <summary>
        /// All items that are part of a set
        /// </summary>        
        public static List<Item> SetItems
        {
            get
            {
                return _setItems ?? (_setItems = ToList().SelectMany(s => s.Items).ToList());
            }
        }
        private static List<Item> _setItems;

        /// <summary>
        /// All items that are part of a set, as ActorSnoId
        /// </summary>        
        public static HashSet<int> SetItemIds
        {
            get
            {
                return (_setItemIds = new HashSet<int>(SetItems.Select(i => i.Id)));
            }
        }
        private static HashSet<int> _setItemIds;


    }

}
