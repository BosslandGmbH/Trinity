using System;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.ProfileTags
{
    public static class TrinityConditions
    {
        public static void Initialize()
        {
            ScriptManager.RegisterShortcutsDefinitions((typeof(TrinityConditions)));
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
                    ZetaDia.IsLoadingWorld ||
                    ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting ||
                    ZetaDia.Me.CommonData.AnimationState == AnimationState.Channeling ||
                    ZetaDia.Me.CommonData.AnimationState == AnimationState.Transform ||
                    ZetaDia.Me.CommonData.AnimationState.ToString() == "13"
                );
        }

        public static bool CurrentSceneName(string sceneName)
        {
            return ZetaDia.Me.CurrentScene.Name.ToLowerInvariant().Contains(sceneName.ToLowerInvariant());
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
            return Enum.TryParse(worldMarkerType, true, out t) && Core.Markers.CurrentWorldMarkers.Any(m => m.MarkerType == t);
        }

        public static bool MarkerNameExists(string markerName)
        {
            return !string.IsNullOrEmpty(markerName) && Core.Markers.CurrentWorldMarkers.Any(m => m.Name == markerName);
        }

        public static bool MarkerNameHashExists(int markerNameHash)
        {
            return Core.Markers.CurrentWorldMarkers.Any(m => m.NameHash == markerNameHash);
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
            var items = ZetaDia.Me.Inventory.StashItems.Where(item => actorId == item.ActorSnoId)
                .Concat(ZetaDia.Me.Inventory.Backpack.Where(item => actorId == item.ActorSnoId)).ToList();

            if (!items.Any())
                return 0;

            if (items.First().ItemStackQuantity > 0)
                return items.Select(i => i.ItemStackQuantity).Aggregate((a, b) => a + b);

            return items.Count;
        }

        public static long BackpackCount(int actorId)
        {
            var items = ZetaDia.Me.Inventory.Backpack.Where(item => actorId == item.ActorSnoId).ToList();

            if (!items.Any())
                return 0;

            if (items.First().ItemStackQuantity > 0)
                return items.Select(i => i.ItemStackQuantity).Aggregate((a, b) => a + b);

            return items.Count;
        }

        public static long StashCount(int actorId)
        {
            var items = ZetaDia.Me.Inventory.StashItems.Where(item => actorId == item.ActorSnoId).ToList();

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

        public static bool CurrentAnimation(int actorId, string animationName)
        {
            var actor = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(a => a.ActorSnoId == actorId);

            if (actor == null || actor.CommonData == null)
                return false;

            var result = actor.CommonData.CurrentAnimation.ToString() == animationName;

            //Logger.LogDebug("Animation for {0} ({1}) is {2} State={3} ({4})",
            //    actor.Name,
            //    actor.ActorSnoId,
            //    actor.CommonData.CurrentAnimation,
            //    actor.CommonData.AnimationState,
            //    result);

            return result;
        }

        public static bool IsBountyLevelArea(int questId)
        {
            var result = ZetaDia.ActInfo.Bounties.FirstOrDefault(q => q.Quest == (SNOQuest)questId && q.LevelAreas.Contains((SNOLevelArea)ZetaDia.CurrentLevelAreaSnoId) || q.StartingLevelArea == (SNOLevelArea)ZetaDia.CurrentLevelAreaSnoId);

            return result != null;
        }

        public static bool IsVendorWindowOpen()
        {
            return UIElements.VendorWindow != null && UIElements.VendorWindow.IsValid && UIElements.VendorWindow.IsVisible;
        }

        public static bool QuestComplete(int questId)
        {
            return ZetaDia.ActInfo.AllQuests.Any(q => q.QuestSNO == questId && q.State == QuestState.Completed);
        }


    }
}
