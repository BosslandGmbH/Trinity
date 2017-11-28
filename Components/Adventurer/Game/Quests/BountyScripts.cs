using System;
using Trinity.Framework.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Game.Actors;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Game.Quests
{
    public class BountyScripts : IReadOnlyDictionary<int, BountyScript>
    {
        private static readonly Dictionary<int, BountyScript> Items = new Dictionary<int, BountyScript>();

        static BountyScripts()
        {
            // 433017 - The Putrid Waters
            // - Move to objective
            // - Interact with gizmo 175603
            // - Interact with gizmo 175603
            // - Use portal at MarkerHash 913850831
            // - Somehow avoid nodes behind a2dun_Aqd_GodHead_Door_LargePuzzle-9058 ActorSnoId: 207615
            // - Interact with gizmo 219880
            // - Move to main objective
            // - Interact with door 207615
            // - Interact with gizmo 190524
            // - Kill mobs

            // 346184 - Blaze of Glory
            // - Move to objective
            // - Interact with npc 152145
            // - Kill mobs for 40 seconds
            // - Interact with npc 152145

            // 368564 - Magic Misfire
            // - Move to objective and enter zone
            // - Interact with npc 283061
            // - Kill mobs
            // - Interact with npc 283061

            // 345500 - Carrion Farm
            // - Move to objective
            // - Interact with npc 81980
            // - Kill 81982 x4
            // - Interact with npc 81980

            // 347591 - Sardar's Treasure
            // - Move to objective
            // - Interact with gizmo 175603
            // - Use portal at MarkerHash 922565181
            // - Somehow avoid nodes behind ActorId: 153836, Type: Gizmo, Name: a2dun_Aqd_GodHead_Door-2289, Distance2d: 92.15494, CollisionRadius: 25.22241, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
            // - Interact with gizmo 219879
            // - Interact with door 153836
            // - Interact with gizmo 190708
            // - Kill mobs

            //344486 - Revenge of Gharbad.txt
            // - Move to objective
            // - Interact with npc 81068
            // - Interact with gizmo 225252
            // - Kill questmobs
            // - Interact with gizmo 225252
            // - Kill questmobs
            // - Interact with npc 81068
            // - Kill 81342

            //368559 - The Reformed Cultist.txt
            // - Move to objective
            // - Interact with portal 333736
            // - Kill questmobs
            // - Interact with npc 247595
            // - Interact with portal 178293
            // - Kill questmobs
            // - Interact with door 316627

            //345505 - Eternal War.txt
            // - Move to objective
            // - Interact with Gizmo 111907
            // - Kill questmobs

            //347520 - Lair of the Lacuni.txt
            // - Move to objective
            // - Interact with Portal 185067
            // - Kill all mobs in the area
            // - Move to objective
            // - Kill all mobs in the area

            //368543 - The Lost Patrol.txt
            // - Move to objective
            // - Kill questmobs
            // - Interact with unit monster type 433181
            // - Move to objective
            // - Kill questmobs
            // - Interact with unit monster type 433181
            // - Move to objective
            // - Kill questmobs
            // - Interact with unit monster type 433181

            Items.Add(350562, new BountyScript
            {
                ScriptItems = new List<BountyScriptItem>
                                                {
                                                    new BountyScriptItem {ActorId = 4798, Type = BountyScriptItemType.InteractWithMonster},
                                                    new BountyScriptItem {ActorId = 3707, Type = BountyScriptItemType.InteractWithGizmo},
                                                }
            });

            Items.Add(347591, new BountyScript
            {
                ScriptItems = new List<BountyScriptItem>
                                                {
                                                    new BountyScriptItem {ActorId = 385817, Type = BountyScriptItemType.InteractWithGizmo, WorldId = 157882},
                                                    new BountyScriptItem {ActorId = 153836, Type = BountyScriptItemType.InteractWithGizmo, WorldId = 157882},
                                                }
            });

            Items.Add(368601, new BountyScript
            {
                ScriptItems = new List<BountyScriptItem>
                                                {
                                                    new BountyScriptItem {ActorId = 336711, Type = BountyScriptItemType.InteractWithMonster, WorldId = 336902},
                                                }
            });

            Items.Add(409897, new BountyScript
            {
                ScriptItems = new List<BountyScriptItem>
                                                {
                                                    new BountyScriptItem {ActorId = 365097, Type = BountyScriptItemType.InteractWithGizmo, WorldId = 181644},
                                                }
            });

            Items.Add(368420, new BountyScript
            {
                ScriptItems = new List<BountyScriptItem>
                                                {
                                                    new BountyScriptItem {ActorId = 4798, Type = BountyScriptItemType.InteractWithMonster, WorldId = 303361},
                                                    new BountyScriptItem {ActorId = 316371, Type = BountyScriptItemType.InteractWithGizmo, WorldId = 303361},
                                                    new BountyScriptItem {ActorId = 4798, Type = BountyScriptItemType.InteractWithMonster, WorldId = 303361},
                                                }
            });

            Items.Add(346180, new BountyScript
            {
                ScriptItems = new List<BountyScriptItem>
                                                {
                                                    new BountyScriptItem {ActorId = 205312, Type = BountyScriptItemType.InteractWithMonster, WorldId = 95804},
                                                    new BountyScriptItem {ActorId = 205468, Type = BountyScriptItemType.InteractWithMonster, WorldId = 95804},
                                                }
            });
        }

        public IEnumerator<KeyValuePair<int, BountyScript>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public bool ContainsKey(int key)
        {
            return Items.ContainsKey(key);
        }

        public bool TryGetValue(int key, out BountyScript value)
        {
            return Items.TryGetValue(key, out value);
        }

        public BountyScript this[int key]
        {
            get { return Items[key]; }
        }

        public IEnumerable<int> Keys
        {
            get { return Items.Keys; }
        }

        public IEnumerable<BountyScript> Values
        {
            get { return Items.Values; }
        }
    }

    public class BountyScript
    {
        public List<BountyScriptItem> ScriptItems { get; set; }
    }

    public class BountyScriptItem
    {
        public BountyScriptItemType Type { get; set; }
        public int ActorId { get; set; }
        public TimeSpan WaitTime { get; set; }
        public int WorldId { get; set; }

        public DiaObject FindMatchingActor()
        {
            return
                ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                    .Where(o => o.ActorSnoId == ActorId && MatchesCondition(o))
                    .OrderBy(a => a.Distance)
                    .FirstOrDefault();
        }

        private bool MatchesCondition(DiaObject actor)
        {
            if (actor.IsValid)
                switch (Type)
                {
                    case BountyScriptItemType.InteractWithGizmo:
                        if (actor is DiaGizmo)
                        {
                            var gizmo = actor as DiaGizmo;
                            return gizmo.IsFullyValid() && ActorFinder.IsGizmoInteractable(gizmo);
                        }
                        return false;

                    case BountyScriptItemType.InteractWithMonster:
                        if (actor is DiaUnit)
                        {
                            var unit = actor as DiaUnit;
                            return ActorFinder.IsUnitInteractable(unit);
                        }
                        return false;

                    default:
                        return false;
                }
            return false;
        }
    }

    public enum BountyScriptItemType
    {
        MoveToObjective,
        MoveToCoordinates,
        InteractWithGizmo,
        InteractWithMonster,
        KillQuestMonsters,
        UsePortal,
        ClearArea
    }

    public enum BountyScriptAfterwards
    {
        Nothing,
        ClearArea
    }
}