using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Framework;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Quests
{
    public static class BountyDataFactory
    {
        private static readonly List<BountyData> Bounties = new List<BountyData>();

        static BountyDataFactory()
        {
            //BrokenBounties();
            AddBounties();
            AddKillBossBounties();
            AddCursedBounties();
            AddGuardedGizmoBounties();
            AddCustomBounties();

            AddNewBounties();
        }

        public static BountyData GetBountyData(SNOQuest questId)
        {
            if (questId <= 0)
                return new BountyData();

            return Bounties.FirstOrDefault(b => b.QuestId == questId) ?? GetDynamicBounty(QuestData.GetQuestData(questId));
        }

        private static BountyData GetDynamicBounty(QuestData quest)
        {
            if (quest == null)
                return null;

            var questId = quest.QuestId;

            if (DynamicBountyDirectory.ContainsKey(questId))
            {
                var type = DynamicBountyDirectory[questId];
                var bountyData = new BountyData();

                switch (type)
                {
                    case DynamicBountyType.BoundShaman:
                    case DynamicBountyType.BlackKingsLegacy:
                    case DynamicBountyType.PlagueOfBurrowers:
                        bountyData = CreateChestAndClearBounty(quest);
                        break;

                    case DynamicBountyType.CursedShrines:
                        bountyData = CreateMultiShrineBounty(quest);
                        break;

                    default:
                        Core.Logger.Error("Dynamic Bounty is not supported {0} ({1})", quest.Name, quest.QuestId, type, bountyData);
                        break;
                }

                Core.Logger.Debug("Created Dynamic Bounty for {0} ({1}) Type={2} {3}", quest.Name, quest.QuestId, type, bountyData);

                return bountyData;
            }
            return null;
        }

        private static BountyData CreateChestAndClearBounty(QuestData quest)
        {
            if (quest == null)
            {
                Core.Logger.Debug("[CreateChestAndClearBounty] quest was null");
                return null;
            }

            if (quest.Waypoint == null)
            {
                Core.Logger.Debug($"[CreateChestAndClearBounty] quest {quest.Name} ({quest.QuestId}) waypoint was null");
                return null;
            }

            return new BountyData()
            {
                QuestId = quest.QuestId,
                Act = quest.Act,
                WorldId = quest.Waypoint.WorldSnoId,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(quest.QuestId, quest.Waypoint.WorldSnoId, BountyHelpers.DynamicBountyPortals.Values.ToList()),
                    new InteractWithGizmoCoroutine(quest.QuestId, SNOWorld.Invalid, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                    new ClearLevelAreaCoroutine(quest.QuestId),
                }
            };
        }

        private static BountyData CreateMultiShrineBounty(QuestData quest)
        {
            return new BountyData()
            {
                QuestId = quest.QuestId,
                Act = quest.Act,
                WorldId = quest.Waypoint.WorldSnoId,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(quest.QuestId, quest.Waypoint.WorldSnoId, BountyHelpers.DynamicBountyPortals.Values.ToList()),
                    new MoveToActorCoroutine(quest.QuestId, 0, SNOActor.P4_BountyGrounds_CursedShrine),
                    //new InteractWithGizmoCoroutine(quest.QuestId, 0, (int)SNOActor.P4_BountyGrounds_CursedShrine, -1, 5, 1, 20, true),
                    new GuardedGizmoCoroutine(quest.QuestId, SNOActor.P4_BountyGrounds_CursedShrine),
                    new ClearLevelAreaCoroutine(quest.QuestId)

                }
            };
        }

        public static Dictionary<SNOQuest, DynamicBountyType> DynamicBountyDirectory = new Dictionary<SNOQuest, DynamicBountyType>
        {
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shrines_Spire2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shaman_Spire1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shrines_Ete, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Burrowers_Ete, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Champ_Spire2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Burrowers, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shaman_Spire2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Burrowers_Spire2, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Champ_Spire1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shaman_Cem, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shrines_BTower1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shaman_BTower1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Champ, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Burrowers, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shrines_Spire2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shaman_GoH2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shaman_Cem, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shaman_Ete, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shaman_Cor, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shaman_Spire1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shaman_Spire1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shaman_Spire1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Shrines_Cor, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shrines_Spire2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shrines_Cem, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shrines_GoH2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shrines_Spire2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Champ_Cor, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Champ_BTower1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Champ_Cor, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Champ_Ete, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Champ_GoH2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Burrowers, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Burrowers_Spire1, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Burrowers_BTower1, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Burrowers_Cor, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Burrowers_Spire2, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Burrowers_Spire1, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shrines_Ete, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Shrines_BTower1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Champ_Cem, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shrines_Spire1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Shrines_Ete, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shrines_Spire1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Champ_Ete, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Champ, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shrines_Goh2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Burrowers_Cor, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_GoH2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Burrowers_Ete, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shaman_Spire2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Burrowers, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shaman_Cem, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shaman_GoH2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shrines_Ete, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Champ_Spire2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shaman_Ete, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shaman_Ete, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Champ_Spire2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Champ, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shrines_Spire2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Champ_Cor, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Champ_GoH2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_BTower1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shaman_Cem, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Shrines_GoH2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shrines_Cor, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Champ_Cem, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shaman_Spire2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shrines_Cem, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shaman_BTower1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Burrowers_Cem, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shrines_Spire1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Champ_Ete, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shaman_GoH2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Champ_Cem, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Champ, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shrines_BTower1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shaman_Ete, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shrines_Spire1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shaman_Cor, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shrines_GoH2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shaman_Cor, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shaman_Spire2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Burrowers_GoH2, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shrines_Ete, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shaman_GoH2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Champ_BTower1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shaman_Cor, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Burrowers_GoH2, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shrines_GoH2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Burrowers, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Champ, DynamicBountyType.BlackKingsLegacy}, //*
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Champ_GoH2, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Champ_Cor, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shrines_Cem, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Champ_BTower1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Burrowers_Cor, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shrines_GoH2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Champ_Ete, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Champ_Spire1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shrines_BTower1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shaman_Cem, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shrines_Cor, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Burrowers_Spire1, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shaman_Spire2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shaman_BTower1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Burrowers, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shaman_GoH2, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Burrowers_Cem, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shrines_Ete, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Shrines_Spire1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shrines_BTower1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Champ_Spire1, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Burrowers_Cem, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Champ, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shrines_Cem, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Shaman_BTower1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shrines_Cor, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Keep_Burrowers_BTower1, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Champ, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Neph_Shrines_Spire1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Zolt_Shrines_BTower1, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Burrowers_GoH2, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Neph_Shaman_Ete, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Champ_Cem, DynamicBountyType.BlackKingsLegacy},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Burrowers_Spire2, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shaman_Cor, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Shrines, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Crypt_Shrines_Cem, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Shaman, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Shrines_Cor, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Burrowers_BTower1, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Shrines_Spire2, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shrines_Cor, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Swr_Shaman_Spire1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A4_bounty_ground_Crypt_Shaman_BTower1, DynamicBountyType.BoundShaman},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Keep_Burrowers_Ete, DynamicBountyType.PlagueOfBurrowers},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Leoric_Shrines_Cem, DynamicBountyType.CursedShrines},
            {SNOQuest.P4_Bounty_A5_bounty_ground_Zolt_Shrines, DynamicBountyType.CursedShrines},
        };

        public enum DynamicBountyType
        {
            None = 0,
            BoundShaman,
            CursedShrines,
            BlackKingsLegacy,
            PlagueOfBurrowers
        }

        private static void BrokenBounties()
        {
            // this is broken because it takes forever to explore oasis
            // to find these objectives and there doesnt appear to be a 
            // pattern to where theyre placed. Navigation is also busted
            // for this area, visited nodes are not being flagged properly
            // when in radius
			
            // A5 - Bounty: Demon Souls (SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 56,
                Coroutines = new List<ISubroutine>
                {
                   new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, 1),

                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, 2912417),
					
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone"),
                    // x1_Fortress_Event_Worldstone_Jamella (SNOActor.x1_Fortress_Event_Worldstone_Jamella) Distance: 27.00935
					new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, SNOActor.x1_Fortress_Event_Worldstone_Jamella, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone", new Vector3(165.6456f, 61.12549f, 0.361735f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, 10, SNOActor.x1_Death_Orb_Little_Event_Worldstone, 0, 20),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone", new Vector3(176.4594f, 153.0325f, 0.471462f)),
					new InteractionCoroutine(SNOActor. x1_Fortress_Portal_Switch, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone", new Vector3(68.77673f, 154.9171f, 0.4347426f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, 10, SNOActor.x1_Death_Orb_Little_Event_Worldstone, 0, 20),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone", new Vector3(51.32025f, 134.7082f, 0.09999999f)),
					new InteractionCoroutine(SNOActor. x1_Fortress_Portal_Switch, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone", new Vector3(70.68292f, 67.76535f, 0.4456832f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, 5000),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, 10, SNOActor.x1_Death_Orb_Little_Event_Worldstone, 0, 20),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone", new Vector3(113.8264f, 13.43758f, 0.1000001f)),
					new InteractionCoroutine(SNOActor. x1_Fortress_Portal_Switch, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, "x1_fortress_SE_05_Worldstone", new Vector3(185.505f, 200.7375f, 0.1f)),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels, SNOWorld.x1_fortress_level_01, SNOActor.x1_Fortress_Event_Worldstone_Jamella, 0, 5),
					
					new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Event_SoulsOfAngels)
                }
            });

            // A5 - 현상금 사냥: 보물고 (SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 55,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, 2912417),
					
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01"),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(60.45612f, 178.8389f, 0.1f)),
					new InteractionCoroutine(SNOActor.x1_Catacombs_Nephalem_Event_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(87.01239f, 48.92456f, 0.1f)),
					new InteractionCoroutine(SNOActor.x1_Catacombs_Nephalem_Event_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(159.8382f, 154.6359f, 0.5821021f)),
					new InteractionCoroutine(SNOActor.x1_Catacombs_Nephalem_Event_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(77.59277f, 55.68237f, 0.216823f)),
					new InteractionCoroutine(SNOActor.x1_Catacombs_Nephalem_Event_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(66.62976f, 178.5327f, 0.09999999f)),
					new InteractionCoroutine(SNOActor.x1_Catacombs_Nephalem_Event_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(92.78125f, 61.78046f, 0.5280025f)),
					new InteractionCoroutine(SNOActor.x1_Catacombs_Nephalem_Event_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(161.8808f, 163.7767f, 0.6154587f)),
					new InteractionCoroutine(SNOActor.x1_Catacombs_Nephalem_Event_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SEW_01", new Vector3(144.2595f, 116.1751f, 0.09999999f)),
					// x1_Catacombs_chest_rare_treasureRoom (SNOActor.x1_Catacombs_chest_rare_treasureRoom) Distance: 5.573406
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, SNOWorld.x1_Catacombs_Level01, SNOActor.x1_Catacombs_chest_rare_treasureRoom, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_TreasureRoom, 90, 0, 0, 45),
                }
            });

        }

        private static void AddKillBossBounties()
        {
            //ActorId: 433670, Type: Gizmo, Name: x1_Global_Chest_BossBounty-73584, Distance2d: 6.557568, CollisionRadius: 11.28195, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 
            //ActorId: 433670, Type: Gizmo, Name: x1_Global_Chest_BossBounty-68275, Distance2d: 78.14486, CollisionRadius: 11.28195, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 
            //ActorId: 433670, Type: Gizmo, Name: x1_Global_Chest_BossBounty-23057, Distance2d: 8.477567, CollisionRadius: 11.28195, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 

            // A4 - Bounty: Kill Izual (SNOQuest.X1_Bounty_A4_Spire_Kill_Izual)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_Izual,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_spire_exterior, // Enter the final worldId here
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.a4dun_spire_exterior },
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 45,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A4_Spire_Kill_Izual, SNOWorld.a4dun_Spire_Level_01, 1038619951, true),
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A4_dun_Spire_01 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A4_dun_Spire_01 || BountyHelpers.ScanForMarker(1038619951, 50) != null)),
					
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_Spire_Kill_Izual, SNOWorld.a4dun_Spire_Level_01, SNOWorld.a4dun_spire_exterior, 1038619951, SNOActor.BossPortal_Izual),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_Spire_Kill_Izual),
                }
            });

            // A2 - Bounty: Kill Belial (SNOQuest.X1_Bounty_A2_Caldeum_Kill_Belial)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Caldeum_Kill_Belial,
                Act = Act.A2,
                WorldId = SNOWorld.A2_Belial_Room_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 20,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Caldeum_Kill_Belial, SNOWorld.a2dun_Cald_Uprising, SNOWorld.A2_Belial_Room_01, 1074632599, SNOActor.Boss_Portal_Belial),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Caldeum_Kill_Belial),
                }
            });
            
            // A5 - Bounty: Kill Adria (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_Adria)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_Adria,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Adria_Boss_Arena_02, // Enter the final worldId here
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.x1_Adria_Boss_Arena },
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 55,
                Coroutines = new List<ISubroutine>
                {
                    //ActorId: 3349, Type: Monster, Name: Belial-1894, Distance2d: 49.40654, CollisionRadius: 0, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_Adria, SNOWorld.x1_Catacombs_Level02, -131340091),
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.x1_Catacombs_Level02 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.x1_Catacombs_Level02 || BountyHelpers.ScanForMarker(-131340091, 50) != null)),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_Adria, SNOWorld.x1_Catacombs_Level02, SNOWorld.x1_Adria_Boss_Arena_02, -131340091, SNOActor.x1_Boss_Portal_Adria),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_Adria),
                }
            });

            // A3 - Bounty: Kill Azmodan (SNOQuest.X1_Bounty_A3_Crater_Kill_Azmodan)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Azmodan,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Azmodan_Arena, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Azmodan, SNOWorld.a3Dun_Crater_Level_03, SNOWorld.a3dun_Azmodan_Arena, 1743679055, SNOActor.Boss_Portal_Azmodan),
                    new MoveToPositionCoroutine(SNOWorld.a3dun_Azmodan_Arena, new Vector3(602, 608, 37)),

                    new MoveToPositionCoroutine(SNOWorld.a3dun_Azmodan_Arena, new Vector3(553, 556, 4)),

                    new MoveToPositionCoroutine(SNOWorld.a3dun_Azmodan_Arena, new Vector3(512, 562, 0)),

                    new MoveToPositionCoroutine(SNOWorld.a3dun_Azmodan_Arena, new Vector3(492, 573, 0)),

                    new MoveToPositionCoroutine(SNOWorld.a3dun_Azmodan_Arena, new Vector3(479, 524, 0)),

                    new MoveToPositionCoroutine(SNOWorld.a3dun_Azmodan_Arena, new Vector3(419, 403, 0)),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Azmodan),
                }
            });

            // A1 - Bounty: Kill the Skeleton King (SNOQuest.X1_Bounty_A1_Cathedral_Kill_SkeletonKing)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_SkeletonKing,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_King_Level08, // Enter the final worldId here
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trDun_Level07D },
                QuestType = BountyQuestType.KillBossBounty,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Kill_SkeletonKing, SNOWorld.a1trDun_Level07, SNOWorld.a1trDun_King_Level08, -267501088, SNOActor.Boss_Portal_SkeletonKing),
                    new MoveToPositionCoroutine(SNOWorld.a1trDun_King_Level08, new Vector3(336, 260, 20)),
					
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Kill_SkeletonKing, SNOWorld.a1trDun_King_Level08, SNOActor.SkeletonKing),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Kill_SkeletonKing, SNOWorld.a1trDun_King_Level08, SNOActor.SkeletonKing, 0, 3),
					
                    new MoveToPositionCoroutine(SNOWorld.a1trDun_King_Level08, new Vector3(338, 288, 15)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Kill_SkeletonKing, SNOWorld.a1trDun_King_Level08, 20000),
					
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Kill_SkeletonKing)
                }
            });

            // A3 - Bounty: Kill Cydaea (SNOQuest.X1_Bounty_A3_Crater_Kill_Cydaea)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Cydaea,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level04B, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 38,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Cydaea, SNOWorld.a3dun_Crater_ST_Level01B, SNOWorld.a3dun_Crater_ST_Level02B, 43541819, SNOActor.g_Portal_ArchTall_Orange),
					
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A3_dun_Crater_ST_Level02B }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A3_dun_Crater_ST_Level02B || BountyHelpers.ScanForMarker(43541885, 50) != null)),
					//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Cydaea, SNOWorld.a3dun_Crater_ST_Level02B, 43541885),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Cydaea, SNOWorld.a3dun_Crater_ST_Level02B, SNOWorld.a3dun_Crater_ST_Level04B, 43541885, SNOActor.Boss_Portal_MistressOfPain),
					
					new MoveToPositionCoroutine(SNOWorld.a3dun_Crater_ST_Level04B, new Vector3(1002, 1264, 53)), 
					new MoveToPositionCoroutine(SNOWorld.a3dun_Crater_ST_Level04B, new Vector3(1045, 1279, 40)), 
					new MoveToPositionCoroutine(SNOWorld.a3dun_Crater_ST_Level04B, new Vector3(1160, 1266, 40)), 
					new MoveToPositionCoroutine(SNOWorld.a3dun_Crater_ST_Level04B, new Vector3(1263, 1163, 40)), 
					new MoveToPositionCoroutine(SNOWorld.a3dun_Crater_ST_Level04B, new Vector3(1275, 1081, 40)), 
					new MoveToPositionCoroutine(SNOWorld.a3dun_Crater_ST_Level04B, new Vector3(1234, 1083, 26)), 
					new MoveToPositionCoroutine(SNOWorld.a3dun_Crater_ST_Level04B, new Vector3(1151, 1092, 0)),
					
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Cydaea),
                }
            });

            // A5 - Bounty: Kill Urzael (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Urzael)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Urzael,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Urzael_Arena, // Enter the final worldId here
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.x1_Urzael_Arena },
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 52,
                Coroutines = new List<ISubroutine>
                {
//                 new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Urzael, SNOWorld.X1_WESTM_ZONE_03, -1689330047, true),
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.X1_WESTM_ZONE_03 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.X1_WESTM_ZONE_03 || BountyHelpers.ScanForMarker(-1689330047, 50) != null)),
					new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Urzael, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.x1_Urzael_Arena, -1689330047, SNOActor.x1_Urzael_Bossportal_OpenWorld),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Urzael),
                }
            });

            // A4 - Bounty: Kill Hammermash (SNOQuest.X1_Bounty_A4_HellRift_Kill_Hammersmash) 
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_HellRift_Kill_Hammersmash,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Hell_Portal_02, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 47,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A4_HellRift_Kill_Hammersmash, SNOWorld.a4dun_Hell_Portal_01, 984446737),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_HellRift_Kill_Hammersmash, SNOWorld.a4dun_Hell_Portal_01, SNOWorld.a4dun_Hell_Portal_02, 984446737, SNOActor.a4_Heaven_Gardens_HellPortal),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A4_HellRift_Kill_Hammersmash, SNOWorld.a4dun_Hell_Portal_02, 614820904),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_HellRift_Kill_Hammersmash),

                }
            });

            // A3 - Bounty: Kill Ghom (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Ghom)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Ghom,
                Act = Act.A3,
                WorldId = SNOWorld.Gluttony_Boss, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 31,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Ghom, SNOWorld.a3Dun_Keep_Level05, 2102427919),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Ghom, SNOWorld.a3Dun_Keep_Level05, SNOWorld.Gluttony_Boss, 2102427919, SNOActor.Boss_Portal_Gluttony),
					
					new MoveToPositionCoroutine(SNOWorld.Gluttony_Boss, new Vector3(532, 359, 0)), 
					new MoveToPositionCoroutine(SNOWorld.Gluttony_Boss, new Vector3(488, 360, 0)),
					new MoveToPositionCoroutine(SNOWorld.Gluttony_Boss, new Vector3(438, 360, 0)),

                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Ghom),
                }
            });

            // A2 - Bounty: Kill Zoltun Kulle (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_ZoltunKulle)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_ZoltunKulle,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_BossFight_Level04, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 25,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Kill_ZoltunKulle, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_BossFight_Level04, 46443076, SNOActor.Boss_Portal_Blacksoulstone),
                    new MoveToPositionCoroutine(SNOWorld.a2Dun_Zolt_BossFight_Level04, new Vector3(75, 61, 0)),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Kill_ZoltunKulle)
                }
            });

            // A1 - Bounty: Kill Queen Araneae (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_QueenAraneae)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_QueenAraneae,
                Act = Act.A1,
                WorldId = SNOWorld.a1Dun_SpiderCave_02, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 12,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_QueenAraneae, SNOWorld.a1Dun_SpiderCave_01, 1317387500, true),
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A1_C6_SpiderCave_01_Main }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A1_C6_SpiderCave_01_Main || BountyHelpers.ScanForMarker(1317387500, 50) != null)),

                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_QueenAraneae, SNOWorld.a1Dun_SpiderCave_01, SNOWorld.a1Dun_SpiderCave_02, 1317387500, SNOActor.Boss_Portal_SpiderQueen),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_QueenAraneae),
                }
            });

            // A5 - Bounty: Kill Malthael(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Malthael_Boss_Arena, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
               {
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael, SNOWorld.x1_fortress_level_02, "x1_fortress_island_NW_01_Waypoint", new Vector3(88.71082f, 181.0916f, 10.28859f)),
                    new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael, SNOWorld.x1_fortress_level_02, 4),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael, SNOWorld.x1_fortress_level_02, SNOWorld.X1_fortress_malthael_entrance, 1012176886, SNOActor.g_Portal_ArchTall_Blue),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael, SNOWorld.X1_fortress_malthael_entrance, SNOWorld.X1_Malthael_Boss_Arena, -144918420, SNOActor.x1_Fortress_Malthael_Boss_Portal),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Malthael)
                }
            });

            // A4 - Bounty: Kill Diablo (SNOQuest.X1_Bounty_A4_Spire_Kill_Diablo)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_Diablo,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Diablo_Arena_Phase3, // Enter the final worldId here
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A4_dun_Diablo_Arena, SNOLevelArea.a4dun_Diablo_ShadowRealm_01, SNOLevelArea.A4_dun_Diablo_Arena_Phase3 },
                QuestType = BountyQuestType.KillBossBounty,
                ////WaypointNumber = 46,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A4_Spire_Kill_Diablo, SNOWorld.a4dun_Spire_Level_02, -72895048, true),
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A4_dun_Spire_02 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A4_dun_Spire_02 || BountyHelpers.ScanForMarker(-72895048, 50) != null)),
				
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_Spire_Kill_Diablo, SNOWorld.a4dun_Spire_Level_02, SNOWorld.a4dun_spire_DiabloEntrance, -72895048, SNOActor.BossPortal_TyraelPurpose),

                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_Spire_Kill_Diablo, SNOWorld.a4dun_spire_DiabloEntrance, SNOWorld.a4dun_Diablo_Arena, -753198453, SNOActor.Boss_Portal_Diablo),

                    new MoveToPositionCoroutine(SNOWorld.a4dun_Diablo_Arena, new Vector3(645, 644, 0)),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Diablo_Arena, new Vector3(579, 576, 0)),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Diablo_Arena, new Vector3(514, 511, 0)),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Diablo_Arena, new Vector3(481, 479, 19)),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Diablo_Arena, new Vector3(376, 375, 40)),

                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_Spire_Kill_Diablo)
                }
            });

            // A3 - Bounty: Kill the Siegebreaker Assault Beast (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Siegebreaker)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Siegebreaker,
                Act = Act.A3,
                //TODO: Enter the final worldId here
                WorldId = (SNOWorld)SNOQuest.X1_Bounty_A3_Battlefields_Kill_Siegebreaker, 
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A3_Battlefield_C },
                QuestType = BountyQuestType.KillBossBounty,
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                //WaypointNumber = 34,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Siegebreaker, SNOWorld.A3_Battlefields_02, -443762283, true),					
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Siegebreaker, SNOWorld.A3_Battlefields_02, SNOWorld.A3_Battlefields_03, -443762283, SNOActor.Boss_Portal_Siegebreaker),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Siegebreaker)
                }
            });

            // A1 - Bounty: Kill the Butcher (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Butcher)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Butcher,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_ButchersLair_02, // Enter the final worldId here
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trDun_ButchersLair_02 },
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 17,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Butcher, SNOWorld.trDun_Leoric_Level03, 356899046, true),
					//new ExplorationCoroutine(new HashSet<int>() { 19776 }, breakCondition: () => (Core.Player.LevelAreaId != 19776 || BountyHelpers.ScanForMarker(356899046, 50) != null)),
		
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Butcher, SNOWorld.trDun_Leoric_Level03, 0, 356899046, SNOActor.Boss_Portal_Butcher),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Butcher)
                }
            });


            // A2 -: Kill Maghda (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Maghda)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Maghda,
                Act = Act.A2,
                WorldId = SNOWorld.caOut_Cellar_Alcarnus_Main,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A2_caOut_Cellar_Alcarnus_Main },
                QuestType = BountyQuestType.KillBossBounty,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                Coroutines = new List<ISubroutine>
                    {
 //                       new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Maghda, SNOWorld.caOUT_Town, 1742967132),
 //						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1262, 1299, 184)),
//						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1133, 1401, 184)), 
//						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1141, 1528, 184)), 
//						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(969, 1474, 197)), 
//						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(979, 1297, 197)),
						
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Maghda, SNOWorld.caOUT_Town, SNOWorld.caOut_Cellar_Alcarnus_Main, 1742967132, SNOActor.Boss_Portal_Maghda),
                        new MoveToPositionCoroutine(SNOWorld.caOut_Cellar_Alcarnus_Main, new Vector3(216, 196, 0)),
                        new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Maghda)
                    }
            });

            // A4 - Kill Rakanoth (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Rakanoth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Rakanoth,
                Act = Act.A4,
                WorldId = SNOWorld.a4Dun_LibraryOfFate,
                QuestType = BountyQuestType.KillBossBounty,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Rakanoth, SNOWorld.a4dun_Garden_of_Hope_01, 739323140),
						
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Rakanoth, SNOWorld.a4dun_Garden_of_Hope_01, SNOWorld.a4Dun_LibraryOfFate, 739323140, SNOActor.Boss_Portal_Despair),
                        new MoveToPositionCoroutine(SNOWorld.a4Dun_LibraryOfFate, new Vector3(351, 443, 0)),
                        new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Rakanoth)
                    }
            });
        }

        private static void AddGuardedGizmoBounties()
        {
            //A2 - Ancient Devices - Interact with operated gizmo repeatly
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Boneyards_Camp_AncientDevices,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.GuardedGizmo,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_Boneyard_01,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A2_Boneyards_Camp_AncientDevices, SNOActor.a2dun_Zolt_ibstone_A_PortalRoulette_Mini)
                }
            });
			
            // A2 - Blood and Iron (SNOQuest.px_Bounty_A2_Oasis_Camp_IronWolves)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Oasis_Camp_IronWolves,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.GuardedGizmo,
                WaypointLevelAreaId = SNOLevelArea.A2_caOut_Oasis,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A2_Oasis_Camp_IronWolves, SNOActor.px_Oasis_Camp_IronWolves)
                }
            });

            // A1 - Bounty: Templar Inquisition
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A1_Wilderness_Camp_TemplarPrisoners,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOut_TristramWilderness },
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A1_Wilderness_Camp_TemplarPrisoners,SNOActor.px_Wilderness_Camp_TemplarPrisoners)
                }
            });


            //A1 - The Queen's Dessert
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A1_SpiderCaves_Camp_Cocoon,
                Act = Act.A1,
                WorldId = SNOWorld.a1Dun_SpiderCave_01,
                WaypointLevelAreaId = SNOLevelArea.A1_C6_SpiderCave_01_Main,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A1_SpiderCaves_Camp_Cocoon, SNOActor.px_SpiderCaves_Camp_Cocoon)
                }
            });


            //A2 - Prisoners of the Cult
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_StingingWinds_Camp_CaldeumPrisoners,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_BorderlandsKhamsin,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1355, 273, 169)),
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1300, 342, 161)),
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1334, 410, 161)),
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1384, 436, 162)),
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1426, 467, 173)),
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1402, 525, 174)),
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1330, 987, 174)),
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A2_StingingWinds_Camp_CaldeumPrisoners,SNOActor.px_caOut_Cage_BountyCamp)
                }
            });


            //A3 - Catapult Command
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A3_Ramparts_Camp_CatapultCommander,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_rmpt_Level02,
                WaypointLevelAreaId = SNOLevelArea.A3_dun_rmpt_Level02,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A3_Ramparts_Camp_CatapultCommander,SNOActor.px_Bounty_Ramparts_Camp_Switch)
                }
            });

            //A3 - Demon Gates
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A3_Crater_Camp_AzmodanMinions,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_02,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A3_Crater_Camp_AzmodanMinions,SNOActor.px_Bounty_Camp_azmodan_fight_spawner)
                }
            });


            //A4 - Tormented Angels
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A4_GardensOfHope_Camp_TrappedAngels,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_B,
                QuestType = BountyQuestType.GuardedGizmo,
                WaypointLevelAreaId = SNOLevelArea.A4_dun_Garden_of_Hope_B,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A4_GardensOfHope_Camp_TrappedAngels,SNOActor.px_Bounty_Camp_TrappedAngels)
                }
            });


            //A4 - The Hell Portals
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A4_Spire_Camp_HellPortals,
                Act = Act.A4,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A4_Spire_Camp_HellPortals,SNOActor.px_Bounty_Camp_Hellportals_Frame)
                }
            });


            //A5 - Rathma's Gift 
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A5_Graveyard_Camp_SkeletonJars,
                Act = Act.A5,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A5_Graveyard_Camp_SkeletonJars, SNOActor.X1_westm_Necro_Jar_of_Souls_Camp_graveyard)
                }
            });


            //A5 - Death's Embrace
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A5_WestmarchFire_Camp_DeathOrbs,
                Act = Act.A5,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A5_WestmarchFire_Camp_DeathOrbs, SNOActor.px_Bounty_Death_Orb_Little)
                }
            });


        }

        private static void AddCustomBounties()
        {
            // A5 - Bounty: The True Son of the Wolf (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Westm_Int_Gen_A_04_KingEvent03, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_Westm_Int_Gen_A_04_KingEvent03, -752748507, SNOActor.g_portal_RandomWestm),
                    new MoveToPositionCoroutine(SNOWorld.X1_Westm_Int_Gen_A_04_KingEvent03, new Vector3(311, 393, 10)),
                    // x1_NPC_Westmarch_Aldritch (SNOActor.x1_NPC_Westmarch_Aldritch) Distance: 9.175469
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03, SNOWorld.X1_Westm_Int_Gen_A_04_KingEvent03, SNOActor.x1_NPC_Westmarch_Aldritch, 0, 5),

					// x1_NPC_Westmarch_Aldritch (SNOActor.x1_NPC_Westmarch_Aldritch) Distance: 8.62778
					new KillUniqueMonsterCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03, SNOWorld.X1_Westm_Int_Gen_A_04_KingEvent03, SNOActor.x1_NPC_Westmarch_Aldritch, 0),
                    //new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03, 60, SNOActor.x1_NPC_Westmarch_Aldritch, 0, 45),

					new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03),
                }
            });

            // A5 - Firestorm (SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Abattoir_Random01_B,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_WESTM_ZONE_01, -660641889),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace,SNOWorld.X1_WESTM_ZONE_01,SNOWorld.X1_Abattoir_Random01,-660641889 ,0),

                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace,SNOWorld.X1_Abattoir_Random01,SNOWorld.X1_Abattoir_Random01_B,2115491808,0),
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 2912417),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(118.8754f, 108.0339f, 10.76492f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(72.1983f, 134.9289f, 10.80386f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(123.1787f, 163.9214f, 10.77483f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(152.291f, 126.2562f, 10.81015f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(118.8754f, 108.0339f, 10.76492f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(72.1983f, 134.9289f, 10.80386f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(123.1787f, 163.9214f, 10.77483f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(152.291f, 126.2562f, 10.81015f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(118.8754f, 108.0339f, 10.76492f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(72.1983f, 134.9289f, 10.80386f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(123.1787f, 163.9214f, 10.77483f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(152.291f, 126.2562f, 10.81015f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(118.8754f, 108.0339f, 10.76492f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(72.1983f, 134.9289f, 10.80386f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(123.1787f, 163.9214f, 10.77483f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, "x1_abattoir_NSEW_06", new Vector3(152.291f, 126.2562f, 10.81015f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, SNOWorld.X1_Abattoir_Random01_B, 5000),

                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace)

                    }
            });
			
            // A5 - Bounty: The Crystal Prison (SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 57,
                Coroutines = new List<ISubroutine>
                {
//                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified, SNOWorld.x1_fortress_level_02, 2912417),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified, SNOWorld.x1_fortress_level_02, "x1_fortress_NW_05_mousetrap", new Vector3(104.2493f, 194.9052f, -9.899999f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified, SNOWorld.x1_fortress_level_02, 2000),
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 5.45564					
					new InteractionCoroutine(SNOActor.x1_Fortress_Portal_Switch, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)),
					//new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified, SNOWorld.x1_fortress_level_02, 1),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified, SNOWorld.x1_fortress_level_02, "x1_fortress_NW_05_mousetrap", new Vector3(136.1091f, 142.7999f, -19.9f)),
                    // x1_Fortress_Crystal_Prison_MouseTrap (SNOActor.x1_Fortress_Crystal_Prison_MouseTrap) Distance: 7.876552
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified, SNOWorld.x1_fortress_level_02, SNOActor.x1_Fortress_Crystal_Prison_MouseTrap, 0, 5),
 //                   new AttackCoroutine(SNOActor.x1_Fortress_Crystal_Prison_MouseTrap),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_Fortified, 20, 0, 0, 45),
                }
            });
			
            // A2 - Clear the Mysterious Cave (SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2)
            // If delete blacklist NPC actorID in GameData.cs, this quest succeed. But It takes long time because of movement problem.
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Cave_MapDungeon_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        // DBs navigation in this little nook of the map is really bad.
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(3193, 4639, 97)),
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(3237, 4674, 97)),
						
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, SNOWorld.caOUT_Town, -1615133822, true),						
						//new MoveToActorCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, SNOWorld.caOUT_Town, SNOActor.A2_UniqueVendor_Event_MapVendor),

						// A2_UniqueVendor_Event_MapVendor (SNOActor.A2_UniqueVendor_Event_MapVendor) Distance: 9.093847
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, SNOWorld.caOUT_Town, SNOActor.A2_UniqueVendor_Event_MapVendor),
						
						new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, 10, SNOActor.A2_UniqueVendor_Event_MapVendor, -1615133822, 45),

                        // Try interact with the NPC.
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, SNOWorld.caOUT_Town, 1000),
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, SNOWorld.caOUT_Town, SNOActor.A2_UniqueVendor_Event_MapVendor),
                        new InteractionCoroutine(SNOActor.A2_UniqueVendor_Event_MapVendor, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
						
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, SNOWorld.caOUT_Town, 6000),

                        // try interact wtih portal or timeout 
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2,SNOWorld.caOUT_Town, SNOWorld.a2Dun_Cave_MapDungeon_Level01, -1615133822, SNOActor.g_Portal_Square_Blue, TimeSpan.FromSeconds(15)),
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, SNOWorld.a2Dun_Cave_MapDungeon_Level01, 1109456219),

                        // Finish Bounty
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2,SNOWorld.a2Dun_Cave_MapDungeon_Level01, 0, 1109456219, SNOActor.g_Portal_ArchTall_Blue),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2)
                    }
            });

            // A2 - Bounty: Lost Treasure of Khan Dakab (SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab)
            // Doesnt interact with door properly
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 24,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, 913850831, true),

					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI"),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_WaterPuzzle", new Vector3(90.85815f, 45.47217f, 79.77377f)),
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle),
 //                 new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle, 0, 5),
					new InteractionCoroutine(SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_WaterPuzzle", new Vector3(48.02637f, 85.47168f, 79.74346f)),
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle),
//                  new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle, 0, 5),
					new InteractionCoroutine(SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),

					//new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_WaterPuzzle", new Vector3(111.7661f, 115.1221f, 73.43011f)),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_WaterPuzzle", new Vector3(93.60815f, 91.57959f, 73.58156f)),
                    //new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, 913850831, SNOActor.g_Portal_ArchTall_Blue),
					// g_Portal_ArchTall_Blue (SNOActor.g_Portal_ArchTall_Blue) Distance: 2.846122
					//new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, SNOActor.g_Portal_ArchTall_Blue, 913850831, 5),
					new InteractionCoroutine(SNOActor.g_Portal_ArchTall_Blue, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					
					
					//new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, "a2dun_Aqd_N_01", new Vector3(38.64484f, 49.05707f, -0.9f)),
                    //new ExplorationCoroutine(new HashSet<int>() { 158594 }, breakCondition: () => (BountyHelpers.ScanForActor(219880) == null)),
                    // a2dun_Aqd_Act_Lever_FacePuzzle_02 (219880) Distance: 20.90631
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, SNOActor.a2dun_Aqd_Act_Lever_FacePuzzle_02),
					new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.caOUT_Town, 1000),
					new InteractionCoroutine(SNOActor.a2dun_Aqd_Act_Lever_FacePuzzle_02, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					
                    // a2dun_Aqd_GodHead_Door_LargePuzzle-8357
					//new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, "a2dun_Aqd_NSE_Vault_01", new Vector3(51.49591f, 21.63858f, -1.504425f)),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, -1469964931),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, SNOActor.a2dun_Aqd_GodHead_Door_LargePuzzle, 0),
					
                    // a2dun_Aqd_Chest_Special_FacePuzzle_Large (190524) Distance: 27.1103					
                    //new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, (int)SNOActor.a2dun_Aqd_Chest_Special_FacePuzzle_Large),
                    //new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Large, 190524, 0, 5),
					new InteractionCoroutine(SNOActor.a2dun_Aqd_Chest_Special_FacePuzzle_Large, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, 30, 0, 0, 45),
                }
            });
            
            // A2 - Bounty : Sardar's Treasure (SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 24,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.caOUT_Town, 922565181),
                    //Scene: caOut_Oasis_Sub80_Cenote_DungeonEntranceA, SnoId: 68275,
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.caOUT_Town, "caOut_Oasis_Sub80_Cenote_DungeonEntranceA"),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.caOUT_Town, SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle, -1, 5),

                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.caOUT_Town, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small, 922565181, SNOActor.g_Portal_Rectangle_Blue),

                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small, SNOActor.a2dun_Aqd_Act_Lever_FacePuzzle_01),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small, SNOActor.a2dun_Aqd_Act_Lever_FacePuzzle_01, 0, 5),

                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small, 2000),

                    //153836 a2dun_Aqd_GodHead_Door (Door) 
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small, "a2dun_Aqd_NSE_Vault_01", new Vector3(47.53217f, 11.98969f, -1.867057f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small, "a2dun_Aqd_NSE_Vault_02", new Vector3(53.40659f, 48.12427f, -9.900001f)),
					// a2dun_Aqd_Chest_Rare_FacePuzzleSmall (SNOActor.a2dun_Aqd_Chest_Rare_FacePuzzleSmall) Distance: 11.21381
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, SNOWorld.a2dun_Aqd_Oasis_RandomFacePuzzle_Small, SNOActor.a2dun_Aqd_Chest_Rare_FacePuzzleSmall, 0, 5),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, 30, 0, 0, 30),
                }
            });

            // A1 - Bounty : The Precious Ores (SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres,
                Act = Act.A1,
                WorldId = SNOWorld.A1_Cave_Fields_MineCaveA_Level02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.trOUT_Town, -431250552),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.trOUT_Town, SNOWorld.A1_Cave_Fields_MineCaveA_Level01, -431250552, SNOActor.g_Portal_Oval_Orange),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level01, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, -431250551, SNOActor.g_Portal_Oval_Orange, true),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, SNOActor.A1_UniqueVendor_Miner, -431250552, 5),


                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, SNOActor.a1dun_caves_Rocks_GoldOre),
                    // a1dun_caves_Rocks_GoldOre (SNOActor.a1dun_caves_Rocks_GoldOre) Distance: 19.51709
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, SNOActor.a1dun_caves_Rocks_GoldOre, 0, 5),
                    // a1duncave_props_crystal_cluster_A (SNOActor.a1duncave_props_crystal_cluster_A) Distance: 20.88908

                    // a1duncave_props_crystal_cluster_A (SNOActor.a1duncave_props_crystal_cluster_A) Distance: 20.88908
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, SNOActor.a1duncave_props_crystal_cluster_A),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, SNOActor.a1duncave_props_crystal_cluster_A, 0, 5),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, 20, SNOActor.a1duncave_props_crystal_cluster_A, 0, 20),
//					new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, 10000),
					
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, SNOActor.A1_UniqueVendor_Miner),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, SNOWorld.A1_Cave_Fields_MineCaveA_Level02, SNOActor.A1_UniqueVendor_Miner, 0, 5),
                }
            });

            // A1 - Scavenged Scabbard (SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer)
            // Fixed pathing to entrance but it doesnt attack the boss at the end. Trinity issue.
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer,
                Act = Act.A1,
                WorldId = SNOWorld.Highlands_RandomDRLG_WestTower_Level02,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.trOUT_Town, SNOWorld.Highlands_RandomDRLG_WestTower_Level01, 497382903, SNOActor.g_Portal_Rectangle_Orange),
						
                        // Move a little to make sure it loads the adjecent scene or it will get stuck.
                        new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A1_Highlands_RandomDRLG_WestTower_Level01 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A1_Highlands_RandomDRLG_WestTower_Level01 || BountyHelpers.ScanForMarker(497382904, 50) != null)),

                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.Highlands_RandomDRLG_WestTower_Level01, SNOWorld.Highlands_RandomDRLG_WestTower_Level02, 497382904, SNOActor.g_Portal_ArchTall_Orange, true),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.Highlands_RandomDRLG_WestTower_Level01, 3000),
                        new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.Highlands_RandomDRLG_WestTower_Level02, SNOActor.A1_UniqueVendor_Armorer, 0, 3),

                        new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.Highlands_RandomDRLG_WestTower_Level02, SNOActor.a1dun_highlands_JeweledScabbard),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.Highlands_RandomDRLG_WestTower_Level02, SNOActor.a1dun_highlands_JeweledScabbard, 0, 5),
                        new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, 10, SNOActor.a1dun_highlands_JeweledScabbard, 0, 20, false),

						// A1_UniqueVendor_Armorer (SNOActor.A1_UniqueVendor_Armorer) Distance: 5.29833
						//new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.Highlands_RandomDRLG_WestTower_Level02, "trDun_Cath_EW_Hall_02", new Vector3(23.64563f, 119.1191f, 0.1000001f)),
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, SNOWorld.Highlands_RandomDRLG_WestTower_Level02, SNOActor.A1_UniqueVendor_Armorer),						
						new InteractionCoroutine(SNOActor.A1_UniqueVendor_Armorer, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                    }
            });

            // A1 - Bounty: Apothecary's Brother (SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue,
                Act = Act.A1,
                WorldId = SNOWorld.A1_Cave_Highlands_VendorRescue, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, SNOWorld.trOUT_Town, 853662530, true),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, SNOWorld.trOUT_Town, SNOWorld.A1_Cave_Highlands_VendorRescue, 853662530, SNOActor.g_Portal_Rectangle_Orange),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, SNOWorld.A1_Cave_Highlands_VendorRescue, SNOActor.Event_VendorRescue_Vendor, -1472187117, 3),
                    //ActorId: SNOActor.Event_VendorRescue_Brother, Type: Monster, Name: Event_VendorRescue_Brother-2746, Distance2d: 2.170177, CollisionRadius: 0, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, SNOWorld.A1_Cave_Highlands_VendorRescue, "trDun_Cave_NSW_01", new Vector3(165.4375f, 136.5941f, 0.1f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, 20, SNOActor.Event_VendorRescue_Brother, 0, 30),
                    // Event_VendorRescue_Vendor (SNOActor.Event_VendorRescue_Vendor) Distance: 11.94286
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, SNOWorld.A1_Cave_Highlands_VendorRescue, SNOActor.Event_VendorRescue_Vendor),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, SNOWorld.A1_Cave_Highlands_VendorRescue, SNOActor.Event_VendorRescue_Vendor, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, SNOWorld.A1_Cave_Highlands_VendorRescue, 25000),
                }
            });

            //A3 - The Lost Patrol(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A3_Bridge_01 },
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
					//new MoveToScenePositionCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_B_02", new Vector3(227.8457f, 112.291f, -24.9f)),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2461, 649, 0)), 
					new WaitCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, 5000),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2270, 604, 0)), 
					new WaitCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, 5000),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1982, 607, -24)), 
					new WaitCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, 5000),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1811, 612, 0)), 
					new WaitCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, 5000),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1435, 623, -24)),
					new WaitCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, 5000),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1047, 613, -24)),
					new WaitCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, 5000),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(950, 617, 0)),
					new WaitCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, SNOWorld.A3_Battlefields_02, 5000),
					new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(664, 608, 0)),

                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol,SNOActor.px_Bridge_Camp_LostPatrol),
					//new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2),
                }
            });
            
            // A2 - Bounty: Prisoners of Kamyr (SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 24,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, 2912417, true),
					
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI"),
					
                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner) Distance: 11.12986
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner),
                    //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, 2912417, 5),
					new InteractionCoroutine(SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, 3000),


                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(114.7661f, 82.81201f, 90.32503f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner) Distance: 5.553013
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner),
                    //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, 2912417, 5),
					new InteractionCoroutine(SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, 3000),

                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(95.05688f, 100.9766f, 90.32501f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner) Distance: 11.12986
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner),
                    //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, 2912417, 5),
					new InteractionCoroutine(SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, 3000),

                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(164.7249f, 159.7368f, 81.5364f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner) Distance: 5.59462
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner),
                    //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, 0, 5),
					new InteractionCoroutine(SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, 3000),

                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(145.3794f, 179.5557f, 81.5364f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner) Distance: 8.649173
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner),
                    //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, 0, 5),
					new InteractionCoroutine(SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, 3000),

                    // FallenChampion_B_PrisonersEvent_Unique (SNOActor.FallenChampion_B_PrisonersEvent_Unique) Distance: 8.834968
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, SNOWorld.caOUT_Town, SNOActor.FallenChampion_B_PrisonersEvent_Unique),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, 30, 0, 0, 45),
                }
            });

            // A2 - Bounty: The Shrine of Rakanishu (SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 24,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, 2912417),
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI"),
					
//                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI"),

                    ////ActorId: SNOActor.caOut_Oasis_RakinishuStone_B_FX, Type: Gizmo, Name: caOut_Oasis_RakinishuStone_B_FX-4071, Distance2d: 18.3527, CollisionRadius: 2.066596, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(106.3967f, 125.3721f, 120.1f)),
                    new AttackCoroutine(SNOActor.caOut_Oasis_RakinishuStone_B_FX),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(90.22925f, 141.2568f, 120.1f)),
                    new AttackCoroutine(SNOActor.caOut_Oasis_RakinishuStone_B_FX),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(79.20288f, 98.57617f, 120.1f)),
                    new AttackCoroutine(SNOActor.caOut_Oasis_RakinishuStone_B_FX),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(116.3853f, 77.13379f, 120.1f)),
                    new AttackCoroutine(SNOActor.caOut_Oasis_RakinishuStone_B_FX),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(152.0964f, 115.9365f, 120.1f)),
                    new AttackCoroutine(SNOActor.caOut_Oasis_RakinishuStone_B_FX),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(134.239f, 159.396f, 120.1f)),
                    new AttackCoroutine(SNOActor.caOut_Oasis_RakinishuStone_B_FX),

                    //ActorId: SNOActor.caOut_Oasis_Rakanishu_CenterStone_A, Type: Gizmo, Name: caOut_Oasis_Rakanishu_CenterStone_A-4001, Distance2d: 18.79537, CollisionRadius: 17.01699, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, SNOWorld.caOUT_Town, "caOut_Oasis_Sub240_POI", new Vector3(106.3967f, 125.3721f, 120.1f)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu,SNOWorld.caOUT_Town,SNOActor.caOut_Oasis_Rakanishu_CenterStone_A, -1)
                }
            });

            // A5 - Bounty: The Great Weapon (SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, 2912417),
                    // X1_Angel_Common_Event_GreatWeapon (SNOActor.X1_Angel_Common_Event_GreatWeapon) Distance: 31.32524
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.X1_Angel_Common_Event_GreatWeapon),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.X1_Angel_Common_Event_GreatWeapon, 0, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(123.2855f, 157.1476f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, SNOWorld.X1_Pand_Ext_2_Battlefields,  5000),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, 200, SNOActor.X1_Angel_Common_Event_GreatWeapon, 0, 45),

                }
            });
            
            // A2 - Bounty: The Guardian Spirits (SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                Coroutines = new List<ISubroutine>
                {
                    //World: caOUT_Town, Id: SNOWorld.caOUT_Town, AnnId: 1999568897, IsGenerated: False
                    //Scene: caOut_Sub240x240_Tower_Ruin, SnoId: 31623,
                    //LevelArea: A2_caOUT_StingingWinds, Id: SNOLevelArea.A2_caOUT_StingingWinds

                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, 2912417, true),
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, "caOut_Sub240x240_Tower_Ruin"),
					
					// A2C2DyingGhostGuy (51293) Distance: 9.568595
					//new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, 51293),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, "caOut_Sub240x240_Tower_Ruin", new Vector3(136.7617f, 136.5654f, 175.9637f)),
                    new InteractionCoroutine(SNOActor.A2C2DyingGhostGuy, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, "caOut_Sub240x240_Tower_Ruin", new Vector3(161.0929f, 112.1465f, 175.5483f)),
                    //// GhostTotem (SNOActor.GhostTotem) Distance: 4.005692
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, SNOActor.GhostTotem, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, 5000),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, "caOut_Sub240x240_Tower_Ruin", new Vector3(136.7617f, 136.5654f, 175.9637f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, "caOut_Sub240x240_Tower_Ruin", new Vector3(101.0205f, 109.6327f, 175.4678f)),
                    //// GhostTotem (SNOActor.GhostTotem) Distance: 6.767324
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, SNOActor.GhostTotem, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, "caOut_Sub240x240_Tower_Ruin", new Vector3(136.7617f, 136.5654f, 175.9637f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, SNOWorld.caOUT_Town, 10000),
 //                new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, 20, 0, 0, 25),
                }
            });

            // A2 - Bounty: Restless Sands (SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands) 8	0
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 21,
                Coroutines = new List<ISubroutine>
                {

                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, 2912417, true),
                    //new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, 1000),
					
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Ruin"),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Ruin", new Vector3(119.9591f, 130.6529f, 158.806f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.oldNecromancer),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.oldNecromancer, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, 5000),
					
                    // caOut_Totem_A (SNOActor.caOut_Totem_A) Distance: 10.77662
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Ruin", new Vector3(132.3932f, 97.01617f, 158.806f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.caOut_Totem_A),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.caOut_Totem_A, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Ruin", new Vector3(57.79712f, 117.5626f, 172.8638f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.caOut_Totem_A),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.caOut_Totem_A, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Ruin", new Vector3(119.9591f, 130.6529f, 158.806f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.oldNecromancer),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.oldNecromancer, 0, 5),
                    // oldNecromancer (SNOActor.oldNecromancer) Distance: 1.06224
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Ruin", new Vector3(119.9591f, 130.6529f, 158.806f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.oldNecromancer),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, SNOWorld.caOUT_Town, SNOActor.oldNecromancer, 0, 5),
//                 new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands)
                }
            });

            // A5 - Bounty: Lost Host (SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.x1_fortress_level_02_islands,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 2912417),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, "x1_fortress_SW_01_B", new Vector3(91.39471f, 131.2191f, -9.900017f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, "x1_fortress_SW_01_B", new Vector3(102.9944f, 130.5471f, -9.900017f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, "x1_fortress_SW_01_B", new Vector3(90.04523f, 133.9944f, -9.900017f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, "x1_fortress_SW_01_B", new Vector3(92.48651f, 131.3251f, -9.900016f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, "x1_fortress_SW_01_B", new Vector3(95.55377f, 115.0262f, -9.900017f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, "x1_fortress_SW_01_B", new Vector3(105.3384f, 146.7679f, -9.900017f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 5000),
					
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, SNOActor.Angel_Trooper_A),
					new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, SNOActor.Angel_Trooper_A, 0, 5),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, SNOWorld.x1_fortress_level_02, 20000),
					
//                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LostHost, 60, 2912417, 0, 30, false),
                }
            });

            // A2 - Kill Taros the Wild (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Taros)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Taros,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2700, 1586, 184)),
                        new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2535, 1497, 184)),
                        new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2483, 1576, 186)),
                        new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2344, 1518, 207)),

                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Taros, SNOWorld.caOUT_Town, SNOActor.LacuniFemale_A_Unique_02, -1258389667),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Taros)
                    }
            });

            // A5 - Bounty : The Hatchery (SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 54,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, 2912417),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SE_06_scorpion_pit", new Vector3(173.532f, 151.7463f, -3.704464f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, 10000),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SE_06_scorpion_pit", new Vector3(179.0787f, 190.6577f, -16.08633f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, SNOActor.X1_Catacombs_Scarab_Spawn),
					new AttackCoroutine(SNOActor.X1_Catacombs_Scarab_Spawn),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, 5000),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SE_06_scorpion_pit", new Vector3(141.2239f, 186.5033f, -14.51319f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, SNOActor.X1_Catacombs_Scarab_Spawn),
					new AttackCoroutine(SNOActor.X1_Catacombs_Scarab_Spawn),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, 5000),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, "x1_Catacombs_SE_06_scorpion_pit", new Vector3(144.2615f, 123.6354f, -13.9f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, SNOActor.X1_Catacombs_Scarab_Spawn),
					new AttackCoroutine(SNOActor.X1_Catacombs_Scarab_Spawn),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, SNOWorld.x1_Catacombs_Level01, 5000),
					
//                 new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Event_ScarabHatchery, 45, 0, 0, 45),
                }
            });

            // A1 - Bounty: Kill Cadhul the Deathcaller (SNOQuest.X1_Bounty_A1_Highlands_Kill_Cadhul)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Kill_Cadhul,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A1_trOUT_Highlands,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Kill_Cadhul, SNOWorld.trOUT_Town, 1928482775, true),		
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2107, 4325, 9)),
					
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2382, 4438, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2087, 4448, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1879, 4437, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2026, 4195, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2010, 4024, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2238, 3962, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2514, 3959, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2463, 4186, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2377, 4399, -1)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2240, 4271, 0)), 
					new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2182, 4090, 0)),

                    new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_Cadhul)
                }
            });

            // A5 - Bounty: The Forest Prayer Disturbed (SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head,
				Act = Act.A5,
				WorldId = SNOWorld.x1_p4_Forest_Coast_01,
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.x1_P2_Forest_Coast_Level_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 2912417),
					new MoveToSceneCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow"),
					
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(73.39761f, 65.86435f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(83.71802f, 65.09787f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(69.86966f, 65.79407f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(78.4863f, 63.40695f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(73.39761f, 65.86435f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(83.71802f, 65.09787f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(69.86966f, 65.79407f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(78.4863f, 63.40695f, 0.1f)),
					new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					
					//new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Stag_Head, 60, 0, 0, 25),
				}
			});

            //A1 - Wortham Survivors - Interact with operated gizmo repeatly
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A1_LeoricsDungeon_Camp_WorthamMilitia,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level02,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A1_LeoricsDungeon_Camp_WorthamMilitia, SNOActor.px_Leorics_Camp_WorthamMilitia_Ex)
                }
            });

            //A1 - 다시 태어난 삼위일체단 - Interact with operated gizmo repeatly
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A1_Highlands_Camp_ResurgentCult,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(SNOQuest.px_Bounty_A1_Highlands_Camp_ResurgentCult, SNOActor.px_Highlands_Camp_ResurgentCult_Totem)
                }
            });
            
            // A5 - Bounty: The Lost Patrol (SNOQuest.X1_Bounty_A5_Westmarch_Event_TustinesBrewery)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_TustinesBrewery,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TustinesBrewery, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_02, -178461554, 0),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_02, new Vector3(305, 361, 10)),
                    // x1_SurvivorCaptain_Rescue_Guards_02 (SNOActor.x1_SurvivorCaptain_Rescue_Guards_02) Distance: 6.273188
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TustinesBrewery, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_02, SNOActor.x1_SurvivorCaptain_Rescue_Guards_02, 0, 5),

                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_02, new Vector3(310, 320, 10)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TustinesBrewery, 10, 0, 0, 45),


                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_02, new Vector3(307, 358, 10)),
                    // x1_SurvivorCaptain_Rescue_Guards_02 (SNOActor.x1_SurvivorCaptain_Rescue_Guards_02) Distance: 6.273188
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TustinesBrewery, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_02, SNOActor.x1_SurvivorCaptain_Rescue_Guards_02, 0, 5),
                    new WaitCoroutine(5000),
                }
            });

            // A5 - Bounty: Clear the Perilous Cave (SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2) 11	6
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2,
                Act = Act.A5,
                WorldId = SNOWorld.x1_BogCave_Random01_B, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2, SNOWorld.x1_BogCave_Random01, 1022651488, true),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2, SNOWorld.x1_Bog_01, SNOWorld.x1_BogCave_Random01, 1022651488, new SNOActor[]{ SNOActor.g_Portal_Circle_Orange, SNOActor.g_portal_Ladder_Very_Short_Orange_VeryBright, SNOActor.g_portal_Ladder_Very_Short_Orange_Bright }),
					
					// g_portal_Ladder_Very_Short_Orange_VeryBright (SNOActor.g_portal_Ladder_Very_Short_Orange_VeryBright) Distance: 9.465642
					//new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2, SNOWorld.x1_Bog_01, SNOActor.g_portal_Ladder_Very_Short_Orange_VeryBright),
					 
					// g_portal_Ladder_Very_Short_Orange_Bright (SNOActor.g_portal_Ladder_Very_Short_Orange_Bright) Distance: 6.553743
					//new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2, SNOWorld.x1_Bog_01, SNOActor.g_portal_Ladder_Very_Short_Orange_Bright, -1711263287, 5),
					
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2, SNOWorld.x1_BogCave_Random01, 1270943969),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2, SNOWorld.x1_BogCave_Random01, SNOWorld.x1_BogCave_Random01_B, 1270943969, SNOActor.g_Portal_ArchTall_Blue),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2),
                }
            });

            // A5 - Bounty: Clear the Plague Tunnels (SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2) 30	9
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Abattoir_Random01_B, // Enter the final worldId here
                QuestType = BountyQuestType.ClearZone,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2, SNOWorld.X1_Abattoir_Random01,  -660641889),
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.X1_WESTM_ZONE_01 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.X1_WESTM_ZONE_01 || BountyHelpers.ScanForMarker(-660641889, 50) != null)),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2, SNOWorld.X1_WESTM_ZONE_01, 0, -660641889, new SNOActor[]{ SNOActor.g_Portal_Rectangle_Blue_Westm_SideDungeon, SNOActor.g_Portal_ArchTall_Blue }),

                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2, SNOWorld.X1_Abattoir_Random01, 2115491808),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2, SNOWorld.X1_Abattoir_Random01, SNOWorld.X1_Abattoir_Random01_B, 2115491808, new SNOActor[]{ SNOActor.g_Portal_Rectangle_Orange, SNOActor.g_Portal_ArchTall_Orange }),
//					new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2, SNOWorld.X1_Abattoir_Random01, 0, 2115491808, SNOActor.g_Portal_Rectangle_Orange),

                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Clear_PlagueTunnels2),
                }
            });

            // A3 - Bounty: The Three Guardians (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A3_dun_ruins_frost_city_A_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, 2912417),
                    new MoveToPositionCoroutine(SNOWorld.a3dun_ruins_frost_city_A_01, new Vector3(357, 947, 0)),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, "Guardians", new Vector3(100.5087f, 88.13977f, -11.82213f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_SE_02_Guardians", new Vector3(107.6098f, 96.29413f, -12.4f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_SE_02_Guardians", new Vector3(107.2299f, 119.635f, -12.4f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_SE_02_Guardians", new Vector3(134.8604f, 121.1194f, -12.4f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_SE_02_Guardians", new Vector3(99.5239f, 90.3031f, -12.4f)),
                    // SNOActor.p4_Ruins_Frost_Goatman_Drum_A (SNOActor.p4_Ruins_Frost_Goatman_Drum_A) Distance: 5.283126
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_Guardians, 90, SNOActor.p4_Ruins_Frost_Goatman_Drum_A, 2912417, 20),
                }
            });
            
            // A1 - Bounty: A Farm Besieged (SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 8,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged, SNOWorld.trOUT_Town, -1252188069),
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged, SNOWorld.trOUT_Town, "POI"),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged, SNOWorld.trOUT_Town, "POI", new Vector3(111.8108f, 96.45514f, 0.1000002f)),
                    // Beast_Corpse_A_02 (SNOActor.Beast_Corpse_A_02) Distance: 8.983345
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged, SNOWorld.trOUT_Town, SNOActor.Beast_Corpse_A_02, -1252188069, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged, 60, SNOActor.Beast_Corpse_A_02, 0, 45),
                }
            });
            
            // A1 - Bounty: Kill Rockmaw (SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town, // Enter the final worldId here
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_Fields_RandomDRLG_ScavengerDenA_Level02 },
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 8,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw, SNOWorld.trOUT_Town, 925091454, true),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw, SNOWorld.trOUT_Town, SNOWorld.A1_Cave_Fields_ScavengerDen_Level01, 925091454, SNOActor.g_Portal_Circle_Orange),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw, SNOWorld.A1_Cave_Fields_ScavengerDen_Level01, SNOWorld.A1_Cave_Fields_ScavengerDen_Level02, 925091455, SNOActor.g_Portal_Oval_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw, SNOWorld.A1_Cave_Fields_ScavengerDen_Level02, 1582533030),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw, 20, 0, 0, 45),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Rockmaw),
                }
            });
            
            // A1 - Bounty: Kill John Gorham Coffin (SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin,
                Act = Act.A1,
                WorldId = SNOWorld.trdun_Crypt_SkeletonKingCrown_00, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 7,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin, SNOWorld.trOUT_Town, -1861222194, true),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin, SNOWorld.trOUT_Town, SNOWorld.trdun_Crypt_SkeletonKingCrown_00, -1861222194, SNOActor.g_Portal_ArchTall_Blue),
					//new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin, SNOWorld.trDun_Crypt_FalsePassage_02, 0, -1861222194, SNOActor.g_Portal_ArchTall_Blue),
                    //new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin, SNOWorld.trdun_Crypt_SkeletonKingCrown_00, 1321851756),
                    new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin, SNOWorld.trdun_Crypt_SkeletonKingCrown_00, 0, 1321851756),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Kill_JohnGorhamCoffin),
                }
            });

            // A5 - Bounty: Leap of Faith (SNOQuest.X1_Bounty_A5_PandFortress_Event_LeapOfFaith)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_LeapOfFaith,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.x1_fortress_level_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Event_LeapOfFaith,SNOWorld.x1_fortress_level_01,1),
                    new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Event_LeapOfFaith, SNOWorld.x1_fortress_level_01, 2912417),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_LeapOfFaith, 60, 0, 2912417, 30),
                }
            });

            // A5 - Bounty: Walk in the Park (SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 2912417),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01,SNOActor.x1_Westm_Chest_Rare,0,3),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(166.5977f, 53.30713f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(182.8521f, 67.52661f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(165.9268f, 76.87494f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(143.7947f, 61.68109f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(163.4475f, 64.09247f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(181.2703f, 65.80365f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(165.8199f, 81.53088f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(149.4128f, 64.20264f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_08", new Vector3(164.1267f, 53.60931f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, SNOWorld.X1_WESTM_ZONE_01, 5000),
					
					
//                  new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark,40,0,0)
                }
            });

            // A5 - Bounty : Resurrection (SNOQuest.X1_Bounty_A5_PandExt_Event_Resurrection)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_Resurrection,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_Cellar_C, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    //new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_Resurrection, SNOWorld.X1_Pand_Ext_2_Battlefields, 0, -1551729969, SNOActor.g_Portal_RectangleTall_Blue),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_Resurrection, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_Ext_Cellar_C, -1551729969, 0),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_Resurrection, SNOWorld.X1_Pand_Ext_Cellar_C, 2912417),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_Resurrection),
                }
            });
            
            // A2 - Bounty: Kill Ashek (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Ashek)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Ashek,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 19,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A2_caOUT_StingingWinds_Canyon, SNOLevelArea.A2_caOUT_StingingWinds_PostBridge },
                Coroutines = new List<ISubroutine>
                {
					new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2657, 1540, 187)), 
					new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2544, 1595, 186)), 
					new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2387, 1543, 207)),
						
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Ashek, SNOWorld.caOUT_Town, 689915896),
                    new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Ashek, SNOWorld.caOUT_Town, 0, 689915896),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Ashek),
                }
            });

            // A5 - Bounty : The Golden Chamber (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 55,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 2912417),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 1227995800),
					
//                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(59.00961f, 90.45087f, 0.4674271f)),
//                 new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, SNOActor.x1_Catacombs_chest_rare_GardenEvent, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(96.20807f, 92.42499f, 0.2714336f)),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(132.951f, 68.6438f, 0.5334398f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(163.7514f, 93.70868f, 0.2504959f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(136.5582f, 127.6992f, 0.5334393f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(100.6373f, 92.96954f, 0.2542905f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(136.5582f, 127.6992f, 0.5334393f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, "x1_Catacombs_SEW_06_garden", new Vector3(100.6373f, 92.96954f, 0.2542905f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, 10000),
//                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, 150, 0, 0, 80),

                    // x1_Catacombs_chest_rare_GardenEvent (SNOActor.x1_Catacombs_chest_rare_GardenEvent) Distance: 5.320904
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Event_TheGoldenChamber, SNOWorld.x1_Catacombs_Level02, SNOActor.x1_Catacombs_chest_rare_GardenEvent, 0, 5),
                }
            });

            // A5 - Bounty: Clear the Repository of Bones (SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_RepositoryOfBones2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_RepositoryOfBones2,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Abattoir_Random02_B, // Enter the final worldId here
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.X1_Abattoir_Random02_B },
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 52,
                Coroutines = new List<ISubroutine>
                {
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.X1_WESTM_ZONE_03 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.X1_WESTM_ZONE_03 || BountyHelpers.ScanForMarker(-660641888, 50) != null)),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_RepositoryOfBones2, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_Abattoir_Random02, -660641888, SNOActor.g_Portal_Rectangle_Blue_Westmarch),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_RepositoryOfBones2, SNOWorld.X1_Abattoir_Random02, 2115492897),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_RepositoryOfBones2, SNOWorld.X1_Abattoir_Random02, SNOWorld.X1_Abattoir_Random02_B, 2115492897, new SNOActor[]{SNOActor.g_Portal_Rectangle_Orange, SNOActor.g_Portal_ArchTall_Orange}),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_RepositoryOfBones2),
                }
            });

            // A5 - Bounty : The Miser's Will (SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser,
                Act = Act.A5,
                WorldId = SNOWorld.x1_WestM_Int_Gen_C_Miser, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.x1_WestM_Int_Gen_C_Miser, 2043324508, SNOActor.g_portal_RandomWestm),
                    new MoveToPositionCoroutine(SNOWorld.x1_WestM_Int_Gen_C_Miser, new Vector3(142, 137, 0)),
                    new MoveToPositionCoroutine(SNOWorld.x1_WestM_Int_Gen_C_Miser, new Vector3(165, 136, 0)),
                    new MoveToPositionCoroutine(SNOWorld.x1_WestM_Int_Gen_C_Miser, new Vector3(147, 159, 0)),
                    new MoveToPositionCoroutine(SNOWorld.x1_WestM_Int_Gen_C_Miser, new Vector3(142, 137, 0)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser, SNOWorld.x1_WestM_Int_Gen_C_Miser, SNOActor.x1_Westm_Chest_Rare_MiserEvent, 0, 5),
                    new MoveToPositionCoroutine(SNOWorld.x1_WestM_Int_Gen_C_Miser, new Vector3(165, 136, 0)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser, SNOWorld.x1_WestM_Int_Gen_C_Miser, SNOActor.x1_Westm_Chest_Rare_MiserEvent,0,5),
                    new MoveToPositionCoroutine(SNOWorld.x1_WestM_Int_Gen_C_Miser, new Vector3(147, 159, 0)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser, SNOWorld.x1_WestM_Int_Gen_C_Miser, SNOActor.x1_Westm_Chest_Rare_MiserEvent, 0, 5),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser)
                }
            });

            // A1 - Bounty : Kill the Cultist Grand Inquisitor (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_CultistGrandInquisitor)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_CultistGrandInquisitor,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level01, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 15,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_CultistGrandInquisitor, SNOWorld.trDun_Leoric_Level01, 1468812546),
                    new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_CultistGrandInquisitor, SNOWorld.trDun_Leoric_Level01, 0, 1468812546),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_CultistGrandInquisitor)
                }
            });

            // A5 - Bounty : Judgment (SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 56,
                Coroutines = new List<ISubroutine>
                {
                    new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity,SNOWorld.x1_fortress_level_01,1),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, 2912417),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, "x1_fortress_S_03_Judgment", new Vector3(73.57855f, 151.708f, -9.899999f)),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, "x1_fortress_S_03_Judgment", new Vector3(99.29626f, 150.8474f, -9.899999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, "x1_fortress_S_03_Judgment", new Vector3(97.68469f, 132.0456f, -9.899999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, "x1_fortress_S_03_Judgment", new Vector3(113.8369f, 150.1263f, -9.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, "x1_fortress_S_03_Judgment", new Vector3(98.00818f, 170.0193f, -9.899999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, "x1_fortress_S_03_Judgment", new Vector3(99.28516f, 149.8546f, -9.899999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, "x1_fortress_S_03_Judgment", new Vector3(80.02435f, 153.6859f, -9.9f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, SNOWorld.x1_fortress_level_01, 10000),
					
//                  new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_CourtOfInsanity, 60, 0, 0, 30),
                }
            });

            // A4 - Bounty : Kill Sledge (SNOQuest.X1_Bounty_A4_HellRift_Kill_Sledge)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_HellRift_Kill_Sledge,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Hell_Portal_02, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 47,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_HellRift_Kill_Sledge, SNOWorld.a4dun_Hell_Portal_01, SNOWorld.a4dun_Hell_Portal_02, 984446737, SNOActor.a4_Heaven_Gardens_HellPortal),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A4_HellRift_Kill_Sledge, SNOWorld.a4dun_Hell_Portal_02, 614820905),
                    new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_HellRift_Kill_Sledge, SNOWorld.a4dun_Hell_Portal_02, 0, 614820905),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A4_HellRift_Kill_Sledge),

                }
            });

            // A3 - Bounty : Kill Tala (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalA)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalA,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A3_dun_ruins_frost_city_A_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalA, SNOWorld.a3dun_ruins_frost_city_A_01, 1714479875),
                    new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalA),

                }
            });
            
            // A3 - Bounty : Kill Aletur (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalD)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalD,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalD, SNOWorld.a3dun_ruins_frost_city_A_01, 720789926),
                    new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalD),
                }
            });

            // A1 - Bounty : Kill Drury Brown (SNOQuest.X1_Bounty_A1_Cemetery_Kill_Drury)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cemetery_Kill_Drury,
                Act = Act.A1,
                WorldId = SNOWorld.trdun_Crypt_SkeletonKingCrown_00, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 7,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Kill_Drury, SNOWorld.trOUT_Town, SNOWorld.trdun_Crypt_SkeletonKingCrown_00, -1861222194, SNOActor.g_Portal_ArchTall_Blue),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Kill_Drury, SNOWorld.trdun_Crypt_SkeletonKingCrown_00, 1321851755),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Kill_Drury),
                }
            });

            // A5 - Bounty : The Lost Legion (SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_Cellar_E, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, -2009241926),
                    //new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_Ext_Cellar_E, -1551729967, SNOActor.g_Portal_ArchTall_Blue),
                    // portal id changed?
//					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.g_Portal_ArchTall_Blue, -1551729967, 5),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields,  SNOWorld.X1_Pand_Ext_Cellar_E, -1551729967, new SNOActor[]{SNOActor.g_Portal_RectangleTall_Blue, SNOActor.g_Portal_ArchTall_Blue}),
                    // g_Portal_ArchTall_Blue (SNOActor.g_Portal_ArchTall_Blue) Distance: 4.875014

                    //new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.g_Portal_ArchTall_Blue, -1551729967, 5),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_Cellar_E, "x1_Pand_Ext_240_Cellar_06", new Vector3(221.9186f, 111.5466f, 10.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_Cellar_E, "x1_Pand_Ext_240_Cellar_06", new Vector3(129.1784f, 116.145f, 0.1000006f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_Cellar_E, "x1_Pand_Ext_240_Cellar_06", new Vector3(76.8408f, 86.60729f, 0.1000005f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_Cellar_E, "x1_Pand_Ext_240_Cellar_06", new Vector3(82.55774f, 155.1393f, 0.1000019f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_2_Battlefields, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, SNOWorld.X1_Pand_Ext_Cellar_E, "x1_Pand_Ext_240_Cellar_06", new Vector3(41.33323f, 182.5292f, 0.1000019f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, 30, 0, 0, 45),
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion,SNOWorld.X1_Pand_Ext_Cellar_E,SNOActor.x1_Global_Chest),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion,SNOWorld.X1_Pand_Ext_Cellar_E,SNOActor.x1_Global_Chest,0),
                }
            });

            // A1 - Bounty : Crumbling Tower (344482)
            Bounties.Add(new BountyData
            {
                QuestId = (SNOQuest)344482,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Cath_Tower_Of_Power, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 13,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine((SNOQuest)344482, SNOWorld.trOUT_Town, SNOWorld.a1trDun_Cath_Tower_Of_Power, -493718752, SNOActor.g_Portal_ArchTall_Orange),
                    new ClearLevelAreaCoroutine((SNOQuest)344482),
                }
            });

            // A5 - Bounty : 바보들의 군주 (SNOQuest.X1_Bounty_A5_Bog_Event_LordOfFools)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Event_LordOfFools,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_LordOfFools, SNOWorld.x1_Bog_01, 2912417),
                    new MoveToSceneCoroutine(0, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_LordOfFools"),
                    new MoveToScenePositionCoroutine(0, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_LordOfFools", new Vector3(113.899f, 80.1803f, 2.100157f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_LordOfFools, 60, 0, 0, 120),
                }
            });
            
            // A5 - Bounty : The Last Stand (SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, -178461555, SNOActor.g_portal_RandomWestm),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(448, 434, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(435, 340, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(395, 358, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(399, 302, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(352, 306, 10)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(355, 355, 20)),
                    // x1_SurvivorCaptain_Rescue_Guards (SNOActor.x1_SurvivorCaptain_Rescue_Guards) Distance: 3.254871
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, SNOActor.x1_SurvivorCaptain_Rescue_Guards, 0, 5),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(309, 347, 20)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 2000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(300, 294, 20)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 2000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(400, 430, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 2000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(448, 434, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 2000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(435, 340, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 2000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(395, 358, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 2000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(399, 302, 0)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, 2000),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(352, 306, 10)),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(355, 355, 20)),
                    // x1_SurvivorCaptain_Rescue_Guards (SNOActor.x1_SurvivorCaptain_Rescue_Guards) Distance: 3.254871
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, SNOActor.x1_SurvivorCaptain_Rescue_Guards, 0, 5),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(309, 347, 20)),
                    new MoveToPositionCoroutine(SNOWorld.X1_WESTM_INT_RESCUE_GUARDS_01, new Vector3(300, 294, 20)),

                }
            });

            // A5 - Bounty : Kill Magrethar (SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar)
            Bounties.Add(new BountyData
            {
                Act = Act.A5,
                QuestId = SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar,
                WorldId = SNOWorld.X1_Pand_HexMaze_Magrethar, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar, SNOWorld.X1_Pand_Ext_2_Battlefields, -702665403),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar, SNOWorld.X1_Pand_Ext_2_Battlefields, 20000),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar (SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar) Distance: 19.76944
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar (SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar) Distance: 19.76944
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar, 0, 5),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar, SNOWorld.X1_Pand_HexMaze_Magrethar, 850055694),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar),

                }
            });

            // A5 - Bounty: The Bogan Haul (SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_BogPeople_Cellar_D, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
					//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits, SNOWorld.x1_Bog_01, -1947203375),
					new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits, SNOWorld.x1_Bog_01, 0, -1947203375, 0),
					// g_Portal_Circle_Orange_Bright (SNOActor.g_Portal_Circle_Orange_Bright) Distance: 13.78108
					//new InteractionCoroutine((int)SNOActor.g_Portal_Circle_Orange_Bright, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),

                    //new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits, SNOWorld.x1_Bog_01, SNOWorld.x1_Bog_BogPeople_Cellar_D, -1947203375, SNOActor.g_Portal_Circle_Orange_Bright),
                    new MoveToPositionCoroutine(SNOWorld.x1_Bog_BogPeople_Cellar_D, new Vector3(149, 152, 0)),
                    new MoveToPositionCoroutine(SNOWorld.x1_Bog_BogPeople_Cellar_D, new Vector3(132, 229, 0)),
                    new MoveToPositionCoroutine(SNOWorld.x1_Bog_BogPeople_Cellar_D, new Vector3(194, 238, 0)),
                    new MoveToPositionCoroutine(SNOWorld.x1_Bog_BogPeople_Cellar_D, new Vector3(236, 190, 0)),
                    new MoveToPositionCoroutine(SNOWorld.x1_Bog_BogPeople_Cellar_D, new Vector3(151, 151, 0)),
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits, SNOWorld.x1_Bog_BogPeople_Cellar_D, SNOActor.x1_Global_Chest),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits, SNOWorld.x1_Bog_BogPeople_Cellar_D, SNOActor.x1_Global_Chest, -0, 5),
                }
            });

            // A5 - Bounty: The Lord of the Hill (SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill, SNOWorld.x1_Bog_01, 2912417),
                    //new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill,SNOWorld.x1_Bog_01,"KingOfTheHill"),
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_KingOfTheHill"),
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill, SNOWorld.x1_Bog_01, 2912417),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_KingOfTheHill", new Vector3(191.7875f, 76.95569f, 10.1f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_KingOfTheHill", new Vector3(156.2918f, 172.7191f, 20.1f)),
                    new MoveToScenePositionCoroutine(0, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_KingOfTheHill", new Vector3(119.5713f, 128.0659f, 30.1f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill,15,0,0,60)
                }
            });

            // A1 - Bounty: Kill the Warden (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Jail_Level01, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 16,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden, SNOWorld.trDun_Leoric_Level02, SNOWorld.trOut_Highlands_DunExteriorA, 1241437688, SNOActor.g_Portal_ArchTall_Orange),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden, SNOWorld.trOut_Highlands_DunExteriorA, SNOWorld.trDun_Jail_Level01, 1303804501, SNOActor.g_Portal_ArchTall_Orange),
//                 new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden, SNOWorld.trDun_Jail_Level01, 1917087943),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden, SNOWorld.trDun_Jail_Level01, "a1dun_Leor_Jail_NSEW_03", new Vector3(118.5699f, 121.7972f, 0.1000001f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden, 30, 0, 1917087943, 30),

                }
            });

            // A1 - Bounty : Kill Garrach the Afflicted (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level03, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 17,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach, SNOWorld.trDun_Leoric_Level03, -1229363584),
                    // a1dun_Leor_Chest_Rare_Garrach (SNOActor.a1dun_Leor_Chest_Rare_Garrach) Distance: 78.48354
//                 new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach, SNOWorld.trDun_Leoric_Level03, SNOActor.a1dun_Leor_Chest_Rare_Garrach),
                    // a1dun_Leor_Chest_Rare_Garrach (SNOActor.a1dun_Leor_Chest_Rare_Garrach) Distance: 78.48354
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach, SNOWorld.trDun_Leoric_Level03, SNOActor.a1dun_Leor_Chest_Rare_Garrach, -1229363584, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach, SNOWorld.trDun_Leoric_Level03, 20000),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach)
                }
            });

            // A5 - The Reformed Cultist (SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.X1_Westm_ReformedCultist, -1342301630, SNOActor.g_portal_RandomWestm ),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(921, 161, 0)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(882, 166, 0)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(779, 163, -9)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(764, 118, -9)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(759, 91, -9)),

                        new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, 30, 0, 0, 150),

                        // x1_BogCellar_TriuneCultist (247595) Distance: 1.502872
                        //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_Westm_ReformedCultist, 247595, 0, 5),
                        //new WaitCoroutine(10000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(747, 40, 0)),
                        // g_Portal_Rectangle_Orange_IconDoor (SNOActor.g_Portal_Rectangle_Orange_IconDoor) Distance: 5.280013
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_Westm_ReformedCultist, SNOActor.g_Portal_Rectangle_Orange_IconDoor, 798857082, 5),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(458, 198, 0)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(461, 130, 10)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(372, 126, 15)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(368, 183, 10)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(303, 178, 10)),

                        new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, 30, 0, 0, 150),
						
                        // X1_Westm_Door_Hidden_Bookshelf (SNOActor.X1_Westm_Door_Hidden_Bookshelf) Distance: 6.397694
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_Westm_ReformedCultist, SNOActor.X1_Westm_Door_Hidden_Bookshelf, 0, 5),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_ReformedCultist, new Vector3(318, 56, 15)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    }
            });
            
            // A1 - : The Matriarch's Bones (SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Crypt_FalsePassage_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
//                  new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trOUT_Town, SNOWorld.trDun_Crypt_FalsePassage_01, -1965109038, SNOActor.g_Portal_ArchTall_Blue),
					new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trOUT_Town, SNOWorld.trDun_Crypt_FalsePassage_01, -1965109038, SNOActor.g_Portal_ArchTall_Blue),

                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, 2912417),

                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01,"dead"),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, 4000),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.CryingGhost_Female_01_A, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(84.57935f, 52.89191f, 0.1f)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.a1dun_Crypts_Bowl_of_Bones_01, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(50.32025f, 116.8566f, 0.09999999f)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.a1dun_Crypts_Bowl_of_Bones_02, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(85.17566f, 187.389f, 0.1f)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.a1dun_Crypts_Bowl_of_Bones_03, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),

                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones,SNOWorld.trDun_Crypt_FalsePassage_01,"dead"),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, 10, 0, 0, 100),
                    // a1dun_Crypts_Dual_Sarcophagus (SNOActor.a1dun_Crypts_Dual_Sarcophagus) Distance: 17.6851
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(209.656f, 121.3727f, 0.1f)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.a1dun_Crypts_Dual_Sarcophagus, 0, 5),
                }
            });

            // A2 - : 파주주 처치 (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, SNOWorld.caOUT_Town, -1483215209, true),
                    // caOut_Boneyards_Dervish_SubAlter (SNOActor.caOut_Boneyards_Dervish_SubAlter) Distance: 2.324677
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, SNOWorld.caOUT_Town, SNOActor.caOut_Boneyards_Dervish_SubAlter),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, SNOWorld.caOUT_Town, SNOActor.caOut_Boneyards_Dervish_SubAlter, -1483215209, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, 10, SNOActor.caOut_Boneyards_Dervish_SubAlter, -1483215209, 20, false),
					
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, SNOWorld.caOUT_Town, SNOActor.caOut_Boneyards_Dervish_SubAlter),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, SNOWorld.caOUT_Town, SNOActor.caOut_Boneyards_Dervish_SubAlter, -1483215209, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, 10, SNOActor.caOut_Boneyards_Dervish_SubAlter, -1483215209, 20, false),
					
                    new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, SNOWorld.caOUT_Town, SNOActor.DuneDervish_A_DervishTwister_Unique, -1483215209),
                    new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu)
                }
            });

            // A5 -: 엇나간 마법 (SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Int_Gen_A_01ZombieSorcerer, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.x1_westm_Int_Gen_A_01ZombieSorcerer, -1750959668, SNOActor.g_portal_RandomWestm),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire, SNOWorld.x1_westm_Int_Gen_A_01ZombieSorcerer, 2912417),
                    // x1_NPC_ZombieCellar_Male_A (SNOActor.x1_NPC_ZombieCellar_Male_A) Distance: 1.151123
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire, SNOWorld.x1_westm_Int_Gen_A_01ZombieSorcerer, SNOActor.x1_NPC_ZombieCellar_Male_A, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire, 15, SNOActor.x1_NPC_ZombieCellar_Male_A, 0, 45),
                    // x1_NPC_ZombieCellar_Male_A (SNOActor.x1_NPC_ZombieCellar_Male_A) Distance: 7.441172
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire, SNOWorld.x1_westm_Int_Gen_A_01ZombieSorcerer, SNOActor.x1_NPC_ZombieCellar_Male_A, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire, SNOWorld.x1_westm_Int_Gen_A_01ZombieSorcerer, 20000),
                }
            });

            // A5 - : Kill Grotescor (SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_HexMaze_Grotescor, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor, SNOWorld.X1_Pand_Ext_2_Battlefields, -702665403),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor, 10, 0, 0, 45),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Grotescor, 0, 5),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_HexMaze_Grotescor, 0, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Grotescor),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor, SNOWorld.X1_Pand_HexMaze_Grotescor, -1067001179),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor),

                }
            });

            // A5 - : Altar of Sadness (SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness, SNOWorld.x1_westm_Graveyard_DeathOrb, 2912417),
                    // x1_Graveyard_Alter_Event_Alter (SNOActor.x1_Graveyard_Alter_Event_Alter) Distance: 5.21285
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Graveyard_Alter_Event_Alter),
                    // x1_Graveyard_Alter_Event_Alter (SNOActor.x1_Graveyard_Alter_Event_Alter) Distance: 5.21285
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Graveyard_Alter_Event_Alter, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness, 15, 0, 0, 80, false),
                    // x1_Graveyard_Alter_Event_Alter_Chest (SNOActor.x1_Graveyard_Alter_Event_Alter_Chest) Distance: 17.49791
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Graveyard_Alter_Event_Alter_Chest, 0, 5),
                }
            });

            // A1 - Revenge of Gharbad (SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, 2912417),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, SNOActor.Gharbad_The_Weak_Ghost),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, SNOActor.Gharbad_The_Weak_Ghost, 2912417, 5),

                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, 10000),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_POI", new Vector3(122.2505f, 165.7817f, -28.86722f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_POI", new Vector3(118.2659f, 99.68457f, -28.36605f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_POI", new Vector3(137.4558f, 130.5591f, -28.34882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, 3000),
//                 new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, 30, 96582, 0, 45, false),
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, SNOActor.Gharbad_The_Weak_Ghost),
					new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, SNOActor.Gharbad_The_Weak_Ghost, 0, 5),

                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, SNOWorld.trOUT_Town, 10000),
					new AttackCoroutine(SNOActor.Gharbad_The_Weak_Ghost),
 //                 new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, 30, 96582, 0, 35, false),
                }
            });
            
            // A5 - A Diversion (SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 2912417),
                    // X1_Angel_Trooper_Event_Ballistae (SNOActor.X1_Angel_Trooper_Event_Ballistae) Distance: 3.603116
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.X1_Angel_Trooper_Event_Ballistae, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(130.6995f, 139.2733f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(130.098f, 105.4987f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(136.1014f, 149.4091f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(91.58203f, 139.6273f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(138.0638f, 143.7219f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(131.9744f, 102.1431f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(136.7021f, 141.1531f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(97.42975f, 140.667f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(136.5885f, 143.6037f, 17.94887f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, SNOWorld.X1_Pand_Ext_2_Battlefields, 10000),
					
//                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, 90, SNOActor.X1_Angel_Trooper_Event_Ballistae, 2912417 , 30, false),
                }
            });
            
            // A5 - Kill Severag (SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_HexMaze_Severag, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag, SNOWorld.X1_Pand_Ext_2_Battlefields, -702665403),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag,10,SNOActor.x1_PortalGuardian_A_Bloone,0,30,false),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Severag (SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Severag) Distance: 15.74465
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Severag, 0, 5),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag, SNOWorld.X1_Pand_HexMaze_Severag, 1891838257),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag),
                }
            });

            // A5 - Kill Bloone (SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_HexMaze_Bloone, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone, SNOWorld.X1_Pand_Ext_2_Battlefields, -702665403),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone,10, SNOActor.x1_PortalGuardian_A_Bloone, 0, 30, false),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Bloone (SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Bloone) Distance: 14.68041
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Bloone, -702665403, 5),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_HexMaze_Bloone, -702665403, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Bloone),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone, SNOWorld.X1_Pand_HexMaze_Bloone, 295634146),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone),
                }
            });

            // A5 - The Hive (SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, 2912417),

                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01"),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(112.7947f, 128.6941f, -17.4f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Ext_Event_Hive_Blocker),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, 10, 0, 0, 20, false),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(126.9121f, 156.9678f, -17.4f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(167.1134f, 105.6968f, -17.4f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(79.55261f, 139.0135f, -17.4f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Ext_Event_Hive_Blocker),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, 10, 0, 0, 20, false),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(167.1134f, 105.6968f, -17.4f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(137.1652f, 87.79041f, -17.39999f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Ext_Event_Hive_Blocker),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, 10, 0, 0, 20, false),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, SNOWorld.X1_Pand_Ext_2_Battlefields, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(85.9928f, 94.09125f, -17.4f)),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, 20, 0, 0, 20, false),
                }
            });

            // A5 - Touch of Death (SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    // this map is really bad for pathing, better to just explore for now.
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 2912417),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 2912417),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(108.632f, 114.135f, 5.099999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(107.2872f, 92.01654f, 5.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(128.494f, 114.0747f, 5.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(114.0435f, 140.9714f, 5.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(84.53052f, 114.5778f, 5.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(110.0929f, 115.1127f, 5.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, SNOWorld.X1_WESTM_ZONE_01, 10000),

//                  new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, 60, 4675, 0, 45,false),
                }
            });

            // A4 - Bounty: Watch Your Step (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Sigil_B, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A4_dun_Garden_of_Hope_B,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, -970799630),

                    new EnterLevelAreaCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 0, -970799630, SNOActor.g_portal_Ladder_Short_Blue_largeRadius),

                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(398, 515, 0)),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(394, 445, 0)),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(322, 454, 0)),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(263, 451, 0)),

                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(278, 398, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(278, 398, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(278, 398, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),

                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(273, 364, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(273, 364, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(273, 364, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),

                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(271, 333, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(271, 333, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(271, 333, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),

                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(274, 304, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(274, 304, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(274, 304, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),

                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(278, 274, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(278, 274, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(278, 274, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                    new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_B, new Vector3(278, 274, 0)),
                    new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, SNOWorld.a4dun_Garden_of_Hope_Random_B, 5000),
                }
            });

            // A2 - Kill Bonesplinter (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Bonesplinter)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Bonesplinter,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Bonesplinter, SNOWorld.caOUT_Town, 1417206738),
                    new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1197, 1293, 184)),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Bonesplinter),
                }
            });

            // A5 - A Shameful Death (SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, 1775330885, SNOActor.g_portal_RandomWestm),
                    // x1_westmarchGuard_CaptainStokely_Event (SNOActor.x1_westmarchGuard_CaptainStokely_Event) Distance: 3.895381
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, SNOActor.x1_westmarchGuard_CaptainStokely_Event),
                    // x1_westmarchGuard_CaptainStokely_Event (SNOActor.x1_westmarchGuard_CaptainStokely_Event) Distance: 3.895381
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, SNOActor.x1_westmarchGuard_CaptainStokely_Event, 0, 5),
                    // x1_TEMP_Westm_GhostSoldier_01 (SNOActor.x1_TEMP_Westm_GhostSoldier_01) Distance: 6.646848
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, SNOActor.x1_TEMP_Westm_GhostSoldier_01, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, 10, 0, 0, 200),
                    // x1_TEMP_Westm_GhostSoldier_01 (SNOActor.x1_TEMP_Westm_GhostSoldier_01) Distance: 24.93696
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, SNOActor.x1_TEMP_Westm_GhostSoldier_01),
                    // x1_TEMP_Westm_GhostSoldier_01 (SNOActor.x1_TEMP_Westm_GhostSoldier_01) Distance: 24.93696
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, SNOActor.x1_TEMP_Westm_GhostSoldier_01, 0, 5),
                    new MoveToPositionCoroutine(SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, new Vector3(315, 293, 10)),
                    // x1_TEMP_Westm_GhostSoldier_01 (SNOActor.x1_TEMP_Westm_GhostSoldier_01) Distance: 21.28371
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, SNOActor.x1_TEMP_Westm_GhostSoldier_01),
                    // x1_TEMP_Westm_GhostSoldier_01 (SNOActor.x1_TEMP_Westm_GhostSoldier_01) Distance: 21.28371
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, SNOWorld.x1_westm_Int_Gen_A_01_CaptainStokely, SNOActor.x1_TEMP_Westm_GhostSoldier_01, 0, 5),

                }
            });

            // A5 - Necromancer's Choice (SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice,
                Act = Act.A5,
                WorldId = SNOWorld.X1_westm_Int_Gen_A02Necromancer, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 852517898, SNOActor.g_portal_RandomWestm),
                    // oldNecromancer (SNOActor.oldNecromancer) Distance: 56.92637

					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, SNOActor.oldNecromancer),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, SNOActor.oldNecromancer, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(77.00719f, 182.8387f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(124.914f, 178.3416f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(85.01294f, 139.8238f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(77.00719f, 182.8387f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(124.914f, 178.3416f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(85.01294f, 139.8238f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 5000),

//                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, 60, 0, 0, 200),
					
                    // oldNecromancer (SNOActor.oldNecromancer) Distance: 27.00111
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, SNOActor.oldNecromancer),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, SNOActor.oldNecromancer, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, SNOWorld.X1_westm_Int_Gen_A02Necromancer, 20000),
                }
            });

            // A2 - The Putrid Waters (SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Special_A_Level01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_A_Level01, 705396550, SNOActor.g_Portal_Rectangle_Blue),
//                  new MoveToMapMarkerCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, 2912417),

					new MoveToScenePositionCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),
                    new WaitCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),
                    new WaitCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),
                    new WaitCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),

                    new WaitCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, SNOWorld.a2dun_Aqd_Special_A_Level01, 5000),
                    new ClearLevelAreaCoroutine(SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters),
                }
            });

            // A5 - 악마의 은신처 (SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_Cellar_C, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_Ext_Cellar_C, -1551729969, 0),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache, SNOWorld.X1_Pand_Ext_Cellar_C, 2912417),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache),
                    // x1_Global_Chest (SNOActor.x1_Global_Chest) Distance: 14.99657
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache, SNOWorld.X1_Pand_Ext_Cellar_C, SNOActor.x1_Global_Chest),
                    // x1_Global_Chest (SNOActor.x1_Global_Chest) Distance: 14.99657
                    //new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache, SNOWorld.X1_Pand_Ext_Cellar_C, SNOActor.x1_Global_Chest, 0, 5),
					// x1_Global_Chest (SNOActor.x1_Global_Chest) Distance: 10.39064
					new InteractionCoroutine(SNOActor.x1_Global_Chest, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)),

                }
            });

            // A5 - Kill Lu'ca (SNOQuest.X1_Bounty_A5_Bog_Kill_Luca)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Kill_Luca,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_BogPeople_Cellar_C, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Kill_Luca, SNOWorld.x1_Bog_01, -1947203376),
//                 new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Kill_Luca, SNOWorld.x1_Bog_01, SNOWorld.x1_Bog_BogPeople_Cellar_C, -1947203376, SNOActor.g_Portal_Circle_Orange_Bright),
					new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Kill_Luca, SNOWorld.x1_Bog_01, SNOWorld.x1_Bog_BogPeople_Cellar_C, -1947203376, new SNOActor[]{ SNOActor.g_Portal_Circle_Orange_Bright, SNOActor.g_portal_Ladder_Very_Short_Orange_VeryBright_BogPeople }),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Bog_Kill_Luca),
                }
            });

            // A1 - Bounty: Carrion Farm (SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, 2912417, true),
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_Sub240x240_FarmHouseB_x01_y01"),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(91.36401f, 74.96613f, 0.1f)),
					// NPC_Human_Male_Event_FarmAmbush (SNOActor.NPC_Human_Male_Event_FarmAmbush) Distance: 3.475385
                    new InteractionCoroutine(SNOActor.NPC_Human_Male_Event_FarmAmbush, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
//					new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, SNOActor.NPC_Human_Male_Event_FarmAmbush, 0, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_TristramFields_Sub240_POI_01", new Vector3(90f, 182.5f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(182.719f, 183.8639f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(184.0928f, 117.3088f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_TristramFields_Sub240_POI_01", new Vector3(90f, 182.5f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(182.719f, 183.8639f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(184.0928f, 117.3088f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, 5000),

                    // NPC_Human_Male_Event_FarmAmbush (SNOActor.NPC_Human_Male_Event_FarmAmbush) Distance: 36.39687
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(91.36401f, 74.96613f, 0.1f)),
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, SNOWorld.trOUT_Town, SNOActor.NPC_Human_Male_Event_FarmAmbush),
                    // NPC_Human_Male_Event_FarmAmbush (SNOActor.NPC_Human_Male_Event_FarmAmbush) Distance: 36.39687
                    new InteractionCoroutine(SNOActor.NPC_Human_Male_Event_FarmAmbush, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                }
            });

            // A5 - Bounty: Penny for Your Troubles (SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, SNOWorld.x1_westm_Graveyard_DeathOrb, 2912417),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady) Distance: 79.41319
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady) Distance: 79.41319
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady, 2912417, 5),
                    // x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event (SNOActor.x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event) Distance: 9.990323
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event),
                    // x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event (SNOActor.x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event) Distance: 9.990323
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event, 0, 5),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady) Distance: 25.86785
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady) Distance: 25.86785
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_WestM_Graveyard_Undead_Husband_Ghostlady, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, 5, 0, 0, 45),
                }
            });

            // A3 - Blood Ties (SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_random_02_Level_02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_random_02, -1049649953, SNOActor.g_Portal_ArchTall_Orange),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02, SNOWorld.a3Dun_Keep_random_02_Level_02, -1693984105, SNOActor.g_Portal_Rectangle_Orange, true),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort) Distance: 22.82924
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02_Level_02, SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort),
 
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort) Distance: 22.82924
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02_Level_02, SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort, -1049649953, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02_Level_02, "a3dun_Keep_EW_02", new Vector3(107.3766f, 180.9147f, 0.1f)),
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies,SNOWorld.a3Dun_Keep_random_02_Level_02,SNOActor.bastionsKeepGuard_Melee_A_02),
                    // FallenGrunt_C_RescueEscort_Unique (260230) Distance: 7.861986
                    //new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02_Level_02, 260230),
					new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, 20, 0, 0, 70,false),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort) Distance: 11.26799
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02_Level_02, SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort) Distance: 11.26799
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02_Level_02, SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, SNOWorld.a3Dun_Keep_random_02_Level_02, 15000),
                }
            });

            // A3 - Bounty: Blaze of Glory (SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2301, 613, 0)),
                    new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 2912417),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory) Distance: 52.0904
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory) Distance: 52.0904
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory, 2912417, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 10000),
					
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04"),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 5000),
//                  new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, 90, SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory, 2912417, 30),

                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory) Distance: 22.61426
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, SNOWorld.A3_Battlefields_02, 20000),
                }
            });

            // A1 - The Jar of Souls (SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Crypt_FalsePassage_01,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trOUT_Town, SNOWorld.trDun_Crypt_FalsePassage_01, -1965109038, SNOActor.g_Portal_ArchTall_Blue),

                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls,SNOWorld.trDun_Crypt_FalsePassage_01,"JarSouls"),
                    //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, 2912417),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(102.2006f, 103.909f, 2.337343f)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.a1dun_Crypts_Jar_of_Souls,2912417,5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(103.8495f, 103.4797f, 2.337343f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(97.08398f, 74.22736f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(129.2886f, 101.6852f, 0.1000002f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(104.57f, 127.1461f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(72.63531f, 97.39606f, 0.1000001f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(100.5025f, 104.5255f, 2.337343f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, 10000),
					
                    // a1dun_Crypts_Jar_of_Souls_02 (SNOActor.a1dun_Crypts_Jar_of_Souls_02) Distance: 4.83636
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(102.2006f, 103.909f, 2.337343f)),
                    // a1dun_Crypts_Jar_of_Souls_02 (SNOActor.a1dun_Crypts_Jar_of_Souls_02) Distance: 4.83636
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.a1dun_Crypts_Jar_of_Souls_02, 0, 5),
                }
            });

            // A5 - Kill Haziael (SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_HexMaze_Haziael, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael, SNOWorld.X1_Pand_Ext_2_Battlefields, -702665403),
                    // x1_PortalGuardian_A_Haziael (SNOActor.x1_PortalGuardian_A_Haziael) Distance: 89.75109
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael, 5, SNOActor.x1_PortalGuardian_A_Haziael, -702665403),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Haziael (SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Haziael) Distance: 14.4978
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Haziael, 0, 5),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael, SNOWorld.X1_Pand_HexMaze_Haziael, 329854592),
                    // X1_Angel_Trooper_Unique_HexMaze (SNOActor.X1_Angel_Trooper_Unique_HexMaze) Distance: 76.83235
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael, 5, SNOActor.X1_Angel_Trooper_Unique_HexMaze, 329854592),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael),

                }
            });

            // A5 - The Rebellious Rabble (SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, // Enter the final worldId here
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                {
//					new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_WESTM_ZONE_01, -752748509),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, -752748509, SNOActor.g_portal_RandomWestm),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, new Vector3(304, 424, 10)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, new Vector3(281, 284, 15)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_WESTM_ZONE_01, 5000),
                    new MoveToPositionCoroutine(SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, new Vector3(421, 262, 25)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01),
                    // x1_NPC_Westmarch_Male_A_Severin (SNOActor.x1_NPC_Westmarch_Male_A_Severin) Distance: 7.18242
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, SNOActor.x1_NPC_Westmarch_Male_A_Severin),
                    // x1_NPC_Westmarch_Male_A_Severin (SNOActor.x1_NPC_Westmarch_Male_A_Severin) Distance: 7.18242
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, SNOActor.x1_NPC_Westmarch_Male_A_Severin, 0, 5),

                }
            });

            // A3 - The Triage (SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(4240, 407, -2)),
                    new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(4174, 385, -2)),
                    new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(4066, 473, 0)),

                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 2912417),

                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Melee_A_01_snatched, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(141.176f, 167.222f, 0.2f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(126.7441f, 150.7867f, 0.1999999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(116.4688f, 166.6454f, 0.1999991f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(127.8069f, 180.6221f, 0.2000004f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(126.7441f, 150.7867f, 0.1999999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(116.4688f, 166.6454f, 0.1999991f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(127.8069f, 180.6221f, 0.2000004f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(141.6938f, 164.2231f, 0.2000004f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, 10000),

//                  new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, 60, 0, 0, 30),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, SNOWorld.A3_Battlefields_02, SNOActor.OmniNPC_Female_Act3_B_MedicalCamp, 0, 5),
                }
            });

            // A2 - Lair of the Lacuni (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair,
                Act = Act.A2,
                WorldId = SNOWorld.caOut_Interior_G_Stranded2, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOUT_Town, SNOWorld.caOut_Interior_G_Stranded2, 288776660, SNOActor.g_Portal_Circle_Orange_Bright),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(216.0627f, 151.6613f, 12.99976f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(207.9222f, 86.58944f, 12.99976f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(163.856f, 119.5887f, -0.8271011f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(132.1614f, 72.54372f, -0.8271086f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(95.32114f, 139.1705f, -3.564227f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(74.49428f, 215.861f, -0.882843f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(176.3916f, 171.3593f, -0.8271087f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, "caOut_Interior_G_x01_y01", new Vector3(179.2931f, 181.3407f, -0.8271185f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, SNOWorld.caOut_Interior_G_Stranded2, 5000),

                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair),
                }
            });

            // A5 - The Rebellious Rabble (SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, -752748509, SNOActor.g_portal_RandomWestm),
                    new MoveToPositionCoroutine(SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, new Vector3(301, 424, 10)),
                    new MoveToPositionCoroutine(SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, new Vector3(279, 277, 15)),
                    new MoveToPositionCoroutine(SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, new Vector3(418, 266, 25)),
                    // x1_NPC_Westmarch_Male_A_Severin (SNOActor.x1_NPC_Westmarch_Male_A_Severin) Distance: 15.74743
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, SNOActor.x1_NPC_Westmarch_Male_A_Severin),
                    // x1_NPC_Westmarch_Male_A_Severin (SNOActor.x1_NPC_Westmarch_Male_A_Severin) Distance: 13.87104
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, SNOWorld.X1_Westm_Int_Gen_A_03_KingEvent01, SNOActor.x1_NPC_Westmarch_Male_A_Severin, 0, 5),
                }
            });

            // A1 - A Stranger in Need (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level02, // Enter the final worldId here
                WaypointLevelAreaId = SNOLevelArea.A1_trDun_Leoric02,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, 2912417),
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02,"Exit_Boss"),
//                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, SNOActor.OmniNPC_Tristram_Male_Leoric_RescueEvent),
                    // a1dun_Leoric_IronMaiden_Event (SNOActor.a1dun_Leoric_IronMaiden_Event) Distance: 5.864555
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden,60,SNOActor.OmniNPC_Tristram_Male_Leoric_RescueEvent,0,30),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (SNOActor.OmniNPC_Tristram_Male_Leoric_RescueEvent) Distance: 1.570357
//                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, SNOActor.a1dun_Leoric_IronMaiden_Event),
                    // a1dun_Leoric_IronMaiden_Event (SNOActor.a1dun_Leoric_IronMaiden_Event) Distance: 10.85653
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, SNOActor.a1dun_Leoric_IronMaiden_Event, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, 15000),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (SNOActor.OmniNPC_Tristram_Male_Leoric_RescueEvent) Distance: 4.789657
 //                   new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, SNOActor.OmniNPC_Tristram_Male_Leoric_RescueEvent),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (SNOActor.OmniNPC_Tristram_Male_Leoric_RescueEvent) Distance: 4.789657
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, SNOActor.OmniNPC_Tristram_Male_Leoric_RescueEvent, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, SNOWorld.trDun_Leoric_Level02, 25000),
                }
            });

            // A5 - Bounty: Noble Deaths (SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Int_Gen_B_02_KingEvent02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02, SNOWorld.X1_WESTM_ZONE_01, -752748508),
					
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.x1_westm_Int_Gen_B_02_KingEvent02, -752748508, SNOActor.g_portal_RandomWestm),
                    new MoveToPositionCoroutine(SNOWorld.x1_westm_Int_Gen_B_02_KingEvent02, new Vector3(407, 293, 7)),
					
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02, SNOWorld.x1_westm_Int_Gen_B_02_KingEvent02, SNOActor.x1_westm_Door_Wide_Clicky, 0, 3),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02, 30, SNOActor.x1_NPC_Westmarch_Gorrel, 0, 50),
                    // x1_NPC_Westmarch_Gorrel_NonUnique (SNOActor.x1_NPC_Westmarch_Gorrel_NonUnique) Distance: 5.344718
					new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02),
					
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02, SNOWorld.x1_westm_Int_Gen_B_02_KingEvent02, SNOActor.x1_NPC_Westmarch_Gorrel_NonUnique),
                    // x1_NPC_Westmarch_Gorrel_NonUnique (SNOActor.x1_NPC_Westmarch_Gorrel_NonUnique) Distance: 5.344718
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02, SNOWorld.x1_westm_Int_Gen_B_02_KingEvent02, SNOActor.x1_NPC_Westmarch_Gorrel_NonUnique, 0, 5),
					
                }
            });

            // A5 - Bounty: Grave Robert (SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 2912417),
                    new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14"),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Graveyard_GraveRobert, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(49.48395f, 74.43878f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(46.55865f, 59.42267f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(58.74109f, 69.40292f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(35.54315f, 68.42136f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(48.36139f, 80.40656f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(49.48395f, 74.43878f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(46.55865f, 59.42267f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(58.74109f, 69.40292f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(35.54315f, 68.42136f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_14", new Vector3(48.36139f, 80.40656f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),

//                  new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, 90, 0, 0, 30, false),

					// x1_Graveyard_GraveRobert (SNOActor.x1_Graveyard_GraveRobert) Distance: 12.73469
					new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Graveyard_GraveRobert, 0, 5),

                }
            });

            // A5 - The Burning Man (SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 2912417),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(134.7274f, 92.38818f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(119.4648f, 83.57068f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(105.3293f, 103.0908f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(124.6479f, 120.5757f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(134.822f, 103.1282f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(108.6432f, 90.27405f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(93.05151f, 110.4031f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, "X1_Bog_Sub240_Wickerman", new Vector3(116.2715f, 107.064f, -29.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, SNOWorld.x1_Bog_01, 5000),
						
						
//                      new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, 60, 288471, 2912417, 30),
                    }
            });
            
            // A5 - Kill Borgoth (SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_HexMaze_Borgoth,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth, SNOWorld.X1_Pand_Ext_2_Battlefields, -702665403),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth, SNOWorld.X1_Pand_Ext_2_Battlefields, 5000),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth,SNOWorld.X1_Pand_Ext_2_Battlefields,SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Borgoth,702665403,3),
                        //new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth,SNOWorld.X1_Pand_Ext_2_Battlefields,SNOWorld.X1_Pand_HexMaze_Borgoth,-702665403,SNOActor.x1_Pand_Maze_PortalTest_OnDeathPortal_Borgoth),
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth,SNOWorld.X1_Pand_HexMaze_Borgoth,653410364),
                        //new KillUniqueMonsterCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth,SNOWorld.X1_Pand_HexMaze_Borgoth,307335,653410364 ),
                        new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth)
                    }
            });

            // A2 - Kill Razormouth (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1972, 1034, 170)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1985, 833, 169)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1805, 965, 169)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1800, 1191, 169)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1816, 1383, 170)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1908, 1079, 172)),
						
						//new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth, SNOWorld.caOUT_Town, "caOut_StingingWinds_E06_S10", new Vector3(62.45496f, 77.93591f, 171.1703f)),
						//new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth, SNOWorld.caOUT_Town, "caOut_StingingWinds_E07_S09", new Vector3(216.5885f, 12.90784f, 170.1795f)),
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth, SNOWorld.caOUT_Town, -79527531, false),

                        new KillUniqueMonsterCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth,SNOWorld.caOUT_Town,SNOActor.SandShark_A_Unique_01,-79527531),
                        new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth)
                    }
            });

            // A5 - Home Invasion (SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Westm_Cellar_Ruffians,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, SNOWorld.X1_Westm_Cellar_Ruffians, 412504359 , SNOActor.g_portal_RandomWestm),
                        //new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(433, 349, 0)),
                        //new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion,SNOWorld.X1_Westm_Cellar_Ruffians,328459,0,5),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, 2000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(410, 335, 15)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, 2000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(396, 308, 15)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, 2000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(396, 308, 15)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, 2000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(364, 358, 0)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, 2000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(328, 345, 0)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, 2000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(281, 350, -9)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, SNOWorld.X1_WESTM_ZONE_01, 2000),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Ruffians, new Vector3(248, 339, -9)),

                    }
            });

            // A5 - Hide and Seek (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Westm_Cellar_Kids,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_Westm_Cellar_Kids, -842376652  , SNOActor.g_portal_RandomWestm),
                        new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,15,0,2912417),
                        new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,SNOWorld.X1_Westm_Cellar_Kids,SNOActor.x1_Child_Kyla,0,5),
                        new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,SNOWorld.X1_Westm_Cellar_Kids,SNOActor.x1_westm_Door_Wide_Clicky),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,SNOWorld.X1_Westm_Cellar_Kids,SNOActor.x1_westm_Door_Wide_Clicky,0,5),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Kids, new Vector3(401, 310, 15)),
                        new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,15,0,0,40,false),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Kids, new Vector3(365, 364, 0)),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,SNOWorld.X1_Westm_Cellar_Kids,SNOActor.x1_westm_Door_Wide_Clicky,0,5),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Kids, new Vector3(336, 342, 0)),
                        new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,15,0,0,40,false),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Kids, new Vector3(288, 343, -9)),
                        new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek,SNOWorld.X1_Westm_Cellar_Kids,SNOActor.x1_Child_Kyla,0,5),
                        new MoveToPositionCoroutine(SNOWorld.X1_Westm_Cellar_Kids, new Vector3(262, 351, -9)),
                    }
            });

            // A4 - Wormsign (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilD)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilD,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Sigil_D,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilD, SNOWorld.a4dun_Garden_of_Hope_Random_B, SNOWorld.a4dun_Sigil_D, -970799628 , SNOActor.g_portal_Ladder_Short_Blue_largeRadius),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilD)
                    }
            });

            // A2 - Kill Thugeesh the Enraged (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thugeesh)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thugeesh,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thugeesh, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level01, -1363317799 , SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thugeesh, SNOWorld.a2Dun_Zolt_Level01, SNOActor.FallenChampion_B_Unique_01, -349018056),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thugeesh)
                    }
            });
            
            // A2 - A Miner's Gold (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2687, 1513, 184)),

                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner,SNOWorld.caOUT_Town, 2912417),

                        new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner,SNOWorld.caOUT_Town,"caOut_Sub240x240_Mine_Destroyed"),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(113.6646f, 99.63721f, 230.3514f)),
						new InteractionCoroutine(SNOActor.A2C2GreedyMiner, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
						
                        //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner,SNOWorld.caOUT_Town,SNOActor.A2C2GreedyMiner,2912417,5),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 20000),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(80.57178f, 72.74329f, 230.2913f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(80.69434f, 157.0466f, 230.3514f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(84.9397f, 49.0614f, 218.9766f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(87.19385f, 177.0327f, 220.3537f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(92.45044f, 110.6554f, 230.3514f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(129.2219f, 96.4989f, 231.3068f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(87.19385f, 177.0327f, 220.3537f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(92.45044f, 110.6554f, 230.3514f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, "caOut_Sub240x240_Mine_Destroyed", new Vector3(129.2219f, 96.4989f, 231.3068f)),
                        
                        // a2dun_Aqd_Chest_Special_GreedyMiner (SNOActor.a2dun_Aqd_Chest_Special_GreedyMiner) Distance: 14.79873
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, SNOWorld.caOUT_Town, SNOActor.a2dun_Aqd_Chest_Special_GreedyMiner, 0, 5),
                    }
            });

            // A1 - Last Stand of the Ancients (SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {

                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, 2912417),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town,SNOActor.Temp_Story_Trigger_Enabled,2912417,5),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, 10000),
                        //new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, 60, SNOActor.Temp_Story_Trigger_Enabled, 2912417),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(52.79926f, 57.75702f, 43.55412f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(101.1246f, 22.6095f, 22.2632f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(98.59137f, 63.27435f, 36.85922f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(86.4173f, 98.62268f, 23.32761f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(98.59137f, 63.27435f, 36.85922f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, SNOWorld.trOUT_Town, 10000),
 //                     new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, 60, 0, 0, 45),
                    }
            });

            // A5 - Out of Time (SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison, SNOWorld.x1_fortress_level_02, 2912417),
//                     new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison,SNOWorld.x1_fortress_level_02,SNOActor.x1_PandExt_Time_Activator),

                        // x1_PandExt_Time_Activator (SNOActor.x1_PandExt_Time_Activator) Distance: 25.83982
//                      new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison, SNOWorld.x1_fortress_level_02, SNOActor.x1_PandExt_Time_Activator, 2912417, 5),
						
						// x1_PandExt_Time_Activator (SNOActor.x1_PandExt_Time_Activator) Distance: 13.5326
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison, SNOWorld.x1_fortress_level_02, SNOActor.x1_PandExt_Time_Activator, 0, 5),

                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison, SNOWorld.x1_fortress_level_02, 2000),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison, 20, 0, 0)
                    }
            });
            
            // A3 - Forged in Battle (SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level03,
                WaypointLevelAreaId = SNOLevelArea.A3_Dun_Keep_Level04,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
					// g_Portal_ArchTall_Orange (SNOActor.g_Portal_ArchTall_Orange) Distance: 17.13974
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level04, SNOActor.g_Portal_ArchTall_Orange),
					
					// g_Portal_ArchTall_Orange (SNOActor.g_Portal_ArchTall_Orange) Distance: 8.634123
					//new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Bog_Clear_PerilousCave2, SNOWorld.a3Dun_Keep_Level04, SNOActor.g_Portal_ArchTall_Orange, -1699330856, 5),
					new InteractionCoroutine(SNOActor.g_Portal_ArchTall_Orange, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, 2912417, true),
					//new ExplorationCoroutine(new HashSet<int>() { 75436 }, breakCondition: () => (Core.Player.LevelAreaId != 75436 || BountyHelpers.ScanForMarker(2912417, 50) != null)),

					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge"),
                    // A3_UniqueVendor_Weaponsmith (SNOActor.A3_UniqueVendor_Weaponsmith) Distance: 17.43861
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, SNOActor.A3_UniqueVendor_Weaponsmith, 1, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(113.2659f, 110.6841f, 0.3101287f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(76.26773f, 85.44995f, 0.2815379f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(144.5543f, 81.32654f, 0.0716451f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(136.5824f, 112.8745f, 0.1000016f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(83.1055f, 111.15f, 0.2562059f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, 8000),
                    // A3_UniqueVendor_Weaponsmith (SNOActor.A3_UniqueVendor_Weaponsmith) Distance: 17.43861
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, SNOActor.A3_UniqueVendor_Weaponsmith, 1, 5),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(76.47034f, 85.85547f, 0.1844295f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(142.7763f, 82.57361f, -0.2093051f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, "a3dun_Keep_SW_03_Forge", new Vector3(124.6754f, 101.4712f, 0.5702538f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, 10000),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, SNOWorld.a3Dun_Keep_Level03, SNOActor.A3_UniqueVendor_Weaponsmith, 1, 5),

                    }
            });

            // A2 - Kill the Stinging Death Swarm (SNOQuest.px_Bounty_A2_Aqueducts_Kill_StingingDeathSwarm)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Aqueducts_Kill_StingingDeathSwarm,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Special_A_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_StingingDeathSwarm, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_A_Level01, 705396550, SNOActor.g_Portal_Rectangle_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_StingingDeathSwarm,SNOWorld.a2dun_Aqd_Special_A_Level01, SNOActor.Swarm_B_Unique_01, 904884897),
                        new ClearLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_StingingDeathSwarm)
                    }
            });
            
            // A4 - Hellbreeder Nest (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Sigil_A,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_dun_Garden_of_Hope_B,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA, SNOWorld.a4dun_Garden_of_Hope_Random_B, -970799631),
						new EnterLevelAreaCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA, SNOWorld.a4dun_Garden_of_Hope_Random_B, 0, -970799631, SNOActor.g_portal_Ladder_Short_Blue_largeRadius),

						// px_morluSpellcaster_D_Unique_EliteStrike (410426) Distance: 19.12132
						new MoveToActorCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA, SNOWorld.a4dun_Sigil_A, SNOActor.px_morluSpellcaster_D_Unique_EliteStrike),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA, SNOWorld.a4dun_Sigil_A, SNOActor.px_morluSpellcaster_D_Unique_EliteStrike, 0),
                        new ClearLevelAreaCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA)
					}
            });

            // A3 - Tide of Battle (SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle, SNOWorld.A3_Battlefields_02, 2912417),
						
                        new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE"),
						// bastionsKeepGuard_Melee_A_01_NPC_Event_TideOfBattle (SNOActor.bastionsKeepGuard_Melee_A_01_NPC_Event_TideOfBattle) Distance: 3.018499
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Melee_A_01_NPC_Event_TideOfBattle),
						
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle, 60, SNOActor.bastionsKeepGuard_Melee_A_01_NPC_Event_TideOfBattle, 2912417, 30),
						
						
						new MoveToScenePositionCoroutine(0, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(144.4851f, 185.6969f, 0.2000003f)),						
						new InteractionCoroutine(SNOActor.bastionsKeepGuard_Melee_A_01_NPC_Event_TideOfBattle, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
 
                        //new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle,SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Melee_A_01_NPC_Event_TideOfBattle, 0, 5)
                    }
            });

            // A1 - Kill Krailen the Wicked (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Krailen)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Krailen,
                Act = Act.A1,
                WorldId = SNOWorld.a1dun_Leor_Manor,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1711, 3856, 40)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1517, 3837, 40)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1587, 4023, 38)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1413, 4076, 40)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1437, 3930, 50)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1214, 3903, 79)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1202, 3773, 80)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1076, 3880, 78)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(957, 3950, 80)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(892, 3860, 90)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1085, 3729, 78)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1080, 3506, 74)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1077, 3389, 65)),

                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Krailen, SNOWorld.trOUT_Town, SNOWorld.a1dun_Leor_Manor, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Krailen, SNOWorld.a1dun_Leor_Manor, SNOActor.TriuneSummoner_A_Unique_02, 718706852),	
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Krailen)
                    }
            });

            // A2 - Rygnar Idol (SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol,
                Act = Act.A1,
                WorldId = SNOWorld.a2c2dun_Zolt_TreasureHunter,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.caOUT_Town, 260002582),
                        //new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.caOUT_Town, SNOWorld.a2c2dun_Zolt_TreasureHunter, 260002582, SNOActor.g_Portal_Circle_Orange_Bright),
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.caOUT_Town, 0, 260002582, SNOActor.g_Portal_Circle_Orange_Bright),
                        new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.a2c2dun_Zolt_TreasureHunter, SNOActor.A2C2Poltahr, 0, 5),
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.a2c2dun_Zolt_TreasureHunter, 1557208829),

                        new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.caOUT_Town, "a2dun_Zolt_Random_W_03_Poltahr"),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.a2c2dun_Zolt_TreasureHunter, "a2dun_Zolt_Random_W_03_Poltahr", new Vector3(77.97357f, 89.26093f, -12.4f)),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, SNOWorld.a2c2dun_Zolt_TreasureHunter, "a2dun_Zolt_Random_W_03_Poltahr", new Vector3(84.41614f, 120.6121f, -11.6196f)),

                        new InteractionCoroutine((SNOActor)SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1),5),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol,SNOWorld.a2c2dun_Zolt_TreasureHunter,SNOActor.a2dun_Zolt_Pedestal,0,5),
						
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, 20, 0, 0),
                    }
            });

            // A1 - Eternal War (SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 2912417),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(56.42166f, 67.17035f, 20.1f)),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, SNOActor.Temp_FesteringWoodsAmbush_Switch, 2912417, 5),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 10000),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(54.41251f, 60.12646f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(66.2486f, 44.25531f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(57.53882f, 62.03363f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(48.30765f, 87.24634f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(61.108f, 63.65912f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(72.8396f, 36.59406f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(58.68997f, 62.7016f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, SNOWorld.trOUT_Town, 5000),

                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, 60, SNOActor.Temp_FesteringWoodsAmbush_Switch, 2912417, 25),
                    }
            });

            // A1 - The Family of Rathe (SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trOUT_Town, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level01, 1070710595, SNOActor.g_Portal_ArchTall_Orange),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe,SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level01, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, 1070710596, SNOActor.g_Portal_ArchTall_Blue),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, "trDun_Crypt_NSEW_01", new Vector3(116.1539f, 97.55945f, -4.9f)),
                        new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe,SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, SNOActor.FamilyTree_Daughter, 0, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, "trDun_Crypt_NSEW_01", new Vector3(103.6976f, 82.98633f, -4.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, "trDun_Crypt_NSEW_01", new Vector3(119.7391f, 97.01306f, -4.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, "trDun_Crypt_NSEW_01", new Vector3(102.7822f, 126.6759f, -4.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, "trDun_Crypt_NSEW_01", new Vector3(80.44812f, 101.0051f, -4.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, 5000),

                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, 60, SNOActor.FamilyTree_Daughter, 0, 30),
                        // FamilyTree_Daughter (SNOActor.FamilyTree_Daughter) Distance: 2.11157
                        new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, SNOWorld.trDun_Crypt_Fields_Flooded_Memories_Level02, SNOActor.FamilyTree_Daughter, 0, 5),
                    }
            });

            // A1 - Kill Theodyn Deathsinger (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Theodyn)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Theodyn,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands2, SNOLevelArea.A1_trOUT_Highlands3, SNOLevelArea.A1_trOUT_LeoricsManor },
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1744, 3830, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1627, 4031, 38)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1718, 4177, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1507, 4153, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1449, 4053, 39)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1206, 3897, 79)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1182, 3741, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1026, 3816, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(911, 3886, 90)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1148, 4082, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1147, 3909, 78)),					

                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Theodyn,SNOWorld.trOUT_Town, SNOActor.TriuneCultist_B_Unique_01, -2069254570),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Theodyn )
                    }
            });

            #region Missing Data

            // A2 - Clear the Vile Cavern (SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2) 
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2,
                Act = Act.A2,
                WorldId = SNOWorld.a2trDun_Boneyard_Spider_Cave_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2, SNOWorld.caOUT_Town, -270313354, true),

 //                     new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2, SNOWorld.caOUT_Town, SNOWorld.a2trDun_Boneyard_Spider_Cave_01, -270313354, SNOActor.g_portal_Ladder_Short_Orange_Bright),
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2, SNOWorld.caOUT_Town, 0, -270313354, SNOActor.g_portal_Ladder_Short_Orange_Bright),
						
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2, SNOWorld.a2trDun_Boneyard_Spider_Cave_01, -270313353, true),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2, SNOWorld.a2trDun_Boneyard_Spider_Cave_01, SNOWorld.a2trDun_Boneyard_Spider_Cave_02, -270313353, SNOActor.g_Portal_ArchTall_Orange),
						
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2)
                    }
            });

            // A3 - Clear Cryder's Outpost (SNOQuest.X1_Bounty_A3_Battlefields_Clear_CrydersOutpost)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_CrydersOutpost,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Random_Cellar_01,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_CrydersOutpost, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_Random_Cellar_01, 211059664, SNOActor.g_Portal_ArchTall_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_CrydersOutpost)
                    }
            });
            #endregion


        }

        private static void RemoveCustomBounties(params SNOQuest[] questIds)
        {
            foreach (var questId in questIds)
            {
                var index = Bounties.FindIndex(b => b.QuestId == questId);
                if (index >= 0)
                {
                    Bounties.RemoveAt(index);
                }
            }
        }

        private static void AddCursedBounties()
        {

            // A3 - Bounty: The Cursed Stronghold (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    // todo needs work, failing.
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 2912417),
                    // x1_Event_CursedShrine (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 22.65772
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                    //ActorId: SNOActor.x1_Global_Chest_CursedChest_B, Type: Gizmo, Name: x1_Event_CursedShrine-36946, Distance2d: 4.163542, CollisionRadius: 10.04086, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0

					new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(73.44867f, 127.4745f, -9.9f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(107.384f, 115.0124f, -9.899999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(51.16956f, 124.3692f, -9.323366f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(71.05536f, 113.5079f, -9.9f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(73.44867f, 127.4745f, -9.9f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(107.384f, 115.0124f, -9.899999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(51.16956f, 124.3692f, -9.323366f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NW_01", new Vector3(71.05536f, 113.5079f, -9.9f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
//                  new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_NorthernAggression, 120, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 20, false),
                }
            });

            // A3 - Bounty: The Cursed Bailey (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_DeadlyNature)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_DeadlyNature,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_02, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_DeadlyNature, SNOWorld.a3dun_ruins_frost_city_A_01, SNOWorld.a3dun_ruins_frost_city_A_02, 1615795536, SNOActor.g_Portal_Rectangle_Blue),
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_DeadlyNature, SNOWorld.a3dun_ruins_frost_city_A_02, 2912417),
                    // x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 27.16287
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_DeadlyNature, SNOWorld.a3dun_ruins_frost_city_A_02, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_DeadlyNature, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30),
                }
            });

            // A2 - The Cursed Archive  (SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Zolt_Blood02,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.caOUT_Town, SNOWorld.a2dun_Zolt_Blood02, -1758560943, SNOActor.g_Portal_Circle_Orange_Bright),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead,SNOWorld.a2dun_Zolt_Blood02, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(66.73035f, 71.62518f, 12.70991f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(85.03119f, 67.16919f, 12.71109f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(89.98163f, 89.61292f, 12.70991f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(66.41211f, 88.02924f, 12.70991f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(75.81531f, 76.53839f, 12.7282f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(86.68158f, 57.90594f, 12.71109f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(95.56787f, 78.08429f, 12.71109f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(67.86664f, 89.98248f, 12.70991f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, SNOWorld.a2dun_Zolt_Blood02, "a2dun_Zolt_Random_NSEW_02", new Vector3(66.0199f, 68.46393f, 12.70991f)),

//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, 90, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A1 - The Cursed Grove (SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove,SNOWorld.trOUT_Town, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove,SNOWorld.trOUT_Town, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
						
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(115.2236f, 90.55316f, 0.2000003f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(137.6882f, 100.93f, 0.2f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(123.3628f, 130.9887f, 0.1999999f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(89.66846f, 106.1782f, 0.2000004f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(113.4138f, 93.34155f, 0.2000001f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(136.521f, 106.8851f, 0.2000001f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(117.2524f, 126.4353f, 0.2000002f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(92.3208f, 107.9351f, 0.2000004f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(113.3569f, 107.1234f, 0.2000004f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, "trOut_Sub240x240_FieldsTreeGroveB_x01_y01", new Vector3(112.2983f, 89.60699f, 0.2000003f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, SNOWorld.trOUT_Town, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });
            
            // A1 - The Cursed Camp (SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(129.8826f, 98.04395f, 9.915272f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(140.8633f, 123.8149f, 9.915271f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(129.5017f, 139.8765f, 9.915272f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(130.6418f, 121.6338f, 9.915272f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(126.72f, 120.3223f, 9.915272f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, SNOWorld.trOUT_Town, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A1 - The Cursed Bellows (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level03,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire,SNOWorld.trDun_Leoric_Level03, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire,SNOWorld.trDun_Leoric_Level03, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(88.48364f, 135.6439f, -9.300045f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(94.53369f, 165.1028f, -9.300045f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(102.0026f, 131.6589f, -9.300045f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(96.51099f, 98.6366f, -9.300044f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(88.48364f, 135.6439f, -9.300045f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, SNOWorld.trDun_Leoric_Level03, 10000),

//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, 90, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
						
                    }
            });

            // A2 - The Cursed Battlement (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband,SNOWorld.caOUT_Town, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(128.3037f, 139.0692f, 207.4763f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(154.2427f, 146.2689f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(143.1792f, 164.8138f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(116.9663f, 157.3429f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(134.8655f, 155.4772f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(129.7896f, 137.2734f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, SNOWorld.caOUT_Town, 10000),

//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)	
                    }
            });

            // A2 - The Cursed Outpost (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 2912417),
                        //new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress,SNOWorld.caOUT_Town, (int)SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
						// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 8.263062
						
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(147.6401f, 133.1014f, 207.4763f)),
						//new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),
						new InteractionCoroutine(SNOActor.x1_Global_Chest_CursedChest_B, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(147.6401f, 133.1014f, 207.4763f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(170.0735f, 144.5144f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(151.7021f, 164.7112f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(128.4934f, 149.3925f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(153.5044f, 151.0697f, 207.4763f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(146.2393f, 127.4581f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(147.6401f, 133.1014f, 207.4763f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(170.0735f, 144.5144f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(151.7021f, 164.7112f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(128.4934f, 149.3925f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(153.5044f, 151.0697f, 207.4763f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(146.2393f, 127.4581f, 207.4762f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, SNOWorld.caOUT_Town, 5000),

//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A2 - The Cursed Shallows (SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave,
                Act = Act.A2,
                WorldId = SNOWorld.a2trDun_Cave_Oasis_Random02,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
//                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.caOUT_Town, 1352061373),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.caOUT_Town, SNOWorld.a2trDun_Cave_Oasis_Random02, 1352061373, SNOActor.g_Portal_Circle_Blue),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave,SNOWorld.a2trDun_Cave_Oasis_Random02, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
						
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(125.217f, 48.37146f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(138.6002f, 37.02307f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(138.7433f, 58.17078f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(115.9611f, 62.86017f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(125.1077f, 45.81134f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(135.4672f, 20.88052f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(143.2992f, 49.99261f, 0.09999999f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(122.6326f, 65.04968f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, "a2dun_Cave_Flooded_NSW_01", new Vector3(122.179f, 43.45789f, 0.1f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, SNOWorld.a2trDun_Cave_Oasis_Random02, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A3 - The Cursed Garrison (SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky,SNOWorld.A3_Battlefields_02, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky,SNOWorld.A3_Battlefields_02, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                        new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_ParentScene"),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(88.71631f, 149.5671f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(112.2556f, 129.1063f, 20.75397f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(131.2407f, 148.1129f, 20.7601f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(115.8555f, 160.5751f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(113.8833f, 140.493f, 20.76011f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(116.342f, 127.2602f, 20.73954f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(133.248f, 139.9616f, 21.01073f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(111.6228f, 157.4414f, 20.16819f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(97.32251f, 139.0292f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(88.94995f, 146.5066f, 20.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, SNOWorld.A3_Battlefields_02, 10000),

//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A3 - The Cursed Glacier (SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_IceCaves_Timed_01,
                QuestType = BountyQuestType.ClearCurse,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A3_Battlefield_B },
                WaypointLevelAreaId = SNOLevelArea.A3_Bridge_Choke_A,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill, SNOWorld.A3_Battlefields_02, 942020622),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill, SNOWorld.A3_Battlefields_02, 0, 942020622, SNOActor.g_Portal_Circle_Blue),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill, SNOWorld.a3dun_IceCaves_Timed_01, 2912417),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill, SNOWorld.a3dun_IceCaves_Timed_01, "a3dun_iceCaves_EW_01", new Vector3(91.36786f, 135.6528f, 0.1000028f)),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill,SNOWorld.a3dun_IceCaves_Timed_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A3 - The Cursed Depths (SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level04,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 2912417),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(180.0914f, 89.91431f, 0.5058147f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 1000),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(180.0914f, 89.91431f, 0.5058147f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(191.4712f, 98.52448f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(178.8483f, 113.7626f, 0.9475988f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(161.0414f, 85.14581f, 0.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(191.4712f, 98.52448f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(178.8483f, 113.7626f, 0.9475988f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(161.0414f, 85.14581f, 0.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, "a3dun_Keep_NSEW_03_War", new Vector3(180.0914f, 89.91431f, 0.5058147f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, SNOWorld.a3Dun_Keep_Level04, 10000),

//                     new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A3 - The Cursed Caldera (SNOQuest.X1_Bounty_A3_Crater_Event_BloodClanAssault)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Event_BloodClanAssault,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_02,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A3_Crater_Event_BloodClanAssault, SNOWorld.a3Dun_Crater_Level_02, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A3_Crater_Event_BloodClanAssault, SNOWorld.a3Dun_Crater_Level_02, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A3_Crater_Event_BloodClanAssault, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A4 - The Cursed Chapel (SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(185.7661f, 167.3568f, 0.8628434f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(203.0096f, 162.3612f, 0.1000002f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(201.2533f, 187.0447f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(174.2996f, 181.8645f, 0.8628433f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(188.3243f, 169.1984f, 0.8628434f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(200.3032f, 153.4366f, 0.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(198.2024f, 182.1554f, 0.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(173.029f, 181.0692f, 0.8628433f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, "a4dun_spire_corrupt_SE_02", new Vector3(183.8937f, 165.0882f, 0.8628433f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, SNOWorld.a4dun_Spire_Level_02, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417,30)
                    }
            });

            // A4 - The Cursed Dais (SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_01,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut, SNOWorld.a4dun_Garden_of_Hope_01, 2912417, true),
						// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 9.826684
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut, SNOWorld.a4dun_Garden_of_Hope_01, SNOActor.x1_Global_Chest_CursedChest_B),
						// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 9.826684
						new WaitCoroutine(SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut, SNOWorld.a4dun_Garden_of_Hope_01, 1000),
						
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut, SNOWorld.a4dun_Garden_of_Hope_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
						//new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut, SNOWorld.a4dun_Garden_of_Hope_01, "a4dun_garden_level_01_E04_S02"),
						
						// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 13.16928
						//new InteractionCoroutine(SNOActor.x1_Global_Chest_CursedChest_B, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A5 - The Cursed Realm (SNOQuest.X1_Bounty_A5_PandExt_Event_HostileRealm)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_HostileRealm,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_HostileRealm, SNOWorld.X1_Pand_Ext_2_Battlefields, 2912417),
						
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Event_HostileRealm, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Event_HostileRealm, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 20, false)
                    }
            });

            // A1 - The Cursed Cellar (SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar,
                Act = Act.A1,
                WorldId = SNOWorld.trOut_Oldtistram_Cellar_2,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOUT_Town, SNOWorld.trOut_Oldtistram_Cellar_2, 1107870150, SNOActor.g_Portal_Square_Blue),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar,SNOWorld.trOut_Oldtistram_Cellar_2, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, "trOut_oldTristram_Cellar_E", new Vector3(124.3548f, 113.0747f, 0.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, "trOut_oldTristram_Cellar_E", new Vector3(139.9767f, 131.7817f, 0.1000038f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, "trOut_oldTristram_Cellar_E", new Vector3(103.9924f, 132.6982f, 0.1000038f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, "trOut_oldTristram_Cellar_E", new Vector3(132.0145f, 145.1403f, 0.1000002f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, "trOut_oldTristram_Cellar_E", new Vector3(128.9004f, 132.4343f, 0.1000037f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, SNOWorld.trOut_Oldtistram_Cellar_2, "trOut_oldTristram_Cellar_E", new Vector3(124.0268f, 106.3538f, 0.1f)),
						
 //                       new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 50)
                    }
            });

            // A5 - The Cursed City (SNOQuest.X1_Bounty_A5_Bog_Event_AncientEvils)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Event_AncientEvils,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_FakeEntrance_04,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Bog_Event_AncientEvils, SNOWorld.x1_Bog_01, SNOWorld.x1_Catacombs_FakeEntrance_04, 1344182686, 0),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A5_Bog_Event_AncientEvils,SNOWorld.x1_Catacombs_FakeEntrance_04, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A5_Bog_Event_AncientEvils,SNOWorld.x1_Catacombs_FakeEntrance_04, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A5_Bog_Event_AncientEvils, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A1 - The Cursed Court (SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level04,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, "trDun_Cath_SW_Hall_01", new Vector3(142.7451f, 127.8218f, -24.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, "trDun_Cath_SW_Hall_01", new Vector3(157.5084f, 141.0096f, -22.92235f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, "trDun_Cath_SW_Hall_01", new Vector3(144.483f, 157.0942f, -22.67825f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, "trDun_Cath_SW_Hall_01", new Vector3(128.2968f, 141.8916f, -23.27051f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, "trDun_Cath_SW_Hall_01", new Vector3(144.0414f, 142.2489f, -22.926f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, "trDun_Cath_SW_Hall_01", new Vector3(138.0294f, 129.1142f, -24.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, SNOWorld.a1trDun_Level04, 10000),

 //                     new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A1 - The Cursed Chamber of Bone (SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level01,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 2912417),

                        //new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(96.61365f, 143.3563f, 0.1000008f)),

                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone,SNOWorld.a1trDun_Level01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(78.06195f, 118.1667f, 1.457689f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(100.1196f, 115.9618f, 2.645245f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(122.5316f, 116.9459f, 1.237238f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(97.40033f, 117.3986f, 2.177974f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(100.0737f, 137.4675f, 1.262186f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(99.34674f, 116.4901f, 2.515652f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(78.06195f, 118.1667f, 1.457689f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(100.1196f, 115.9618f, 2.645245f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(122.5316f, 116.9459f, 1.237238f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(97.40033f, 117.3986f, 2.177974f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(100.0737f, 137.4675f, 1.262186f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(99.34674f, 116.4901f, 2.515652f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, "trDun_Cath_NSW_01", new Vector3(100.7628f, 147.187f, 0.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, SNOWorld.a1trDun_Level01, 5000),
						
//                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, 90, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30, false)
                    }
            });

            // A1 - The Cursed Hatchery (SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap,
                Act = Act.A1,
                WorldId = SNOWorld.a1Dun_SpiderCave_01,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap,SNOWorld.a1Dun_SpiderCave_01, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap,SNOWorld.a1Dun_SpiderCave_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
						
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(61.84448f, 139.3744f, 0.1000021f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(80.45703f, 148.389f, 0.1000021f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(63.43884f, 164.4423f, 0.1000021f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(43.03125f, 147.7166f, 0.1000002f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(59.40161f, 138.2727f, 0.100002f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(77.58911f, 149.9207f, 0.1000021f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(62.28101f, 160.7621f, 0.1000021f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(45.87036f, 144.7222f, 0.1000002f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, "a2dun_Spider_NSEW_04", new Vector3(60.56909f, 136.0889f, 0.1000021f)),
						new WaitCoroutine(SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, SNOWorld.a1Dun_SpiderCave_01, 10000),
						
                    //new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    
                }
            });

            // A2 - The Cursed Spire (SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level01,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level01, -1363317799, SNOActor.g_Portal_Circle_Orange_Bright),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery,SNOWorld.a2Dun_Zolt_Level01, 2912417),

                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery,SNOWorld.a2Dun_Zolt_Level01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, "a2dun_Zolt_Hall_EW_02", new Vector3(82.68777f, 98.2832f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, "a2dun_Zolt_Hall_EW_02", new Vector3(60.56305f, 100.6702f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, "a2dun_Zolt_Hall_EW_02", new Vector3(78.98157f, 120.9982f, 0.1000008f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, "a2dun_Zolt_Hall_EW_02", new Vector3(102.0769f, 103.844f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, "a2dun_Zolt_Hall_EW_02", new Vector3(81.83029f, 117.8888f, 0.1839012f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, "a2dun_Zolt_Hall_EW_02", new Vector3(64.01669f, 104.2493f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, SNOWorld.a2Dun_Zolt_Level01, "a2dun_Zolt_Hall_EW_02", new Vector3(81.71918f, 96.44299f, 0.1000001f)),
						
//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A2 - The Cursed Pit (SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level02,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level02, -1363317798, SNOActor.g_Portal_Circle_Orange_Bright),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl,SNOWorld.a2Dun_Zolt_Level02, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, "a2dun_Zolt_Hall_EW_02", new Vector3(89.11694f, 97.45593f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, "a2dun_Zolt_Hall_EW_02", new Vector3(90.1842f, 81.74347f, 0.1250016f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, "a2dun_Zolt_Hall_EW_02", new Vector3(114.6979f, 99.00391f, 0.1000002f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, "a2dun_Zolt_Hall_EW_02", new Vector3(89.98718f, 116.8755f, 0.1000004f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, "a2dun_Zolt_Hall_EW_02", new Vector3(62.31152f, 97.84348f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, "a2dun_Zolt_Hall_EW_02", new Vector3(91.53711f, 99.19025f, 0.1000001f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, SNOWorld.a2Dun_Zolt_Level02, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A5 - The Cursed Forum (SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 2912417, true),
                        new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06"),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(115.2922f, 106.7599f, 5.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(120.9968f, 91.24005f, 5.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(147.4745f, 110.2294f, 5.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(106.0229f, 141.2579f, 5.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(115.2922f, 106.7599f, 5.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(106.0229f, 141.2579f, 5.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_06", new Vector3(115.2922f, 106.7599f, 5.1f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, SNOWorld.X1_WESTM_ZONE_01, 10000),
						
//                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30, false),
                    }
            });

            // A5 - The Cursed War Room (SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins,SNOWorld.x1_fortress_level_01,1),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 2912417),
                        new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(124.3028f, 116.0892f, -19.25122f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(137.4039f, 121.5612f, -19.25122f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(126.3025f, 136.8546f, -19.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(105.034f, 120.4681f, -19.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(124.6398f, 115.7779f, -19.25122f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(145.7731f, 116.5219f, -19.25122f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(129.1796f, 134.8818f, -19.43344f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(102.9052f, 117.5618f, -19.9f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
                        new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_02", new Vector3(125.6252f, 112.6357f, -19.25122f)),
                        new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, SNOWorld.x1_fortress_level_01, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A5 - The Cursed Peat (SNOQuest.X1_Bounty_A5_Bog_Event_HunterKillers)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Event_HunterKillers,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A5_Bog_Event_HunterKillers, SNOWorld.x1_Bog_01, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A5_Bog_Event_HunterKillers,SNOWorld.x1_Bog_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A5_Bog_Event_HunterKillers, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A5 - The Cursed Bone Pit (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Abattoir_Random02_B,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_Abattoir_Random02, -660641888, SNOActor.g_Portal_Rectangle_Blue_Westmarch),
 //                       new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit, SNOWorld.X1_Abattoir_Random02, SNOWorld.X1_Abattoir_Random02_B, 2115492897, SNOActor.g_Portal_Rectangle_Orange, true),
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit, SNOWorld.X1_Abattoir_Random02, SNOWorld.X1_Abattoir_Random02_B, 2115492897, 0),
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit, SNOWorld.X1_Abattoir_Random02_B, 2912417),
                        new InteractWithGizmoCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit,SNOWorld.X1_Abattoir_Random02_B, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30),
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                    }
            });

            // A4 - The Cursed Pulpit (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Sigil_C,
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = SNOLevelArea.A4_dun_Garden_of_Hope_B,
                Coroutines = new List<ISubroutine>
                    {

                        new MoveToMapMarkerCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Garden_of_Hope_Random_B, -970799629),
						
//						new InteractWithGizmoCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Garden_of_Hope_Random_B, SNOActor.g_portal_Ladder_Short_Blue_largeRadius, -970799629, 5),
						new EnterLevelAreaCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Garden_of_Hope_Random_B, SNOWorld.a4dun_Sigil_C, -970799629, SNOActor.g_portal_Ladder_Short_Blue_largeRadius),

                        new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_C, new Vector3(349, 557, 0)),
                        new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_C, new Vector3(348, 421, -14)),
                        new MoveToPositionCoroutine(SNOWorld.a4dun_Sigil_C, new Vector3(341, 351, -12)),

                        new InteractWithGizmoCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(111.6542f, 110.5945f, -12.64518f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(67.57697f, 111.5117f, -14.9f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(119.326f, 77.41565f, -14.66502f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(161.381f, 120.7327f, -14.9f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(108.6663f, 148.4656f, -14.9f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(73.42627f, 108.64f, -14.9f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(119.326f, 77.41565f, -14.66502f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(161.381f, 120.7327f, -14.9f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(108.6663f, 148.4656f, -14.9f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
                        new MoveToScenePositionCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, SNOWorld.a4dun_Sigil_C, "a4dun_spire_SigilRoom_C", new Vector3(73.42627f, 108.64f, -14.9f)),
                        new WaitCoroutine(SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC,SNOWorld.a4dun_Sigil_C, 5000),
						
//                     new ClearAreaForNSecondsCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, 80, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30, false)
                    }
            });
        }

        private static void AddBounties()
        {

            // A5 - Bounty: Tollifer's Last Stand (SNOQuest.X1_Bounty_A5_Westmarch_Event_TollifersLastStand)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_TollifersLastStand,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.X1_WESTM_ZONE_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TollifersLastStand, SNOWorld.X1_WESTM_ZONE_01, 2912417, true),
//					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TollifersLastStand, SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_SkeletonArcher_Westmarch_CorpseSpawn_09),
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TollifersLastStand, SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_SkeletonArcher_Westmarch_CorpseSpawn_09, 2912417, 5),

					new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TollifersLastStand, 60, SNOActor.x1_SkeletonArcher_Westmarch_CorpseSpawn_09, 2912417, 30),
				}
            });

            // A5 - Bounty: Pest Problems (SNOQuest.X1_Bounty_A5_WestmarchFire_Event_PestProblems)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Event_PestProblems,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_03, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
//					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_PestProblems, SNOWorld.X1_WESTM_ZONE_03, -1883144025),
					new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_PestProblems, SNOWorld.X1_WESTM_ZONE_03, 0, -1883144025, SNOActor.g_portal_RandomWestm),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_PestProblems),
                }
            });

            // A5 - Bounty: Kill Hed Monh Ton (SNOQuest.P4_Bounty_A5_ForestCoast_Kill_Unique_HedMonTon)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_Kill_Unique_HedMonTon,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.x1_P2_Forest_Coast_Level_01,
                Coroutines = new List<ISubroutine>
                {
                    new KillUniqueMonsterCoroutine (SNOQuest.P4_Bounty_A5_ForestCoast_Kill_Unique_HedMonTon, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.P4_Sasquatch_B_Unique_01, 0),
                    new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Kill_Unique_HedMonTon),
                }
            });

            // A1 - Clear the Cave of the Moon Clan (SNOQuest.X1_Bounty_A1_Highlands_Clear_CaveMoonClan2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Clear_CaveMoonClan2,
                Act = Act.A1,
                WorldId = SNOWorld.A1_Cave_Highlands_GoatCaveA_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Clear_CaveMoonClan2, SNOWorld.trOUT_Town, -1187439574, true),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Clear_CaveMoonClan2, SNOWorld.trOUT_Town, 0, -1187439574, SNOActor.g_Portal_Square_Orange),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Clear_CaveMoonClan2, SNOWorld.A1_Cave_Highlands_GoatCaveA_Level01, SNOWorld.A1_Cave_Highlands_GoatCaveA_Level02, -1187439573, SNOActor.g_Portal_Oval_Orange, true),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Clear_CaveMoonClan2)
                    }
            });

            // A1 - Kill Logrut the Warrior (SNOQuest.X1_Bounty_A1_Highlands_Kill_Logrut)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Kill_Logrut,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands_Bridge, SNOLevelArea.A1_trOUT_Highlands },
                Coroutines = new List<ISubroutine>
                    {
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Kill_Logrut, SNOWorld.trOUT_Town, -373794395, true),
						
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2382, 4438, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2087, 4448, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1879, 4437, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2026, 4195, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2010, 4024, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2238, 3962, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2514, 3959, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2463, 4186, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2377, 4399, -1)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2240, 4271, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2182, 4090, 0)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_Logrut, SNOWorld.trOUT_Town, SNOActor.Goatman_Melee_B_Unique_01, -373794395),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_Logrut)
                    }
            });

            // A1 - Kill Buras the Impaler (SNOQuest.X1_Bounty_A1_Highlands_Kill_Buras) 
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Kill_Buras,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands_Bridge, SNOLevelArea.A1_trOUT_Highlands },
                Coroutines = new List<ISubroutine>
                    {
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Kill_Buras, SNOWorld.trOUT_Town, 1205746285, true),
						
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2382, 4438, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2087, 4448, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1879, 4437, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2026, 4195, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2010, 4024, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2238, 3962, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2514, 3959, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2463, 4186, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2377, 4399, -1)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2240, 4271, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2182, 4090, 0)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_Buras, SNOWorld.trOUT_Town, SNOActor.Goatman_Ranged_A_Unique_01, 1205746285),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_Buras )
                    }
            });

            // A1 - Kill Lorzak the Powerful (SNOQuest.X1_Bounty_A1_Highlands_Kill_Lorzak)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Kill_Lorzak,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands_Bridge, SNOLevelArea.A1_trOUT_Highlands },
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2382, 4438, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2087, 4448, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1879, 4437, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2026, 4195, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2010, 4024, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2238, 3962, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2514, 3959, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2463, 4186, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2377, 4399, -1)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2240, 4271, 0)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(2182, 4090, 0)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_Lorzak, SNOWorld.trOUT_Town, SNOActor.Goatman_Shaman_A_Unique_01, -1684494028),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_Lorzak )
                    }
            });

            // A1 - Kill Red Rock (SNOQuest.X1_Bounty_A1_Highlands_Kill_RedRock)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Kill_RedRock,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands_Bridge, SNOLevelArea.A1_trOUT_Highlands },
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Kill_RedRock, SNOWorld.trOUT_Town, 2067238310, true),
	
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_RedRock, SNOWorld.trOUT_Town, SNOActor.Beast_A_Unique_02, 2067238310),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Kill_RedRock)
                    }
            });
            
            // A1 - Clear the Den of the Fallen (SNOQuest.X1_Bounty_A1_Wilderness_Clear_DenOfTheFallen2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Wilderness_Clear_DenOfTheFallen2,
                Act = Act.A1,
                WorldId = SNOWorld.A1_Cave_Wilderness_Den_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                       new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Clear_DenOfTheFallen2, SNOWorld.trOUT_Town, -2043651426, true),
						
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Clear_DenOfTheFallen2, SNOWorld.trOUT_Town, 0, -2043651426, SNOActor.g_Portal_Circle_Blue),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Clear_DenOfTheFallen2, SNOWorld.A1_Cave_Wilderness_Den_Level01, SNOWorld.A1_Cave_Wilderness_Den_Level02, -711350153, SNOActor.g_Portal_Oval_Orange, true),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Clear_DenOfTheFallen2)
                    }
            });

            // A1 - Kill Mange (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Mange)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Wilderness_Kill_Mange,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Mange,SNOWorld.trOUT_Town, SNOActor.Scavenger_A_Unique_01, -275004780),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Mange)
                    }
            });

            // A1 - Clear the Scavenger's Den (SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2) 
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2,
                Act = Act.A1,
                WorldId = SNOWorld.A1_Cave_Fields_ScavengerDen_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2, SNOWorld.trOUT_Town, 925091454, true),
						
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2, SNOWorld.trOUT_Town, SNOWorld.A1_Cave_Fields_ScavengerDen_Level01, 925091454, SNOActor.g_Portal_Circle_Orange),
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2, SNOWorld.A1_Cave_Fields_ScavengerDen_Level01, 925091455),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2, SNOWorld.A1_Cave_Fields_ScavengerDen_Level01, SNOWorld.A1_Cave_Fields_ScavengerDen_Level02, 925091455, SNOActor.g_Portal_Oval_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2)
                    }
            });

            // A1 - Kill Melmak (SNOQuest.X1_Bounty_A1_Fields_Kill_Melmak)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Kill_Melmak,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Melmak,SNOWorld.trOUT_Town, SNOActor.Goatman_Melee_A_Unique_01, 1683727460),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Melmak)
                    }
            });

            // A1 - Kill Grimsmack (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Grimshack)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Grimshack,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOut_TristramFields_B },
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Grimshack, SNOWorld.trOUT_Town, 1737003285, false),	
						
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(745, 866, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(868, 780, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(713, 683, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(543, 590, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(234, 551, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(177, 728, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(188, 900, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(421, 951, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(615, 979, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(460, 848, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(357, 732, 20)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Grimshack, SNOWorld.trOUT_Town, SNOActor.Ghoul_A_Unique_01, 1737003285),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Grimshack)
                    }
            });

            // A1 - Clear the Crypt of the Ancients (SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_CryptOfTheAncients)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_CryptOfTheAncients,
                Act = Act.A1,
                WorldId = SNOWorld.trdun_Cave_Nephalem_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_CryptOfTheAncients, SNOWorld.trOUT_Town, SNOWorld.trdun_Cave_Nephalem_02, 976523526, SNOActor.g_Portal_Square_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_CryptOfTheAncients)
                    }
            });

            // A1 - Clear Warrior's Rest (SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_WarriorsRest)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_WarriorsRest,
                Act = Act.A1,
                WorldId = SNOWorld.trdun_Cave_Nephalem_01,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_WarriorsRest, SNOWorld.trOUT_Town, 976523525, true),
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_WarriorsRest, SNOWorld.trOUT_Town, 0, 976523525, SNOActor.g_Portal_Square_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_WarriorsRest)
                    }
            });

            // A1 - Kill Zhelobb the Venomous (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Zhelobb)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Zhelobb,
                Act = Act.A1,
                WorldId = SNOWorld.a1Dun_SpiderCave_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Zhelobb,SNOWorld.a1Dun_SpiderCave_01, SNOActor.Spider_A_Unique_01, 1307894141),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Zhelobb)
                    }
            });

            // A1 - Kill Venimite (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Venimite)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Venimite,
                Act = Act.A1,
                WorldId = SNOWorld.a1Dun_SpiderCave_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Venimite,SNOWorld.a1Dun_SpiderCave_01, SNOActor.Spiderling_A_Unique_01, -1487666649),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Venimite)
                    }
            });

            // A1 - Kill Rathlin the Widowmaker (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Rathlin)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Rathlin,
                Act = Act.A1,
                WorldId = SNOWorld.a1Dun_SpiderCave_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Rathlin,SNOWorld.a1Dun_SpiderCave_01, SNOActor.Spider_Poison_A_Unique_01, 1425915828),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Rathlin)
                    }
            });

            // A1 - Kill Crassus the Tormentor (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Crassus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Crassus,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Crassus,SNOWorld.trDun_Leoric_Level01, SNOActor.TriuneCultist_A_Unique_02, -11732714),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Crassus)
                    }
            });

            // A1 - Kill Bludgeonskull  (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Bludgeonskull)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Bludgeonskull,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Bludgeonskull,SNOWorld.trDun_Leoric_Level01, SNOActor.Triune_Berserker_A_Unique_01, -820238703),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Bludgeonskull)
                    }
            });

            // A1 - Kill Qurash the Reviled (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Qurash)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Qurash,
                Act = Act.A1,
                WorldId = SNOWorld.a1Dun_SpiderCave_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Qurash,SNOWorld.a1Dun_SpiderCave_01, SNOActor.Spider_Poison_A_Unique_02, 1425915829),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Qurash)
                    }
            });

            // A2 - Kill Saha the Slasher (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Saha)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Saha,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2655, 1531, 187)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2517, 1602, 186)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(2400, 1549, 203)),

						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Saha, SNOWorld.caOUT_Town, -1258389668),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Saha, SNOWorld.caOUT_Town, SNOActor.LacuniFemale_A_Unique_01, -1258389668),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Saha)
                    }
            });
            
            // A2 - Kill Shondar the Invoker (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Shondar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Shondar,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Shondar,SNOWorld.caOUT_Town, SNOActor.TriuneSummoner_C_Unique_01, 898630437),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Shondar)
                    }
            });

            // A2 - Clear the Ancient Cave (SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2,
                Act = Act.A2,
                WorldId = SNOWorld.a2trDun_Cave_Oasis_Random01_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2, SNOWorld.caOUT_Town, 1352061372),
//                     new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2, SNOWorld.caOUT_Town, SNOWorld.a2trDun_Cave_Oasis_Random01, 1352061372, SNOActor.g_Portal_Circle_Blue),

						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2, SNOWorld.caOUT_Town, 0, 1352061372, SNOActor.g_Portal_Circle_Blue),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2, SNOWorld.a2trDun_Cave_Oasis_Random01, SNOWorld.a2trDun_Cave_Oasis_Random01_Level02, 622615957, SNOActor.g_Portal_ArchTall_Blue),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2)
                    }
            });

            // A2 - Clear the Flooded Cave (SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2,
                Act = Act.A2,
                WorldId = SNOWorld.a2trDun_Cave_Oasis_Random02_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2, SNOWorld.caOUT_Town, 1352061373),
//                      new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2, SNOWorld.caOUT_Town, SNOWorld.a2trDun_Cave_Oasis_Random02, 1352061373, SNOActor.g_Portal_Circle_Blue),
						// g_Portal_Circle_Blue (SNOActor.g_Portal_Circle_Blue) Distance: 12.11661
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2, SNOWorld.caOUT_Town, SNOWorld.a2trDun_Cave_Oasis_Random02, 1352061373, SNOActor.g_Portal_Circle_Blue),
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2, SNOWorld.a2trDun_Cave_Oasis_Random02, -1718038890, true),
						new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A2_trDun_Cave_Oasis_Random02 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A2_trDun_Cave_Oasis_Random02 || BountyHelpers.ScanForMarker(-1718038890, 50) != null)),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2, SNOWorld.a2trDun_Cave_Oasis_Random02, 0, -1718038890, SNOActor.g_Portal_ArchTall_Blue),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2)
                    }
            });

            // A2 - Kill Scar Talon (SNOQuest.X1_Bounty_A2_Oasis_Kill_ScarTalon)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Kill_ScarTalon,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Kill_ScarTalon, SNOWorld.caOUT_Town, 865852689, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_ScarTalon, SNOWorld.caOUT_Town, SNOActor.Bloodhawk_A_Unique_01, 865852689),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_ScarTalon)
                    }
            });

            // A2 - Clear the Cave of Burrowing Horror (SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Boneyard_Worm_Cave_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2, SNOWorld.caOUT_Town, 1028158260, true),
                        new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2, SNOWorld.caOUT_Town, "caOut_Boneyard_Sub80_WormCaveEntrance_01"),
                        new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2, 20, 0, 1028158260, 20),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2, SNOWorld.caOUT_Town, SNOWorld.a2Dun_Boneyard_Worm_Cave_01, 1028158260, SNOActor.g_Portal_Circle_Orange),
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2, SNOWorld.a2Dun_Boneyard_Worm_Cave_01, 1028158261, true),						
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2, SNOWorld.a2Dun_Boneyard_Worm_Cave_01, 0, 1028158261, SNOActor.g_Portal_ArchTall_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2)
                    }
            });

            // A2 - Kill Raiha (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Raiha)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Kill_Raiha,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Raiha, SNOWorld.caOUT_Town, 979055773, false),	
						
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(219, 3829, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(426, 3968, 108)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1006, 4071, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1212, 3940, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1118, 3814, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(906, 3917, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(485, 3768, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(365, 3688, 97)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Raiha, SNOWorld.caOUT_Town, SNOActor.LacuniFemale_B_Unique_01, 979055773),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Raiha)
                    }
            });

            // A2 - Kill Blarg (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Blarg) 
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Kill_Blarg,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Blarg, SNOWorld.caOUT_Town, 1108776135, false),	
						
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(219, 3829, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(426, 3968, 108)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1006, 4071, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1212, 3940, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1118, 3814, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(906, 3917, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(485, 3768, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(365, 3688, 97)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Blarg, SNOWorld.caOUT_Town, SNOActor.Sandling_B_Unique_01, 1108776135),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Blarg)
                    }
            });

            // A2 - Kill Bloodfeather (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Bloodfeather)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Kill_Bloodfeather,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Boneyards_Kill_Bloodfeather, SNOWorld.caOUT_Town, 865852690, false),	
						
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(219, 3829, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(426, 3968, 108)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1006, 4071, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1212, 3940, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1118, 3814, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(906, 3917, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(485, 3768, 110)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(365, 3688, 97)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Bloodfeather, SNOWorld.caOUT_Town, SNOActor.Bloodhawk_A_Unique_02, 865852690),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Bloodfeather)
                    }
            });

            // A2 - Kill the Archivist (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheArchivist)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheArchivist,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheArchivist, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level01, -1363317799, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheArchivist,SNOWorld.a2Dun_Zolt_Level01, SNOActor.Ghost_D_Unique01, -1954248257),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheArchivist)
                    }
            });
            
            // A2 - Kill Hellscream (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Hellscream)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Hellscream,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Hellscream, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level01, -1363317799, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Hellscream,SNOWorld.a2Dun_Zolt_Level01, SNOActor.FleshPitFlyer_C_Unique_02, -1743829606),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Hellscream)
                    }
            });

            // A2 - Kill Thrum (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thrum)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thrum,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_ShadowRealm_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thrum, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_ShadowRealm_Level01, 1867081263, SNOActor.g_Portal_Circle_Zolt),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thrum,SNOWorld.a2Dun_Zolt_ShadowRealm_Level01, SNOActor.sandMonster_C_Unique_01, 221109478),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thrum)
                    }
            });

            // A2 - Kill the Tomekeeper (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheTomekeeper) 
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheTomekeeper,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_ShadowRealm_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheTomekeeper, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_ShadowRealm_Level01, 1867081263, SNOActor.g_Portal_Circle_Zolt),
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheTomekeeper, SNOWorld.a2Dun_Zolt_ShadowRealm_Level01, -65631842, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheTomekeeper ,SNOWorld.a2Dun_Zolt_ShadowRealm_Level01, SNOActor.Ghost_D_Unique_01, -65631842),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheTomekeeper)
                    }
            });

            // A2 - Kill Mage Lord Skomara (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordSkomora)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordSkomora,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordSkomora, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level02, -1363317798, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordSkomora,SNOWorld.a2Dun_Zolt_Level02, SNOActor.skeletonMage_Cold_B_Unique_01, 1133968439),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordSkomora)
                    }
            });

            // A2 - Kill Mage Lord Ghuyan (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordGhuyan)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordGhuyan,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordGhuyan, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level02, -1363317798, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordGhuyan,SNOWorld.a2Dun_Zolt_Level02, SNOActor.skeletonMage_Fire_B_Unique_01, 1198145179),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordGhuyan)
                    }
            });

            // A3 - Kill Bricktop (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bricktop)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bricktop,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_rmpt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bricktop,SNOWorld.a3Dun_rmpt_Level02, SNOActor.demonTrooper_A_Unique_03, 154606422),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bricktop)
                    }
            });

            // A3 - Kill Bashface the Truncheon (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bashface)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bashface,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_rmpt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bashface, SNOWorld.a3Dun_rmpt_Level02, 154606421, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bashface, SNOWorld.a3Dun_rmpt_Level02, SNOActor.demonTrooper_A_Unique_02, 154606421),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bashface)
                    }
            });

            // A3 - Kill Marchocyas (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Marchocyas)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Marchocyas,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_rmpt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Marchocyas, SNOWorld.a3Dun_rmpt_Level02, SNOWorld.a3dun_rmpt_Level01, -1078336204, SNOActor.g_Portal_ArchTall_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Marchocyas,SNOWorld.a3dun_rmpt_Level01, SNOActor.demonFlyer_A_Unique_01, 1020521099),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Marchocyas)
                    }
            });

            // A3 - Kill Thornback (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Thornback)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Thornback,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Thornback,SNOWorld.a3Dun_Keep_Level03, SNOActor.QuillDemon_C_Unique_01, -1497339214),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Thornback)
                    }
            });

            // A3 - Kill Captain Donn Adams (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDonnAdams)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDonnAdams,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDonnAdams,SNOWorld.a3Dun_Keep_Level03, SNOActor.Shield_Skeleton_E_Unique_02, 207753752),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDonnAdams)
                    }
            });

            // A3 - Kill Captain Dale (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDale)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDale,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDale,SNOWorld.a3Dun_Keep_Level03, SNOActor.Shield_Skeleton_E_Unique_01, 207753751),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDale)
                    }
            });

            // A3 - Kill the Crusher (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_TheCrusher)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_TheCrusher,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level04,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_TheCrusher,SNOWorld.a3Dun_Keep_Level04, SNOActor.demonTrooper_B_Unique_01, -1902915435),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_TheCrusher)
                    }
            });

            // A3 - Kill Belagg Pierceflesh (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Belagg)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Belagg,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level05,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Belagg,SNOWorld.a3Dun_Keep_Level05, SNOActor.SkeletonArcher_E_Unique_01, -650015436),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Belagg)
                    }
            });

            // A3 - Kill Gugyn the Gauntlet (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Gugyn)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Gugyn,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level05,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Gugyn,SNOWorld.a3Dun_Keep_Level05, SNOActor.Brickhouse_A_Unique_02, 681588934),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Gugyn)
                    }
            });

            // A3 - Clear the Caverns of Frost (SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,//SNOWorld.a3dun_IceCaves_Random_01_Level_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2, SNOWorld.A3_Battlefields_02, 1029056444, true),
//                     new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2, SNOWorld.A3_Battlefields_02, SNOWorld.a3dun_IceCaves_Random_01, 1029056444, SNOActor.g_Portal_Circle_Blue),
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2, SNOWorld.A3_Battlefields_02, 0, 1029056444, SNOActor.g_Portal_Circle_Blue),
						new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A3_dun_IceCaves_Random_01 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A3_dun_IceCaves_Random_01 || BountyHelpers.ScanForMarker(151580180, 50) != null)),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2, SNOWorld.a3dun_IceCaves_Random_01, SNOWorld.A3_Battlefields_02, 151580180, SNOActor.g_Portal_Oval_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2)
                    }
            });

            // A3 - Clear the Icefall Caves (SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_IceCaves_Timed_01_Level_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2, SNOWorld.A3_Battlefields_02, 942020622, true),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2, SNOWorld.A3_Battlefields_02, SNOWorld.a3dun_IceCaves_Timed_01, 942020622, SNOActor.g_Portal_Circle_Blue),
						new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A3_dun_IceCaves_Timed_01 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A3_dun_IceCaves_Timed_01 || BountyHelpers.ScanForMarker(-802596186, 50) != null)),
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2, SNOWorld.a3dun_IceCaves_Timed_01, -802596186, true),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2, SNOWorld.a3dun_IceCaves_Timed_01, SNOWorld.a3dun_IceCaves_Timed_01_Level_02, -802596186, SNOActor.g_Portal_Oval_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2)
                    }
            });

            // A3 - Kill Groak the Brawler (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Groak)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Groak,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Groak,SNOWorld.A3_Battlefields_02, SNOActor.GoatMutant_Melee_A_Unique_01, 188457921),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Groak)
                    }
            });

            // A3 - Kill Mehshak the Abomination (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Mehshak)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Mehshak,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Mehshak,SNOWorld.A3_Battlefields_02, SNOActor.fastMummy_C_Unique_01, 1668992859),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Mehshak)
                    }
            });

            // A3 - Kill Shertik the Brute (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Shertik)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Shertik,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Shertik,SNOWorld.A3_Battlefields_02, SNOActor.GoatMutant_Melee_A_Unique_03, 188457923),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Shertik)
                    }
            });

            // A3 - Kill Emberwing (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Emberwing)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Emberwing,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        // todo needs work, failing.as
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Emberwing,SNOWorld.A3_Battlefields_02, SNOActor.demonFlyer_B_Unique_01, -1037000756),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Emberwing)
                    }
            });

            // A3 - Kill Garganug (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Garganug)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Garganug,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A3_Bridge_01 },
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2461, 649, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2270, 604, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1982, 607, -24)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1811, 612, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1435, 623, -24)),
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1047, 613, -24)),
						
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Garganug, SNOWorld.A3_Battlefields_02, -838403687),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Garganug, SNOWorld.A3_Battlefields_02, SNOActor.ThousandPounder_Unique_01, -838403687),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Garganug)
                    }
            });

            // A3 - Kill Hyrug the Malformed (SNOQuest.X1_Bounty_A3_Crater_Kill_Hyrug)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Hyrug,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level01B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Hyrug,SNOWorld.a3dun_Crater_ST_Level01B, SNOActor.GoatMutant_Melee_B_Unique_01, -1869063934),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Hyrug)
                    }
            });

            // A3 - Kill Maggrus the Savage (SNOQuest.X1_Bounty_A3_Crater_Kill_Maggrus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Maggrus,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level02B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Maggrus, SNOWorld.a3dun_Crater_ST_Level01B, SNOWorld.a3dun_Crater_ST_Level02B, 43541819, SNOActor.g_Portal_ArchTall_Orange),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Maggrus,SNOWorld.a3dun_Crater_ST_Level02B, SNOActor.GoatMutant_Melee_B_Unique_02, -1869063933),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Maggrus)
                    }
            });

            // A3 - Kill Charuch the Spear (SNOQuest.X1_Bounty_A3_Crater_Kill_Charuch)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Charuch,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Charuch,SNOWorld.a3Dun_Crater_Level_02, SNOActor.GoatMutant_Ranged_B_Unique_01, 1343937195),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Charuch)
                    }
            });

            // A3 - Kill Mhawgann the Unholy (SNOQuest.X1_Bounty_A3_Crater_Kill_Mhawgann)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Mhawgann,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Mhawgann,SNOWorld.a3Dun_Crater_Level_02, SNOActor.GoatMutant_Shaman_B_Unique_01, -1546303118),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Mhawgann)
                    }
            });

            // A3 - Kill Severclaw  (SNOQuest.X1_Bounty_A3_Crater_Kill_Severclaw)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Severclaw,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Severclaw,SNOWorld.a3Dun_Crater_Level_01, SNOActor.Monstrosity_Scorpion_A_Unique_01, -1740074819),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Severclaw)
                    }
            });

            // A3 - Kill Valifahr the Noxious (SNOQuest.X1_Bounty_A3_Crater_Kill_Valifahr)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Valifahr,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_01,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A3_Dun_Crater_Level_01,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Valifahr,SNOWorld.a3Dun_Crater_Level_01, SNOActor.creepMob_A_Unique_02, 1965678404),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Valifahr)
                    }
            });

            // A3 - Kill Demonika the Wicked (SNOQuest.X1_Bounty_A3_Crater_Kill_Demonika)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Demonika,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Demonika,SNOWorld.a3dun_Crater_ST_Level01, SNOActor.Succubus_A_Unique_02, 236772676),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Demonika)
                    }
            });

            // A3 - Kill Axgore the Cleaver (SNOQuest.X1_Bounty_A3_Crater_Kill_Axgore)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Axgore,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Axgore,SNOWorld.a3Dun_Crater_Level_03, SNOActor.azmodanBodyguard_A_Unique_03, -1180611613),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Axgore)
                    }
            });

            // A3 - Kill Brimstone (SNOQuest.X1_Bounty_A3_Crater_Kill_Brimstone)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Brimstone,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Brimstone, SNOWorld.a3dun_Crater_ST_Level01, SNOWorld.a3dun_Crater_ST_Level02, 2083727833, SNOActor.g_Portal_ArchTall_Orange),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Brimstone,SNOWorld.a3dun_Crater_ST_Level02, SNOActor.azmodanBodyguard_A_Unique_02, -1180611614),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Brimstone)
                    }
            });

            // A1 - Kill Battlerage the Plagued (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Battlerage)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Battlerage,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Battlerage,SNOWorld.trDun_Leoric_Level01, SNOActor.Triune_Berserker_A_Unique_02, -820238702),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Battlerage)
                    }
            });

            // A1 - Kill Treefist Woodhead (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Treefist)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Treefist,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Treefist,SNOWorld.trDun_Leoric_Level03, SNOActor.Triune_Berserker_A_Unique_03, -820238701),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Treefist)
                    }
            });

            // A1 - Kill Boneslag the Berserker (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Boneslag)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Boneslag,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Boneslag,SNOWorld.trDun_Leoric_Level03, SNOActor.Triune_Berserker_A_Unique_04, -820238700),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Boneslag)
                    }
            });

            // A1 - Kill Hawthorne Gable (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_HawthorneGable)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_HawthorneGable,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_HawthorneGable,SNOWorld.trOUT_Town, SNOActor.Ghost_A_Unique_02, 1811966428),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_HawthorneGable)
                    }
            });

            // A1 - Kill Fecklar's Ghost (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_FecklarsGhost)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_FecklarsGhost,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_FecklarsGhost,SNOWorld.trOUT_Town, SNOActor.Ghost_A_Unique_01, 1811966427),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_FecklarsGhost)
                    }
            });

            // A1 - Clear Khazra Den (SNOQuest.X1_Bounty_A1_Fields_Clear_KhazraDen)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Clear_KhazraDen,
                Act = Act.A1,
                WorldId = SNOWorld.Fields_Cave_SwordOfJustice_Level01,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Clear_KhazraDen, SNOWorld.trOUT_Town, 2036518712),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Fields_Clear_KhazraDen, SNOWorld.trOUT_Town, SNOWorld.Fields_Cave_SwordOfJustice_Level01, 2036518712, SNOActor.g_Portal_Circle_Blue),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Clear_KhazraDen)
                    }
            });

            // A1 - Kill Charger (SNOQuest.X1_Bounty_A1_Fields_Kill_Charger)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Kill_Charger,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Charger,SNOWorld.trOUT_Town, SNOActor.Beast_A_Unique_01, 2067238309),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Charger)
                    }
            });

            // A1 - Kill Dreadclaw the Leaper (SNOQuest.X1_Bounty_A1_Fields_Kill_Dreadclaw)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Kill_Dreadclaw,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Dreadclaw, SNOWorld.trOUT_Town, 1962440661, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Dreadclaw, SNOWorld.trOUT_Town, SNOActor.Scavenger_B_Unique_01, 1962440661),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Dreadclaw)
                    }
            });

            // A1 - Kill the Dataminer (SNOQuest.X1_Bounty_A1_Cemetery_Kill_Dataminer)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cemetery_Kill_Dataminer,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Crypt_FalsePassage_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_Dataminer, SNOWorld.trOUT_Town, SNOWorld.trDun_Crypt_FalsePassage_01, -1965109038, SNOActor.g_Portal_ArchTall_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_Dataminer,SNOWorld.trDun_Crypt_FalsePassage_01, SNOActor.graveDigger_B_Ghost_Unique_01, -504552226),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_Dataminer)
                    }
            });

            // A1 - Kill Digger O'Dell (SNOQuest.X1_Bounty_A1_Cemetery_Kill_DiggerODell)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cemetery_Kill_DiggerODell,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Crypt_FalsePassage_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_DiggerODell, SNOWorld.trOUT_Town, 0, -1965109037, SNOActor.g_Portal_ArchTall_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_DiggerODell,SNOWorld.trDun_Crypt_FalsePassage_02, SNOActor.graveDigger_B_Ghost_Unique, -690325730),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cemetery_Kill_DiggerODell)
                    }
            });

            // A1 - Kill Mira Eamon (SNOQuest.X1_Bounty_A1_Wilderness_Kill_MiraEamon)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Wilderness_Kill_MiraEamon,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Wilderness_Kill_MiraEamon, SNOWorld.trOUT_Town, 75689489, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_MiraEamon, SNOWorld.trOUT_Town, SNOActor.ZombieFemale_A_BlacksmithA, 75689489),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_MiraEamon)
                    }
            });

            // A2 - Clear Sirocco Caverns (SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2,
                Act = Act.A2,
                WorldId = SNOWorld.A2C2Dun_Cave_Random01_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2, SNOWorld.caOUT_Town, 2000747858, true),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2, SNOWorld.caOUT_Town, 0, 2000747858, SNOActor.g_Portal_Circle_Orange_Bright),
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2, SNOWorld.A2C2Dun_Cave_Random01, 2108407595),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2, SNOWorld.A2C2Dun_Cave_Random01, SNOWorld.A2C2Dun_Cave_Random01_Level02, 2108407595, SNOActor.g_Portal_ArchTall_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2)
                    }
            });

            // A2 - Kill Gart the Mad (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Gart)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Gart,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Gart, SNOWorld.caOUT_Town, 1708503799, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Gart, SNOWorld.caOUT_Town, SNOActor.FallenChampion_A_Unique_01, 1708503799),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Gart)
                    }
            });

            // A2 - Kill Hemit (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Hemit)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Hemit,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Hemit,SNOWorld.caOUT_Town, SNOActor.FallenChampion_A_Unique_02, 1708503800),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Hemit)
                    }
            });

            // A2 - Kill Yeth (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Yeth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Yeth,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                Coroutines = new List<ISubroutine>
                    {
			
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1262, 1299, 184)),
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1133, 1401, 184)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1141, 1528, 184)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(969, 1474, 197)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(979, 1297, 197)),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Yeth,SNOWorld.caOUT_Town, SNOActor.Triune_Summonable_D_Unique_01, -158539678),
						
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Yeth)
                    }
            });

            // A2 - Kill High Cultist Murdos (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_HighPriestMurdos)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Alcarnus_Kill_HighPriestMurdos,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                //LevelAreaIds = new HashSet<int> { 19825, 19835 },
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_HighPriestMurdos,SNOWorld.caOUT_Town, SNOActor.TriuneCultist_C_Unique_01, 168190871),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_HighPriestMurdos)
                    }
            });

            // A2 - Kill Jhorum the Cleric (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Jhorum)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Jhorum,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Jhorum,SNOWorld.caOUT_Town, SNOActor.TriuneCultist_C_Unique_02, 168190872),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Jhorum)
                    }
            });

            // A2 - 사마시 처치 (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Sammash)
            // added WaypointLevelAreaId
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Sammash,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                WaypointLevelAreaId = SNOLevelArea.A2_caOUT_StingingWinds,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        //new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Sammash, SNOWorld.caOUT_Town , -640315117),
					
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1262, 1299, 184)),
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1133, 1401, 184)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(1141, 1528, 184)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(969, 1474, 197)), 
						new MoveToPositionCoroutine(SNOWorld.caOUT_Town, new Vector3(979, 1297, 197)),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Sammash,SNOWorld.caOUT_Town, SNOActor.Triune_Berserker_C_Unique_01, -640315117),
						
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Sammash)
                    }
            });
            
            // A2 - Kill Bashiok (SNOQuest.X1_Bounty_A2_Oasis_Kill_Bashiok)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Kill_Bashiok,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Kill_Bashiok, SNOWorld.caOUT_Town, -1086742663),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Bashiok, SNOWorld.caOUT_Town, SNOActor.FallenShaman_A_Unique01Whipple, -1086742663),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Bashiok)
                    }
            });

            // A2 - Kill Torsar (SNOQuest.X1_Bounty_A2_Oasis_Kill_Torsar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Kill_Torsar,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Kill_Torsar, SNOWorld.caOUT_Town, -1719600680),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Torsar,SNOWorld.caOUT_Town, SNOActor.DuneDervish_B_Unique_01, -1719600680),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Torsar)
                    }
            });

            // A2 - Kill Plagar the Damned (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Plagar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Kill_Plagar,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Plagar, SNOWorld.caOUT_Town, 1840003643, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Plagar,SNOWorld.caOUT_Town, SNOActor.fastMummy_B_Unique_02, 1840003643),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_Plagar)
                    }
            });

            // A2 - Kill Rockgut (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Rockgut)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Rockgut,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Rockgut, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level01, -1363317799, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Rockgut,SNOWorld.a2Dun_Zolt_Level01, SNOActor.sandMonster_B_Unique_01, -2016335963),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Rockgut)
                    }
            });

            // A2 - Kill Mage Lord Caustus (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordCaustus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordCaustus,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordCaustus, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level02, -1363317798, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordCaustus,SNOWorld.a2Dun_Zolt_Level02, SNOActor.skeletonMage_Poison_B_Unique_01, -499083571),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordCaustus)
                    }
            });

            // A2 - Kill Mage Lord Flaydren (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordFlaydren)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordFlaydren,
                Act = Act.A2,
                WorldId = SNOWorld.a2Dun_Zolt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordFlaydren, SNOWorld.a2Dun_Zolt_Lobby, SNOWorld.a2Dun_Zolt_Level02, -1363317798, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordFlaydren,SNOWorld.a2Dun_Zolt_Level02, SNOActor.skeletonMage_Lightning_B_Unique_01, 1356599065),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordFlaydren)
                    }
            });

            // A1 - Kill Sotnob the Fool (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Sotnob)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Sotnob,
                Act = Act.A1,
                WorldId = SNOWorld.trDun_Leoric_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Sotnob,SNOWorld.trDun_Leoric_Level02, SNOActor.x1_devilshand_unique_SkeletonSummoner_B, -1795932517),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Sotnob)
                    }
            });

            // A3 - Kill Axegrave the Executioner (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Axegrave)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Axegrave,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level04,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Axegrave,SNOWorld.a3Dun_Keep_Level04, SNOActor.skeleton_twoHander_Keep_Swift_E_Unique_01, 709032730),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Axegrave)
                    }
            });

            // A3 - Kill Lashtongue (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Lashtongue)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Lashtongue,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Lashtongue,SNOWorld.a3Dun_Keep_Level03, SNOActor.SoulRipper_A_Unique_01, -1791040149),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Lashtongue)
                    }
            });

            // A3 - Kill Aloysius the Ghastly (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Aloysius)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Aloysius,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level04,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Aloysius,SNOWorld.a3Dun_Keep_Level04, SNOActor.SoulRipper_A_Unique_02, -1791040148),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Aloysius)
                    }
            });

            // A3 - Kill the Vicious Gray Turkey (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_ViciousGrayTurkey)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_ViciousGrayTurkey,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level05,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_ViciousGrayTurkey,SNOWorld.a3Dun_Keep_Level05, SNOActor.Brickhouse_A_Unique_01, 681588933),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_ViciousGrayTurkey)
                    }
            });

            // A3 - Clear the Forward Barracks (SNOQuest.X1_Bounty_A3_Battlefields_Clear_ForwardBarracks)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_ForwardBarracks,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Random_Cellar_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_ForwardBarracks, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_Random_Cellar_02, 211059665, SNOActor.g_portal_Ladder_Short_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_ForwardBarracks)
                    }
            });

            // A3 - Clear the Fortified Bunker (SNOQuest.X1_Bounty_A3_Battlefields_Clear_FortifiedBunker2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_FortifiedBunker2,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_random_01_Level_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_FortifiedBunker2, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_random_01, -1049649954, SNOActor.g_Portal_ArchTall_Orange),
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_FortifiedBunker2, SNOWorld.a3Dun_Keep_random_01, -1761785482, true),
                        new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_FortifiedBunker2, SNOWorld.a3Dun_Keep_random_01, SNOWorld.a3Dun_Keep_random_01_Level_02, -1761785482, SNOActor.g_Portal_Rectangle_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_FortifiedBunker2)
                    }
            });

            // A3 - Clear the Battlefield Stores Level 2 (SNOQuest.X1_Bounty_A3_Battlefields_Clear_BattlefieldStores2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_BattlefieldStores2,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_random_03_Level_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_BattlefieldStores2, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_random_03, -1049649952, SNOActor.g_Portal_ArchTall_Orange),
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_BattlefieldStores2, SNOWorld.a3Dun_Keep_random_03, -1626182728, true),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_BattlefieldStores2, SNOWorld.a3Dun_Keep_random_03, SNOWorld.a3Dun_Keep_random_03_Level_02, -1626182728, SNOActor.g_Portal_Rectangle_Orange, true),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_BattlefieldStores2)
                    }
            });

            // A3 - Clear the Foundry (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheFoundry2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheFoundry2,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_random_04_Level_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheFoundry2, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_random_04, -1049649951, SNOActor.g_Portal_ArchTall_Orange),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheFoundry2, SNOWorld.a3Dun_Keep_random_04, SNOWorld.a3Dun_Keep_random_04_Level_02, -1558381351, SNOActor.g_Portal_Rectangle_Orange, true),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheFoundry2)
                    }
            });

            // A3 - Clear the Underbridge (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheUnderbridge)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheUnderbridge,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Random_Cellar_03,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A3_Bridge_01 },
                QuestType = BountyQuestType.ClearZone,
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2461, 649, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2270, 604, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1982, 607, -24)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1811, 612, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1435, 623, -24)),
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1047, 613, -24)),
						
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheUnderbridge, SNOWorld.A3_Battlefields_02, 211059666),
						
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheUnderbridge, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_Random_Cellar_03, 211059666, SNOActor.g_Portal_ArchTall_Orange),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheUnderbridge)
                    }
            });

            // A3 - Clear the Barracks (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheBarracks2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheBarracks2,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_random_02_Level_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheBarracks2, SNOWorld.A3_Battlefields_02, SNOWorld.a3Dun_Keep_random_02, -1049649953, SNOActor.g_Portal_ArchTall_Orange),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheBarracks2, SNOWorld.a3Dun_Keep_random_02, SNOWorld.a3Dun_Keep_random_02_Level_02, -1693984105, SNOActor.g_Portal_Rectangle_Orange, true),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheBarracks2)
                    }
            });

            // A3 - Kill Dreadgrasp (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Dreadgrasp)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Dreadgrasp,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Dreadgrasp,SNOWorld.A3_Battlefields_02, SNOActor.creepMob_A_Unique_01, 1965678403),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Dreadgrasp)
                    }
            });

            // A3 - Kill Ghallem the Cruel (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Ghallem)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Ghallem,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Ghallem,SNOWorld.A3_Battlefields_02, SNOActor.GoatMutant_Ranged_A_Unique_01, -893508246),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Ghallem)
                    }
            });

            // A3 - Kill Direclaw the Demonflyer (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Direclaw)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Direclaw,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.KillMonster,
				//WaypointLevelAreaId = 37,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Direclaw,SNOWorld.A3_Battlefields_02, SNOActor.demonFlyer_B_Unique_02, -1037000755),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Direclaw)
                    }
            });

            // A3 - Kill Shandra'Har (SNOQuest.X1_Bounty_A3_Battlefields_Kill_ShandraHar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_ShandraHar,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A3_Bridge_01 },
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2461, 649, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2270, 604, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1982, 607, -24)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1811, 612, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1435, 623, -24)),
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1047, 613, -24)),
						
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_ShandraHar, SNOWorld.A3_Battlefields_02, 511218737),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_ShandraHar, SNOWorld.A3_Battlefields_02, SNOActor.GoatMutant_Shaman_A_Unique_01, 511218737),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_ShandraHar)
                    }
            });

            // A3 - Kill Brutu (SNOQuest.X1_Bounty_A3_Crater_Kill_Brutu)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Brutu,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level02B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Brutu, SNOWorld.a3dun_Crater_ST_Level01B, SNOWorld.a3dun_Crater_ST_Level02B, 43541819, SNOActor.g_Portal_ArchTall_Orange),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Brutu,SNOWorld.a3dun_Crater_ST_Level02B, SNOActor.GoatMutant_Shaman_B_Unique_02, -1546303117),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Brutu)
                    }
            });

            // A3 - Kill Scorpitox (SNOQuest.X1_Bounty_A3_Crater_Kill_Scorpitox)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Scorpitox,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Scorpitox,SNOWorld.a3Dun_Crater_Level_03, SNOActor.Monstrosity_Scorpion_B_Unique_01, 497370622),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Scorpitox)
                    }
            });

            // A3 - Kill Gorog the Bruiser (SNOQuest.X1_Bounty_A3_Crater_Kill_Gorog)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Gorog,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Gorog,SNOWorld.a3Dun_Crater_Level_03, SNOActor.ThousandPounder_C_Unique_01, 1912157531),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Gorog)
                    }
            });

            // A3 - Kill Sawtooth (SNOQuest.X1_Bounty_A3_Crater_Kill_Sawtooth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Sawtooth,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Sawtooth, SNOWorld.a3Dun_Crater_Level_02, -234877570),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Sawtooth,SNOWorld.a3Dun_Crater_Level_02, SNOActor.Rockworm_A3_Crater_Unique_02, -234877570),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Sawtooth)
                    }
            });

            // A3 - Kill Gormungandr (SNOQuest.X1_Bounty_A3_Crater_Kill_Gormungandr)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Gormungandr,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Gormungandr,SNOWorld.a3Dun_Crater_Level_01, SNOActor.Rockworm_A3_Crater_Unique_01, -234877571),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Gormungandr)
                    }
            });

            // A3 - Kill Crabbs (SNOQuest.X1_Bounty_A3_Crater_Kill_Crabbs)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Crabbs,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Crabbs, SNOWorld.a3dun_Crater_ST_Level01, SNOWorld.a3dun_Crater_ST_Level02, 2083727833, SNOActor.g_Portal_ArchTall_Orange),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Crabbs,SNOWorld.a3dun_Crater_ST_Level02, SNOActor.Monstrosity_Scorpion_A_Unique_02, -1740074818),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Crabbs)
                    }
            });

            // A3 - Kill Riplash (SNOQuest.X1_Bounty_A3_Crater_Kill_Riplash)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Riplash,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Riplash, SNOWorld.a3dun_Crater_ST_Level01, SNOWorld.a3dun_Crater_ST_Level02, 2083727833, SNOActor.g_Portal_ArchTall_Orange),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Riplash,SNOWorld.a3dun_Crater_ST_Level02, SNOActor.SoulRipper_A_Unique_03, -1791040147),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Riplash)
                    }
            });

            // A3 - Kill Gholash (SNOQuest.X1_Bounty_A3_Crater_Kill_Gholash)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Gholash,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Gholash,SNOWorld.a3dun_Crater_ST_Level01, SNOActor.Ghoul_E_Unique_01, 2096850457),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Gholash)
                    }
            });

            // A3 - Kill Haxxor (SNOQuest.X1_Bounty_A3_Crater_Kill_Haxxor)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Haxxor,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_Crater_ST_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Haxxor,SNOWorld.a3dun_Crater_ST_Level01, SNOActor.azmodanBodyguard_A_Unique_01, -1180611615),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Haxxor)
                    }
            });

            // A4 - Kill Oah' Tash (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_OohTash)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_OohTash,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_OohTash,SNOWorld.a4dun_Garden_of_Hope_Random_B, SNOActor.morluMelee_A_Unique_01, -491275859),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_OohTash)
                    }
            });

            // A4 - Kill Kao' Ahn (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_KaoAhn)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_KaoAhn,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_KaoAhn,SNOWorld.a4dun_Garden_of_Hope_Random_B, SNOActor.morluMelee_A_Unique_02, -491275858),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_KaoAhn)
                    }
            });

            // A4 - Kill Torchlighter (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Torchlighter)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Torchlighter,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Torchlighter,SNOWorld.a4dun_Garden_of_Hope_01, SNOActor.BigRed_A_Unique_01, -1880824093),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Torchlighter)
                    }
            });

            // A4 - Kill Khatun (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Khatun)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Khatun,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Khatun,SNOWorld.a4dun_Garden_of_Hope_01, SNOActor.CoreEliteDemon_A_Unique_01, 2088134309),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Khatun)
                    }
            });

            // A4 - Kill Veshan the Fierce (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Veshan)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Veshan,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Veshan,SNOWorld.a4dun_Garden_of_Hope_01, SNOActor.MastaBlasta_Rider_A_Unique_01, -1531192840),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Veshan)
                    }
            });

            // A4 - Kill Haures (SNOQuest.X1_Bounty_A4_Spire_Kill_Haures)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_Haures,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Haures,SNOWorld.a4dun_Spire_Level_02, SNOActor.HoodedNightmare_A_Unique_02, 483254089),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Haures)
                    }
            });

            // A4 - Kill Grimnight the Soulless (SNOQuest.X1_Bounty_A4_Spire_Kill_Grimnight)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_Grimnight,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Grimnight,SNOWorld.a4dun_Spire_Level_02, SNOActor.HoodedNightmare_A_Unique_01, 483254088),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Grimnight)
                    }
            });

            // A4 - Kill Sao'Thall (SNOQuest.X1_Bounty_A4_Spire_Kill_SaoThall)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_SaoThall,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_SaoThall,SNOWorld.a4dun_Spire_Level_02, SNOActor.MorluSpellcaster_A_Sao_Unique, 86638345),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_SaoThall)
                    }
            });

            // A4 - Kill Kysindra the Wretched (SNOQuest.X1_Bounty_A4_Spire_Kill_Kysindra)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_Kysindra,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Kysindra,SNOWorld.a4dun_Spire_Level_01, SNOActor.Succubus_C_Unique_01, 416696261),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Kysindra)
                    }
            });

            // A4 - Kill Pyres the Damned (SNOQuest.X1_Bounty_A4_Spire_Kill_Pyres)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_Pyres,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Pyres,SNOWorld.a4dun_Spire_Level_01, SNOActor.Angel_Corrupt_A_Unique_03, -1855410867),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Pyres)
                    }
            });

            // A4 - Kill Slarg the Behemoth (SNOQuest.X1_Bounty_A4_Spire_Kill_Slarg)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_Slarg,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Slarg,SNOWorld.a4dun_Spire_Level_01, SNOActor.MastaBlasta_Steed_A_Unique_01, 166937943),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_Slarg)
                    }
            });

            // A4 - Kill Rhau'Kye (SNOQuest.X1_Bounty_A4_Spire_Kill_RhauKye)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_RhauKye,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_RhauKye,SNOWorld.a4dun_Spire_Level_02, SNOActor.morluSpellcaster_A_Unique_01, -243218457),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_RhauKye)
                    }
            });

            // A3 - Kill Snitchley (SNOQuest.X1_Bounty_A3_Crater_Kill_Snitchley)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Snitchley,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Crater_Kill_Snitchley, SNOWorld.a3Dun_Crater_Level_02, 1183731102, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Snitchley, SNOWorld.a3Dun_Crater_Level_02, SNOActor.treasureGoblin_C_Unique_DevilsHand, 1183731102),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Snitchley)
                    }
            });

            // A3 - Kill Bholen (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Bholen)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Bholen,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Keep_Level04,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Bholen,SNOWorld.a3Dun_Keep_Level04, SNOActor.ThousandPounder_C_Unique_DevilsHand, -35192228),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Bholen)
                    }
            });

            // A2 - Clear the Ruins (SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Zolt_SW_Random01_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2, SNOWorld.caOUT_Town, 151028377, true),						
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2, SNOWorld.caOUT_Town, 0, 151028377, SNOActor.g_Portal_Circle_Orange_Bright),
						
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2, SNOWorld.a2dun_Zolt_SW_Random01, 151028378, true),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2, SNOWorld.a2dun_Zolt_SW_Random01, 0, 151028378, SNOActor.g_Portal_Circle_Orange_Bright),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2)
                    }
            });

            // A2 - Kill Mage Lord Misgen (SNOQuest.X1_Bounty_A2_Boneyards_Kill_MageLordMisgen)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Boneyards_Kill_MageLordMisgen,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Zolt_Blood02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_MageLordMisgen, SNOWorld.caOUT_Town, SNOWorld.a2dun_Zolt_Blood02, -1758560943, SNOActor.g_Portal_Circle_Orange_Bright),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_MageLordMisgen,SNOWorld.a2dun_Zolt_Blood02, SNOActor.skeletonMage_Fire_B_Unique_BloodGuardian, -1250582635),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Boneyards_Kill_MageLordMisgen)
                    }
            });

            // A2 - Kill Inquisitor Hamath (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_InquisitorHamath)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Kill_InquisitorHamath,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Kill_InquisitorHamath, SNOWorld.caOUT_Town, -761143890, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_InquisitorHamath, SNOWorld.caOUT_Town, SNOActor.x1_devilshand_unique_TriuneSummoner_C, -761143890),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_InquisitorHamath)
                    }
            });

            // A5 - Kill Vilepaw (SNOQuest.X1_Bounty_A5_Bog_Kill_Vilepaw)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Kill_Vilepaw,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Kill_Vilepaw, SNOWorld.x1_Bog_01, -3661302, false),
						// x1_BogFamily_melee_A_Unique_DH (SNOActor.x1_BogFamily_melee_A_Unique_DH) Distance: 3.765412
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Bog_Kill_Vilepaw, SNOWorld.x1_Bog_01, SNOActor.x1_BogFamily_melee_A_Unique_DH),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Vilepaw, SNOWorld.x1_Bog_01, SNOActor.x1_BogFamily_melee_A_Unique_DH, -3661302),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Vilepaw)
                    }
            });

            // A3 - Kill Lummock the Brute (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Lummock)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Lummock,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A3_Bridge_01 },
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2461, 649, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(2270, 604, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1982, 607, -24)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1811, 612, 0)), 
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1435, 623, -24)),
						new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(1047, 613, -24)),
						
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Lummock, SNOWorld.A3_Battlefields_02, -169094470),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Lummock, SNOWorld.A3_Battlefields_02, SNOActor.FallenChampion_D_Unique_01, -169094470),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Lummock)
                    }
            });

            // A3 - Kill Growlfang (SNOQuest.X1_Bounty_A3_Crater_Kill_Growlfang)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Crater_Kill_Growlfang,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_Crater_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Growlfang,SNOWorld.a3Dun_Crater_Level_01, SNOActor.FallenHound_D_Unique_01, 830423049),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Crater_Kill_Growlfang)
                    }
            });

            // A4 - Kill the Aspect of Anguish (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectAnguish)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectAnguish,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectAnguish,SNOWorld.a4dun_Garden_of_Hope_Random_B, SNOActor.a4dun_Aspect_Anguish, -1889188727),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectAnguish)
                    }
            });

            // A4 - Kill the Aspect of Pain (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectPain)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectPain,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_B,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectPain,SNOWorld.a4dun_Garden_of_Hope_Random_B, SNOActor.a4dun_Aspect_Pain, -189435038),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectPain)
                    }
            });

            // A4 - Kill the Aspect of Destruction (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectDestruction)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectDestruction,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectDestruction,SNOWorld.a4dun_Garden_of_Hope_01, SNOActor.a4dun_Aspect_Destruction, 1086210222),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectDestruction)
                    }
            });

            // A4 - Kill the Aspect of Hatred (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectHatred)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_AspectHatred,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectHatred,SNOWorld.a4dun_Spire_Level_01, SNOActor.a4dun_Aspect_Hatred, -449006222),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectHatred)
                    }
            });

            // A4 - Kill the Aspect of Lies (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectLies)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_AspectLies,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectLies,SNOWorld.a4dun_Spire_Level_01, SNOActor.a4dun_Aspect_Lies, -189570201),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectLies)
                    }
            });

            // A4 - Kill the Aspect of Sin (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectSin)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_AspectSin,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectSin,SNOWorld.a4dun_Spire_Level_02, SNOActor.a4dun_Aspect_Sin, -1046941116),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectSin)
                    }
            });

            // A4 - Kill the Aspect of Terror (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectTerror)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_Spire_Kill_AspectTerror,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectTerror,SNOWorld.a4dun_Spire_Level_02, SNOActor.a4dun_Aspect_Terror, 25290648),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_Spire_Kill_AspectTerror)
                    }
            });

            // A5 - Kill Erdith (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Erdith)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Kill_Erdith,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Erdith,SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_DeathMaiden_Unique_A_DH, 869390077),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Erdith)
                    }
            });

            // A1 - Kill Ragus Grimlow (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Ragus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Ragus,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Ragus,SNOWorld.a1trDun_Level01, SNOActor.Corpulent_A_Unique_01, 490952466),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Ragus)
                    }
            });

            // A1 - Kill Braluk Grimlow (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Braluk)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Braluk,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Braluk,SNOWorld.a1trDun_Level01, SNOActor.Corpulent_A_Unique_02, 490952467),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Braluk)
                    }
            });

            // A1 - Kill Glidewing (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Glidewing)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Glidewing,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Glidewing,SNOWorld.a1trDun_Level01, SNOActor.FleshPitFlyer_A_Unique_01, 0),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Glidewing)
                    }
            });

            // A1 - Kill Merrium Skullthorn (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Merrium)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Merrium,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level04,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Merrium, SNOWorld.a1trDun_Level04, SNOActor.Skeleton_A_Unique_03, -222239427),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Merrium)
                    }
            });

            // A1 - Kill Cudgelarm (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Cudgelarm)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Cudgelarm,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level04,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Cudgelarm, SNOWorld.a1trDun_Level04, SNOActor.Unburied_A_Unique_01, 762156596),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Cudgelarm)
                    }
            });

            // A1 - Kill Firestarter (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Firestarter)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Firestarter,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level04,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Firestarter, SNOWorld.a1trDun_Level04, SNOActor.FleshPitFlyer_A_Unique_02, -1923753192),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Firestarter)
                    }
            });

                        // A1 - Kill Captain Cage (SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainCage)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainCage,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level06,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainCage,SNOWorld.a1trDun_Level06, SNOActor.Shield_Skeleton_A_Unique_01, -152093421),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainCage)
                    }
            });

            // A1 - Kill Killian Damort (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Killian)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Killian,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level06,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Killian,SNOWorld.a1trDun_Level06, SNOActor.SkeletonArcher_A_Unique_01, -1009862608),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Killian)
                    }
            });

            // A1 - Kill Bellybloat the Scarred (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Bellybloat)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Bellybloat,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level06,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Bellybloat,SNOWorld.a1trDun_Level06, SNOActor.Corpulent_B_Unique_01, -1566569389),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Bellybloat)
                    }
            });

            // A1 - Kill Rad'noj (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Radnoj)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_Radnoj,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level07,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Radnoj,SNOWorld.a1trDun_Level07, SNOActor.Adventurer_D_TemplarIntroUnique_AdventureMode, 1600048076),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_Radnoj)
                    }
            });

            // A1 - Kill Captain Clegg (SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainClegg)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainClegg,
                Act = Act.A1,
                WorldId = SNOWorld.a1trDun_Level07,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainClegg,SNOWorld.a1trDun_Level07, SNOActor.Unique_CaptainDaltyn_AdventureMode, -1200730604),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainClegg)
                    }
            });

            // A4 - Clear the Hell Rift (SNOQuest.X1_Bounty_A4_HellRift_Clear_HellRift2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A4_HellRift_Clear_HellRift2,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Hell_Portal_02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_HellRift_Clear_HellRift2, SNOWorld.a4dun_Hell_Portal_01, SNOWorld.a4dun_Hell_Portal_02, 984446737, SNOActor.a4_Heaven_Gardens_HellPortal),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A4_HellRift_Clear_HellRift2)
                    }
            });

            // A5 - Kill Morghum the Beast (SNOQuest.X1_Bounty_A5_Bog_Kill_Morghum)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Kill_Morghum,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                LevelAreaIds = new HashSet<SNOLevelArea> { (SNOLevelArea)SNOWorld.x1_Bog_01 },
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Morghum,SNOWorld.x1_Bog_01, SNOActor.x1_BogFamily_Brute_Unique_B, -667462790),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Morghum)
                    }
            });

            // A5 - Kill Fangbite (SNOQuest.X1_Bounty_A5_Bog_Kill_Fangbite)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Kill_Fangbite,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { (SNOLevelArea)SNOWorld.x1_Bog_01 },
                WaypointLevelAreaId = SNOLevelArea.x1_Bog_01_Part2,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Fangbite,SNOWorld.x1_Bog_01, SNOActor.x1_NightScreamer_Unique_A, 474994762),
                        //new MoveToPositionCoroutine(SNOWorld.x1_Bog_01, new Vector3(474, 946, 0)),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Fangbite)
                    }
            });

            // A5 - Kill Slinger (SNOQuest.X1_Bounty_A5_Bog_Kill_Slinger)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Kill_Slinger,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Slinger,SNOWorld.x1_Bog_01, SNOActor.x1_BogFamily_ranged_Unique_A, -504666936),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Slinger)
                    }
            });

            // A5 - Kill Almash the Grizzly (SNOQuest.X1_Bounty_A5_Bog_Kill_Almash)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Kill_Almash,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Almash,SNOWorld.x1_Bog_01, SNOActor.x1_BogFamily_ranged_Unique_B, -504666935),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Almash)
                    }
            });

            // A5 - Kill Tadardya (SNOQuest.X1_Bounty_A5_Bog_Kill_Tadardya)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Kill_Tadardya,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Tadardya,SNOWorld.x1_Bog_01, SNOActor.x1_NightScreamer_Unique_B, 474994763),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Bog_Kill_Tadardya)
                    }
            });

            // A5 - Kill Vek Tabok (SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekTabok)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekTabok,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekTabok, SNOWorld.x1_Catacombs_Level01, -1650018965),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekTabok, SNOWorld.x1_Catacombs_Level01, SNOActor.x1_MoleMutant_Melee_Unique_A, -1650018965),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekTabok)
                    }
            });

            // A5 - Kill Vek Marru (SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekMarru)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekMarru,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekMarru, SNOWorld.x1_Catacombs_Level01, -1650018964),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekMarru, SNOWorld.x1_Catacombs_Level01, SNOActor.x1_MoleMutant_Melee_Unique_B, -1650018964),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekMarru)
                    }
            });

            // A5 - Kill Nak Sarugg (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakSarugg)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakSarugg,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakSarugg,SNOWorld.x1_Catacombs_Level02, SNOActor.x1_moleMutant_Ranged_Unique_A, -1826336524),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakSarugg)
                    }
            });

            // A5 - Kill Nak Qujin (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakQujin)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakQujin,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakQujin,SNOWorld.x1_Catacombs_Level02, SNOActor.x1_moleMutant_Ranged_Unique_B, -1826336523),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakQujin)
                    }
            });

            // A5 - Kill Bari Hattar (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariHattar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariHattar,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariHattar,SNOWorld.x1_Catacombs_Level02, SNOActor.x1_MoleMutant_Shaman_Unique_A, 100982043),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariHattar)
                    }
            });

            // A5 - Kill Bari Moqqu (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariMoqqu)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariMoqqu,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Catacombs_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariMoqqu,SNOWorld.x1_Catacombs_Level02, SNOActor.x1_MoleMutant_Shaman_Unique_B, 100982044),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariMoqqu)
                    }
            });

            // A5 - Kill Getzlord (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Getzlord)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Kill_Getzlord,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Getzlord,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_westmarchBrute_Unique_B, 767269294),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Getzlord)
                    }
            });

            // A5 - Kill Yergacheph (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Yergacheph)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Kill_Yergacheph,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Yergacheph,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_FloaterAngel_Unique_04, -1454331435),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Yergacheph)
                    }
            });

            // A5 - Kill Katherine Batts (SNOQuest.X1_Bounty_A5_Westmarch_Kill_KatherineBatts)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Kill_KatherineBatts,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_KatherineBatts,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_Ghost_Dark_Unique_A, 1607064708),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_KatherineBatts)
                    }
            });

            // A5 - Kill Matanzas the Loathsome (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Matanzas)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Kill_Matanzas,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Matanzas,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_FloaterAngel_Unique_05, -1454331434),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_Matanzas)
                    }
            });

            // A5 - Kill Hedros (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Necrosys)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Kill_Necrosys,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Necrosys,SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Dark_Angel_Unique_A, 885165158),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Necrosys)
                    }
            });

            // A5 - Kill Purah (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Purah)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Kill_Purah,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Purah,SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Dark_Angel_Unique_B, 885165159),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Purah)
                    }
            });

            // A5 - Kill Targerious (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Targerious)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Kill_Targerious,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Targerious,SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_westmarchRanged_Unique_B, -1662036898),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Graveyard_Kill_Targerious)
                    }
            });

            // A5 - Kill Micheboar (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Micheboar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Micheboar,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Micheboar,SNOWorld.X1_WESTM_ZONE_03, SNOActor.x1_westmarchBrute_Unique_D, -1039640431),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Micheboar)
                    }
            });

            // A5 - Kill Theodosia Buhre (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Theodosia)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Theodosia,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Theodosia,SNOWorld.X1_WESTM_ZONE_03, SNOActor.x1_Ghost_Dark_Unique_C, 1607064710),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Theodosia)
                    }
            });

            // A5 - Kill Sumaryss the Damned (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Sumaryss)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Sumaryss,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Sumaryss,SNOWorld.X1_WESTM_ZONE_03, SNOActor.x1_westmarchRanged_Unique_A, -1662036899),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Sumaryss)
                    }
            });

            // A5 - Kill Rockulus (SNOQuest.X1_Bounty_A5_PandExt_Kill_Rockulus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Rockulus,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Rockulus,SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.X1_armorScavenger_Unique_A, -710718755),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Rockulus)
                    }
            });

            // A5 - Kill Obsidious (SNOQuest.X1_Bounty_A5_PandExt_Kill_Obsidious)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Obsidious,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Obsidious,SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.X1_armorScavenger_Unique_B, -710718754),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Obsidious)
                    }
            });

            // A5 - Kill Slarth the Tunneler (SNOQuest.X1_Bounty_A5_PandExt_Kill_Slarth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Slarth,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Slarth,SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Rockworm_Pand_Unique_A, -1819959148),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Slarth)
                    }
            });

            // A5 - Kill Burrask the Tunneler (SNOQuest.X1_Bounty_A5_PandExt_Kill_Burrask)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Burrask,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Burrask,SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Rockworm_Pand_Unique_B, -1819959147 ),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Burrask)
                    }
            });

            // A5 - Kill Watareus (SNOQuest.X1_Bounty_A5_PandExt_Kill_Watareus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Watareus,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Watareus,SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Squigglet_Unique_A, -1127556653),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Watareus)
                    }
            });

            // A5 - Kill Baethus (SNOQuest.X1_Bounty_A5_PandExt_Kill_Baethus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Baethus,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Baethus,SNOWorld.X1_Pand_Ext_2_Battlefields, SNOActor.x1_Squigglet_Unique_B, -1127556652),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Baethus)
                    }
            });

            // A5 - Kill Lograth (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
//                  new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth,SNOWorld.x1_fortress_level_01,1),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_05_soul_well", new Vector3(95.2782f, 193.9407f, 20.1f)),
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 5.001424
					//new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth, SNOWorld.x1_fortress_level_01, SNOActor.x1_Fortress_Portal_Switch, -1751517829, 5),	
					new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth, SNOWorld.x1_fortress_level_01, 1),
					
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth, SNOWorld.x1_fortress_level_01, -1443986728),
					
                    new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth, SNOWorld.x1_fortress_level_01, 0, -1443986728),
                    new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth)
                }
            });

            // A5 - Kill Valtesk the Cruel (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Valtesk)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Kill_Valtesk,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Valtesk,SNOWorld.x1_fortress_level_01,1),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Valtesk,SNOWorld.x1_fortress_level_01, SNOActor.x1_sniperAngel_Unique_A, -413057866),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Valtesk)
                    }
            });

            // A5 - Kill Scythys (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Scythys)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Kill_Scythys,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Kill_Scythys,SNOWorld.x1_fortress_level_01,1),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Scythys,SNOWorld.x1_fortress_level_01, SNOActor.x1_Wraith_Unique_A, 327834221),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Scythys)
                    }
            });

            // A5 - Kill Ballartrask the Defiler (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Ballartrask)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Kill_Ballartrask,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Ballartrask,SNOWorld.x1_fortress_level_02, SNOActor.x1_FortressBrute_Unique_B, -1443986727),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Ballartrask)
                    }
            });

            // A5 - Kill Zorrus (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Zorrus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Kill_Zorrus,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Zorrus, SNOWorld.x1_fortress_level_02, SNOActor.x1_leaperAngel_Unique_B, -1443868993),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Zorrus)
                    }
            });

            // A5 - Kill Xaphane (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Xaphane)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Kill_Xaphane,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Xaphane,SNOWorld.x1_fortress_level_02, SNOActor.x1_Wraith_Unique_B, 327834222),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandFortress_Kill_Xaphane)
                    }
            });

            // A1 - Kill Jezeb the Conjuror (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Jezeb)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Jezeb,
                Act = Act.A1,
                WorldId = SNOWorld.a1dun_Leor_Manor,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Jezeb, SNOWorld.trOUT_Town, -1019926638),
						
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1711, 3856, 40)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1517, 3837, 40)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1587, 4023, 38)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1413, 4076, 40)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1437, 3930, 50)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1214, 3903, 79)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1202, 3773, 80)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1076, 3880, 78)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(957, 3950, 80)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(892, 3860, 90)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1085, 3729, 78)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1080, 3506, 74)),
                        new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1077, 3389, 65)),

                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Jezeb, SNOWorld.trOUT_Town, SNOWorld.a1dun_Leor_Manor, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
						
						new MoveToPositionCoroutine(SNOWorld.a1dun_Leor_Manor, new Vector3(933, 544, 26)),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Jezeb, SNOWorld.a1dun_Leor_Manor, SNOActor.TriuneSummoner_A_Unique_01, 718706851),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Jezeb)
                    }
            });

            // A5 - Kill Mulliuqs the Horrid (SNOQuest.X1_Bounty_A5_PandExt_Kill_Muilliuqs)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Muilliuqs,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_Cellar_A,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Muilliuqs, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_Ext_Cellar_A, -1551729971, 0),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Muilliuqs,SNOWorld.X1_Pand_Ext_Cellar_A, SNOActor.x1_Squigglet_A_unique_cellarEventB, -1933569239),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_PandExt_Kill_Muilliuqs)
                    }
            });

            // A5 - Kill Dale Hawthorne (SNOQuest.X1_Bounty_A5_Westmarch_Kill_DaleHawthorne)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Kill_DaleHawthorne,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_DaleHawthorne,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_SkeletonArcher_Westmarch_Unique_A, -501532859),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_DaleHawthorne)
                    }
            });

            // A5 - Kill Captain Gerber (SNOQuest.X1_Bounty_A5_Westmarch_Kill_CaptainGerber)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Kill_CaptainGerber,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_CaptainGerber,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_Shield_Skeleton_Westmarch_Unique_A, -444054584),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_CaptainGerber)
                    }
            });

            // A5 - Kill Igor Stalfos (SNOQuest.X1_Bounty_A5_Westmarch_Kill_IgorStalfos)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Kill_IgorStalfos,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_IgorStalfos,SNOWorld.X1_WESTM_ZONE_01, SNOActor.x1_Skeleton_Westmarch_Unique_A, 2013948912),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_Westmarch_Kill_IgorStalfos)
                    }
            });

            // A5 - Kill Pan Fezbane (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_PanFezbane)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_PanFezbane,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_03,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_PanFezbane,SNOWorld.X1_WESTM_ZONE_03, SNOActor.x1_westmarchHound_Leader_Unique_A, -877704682),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_PanFezbane)
                    }
            });

            // A1 - Kill Growler (SNOQuest.X1_Bounty_A1_Fields_Kill_Growler)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Kill_Growler,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Growler,SNOWorld.trOUT_Town, 2067238311, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Growler,SNOWorld.trOUT_Town, SNOActor.Beast_A_Unique_03, 2067238311),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Growler)
                    }
            });

            // A1 - Kill Krelm the Flagitious (SNOQuest.X1_Bounty_A1_Fields_Kill_Krelm)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Kill_Krelm,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Fields_Kill_Krelm, SNOWorld.trOUT_Town, 1683727462, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Krelm, SNOWorld.trOUT_Town, SNOActor.Goatman_Melee_A_Unique_03, 1683727462),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Fields_Kill_Krelm)
                    }
            });

            // A1 - Kill Lord Brone (X1_Bounty_A1_FesteringWoods_Kill_LordBrone)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_LordBrone,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_LordBrone, SNOWorld.trOUT_Town, 2015206012, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_LordBrone, SNOWorld.trOUT_Town, SNOActor.Skeleton_B_Unique_01, 2015206012),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_LordBrone)
                    }
            });

            // A1 - Kill Galush Valdant (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Galush)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Galush,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
				LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOut_TristramFields_B },
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Galush, SNOWorld.trOUT_Town, 2015206013, false),
						
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(745, 866, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(868, 780, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(713, 683, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(543, 590, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(234, 551, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(177, 728, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(188, 900, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(421, 951, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(615, 979, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(460, 848, 20)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(357, 732, 20)),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Galush, SNOWorld.trOUT_Town, SNOActor.Skeleton_B_Unique_02, 2015206013),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Galush)
                    }
            });

            // A1 - Kill Reggrel the Despised (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Reggrel)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Reggrel,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands2, SNOLevelArea.A1_trOUT_Highlands3, SNOLevelArea.A1_trOUT_LeoricsManor },
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1744, 3830, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1627, 4031, 38)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1718, 4177, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1507, 4153, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1449, 4053, 39)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1206, 3897, 79)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1182, 3741, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1026, 3816, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(911, 3886, 90)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1148, 4082, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1147, 3909, 78)),
						
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Reggrel, SNOWorld.trOUT_Town, -1504570442, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Reggrel, SNOWorld.trOUT_Town, SNOActor.Goatman_Shaman_C_Unique_01, -1504570442),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Reggrel)
                    }
            });

            // A1 - Kill Hrugowl the Defiant (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Hrugowl)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Hrugowl,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands2, SNOLevelArea.A1_trOUT_Highlands3, SNOLevelArea.A1_trOUT_LeoricsManor },
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1744, 3830, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1627, 4031, 38)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1718, 4177, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1507, 4153, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1449, 4053, 39)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1206, 3897, 79)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1182, 3741, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1026, 3816, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(911, 3886, 90)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1148, 4082, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1147, 3909, 78)),
						
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Hrugowl, SNOWorld.trOUT_Town, -1504570441, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Hrugowl, SNOWorld.trOUT_Town, SNOActor.Goatman_Shaman_C_Unique_02, -1504570441),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Hrugowl)
                    }
            });

            // A1 - Kill Percepeus (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Percepeus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Percepeus,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<SNOLevelArea> { SNOLevelArea.A1_trOUT_Highlands2, SNOLevelArea.A1_trOUT_Highlands3, SNOLevelArea.A1_trOUT_LeoricsManor },
                Coroutines = new List<ISubroutine>
                    {
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1744, 3830, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1627, 4031, 38)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1718, 4177, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1507, 4153, 40)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1449, 4053, 39)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1206, 3897, 79)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1182, 3741, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1026, 3816, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(911, 3886, 90)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1148, 4082, 80)), 
						new MoveToPositionCoroutine(SNOWorld.trOUT_Town, new Vector3(1147, 3909, 78)),
						
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Percepeus, SNOWorld.trOUT_Town, -11732713, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Percepeus, SNOWorld.trOUT_Town, SNOActor.TriuneCultist_A_Unique_03, -11732713),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Percepeus )
                    }
            });

            // A2 - Kill Balzhak (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Balzhak)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Balzhak,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Balzhak, SNOWorld.caOUT_Town, -349018055, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Balzhak, SNOWorld.caOUT_Town, SNOActor.FallenChampion_B_Unique_02, -349018055),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Balzhak)
                    }
            });

            // A2 - Kill Samaras the Chaser (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Samaras)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Samaras,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Samaras, SNOWorld.caOUT_Town, 337921175, false),
						
						// DuneDervish_A_Unique_01 (367011) Distance: 0.01996064
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Samaras, SNOWorld.caOUT_Town, SNOActor.DuneDervish_A_Unique_01),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Samaras, SNOWorld.caOUT_Town, SNOActor.DuneDervish_A_Unique_01, 337921175),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Samaras)
                    }
            });

            // A2 - Kill Barty the Minuscule (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Barty)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Barty,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Barty, SNOWorld.caOUT_Town, -1367605959, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Barty, SNOWorld.caOUT_Town, SNOActor.FallenGrunt_B_Unique_01, -1367605959),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Barty)
                    }
            });

            // A2 - Kill Gryssian (SNOQuest.X1_Bounty_A2_Oasis_Kill_Gryssian)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Kill_Gryssian,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Kill_Gryssian, SNOWorld.caOUT_Town, -764145050, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Gryssian, SNOWorld.caOUT_Town, SNOActor.snakeMan_Caster_B_Unique_01, -764145050),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Gryssian)
                    }
            });

            // A2 - Kill Khahul the Serpent (SNOQuest.X1_Bounty_A2_Oasis_Kill_Khahul)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Kill_Khahul,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Kill_Khahul, SNOWorld.caOUT_Town, -764145049, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Khahul,SNOWorld.caOUT_Town, SNOActor.snakeMan_Caster_B_Unique_02, -764145049),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Khahul)
                    }
            });

            // A2 - Kill Tridiun the Impaler (SNOQuest.X1_Bounty_A2_Oasis_Kill_Tridiun)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A2_Oasis_Kill_Tridiun,
                Act = Act.A2,
                WorldId = SNOWorld.caOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A2_Oasis_Kill_Tridiun, SNOWorld.caOUT_Town, 290002476, false),
						
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Tridiun, SNOWorld.caOUT_Town, SNOActor.snakeMan_Melee_B_Unique_01, 290002476),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A2_Oasis_Kill_Tridiun)
                    }
            });

            // A3 - Kill Thromp the Breaker (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Thromp)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Thromp,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_rmpt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Thromp, SNOWorld.a3Dun_rmpt_Level02, SNOWorld.a3dun_rmpt_Level01, -1078336204, SNOActor.g_Portal_ArchTall_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Thromp,SNOWorld.a3dun_rmpt_Level01, SNOActor.demonTrooper_B_Unique_02, -1902915434),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Thromp)
                    }
            });

            // A3 - Kill Obis the Mighty (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Obis)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Obis,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_rmpt_Level02, //SNOWorld.a3dun_rmpt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Obis, SNOWorld.a3Dun_rmpt_Level02, SNOWorld.a3dun_rmpt_Level01, -1078336204, SNOActor.g_Portal_ArchTall_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Obis,SNOWorld.a3dun_rmpt_Level01, SNOActor.demonTrooper_B_Unique_03, -1902915433),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Obis)
                    }
            });

            // A3 - Kill Ganthar the Trickster (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Ganthar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Ganthar,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_rmpt_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Ganthar, SNOWorld.a3Dun_rmpt_Level02, SNOWorld.a3dun_rmpt_Level01, -1078336204, SNOActor.g_Portal_ArchTall_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Ganthar,SNOWorld.a3dun_rmpt_Level01, SNOActor.FallenShaman_C_Unique_01, 2128210786),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Ganthar)
                    }
            });

            // A3 - Kill Greelode the Unforgiving (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Greelode)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Greelode,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_rmpt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Greelode,SNOWorld.a3Dun_rmpt_Level02, SNOActor.ThousandPounder_B_Unique_02, -325287909),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Greelode)
                    }
            });

            // A3 - Kill Barrucus (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Barrucus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Barrucus,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_rmpt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Barrucus,SNOWorld.a3Dun_rmpt_Level02, SNOActor.FallenHound_E_Unique_01, -1227098806),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Barrucus)
                    }
            });

            // A3 - Kill Allucayrd (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Allucayrd)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Ramparts_Kill_Allucayrd,
                Act = Act.A3,
                WorldId = SNOWorld.a3Dun_rmpt_Level02,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Allucayrd,SNOWorld.a3Dun_rmpt_Level02, SNOActor.demonFlyer_C_unique_01, -686066387),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A3_Ramparts_Kill_Allucayrd)
                    }
            });

            // A1 - Kill Kankerrot (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Horrus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Wilderness_Kill_Horrus,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Horrus,SNOWorld.trOUT_Town, SNOActor.Unburied_C_Unique_01, 942080182),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Horrus)
                    }
            });

            // A1 - Kill Horrus the Nightstalker (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Kankerrot)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Wilderness_Kill_Kankerrot,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
						new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Wilderness_Kill_Kankerrot, SNOWorld.trOUT_Town, 490952468, false),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Kankerrot, SNOWorld.trOUT_Town, SNOActor.Corpulent_A_Unique_03, 490952468),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Kankerrot)
                    }
            });

            // A4 - Kill Sabnock The Infector (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Sabnock)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Sabnock,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Sabnock,SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1, SNOActor.P2_HoodedNightmare_A_Unique_01, 1712473545),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Sabnock)
                    }
            });

            // A4 - Kill Malefactor Vephar (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Vephar)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Vephar,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Vephar,SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1, SNOActor.P2_TerrorDemon_A_Unique_01, 1151200136),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Vephar)
                    }
            });

            // A4 - Kill Amduscias (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Amduscias)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Amduscias,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Amduscias,SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1, SNOActor.P2_morluSpellcaster_A_Unique_01, 1666317960),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Amduscias)
                    }
            });

            // A4 - Kill Cimeries (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Cimeries)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Cimeries,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Cimeries,SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1, SNOActor.P2_morluMelee_A_Unique_01, 1607519118),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Cimeries)
                    }
            });

            // A4 - Kill Erra (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Raym)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Raym,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_A,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Raym,SNOWorld.a4dun_Garden_of_Hope_Random_A, SNOActor.P2_MalletDemon_A_Unique_01, 1155578409),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Raym)
                    }
            });

            // A4 - Kill Palerider Beleth (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Beleth)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Beleth,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_A,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Beleth,SNOWorld.a4dun_Garden_of_Hope_Random_A, SNOActor.P2_MastaBlasta_Combined_Unique_A, -595378876),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Beleth)
                    }
            });

            // A4 - Kill Emberdread (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Emberdread)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Emberdread,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_A,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Emberdread,SNOWorld.a4dun_Garden_of_Hope_Random_A, SNOActor.P2_BigRed_Burned_A_Unique, 1049069571),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Emberdread)
                    }
            });

            // A4 - Kill Lasciate (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Lasciate)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Lasciate,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Garden_of_Hope_Random_A,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Lasciate,SNOWorld.a4dun_Garden_of_Hope_Random_A, SNOActor.P2_Angel_Corrupt_A_Unique_01, -1381007476),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Lasciate)
                    }
            });

            // A4 - Clear the Besieged Tower Level 2 (SNOQuest.P2_Bounty_A4_BesiegedTower_Clear_BesiegedTower2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P2_Bounty_A4_BesiegedTower_Clear_BesiegedTower2,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level2,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(SNOQuest.P2_Bounty_A4_BesiegedTower_Clear_BesiegedTower2, SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1, -97144983),
                        new EnterLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_BesiegedTower_Clear_BesiegedTower2, SNOWorld.a4dun_CorruptSpire_SideDungeon_A_Level1, 0, -97144983, SNOActor.a4dun_spire_Elevator_Portal),
                        new ClearLevelAreaCoroutine (SNOQuest.P2_Bounty_A4_BesiegedTower_Clear_BesiegedTower2)
                    }
            });

            // A5 - Clear the Guild Hideout (SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_GuildHideout2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_GuildHideout2,
                Act = Act.A5,
                WorldId = SNOWorld.X1_westm_cesspools_randomA_level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
 //                     new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_GuildHideout2, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_westm_cesspools_randomA_level01, 48502155, SNOActor.g_Portal_Rectangle_Blue_Westm_SideDungeon),
						new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_GuildHideout2, SNOWorld.X1_WESTM_ZONE_03, 0, 48502155, SNOActor.g_Portal_Rectangle_Blue_Westm_SideDungeon),
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_GuildHideout2, SNOWorld.X1_westm_cesspools_randomA_level01, SNOWorld.X1_westm_cesspools_randomA_level02, 48502156, SNOActor.g_Portal_Square_Orange, true),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_GuildHideout2)
                    }
            });

            // A5 - Kill Brent Brewington (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Brent)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Brent,
                Act = Act.A5,
                WorldId = SNOWorld.X1_westm_cesspools_randomA_level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Brent, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_westm_cesspools_randomA_level01, 48502155, SNOActor.g_Portal_Rectangle_Blue_Westm_SideDungeon),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Brent,SNOWorld.X1_westm_cesspools_randomA_level01, SNOActor.X1_graveRobber_C_ScoundrelEvent_Unique02, 1803003715),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Brent)
                    }
            });

            // A5 - Kill Meriel Regodon (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Meriel)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Meriel,
                Act = Act.A5,
                WorldId = SNOWorld.X1_westm_cesspools_randomA_level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Meriel, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_westm_cesspools_randomA_level01, 48502155, SNOActor.g_Portal_Rectangle_Blue_Westm_SideDungeon),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Meriel,SNOWorld.X1_westm_cesspools_randomA_level01, SNOActor.X1_graveRobber_B_ScoundrelEvent_Unique03, -1084271549),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Meriel)
                    }
            });

            // A5 - Kill Denis Genest (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Denis)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Denis,
                Act = Act.A5,
                WorldId = SNOWorld.X1_westm_cesspools_randomA_level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Denis, SNOWorld.X1_WESTM_ZONE_03, SNOWorld.X1_westm_cesspools_randomA_level01, 48502155, SNOActor.g_Portal_Rectangle_Blue_Westm_SideDungeon),
                        new KillUniqueMonsterCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Denis,SNOWorld.X1_westm_cesspools_randomA_level01, SNOActor.X1_graveRobber_A_ScoundrelEvent_Unique01, 323420480),
                        new ClearLevelAreaCoroutine (SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Denis)
                    }
            });

            // A2 - Clear the Western Channel (SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Special_A_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_A_Level01, 705396550, SNOActor.g_Portal_Rectangle_Blue),
                        new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2, SNOWorld.a2dun_Aqd_Special_A_Level01, 0, 705396551, SNOActor.g_Portal_Square_Blue),
                        new ClearLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2)
                    }
            });

            // A2 - Clear the Eastern Channel (SNOQuest.px_Bounty_A2_Aqueducts_Clear_EasternChannel2)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Aqueducts_Clear_EasternChannel2,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Special_B_Level02,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_EasternChannel2, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_B_Level01, 1037011047, SNOActor.g_Portal_Square_Blue),
                        new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_EasternChannel2, SNOWorld.a2dun_Aqd_Special_B_Level01, 0, 1037011048, SNOActor.g_Portal_Square_Blue),
                        new ClearLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_EasternChannel2)
                    }
            });

            // A2 - Kill Yakara (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Yakara)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Aqueducts_Kill_Yakara,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Special_B_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Yakara, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_B_Level01, 1037011047, SNOActor.g_Portal_Square_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Yakara,SNOWorld.a2dun_Aqd_Special_B_Level01, SNOActor.snakeMan_Melee_A_Unique_02, -1947442964),
                        new ClearLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Yakara)
                    }
            });

            // A2 - Kill Grool (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Grool)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Aqueducts_Kill_Grool,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Special_B_Level01,
                //WaypointNumber = 26,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//                      new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Grool, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_B_Level01, 1037011047, SNOActor.g_Portal_Square_Blue),
						new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Grool, SNOWorld.a2dun_Aqd_Special_01, 0, 1037011047, SNOActor.g_Portal_Square_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Grool,SNOWorld.a2dun_Aqd_Special_B_Level01, SNOActor.Ghoul_B_Unique_01, -320518570),
                        new ClearLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_Grool)
                    }
            });

            // A2 - Kill Otzi the Cursed (SNOQuest.px_Bounty_A2_Aqueducts_Kill_OtziTheCursed)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.px_Bounty_A2_Aqueducts_Kill_OtziTheCursed,
                Act = Act.A2,
                WorldId = SNOWorld.a2dun_Aqd_Special_A_Level01,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        //                        new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_A_Level01, 705396550, SNOActor.g_Portal_Rectangle_Blue),
                        //new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2, SNOWorld.a2dun_Aqd_Special_A_Level01, a2dun_Aqd_Special_A_Level02, 705396551, SNOActor.g_Portal_Square_Blue),

						new EnterLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_OtziTheCursed, SNOWorld.a2dun_Aqd_Special_01, SNOWorld.a2dun_Aqd_Special_A_Level01, 705396550, SNOActor.g_Portal_Rectangle_Blue),
                        new KillUniqueMonsterCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_OtziTheCursed,SNOWorld.a2dun_Aqd_Special_A_Level01, SNOActor.fastMummy_B_Unique_01, 1840003642),
                        new ClearLevelAreaCoroutine (SNOQuest.px_Bounty_A2_Aqueducts_Kill_OtziTheCursed)
                    }
            });

            // A1 - Bounty: Kill Johanys (SNOQuest.X1_Bounty_A1_Leoric_Kill_Johanys)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Johanys,
                Act = Act.A1,
                WorldId = SNOWorld.x1_p4_Leoric_Estate,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A1_trOUT_LeoricsManor,
                Coroutines = new List<ISubroutine>
                {
                   new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Johanys, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                   new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Johanys, SNOWorld.x1_p4_Leoric_Estate, -829990205),
                   new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Johanys),
                }
            });

            // A5 - Bounty: Binding Evil (SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 2912417),
					
					new MoveToActorCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.P4_Forest_Mysterious_Hermit_Friendly),
                    // P4_Forest_Mysterious_Hermit_Friendly (SNOActor.P4_Forest_Mysterious_Hermit_Friendly) Distance: 2.445482
                    new InteractWithUnitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.P4_Forest_Mysterious_Hermit_Friendly, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(60.12238f, 71.47668f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(78.24176f, 65.46866f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(71.70166f, 78.02289f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(55.58252f, 79.62216f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(61.52814f, 64.55441f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(80.84772f, 58.59448f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(77.08563f, 77.41998f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(52.39435f, 82.44772f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(60.12238f, 71.47668f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(78.24176f, 65.46866f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(71.70166f, 78.02289f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(55.58252f, 79.62216f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(61.52814f, 64.55441f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(80.84772f, 58.59448f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(77.08563f, 77.41998f, 0.09999999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_02", new Vector3(52.39435f, 82.44772f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, SNOWorld.x1_p4_Forest_Coast_01, 5000),
					
//                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Evil_Trees, 60, 0,  2912417, 15, false),
                }
            });

            // A4 - Bounty: The Black King's Legacy (SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2) 
            // New Bounty Portal to Leorics Chest with Elites Routine - wasnt working with dynamic setup
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2,
                Act = Act.A4,
                WorldId = SNOWorld.a4dun_Spire_Level_02,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 49,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2, SNOWorld.a4dun_Spire_Level_02, 0, 37697317, SNOActor.P4_bounty_grounds_Leorics_Garden_Champ),
                    new MoveToActorCoroutine(SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2, SNOWorld.a4dun_Spire_Level_02, SNOActor.P4_bounty_grounds_Leorics_Garden_Champ),
                    // x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 24.08629
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2, SNOWorld.a4dun_Spire_Level_02, SNOActor.P4_bounty_grounds_Leorics_Garden_Champ, 37697317, 5),
                                        new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2, SNOWorld.p4_bounty_grounds_Leorics_Garden, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                                        new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2, 60, 0, 0, 45),
                                        new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A4_bounty_ground_Leoric_Champ_Spire2),
                                        new MoveToPositionCoroutine(SNOWorld.p4_bounty_grounds_Leorics_Garden, new Vector3(159, 182, 0)),
                }
            });
            
            // A3 - Bounty: Last of the Barbarians (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41, // Ruins of Sescheron
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, 2912417),

                    //new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_W_Entrance_02_LoDGate_E02_S01", new Vector3(117.0242f, 19.4599f, 0.6535801f)),

                    new MoveToSceneCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NS_01"),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NS_01", new Vector3(183.717f, 168.832f, -0.01446433f)),
                    new MoveToActorCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage),

                    // px_Ruins_frost_camp_cage (SNOActor.px_Ruins_frost_camp_cage) Distance: 13.54914
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage, 0, 5),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NS_01", new Vector3(122.0508f, 144.9282f, 0.1000014f)),
                    new MoveToActorCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage),

                    // px_Ruins_frost_camp_cage (SNOActor.px_Ruins_frost_camp_cage) Distance: 62.45116
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage, 0, 5),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NS_01", new Vector3(183.717f, 168.832f, -0.01446433f)),
                    new MoveToActorCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage),

                    // px_Ruins_frost_camp_cage (SNOActor.px_Ruins_frost_camp_cage) Distance: 13.54914
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage, 0, 5),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NS_01", new Vector3(122.0508f, 144.9282f, 0.1000014f)),
                    new MoveToActorCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage),

                    // px_Ruins_frost_camp_cage (SNOActor.px_Ruins_frost_camp_cage) Distance: 62.45116
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage, 0, 5),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NS_01", new Vector3(161.8187f, 61.48199f, -9.913034f)),

                    new MoveToActorCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage),

                    // px_Ruins_frost_camp_cage (SNOActor.px_Ruins_frost_camp_cage) Distance: 62.45116
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage, 0, 5),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NS_01", new Vector3(49.39655f, 63.32892f, 0.1000005f)),

                    new MoveToActorCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage),

                    // px_Ruins_frost_camp_cage (SNOActor.px_Ruins_frost_camp_cage) Distance: 92.18051
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_frost_camp_cage, 0, 5),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_bounty_ground_Swr_Champ_Cor, 90, 0, 0, 45),

                    // px_Ruins_Frost_Camp_BarbSkular (SNOActor.px_Ruins_Frost_Camp_BarbSkular) Distance: 8.818088
                    new InteractWithUnitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_SkularsQuest, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.px_Ruins_Frost_Camp_BarbSkular, 0, 5),

                }
            });
            
            // A1 - Bounty: Kill Hannes (SNOQuest.X1_Bounty_A1_Leoric_Kill_Hannes)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Hannes,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A1_trOUT_LeoricsManor,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Hannes, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Hannes),
                }
            });

            // A3 - Bounty: King of the Ziggurat (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat) - 6	0
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                 {
                     new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 2912417),
                     new MoveToSceneCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "Ziggurat"),

                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(63.26221f, 120.9539f, 10.09718f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.5901f, 76.61993f, 10.77626f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(169.4726f, 117.2496f, 10.27893f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(123.4897f, 170.7134f, 10.7916f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(63.26221f, 120.9539f, 10.09718f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.5901f, 76.61993f, 10.77626f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(169.4726f, 117.2496f, 10.27893f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 5000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(123.4897f, 170.7134f, 10.7916f)),
                     new WaitCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, 10000),
                     new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
 //                  new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, 90, 0, 0, 30),

                     new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Event_TheZiggurat, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.p4_Ruins_Frost_Chest_Pillar_Reward, 0, 5),
                 }
            });

            // A5 - Bounty: The Ghost Prison (SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    //ActorId: SNOActor.P4_Forest_Coast_Holy_Relics, Type: Gizmo, Name: P4_Forest_Coast_Holy_Relics-4632, Distance2d: 16.82734, CollisionRadius: 17.19083, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0                     
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, 2912417),
//                 new MoveToActorCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.P4_Forest_Coast_Holy_Relics),
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.P4_Forest_Coast_Holy_Relics, -1),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(60.66754f, 57.75403f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(58.20206f, 37.20313f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(76.867f, 57.91034f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(67.42307f, 80.11035f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(44.89722f, 56.82794f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(63.0864f, 58.17493f, 0.1f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, SNOWorld.x1_p4_Forest_Coast_01, 10000),
					
//                  new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Ghost_Prison, 60, 0, 0, 45),
                }
            });
            
            // A5 - Bounty: Kill Fharzula (SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_p4_Forest_Coast_01, 1493379889, true),
                    
                    //Actor: x1_westm_Door_Wide_Clicky-2290 (SNOActor.x1_westm_Door_Wide_Clicky) Gizmo  -->
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.x1_westm_Door_Wide_Clicky, 0, 5),

                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_p4_Forest_Coast_01, SNOWorld.x1_P4_Forest_Coast_Tower_Mid_Lvl_B, 1493379889, SNOActor.g_Portal_Rectangle_Blue),

                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_P4_Forest_Coast_Tower_Mid_Lvl_B, SNOWorld.x1_P4_Forest_Coast_Tower_Mid_Lvl, 1493379890, SNOActor.g_Portal_Rectangle_Blue),

                    //Actor: p4_Forest_Coast_stool_A-2526 (441306) Gizmo  
                    //ActorId: SNOActor.p4_Forest_HighCleric_SpawnLectern, Type: Gizmo, Name: p4_Forest_HighCleric_SpawnLectern-26508, Distance2d: 11.52699, CollisionRadius: 7.813453, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_P4_Forest_Coast_Tower_Mid_Lvl, SNOActor.p4_Forest_HighCleric_SpawnLectern, -1, 5),

                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_P4_Forest_Coast_Tower_Mid_Lvl, 2000),

                    // p4_Forest_ClericGhost (SNOActor.p4_Forest_ClericGhost) Distance: 18.61721
                    new InteractWithUnitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_P4_Forest_Coast_Tower_Mid_Lvl, SNOActor.p4_Forest_ClericGhost, 0, 5),

                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, SNOWorld.x1_P4_Forest_Coast_Tower_Mid_Lvl, 8000),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_Sacrifice, 60, 0, 0, 45),

                }
            });

            // A5 - Bounty: The Cursed Wood (SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_02)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_02,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.x1_P2_Forest_Coast_Level_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_02, SNOWorld.x1_p4_Forest_Coast_01, 2912417, true), 
                    // x1_Global_Chest_CursedChest_B ((int)SNOActor.x1_Global_Chest_CursedChest_B) Distance: 22.02429
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_02, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_02, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30),
                }
            });
            
            // A3 - Bounty: Kill Garan (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalC)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalC,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01,
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
					new MoveToPositionCoroutine(SNOWorld.a3dun_ruins_frost_city_A_01, new Vector3(598, 763, 0)),
					new MoveToPositionCoroutine(SNOWorld.a3dun_ruins_frost_city_A_01, new Vector3(437, 320, -9)),
					
					new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalC, SNOWorld.a3dun_ruins_frost_city_A_01, 1052019909, true),
                    // P4_Ruins_CannibalBarbarian_C_Unique (SNOActor.P4_Ruins_CannibalBarbarian_C_Unique) Distance: 46.67881
					new KillUniqueMonsterCoroutine (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalC, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.P4_Ruins_CannibalBarbarian_C_Unique, 1052019909),
                    //new MoveToActorCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalC, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.P4_Ruins_CannibalBarbarian_C_Unique),
                    new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalC),
                }
            });

            // A3 - Bounty: Kill Chiltara (SNOQuest.X1_Bounty_A3_Battlefields_Kill_Chiltara)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Kill_Chiltara,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 34,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(SNOWorld.A3_Battlefields_02, new Vector3(3356, 562, 0)),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Chiltara, SNOWorld.A3_Battlefields_02, SNOWorld.a3dun_IceCaves_Random_01, 1029056444, SNOActor.g_Portal_Circle_Blue),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Chiltara, SNOWorld.a3dun_IceCaves_Random_01, SNOWorld.a3dun_IceCaves_Random_01_Level_02, 151580180, SNOActor.g_Portal_Oval_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Chiltara, SNOWorld.a3dun_IceCaves_Random_01_Level_02, 1645435934),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Kill_Chiltara),
                }
            });

            // A3 - Crazy Climber (SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber,
                Act = Act.A3,
                WorldId = SNOWorld.A3_Battlefields_02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A3_Battlefield_B,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, 2912417),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_05", new Vector3(196.2253f, 85.36218f, 0.624619f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, 1000),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Injured_Reinforcement_Event, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_05", new Vector3(192.509f, 83.62573f, 0.7063226f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, 1000),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Injured_Reinforcement_Event, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_05", new Vector3(131.6942f, 108.8494f, 38.24864f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_05", new Vector3(76.2251f, 108.259f, 38.20418f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_05", new Vector3(79.22241f, 41.45508f, 63.26687f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_05", new Vector3(79.22241f, 41.45508f, 63.26687f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, "a3dun_Bridge_NS_Towers_05", new Vector3(143.4139f, 35.06018f, 87.60001f)),

                    // bastionsKeepGuard_Lieutenant_Reinforcement_Event (SNOActor.bastionsKeepGuard_Lieutenant_Reinforcement_Event) Distance: 3.705163
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, SNOWorld.A3_Battlefields_02, SNOActor.bastionsKeepGuard_Lieutenant_Reinforcement_Event, 0, 5),
                }
            });

            // A1 - Kill Morgan LeDay (SNOQuest.X1_Bounty_A1_Leoric_Kill_Morgan_LeDay)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Morgan_LeDay,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Morgan_LeDay, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Morgan_LeDay, SNOWorld.x1_p4_Leoric_Estate, -707852521),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Morgan_LeDay),
                }
            });

            // A1 - Kill Teffeney (SNOQuest.X1_Bounty_A1_Wilderness_Kill_Teffeney)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Wilderness_Kill_Teffeney,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A1_trOut_TristramWilderness,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Wilderness_Kill_Teffeney, SNOWorld.trOUT_Town, 883191128, false),
                    new KillUniqueMonsterCoroutine(SNOQuest.X1_Bounty_A1_Wilderness_Kill_Teffeney, SNOWorld.trOUT_Town, SNOActor.ZombieFemale_B_Unique001, 883191128),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Wilderness_Kill_Teffeney),
                }
            });

            // A3 - The Cursed Eternal Shrine (SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedShrine01)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedShrine01,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedShrine01, SNOWorld.a3dun_ruins_frost_city_A_01, SNOWorld.p4_Forest_Snow_01, -1665315172, SNOActor.g_Portal_Rectangle_Blue),
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedShrine01, SNOWorld.p4_Forest_Snow_01, 2912417),
                    // x1_Event_CursedShrine ((int)SNOActor.x1_Global_Chest_CursedChest_B) Distance: 11.55153
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedShrine01, SNOWorld.p4_Forest_Snow_01, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedShrine01, 90, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30),
                }
            });

            // A3 - 저주받은 영겁의 숲 (SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedChest_01)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedChest_01,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedChest_01, SNOWorld.a3dun_ruins_frost_city_A_01, SNOWorld.p4_Forest_Snow_01, -1665315172, SNOActor.g_Portal_Rectangle_Blue),
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedChest_01, SNOWorld.p4_Forest_Snow_01, 2912417),
                    // x1_Global_Chest_CursedChest_B ((int)SNOActor.x1_Global_Chest_CursedChest_B) Distance: 24.78498
					new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedChest_01, SNOWorld.p4_Forest_Snow_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedChest_01, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30),
//                    new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Event_CursedChest_01),
                }
            });

            // A5 - 저주받은 숲 (SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_01)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_01,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.x1_P2_Forest_Coast_Level_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_01, SNOWorld.x1_p4_Forest_Coast_01, 2912417),
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_01, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_CursedChest_01, 60, 0, 0, 30),
                }
            });

            // A1 - 조비안스 처치 (SNOQuest.X1_Bounty_A1_Leoric_Kill_Jovians)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Jovians,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Jovians, SNOWorld.trOUT_Town, -1019926638, true),
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Jovians, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Jovians, SNOWorld.x1_p4_Leoric_Estate, -1320159821),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Jovians),
                }
            });

            // A5 -  Kill The Succulent (SNOQuest.P4_Bounty_A5_ForestCoast_Cave_Clear)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_Cave_Clear,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01,
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Cave_Clear, SNOWorld.x1_p4_Forest_Coast_01, SNOWorld.x1_P4_Forest_Coast_Cave_Level01, -1189768563, SNOActor.g_Portal_Circle_Blue),
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Cave_Clear, SNOWorld.x1_p4_Forest_Coast_01, -1189768563),
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Cave_Clear, SNOWorld.x1_P4_Forest_Coast_Cave_Level01, 1055678975),
                    new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Cave_Clear),
                }
            });

            // A3 - Clear The Icy Pit (SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear,
                Act = Act.A3,
                WorldId = (SNOWorld)SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A3_dun_ruins_frost_city_A_01,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear, SNOWorld.a3dun_ruins_frost_city_A_01, SNOWorld.p4_Forest_Snow_01, -1665315172, SNOActor.g_Portal_Rectangle_Blue),
                    // g_Portal_Rectangle_Blue (SNOActor.g_Portal_Rectangle_Blue) Distance: 16.18488
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear, SNOWorld.a3dun_ruins_frost_city_A_01, SNOActor.g_Portal_Rectangle_Blue, -1665315172, 5),
                    new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear, SNOWorld.p4_Forest_Snow_01, 0, 1537341835, SNOActor.g_Portal_ArchTall_Blue),
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear, SNOWorld.p4_Forest_Snow_01, 1537341835),
                    // g_Portal_ArchTall_Blue (SNOActor.g_Portal_ArchTall_Blue) Distance: 48.33623
                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear, SNOWorld.p4_Forest_Snow_01, SNOActor.g_Portal_ArchTall_Blue, 1537341835, 5),
                    new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Cave_Clear),
                }
            });

            // A1 - Kill Spatharii (SNOQuest.X1_Bounty_A1_Leoric_Kill_Spatharii)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Spatharii,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Spatharii, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Spatharii, SNOWorld.x1_p4_Leoric_Estate, -1192804794),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Spatharii),
                }
            });

            // A5 - Kill Sartor (SNOQuest.X1_Bounty_A5_PandExt_Kill_Sartor)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Kill_Sartor,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 59,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Kill_Sartor, SNOWorld.X1_Pand_Ext_2_Battlefields, 255791851),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Kill_Sartor),
                }
            });

            // A1 - Kill Walloon (SNOQuest.X1_Bounty_A1_Leoric_Kill_Walloon)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Walloon,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Walloon, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Walloon, SNOWorld.x1_p4_Leoric_Estate, 1929101567),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Walloon),
                }
            });

            // A1 - Kill Boyarsk (SNOQuest.X1_Bounty_A1_Leoric_Kill_Boyarsk)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Boyarsk,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Boyarsk, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Boyarsk, SNOWorld.x1_p4_Leoric_Estate, -1396422803),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Boyarsk),
                }
            });

            // A5 - The Cursed Pond (SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01,
                Act = Act.A5,
                WorldId = SNOWorld.x1_p4_Forest_Coast_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 2912417),
                    // x1_Event_CursedShrine (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 15.57718
//                    new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
					new InteractWithGizmoCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(59.4035f, 51.7612f, -0.7321681f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(75.91962f, 49.12241f, -0.8999999f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(86.51501f, 68.63174f, -0.9f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(57.19635f, 72.65244f, -0.4761491f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(64.28271f, 57.70676f, -0.519156f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(80.21851f, 41.82578f, -0.5355958f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(84.3645f, 65.76697f, -0.8745145f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(58.29736f, 70.78946f, -0.4374949f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, "p4_Forest_Coast_NSEW_06", new Vector3(57.27289f, 50.69296f, -0.9f)),
                    new WaitCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, SNOWorld.x1_p4_Forest_Coast_01, 10000),
										
//                  new ClearAreaForNSecondsCoroutine(SNOQuest.P4_Bounty_A5_ForestCoast_Event_ForestShrine01, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30),
                }
            });
            
            // A1 - Kill Baxtrus (SNOQuest.X1_Bounty_A1_Leoric_Kill_Baxtrus)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Leoric_Kill_Baxtrus,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Baxtrus, SNOWorld.trOUT_Town, SNOWorld.x1_p4_Leoric_Estate, -1019926638, SNOActor.g_Portal_ArchTall_Orange),
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Baxtrus, SNOWorld.x1_p4_Leoric_Estate, 314659300),
                    new ClearLevelAreaCoroutine(SNOQuest.X1_Bounty_A1_Leoric_Kill_Baxtrus),
                }
            });
            
            // A5 - Research Problems (SNOQuest.X1_Bounty_A5_Bog_Event_AngryBats)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Bog_Event_AngryBats,
                Act = Act.A5,
                WorldId = SNOWorld.x1_Bog_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 54,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_AngryBats, SNOWorld.x1_Bog_01, 2912417),
                    // OmniNPC_Tristram_Male_E_angryBatsEvent (SNOActor.OmniNPC_Tristram_Male_E_angryBatsEvent) Distance: 58.61249
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_AngryBats, SNOWorld.x1_Bog_01, SNOActor.OmniNPC_Tristram_Male_E_angryBatsEvent, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_AngryBats, 60, 0, 0, 40),
                    //ActorId: 237876, Type: Monster, Name: x1_BogFamily_brute_A-1514
                    //ActorId: 239516, Type: Monster, Name: x1_NightScreamer_A-1438
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_AngryBats, SNOWorld.x1_Bog_01, SNOActor.OmniNPC_Tristram_Male_E_angryBatsEvent, 0, 5),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Bog_Event_AngryBats, 60, 0, 0, 40),
                }
            });

            // A5 - Bounty: Demon Prison (SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison,
                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 57,
                Coroutines = new List<ISubroutine>
                {
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 1.162077
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, SNOActor.x1_Fortress_Portal_Switch),
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 1.162077
					new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 1),
					//new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, SNOActor.x1_Fortress_Portal_Switch, -1751517829, 5),

                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 2912417),
                    // X1_Fortress_NephalemSpirit (SNOActor.X1_Fortress_NephalemSpirit) Distance: 21.83757
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, SNOActor.X1_Fortress_NephalemSpirit, 2912417, 5),
                    //ActorId: SNOActor.x1_Fortress_Crystal_Prison_Yellow, Type: Gizmo, Name: x1_Fortress_Crystal_Prison_Yellow-2093, Distance2d: 19.41266, CollisionRadius: 14.90234, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, SNOActor.x1_Fortress_Crystal_Prison_Yellow, -1, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, "x1_fortress_NEW_02", new Vector3(118.0137f, 140.5249f, -9.534434f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, "x1_fortress_NEW_02", new Vector3(119.1356f, 114.215f, -9.534435f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, "x1_fortress_NEW_02", new Vector3(137.9068f, 127.946f, -9.806384f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, "x1_fortress_NEW_02", new Vector3(125.8439f, 149.037f, -9.534435f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, "x1_fortress_NEW_02", new Vector3(99.97601f, 134.6227f, -9.814684f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 10000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, "x1_fortress_NEW_02", new Vector3(117.2425f, 125.2808f, -9.534436f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, SNOWorld.x1_fortress_level_01, 10000),
	
//                  new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_DemonPrison, 60, 0, 0, 40),
                }
            });

            // A3 - Kill Korae and Samae (SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalB)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalB,
                Act = Act.A3,
                WorldId = SNOWorld.a3dun_ruins_frost_city_A_01,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalB, SNOWorld.a3dun_ruins_frost_city_A_01, -100010011),
                    new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_RuinsOfSescheron_Kill_CannibalB, SNOWorld.a3dun_ruins_frost_city_A_01, 1383249892),
                }
            });

            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.x1_Pand_Ext_240_Cellar_04Demon_Event,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_2_Battlefields, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 59,
                Coroutines = new List<ISubroutine>
                {

                    new EnterLevelAreaCoroutine(SNOQuest.x1_Pand_Ext_240_Cellar_04Demon_Event, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_Ext_Cellar_D, -1551729968, SNOActor.g_Portal_ArchTall_Blue),

                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(144, 196, -13)),
                    new InteractWithGizmoCoroutine(SNOQuest.x1_Pand_Ext_240_Cellar_04Demon_Event, SNOWorld.X1_Pand_Ext_Cellar_D, SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_1, 0, 5),

                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(160, 74, -13)),
                    //ActorId: SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_2, Type: Gizmo, Name: x1_Fortress_Crystal_Prison_DemonEvent_2-8105, Distance2d: 8.03618, CollisionRadius: 14.90234, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
                    new InteractWithGizmoCoroutine(SNOQuest.x1_Pand_Ext_240_Cellar_04Demon_Event, SNOWorld.X1_Pand_Ext_Cellar_D, SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_2, 0, 5),

                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(139, 69, -13)),
                    new InteractWithGizmoCoroutine(SNOQuest.x1_Pand_Ext_240_Cellar_04Demon_Event, SNOWorld.X1_Pand_Ext_Cellar_D, SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_2, 0, 5),


                    new ClearAreaForNSecondsCoroutine(SNOQuest.x1_Pand_Ext_240_Cellar_04Demon_Event, 60, 0, 0, 45),

                    // by Chest
                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(144, 127, -13)),
                }
            });

            // A5 - The Cursed Crystals (SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals)
            // Portal and crystals keep changing actor ids, is this a dynamic Bounty?
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals,
                Act = Act.A5,
                WorldId = SNOWorld.X1_Pand_Ext_Cellar_D, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.X1_Pand_Ext_2_Battlefields,
                Coroutines = new List<ISubroutine>
                {
                    //ActorId: SNOActor.g_Portal_RectangleTall_Blue, Type: Gizmo, Name: g_Portal_RectangleTall_Blue-47533, Distance2d: 10.84147, 
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals, SNOWorld.X1_Pand_Ext_2_Battlefields, -1551729968),
					new EnterLevelAreaCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals, SNOWorld.X1_Pand_Ext_2_Battlefields, SNOWorld.X1_Pand_Ext_Cellar_D, -1551729968, new SNOActor[]{SNOActor.g_Portal_RectangleTall_Blue, SNOActor.g_Portal_ArchTall_Blue}),


                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(147, 188, -13)),
                    // x1_Fortress_Crystal_Prison_DemonEvent_1 (SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_1) Distance: 31.76865
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals, SNOWorld.X1_Pand_Ext_Cellar_D, SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_1, 0, 5),

                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(175, 72, -13)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals, SNOWorld.X1_Pand_Ext_Cellar_D, SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_2, 0, 5),

                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(142, 74, -13)),
                    new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals, SNOWorld.X1_Pand_Ext_Cellar_D, SNOActor.x1_Fortress_Crystal_Prison_DemonEvent_3, 0, 5),

                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals, 120, 0, 0, 45),

                    // by Chest
                    new MoveToPositionCoroutine(SNOWorld.X1_Pand_Ext_Cellar_D, new Vector3(144, 127, -13)),
                }
            });

            // A5 - Bounty: Grave Situation (SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.X1_Westm_Graveyard_DeathOrb,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 2912417),
//                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 331951),
                    //new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 3000),
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13"),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(79.35699f, 47.7887f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(89.59161f, 69.89282f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(64.90564f, 82.27252f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(66.88336f, 34.40729f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(77.21259f, 48.01953f, 0.1f)),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(79.35699f, 47.7887f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(89.59161f, 69.89282f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(64.90564f, 82.27252f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(66.88336f, 34.40729f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_13", new Vector3(77.21259f, 48.01953f, 0.1f)),

                    //new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, 60, 331951, 2912417, 30),
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_westmarchGuard_Melee_A_01_Graveyard_Soldier_Holdout, 0, 5),
                }
            });

            // A5 - Cryptology (SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology,
                Act = Act.A5,
                WorldId = SNOWorld.x1_westm_Graveyard_DeathOrb,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.X1_Westm_Graveyard_DeathOrb,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, 2912417),
                    // x1_westmarchFemale_A_Graveyard_Unique_1 (SNOActor.x1_westmarchFemale_A_Graveyard_Unique_1) Distance: 8.440428
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_westmarchFemale_A_Graveyard_Unique_1, 2912417, 5),

                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_06", new Vector3(46.19928f, 72.97696f, 0.6203882f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_06", new Vector3(47.20917f, 90.77298f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, "x1_westm_graveyard_NSEW_06", new Vector3(82.65125f, 81.61206f, 0.1f)),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, 5000),
//                    new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, 60, 0, 0, 45),
                    // x1_Westm_Graveyard_Ghost_Female_01_UniqueEvent (SNOActor.x1_Westm_Graveyard_Ghost_Female_01_UniqueEvent) Distance: 5.017566
                    new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, SNOActor.x1_Westm_Graveyard_Ghost_Female_01_UniqueEvent, 0, 5),
                    new WaitCoroutine(SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, SNOWorld.x1_westm_Graveyard_DeathOrb, 10000),
                }
            });
        }
		
        private static void AddNewBounties()
        {
            // A1 - The Cursed Mill (SNOQuest.X1_Bounty_A1_Fields_Event_CultistLegion)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A1_Fields_Event_CultistLegion,
                Act = Act.A1,
                WorldId = SNOWorld.trOUT_Town,
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = SNOLevelArea.A1_trOut_TristramFields_A,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_CultistLegion, SNOWorld.trOUT_Town, 2912417, true),
						
						// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 6.46348
						new MoveToActorCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_CultistLegion, SNOWorld.trOUT_Town, SNOActor.x1_Global_Chest_CursedChest_B),
						
						// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 6.46348
						new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A1_Fields_Event_CultistLegion, SNOWorld.trOUT_Town, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),
						
                        new ClearAreaForNSecondsCoroutine (SNOQuest.X1_Bounty_A1_Fields_Event_CultistLegion, 60, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30, false)
                    }
            });

            // A2 - Bounty: The Cursed Temple (SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Church_Level_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Church_Level_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01, SNOWorld.p6_Church_Level_01, 2912417, true),
					
					//new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01, SNOWorld.p6_Church_Level_01, "p6_Church_NSW_01"),
					//new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01, SNOWorld.p6_Church_Level_01, "p6_Church_NSW_01", new Vector3(131.3778f, 125.112f, -9.592178f)),
					// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 11.98459
					//new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01, SNOWorld.p6_Church_Level_01, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),
					new InteractionCoroutine(SNOActor.x1_Global_Chest_CursedChest_B, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 11.98422
					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01, SNOWorld.p6_Church_Level_01, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),

					new ClearAreaForNSecondsCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_CursedChest_01, 40, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30),
				}
			});

            // A2 - Bounty: Kill Agustin The Marked (SNOQuest.P6_Bounty_A2_Moors_Kill_Birdie)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A2_Moors_Kill_Birdie,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Moor_01,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
                Coroutines = new List<ISubroutine>
                {
				   new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moors_Kill_Birdie, SNOWorld.p6_Moor_01, -2033595591, false),
                   // Name: p6_RavenFlyer_Unique_A-23893 ActorSnoId: p6_RavenFlyer_Unique_A, Distance: 4.847482
                   new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A2_Moors_Kill_Birdie, SNOWorld.p6_Moor_01, SNOActor.p6_RavenFlyer_Unique_A, -2033595591),
                   new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A2_Moors_Kill_Birdie)
                }
            });

            // A2 - Bounty: Kill Vidian (SNOQuest.p6_Bounty_A2_Church_Kill_Envy) // Noin
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.p6_Bounty_A2_Church_Kill_Envy,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Church_Level_02, // Enter the final worldId here
				//LevelAreaIds = new HashSet<int> { P6_Envy_boss },
                QuestType = BountyQuestType.KillBossBounty,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Church_Level_01,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine (SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_01, -1355958932, true),
					new ExplorationCoroutine(new HashSet<SNOLevelArea>() { SNOLevelArea.A2_p6_Church_Level_01 }, breakCondition: () => (Core.Player.LevelAreaId != SNOLevelArea.A2_p6_Church_Level_01 || BountyHelpers.ScanForMarker(-1355958932, 50) != null || ActorFinder.FindActor(SNOActor.g_Portal_ArchTall_Orange,-1355958932) != null)),
		   			// new WaitCoroutine(SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_01, 3000),
					 
					//new MoveToSceneCoroutine(SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02"),
					//new MoveToScenePositionCoroutine(SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02", new Vector3(83.56152f, 120.0135f, 0.1f)),
					 
					 // g_Portal_ArchTall_Orange (SNOActor.g_Portal_ArchTall_Orange) Distance: 46.05756
					 new MoveToActorCoroutine(SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_01, SNOActor.g_Portal_ArchTall_Orange),
					//new ClearAreaForNSecondsCoroutine (SNOQuest.p6_Bounty_A2_Church_Kill_Envy, 20, 0, -1355958932, 45),
					
                     new EnterLevelAreaCoroutine(SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_01, SNOWorld.p6_Church_Level_02, -1355958932, SNOActor.g_Portal_ArchTall_Orange),
                     new MoveToMapMarkerCoroutine (SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_02, 1944200381),
                     new EnterLevelAreaCoroutine(SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_02, SNOWorld.p6_Church_Level_02_Boss, 1944200381, SNOActor.P6_Envy_Bossportal),
                    // Type: Monster Name: p6_Shepherd_Boss-32030 ActorSnoId: p6_Shepherd_Boss
                    //new KillUniqueMonsterCoroutine (SNOQuest.p6_Bounty_A2_Church_Kill_Envy, SNOWorld.p6_Church_Level_02, (int)SNOActor.p6_Shepherd_Boss, 0),
                    new ClearLevelAreaCoroutine(SNOQuest.p6_Bounty_A2_Church_Kill_Envy),
                }
            });
			
			
            // A2 - Bounty: The Cursed Lake (SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine (SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 2912417),
                    new InteractWithGizmoCoroutine (SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02,SNOWorld.p6_Moor_01, SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 5),
					
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(127.2107f, 190.3175f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(148.1393f, 187.8151f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(138.9342f, 220.3177f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(122.736f, 220.217f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(126.5206f, 184.5174f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(143.0248f, 186.709f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(135.8276f, 217.3705f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(118.9543f, 221.3475f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_02", new Vector3(127.2859f, 201.4205f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, SNOWorld.p6_Moor_01, 5000),
					
//                  new ClearAreaForNSecondsCoroutine (SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_02, 60, (int)SNOActor.x1_Global_Chest_CursedChest_B, 2912417, 30)
                }
            });

            // A2 - Bounty: Kill Bain (SNOQuest.P6_Bounty_A2_Church_Kill_Bain)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A2_Church_Kill_Bain,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Church_Level_01,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Church_Level_01,
                Coroutines = new List<ISubroutine>
                {
                    // a2dun_Aqd_GodHead_Door_LargePuzzle-8357
                    new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Church_Kill_Bain, SNOWorld.p6_Church_Level_01, 632612270),
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A2_Church_Kill_Bain, SNOWorld.p6_Church_Level_01, 0, 632612270),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A2_Church_Kill_Bain),
                }
            });

            // A2 - Bounty: Clear the Forgotten Well (SNOQuest.p6_Bounty_A2_Moors_Clear_Cave_Well)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.p6_Bounty_A2_Moors_Clear_Cave_Well,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Moors_Cave_Well_01,
                QuestType = BountyQuestType.ClearZone,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.p6_Bounty_A2_Moors_Clear_Cave_Well, SNOWorld.p6_Moor_01, -1484677704, true),
                    // g_Portal_Circle_Blue-32941 
                    new EnterLevelAreaCoroutine(SNOQuest.p6_Bounty_A2_Moors_Clear_Cave_Well, SNOWorld.p6_Moor_01, SNOWorld.p6_Moors_Cave_Well_01, -1484677704, SNOActor.g_Portal_Circle_Blue),
                    new ClearLevelAreaCoroutine(SNOQuest.p6_Bounty_A2_Moors_Clear_Cave_Well)
                }
            });

			// A2 - Bounty: The Cursed Moors (SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
//				StartingLevelAreaId = A2_p6_Moor_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 2912417),

					// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 12.33186
					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(56.06293f, 41.90753f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(44.28119f, 61.59918f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(57.70996f, 72.89557f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(67.19409f, 50.25159f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(54.0108f, 43.41974f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(42.70831f, 63.90674f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(54.65527f, 74.69647f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(73.12183f, 51.50494f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(55.69476f, 39.52325f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_06", new Vector3(47.83008f, 53.68457f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moors_Event_CursedChest_01, SNOWorld.p6_Moor_01, 5000),

				}
			});

			// A2 - Bounty: Break A Few Eggs (SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_RavenEggs)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_RavenEggs,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Moor_01,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_RavenEggs, SNOWorld.p6_Moor_01, 2912417),
					
					new MoveToActorCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_RavenEggs, SNOWorld.p6_Moor_01, SNOActor.x1_Graveyard_GraveRobert),
                    new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_RavenEggs, SNOWorld.p6_Moor_01, 5000),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_RavenEggs, 60, 0, 0, 30),
                }
            });


			// A2 - Bounty: Blood Statue (SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
				Coroutines = new List<ISubroutine>
				{
					//new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 2912417),
					new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01"),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(122.5996f, 153.1941f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(155.3887f, 135.8263f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(120.1375f, 145.9866f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(93.00769f, 140.5725f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(119.4483f, 146.8486f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					
					// p6_Moor_Event_Statue_Destruction (p6_Moor_Event_Statue_Destruction) Distance: 21.87882
					new MoveToActorCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, SNOActor.p6_Moor_Event_Statue_Destruction),
					new AttackCoroutine(SNOActor.p6_Moor_Event_Statue_Destruction),
										
					// p6_Moor_Event_Statue_Destruction (p6_Moor_Event_Statue_Destruction) Distance: 21.87882
					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, SNOActor.p6_Moor_Event_Statue_Destruction, 0, 5),
					
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(122.5996f, 153.1941f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(155.3887f, 135.8263f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(120.1375f, 145.9866f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(93.00769f, 140.5725f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_01", new Vector3(119.4483f, 146.8486f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCaravan, SNOWorld.p6_Moor_01, 3000),
				}
			});

			// A2 - Bounty: Grave Mistakes (SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B, SNOWorld.p6_Moor_01, 2912417),
//                 new MoveToActorCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B, SNOWorld.p6_Moor_01, (int)SNOActor.p6_Event_Moor_GraveRobbers_graveDigger_A),
					//new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B, SNOWorld.p6_Moor_01, "p6_Moor_NW_01_Event"),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B, SNOWorld.p6_Moor_01, "p6_Moor_NW_01_Event", new Vector3(49.91138f, 52.32275f, -9.900001f)),
                    new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B, SNOWorld.p6_Moor_01, 5000),
                    new ClearAreaForNSecondsCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B, 60, 0, 2912417, 50)
                }
            });

			// A2 - Bounty: The Hematic Key (SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Church_Level_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Church_Level_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, 2912417),
                    //new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_B, SNOWorld.p6_Moor_01, 2000),
					new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02"),
					
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02", new Vector3(80.28949f, 119.7857f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02", new Vector3(82.9693f, 93.61859f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(0, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02", new Vector3(103.1648f, 118.6144f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(0, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02", new Vector3(81.62305f, 138.122f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(0, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02", new Vector3(62.03271f, 118.5316f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, "p6_Church_S_Exit_02", new Vector3(80.28949f, 119.7857f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_Blood_Door, SNOWorld.p6_Church_Level_01, 5000),
				}
			});

			// A2 - Bounty: Off The Wagon (SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, 2912417),
					new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, "p6_Moor_NSE_02"),
					
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, "p6_Moor_NSE_02", new Vector3(97.43164f, 97f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, "p6_Moor_NSE_02", new Vector3(65.58057f, 99.25671f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, "p6_Moor_NSE_02", new Vector3(92.7085f, 113.7571f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, "p6_Moor_NSE_02", new Vector3(63.31616f, 114.4441f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, "p6_Moor_NSE_02", new Vector3(68.448f, 97.45221f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, "p6_Moor_NSE_02", new Vector3(90.53149f, 99.51135f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, SNOWorld.p6_Moor_01, 5000),

//					new ClearAreaForNSecondsCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Wagon, 20, 0, 2912417, 30),
				}
			});

            // A2 - Bounty: Kill Seanus Greyback (SNOQuest.P6_Bounty_A2_Moors_Kill_SeanusGreyback)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A2_Moors_Kill_SeanusGreyback,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moors_Kill_SeanusGreyback, SNOWorld.p6_Moor_01, -840003354, true),
					
                   // Name: P6_Werewolf_White_Unique_A-691 ActorSnoId: P6_Werewolf_White_Unique_A
					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A2_Moors_Kill_SeanusGreyback, SNOWorld.p6_Moor_01, SNOActor.P6_Werewolf_White_Unique_A, -840003354),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A2_Moors_Kill_SeanusGreyback),
                }
            });

			// A2 - Bounty: Blood Collection (SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, 2912417, true),
					new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_03"),
					
					// p6_Event_Moor_BloodCollection_Altar (p6_Event_Moor_BloodCollection_Altar) Distance: 22.97623
					//new MoveToActorCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, p6_Event_Moor_BloodCollection_Altar),	
					// p6_Event_Moor_BloodCollection_Altar (p6_Event_Moor_BloodCollection_Altar) Distance: 22.97623
					//new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, p6_Event_Moor_BloodCollection_Altar, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_03", new Vector3(143.6005f, 104.3372f, 10.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_03", new Vector3(156.6248f, 100.9734f, 10.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_03", new Vector3(160.0651f, 125.4788f, 10.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_03", new Vector3(142.764f, 132.5328f, 10.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_240_03", new Vector3(143.2507f, 108.7601f, 10.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, 5000),

					// p6_Event_Moor_BloodCollection_Altar (p6_Event_Moor_BloodCollection_Altar) Distance: 58.35899
                    new InteractionCoroutine(SNOActor.p6_Event_Moor_BloodCollection_Altar, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					// p6_Event_Moor_BloodCollection_Altar (p6_Event_Moor_BloodCollection_Altar) Distance: 10.84266
					//new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_BloodCollection, SNOWorld.p6_Moor_01, (int)SNOActor.p6_Event_Moor_BloodCollection_Altar, 0, 5),

				}
			});

			// A2 - Bounty: Kill Elizar Bathory (SNOQuest.P6_Bounty_A2_Church_Kill_TempleCultist_Caster_A)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Church_Kill_TempleCultist_Caster_A,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Church_Level_01, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Church_Level_01,
				Coroutines = new List<ISubroutine>
				{
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Church_Kill_TempleCultist_Caster_A, SNOWorld.p6_Church_Level_01, -1213009173),

					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A2_Church_Kill_TempleCultist_Caster_A, SNOWorld.p6_Church_Level_01, 0, -1213009173),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A2_Church_Kill_TempleCultist_Caster_A),
				}
			});


			// A2 - Bounty: Kill Lupgaron (SNOQuest.P6_Bounty_A2_Moors_Kill_Lupgaron)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Moors_Kill_Lupgaron,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
				Coroutines = new List<ISubroutine>
				{
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moors_Kill_Lupgaron, SNOWorld.p6_Moor_01, -840003353),

					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A2_Moors_Kill_Lupgaron, SNOWorld.p6_Moor_01, 0, -840003353),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A2_Moors_Kill_Lupgaron),
				}
			});

            // A2 - Bounty: One In The Hand (SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Moor_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Moor_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 2912417, true),
					new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04"),

//					new ClearAreaForNSecondsCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, 30, 0, 2912417, 30),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(59.41333f, 83.82306f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(77.81445f, 87.34912f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(64.24573f, 103.5694f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(48.74854f, 90.33655f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(59.41333f, 83.82306f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(77.81445f, 87.34912f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(64.24573f, 103.5694f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, "p6_Moor_NSEW_04", new Vector3(48.74854f, 90.33655f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Moor_Event_GraveRobbers_Chest, SNOWorld.p6_Moor_01, 5000),
				}
			});

			// A2 - Bounty: Ascension Ritual (SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual,
                Act = Act.A2,
                WorldId = SNOWorld.p6_Church_Level_01,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Church_Level_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, 2912417),
                    new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, "p6_Church_SEW_02", new Vector3(71.31311f, 118.5031f, 0.1000002f)),
					
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, "p6_Church_SEW_02", new Vector3(100.03f, 120.3733f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, "p6_Church_SEW_02", new Vector3(101.3124f, 101.2228f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, "p6_Church_SEW_02", new Vector3(133.4077f, 124.4439f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, "p6_Church_SEW_02", new Vector3(101.0634f, 138.1555f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, "p6_Church_SEW_02", new Vector3(74.04614f, 117.9331f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, "p6_Church_SEW_02", new Vector3(100.03f, 120.3733f, 0.1000002f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, SNOWorld.p6_Church_Level_01, 5000),
					
                    // Name: P6_Church_BloodPool-1277 ActorSnoId: P6_Church_BloodPool
//                    new ClearAreaForNSecondsCoroutine(SNOQuest.P6_Bounty_A2_Church_Event_AscensionRitual, 60, (int)SNOActor.P6_Church_BloodPool, 0)
                }
            });
            

			// A4 - Bounty: Kill Jarryd (SNOQuest.P6_Bounty_A4_RoF_V4_Clear_festeringWoods)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V4_Clear_festeringWoods,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V4, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_01,
                Coroutines = new List<ISubroutine>
                {
                    // Name: p6_Skeleton_B_Unique_RoF_V4_01-18093 ActorSnoId: p6_Skeleton_B_Unique_RoF_V4_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_festeringWoods, SNOWorld.Lost_Souls_Prototype_V4, SNOActor.p6_Skeleton_B_Unique_RoF_V4_01, 352361040),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_festeringWoods),
                }
            });

			// A4 - Bounty: Kill Argosh (SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Demons)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Demons,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V2,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_02_Section_01,
                Coroutines = new List<ISubroutine>
                {
                    // Name: p6_Unburied_C_Unique_01_ROF_V2_01-1437 ActorSnoId: 4707559
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Demons, SNOWorld.Lost_Souls_Prototype_V2, (SNOActor)4707559, 881692424),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Demons),
                }
            });

			// A4 - Bounty: Kill Bjortor (SNOQuest.P6_Bounty_A4_RoF_V2_Kill_Sasquatch)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V2_Kill_Sasquatch,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V2,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_trDun_Cath_EW_Hall_01b,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Kill_Sasquatch, SNOWorld.Lost_Souls_Prototype_V2, -1254225905),
                    // P6_Sasquatch_B_Unique_RoF_01-17925 ActorSnoId: P6_Sasquatch_B_Unique_RoF_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Kill_Sasquatch, SNOWorld.Lost_Souls_Prototype_V2, SNOActor.P6_Sasquatch_B_Unique_RoF_01, -1254225905),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Kill_Sasquatch),
                }
            });

			// A4 - Bounty: Kill Gro'Mag (SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Cemetary)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Cemetary,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V5,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_05_Section_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Cemetary, SNOWorld.Lost_Souls_Prototype_V5, "LS_a4dun_spire_LibraryOfFate_05", new Vector3(117.0737f, 229.1764f, 31.1f)),
                    new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Cemetary, SNOWorld.Lost_Souls_Prototype_V5, "LS_a4dun_spire_corrupt_NS_02", new Vector3(226.8757f, 92.24744f, 31.1f)),
                    // p6_x1_westmarchRanged_A_Unique_ROF_V5_01-75810 ActorSnoId: p6_x1_westmarchRanged_A_Unique_ROF_V5_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Cemetary, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p6_x1_westmarchRanged_A_Unique_ROF_V5_01, 465303410),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Cemetary),
                }
            });

			// A4 - Bounty: Kill Janderson (SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Skeleton_B)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Skeleton_B,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V5, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_Labyrinth_01b,
                Coroutines = new List<ISubroutine>
                {
                    // Name: P6_Skeleton_B_Unique_01_RoF-5950 ActorSnoId: P6_Skeleton_B_Unique_01_RoF
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Skeleton_B, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.P6_Skeleton_B_Unique_01_RoF, 243521159),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Skeleton_B),
                }
            });

			// A4 - Bounty: Kill Jadtalek (SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Labyrinth)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Labyrinth,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V5, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_Labyrinth_01b,
				Coroutines = new List<ISubroutine>
				{
			//		new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Labyrinth, SNOWorld.Lost_Souls_Prototype_V5, 1032245578),
							
					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Labyrinth, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p6_Ghoul_A_Unique_RoF_V5_01, 1032245578),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Labyrinth),
				}
			});

			// A4 - Bounty: Kill Barfield (SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Cathedral)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Cathedral,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V2, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_trDun_Cath_EW_Hall_01b,
				Coroutines = new List<ISubroutine>
				{
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Cathedral, SNOWorld.Lost_Souls_Prototype_V2, 1096650244),

					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Cathedral, SNOWorld.Lost_Souls_Prototype_V2, 0, 1096650244),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Cathedral),
				}
			});

			// A4 - Bounty: Kill Barrigast (SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V3,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_03,
                Coroutines = new List<ISubroutine>
                {
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(186.2926f, 66.6853f, -74.9f)),
					new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, 1),
                    // p6_Angel_Corrupt_A_Unique_RoF_V3_01-22241 ActorSnoId: p6_Angel_Corrupt_A_Unique_RoF_V3_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.p6_Angel_Corrupt_A_Unique_RoF_V3_01, -856974543),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb)
                }
            });

			// A4 - Bounty: Kill Strychnos (SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Catacombs)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Catacombs,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V4, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_01,
				Coroutines = new List<ISubroutine>
				{
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Catacombs, SNOWorld.Lost_Souls_Prototype_V4, 1111559942),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Catacombs, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(32.23874f, 35.65826f, 0.1f)),

					// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 5.764746
					new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, 1),
					

					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Catacombs, SNOWorld.Lost_Souls_Prototype_V4, SNOActor.p6_QuillDemon_C_Unique_RoF_V4_01, 1111559942),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Catacombs),
				}
			});

			// A4 - Bounty: The Cursed Realm (SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V4, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_01,
				Coroutines = new List<ISubroutine>
				{
						new MoveToSceneCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02"),

						// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 13.48283
						new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),

						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(27.49054f, 20.08649f, 0.1f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(7.732239f, 25.5159f, 0.09999999f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(10.12387f, 51.27966f, 0.1f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(35.59381f, 48.27234f, 0.1000001f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(24.9248f, 20.7507f, 0.1f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_SEW_03", new Vector3(116.2039f, 22.84851f, 0.1f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(4.463196f, 49.94196f, 0.1f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(31.73489f, 47.32837f, 0.1507125f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),
						new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(30.27902f, 32.13583f, 0.1f)),
						new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V4, 5000),

				}
			});

			// A4 - Bounty: The Cursed Realm (SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V2, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_02_Section_01,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 2912417),

					// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 13.40324
					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(61.92737f, 204.1401f, 0.1000029f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(76.85905f, 184.4044f, 0.1935999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(94.93716f, 203.6592f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(72.48926f, 228.8552f, 0.1057936f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(46.67148f, 231.2432f, 0.1000032f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(60.69748f, 204.556f, 0.1000011f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(76.85905f, 184.4044f, 0.1935999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(94.93716f, 203.6592f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(72.48926f, 228.8552f, 0.1057936f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(46.67148f, 231.2432f, 0.1000032f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, "LS_a3dun_Crater_SW_01_W01_N01", new Vector3(60.69748f, 204.556f, 0.1000011f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V2, 5000),
				}
			});
   

			// A4 - Bounty: Kill Prratshet the Reaper (SNOQuest.P6_Bounty_A4_RoF_V3_Clear_HexMaze)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V3_Clear_HexMaze,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V3,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_03,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_HexMaze, SNOWorld.Lost_Souls_Prototype_V3, -1208602370, false),
                    new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_HexMaze, SNOWorld.Lost_Souls_Prototype_V3, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(184.2814f, 72.55072f, -74.9f)),
					//new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_HexMaze, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.x1_Fortress_Portal_Switch, 0, 5),
					new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, 1),
					
                    // p6_Angel_Corrupt_A_Unique_RoF_V3_01-22241 ActorSnoId: p6_Angel_Corrupt_A_Unique_RoF_V3_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_HexMaze, SNOWorld.Lost_Souls_Prototype_V3,  SNOActor.p6_Angel_Corrupt_A_Unique_RoF_V3_01, -1208602370),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_HexMaze)
                }
            });

			// A4 - Bounty: Kill Necronom (SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V4, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_01,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext, SNOWorld.Lost_Souls_Prototype_V4, 1900506513, false),
//					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(32.23874f, 35.65826f, 0.1f)),
//					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext, SNOWorld.Lost_Souls_Prototype_V4, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),
					
                    // Name: p6_X1_armorScavenger_Unique_RoF_V4_01-809 ActorSnoId: p6_X1_armorScavenger_Unique_RoF_V4_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext, SNOWorld.Lost_Souls_Prototype_V4, SNOActor.p6_X1_armorScavenger_Unique_RoF_V4_01, 1900506513),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext)
                }
            });

			// A4 - Bounty: Kill Silli (SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V3, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_03,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm, SNOWorld.Lost_Souls_Prototype_V3, -454310883),
					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm, SNOWorld.trOUT_Town, SNOActor.p6_DeathMaiden_Unique_RoF_V3, -454310883),


					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm, SNOWorld.Lost_Souls_Prototype_V3, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(185.975f, 66.30786f, -74.9f)),
                    new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm, SNOWorld.Lost_Souls_Prototype_V3, 1),
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 5.548859
					//new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.x1_Fortress_Portal_Switch, 0, 5),		
					//new InteractionCoroutine((int)SNOActor.x1_Fortress_Portal_Switch, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm),
				}
			});

			// A4 - Bounty: Kill Guytan Pyrrus (SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Crater)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Crater,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V2,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_02_Section_01,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Crater, SNOWorld.Lost_Souls_Prototype_V2, -174945948, true),
                    // Name: p6_GoatMutant_Shaman_B_Unique_RoF_V2_01-1067 ActorSnoId: p6_GoatMutant_Shaman_B_Unique_RoF_V2_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Crater, SNOWorld.Lost_Souls_Prototype_V2, SNOActor.p6_GoatMutant_Shaman_B_Unique_RoF_V2_01, -174945948),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Crater)
                }
            });

			// A4 - Bounty: Kill Serpentor (SNOQuest.P6_Bounty_A4_RoF_V4_Kill_Squigglet)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V4_Kill_Squigglet,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V4,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_01,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Kill_Squigglet, SNOWorld.Lost_Souls_Prototype_V4, -777267042, true),
                    // Name: P6_x1_Squigglet_A_Unique_RoF-1024 ActorSnoId: P6_x1_Squigglet_A_Unique_RoF, 
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Kill_Squigglet, SNOWorld.Lost_Souls_Prototype_V4, SNOActor.P6_x1_Squigglet_A_Unique_RoF, -777267042),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Kill_Squigglet)
                }
            });

			// A4 - Bounty: Kill the Manipulator of Lore (SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons)
			Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V3,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_03,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons, SNOWorld.Lost_Souls_Prototype_V3, 619535446, false),
					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.p6_x1_Wraith_Unique_A_Unique_ROF_V3_01, 619535446),
					
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons, SNOWorld.Lost_Souls_Prototype_V3, 619535446),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons, SNOWorld.Lost_Souls_Prototype_V3, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(186.7634f, 76.08228f, -74.89999f)),
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 6.733983
					//new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.x1_Fortress_Portal_Switch, 0, 5),
					new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, 1),

					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Demons),
                }
            });

			// A4 - Bounty: Kill Old Hardshell (SNOQuest.P6_Bounty_A4_RoF_V5_Clear_BogCave)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V5_Clear_BogCave,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V5, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_Labyrinth_01b,
                Coroutines = new List<ISubroutine>
                {
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_BogCave, SNOWorld.Lost_Souls_Prototype_V5, 1898390417, false),
                    // Name: p6_Crab_Mother_Unique_RoF_V5_01-10653 ActorSnoId: p6_Crab_Mother_Unique_RoF_V5_01
                    new KillUniqueMonsterCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_BogCave, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p6_Crab_Mother_Unique_RoF_V5_01, 1898390417),
                    new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Clear_BogCave)
                }
            });

			// A4 - Bounty: The Cursed Realm (SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V3, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_03,
				Coroutines = new List<ISubroutine>
				{
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(186.2604f, 66.41125f, -74.89999f)),
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 5.298114
					new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, 1),

					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 2912417),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(133.6786f, 120.2815f, -114.7412f)),
					// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 15.23479
					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.x1_Global_Chest_CursedChest_B, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(139.3061f, 103.1571f, -114.0964f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(155.2884f, 106.2856f, -114.5432f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(135.0764f, 131.937f, -114.8946f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(116.3545f, 127.3228f, -114.4024f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(137.498f, 110.136f, -114.5155f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(132.6483f, 86.73944f, -113.9986f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(154.9095f, 98.44647f, -114.9f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(147.0152f, 133.1912f, -112.8296f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(118.5731f, 127.9306f, -114.5093f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, "Lost_Souls_x1_westm_deathorb_B_W01_N01", new Vector3(129.8547f, 110.9499f, -113.8139f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Event_CursedChest01, SNOWorld.Lost_Souls_Prototype_V3, 5000),

				}
			});

			// A4 - Bounty: Kill Reyes (SNOQuest.P6_Bounty_A4_RoF_V3_Kill_MastaBlasta_Rider)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V3_Kill_MastaBlasta_Rider,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V3, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_03,
				Coroutines = new List<ISubroutine>
				{
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Kill_MastaBlasta_Rider, SNOWorld.Lost_Souls_Prototype_V3, -1860097213),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Kill_MastaBlasta_Rider, SNOWorld.Lost_Souls_Prototype_V3, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(185.975f, 66.09912f, -74.9f)),
					// x1_Fortress_Portal_Switch (SNOActor.x1_Fortress_Portal_Switch) Distance: 2.342543
					//new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Kill_MastaBlasta_Rider, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.x1_Fortress_Portal_Switch, 0, 5),
					new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, 1),

					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V3_Kill_MastaBlasta_Rider, SNOWorld.Lost_Souls_Prototype_V3, SNOActor.p6_morluSpellcaster_A_Unique_RoF_V5_01, -1860097213),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V3_Kill_MastaBlasta_Rider),
				}
			});

			// A4 - Bounty: Kill Mzuuman (SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Spire_Corrupt)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Spire_Corrupt,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V5,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_05_Section_01,
                Coroutines = new List<ISubroutine>
                {
                    // Name: p6_morluSpellcaster_A_Unique_RoF_V5_01-950 ActorSnoId: p6_morluSpellcaster_A_Unique_RoF_V5_01
                    new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Spire_Corrupt, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p6_morluSpellcaster_A_Unique_RoF_V5_01, 2034862812),
                    new ClearLevelAreaCoroutine (SNOQuest.P6_Bounty_A4_RoF_V5_Clear_Spire_Corrupt)
                }
            });

			// A4 - Bounty: Kill Senahde (SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V5, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_Labyrinth_01b,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 1629495090),

					// p4_forest_coast_cave_mermaid_idol (p4_forest_coast_cave_mermaid_idol) Distance: 9.770506
					new MoveToActorCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p4_forest_coast_cave_mermaid_idol),
					// p4_forest_coast_cave_mermaid_idol (p4_forest_coast_cave_mermaid_idol) Distance: 9.770506
					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p4_forest_coast_cave_mermaid_idol, 1629495090, 5),

					// p4_forest_coast_cave_mermaid_idol (449645) Distance: 12.69958
					new MoveToActorCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p4_forest_coast_cave_mermaid_idol),
					// p4_forest_coast_cave_mermaid_idol (449645) Distance: 12.69958
					new InteractWithGizmoCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, SNOActor.p4_forest_coast_cave_mermaid_idol, 0, 5),

//					new ClearAreaForNSecondsCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, 20, 0, 1629495090, 30),

					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(135.1777f, 45.96167f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(133.6121f, 12.90109f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(118.8056f, 36.12866f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(129.1808f, 51.20551f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(135.1777f, 45.96167f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(133.6121f, 12.90109f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(118.8056f, 36.12866f, 0.09999999f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, "LS_px_Cave_A_NSW_01", new Vector3(129.1808f, 51.20551f, 0.1f)),
					new WaitCoroutine(SNOQuest.P6_Bounty_A4_RoF_V5_Kill_Senahde, SNOWorld.Lost_Souls_Prototype_V5, 5000),
				}
			});

			// A4 - Bounty: Kill K'Zigler (SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Demons)
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Demons,
                Act = Act.A4,
                WorldId = SNOWorld.Lost_Souls_Prototype_V4, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_a4dun_spire_LibraryOfFate_01,
                Coroutines = new List<ISubroutine>
                {
					new MoveToScenePositionCoroutine(SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Pand_Ext, SNOWorld.Lost_Souls_Prototype_V4, "Lost_Souls_x1_Pand_Ext_120_Edge_NEW_02", new Vector3(32.23874f, 35.65826f, 0.1f)),

					// x1_Global_Chest_CursedChest_B (SNOActor.x1_Global_Chest_CursedChest_B) Distance: 5.764746
					new MoveThroughDeathGates(SNOQuest.P6_Bounty_A4_RoF_V3_Clear_Westm_Deathorb, SNOWorld.Lost_Souls_Prototype_V3, 1),
					
                     // Name: p6_X1_armorScavenger_A_Unique_ROF_V4_01-4477 ActorSnoId: p6_X1_armorScavenger_A_Unique_ROF_V4_01
                    new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Demons, SNOWorld.Lost_Souls_Prototype_V4, SNOActor.p6_X1_armorScavenger_A_Unique_ROF_V4_01, -541902607),
                    new ClearLevelAreaCoroutine (SNOQuest.P6_Bounty_A4_RoF_V4_Clear_Demons)
                }
            });

			// A5 - Bounty: Brutal Assault (SNOQuest.X1_Bounty_A5_PandFortress_Event_BrutalAssault)
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_BrutalAssault,
				Act = Act.A5,
				WorldId = SNOWorld.x1_fortress_level_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.x1_fortress_level_01,
				Coroutines = new List<ISubroutine>
				{
                    new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_BrutalAssault, SNOWorld.x1_fortress_level_01, "x1_fortress_NE_05_soul_well", new Vector3(91.4856f, 196.1569f, 20.1f)),
                    new MoveThroughDeathGates(SNOQuest.X1_Bounty_A5_PandFortress_Event_BrutalAssault, SNOWorld.x1_fortress_level_01, 1),
					
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_BrutalAssault, SNOWorld.x1_fortress_level_01, 2912417),
					
					// x1_Global_Chest_Locked (358656) Distance: 17.29929
                    new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_BrutalAssault, SNOWorld.x1_fortress_level_01, SNOActor.x1_Global_Chest_Locked),

					new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_BrutalAssault, 30, SNOActor.x1_Global_Chest_Locked, 2912417, 30),
				}
			});
			
////================================ add new bounties by 斗靑 =============================
			
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Forest,
				Act = Act.A4,
				WorldId = SNOWorld.Lost_Souls_Prototype_V2, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A4_P6_RoF_trDun_Cath_EW_Hall_01b,
				Coroutines = new List<ISubroutine>
				{
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Forest, SNOWorld.Lost_Souls_Prototype_V2, 1719291897),

                    // Name: p6_RatKing_B_Unique_RoF_V2_01-2344 ActorSnoId: p6_RatKing_B_Unique_RoF_V2_01
					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Forest, SNOWorld.Lost_Souls_Prototype_V2, SNOActor.p6_RatKing_B_Unique_RoF_V2_01, 1719291897),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A4_RoF_V2_Clear_Forest),
				}
			});
			
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P4_Bounty_A3_ForestSnow_Kill_Unique_Howler,
				Act = Act.A3,
				WorldId = SNOWorld.p4_Forest_Snow_01, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A3_dun_ruins_frost_city_A_01,
				Coroutines = new List<ISubroutine>
				{
					new EnterLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Kill_Unique_Howler, SNOWorld.a3dun_ruins_frost_city_A_01, SNOWorld.p4_Forest_Snow_01, -1665315172, SNOActor.g_Portal_Rectangle_Blue),

					new MoveToMapMarkerCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Kill_Unique_Howler, SNOWorld.p4_Forest_Snow_01, 789423692, false),
					
                    // Name: p4_Forest_Snow_ZombieSkinny_A-1092 ActorSnoId: p4_Forest_Snow_ZombieSkinny_A
                    new KillUniqueMonsterCoroutine (SNOQuest.P4_Bounty_A3_ForestSnow_Kill_Unique_Howler, SNOWorld.p4_Forest_Snow_01, SNOActor.p4_Forest_Snow_ZombieSkinny_A, 789423692),
					new ClearLevelAreaCoroutine(SNOQuest.P4_Bounty_A3_ForestSnow_Kill_Unique_Howler),

				}
			});
			
		
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead,
				Act = Act.A5,
				WorldId = SNOWorld.X1_WESTM_ZONE_01, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.X1_WESTM_ZONE_01,
				Coroutines = new List<ISubroutine>
				{
					//new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 2912417),
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05"),

//					new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, 30, 0, 2912417, 30),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(173.5288f, 81.11987f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(153.3018f, 105.2089f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(171.6237f, 122.2426f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(189.8605f, 97.67358f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(173.5288f, 81.11987f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(153.3018f, 105.2089f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(171.6237f, 122.2426f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_EW_05", new Vector3(189.8605f, 97.67358f, 10.1f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheAngeredDead, SNOWorld.X1_WESTM_ZONE_01, 5000),
				}
			});
			
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.P6_Bounty_A2_Church_Kill_TempleMonstrosity_A,
				Act = Act.A2,
				WorldId = SNOWorld.p6_Church_Level_01, // Enter the final worldId here
				QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = SNOLevelArea.A2_p6_Church_Level_01,
				Coroutines = new List<ISubroutine>
				{
//					new MoveToMapMarkerCoroutine(SNOQuest.P6_Bounty_A2_Church_Kill_TempleMonstrosity_A, SNOWorld.p6_Church_Level_01, -175090627),

					new KillUniqueMonsterCoroutine (SNOQuest.P6_Bounty_A2_Church_Kill_TempleMonstrosity_A, SNOWorld.p6_Church_Level_01, SNOActor.p6_TempleMonstrosity_Unique_A, -175090627),
					new ClearLevelAreaCoroutine(SNOQuest.P6_Bounty_A2_Church_Kill_TempleMonstrosity_A),
				}
			});
			
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest,
                Act = Act.A5,
                WorldId = SNOWorld.X1_WESTM_ZONE_01, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.X1_WESTM_ZONE_01,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 2912417),
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05"),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(63.87427f, 47.55865f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(126.0888f, 59.15466f, 0.1000003f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(120.716f, 108.0188f, -4.899998f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(120.3612f, 145.6283f, 0.1000024f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(62.76691f, 178.4937f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(128.4061f, 169.2341f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(133.03f, 107.4254f, -4.829605f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(121.0415f, 58.47754f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(127.6298f, 114.5023f, -4.899999f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(99.05939f, 168.1439f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(52.3009f, 111.2554f, 10.1f)),
					// X1_Westm_Event_TheHarvest_Noble (X1_Westm_Event_TheHarvest_Noble) Distance: 9.714499
					new MoveToActorCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, SNOActor.X1_Westm_Event_TheHarvest_Noble),

					// X1_Westm_Event_TheHarvest_Noble (X1_Westm_Event_TheHarvest_Noble) Distance: 9.714499
					new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, SNOActor.X1_Westm_Event_TheHarvest_Noble, 0, 5),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(180.4313f, 41.80878f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(0, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(175.1393f, 65.50281f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(0, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(176.7676f, 41.88159f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),
					new MoveToScenePositionCoroutine(0, SNOWorld.X1_WESTM_ZONE_01, "x1_westm_NSEW_05", new Vector3(155.5331f, 47.70239f, 0.100001f)),
					new WaitCoroutine(SNOQuest.X1_Bounty_A5_Westmarch_Event_TheHarvest, SNOWorld.X1_WESTM_ZONE_01, 5000),

                }
            });
			
            Bounties.Add(new BountyData
            {
                QuestId = SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison,

                Act = Act.A5,
                WorldId = SNOWorld.x1_fortress_level_02, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = SNOLevelArea.x1_fortress_level_02_islands,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, 2912417),
					new MoveToSceneCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, "x1_fortress_NS_01_AncientJail"),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, "x1_fortress_NS_01_AncientJail", new Vector3(149.0823f, 126.6353f, -9.231695f)),
					// X1_Fortress_NephalemSpirit (SNOActor.X1_Fortress_NephalemSpirit) Distance: 11.29866
					new InteractWithUnitCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.X1_Tristram_Adventure_Mode_Hub, SNOActor.X1_Fortress_NephalemSpirit, 2912417, 5),
					
					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, "x1_fortress_NS_01_AncientJail", new Vector3(164.5552f, 93.61511f, -9.899999f)),
					// x1_Fortress_Crystal_Prison_Blue (x1_Fortress_Crystal_Prison_Blue) Distance: 8.304984
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, SNOActor.x1_Fortress_Crystal_Prison_Blue, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, "x1_fortress_NS_01_AncientJail", new Vector3(126.7645f, 94.18115f, -9.899999f)),
					// x1_Fortress_Crystal_Prison_Blue (x1_Fortress_Crystal_Prison_Blue) Distance: 8.304984
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, SNOActor.x1_Fortress_Crystal_Prison_Blue, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, "x1_fortress_NS_01_AncientJail", new Vector3(128.3707f, 153.6636f, -9.9f)),
					// x1_Fortress_Crystal_Prison_Blue (x1_Fortress_Crystal_Prison_Blue) Distance: 8.304984
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, SNOActor.x1_Fortress_Crystal_Prison_Blue, 0, 5),

					new MoveToScenePositionCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, "x1_fortress_NS_01_AncientJail", new Vector3(166.791f, 153.5845f, -9.9f)),
					// x1_Fortress_Crystal_Prison_Blue (x1_Fortress_Crystal_Prison_Blue) Distance: 8.304984
					new InteractWithGizmoCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, SNOWorld.x1_fortress_level_02, SNOActor.x1_Fortress_Crystal_Prison_Blue, 0, 5),

					new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_PandFortress_Event_AncientPrison, 30, 0, 0, 45),
					
                }
            });
			
			Bounties.Add(new BountyData
			{
				QuestId = SNOQuest.X1_Bounty_A5_WestmarchFire_Event_BrutelyUnfortunate,
				Act = Act.A5,
				WorldId = SNOWorld.X1_WESTM_ZONE_03, // Enter the final worldId here
				QuestType = BountyQuestType.SpecialEvent,
				WaypointLevelAreaId = SNOLevelArea.X1_WESTM_ZONE_03,
				Coroutines = new List<ISubroutine>
				{
					new MoveToMapMarkerCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_BrutelyUnfortunate, SNOWorld.X1_WESTM_ZONE_03, 2912417),
					new ClearAreaForNSecondsCoroutine(SNOQuest.X1_Bounty_A5_WestmarchFire_Event_BrutelyUnfortunate, 60, 0, 2912417, 45),
				}
			});
			
        }
		
        // [Obsolete("Use waypoint factory for levelarea instead")]
        public static readonly Dictionary<SNOQuest, int> QuestWaypointNumbers = new Dictionary<SNOQuest, int>
        {
            { SNOQuest.X1_Bounty_A1_Highlands_Kill_Logrut, 13 },
            { SNOQuest.X1_Bounty_A1_Highlands_Kill_Buras, 13 },
            { SNOQuest.X1_Bounty_A1_Highlands_Kill_Lorzak, 13 },
            { SNOQuest.X1_Bounty_A1_Highlands_Kill_RedRock, 13 },
            { SNOQuest.X1_Bounty_A1_Highlands_Clear_CaveMoonClan2, 13 },
            { SNOQuest.X1_Bounty_A1_Wilderness_Clear_DenOfTheFallen2, 6 },
            { SNOQuest.X1_Bounty_A1_Wilderness_Kill_Mange, 6 },
            { SNOQuest.X1_Bounty_A1_Fields_Clear_ScavengerDen2, 8 },
            { SNOQuest.X1_Bounty_A1_Fields_Kill_Melmak, 8 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Last_Stand, 10 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Event_Eternal_War, 10 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Grimshack, 10 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_CryptOfTheAncients, 10 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Clear_WarriorsRest, 10 },
            { SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Zhelobb, 12 },
            { SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Venimite, 12 },
            { SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Rathlin, 12 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Crassus, 15 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Bludgeonskull, 15 },
            { SNOQuest.X1_Bounty_A1_SpiderCaves_Kill_Qurash, 12 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_GreedyMiner, 19 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Saha, 19 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Taros, 19 },
            { SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Shondar, 22 },
            { SNOQuest.X1_Bounty_A2_Oasis_Clear_AncientCave2, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Clear_FloodedCave2, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Kill_ScarTalon, 23 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Clear_VileCavern2, 24 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Clear_CaveOfBurrowingHorror2, 24 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Kill_Raiha, 24 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Kill_Blarg, 24 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Kill_Bloodfeather, 24 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheArchivist, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Hellscream, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thrum, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_TheTomekeeper, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordSkomora, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordGhuyan, 25 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bricktop, 28 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Bashface, 28 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Marchocyas, 28 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Event_KeepBlacksmith, 29 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Thornback, 29 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDonnAdams, 29 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_CaptainDale, 29 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_TheCrusher, 30 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Belagg, 31 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Gugyn, 31 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Event_TideOfBattle, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_CrydersOutpost, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_CavernsOfFrost2, 33 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_IcefallCaves2, 33 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Groak, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Mehshak, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Shertik, 33 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Emberwing, 33 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Garganug, 37 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Hyrug, 38 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Maggrus, 38 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Charuch, 37 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Mhawgann, 37 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Severclaw, 35 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Valifahr, 38 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Event_CrazyClimber, 37 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Demonika, 36 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Axgore, 39 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Brimstone, 36 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Battlerage, 15 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Treefist, 17 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Boneslag, 17 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_HawthorneGable, 10 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_FecklarsGhost, 10 },
            { SNOQuest.X1_Bounty_A1_Fields_Event_FamilyRathe, 8 },
            { SNOQuest.X1_Bounty_A1_Fields_Clear_KhazraDen, 8 },
            { SNOQuest.X1_Bounty_A1_Fields_Kill_Charger, 8 },
            { SNOQuest.X1_Bounty_A1_Fields_Kill_Dreadclaw, 8 },
            { SNOQuest.X1_Bounty_A1_Cemetery_Kill_Dataminer, 7 },
            { SNOQuest.X1_Bounty_A1_Cemetery_Kill_DiggerODell, 7 },
            { SNOQuest.X1_Bounty_A1_Wilderness_Kill_MiraEamon, 6 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Clear_SiroccoCaverns2, 19 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Gart, 19 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Hemit, 19 },
            { SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Yeth, 22 },
            { SNOQuest.X1_Bounty_A2_Alcarnus_Kill_HighPriestMurdos, 22 },
            { SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Jhorum, 22 },
            { SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Sammash, 22 },
            { SNOQuest.X1_Bounty_A2_Oasis_Clear_MysteriousCave2, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Kill_Bashiok, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Kill_Torsar, 23 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Kill_Plagar, 24 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Rockgut, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordCaustus, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_MageLordFlaydren, 25 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Sotnob, 16 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Axegrave, 30 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Lashtongue, 29 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Aloysius, 30 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_ViciousGrayTurkey, 31 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_ForwardBarracks, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_FortifiedBunker2, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_BattlefieldStores2, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheFoundry2, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheUnderbridge, 34 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Clear_TheBarracks2, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Dreadgrasp, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Ghallem, 33 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Direclaw, 37 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_ShandraHar, 34 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Brutu, 38 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Scorpitox, 39 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Gorog, 39 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Sawtooth, 37 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Gormungandr, 35 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Crabbs, 36 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Riplash, 36 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Gholash, 36 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Haxxor, 36 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_OohTash, 43 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_KaoAhn, 43 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Torchlighter, 41 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Khatun, 41 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Veshan, 41 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_Haures, 45 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_Grimnight, 45 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_SaoThall, 45 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_Kysindra, 44 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_Pyres, 44 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_Slarg, 44 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_RhauKye, 45 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Snitchley, 37 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Kill_Bholen, 30 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Clear_TheRuins2, 21 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Event_RygnarIdol, 21 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Kill_MageLordMisgen, 24 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Kill_InquisitorHamath, 21 },
            { SNOQuest.X1_Bounty_A5_Bog_Kill_Vilepaw, 52 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Kill_Lummock, 34 },
            { SNOQuest.X1_Bounty_A3_Crater_Kill_Growlfang, 35 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectAnguish, 43 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectPain, 43 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_AspectDestruction, 41 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_AspectHatred, 44 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_AspectLies, 44 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_AspectSin, 45 },
            { SNOQuest.X1_Bounty_A4_Spire_Kill_AspectTerror, 45 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Kill_Erdith, 50 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Ragus, 2 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Braluk, 2 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Glidewing, 2 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Merrium, 3 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Cudgelarm, 3 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Firestarter, 3 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainCage, 4 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Killian, 4 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Bellybloat, 4 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_Radnoj, 5 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Kill_CaptainClegg, 5 },
            { SNOQuest.X1_Bounty_A4_HellRift_Clear_HellRift2, 46 },
            { SNOQuest.X1_Bounty_A5_Bog_Kill_Morghum, 52 },
            { SNOQuest.X1_Bounty_A5_Bog_Kill_Fangbite, 52 },
            { SNOQuest.X1_Bounty_A5_Bog_Kill_Slinger, 52 },
            { SNOQuest.X1_Bounty_A5_Bog_Kill_Almash, 52 },
            { SNOQuest.X1_Bounty_A5_Bog_Kill_Tadardya, 52 },
            { SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekTabok, 53 },
            { SNOQuest.X1_Bounty_A5_PathToCorvus_Kill_VekMarru, 53 },
            { SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakSarugg, 54 },
            { SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_NakQujin, 54 },
            { SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariHattar, 54 },
            { SNOQuest.X1_Bounty_A5_RuinsOfCorvus_Kill_BariMoqqu, 54 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Kill_Getzlord, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Kill_Yergacheph, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Kill_KatherineBatts, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Kill_Matanzas, 49 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Kill_Necrosys, 50 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Kill_Purah, 50 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Kill_Targerious, 50 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Micheboar, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Theodosia, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Sumaryss, 51 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Rockulus, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Obsidious, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Slarth, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Burrask, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Watareus, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Baethus, 57 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Kill_Lograth, 55 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Kill_Valtesk, 55 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Kill_Scythys, 55 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Kill_Ballartrask, 56 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Kill_Zorrus, 56 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Kill_Xaphane, 56 },
            { SNOQuest.X1_Bounty_A1_Fields_Event_FleshpitGrove, 8 },
            { SNOQuest.X1_Bounty_A1_Fields_Event_CultistLegion, 8 },
            { SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Krailen, 14 },
            { SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Jezeb, 14 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Muilliuqs, 57 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_ReformedCultist, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Kill_DaleHawthorne, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Kill_CaptainGerber, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Kill_IgorStalfos, 49 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_PanFezbane, 51 },
            { SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Bloone, 57 },
            { SNOQuest.X1_Bounty_A1_Fields_Kill_Growler, 8 },
            { SNOQuest.X1_Bounty_A1_Fields_Kill_Krelm, 8 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_LordBrone, 10 },
            { SNOQuest.X1_Bounty_A1_FesteringWoods_Kill_Galush, 10 },
            { SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Reggrel, 14 },
            { SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Hrugowl, 14 },
            { SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Theodyn, 14 },
            { SNOQuest.X1_Bounty_A1_Highlands_Northern_Kill_Percepeus, 14 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Balzhak, 21 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Samaras, 21 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Kill_Barty, 21 },
            { SNOQuest.X1_Bounty_A2_Oasis_Kill_Gryssian, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Kill_Khahul, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Kill_Tridiun, 23 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Thromp, 28 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Obis, 28 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Ganthar, 28 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Greelode, 28 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Barrucus, 28 },
            { SNOQuest.X1_Bounty_A3_Ramparts_Kill_Allucayrd, 28 },
            { SNOQuest.X1_Bounty_A1_Wilderness_Kill_Horrus, 6 },
            { SNOQuest.X1_Bounty_A1_Wilderness_Kill_Kankerrot, 6 },
            { SNOQuest.X1_Bounty_A1_Highlands_Event_KhazraWarband, 13 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_Deathfire, 17 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_FallenWarband, 19 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_DesertFortress, 19 },
            { SNOQuest.X1_Bounty_A2_Oasis_Event_SunkenGrave, 23 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Event_InfernalSky, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Event_DeathChill, 33 },
            { SNOQuest.X1_Bounty_A3_KeepDepths_Event_ForsakenSoldiers, 30 },
            { SNOQuest.X1_Bounty_A3_Crater_Event_BloodClanAssault, 37 },
            { SNOQuest.X1_Bounty_A4_Spire_Event_ArmyOfHell, 45 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Event_Juggernaut, 42 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_HostileRealm, 57 },
            { SNOQuest.X1_Bounty_A1_OldTristram_Event_DeathCellar, 1 },
            { SNOQuest.X1_Bounty_A5_Bog_Event_AncientEvils, 52 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Event_GhoulSwarm, 3 },
            { SNOQuest.X1_Bounty_A1_Cathedral_Event_ChamberOfBone, 2 },
            { SNOQuest.X1_Bounty_A1_SpiderCaves_Event_SpiderTrap, 12 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Event_FoulHatchery, 25 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Event_Dustbowl, 25 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_GuardSlaughter, 49 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Event_FlyingAssasins, 55 },
            { SNOQuest.X1_Bounty_A5_Bog_Event_HunterKillers, 52 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Bonepit, 51 },
            { SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Sabnock, 47 },
            { SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Vephar, 47 },
            { SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Amduscias, 47 },
            { SNOQuest.P2_Bounty_A4_CorruptSpire_Kill_Cimeries, 47 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Raym, 42 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Beleth, 42 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Emberdread, 42 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Kill_Lasciate, 42 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilA, 43 },
            { SNOQuest.P2_Bounty_A4_BesiegedTower_Clear_BesiegedTower2, 47 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilC, 43 },
            { SNOQuest.px_Bounty_A1_Wilderness_Camp_TemplarPrisoners, 6 },
            { SNOQuest.px_Bounty_A1_Highlands_Camp_ResurgentCult, 13 },
            { SNOQuest.px_Bounty_A2_Oasis_Camp_IronWolves, 23 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Clear_GuildHideout2, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Brent, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Meriel, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Kill_Denis, 51 },
            { SNOQuest.px_Bounty_A1_SpiderCaves_Camp_Cocoon, 12 },
            { SNOQuest.px_Bounty_A2_Aqueducts_Clear_WesternChannel2, 26 },
            { SNOQuest.px_Bounty_A2_Aqueducts_Clear_EasternChannel2, 26 },
            { SNOQuest.px_Bounty_A2_Aqueducts_Kill_Yakara, 26 },
            { SNOQuest.px_Bounty_A2_Aqueducts_Kill_Grool, 26 },
            { SNOQuest.px_Bounty_A2_Aqueducts_Kill_OtziTheCursed, 26 },
            { SNOQuest.px_Bounty_A2_Aqueducts_Kill_StingingDeathSwarm, 26 },
            { SNOQuest.px_Bounty_A2_Boneyards_Camp_AncientDevices, 24 },
            { SNOQuest.px_Bounty_A2_StingingWinds_Camp_CaldeumPrisoners, 21 },
            { SNOQuest.px_Bounty_A4_GardensOfHope_Camp_TrappedAngels, 43 },
            { SNOQuest.px_Bounty_A3_Bridge_Camp_LostPatrol, 34 },
            { SNOQuest.px_Bounty_A5_WestmarchFire_Camp_DeathOrbs, 51 },
            { SNOQuest.px_Bounty_A3_Crater_Camp_AzmodanMinions, 37 },
            { SNOQuest.px_Bounty_A5_Graveyard_Camp_SkeletonJars, 50 },
            { SNOQuest.px_Bounty_A3_Ramparts_Camp_CatapultCommander, 28 },
            { SNOQuest.px_Bounty_A4_Spire_Camp_HellPortals, 44 },
            { SNOQuest.px_Bounty_A1_LeoricsDungeon_Camp_WorthamMilitia, 16 },
            { (SNOQuest)344482, 13 },
            { SNOQuest.X1_Bounty_A1_Highlands_Event_Gharbad_The_Weak, 13 },
            { SNOQuest.X1_Bounty_A1_Highlands_Event_Vendor_Armorer, 13 },
            { SNOQuest.X1_Bounty_A1_Highlands_Kill_Cadhul, 13 },
            { SNOQuest.X1_Bounty_A1_Fields_Event_FarmhouseAmbush, 8 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Event_IronMaiden, 16 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Razormouth, 19 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Kill_Ashek, 19 },
            { SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Bonesplinter, 22 },
            { SNOQuest.X1_Bounty_A2_Oasis_Event_ShrineOfRakanishu, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Event_LostTreasureKhanDakab, 23 },
            { SNOQuest.X1_Bounty_A2_ZKArchives_Kill_Thugeesh, 25 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Event_Triage, 32 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Event_BlazeOfGlory, 34 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Garrach, 17 },
            { SNOQuest.X1_Bounty_A1_LeoricsDungeon_Kill_Warden, 16 },
            { SNOQuest.X1_Bounty_A1_Fields_Event_PreciousOres, 8 },
            { SNOQuest.X1_Bounty_A1_Fields_Event_FarmBesieged, 8 },
            { SNOQuest.X1_Bounty_A2_HowlingPlateau_Event_LacuniLair, 19 },
            { SNOQuest.X1_Bounty_A2_Oasis_Event_SardarsTreasure, 23 },
            { SNOQuest.X1_Bounty_A2_Oasis_Event_PrisonersOfKamyr, 23 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Kill_Pazuzu, 24 },
            { SNOQuest.X1_Bounty_A1_Cemetery_Event_JarOfSouls, 7 },
            { SNOQuest.X1_Bounty_A1_Cemetery_Event_MatriarchsBones, 7 },
            { SNOQuest.X1_Bounty_A3_Battlefields_Event_BloodTies, 32 },
            { SNOQuest.X1_Bounty_A1_Highlands_Northern_Event_VendorRescue, 14 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Event_GuardianSpirits, 21 },
            { SNOQuest.X1_Bounty_A2_StingingWinds_Event_RestlessSands, 21 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Event_Cryptology, 50 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Event_UndeadHusband, 50 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Event_SoldierHoldout, 50 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Event_GraveRobert, 50 },
            { SNOQuest.X1_Bounty_A5_Graveyard_Event_AltarOfSadness, 50 },
            { SNOQuest.X1_Bounty_A5_Bog_Event_Wickerman, 52 },
            { SNOQuest.X1_Bounty_A5_Bog_Event_LordOfFools, 52 },
            { SNOQuest.X1_Bounty_A5_Bog_Event_KingOfTheHill, 52 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_TouchOfDeath, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_WalkInThePark, 49 },
            { SNOQuest.X1_Bounty_A5_PandExt_Kill_Sartor, 57 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Event_BrutelyUnfortunate, 51 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_SiegeDistraction, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_TheGreatWeapon, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_TheHive, 57 },
            { SNOQuest.X1_Bounty_A5_Bog_Kill_Luca, 52 },
            { SNOQuest.X1_Bounty_A5_Bog_Event_3Boggits, 52 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_Resurrection, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_DemonCache, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals, 57 },
            { SNOQuest.X1_Bounty_A5_PandExt_Event_LostLegion, 57 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_NecromancersChoice, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_TheMiser, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_CaptainStokely, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_TheLastStand, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent01, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_KingEvent02, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_HomeInvasion, 49 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_MagicMisfire, 49 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Event_KingEvent03, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Event_HideAndSeek, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Event_Corpsefinder, 51 },
            { SNOQuest.X1_Bounty_A5_WestmarchFire_Event_PestProblems, 51 },
            { SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Magrethar, 57 },
            { SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Borgoth, 57 },
            { SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Severag, 57 },
            { SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Grotescor, 57 },
            { SNOQuest.X1_Bounty_A5_Pand_HexMaze_Kill_Haziael, 57 },
            { SNOQuest.X1_Bounty_A5_PandFortress_Event_ChronoPrison, 56 },
            { SNOQuest.X1_Bounty_A5_Westmarch_Event_AbattoirFurnace, 49 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilD, 43 },
            { SNOQuest.P2_Bounty_A4_GardensOfHope_Event_SigilB, 43 },
            { SNOQuest.px_Bounty_A2_Aqueducts_Event_ThePutridWaters, 26 },
            { SNOQuest.X1_Bounty_A4_GardensOfHope_Kill_Rakanoth, 41 },
            { SNOQuest.X1_Bounty_A2_Alcarnus_Kill_Maghda, 22 },
            { SNOQuest.X1_Bounty_A2_Boneyards_Event_ArmyOfTheDead, 24 },

        };
    }
}
