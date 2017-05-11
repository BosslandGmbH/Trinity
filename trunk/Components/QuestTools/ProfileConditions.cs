using System;
using System.Linq;
using Trinity;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.ProfileTags;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.QuestTools
{
    public static class ProfileConditions
    {
        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            IsInitialized = true;
            ScriptManager.RegisterShortcutsDefinitions((typeof(ProfileConditions)));
        }

        public static string ProfileSetting(string key)
        {
            string value;
            return ProfileSettingTag.ProfileSettings.TryGetValue(key, out value) ? value : string.Empty;
        }

        public static bool IsCastingOrLoading()
        {
            return

                ZetaDia.Me != null &&
                ZetaDia.Me.IsValid &&
                ZetaDia.Me.CommonData != null &&
                ZetaDia.Me.CommonData.IsValid &&
                !ZetaDia.Me.IsDead &&
                (
                    ZetaDia.Globals.IsLoadingWorld ||
                    ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting ||
                    ZetaDia.Me.CommonData.AnimationState == AnimationState.Channeling ||
                    ZetaDia.Me.CommonData.AnimationState == AnimationState.Transform ||
                    ZetaDia.Me.CommonData.AnimationState.ToString() == "13"
                );
        }

        public static bool HaveBounty(int questId)
        {
            return ZetaDia.Storage.Quests.Bounties.FirstOrDefault(bounty => bounty.Info.QuestSNO == questId && bounty.Info.State != QuestState.Completed) != null;
        }

        public static bool CurrentSceneId(int sceneSnoId)
        {
            return ZetaDia.Me.CurrentScene.SceneInfo.SNOId == sceneSnoId;
        }

        public static bool CurrentSceneName(string sceneName)
        {
            return ZetaDia.Me.CurrentScene.Name.ToLowerInvariant().Contains(sceneName.ToLowerInvariant());
        }

        public static bool IsInDeathGateWorld()
        {
            return DeathGates.IsInDeathGateWorld;
        }

        public static bool IsInDeathGateScene()
        {
            return DeathGates.Scenes.FirstOrDefault(s => s.WorldScene.IsInScene(ZetaDia.Me.Position)) != null;
        }

        public static bool GameFinishedWithinSeconds(int seconds)
        {
            return DateTime.UtcNow.Subtract(GameUI.LastClosedCreditsTime).TotalSeconds <= seconds;
        }

        public static bool CurrentDifficulty(string difficulty)
        {
            GameDifficulty d;
            return Enum.TryParse(difficulty, true, out d) && CharacterSettings.Instance.GameDifficulty == d;
        }

        public static bool CurrentDifficultyLessThan(string difficulty)
        {
            GameDifficulty d;
            if (Enum.TryParse(difficulty, true, out d))
            {
                var currentIndex = (int)CharacterSettings.Instance.GameDifficulty;
                var testIndex = (int)d;

                return currentIndex < testIndex;
            }
            return false;
        }

        public static bool CurrentDifficultyGreaterThan(string difficulty)
        {
            GameDifficulty d;
            if (Enum.TryParse(difficulty, true, out d))
            {
                var currentIndex = (int)CharacterSettings.Instance.GameDifficulty;
                var testIndex = (int)d;

                return currentIndex > testIndex;
            }
            return false;
        }

        public static bool CurrentClass(string actorClass)
        {
            ActorClass a;
            return Enum.TryParse(actorClass, true, out a) && ZetaDia.Service.Hero.Class == a;
        }

        public static bool MarkerTypeExists(string worldMarkerType)
        {
            WorldMarkerType t;
            return Enum.TryParse(worldMarkerType, true, out t) && Core.Markers.Any(m => m.MarkerType == t);
        }

        public static bool MarkerNameExists(string markerName)
        {
            return !string.IsNullOrEmpty(markerName) && Core.Markers.Any(m => m.Name == markerName);
        }


        public static bool MarkerExistsNearMe(int hashId, float range)
        {
            return Core.Markers.Any(m => m.NameHash == hashId && m.Distance <= range);
        }

        public static bool MarkerNameHashExists(int markerNameHash)
        {
            return Core.Markers.Any(m => m.NameHash == markerNameHash);
        }

        public static bool MarkerTypeWithinRange(string worldMarkerType, float range)
        {
            if (string.IsNullOrEmpty(worldMarkerType) || worldMarkerType == "None") return false;

            WorldMarkerType t;
            return Enum.TryParse(worldMarkerType, true, out t) && Core.Markers.Any(m => m.MarkerType == t && m.Distance <= range);
        }

        public static bool PercentNodesVisited(int percent)
        {
            var nodes = ExplorationGrid.Instance.WalkableNodes.Count(n => Core.Player.LevelAreaId == n.LevelAreaId && !n.IsBlacklisted && !n.IsIgnored && n.HasEnoughNavigableCells);
            var univistedNodes = ExplorationGrid.Instance.WalkableNodes.Count(n => !n.IsVisited && Core.Player.LevelAreaId == n.LevelAreaId && !n.IsBlacklisted && !n.IsIgnored && n.HasEnoughNavigableCells);
            return nodes <= 0 || (1 - univistedNodes / (double)nodes) * 100 > percent;
        }



        public static bool BossNearby(int range)
        {
            return Core.Targets.ByMonsterQuality[MonsterQuality.Boss].Any();
        }

        public static bool EliteNearby(int range)
        {
            return Core.Targets.Any(m => m.IsUnit && m.IsElite);
        }

        public static bool CurrentHeroLevel(int level)
        {
            return ZetaDia.Service.Hero.Level == level;
        }

        public static long ItemCount(int actorId)
        {
            var items = InventoryManager.StashItems.Where(item => actorId == item.ActorSnoId)
                .Concat(InventoryManager.Backpack.Where(item => actorId == item.ActorSnoId)).ToList();

            if (!items.Any())
                return 0;

            if (items.First().ItemStackQuantity > 0)
                return items.Select(i => i.ItemStackQuantity).Aggregate((a, b) => a + b);

            return items.Count;
        }

        public static long BackpackCount(int actorId)
        {
            var items = InventoryManager.Backpack.Where(item => actorId == item.ActorSnoId).ToList();

            if (!items.Any())
                return 0;

            if (items.First().ItemStackQuantity > 0)
                return items.Select(i => i.ItemStackQuantity).Aggregate((a, b) => a + b);

            return items.Count;
        }

        public static long StashCount(int actorId)
        {
            var items = InventoryManager.StashItems.Where(item => actorId == item.ActorSnoId).ToList();

            if (!items.Any())
                return 0;

            if (items.First().ItemStackQuantity > 0)
                return items.Select(i => i.ItemStackQuantity).Aggregate((a, b) => a + b);

            return items.Count;
        }

        public static bool ItemCountGreaterThan(int actorId, int amount)
        {
            return ItemCount(actorId) > amount;
        }

        public static bool ItemCountLessThan(int actorId, int amount)
        {
            return ItemCount(actorId) < amount;
        }

        public static bool ActorExistsNearMe(int actorId, float range)
        {
            var nearbyActors = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where(i => i.IsValid && i.ActorSnoId == actorId && Vector3.Distance(i.Position, ZetaDia.Me.Position) <= range).ToList();
            return nearbyActors.Count > 0;
        }

        public static bool HasBeenOperated(int actorId)
        {
            var actor = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(a => a.ActorSnoId == actorId);
            return actor != null && actor.HasBeenOperated;
        }

        public static bool ActorAnimationExistsNearMe(int actorId, string animationName, float radius)
        {
            var actor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault(a => a.ActorSnoId == actorId && a.Distance <= radius);

            if (actor?.CommonData == null || !actor.IsFullyValid())
                return false;

            var result = actor.CommonData.CurrentAnimation.ToString().ToLowerInvariant().Contains(animationName.ToLowerInvariant());

            //Core.Logger.Debug("Animation for {0} ({1}) is {2} State={3} ({4})",
            //    actor.Name,
            //    actor.ActorSnoId,
            //    actor.CommonData.CurrentAnimation,
            //    actor.CommonData.AnimationState,
            //    result);

            return result;
        }

        public static bool CurrentAnimation(int actorId, string animationName)
        {
            return ActorAnimationExistsNearMe(actorId, animationName, 200f);
        }

        public static bool IsBountyLevelArea(int questId)
        {
            var result = ZetaDia.Storage.Quests.Bounties.FirstOrDefault(q => q.Quest == (SNOQuest)questId && q.LevelAreas.Contains((SNOLevelArea)ZetaDia.CurrentLevelAreaSnoId) || q.StartingLevelArea == (SNOLevelArea)ZetaDia.CurrentLevelAreaSnoId);

            return result != null;
        }

        public static bool IsVendorWindowOpen()
        {
            return UIElements.VendorWindow != null && UIElements.VendorWindow.IsValid && UIElements.VendorWindow.IsVisible;
        }

        public static bool QuestComplete(int questId)
        {
            return ZetaDia.Storage.Quests.AllQuests.Any(q => q.QuestSNO == questId && q.State == QuestState.Completed);
        }


    }
}
