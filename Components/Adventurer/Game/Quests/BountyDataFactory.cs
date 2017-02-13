using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Components.Adventurer.Util.Logger;

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
        }

        public static BountyData GetBountyData(int questId)
        {
            if(questId <= 0)
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
                        Util.Logger.Error("Dynamic Bounty is not supported {0} ({1})", quest.Name, quest.QuestId, type, bountyData);
                        break;
                }

                Util.Logger.Debug("Created Dynamic Bounty for {0} ({1}) Type={2} {3}", quest.Name, quest.QuestId, type, bountyData);

                return bountyData;
            }
            return null;
        }

        private static BountyData CreateChestAndClearBounty(QuestData quest)
        {
            if (quest == null)
            {
                Logger.Debug("[CreateChestAndClearBounty] quest was null");
                return null;
            }

            if(quest.Waypoint == null)
            {
                Logger.Debug($"[CreateChestAndClearBounty] quest {quest.Name} ({quest.QuestId}) waypoint was null");
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
                    new ClearAreaForNSecondsCoroutine(quest.QuestId, 60, 0, 0, 45),
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

            // A2 - Blood and Iron (432334)
            Bounties.Add(new BountyData
            {
                QuestId = 432334,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(432334, 432331)
                }
            });

            // A2 - The Shrine of Rakanishu (346065)
            Bounties.Add(new BountyData
            {
                QuestId = 346065,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(346065,70885,2912417),
                        new ClearAreaForNSecondsCoroutine(346065,30,222271,2912417,70,false)
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
                    new MoveToActorCoroutine(363344, 338600, 354407),
                    // X1_Angel_Common_Event_GreatWeapon (354407) Distance: 31.32524
                    new InteractWithUnitCoroutine(363344, 338600, 354407, 0, 5),
                    new MoveToScenePositionCoroutine(363344, 338600, "GreatWeapon", new Vector3(114.6218f, 161.0695f, -17.9f)),
                    new ClearAreaForNSecondsCoroutine(363344, 100, 354407, 0, 45),

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
                    new MoveToMapMarkerCoroutine(363390, 271235, 2912417),
                    new MoveToScenePositionCoroutine(363390, 271235, "mousetrap", new Vector3(123.1343f, 134.059f, -19.52854f)),
                    // x1_Fortress_Crystal_Prison_MouseTrap (363870) Distance: 7.876552
                    new InteractWithGizmoCoroutine(363390, 271235, 363870, 0, 5),
                    new ClearAreaForNSecondsCoroutine(363390, 20, 0, 0, 45),
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
                    new MoveToPositionCoroutine(95804, new Vector3(2595, 610, 0)),
                    new MoveToMapMarkerCoroutine(346186, 95804, 2912417),
                    // bastionsKeepGuard_Injured_Reinforcement_Event (153419) Distance: 36.5172
                    new MoveToActorCoroutine(346186, 95804, 153419),
                    // bastionsKeepGuard_Injured_Reinforcement_Event (153419) Distance: 45.71638
                    new InteractWithUnitCoroutine(346186, 95804, 153419, 2912417, 5),
                    new MoveToPositionCoroutine(95804, new Vector3(1761, 593, 38)),
                    new MoveToPositionCoroutine(95804, new Vector3(1760, 520, 63)),
                    new MoveToPositionCoroutine(95804, new Vector3(1820, 523, 87)),
                    // bastionsKeepGuard_Lieutenant_Reinforcement_Event (153428) Distance: 3.705163
                    new InteractWithUnitCoroutine(346186, 95804, 153428, 0, 5),
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
//                        new EnterLevelAreaCoroutine(375350,261712,338930,-660641889 ,376027),
                        new EnterLevelAreaCoroutine(375350,261712,338930,-660641889 ,0),

                        new EnterLevelAreaCoroutine(375350,338930,338968,2115491808,0),
                        new MoveToMapMarkerCoroutine(375350,338968,2912417),
                        new ClearAreaForNSecondsCoroutine(375350,45,0,0,60)

                    }
            });

            // A3 - Bounty: Sescheron's Defenders (436280) 4	0

            Bounties.Add(new BountyData
            {
                QuestId = 436280,
                Act = Act.A3,
                WorldId = 428493, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                //WaypointNumber = 40,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(436280, 428493, 2912417),
                    new MoveToActorCoroutine(428493, 428493,435703),
                    // 435703 (435703) Distance: 4.695982
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    new MoveToActorCoroutine(428493, 428493,435703),
                    // 435703 (435703) Distance: 7.569592
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),
                    // 435720 (435720) Distance: 10.63434
                    new ClearAreaForNSecondsCoroutine(436280,10,0,0),
                    new MoveToActorCoroutine(436280, 428493, 435720),
                    // 435720 (435720) Distance: 10.63434
                    new InteractWithUnitCoroutine(436280, 428493, 435720, 0, 5),
                    new WaitCoroutine(1000),

                }
            });





            //// A3 - Bounty: King of the Ziggurat (432803) - 6	0

            //Bounties.Add(new BountyData
            //{
            //    QuestId = 432803,
            //    Act = Act.A3,
            //    WorldId = 428493, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 40,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(432803, 428493, 2912417),
            //        new MoveToSceneCoroutine(432803, 428493, "Ziggurat"),
            //        new ClearAreaForNSecondsCoroutine(432803, 70, 0, 0, 100),
            //        // 437935 (437935) Distance: 8.863441
            //        new MoveToActorCoroutine(432803, 428493, 437935),
            //        // 437935 (437935) Distance: 8.863441
            //        new InteractWithGizmoCoroutine(432803, 428493, 437935, 0, 5),
            //    }
            //});

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
                    // oldNecromancer (4798) Distance: 14.02715
                    new MoveToActorCoroutine(350562, 70885, 4798),
                    // oldNecromancer (4798) Distance: 14.02715
                    new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
                    new WaitCoroutine(5000),
                    // caOut_Totem_A (3707) Distance: 10.77662
                    new MoveToActorCoroutine(350562, 70885, 3707),
                    // caOut_Totem_A (3707) Distance: 10.77662
                    new InteractWithGizmoCoroutine(350562, 70885, 3707, 0, 5),
                    new WaitCoroutine(5000),
                    // caOut_Totem_A (3707) Distance: 10.77662
                    new MoveToActorCoroutine(350562, 70885, 3707),
                    // caOut_Totem_A (3707) Distance: 10.77662
                    new InteractWithGizmoCoroutine(350562, 70885, 3707, 0, 5),
                    new WaitCoroutine(5000),
                    // oldNecromancer (4798) Distance: 1.06224
                    new MoveToActorCoroutine(350562, 70885, 4798),
                    // oldNecromancer (4798) Distance: 1.06224
                    new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
                    new ClearAreaForNSecondsCoroutine(350562, 10, 0, 0, 45),
                    // oldNecromancer (4798) Distance: 1.06224
                    new MoveToActorCoroutine(350562, 70885, 4798),
                    // oldNecromancer (4798) Distance: 1.06224
                    new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
                    new ClearLevelAreaCoroutine(350562)
                }
            });


            //// A5 - Bounty: Noble Deaths (368536)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 368536,
            //    Act = Act.A5,
            //    WorldId = 336852, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(368536, 261712, 336852, -752748508, 333736),
            //        new MoveToPositionCoroutine(336852, new Vector3(407, 293, 7)),
            //        new InteractWithGizmoCoroutine(368536, 336852,273323,0,3),
            //        // x1_NPC_Westmarch_Gorrel_NonUnique (357018) Distance: 5.344718
            //        new MoveToActorCoroutine(368536, 336852, 357018),
            //        // x1_NPC_Westmarch_Gorrel_NonUnique (357018) Distance: 5.344718
            //        new InteractWithUnitCoroutine(368536, 336852, 357018, 0, 5),
            //    }
            //});

            //// A2 - Bounty: Sardar's Treasure (347591)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 347591,
            //    Act = Act.A2,
            //    WorldId = 157882, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(347591, 70885, 922565181),
            //        // a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle (175603) Distance: 34.29427
            //        new MoveToActorCoroutine(347591, 70885, 175603),
            //        // a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle (175603) Distance: 19.30635
            //        new InteractWithGizmoCoroutine(347591, 70885, 175603, 922565181, 5),
            //        new EnterLevelAreaCoroutine(347591, 70885, 157882, 922565181, 175467),
            //        new MoveToPositionCoroutine(157882, new Vector3(443, 287, 8)),
            //        // a2dun_Aqd_Act_Lever_FacePuzzle_01 (219879) Distance: 6.683785
            //        new MoveToActorCoroutine(347591, 157882, 219879),
            //        // a2dun_Aqd_Act_Lever_FacePuzzle_01 (219879) Distance: 6.683785
            //        new InteractWithGizmoCoroutine(347591, 157882, 219879, 0, 5),
            //        new MoveToPositionCoroutine(157882, new Vector3(291, 289, -9)),
            //        // a2dun_Aqd_Chest_Rare_FacePuzzleSmall (190708) Distance: 12.5555
            //        new InteractWithGizmoCoroutine(347591, 157882, 190708, 0, 5),

            //        new ClearAreaForNSecondsCoroutine(347591, 30, 0, 0, 45),
            //    }
            //});

            //// A2 - Bounty: Prisoners of Kamyr (347595)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 347595,
            //    Act = Act.A2,
            //    WorldId = 70885, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(347595, 70885, 2912417),
            //        // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 59.82738
            //        new MoveToActorCoroutine(347595, 70885, 183609),
            //        // StakeA_caOut_Props_Guard (108874) Distance: 61.24287
            //        new ClearAreaForNSecondsCoroutine(347595,30,0,0,200,false)
            //    }
            //});



            //// A2 - Bounty: Lost Treasure of Khan Dakab (346067)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 346067,
            //    Act = Act.A2,
            //    WorldId = 158593, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(346067, 70885, 913850831),
            //        new MoveToSceneCoroutine(346067, 70885,"POI"),
            //        new MoveToActorCoroutine(346067, 70885,175603),
            //        new InteractWithGizmoCoroutine(346067, 70885,175603,0,5,2,5),
            //        new MoveToActorCoroutine(346067, 70885,175603),
            //        new InteractWithGizmoCoroutine(346067, 70885,175603,0,5,2,5),
            //        new MoveToActorCoroutine(346067, 70885,176002),
            //        new InteractWithGizmoCoroutine(346067,70885,176002,3),
            //        //new EnterLevelAreaCoroutine(346067, 70885, 0, 913850831, 176002),
            //        new MoveToPositionCoroutine(158593, new Vector3(628, 283, 0)),
            //        // a2dun_Aqd_Act_Lever_FacePuzzle_02 (219880) Distance: 6.749629
            //        new InteractWithGizmoCoroutine(346067, 158593, 219880, 0, 5),
            //        new MoveToPositionCoroutine(158593, new Vector3(290, 167, -9)),
            //        // a2dun_Aqd_Chest_Special_FacePuzzle_Large (190524) Distance: 10.15839
            //        new InteractWithGizmoCoroutine(346067, 158593, 190524, 0, 5),
            //        new ClearAreaForNSecondsCoroutine(346067, 15, 0, 0, 45),
            //    }
            //});

            //// A5 - Bounty: Finding the Forgotten (368611)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 368611,
            //    Act = Act.A5,
            //    WorldId = 330761, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(368611, 263494, 330761, 683724333, 333736),
            //        new MoveToMapMarkerCoroutine(368611, 330761, 2912417),
            //        new MoveToActorCoroutine(368611, 330761, 289249),
            //        new InteractWithGizmoCoroutine(368611, 330761, 289249,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309381),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309381,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309400),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309400,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309398),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309398,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309387),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309387,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309403),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309403,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309391),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309391,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309410),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309410,0,1,1,1),
            //        new MoveToActorCoroutine(368611, 330761, 309380),
            //        new InteractWithGizmoCoroutine(368611, 330761, 309380,0,1,1,1),
            //    }
            //});


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
                    new EnterLevelAreaCoroutine(361421, 121579, 214956, 1038619951, 225195),
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
                    new EnterLevelAreaCoroutine(361234, 50585, 73261, -267501088, 159573),
                    new MoveToPositionCoroutine(73261, new Vector3(336, 260, 20)),
                    new InteractWithGizmoCoroutine(361234,73261,175181,0,3),
                    new MoveToPositionCoroutine(73261, new Vector3(338, 288, 15)),
                    new WaitCoroutine(20000),
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
               //WaypointNumber = 60,
               Coroutines = new List<ISubroutine>
               {
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
                    new EnterLevelAreaCoroutine(349288, 129305, 205399, -72895048, 210763),
                    new EnterLevelAreaCoroutine(349288, 205399, 109561, -753198453, 161279),

                    new MoveToScenePositionCoroutine(349288, 109561, "a4dun_Diablo_Arena_E04_S04", new Vector3(30.63171f, 29.09433f, 0.1000014f)),

                    new MoveToSceneCoroutine(349288, 109561, "a4dun_Diablo_Arena_E03_S03"),

                    new MoveToScenePositionCoroutine(349288, 109561, "a4dun_Diablo_Arena_E03_S03", new Vector3(195.8414f, 194.9717f, 40.1f)),

                    new MoveToScenePositionCoroutine(349288, 109561, "a4dun_Diablo_Arena_E03_S03", new Vector3(151.3365f, 147.0149f, 40.61041f)),

                    new MoveToPositionCoroutine(109561, new Vector3(678, 677, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(645, 644, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(623, 622, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(602, 600, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(579, 576, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(562, 560, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(543, 541, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(530, 528, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(514, 511, 0)),
                    new MoveToPositionCoroutine(109561, new Vector3(501, 501, 1)),
                    new MoveToPositionCoroutine(109561, new Vector3(492, 489, 10)),
                    new MoveToPositionCoroutine(109561, new Vector3(481, 479, 19)),
                    new MoveToPositionCoroutine(109561, new Vector3(468, 468, 28)),
                    new MoveToPositionCoroutine(109561, new Vector3(456, 453, 39)),
                    new MoveToPositionCoroutine(109561, new Vector3(428, 428, 40)),
                    new MoveToPositionCoroutine(109561, new Vector3(376, 375, 40)),
                    new WaitCoroutine(150000),
                    new MoveToPositionCoroutine(196292, new Vector3(354, 353, 40)),
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
                        new EnterLevelAreaCoroutine (347558, 70885, 195200, 1742967132, 195234),
                        new MoveToPositionCoroutine(195200, new Vector3(216, 196, 0)),
                        new KillUniqueMonsterCoroutine (347558, 195200, 6031, 0),
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
                        new EnterLevelAreaCoroutine (349262, 109513, 166640, 739323140 , 161276),
                        new MoveToPositionCoroutine(166640, new Vector3(351, 443, 0)),
                        new KillUniqueMonsterCoroutine (349262, 166640, 4630, 0),
                    }
            });
        }

        private static void AddGuardedGizmoBounties()
        {


            //A1 - Wortham Survivors
            Bounties.Add(new BountyData
            {
                QuestId = 434378,
                Act = Act.A1,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(434378,434366)
                }
            });


            //A1 - Templar Inquisition
            Bounties.Add(new BountyData
            {
                QuestId = 430723,
                Act = Act.A1,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(430723,430733)
                }
            });


            //A1 - The Triune Reborn
            Bounties.Add(new BountyData
            {
                QuestId = 432293,
                Act = Act.A1,
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(432293,432259)
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



            //A2 - Ancient Devices
            Bounties.Add(new BountyData
            {
                QuestId = 433025,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433025, 432885)
                }
            });


            //A3 - The Lost Patrol
            Bounties.Add(new BountyData
            {
                QuestId = 433217,
                Act = Act.A3,
                WorldId = 95804,
                WaypointLevelAreaId = 112565,
                QuestType = BountyQuestType.GuardedGizmo,
                Coroutines = new List<ISubroutine>
                {
                    new GuardedGizmoCoroutine(433217,433184)
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
                WorldId = 0,
                QuestType = BountyQuestType.GuardedGizmo,
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
            RemoveCustomBounties(347598, 346086, 346188, 369271);

            //// A2 - Bounty: The Guardian Spirits (350560)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 350560,
            //    Act = Act.A2,
            //    WorldId = 70885, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 21,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        //World: caOUT_Town, Id: 70885, AnnId: 1999568897, IsGenerated: False
            //        //Scene: caOut_Sub240x240_Tower_Ruin, SnoId: 31623,
            //        //LevelArea: A2_caOUT_StingingWinds, Id: 19839

            //        new MoveToMapMarkerCoroutine(350560, 70885, 2912417),

            //        new MoveToSceneCoroutine(350560, 70885, "caOut_StingingWinds_E05_S07"),

            //        //// A2C2DyingGhostGuy (51293) Distance: 4.714807
            //        //new InteractWithUnitCoroutine(350560, 70885, 51293, 2912417, 5),
            //        //new WaitCoroutine(5000),
            //        //new MoveToScenePositionCoroutine(350560, 70885, null, new Vector3(98.66846f, 108.3327f, 175.6606f)),
            //        //// GhostTotem (59436) Distance: 4.005692
            //        //new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
            //        //new MoveToScenePositionCoroutine(350560, 70885, null, new Vector3(156.0665f, 112.9069f, 175.5679f)),
            //        //// GhostTotem (59436) Distance: 6.767324
            //        //new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
            //        //new MoveToScenePositionCoroutine(350560, 70885, null, new Vector3(134.1381f, 139.6368f, 175.9637f)),
            //        new WaitCoroutine(1500),
            //        // A2C2DyingGhostGuy (51293) Distance: 8.124561
            //        new InteractWithUnitCoroutine(350560, 70885, 51293, 2912417, 5),
            //        // GhostTotem (59436) Distance: 6.767324
            //        new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
            //        new WaitCoroutine(3000),
            //        // GhostTotem (59436) Distance: 4.005692
            //        new InteractWithGizmoCoroutine(350560, 70885, 59436, 0, 5),
            //        new WaitCoroutine(3000),
            //        new ClearAreaForNSecondsCoroutine(350560, 20, 0, 0, 45),
            //    }
            //});

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
                    new EnterLevelAreaCoroutine(369348, 267412, 234962, 1022651488,0),
                    new EnterLevelAreaCoroutine(369348, 234962, 336572, 1270943969, 0),
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
                    new EnterLevelAreaCoroutine(369377, 261712, 338930, -660641889, 376027),
                    new EnterLevelAreaCoroutine(369377, 338930, 338968, 2115491808, 0),
                    //new EnterLevelAreaCoroutine(369377, 338930, 338968, 2115491808, 175482),
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
                    new MoveToScenePositionCoroutine(436284, 428493, "Guardians", new Vector3(100.5087f, 88.13977f, -11.82213f)),
                    // 437152 (437152) Distance: 5.283126
                    new ClearAreaForNSecondsCoroutine(436284,60,437152,0,60),
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
                    new EnterLevelAreaCoroutine(355276, 71150, 81163, 925091454, 175501),
                    new EnterLevelAreaCoroutine(355276, 81163, 81164, 925091455, 176038),
                    new MoveToMapMarkerCoroutine(355276, 81164, 1582533030),
                    new ClearAreaForNSecondsCoroutine(355276, 20, 0, 0, 45),
                    new ClearLevelAreaCoroutine(355276),
                }
            });




            //// A5 - Bounty: Demon Souls (363409)
            /// // Death orbs are Monster/Unit that are not hostile, possibly need interact. trinity doesnt know what to do with them.
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 363409,
            //    Act = Act.A5,
            //    WorldId = 271233, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 56,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(363409, 271233, 2912417),
            //        // x1_Fortress_Event_Worldstone_Jamella (334487) Distance: 27.00935
            //        new InteractWithUnitCoroutine(363409, 271233, 334487, 2912417, 5),
            //        new MoveToScenePositionCoroutine(363409, 271233, "Worldstone", new Vector3(54.08716f, 149.2113f, 0.471462f)),
                    
            //        ////ActorId: 334466, Type: Monster, Name: x1_Death_Orb_Little_Event_Worldstone-4542, Distance2d: 13.59136, 
            //        //new InteractWithUnitCoroutine(363409, 271233, 334466, 2912417, 5),

            //        new ClearAreaForNSecondsCoroutine(363409,20,334466,0),
            //        new MoveToScenePositionCoroutine(363409, 271233, "Worldstone", new Vector3(161.1299f, 46.66309f, 0.471462f)),
            //        new ClearAreaForNSecondsCoroutine(363409,20,334466,0),
            //        new MoveToScenePositionCoroutine(363409, 271233, "Worldstone", new Vector3(76.70563f, 60.66968f, 0.4456833f)),
            //        new ClearAreaForNSecondsCoroutine(363409,20,334466,0),
            //        new MoveToScenePositionCoroutine(363409, 271233, "x1_fortress_SE_05_Worldstone", new Vector3(185.5141f, 196.046f, 0.1f)),
                    
            //       // //ActorId: 334466, Type: Monster, Name: x1_Death_Orb_Little_Event_Worldstone-4542, Distance2d: 13.59136, 
            //       // new InteractWithUnitCoroutine(363409, 271233, 334466, 2912417, 5),

            //       ////ActorId: 334466, Type: Monster, Name: x1_Death_Orb_Little_Event_Worldstone-4542, Distance2d: 13.59136, 
            //       // new InteractWithUnitCoroutine(363409, 271233, 334466, 2912417, 5),

            //        // x1_Fortress_Event_Worldstone_Jamella (334487) Distance: 3.279212
            //        new InteractWithUnitCoroutine(363409, 271233, 334487, 0, 5),
            //    }
            //});

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
                    new EnterLevelAreaCoroutine(347092, 71150, 154587, -1861222194, 176002),
                    new MoveToMapMarkerCoroutine(347092, 154587, 1321851756),
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
                    new MoveToMapMarkerCoroutine(363407, 271233, 2912417),
                    new ClearAreaForNSecondsCoroutine(363407, 60, 0, 0, 100),
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
                    new ClearAreaForNSecondsCoroutine(359403,40,0,0)
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
                    new ClearAreaForNSecondsCoroutine(363431, 20, 0, 0, 45),
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
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(345976, 70885, 689915896),
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
                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(100.6373f, 92.96954f, 0.2542905f)),
                    new ClearAreaForNSecondsCoroutine(359907, 60, 0, 0, 45),
                    new MoveToScenePositionCoroutine(359907, 283566, "x1_Catacombs_SEW_06_garden", new Vector3(59.00961f, 90.45087f, 0.4674271f)),
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
                    new EnterLevelAreaCoroutine(369383, 263494, 338976, -660641888, 329025),
                    new EnterLevelAreaCoroutine(369383, 338976, 338977, 2115492897, 0),
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
                    new EnterLevelAreaCoroutine(368433, 261712, 246369, 2043324508, 333736),
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
                    new ClearAreaForNSecondsCoroutine(363405, 60, 0, 0, 45),
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
                    new EnterLevelAreaCoroutine(347090, 71150, 154587, -1861222194, 176002),
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
                    //new EnterLevelAreaCoroutine(367926, 338600, 357658, -1551729967, 176002),
                    // portal id changed?
                    new EnterLevelAreaCoroutine(367926, 338600, 0, -1551729967, new int[]{ 176004, 176002 }),
                    // g_Portal_ArchTall_Blue (176002) Distance: 4.875014

                    //new InteractWithGizmoCoroutine(367926, 338600, 176002, -1551729967, 5),

                    new MoveToPositionCoroutine(357658, new Vector3(187, 118, 3)),
                    new MoveToPositionCoroutine(357658, new Vector3(128, 109, 0)),
                    new MoveToPositionCoroutine(357658, new Vector3(67, 78, 0)),
                    new MoveToPositionCoroutine(357658, new Vector3(53, 123, 0)),
                    new MoveToPositionCoroutine(357658, new Vector3(95, 150, 0)),
                    new MoveToPositionCoroutine(357658, new Vector3(38, 169, 0)),
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
                    new MoveToPositionCoroutine(294633, new Vector3(435, 340, 0)),
                    new MoveToPositionCoroutine(294633, new Vector3(395, 358, 0)),
                    new MoveToPositionCoroutine(294633, new Vector3(399, 302, 0)),
                    new MoveToPositionCoroutine(294633, new Vector3(352, 306, 10)),
                    new MoveToPositionCoroutine(294633, new Vector3(355, 355, 20)),
                    // x1_SurvivorCaptain_Rescue_Guards (295471) Distance: 3.254871
                    new InteractWithUnitCoroutine(368525, 294633, 295471, 0, 5),
                    new MoveToPositionCoroutine(294633, new Vector3(309, 347, 20)),
                    new MoveToPositionCoroutine(294633, new Vector3(300, 294, 20)),

                    new MoveToPositionCoroutine(294633, new Vector3(400, 430, 0)),

                    new MoveToPositionCoroutine(294633, new Vector3(448, 434, 0)),
                    new MoveToPositionCoroutine(294633, new Vector3(435, 340, 0)),
                    new MoveToPositionCoroutine(294633, new Vector3(395, 358, 0)),
                    new MoveToPositionCoroutine(294633, new Vector3(399, 302, 0)),
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
                    new WaitCoroutine(20000),
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
                    new EnterLevelAreaCoroutine(367872, 267412, 271533, -1947203375, 359453),
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
                    new MoveToMapMarkerCoroutine(347030, 94676, 1917087943),
                    new ClearAreaForNSecondsCoroutine(347030, 10, 0, 1917087943, 45),

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
                    new MoveToActorCoroutine(347027, 58983, 357509),
                    // a1dun_Leor_Chest_Rare_Garrach (357509) Distance: 78.48354
                    new InteractWithGizmoCoroutine(347027, 58983, 357509, -1229363584, 5),
                    new WaitCoroutine(20000),
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
                        new MoveToPositionCoroutine(310845, new Vector3(763, 140, -9)),
                        new ClearAreaForNSecondsCoroutine (368559, 15, 0, 2912417),
                        // x1_BogCellar_TriuneCultist (247595) Distance: 1.502872
                        //new InteractWithUnitCoroutine(368559, 310845, 247595, 0, 5),
                        //new WaitCoroutine(10000),
                        new MoveToPositionCoroutine(310845, new Vector3(747, 40, 0)),
                        // g_Portal_Rectangle_Orange_IconDoor (178293) Distance: 5.280013
                        new InteractWithGizmoCoroutine(368559, 310845, 178293, 798857082, 5),
                        new MoveToPositionCoroutine(310845, new Vector3(453, 127, 10)),
                        new MoveToPositionCoroutine(310845, new Vector3(366, 125, 15)),
                        new MoveToPositionCoroutine(310845, new Vector3(374, 192, 10)),
                        new MoveToPositionCoroutine(310845, new Vector3(286, 202, 15)),
                        new MoveToPositionCoroutine(310845, new Vector3(301, 119, 15)),
                        // X1_Westm_Door_Hidden_Bookshelf (316627) Distance: 6.397694
                        new InteractWithGizmoCoroutine(368559, 310845, 316627, 0, 5),
                        new MoveToPositionCoroutine(310845, new Vector3(318, 56, 15)),
                        new WaitCoroutine(5000),
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
                    new EnterLevelAreaCoroutine(349020, 71150, 72636, -1965109038, 176002),
                    new MoveToSceneCoroutine(349020,72636,"dead"),
                    //new MoveToMapMarkerCoroutine(349020, 72636, 2912417),
                    new WaitCoroutine(4000),
                    // CryingGhost_Female_01_A (3892) Distance: 42.10099
                    new InteractWithUnitCoroutine(349020, 72636, 3892, 2912417, 5),
                    // a1dun_Crypts_Bowl_of_Bones_01 (102079) Distance: 141.0878
                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(84.57935f, 52.89191f, 0.1f)),
                    // a1dun_Crypts_Bowl_of_Bones_01 (102079) Distance: 6.513741
                    new InteractWithGizmoCoroutine(349020, 72636, 102079, 0, 5),
                    // a1dun_Crypts_Bowl_of_Bones_03 (174754) Distance: 5.019573
                    new MoveToSceneCoroutine(349020,72636,"dead"),
                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(50.32025f, 116.8566f, 0.09999999f)),
                    // a1dun_Crypts_Bowl_of_Bones_02 (174753) Distance: 4.810955
                    new InteractWithGizmoCoroutine(349020, 72636, 174753, 0, 5),
                    // a1dun_Crypts_Bowl_of_Bones_02 (174753) Distance: 85.87789
                    new MoveToSceneCoroutine(349020,72636,"dead"),
                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(85.17566f, 187.389f, 0.1f)),
                    // a1dun_Crypts_Bowl_of_Bones_03 (174754) Distance: 4.641604
                    new InteractWithGizmoCoroutine(349020, 72636, 174754, 0, 5),
                    new MoveToSceneCoroutine(349020,72636,"dead"),
                    new ClearAreaForNSecondsCoroutine(349020, 10, 0, 0, 100),
                    // a1dun_Crypts_Dual_Sarcophagus (105754) Distance: 17.6851
                    new MoveToScenePositionCoroutine(349020, 72636, "trDun_Crypt_EW_Hall_1000dead_01", new Vector3(209.656f, 121.3727f, 0.1f)),
                    // a1dun_Crypts_Dual_Sarcophagus (105754) Distance: 17.6851
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
                    new MoveToMapMarkerCoroutine(347638,70885, -1483215209),
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
                    new WaitCoroutine(5000),
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
                    new MoveToMapMarkerCoroutine(344486, 71150, 2912417),
                    // Gharbad_The_Weak_Ghost (81068) Distance: 20.89377
                    new MoveToActorCoroutine(344486, 71150, 81068),
                    // Gharbad_The_Weak_Ghost (81068) Distance: 20.89377
                    new InteractWithUnitCoroutine(344486, 71150, 81068, 2912417, 5),
                    new WaitCoroutine(5000),
                    new MoveToScenePositionCoroutine(344486, 71150, "trOut_Highlands_Sub240_POI", new Vector3(122.2505f, 165.7817f, -28.86722f)),
                    new MoveToScenePositionCoroutine(344486, 71150, "trOut_Highlands_Sub240_POI", new Vector3(118.2659f, 99.68457f, -28.36605f)),
                    new ClearAreaForNSecondsCoroutine(344486, 20, 96582, 0, 45),
                    new MoveToScenePositionCoroutine(344486, 71150, "trOut_Highlands_Sub240_POI", new Vector3(137.4558f, 130.5591f, -28.34882f)),
                    new MoveToActorCoroutine(344486, 71150, 81068),
                    // Gharbad_The_Weak_Ghost (81068) Distance: 8.892706
                    new InteractWithUnitCoroutine(344486, 71150, 81068, 0, 5),
                    new ClearAreaForNSecondsCoroutine(344486, 20, 96582, 0, 45),
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
                    new EnterLevelAreaCoroutine(347060, 71150, 82370, -431250552, 176038),
                    new EnterLevelAreaCoroutine(347060, 82370, 82371, -431250551, 176038, true),
                    // A1_UniqueVendor_Miner (107076) Distance: 0.3106932
                    new MoveToActorCoroutine(347060, 82371, 107076),
                    // A1_UniqueVendor_Miner (107076) Distance: 0.3106932
                    new InteractWithUnitCoroutine(347060, 82371, 107076, -431250552, 5),
                    // a1dun_caves_Rocks_GoldOre (204032) Distance: 19.51709
                    new MoveToActorCoroutine(347060, 82371, 204032),
                    // a1dun_caves_Rocks_GoldOre (204032) Distance: 19.51709
                    new InteractWithGizmoCoroutine(347060, 82371, 204032, 0, 5),
                    // a1duncave_props_crystal_cluster_A (202277) Distance: 20.88908
                    new MoveToActorCoroutine(347060, 82371, 202277),
                    // a1duncave_props_crystal_cluster_A (202277) Distance: 20.88908
                    new InteractWithGizmoCoroutine(347060, 82371, 202277, 0, 5),
                    new WaitCoroutine(3000),
                    // A1_UniqueVendor_Miner (107076) Distance: 14.53862
                    new MoveToActorCoroutine(347060, 82371, 107076),
                    // Could not detect an active quest gizmo, you must be out of range.
                    // A1_UniqueVendor_Miner (107076) Distance: 14.53862
                    new InteractWithUnitCoroutine(347060, 82371, 107076, 0, 5),
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
                    new ClearAreaForNSecondsCoroutine(363342, 60, 0, -2147483648 , 80,false),
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

            // A5 - Bounty: The True Son of the Wolf (368601)
            Bounties.Add(new BountyData
            {
                QuestId = 368601,
                Act = Act.A5,
                WorldId = 336902, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368601, 263494, 336902, -752748507, 333736),
                    new MoveToPositionCoroutine(336902, new Vector3(311, 393, 10)),
                    // x1_NPC_Westmarch_Aldritch (336711) Distance: 9.175469
                    new InteractWithUnitCoroutine(368601, 336902, 336711, 0, 5),
                    new ClearAreaForNSecondsCoroutine(368601, 30, 336711, 0, 120),
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
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(17.71436f, 173.9963f, 0.1487222f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(114.0653f, 114.5838f, -17.4f)),
                    new ClearAreaForNSecondsCoroutine(363346, 60, 0, 0, 150,false),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(195.6577f, 2.806244f, 0.09999999f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(130.1381f, 87.56168f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(142.9594f, 124.983f, -17.4f)),
                    new MoveToScenePositionCoroutine(363346, 338600, "x1_Pand_Ext_240_NSEW_Event_RockHive_01", new Vector3(111.6299f, 118.7759f, -17.4f)),
                    new ClearAreaForNSecondsCoroutine(363346, 60, 0, 0, 150,false),

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
                    new ClearAreaForNSecondsCoroutine(359399, 60, 4675, 0, 45,false),
                }
            });

            // A4 - Bounty: Watch Your Step (409895)
            Bounties.Add(new BountyData
            {
                QuestId = 409895,
                Act = Act.A4,
                WorldId = 140709, // Enter the final worldId here
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(409895, 409511, 140709, -970799630, 204183),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 415, 0)),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 385, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 385, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 385, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 385, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 385, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 385, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 351, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 351, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 351, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 351, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 351, 0)),
                    new WaitCoroutine(1000),

                    new MoveToPositionCoroutine(140709, new Vector3(272, 323, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 323, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 323, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 323, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(272, 323, 0)),
                    new WaitCoroutine(1000),

                    new MoveToPositionCoroutine(140709, new Vector3(273, 296, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 296, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 296, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 296, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(273, 296, 0)),
                    new WaitCoroutine(1000),
                   
                    new MoveToPositionCoroutine(140709, new Vector3(276, 281, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(276, 281, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(276, 281, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(276, 281, 0)),
                    new WaitCoroutine(1000),
                    new MoveToPositionCoroutine(140709, new Vector3(276, 281, 0)),
                    new WaitCoroutine(1000),

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
                    new ClearAreaForNSecondsCoroutine(368420, 20, 0, 0, 200),

                    new MoveToScenePositionCoroutine(368420, 303361, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(130.7036f, 177.8941f, 0.1f)),
                    new MoveToScenePositionCoroutine(368420, 303361, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(89.54332f, 182.3007f, 0.1000009f)),
                    new MoveToScenePositionCoroutine(368420, 303361, "x1_westm_Int_Gen_A_01_Necromancer", new Vector3(141.7386f, 143.7506f, 0.1f)),


                    // oldNecromancer (4798) Distance: 27.00111
                    new MoveToActorCoroutine(368420, 303361, 4798),
                    // oldNecromancer (4798) Distance: 27.00111
                    new InteractWithUnitCoroutine(368420, 303361, 4798, 0, 5),
                    new WaitCoroutine(15000),
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
                    new MoveToMapMarkerCoroutine(433017, 432993, 2912417),
                    //new MoveToMapMarkerCoroutine(433017, 432993, 2912417),
                    new ClearAreaForNSecondsCoroutine(433017, 20, 121821, 0, 45),
                    // a2dun_Aqd_Mummy_Spawner_Muck (121821) Distance: 9.79975
                    //new MoveToActorCoroutine(433017, 432993, 121821),
                    new InteractWithGizmoCoroutine(433017, 432993, 121821,0,5),
                    new WaitCoroutine(2000),
 
                    // a2dun_Aqd_Mummy_Spawner_Muck (121821) Distance: 16.20963
                    //new MoveToActorCoroutine(433017, 432993, 121821),
                    new InteractWithGizmoCoroutine(433017, 432993, 121821,0,5),
                    new WaitCoroutine(2000),
 
                    // a2dun_Aqd_Mummy_Spawner_Muck (121821) Distance: 17.57193
                    //new MoveToActorCoroutine(433017, 432993, 121821),
                    new InteractWithGizmoCoroutine(433017, 432993, 121821,0,5),
                    new WaitCoroutine(2000),
 
                    // a2dun_Aqd_Mummy_Spawner_Muck (121821) Distance: 28.6147
                    //new MoveToActorCoroutine(433017, 432993, 121821),
                    new InteractWithGizmoCoroutine(433017, 432993, 121821,0,5),
                    new WaitCoroutine(2000),
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
                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(95.08228f, 73.23065f, 0.1f)),
                    new MoveToActorCoroutine(345500, 71150, 81980),
 
                    // NPC_Human_Male_Event_FarmAmbush (81980) Distance: 59.84805
                    new InteractWithUnitCoroutine(345500, 71150, 81980, 0, 5),


                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(93.28857f, 187.4562f, 0.1f)),

                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(129.1501f, 166.2041f, 0.1f)),

                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(194.272f, 178.9236f, 0.1f)),

                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(177.3428f, 106.7409f, 0.1f)),

                    new MoveToScenePositionCoroutine(345500, 71150, "trOut_TristramFields_Sub240_POI_01", new Vector3(92.67603f, 78.44818f, 0.1f)),
                    // NPC_Human_Male_Event_FarmAmbush (81980) Distance: 5.45416
                    new InteractWithUnitCoroutine(345500, 71150, 81980, 0, 5),

                    new ClearAreaForNSecondsCoroutine(345500,30,0,0,200,false),
                    // NPC_Human_Male_Event_FarmAmbush (81980) Distance: 36.39687
                    new MoveToActorCoroutine(345500, 71150, 81980),
 
                    // NPC_Human_Male_Event_FarmAmbush (81980) Distance: 36.39687
                    new InteractWithUnitCoroutine(345500, 71150, 81980, 0, 5),                }
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
                    new MoveToPositionCoroutine(71150, new Vector3(1443, 4034, 38)),
                    new MoveToPositionCoroutine(71150, new Vector3(1274, 3892, 78)),
                    new EnterLevelAreaCoroutine(350529, 71150, 132995, 853662530, 0),
                    // Event_VendorRescue_Vendor (129782) Distance: 13.24948
                    new MoveToActorCoroutine(350529, 132995, 129782),
                    // Event_VendorRescue_Vendor (129782) Distance: 13.24948
                    new InteractWithUnitCoroutine(350529, 132995, 129782, -1472187117, 5),
                    //ActorId: 136009, Type: Monster, Name: Event_VendorRescue_Brother-2746, Distance2d: 2.170177, CollisionRadius: 0, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 
                    new MoveToActorCoroutine(350529, 132995,136009),
                    new ClearAreaForNSecondsCoroutine(350529, 10, 136009, 0, 70,false),
                    // Event_VendorRescue_Vendor (129782) Distance: 11.94286
                    new MoveToActorCoroutine(350529, 132995, 129782),
                    // Event_VendorRescue_Vendor (129782) Distance: 11.94286
                    new InteractWithUnitCoroutine(350529, 132995, 129782, 0, 5),
                    new WaitCoroutine(10000),
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

                    new MoveToActorCoroutine(349196,221749,118261),
                    // FallenGrunt_C_RescueEscort_Unique (260230) Distance: 7.861986
                    //new MoveToActorCoroutine(349196, 221749, 260230),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (174995) Distance: 11.26799
                    new MoveToActorCoroutine(349196, 221749, 174995),
                    // bastionsKeepGuard_Melee_A_02_NPC_RescueEscort (174995) Distance: 11.26799
                    new InteractWithUnitCoroutine(349196, 221749, 174995, 0, 5),
                    new WaitCoroutine(10000),
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
                    new MoveToMapMarkerCoroutine(346184, 95804, 2912417),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (152145) Distance: 52.0904
                    new MoveToActorCoroutine(346184, 95804, 152145),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (152145) Distance: 52.0904
                    new InteractWithUnitCoroutine(346184, 95804, 152145, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(346184, 60, 152145, 0, 120,false),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (152145) Distance: 22.61426
                    new MoveToActorCoroutine(346184, 95804, 152145),
                    // bastionsKeepGuard_Melee_A_02_BlazeOfGlory (152145) Distance: 22.61426
                    new InteractWithUnitCoroutine(346184, 95804, 152145, 0, 5),
                    new WaitCoroutine(15000),
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
                    new MoveToActorCoroutine(349016, 72636,93713),
                    new InteractWithGizmoCoroutine(349016, 72636, 93713,2912417,5),
                    new ClearAreaForNSecondsCoroutine(349016, 65, 93713, 2912417, 120),
                    // a1dun_Crypts_Jar_of_Souls_02 (219334) Distance: 4.83636
                    new MoveToActorCoroutine(349016, 72636, 219334),
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
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                {
                    new EnterLevelAreaCoroutine(368532, 261712, 336844, -752748509, 333736),
                    new MoveToPositionCoroutine(336844, new Vector3(304, 424, 10)),
                    new MoveToPositionCoroutine(336844, new Vector3(281, 284, 15)),
                    new MoveToPositionCoroutine(336844, new Vector3(421, 262, 25)),
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
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(346180, 95804, 2912417),
                    // bastionsKeepGuard_Melee_A_01_snatched (205312) Distance: 34.26495
                    new MoveToActorCoroutine(346180, 95804, 205312),
                    // bastionsKeepGuard_Melee_A_01_snatched (205312) Distance: 34.26495
                    new InteractWithUnitCoroutine(346180, 95804, 205312, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(346180, 40, 206088, 0, 100, false),
                    // OmniNPC_Female_Act3_B_MedicalCamp (205468) Distance: 24.66534
                    new MoveToActorCoroutine(346180, 95804, 205468),
                    // OmniNPC_Female_Act3_B_MedicalCamp (205468) Distance: 24.66534
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

                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(237.9481f, 218.7702f, 12.86322f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(188.0312f, 80.27813f, 13.02337f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(166.8428f, 171.1376f, -0.8271087f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(124.2203f, 208.412f, -0.8271091f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(147.923f, 87.49158f, -0.811231f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(138.6424f, 77.63069f, -0.8271122f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(173.1153f, 185.4816f, -0.8271179f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(227.2027f, 174.8042f, 13.07317f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(169.9771f, 165.031f, -0.8271087f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(145.8862f, 182.5604f, -0.8271087f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(147.9561f, 82.96754f, -0.7381248f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(195.4805f, 93.29275f, 12.89117f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(165.5553f, 161.1067f, -0.8271087f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(165.5553f, 161.1067f, -0.8271087f)),
                    new MoveToScenePositionCoroutine(347520, 60838, "caOut_Interior_G_x01_y01", new Vector3(161.0718f, 123.2829f, -0.8271011f)),


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
                    new MoveToActorCoroutine(345546, 58982, 218071),
                    // a1dun_Leoric_IronMaiden_Event (221574) Distance: 5.864555
                    new ClearAreaForNSecondsCoroutine(345546,30,218071,0,60),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (218071) Distance: 1.570357
                    new MoveToActorCoroutine(345546, 58982, 221574),
                    // a1dun_Leoric_IronMaiden_Event (221574) Distance: 10.85653
                    new InteractWithGizmoCoroutine(345546, 58982, 221574, 0, 5),
                    new WaitCoroutine(15000),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (218071) Distance: 4.789657
                    new MoveToActorCoroutine(345546, 58982, 218071),
                    // OmniNPC_Tristram_Male_Leoric_RescueEvent (218071) Distance: 4.789657
                    new InteractWithUnitCoroutine(345546, 58982, 218071, 0, 5),
                    new WaitCoroutine(25000),
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
                    new EnterLevelAreaCoroutine(368536, 261712, 336852, -752748508, 333736),
                    new MoveToPositionCoroutine(336852, new Vector3(407, 293, 7)),
                    new InteractWithGizmoCoroutine(368536, 336852,273323,0,3),
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

                    new MoveToSceneCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14"),
                    new MoveToScenePositionCoroutine(359112, 338944, "x1_westm_graveyard_NSEW_14", new Vector3(51.71503f, 80.73083f, 0.1f)),

                    // x1_Graveyard_GraveRobert (351621) Distance: 45.1498
                    new InteractWithUnitCoroutine(359112, 338944, 351621, 2912417, 5),
                    new ClearAreaForNSecondsCoroutine(359112, 45, 351621, 2912417, 45),
                    // x1_Graveyard_GraveRobert (351621) Distance: 13.97965
                    new MoveToActorCoroutine(359112, 332336, 351621),
                    // x1_Graveyard_GraveRobert (351621) Distance: 13.97965
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
                        new MoveToMapMarkerCoroutine(359280,267412,2912417),
                        new ClearAreaForNSecondsCoroutine(359280,60,288471,2912417,60),
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
                        new MoveToMapMarkerCoroutine(368912,338600,-702665403),
                        new WaitCoroutine(5000),
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
                        new MoveToPositionCoroutine(70885, new Vector3(2156, 1228, 207)),
                        new KillUniqueMonsterCoroutine(345973,70885,221402,-79527531),
                        new ClearLevelAreaCoroutine(345973)
                    }
            });

            //// A1 - Scavenged Scabbard (344488)
            /// // Fixed pathing to entrance but it doesnt attack the boss at the end. Trinity issue.
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 344488,
            //    Act = Act.A1,
            //    WorldId = 82313,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    Coroutines = new List<ISubroutine>
            //        {
            //            new MoveToPositionCoroutine(71150, new Vector3(2384, 4640, 0)),
            //            new MoveToPositionCoroutine(71150, new Vector3(2384, 4432, -1)),
            //            new MoveToPositionCoroutine(71150, new Vector3(2344, 4112, 0)),
            //            new MoveToPositionCoroutine(71150, new Vector3(2372, 3943, 0)),
            //            // g_Portal_Rectangle_Orange (175482) Distance: 35.20034
            //            new InteractWithGizmoCoroutine(344488, 71150, 175482, 497382903, 5),
 
            //            //Navigation appears to be busted on this level
            //            //new EnterLevelAreaCoroutine (344488, 71150, 82076, 497382903, 175482),

            //            new EnterLevelAreaCoroutine (344488, 82076, 82313, 497382904, 176001, true),
            //            new MoveToPositionCoroutine(82313, new Vector3(328, 519, 0)),
            //            new InteractWithUnitCoroutine(344488,82313,81609,0,3),
            //            new MoveToActorCoroutine(344488,82313,222404),
            //            new ClearAreaForNSecondsCoroutine(344488,10,222404,0,60,false),
            //            new InteractWithGizmoCoroutine(344488,82313,222404,0,3),
            //            new InteractWithUnitCoroutine(344488,82313,81609,0,3),

            //        }
            //});

            // A5 - Home Invasion (368555)
            Bounties.Add(new BountyData
            {
                QuestId = 368555,
                Act = Act.A5,
                WorldId = 351794,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (368555, 261712, 351794, 412504359 , 333736),
                        //new MoveToPositionCoroutine(351794, new Vector3(433, 349, 0)),
                        //new InteractWithGizmoCoroutine(368555,351794,328459,0,5),
                        new MoveToPositionCoroutine(351794, new Vector3(410, 335, 15)),
                        new MoveToPositionCoroutine(351794, new Vector3(396, 308, 15)),

                        new MoveToPositionCoroutine(351794, new Vector3(396, 308, 15)),

                        new MoveToPositionCoroutine(351794, new Vector3(364, 358, 0)),

                        new MoveToPositionCoroutine(351794, new Vector3(328, 345, 0)),

                        new MoveToPositionCoroutine(351794, new Vector3(281, 350, -9)),

                        new MoveToPositionCoroutine(351794, new Vector3(248, 339, -9)),


                        //new MoveToPositionCoroutine(351794, new Vector3(378, 374, 0)),
                        //new MoveToPositionCoroutine(351794, new Vector3(406, 338, 15)),
                        //new InteractWithGizmoCoroutine(368555,351794,273323,0,5),
                        //new MoveToPositionCoroutine(351794, new Vector3(393, 305, 15)),
                        //new ClearAreaForNSecondsCoroutine(368607,5,0,0,40,false),
                        //new MoveToPositionCoroutine(351794, new Vector3(369, 360, 0)),
                        //new MoveToPositionCoroutine(351794, new Vector3(260, 345, -9))
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
                        new MoveToMapMarkerCoroutine(345954,70885, 2912417, 2924),
                        new InteractWithUnitCoroutine(345954,70885,2924,2912417,5),
                        new ClearAreaForNSecondsCoroutine (345954, 60, 2924, 2912417, 20),

                        //new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(59.35184f, 54.93524f, 43.3257f)),
                        //new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(103.5425f, 13.15448f, 20.13758f)),
                        //new MoveToScenePositionCoroutine(345502, 71150, "trOut_FesteringWoods_Sub120_Generic_03", new Vector3(83.09296f, 101.742f, 21.35553f)),

                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(554.2419f, -335.7521f, 230.3514f)),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(596.1028f, -382.4034f, 230.3126f)),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(567.9431f, -433.8203f, 218.9766f)),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(646.894f, -428.8292f, 207.4076f)),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(623.7473f, -285.5134f, 207.4762f)),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(558.137f, -342.7196f, 230.3514f)),
                        new MoveToScenePositionCoroutine(345954, 70885, "caOut_Sub240x240_Mine_Destroyed", new Vector3(611.3267f, -378.5343f, 231.1981f)),

                        // a2dun_Aqd_Chest_Special_GreedyMiner (260238) Distance: 14.76077
                         new InteractWithGizmoCoroutine(345954,70885,260238,0,5)
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

                        new MoveToMapMarkerCoroutine(345502,71150,2912417, 102008),
                        new InteractWithGizmoCoroutine(345502,71150,102008,2912417,5),
                        new WaitCoroutine(10000),
                        //new ClearAreaForNSecondsCoroutine (345502, 60, 102008, 2912417),
                        new ClearAreaForNSecondsCoroutine(345502, 60, 0, 0, 45),
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
                        new MoveToMapMarkerCoroutine(374571,271235,2912417, 301177),
                        new MoveToActorCoroutine(374571,271235,301177),

                        // x1_PandExt_Time_Activator (301177) Distance: 25.83982
                        new InteractWithGizmoCoroutine(374571, 271235, 301177, 2912417, 5),

                        new WaitCoroutine(2000),
                        new ClearAreaForNSecondsCoroutine (374571, 15, 0, 0)
                    }
            });

            //// A5 - Kill Sartor (359543)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 433013,
            //    Act = Act.A5,
            //    WorldId = 338600,
            //    QuestType = BountyQuestType.KillMonster,
            //    Coroutines = new List<ISubroutine>
            //        {
            //            new KillUniqueMonsterCoroutine (359543, 222189, 255791851),
            //            new ClearLevelAreaCoroutine (359543)
            //        }
            //});

            // A3 - Forged in Battle (346146)
            Bounties.Add(new BountyData
            {
                QuestId = 346146,
                Act = Act.A3,
                WorldId = 93104,
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToActorCoroutine(346146,93104,149331),
                        new MoveToActorCoroutine(346146,93104,149331),
                        new InteractWithUnitCoroutine(346146,93104, 149331, 0, 5),
                        new ClearAreaForNSecondsCoroutine (346146, 60, 0, 0),
                        new MoveToActorCoroutine(346146,93104,149331),
                        new InteractWithUnitCoroutine(346146, 93104, 149331, 0, 5),
                        new WaitCoroutine(10000)
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
                QuestType = BountyQuestType.SpecialEvent,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine(409884, 409511, 139965, -970799631 , 204183),
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
                        new MoveToMapMarkerCoroutine(346182,95804, 2912417),
                        new ClearAreaForNSecondsCoroutine (346182, 40, 0, 0),
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
                        new MoveToPositionCoroutine(71150, new Vector3(1497, 4082, 40)),
                        new MoveToPositionCoroutine(71150, new Vector3(1428, 4011, 38)),
                        new MoveToPositionCoroutine(71150, new Vector3(1279, 3897, 78)),
                        new MoveToPositionCoroutine(71150, new Vector3(1090, 4052, 80)),
                        new MoveToPositionCoroutine(71150, new Vector3(1079, 3703, 78)),
                        new MoveToPositionCoroutine(71150, new Vector3(1081, 3521, 78)),
                        new EnterLevelAreaCoroutine (367559, 71150, 75049, -1019926638 , 176001),
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
                        new MoveToMapMarkerCoroutine(350564,2812, -1, 307),
                        new WaitCoroutine(2000),
                        new MoveToActorCoroutine(350564,2812, 307),
                        new InteractWithGizmoCoroutine(350564,2812, 307,0,5),
                        new WaitCoroutine(3000),
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
                        new MoveToMapMarkerCoroutine(345505, 71150, 2912417, 111907),
                        new InteractWithGizmoCoroutine(345505,71150, 111907, 0, 5),
                        new WaitCoroutine(10000),
                        new ClearAreaForNSecondsCoroutine (345505, 30, 111907, 0, 25),
                        new ClearAreaForNSecondsCoroutine (345505, 30, 111907, 0, 90),
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
                        new EnterLevelAreaCoroutine (347058,71150, 102299, 1070710595, 176001),
                        new EnterLevelAreaCoroutine (347058,102299, 165797, 1070710596, 176002 , true),
                        new MoveToActorCoroutine(347058, 165797, 76907),
                        new InteractWithUnitCoroutine(347058,165797, 76907, 0, 5),
                        new ClearAreaForNSecondsCoroutine (365401, 25, 76907, 0),
                        // FamilyTree_Daughter (76907) Distance: 2.111575
                        new MoveToActorCoroutine(347058, 165797, 76907),
                        // FamilyTree_Daughter (76907) Distance: 2.111575
                        new InteractWithUnitCoroutine(347058, 165797, 76907, 0, 5),
                    }
            });

            // A2 - Clear the Mysterious Cave (347598)
            Bounties.Add(new BountyData
            {
                QuestId = 347598,
                Act = Act.A2,
                WorldId = 194238,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
                        new MoveToMapMarkerCoroutine(347598,70885,-1615133822, 115928),
                        //new WaitCoroutine(3000),
                        //new ClearAreaForNSecondsCoroutine(347598,5,115928,1615133822,50,false),

                        new MoveToSceneCoroutine(347598, 70885, "caOut_Oasis_Sub240_POI_Edge"),

                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(58.08691f, 62.02783f, 97.96429f)),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(86.58594f, 95.48242f, 97.34129f)),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(102.3354f, 112.7808f, 97.34136f)),
                        new MoveToScenePositionCoroutine(347598, 70885, "caOut_Oasis_Edge_SE_01", new Vector3(117.9868f, 99.23682f, 97.34128f)),

                        new InteractWithUnitCoroutine(347598,70885,115928,-1615133822,5,1,10),
                        new WaitCoroutine(10000),
                        new EnterLevelAreaCoroutine (347598,70885, 169477, -1615133822, 176007),
                        new EnterLevelAreaCoroutine (347598,169477, 194238, 1109456219, 176002, true),
                        new ClearLevelAreaCoroutine (347598)
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
                        new MoveToPositionCoroutine(71150,new Vector3(1472, 4027, 38)),
                        new MoveToPositionCoroutine(71150, new Vector3(1282, 3897, 78)),
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
                        new EnterLevelAreaCoroutine (346086, 70885, 111670, -270313354, 206234),
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
                    // x1_Event_CursedShrine (364601) Distance: 22.65772
                    new InteractWithGizmoCoroutine(436267, 428493, 364601, 0, 5),
                    //ActorId: 364601, Type: Gizmo, Name: x1_Event_CursedShrine-36946, Distance2d: 4.163542, CollisionRadius: 10.04086, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0
                    new ClearAreaForNSecondsCoroutine(436267,60,364601,0,45),
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
                    new MoveToMapMarkerCoroutine(436269, 430335, 2912417,365097),
                    // x1_Global_Chest_CursedChest_B (365097) Distance: 27.16287
                    new InteractWithGizmoCoroutine(436269, 430335, 365097, 0, 5),
                    new ClearAreaForNSecondsCoroutine(436269, 60, 364559, 0, 45),
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
                        new EnterLevelAreaCoroutine (375264, 70885, 123183, -1758560943, 185067),
                        new MoveToMapMarkerCoroutine (375264,123183, 2912417 , 364601),
                        new InteractWithGizmoCoroutine (375264,123183, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (375264, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (365381,71150, 2912417, 365097),
                        new InteractWithGizmoCoroutine (365381,71150, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (365381, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (365401, 71150, 2912417, 364601),
                        new InteractWithGizmoCoroutine (365401,71150, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (365401, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369763, 71150, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369763,71150, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369763, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369789,58983, 2912417, 365097),
                        new InteractWithGizmoCoroutine (369789,58983, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (369789, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (369797,70885, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369797,70885, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369797, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369800, 70885, 2912417, 365097),
                        new InteractWithGizmoCoroutine (369800,70885, 365097, 2912417, 5),
                        new ClearAreaForNSecondsCoroutine (369800, 60, 364559, 2912417)
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
                        new EnterLevelAreaCoroutine (369813, 70885, 62569, 1352061373, 176003),
                        new MoveToMapMarkerCoroutine (369813,62569, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369813,62569, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369813, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369825,95804, 2912417, 365097),
                        new InteractWithGizmoCoroutine (369825,95804, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (369825, 60, 364559, 2912417)
                    }
            });

            // A3 - The Cursed Glacier (369851)
            Bounties.Add(new BountyData
            {
                QuestId = 369851,
                Act = Act.A3,
                WorldId = 189910,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (369851, 95804, 189910, 942020622, 176003),
                        new MoveToMapMarkerCoroutine (369851, 189910, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369851,189910, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369851, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369853,75434, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369853,75434, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369853, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369868,81934, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369868,81934, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369868, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369878, 129305, 2912417, 365097),
                        new InteractWithGizmoCoroutine (369878,129305, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (369878, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (369900,109513, 2912417, 368169),
                        new InteractWithGizmoCoroutine (369900,109513, 368169, 2912417),
                        new ClearAreaForNSecondsCoroutine (369900, 60, 368169, 2912417)
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
                        new MoveToMapMarkerCoroutine (369908,338600, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369908,338600, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369908, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (369944, 106752, 2912417, 365097),
                        new InteractWithGizmoCoroutine (369944,106752, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (369944, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (369952,341040, 2912417, 364601),
                        new InteractWithGizmoCoroutine (369952,341040, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (369952, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (375191,50582, 2912417, 365097),
                        new InteractWithGizmoCoroutine (375191,50582, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (375191, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (375198,50579, 2912417, 364601),
                        new InteractWithGizmoCoroutine (375198,50579, 364601, 2912417),
                        new ClearAreaForNSecondsCoroutine (375198, 60, 364601, 2912417)
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
                        new MoveToMapMarkerCoroutine (375201,180550, 2912417, 365097),
                        new InteractWithGizmoCoroutine (375201,180550, 365097, 2912417),
                    //new ClearAreaForNSecondsCoroutine (375201, 60, 364559, 2912417)
                    new ClearAreaForNSecondsCoroutine (375201, 60, 364559, 2912417,10)
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
                        new MoveToMapMarkerCoroutine (375257,50610, 2912417, 365097),

                        // x1_Global_Chest_CursedChest_B (365097) Distance: 8.065943 new MoveToSceneCoroutine(375257, 50610, "a2dun_Zolt_Hall_EW_02"),
                        new MoveToActorCoroutine(375257, 50610, 365097),

                        new InteractWithGizmoCoroutine (375257,50610, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (375257, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (375261,50611, 2912417, 365097),
                        new InteractWithGizmoCoroutine (375261,50611, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (375261, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (375268,261712, 2912417, 365097),
                        new MoveToSceneCoroutine(375268, 261712, "x1_westm_NSEW_06"),
                        new InteractWithGizmoCoroutine (375268,261712, 365097, 2912417),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(86.94189f, 81.26221f, 5.1f)),
                        new WaitCoroutine(2000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(82.91467f, 149.3269f, 5.1f)),
                        new WaitCoroutine(2000),
                        new MoveToSceneCoroutine(375268, 261712, "x1_westm_NSEW_06"),
                        new WaitCoroutine(2000),
                        new MoveToScenePositionCoroutine(375268, 261712, "x1_westm_NSEW_06", new Vector3(148.5044f, 72.07056f, 5.1f)),
                        new WaitCoroutine(2000),
                        new InteractWithGizmoCoroutine (375268,261712, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (375268, 60, 0, 0, 50, false),
                        new ClearLevelAreaCoroutine(375268),
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
                        new MoveToMapMarkerCoroutine (375275,271233, 2912417, 365097),
                        new InteractWithGizmoCoroutine (375275,271233, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (375275, 60, 364559, 2912417)
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
                        new MoveToMapMarkerCoroutine (375278,267412, 2912417, 365097),
                        new InteractWithGizmoCoroutine (375278,267412, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (375278, 60, 364559, 2912417)
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
                        new EnterLevelAreaCoroutine (375348, 338976, 338977, 2115492897, 175482, true),
                        new MoveToMapMarkerCoroutine (375348, 338977, 2912417, 365097),
                        new InteractWithGizmoCoroutine (375348,338977, 365097, 2912417),
                        new ClearAreaForNSecondsCoroutine (375348, 60, 364559, 2912417)
                    }
            });

            // A4 - The Cursed Pulpit (409897)
            Bounties.Add(new BountyData
            {
                QuestId = 409897,
                Act = Act.A4,
                WorldId = 181644,
                QuestType = BountyQuestType.ClearCurse,
                Coroutines = new List<ISubroutine>
                    {
                        new EnterLevelAreaCoroutine (409897, 409511, 181644, -970799629, 204183),
                        new MoveToActorCoroutine (409897,181644, 365097),
                        new InteractWithGizmoCoroutine (409897,181644, 365097,0,2),
                        new ClearAreaForNSecondsCoroutine (409897, 80, 364559,0,100)  // increased 
                    }
            });
        }

        private static void AddBounties()
        {

            // A1 - Clear the Cave of the Moon Clan (344547)
            Bounties.Add(new BountyData
            {
                QuestId = 344547,
                Act = Act.A1,
                WorldId = 82511,
                QuestType = BountyQuestType.ClearZone,
                Coroutines = new List<ISubroutine>
                    {
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
                        new EnterLevelAreaCoroutine (345496, 71150, 81163, 925091454, 175501),
                        new EnterLevelAreaCoroutine (345496, 81163, 81164, 925091455, 176038, true),
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

            // A2 - Kill Taros the Wild (345971)
            Bounties.Add(new BountyData
            {
                QuestId = 345971,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (345971,70885, 221372, -1258389667),
                        new ClearLevelAreaCoroutine (345971)
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
                        new EnterLevelAreaCoroutine (346069, 70885, 62568, 1352061372, 176003),
                        new EnterLevelAreaCoroutine (346069, 62568, 194240, 622615957, 176002, true),
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
                        new EnterLevelAreaCoroutine (346071, 70885, 62569, 1352061373, 176003),
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
                        new EnterLevelAreaCoroutine (346088, 70885, 111666, 1028158260, 175501),
                        new EnterLevelAreaCoroutine (346088, 111666, 218970, 1028158261, 176001, true),
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
                        new EnterLevelAreaCoroutine (346190, 95804, 189259, 1029056444, 176003),
                        new EnterLevelAreaCoroutine (346190, 189259, 221688, 151580180, 176038, true),
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
                        new EnterLevelAreaCoroutine (346192, 95804, 189910, 942020622, 176003),
                        new EnterLevelAreaCoroutine (346192, 189910, 221689, -802596186, 176038, true),
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
                        new EnterLevelAreaCoroutine (347065, 71150, 119888, 2036518712, 176003),
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
                        new EnterLevelAreaCoroutine (347097, 71150, 72637, -1965109037, 176002),
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
                        new EnterLevelAreaCoroutine (347525, 70885, 50589, 2000747858, 185067),
                        new EnterLevelAreaCoroutine (347525, 50589, 220804, 2108407595, 176001, true),
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
                //LevelAreaIds = new HashSet<int> { 19825 },
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
                        new MoveToPositionCoroutine(70885, new Vector3(1154, 1406, 184)),
                        new KillUniqueMonsterCoroutine (347569,70885, 222003, -640315117),
                        new ClearLevelAreaCoroutine (347569)
                    }
            });

            //// A2 - Clear the Mysterious Cave (347598)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 347598,
            //    Act = Act.A2,
            //    WorldId = 194238,
            //    QuestType = BountyQuestType.ClearZone,
            //    Coroutines = new List<ISubroutine>
            //        {
            //            new EnterLevelAreaCoroutine (347598, 70885, 169477, -1615133822, 176007),
            //            new EnterLevelAreaCoroutine (347598, 169477, 194238, 1109456219, 176002, true),
            //            new ClearLevelAreaCoroutine (347598)
            //        }
            //});

            // A2 - Kill Bashiok (347600)
            Bounties.Add(new BountyData
            {
                QuestId = 347600,
                Act = Act.A2,
                WorldId = 70885,
                QuestType = BountyQuestType.KillMonster,
                Coroutines = new List<ISubroutine>
                    {
                        new KillUniqueMonsterCoroutine (347600,70885, 215445, -1086742663),
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
                        new EnterLevelAreaCoroutine (349202, 174516, 221748, -1761785482, 175482, true),
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
                        new EnterLevelAreaCoroutine (350556, 70885, 50594, 151028377, 185067),
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
                        new KillUniqueMonsterCoroutine (361331,50579, 218314, -1923753193),
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
                        new KillUniqueMonsterCoroutine (362996,283552, 362299, -1650018965),
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
                        new KillUniqueMonsterCoroutine (363000,283552, 362303, -1650018964),
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
                Coroutines = new List<ISubroutine>
                    {

                        new MoveToPositionCoroutine(71150,new Vector3(1497, 4082, 40)),
                        new MoveToPositionCoroutine(71150,new Vector3(1428, 4011, 38)),
                        new MoveToPositionCoroutine(71150,new Vector3(1279, 3897, 78)),
                        new MoveToPositionCoroutine(71150,new Vector3(1090, 4052, 80)),
                        new MoveToPositionCoroutine(71150,new Vector3(1079, 3703, 78)),
                        new MoveToPositionCoroutine(71150,new Vector3(1081, 3521, 78)),
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
                        new EnterLevelAreaCoroutine (409893, 409000, 409374, -97144983, 210758),
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

            //// A2 - Clear the Western Channel (433003)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 433003,
            //    Act = Act.A2,
            //    WorldId = 432998,
            //    QuestType = BountyQuestType.ClearZone,
            //    Coroutines = new List<ISubroutine>
            //        {
            //            new EnterLevelAreaCoroutine (433003, 59486, 432993, 705396550, 175467),
            //            new EnterLevelAreaCoroutine (433003, 432993, 432998, 705396551, 176007, true),
            //            new ClearLevelAreaCoroutine (433003)
            //        }
            //});

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
                        new EnterLevelAreaCoroutine (433009, 59486, 432997, 1037011047, 176007),
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
                    new ClearAreaForNSecondsCoroutine(445044, 60, 0, 0, 45),
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



            //// A5 - Bounty: The Black King's Legacy (448615)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448615,
            //    Act = Act.A5,
            //    WorldId = 283566, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 56,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448615, 283566, 448396, 498366490, 448494),
            //        new MoveToMapMarkerCoroutine(448615, 283566, 498366490),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 21.38146
            //        new InteractWithGizmoCoroutine(448615, 448396, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448615, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448615),
            //        new MoveToPositionCoroutine(448396, new Vector3(143, 191, 0)),
            //    }
            //});


            //// A4 - Bounty: The Black King's Legacy (448155)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448155,
            //    Act = Act.A4,
            //    WorldId = 409000, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 49,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448155, 409000, 443801, -2005510577, 446440),
            //        new MoveToMapMarkerCoroutine(448155, 409000, -2005510577),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 24.08629
            //        new InteractWithGizmoCoroutine(448155, 443801, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448155, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448155),
            //        new MoveToPositionCoroutine(443801, new Vector3(478, 475, 0)),
            //    }
            //});

            //// A5 - Bounty: The Black King's Legacy (448619)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448619,
            //    Act = Act.A5,
            //    WorldId = 283566,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 56, // Ruins of Corvus
            //    Coroutines = new List<ISubroutine>
            //    {
            //        //QuestSnoId: 448619 QuestStep: 5, Description: Find the Portal to the Blighted Sewer, Header: 448619 5 0, State: InProgress, Index: 0
            //        new EnterLevelAreaCoroutine(448619, 283566, 0, -816816641, 448500),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 23.90381
            //        new InteractWithGizmoCoroutine(448619, 448373, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448619, 60, 0, 0, 45),
            //    }
            //});

            //// A5 - Bounty: The Black King's Legacy (448669)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448669,
            //    Act = Act.A5,
            //    WorldId = 338600, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 59, //Battlefield of etenity
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448669, 338600, 448396, 498366490, 448494),
            //        new MoveToMapMarkerCoroutine(448669, 338600, 498366490),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 21.38146
            //        new InteractWithGizmoCoroutine(448669, 448396, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448669, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448669),
            //    }
            //});

            //// A4 - Bounty: The Black King's Legacy (448208)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448208,
            //    Act = Act.A4,
            //    WorldId = 409510, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 44,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448208, 409510, 443801, -2005510577, 446440),
            //        new MoveToMapMarkerCoroutine(448208, 409510, -2005510577),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 24.08629
            //        new InteractWithGizmoCoroutine(448208, 443801, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448208, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448208),
            //    }
            //});

            //// A5 - Bounty: The Cursed Shrines (448625)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448625,
            //    Act = Act.A5,
            //    WorldId = 283566, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 56,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448625, 283566, 448396, 498366490, 448494),
            //        new MoveToMapMarkerCoroutine(448625, 283566, 498366490),

            //        new MoveToScenePositionCoroutine(448625, 448396, "p4_A5_bounty_grounds_Leorics_Garden_01", new Vector3(169.0945f, 226.6108f, 0.09999999f)),
            //        new InteractWithGizmoCoroutine(448625, 448396, 450222, 0, 5),

            //        new MoveToScenePositionCoroutine(448625, 448396, "p4_A5_bounty_grounds_Leorics_Garden_01", new Vector3(88.55468f, 224.9764f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448625, 448396, 450222, 0, 5),

            //        new MoveToScenePositionCoroutine(448625, 448396, "p4_A5_bounty_grounds_Leorics_Garden_01", new Vector3(89.03093f, 123.194f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448625, 448396, 450222, 0, 5),

            //        new MoveToScenePositionCoroutine(448625, 448396, "p4_A5_bounty_grounds_Leorics_Garden_01", new Vector3(157.2282f, 103.6414f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448625, 448396, 450222, 0, 5),

            //        new MoveToScenePositionCoroutine(448625, 448396, "p4_A5_bounty_grounds_Leorics_Garden_01", new Vector3(237.2581f, 97.85773f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448625, 448396, 450222, 0, 5),

            //        new MoveToScenePositionCoroutine(448625, 448396, "p4_A5_bounty_grounds_Leorics_Garden_01", new Vector3(240.2202f, 159.7054f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448625, 448396, 450222, 0, 5),

            //        new MoveToScenePositionCoroutine(448625, 448396, "p4_A5_bounty_grounds_Leorics_Garden_01", new Vector3(171.2832f, 178.5227f, 0.09999999f)),
            //    }
            //});


            //// A5 - Bounty: The Cursed Shrines (448501)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448501,
            //    Act = Act.A5,
            //    WorldId = 261712,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 51,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448501, 261712, 448409, 467573611, 448491),

            //        new MoveToPositionCoroutine(448409, new Vector3(360, 392, 10)),
            //        new MoveToActorCoroutine(448501, 448409, 450222),
            //        new InteractWithGizmoCoroutine(448501, 448409, 450222, -1, 15),

            //        new MoveToPositionCoroutine(448409, new Vector3(447, 353, 10)),
            //        new MoveToActorCoroutine(448501, 448409, 450222),
            //        new InteractWithGizmoCoroutine(448501, 448409, 450222, -1, 15),

            //        new MoveToPositionCoroutine(448409, new Vector3(543, 396, 10)),
            //        new MoveToActorCoroutine(448501, 448409, 450222),
            //        new InteractWithGizmoCoroutine(448501, 448409, 450222, -1, 15),

            //        new MoveToPositionCoroutine(448409, new Vector3(535, 509, 10)),
            //        new MoveToActorCoroutine(448501, 448409, 450222),
            //        new InteractWithGizmoCoroutine(448501, 448409, 450222, -1, 15),

            //        new MoveToPositionCoroutine(448409, new Vector3(448, 542, 10)),
            //        new MoveToActorCoroutine(448501, 448409, 450222),
            //        new InteractWithGizmoCoroutine(448501, 448409, 450222, -1, 15),

            //        new MoveToPositionCoroutine(448409, new Vector3(358, 507, 10)),
            //        new MoveToActorCoroutine(448501, 448409, 450222),
            //        new InteractWithGizmoCoroutine(448501, 448409, 450222, -1, 15),

            //        new MoveToPositionCoroutine(448409, new Vector3(470, 443, 5)),
            //        new MoveToActorCoroutine(448501, 448409, 364559),
            //        new InteractWithGizmoCoroutine(448501, 448409, 364559, -1, 15),

            //    }
            //});

            ////// A4 - Bounty: The Cursed Shrines (448276)
            ////Bounties.Add(new BountyData
            ////{
            ////    QuestId = 448276,
            ////    Act = Act.A4,
            ////    WorldId = 129305, 
            ////    QuestType = BountyQuestType.SpecialEvent,
            ////    //WaypointNumber = 47,
            ////    Coroutines = new List<ISubroutine>
            ////    {
            ////        // Coroutines goes here
            ////    }
            ////});

            //// A5 - Bounty: The Cursed Shrines (448572)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448572,
            //    Act = Act.A5,
            //    WorldId = 338944,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 52,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448572, 338944, 448381, 1012330756, 448491),
            //        new MoveToScenePositionCoroutine(448572, 448381, "p4_A5_bounty_grounds_Neph_01_E01_S01", new Vector3(129.9281f, 171.6445f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448572, 448381, 450222, 0, 5),
            //        new MoveToScenePositionCoroutine(448572, 448381, "p4_A5_bounty_grounds_Neph_01_E01_S01", new Vector3(180.9172f, 122.7384f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448572, 448381, 450222, 0, 5),
            //        new MoveToScenePositionCoroutine(448572, 448381, "p4_A5_bounty_grounds_Neph_01_E01_S01", new Vector3(234.1263f, 67.6698f, 0.09999999f)),
            //        new InteractWithGizmoCoroutine(448572, 448381, 450222, 0, 5),
            //        new MoveToScenePositionCoroutine(448572, 448381, "p4_A5_bounty_grounds_Neph_01_E01_S01", new Vector3(131.2455f, 72.02264f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448572, 448381, 450222, 0, 5),
            //        new MoveToScenePositionCoroutine(448572, 448381, "p4_A5_bounty_grounds_Neph_01_E01_S01", new Vector3(78.91064f, 136.8826f, 0.1f)),
            //        new InteractWithGizmoCoroutine(448572, 448381, 450222, 0, 5),
            //        new MoveToScenePositionCoroutine(448572, 448381, "p4_A5_bounty_grounds_Neph_01_E01_S01", new Vector3(77.63342f, 226.1407f, 0.09999999f)),
            //        new InteractWithGizmoCoroutine(448572, 448381, 450222, 0, 5),
            //        new MoveToScenePositionCoroutine(448572, 448381, "p4_A5_bounty_grounds_Neph_01_E01_S01", new Vector3(179.1443f, 180.3588f, 0.1f)),
            //        //ActorId: 364559, Type: Gizmo, Name: x1_Global_Chest_CursedChest-5528, Distance2d: 19.77113, CollisionRadius: 8.814026, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0 
            //        new InteractWithGizmoCoroutine(448572, 448381, 364559, 0, 5),
            //    }
            //});

            //// A4 - Bounty: The Cursed Shrines (448214)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448214,
            //    Act = Act.A4,
            //    WorldId = 409510,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 44,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448214, 409510, 443678, 124027337, 446439),

            //        new MoveToPositionCoroutine(443678, new Vector3(168, 230, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(102, 227, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(104, 145, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(148, 97, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(245, 95, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(219, 137, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(224, 226, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 364559),
            //        new InteractWithGizmoCoroutine(448619, 443678, 364559, -1, 5),
            //        new WaitCoroutine(2500),

            //    }
            //});

            //// A4 - Bounty: The Cursed Shrines (448268)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448268,
            //    Act = Act.A4,
            //    WorldId = 129305,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 47,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448268, 129305, 443678, 124027337, 446439),

            //        new MoveToPositionCoroutine(443678, new Vector3(168, 230, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),            

            //        new MoveToPositionCoroutine(443678, new Vector3(102, 227, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(104, 145, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(148, 97, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(245, 95, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(219, 137, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 445737),
            //        new InteractWithGizmoCoroutine(448214, 443678, 445737, -1, 5),

            //        new MoveToPositionCoroutine(443678, new Vector3(224, 226, 0)),
            //        new MoveToActorCoroutine(444573, 443678, 364559),
            //        new InteractWithGizmoCoroutine(448619, 443678, 364559, -1, 5),
            //        new WaitCoroutine(2500),
            //    }
            //});


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
                    new WaitCoroutine(2500),

                    //new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_W_Entrance_02_LoDGate_E02_S01", new Vector3(117.0242f, 19.4599f, 0.6535801f)),

                    new MoveToSceneCoroutine(436280, 428493, "p4_ruins_frost_NS_01"),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(64.43317f, 166.5968f, -9.9f)),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(130.7856f, 147.6561f, 0.1000014f)),

                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 13.54914
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),

                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 62.45116
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(223.5715f, 81.98212f, -9.899994f)),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(150.1893f, 81.20508f, -9.835966f)),

                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 92.18051
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),

                    new MoveToScenePositionCoroutine(436280, 428493, "p4_ruins_frost_NS_01", new Vector3(84.56f, 75.17822f, 0.1000005f)),

                    new MoveToActorCoroutine(436280, 428493, 435703),

                    // px_Ruins_frost_camp_cage (435703) Distance: 124.4732
                    new InteractWithGizmoCoroutine(436280, 428493, 435703, 0, 5),

                    new ClearAreaForNSecondsCoroutine(448619, 20, 0, 0, 45),

                    // px_Ruins_Frost_Camp_BarbSkular (435720) Distance: 8.818088
                    new InteractWithUnitCoroutine(436280, 428493, 435720, 0, 5),



                }
            });

            //// A2 - Bounty: Sardar's Treasure (347591)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 347591,
            //    Act = Act.A2,
            //    WorldId = 70885, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 24,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(347591, 70885, 922565181),
            //        //Scene: caOut_Oasis_Sub80_Cenote_DungeonEntranceA, SnoId: 68275,
            //        new MoveToSceneCoroutine(347591, 70885, "caOut_Oasis_Sub80_Cenote_DungeonEntranceA"),

            //        // a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle (175603) Distance: 21.63043                    
            //        // note InteractWithGizmoCoroutine is generated with a nearby marker position, must set to -1 or it will bug out if marker is not navigable.
            //        new InteractWithGizmoCoroutine(347591, 70885, 175603, -1, 5),

            //        new EnterLevelAreaCoroutine(347591, 70885, 157882, 922565181, 175467),

            //        // a2dun_Aqd_Act_Lever_FacePuzzle_01 (219879) Distance: 20.38647
            //        new InteractWithGizmoCoroutine(347591, 157882, 219879, 0, 5),

            //        //153836 a2dun_Aqd_GodHead_Door (Door) 
            //        new MoveToActorCoroutine(347591, 70885, 153836),

            //        // a2dun_Aqd_Chest_Rare_FacePuzzleSmall (190708) Distance: 30.81505
            //        new InteractWithGizmoCoroutine(347591, 157882, 190708, 0, 5),

            //        new ClearAreaForNSecondsCoroutine(347591, 30, 0, 0, 45),
            //    }
            //});

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

            //// A3 - Bounty: King of the Ziggurat (436282)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 436282,
            //    Act = Act.A3,
            //    WorldId = 428493, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 41,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(436282, 428493, 2912417),
            //        new ClearAreaForNSecondsCoroutine(436282, 120, 0, 0, 45),
            //        // p4_Ruins_Frost_Chest_Pillar_Reward (437935) Distance: 23.92906
            //        new InteractWithGizmoCoroutine(436282, 428493, 437935, 0, 5),
            //    }
            //});

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

                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-37.85986f, 358.9913f, 0.3457116f)),
                    new WaitCoroutine(1000),
                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-113.9968f, 362.2299f, 20.4907f)),
                    new WaitCoroutine(3000),
                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-178.5076f, 361.7603f, 9.386467f)),
                    new WaitCoroutine(1000),
                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-115.8213f, 364.533f, 20.4907f)),
                    new WaitCoroutine(3000),
                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-125.3792f, 428.8734f, 5.399465f)),
                    new WaitCoroutine(1000),
                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-118.6995f, 358.3481f, 20.4907f)),
                    new WaitCoroutine(3000),
                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-117.0858f, 302.4371f, 10.26097f)),
                    new WaitCoroutine(1000),
                    new MoveToScenePositionCoroutine(436282, 428493, "p4_ruins_frost_NSEW_03_Ziggurat", new Vector3(-121.4947f, 356.236f, 20.4907f)),
                    new WaitCoroutine(3000),
                    new ClearAreaForNSecondsCoroutine(436282, 70, 0, 0, 100),

                    new InteractWithGizmoCoroutine(436282, 428493, 437935, 0, 5),
                }
            });

            // A2 - Bounty: Restless Sands (350562)
            // Has trouble finding all the gizmos that spawn monsters.
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 350562,
            //    Act = Act.A2,
            //    WorldId = 70885,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 22,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(350562, 70885, 2912417),
            //        // oldNecromancer (4798) Distance: 25.94104
            //        new InteractWithUnitCoroutine(350562, 70885, 4798, 2912417, 5),
            //        // caOut_Totem_A (3707) Distance: 86.27297
            //        new InteractWithGizmoCoroutine(350562, 70885, 3707, 0, 5),
            //        new ClearAreaForNSecondsCoroutine(350562, 10, 0, 0, 45),
            //        new MoveToScenePositionCoroutine(350562, 70885, "caOut_StingingWinds_E04_S09", new Vector3(71.31079f, 119.0617f, 172.3481f)),
            //        // caOut_Totem_A (3707) Distance: 68.86753
            //        new InteractWithGizmoCoroutine(350562, 70885, 3707, 0, 5),
            //        new ClearAreaForNSecondsCoroutine(350562, 10, 0, 0, 45),
            //        // oldNecromancer (4798) Distance: 9.559196
            //        new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
            //        new ClearAreaForNSecondsCoroutine(350562, 10, 0, 0, 45),
            //        new InteractWithUnitCoroutine(350562, 70885, 4798, 0, 5),
            //    }
            //});

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
                    new MoveToActorCoroutine(444573, 408254, 434356),
                    new InteractWithGizmoCoroutine(444573, 408254, 434356, -1),
                    new ClearAreaForNSecondsCoroutine(444573, 60, 0, 0, 45),
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

                    new WaitCoroutine(2000),

                    // p4_Forest_ClericGhost (433832) Distance: 18.61721
                    new InteractWithUnitCoroutine(444702, 441322, 433832, 0, 5),

                    new WaitCoroutine(8000),

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
                    new ClearAreaForNSecondsCoroutine(445158, 60, 0, 0, 45),
                }
            });

            //// A5 - Bounty: A Plague of Burrowers (448528)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448528,
            //    Act = Act.A5,
            //    WorldId = 261712,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 51,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448528, 261712, 448373, -816816641, 448500),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 19.65089
            //        new InteractWithGizmoCoroutine(448528, 448373, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448528, 60, 0, 0, 45),
            //    }
            //});

            //// A4 - Bounty: A Plague of Burrowers (448342)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448342,
            //    Act = Act.A4,
            //    WorldId = 121579,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 46,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448342, 121579, 443705, -2005068563, 446440),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 14.38483
            //        new InteractWithGizmoCoroutine(448342, 443705, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448342, 60, 0, 0, 45),
            //    }
            //});

            //// A4 - Bounty: A Plague of Burrowers (448181)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448181,
            //    Act = Act.A4,
            //    WorldId = 409000, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 49,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448181, 409000, 443705, -2005068563, 446440),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 14.38483
            //        new InteractWithGizmoCoroutine(448181, 443705, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448181, 60, 0, 0, 45),
            //    }
            //});

            //// A5 - Bounty: A Plague of Burrowers (448647)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448647,
            //    Act = Act.A5,
            //    WorldId = 283566, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 56,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448647, 283566, 448402, -728782754, 448505),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 20.7795
            //        new InteractWithGizmoCoroutine(448647, 448402, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448647, 60, 0, 0, 45),

            //    }
            //});

            //// A4 - Bounty: A Plague of Burrowers (448292)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448292,
            //    Act = Act.A4,
            //    WorldId = 129305, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 47,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448292, 129305, 443678, 124027337, 446439),
            //        new MoveToMapMarkerCoroutine(448292, 129305, 124027337),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 20.7795
            //        new InteractWithGizmoCoroutine(448292, 443678, 365097, 2912417, 5),
            //        new MoveToPositionCoroutine(443678, new Vector3(100, 221, 0)),
            //        new MoveToPositionCoroutine(443678, new Vector3(97, 97, 0)),
            //        new MoveToPositionCoroutine(443678, new Vector3(250, 102, 0)),
            //        new MoveToPositionCoroutine(443678, new Vector3(227, 186, 0)),
            //        new ClearLevelAreaCoroutine(448292),
            //    }
            //});

            //// A4 - Bounty: The Bound Shaman (448224)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448224,
            //    Act = Act.A4,
            //    WorldId = 409510, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 44,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448224, 409510, 443720, -1454464458, 446440),
            //        new MoveToMapMarkerCoroutine(448224, 409510, -1454464458),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 30.22656
            //        new InteractWithGizmoCoroutine(448224, 443720, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448224, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448224),
            //        new MoveToPositionCoroutine(443720, new Vector3(471, 450, 5)),
            //    }
            //});

            //// A5 - Bounty: The Bound Shaman (448586)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448586,
            //    Act = Act.A5,
            //    WorldId = 338944,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 52,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448586, 283566, 448366, 1012772770, 448515),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 14.38483
            //        new InteractWithGizmoCoroutine(448586, 448366, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448586, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448586),
            //    }
            //});

            //// A5 - Bounty: The Bound Shaman (448693)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448693,
            //    Act = Act.A5,
            //    WorldId = 338600, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 59,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448693, 338600, 448373, -816816641, 448500),
            //        new MoveToMapMarkerCoroutine(448693, 338600, -816816641),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 23.90381
            //        new InteractWithGizmoCoroutine(448693, 448373, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448693, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448693),
            //        // x1_Global_Chest_StartsClean (363725) Distance: 49.44165
            //        new InteractWithGizmoCoroutine(0, 448373, 363725, 298775102, 5),
            //    }
            //});

            //// A5 - Bounty: The Bound Shaman (448641)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448641,
            //    Act = Act.A5,
            //    WorldId = 283566, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 56,
            //    Coroutines = new IndexedList<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448641, 283566, 448366, 1012772770, 448515),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 14.38483
            //        new InteractWithGizmoCoroutine(448641, 448366, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448641, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448641),
            //    }
            //});

            //// A4 - Bounty: The Bound Shaman (448334)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448334,
            //    Act = Act.A4,
            //    WorldId = 121579,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 46,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448334, 121579, 443678, 124027337, 446439),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 20.7795
            //        new InteractWithGizmoCoroutine(448334, 443678, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448334, 60, 0, 0, 45),
            //    }
            //});

            //// A4 - Bounty: The Bound Shaman (448336)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448336,
            //    Act = Act.A4,
            //    WorldId = 121579, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 46,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448336, 121579, 443801, -2005510577, 446440),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 24.08629
            //        new InteractWithGizmoCoroutine(448336, 443801, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448336, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448340),
            //    }
            //});

            //// A4 - Bounty: The Bound Shaman (448340)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 448340,
            //    Act = Act.A4,
            //    WorldId = 121579,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 46,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new EnterLevelAreaCoroutine(448340, 121579, 443705, -2005068563, 446440),
            //        // x1_Global_Chest_CursedChest_B (365097) Distance: 14.38483
            //        new InteractWithGizmoCoroutine(448340, 443705, 365097, 2912417, 5),
            //        new ClearAreaForNSecondsCoroutine(448340, 60, 0, 0, 45),
            //        new ClearLevelAreaCoroutine(448340),
            //    }
            //});



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
                    new MoveToActorCoroutine(436276, 428493, 435879),
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
                //WaypointNumber = 35,
                Coroutines = new List<ISubroutine>
                {
                    new MoveToMapMarkerCoroutine(346186, 95804, 2912417),
                    // bastionsKeepGuard_Injured_Reinforcement_Event (153419) Distance: 17.03967
                    new InteractWithUnitCoroutine(346186, 95804, 153419, 2912417, 5),
                    new MoveToPositionCoroutine(95804, new Vector3(2488, 594, 38)),
                    new MoveToPositionCoroutine(95804, new Vector3(2474, 520, 63)),
                    new MoveToPositionCoroutine(95804, new Vector3(2540, 519, 87)),
                    // bastionsKeepGuard_Lieutenant_Reinforcement_Event (153428) Distance: 4.879069
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
                    // x1_Event_CursedShrine (364601) Distance: 11.55153
                    new InteractWithGizmoCoroutine(447419, 444305, 364601, 0, 5),
                    new ClearAreaForNSecondsCoroutine(447419, 60, 0, 0, 45),
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
                    new InteractWithGizmoCoroutine(447218, 444305, 365097, 0, 5),
                    new ClearAreaForNSecondsCoroutine(447218, 60, 0, 0, 45),
                    new ClearLevelAreaCoroutine(447218),
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

                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(95.05688f, 100.9766f, 90.32501f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 11.12986
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 2912417, 5),
                    new WaitCoroutine(3000),

                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(114.7661f, 82.81201f, 90.32503f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 5.553013
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 2912417, 5),
                    new WaitCoroutine(3000),

                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(164.7249f, 159.7368f, 81.5364f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 5.59462
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 0, 5),
                    new WaitCoroutine(3000),

                    new MoveToScenePositionCoroutine(347595, 70885, "caOut_Oasis_Sub240_POI", new Vector3(145.3794f, 179.5557f, 81.5364f)),

                    // caldeumTortured_Poor_Male_A_ZakarwaPrisoner (183609) Distance: 8.649173
                    new InteractWithUnitCoroutine(347595, 70885, 183609, 0, 5),
                    new WaitCoroutine(3000),

                    // FallenChampion_B_PrisonersEvent_Unique (260228) Distance: 8.834968
                    new MoveToActorCoroutine(347595, 70885, 260228),

                    new ClearAreaForNSecondsCoroutine(347595, 30, 0, 0, 45),
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
                    // x1_Event_CursedShrine (364601) Distance: 15.57718
                    new InteractWithGizmoCoroutine(436965, 408254, 364601, 0, 5),
                    new ClearAreaForNSecondsCoroutine(436965, 60, 0, 0, 45),
                }
            });

            //// A2 - Bounty: Lost Treasure of Khan Dakab (346067)
            /// // Doesnt interact with door properly
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 346067,
            //    Act = Act.A2,
            //    WorldId = 70885, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 24,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(346067, 70885, 913850831),
            //        new MoveToSceneCoroutine(346067, 70885, "caOut_Oasis_Sub240_POI"),
            //        new MoveToScenePositionCoroutine(346067, 70885, "caOut_Oasis_Sub240_POI", new Vector3(126.1895f, 117.2666f, 73.43011f)),
            //        // a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle (175603) Distance: 6.847974
            //        new InteractWithGizmoCoroutine(346067, 70885, 175603, 0, 5), 
            //        // a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle (175603) Distance: 41.47263
            //        new InteractWithGizmoCoroutine(346067, 70885, 175603, 0, 5),
            //        new MoveToScenePositionCoroutine(346067, 70885, "caOut_Oasis_Sub240_POI", new Vector3(66.01099f, 60.96484f, 71.31847f)),
            //        // g_Portal_ArchTall_Blue (176002) Distance: 11.88592
            //        //new InteractWithGizmoCoroutine(346067, 70885, 176002, 913850831, 5),
            //        new EnterLevelAreaCoroutine(346067, 70885, 158593, 913850831, 176002),
            //        // a2dun_Aqd_Act_Lever_FacePuzzle_02 (219880) Distance: 20.90631
            //        new InteractWithGizmoCoroutine(346067, 158593, 219880, 0, 5),
            //        // a2dun_Aqd_Chest_Special_FacePuzzle_Large (190524) Distance: 27.1103
            //        new InteractWithGizmoCoroutine(346067, 158593, 190524, 0, 5),
            //        new ClearAreaForNSecondsCoroutine(346067, 30, 0, 0, 45),
            //    }
            //});

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
                    new MoveToSceneCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI"),
                    new MoveToScenePositionCoroutine(346065, 70885, "caOut_Oasis_Sub240_POI", new Vector3(106.3967f, 125.3721f, 120.1f)),

                    ////ActorId: 222268, Type: Gizmo, Name: caOut_Oasis_RakinishuStone_B_FX-4071, Distance2d: 18.3527, CollisionRadius: 2.066596, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0

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

            //// A5 - Bounty: Demon Prison (363394)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 363394,
            //    Act = Act.A5,
            //    WorldId = 271233, 
            //    QuestType = BountyQuestType.SpecialEvent,
            //    //WaypointNumber = 57,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(363394, 271233, 2912417),
            //        // X1_Fortress_NephalemSpirit (354345) Distance: 21.83757
            //        new InteractWithUnitCoroutine(363394, 271233, 354345, 2912417, 5),
            //        //ActorId: 363943, Type: Gizmo, Name: x1_Fortress_Crystal_Prison_Yellow-2093, Distance2d: 19.41266, CollisionRadius: 14.90234, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
            //        new InteractWithGizmoCoroutine(363394, 271233, 363943, -1, 5),
            //        new ClearAreaForNSecondsCoroutine(363394, 60, 0, 0, 40),
            //    }
            //});

            //// A5 - Bounty: Lost Host (363402)
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 363402,
            //    Act = Act.A5,
            //    WorldId = 271235,
            //    QuestType = BountyQuestType.SpecialEvent,
            //    WaypointNumber = 58,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        new MoveToMapMarkerCoroutine(363402, 271235, 2912417),
            //        new ClearAreaForNSecondsCoroutine(363402, 60, 0, 0, 45),
            //    }
            //});

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

            ////    [1F372FD4]
            ////[The Cursed Crystals]
            ////QuestSnoId: 365385, QuestMeter: -1, QuestState: InProgress, QuestStep: 6, KillCount: 0, BonusCount: 0
            ////[Step]
            ////        Operate Ancient Crystal 1, Id: 2
            ////[Objective]
            ////        Type: InteractWithActor
            ////[Step] Defeat Wave 1, Id: 4
            ////[Objective]
            ////        x1_Pand_Ext_240_Cellar_04_DemonEvent_A, Type: KillGroup
            ////[Step] Operate Ancient Crystal 2, Id: 6
            ////[Objective]
            ////        Type: InteractWithActor
            ////[Step] Defeat Wave 2, Id: 8
            ////[Objective]
            ////        x1_Pand_Ext_240_Cellar_04_DemonEvent_B, Type: KillGroup
            ////[Step] Operate Ancient Crystal 3, Id: 13
            ////[Objective]
            ////        Type: InteractWithActor
            ////[Step] Kill Boss, Id: 10
            ////[Objective]
            ////        x1_Pand_Ext_240_Cellar_04_DemonEvent_C, Type: KillGroup

            //// A5 - Bounty: The Cursed Crystals (365385)
            /// // Bugged trying to find/interact with second pylon
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 365385,
            //    Act = Act.A5,
            //    WorldId = 338600, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    WaypointNumber = 59,
            //    Coroutines = new List<ISubroutine>
            //    {

            //        new EnterLevelAreaCoroutine(365385, 338600, 357656, -1551729968, 176002),

            //        new MoveToPositionCoroutine(357656, new Vector3(144, 196, -13)),
            //        new InteractWithGizmoCoroutine(365385, 357656, 365488, 0, 5),
                    
            //        new MoveToPositionCoroutine(357656, new Vector3(160, 74, -13)),
            //        //ActorId: 365489, Type: Gizmo, Name: x1_Fortress_Crystal_Prison_DemonEvent_2-8105, Distance2d: 8.03618, CollisionRadius: 14.90234, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
            //        new InteractWithGizmoCoroutine(365385, 357656, 365489, 0, 5),

            //        new MoveToPositionCoroutine(357656, new Vector3(139, 69, -13)),
            //        new InteractWithGizmoCoroutine(367904, 357656, 365489, 0, 5),


            //        new ClearAreaForNSecondsCoroutine(367904, 60, 0, 0, 45),

            //        // by Chest
            //        new MoveToPositionCoroutine(357656, new Vector3(144, 127, -13)),
            //    }
            //});

            //// A5 - Bounty: The Cursed Crystals (367904)
            // // Portal and crystals keep changing actor ids, is this a dynamic bounty?
            //Bounties.Add(new BountyData
            //{
            //    QuestId = 367904,
            //    Act = Act.A5,
            //    WorldId = 338600, // Enter the final worldId here
            //    QuestType = BountyQuestType.SpecialEvent,
            //    WaypointNumber = 59,
            //    Coroutines = new List<ISubroutine>
            //    {
            //        //ActorId: 176004, Type: Gizmo, Name: g_Portal_RectangleTall_Blue-47533, Distance2d: 10.84147, 
            //        new EnterLevelAreaCoroutine(367904, 338600, 357656, -1551729968, 176002),

            //        new MoveToPositionCoroutine(357656, new Vector3(147, 188, -13)),
            //        // x1_Fortress_Crystal_Prison_DemonEvent_1 (365488) Distance: 31.76865
            //        new InteractWithGizmoCoroutine(367904, 357656, 365488, 0, 5),

            //        new MoveToPositionCoroutine(357656, new Vector3(175, 72, -13)),
            //        new InteractWithGizmoCoroutine(367904, 357656, 365489, 0, 5),

            //        new MoveToPositionCoroutine(357656, new Vector3(142, 74, -13)),
            //        new InteractWithGizmoCoroutine(367904, 357656, 365995, 0, 5),

            //        new ClearAreaForNSecondsCoroutine(367904, 60, 0, 0, 45),

            //        // by Chest
            //        new MoveToPositionCoroutine(357656, new Vector3(144, 127, -13)),
            //    }
            //});

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
                    new MoveToActorCoroutine(359101, 338944, 331951),
                    new WaitCoroutine(3000),
                    new ClearAreaForNSecondsCoroutine(359101, 60, 0, 0, 45),
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
                    new ClearAreaForNSecondsCoroutine(359079, 60, 0, 0, 45),
                    // x1_Westm_Graveyard_Ghost_Female_01_UniqueEvent (357197) Distance: 5.017566
                    new InteractWithUnitCoroutine(359079, 338944, 357197, 0, 5),
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
            //{ 349216, 34 },
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
