using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer
{
    public static class AdvDia
    {
        public static int CurrentWorldId => Core.Player.WorldSnoId;
        public static MainGridProvider MainGridProvider => (MainGridProvider)Zeta.Bot.Navigation.Navigator.SearchGridProvider;        
        public static DefaultNavigationProvider Navigator => Zeta.Bot.Navigation.Navigator.NavigationProvider as DefaultNavigationProvider;
        public static int CurrentWorldDynamicId => Core.Player.WorldDynamicId;
        public static int CurrentLevelAreaId => Core.Player.LevelAreaId;
        public static Vector3 MyPosition => Core.Player.Position;
        public static float MyZDiff(Vector3 toPosition) => Math.Abs(toPosition.Z - MyPosition.Z);   
        public static WorldScene CurrentWorldScene => Core.Scenes.CurrentScene;
        public static bool IsInTown => Core.Player.IsInTown;
        public static int BattleNetHeroId => Core.Player.HeroId;
        public static List<MinimapMarker> CurrentWorldMarkers  => ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(m => m.IsValid && m.NameHash != -1).ToList();
        public static RiftQuest RiftQuest => new RiftQuest();
        public static IEnumerable<TrinityItem> StashAndBackpackItems => Core.Inventory;
        public static SNOAnim MyCurrentAnimation => Core.Player.CurrentAnimation;
        public static string BattleNetBattleTagName => Core.Player.BattleTag;
        public static bool IsInArchonForm => ZetaDia.Me.GetAllBuffs().Any(b => b.SNOId == (int)SNOPower.Wizard_Archon); 
    }

}