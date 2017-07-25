using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Framework;
using Trinity.ProfileTags;
using Zeta.Common;
using Zeta.Game;


// This bounties is improved by Noin (2.03) 

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

        public static BountyData GetBountyData(int questId)
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
                //WaypointNumber = quest.Waypoint.Number,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(quest.QuestId, quest.Waypoint.WorldSnoId, BountyHelpers.DynamicBountyPortals.Values.ToList()),
                    new InteractWithGizmoCoroutine(quest.QuestId, -1, 365097, 2912417, 5),
//                 new ClearAreaForNSecondsCoroutine(quest.QuestId, 60, 0, 0, 60),
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
                //WaypointNumber = quest.Waypoint.Number,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(quest.QuestId, quest.Waypoint.WorldSnoId, BountyHelpers.DynamicBountyPortals.Values.ToList()),
                    new ClearLevelAreaCoroutine(quest.QuestId),
                    new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5, 1, 10, true),
                    new ClearLevelAreaCoroutine(quest.QuestId)
                }
            };
        }

        public static Dictionary<int, DynamicBountyType> DynamicBountyDirectory = new Dictionary<int, DynamicBountyType>
        {

            {448274, DynamicBountyType.CursedShrines},
            {448109, DynamicBountyType.CursedShrines},
            {448119, DynamicBountyType.BoundShaman},
            {448520, DynamicBountyType.BoundShaman},
            {448332, DynamicBountyType.BoundShaman},
            {448683, DynamicBountyType.CursedShrines},
            {448697, DynamicBountyType.PlagueOfBurrowers},
            {446370, DynamicBountyType.BoundShaman},
            {448264, DynamicBountyType.BlackKingsLegacy},
            {448127, DynamicBountyType.PlagueOfBurrowers},
            {448280, DynamicBountyType.BoundShaman},
            {448288, DynamicBountyType.PlagueOfBurrowers},
            {448114, DynamicBountyType.CursedShrines},
            {448318, DynamicBountyType.BlackKingsLegacy},
            {448578, DynamicBountyType.BoundShaman},
            {448123, DynamicBountyType.BoundShaman},
            {448165, DynamicBountyType.CursedShrines},
            {448175, DynamicBountyType.BoundShaman},
            {447945, DynamicBountyType.BlackKingsLegacy},
            {446557, DynamicBountyType.PlagueOfBurrowers},
            {448105, DynamicBountyType.CursedShrines},
            {448272, DynamicBountyType.CursedShrines},
            {444433, DynamicBountyType.BlackKingsLegacy},
            {448112, DynamicBountyType.CursedShrines},
            {448224, DynamicBountyType.BoundShaman},
            {448586, DynamicBountyType.BoundShaman},
            {448693, DynamicBountyType.BoundShaman},
            {448641, DynamicBountyType.BoundShaman},
            {448334, DynamicBountyType.BoundShaman},
            {448336, DynamicBountyType.BoundShaman},
            {448340, DynamicBountyType.BoundShaman},
            {448625, DynamicBountyType.CursedShrines},
            {448501, DynamicBountyType.CursedShrines},
            {448276, DynamicBountyType.CursedShrines},
            {448572, DynamicBountyType.CursedShrines},
            {448214, DynamicBountyType.CursedShrines},
            {448268, DynamicBountyType.CursedShrines},
            {448615, DynamicBountyType.BlackKingsLegacy},
            {448155, DynamicBountyType.BlackKingsLegacy},
            {448619, DynamicBountyType.BlackKingsLegacy},
            {448669, DynamicBountyType.BlackKingsLegacy},
            {448208, DynamicBountyType.BlackKingsLegacy},
            {448528, DynamicBountyType.PlagueOfBurrowers},
            {448342, DynamicBountyType.PlagueOfBurrowers},
            {448181, DynamicBountyType.PlagueOfBurrowers},
            {448647, DynamicBountyType.PlagueOfBurrowers},
            {448292, DynamicBountyType.PlagueOfBurrowers},
            {448344, DynamicBountyType.PlagueOfBurrowers},
            {448677, DynamicBountyType.CursedShrines},
            {448163, DynamicBountyType.CursedShrines},
            {448562, DynamicBountyType.BlackKingsLegacy},
            {448322, DynamicBountyType.CursedShrines},
            {448679, DynamicBountyType.CursedShrines},
            {448320, DynamicBountyType.CursedShrines},
            {448667, DynamicBountyType.BlackKingsLegacy},
            {448103, DynamicBountyType.BlackKingsLegacy},
            {448518, DynamicBountyType.BoundShaman},
            {448220, DynamicBountyType.CursedShrines},
            {448643, DynamicBountyType.PlagueOfBurrowers},
            {448206, DynamicBountyType.BlackKingsLegacy},
            {448699, DynamicBountyType.PlagueOfBurrowers},
            {448278, DynamicBountyType.BoundShaman},
            {448125, DynamicBountyType.PlagueOfBurrowers},
            {448580, DynamicBountyType.BoundShaman},
            {448226, DynamicBountyType.BoundShaman},
            {448685, DynamicBountyType.CursedShrines},
            {448258, DynamicBountyType.BlackKingsLegacy},
            {448689, DynamicBountyType.BoundShaman},
            {448695, DynamicBountyType.BoundShaman},
            {448262, DynamicBountyType.BlackKingsLegacy},
            {448492, DynamicBountyType.BlackKingsLegacy},
            {448266, DynamicBountyType.CursedShrines},
            {448613, DynamicBountyType.BlackKingsLegacy},
            {448524, DynamicBountyType.BoundShaman},
            {448210, DynamicBountyType.BlackKingsLegacy},
            {445856, DynamicBountyType.CursedShrines},
            {448508, DynamicBountyType.CursedShrines},
            {448153, DynamicBountyType.BlackKingsLegacy},
            {448582, DynamicBountyType.BoundShaman},
            {448216, DynamicBountyType.CursedShrines},
            {448503, DynamicBountyType.CursedShrines},
            {448627, DynamicBountyType.CursedShrines},
            {448560, DynamicBountyType.BlackKingsLegacy},
            {448286, DynamicBountyType.BoundShaman},
            {448568, DynamicBountyType.CursedShrines},
            {448177, DynamicBountyType.BoundShaman},
            {448588, DynamicBountyType.PlagueOfBurrowers},
            {448330, DynamicBountyType.CursedShrines},
            {448260, DynamicBountyType.BlackKingsLegacy},
            {448673, DynamicBountyType.BlackKingsLegacy},
            {448230, DynamicBountyType.BoundShaman},
            {448564, DynamicBountyType.BlackKingsLegacy},
            {448495, DynamicBountyType.BlackKingsLegacy},
            {448161, DynamicBountyType.CursedShrines},
            {448687, DynamicBountyType.BoundShaman},
            {448328, DynamicBountyType.CursedShrines},
            {448637, DynamicBountyType.BoundShaman},
            {448212, DynamicBountyType.CursedShrines},
            {448635, DynamicBountyType.BoundShaman},
            {448282, DynamicBountyType.BoundShaman},
            {448234, DynamicBountyType.PlagueOfBurrowers},
            {448675, DynamicBountyType.CursedShrines},
            {448232, DynamicBountyType.BoundShaman},
            {448157, DynamicBountyType.BlackKingsLegacy},
            {448639, DynamicBountyType.BoundShaman},
            {448238, DynamicBountyType.PlagueOfBurrowers},
            {448218, DynamicBountyType.CursedShrines},
            {448511, DynamicBountyType.CursedShrines},
            {448530, DynamicBountyType.PlagueOfBurrowers},
            {448100, DynamicBountyType.BlackKingsLegacy}, //*
            {448204, DynamicBountyType.BlackKingsLegacy},
            {448617, DynamicBountyType.BlackKingsLegacy},
            {447911, DynamicBountyType.BoundShaman},
            {448574, DynamicBountyType.CursedShrines},
            {448150, DynamicBountyType.BlackKingsLegacy},
            {448645, DynamicBountyType.PlagueOfBurrowers},
            {448222, DynamicBountyType.CursedShrines},
            {448314, DynamicBountyType.BlackKingsLegacy},
            {448516, DynamicBountyType.BoundShaman},
            {448671, DynamicBountyType.BlackKingsLegacy},
            {448316, DynamicBountyType.BlackKingsLegacy},
            {448159, DynamicBountyType.CursedShrines},
            {448584, DynamicBountyType.BoundShaman},
            {448621, DynamicBountyType.CursedShrines},
            {448346, DynamicBountyType.PlagueOfBurrowers},
            {448284, DynamicBountyType.BoundShaman},
            {448179, DynamicBountyType.BoundShaman},
            {448526, DynamicBountyType.PlagueOfBurrowers},
            {448228, DynamicBountyType.BoundShaman},
            {448590, DynamicBountyType.PlagueOfBurrowers},
            {448681, DynamicBountyType.CursedShrines},
            {448324, DynamicBountyType.CursedShrines},
            {448167, DynamicBountyType.CursedShrines},
            {448312, DynamicBountyType.BlackKingsLegacy},
            {448592, DynamicBountyType.PlagueOfBurrowers},
            {448116, DynamicBountyType.CursedShrines},
            {448498, DynamicBountyType.BlackKingsLegacy},
            {448576, DynamicBountyType.CursedShrines},
            {448173, DynamicBountyType.BoundShaman},
            {448629, DynamicBountyType.CursedShrines},
            {448185, DynamicBountyType.PlagueOfBurrowers},
            {448489, DynamicBountyType.BlackKingsLegacy},
            {448326, DynamicBountyType.CursedShrines},
            {448169, DynamicBountyType.CursedShrines},
            {448236, DynamicBountyType.PlagueOfBurrowers},
            {448691, DynamicBountyType.BoundShaman},
            {448558, DynamicBountyType.BlackKingsLegacy},
            {448290, DynamicBountyType.PlagueOfBurrowers},
            {448633, DynamicBountyType.BoundShaman},
            {448506, DynamicBountyType.CursedShrines},
            {448566, DynamicBountyType.CursedShrines},
            {448522, DynamicBountyType.BoundShaman},
            {448623, DynamicBountyType.CursedShrines},
            {448183, DynamicBountyType.PlagueOfBurrowers},
            {448270, DynamicBountyType.CursedShrines},
            {448631, DynamicBountyType.CursedShrines},
            {448338, DynamicBountyType.BoundShaman},
            {448171, DynamicBountyType.BoundShaman},
            {448701, DynamicBountyType.PlagueOfBurrowers},
            {448570, DynamicBountyType.CursedShrines},
            {448513, DynamicBountyType.CursedShrines},
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


            // A2 - Clear the Mysterious Cave (347598)
            // If delete blacklist NPC actorID in GameData.cs, this quest succeed. But It takes long time because of movement problem.
            Bounties.Add(new BountyData
            {
                QuestId = 347598,
                Act = Act.A2,
                WorldId = 194238,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        // DBs navigation in this little nook of the map is really bad.
                        new MoveToMapMarkerCoroutine(347598,70885,-1615133822),

                        new MoveToSceneCoroutine(347598, 70885, "caOut_Oasis_Sub240_POI_Edge"),

                        // Move into the scene nook by npc
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(93.72876f, 101.2554f, 97.34128f)),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(93.72876f, 101.2554f, 97.34128f), true),

                        // kill anything first to ensure combat doesn't move hero out of place.
//                        new ClearAreaForNSecondsCoroutine(347598, 20, 0, 0, 15, false),

                        // the forced straightline movement sequence to get player by NPC
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(93.72876f, 101.2554f, 97.34128f)),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(93.72876f, 101.2554f, 97.34128f), true),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(125.0679f, 114.7373f, 97.34129f), true),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(116.5557f, 98.87109f, 97.34128f), true),

                        new WaitCoroutine(347598, 70885, 8500),
                        // Try interact with the NPC.
                        // A2_UniqueVendor_Event_MapVendor-15507 ActorSnoId: 115928
                        new InteractionCoroutine(115928, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1), 6),

                        // Wait 6 seconds for him to open door (and also try interact in case we failed previously)
//                        new InteractWithUnitCoroutine(347598,70885,115928,0,3,2,6),
//                        new WaitCoroutine(347598, 70885, 5000),

                        // try interact wtih portal or timeout 
                        new EnterLevelAreaCoroutine (347598,70885, 169477, -1615133822, 176007, TimeSpan.FromSeconds(15)),

                        new ClearAreaForNSecondsCoroutine(347598, 5, 0, 0, 15, false),

                        // try again (this should all get skipped due to worldId/QuestId if we already went inside)
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(93.72876f, 101.2554f, 97.34128f)),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(93.72876f, 101.2554f, 97.34128f), true),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(125.0679f, 114.7373f, 97.34129f), true),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(116.5557f, 98.87109f, 97.34128f), true),                        new InteractWithUnitCoroutine(347598,70885,115928,0,10,2,10),
                        new InteractWithUnitCoroutine(347598,70885,115928,0,10,2,10),
                        new WaitCoroutine(347598, 70885, 4000),
                        new EnterLevelAreaCoroutine (347598,70885, 169477, -1615133822, 176007),

                        // Finish bounty
                        new EnterLevelAreaCoroutine (347598,169477, 194238, 1109456219, 176002, true),
                        new ClearLevelAreaCoroutine (347598)
                    }
            });


            // A2 - Blood and Iron (432334)
            Bounties.Add(new BountyData
            {
                QuestId = 432334,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.GuardedGizmo,
                WaypointLevelAreaId = 57425,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(432334, 432331)
                }
            });

            // A1 - The Cursed Mill (365401)
            Bounties.Add(new BountyData
            {
                QuestId = 365401,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (365401, 71150, 2912417),
                        new InteractWithGizmoCoroutine (365401,71150, 365097, 0, 5),
                        new ClearAreaForNSecondsCoroutine (365401, 60, 365097, 2912417, 30, false)
                    }
            });


            // A5 - Bounty: The True Son of the Wolf (368601)
            Bounties.Add(new BountyData
            {
                QuestId = 368601,
                Act = Act.A5,
                WorldId = 336902, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368601, 263494, 336902, -752748507, 333736),
                    new MoveToPositionCoroutine(336902, new Vector3(311, 393, 10)),
                    // x1_NPC_Westmarch_Aldritch (336711) Distance: 9.175469
                    new InteractWithUnitCoroutine(368601, 336902, 336711, 0, 5),
                    new AttackCoroutine(336711),
                    new ClearAreaForNSecondsCoroutine(368601, 60, 336711, 0, 30),

//                 new ClearLevelAreaCoroutine(336902),
                }
            });

            // A5 - Bounty: The Crystal Prison (363390)
            Bounties.Add(new BountyData
            {
                QuestId = 363390,
                Act = Act.A5,
                WorldId = 271235, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 57,
                Coroutines = new List<ISubroutine>
                {
//                    new MoveToMapMarkerCoroutine(363390, 271235, 2912417),
					new MoveToScenePositionCoroutine(363390, 271235, "mousetrap", new Vector3(123.1343f, 134.059f, -19.52854f)),
//                    new MoveToMapMarkerCoroutine(363390, 271235, 1557208829),
                    new MoveThroughDeathGates(363390, 271235, 1),
//                    new MoveToActorCoroutine(363390, 271235, 363870),
                    // x1_Fortress_Crystal_Prison_MouseTrap (363870) Distance: 7.876552
                    new InteractWithGizmoCoroutine(363390, 271235, 363870, 0, 5),
                    new ClearAreaForNSecondsCoroutine(363390, 20, 0, 0, 45),
                }
            });

            // A5 - Firestorm (375350)
            Bounties.Add(new BountyData
            {
                QuestId = 375350,
                Act = Act.A5,
                WorldId = 338968,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(375350, 261712, -660641889),
                        new EnterLevelAreaCoroutine(375350,261712,338930,-660641889 ,0),

                        new EnterLevelAreaCoroutine(375350,338930,338968,2115491808,0),
                        new MoveToMapMarkerCoroutine(375350, 338968, 2912417),


                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(118.8754f, 108.0339f, 10.76492f)),
                        new WaitCoroutine(375350, 338968, 10000),
                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(72.1983f, 134.9289f, 10.80386f)),
                        new WaitCoroutine(375350, 338968, 10000),
                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(123.1787f, 163.9214f, 10.77483f)),
                        new WaitCoroutine(375350, 338968, 10000),
                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(152.291f, 126.2562f, 10.81015f)),
                        new WaitCoroutine(375350, 338968, 10000),
                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(118.8754f, 108.0339f, 10.76492f)),
                        new WaitCoroutine(375350, 338968, 10000),
                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(72.1983f, 134.9289f, 10.80386f)),
                        new WaitCoroutine(375350, 338968, 10000),
                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(123.1787f, 163.9214f, 10.77483f)),
                        new WaitCoroutine(375350, 338968, 10000),
                        new MoveToScenePositionCoroutine(375350, 338968, "x1_abattoir_NSEW_06", new Vector3(152.291f, 126.2562f, 10.81015f)),
                        new WaitCoroutine(375350, 338968, 10000),

                        new ClearLevelAreaCoroutine (375350)

                    }
            });


            // A5 - Bounty: Demon Souls (363409)
            // Death orbs are Monster/Unit that are not hostile, possibly need interact. trinity doesnt know what to do with them.
            Bounties.Add(new BountyData
            {
                QuestId = 363409,
                Act = Act.A5,
                WorldId = 271233, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 56,
                Coroutines = new List<ISubroutine>
                {
                   new MoveThroughDeathGates(363409, 271233, 1),

                    new MoveToMapMarkerCoroutine(363409, 271233, 2912417),
                    // x1_Fortress_Event_Worldstone_Jamella (334487) Distance: 27.00935
					new InteractWithUnitCoroutine(363409, 271233, 334487, 2912417, 5),

                    new MoveToScenePositionCoroutine(363409, 271233, "x1_fortress_SE_05_Worldstone", new Vector3(165.6456f, 61.12549f, 0.361735f)),
                    new ClearAreaForNSecondsCoroutine(363409, 20, 334466, 0, 45),

                    new MoveToScenePositionCoroutine(363409, 271233, "x1_fortress_SE_05_Worldstone", new Vector3(176.6174f, 147.6885f, 0.471462f)),
                    new MoveThroughDeathGates(363409, 271233, 1),

                    new MoveToScenePositionCoroutine(363409, 271233, "x1_fortress_SE_05_Worldstone", new Vector3(68.77673f, 154.9171f, 0.4347426f)),
                    new ClearAreaForNSecondsCoroutine(363409, 20, 334466, 0, 45),
                    new MoveThroughDeathGates(363409, 271233, 1),

                    new MoveToScenePositionCoroutine(363409, 271233, "x1_fortress_SE_05_Worldstone", new Vector3(70.68292f, 67.76535f, 0.4456832f)),
                    new ClearAreaForNSecondsCoroutine(363409, 20, 334466, 0, 45),
                    new MoveThroughDeathGates(363409, 271233, 1),

                    new MoveToScenePositionCoroutine(363409, 271233, "x1_fortress_SE_05_Worldstone", new Vector3(185.505f, 200.7375f, 0.1f)),
                    new InteractWithUnitCoroutine(363409, 271233, 334487, 0, 5),
                }
            });

            // A5 - 현상금 사냥: 보물고 (359911)
            Bounties.Add(new BountyData
            {
                QuestId = 359911,
                Act = Act.A5,
                WorldId = 283552, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 55,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359911, 283552, 2912417),
					
//					new MoveToScenePositionCoroutine(359911, 283552, "x1_Catacombs_SEW_01", new Vector3(82.76743f, 533.9854f, 0.1f)),
					new MoveToScenePositionCoroutine(359911, 283552, "x1_Catacombs_SEW_01", new Vector3(122.0335f, 166.6719f, 0.1f)),
                    new MoveToActorCoroutine(359911, 283552, 368515),
                    new InteractWithGizmoCoroutine(359911, 283552, 368515, 0, 5),

                    new MoveToScenePositionCoroutine(359911, 283552, "x1_Catacombs_SEW_01", new Vector3(109.7384f, 62.82172f, 0.5280025f)),
                    new MoveToActorCoroutine(359911, 283552, 368515),
                    new InteractWithGizmoCoroutine(359911, 283552, 368515, 0, 5),

                    new MoveToScenePositionCoroutine(359911, 283552, "x1_Catacombs_SEW_01", new Vector3(166.2706f, 155.6351f, 0.6154587f)),
                    new MoveToActorCoroutine(359911, 283552, 368515),
                    new InteractWithGizmoCoroutine(359911, 283552, 368515, 0, 5),


                    new MoveToScenePositionCoroutine(359911, 283552, "x1_Catacombs_SEW_01", new Vector3(144.2595f, 116.1751f, 0.09999999f)),
                    new MoveToActorCoroutine(359911, 283552, 356908),
					// x1_Catacombs_chest_rare_treasureRoom (356908) Distance: 5.573406
					new InteractWithGizmoCoroutine(359911, 283552, 356908, 0, 5),
                    new ClearAreaForNSecondsCoroutine(359911, 90, 0, 0, 45),
                }
            });
        }

        private static void AddKillBossBounties()
        {


            //ActorId: 433670, Type: Gizmo, Name: x1_Global_Chest_BossBounty-73584, Distance2d: 6.557568, CollisionRadius: 11.28195, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 
            //ActorId: 433670, Type: Gizmo, Name: x1_Global_Chest_BossBounty-68275, Distance2d: 78.14486, CollisionRadius: 11.28195, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 
            //ActorId: 433670, Type: Gizmo, Name: x1_Global_Chest_BossBounty-23057, Distance2d: 8.477567, CollisionRadius: 11.28195, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 

            // A4 - Bounty: Kill Izual (361421)
            Bounties.Add(new BountyData
            {
                QuestId = 361421,
                Act = Act.A4,
                WorldId = 214956, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 45,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(361421, 121579, 1038619951),
                    new EnterLevelAreaCoroutine(361421, 121579, 0, 1038619951, 225195),
                    new ClearLevelAreaCoroutine(361421),
                }
            });

            // A2 - Bounty: Kill Belial (358353)
            Bounties.Add(new BountyData
            {
                QuestId = 358353,
                Act = Act.A2,
                WorldId = 60756, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 20,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(358353, 109894, 60756, 1074632599, 159574),
                    new ClearLevelAreaCoroutine(358353),
                }
            });


            // A5 - Bounty: Kill Adria (359915)
            Bounties.Add(new BountyData
            {
                QuestId = 359915,
                Act = Act.A5,
                WorldId = 297771, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 55,
                Coroutines = new List<ISubroutine>
                {
                    //ActorId: 3349, Type: Monster, Name: Belial-1894, Distance2d: 49.40654, CollisionRadius: 0, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0 
                    new MoveToMapMarkerCoroutine(359915, 283566, -131340091),
                    new EnterLevelAreaCoroutine(359915, 283566, 297771, -131340091, 293005),
                    new ClearLevelAreaCoroutine(359915),
                }
            });

            // A3 - Bounty: Kill Azmodan (349244)
            Bounties.Add(new BountyData
            {
                QuestId = 349244,
                Act = Act.A3,
                WorldId = 121214, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 39,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(349244, 119290, 0, 1743679055, 159575),
                    new MoveToPositionCoroutine(121214, new Vector3(602, 608, 37)),

                    new MoveToPositionCoroutine(121214, new Vector3(553, 556, 4)),

                    new MoveToPositionCoroutine(121214, new Vector3(512, 562, 0)),

                    new MoveToPositionCoroutine(121214, new Vector3(492, 573, 0)),

                    new MoveToPositionCoroutine(121214, new Vector3(479, 524, 0)),

                    new MoveToPositionCoroutine(121214, new Vector3(419, 403, 0)),
                    new ClearLevelAreaCoroutine(349244),
                }
            });

            // A1 - Bounty: Kill the Skeleton King (361234)
            Bounties.Add(new BountyData
            {
                QuestId = 361234,
                Act = Act.A1,
                WorldId = 73261, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 5,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(361234, 50585, -267501088),
                    new EnterLevelAreaCoroutine(361234, 50585, 73261, -267501088, 159573),
                    new MoveToPositionCoroutine(73261, new Vector3(336, 260, 20)),
                    new InteractWithGizmoCoroutine(361234, 73261, 175181, 0, 3),
                    new MoveToPositionCoroutine(73261, new Vector3(338, 288, 15)),
                    new WaitCoroutine(361234, 73261, 20000),
                    new ClearLevelAreaCoroutine(361234)
                }
            });

            // A3 - Bounty: Kill Cydaea (349224)
            Bounties.Add(new BountyData
            {
                QuestId = 349224,
                Act = Act.A3,
                WorldId = 119650, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 38,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(349224, 119641, 139272, 43541819, 176001),
                    new EnterLevelAreaCoroutine(349224, 139272, 119650, 43541885, 161278),
                    new ClearLevelAreaCoroutine(349224),
                }
            });

            // A5 - Bounty: Kill Urzael (359919)
            Bounties.Add(new BountyData
            {
                QuestId = 359919,
                Act = Act.A5,
                WorldId = 308446, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 52,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359919, 263494, -1689330047),
                    new EnterLevelAreaCoroutine(359919, 263494, 308446, -1689330047, 367633),
                    new ClearLevelAreaCoroutine(359919),
                }
            });

            // A4 - Bounty: Kill Hammermash (364333)
            Bounties.Add(new BountyData
            {
                QuestId = 364333,
                Act = Act.A4,
                WorldId = 109530, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 47,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(364333, 109525, 984446737),
                    new EnterLevelAreaCoroutine(364333, 109525, 109530, 984446737, 224890),
                    new MoveToMapMarkerCoroutine(364333, 109530, 614820904),
                    new ClearLevelAreaCoroutine(364333),

                }
            });

            // A3 - Bounty: Kill Ghom (346166)
            Bounties.Add(new BountyData
            {
                QuestId = 346166,
                Act = Act.A3,
                WorldId = 103209, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 31,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(346166, 136415, 2102427919),
                    new EnterLevelAreaCoroutine(346166, 136415, 103209, 2102427919, 161277),
                    new ClearLevelAreaCoroutine(346166),
                }
            });

            // A2 - Bounty: Kill Zoltun Kulle (347656)
            Bounties.Add(new BountyData
            {
                QuestId = 347656,
                Act = Act.A2,
                WorldId = 60193, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 25,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(347656, 50613, 60193, 46443076, 159581),
                    new MoveToPositionCoroutine(60193, new Vector3(75, 61, 0)),
                    new ClearLevelAreaCoroutine(347656)
                }
            });

            // A1 - Bounty: Kill Queen Araneae (345528)
            Bounties.Add(new BountyData
            {
                QuestId = 345528,
                Act = Act.A1,
                WorldId = 182976, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 12,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(345528, 180550, 1317387500),
                    new EnterLevelAreaCoroutine(345528, 180550, 182976, 1317387500, 183032),
                    new ClearLevelAreaCoroutine(345528),
                }
            });

            // A5 - Bounty: Kill Malthael(359927)
            Bounties.Add(new BountyData
            {
                QuestId = 359927,
                Act = Act.A5,
                WorldId = 328484, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                LevelAreaIds = new HashSet<int> { 459863 },
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
               {
                    new MoveToScenePositionCoroutine(359927, 271235, "x1_fortress_island_NW_01_Waypoint", new Vector3(88.71082f, 181.0916f, 10.28859f)),
                    new MoveThroughDeathGates(359927, 271235, 4),
                    new EnterLevelAreaCoroutine(359927, 271235, 346410, 1012176886, 176002),
                    new EnterLevelAreaCoroutine(359927, 346410, 328484, -144918420, 374257),
                    new ClearLevelAreaCoroutine(359927)
                }
            });

            // A4 - Bounty: Kill Diablo (349288)
            Bounties.Add(new BountyData
            {
                QuestId = 349288,
                Act = Act.A4,
                WorldId = 196292, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                ////WaypointNumber = 46,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(349288, 129305, -72895048),

                    new EnterLevelAreaCoroutine(349288, 129305, 0, -72895048, 210763),

                    new EnterLevelAreaCoroutine(349288, 205399, 109561, -753198453, 161279),

                    new MoveToPositionCoroutine(109561, new Vector3(645, 644, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(579, 576, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(514, 511, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(481, 479, 19)),
                    new MoveToPositionCoroutine(109561, new Vector3(376, 375, 40)),

                    new ClearLevelAreaCoroutine(349288)
                }
            });

            // A3 - Bounty: Kill the Siegebreaker Assault Beast (349242)
            Bounties.Add(new BountyData
            {
                QuestId = 349242,
                Act = Act.A3,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                WaypointLevelAreaId = 112565,
                //WaypointNumber = 34,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(349242, 95804, -443762283),
                    new EnterLevelAreaCoroutine(349242, 95804, 226713, -443762283, 226784),
                    new ClearLevelAreaCoroutine(349242)
                }
            });

            // A1 - Bounty: Kill the Butcher (347032)
            Bounties.Add(new BountyData
            {
                QuestId = 347032,
                Act = Act.A1,
                WorldId = 78839, // Enter the final worldId here
                QuestType = BountyQuestType.KillBossBounty,
                //WaypointNumber = 17,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347032, 58983, 356899046),
                    new EnterLevelAreaCoroutine(347032, 58983, 78839, 356899046, 158944),
                    new ClearLevelAreaCoroutine(347032)
                }
            });

            // A2 -: Kill Maghda (347558)
            Bounties.Add(new BountyData
            {
                QuestId = 347558,
                Act = Act.A2,
                WorldId = 195200,
                QuestType = BountyQuestType.KillBossBounty,
                WaypointLevelAreaId = 19839,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(347558, 70885, 1742967132),
                        new EnterLevelAreaCoroutine (347558, 70885, 195200, 1742967132, 195234),
                        new MoveToPositionCoroutine(195200, new Vector3(216, 196, 0)),
                        new ClearLevelAreaCoroutine(347558)
                    }
            });

            // A4 - Kill Rakanoth (349262)
            Bounties.Add(new BountyData
            {
                QuestId = 349262,
                Act = Act.A4,
                WorldId = 166640,
                QuestType = BountyQuestType.KillBossBounty,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (349262, 109513, 739323140),
                        new EnterLevelAreaCoroutine (349262, 109513, 166640, 739323140, 161276),
                        new MoveToPositionCoroutine(166640, new Vector3(351, 443, 0)),
                        new ClearLevelAreaCoroutine(349262)
                    }
            });
        }

        private static void AddGuardedGizmoBounties()
        {

            //A1 - Templar Inquisition
            Bounties.Add(new BountyData
            {
                QuestId = 430723,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(430723,430733)
                }
            });


            //A1 - The Queen's Dessert
            Bounties.Add(new BountyData
            {
                QuestId = 432784,
                Act = Act.A1,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(432784,432770)
                }
            });


            //A2 - Prisoners of the Cult
            Bounties.Add(new BountyData
            {
                QuestId = 433053,
                Act = Act.A2,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                LevelAreaIds = new HashSet<int> { 19839 },
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(70885, new Vector3(1355, 273, 169)),
                    new MoveToPositionCoroutine(70885, new Vector3(1300, 342, 161)),
                    new MoveToPositionCoroutine(70885, new Vector3(1334, 410, 161)),
                    new MoveToPositionCoroutine(70885, new Vector3(1384, 436, 162)),
                    new MoveToPositionCoroutine(70885, new Vector3(1426, 467, 173)),
                    new MoveToPositionCoroutine(70885, new Vector3(1402, 525, 174)),
                    new MoveToPositionCoroutine(70885, new Vector3(1330, 987, 174)),
                    new GuardedGizmoCoroutine(433053,433051)
                }
            });


            //A3 - Catapult Command
            Bounties.Add(new BountyData
            {
                QuestId = 433392,
                Act = Act.A3,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433392,433385)
                }
            });

            //A3 - Demon Gates
            Bounties.Add(new BountyData
            {
                QuestId = 433309,
                Act = Act.A3,
                WorldId = 81934,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433309,433295)
                }
            });


            //A4 - Tormented Angels
            Bounties.Add(new BountyData
            {
                QuestId = 433099,
                Act = Act.A4,
                WorldId = 409511,
                QuestType = BountyQuestType.GuardedGizmo,
                WaypointLevelAreaId = 409517,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433099,433124)
                }
            });


            //A4 - The Hell Portals
            Bounties.Add(new BountyData
            {
                QuestId = 433422,
                Act = Act.A4,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433422,433402)
                }
            });


            //A5 - Rathma's Gift 
            Bounties.Add(new BountyData
            {
                QuestId = 433339,
                Act = Act.A5,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433339, 433316)
                }
            });


            //A5 - Death's Embrace
            Bounties.Add(new BountyData
            {
                QuestId = 433256,
                Act = Act.A5,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433256, 433246)
                }
            });


        }

        private static void AddCustomBounties()
        {
            //          RemoveCustomBounties(347598, 346086, 346188, 369271);

            // A2 - Bounty: Sardar's Treasure (347591)
            Bounties.Add(new BountyData
            {
                QuestId = 347591,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 24,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347591, 70885, 922565181),
                    //Scene: caOut_Oasis_Sub80_Cenote_DungeonEntranceA, SnoId: 68275,
                    new MoveToSceneCoroutine(347591, 70885, "caOut_Oasis_Sub80_Cenote_DungeonEntranceA"),
                    new InteractWithGizmoCoroutine(347591, 70885, 175603, -1, 5),

                    new EnterLevelAreaCoroutine(347591, 70885, 157882, 922565181, 175467),

                    new MoveToActorCoroutine(347591, 157882, 219879),
                    new InteractWithGizmoCoroutine(347591, 157882, 219879, 0, 5),

                    new WaitCoroutine(347591, 157882, 2000),

                    //153836 a2dun_Aqd_GodHead_Door (Door) 
					new MoveToScenePositionCoroutine(347591, 157882, "a2dun_Aqd_NSE_Vault_01", new Vector3(47.53217f, 11.98969f, -1.867057f)),
                    new MoveToScenePositionCoroutine(347591, 157882, "a2dun_Aqd_NSE_Vault_02", new Vector3(53.40659f, 48.12427f, -9.900001f)),
					// a2dun_Aqd_Chest_Rare_FacePuzzleSmall (190708) Distance: 11.21381
					new InteractWithGizmoCoroutine(347591, 157882, 190708, 0, 5),

                    new ClearAreaForNSecondsCoroutine(347591, 30, 0, 0, 30),
                }
            });

            // A1 - Bounty: The Precious Ores (347060)
            Bounties.Add(new BountyData
            {
                QuestId = 347060,
                Act = Act.A1,
                WorldId = 82371, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347060, 71150, -431250552),
                    new EnterLevelAreaCoroutine(347060, 71150, 82370, -431250552, 176038),
                    new EnterLevelAreaCoroutine(347060, 82370, 82371, -431250551, 176038, true),
                    new InteractWithUnitCoroutine(347060, 82371, 107076, -431250552, 5),


                    new MoveToActorCoroutine(347060, 82371, 204032),
                    // a1dun_caves_Rocks_GoldOre (204032) Distance: 19.51709
                    new InteractWithGizmoCoroutine(347060, 82371, 204032, 0, 5),
                    // a1duncave_props_crystal_cluster_A (202277) Distance: 20.88908

                    // a1duncave_props_crystal_cluster_A (202277) Distance: 20.88908
                    new MoveToActorCoroutine(347060, 82371, 202277),
                    new InteractWithGizmoCoroutine(347060, 82371, 202277, 0, 5),

                    new ClearAreaForNSecondsCoroutine(347060, 20, 202277, 0, 20),
//					new WaitCoroutine(347060, 82371, 10000),
					
					new MoveToActorCoroutine(347060, 82371, 107076),
                    new InteractWithUnitCoroutine(347060, 82371, 107076, 0, 5),
                }
            });

            // A1 - Scavenged Scabbard (344488)
            // Fixed pathing to entrance but it doesnt attack the boss at the end. Trinity issue.
            Bounties.Add(new BountyData
            {
                QuestId = 344488,
                Act = Act.A1,
                WorldId = 82313,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(344488, 71150, 497382903),

                        new EnterLevelAreaCoroutine (344488, 71150, 82076, 497382903, 175482),
 
                        //Navigation appears to be busted on this level
                        //new EnterLevelAreaCoroutine (344488, 71150, 82076, 497382903, 175482),

                        new EnterLevelAreaCoroutine (344488, 82076, 82313, 497382904, 176001, true),
                        new WaitCoroutine(344488, 82076, 3000),
                        new InteractWithUnitCoroutine(344488, 82313, 81609, 0, 3),

                        new MoveToActorCoroutine(344488, 82313, 222404),
                        new InteractWithGizmoCoroutine(344488, 82313, 222404, 0, 5),
                        new ClearAreaForNSecondsCoroutine(344488, 10, 222404, 0, 20, false),

                        new MoveToActorCoroutine(344488, 82313, 81609),
                        new InteractWithUnitCoroutine(344488, 82313, 81609, 0, 5),
                    }
            });

            // A1 - Bounty: Apothecary's Brother (350529)
            Bounties.Add(new BountyData
            {
                QuestId = 350529,
                Act = Act.A1,
                WorldId = 132995, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(350529, 71150, 853662530),

                    new EnterLevelAreaCoroutine(350529, 71150, 0, 853662530, 175482),
                    new InteractWithUnitCoroutine(350529, 132995, 129782, -1472187117, 5),
                    //ActorId: 136009, Type: Monster, Name: Event_VendorRescue_Brother-2746, Distance2d: 2.170177, CollisionRadius: 0, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 

					new MoveToScenePositionCoroutine(350529, 132995, "trDun_Cave_NSW_01", new Vector3(165.4375f, 136.5941f, 0.1f)),
//         			new MoveToActorCoroutine(350529, 132995, 136009),
                    new ClearAreaForNSecondsCoroutine(350529, 20, 136009, 0, 30),
                    // Event_VendorRescue_Vendor (129782) Distance: 11.94286
                    new MoveToActorCoroutine(350529, 132995, 129782),
                    new InteractWithUnitCoroutine(350529, 132995, 129782, 0, 5),
                    new WaitCoroutine(350529, 132995, 25000),
                }
            });

            //A3 - The Lost Patrol(433217)
            Bounties.Add(new BountyData
            {
                QuestId = 433217,
                Act = Act.A3,
                WorldId = 95804,
                WaypointLevelAreaId = 112565,
                QuestType = BountyQuestType.GuardedGizmo,
                LevelAreaIds = new HashSet<int> { 69504 },
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433217,433184)
                }
            });

            // A2 - Bounty: Prisoners of Kamyr (347595)
            Bounties.Add(new BountyData
            {
                QuestId = 347595,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 24,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347595, 70885, 2912417),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 11.12986
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 2912417, 5),
                    new WaitCoroutine(347595, 70885, 3000),

                    new MoveToSceneCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(114.7661f, 82.81201f, 90.32503f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 5.553013
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 2912417, 5),
                    new WaitCoroutine(347595, 70885, 3000),

                    new MoveToSceneCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(95.05688f, 100.9766f, 90.32501f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 11.12986
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 2912417, 5),
                    new WaitCoroutine(347595, 70885, 3000),

                    new MoveToSceneCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(164.7249f, 159.7368f, 81.5364f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 5.59462
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 0, 5),
                    new WaitCoroutine(347595, 70885, 3000),

                    new MoveToSceneCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(145.3794f, 179.5557f, 81.5364f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 8.649173
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 0, 5),
                    new WaitCoroutine(347595, 70885, 3000),

                    // FallenChampion_B_PrisonersEvent_Unique (260228) Distance: 8.834968
                    new MoveToActorCoroutine(347595, 70885, 260228),

                    new ClearAreaForNSecondsCoroutine(347595, 30, 0, 0, 45),
                }
            });

            // A2 - Bounty: The Shrine of Rakanishu (346065)
            Bounties.Add(new BountyData
            {
                QuestId = 346065,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 24,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(346065, 70885, 2912417),
					
//                    new MoveToSceneCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI"),

                    ////ActorId: 222268, Type: Gizmo, Name: caOut_Oasis_RakinishuStone_B_FX-4071, Distance2d: 18.3527, CollisionRadius: 2.066596, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(106.3967f, 125.3721f, 120.1f)),
                    new AttackCoroutine(222268),
                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(90.22925f, 141.2568f, 120.1f)),
                    new AttackCoroutine(222268),
                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(79.20288f, 98.57617f, 120.1f)),
                    new AttackCoroutine(222268),
                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(116.3853f, 77.13379f, 120.1f)),
                    new AttackCoroutine(222268),
                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(152.0964f, 115.9365f, 120.1f)),
                    new AttackCoroutine(222268),
                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(134.239f, 159.396f, 120.1f)),
                    new AttackCoroutine(222268),

                    //ActorId: 113845, Type: Gizmo, Name: caOut_Oasis_Rakanishu_CenterStone_A-4001, Distance2d: 18.79537, CollisionRadius: 17.01699, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 

                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(106.3967f, 125.3721f, 120.1f)),
                    new InteractWithGizmoCoroutine(346065,70885,113845, -1)
                }
            });

            // A5 - Bounty: The Great Weapon (363344)
            Bounties.Add(new BountyData
            {
                QuestId = 363344,
                Act = Act.A5,
                WorldId = 338600, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(363344, 338600, 2912417),
                    // X1_Angel_Common_Event_GreatWeapon (354407) Distance: 31.32524
					new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(164.6685f, 231.1599f, 0.1f)),
                    new MoveToActorCoroutine(363344, 338600, 354407),
                    new InteractWithUnitCoroutine(363344, 338600, 354407, 0, 5),

                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(62.6709f, 231.9426f, 0.1f)),
                    new MoveToActorCoroutine(363344, 338600, 354407),
                    new InteractWithUnitCoroutine(363344, 338600, 354407, 0, 5),

                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(123.2855f, 157.1476f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  10000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(122.0012f, 164.2465f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(76.9765f, 155.9859f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),
                    new MoveToScenePositionCoroutine(363344, 338600, "x1_Pand_Ext_240_NSEW_Event_GreatWeapon_01", new Vector3(165.17f, 144.5193f, -17.9f)),
                    new WaitCoroutine(363344, 338600,  5000),

                    new ClearAreaForNSecondsCoroutine(363344, 200, 354407, 0, 45),

                }
            });

            // A2 - Bounty: The Guardian Spirits (350560)
            Bounties.Add(new BountyData
            {
                QuestId = 350560,
                Act = Act.A2,
                WorldId = 70885, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 21,
                Coroutines = new List<ISubroutine>
                {
                    //World: caOUT_Town, Id: 70885, AnnId: 1999568897, IsGenerated: False
                    //Scene: caOut_Sub240x240_Tower_Ruin, SnoId: 31623,
                    //LevelArea: A2_caOUT_StingingWinds, Id: 19839

                    new MoveToMapMarkerCoroutine(350560, 70885, 2912417),

                    new InteractWithUnitCoroutine(350560, 70885, 51293, 2912417, 5),
                    new WaitCoroutine(350560, 70885, 5000),

                    new MoveToScenePositionCoroutine(350560, 70885, "caOut_Sub240x240_Tower_Ruin", new Vector3(161.0929f, 112.1465f, 175.5483f)),
                    //// GhostTotem (59436) Distance: 4.005692
                    new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
                    new WaitCoroutine(350560, 70885, 5000),

                    new MoveToScenePositionCoroutine(350560, 70885, "caOut_Sub240x240_Tower_Ruin", new Vector3(101.0205f, 109.6327f, 175.4678f)),
                    //// GhostTotem (59436) Distance: 6.767324
                    new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
                    new WaitCoroutine(350560, 70885, 5000),

                    new MoveToScenePositionCoroutine(350560, 70885, "caOut_Sub240x240_Tower_Ruin", new Vector3(161.0929f, 112.1465f, 175.5483f)),
                    //// GhostTotem (59436) Distance: 4.005692
                    new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
                    new WaitCoroutine(350560, 70885, 5000),

                    new MoveToScenePositionCoroutine(350560, 70885, "caOut_Sub240x240_Tower_Ruin", new Vector3(101.0205f, 109.6327f, 175.4678f)),
                    //// GhostTotem (59436) Distance: 6.767324
                    new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
                    new WaitCoroutine(350560, 70885, 5000),

                    new MoveToScenePositionCoroutine(350560, 70885, "caOut_Sub240x240_Tower_Ruin", new Vector3(136.7617f, 136.5654f, 175.9637f)),
                    new WaitCoroutine(350560, 70885, 10000),
 //                new ClearAreaForNSecondsCoroutine(350560, 20, 0, 0, 25),
                }
            });

            // A2 - Bounty: Restless Sands (350562) 8	0
            Bounties.Add(new BountyData
            {
                QuestId = 350562,
                Act = Act.A2,
                WorldId = 70885, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 21,
                Coroutines = new List<ISubroutine>
                {

                    new MoveToMapMarkerCoroutine(350562, 70885, 2912417),
                    new WaitCoroutine(350562, 70885, 1000),

                    new MoveToScenePositionCoroutine(350562, 70885, "caOut_Sub240x240_Battlements_Ruin", new Vector3(119.9591f, 130.6529f, 158.806f)),
                    new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
                    new WaitCoroutine(350562, 70885, 5000),
					
                    // caOut_Totem_A (3707) Distance: 10.77662
					new MoveToScenePositionCoroutine(350562, 70885, "caOut_Sub240x240_Battlements_Ruin", new Vector3(132.3932f, 97.01617f, 158.806f)),
                    new InteractWithGizmoCoroutine(350562, 70885, 3707, 0, 5),
                    new WaitCoroutine(350562, 70885, 5000),

                    new MoveToScenePositionCoroutine(350562, 70885, "caOut_Sub240x240_Battlements_Ruin", new Vector3(57.79712f, 117.5626f, 172.8638f)),
                    new InteractWithGizmoCoroutine(350562, 70885, 3707, 0, 5),
                    new WaitCoroutine(350562, 70885, 5000),

                    new MoveToScenePositionCoroutine(350562, 70885, "caOut_Sub240x240_Battlements_Ruin", new Vector3(119.9591f, 130.6529f, 158.806f)),
                    new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
                    // oldNecromancer (4798) Distance: 1.06224
                    new MoveToScenePositionCoroutine(350562, 70885, "caOut_Sub240x240_Battlements_Ruin", new Vector3(119.9591f, 130.6529f, 158.806f)),
                    new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
//                 new ClearLevelAreaCoroutine(350562)
                }
            });

            // A5 - Bounty: Lost Host (363402)
            Bounties.Add(new BountyData
            {
                QuestId = 363402,
                Act = Act.A5,
                WorldId = 271235,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(363402, 271235, 2912417),

                    new MoveToScenePositionCoroutine(363402, 271235, "x1_fortress_SW_01_B", new Vector3(91.39471f, 131.2191f, -9.900017f)),
                    new MoveToScenePositionCoroutine(363402, 271235, "x1_fortress_SW_01_B", new Vector3(102.9944f, 130.5471f, -9.900017f)),
                    new MoveToScenePositionCoroutine(363402, 271235, "x1_fortress_SW_01_B", new Vector3(90.04523f, 133.9944f, -9.900017f)),
                    new MoveToScenePositionCoroutine(363402, 271235, "x1_fortress_SW_01_B", new Vector3(92.48651f, 131.3251f, -9.900016f)),
                    new MoveToScenePositionCoroutine(363402, 271235, "x1_fortress_SW_01_B", new Vector3(95.55377f, 115.0262f, -9.900017f)),
                    new MoveToScenePositionCoroutine(363402, 271235, "x1_fortress_SW_01_B", new Vector3(105.3384f, 146.7679f, -9.900017f)),
					
//                    new ClearAreaForNSecondsCoroutine(363402, 60, 2912417, 0, 30, false),
                }
            });

            // A2 - Kill Taros the Wild (345971)
            Bounties.Add(new BountyData
            {
                QuestId = 345971,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToPositionCoroutine(70885, new Vector3(2700, 1586, 184)),
                        new MoveToPositionCoroutine(70885, new Vector3(2535, 1497, 184)),
                        new MoveToPositionCoroutine(70885, new Vector3(2483, 1576, 186)),
                        new MoveToPositionCoroutine(70885, new Vector3(2344, 1518, 207)),

                        new KillUniqueMonsterCoroutine (345971, 70885, 221372, -1258389667),
                        new ClearLevelAreaCoroutine (345971)
                    }
            });

            // A5 - Bounty: The Hatchery (363431)
            Bounties.Add(new BountyData
            {
                QuestId = 363431,
                Act = Act.A5,
                WorldId = 283552, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 54,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(363431, 283552, 2912417),
                    new ClearAreaForNSecondsCoroutine(363431, 45, 0, 0, 45),
                }
            });

            // A1 - 현상금 사냥: 죽음을 부르는 자 카둘 처치 (344490)
            Bounties.Add(new BountyData
            {
                QuestId = 344490,
                Act = Act.A1,
                WorldId = 71150, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 13,
                Coroutines = new List<ISubroutine>
                {
//					new MoveToMapMarkerCoroutine(344490, 71150, 1928482775),
					
					new MoveToScenePositionCoroutine(344490, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(133.3196f, 129.1948f, 9.915271f)),
                    new MoveToScenePositionCoroutine(344490, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(130.6052f, 98.22607f, 9.915272f)),
                    new MoveToScenePositionCoroutine(344490, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(155.8533f, 125.9028f, 9.915272f)),
                    new MoveToScenePositionCoroutine(344490, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(133.0754f, 148.5098f, 9.915272f)),
                    new MoveToScenePositionCoroutine(344490, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(119.749f, 125.9014f, 9.915271f)),

                    new ClearAreaForNSecondsCoroutine(344490, 60, 0, 0, 45),
                    new ClearLevelAreaCoroutine (344490)
                }
            });

            // A5 - 현상금 사냥: 숲의 기도 방해 (444397)
            Bounties.Add(new BountyData
            {
                QuestId = 444397,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 59,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(444397, 408254, 2912417),

                    new MoveToScenePositionCoroutine(0, 408254, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(73.39761f, 65.86435f, 0.1f)),
                    new MoveToScenePositionCoroutine(0, 408254, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(83.71802f, 65.09787f, 0.1f)),
                    new MoveToScenePositionCoroutine(0, 408254, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(69.86966f, 65.79407f, 0.1f)),
                    new MoveToScenePositionCoroutine(0, 408254, "p4_Forest_Coast_NSEW_Sub120_EffigyMeadow", new Vector3(78.4863f, 63.40695f, 0.1f)),

                    new ClearAreaForNSecondsCoroutine(444397, 60, 0, 0, 25),
                }
            });

            //A1 - Wortham Survivors - Interact with operated gizmo repeatly
            Bounties.Add(new BountyData
            {
                QuestId = 434378,
                Act = Act.A1,
                WorldId = 58982,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(434378,434366)
                }
            });

            //A1 - The Triune Reborn - Interact with operated gizmo repeatly
            Bounties.Add(new BountyData
            {
                QuestId = 432293,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(432293,432259)
                }
            });



            // A5 - Bounty: The Lost Patrol (368543)
            Bounties.Add(new BountyData
            {
                QuestId = 368543,
                Act = Act.A5,
                WorldId = 306915, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368543, 261712, 306915, -178461554, 0),
                    new MoveToPositionCoroutine(306915, new Vector3(305, 361, 10)),
                    // x1_SurvivorCaptain_Rescue_Guards_02 (306914) Distance: 6.273188
                    new InteractWithUnitCoroutine(368543, 306915, 306914, 0, 5),

                    new MoveToPositionCoroutine(306915, new Vector3(310, 320, 10)),
                    new ClearAreaForNSecondsCoroutine(368543, 10, 0, 0, 45),


                    new MoveToPositionCoroutine(306915, new Vector3(307, 358, 10)),
                    // x1_SurvivorCaptain_Rescue_Guards_02 (306914) Distance: 6.273188
                    new InteractWithUnitCoroutine(368543, 306915, 306914, 0, 5),
                    new WaitCoroutine(5000),
                }
            });

            // A5 - Bounty: Clear the Perilous Cave (369348) 11	6

            Bounties.Add(new BountyData
            {
                QuestId = 369348,
                Act = Act.A5,
                WorldId = 336572, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
                    //new EnterLevelAreaCoroutine(369348, 267412, 234962, 1022651488, 175501),
                    new MoveToMapMarkerCoroutine(369348, 234962, 1022651488),
                    new EnterLevelAreaCoroutine(369348, 267412, 234962, 1022651488, new int[]{ 175501, 359447, 358853 }),
//					new EnterLevelAreaCoroutine(369348, 267412, 0, 1022651488, 359447),
					new MoveToMapMarkerCoroutine(369348, 234962, 1270943969),
                    new EnterLevelAreaCoroutine(369348, 234962, 336572, 1270943969, 176002),
                    new ClearLevelAreaCoroutine(369348),
                }
            });

            // A5 - Bounty: Clear the Plague Tunnels (369377) 30	9

            Bounties.Add(new BountyData
            {
                QuestId = 369377,
                Act = Act.A5,
                WorldId = 338968, // Enter the final worldId here
                QuestType = BountyQuestType.ClearZone,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(369377, 338930,  -660641889),
                    new EnterLevelAreaCoroutine(369377, 261712, 338930, -660641889, new int[]{ 376027, 176002 }),

                    new MoveToMapMarkerCoroutine(369377, 338930, 2115491808),
                    new EnterLevelAreaCoroutine(369377, 338930, 338968, 2115491808, new int[]{ 175482, 176001 }),

                    new ClearLevelAreaCoroutine(369377),
                }
            });

            // A3 - Bounty: Khazra Guardians (436284)
            Bounties.Add(new BountyData
            {
                QuestId = 436284,
                Act = Act.A3,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436284, 428493, 2912417),
                    new MoveToPositionCoroutine(428493, new Vector3(357, 947, 0)),

                    new MoveToScenePositionCoroutine(436284, 428493, "Guardians", new Vector3(100.5087f, 88.13977f, -11.82213f)),
                    new WaitCoroutine(436284, 428493, 5000),
                    new MoveToScenePositionCoroutine(436284, 428493, "p4_ruins_frost_SE_02_Guardians", new Vector3(107.6098f, 96.29413f, -12.4f)),
                    new WaitCoroutine(436284, 428493, 5000),
                    new MoveToScenePositionCoroutine(436284, 428493, "p4_ruins_frost_SE_02_Guardians", new Vector3(107.2299f, 119.635f, -12.4f)),
                    new WaitCoroutine(436284, 428493, 5000),
                    new MoveToScenePositionCoroutine(436284, 428493, "p4_ruins_frost_SE_02_Guardians", new Vector3(134.8604f, 121.1194f, -12.4f)),
                    new WaitCoroutine(436284, 428493, 5000),
                    new MoveToScenePositionCoroutine(436284, 428493, "p4_ruins_frost_SE_02_Guardians", new Vector3(99.5239f, 90.3031f, -12.4f)),
                    // 437152 (437152) Distance: 5.283126
                    new ClearAreaForNSecondsCoroutine(436284, 90, 437152, 2912417, 20),
                }
            });


            // A1 - Bounty: A Farm Besieged (347062)
            Bounties.Add(new BountyData
            {
                QuestId = 347062,
                Act = Act.A1,
                WorldId = 71150, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 8,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347062, 71150, -1252188069),
                    new MoveToSceneCoroutine(347062, 71150, "POI"),
                    new MoveToScenePositionCoroutine(347062, 71150, "POI", new Vector3(111.8108f, 96.45514f, 0.1000002f)),
                    // Beast_Corpse_A_02 (3341) Distance: 8.983345
                    new InteractWithGizmoCoroutine(347062, 71150, 3341, -1252188069, 5),
                    new ClearAreaForNSecondsCoroutine(347062, 60, 3341, 0, 45),
                }
            });


            // A1 - Bounty: Kill Rockmaw (355276)
            Bounties.Add(new BountyData
            {
                QuestId = 355276,
                Act = Act.A1,
                WorldId = 71150, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 8,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(355276, 71150, 925091454),
                    new EnterLevelAreaCoroutine(355276, 71150, 81163, 925091454, 175501),
                    new EnterLevelAreaCoroutine(355276, 81163, 81164, 925091455, 176038),
                    new MoveToMapMarkerCoroutine(355276, 81164, 1582533030),
                    new ClearAreaForNSecondsCoroutine(355276, 20, 0, 0, 45),
                    new ClearLevelAreaCoroutine(355276),
                }
            });


            // A1 - Bounty: Kill John Gorham Coffin (347092)
            Bounties.Add(new BountyData
            {
                QuestId = 347092,
                Act = Act.A1,
                WorldId = 154587, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 7,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347092, 71150, -1861222194),
                    new EnterLevelAreaCoroutine(347092, 71150, 154587, -1861222194, 176002),
                    new MoveToMapMarkerCoroutine (347092, 154587, 1321851756),
                    new KillUniqueMonsterCoroutine (347092, 154587, 0, 1321851756),
                    new ClearLevelAreaCoroutine(347092),
                }
            });

            // A5 - Bounty: Leap of Faith (363407)
            Bounties.Add(new BountyData
            {
                QuestId = 363407,
                Act = Act.A5,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 56,
                Coroutines = new List<ISubroutine>
                {
                    new MoveThroughDeathGates(363407,271233,1),
                    new MoveToMapMarkerCoroutine (363407, 271233, 2912417),
                    new ClearAreaForNSecondsCoroutine(363407, 60, 0, 2912417, 40),
                }
            });

            // A5 - Bounty: Walk in the Park (359403)
            Bounties.Add(new BountyData
            {
                QuestId = 359403,
                Act = Act.A5,
                WorldId = 261712, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359403, 261712, 2912417),
                    new InteractWithGizmoCoroutine(359403, 261712,289248,0,3),

                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(166.5977f, 53.30713f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(182.8521f, 67.52661f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(165.9268f, 76.87494f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(143.7947f, 61.68109f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(163.4475f, 64.09247f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(181.2703f, 65.80365f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(165.8199f, 81.53088f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(149.4128f, 64.20264f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
                    new MoveToScenePositionCoroutine(359403, 261712, "x1_westm_NSEW_08", new Vector3(164.1267f, 53.60931f, 10.1f)),
                    new WaitCoroutine(359403, 261712, 5000),
					
					
//                  new ClearAreaForNSecondsCoroutine(359403,40,0,0)
                }
            });


            // A5 - Bounty: Resurrection (367884)
            Bounties.Add(new BountyData
            {
                QuestId = 367884,
                Act = Act.A5,
                WorldId = 357653, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    //new EnterLevelAreaCoroutine(367884, 338600, 0, -1551729969, 176004),
                    new EnterLevelAreaCoroutine(367884, 338600, 357653, -1551729969, 0),
                    new MoveToMapMarkerCoroutine(367884, 357653, 2912417),
                    new ClearLevelAreaCoroutine(367884),
                }
            });



            // A2 - Bounty: Kill Ashek (345976)
            Bounties.Add(new BountyData
            {
                QuestId = 345976,
                Act = Act.A2,
                WorldId = 70885, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 19,
                LevelAreaIds = new HashSet<int> { 19836, 19838 },
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(70885, new Vector3(2032, 1067, 170)),
                    new MoveToMapMarkerCoroutine(345976, 70885, 689915896),
                    new KillUniqueMonsterCoroutine (345976, 70885, 0, 689915896),
                    new ClearLevelAreaCoroutine(345976),
                }
            });

            // A5 - Bounty: The Golden Chamber (359907)
            Bounties.Add(new BountyData
            {
                QuestId = 359907,
                Act = Act.A5,
                WorldId = 283566, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 55,
                Coroutines = new List<ISubroutine>
                {
                    //new MoveToMapMarkerCoroutine(359907, 283566, 2912417),
                    new MoveToMapMarkerCoroutine(359907, 283566, 1227995800),
					
//                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(59.00961f, 90.45087f, 0.4674271f)),
//                 new InteractWithGizmoCoroutine(359907, 283566, 356805, 0, 5),

					new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(96.20807f, 92.42499f, 0.2714336f)),

                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(132.951f, 68.6438f, 0.5334398f)),
                    new WaitCoroutine(359907, 283566, 10000),
                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(163.7514f, 93.70868f, 0.2504959f)),
                    new WaitCoroutine(359907, 283566, 10000),
                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(136.5582f, 127.6992f, 0.5334393f)),
                    new WaitCoroutine(359907, 283566, 10000),
                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(100.6373f, 92.96954f, 0.2542905f)),
                    new WaitCoroutine(359907, 283566, 10000),
                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(136.5582f, 127.6992f, 0.5334393f)),
                    new WaitCoroutine(359907, 283566, 10000),
                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(100.6373f, 92.96954f, 0.2542905f)),
                    new WaitCoroutine(359907, 283566, 10000),
