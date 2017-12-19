using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Game;

namespace Trinity.Framework.Reference
{
    public class Sets : FieldCollection<Sets, Set>
    {

        public static Set ThousandStorms = new Set
        {
            Name = "千风飓",
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
            Name = "诺瓦德的热忱",
            Items = new List<Item>
            {
                Legendary.ShieldOfTheSteed,
                Legendary.FlailOfTheCharge
            },
            FirstBonusItemCount = 2,
        };

        public static Set ThornsoftheInvoker = new Set
        {
            Name = "唤魔师的荆棘",
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
            Name = "尹娜的真言",
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
            Name = "玉魂师的战甲",
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
            Name = "维尔的神装",
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
            Name = "暗影装束",
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
            Name = "大地之力",
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
        //    Name = "艾席拉的制服",
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
        //    Name = "凯恩的天命",
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
        //    Name = "奥吉德的权利",
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
        //    Name = "贤者之难",
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
            Name = "祖尼玛萨之魂",
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
            Name = "德尔西尼的杰作",
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
            Name = "娜塔亚的复仇",
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
            Name = "邪秽之精",
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
            Name = "意志壁垒",
            Items = new List<Item>
            {
                Legendary.Focus,  
                Legendary.Restraint
            },
            FirstBonusItemCount = 2
        };

        public static Set LegacyOfNightmares = new Set
        {
            Name = "梦魇者的遗礼",
            Items = new List<Item>
            {
                Legendary.LitanyOfTheUndaunted,  
                Legendary.TheWailingHost
            },
            FirstBonusItemCount = 2
        };

        public static Set ChantodosResolve = new Set
        {
            Name = "迦陀朵的决心",
            Items = new List<Item>
            {
                Legendary.ChantodosWill,
                Legendary.ChantodosForce
            },
            FirstBonusItemCount = 2
        };

        public static Set BulKathossOath = new Set
        {
            Name = "布尔凯索之誓",
            Items = new List<Item>
            {
                Legendary.BulkathossWarriorBlood,
                Legendary.BulkathossSolemnVow
            },
            FirstBonusItemCount = 2
        };

        public static Set ManajumasWay = new Set
        {
            Name = "马纳祖玛之道",
            Items = new List<Item>
            {
                Legendary.ManajumasCarvingKnife,
                Legendary.ManajumasGoryFetch
            },
            FirstBonusItemCount = 2
        };

        public static Set IstvansPairedBlades = new Set
        {
            Name = "伊斯特凡的对剑",
            Items = new List<Item>
            {
                Legendary.TheSlanderer,
                Legendary.LittleRogue
            },
            FirstBonusItemCount = 2
        };

        public static Set DanettasHatred = new Set
        {
            Name = "达内塔之憎",
            Items = new List<Item>
            {
                Legendary.DanettasSpite,
                Legendary.DanettasRevenge
            },
            FirstBonusItemCount = 2
        };

        //public static Set GuardiansJeopardy = new Set
        //{
        //    Name = "守护者的险境",
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
        //    Name = "克里森船长的饰衣",
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
        //    Name = "恶魔之皮",
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
            Name = "蕾蔻的传世铠",
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
            Name = "掠夺者的化身",
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
            Name = "魔牙战装",
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
            Name = "不死鸟的华服",
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
            Name = "阿克汉的战甲",
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
            Name = "黑棘的战铠",
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
            Name = "塔·拉夏的法理",
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
            Name = "无尽之途",
            Items = new List<Item>
            {
                Legendary.TheTravelersPledge,
                Legendary.TheCompassRose
            },
            FirstBonusItemCount = 2,
        };

        public static Set RolandsLegacy = new Set
        {
            Name = "罗兰的传世甲",
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
            Name = "不朽之王的呼唤",
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
            Name = "废土之怒",
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
            Name = "猴王战甲",
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
            Name = "神龙之魂",
            Items = new List<Item>
            {
                Legendary.ShenlongsFistOfLegend,  
                Legendary.ShenlongsRelentlessAssault
            },
            FirstBonusItemCount = 2
        };

        //public static Set HallowedProtectors = new Set
        //{
        //    Name = "神圣护卫",
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
            Name = "乌莲娜的谋略",
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
            Name = "亚拉基尔的灵魂",
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
            Name = "圣光追寻者",
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

        public static Set TragoulsAvatar = new Set
        {
            Name = "塔格奥的化身",
            Items = new List<Item>
            {
                Legendary.TragoulsClaws,
                Legendary.TragoulsGuise,
                Legendary.TragoulsHeart,
                Legendary.TragoulsHide,
                Legendary.TragoulsScales,
                Legendary.TragoulsStalwartGreaves
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6,
            ClassRestriction = ActorClass.Necromancer,
        };

        public static Set BonesOfRathma = new Set
        {
            Name = "拉斯玛的骨甲",
            Items = new List<Item>
            {
                Legendary.RathmasMacabreVambraces,
                Legendary.RathmasOssifiedSabatons,
                Legendary.RathmasRibcagePlate,
                Legendary.RathmasSkeletalLegplates,
                Legendary.RathmasSkullHelm,
                Legendary.RathmasSpikes
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6,
            ClassRestriction = ActorClass.Necromancer,
        };

        public static Set PestilenceMastersShroud = new Set
        {
            Name = "死疫使者的裹布",
            Items = new List<Item>
            {
                Legendary.PestilenceBattleBoots,
                Legendary.PestilenceDefense,
                Legendary.PestilenceGloves,
                Legendary.PestilenceIncantations,
                Legendary.PestilenceMask,
                Legendary.PestilenceRobe,
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6,
            ClassRestriction = ActorClass.Necromancer,
        };

        public static Set GraceOfInarius = new Set
        {
            Name = "伊纳瑞斯的恩泽",
            Items = new List<Item>
            {
                Legendary.InariussConviction,
                Legendary.InariussMartyrdom,
                Legendary.InariussPerseverance,
                Legendary.InariussReticence,
                Legendary.InariussUnderstanding,
                Legendary.InariussWill,
            },
            FirstBonusItemCount = 2,
            SecondBonusItemCount = 4,
            ThirdBonusItemCount = 6,
            ClassRestriction = ActorClass.Necromancer,
        };

        public static Set JessethArms = new Set
        {
            Name = "杰西斯武装",
            Items = new List<Item>
            {
                Legendary.JessethSkullscythe,
                Legendary.JessethSkullshield,
            },
            FirstBonusItemCount = 2,
            ClassRestriction = ActorClass.Necromancer,
        };

        //public static Set BornsCommand = new Set
        //{
        //    Name = "博恩的号令",
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
