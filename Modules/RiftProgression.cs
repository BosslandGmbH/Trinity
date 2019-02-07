using System.Collections.Generic;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Modules
{
    public class RiftProgression : Module
    {
        protected override int UpdateIntervalMs => 500;

        protected override void OnPulse()
        {
            IsNephalemRift = ZetaDia.Storage.CurrentRiftType == RiftType.Nephalem;
            IsGreaterRift = ZetaDia.Storage.CurrentRiftType == RiftType.Greater;

            if (!IsNephalemRift && !IsNephalemRift)
                return;

            CurrentProgressionPct = ZetaDia.Globals.RiftProgressionPercent;
            IsInRift = !ZetaDia.Globals.IsLoadingWorld && ZetaDia.Storage.RiftStarted && GameData.RiftWorldIds.Contains(ZetaDia.Globals.WorldSnoId);
            IsGaurdianSpawned = IsInRift && ZetaDia.Storage.RiftGuardianSpawned;
            RiftComplete = ZetaDia.Storage.RiftCompleted;
        }

        public RiftQuest Quest => new RiftQuest();
        public float CurrentProgressionPct { get; private set; }
        public bool IsInRift { get; private set; }
        public bool IsGreaterRift { get; private set; }
        public bool IsNephalemRift { get; private set; }
        public bool IsGaurdianSpawned { get; private set; }
        public bool RiftComplete { get; private set; }

        public double GetRiftValue(TrinityActor actor)
        {
            var riftValue = 0d;
            if (IsInRift)
            {
                TryGetRiftValue(actor, out riftValue);
            }
            return riftValue;
        }

        public bool TryGetRiftValue(TrinityActor actor, out double riftValuePct)
        {
            riftValuePct = -1;
            if (actor.IsMinion)
            {
                return true;
            }
            if (actor.IsBoss && !actor.IsSummoned)
            {
                riftValuePct = 10d;
                return true;
            }
            if (actor.IsMinion)
            {
                riftValuePct = 0.25d;
                return true;
            }
            if (actor.IsElite)
            {
                riftValuePct = 1d;
                return true;
            }
            if (Values.ContainsKey(actor.ActorSnoId))
            {
                var baseValue = Values[actor.ActorSnoId];
                riftValuePct = actor.IsElite ? baseValue * 4 : baseValue;
                return true;
            }
            return false;
        }

        public static Dictionary<SNOActor, double> Values = new Dictionary<SNOActor, double>()
        {
            { SNOActor.x1_Dark_Angel, 0.395064 },
            { SNOActor.Corpulent_A, 0.3029622 },
            { SNOActor.TriuneVesselActivated_B, 0.4436438 },
            { SNOActor.x1_MoleMutant_Melee_A, 0.2662049 },
            { SNOActor.FallenShaman_A, 0.1774855 },
            { SNOActor.sandWasp_D, 0.07101808 },
            { SNOActor.snakeMan_Melee_A, 0.1774855 },
            { SNOActor.TriuneVesselActivated_A, 0.4436438 },
            { SNOActor.ZombieFemale_C, 0.1597416 },
            { SNOActor.LacuniMale_A, 0.3549244 },
            { SNOActor.Triune_Berserker_B, 0.3549244 },
            { SNOActor.TriuneCultist_D, 0.05327827 },
            { SNOActor.electricEel_A, 0.008918582 },
            { SNOActor.TerrorDemon_A_LootRun, 0.2662049 },
            { SNOActor.snakeMan_Caster_A, 0.1774855 },
            { SNOActor.x1_westmarch_rat_A, 0.008918582 },
            { SNOActor.skeleton_twoHander_A, 0.2218452 },
            { SNOActor.Shield_Skeleton_A, 0.08876603 },
            { SNOActor.p4_rat_A, 0.008918582 },
            { SNOActor.SkeletonSummoner_B, 0.1774855 },
            { SNOActor.MastaBlasta_Steed_A_NoMount, 0.4436438 },
            { SNOActor.skeletonMage_Fire_A, 0.07101808 },
            { SNOActor.x1_BogFamily_ranged_A, 0.4436438 },
            { SNOActor.QuillDemon_D, 0.07101808 },
            { SNOActor.MalletDemon_A, 0.9759604 },
            { SNOActor.Scavenger_A, 0.05327417 },
            { SNOActor.ZombieSkinny_E, 0.08876603 },
            { SNOActor.x1_bogBlight_Maggot_A, 0.0266584 },
            { SNOActor.x1_LR_WestmarchBat_A, 0.05327827 },
            { SNOActor.Shield_Skeleton_D, 0.08876603 },
            { SNOActor.ZombieSkinny_A, 0.08876603 },
            { SNOActor.sandMonster_A_Gauntlet, 0.5323632 },
            { SNOActor.Ghoul_C, 0.03553442 },
            { SNOActor.CryptChild_A, 0.008918582 },
            { SNOActor.Brickhouse_B, 0.6210827 },
            { SNOActor.SkeletonArcher_A, 0.07101808 },
            { SNOActor.ThousandPounder_C, 0.6210827 },
            { SNOActor.Angel_Corrupt_A, 0.2041013 },
            { SNOActor.Goatman_Melee_A, 0.2218452 },
            { SNOActor.p1_LR_BogBlight_A, 0.8872409 },
            { SNOActor.BileCrawler_A, 0.05327827 },
            { SNOActor.HoodedNightmare_A, 0.230713 },
            { SNOActor.SoulRipper_B, 0.1774855 },
            { SNOActor.graveRobber_A_Ghost, 0.1774855 },
            { SNOActor.azmodanBodyguard_B, 0.3549244 },
            { SNOActor.x1_electricEel_B, 0.1331258 },
            { SNOActor.creepMob_A, 0.3105647 },
            { SNOActor.x1_portalGuardianMinion_Ranged_A, 0.0266584 },
            { SNOActor.x1_leaperAngel_A, 0.2041013 },
            { SNOActor.electricEel_B, 0.008918582 },
            { SNOActor.LacuniMale_B, 0.3549244 },
            { SNOActor.FallenGrunt_B, 0.0266584 },
            { SNOActor.SkeletonArcher_B, 0.07101808 },
            { SNOActor.ZombieSkinny_B, 0.07101808 },
            { SNOActor.x1_WitherMoth_A, 0.1774855 },
            { SNOActor.FallenShaman_B, 0.1774855 },
            { SNOActor.FallenHound_B, 0.3549244 },
            { SNOActor.fastMummy_C, 0.1065058 },
            { SNOActor.GoatMutant_Ranged_A, 0.05327827 },
            { SNOActor.Goatman_Melee_A_Ghost, 0.2218452 },
            { SNOActor.shadowVermin_A, 0.008918582 },
            { SNOActor.x1_westmarchRanged_A, 0.3549244 },
            { SNOActor.Goatman_Ranged_A, 0.07102218 },
            { SNOActor.GoatMutant_Shaman_A, 0.2218452 },
            { SNOActor.MastaBlasta_Rider_A, 0.1774855 },
            { SNOActor.Spider_Elemental_Fire_tesla_A, 0.6210827 },
            { SNOActor.Unburied_C, 0.7098021 },
            { SNOActor.Spider_Elemental_Fire_A, 0.7098021 },
            { SNOActor.Swarm_B, 0.1774855 },
            { SNOActor.sandWasp_B, 0.07101808 },
            { SNOActor.Scavenger_C, 0.06215026 },
            { SNOActor.snakeMan_Melee_B, 0.1774855 },
            { SNOActor.morluSpellcaster_A, 0.3105647 },
            { SNOActor.fastMummy_B, 0.1065058 },
            { SNOActor.Ghoul_A, 0.07101808 },
            { SNOActor.ZombieSkinny_C, 0.08876603 },
            { SNOActor.Spiderling_A, 0.008918582 },
            { SNOActor.Goatman_Ranged_B, 0.07102218 },
            { SNOActor.x1_BogFamily_brute_A, 0.6210827 },
            { SNOActor.sandMonster_A, 0.5323632 },
            { SNOActor.Ghoul_E, 0.03553442 },
            { SNOActor.BigRed_A, 0.2662049 },
            { SNOActor.p1_LR_Ghost_A, 0.1774855 },
            { SNOActor.x1_Squigglet_A, 0.3016927 },
            { SNOActor.Shield_Skeleton_B, 0.08876603 },
            { SNOActor.TriuneCultist_C, 0.05327827 },
            { SNOActor.FallenGrunt_D, 0.0266584 },
            { SNOActor.X1_BigRed_Chronodemon_Burned_A, 0.3105647 },
            { SNOActor.x1_Shield_Skeleton_Westmarch_A, 0.2218452 },
            { SNOActor.skeletonMage_Poison_A, 0.08876603 },
            { SNOActor.FallenShaman_C, 0.1774855 },
            { SNOActor.FallenHound_C, 0.3549244 },
            { SNOActor.snakeMan_Melee_C, 0.1774855 },
            { SNOActor.snakeMan_Caster_C, 0.1774855 },
            { SNOActor.p1_LR_Ghost_C, 0.1774855 },
            { SNOActor.Succubus_C, 0.1774855 },
            { SNOActor.BileCrawler_A_Large_Aggro, 0.008918582 },
            { SNOActor.shadowVermin_B, 0.008918582 },
            { SNOActor.x1_sniperAngel_A, 0.1774855 },
            { SNOActor.p1_LR_Ghost_B, 0.1774855 },
            { SNOActor.SkeletonSummoner_D, 0.1774855 },
            { SNOActor.FallenHound_A, 0.3549244 },
            { SNOActor.Succubus_B, 0.1774855 },
            { SNOActor.Beast_D, 0.8872409 },
            { SNOActor.x1_SkeletonArcher_Westmarch_A, 0.08876603 },
            { SNOActor.Brickhouse_A, 0.6210827 },
            { SNOActor.X1_CaveRipper_A, 0.1774855 },
            { SNOActor.FleshPitFlyerSpawner_B, 0.5323632 },
            { SNOActor.FallenGrunt_A, 0.0266584 },
            { SNOActor.p1_LR_Ghost_D, 0.1774855 },
            { SNOActor.FallenChampion_C, 0.5323632 },
            { SNOActor.shadowVermin_C, 0.008918582 },
            { SNOActor.x1_WestmarchBat_C, 0.05327827 },
            { SNOActor.x1_BogFamily_melee_A, 0.008918582 },
            { SNOActor.TriuneSummoner_C, 0.1331258 },
            { SNOActor.Zombie_B, 0.1331258 },
            { SNOActor.Scavenger_B, 0.09763802 },
            { SNOActor.TriuneSummoner_B, 0.1331258 },
            { SNOActor.MastaBlasta_Rider_A_noride, 0.1774855 },
            { SNOActor.Ghoul_D, 0.03553442 },
            { SNOActor.FallenChampion_D, 0.5323632 },
            { SNOActor.SkeletonArcher_E, 0.07101808 },
            { SNOActor.Skeleton_A, 0.07101808 },
            { SNOActor.FallenChampion_B, 0.5323632 },
            { SNOActor.BileCrawler_B, 0.008918582 },
            { SNOActor.demonTrooper_B, 0.1331258 },
            { SNOActor.Skeleton_D_Fire, 0.08876603 },
            { SNOActor.x1_westmarchBrute_C, 0.9759604 },
            { SNOActor.Spiderling_B, 0.008918582 },
            { SNOActor.FallenShaman_D, 0.1774855 },
            { SNOActor.TriuneSummoner_D, 0.1331258 },
            { SNOActor.SoulRipper_A, 0.1774855 },
            { SNOActor.QuillDemon_A, 0.07101808 },
            { SNOActor.x1_Shield_Skeleton_Westmarch_Ghost_A, 0.1774855 },
            { SNOActor.Monstrosity_Scorpion_A, 0.04440634 },
            { SNOActor.Skeleton_B, 0.07101808 },
            { SNOActor.Spider_A, 0.2662049 },
            { SNOActor.morluSpellcaster_D, 0.3105647 },
            { SNOActor.Triune_Berserker_A, 0.3549244 },
            { SNOActor.FallenGrunt_C, 0.0266584 },
            { SNOActor.FleshPitFlyer_D, 0.04440225 },
            { SNOActor.skeletonMage_Lightning_A, 0.05327827 },
            { SNOActor.x1_Monstrosity_ScorpionBug_A, 0.008918582 },
            { SNOActor.x1_LR_demonFlyer_A, 0.1774855 },
            { SNOActor.FleshPitFlyer_B, 0.03553033 },
            { SNOActor.Goatman_Shaman_A, 0.1774855 },
            { SNOActor.FleshPitFlyer_A, 0.07101808 },
            { SNOActor.DuneDervish_C, 0.3549244 },
            { SNOActor.SkeletonSummoner_A, 0.1774855 },
            { SNOActor.x1_Shield_Skeleton_D, 0.08876603 },
            { SNOActor.SkeletonArcher_F, 0.07101808 },
            { SNOActor.skeletonMage_Cold_A, 0.03553033 },
            { SNOActor.skeletonMage_Fire_B, 0.1171522 },
            { SNOActor.Triune_Berserker_C, 0.3549244 },
            { SNOActor.Ghoul_B, 0.03553442 },
            { SNOActor.GoatMutant_Melee_A, 0.2218452 },
            { SNOActor.sandWasp_A, 0.07101808 },
            { SNOActor.x1_Beast_Skeleton_A, 0.7098021 },
            { SNOActor.x1_westmarchBrute_A, 0.9759604 },
            { SNOActor.GoatMutant_Ranged_B, 0.05327827 },
            { SNOActor.GoatMutant_Melee_B, 0.2218452 },
            { SNOActor.p1_LR_Ghost_Dark_A, 0.1331258 },
            { SNOActor.x1_FloaterAngel_A, 0.4436438 },
            { SNOActor.ZombieFemale_B, 0.1597416 },
            { SNOActor.Zombie_Inferno_C, 0.08876603 },
            { SNOActor.FallenLunatic_A, 0.0266584 },
            { SNOActor.Zombie_C, 0.1508696 },
            { SNOActor.Monstrosity_Scorpion_B, 0.04440634 },
            { SNOActor.TriuneSummoner_A, 0.1331258 },
            { SNOActor.Skeleton_C, 0.07457098 },
            { SNOActor.skeleton_twoHander_C, 0.2218452 },
            { SNOActor.FallenChampion_A, 0.5323632 },
            { SNOActor.x1_Skeleton_Westmarch_Ghost_A, 0.07101808 },
            { SNOActor.Spider_B, 0.1774855 },
            { SNOActor.Swarm_A, 0.08876603 },
            { SNOActor.SkeletonArcher_C, 0.07101808 },
            { SNOActor.fastMummy_A, 0.1065058 },
            { SNOActor.x1_Skeleton_Westmarch_A, 0.1508696 },
            { SNOActor.Zombie_D, 0.2218452 },
            { SNOActor.Lamprey_A, 0.05327827 },
            { SNOActor.azmodanBodyguard_A, 0.3549244 },
            { SNOActor.TriuneVessel_A, 0.05327827 },
            { SNOActor.graveDigger_B_Ghost, 0.08876603 },
            { SNOActor.Beast_B, 0.8872409 },
            { SNOActor.Unburied_A, 0.5323632 },
            { SNOActor.Sandling_A, 0.04440225 },
            { SNOActor.TentacleBear_A, 0.1331258 },
            { SNOActor.TentacleBear_B, 0.1331258 },
            { SNOActor.TentacleBear_C, 0.1331258 },
            { SNOActor.skeleton_twoHander_B, 0.2218452 },
            { SNOActor.TriuneCultist_A, 0.05327827 },
            { SNOActor.Triune_Summonable_A, 0.1774855 },
            { SNOActor.LacuniFemale_B, 0.1774855 },
            { SNOActor.ThousandPounder, 0.6210827 },
            { SNOActor.LacuniFemale_A, 0.1065058 },
            { SNOActor.CoreEliteDemon_A_NoPod, 0.1774855 },
            { SNOActor.ZombieSkinny_D, 0.08876603 },
            { SNOActor.x1_SkeletonArcher_Westmarch_Ghost_A, 0.04440225 },
            { SNOActor.TriuneCultist_B, 0.05327827 },
            { SNOActor.x1_Wraith_A, 0.3549244 },
            { SNOActor.demonTrooper_A, 0.1331258 },
            { SNOActor.x1_LR_DeathMaiden_A, 0.6654423 },
            { SNOActor.X1_armorScavenger_A, 0.5323632 },
            { SNOActor.X1_demonTrooper_Chronodemon_Burned_A, 0.1774855 },
            { SNOActor.x1_portalGuardianMinion_Melee_A, 0.0266584 },
            { SNOActor.X1_Skeleton_Fire_A, 0.1331258 },
        };
    }
}