//                    new ClearAreaForNSecondsCoroutine(359907, 150, 0, 0, 80),

                    // x1_Catacombs_chest_rare_GardenEvent (356805) Distance: 5.320904
                    new InteractWithGizmoCoroutine(359907, 283566, 356805, 0, 5),
                }
            });

            // A5 - Bounty: Clear the Repository of Bones (369383)
            Bounties.Add(new BountyData
            {
                QuestId = 369383,
                Act = Act.A5,
                WorldId = 338977, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 52,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(369383, 263494, 0, -660641888, 329025),
                    new MoveToMapMarkerCoroutine(369383, 338976, 2115492897),
                    new EnterLevelAreaCoroutine(369383, 338976, 0, 2115492897, 0),
//					new EnterLevelAreaCoroutine(369383, 338976, 0, 2115492897, 175482),
                    new ClearLevelAreaCoroutine(369383),
                }
            });

            // A5 - Bounty: The Miser's Will (368433)
            Bounties.Add(new BountyData
            {
                QuestId = 368433,
                Act = Act.A5,
                WorldId = 246369, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368433, 261712, 0, 2043324508, 333736),
                    new MoveToPositionCoroutine(246369, new Vector3(142, 137, 0)),
                    new MoveToPositionCoroutine(246369, new Vector3(165, 136, 0)),
                    new MoveToPositionCoroutine(246369, new Vector3(147, 159, 0)),
                    new MoveToPositionCoroutine(246369, new Vector3(142, 137, 0)),
                    new InteractWithGizmoCoroutine(368433,246369,359245,0,5),
                    new MoveToPositionCoroutine(246369, new Vector3(165, 136, 0)),
                    new InteractWithGizmoCoroutine(368433,246369,359245,0,5),
                    new MoveToPositionCoroutine(246369, new Vector3(147, 159, 0)),
                    new InteractWithGizmoCoroutine(368433,246369,359245,0,5),
                    new ClearLevelAreaCoroutine(368433)
                }
            });

            // A1 - Bounty: Kill the Cultist Grand Inquisitor (347025)
            Bounties.Add(new BountyData
            {
                QuestId = 347025,
                Act = Act.A1,
                WorldId = 2826, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 15,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347025, 2826, 1468812546),
                    new KillUniqueMonsterCoroutine (347025, 2826, 0, 1468812546),
                    new ClearLevelAreaCoroutine(347025)
                }
            });


            // A5 - Bounty: Judgment (363405)
            Bounties.Add(new BountyData
            {
                QuestId = 363405,
                Act = Act.A5,
                WorldId = 271233, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 56,
                Coroutines = new List<ISubroutine>
                {
                    new MoveThroughDeathGates(363405,271233,1),
                    new MoveToMapMarkerCoroutine(363405, 271233, 2912417),
                    new MoveToScenePositionCoroutine(363405, 271233, "x1_fortress_S_03_Judgment", new Vector3(73.57855f, 151.708f, -9.899999f)),

                    new MoveToScenePositionCoroutine(363405, 271233, "x1_fortress_S_03_Judgment", new Vector3(99.29626f, 150.8474f, -9.899999f)),
                    new WaitCoroutine(363405, 271233, 10000),
                    new MoveToScenePositionCoroutine(363405, 271233, "x1_fortress_S_03_Judgment", new Vector3(97.68469f, 132.0456f, -9.899999f)),
                    new WaitCoroutine(363405, 271233, 10000),
                    new MoveToScenePositionCoroutine(363405, 271233, "x1_fortress_S_03_Judgment", new Vector3(113.8369f, 150.1263f, -9.9f)),
                    new WaitCoroutine(363405, 271233, 10000),
                    new MoveToScenePositionCoroutine(363405, 271233, "x1_fortress_S_03_Judgment", new Vector3(98.00818f, 170.0193f, -9.899999f)),
                    new WaitCoroutine(363405, 271233, 10000),
                    new MoveToScenePositionCoroutine(363405, 271233, "x1_fortress_S_03_Judgment", new Vector3(99.28516f, 149.8546f, -9.899999f)),
                    new WaitCoroutine(363405, 271233, 10000),
                    new MoveToScenePositionCoroutine(363405, 271233, "x1_fortress_S_03_Judgment", new Vector3(80.02435f, 153.6859f, -9.9f)),
                    new WaitCoroutine(363405, 271233, 10000),
					
//                  new ClearAreaForNSecondsCoroutine(363405, 60, 0, 0, 30),
                }
            });

            // A4 - Bounty: Kill Sledge (364336)
            Bounties.Add(new BountyData
            {
                QuestId = 364336,
                Act = Act.A4,
                WorldId = 109530, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 47,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(364336, 109525, 109530, 984446737, 224890),
                    new MoveToMapMarkerCoroutine(364336, 109530, 614820905),
                    new KillUniqueMonsterCoroutine (364336, 109530, 0, 614820905),
                    new ClearLevelAreaCoroutine(364336),

                }
            });

            // A3 - Bounty: Kill Tala (436271)
            Bounties.Add(new BountyData
            {
                QuestId = 436271,
                Act = Act.A3,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436271, 428493, 1714479875),
                    new ClearLevelAreaCoroutine(436271),

                }
            });


            // A3 - Bounty: Kill Aletur (436278)
            Bounties.Add(new BountyData
            {
                QuestId = 436278,
                Act = Act.A3,
                WorldId = 428493, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436278, 428493, 720789926),
                    new ClearLevelAreaCoroutine(436278),
                }
            });

            // A1 - Bounty: Kill Drury Brown (347090)
            Bounties.Add(new BountyData
            {
                QuestId = 347090,
                Act = Act.A1,
                WorldId = 154587, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 7,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(347090, 71150, 0, -1861222194, 176002),
                    new MoveToMapMarkerCoroutine(347090, 154587, 1321851755),
                    new ClearLevelAreaCoroutine(347090),
                }
            });

            // A5 - Bounty: The Lost Legion (367926)
            Bounties.Add(new BountyData
            {
                QuestId = 367926,
                Act = Act.A5,
                WorldId = 357658, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(367926, 338600, -2009241926),
                    //new EnterLevelAreaCoroutine(367926, 338600, 357658, -1551729967, 176002),
                    // portal id changed?
//					new InteractWithGizmoCoroutine(367926, 338600, 176002, -1551729967, 5),
                    new EnterLevelAreaCoroutine(367926, 338600,  357658, -1551729967, new int[]{ 176004, 176002 }),
                    // g_Portal_ArchTall_Blue (176002) Distance: 4.875014

                    //new InteractWithGizmoCoroutine(367926, 338600, 176002, -1551729967, 5),

					new MoveToScenePositionCoroutine(367926, 357658, "x1_Pand_Ext_240_Cellar_06", new Vector3(221.9186f, 111.5466f, 10.1f)),
                    new WaitCoroutine(367926, 338600, 5000),
                    new MoveToScenePositionCoroutine(367926, 357658, "x1_Pand_Ext_240_Cellar_06", new Vector3(129.1784f, 116.145f, 0.1000006f)),
                    new WaitCoroutine(367926, 338600, 5000),
                    new MoveToScenePositionCoroutine(367926, 357658, "x1_Pand_Ext_240_Cellar_06", new Vector3(76.8408f, 86.60729f, 0.1000005f)),
                    new WaitCoroutine(367926, 338600, 5000),
                    new MoveToScenePositionCoroutine(367926, 357658, "x1_Pand_Ext_240_Cellar_06", new Vector3(82.55774f, 155.1393f, 0.1000019f)),
                    new WaitCoroutine(367926, 338600, 5000),
                    new MoveToScenePositionCoroutine(367926, 357658, "x1_Pand_Ext_240_Cellar_06", new Vector3(41.33323f, 182.5292f, 0.1000019f)),
                    new ClearAreaForNSecondsCoroutine(367926, 30, 0, 0, 45),
                    new MoveToActorCoroutine(367926,357658,357331),
                    new InteractWithGizmoCoroutine(367926,357658,357331,0),
                }
            });

            // A1 - Bounty: Crumbling Tower (344482)
            Bounties.Add(new BountyData
            {
                QuestId = 344482,
                Act = Act.A1,
                WorldId = 80057, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 13,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(344482, 71150, 80057, -493718752, 176001),
                    new ClearLevelAreaCoroutine(344482),
                }
            });

            // A5 - Bounty: Lord of Fools (359314)
            Bounties.Add(new BountyData
            {
                QuestId = 359314,
                Act = Act.A5,
                WorldId = 267412, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359314, 267412, 2912417),
                    new MoveToSceneCoroutine(0, 267412, "X1_Bog_Sub240_LordOfFools"),
                    new MoveToScenePositionCoroutine(0, 267412, "X1_Bog_Sub240_LordOfFools", new Vector3(113.899f, 80.1803f, 2.100157f)),
                    new ClearAreaForNSecondsCoroutine(359314, 60, 0, 0, 120),
                }
            });



            // A5 - Bounty: The Last Stand (368525)
            Bounties.Add(new BountyData
            {
                QuestId = 368525,
                Act = Act.A5,
                WorldId = 294633, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368525, 261712, 0, -178461555, 333736),
                    new MoveToPositionCoroutine(294633, new Vector3(448, 434, 0)),
                    new WaitCoroutine(368525, 261712, 5000),
                    new MoveToPositionCoroutine(294633, new Vector3(435, 340, 0)),
                    new WaitCoroutine(368525, 261712, 5000),
                    new MoveToPositionCoroutine(294633, new Vector3(395, 358, 0)),
                    new WaitCoroutine(368525, 261712, 5000),
                    new MoveToPositionCoroutine(294633, new Vector3(399, 302, 0)),
                    new WaitCoroutine(368525, 261712, 5000),
                    new MoveToPositionCoroutine(294633, new Vector3(352, 306, 10)),
                    new WaitCoroutine(368525, 261712, 5000),
                    new MoveToPositionCoroutine(294633, new Vector3(355, 355, 20)),
                    // x1_SurvivorCaptain_Rescue_Guards (295471) Distance: 3.254871
                    new InteractWithUnitCoroutine(368525, 294633, 295471, 0, 5),
                    new MoveToPositionCoroutine(294633, new Vector3(309, 347, 20)),
                    new WaitCoroutine(368525, 261712, 2000),
                    new MoveToPositionCoroutine(294633, new Vector3(300, 294, 20)),
                    new WaitCoroutine(368525, 261712, 2000),
                    new MoveToPositionCoroutine(294633, new Vector3(400, 430, 0)),
                    new WaitCoroutine(368525, 261712, 2000),
                    new MoveToPositionCoroutine(294633, new Vector3(448, 434, 0)),
                    new WaitCoroutine(368525, 261712, 2000),
                    new MoveToPositionCoroutine(294633, new Vector3(435, 340, 0)),
                    new WaitCoroutine(368525, 261712, 2000),
                    new MoveToPositionCoroutine(294633, new Vector3(395, 358, 0)),
                    new WaitCoroutine(368525, 261712, 2000),
                    new MoveToPositionCoroutine(294633, new Vector3(399, 302, 0)),
                    new WaitCoroutine(368525, 261712, 2000),
                    new MoveToPositionCoroutine(294633, new Vector3(352, 306, 10)),
                    new MoveToPositionCoroutine(294633, new Vector3(355, 355, 20)),
                    // x1_SurvivorCaptain_Rescue_Guards (295471) Distance: 3.254871
                    new InteractWithUnitCoroutine(368525, 294633, 295471, 0, 5),
                    new MoveToPositionCoroutine(294633, new Vector3(309, 347, 20)),
                    new MoveToPositionCoroutine(294633, new Vector3(300, 294, 20)),

                }
            });

            // A5 - Bounty: Kill Magrethar (368796)
            Bounties.Add(new BountyData
            {
                Act = Act.A5,
                QuestId = 368796,
                WorldId = 374774, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(368796, 338600, -702665403),
                    new WaitCoroutine(368796, 338600, 20000),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar (374772) Distance: 19.76944
                    new MoveToActorCoroutine(368796, 338600, 374772),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Magrethar (374772) Distance: 19.76944
                    new InteractWithGizmoCoroutine(368796, 338600, 374772, 0, 5),
                    new MoveToMapMarkerCoroutine(368796, 374774, 850055694),
                    new ClearLevelAreaCoroutine(368796),

                }
            });

            // A5 - Bounty: The Bogan Haul (367872)
            Bounties.Add(new BountyData
            {
                QuestId = 367872,
                Act = Act.A5,
                WorldId = 271533, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(367872, 267412, 0, -1947203375, 0),
                    //new EnterLevelAreaCoroutine(367872, 267412, 271533, -1947203375, 185067),
                    new MoveToPositionCoroutine(271533, new Vector3(149, 152, 0)),
                    new MoveToPositionCoroutine(271533, new Vector3(132, 229, 0)),
                    new MoveToPositionCoroutine(271533, new Vector3(194, 238, 0)),
                    new MoveToPositionCoroutine(271533, new Vector3(236, 190, 0)),
                    new MoveToPositionCoroutine(271533, new Vector3(151, 151, 0)),
                    new MoveToActorCoroutine(367872,271533,357331),
                    new InteractWithGizmoCoroutine(367872, 271533, 357331, -0, 5),
                }
            });

            // A5 - Bounty: The Lord of the Hill (359319)
            Bounties.Add(new BountyData
            {
                QuestId = 359319,
                Act = Act.A5,
                WorldId = 267412, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359319, 267412, 2912417),
                    new MoveToSceneCoroutine(359319,267412,"KingOfTheHill"),
                    new MoveToSceneCoroutine(359319, 267412, "X1_Bog_Sub240_KingOfTheHill"),
                    new MoveToMapMarkerCoroutine(359319, 267412, 2912417),
                    new MoveToScenePositionCoroutine(359319, 267412, "X1_Bog_Sub240_KingOfTheHill", new Vector3(191.7875f, 76.95569f, 10.1f)),
                    new MoveToScenePositionCoroutine(359319, 267412, "X1_Bog_Sub240_KingOfTheHill", new Vector3(156.2918f, 172.7191f, 20.1f)),
                    new MoveToScenePositionCoroutine(0, 267412, "X1_Bog_Sub240_KingOfTheHill", new Vector3(119.5713f, 128.0659f, 30.1f)),
                    new ClearAreaForNSecondsCoroutine(359319,15,0,0,60)
                }
            });

            // A1 - Bounty: Kill the Warden (347030)
            Bounties.Add(new BountyData
            {
                QuestId = 347030,
                Act = Act.A1,
                WorldId = 94676, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 16,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(347030, 58982, 87707, 1241437688, 176001),
                    new EnterLevelAreaCoroutine(347030, 87707, 94676, 1303804501, 176001),
//                 new MoveToMapMarkerCoroutine(347030, 94676, 1917087943),
					new MoveToScenePositionCoroutine(347030, 94676, "a1dun_Leor_Jail_NSEW_03", new Vector3(118.5699f, 121.7972f, 0.1000001f)),
                    new ClearAreaForNSecondsCoroutine(347030, 30, 0, 1917087943, 30),

                }
            });

            // A1 - Bounty: Kill Garrach the Afflicted (347027)
            Bounties.Add(new BountyData
            {
                QuestId = 347027,
                Act = Act.A1,
                WorldId = 58983, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 17,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347027, 58983, -1229363584),
                    // a1dun_Leor_Chest_Rare_Garrach (357509) Distance: 78.48354
//                 new MoveToActorCoroutine(347027, 58983, 357509),
                    // a1dun_Leor_Chest_Rare_Garrach (357509) Distance: 78.48354
                    new InteractWithGizmoCoroutine(347027, 58983, 357509, -1229363584, 5),
                    new WaitCoroutine(347027, 58983, 20000),
                    new ClearLevelAreaCoroutine(347027)
                }
            });

            // A5 - The Reformed Cultist (368559)
            Bounties.Add(new BountyData
            {
                QuestId = 368559,
                Act = Act.A5,
                WorldId = 271235,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine(368559,261712,310845,-1342301630,333736 ),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(921, 161, 0)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(882, 166, 0)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(779, 163, -9)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(764, 118, -9)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(759, 91, -9)),

                        new ClearAreaForNSecondsCoroutine(368559, 30, 0, 0, 150),

                        // x1_BogCellar_TriuneCultist (247595) Distance: 1.502872
                        //new InteractWithUnitCoroutine(368559, 310845, 247595, 0, 5),
                        //new WaitCoroutine(10000),
                        new MoveToPositionCoroutine(310845, new Vector3(747, 40, 0)),
                        // g_Portal_Rectangle_Orange_IconDoor (178293) Distance: 5.280013
                        new InteractWithGizmoCoroutine(368559, 310845, 178293, 798857082, 5),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(458, 198, 0)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(461, 130, 10)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(372, 126, 15)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(368, 183, 10)),
                        new WaitCoroutine(368559, 261712, 5000),
                        new MoveToPositionCoroutine(310845, new Vector3(303, 178, 10)),

                        new ClearAreaForNSecondsCoroutine(368559, 30, 0, 0, 150),
						
                        // X1_Westm_Door_Hidden_Bookshelf (316627) Distance: 6.397694
                        new InteractWithGizmoCoroutine(368559, 310845, 316627, 0, 5),
                        new MoveToPositionCoroutine(310845, new Vector3(318, 56, 15)),
                        new WaitCoroutine(368559, 261712, 5000),
                    }
            });



            // A1 - Bounty: The Matriarch's Bones (349020)
            Bounties.Add(new BountyData
            {
                QuestId = 349020,
                Act = Act.A1,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
//                  new EnterLevelAreaCoroutine(349020, 71150, 72636, -1965109038, 176002),
					new EnterLevelAreaCoroutine(349020, 71150, 0, -1965109038, 176002),

                    new MoveToMapMarkerCoroutine(349020, 72636, 2912417),

                    new MoveToSceneCoroutine(349020, 72636,"dead"),
                    new WaitCoroutine(349020, 72636, 4000),
                    new InteractWithUnitCoroutine(349020, 72636, 3892, 2912417, 5),

                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(84.57935f, 52.89191f, 0.1f)),
                    new InteractWithGizmoCoroutine(349020, 72636, 102079, 0, 5),
                    new WaitCoroutine(349020, 72636, 10000),

                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(50.32025f, 116.8566f, 0.09999999f)),
                    new InteractWithGizmoCoroutine(349020, 72636, 174753, 0, 5),
                    new WaitCoroutine(349020, 72636, 10000),

                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(85.17566f, 187.389f, 0.1f)),
                    new InteractWithGizmoCoroutine(349020, 72636, 174754, 0, 5),
                    new WaitCoroutine(349020, 72636, 10000),

                    new MoveToSceneCoroutine(349020,72636,"dead"),
                    new ClearAreaForNSecondsCoroutine(349020, 10, 0, 0, 100),
                    // a1dun_Crypts_Dual_Sarcophagus (105754) Distance: 17.6851
                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(209.656f, 121.3727f, 0.1f)),
                    new InteractWithGizmoCoroutine(349020, 72636, 105754, 0, 5),
                }
            });

            // A2 - Bounty: Kill Pazuzu (347638)
            Bounties.Add(new BountyData
            {
                QuestId = 347638,
                Act = Act.A2,
                WorldId = 70885, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(347638, 70885, -1483215209),
                    // caOut_Boneyards_Dervish_SubAlter (121153) Distance: 2.324677
                    new InteractWithGizmoCoroutine(347638, 70885, 121153, -1483215209, 5),
                    new ClearAreaForNSecondsCoroutine(347638,10,0,-1483215209,45,false),
                    new InteractWithGizmoCoroutine(347638, 70885, 121153, -1483215209, 5),
                    new ClearAreaForNSecondsCoroutine(347638,10,0,-1483215209,45,false),
                    new KillUniqueMonsterCoroutine (347638,70885, 111868, -1483215209),
                    new ClearLevelAreaCoroutine (347638)
                }
            });

            // A5 - Bounty: Magic Misfire (368564)
            Bounties.Add(new BountyData
            {
                QuestId = 368564,
                Act = Act.A5,
                WorldId = 302876, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368564, 261712, 302876, -1750959668, 333736),
                    new MoveToMapMarkerCoroutine(368564, 302876, 2912417),
                    // x1_NPC_ZombieCellar_Male_A (283061) Distance: 1.151123
                    new InteractWithUnitCoroutine(368564, 302876, 283061, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(368564, 15, 283061, 0, 45),
                    // x1_NPC_ZombieCellar_Male_A (283061) Distance: 7.441172
                    new InteractWithUnitCoroutine(368564, 302876, 283061, 0, 5),
                    new WaitCoroutine(368564, 302876, 20000),
                }
            });

            // A5 - Bounty: Kill Grotescor (368919)
            Bounties.Add(new BountyData
            {
                QuestId = 368919,
                Act = Act.A5,
                WorldId = 374766, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(368919, 338600, -702665403),
                    new ClearAreaForNSecondsCoroutine(368919, 10, 0, 0, 45),
                    new InteractWithGizmoCoroutine(368919, 338600, 374764, 0, 5),
                    new EnterLevelAreaCoroutine(368919, 338600,374766,0,374764),
                    new MoveToMapMarkerCoroutine(368919, 374766, -1067001179),
                    new ClearLevelAreaCoroutine(368919),

                }
            });

            // A5 - Bounty: Altar of Sadness (359116)
            Bounties.Add(new BountyData
            {
                QuestId = 359116,
                Act = Act.A5,
                WorldId = 338944, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359116, 338944, 2912417),
                    // x1_Graveyard_Alter_Event_Alter (335575) Distance: 5.21285
                    new MoveToActorCoroutine(359116, 338944, 335575),
                    // x1_Graveyard_Alter_Event_Alter (335575) Distance: 5.21285
                    new InteractWithGizmoCoroutine(359116, 338944, 335575, 0, 5),
                    new ClearAreaForNSecondsCoroutine(359116, 15, 0, 0, 80, false),
                    // x1_Graveyard_Alter_Event_Alter_Chest (340085) Distance: 17.49791
                    new InteractWithGizmoCoroutine(359116, 338944, 340085, 0, 5),
                }
            });

            // A1 - Bounty: Revenge of Gharbad (344486)
            Bounties.Add(new BountyData
            {
                QuestId = 344486,
                Act = Act.A1,
                WorldId = 71150, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine (344486, 71150, 2912417),
                    new InteractWithUnitCoroutine(344486, 71150, 81068, 2912417, 5),

                    new WaitCoroutine(344486, 71150, 10000),

                    new MoveToScenePositionCoroutine(344486, 71150, "trOut_Highlands_Sub240_POI", new Vector3(122.2505f, 165.7817f, -28.86722f)),
                    new WaitCoroutine(344486, 71150, 5000),
                    new MoveToScenePositionCoroutine(344486, 71150, "trOut_Highlands_Sub240_POI", new Vector3(118.2659f, 99.68457f, -28.36605f)),
                    new WaitCoroutine(344486, 71150, 5000),
                    new MoveToScenePositionCoroutine(344486, 71150, "trOut_Highlands_Sub240_POI", new Vector3(137.4558f, 130.5591f, -28.34882f)),
                    new WaitCoroutine(344486, 71150, 3000),
//                 new ClearAreaForNSecondsCoroutine(344486, 30, 96582, 0, 45, false),
//                 new MoveToActorCoroutine(344486, 71150, 81068),
                    // Gharbad_The_Weak_Ghost (81068) Distance: 8.892706
					new InteractWithUnitCoroutine(344486, 71150, 81068, 0, 5),
                    new WaitCoroutine(344486, 71150, 10000),
                    new ClearAreaForNSecondsCoroutine(344486, 30, 96582, 0, 35, false),
                }
            });



            // A5 - Bounty: A Diversion (363342)
            Bounties.Add(new BountyData
            {
                QuestId = 363342,
                Act = Act.A5,
                WorldId = 338600, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(363342, 338600, 2912417),
                    // X1_Angel_Trooper_Event_Ballistae (351967) Distance: 3.603116
                    new InteractWithUnitCoroutine(363342, 338600, 351967, 2912417, 5),

                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(130.6995f, 139.2733f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(130.098f, 105.4987f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(136.1014f, 149.4091f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(91.58203f, 139.6273f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(138.0638f, 143.7219f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(131.9744f, 102.1431f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(136.7021f, 141.1531f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(97.42975f, 140.667f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
                    new MoveToScenePositionCoroutine(363342, 338600, "x1_Pand_Ext_240_NSEW_Event_Ballista_01", new Vector3(136.5885f, 143.6037f, 17.94887f)),
                    new WaitCoroutine(363342, 338600, 10000),
					
//                    new ClearAreaForNSecondsCoroutine(363342, 90, 351967, 2912417 , 30, false),
                }
            });


            // A5 - Bounty: Kill Severag (368915)
            Bounties.Add(new BountyData
            {
                QuestId = 368915,
                Act = Act.A5,
                WorldId = 374778, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(368915, 338600, -702665403),
                    new ClearAreaForNSecondsCoroutine(368915,10,374749,0,30,false),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Severag (374776) Distance: 15.74465
                    new InteractWithGizmoCoroutine(368915, 338600, 374776, 0, 5),
                    new MoveToMapMarkerCoroutine(368915, 374778, 1891838257),
                    new ClearLevelAreaCoroutine(368915),
                }
            });

            // A5 - Bounty: Kill Bloone (368917)
            Bounties.Add(new BountyData
            {
                QuestId = 368917,
                Act = Act.A5,
                WorldId = 374758, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(368917, 338600, -702665403),
                    new ClearAreaForNSecondsCoroutine(368917,10,374749,0,30,false),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Bloone (374756) Distance: 14.68041
                    new InteractWithGizmoCoroutine(368917, 338600, 374756, 0, 5),
                    new EnterLevelAreaCoroutine(368917, 338600, 374758, 0, 374756),
                    new MoveToMapMarkerCoroutine(368917, 374758, 295634146),
                    new ClearLevelAreaCoroutine(368917),
                }
            });

            // A5 - Bounty: The Hive (363346)
            Bounties.Add(new BountyData
            {
                QuestId = 363346,
                Act = Act.A5,
                WorldId = 338600, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(363346, 338600, 2912417),

                    new MoveToSceneCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01"),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(112.7947f, 128.6941f, -17.4f)),
                    new AttackCoroutine(367456),

                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(126.9121f, 156.9678f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(167.1134f, 105.6968f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(79.55261f, 139.0135f, -17.4f)),
                    new AttackCoroutine(367456),

                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(167.1134f, 105.6968f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(137.1652f, 87.79041f, -17.39999f)),
                    new AttackCoroutine(367456),

                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(126.9121f, 156.9678f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(167.1134f, 105.6968f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(79.55261f, 139.0135f, -17.4f)),
                    new AttackCoroutine(367456),

                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(167.1134f, 105.6968f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(137.1652f, 87.79041f, -17.39999f)),
                    new AttackCoroutine(367456),

                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(85.9928f, 94.09125f, -17.4f)),
                    new ClearAreaForNSecondsCoroutine(363346, 20, 0, 0, 20, false),
                }
            });

            // A5 - Bounty: Touch of Death (359399)
            Bounties.Add(new BountyData
            {
                QuestId = 359399,
                Act = Act.A5,
                WorldId = 261712, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    // this map is really bad for pathing, better to just explore for now.
                    //new MoveToMapMarkerCoroutine(359399, 261712, 2912417),
                    new ClearLevelAreaCoroutine(359399),
                    new MoveToMapMarkerCoroutine(359399, 261712, 2912417),

                    new MoveToScenePositionCoroutine(359399, 261712, "x1_westm_NSEW_06", new Vector3(108.632f, 114.135f, 5.099999f)),
                    new WaitCoroutine(359399, 261712, 10000),
                    new MoveToScenePositionCoroutine(359399, 261712, "x1_westm_NSEW_06", new Vector3(107.2872f, 92.01654f, 5.1f)),
                    new WaitCoroutine(359399, 261712, 10000),
                    new MoveToScenePositionCoroutine(359399, 261712, "x1_westm_NSEW_06", new Vector3(128.494f, 114.0747f, 5.1f)),
                    new WaitCoroutine(359399, 261712, 10000),
                    new MoveToScenePositionCoroutine(359399, 261712, "x1_westm_NSEW_06", new Vector3(114.0435f, 140.9714f, 5.1f)),
                    new WaitCoroutine(359399, 261712, 10000),
                    new MoveToScenePositionCoroutine(359399, 261712, "x1_westm_NSEW_06", new Vector3(84.53052f, 114.5778f, 5.1f)),
                    new WaitCoroutine(359399, 261712, 10000),
                    new MoveToScenePositionCoroutine(359399, 261712, "x1_westm_NSEW_06", new Vector3(110.0929f, 115.1127f, 5.1f)),
                    new WaitCoroutine(359399, 261712, 10000),

//                  new ClearAreaForNSecondsCoroutine(359399, 60, 4675, 0, 45,false),
                }
            });

            // A4 - Bounty: Watch Your Step (409895)
            Bounties.Add(new BountyData
            {
                QuestId = 409895,
                Act = Act.A4,
                WorldId = 140709, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 409517,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(409895, 409511, -970799630),
                    new EnterLevelAreaCoroutine(409895, 409511, 140709, -970799630, 204183),

                    new MoveToPositionCoroutine(140709, new Vector3(398, 515, 0)),
                    new MoveToPositionCoroutine(140709, new Vector3(394, 445, 0)),
                    new MoveToPositionCoroutine(140709, new Vector3(322, 454, 0)),
                    new MoveToPositionCoroutine(140709, new Vector3(263, 451, 0)),

                    new MoveToPositionCoroutine(140709, new Vector3(278, 398, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(278, 398, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(278, 398, 0)),
                    new WaitCoroutine(409895, 409511, 5000),

                    new MoveToPositionCoroutine(140709, new Vector3(273, 364, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 364, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 364, 0)),
                    new WaitCoroutine(409895, 409511, 5000),

                    new MoveToPositionCoroutine(140709, new Vector3(271, 333, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(271, 333, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(271, 333, 0)),
                    new WaitCoroutine(409895, 409511, 5000),

                    new MoveToPositionCoroutine(140709, new Vector3(274, 304, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(274, 304, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(274, 304, 0)),
                    new WaitCoroutine(409895, 409511, 5000),

                    new MoveToPositionCoroutine(140709, new Vector3(278, 274, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(278, 274, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(278, 274, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                    new MoveToPositionCoroutine(140709, new Vector3(278, 274, 0)),
                    new WaitCoroutine(409895, 409511, 5000),
                }
            });

            // A2 - Bounty: Kill Bonesplinter (346034)
            Bounties.Add(new BountyData
            {
                QuestId = 346034,
                Act = Act.A2,
                WorldId = 70885, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 19839,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(346034, 70885, 1417206738),
                    new MoveToPositionCoroutine(70885, new Vector3(1197, 1293, 184)),
                    new ClearLevelAreaCoroutine(346034),
                }
            });

            // A5 - Bounty: A Shameful Death (368445)
            Bounties.Add(new BountyData
            {
                QuestId = 368445,
                Act = Act.A5,
                WorldId = 321968, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368445, 261712, 321968, 1775330885, 333736),
                    // x1_westmarchGuard_CaptainStokely_Event (321930) Distance: 3.895381
                    new MoveToActorCoroutine(368445, 321968, 321930),
                    // x1_westmarchGuard_CaptainStokely_Event (321930) Distance: 3.895381
                    new InteractWithGizmoCoroutine(368445, 321968, 321930, 0, 5),
                    // x1_TEMP_Westm_GhostSoldier_01 (321931) Distance: 6.646848
                    new InteractWithUnitCoroutine(368445, 321968, 321931, 0, 5),
                    new ClearAreaForNSecondsCoroutine(368445, 10, 0, 0, 200),
                    // x1_TEMP_Westm_GhostSoldier_01 (321931) Distance: 24.93696
                    new MoveToActorCoroutine(368445, 321968, 321931),
                    // x1_TEMP_Westm_GhostSoldier_01 (321931) Distance: 24.93696
                    new InteractWithUnitCoroutine(368445, 321968, 321931, 0, 5),
                    new MoveToPositionCoroutine(321968, new Vector3(315, 293, 10)),
                    // x1_TEMP_Westm_GhostSoldier_01 (321931) Distance: 21.28371
                    new MoveToActorCoroutine(368445, 321968, 321931),
                    // x1_TEMP_Westm_GhostSoldier_01 (321931) Distance: 21.28371
                    new InteractWithUnitCoroutine(368445, 321968, 321931, 0, 5),

                }
            });

            // A5 - Bounty: Necromancer's Choice (368420)
            Bounties.Add(new BountyData
            {
                QuestId = 368420,
                Act = Act.A5,
                WorldId = 303361, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368420, 261712, 303361, 852517898, 333736),
                    // oldNecromancer (4798) Distance: 56.92637
                    new MoveToActorCoroutine(368420, 303361, 4798),
                    // oldNecromancer (4798) Distance: 56.92637
                    new InteractWithUnitCoroutine(368420, 303361, 4798, 0, 5),
                    // X1_westm_Necro_Jar_of_Souls (316371) Distance: 12.90147
                    new MoveToActorCoroutine(368420, 303361, 316371),
                    // X1_westm_Necro_Jar_of_Souls (316371) Distance: 12.90147
                    new InteractWithGizmoCoroutine(368420, 303361, 316371, 0, 5),


                    new MoveToPositionCoroutine(303361, new Vector3(84, 185, 0)),
                    new MoveToPositionCoroutine(303361, new Vector3(120, 181, 0)),
                    new MoveToPositionCoroutine(303361, new Vector3(105, 129, 10)),
                    new MoveToPositionCoroutine(303361, new Vector3(79, 132, 10)),
                    new MoveToPositionCoroutine(303361, new Vector3(106, 128, 10)),
                    new MoveToPositionCoroutine(303361, new Vector3(137, 166, 0)),
                    new MoveToPositionCoroutine(303361, new Vector3(83, 179, 0)),
                    new MoveToPositionCoroutine(303361, new Vector3(102, 126, 10)),

                    new ClearAreaForNSecondsCoroutine(368420, 60, 0, 0, 200),
					
                    // oldNecromancer (4798) Distance: 27.00111
                    new MoveToActorCoroutine(368420, 303361, 4798),
                    // oldNecromancer (4798) Distance: 27.00111
                    new InteractWithUnitCoroutine(368420, 303361, 4798, 0, 5),
                    new WaitCoroutine(368420, 303361, 20000),
                }
            });

            // A2 - Bounty: The Putrid Waters (433017)
            Bounties.Add(new BountyData
            {
                QuestId = 433017,
                Act = Act.A2,
                WorldId = 432993, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(433017, 59486, 432993, 705396550, 175467),
//                  new MoveToMapMarkerCoroutine(433017, 432993, 2912417),

					new MoveToScenePositionCoroutine(433017, 432993, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),
                    new WaitCoroutine(433017, 432993, 5000),
                    new MoveToScenePositionCoroutine(433017, 432993, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),
                    new WaitCoroutine(433017, 432993, 5000),
                    new MoveToScenePositionCoroutine(433017, 432993, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),
                    new WaitCoroutine(433017, 432993, 5000),
                    new MoveToScenePositionCoroutine(433017, 432993, "a2dun_Aqd_NE_02", new Vector3(47.39954f, 56.66663f, -1.567047f)),

                    new WaitCoroutine(433017, 432993, 5000),
                    new ClearLevelAreaCoroutine(433017),
                }
            });

            // A5 - Bounty: The Demon Cache (367888)
            Bounties.Add(new BountyData
            {
                QuestId = 367888,
                Act = Act.A5,
                WorldId = 357653, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(367888, 338600, 357653, -1551729969, 0),
                    new MoveToMapMarkerCoroutine(367888, 357653, 2912417),
                    new ClearLevelAreaCoroutine(367888),
                    // x1_Global_Chest (357331) Distance: 14.99657
                    new MoveToActorCoroutine(367888, 357653, 357331),
                    // x1_Global_Chest (357331) Distance: 14.99657
                    new InteractWithGizmoCoroutine(367888, 357653, 357331, 0, 5),

                }
            });

            // A5 - Bounty: Kill Lu'ca (367870)
            Bounties.Add(new BountyData
            {
                QuestId = 367870,
                Act = Act.A5,
                WorldId = 269874, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(367870, 267412, 269874, -1947203376, 0),
                    new ClearLevelAreaCoroutine(367870),
                }
            });

            // A1 - Bounty: Carrion Farm (345500)
            Bounties.Add(new BountyData
            {
                QuestId = 345500,
                Act = Act.A1,
                WorldId = 71150, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(345500, 71150, 2912417),
                    // NPC_Human_Male_Event_FarmAmbush (81980) Distance: 59.84805
					new MoveToScenePositionCoroutine(345500, 71150, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(88.14417f, 64.0542f, 0.09999999f)),
                    new WaitCoroutine(345500, 71150, 1000),
                    new InteractWithUnitCoroutine(345500, 71150, 81980, 0, 5),

                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(90f, 182.5f, 0.1f)),
                    new WaitCoroutine(345500, 71150, 5000),
                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(182.719f, 183.8639f, 0.1f)),
                    new WaitCoroutine(345500, 71150, 5000),
                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(184.0928f, 117.3088f, 0.1f)),
                    new WaitCoroutine(345500, 71150, 5000),
                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(90f, 182.5f, 0.1f)),
                    new WaitCoroutine(345500, 71150, 5000),
                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(182.719f, 183.8639f, 0.1f)),
                    new WaitCoroutine(345500, 71150, 5000),
                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(184.0928f, 117.3088f, 0.1f)),
                    new WaitCoroutine(345500, 71150, 5000),

                    // NPC_Human_Male_Event_FarmAmbush (81980) Distance: 36.39687
					new MoveToScenePositionCoroutine(345500, 71150, "trOut_Sub240x240_FarmHouseB_x01_y01", new Vector3(88.14417f, 64.0542f, 0.09999999f)),
                    // NPC_Human_Male_Event_FarmAmbush (81980) Distance: 36.39687
                    new InteractWithUnitCoroutine(345500, 71150, 81980, 0, 5),
                }
            });

            // A5 - Bounty: Penny for Your Troubles (359096)
            Bounties.Add(new BountyData
            {
                QuestId = 359096,
                Act = Act.A5,
                WorldId = 338944, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359096, 338944, 2912417),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (331391) Distance: 79.41319
                    new MoveToActorCoroutine(359096, 338944, 331391),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (331391) Distance: 79.41319
                    new InteractWithUnitCoroutine(359096, 338944, 331391, 2912417, 5),
                    // x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event (331397) Distance: 9.990323
                    new MoveToActorCoroutine(359096, 338944, 331397),
                    // x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event (331397) Distance: 9.990323
                    new InteractWithGizmoCoroutine(359096, 338944, 331397, 0, 5),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (331391) Distance: 25.86785
                    new MoveToActorCoroutine(359096, 338944, 331391),
                    // x1_WestM_Graveyard_Undead_Husband_Ghostlady (331391) Distance: 25.86785
                    new InteractWithUnitCoroutine(359096, 338944, 331391, 0, 5),
                    new ClearAreaForNSecondsCoroutine(359096, 5, 0, 0, 45),
                }
            });

            // A3 - Bounty: Blood Ties (349196)
            Bounties.Add(new BountyData
            {
                QuestId = 349196,
                Act = Act.A3,
                WorldId = 221749, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(349196, 95804, 174555, -1049649953, 176001),
                    new EnterLevelAreaCoroutine(349196, 174555, 221749, -1693984105, 175482, true),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (174995) Distance: 22.82924
                    new MoveToActorCoroutine(349196, 221749, 174995),
 
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (174995) Distance: 22.82924
                    new InteractWithUnitCoroutine(349196, 221749, 174995, -1049649953, 5),

                    new MoveToScenePositionCoroutine(349196, 221749, "a3dun_Keep_EW_02", new Vector3(107.3766f, 180.9147f, 0.1f)),
                    new MoveToActorCoroutine(349196,221749,118261),
                    // FallenGrunt_C_RescueEscort_Unique (260230) Distance: 7.861986
                    //new MoveToActorCoroutine(349196, 221749, 260230),
					new ClearAreaForNSecondsCoroutine(349196, 20, 0, 0, 70,false),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (174995) Distance: 11.26799
                    new MoveToActorCoroutine(349196, 221749, 174995),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (174995) Distance: 11.26799
                    new InteractWithUnitCoroutine(349196, 221749, 174995, 0, 5),
                    new WaitCoroutine(349196, 221749, 15000),
                }
            });

            // A3 - Bounty: Blaze of Glory (346184)
            Bounties.Add(new BountyData
            {
                QuestId = 346184,
                Act = Act.A3,
                WorldId = 95804,
                WaypointLevelAreaId = 112565,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(95804, new Vector3(2301, 613, 0)),
                    new MoveToMapMarkerCoroutine (346184, 95804, 2912417),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (152145) Distance: 52.0904
                    new MoveToActorCoroutine(346184, 95804, 152145),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (152145) Distance: 52.0904
                    new InteractWithUnitCoroutine(346184, 95804, 152145, 2912417, 5),
                    new WaitCoroutine(346184, 95804, 10000),

                    new MoveToScenePositionCoroutine(346184, 95804, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(346184, 95804, 10000),
                    new MoveToScenePositionCoroutine(346184, 95804, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(346184, 95804, 10000),
                    new MoveToScenePositionCoroutine(346184, 95804, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(346184, 95804, 10000),
                    new MoveToScenePositionCoroutine(346184, 95804, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(346184, 95804, 10000),
                    new MoveToScenePositionCoroutine(346184, 95804, "a3dun_Bridge_NS_04", new Vector3(116.0872f, 115.6413f, 0.7101882f)),
                    new WaitCoroutine(346184, 95804, 10000),
                    new MoveToScenePositionCoroutine(346184, 95804, "a3dun_Bridge_NS_04", new Vector3(41.17871f, 112.5601f, 0.7345613f)),
                    new WaitCoroutine(346184, 95804, 10000),
                    new ClearAreaForNSecondsCoroutine(346184, 90, 152145, 2912417, 30),

                    new MoveToActorCoroutine(346184, 95804, 152145),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (152145) Distance: 22.61426
                    new InteractWithUnitCoroutine(346184, 95804, 152145, 0, 5),
                    new WaitCoroutine(346184, 95804, 20000),
                }
            });


            // A1 - Bounty: The Jar of Souls (349016)
            Bounties.Add(new BountyData
            {
                QuestId = 349016,
                Act = Act.A1,
                WorldId = 72636,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(349016, 71150, 72636, -1965109038, 176002),

                    new MoveToSceneCoroutine(349016,72636,"JarSouls"),
                    //new MoveToMapMarkerCoroutine(349016, 72636, 2912417),
                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(102.2006f, 103.909f, 2.337343f)),
                    new InteractWithGizmoCoroutine(349016, 72636, 93713,2912417,5),

                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(103.8495f, 103.4797f, 2.337343f)),
                    new WaitCoroutine(349016, 72636, 10000),
                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(97.08398f, 74.22736f, 0.1f)),
                    new WaitCoroutine(349016, 72636, 10000),
                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(129.2886f, 101.6852f, 0.1000002f)),
                    new WaitCoroutine(349016, 72636, 10000),
                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(104.57f, 127.1461f, 0.1f)),
                    new WaitCoroutine(349016, 72636, 10000),
                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(72.63531f, 97.39606f, 0.1000001f)),
                    new WaitCoroutine(349016, 72636, 10000),
                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(100.5025f, 104.5255f, 2.337343f)),
                    new WaitCoroutine(349016, 72636, 10000),
					
                    // a1dun_Crypts_Jar_of_Souls_02 (219334) Distance: 4.83636
                    new MoveToScenePositionCoroutine(349016, 72636, "trDun_Crypt_NSEW_JarSouls_01", new Vector3(102.2006f, 103.909f, 2.337343f)),
                    // a1dun_Crypts_Jar_of_Souls_02 (219334) Distance: 4.83636
                    new InteractWithGizmoCoroutine(349016, 72636, 219334, 0, 5),
                }
            });

            // A5 - Bounty: Kill Haziael (368921)
            Bounties.Add(new BountyData
            {
                QuestId = 368921,
                Act = Act.A5,
                WorldId = 374770, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(368921, 338600, -702665403),
                    // x1_PortalGuardian_A_Haziael (374752) Distance: 89.75109
                    new ClearAreaForNSecondsCoroutine(368921, 5, 374752, -702665403),
                    // x1_Pand_Maze_PortalTest_OnDeathPortal_Haziael (374768) Distance: 14.4978
					new InteractWithGizmoCoroutine(368921, 338600, 374768, 0, 5),
                    new MoveToMapMarkerCoroutine(368921, 374770, 329854592),
                    // X1_Angel_Trooper_Unique_HexMaze (307329) Distance: 76.83235
                    new ClearAreaForNSecondsCoroutine(368921, 5, 307329, 329854592),
                    new ClearLevelAreaCoroutine(368921),

                }
            });

            // A5 - Bounty: The Rebellious Rabble (368532)
            Bounties.Add(new BountyData
            {
                QuestId = 368532,
                Act = Act.A5,
                WorldId = 336844, // Enter the final worldId here
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                {
//					new MoveToMapMarkerCoroutine (368532, 261712, -752748509),
                    new EnterLevelAreaCoroutine(368532, 261712, 336844, -752748509, 333736),
                    new WaitCoroutine(368532, 261712, 5000),
                    new MoveToPositionCoroutine(336844, new Vector3(304, 424, 10)),
                    new WaitCoroutine(368532, 261712, 5000),
                    new MoveToPositionCoroutine(336844, new Vector3(281, 284, 15)),
                    new WaitCoroutine(368532, 261712, 5000),
                    new MoveToPositionCoroutine(336844, new Vector3(421, 262, 25)),
                    new WaitCoroutine(368532, 261712, 5000),
                    // x1_NPC_Westmarch_Male_A_Severin (336222) Distance: 7.18242
                    new MoveToActorCoroutine(368532, 336844, 336222),
                    // x1_NPC_Westmarch_Male_A_Severin (336222) Distance: 7.18242
                    new InteractWithUnitCoroutine(368532, 336844, 336222, 0, 5),
                }
            });

            // A3 - Bounty: The Triage (346180)
            Bounties.Add(new BountyData
            {
                QuestId = 346180,
                Act = Act.A3,
                WorldId = 95804, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                LevelAreaIds = new HashSet<int> { 112548 },
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(95804, new Vector3(4240, 407, -2)),
                    new MoveToPositionCoroutine(95804, new Vector3(4174, 385, -2)),
                    new MoveToPositionCoroutine(95804, new Vector3(4066, 473, 0)),

                    new MoveToMapMarkerCoroutine(346180, 95804, 2912417),

                    new InteractWithUnitCoroutine(346180, 95804, 205312, 2912417, 5),

                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(141.176f, 167.222f, 0.2f)),
                    new WaitCoroutine(346180, 95804, 10000),
                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(126.7441f, 150.7867f, 0.1999999f)),
                    new WaitCoroutine(346180, 95804, 10000),
                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(116.4688f, 166.6454f, 0.1999991f)),
                    new WaitCoroutine(346180, 95804, 10000),
                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(127.8069f, 180.6221f, 0.2000004f)),
                    new WaitCoroutine(346180, 95804, 10000),
                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(126.7441f, 150.7867f, 0.1999999f)),
                    new WaitCoroutine(346180, 95804, 10000),
                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(116.4688f, 166.6454f, 0.1999991f)),
                    new WaitCoroutine(346180, 95804, 10000),
                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(127.8069f, 180.6221f, 0.2000004f)),
                    new WaitCoroutine(346180, 95804, 10000),
                    new MoveToScenePositionCoroutine(346180, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(141.6938f, 164.2231f, 0.2000004f)),
                    new WaitCoroutine(346180, 95804, 10000),

//                  new ClearAreaForNSecondsCoroutine(346180, 60, 0, 0, 30),
                    new InteractWithUnitCoroutine(346180, 95804, 205468, 0, 5),
                }
            });

            // A2 - Bounty: Lair of the Lacuni (347520)
            Bounties.Add(new BountyData
            {
                QuestId = 347520,
                Act = Act.A2,
                WorldId = 60838, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(347520, 70885, 60838, 288776660, 185067),

                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(216.0627f, 151.6613f, 12.99976f)),
                    new WaitCoroutine(347520, 60838, 5000),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(207.9222f, 86.58944f, 12.99976f)),
                    new WaitCoroutine(347520, 60838, 5000),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(163.856f, 119.5887f, -0.8271011f)),
                    new WaitCoroutine(347520, 60838, 5000),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(132.1614f, 72.54372f, -0.8271086f)),
                    new WaitCoroutine(347520, 60838, 5000),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(95.32114f, 139.1705f, -3.564227f)),
                    new WaitCoroutine(347520, 60838, 5000),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(74.49428f, 215.861f, -0.882843f)),
                    new WaitCoroutine(347520, 60838, 5000),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(176.3916f, 171.3593f, -0.8271087f)),
                    new WaitCoroutine(347520, 60838, 5000),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(179.2931f, 181.3407f, -0.8271185f)),
                    new WaitCoroutine(347520, 60838, 5000),

                    new ClearLevelAreaCoroutine(347520),
                }
            });

            // A5 - Bounty: The Rebellious Rabble (368532)
            Bounties.Add(new BountyData
            {
                QuestId = 368532,
                Act = Act.A5,
                WorldId = 336844, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368532, 261712, 336844, -752748509, 333736),
                    new MoveToPositionCoroutine(336844, new Vector3(301, 424, 10)),
                    new MoveToPositionCoroutine(336844, new Vector3(279, 277, 15)),
                    new MoveToPositionCoroutine(336844, new Vector3(418, 266, 25)),
                    // x1_NPC_Westmarch_Male_A_Severin (336222) Distance: 15.74743
                    new MoveToActorCoroutine(368532, 336844, 336222),
                    // x1_NPC_Westmarch_Male_A_Severin (336222) Distance: 13.87104
                    new InteractWithUnitCoroutine(368532, 336844, 336222, 0, 5),
                }
            });

            // A1 - Bounty: A Stranger in Need (345546)
            Bounties.Add(new BountyData
            {
                QuestId = 345546,
                Act = Act.A1,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(345546, 58982, 2912417),
                    new MoveToSceneCoroutine(345546, 58982,"Exit_Boss"),
//                    new MoveToActorCoroutine(345546, 58982, 218071),
                    // a1dun_Leoric_IronMaiden_Event (221574) Distance: 5.864555
                    new ClearAreaForNSecondsCoroutine(345546,60,218071,0,30),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (218071) Distance: 1.570357
//                    new MoveToActorCoroutine(345546, 58982, 221574),
                    // a1dun_Leoric_IronMaiden_Event (221574) Distance: 10.85653
                    new InteractWithGizmoCoroutine(345546, 58982, 221574, 0, 5),
                    new WaitCoroutine(345546, 58982, 15000),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (218071) Distance: 4.789657
 //                   new MoveToActorCoroutine(345546, 58982, 218071),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (218071) Distance: 4.789657
                    new InteractWithUnitCoroutine(345546, 58982, 218071, 0, 5),
                    new WaitCoroutine(345546, 58982, 25000),
                }
            });

            // A5 - Bounty: Noble Deaths (368536)
            Bounties.Add(new BountyData
            {
                QuestId = 368536,
                Act = Act.A5,
                WorldId = 336852, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(368536, 261712, -752748508),
                    new EnterLevelAreaCoroutine(368536, 261712, 336852, -752748508, 333736),
                    new MoveToPositionCoroutine(336852, new Vector3(407, 293, 7)),
                    new InteractWithGizmoCoroutine(368536, 336852,273323,0,3),
                    new ClearAreaForNSecondsCoroutine(368536, 20, 0, 0, 20),
                    // x1_NPC_Westmarch_Gorrel_NonUnique (357018) Distance: 5.344718
                    new MoveToActorCoroutine(368536, 336852, 357018),
                    // x1_NPC_Westmarch_Gorrel_NonUnique (357018) Distance: 5.344718
                    new InteractWithUnitCoroutine(368536, 336852, 357018, 0, 5),
                }
            });

            // A5 - Bounty: Grave Robert (359112)
            Bounties.Add(new BountyData
            {
                QuestId = 359112,
                Act = Act.A5,
                WorldId = 338944,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359112, 338944, 2912417),

                    new InteractWithUnitCoroutine(359112, 338944, 351621, 2912417, 5),
                    new MoveToSceneCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14"),

                    new MoveToScenePositionCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14", new Vector3(49.48395f, 74.43878f, 0.09999999f)),
                    new WaitCoroutine(359112, 338944, 10000),
                    new MoveToScenePositionCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14", new Vector3(46.55865f, 59.42267f, 0.1f)),
                    new WaitCoroutine(359112, 338944, 10000),
                    new MoveToScenePositionCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14", new Vector3(58.74109f, 69.40292f, 0.1f)),
                    new WaitCoroutine(359112, 338944, 10000),
                    new MoveToScenePositionCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14", new Vector3(35.54315f, 68.42136f, 0.1f)),
                    new WaitCoroutine(359112, 338944, 10000),
                    new MoveToScenePositionCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14", new Vector3(48.36139f, 80.40656f, 0.1f)),
                    new WaitCoroutine(359112, 338944, 10000),

                    new ClearAreaForNSecondsCoroutine(359112, 90, 0, 0, 30, false),

                    new InteractWithUnitCoroutine(359112, 338944, 351621, 0, 5),
                }
            });

            // A5 - The Burning Man (359280)
            Bounties.Add(new BountyData
            {
                QuestId = 359280,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(359280, 267412, 2912417),

                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(134.7274f, 92.38818f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(119.4648f, 83.57068f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(105.3293f, 103.0908f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(124.6479f, 120.5757f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(134.822f, 103.1282f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(108.6432f, 90.27405f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(93.05151f, 110.4031f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
                        new MoveToScenePositionCoroutine(359280, 267412, "X1_Bog_Sub240_Wickerman", new Vector3(116.2715f, 107.064f, -29.9f)),
                        new WaitCoroutine(359280, 267412, 5000),
						
						
//                      new ClearAreaForNSecondsCoroutine(359280, 60, 288471, 2912417, 30),
                    }
            });


            // A5 - Kill Borgoth (368912)
            Bounties.Add(new BountyData
            {
                QuestId = 368912,
                Act = Act.A5,
                WorldId = 374762,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(368912, 338600, -702665403),
                        new WaitCoroutine(368912, 338600, 5000),
                        new InteractWithGizmoCoroutine(368912,338600,374760,702665403,3),
                        //new EnterLevelAreaCoroutine(368912,338600,374762,-702665403,374760),
                        new MoveToMapMarkerCoroutine(368912,374762,653410364),
                        //new KillUniqueMonsterCoroutine(368912,374762,307335,653410364 ),
                        new ClearLevelAreaCoroutine(368912)
                    }
            });

            // A2 - Kill Razormouth (345973)
            Bounties.Add(new BountyData
            {
                QuestId = 345973,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(345973, 70885, -79527531),
//                        new MoveToPositionCoroutine(70885, new Vector3(2156, 1228, 207)),
                        new KillUniqueMonsterCoroutine(345973,70885,221402,-79527531),
                        new ClearLevelAreaCoroutine(345973)
                    }
            });



            // A5 - Home Invasion (368555)
            Bounties.Add(new BountyData
            {
                QuestId = 368555,
                Act = Act.A5,
                WorldId = 351794,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (368555, 261712, 351794, 412504359 , 333736),
                        //new MoveToPositionCoroutine(351794, new Vector3(433, 349, 0)),
                        //new InteractWithGizmoCoroutine(368555,351794,328459,0,5),
                        new WaitCoroutine(368555, 261712, 2000),
                        new MoveToPositionCoroutine(351794, new Vector3(410, 335, 15)),
                        new WaitCoroutine(368555, 261712, 2000),
                        new MoveToPositionCoroutine(351794, new Vector3(396, 308, 15)),
                        new WaitCoroutine(368555, 261712, 2000),
                        new MoveToPositionCoroutine(351794, new Vector3(396, 308, 15)),
                        new WaitCoroutine(368555, 261712, 2000),
                        new MoveToPositionCoroutine(351794, new Vector3(364, 358, 0)),
                        new WaitCoroutine(368555, 261712, 2000),
                        new MoveToPositionCoroutine(351794, new Vector3(328, 345, 0)),
                        new WaitCoroutine(368555, 261712, 2000),
                        new MoveToPositionCoroutine(351794, new Vector3(281, 350, -9)),
                        new WaitCoroutine(368555, 261712, 2000),
                        new MoveToPositionCoroutine(351794, new Vector3(248, 339, -9)),

                    }
            });


            // A5 - Hide and Seek (368607)
            Bounties.Add(new BountyData
            {
                QuestId = 368607,
                Act = Act.A5,
                WorldId = 351793,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (368607, 263494, 351793, -842376652  , 333736),
                        new ClearAreaForNSecondsCoroutine(368607,15,0,2912417),
                        new InteractWithUnitCoroutine(368607,351793,314816,0,5),
                        new MoveToActorCoroutine(368607,351793,273323),
                        new InteractWithGizmoCoroutine(368607,351793,273323,0,5),
                        new MoveToPositionCoroutine(351793, new Vector3(401, 310, 15)),
                        new ClearAreaForNSecondsCoroutine(368607,15,0,0,40,false),
                        new MoveToPositionCoroutine(351793, new Vector3(365, 364, 0)),
                        new InteractWithGizmoCoroutine(368607,351793,273323,0,5),
                        new MoveToPositionCoroutine(351793, new Vector3(336, 342, 0)),
                        new ClearAreaForNSecondsCoroutine(368607,15,0,0,40,false),
                        new MoveToPositionCoroutine(351793, new Vector3(288, 343, -9)),
                        new InteractWithUnitCoroutine(368607,351793,314816,0,5),
                        new MoveToPositionCoroutine(351793, new Vector3(262, 351, -9)),
                    }
            });

            // A4 - Wormsign (409888)
            Bounties.Add(new BountyData
            {
                QuestId = 409888,
                Act = Act.A4,
                WorldId = 180517,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (409888, 409511, 180517, -970799628 , 204183),
                        new ClearLevelAreaCoroutine (409888)
                    }
            });

            // A2 - Kill Thugeesh the Enraged (346111)
            Bounties.Add(new BountyData
            {
                QuestId = 346111,
                Act = Act.A2,
                WorldId = 50610,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346111, 50613, 50610, -1363317799 , 185067),
                        new KillUniqueMonsterCoroutine (346111, 50610, 222335, -349018056),
                        new ClearLevelAreaCoroutine (346111)
                    }
            });


            // A2 - A Miner's Gold (345954)
            Bounties.Add(new BountyData
            {
                QuestId = 345954,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToPositionCoroutine(70885, new Vector3(2687, 1513, 184)),

                        new MoveToMapMarkerCoroutine(345954,70885, 2912417),

                        new MoveToSceneCoroutine(345954,70885,"caOut_Sub240x240_Mine_Destroyed"),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(113.6646f, 99.63721f, 230.3514f)),
                        new InteractWithUnitCoroutine(345954,70885,2924,2912417,5),
                        new WaitCoroutine(345954, 70885, 20000),

                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(80.57178f, 72.74329f, 230.2913f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(80.69434f, 157.0466f, 230.3514f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(84.9397f, 49.0614f, 218.9766f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(87.19385f, 177.0327f, 220.3537f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(92.45044f, 110.6554f, 230.3514f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(129.2219f, 96.4989f, 231.3068f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(87.19385f, 177.0327f, 220.3537f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(92.45044f, 110.6554f, 230.3514f)),
                        new WaitCoroutine(345954, 70885, 5000),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(129.2219f, 96.4989f, 231.3068f)),
                        
                        // a2dun_Aqd_Chest_Special_GreedyMiner (260238) Distance: 14.79873
                        new InteractWithGizmoCoroutine(345954, 70885, 260238, 0, 5),
                    }
            });

            // A1 - Last Stand of the Ancients (345502)
            Bounties.Add(new BountyData
            {
                QuestId = 345502,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {

                        new MoveToMapMarkerCoroutine(345502, 71150, 2912417),
                        new InteractWithGizmoCoroutine(345502, 71150,102008,2912417,5),
                        new WaitCoroutine(345502, 71150, 10000),
                        //new ClearAreaForNSecondsCoroutine (345502, 60, 102008, 2912417),
						new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(52.79926f, 57.75702f, 43.55412f)),
                        new WaitCoroutine(345502, 71150, 10000),
                        new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(101.1246f, 22.6095f, 22.2632f)),
                        new WaitCoroutine(345502, 71150, 10000),
                        new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(98.59137f, 63.27435f, 36.85922f)),
                        new WaitCoroutine(345502, 71150, 10000),
                        new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(86.4173f, 98.62268f, 23.32761f)),
                        new WaitCoroutine(345502, 71150, 10000),
                        new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(98.59137f, 63.27435f, 36.85922f)),
                        new WaitCoroutine(345502, 71150, 10000),
 //                     new ClearAreaForNSecondsCoroutine(345502, 60, 0, 0, 45),
                    }
            });


            // A5 - Out of Time (374571)
            Bounties.Add(new BountyData
            {
                QuestId = 374571,
                Act = Act.A5,
                WorldId = 271235,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(374571, 271235, 2912417),
//                     new MoveToActorCoroutine(374571,271235,301177),

                        // x1_PandExt_Time_Activator (301177) Distance: 25.83982
//                      new InteractWithGizmoCoroutine(374571, 271235, 301177, 2912417, 5),
						
						// x1_PandExt_Time_Activator (301177) Distance: 13.5326
                        new InteractWithGizmoCoroutine(374571, 271235, 301177, 0, 5),

                        new WaitCoroutine(374571, 271235, 2000),
                        new ClearAreaForNSecondsCoroutine (374571, 20, 0, 0)
                    }
            });


            // A3 - Forged in Battle (346146)
            Bounties.Add(new BountyData
            {
                QuestId = 346146,
                Act = Act.A3,
                WorldId = 93104,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {

                    new MoveToMapMarkerCoroutine(346146, 93104, 2912417),

                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(83.1055f, 111.15f, 0.2562059f)),
                    // A3_UniqueVendor_Weaponsmith (149331) Distance: 17.43861
                    new InteractWithUnitCoroutine(346146, 93104, 149331, 1, 5),
                    new WaitCoroutine(346146, 93104, 10000),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(113.2659f, 110.6841f, 0.3101287f)),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(76.26773f, 85.44995f, 0.2815379f)),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(144.5543f, 81.32654f, 0.0716451f)),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(136.5824f, 112.8745f, 0.1000016f)),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(83.1055f, 111.15f, 0.2562059f)),
                    new WaitCoroutine(346146, 93104, 8000),
                    // A3_UniqueVendor_Weaponsmith (149331) Distance: 17.43861
                    new InteractWithUnitCoroutine(346146, 93104, 149331, 1, 5),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(76.47034f, 85.85547f, 0.1844295f)),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(142.7763f, 82.57361f, -0.2093051f)),
                    new MoveToScenePositionCoroutine(346146, 93104, "a3dun_Keep_SW_03_Forge", new Vector3(124.6754f, 101.4712f, 0.5702538f)),
                    new WaitCoroutine(346146, 93104, 10000),
                    new InteractWithUnitCoroutine(346146, 93104, 149331, 1, 5),

                    }
            });

            // A2 - Kill the Stinging Death Swarm (433013)
            Bounties.Add(new BountyData
            {
                QuestId = 433013,
                Act = Act.A2,
                WorldId = 432993,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (433013, 59486, 432993, 705396550, 175467),
                        new KillUniqueMonsterCoroutine (433013,432993, 222189, 904884897),
                        new ClearLevelAreaCoroutine (433013)
                    }
            });


            // A4 - Hellbreeder Nest (409884)
            Bounties.Add(new BountyData
            {
                QuestId = 409884,
                Act = Act.A4,
                WorldId = 139965,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 409517,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(409884, 409511, -970799631),
                        new EnterLevelAreaCoroutine(409884, 409511, 0, -970799631 , 204183),

                        new MoveToPositionCoroutine(139965, new Vector3(365, 288, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(273, 284, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(277, 437, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(375, 427, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(365, 288, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(273, 284, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(277, 437, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(375, 427, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(365, 288, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(273, 284, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(277, 437, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(375, 427, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(365, 288, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(273, 284, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(277, 437, 0)),
                        new MoveToPositionCoroutine(139965, new Vector3(375, 427, 0)),

                        new ClearLevelAreaCoroutine(409884)

                        // Look for actorId 410426

					}
            });

            // A3 - Tide of Battle (346182)
            Bounties.Add(new BountyData
            {
                QuestId = 346182,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(346182, 95804, 2912417),

                        new MoveToScenePositionCoroutine(346182, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(122.116f, 75.22986f, 0.1999994f)),
                        new WaitCoroutine(346182, 95804, 10000),
                        new MoveToScenePositionCoroutine(346182, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(119.1917f, 49.41223f, 0.1999999f)),
                        new WaitCoroutine(346182, 95804, 10000),
                        new MoveToScenePositionCoroutine(346182, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(119.6423f, 99.36554f, 0.2f)),
                        new WaitCoroutine(346182, 95804, 10000),
                        new MoveToScenePositionCoroutine(346182, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(119.1917f, 49.41223f, 0.1999999f)),
                        new WaitCoroutine(346182, 95804, 10000),
                        new MoveToScenePositionCoroutine(346182, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(119.6423f, 99.36554f, 0.2f)),
                        new WaitCoroutine(346182, 95804, 10000),
                        new MoveToScenePositionCoroutine(346182, 95804, "a3_Battlefield_Sub120_HumanGenericE", new Vector3(122.116f, 75.22986f, 0.1999994f)),
                        new WaitCoroutine(346182, 95804, 10000),
						
//                     new ClearAreaForNSecondsCoroutine (346182, 60, 207272, 2912417, 30),
 
                        new InteractWithUnitCoroutine(346182,95804, 207272, 0, 5)
                    }
            });

            // A1 - Kill Krailen the Wicked (367559)
            Bounties.Add(new BountyData
            {
                QuestId = 367559,
                Act = Act.A1,
                WorldId = 75049,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToPositionCoroutine(71150, new Vector3(1711, 3856, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1517, 3837, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1587, 4023, 38)),
                        new MoveToPositionCoroutine(71150, new Vector3(1413, 4076, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1437, 3930, 50)),
                        new MoveToPositionCoroutine(71150, new Vector3(1214, 3903, 79)),
                        new MoveToPositionCoroutine(71150, new Vector3(1202, 3773, 80)),
                        new MoveToPositionCoroutine(71150, new Vector3(1076, 3880, 78)),
                        new MoveToPositionCoroutine(71150, new Vector3(957, 3950, 80)),
                        new MoveToPositionCoroutine(71150, new Vector3(892, 3860, 90)),
                        new MoveToPositionCoroutine(71150, new Vector3(1085, 3729, 78)),
                        new MoveToPositionCoroutine(71150, new Vector3(1080, 3506, 74)),
                        new MoveToPositionCoroutine(71150, new Vector3(1077, 3389, 65)),

                        new EnterLevelAreaCoroutine (367561, 71150, 75049, -1019926638, 176001),
                        new KillUniqueMonsterCoroutine (367559,75049, 218664, 718706852),
                        new ClearLevelAreaCoroutine (367559)
                    }
            });

            // A2 - Rygnar Idol (350564)
            Bounties.Add(new BountyData
            {
                QuestId = 350564,
                Act = Act.A1,
                WorldId = 2812,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (350564, 70885, 2812, 260002582, 185067),
                        new InteractWithUnitCoroutine(350564,2812, 2935, 0, 5),
                        new MoveToMapMarkerCoroutine(350564, 2812, 1557208829),

                        new MoveToSceneCoroutine(350564, 70885, "a2dun_Zolt_Random_W_03_Poltahr"),
                        new MoveToScenePositionCoroutine(350564, 2812, "a2dun_Zolt_Random_W_03_Poltahr", new Vector3(77.97357f, 89.26093f, -12.4f)),
                        new MoveToScenePositionCoroutine(350564, 2812, "a2dun_Zolt_Random_W_03_Poltahr", new Vector3(84.41614f, 120.6121f, -11.6196f)),

                        new InteractionCoroutine(350564, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1),5),
                        new InteractWithGizmoCoroutine(350564,2812, 307,0,5),

                        //new WaitCoroutine(2000),
                        //new MoveToActorCoroutine(350564,2812, 307),
                        //new InteractWithGizmoCoroutine(350564,2812, 307,0,5),
                        //new WaitCoroutine(3000),

                        new ClearAreaForNSecondsCoroutine (350564, 20, 0, 0),
                    }
            });

            // A1 - Eternal War (345505)
            Bounties.Add(new BountyData
            {
                QuestId = 345505,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(345505, 71150, 2912417),
                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(56.42166f, 67.17035f, 20.1f)),
                        new InteractWithGizmoCoroutine(345505, 71150, 111907, 2912417, 5),
                        new WaitCoroutine(345505, 71150, 10000),

                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(54.41251f, 60.12646f, 20.1f)),
                        new WaitCoroutine(345505, 71150, 5000),
                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(66.2486f, 44.25531f, 20.1f)),
                        new WaitCoroutine(345505, 71150, 5000),
                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(57.53882f, 62.03363f, 20.1f)),
                        new WaitCoroutine(345505, 71150, 5000),
                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(48.30765f, 87.24634f, 20.1f)),
                        new WaitCoroutine(345505, 71150, 5000),
                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(61.108f, 63.65912f, 20.1f)),
                        new WaitCoroutine(345505, 71150, 5000),
                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(72.8396f, 36.59406f, 20.1f)),
                        new WaitCoroutine(345505, 71150, 5000),
                        new MoveToScenePositionCoroutine(345505, 71150, "trOut_FesteringWoods_Sub120_Generic_02", new Vector3(58.68997f, 62.7016f, 20.1f)),
                        new WaitCoroutine(345505, 71150, 5000),

                        new ClearAreaForNSecondsCoroutine (345505, 60, 111907, 2912417, 25),
                    }
            });

            // A1 - The Family of Rathe (347058)
            Bounties.Add(new BountyData
            {
                QuestId = 347058,
                Act = Act.A1,
                WorldId = 165797,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (347058,71150, 0, 1070710595, 176001),
                        new EnterLevelAreaCoroutine (347058,102299, 0, 1070710596, 176002),

                        new MoveToScenePositionCoroutine(347058, 165797, "trDun_Crypt_NSEW_01", new Vector3(116.1539f, 97.55945f, -4.9f)),
                        new InteractWithUnitCoroutine(347058,165797, 76907, 0, 5),

                        new MoveToScenePositionCoroutine(347058, 165797, "trDun_Crypt_NSEW_01", new Vector3(103.6976f, 82.98633f, -4.9f)),
                        new WaitCoroutine(347058, 165797, 5000),
                        new MoveToScenePositionCoroutine(347058, 165797, "trDun_Crypt_NSEW_01", new Vector3(119.7391f, 97.01306f, -4.9f)),
                        new WaitCoroutine(347058, 165797, 5000),
                        new MoveToScenePositionCoroutine(347058, 165797, "trDun_Crypt_NSEW_01", new Vector3(102.7822f, 126.6759f, -4.9f)),
                        new WaitCoroutine(347058, 165797, 5000),
                        new MoveToScenePositionCoroutine(347058, 165797, "trDun_Crypt_NSEW_01", new Vector3(80.44812f, 101.0051f, -4.9f)),
                        new WaitCoroutine(347058, 165797, 5000),

                        new ClearAreaForNSecondsCoroutine (347058, 60, 76907, 0, 30),
                        // FamilyTree_Daughter (76907) Distance: 2.11157
                        new InteractWithUnitCoroutine(347058, 165797, 76907, 0, 5),
                    }
            });



            // A1 - Kill Theodyn Deathsinger (369271)
            Bounties.Add(new BountyData
            {
                QuestId = 369271,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 19941, 1199, 19943 },
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(369271, 71150, -2069254570),
						
				 
						new MoveToPositionCoroutine(71150, new Vector3(1493, 4067, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1107, 4044, 80)),
                        new MoveToPositionCoroutine(71150, new Vector3(1066, 3882, 80)),

                        new KillUniqueMonsterCoroutine (369271,71150, 366990, -2069254570),
                        new ClearLevelAreaCoroutine (369271 )
                    }
            });

            #region Missing Data

            // A2 - Clear the Vile Cavern (346086)
            Bounties.Add(new BountyData
            {
                QuestId = 346086,
                Act = Act.A2,
                WorldId = 218967,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(346086, 70885, -270313354),
                        new EnterLevelAreaCoroutine(346086, 70885, 0, -270313354, 206234),
                        new EnterLevelAreaCoroutine (346086, 111670, 218967, -270313353, 176001, true),
                        new ClearLevelAreaCoroutine (346086)
                    }
            });

            // A3 - Clear Cryder's Outpost (346188)
            Bounties.Add(new BountyData
            {
                QuestId = 346188,
                Act = Act.A3,
                WorldId = 185217,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346188, 95804, 185217, 211059664, 176001),
                        new ClearLevelAreaCoroutine (346188)
                    }
            });
            #endregion


        }

        private static void RemoveCustomBounties(params int[] questIds)
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
            // A3 - Bounty: The Cursed Stronghold (436267)
            Bounties.Add(new BountyData
            {
                QuestId = 436267,
                Act = Act.A3,
                WorldId = 428493, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    // todo needs work, failing.
                    new MoveToMapMarkerCoroutine(436267, 428493, 2912417),
                    // x1_Event_CursedShrine (365097) Distance: 22.65772
                    new InteractWithGizmoCoroutine(436267, 428493, 365097, 2912417, 5),
                    //ActorId: 365097, Type: Gizmo, Name: x1_Event_CursedShrine-36946, Distance2d: 4.163542, CollisionRadius: 10.04086, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0

					new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(73.44867f, 127.4745f, -9.9f)),
                    new WaitCoroutine(436267, 428493, 10000),
                    new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(107.384f, 115.0124f, -9.899999f)),
                    new WaitCoroutine(436267, 428493, 10000),
                    new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(51.16956f, 124.3692f, -9.323366f)),
                    new WaitCoroutine(436267, 428493, 10000),
                    new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(71.05536f, 113.5079f, -9.9f)),
                    new WaitCoroutine(436267, 428493, 10000),
                    new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(73.44867f, 127.4745f, -9.9f)),
                    new WaitCoroutine(436267, 428493, 10000),
                    new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(107.384f, 115.0124f, -9.899999f)),
                    new WaitCoroutine(436267, 428493, 10000),
                    new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(51.16956f, 124.3692f, -9.323366f)),
                    new WaitCoroutine(436267, 428493, 10000),
                    new MoveToScenePositionCoroutine(436267, 428493, "p4_ruins_frost_NW_01", new Vector3(71.05536f, 113.5079f, -9.9f)),
                    new WaitCoroutine(436267, 428493, 10000),
//                  new ClearAreaForNSecondsCoroutine(436267, 120, 365097, 2912417, 20, false),
                }
            });

            // A3 - Bounty: The Cursed Bailey (436269)
            Bounties.Add(new BountyData
            {
                QuestId = 436269,
                Act = Act.A3,
                WorldId = 430335, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(436269, 428493, 430335, 1615795536, 175467),
                    new MoveToMapMarkerCoroutine(436269, 430335, 2912417),
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 27.16287
                    new InteractWithGizmoCoroutine(436269, 430335, 365097, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(436269, 60, 365097, 2912417, 30),
                }
            });

            // A2 - The Cursed Archive  (375264)
            Bounties.Add(new BountyData
            {
                QuestId = 375264,
                Act = Act.A2,
                WorldId = 123183,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine(375264, 70885, 123183, -1758560943, 185067),
                        new MoveToMapMarkerCoroutine (375264, 123183, 2912417),
                        new InteractWithGizmoCoroutine (375264,123183, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(66.73035f, 71.62518f, 12.70991f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(85.03119f, 67.16919f, 12.71109f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(89.98163f, 89.61292f, 12.70991f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(66.41211f, 88.02924f, 12.70991f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(75.81531f, 76.53839f, 12.7282f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(86.68158f, 57.90594f, 12.71109f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(95.56787f, 78.08429f, 12.71109f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(67.86664f, 89.98248f, 12.70991f)),
                        new WaitCoroutine(375264, 123183, 10000),
                        new MoveToScenePositionCoroutine(375264, 123183, "a2dun_Zolt_Random_NSEW_02", new Vector3(66.0199f, 68.46393f, 12.70991f)),

//                      new ClearAreaForNSecondsCoroutine (375264, 90, 365097, 2912417, 30)
                    }
            });

            // A1 - The Cursed Grove (365381)
            Bounties.Add(new BountyData
            {
                QuestId = 365381,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (365381,71150, 2912417),
                        new InteractWithGizmoCoroutine (365381,71150, 365097, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (365381, 60, 365097, 2912417, 30)
                    }
            });



            // A1 - The Cursed Camp (369763)
            Bounties.Add(new BountyData
            {
                QuestId = 369763,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (369763, 71150, 2912417),
                        new InteractWithGizmoCoroutine (369763, 71150, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369763, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(129.8826f, 98.04395f, 9.915272f)),
                        new WaitCoroutine(369763, 71150, 10000),
                        new MoveToScenePositionCoroutine(369763, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(140.8633f, 123.8149f, 9.915271f)),
                        new WaitCoroutine(369763, 71150, 10000),
                        new MoveToScenePositionCoroutine(369763, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(129.5017f, 139.8765f, 9.915272f)),
                        new WaitCoroutine(369763, 71150, 10000),
                        new MoveToScenePositionCoroutine(369763, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(130.6418f, 121.6338f, 9.915272f)),
                        new WaitCoroutine(369763, 71150, 10000),
                        new MoveToScenePositionCoroutine(369763, 71150, "trOut_Highlands_Sub240_LeoricBattlements_A", new Vector3(126.72f, 120.3223f, 9.915272f)),
                        new WaitCoroutine(369763, 71150, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (369763, 60, 365097, 2912417, 30)
                    }
            });

            // A1 - The Cursed Bellows (369789)
            Bounties.Add(new BountyData
            {
                QuestId = 369789,
                Act = Act.A1,
                WorldId = 58983,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (369789,58983, 2912417),
                        new InteractWithGizmoCoroutine (369789,58983, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369789, 58983, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(88.48364f, 135.6439f, -9.300045f)),
                        new WaitCoroutine(369789, 58983, 10000),
                        new MoveToScenePositionCoroutine(369789, 58983, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(94.53369f, 165.1028f, -9.300045f)),
                        new WaitCoroutine(369789, 58983, 10000),
                        new MoveToScenePositionCoroutine(369789, 58983, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(102.0026f, 131.6589f, -9.300045f)),
                        new WaitCoroutine(369789, 58983, 10000),
                        new MoveToScenePositionCoroutine(369789, 58983, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(96.51099f, 98.6366f, -9.300044f)),
                        new WaitCoroutine(369789, 58983, 10000),
                        new MoveToScenePositionCoroutine(369789, 58983, "a1dun_Leor_EW_01_BellowsRoom", new Vector3(88.48364f, 135.6439f, -9.300045f)),
                        new WaitCoroutine(369789, 58983, 10000),

//                      new ClearAreaForNSecondsCoroutine (369789, 90, 365097, 2912417, 30)
						
                    }
            });

            // A2 - The Cursed Battlement (369797)
            Bounties.Add(new BountyData
            {
                QuestId = 369797,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(369797, 70885, 2912417),
                        new InteractWithGizmoCoroutine (369797,70885, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369797, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(128.3037f, 139.0692f, 207.4763f)),
                        new WaitCoroutine(369797, 70885, 10000),
                        new MoveToScenePositionCoroutine(369797, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(154.2427f, 146.2689f, 207.4762f)),
                        new WaitCoroutine(369797, 70885, 10000),
                        new MoveToScenePositionCoroutine(369797, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(143.1792f, 164.8138f, 207.4762f)),
                        new WaitCoroutine(369797, 70885, 10000),
                        new MoveToScenePositionCoroutine(369797, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(116.9663f, 157.3429f, 207.4762f)),
                        new WaitCoroutine(369797, 70885, 10000),
                        new MoveToScenePositionCoroutine(369797, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(134.8655f, 155.4772f, 207.4762f)),
                        new WaitCoroutine(369797, 70885, 10000),
                        new MoveToScenePositionCoroutine(369797, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(129.7896f, 137.2734f, 207.4762f)),
                        new WaitCoroutine(369797, 70885, 10000),

//                      new ClearAreaForNSecondsCoroutine (369797, 60, 365097, 2912417, 30)	
                    }
            });

            // A2 - The Cursed Outpost (369800)
            Bounties.Add(new BountyData
            {
                QuestId = 369800,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (369800, 70885, 2912417),
                        new InteractWithGizmoCoroutine (369800,70885, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369800, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(147.6401f, 133.1014f, 207.4763f)),
                        new WaitCoroutine(369800, 70885, 10000),
                        new MoveToScenePositionCoroutine(369800, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(170.0735f, 144.5144f, 207.4762f)),
                        new WaitCoroutine(369800, 70885, 10000),
                        new MoveToScenePositionCoroutine(369800, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(151.7021f, 164.7112f, 207.4762f)),
                        new WaitCoroutine(369800, 70885, 10000),
                        new MoveToScenePositionCoroutine(369800, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(128.4934f, 149.3925f, 207.4762f)),
                        new WaitCoroutine(369800, 70885, 10000),
                        new MoveToScenePositionCoroutine(369800, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(153.5044f, 151.0697f, 207.4763f)),
                        new WaitCoroutine(369800, 70885, 10000),
                        new MoveToScenePositionCoroutine(369800, 70885, "caOut_Sub240x240_Battlements_Destroyed", new Vector3(146.2393f, 127.4581f, 207.4762f)),
                        new WaitCoroutine(369800, 70885, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (369800, 60, 365097, 2912417, 30)
                    }
            });

            // A2 - The Cursed Shallows (369813)
            Bounties.Add(new BountyData
            {
                QuestId = 369813,
                Act = Act.A2,
                WorldId = 62569,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
//                        new MoveToMapMarkerCoroutine (369813, 70885, 1352061373),
                        new EnterLevelAreaCoroutine(369813, 70885, 0, 1352061373, 176003),
                        new MoveToMapMarkerCoroutine (369813, 62569, 2912417),
                        new InteractWithGizmoCoroutine (369813,62569, 365097, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (369813, 60, 365097, 2912417, 30)
                    }
            });

            // A3 - The Cursed Garrison (369825)
            Bounties.Add(new BountyData
            {
                QuestId = 369825,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (369825,95804, 2912417),
                        new InteractWithGizmoCoroutine (369825,95804, 365097, 2912417, 5),
                        new MoveToSceneCoroutine(369825, 95804, "a3_Battlefield_Sub240_ParentScene"),

                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(88.71631f, 149.5671f, 20.1f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(112.2556f, 129.1063f, 20.75397f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(131.2407f, 148.1129f, 20.7601f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(115.8555f, 160.5751f, 20.1f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(113.8833f, 140.493f, 20.76011f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(116.342f, 127.2602f, 20.73954f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(133.248f, 139.9616f, 21.01073f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(111.6228f, 157.4414f, 20.16819f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(97.32251f, 139.0292f, 20.1f)),
                        new WaitCoroutine(369825, 95804, 10000),
                        new MoveToScenePositionCoroutine(369825, 95804, "a3_Battlefield_Sub240_HumanOutpost_02", new Vector3(88.94995f, 146.5066f, 20.1f)),
                        new WaitCoroutine(369825, 95804, 10000),

//                      new ClearAreaForNSecondsCoroutine (369825, 60, 365097, 2912417, 30)
                    }
            });

            // A3 - The Cursed Glacier (369851)
            Bounties.Add(new BountyData
            {
                QuestId = 369851,
                Act = Act.A3,
                WorldId = 189910,
                QuestType = BountyQuestType.ClearCurse,
                LevelAreaIds = new HashSet<int> { 112565 },
                WaypointLevelAreaId = 155048,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(369851, 95804, 942020622),
                        new EnterLevelAreaCoroutine(369851, 95804, 0, 942020622, 176003),
                        new MoveToMapMarkerCoroutine (369851, 189910, 2912417),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(91.36786f, 135.6528f, 0.1000028f)),
                        new InteractWithGizmoCoroutine (369851,189910, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(91.36786f, 135.6528f, 0.1000028f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(97.97012f, 105.6096f, 0.2187943f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(99.60431f, 158.5022f, 0.1000038f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(112.0791f, 139.3915f, 0.6401573f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(97.97012f, 105.6096f, 0.2187943f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(99.60431f, 158.5022f, 0.1000038f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(112.0791f, 139.3915f, 0.6401573f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(97.97012f, 105.6096f, 0.2187943f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(99.60431f, 158.5022f, 0.1000038f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(112.0791f, 139.3915f, 0.6401573f)),
                        new WaitCoroutine(369851, 189910, 5000),
                        new MoveToScenePositionCoroutine(369851, 189910, "a3dun_iceCaves_EW_01", new Vector3(91.36786f, 135.6528f, 0.1000028f)),
                        new WaitCoroutine(369851, 189910, 5000),

 //                       new ClearAreaForNSecondsCoroutine (369851, 60, 365097, 2912417, 30)
                    }
            });

            // A3 - The Cursed Depths (369853)
            Bounties.Add(new BountyData
            {
                QuestId = 369853,
                Act = Act.A3,
                WorldId = 75434,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(369853, 75434, 2912417),

                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(180.0914f, 89.91431f, 0.5058147f)),
                        new WaitCoroutine(369853, 75434, 1000),
                        new InteractWithGizmoCoroutine(369853, 75434, 365097, 0, 5),

                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(180.0914f, 89.91431f, 0.5058147f)),
                        new WaitCoroutine(369853, 75434, 10000),
                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(191.4712f, 98.52448f, 0.1000001f)),
                        new WaitCoroutine(369853, 75434, 10000),
                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(178.8483f, 113.7626f, 0.9475988f)),
                        new WaitCoroutine(369853, 75434, 10000),
                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(161.0414f, 85.14581f, 0.1f)),
                        new WaitCoroutine(369853, 75434, 10000),
                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(191.4712f, 98.52448f, 0.1000001f)),
                        new WaitCoroutine(369853, 75434, 10000),
                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(178.8483f, 113.7626f, 0.9475988f)),
                        new WaitCoroutine(369853, 75434, 10000),
                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(161.0414f, 85.14581f, 0.1f)),
                        new WaitCoroutine(369853, 75434, 10000),
                        new MoveToScenePositionCoroutine(369853, 75434, "a3dun_Keep_NSEW_03_War", new Vector3(180.0914f, 89.91431f, 0.5058147f)),
                        new WaitCoroutine(369853, 75434, 10000),

//                     new ClearAreaForNSecondsCoroutine (369853, 60, 365097, 2912417, 30)
                    }
            });

            // A3 - The Cursed Caldera (369868)
            Bounties.Add(new BountyData
            {
                QuestId = 369868,
                Act = Act.A3,
                WorldId = 81934,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (369868, 81934, 2912417),

                        new InteractWithGizmoCoroutine (369868, 81934, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369868, 81934, "a3dun_Crater_NSW_01_W01_S01", new Vector3(66.18164f, 119.5161f, 0.4913304f)),
                        new WaitCoroutine(369868, 81934, 10000),
                        new MoveToScenePositionCoroutine(369868, 81934, "a3dun_Crater_NSW_01_W01_S01", new Vector3(64.47034f, 146.4824f, 0.1000076f)),
                        new WaitCoroutine(369868, 81934, 10000),
                        new MoveToScenePositionCoroutine(369868, 81934, "a3dun_Crater_NSW_01_W01_S01", new Vector3(32.42139f, 139.2941f, 0.1000038f)),
                        new WaitCoroutine(369868, 81934, 10000),
                        new MoveToScenePositionCoroutine(369868, 81934, "a3dun_Crater_NSW_01_W01_S01", new Vector3(45.05969f, 120.4674f, 0.1000062f)),
                        new WaitCoroutine(369868, 81934, 10000),
                        new MoveToScenePositionCoroutine(369868, 81934, "a3dun_Crater_NSW_01_W01_S01", new Vector3(32.42139f, 139.2941f, 0.1000038f)),
                        new WaitCoroutine(369868, 81934, 10000),
                        new MoveToScenePositionCoroutine(369868, 81934, "a3dun_Crater_NSW_01_W01_S01", new Vector3(45.05969f, 120.4674f, 0.1000062f)),
                        new WaitCoroutine(369868, 81934, 10000),

//                        new ClearAreaForNSecondsCoroutine (369868, 60, 365097, 2912417, 30)
                    }
            });

            // A4 - The Cursed Chapel (369878)
            Bounties.Add(new BountyData
            {
                QuestId = 369878,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (369878, 129305, 2912417),
                        new InteractWithGizmoCoroutine (369878, 129305, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(185.7661f, 167.3568f, 0.8628434f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(203.0096f, 162.3612f, 0.1000002f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(201.2533f, 187.0447f, 0.1000001f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(174.2996f, 181.8645f, 0.8628433f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(188.3243f, 169.1984f, 0.8628434f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(200.3032f, 153.4366f, 0.1f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(198.2024f, 182.1554f, 0.1f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(173.029f, 181.0692f, 0.8628433f)),
                        new WaitCoroutine(369878, 129305, 10000),
                        new MoveToScenePositionCoroutine(369878, 129305, "a4dun_spire_corrupt_SE_02", new Vector3(183.8937f, 165.0882f, 0.8628433f)),
                        new WaitCoroutine(369878, 129305, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (369878, 60, 365097, 2912417,30)
                    }
            });

            // A4 - The Cursed Dais (369900)
            Bounties.Add(new BountyData
            {
                QuestId = 369900,
                Act = Act.A4,
                WorldId = 109513,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (369900, 109513, 2912417),
                        new InteractWithGizmoCoroutine(369900, 109513, 365097, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (369900, 60, 365097, 2912417, 30)
                    }
            });

            // A5 - The Cursed Realm (369908)
            Bounties.Add(new BountyData
            {
                QuestId = 369908,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(369908, 338600, 2912417),
                        new InteractWithGizmoCoroutine (369908, 338600, 365097, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (369908, 60, 365097, 2912417, 20, false)
                    }
            });

            // A1 - The Cursed Cellar (369944)
            Bounties.Add(new BountyData
            {
                QuestId = 369944,
                Act = Act.A1,
                WorldId = 106752,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (369944, 71150, 106752, 1107870150, 176007),
                        new MoveToMapMarkerCoroutine (369944, 106752, 2912417),
                        new InteractWithGizmoCoroutine (369944,106752, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(369944, 106752, "trOut_oldTristram_Cellar_E", new Vector3(124.3548f, 113.0747f, 0.1f)),
                        new WaitCoroutine(369944, 106752, 10000),
                        new MoveToScenePositionCoroutine(369944, 106752, "trOut_oldTristram_Cellar_E", new Vector3(139.9767f, 131.7817f, 0.1000038f)),
                        new WaitCoroutine(369944, 106752, 10000),
                        new MoveToScenePositionCoroutine(369944, 106752, "trOut_oldTristram_Cellar_E", new Vector3(103.9924f, 132.6982f, 0.1000038f)),
                        new WaitCoroutine(369944, 106752, 10000),
                        new MoveToScenePositionCoroutine(369944, 106752, "trOut_oldTristram_Cellar_E", new Vector3(132.0145f, 145.1403f, 0.1000002f)),
                        new WaitCoroutine(369944, 106752, 10000),
                        new MoveToScenePositionCoroutine(369944, 106752, "trOut_oldTristram_Cellar_E", new Vector3(128.9004f, 132.4343f, 0.1000037f)),
                        new WaitCoroutine(369944, 106752, 10000),
                        new MoveToScenePositionCoroutine(369944, 106752, "trOut_oldTristram_Cellar_E", new Vector3(124.0268f, 106.3538f, 0.1f)),
						
 //                       new ClearAreaForNSecondsCoroutine (369944, 60, 365097, 2912417, 50)
                    }
            });

            // A5 - The Cursed City (369952)
            Bounties.Add(new BountyData
            {
                QuestId = 369952,
                Act = Act.A5,
                WorldId = 341040,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (369952, 267412, 341040, 1344182686, 0),
                        new MoveToMapMarkerCoroutine (369952,341040, 2912417),
                        new InteractWithGizmoCoroutine (369952,341040, 365097, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (369952, 60, 365097, 2912417, 30)
                    }
            });

            // A1 - The Cursed Court (375191)
            Bounties.Add(new BountyData
            {
                QuestId = 375191,
                Act = Act.A1,
                WorldId = 50582,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (375191,50582, 2912417),
                        new InteractWithGizmoCoroutine (375191,50582, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(375191, 50582, "trDun_Cath_SW_Hall_01", new Vector3(142.7451f, 127.8218f, -24.9f)),
                        new WaitCoroutine(375191, 50582, 10000),
                        new MoveToScenePositionCoroutine(375191, 50582, "trDun_Cath_SW_Hall_01", new Vector3(157.5084f, 141.0096f, -22.92235f)),
                        new WaitCoroutine(375191, 50582, 10000),
                        new MoveToScenePositionCoroutine(375191, 50582, "trDun_Cath_SW_Hall_01", new Vector3(144.483f, 157.0942f, -22.67825f)),
                        new WaitCoroutine(375191, 50582, 10000),
                        new MoveToScenePositionCoroutine(375191, 50582, "trDun_Cath_SW_Hall_01", new Vector3(128.2968f, 141.8916f, -23.27051f)),
                        new WaitCoroutine(375191, 50582, 10000),
                        new MoveToScenePositionCoroutine(375191, 50582, "trDun_Cath_SW_Hall_01", new Vector3(144.0414f, 142.2489f, -22.926f)),
                        new WaitCoroutine(375191, 50582, 10000),
                        new MoveToScenePositionCoroutine(375191, 50582, "trDun_Cath_SW_Hall_01", new Vector3(138.0294f, 129.1142f, -24.9f)),
                        new WaitCoroutine(375191, 50582, 10000),

 //                     new ClearAreaForNSecondsCoroutine (375191, 60, 365097, 2912417, 30)
                    }
            });

            // A1 - The Cursed Chamber of Bone (375198)
            Bounties.Add(new BountyData
            {
                QuestId = 375198,
                Act = Act.A1,
                WorldId = 50579,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (375198, 50579, 2912417),

                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(96.61365f, 143.3563f, 0.1000008f)),

                        new InteractWithGizmoCoroutine (375198,50579, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(78.06195f, 118.1667f, 1.457689f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(100.1196f, 115.9618f, 2.645245f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(122.5316f, 116.9459f, 1.237238f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(97.40033f, 117.3986f, 2.177974f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(100.0737f, 137.4675f, 1.262186f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(99.34674f, 116.4901f, 2.515652f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(78.06195f, 118.1667f, 1.457689f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(100.1196f, 115.9618f, 2.645245f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(122.5316f, 116.9459f, 1.237238f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(97.40033f, 117.3986f, 2.177974f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(100.0737f, 137.4675f, 1.262186f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(99.34674f, 116.4901f, 2.515652f)),
                        new WaitCoroutine(375198, 50579, 5000),
                        new MoveToScenePositionCoroutine(375198, 50579, "trDun_Cath_NSW_01", new Vector3(100.7628f, 147.187f, 0.1f)),
                        new WaitCoroutine(375198, 50579, 5000),
						
//                        new ClearAreaForNSecondsCoroutine (375198, 90, 365097, 2912417, 30, false)
                    }
            });

            // A1 - The Cursed Hatchery (375201)
            Bounties.Add(new BountyData
            {
                QuestId = 375201,
                Act = Act.A1,
                WorldId = 180550,
                QuestType = BountyQuestType.ClearCurse,
                LevelAreaIds = new HashSet<int> { 78572 },
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (375201,180550, 2912417),
                        new InteractWithGizmoCoroutine (375201,180550, 365097, 2912417, 5),
                    //new ClearAreaForNSecondsCoroutine (375201, 60, 365097, 2912417, 30)
                    new ClearAreaForNSecondsCoroutine (375201, 60, 365097, 2912417, 30)
                }
            });

            // A2 - The Cursed Spire (375257)
            Bounties.Add(new BountyData
            {
                QuestId = 375257,
                Act = Act.A2,
                WorldId = 50610,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (375257, 50613, 50610, -1363317799, 185067),
                        new MoveToMapMarkerCoroutine (375257,50610, 2912417),

                        new InteractWithGizmoCoroutine (375257,50610, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02", new Vector3(82.68777f, 98.2832f, 0.1000001f)),
                        new WaitCoroutine(375257, 50610, 10000),
                        new MoveToScenePositionCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02", new Vector3(60.56305f, 100.6702f, 0.1000001f)),
                        new WaitCoroutine(375257, 50610, 10000),
                        new MoveToScenePositionCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02", new Vector3(78.98157f, 120.9982f, 0.1000008f)),
                        new WaitCoroutine(375257, 50610, 10000),
                        new MoveToScenePositionCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02", new Vector3(102.0769f, 103.844f, 0.1000001f)),
                        new WaitCoroutine(375257, 50610, 10000),
                        new MoveToScenePositionCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02", new Vector3(81.83029f, 117.8888f, 0.1839012f)),
                        new WaitCoroutine(375257, 50610, 10000),
                        new MoveToScenePositionCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02", new Vector3(64.01669f, 104.2493f, 0.1000001f)),
                        new WaitCoroutine(375257, 50610, 10000),
                        new MoveToScenePositionCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02", new Vector3(81.71918f, 96.44299f, 0.1000001f)),
						
//                      new ClearAreaForNSecondsCoroutine (375257, 60, 365097, 2912417, 30)
                    }
            });

            // A2 - The Cursed Pit (375261)
            Bounties.Add(new BountyData
            {
                QuestId = 375261,
                Act = Act.A2,
                WorldId = 50611,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (375261, 50613, 50611, -1363317798, 185067),
                        new MoveToMapMarkerCoroutine (375261, 50611, 2912417),
                        new InteractWithGizmoCoroutine (375261,50611, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(375261, 50611, "a2dun_Zolt_Hall_EW_02", new Vector3(89.11694f, 97.45593f, 0.1000001f)),
                        new WaitCoroutine(375261, 50611, 10000),
                        new MoveToScenePositionCoroutine(375261, 50611, "a2dun_Zolt_Hall_EW_02", new Vector3(90.1842f, 81.74347f, 0.1250016f)),
                        new WaitCoroutine(375261, 50611, 10000),
                        new MoveToScenePositionCoroutine(375261, 50611, "a2dun_Zolt_Hall_EW_02", new Vector3(114.6979f, 99.00391f, 0.1000002f)),
                        new WaitCoroutine(375261, 50611, 10000),
                        new MoveToScenePositionCoroutine(375261, 50611, "a2dun_Zolt_Hall_EW_02", new Vector3(89.98718f, 116.8755f, 0.1000004f)),
                        new WaitCoroutine(375261, 50611, 10000),
                        new MoveToScenePositionCoroutine(375261, 50611, "a2dun_Zolt_Hall_EW_02", new Vector3(62.31152f, 97.84348f, 0.1000001f)),
                        new WaitCoroutine(375261, 50611, 10000),
                        new MoveToScenePositionCoroutine(375261, 50611, "a2dun_Zolt_Hall_EW_02", new Vector3(91.53711f, 99.19025f, 0.1000001f)),
                        new WaitCoroutine(375261, 50611, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (375261, 60, 365097, 2912417, 30)
                    }
            });

            // A5 - The Cursed Forum (375268)
            Bounties.Add(new BountyData
            {
                QuestId = 375268,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (375268, 261712, 2912417),
                        new MoveToSceneCoroutine(375268, 261712, "x1_westm_NSEW_06"),
                        new InteractWithGizmoCoroutine (375268,261712, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(115.2922f, 106.7599f, 5.1f)),
                        new WaitCoroutine(375268, 261712, 10000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(120.9968f, 91.24005f, 5.1f)),
                        new WaitCoroutine(375268, 261712, 10000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(147.4745f, 110.2294f, 5.1f)),
                        new WaitCoroutine(375268, 261712, 10000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(106.0229f, 141.2579f, 5.1f)),
                        new WaitCoroutine(375268, 261712, 10000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(115.2922f, 106.7599f, 5.1f)),
                        new WaitCoroutine(375268, 261712, 10000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(106.0229f, 141.2579f, 5.1f)),
                        new WaitCoroutine(375268, 261712, 10000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(115.2922f, 106.7599f, 5.1f)),
                        new WaitCoroutine(375268, 261712, 10000),
						
//                        new ClearAreaForNSecondsCoroutine (375268, 60, 365097, 2912417, 30, false),
                    }
            });

            // A5 - The Cursed War Room (375275)
            Bounties.Add(new BountyData
            {
                QuestId = 375275,
                Act = Act.A5,
                WorldId = 271233,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveThroughDeathGates(375275,271233,1),
                        new MoveToMapMarkerCoroutine (375275, 271233, 2912417),
                        new InteractWithGizmoCoroutine(375275, 271233, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(124.3028f, 116.0892f, -19.25122f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(137.4039f, 121.5612f, -19.25122f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(126.3025f, 136.8546f, -19.9f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(105.034f, 120.4681f, -19.9f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(124.6398f, 115.7779f, -19.25122f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(145.7731f, 116.5219f, -19.25122f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(129.1796f, 134.8818f, -19.43344f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(102.9052f, 117.5618f, -19.9f)),
                        new WaitCoroutine(375275, 271233, 10000),
                        new MoveToScenePositionCoroutine(375275, 271233, "x1_fortress_NE_02", new Vector3(125.6252f, 112.6357f, -19.25122f)),
                        new WaitCoroutine(375275, 271233, 10000),
						
//                      new ClearAreaForNSecondsCoroutine (375275, 60, 365097, 2912417, 30)
                    }
            });

            // A5 - The Cursed Peat (375278)
            Bounties.Add(new BountyData
            {
                QuestId = 375278,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine (375278, 267412, 2912417),
                        new InteractWithGizmoCoroutine (375278,267412, 365097, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (375278, 60, 365097, 2912417, 30)
                    }
            });

            // A5 - The Cursed Bone Pit (375348)
            Bounties.Add(new BountyData
            {
                QuestId = 375348,
                Act = Act.A5,
                WorldId = 338977,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (375348, 263494, 338976, -660641888, 329025),
 //                       new EnterLevelAreaCoroutine (375348, 338976, 338977, 2115492897, 175482, true),
						new EnterLevelAreaCoroutine(375348, 338976, 0, 2115492897, 0),
                        new MoveToMapMarkerCoroutine (375348, 338977, 2912417),
                        new InteractWithGizmoCoroutine (375348,338977, 365097, 2912417, 30),
                        new ClearAreaForNSecondsCoroutine (375348, 60, 365097, 2912417, 30)
                    }
            });

            // A4 - The Cursed Pulpit (409897)
            Bounties.Add(new BountyData
            {
                QuestId = 409897,
                Act = Act.A4,
                WorldId = 181644,
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = 409517,
                Coroutines = new List<ISubroutine>
                    {

                        new MoveToMapMarkerCoroutine(409897, 409511, -970799629),
						
//						new InteractWithGizmoCoroutine(409897, 409511, 204183, -970799629, 5),
						new EnterLevelAreaCoroutine(409897, 409511, 0, -970799629, 204183),

                        new MoveToPositionCoroutine(181644, new Vector3(349, 557, 0)),
                        new MoveToPositionCoroutine(181644, new Vector3(348, 421, -14)),
                        new MoveToPositionCoroutine(181644, new Vector3(341, 351, -12)),

                        new InteractWithGizmoCoroutine (409897,181644, 365097, 2912417, 5),

                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(111.6542f, 110.5945f, -12.64518f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(67.57697f, 111.5117f, -14.9f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(119.326f, 77.41565f, -14.66502f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(161.381f, 120.7327f, -14.9f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(108.6663f, 148.4656f, -14.9f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(73.42627f, 108.64f, -14.9f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(119.326f, 77.41565f, -14.66502f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(161.381f, 120.7327f, -14.9f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(108.6663f, 148.4656f, -14.9f)),
                        new WaitCoroutine(409897,181644, 5000),
                        new MoveToScenePositionCoroutine(409897, 181644, "a4dun_spire_SigilRoom_C", new Vector3(73.42627f, 108.64f, -14.9f)),
                        new WaitCoroutine(409897,181644, 5000),
						
//                     new ClearAreaForNSecondsCoroutine (409897, 80, 365097, 2912417, 30, false)
                    }
            });
        }

        private static void AddBounties()
        {
            // A5 - 현상금 사냥: 톨리퍼 최후의 저항 (359426)
            Bounties.Add(new BountyData
            {
                QuestId = 359426,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 51,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359426, 261712, 2912417),
//					new MoveToActorCoroutine(359426, 261712, 312375),
					new InteractWithGizmoCoroutine(359426, 261712, 312375, 2912417, 5),

                    new MoveToScenePositionCoroutine(359426, 261712, "x1_westm_NS_03", new Vector3(112.1254f, 119.2485f, 10.1f)),
                    new WaitCoroutine(359426, 261712, 10000),
                    new MoveToScenePositionCoroutine(359426, 261712, "x1_westm_NS_03", new Vector3(162.8093f, 119.7774f, 10.1f)),
                    new WaitCoroutine(359426, 261712, 10000),
                    new MoveToScenePositionCoroutine(359426, 261712, "x1_westm_NS_03", new Vector3(112.1254f, 119.2485f, 10.1f)),
                    new WaitCoroutine(359426, 261712, 10000),
                    new MoveToScenePositionCoroutine(359426, 261712, "x1_westm_NS_03", new Vector3(70.59937f, 123.0006f, 10.1f)),
                    new WaitCoroutine(359426, 261712, 10000),
                    new MoveToScenePositionCoroutine(359426, 261712, "x1_westm_NS_03", new Vector3(112.1254f, 119.2485f, 10.1f)),
                    new WaitCoroutine(359426, 261712, 10000),

//					new ClearAreaForNSecondsCoroutine(359426, 60, 312375, 2912417, 30),
				}
            });

            // A5 - 현상금 사냥: 쥐잡기 (368613)
            Bounties.Add(new BountyData
            {
                QuestId = 368613,
                Act = Act.A5,
                WorldId = 263494, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 53,
                Coroutines = new List<ISubroutine>
                {
//					new MoveToMapMarkerCoroutine(368613, 263494, -1883144025),
					new EnterLevelAreaCoroutine(368613, 263494, 0, -1883144025, 333736),
                    new ClearLevelAreaCoroutine(368613),
                }
            });




            // A5 - 현상금 사냥: 헤드 몬 톤 처치 (428251)
            Bounties.Add(new BountyData
            {
                QuestId = 428251,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.KillMonster,
                WaypointNumber = 59,
                Coroutines = new List<ISubroutine>
                {
                    new KillUniqueMonsterCoroutine (428251,408254, 435470, 0),
                    new ClearLevelAreaCoroutine(428251),
                }
            });

            // A1 - Clear the Cave of the Moon Clan (344547)
            Bounties.Add(new BountyData
            {
                QuestId = 344547,
                Act = Act.A1,
                WorldId = 82511,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToPositionCoroutine(71150, new Vector3(2440, 4620, 0)),
                        new EnterLevelAreaCoroutine (344547, 71150, 82502, -1187439574, 176008),
                        new EnterLevelAreaCoroutine (344547, 82502, 82511, -1187439573, 176038, true),
                        new ClearLevelAreaCoroutine (344547)
                    }
            });

            // A1 - Kill Logrut the Warrior (344497)
            Bounties.Add(new BountyData
            {
                QuestId = 344497,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 93632, 19940 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (344497,71150, 218469, -373794395),
                        new ClearLevelAreaCoroutine (344497)
                    }
            });

            // A1 - Kill Buras the Impaler (344499)
            Bounties.Add(new BountyData
            {
                QuestId = 344499,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 93632, 19940 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (344499,71150, 218473, 1205746285),
                        new ClearLevelAreaCoroutine (344499 )
                    }
            });

            // A1 - Kill Lorzak the Powerful (344501)
            Bounties.Add(new BountyData
            {
                QuestId = 344501,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 93632, 19940 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (344501,71150, 218508, -1684494028),
                        new ClearLevelAreaCoroutine (344501 )
                    }
            });

            // A1 - Kill Red Rock (344503)
            Bounties.Add(new BountyData
            {
                QuestId = 344503,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 93632, 19940 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (344503,71150, 218536, 2067238310),
                        new ClearLevelAreaCoroutine (344503)
                    }
            });


            // A1 - Clear the Den of the Fallen (345488)
            Bounties.Add(new BountyData
            {
                QuestId = 345488,
                Act = Act.A1,
                WorldId = 194231,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (345488, 71150, 135193, -2043651426, 176003),
                        new EnterLevelAreaCoroutine (345488, 135193, 194231, -711350153, 176038, true),
                        new ClearLevelAreaCoroutine (345488)
                    }
            });

            // A1 - Kill Mange (345490)
            Bounties.Add(new BountyData
            {
                QuestId = 345490,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345490,71150, 218332, -275004780),
                        new ClearLevelAreaCoroutine (345490)
                    }
            });

            // A1 - Clear the Scavenger's Den (345496)
            Bounties.Add(new BountyData
            {
                QuestId = 345496,
                Act = Act.A1,
                WorldId = 81164,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                         new EnterLevelAreaCoroutine(345496, 71150, 0, 925091454, 175501),
                        new EnterLevelAreaCoroutine(345496, 81163, 0, 925091455, 176038),
                        new ClearLevelAreaCoroutine (345496)
                    }
            });

            // A1 - Kill Melmak (345498)
            Bounties.Add(new BountyData
            {
                QuestId = 345498,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345498,71150, 218428, 1683727460),
                        new ClearLevelAreaCoroutine (345498)
                    }
            });

            // A1 - Kill Grimsmack (345507)
            Bounties.Add(new BountyData
            {
                QuestId = 345507,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345507,71150, 218444, 1737003285),
                        new ClearLevelAreaCoroutine (345507)
                    }
            });

            // A1 - Clear the Crypt of the Ancients (345517)
            Bounties.Add(new BountyData
            {
                QuestId = 345517,
                Act = Act.A1,
                WorldId = 60394,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (345517, 71150, 60394, 976523526, 176008),
                        new ClearLevelAreaCoroutine (345517)
                    }
            });

            // A1 - Clear Warrior's Rest (345520)
            Bounties.Add(new BountyData
            {
                QuestId = 345520,
                Act = Act.A1,
                WorldId = 60393,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (345520, 71150, 60393, 976523525, 176008),
                        new ClearLevelAreaCoroutine (345520)
                    }
            });

            // A1 - Kill Zhelobb the Venomous (345522)
            Bounties.Add(new BountyData
            {
                QuestId = 345522,
                Act = Act.A1,
                WorldId = 180550,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345522,180550, 218448, 1307894141),
                        new ClearLevelAreaCoroutine (345522)
                    }
            });

            // A1 - Kill Venimite (345524)
            Bounties.Add(new BountyData
            {
                QuestId = 345524,
                Act = Act.A1,
                WorldId = 180550,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345524,180550, 218456, -1487666649),
                        new ClearLevelAreaCoroutine (345524)
                    }
            });

            // A1 - Kill Rathlin the Widowmaker (345526)
            Bounties.Add(new BountyData
            {
                QuestId = 345526,
                Act = Act.A1,
                WorldId = 180550,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345526,180550, 218458, 1425915828),
                        new ClearLevelAreaCoroutine (345526)
                    }
            });

            // A1 - Kill Crassus the Tormentor (345542)
            Bounties.Add(new BountyData
            {
                QuestId = 345542,
                Act = Act.A1,
                WorldId = 2826,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345542,2826, 218676, -11732714),
                        new ClearLevelAreaCoroutine (345542)
                    }
            });

            // A1 - Kill Bludgeonskull  (345544)
            Bounties.Add(new BountyData
            {
                QuestId = 345544,
                Act = Act.A1,
                WorldId = 2826,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345544,2826, 218672, -820238703),
                        new ClearLevelAreaCoroutine (345544)
                    }
            });

            // A1 - Kill Qurash the Reviled (345862)
            Bounties.Add(new BountyData
            {
                QuestId = 345862,
                Act = Act.A1,
                WorldId = 180550,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345862,180550, 218462, 1425915829),
                        new ClearLevelAreaCoroutine (345862)
                    }
            });

            // A2 - Kill Saha the Slasher (345960)
            Bounties.Add(new BountyData
            {
                QuestId = 345960,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345960,70885, 221367, -1258389668),
                        new ClearLevelAreaCoroutine (345960)
                    }
            });

            // A2 - Kill Shondar the Invoker (346036)
            Bounties.Add(new BountyData
            {
                QuestId = 346036,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 19839,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346036,70885, 222001, 898630437),
                        new ClearLevelAreaCoroutine (346036)
                    }
            });

            // A2 - Clear the Ancient Cave (346069)
            Bounties.Add(new BountyData
            {
                QuestId = 346069,
                Act = Act.A2,
                WorldId = 194240,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(346069, 70885, 1352061372),
//                     new EnterLevelAreaCoroutine (346069, 70885, 62568, 1352061372, 176003),
						new EnterLevelAreaCoroutine(346069, 70885, 0, 1352061372, 176003),
                        new EnterLevelAreaCoroutine(346069, 62568, 0, 622615957, 176002),
                        new ClearLevelAreaCoroutine (346069)
                    }
            });

            // A2 - Clear the Flooded Cave (346071)
            Bounties.Add(new BountyData
            {
                QuestId = 346071,
                Act = Act.A2,
                WorldId = 161011,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(346071, 70885, 1352061373),
//                      new EnterLevelAreaCoroutine (346071, 70885, 62569, 1352061373, 176003),
						new EnterLevelAreaCoroutine(346071, 70885, 0, 1352061373, 176003),
                        new EnterLevelAreaCoroutine (346071, 62569, 161011, -1718038890, 176002, true),
                        new ClearLevelAreaCoroutine (346071)
                    }
            });

            // A2 - Kill Scar Talon (346075)
            Bounties.Add(new BountyData
            {
                QuestId = 346075,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346075,70885, 222011, 865852689),
                        new ClearLevelAreaCoroutine (346075)
                    }
            });

            // A2 - Clear the Cave of Burrowing Horror (346088)
            Bounties.Add(new BountyData
            {
                QuestId = 346088,
                Act = Act.A2,
                WorldId = 218970,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(346088, 70885, 1028158260),
                        new ClearAreaForNSecondsCoroutine(346088, 20, 0, 1028158260, 20),
                        new EnterLevelAreaCoroutine(346088, 70885, 0, 1028158260, 175501),
                        new EnterLevelAreaCoroutine(346088, 111666, 0, 1028158261, 176001),
                        new ClearLevelAreaCoroutine (346088)
                    }
            });

            // A2 - Kill Raiha (346090)
            Bounties.Add(new BountyData
            {
                QuestId = 346090,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346090,70885, 222339, 979055773),
                        new ClearLevelAreaCoroutine (346090)
                    }
            });

            // A2 - Kill Blarg (346092)
            Bounties.Add(new BountyData
            {
                QuestId = 346092,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346092,70885, 222352, 1108776135),
                        new ClearLevelAreaCoroutine (346092)
                    }
            });

            // A2 - Kill Bloodfeather (346094)
            Bounties.Add(new BountyData
            {
                QuestId = 346094,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346094,70885, 222385, 865852690),
                        new ClearLevelAreaCoroutine (346094)
                    }
            });

            // A2 - Kill the Archivist (346108)
            Bounties.Add(new BountyData
            {
                QuestId = 346108,
                Act = Act.A2,
                WorldId = 50610,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346108, 50613, 50610, -1363317799, 185067),
                        new KillUniqueMonsterCoroutine (346108,50610, 165602, -1954248257),
                        new ClearLevelAreaCoroutine (346108)
                    }
            });



            // A2 - Kill Hellscream (346115)
            Bounties.Add(new BountyData
            {
                QuestId = 346115,
                Act = Act.A2,
                WorldId = 50610,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346115, 50613, 50610, -1363317799, 185067),
                        new KillUniqueMonsterCoroutine (346115,50610, 222427, -1743829606),
                        new ClearLevelAreaCoroutine (346115)
                    }
            });

            // A2 - Kill Thrum (346117)
            Bounties.Add(new BountyData
            {
                QuestId = 346117,
                Act = Act.A2,
                WorldId = 80589,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346117, 50613, 80589, 1867081263, 228873),
                        new KillUniqueMonsterCoroutine (346117,80589, 222523, 221109478),
                        new ClearLevelAreaCoroutine (346117)
                    }
            });

            // A2 - Kill the Tomekeeper (346119)
            Bounties.Add(new BountyData
            {
                QuestId = 346119,
                Act = Act.A2,
                WorldId = 80589,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346119, 50613, 80589, 1867081263, 228873),
                        new KillUniqueMonsterCoroutine (346119,80589, 222526, -65631842),
                        new ClearLevelAreaCoroutine (346119)
                    }
            });

            // A2 - Kill Mage Lord Skomara (346121)
            Bounties.Add(new BountyData
            {
                QuestId = 346121,
                Act = Act.A2,
                WorldId = 50611,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346121, 50613, 50611, -1363317798, 185067),
                        new KillUniqueMonsterCoroutine (346121,50611, 222502, 1133968439),
                        new ClearLevelAreaCoroutine (346121)
                    }
            });

            // A2 - Kill Mage Lord Ghuyan (346123)
            Bounties.Add(new BountyData
            {
                QuestId = 346123,
                Act = Act.A2,
                WorldId = 50611,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346123, 50613, 50611, -1363317798, 185067),
                        new KillUniqueMonsterCoroutine (346123,50611, 222510, 1198145179),
                        new ClearLevelAreaCoroutine (346123)
                    }
            });

            // A3 - Kill Bricktop (346128)
            Bounties.Add(new BountyData
            {
                QuestId = 346128,
                Act = Act.A3,
                WorldId = 93099,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346128,93099, 220397, 154606422),
                        new ClearLevelAreaCoroutine (346128)
                    }
            });

            // A3 - Kill Bashface the Truncheon (346130)
            Bounties.Add(new BountyData
            {
                QuestId = 346130,
                Act = Act.A3,
                WorldId = 93099,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346130,93099, 220395, 154606421),
                        new ClearLevelAreaCoroutine (346130)
                    }
            });

            // A3 - Kill Marchocyas (346132)
            Bounties.Add(new BountyData
            {
                QuestId = 346132,
                Act = Act.A3,
                WorldId = 81019,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346132, 93099, 81019, -1078336204, 176002),
                        new KillUniqueMonsterCoroutine (346132,81019, 220232, 1020521099),
                        new ClearLevelAreaCoroutine (346132)
                    }
            });

            // A3 - Kill Thornback (346148)
            Bounties.Add(new BountyData
            {
                QuestId = 346148,
                Act = Act.A3,
                WorldId = 93104,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346148,93104, 220455, -1497339214),
                        new ClearLevelAreaCoroutine (346148)
                    }
            });

            // A3 - Kill Captain Donn Adams (346154)
            Bounties.Add(new BountyData
            {
                QuestId = 346154,
                Act = Act.A3,
                WorldId = 93104,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346154,93104, 220710, 207753752),
                        new ClearLevelAreaCoroutine (346154)
                    }
            });

            // A3 - Kill Captain Dale (346157)
            Bounties.Add(new BountyData
            {
                QuestId = 346157,
                Act = Act.A3,
                WorldId = 93104,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346157,93104, 220468, 207753751),
                        new ClearLevelAreaCoroutine (346157)
                    }
            });

            // A3 - Kill the Crusher (346160)
            Bounties.Add(new BountyData
            {
                QuestId = 346160,
                Act = Act.A3,
                WorldId = 75434,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346160,75434, 220476, -1902915435),
                        new ClearLevelAreaCoroutine (346160)
                    }
            });

            // A3 - Kill Belagg Pierceflesh (346162)
            Bounties.Add(new BountyData
            {
                QuestId = 346162,
                Act = Act.A3,
                WorldId = 136415,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346162,136415, 220499, -650015436),
                        new ClearLevelAreaCoroutine (346162)
                    }
            });

            // A3 - Kill Gugyn the Gauntlet (346164)
            Bounties.Add(new BountyData
            {
                QuestId = 346164,
                Act = Act.A3,
                WorldId = 136415,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346164,136415, 220491, 681588934),
                        new ClearLevelAreaCoroutine (346164)
                    }
            });

            // A3 - Clear the Caverns of Frost (346190)
            Bounties.Add(new BountyData
            {
                QuestId = 346190,
                Act = Act.A3,
                WorldId = 95804,//221688,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346190, 95804, 0, 1029056444, 176003),
                        new EnterLevelAreaCoroutine(346190, 189259, 0, 151580180, 176038),
                        new ClearLevelAreaCoroutine (346190)
                    }
            });

            // A3 - Clear the Icefall Caves (346192)
            Bounties.Add(new BountyData
            {
                QuestId = 346192,
                Act = Act.A3,
                WorldId = 221689,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine(346192, 95804, 189910, 942020622, 176003),
                        new EnterLevelAreaCoroutine(346192, 189910, 0, -802596186, 176038),
                        new ClearLevelAreaCoroutine (346192)
                    }
            });

            // A3 - Kill Groak the Brawler (346194)
            Bounties.Add(new BountyData
            {
                QuestId = 346194,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346194,95804, 220688, 188457921),
                        new ClearLevelAreaCoroutine (346194)
                    }
            });

            // A3 - Kill Mehshak the Abomination (346196)
            Bounties.Add(new BountyData
            {
                QuestId = 346196,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346196,95804, 220691, 1668992859),
                        new ClearLevelAreaCoroutine (346196)
                    }
            });

            // A3 - Kill Shertik the Brute (346200)
            Bounties.Add(new BountyData
            {
                QuestId = 346200,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346200,95804, 220708, 188457923),
                        new ClearLevelAreaCoroutine (346200)
                    }
            });

            // A3 - Kill Emberwing (346202)
            Bounties.Add(new BountyData
            {
                QuestId = 346202,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        // todo needs work, failing.as
                        new KillUniqueMonsterCoroutine (346202,95804, 220701, -1037000756),
                        new ClearLevelAreaCoroutine (346202)
                    }
            });

            // A3 - Kill Garganug (346204)
            Bounties.Add(new BountyData
            {
                QuestId = 346204,
                Act = Act.A3,
                WorldId = 95804,
                WaypointLevelAreaId = 112565,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346204,95804, 220773, -838403687),
                        new ClearLevelAreaCoroutine (346204)
                    }
            });

            // A3 - Kill Hyrug the Malformed (346215)
            Bounties.Add(new BountyData
            {
                QuestId = 346215,
                Act = Act.A3,
                WorldId = 119641,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346215,119641, 220857, -1869063934),
                        new ClearLevelAreaCoroutine (346215)
                    }
            });

            // A3 - Kill Maggrus the Savage (346217)
            Bounties.Add(new BountyData
            {
                QuestId = 346217,
                Act = Act.A3,
                WorldId = 139272,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346217, 119641, 139272, 43541819, 176001),
                        new KillUniqueMonsterCoroutine (346217,139272, 220862, -1869063933),
                        new ClearLevelAreaCoroutine (346217)
                    }
            });

            // A3 - Kill Charuch the Spear (346219)
            Bounties.Add(new BountyData
            {
                QuestId = 346219,
                Act = Act.A3,
                WorldId = 81934,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346219,81934, 220853, 1343937195),
                        new ClearLevelAreaCoroutine (346219)
                    }
            });

            // A3 - Kill Mhawgann the Unholy (346222)
            Bounties.Add(new BountyData
            {
                QuestId = 346222,
                Act = Act.A3,
                WorldId = 81934,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346222,81934, 220850, -1546303118),
                        new ClearLevelAreaCoroutine (346222)
                    }
            });

            // A3 - Kill Severclaw  (346225)
            Bounties.Add(new BountyData
            {
                QuestId = 346225,
                Act = Act.A3,
                WorldId = 81049,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346225,81049, 220795, -1740074819),
                        new ClearLevelAreaCoroutine (346225)
                    }
            });

            // A3 - Kill Valifahr the Noxious (346228)
            Bounties.Add(new BountyData
            {
                QuestId = 346228,
                Act = Act.A3,
                WorldId = 81049,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346228,81049, 220789, 1965678404),
                        new ClearLevelAreaCoroutine (346228)
                    }
            });

            // A3 - Kill Demonika the Wicked (346230)
            Bounties.Add(new BountyData
            {
                QuestId = 346230,
                Act = Act.A3,
                WorldId = 79401,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346230,79401, 220783, 236772676),
                        new ClearLevelAreaCoroutine (346230)
                    }
            });

            // A3 - Kill Axgore the Cleaver (346232)
            Bounties.Add(new BountyData
            {
                QuestId = 346232,
                Act = Act.A3,
                WorldId = 119290,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (346232,119290, 220884, -1180611613),
                        new ClearLevelAreaCoroutine (346232)
                    }
            });

            // A3 - Kill Brimstone (346235)
            Bounties.Add(new BountyData
            {
                QuestId = 346235,
                Act = Act.A3,
                WorldId = 80763,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (346235, 79401, 80763, 2083727833, 176001),
                        new KillUniqueMonsterCoroutine (346235,80763, 220812, -1180611614),
                        new ClearLevelAreaCoroutine (346235)
                    }
            });

            // A1 - Kill Battlerage the Plagued (347011)
            Bounties.Add(new BountyData
            {
                QuestId = 347011,
                Act = Act.A1,
                WorldId = 2826,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347011,2826, 218674, -820238702),
                        new ClearLevelAreaCoroutine (347011)
                    }
            });

            // A1 - Kill Treefist Woodhead (347020)
            Bounties.Add(new BountyData
            {
                QuestId = 347020,
                Act = Act.A1,
                WorldId = 58983,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347020,58983, 218678, -820238701),
                        new ClearLevelAreaCoroutine (347020)
                    }
            });

            // A1 - Kill Boneslag the Berserker (347023)
            Bounties.Add(new BountyData
            {
                QuestId = 347023,
                Act = Act.A1,
                WorldId = 58983,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347023,58983, 220034, -820238700),
                        new ClearLevelAreaCoroutine (347023)
                    }
            });

            // A1 - Kill Hawthorne Gable (347054)
            Bounties.Add(new BountyData
            {
                QuestId = 347054,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347054,71150, 218441, 1811966428),
                        new ClearLevelAreaCoroutine (347054)
                    }
            });

            // A1 - Kill Fecklar's Ghost (347056)
            Bounties.Add(new BountyData
            {
                QuestId = 347056,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347056,71150, 209553, 1811966427),
                        new ClearLevelAreaCoroutine (347056)
                    }
            });

            // A1 - Clear Khazra Den (347065)
            Bounties.Add(new BountyData
            {
                QuestId = 347065,
                Act = Act.A1,
                WorldId = 119888,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(347065, 71150, 2036518712),
                        new EnterLevelAreaCoroutine(347065, 71150, 119888, 2036518712, 176003),
                        new ClearLevelAreaCoroutine (347065)
                    }
            });

            // A1 - Kill Charger (347070)
            Bounties.Add(new BountyData
            {
                QuestId = 347070,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347070,71150, 218422, 2067238309),
                        new ClearLevelAreaCoroutine (347070)
                    }
            });

            // A1 - Kill Dreadclaw the Leaper (347073)
            Bounties.Add(new BountyData
            {
                QuestId = 347073,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347073,71150, 218424, 1962440661),
                        new ClearLevelAreaCoroutine (347073)
                    }
            });

            // A1 - Kill the Dataminer (347095)
            Bounties.Add(new BountyData
            {
                QuestId = 347095,
                Act = Act.A1,
                WorldId = 72636,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (347095, 71150, 72636, -1965109038, 176002),
                        new KillUniqueMonsterCoroutine (347095,72636, 225502, -504552226),
                        new ClearLevelAreaCoroutine (347095)
                    }
            });

            // A1 - Kill Digger O'Dell (347097)
            Bounties.Add(new BountyData
            {
                QuestId = 347097,
                Act = Act.A1,
                WorldId = 72637,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (347097, 71150, 0, -1965109037, 176002),
                        new KillUniqueMonsterCoroutine (347097,72637, 218206, -690325730),
                        new ClearLevelAreaCoroutine (347097)
                    }
            });

            // A1 - Kill Mira Eamon (347099)
            Bounties.Add(new BountyData
            {
                QuestId = 347099,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347099,71150, 85900, 75689489),
                        new ClearLevelAreaCoroutine (347099)
                    }
            });

            // A2 - Clear Sirocco Caverns (347525)
            Bounties.Add(new BountyData
            {
                QuestId = 347525,
                Act = Act.A2,
                WorldId = 220804,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(347525, 70885, 2000747858),
                        new EnterLevelAreaCoroutine (347525, 70885, 50589, 2000747858, 185067),
                        new MoveToMapMarkerCoroutine(347525, 70885, 2108407595),
                        new EnterLevelAreaCoroutine(347525, 50589, 0, 2108407595, 176001),
                        new ClearLevelAreaCoroutine (347525)
                    }
            });

            // A2 - Kill Gart the Mad (347532)
            Bounties.Add(new BountyData
            {
                QuestId = 347532,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347532,70885, 221377, 1708503799),
                        new ClearLevelAreaCoroutine (347532)
                    }
            });

            // A2 - Kill Hemit (347534)
            Bounties.Add(new BountyData
            {
                QuestId = 347534,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347534,70885, 221379, 1708503800),
                        new ClearLevelAreaCoroutine (347534)
                    }
            });

            // A2 - Kill Yeth (347560)
            Bounties.Add(new BountyData
            {
                QuestId = 347560,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 19825 },
                WaypointLevelAreaId = 19839,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347560,70885, 221810, -158539678),
                        new ClearLevelAreaCoroutine (347560)
                    }
            });

            // A2 - Kill High Cultist Murdos (347563)
            Bounties.Add(new BountyData
            {
                QuestId = 347563,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                //LevelAreaIds = new HashSet<int> { 19825, 19835 },
                WaypointLevelAreaId = 19839,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347563,70885, 221981, 168190871),
                        new ClearLevelAreaCoroutine (347563)
                    }
            });

            // A2 - Kill Jhorum the Cleric (347565)
            Bounties.Add(new BountyData
            {
                QuestId = 347565,
                Act = Act.A2,
                WorldId = 70885,
                WaypointLevelAreaId = 19839,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347565,70885, 221999, 168190872),
                        new ClearLevelAreaCoroutine (347565)
                    }
            });

            // A2 - Kill Sammash (347569)
            // added WaypointLevelAreaId
            Bounties.Add(new BountyData
            {
                QuestId = 347569,
                Act = Act.A2,
                WorldId = 70885,
                WaypointLevelAreaId = 19839,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
 //                       new MoveToMapMarkerCoroutine(347569, 70885 , -640315117),
                        new KillUniqueMonsterCoroutine (347569,70885, 222003, -640315117),
                        new ClearLevelAreaCoroutine (347569)
                    }
            });


            // A2 - Kill Bashiok (347600)
            Bounties.Add(new BountyData
            {
                QuestId = 347600,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(347600, 70885, -1086742663),
                        new KillUniqueMonsterCoroutine (347600, 70885, 215445, -1086742663),
                        new ClearLevelAreaCoroutine (347600)
                    }
            });

            // A2 - Kill Torsar (347604)
            Bounties.Add(new BountyData
            {
                QuestId = 347604,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(347604, 70885, -1719600680),
                        new KillUniqueMonsterCoroutine (347604,70885, 222180, -1719600680),
                        new ClearLevelAreaCoroutine (347604)
                    }
            });

            // A2 - Kill Plagar the Damned (347607)
            Bounties.Add(new BountyData
            {
                QuestId = 347607,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine (347607, 70885, 1840003643),
                        new KillUniqueMonsterCoroutine (347607,70885, 222400, 1840003643),
                        new ClearLevelAreaCoroutine (347607)
                    }
            });

            // A2 - Kill Rockgut (347650)
            Bounties.Add(new BountyData
            {
                QuestId = 347650,
                Act = Act.A2,
                WorldId = 50610,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (347650, 50613, 50610, -1363317799, 185067),
                        new KillUniqueMonsterCoroutine (347650,50610, 222413, -2016335963),
                        new ClearLevelAreaCoroutine (347650)
                    }
            });

            // A2 - Kill Mage Lord Caustus (347652)
            Bounties.Add(new BountyData
            {
                QuestId = 347652,
                Act = Act.A2,
                WorldId = 50611,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (347652, 50613, 50611, -1363317798, 185067),
                        new KillUniqueMonsterCoroutine (347652,50611, 222512, -499083571),
                        new ClearLevelAreaCoroutine (347652)
                    }
            });

            // A2 - Kill Mage Lord Flaydren (347654)
            Bounties.Add(new BountyData
            {
                QuestId = 347654,
                Act = Act.A2,
                WorldId = 50611,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (347654, 50613, 50611, -1363317798, 185067),
                        new KillUniqueMonsterCoroutine (347654,50611, 222511, 1356599065),
                        new ClearLevelAreaCoroutine (347654)
                    }
            });

            // A1 - Kill Sotnob the Fool (349026)
            Bounties.Add(new BountyData
            {
                QuestId = 349026,
                Act = Act.A1,
                WorldId = 58982,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349026,58982, 332432, -1795932517),
                        new ClearLevelAreaCoroutine (349026)
                    }
            });

            // A3 - Kill Axegrave the Executioner (349115)
            Bounties.Add(new BountyData
            {
                QuestId = 349115,
                Act = Act.A3,
                WorldId = 75434,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349115,75434, 220479, 709032730),
                        new ClearLevelAreaCoroutine (349115)
                    }
            });

            // A3 - Kill Lashtongue (349117)
            Bounties.Add(new BountyData
            {
                QuestId = 349117,
                Act = Act.A3,
                WorldId = 93104,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349117,93104, 220444, -1791040149),
                        new ClearLevelAreaCoroutine (349117)
                    }
            });

            // A3 - Kill Aloysius the Ghastly (349119)
            Bounties.Add(new BountyData
            {
                QuestId = 349119,
                Act = Act.A3,
                WorldId = 75434,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349119,75434, 220481, -1791040148),
                        new ClearLevelAreaCoroutine (349119)
                    }
            });

            // A3 - Kill the Vicious Gray Turkey (349121)
            Bounties.Add(new BountyData
            {
                QuestId = 349121,
                Act = Act.A3,
                WorldId = 136415,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349121,136415, 220485, 681588933),
                        new ClearLevelAreaCoroutine (349121)
                    }
            });

            // A3 - Clear the Forward Barracks (349198)
            Bounties.Add(new BountyData
            {
                QuestId = 349198,
                Act = Act.A3,
                WorldId = 185247,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349198, 95804, 185247, 211059665, 185156),
                        new ClearLevelAreaCoroutine (349198)
                    }
            });

            // A3 - Clear the Fortified Bunker (349202)
            Bounties.Add(new BountyData
            {
                QuestId = 349202,
                Act = Act.A3,
                WorldId = 221748,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349202, 95804, 174516, -1049649954, 176001),
                        new EnterLevelAreaCoroutine(349202, 174516, 0, -1761785482, 175482),
                        new ClearLevelAreaCoroutine (349202)
                    }
            });

            // A3 - Clear the Battlefield Stores Level 2 (349204)
            Bounties.Add(new BountyData
            {
                QuestId = 349204,
                Act = Act.A3,
                WorldId = 221750,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349204, 95804, 174560, -1049649952, 176001),
                        new EnterLevelAreaCoroutine (349204, 174560, 221750, -1626182728, 175482, true),
                        new ClearLevelAreaCoroutine (349204)
                    }
            });

            // A3 - Clear the Foundry (349206)
            Bounties.Add(new BountyData
            {
                QuestId = 349206,
                Act = Act.A3,
                WorldId = 221751,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349206, 95804, 174665, -1049649951, 176001),
                        new EnterLevelAreaCoroutine (349206, 174665, 221751, -1558381351, 175482, true),
                        new ClearLevelAreaCoroutine (349206)
                    }
            });

            // A3 - Clear the Underbridge (349208)
            Bounties.Add(new BountyData
            {
                QuestId = 349208,
                Act = Act.A3,
                WorldId = 197622,
                QuestType = BountyQuestType.ClearZone,
                WaypointLevelAreaId = 112565,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349208, 95804, 197622, 211059666, 176001),
                        new ClearLevelAreaCoroutine (349208)
                    }
            });

            // A3 - Clear the Barracks (349210)
            Bounties.Add(new BountyData
            {
                QuestId = 349210,
                Act = Act.A3,
                WorldId = 221749,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349210, 95804, 174555, -1049649953, 176001),
                        new EnterLevelAreaCoroutine (349210, 174555, 221749, -1693984105, 175482, true),
                        new ClearLevelAreaCoroutine (349210)
                    }
            });

            // A3 - Kill Dreadgrasp (349212)
            Bounties.Add(new BountyData
            {
                QuestId = 349212,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 112548 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349212,95804, 220509, 1965678403),
                        new ClearLevelAreaCoroutine (349212)
                    }
            });

            // A3 - Kill Ghallem the Cruel (349214)
            Bounties.Add(new BountyData
            {
                QuestId = 349214,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349214,95804, 220705, -893508246),
                        new ClearLevelAreaCoroutine (349214)
                    }
            });

            // A3 - Kill Direclaw the Demonflyer (349216)
            Bounties.Add(new BountyData
            {
                QuestId = 349216,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349216,95804, 220775, -1037000755),
                        new ClearLevelAreaCoroutine (349216)
                    }
            });

            // A3 - Kill Shandra'Har (349218)
            Bounties.Add(new BountyData
            {
                QuestId = 349218,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 112565,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349218,95804, 220727, 511218737),
                        new ClearLevelAreaCoroutine (349218)
                    }
            });

            // A3 - Kill Brutu (349222)
            Bounties.Add(new BountyData
            {
                QuestId = 349222,
                Act = Act.A3,
                WorldId = 139272,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349222, 119641, 139272, 43541819, 176001),
                        new KillUniqueMonsterCoroutine (349222,139272, 220868, -1546303117),
                        new ClearLevelAreaCoroutine (349222)
                    }
            });

            // A3 - Kill Scorpitox (349226)
            Bounties.Add(new BountyData
            {
                QuestId = 349226,
                Act = Act.A3,
                WorldId = 119290,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349226,119290, 220889, 497370622),
                        new ClearLevelAreaCoroutine (349226)
                    }
            });

            // A3 - Kill Gorog the Bruiser (349228)
            Bounties.Add(new BountyData
            {
                QuestId = 349228,
                Act = Act.A3,
                WorldId = 119290,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349228,119290, 220881, 1912157531),
                        new ClearLevelAreaCoroutine (349228)
                    }
            });

            // A3 - Kill Sawtooth (349230)
            Bounties.Add(new BountyData
            {
                QuestId = 349230,
                Act = Act.A3,
                WorldId = 81934,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(349230, 81934, -234877570),
                        new KillUniqueMonsterCoroutine (349230,81934, 220851, -234877570),
                        new ClearLevelAreaCoroutine (349230)
                    }
            });

            // A3 - Kill Gormungandr (349232)
            Bounties.Add(new BountyData
            {
                QuestId = 349232,
                Act = Act.A3,
                WorldId = 81049,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349232,81049, 220777, -234877571),
                        new ClearLevelAreaCoroutine (349232)
                    }
            });

            // A3 - Kill Crabbs (349234)
            Bounties.Add(new BountyData
            {
                QuestId = 349234,
                Act = Act.A3,
                WorldId = 80763,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349234, 79401, 80763, 2083727833, 176001),
                        new KillUniqueMonsterCoroutine (349234,80763, 220817, -1740074818),
                        new ClearLevelAreaCoroutine (349234)
                    }
            });

            // A3 - Kill Riplash (349236)
            Bounties.Add(new BountyData
            {
                QuestId = 349236,
                Act = Act.A3,
                WorldId = 80763,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (349236, 79401, 80763, 2083727833, 176001),
                        new KillUniqueMonsterCoroutine (349236,80763, 220814, -1791040147),
                        new ClearLevelAreaCoroutine (349236)
                    }
            });

            // A3 - Kill Gholash (349238)
            Bounties.Add(new BountyData
            {
                QuestId = 349238,
                Act = Act.A3,
                WorldId = 79401,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349238,79401, 220810, 2096850457),
                        new ClearLevelAreaCoroutine (349238)
                    }
            });

            // A3 - Kill Haxxor (349240)
            Bounties.Add(new BountyData
            {
                QuestId = 349240,
                Act = Act.A3,
                WorldId = 79401,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349240,79401, 220806, -1180611615),
                        new ClearLevelAreaCoroutine (349240)
                    }
            });

            // A4 - Kill Oah' Tash (349252)
            Bounties.Add(new BountyData
            {
                QuestId = 349252,
                Act = Act.A4,
                WorldId = 409511,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349252,409511, 219925, -491275859),
                        new ClearLevelAreaCoroutine (349252)
                    }
            });

            // A4 - Kill Kao' Ahn (349254)
            Bounties.Add(new BountyData
            {
                QuestId = 349254,
                Act = Act.A4,
                WorldId = 409511,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349254,409511, 219936, -491275858),
                        new ClearLevelAreaCoroutine (349254)
                    }
            });

            // A4 - Kill Torchlighter (349256)
            Bounties.Add(new BountyData
            {
                QuestId = 349256,
                Act = Act.A4,
                WorldId = 109513,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349256,109513, 219727, -1880824093),
                        new ClearLevelAreaCoroutine (349256)
                    }
            });

            // A4 - Kill Khatun (349258)
            Bounties.Add(new BountyData
            {
                QuestId = 349258,
                Act = Act.A4,
                WorldId = 109513,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349258,109513, 219651, 2088134309),
                        new ClearLevelAreaCoroutine (349258)
                    }
            });

            // A4 - Kill Veshan the Fierce (349260)
            Bounties.Add(new BountyData
            {
                QuestId = 349260,
                Act = Act.A4,
                WorldId = 109513,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349260,109513, 219668, -1531192840),
                        new ClearLevelAreaCoroutine (349260)
                    }
            });

            // A4 - Kill Haures (349270)
            Bounties.Add(new BountyData
            {
                QuestId = 349270,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349270,129305, 219960, 483254089),
                        new ClearLevelAreaCoroutine (349270)
                    }
            });

            // A4 - Kill Grimnight the Soulless (349272)
            Bounties.Add(new BountyData
            {
                QuestId = 349272,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349272,129305, 219949, 483254088),
                        new ClearLevelAreaCoroutine (349272)
                    }
            });

            // A4 - Kill Sao'Thall (349274)
            Bounties.Add(new BountyData
            {
                QuestId = 349274,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349274,129305, 218873, 86638345),
                        new ClearLevelAreaCoroutine (349274)
                    }
            });

            // A4 - Kill Kysindra the Wretched (349276)
            Bounties.Add(new BountyData
            {
                QuestId = 349276,
                Act = Act.A4,
                WorldId = 121579,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349276,121579, 219847, 416696261),
                        new ClearLevelAreaCoroutine (349276)
                    }
            });

            // A4 - Kill Pyres the Damned (349278)
            Bounties.Add(new BountyData
            {
                QuestId = 349278,
                Act = Act.A4,
                WorldId = 121579,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349278,121579, 219893, -1855410867),
                        new ClearLevelAreaCoroutine (349278)
                    }
            });

            // A4 - Kill Slarg the Behemoth (349280)
            Bounties.Add(new BountyData
            {
                QuestId = 349280,
                Act = Act.A4,
                WorldId = 121579,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349280,121579, 219916, 166937943),
                        new ClearLevelAreaCoroutine (349280)
                    }
            });

            // A4 - Kill Rhau'Kye (349282)
            Bounties.Add(new BountyData
            {
                QuestId = 349282,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 109540 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349282,129305, 219985, -243218457),
                        new ClearLevelAreaCoroutine (349282)
                    }
            });

            // A3 - Kill Snitchley (349298)
            Bounties.Add(new BountyData
            {
                QuestId = 349298,
                Act = Act.A3,
                WorldId = 81934,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349298,81934, 343046, 1183731102),
                        new ClearLevelAreaCoroutine (349298)
                    }
            });

            // A3 - Kill Bholen (349301)
            Bounties.Add(new BountyData
            {
                QuestId = 349301,
                Act = Act.A3,
                WorldId = 75434,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (349301,75434, 343033, -35192228),
                        new ClearLevelAreaCoroutine (349301)
                    }
            });

            // A2 - Clear the Ruins (350556)
            Bounties.Add(new BountyData
            {
                QuestId = 350556,
                Act = Act.A2,
                WorldId = 222575,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(350556, 70885, 151028377),
                        new EnterLevelAreaCoroutine(350556, 70885, 50594, 151028377, 185067),
                        new EnterLevelAreaCoroutine (350556, 50594, 222575, 151028378, 185067, true),
                        new ClearLevelAreaCoroutine (350556)
                    }
            });

            // A2 - Kill Mage Lord Misgen (354788)
            Bounties.Add(new BountyData
            {
                QuestId = 354788,
                Act = Act.A2,
                WorldId = 123183,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (354788, 70885, 123183, -1758560943, 185067),
                        new KillUniqueMonsterCoroutine (354788,123183, 230757, -1250582635),
                        new ClearLevelAreaCoroutine (354788)
                    }
            });

            // A2 - Kill Inquisitor Hamath (355278)
            Bounties.Add(new BountyData
            {
                QuestId = 355278,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (355278,70885, 332433, -761143890),
                        new ClearLevelAreaCoroutine (355278)
                    }
            });

            // A5 - Kill Vilepaw (355282)
            Bounties.Add(new BountyData
            {
                QuestId = 355282,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (355282,267412, 351179, -3661302),
                        new ClearLevelAreaCoroutine (355282)
                    }
            });

            // A3 - Kill Lummock the Brute (356417)
            Bounties.Add(new BountyData
            {
                QuestId = 356417,
                Act = Act.A3,
                WorldId = 95804,
                WaypointLevelAreaId = 112565,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (356417,95804, 220377, -169094470),
                        new ClearLevelAreaCoroutine (356417)
                    }
            });

            // A3 - Kill Growlfang (356422)
            Bounties.Add(new BountyData
            {
                QuestId = 356422,
                Act = Act.A3,
                WorldId = 81049,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (356422,81049, 220381, 830423049),
                        new ClearLevelAreaCoroutine (356422)
                    }
            });

            // A4 - Kill the Aspect of Anguish (357127)
            Bounties.Add(new BountyData
            {
                QuestId = 357127,
                Act = Act.A4,
                WorldId = 409511,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (357127,409511, 197356, -1889188727),
                        new ClearLevelAreaCoroutine (357127)
                    }
            });

            // A4 - Kill the Aspect of Pain (357129)
            Bounties.Add(new BountyData
            {
                QuestId = 357129,
                Act = Act.A4,
                WorldId = 409511,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (357129,409511, 197491, -189435038),
                        new ClearLevelAreaCoroutine (357129)
                    }
            });

            // A4 - Kill the Aspect of Destruction (357131)
            Bounties.Add(new BountyData
            {
                QuestId = 357131,
                Act = Act.A4,
                WorldId = 109513,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (357131,109513, 197495, 1086210222),
                        new ClearLevelAreaCoroutine (357131)
                    }
            });

            // A4 - Kill the Aspect of Hatred (357133)
            Bounties.Add(new BountyData
            {
                QuestId = 357133,
                Act = Act.A4,
                WorldId = 121579,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (357133,121579, 197492, -449006222),
                        new ClearLevelAreaCoroutine (357133)
                    }
            });

            // A4 - Kill the Aspect of Lies (357135)
            Bounties.Add(new BountyData
            {
                QuestId = 357135,
                Act = Act.A4,
                WorldId = 121579,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (357135,121579, 197493, -189570201),
                        new ClearLevelAreaCoroutine (357135)
                    }
            });

            // A4 - Kill the Aspect of Sin (357137)
            Bounties.Add(new BountyData
            {
                QuestId = 357137,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (357137,129305, 197496, -1046941116),
                        new ClearLevelAreaCoroutine (357137)
                    }
            });

            // A4 - Kill the Aspect of Terror (357139)
            Bounties.Add(new BountyData
            {
                QuestId = 357139,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (357139,129305, 197489, 25290648),
                        new ClearLevelAreaCoroutine (357139)
                    }
            });

            // A5 - Kill Erdith (359233)
            Bounties.Add(new BountyData
            {
                QuestId = 359233,
                Act = Act.A5,
                WorldId = 338944,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (359233,338944, 351183, 869390077),
                        new ClearLevelAreaCoroutine (359233)
                    }
            });

            // A1 - Kill Ragus Grimlow (361320)
            Bounties.Add(new BountyData
            {
                QuestId = 361320,
                Act = Act.A1,
                WorldId = 50579,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 19780 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361320,50579, 218307, 490952466),
                        new ClearLevelAreaCoroutine (361320)
                    }
            });

            // A1 - Kill Braluk Grimlow (361327)
            Bounties.Add(new BountyData
            {
                QuestId = 361327,
                Act = Act.A1,
                WorldId = 50579,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361327,50579, 218308, 490952467),
                        new ClearLevelAreaCoroutine (361327)
                    }
            });

            // A1 - Kill Glidewing (361331)
            Bounties.Add(new BountyData
            {
                QuestId = 361331,
                Act = Act.A1,
                WorldId = 50579,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361331,50579, 218314, 0),
                        new ClearLevelAreaCoroutine (361331)
                    }
            });

            // A1 - Kill Merrium Skullthorn (361334)
            Bounties.Add(new BountyData
            {
                QuestId = 361334,
                Act = Act.A1,
                WorldId = 50582,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361334,50582, 218364, -222239427),
                        new ClearLevelAreaCoroutine (361334)
                    }
            });

            // A1 - Kill Cudgelarm (361336)
            Bounties.Add(new BountyData
            {
                QuestId = 361336,
                Act = Act.A1,
                WorldId = 50582,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361336,50582, 218356, 762156596),
                        new ClearLevelAreaCoroutine (361336)
                    }
            });

            // A1 - Kill Firestarter (361339)
            Bounties.Add(new BountyData
            {
                QuestId = 361339,
                Act = Act.A1,
                WorldId = 50582,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361339,50582, 218362, -1923753192),
                        new ClearLevelAreaCoroutine (361339)
                    }
            });

            // A1 - Kill Captain Cage (361341)
            Bounties.Add(new BountyData
            {
                QuestId = 361341,
                Act = Act.A1,
                WorldId = 50584,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361341,50584, 218396, -152093421),
                        new ClearLevelAreaCoroutine (361341)
                    }
            });

            // A1 - Kill Killian Damort (361343)
            Bounties.Add(new BountyData
            {
                QuestId = 361343,
                Act = Act.A1,
                WorldId = 50584,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361343,50584, 218400, -1009862608),
                        new ClearLevelAreaCoroutine (361343)
                    }
            });

            // A1 - Kill Bellybloat the Scarred (361345)
            Bounties.Add(new BountyData
            {
                QuestId = 361345,
                Act = Act.A1,
                WorldId = 50584,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361345,50584, 218405, -1566569389),
                        new ClearLevelAreaCoroutine (361345)
                    }
            });

            // A1 - Kill Rad'noj (361352)
            Bounties.Add(new BountyData
            {
                QuestId = 361352,
                Act = Act.A1,
                WorldId = 50585,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361352,50585, 361347, 1600048076),
                        new ClearLevelAreaCoroutine (361352)
                    }
            });

            // A1 - Kill Captain Clegg (361354)
            Bounties.Add(new BountyData
            {
                QuestId = 361354,
                Act = Act.A1,
                WorldId = 50585,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (361354,50585, 361349, -1200730604),
                        new ClearLevelAreaCoroutine (361354)
                    }
            });

            // A4 - Clear the Hell Rift (362140)
            Bounties.Add(new BountyData
            {
                QuestId = 362140,
                Act = Act.A4,
                WorldId = 109530,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (362140, 109525, 109530, 984446737, 224890),
                        new ClearLevelAreaCoroutine (362140)
                    }
            });

            // A5 - Kill Morghum the Beast (362913)
            Bounties.Add(new BountyData
            {
                QuestId = 362913,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 245964 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (362913,267412, 361771, -667462790),
                        new ClearLevelAreaCoroutine (362913)
                    }
            });

            // A5 - Kill Fangbite (362915)
            Bounties.Add(new BountyData
            {
                QuestId = 362915,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.KillMonster,
                //LevelAreaIds = new HashSet<int> { 245964 },
                WaypointLevelAreaId = 258142,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (362915,267412, 361755, 474994762),
                        //new MoveToPositionCoroutine(267412, new Vector3(474, 946, 0)),
                        new ClearLevelAreaCoroutine (362915)
                    }
            });

            // A5 - Kill Slinger (362921)
            Bounties.Add(new BountyData
            {
                QuestId = 362921,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (362921,267412, 361973, -504666936),
                        new ClearLevelAreaCoroutine (362921)
                    }
            });

            // A5 - Kill Almash the Grizzly (362923)
            Bounties.Add(new BountyData
            {
                QuestId = 362923,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (362923,267412, 361974, -504666935),
                        new ClearLevelAreaCoroutine (362923)
                    }
            });

            // A5 - Kill Tadardya (362925)
            Bounties.Add(new BountyData
            {
                QuestId = 362925,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (362925,267412, 361991, 474994763),
                        new ClearLevelAreaCoroutine (362925)
                    }
            });

            // A5 - Kill Vek Tabok (362996)
            Bounties.Add(new BountyData
            {
                QuestId = 362996,
                Act = Act.A5,
                WorldId = 283552,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(362996, 283552, -1650018965),
                        new KillUniqueMonsterCoroutine (362996, 283552, 362299, -1650018965),
                        new ClearLevelAreaCoroutine (362996)
                    }
            });

            // A5 - Kill Vek Marru (363000)
            Bounties.Add(new BountyData
            {
                QuestId = 363000,
                Act = Act.A5,
                WorldId = 283552,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(363000, 283552, -1650018964),
                        new KillUniqueMonsterCoroutine (363000, 283552, 362303, -1650018964),
                        new ClearLevelAreaCoroutine (363000)
                    }
            });

            // A5 - Kill Nak Sarugg (363013)
            Bounties.Add(new BountyData
            {
                QuestId = 363013,
                Act = Act.A5,
                WorldId = 283566,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363013,283566, 362305, -1826336524),
                        new ClearLevelAreaCoroutine (363013)
                    }
            });

            // A5 - Kill Nak Qujin (363016)
            Bounties.Add(new BountyData
            {
                QuestId = 363016,
                Act = Act.A5,
                WorldId = 283566,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363016,283566, 362309, -1826336523),
                        new ClearLevelAreaCoroutine (363016)
                    }
            });

            // A5 - Kill Bari Hattar (363019)
            Bounties.Add(new BountyData
            {
                QuestId = 363019,
                Act = Act.A5,
                WorldId = 283566,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363019,283566, 362307, 100982043),
                        new ClearLevelAreaCoroutine (363019)
                    }
            });

            // A5 - Kill Bari Moqqu (363021)
            Bounties.Add(new BountyData
            {
                QuestId = 363021,
                Act = Act.A5,
                WorldId = 283566,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363021,283566, 362310, 100982044),
                        new ClearLevelAreaCoroutine (363021)
                    }
            });

            // A5 - Kill Getzlord (363075)
            Bounties.Add(new BountyData
            {
                QuestId = 363075,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363075,261712, 311343, 767269294),
                        new ClearLevelAreaCoroutine (363075)
                    }
            });

            // A5 - Kill Yergacheph (363078)
            Bounties.Add(new BountyData
            {
                QuestId = 363078,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363078,261712, 309462, -1454331435),
                        new ClearLevelAreaCoroutine (363078)
                    }
            });

            // A5 - Kill Katherine Batts (363080)
            Bounties.Add(new BountyData
            {
                QuestId = 363080,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363080,261712, 360826, 1607064708),
                        new ClearLevelAreaCoroutine (363080)
                    }
            });

            // A5 - Kill Matanzas the Loathsome (363082)
            Bounties.Add(new BountyData
            {
                QuestId = 363082,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363082,261712, 360849, -1454331434),
                        new ClearLevelAreaCoroutine (363082)
                    }
            });

            // A5 - Kill Hedros (363086)
            Bounties.Add(new BountyData
            {
                QuestId = 363086,
                Act = Act.A5,
                WorldId = 338944,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363086,338944, 361291, 885165158),
                        new ClearLevelAreaCoroutine (363086)
                    }
            });

            // A5 - Kill Purah (363090)
            Bounties.Add(new BountyData
            {
                QuestId = 363090,
                Act = Act.A5,
                WorldId = 338944,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363090,338944, 361313, 885165159),
                        new ClearLevelAreaCoroutine (363090)
                    }
            });

            // A5 - Kill Targerious (363092)
            Bounties.Add(new BountyData
            {
                QuestId = 363092,
                Act = Act.A5,
                WorldId = 338944,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363092,338944, 361417, -1662036898),
                        new ClearLevelAreaCoroutine (363092)
                    }
            });

            // A5 - Kill Micheboar (363177)
            Bounties.Add(new BountyData
            {
                QuestId = 363177,
                Act = Act.A5,
                WorldId = 263494,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363177,263494, 360864, -1039640431),
                        new ClearLevelAreaCoroutine (363177)
                    }
            });

            // A5 - Kill Theodosia Buhre (363180)
            Bounties.Add(new BountyData
            {
                QuestId = 363180,
                Act = Act.A5,
                WorldId = 263494,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363180,263494, 360869, 1607064710),
                        new ClearLevelAreaCoroutine (363180)
                    }
            });

            // A5 - Kill Sumaryss the Damned (363185)
            Bounties.Add(new BountyData
            {
                QuestId = 363185,
                Act = Act.A5,
                WorldId = 263494,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363185,263494, 360881, -1662036899),
                        new ClearLevelAreaCoroutine (363185)
                    }
            });

            // A5 - Kill Rockulus (363194)
            Bounties.Add(new BountyData
            {
                QuestId = 363194,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363194,338600, 362891, -710718755),
                        new ClearLevelAreaCoroutine (363194)
                    }
            });

            // A5 - Kill Obsidious (363196)
            Bounties.Add(new BountyData
            {
                QuestId = 363196,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363196,338600, 362895, -710718754),
                        new ClearLevelAreaCoroutine (363196)
                    }
            });

            // A5 - Kill Slarth the Tunneler (363198)
            Bounties.Add(new BountyData
            {
                QuestId = 363198,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363198,338600, 363051, -1819959148),
                        new ClearLevelAreaCoroutine (363198)
                    }
            });

            // A5 - Kill Burrask the Tunneler (363200)
            Bounties.Add(new BountyData
            {
                QuestId = 363200,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363200,338600, 363060, -1819959147 ),
                        new ClearLevelAreaCoroutine (363200)
                    }
            });

            // A5 - Kill Watareus (363202)
            Bounties.Add(new BountyData
            {
                QuestId = 363202,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363202,338600, 363073, -1127556653),
                        new ClearLevelAreaCoroutine (363202)
                    }
            });

            // A5 - Kill Baethus (363204)
            Bounties.Add(new BountyData
            {
                QuestId = 363204,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363204,338600, 363108, -1127556652),
                        new ClearLevelAreaCoroutine (363204)
                    }
            });

            // A5 - Kill Lograth (363552)
            Bounties.Add(new BountyData
            {
                QuestId = 363552,
                Act = Act.A5,
                WorldId = 271233,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    new MoveThroughDeathGates(363552,271233,1),
                    new KillUniqueMonsterCoroutine (363552,271233, 363378, -1443986728),
                    new ClearLevelAreaCoroutine (363552)
                }
            });

            // A5 - Kill Valtesk the Cruel (363555)
            Bounties.Add(new BountyData
            {
                QuestId = 363555,
                Act = Act.A5,
                WorldId = 271233,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveThroughDeathGates(363555,271233,1),
                        new KillUniqueMonsterCoroutine (363555,271233, 363367, -413057866),
                        new ClearLevelAreaCoroutine (363555)
                    }
            });

            // A5 - Kill Scythys (363557)
            Bounties.Add(new BountyData
            {
                QuestId = 363557,
                Act = Act.A5,
                WorldId = 271233,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveThroughDeathGates(363557,271233,1),
                        new KillUniqueMonsterCoroutine (363557,271233, 363232, 327834221),
                        new ClearLevelAreaCoroutine (363557)
                    }
            });

            // A5 - Kill Ballartrask the Defiler (363559)
            Bounties.Add(new BountyData
            {
                QuestId = 363559,
                Act = Act.A5,
                WorldId = 271235,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363559,271235, 363421, -1443986727),
                        new ClearLevelAreaCoroutine (363559)
                    }
            });

            // A5 - Kill Zorrus (363561)
            Bounties.Add(new BountyData
            {
                QuestId = 363561,
                Act = Act.A5,
                WorldId = 271235,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363561,271235, 363230, -1443868993),
                        new ClearLevelAreaCoroutine (363561)
                    }
            });

            // A5 - Kill Xaphane (363563)
            Bounties.Add(new BountyData
            {
                QuestId = 363563,
                Act = Act.A5,
                WorldId = 271235,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (363563,271235, 363361, 327834222),
                        new ClearLevelAreaCoroutine (363563)
                    }
            });

            // A1 - Kill Jezeb the Conjuror (367561)
            Bounties.Add(new BountyData
            {
                QuestId = 367561,
                Act = Act.A1,
                WorldId = 75049,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 19941,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(367561, 71150, -1019926638),
						
						new MoveToPositionCoroutine(71150, new Vector3(1711, 3856, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1517, 3837, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1587, 4023, 38)),
                        new MoveToPositionCoroutine(71150, new Vector3(1413, 4076, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1437, 3930, 50)),
                        new MoveToPositionCoroutine(71150, new Vector3(1214, 3903, 79)),
                        new MoveToPositionCoroutine(71150, new Vector3(1202, 3773, 80)),
                        new MoveToPositionCoroutine(71150, new Vector3(1076, 3880, 78)),
                        new MoveToPositionCoroutine(71150, new Vector3(957, 3950, 80)),
                        new MoveToPositionCoroutine(71150, new Vector3(892, 3860, 90)),
                        new MoveToPositionCoroutine(71150, new Vector3(1085, 3729, 78)),
                        new MoveToPositionCoroutine(71150, new Vector3(1080, 3506, 74)),
                        new MoveToPositionCoroutine(71150, new Vector3(1077, 3389, 65)),

                        new EnterLevelAreaCoroutine (367561, 71150, 75049, -1019926638, 176001),
                        new KillUniqueMonsterCoroutine (367561,71150, 218662, 718706851),
                        new ClearLevelAreaCoroutine (367561)
                    }
            });

            // A5 - Kill Mulliuqs the Horrid (367935)
            Bounties.Add(new BountyData
            {
                QuestId = 367935,
                Act = Act.A5,
                WorldId = 322531,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (367935, 338600, 322531, -1551729971, 0),
                        new KillUniqueMonsterCoroutine (367935,322531, 354378, -1933569239),
                        new ClearLevelAreaCoroutine (367935)
                    }
            });

            // A5 - Kill Dale Hawthorne (368781)
            Bounties.Add(new BountyData
            {
                QuestId = 368781,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (368781,261712, 360861, -501532859),
                        new ClearLevelAreaCoroutine (368781)
                    }
            });

            // A5 - Kill Captain Gerber (368783)
            Bounties.Add(new BountyData
            {
                QuestId = 368783,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (368783,261712, 360853, -444054584),
                        new ClearLevelAreaCoroutine (368783)
                    }
            });

            // A5 - Kill Igor Stalfos (368785)
            Bounties.Add(new BountyData
            {
                QuestId = 368785,
                Act = Act.A5,
                WorldId = 261712,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (368785,261712, 360858, 2013948912),
                        new ClearLevelAreaCoroutine (368785)
                    }
            });

            // A5 - Kill Pan Fezbane (368789)
            Bounties.Add(new BountyData
            {
                QuestId = 368789,
                Act = Act.A5,
                WorldId = 263494,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (368789,263494, 363910, -877704682),
                        new ClearLevelAreaCoroutine (368789)
                    }
            });

            // A1 - Kill Growler (369243)
            Bounties.Add(new BountyData
            {
                QuestId = 369243,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369243,71150, 365335, 2067238311),
                        new ClearLevelAreaCoroutine (369243)
                    }
            });

            // A1 - Kill Krelm the Flagitious (369246)
            Bounties.Add(new BountyData
            {
                QuestId = 369246,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369246,71150, 365330, 1683727462),
                        new ClearLevelAreaCoroutine (369246)
                    }
            });

            // A1 - Kill Lord Brone (369249)
            Bounties.Add(new BountyData
            {
                QuestId = 369249,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369249,71150, 365425, 2015206012),
                        new ClearLevelAreaCoroutine (369249)
                    }
            });

            // A1 - Kill Galush Valdant (369251)
            Bounties.Add(new BountyData
            {
                QuestId = 369251,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369251,71150, 365429, 2015206013),
                        new ClearLevelAreaCoroutine (369251)
                    }
            });

            // A1 - Kill Reggrel the Despised (369253)
            Bounties.Add(new BountyData
            {
                QuestId = 369253,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 19941, 1199, 19943 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369253,71150, 366975, -1504570442),
                        new ClearLevelAreaCoroutine (369253)
                    }
            });

            // A1 - Kill Hrugowl the Defiant (369257)
            Bounties.Add(new BountyData
            {
                QuestId = 369257,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 19941, 1199, 19943 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369257,71150, 366981, -1504570441),
                        new ClearLevelAreaCoroutine (369257)
                    }
            });

            // A1 - Kill Percepeus (369273)
            Bounties.Add(new BountyData
            {
                QuestId = 369273,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                LevelAreaIds = new HashSet<int> { 19941, 1199, 19943 },
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369273,71150, 366998, -11732713),
                        new ClearLevelAreaCoroutine (369273 )
                    }
            });

            // A2 - Kill Balzhak (369277)
            Bounties.Add(new BountyData
            {
                QuestId = 369277,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369277,70885, 367006, -349018055),
                        new ClearLevelAreaCoroutine (369277)
                    }
            });

            // A2 - Kill Samaras the Chaser (369288)
            Bounties.Add(new BountyData
            {
                QuestId = 369288,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369288,70885, 367011, 337921175),
                        new ClearLevelAreaCoroutine (369288)
                    }
            });

            // A2 - Kill Barty the Minuscule (369291)
            Bounties.Add(new BountyData
            {
                QuestId = 369291,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369291,70885, 367018, -1367605959),
                        new ClearLevelAreaCoroutine (369291)
                    }
            });

            // A2 - Kill Gryssian (369298)
            Bounties.Add(new BountyData
            {
                QuestId = 369298,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369298,70885, 367073, -764145050),
                        new ClearLevelAreaCoroutine (369298)
                    }
            });

            // A2 - Kill Khahul the Serpent (369300)
            Bounties.Add(new BountyData
            {
                QuestId = 369300,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369300,70885, 367095, -764145049),
                        new ClearLevelAreaCoroutine (369300)
                    }
            });

            // A2 - Kill Tridiun the Impaler (369302)
            Bounties.Add(new BountyData
            {
                QuestId = 369302,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369302,70885, 367096, 290002476),
                        new ClearLevelAreaCoroutine (369302)
                    }
            });

            // A3 - Kill Thromp the Breaker (369312)
            Bounties.Add(new BountyData
            {
                QuestId = 369312,
                Act = Act.A3,
                WorldId = 81019,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (369312, 93099, 81019, -1078336204, 176002),
                        new KillUniqueMonsterCoroutine (369312,81019, 367333, -1902915434),
                        new ClearLevelAreaCoroutine (369312)
                    }
            });

            // A3 - Kill Obis the Mighty (369316)
            Bounties.Add(new BountyData
            {
                QuestId = 369316,
                Act = Act.A3,
                WorldId = 93099, //81019,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (369316, 93099, 81019, -1078336204, 176002),
                        new KillUniqueMonsterCoroutine (369316,81019, 367335, -1902915433),
                        new ClearLevelAreaCoroutine (369316)
                    }
            });

            // A3 - Kill Ganthar the Trickster (369319)
            Bounties.Add(new BountyData
            {
                QuestId = 369319,
                Act = Act.A3,
                WorldId = 81019,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (369319, 93099, 81019, -1078336204, 176002),
                        new KillUniqueMonsterCoroutine (369319,81019, 367341, 2128210786),
                        new ClearLevelAreaCoroutine (369319)
                    }
            });

            // A3 - Kill Greelode the Unforgiving (369323)
            Bounties.Add(new BountyData
            {
                QuestId = 369323,
                Act = Act.A3,
                WorldId = 93099,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369323,93099, 367360, -325287909),
                        new ClearLevelAreaCoroutine (369323)
                    }
            });

            // A3 - Kill Barrucus (369326)
            Bounties.Add(new BountyData
            {
                QuestId = 369326,
                Act = Act.A3,
                WorldId = 93099,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369326,93099, 367371, -1227098806),
                        new ClearLevelAreaCoroutine (369326)
                    }
            });

            // A3 - Kill Allucayrd (369329)
            Bounties.Add(new BountyData
            {
                QuestId = 369329,
                Act = Act.A3,
                WorldId = 93099,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369329,93099, 367366, -686066387),
                        new ClearLevelAreaCoroutine (369329)
                    }
            });

            // A1 - Kill Kankerrot (369399)
            Bounties.Add(new BountyData
            {
                QuestId = 369399,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369399,71150, 365906, 942080182),
                        new ClearLevelAreaCoroutine (369399)
                    }
            });

            // A1 - Kill Horrus the Nightstalker (369404)
            Bounties.Add(new BountyData
            {
                QuestId = 369404,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (369404,71150, 365450, 490952468),
                        new ClearLevelAreaCoroutine (369404)
                    }
            });

            // A4 - Kill Sabnock The Infector (409753)
            Bounties.Add(new BountyData
            {
                QuestId = 409753,
                Act = Act.A4,
                WorldId = 409000,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409753,409000, 409612, 1712473545),
                        new ClearLevelAreaCoroutine (409753)
                    }
            });

            // A4 - Kill Malefactor Vephar (409755)
            Bounties.Add(new BountyData
            {
                QuestId = 409755,
                Act = Act.A4,
                WorldId = 409000,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409755,409000, 409631, 1151200136),
                        new ClearLevelAreaCoroutine (409755)
                    }
            });

            // A4 - Kill Amduscias (409757)
            Bounties.Add(new BountyData
            {
                QuestId = 409757,
                Act = Act.A4,
                WorldId = 409000,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409757,409000, 409615, 1666317960),
                        new ClearLevelAreaCoroutine (409757)
                    }
            });

            // A4 - Kill Cimeries (409759)
            Bounties.Add(new BountyData
            {
                QuestId = 409759,
                Act = Act.A4,
                WorldId = 409000,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409759,409000, 409614, 1607519118),
                        new ClearLevelAreaCoroutine (409759)
                    }
            });

            // A4 - Kill Erra (409761)
            Bounties.Add(new BountyData
            {
                QuestId = 409761,
                Act = Act.A4,
                WorldId = 409510,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409761,409510, 409610, 1155578409),
                        new ClearLevelAreaCoroutine (409761)
                    }
            });

            // A4 - Kill Palerider Beleth (409763)
            Bounties.Add(new BountyData
            {
                QuestId = 409763,
                Act = Act.A4,
                WorldId = 409510,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409763,409510, 409628, -595378876),
                        new ClearLevelAreaCoroutine (409763)
                    }
            });

            // A4 - Kill Emberdread (409765)
            Bounties.Add(new BountyData
            {
                QuestId = 409765,
                Act = Act.A4,
                WorldId = 409510,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409765,409510, 409543, 1049069571),
                        new ClearLevelAreaCoroutine (409765)
                    }
            });

            // A4 - Kill Lasciate (409767)
            Bounties.Add(new BountyData
            {
                QuestId = 409767,
                Act = Act.A4,
                WorldId = 409510,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (409767,409510, 409639, -1381007476),
                        new ClearLevelAreaCoroutine (409767)
                    }
            });

            // A4 - Clear the Besieged Tower Level 2 (409893)
            Bounties.Add(new BountyData
            {
                QuestId = 409893,
                Act = Act.A4,
                WorldId = 409374,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
//						new MoveToMapMarkerCoroutine(409893, 409000, -97144983),
                        new EnterLevelAreaCoroutine (409893, 409000, 0, -97144983, 210758),
                        new ClearLevelAreaCoroutine (409893)
                    }
            });

            // A5 - Clear the Guild Hideout (432699)
            Bounties.Add(new BountyData
            {
                QuestId = 432699,
                Act = Act.A5,
                WorldId = 432698,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (432699, 263494, 432697, 48502155, 376027),
                        new EnterLevelAreaCoroutine (432699, 432697, 432698, 48502156, 176008, true),
                        new ClearLevelAreaCoroutine (432699)
                    }
            });

            // A5 - Kill Brent Brewington (432701)
            Bounties.Add(new BountyData
            {
                QuestId = 432701,
                Act = Act.A5,
                WorldId = 432697,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (432701, 263494, 432697, 48502155, 376027),
                        new KillUniqueMonsterCoroutine (432701,432697, 430880, 1803003715),
                        new ClearLevelAreaCoroutine (432701)
                    }
            });

            // A5 - Kill Meriel Regodon (432705)
            Bounties.Add(new BountyData
            {
                QuestId = 432705,
                Act = Act.A5,
                WorldId = 432697,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (432705, 263494, 432697, 48502155, 376027),
                        new KillUniqueMonsterCoroutine (432705,432697, 430881, -1084271549),
                        new ClearLevelAreaCoroutine (432705)
                    }
            });

            // A5 - Kill Denis Genest (432707)
            Bounties.Add(new BountyData
            {
                QuestId = 432707,
                Act = Act.A5,
                WorldId = 432697,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (432707, 263494, 432697, 48502155, 376027),
                        new KillUniqueMonsterCoroutine (432707,432697, 430879, 323420480),
                        new ClearLevelAreaCoroutine (432707)
                    }
            });

            // A2 - Clear the Western Channel (433003)
            Bounties.Add(new BountyData
            {
                QuestId = 433003,
                Act = Act.A2,
                WorldId = 432998,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (433003, 59486, 432993, 705396550, 175467),
                        new EnterLevelAreaCoroutine (433003, 432993, 432998, 705396551, 176007, true),
                        new ClearLevelAreaCoroutine (433003)
                    }
            });

            // A2 - Clear the Eastern Channel (433005)
            Bounties.Add(new BountyData
            {
                QuestId = 433005,
                Act = Act.A2,
                WorldId = 433001,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (433005, 59486, 432997, 1037011047, 176007),
                        new EnterLevelAreaCoroutine (433005, 432997, 433001, 1037011048, 176007, true),
                        new ClearLevelAreaCoroutine (433005)
                    }
            });

            // A2 - Kill Yakara (433007)
            Bounties.Add(new BountyData
            {
                QuestId = 433007,
                Act = Act.A2,
                WorldId = 432997,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (433007, 59486, 432997, 1037011047, 176007),
                        new KillUniqueMonsterCoroutine (433007,432997, 222238, -1947442964),
                        new ClearLevelAreaCoroutine (433007)
                    }
            });

            // A2 - Kill Grool (433009)
            Bounties.Add(new BountyData
            {
                QuestId = 433009,
                Act = Act.A2,
                WorldId = 432997,
                //WaypointNumber = 26,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
//                      new EnterLevelAreaCoroutine (433009, 59486, 432997, 1037011047, 176007),
						new EnterLevelAreaCoroutine (433009, 59486, 0, 1037011047, 176007),
                        new KillUniqueMonsterCoroutine (433009,432997, 222236, -320518570),
                        new ClearLevelAreaCoroutine (433009)
                    }
            });

            // A2 - Kill Otzi the Cursed (433011)
            Bounties.Add(new BountyData
            {
                QuestId = 433011,
                Act = Act.A2,
                WorldId = 432993,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        //                        new EnterLevelAreaCoroutine (433003, 59486, 432993, 705396550, 175467),
                        //new EnterLevelAreaCoroutine (433003, 432993, 432998, 705396551, 176007),

						new EnterLevelAreaCoroutine (433011, 59486, 432993, 705396550, 175467),
                        new KillUniqueMonsterCoroutine (433011,432993, 222186, 1840003642),
                        new ClearLevelAreaCoroutine (433011)
                    }
            });

            // A1 - Bounty: Kill Johanys (449835)
            Bounties.Add(new BountyData
            {
                QuestId = 449835,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                   new EnterLevelAreaCoroutine(449835, 71150, 0, -1019926638, 176001),
                   new MoveToMapMarkerCoroutine(449835, 443346, -829990205),
                   new ClearLevelAreaCoroutine(449835),
                }
            });

            // A5 - Bounty: Binding Evil (445044)
            Bounties.Add(new BountyData
            {
                QuestId = 445044,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(445044, 408254, 2912417),
                    //ActorId: 433408, Type: Monster, Name: P4_Forest_Mysterious_Man_01-1071, Distance2d: 23.84807, 
                    // P4_Forest_Mysterious_Hermit_Friendly (437085) Distance: 2.445482
                    new InteractWithUnitCoroutine(445044, 408254, 437085, 2912417, 5),

                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(60.12238f, 71.47668f, 0.09999999f)),
                    new WaitCoroutine(445044, 408254,10000),
                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(78.24176f, 65.46866f, 0.1f)),
                    new WaitCoroutine(445044, 408254,10000),
                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(71.70166f, 78.02289f, 0.1f)),
                    new WaitCoroutine(445044, 408254,10000),
                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(55.58252f, 79.62216f, 0.1f)),
                    new WaitCoroutine(445044, 408254,10000),
                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(61.52814f, 64.55441f, 0.1f)),
                    new WaitCoroutine(445044, 408254,10000),
                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(80.84772f, 58.59448f, 0.09999999f)),
                    new WaitCoroutine(445044, 408254,10000),
                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(77.08563f, 77.41998f, 0.09999999f)),
                    new WaitCoroutine(445044, 408254,10000),
                    new MoveToScenePositionCoroutine(445044, 408254, "p4_Forest_Coast_NSEW_02", new Vector3(52.39435f, 82.44772f, 0.1f)),
                    new WaitCoroutine(445044, 408254,10000),
					
//                    new ClearAreaForNSecondsCoroutine(445044, 60, 0,  2912417, 15, false),
                }
            });

            // A4 - Bounty: The Black King's Legacy (448260) 
            // New Bounty Portal to Leorics Chest with Elites Routine - wasnt working with dynamic setup
            Bounties.Add(new BountyData
            {
                QuestId = 448260,
                Act = Act.A4,
                WorldId = 129305,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 49,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(448260, 129305, 0, 37697317, 444404),
                    new MoveToActorCoroutine(448260, 129305, 444404),
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 24.08629
                    new InteractWithGizmoCoroutine(448260, 129305, 444404, 37697317, 5),
                                        new InteractWithGizmoCoroutine(448260, 443756, 365097, 2912417, 5),
                                        new ClearAreaForNSecondsCoroutine(448260, 60, 0, 0, 45),
                                        new ClearLevelAreaCoroutine(448260),
                                        new MoveToPositionCoroutine(443756, new Vector3(159, 182, 0)),
                }
            });


            // A3 - Bounty: Last of the Barbarians (436280)
            Bounties.Add(new BountyData
            {
                QuestId = 436280,
                Act = Act.A3,
                WorldId = 428493,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41, // Ruins of Sescheron
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436280, 428493, 2912417),

                    //new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_W_Entrance_02_LoDGate_E02_S01", new Vector3(117.0242f, 19.4599f, 0.6535801f)),

                    new MoveToSceneCoroutine(436280, 428493, "p4_ruins_frost_NS_01"),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(183.717f, 168.832f, -0.01446433f)),
                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 13.54914
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    new WaitCoroutine(436280, 428493, 5000),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(122.0508f, 144.9282f, 0.1000014f)),
                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 62.45116
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    new WaitCoroutine(436280, 428493, 5000),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(183.717f, 168.832f, -0.01446433f)),
                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 13.54914
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    new WaitCoroutine(436280, 428493, 5000),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(122.0508f, 144.9282f, 0.1000014f)),
                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 62.45116
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    new WaitCoroutine(436280, 428493, 5000),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(161.8187f, 61.48199f, -9.913034f)),

                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 62.45116
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    new WaitCoroutine(436280, 428493, 5000),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(49.39655f, 63.32892f, 0.1000005f)),

                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 92.18051
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    new WaitCoroutine(436280, 428493, 5000),

                    new ClearAreaForNSecondsCoroutine(448619, 90, 0, 0, 45),

                    // px_Ruins_Frost_Camp_BarbSkular (435720) Distance: 8.818088
                    new InteractWithUnitCoroutine(436280, 428493, 435720, 0, 5),

                }
            });



            // A1 - Bounty: Kill Hannes (449837)
            Bounties.Add(new BountyData
            {
                QuestId = 449837,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(449837, 71150, 443346, -1019926638, 176001),
                    new ClearLevelAreaCoroutine(449837),
                }
            });


            // A3 - Bounty: King of the Ziggurat (436282) - 6	0

            Bounties.Add(new BountyData
            {
                QuestId = 436282,
                Act = Act.A3,
                WorldId = 428493, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                 {
                     new MoveToMapMarkerCoroutine(436282, 428493, 2912417),
                     new MoveToSceneCoroutine(436282, 428493, "Ziggurat"),

                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(436282, 428493, 5000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(63.26221f, 120.9539f, 10.09718f)),
                     new WaitCoroutine(436282, 428493, 10000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(436282, 428493, 5000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.5901f, 76.61993f, 10.77626f)),
                     new WaitCoroutine(436282, 428493, 10000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(436282, 428493, 5000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(169.4726f, 117.2496f, 10.27893f)),
                     new WaitCoroutine(436282, 428493, 10000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new WaitCoroutine(436282, 428493, 5000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(123.4897f, 170.7134f, 10.7916f)),
                     new WaitCoroutine(436282, 428493, 10000),
                     new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(119.4838f, 123.1133f, 20.4907f)),
                     new ClearAreaForNSecondsCoroutine(436282, 90, 0, 0, 30),

                     new InteractWithGizmoCoroutine(436282, 428493, 437935, 0, 5),
                 }
            });


            // A5 - Bounty: The Ghost Prison (444573)
            Bounties.Add(new BountyData
            {
                QuestId = 444573,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    //ActorId: 434356, Type: Gizmo, Name: P4_Forest_Coast_Holy_Relics-4632, Distance2d: 16.82734, CollisionRadius: 17.19083, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0                     
                    new MoveToMapMarkerCoroutine(444573, 408254, 2912417),
//                 new MoveToActorCoroutine(444573, 408254, 434356),
                    new InteractWithGizmoCoroutine(444573, 408254, 434356, -1),

                    new MoveToScenePositionCoroutine(444573, 408254, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(60.66754f, 57.75403f, 0.1f)),
                    new WaitCoroutine(444573, 408254, 10000),
                    new MoveToScenePositionCoroutine(444573, 408254, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(58.20206f, 37.20313f, 0.1f)),
                    new WaitCoroutine(444573, 408254, 10000),
                    new MoveToScenePositionCoroutine(444573, 408254, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(76.867f, 57.91034f, 0.1f)),
                    new WaitCoroutine(444573, 408254, 10000),
                    new MoveToScenePositionCoroutine(444573, 408254, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(67.42307f, 80.11035f, 0.1f)),
                    new WaitCoroutine(444573, 408254, 10000),
                    new MoveToScenePositionCoroutine(444573, 408254, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(44.89722f, 56.82794f, 0.1f)),
                    new WaitCoroutine(444573, 408254, 10000),
                    new MoveToScenePositionCoroutine(444573, 408254, "p4_Forest_Coast_NSEW_Sub120_06", new Vector3(63.0864f, 58.17493f, 0.1f)),
                    new WaitCoroutine(444573, 408254, 10000),
					
//                  new ClearAreaForNSecondsCoroutine(444573, 60, 0, 0, 45),
                }
            });



            // A5 - Bounty: Kill Fharzula (444702)
            Bounties.Add(new BountyData
            {
                QuestId = 444702,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(444702, 408254, 1493379889),
                    
                    //Actor: x1_westm_Door_Wide_Clicky-2290 (273323) Gizmo  -->
                    new InteractWithGizmoCoroutine(444702, 408254, 273323, 0, 5),

                    new EnterLevelAreaCoroutine(444702, 408254, 441412, 1493379889, 175467),

                    new EnterLevelAreaCoroutine(444702, 441412, 441322, 1493379890, 175467),

                    //Actor: p4_Forest_Coast_stool_A-2526 (441306) Gizmo  
                    //ActorId: 434076, Type: Gizmo, Name: p4_Forest_HighCleric_SpawnLectern-26508, Distance2d: 11.52699, CollisionRadius: 7.813453, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 
                    new InteractWithGizmoCoroutine(444702, 441322, 434076, -1, 5),

                    new WaitCoroutine(444702, 441322, 2000),

                    // p4_Forest_ClericGhost (433832) Distance: 18.61721
                    new InteractWithUnitCoroutine(444702, 441322, 433832, 0, 5),

                    new WaitCoroutine(444702, 441322, 8000),

                    new ClearAreaForNSecondsCoroutine(444702, 60, 0, 0, 45),

                }
            });



            // A5 - Bounty: The Cursed Wood (445158)
            Bounties.Add(new BountyData
            {
                QuestId = 445158,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(445158, 408254, 2912417), 
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 22.02429
                    new InteractWithGizmoCoroutine(445158, 408254, 365097, 0, 5),
                    new ClearAreaForNSecondsCoroutine(445158, 60, 365097, 2912417, 30),
                }
            });


            // A3 - Bounty: Kill Garan (436276)
            Bounties.Add(new BountyData
            {
                QuestId = 436276,
                Act = Act.A3,
                WorldId = 428493,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436276, 428493, 1052019909),
                    // P4_Ruins_CannibalBarbarian_C_Unique (435879) Distance: 46.67881
 //                 new MoveToActorCoroutine(436276, 428493, 435879),
                    new ClearLevelAreaCoroutine(436276),
                }
            });

            // A3 - Bounty: Kill Chiltara (349220)
            Bounties.Add(new BountyData
            {
                QuestId = 349220,
                Act = Act.A3,
                WorldId = 95804,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 34,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToPositionCoroutine(95804, new Vector3(3356, 562, 0)),
                    new EnterLevelAreaCoroutine(349220, 95804, 189259, 1029056444, 176003),
                    new EnterLevelAreaCoroutine(349220, 189259, 221688, 151580180, 176038),
                    new MoveToMapMarkerCoroutine(349220, 221688, 1645435934),
                    new ClearLevelAreaCoroutine(349220),
                }
            });

            // A3 - Bounty: Crazy Climber (346186)
            Bounties.Add(new BountyData
            {
                QuestId = 346186,
                Act = Act.A3,
                WorldId = 95804, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 34,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(346186, 95804, 2912417),

                    new MoveToScenePositionCoroutine(346186, 95804, "a3dun_Bridge_NS_Towers_05", new Vector3(196.2253f, 85.36218f, 0.624619f)),
                    new WaitCoroutine(346186, 95804, 1000),
                    new InteractWithUnitCoroutine(346186, 95804, 153419, 2912417, 5),

                    new MoveToScenePositionCoroutine(346186, 95804, "a3dun_Bridge_NS_Towers_05", new Vector3(192.509f, 83.62573f, 0.7063226f)),
                    new WaitCoroutine(346186, 95804, 1000),
                    new InteractWithUnitCoroutine(346186, 95804, 153419, 2912417, 5),

                    new MoveToScenePositionCoroutine(346186, 95804, "a3dun_Bridge_NS_Towers_05", new Vector3(131.6942f, 108.8494f, 38.24864f)),
                    new WaitCoroutine(346186, 95804, 5000),
                    new MoveToScenePositionCoroutine(346186, 95804, "a3dun_Bridge_NS_Towers_05", new Vector3(76.2251f, 108.259f, 38.20418f)),
                    new WaitCoroutine(346186, 95804, 5000),
                    new MoveToScenePositionCoroutine(346186, 95804, "a3dun_Bridge_NS_Towers_05", new Vector3(79.22241f, 41.45508f, 63.26687f)),
                    new WaitCoroutine(346186, 95804, 10000),
                    new MoveToScenePositionCoroutine(346186, 95804, "a3dun_Bridge_NS_Towers_05", new Vector3(79.22241f, 41.45508f, 63.26687f)),
                    new WaitCoroutine(346186, 95804, 10000),
                    new MoveToScenePositionCoroutine(346186, 95804, "a3dun_Bridge_NS_Towers_05", new Vector3(143.4139f, 35.06018f, 87.60001f)),

                    // bastionsKeepGuard_Lieutenant_Reinforcement_Event (153428) Distance: 3.705163
                    new InteractWithUnitCoroutine(346186, 95804, 153428, 0, 5),
                }
            });

            // A1 - Bounty: Kill Morgan LeDay (449841)
            Bounties.Add(new BountyData
            {
                QuestId = 449841,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(449841, 71150, 443346, -1019926638, 176001),
                    new MoveToMapMarkerCoroutine(449841, 443346, -707852521),
                    new ClearLevelAreaCoroutine(449841),
                }
            });

            // A1 - Bounty: Kill Teffeney (449864)
            Bounties.Add(new BountyData
            {
                QuestId = 449864,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 6,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(449864, 71150, 883191128),
                    new ClearLevelAreaCoroutine(449864),
                }
            });

            // A3 - Bounty: The Cursed Eternal Shrine (447419)
            Bounties.Add(new BountyData
            {
                QuestId = 447419,
                Act = Act.A3,
                WorldId = 428493,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(447419, 428493, 444305, -1665315172, 175467),
                    new MoveToMapMarkerCoroutine(447419, 444305, 2912417),
                    // x1_Event_CursedShrine (365097) Distance: 11.55153
                    new InteractWithGizmoCoroutine(447419, 444305, 365097, 0, 5),
                    new ClearAreaForNSecondsCoroutine(447419, 90, 365097, 2912417, 30),
                }
            });

            // A3 - Bounty: The Cursed Eternal Woods (447218)
            Bounties.Add(new BountyData
            {
                QuestId = 447218,
                Act = Act.A3,
                WorldId = 428493,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(447218, 428493, 444305, -1665315172, 175467),
                    new MoveToMapMarkerCoroutine(447218, 444305, 2912417),
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 24.78498
					new InteractWithGizmoCoroutine(447218, 444305, 365097, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(447218, 60, 365097, 2912417, 30),
//                    new ClearLevelAreaCoroutine(447218),
                }
            });

            // A5 - 현상금 사냥: 저주받은 숲 (445139)
            Bounties.Add(new BountyData
            {
                QuestId = 445139,
                Act = Act.A5,
                WorldId = 408254, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 59,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(445139, 408254, 2912417),
                    new InteractWithGizmoCoroutine(445139, 408254, 365097, 0, 5),
                    new ClearAreaForNSecondsCoroutine(445139, 60, 0, 0, 30),
                }
            });

            // A1 - Bounty: Kill Jovians (449391)
            Bounties.Add(new BountyData
            {
                QuestId = 449391,
                Act = Act.A1,
                WorldId = 71150, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(449391, 71150, 443346, -1019926638, 176001),
                    new MoveToMapMarkerCoroutine(449391, 443346, -1320159821),
                    new ClearLevelAreaCoroutine(449391),
                }
            });

            // A5 - Bounty: Kill The Succulent (444877)
            Bounties.Add(new BountyData
            {
                QuestId = 444877,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.KillMonster,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(444877, 408254, 432535, -1189768563, 176003),
                    new MoveToMapMarkerCoroutine(444877, 408254, -1189768563),
                    new MoveToMapMarkerCoroutine(444877, 432535, 1055678975),
                    new ClearLevelAreaCoroutine(444877),
                }
            });

            // A3 - Bounty: Clear The Icy Pit (445799)
            Bounties.Add(new BountyData
            {
                QuestId = 445799,
                Act = Act.A3,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(445799, 428493, 444305, -1665315172, 175467),
                    // g_Portal_Rectangle_Blue (175467) Distance: 16.18488
                    new InteractWithGizmoCoroutine(445799, 428493, 175467, -1665315172, 5),
                    new EnterLevelAreaCoroutine(445799, 444305, 0, 1537341835, 176002),
                    new MoveToMapMarkerCoroutine(445799, 444305, 1537341835),
                    // g_Portal_ArchTall_Blue (176002) Distance: 48.33623
                    new InteractWithGizmoCoroutine(445799, 444305, 176002, 1537341835, 5),
                    new ClearLevelAreaCoroutine(445799),
                }
            });



            // A1 - Bounty: Kill Spatharii (449389)
            Bounties.Add(new BountyData
            {
                QuestId = 449389,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(449389, 71150, 443346, -1019926638, 176001),
                    new MoveToMapMarkerCoroutine(449389, 443346, -1192804794),
                    new ClearLevelAreaCoroutine(449389),
                }
            });

            // A5 - Bounty: Kill Sartor (359543)
            Bounties.Add(new BountyData
            {
                QuestId = 359543,
                Act = Act.A5,
                WorldId = 338600,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 59,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359543, 338600, 255791851),
                    new ClearLevelAreaCoroutine(359543),
                }
            });

            // A1 - Bounty: Kill Walloon (449380)
            Bounties.Add(new BountyData
            {
                QuestId = 449380,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(449380, 71150, 443346, -1019926638, 176001),
                    new MoveToMapMarkerCoroutine(449380, 443346, 1929101567),
                    new ClearLevelAreaCoroutine(449380),
                }
            });

            // A1 - Bounty: Kill Boyarsk (449839)
            Bounties.Add(new BountyData
            {
                QuestId = 449839,
                Act = Act.A1,
                WorldId = 71150,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 50,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(449839, 71150, 443346, -1019926638, 176001),
                    new MoveToMapMarkerCoroutine(449839, 443346, -1396422803),
                    new ClearLevelAreaCoroutine(449839),
                }
            });

            // A5 - Bounty: The Cursed Pond (436965)
            Bounties.Add(new BountyData
            {
                QuestId = 436965,
                Act = Act.A5,
                WorldId = 408254,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 60,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436965, 408254, 2912417),
                    // x1_Event_CursedShrine (365097) Distance: 15.57718
//                    new InteractWithGizmoCoroutine(436965, 408254, 365097, 2912417, 5),
					new InteractWithGizmoCoroutine(436965, 408254, 365097, 2912417, 5),

                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(59.4035f, 51.7612f, -0.7321681f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(75.91962f, 49.12241f, -0.8999999f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(86.51501f, 68.63174f, -0.9f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(57.19635f, 72.65244f, -0.4761491f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(64.28271f, 57.70676f, -0.519156f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(80.21851f, 41.82578f, -0.5355958f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(84.3645f, 65.76697f, -0.8745145f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(58.29736f, 70.78946f, -0.4374949f)),
                    new WaitCoroutine(436965, 408254, 10000),
                    new MoveToScenePositionCoroutine(436965, 408254, "p4_Forest_Coast_NSEW_06", new Vector3(57.27289f, 50.69296f, -0.9f)),
                    new WaitCoroutine(436965, 408254, 10000),
										
//                  new ClearAreaForNSecondsCoroutine(436965, 60, 365097, 2912417, 30),
                }
            });



            // A1 - Bounty: Kill Baxtrus (449833)
            Bounties.Add(new BountyData
            {
                QuestId = 449833,
                Act = Act.A1,
                WorldId = 71150, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 18,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(449833, 71150, 443346, -1019926638, 176001),
                    new MoveToMapMarkerCoroutine(449833, 443346, 314659300),
                    new ClearLevelAreaCoroutine(449833),
                }
            });


            // A5 - Bounty: Research Problems (359310)
            Bounties.Add(new BountyData
            {
                QuestId = 359310,
                Act = Act.A5,
                WorldId = 267412,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 54,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359310, 267412, 2912417),
                    // OmniNPC_Tristram_Male_E_angryBatsEvent (343333) Distance: 58.61249
                    new InteractWithUnitCoroutine(359310, 267412, 343333, 0, 5),
                    new ClearAreaForNSecondsCoroutine(359310, 60, 0, 0, 40),
                    //ActorId: 237876, Type: Monster, Name: x1_BogFamily_brute_A-1514
                    //ActorId: 239516, Type: Monster, Name: x1_NightScreamer_A-1438
                    new InteractWithUnitCoroutine(359310, 267412, 343333, 0, 5),
                    new ClearAreaForNSecondsCoroutine(359310, 60, 0, 0, 40),
                }
            });

            // A5 - Bounty: Demon Prison (363394)
            Bounties.Add(new BountyData
            {
                QuestId = 363394,
                Act = Act.A5,
                WorldId = 271233,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 57,
                Coroutines = new List<ISubroutine>
                {
					// x1_Fortress_Portal_Switch (328830) Distance: 1.162077
                    new MoveToActorCoroutine(363394, 271233, 328830),
					// x1_Fortress_Portal_Switch (328830) Distance: 1.162077
					new InteractWithGizmoCoroutine(363394, 271233, 328830, -1751517829, 5),

                    new MoveToMapMarkerCoroutine(363394, 271233, 2912417),
                    // X1_Fortress_NephalemSpirit (354345) Distance: 21.83757
                    new InteractWithUnitCoroutine(363394, 271233, 354345, 2912417, 5),
                    //ActorId: 363943, Type: Gizmo, Name: x1_Fortress_Crystal_Prison_Yellow-2093, Distance2d: 19.41266, CollisionRadius: 14.90234, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
                    new InteractWithGizmoCoroutine(363394, 271233, 363943, -1, 5),

                    new MoveToScenePositionCoroutine(363394, 271233, "x1_fortress_NEW_02", new Vector3(118.0137f, 140.5249f, -9.534434f)),
                    new WaitCoroutine(363394, 271233, 10000),
                    new MoveToScenePositionCoroutine(363394, 271233, "x1_fortress_NEW_02", new Vector3(119.1356f, 114.215f, -9.534435f)),
                    new WaitCoroutine(363394, 271233, 10000),
                    new MoveToScenePositionCoroutine(363394, 271233, "x1_fortress_NEW_02", new Vector3(137.9068f, 127.946f, -9.806384f)),
                    new WaitCoroutine(363394, 271233, 10000),
                    new MoveToScenePositionCoroutine(363394, 271233, "x1_fortress_NEW_02", new Vector3(125.8439f, 149.037f, -9.534435f)),
                    new WaitCoroutine(363394, 271233, 10000),
                    new MoveToScenePositionCoroutine(363394, 271233, "x1_fortress_NEW_02", new Vector3(99.97601f, 134.6227f, -9.814684f)),
                    new WaitCoroutine(363394, 271233, 10000),
                    new MoveToScenePositionCoroutine(363394, 271233, "x1_fortress_NEW_02", new Vector3(117.2425f, 125.2808f, -9.534436f)),
                    new WaitCoroutine(363394, 271233, 10000),
	
//                  new ClearAreaForNSecondsCoroutine(363394, 60, 0, 0, 40),
                }
            });

            // A3 - Bounty: Kill Korae and Samae (436274)
            Bounties.Add(new BountyData
            {
                QuestId = 436274,
                Act = Act.A3,
                WorldId = 428493,
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 41,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436274, 428493, -100010011),
                    new MoveToMapMarkerCoroutine(436274, 428493, 1383249892),
                }
            });


            Bounties.Add(new BountyData
            {
                QuestId = 365385,
                Act = Act.A5,
                WorldId = 338600, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 59,
                Coroutines = new List<ISubroutine>
                {

                    new EnterLevelAreaCoroutine(365385, 338600, 357656, -1551729968, 176002),

                    new MoveToPositionCoroutine(357656, new Vector3(144, 196, -13)),
                    new InteractWithGizmoCoroutine(365385, 357656, 365488, 0, 5),

                    new MoveToPositionCoroutine(357656, new Vector3(160, 74, -13)),
                    //ActorId: 365489, Type: Gizmo, Name: x1_Fortress_Crystal_Prison_DemonEvent_2-8105, Distance2d: 8.03618, CollisionRadius: 14.90234, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
                    new InteractWithGizmoCoroutine(365385, 357656, 365489, 0, 5),

                    new MoveToPositionCoroutine(357656, new Vector3(139, 69, -13)),
                    new InteractWithGizmoCoroutine(357656, 357656, 365489, 0, 5),


                    new ClearAreaForNSecondsCoroutine(357656, 60, 0, 0, 45),

                    // by Chest
                    new MoveToPositionCoroutine(357656, new Vector3(144, 127, -13)),
                }
            });

            // A5 - Bounty: The Cursed Crystals (367904)
            // Portal and crystals keep changing actor ids, is this a dynamic bounty?
            Bounties.Add(new BountyData
            {
                QuestId = 367904,
                Act = Act.A5,
                WorldId = 338600, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 58,
                Coroutines = new List<ISubroutine>
                {
                    //ActorId: 176004, Type: Gizmo, Name: g_Portal_RectangleTall_Blue-47533, Distance2d: 10.84147, 
					new MoveToMapMarkerCoroutine(367904, 338600, -1551729968),
//                    new EnterLevelAreaCoroutine(367904, 338600, 357656, -1551729968, 176002),
					new EnterLevelAreaCoroutine(367904, 338600, 0, -1551729968, 0),


                    new MoveToPositionCoroutine(357656, new Vector3(147, 188, -13)),
                    // x1_Fortress_Crystal_Prison_DemonEvent_1 (365488) Distance: 31.76865
                    new InteractWithGizmoCoroutine(367904, 357656, 365488, 0, 5),

                    new MoveToPositionCoroutine(357656, new Vector3(175, 72, -13)),
                    new InteractWithGizmoCoroutine(367904, 357656, 365489, 0, 5),

                    new MoveToPositionCoroutine(357656, new Vector3(142, 74, -13)),
                    new InteractWithGizmoCoroutine(367904, 357656, 365995, 0, 5),

                    new ClearAreaForNSecondsCoroutine(367904, 120, 0, 0, 45),

                    // by Chest
                    new MoveToPositionCoroutine(357656, new Vector3(144, 127, -13)),
                }
            });

            // A5 - Bounty: Grave Situation (359101)
            Bounties.Add(new BountyData
            {
                QuestId = 359101,
                Act = Act.A5,
                WorldId = 338944,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 52,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359101, 338944, 2912417),
//                    new MoveToActorCoroutine(359101, 338944, 331951),
                    new WaitCoroutine(359101, 338944, 3000),

                    new MoveToScenePositionCoroutine(359101, 338944, "x1_westm_graveyard_NSEW_13", new Vector3(79.35699f, 47.7887f, 0.1f)),
                    new WaitCoroutine(359101, 338944, 10000),
                    new MoveToScenePositionCoroutine(359101, 338944, "x1_westm_graveyard_NSEW_13", new Vector3(89.59161f, 69.89282f, 0.1f)),
                    new WaitCoroutine(359101, 338944, 10000),
                    new MoveToScenePositionCoroutine(359101, 338944, "x1_westm_graveyard_NSEW_13", new Vector3(64.90564f, 82.27252f, 0.1f)),
                    new WaitCoroutine(359101, 338944, 10000),
                    new MoveToScenePositionCoroutine(359101, 338944, "x1_westm_graveyard_NSEW_13", new Vector3(66.88336f, 34.40729f, 0.1f)),
                    new WaitCoroutine(359101, 338944, 10000),
                    new MoveToScenePositionCoroutine(359101, 338944, "x1_westm_graveyard_NSEW_13", new Vector3(77.21259f, 48.01953f, 0.1f)),

                    new ClearAreaForNSecondsCoroutine(359101, 60, 331951, 2912417, 30),
                    new InteractWithUnitCoroutine(359101, 338944, 331948, 0, 5),
                }
            });

            // A5 - Bounty: Cryptology (359079)
            Bounties.Add(new BountyData
            {
                QuestId = 359079,
                Act = Act.A5,
                WorldId = 338944,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointNumber = 52,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(359079, 338944, 2912417),
                    // x1_westmarchFemale_A_Graveyard_Unique_1 (332861) Distance: 8.440428
                    new InteractWithUnitCoroutine(359079, 338944, 332861, 2912417, 5),

                    new MoveToScenePositionCoroutine(359079, 338944, "x1_westm_graveyard_NSEW_06", new Vector3(46.19928f, 72.97696f, 0.6203882f)),
                    new WaitCoroutine(359079, 338944, 5000),
                    new MoveToScenePositionCoroutine(359079, 338944, "x1_westm_graveyard_NSEW_06", new Vector3(47.20917f, 90.77298f, 0.1f)),
                    new WaitCoroutine(359079, 338944, 5000),
                    new MoveToScenePositionCoroutine(359079, 338944, "x1_westm_graveyard_NSEW_06", new Vector3(82.65125f, 81.61206f, 0.1f)),
                    new WaitCoroutine(359079, 338944, 5000),
//                    new ClearAreaForNSecondsCoroutine(359079, 60, 0, 0, 45),
                    // x1_Westm_Graveyard_Ghost_Female_01_UniqueEvent (357197) Distance: 5.017566
                    new InteractWithUnitCoroutine(359079, 338944, 357197, 0, 5),
                    new WaitCoroutine(359079, 338944, 10000),
                }
            });



        }

        private static void AddNewBounties()
        {
            // A1 - Bounty: The Cursed Mill (365401)
            Bounties.Add(new BountyData
            {
                QuestId = 365401,
                Act = Act.A1,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: The Cursed Temple (464598)
            Bounties.Add(new BountyData
            {
                QuestId = 464598,
                Act = Act.A2,
                WorldId = 456634, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = 456638,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine (464598, 456634, 2912417),
                    new InteractWithGizmoCoroutine (464598,456634, 365097, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine (464598, 60, 365097, 2912417, 30)
                }
            });

            // A2 - Bounty: Kill Agustin The Marked (464730)
            Bounties.Add(new BountyData
            {
                QuestId = 464730,
                Act = Act.A2,
                WorldId = 460372,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 460671,
                Coroutines = new List<ISubroutine>
                {
                   // Name: p6_RavenFlyer_Unique_A-23893 ActorSnoId: 464732, Distance: 4.847482
                   new KillUniqueMonsterCoroutine (464730, 460372, 464732, -2033595591),
                   new ClearLevelAreaCoroutine(464730)
                }
            });

            // A2 - Bounty: Kill Vidian (474066)
            Bounties.Add(new BountyData
            {
                QuestId = 474066,
                Act = Act.A2,
                WorldId = 464096, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 456638,
                Coroutines = new List<ISubroutine>
                {
                     new EnterLevelAreaCoroutine(474066, 456634, 464096, -1355958932, 176001),
                     new MoveToMapMarkerCoroutine (474066, 464096, 1944200381),
                     new EnterLevelAreaCoroutine(474066, 464096, 470238, 1944200381, 467345),
                     // Type: Monster Name: p6_Shepherd_Boss-32030 ActorSnoId: 464225
                     new KillUniqueMonsterCoroutine (474066, 464096, 464225, 0),
                }
            });

            // A2 - Bounty: The Cursed Lake (466835)
            Bounties.Add(new BountyData
            {
                QuestId = 466835,
                Act = Act.A2,
                WorldId = 460372, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = 460671,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine (466835, 460372, 2912417),
                    new InteractWithGizmoCoroutine (466835,460372, 365097, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine (466835, 60, 365097, 2912417, 30)
                }
            });

            // A2 - Bounty: Lost Treasure of Khan Dakab (346067)
            Bounties.Add(new BountyData
            {
                QuestId = 346067,
                Act = Act.A2,
                WorldId = 158593,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 57425,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToSceneCoroutine(346067, 70885, "caOut_Oasis_Sub240_POI"),

                    new MoveToScenePositionCoroutine(346067, 70885, "caOut_Oasis_Sub240_WaterPuzzle", new Vector3(95.17871f, 61.56543f, 80.34052f)),
                    new InteractWithGizmoCoroutine(346067, 70885, 175603, 0, 5),

                    new MoveToScenePositionCoroutine(346067, 70885, "caOut_Oasis_Sub240_WaterPuzzle", new Vector3(65.4021f, 90.04639f, 80.63489f)),
                    new InteractWithGizmoCoroutine(346067, 70885, 175603, 0, 5),

                    // Haxx.. g_Portal_ArchTall_Blue (176002) Distance: 29.04947
                    new MoveToScenePositionCoroutine(346067, 70885, "caOut_Oasis_Sub240_WaterPuzzle", new Vector3(94.06543f, 95.49268f, 73.67375f)),
                    new InteractionCoroutine(176002, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1)),

                    // Find and pull the lever. a2dun_Aqd_Act_Lever_FacePuzzle_02 (219880) Distance: 20.90631
                    new InteractWithGizmoCoroutine(346067, 158593, 219880, 0, 5),

                    // a2dun_Aqd_GodHead_Door_LargePuzzle-8357
                    new MoveToMapMarkerCoroutine(346067, 158593, -1469964931),
                    new InteractWithGizmoCoroutine(346067, 158593, 207615, 0),

                    // a2dun_Aqd_Chest_Special_FacePuzzle_Large (190524) Distance: 27.1103
                    new InteractWithGizmoCoroutine(346067, 158593, 190524, 0, 5),
                    new ClearAreaForNSecondsCoroutine(346067, 30, 0, 0, 45),
                }
            });

            // A2 - Bounty: Kill Bain (464406)
            Bounties.Add(new BountyData
            {
                QuestId = 464406,
                Act = Act.A2,
                WorldId = 456634,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 456638,
                Coroutines = new List<ISubroutine>
                {
                    // a2dun_Aqd_GodHead_Door_LargePuzzle-8357
                    new MoveToMapMarkerCoroutine(464406, 456634, 632612270),
                    new KillUniqueMonsterCoroutine(464406, 456634, 464204, 0),
                    new ClearLevelAreaCoroutine(464406),
                }
            });

            // A2 - Bounty: Clear the Forgotten Well (467028)
            Bounties.Add(new BountyData
            {
                QuestId = 467028,
                Act = Act.A2,
                WorldId = 465884,
                QuestType = BountyQuestType.ClearZone,
                WaypointLevelAreaId = 460671,
                Coroutines = new List<ISubroutine>
                {
                    // g_Portal_Circle_Blue-32941 
                    new EnterLevelAreaCoroutine(467028, 460372, 465884, -1484677704, 176003),
                    new ClearLevelAreaCoroutine(467028)
                }
            });

            // A2 - Bounty: The Cursed Moors (464596)
            Bounties.Add(new BountyData
            {
                QuestId = 464596,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 270011,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: Break A Few Eggs (469699)
            Bounties.Add(new BountyData
            {
                QuestId = 469699,
                Act = Act.A2,
                WorldId = 460372,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 460671,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(469699, 460372, 2912417),
                    new WaitCoroutine(469699, 460372, 5000),
                    // : P6_Friendly_Adventurer-465 ActorSnoId: 462095
                    new ClearAreaForNSecondsCoroutine(469699, 60, 462095, 0, 50)
                }
            });


            // A2 - Bounty: Blood Statue (465128)
            Bounties.Add(new BountyData
            {
                QuestId = 465128,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 92945,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: Grave Mistakes (466790)
            Bounties.Add(new BountyData
            {
                QuestId = 466790,
                Act = Act.A2,
                WorldId = 460372, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 460671,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(466790, 460372, 2912417),
                    new MoveToActorCoroutine(466790, 460372, (int)SNOActor.p6_Event_Moor_GraveRobbers_graveDigger_A),
                    new WaitCoroutine(466790, 460372, 5000),
                    new ClearAreaForNSecondsCoroutine(466790, 60, (int)SNOActor.p6_Event_Moor_GraveRobbers_graveDigger_A, 0, 50)
                }
            });

            // A2 - Bounty: The Hematic Key (471761)
            Bounties.Add(new BountyData
            {
                QuestId = 471761,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: Clear the Mysterious Cave (347598)
            Bounties.Add(new BountyData
            {
                QuestId = 347598,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: Off The Wagon (470068)
            Bounties.Add(new BountyData
            {
                QuestId = 470068,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: Kill Seanus Greyback (464180)
            Bounties.Add(new BountyData
            {
                QuestId = 464180,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 270011,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: Blood Collection (465117)
            Bounties.Add(new BountyData
            {
                QuestId = 465117,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 0,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: Kill Elizar Bathory (471622)
            Bounties.Add(new BountyData
            {
                QuestId = 471622,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 270011,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            //A2 - Ancient Devices - Interact with operated gizmo repeatly
            Bounties.Add(new BountyData
            {
                QuestId = 433025,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.GuardedGizmo,
                WaypointLevelAreaId = 53834,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433025, 432885)
                }
            });

            // A2 - Bounty: Kill Lupgaron (471712)
            Bounties.Add(new BountyData
            {
                QuestId = 471712,
                Act = Act.A2,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 109538,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A2 - Bounty: One In The Hand (470218)
            Bounties.Add(new BountyData
            {
                QuestId = 470218,
                Act = Act.A2,
                WorldId = 460372,
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 460671,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(470218, 460372, 2912417),
                    new WaitCoroutine(470218, 460372, 7500),
                    // p6_Moor_Chest-3053 ActorSnoId: 462211
                    new ClearAreaForNSecondsCoroutine(470218, 30, 462211, 2912417),
                }
            });

            // A4 - Bounty: Kill Argosh (470722)
            Bounties.Add(new BountyData
            {
                QuestId = 470722,
                Act = Act.A4,
                WorldId = 456029,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464063,
                Coroutines = new List<ISubroutine>
                {
                    // Name: p6_Unburied_C_Unique_01_ROF_V2_01-1437 ActorSnoId: 4707559
                    new KillUniqueMonsterCoroutine(470722, 456029, 4707559, 881692424),
                    new ClearLevelAreaCoroutine(470722),
                }
            });

            // A4 - Bounty: Kill Bjortor (470712)
            Bounties.Add(new BountyData
            {
                QuestId = 470712,
                Act = Act.A4,
                WorldId = 456029,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 475854,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(470712, 456029, -1254225905),
                    // P6_Sasquatch_B_Unique_RoF_01-17925 ActorSnoId: 470709
                    new KillUniqueMonsterCoroutine(470712, 456029, 470709, -1254225905),
                    new ClearLevelAreaCoroutine(470712),
                }
            });

            // A4 - Bounty: Kill Gro'Mag (468481)
            Bounties.Add(new BountyData
            {
                QuestId = 468481,
                Act = Act.A4,
                WorldId = 460587,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464066,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToScenePositionCoroutine(468481, 460587, "LS_a4dun_spire_LibraryOfFate_05", new Vector3(117.0737f, 229.1764f, 31.1f)),
                    new MoveToScenePositionCoroutine(468481, 460587, "LS_a4dun_spire_corrupt_NS_02", new Vector3(226.8757f, 92.24744f, 31.1f)),
                    // p6_x1_westmarchRanged_A_Unique_ROF_V5_01-75810 ActorSnoId: 468511
                    new KillUniqueMonsterCoroutine(468481, 460587, 468511, 465303410),
                    new ClearLevelAreaCoroutine(468481),
                }
            });

            // A4 - Bounty: Kill Janderson (470746)
            Bounties.Add(new BountyData
            {
                QuestId = 470746,
                Act = Act.A4,
                WorldId = 460587, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 475856,
                Coroutines = new List<ISubroutine>
                {
                    // Name: P6_Skeleton_B_Unique_01_RoF-5950 ActorSnoId: 470744
                    new KillUniqueMonsterCoroutine(470746, 460587, 470744, 243521159),
                    new ClearLevelAreaCoroutine(470746),
                }
            });

            // A4 - Bounty: Kill Jadtalek (471158)
            Bounties.Add(new BountyData
            {
                QuestId = 471158,
                Act = Act.A4,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A4 - Bounty: Kill Barfield (471107)
            Bounties.Add(new BountyData
            {
                QuestId = 471107,
                Act = Act.A4,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A4 - Bounty: Kill Barrigast (471135)
            Bounties.Add(new BountyData
            {
                QuestId = 471135,
                Act = Act.A4,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A4 - Bounty: Kill Strychnos (471228)
            Bounties.Add(new BountyData
            {
                QuestId = 471228,
                Act = Act.A4,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A4 - Bounty: The Cursed Realm (470608)
            Bounties.Add(new BountyData
            {
                QuestId = 470608,
                Act = Act.A4,
                WorldId = 458965, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = 464857,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(470608, 458965, 2912417),
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 27.16287
                    new InteractWithGizmoCoroutine(470608, 458965, 365097, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(470608, 60, 365097, 2912417, 30),
                }
            });

            // A4 - Bounty: The Cursed Realm (470561)
            Bounties.Add(new BountyData
            {
                QuestId = 470561,
                Act = Act.A4,
                WorldId = 456029, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = 464063,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(470561, 456029, 2912417),
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 27.16287
                    new InteractWithGizmoCoroutine(470561, 456029, 365097, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(470561, 60, 365097, 2912417, 30),
                }
            });
   

            // A4 - Bounty: Kill Prratshet the Reaper (471131)
            Bounties.Add(new BountyData
            {
                QuestId = 471131,
                Act = Act.A4,
                WorldId = 457461,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464065,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToScenePositionCoroutine(471131, 457461, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(184.2814f, 72.55072f, -74.9f)),
                    new MoveThroughDeathGates(471131, 457461, 1),
                    // p6_Angel_Corrupt_A_Unique_RoF_V3_01-22241 ActorSnoId: 471142
                    new KillUniqueMonsterCoroutine(471131, 457461, 471142, -1208602370),
                    new ClearLevelAreaCoroutine(471131)
                }
            });

            // A4 - Bounty: Kill Necronom (471224)
            Bounties.Add(new BountyData
            {
                QuestId = 471224,
                Act = Act.A4,
                WorldId = 458965, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464857,
                Coroutines = new List<ISubroutine>
                {
                    // Name: p6_X1_armorScavenger_Unique_RoF_V4_01-809 ActorSnoId: 471230
                    new KillUniqueMonsterCoroutine(471224, 458965, 471230, 1900506513),
                    new ClearLevelAreaCoroutine(471224)
                }
            });

            // A4 - Bounty: Kill Silli (471133)
            Bounties.Add(new BountyData
            {
                QuestId = 471133,
                Act = Act.A4,
                WorldId = 457461,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464065,
                Coroutines = new List<ISubroutine>
                {
                    // p6_DeathMaiden_Unique_RoF_V3-711 ActorSnoId: 471137
                    new KillUniqueMonsterCoroutine(471133, 457461, 471137, -454310883),
                    new MoveToScenePositionCoroutine(471133, 457461, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(185.6754f, 73.09955f, -74.9f)),
                    new MoveThroughDeathGates(471133, 457461, 1),
                    new ClearLevelAreaCoroutine(471133)
                }
            });

            // A4 - Bounty: Kill Guytan Pyrrus (471105)
            Bounties.Add(new BountyData
            {
                QuestId = 471105,
                Act = Act.A4,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A4 - Bounty: Kill the Manipulator of Lore (470717)
            Bounties.Add(new BountyData
            {
                QuestId = 470717,
                Act = Act.A4,
                WorldId = 457461,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464065,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(470717, 457461, 619535446),
                    new ClearLevelAreaCoroutine(470717),
                }
            });

            // A4 - Bounty: Kill Old Hardshell (471156)
            Bounties.Add(new BountyData
            {
                QuestId = 471156,
                Act = Act.A4,
                WorldId = 460587, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 475856,
                Coroutines = new List<ISubroutine>
                {
                    // Name: p6_Crab_Mother_Unique_RoF_V5_01-10653 ActorSnoId: 471182
                    new KillUniqueMonsterCoroutine(471156, 460587, 471182, 1898390417),
                    new ClearLevelAreaCoroutine(471156)
                }
            });

            // A4 - Bounty: The Cursed Realm (467798)
            Bounties.Add(new BountyData
            {
                QuestId = 467798,
                Act = Act.A4,
                WorldId = 457461, // Enter the final worldId here
                QuestType = BountyQuestType.ClearCurse,
                WaypointLevelAreaId = 464065,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToScenePositionCoroutine(467798, 457461, "P6_Lost_Souls_a4dun_spire_LibraryOfFate_03", new Vector3(120.01f, 229.5111f, 15.1f)),
                    new MoveToScenePositionCoroutine(467798, 457461, "P6_Lost_Souls_x1_fortress_EW_05_soul_well", new Vector3(186.6548f, 76.25372f, -74.9f)),
                    new MoveThroughDeathGates(467798, 457461, 1),

                    new MoveToMapMarkerCoroutine(467798, 457461, 2912417),
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 27.16287
                    new InteractWithGizmoCoroutine(467798, 457461, 365097, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(467798, 60, 365097, 2912417, 30),
                }
            });

            // A4 - Bounty: Kill Reyes (464928)
            Bounties.Add(new BountyData
            {
                QuestId = 464928,
                Act = Act.A4,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 92945,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A4 - Bounty: Kill Mzuuman (471160)
            Bounties.Add(new BountyData
            {
                QuestId = 471160,
                Act = Act.A4,
                WorldId = 460587,
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464066,
                Coroutines = new List<ISubroutine>
                {
                    // Name: p6_morluSpellcaster_A_Unique_RoF_V5_01-950 ActorSnoId: 471180
                    new KillUniqueMonsterCoroutine (471160, 460587, 471180, 2034862812),
                    new ClearLevelAreaCoroutine (471160)
                }
            });

            // A4 - Bounty: Kill Senahde (468089)
            Bounties.Add(new BountyData
            {
                QuestId = 468089,
                Act = Act.A4,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 19954,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });

            // A4 - Bounty: Kill K'Zigler (470651)
            Bounties.Add(new BountyData
            {
                QuestId = 470651,
                Act = Act.A4,
                WorldId = 458965, // Enter the final worldId here
                QuestType = BountyQuestType.KillMonster,
                WaypointLevelAreaId = 464857,
                Coroutines = new List<ISubroutine>
                {
                     // Name: p6_X1_armorScavenger_A_Unique_ROF_V4_01-4477 ActorSnoId: 470666
                    new KillUniqueMonsterCoroutine (470651, 458965, 470666, -541902607),
                    new ClearLevelAreaCoroutine (470651)
                }
            });

            // A5 - Bounty: Brutal Assault (359939)
            Bounties.Add(new BountyData
            {
                QuestId = 359939,
                Act = Act.A5,
                WorldId = 0, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                WaypointLevelAreaId = 332339,
                Coroutines = new List<ISubroutine>
                {
                    // Coroutines goes here
                }
            });
        }

        [Obsolete("Use waypoint factory for levelarea instead")]
        public static readonly Dictionary<int, int> QuestWaypointNumbers = new Dictionary<int, int>
        {
            { 344497, 13 },
            { 344499, 13 },
            { 344501, 13 },
            { 344503, 13 },
            { 344547, 13 },
            { 345488, 6 },
            { 345490, 6 },
            { 345496, 8 },
            { 345498, 8 },
            { 345502, 10 },
            { 345505, 10 },
            { 345507, 10 },
            { 345517, 10 },
            { 345520, 10 },
            { 345522, 12 },
            { 345524, 12 },
            { 345526, 12 },
            { 345542, 15 },
            { 345544, 15 },
            { 345862, 12 },
            { 345954, 19 },
            { 345960, 19 },
            { 345971, 19 },
            { 346036, 22 },
            { 346069, 23 },
            { 346071, 23 },
            { 346075, 23 },
            { 346086, 24 },
            { 346088, 24 },
            { 346090, 24 },
            { 346092, 24 },
            { 346094, 24 },
            { 346108, 25 },
            { 346115, 25 },
            { 346117, 25 },
            { 346119, 25 },
            { 346121, 25 },
            { 346123, 25 },
            { 346128, 28 },
            { 346130, 28 },
            { 346132, 28 },
            { 346146, 29 },
            { 346148, 29 },
            { 346154, 29 },
            { 346157, 29 },
            { 346160, 30 },
            { 346162, 31 },
            { 346164, 31 },
            { 346182, 32 },
            { 346188, 32 },
            { 346190, 33 },
            { 346192, 33 },
            { 346194, 32 },
            { 346196, 32 },
            { 346200, 33 },
            { 346202, 33 },
            { 346204, 34 },
            { 346215, 38 },
            { 346217, 38 },
            { 346219, 37 },
            { 346222, 37 },
            { 346225, 35 },
            { 346228, 35 },
            { 346186, 35 },
            { 346230, 36 },
            { 346232, 39 },
            { 346235, 36 },
            { 347011, 15 },
            { 347020, 17 },
            { 347023, 17 },
            { 347054, 10 },
            { 347056, 10 },
            { 347058, 8 },
            { 347065, 8 },
            { 347070, 8 },
            { 347073, 8 },
            { 347095, 7 },
            { 347097, 7 },
            { 347099, 6 },
            { 347525, 19 },
            { 347532, 19 },
            { 347534, 19 },
            { 347560, 22 },
            { 347563, 22 },
            { 347565, 22 },
            { 347569, 22 },
            { 347598, 23 },
            { 347600, 23 },
            { 347604, 23 },
            { 347607, 24 },
            { 347650, 25 },
            { 347652, 25 },
            { 347654, 25 },
            { 349026, 16 },
            { 349115, 30 },
            { 349117, 29 },
            { 349119, 30 },
            { 349121, 31 },
            { 349198, 32 },
            { 349202, 32 },
            { 349204, 32 },
            { 349206, 32 },
            { 349208, 34 },
            { 349210, 32 },
            { 349212, 32 },
            { 349214, 33 },
            { 349216, 35 },
            { 349218, 34 },
            { 349222, 38 },
            { 349226, 39 },
            { 349228, 39 },
            { 349230, 37 },
            { 349232, 35 },
            { 349234, 36 },
            { 349236, 36 },
            { 349238, 36 },
            { 349240, 36 },
            { 349252, 43 },
            { 349254, 43 },
            { 349256, 41 },
            { 349258, 41 },
            { 349260, 41 },
            { 349270, 45 },
            { 349272, 45 },
            { 349274, 45 },
            { 349276, 44 },
            { 349278, 44 },
            { 349280, 44 },
            { 349282, 45 },
            { 349298, 37 },
            { 349301, 30 },
            { 350556, 21 },
            { 350564, 21 },
            { 354788, 24 },
            { 355278, 21 },
            { 355282, 52 },
            { 356417, 34 },
            { 356422, 35 },
            { 357127, 43 },
            { 357129, 43 },
            { 357131, 41 },
            { 357133, 44 },
            { 357135, 44 },
            { 357137, 45 },
            { 357139, 45 },
            { 359233, 50 },
            { 361320, 2 },
            { 361327, 2 },
            { 361331, 2 },
            { 361334, 3 },
            { 361336, 3 },
            { 361339, 3 },
            { 361341, 4 },
            { 361343, 4 },
            { 361345, 4 },
            { 361352, 5 },
            { 361354, 5 },
            { 362140, 46 },
            { 362913, 52 },
            { 362915, 52 },
            { 362921, 52 },
            { 362923, 52 },
            { 362925, 52 },
            { 362996, 53 },
            { 363000, 53 },
            { 363013, 54 },
            { 363016, 54 },
            { 363019, 54 },
            { 363021, 54 },
            { 363075, 49 },
            { 363078, 49 },
            { 363080, 49 },
            { 363082, 49 },
            { 363086, 50 },
            { 363090, 50 },
            { 363092, 50 },
            { 363177, 51 },
            { 363180, 51 },
            { 363185, 51 },
            { 363194, 57 },
            { 363196, 57 },
            { 363198, 57 },
            { 363200, 57 },
            { 363202, 57 },
            { 363204, 57 },
            { 363552, 55 },
            { 363555, 55 },
            { 363557, 55 },
            { 363559, 56 },
            { 363561, 56 },
            { 363563, 56 },
            { 365381, 8 },
            { 365401, 8 },
            { 367559, 14 },
            { 367561, 14 },
            { 367935, 57 },
            { 368559, 49 },
            { 368781, 49 },
            { 368783, 49 },
            { 368785, 49 },
            { 368789, 51 },
            { 368917, 57 },
            { 369243, 8 },
            { 369246, 8 },
            { 369249, 10 },
            { 369251, 10 },
            { 369253, 14 },
            { 369257, 14 },
            { 369271, 14 },
            { 369273, 14 },
            { 369277, 21 },
            { 369288, 21 },
            { 369291, 21 },
            { 369298, 23 },
            { 369300, 23 },
            { 369302, 23 },
            { 369312, 28 },
            { 369316, 28 },
            { 369319, 28 },
            { 369323, 28 },
            { 369326, 28 },
            { 369329, 28 },
            { 369399, 6 },
            { 369404, 6 },
            { 369763, 13 },
            { 369789, 17 },
            { 369797, 19 },
            { 369800, 19 },
            { 369813, 23 },
            { 369825, 32 },
            { 369851, 33 },
            { 369853, 30 },
            { 369868, 37 },
            { 369878, 45 },
            { 369900, 42 },
            { 369908, 57 },
            { 369944, 1 },
            { 369952, 52 },
            { 375191, 3 },
            { 375198, 2 },
            { 375201, 12 },
            { 375257, 25 },
            { 375261, 25 },
            { 375268, 49 },
            { 375275, 55 },
            { 375278, 52 },
            { 375348, 51 },
            { 409753, 47 },
            { 409755, 47 },
            { 409757, 47 },
            { 409759, 47 },
            { 409761, 42 },
            { 409763, 42 },
            { 409765, 42 },
            { 409767, 42 },
            { 409884, 43 },
            { 409893, 47 },
            { 409897, 43 },
            { 430723, 6 },
            { 432293, 13 },
            { 432334, 23 },
            { 432699, 51 },
            { 432701, 51 },
            { 432705, 51 },
            { 432707, 51 },
            { 432784, 12 },
            { 433003, 26 },
            { 433005, 26 },
            { 433007, 26 },
            { 433009, 26 },
            { 433011, 26 },
            { 433013, 26 },
            { 433025, 24 },
            { 433053, 21 },
            { 433099, 43 },
            { 433217, 34 },
            { 433256, 51 },
            { 433309, 37 },
            { 433339, 50 },
            { 433392, 28 },
            { 433422, 44 },
            { 434378, 16 },
            { 344482, 13 },
            { 344486, 13 },
            { 344488, 13 },
            { 344490, 13 },
            { 345500, 8 },
            { 345546, 16 },
            { 345973, 19 },
            { 345976, 19 },
            { 346034, 22 },
            { 346065, 23 },
            { 346067, 23 },
            { 346111, 25 },
            { 346180, 32 },
            { 346184, 34 },
            { 347027, 17 },
            { 347030, 16 },
            { 347060, 8 },
            { 347062, 8 },
            { 347520, 19 },
            { 347591, 23 },
            { 347595, 23 },
            { 347638, 24 },
            { 349016, 7 },
            { 349020, 7 },
            { 349196, 32 },
            { 350529, 14 },
            { 350560, 21 },
            { 350562, 21 },
            { 359079, 50 },
            { 359096, 50 },
            { 359101, 50 },
            { 359112, 50 },
            { 359116, 50 },
            { 359280, 52 },
            { 359314, 52 },
            { 359319, 52 },
            { 359399, 49 },
            { 359403, 49 },
            { 359543, 57 },
            { 359758, 51 },
            { 363342, 57 },
            { 363344, 57 },
            { 363346, 57 },
            { 367870, 52 },
            { 367872, 52 },
            { 367884, 57 },
            { 367888, 57 },
            { 367904, 57 },
            { 367926, 57 },
            { 368420, 49 },
            { 368433, 49 },
            { 368445, 49 },
            { 368525, 49 },
            { 368532, 49 },
            { 368536, 49 },
            { 368555, 49 },
            { 368564, 49 },
            { 368601, 51 },
            { 368607, 51 },
            { 368611, 51 },
            { 368613, 51 },
            { 368796, 57 },
            { 368912, 57 },
            { 368915, 57 },
            { 368919, 57 },
            { 368921, 57 },
            { 374571, 56 },
            { 375350, 49 },
            { 409888, 43 },
            { 409895, 43 },
            { 433017, 26 },
            { 349262, 41 },
            { 347558, 22 },
            { 375264, 24 },

        };
    }
}
