using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.UI.UIComponents;
using Trinity.Components.Adventurer.Util;
using Trinity.Components.Adventurer.Game.Actors;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Logger = Trinity.Components.Adventurer.Util.Logger;
using TabControl = System.Windows.Controls.TabControl;

namespace Trinity.Components.Adventurer.UI
{
    class DeveloperUI
    {
        private static TabItem _tabItem;

        internal static void RemoveTab()
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    Window mainWindow = Application.Current.MainWindow;
                    var tabs = mainWindow.FindName("tabControlMain") as TabControl;
                    if (tabs == null)
                        return;
                    tabs.Items.Remove(_tabItem);

                }
            );
        }

        private static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromRgb(67, 67, 67));

        private static Button CreateMajorButton(string buttonText, RoutedEventHandler clickHandler)
        {
            var button = new Button
            {              
                Background = Brushes.DarkSlateBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(2),
                Content = buttonText
            };
            button.Click += clickHandler;
            return button;
        }

        internal static void InstallTab()
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {

                    var mainWindow = Application.Current.MainWindow;
                    //var settings = new UniformGrid
                    //{
                    //    Columns = 1,
                    //    Background = BackgroundBrush,
                    //    Height = 160,
                    //    Margin = new Thickness(10, 10, 10, 10),
                    //};

                    //settings.Children.Add(CreateMajorButton("Configure Adventurer", ShowMainAdventurerWindow));

                    var dumpers = new StackPanel { Background = Brushes.DimGray, Height = 176, Margin = new Thickness(2, 2, 0, 2) };
                    dumpers.Children.Add(CreateTitle("Dumpers"));
                    dumpers.Children.Add(CreateButton("Map Markers", DumpMapMarkers_Click));
                    dumpers.Children.Add(CreateButton("All Actors", DumpObjects_Click));
                    dumpers.Children.Add(CreateButton("Specific Actor", DumpActor_Click));
                    dumpers.Children.Add(CreateButton("Unsupported Bounties", DumpUnsupportedBounties_Click));
                    dumpers.Children.Add(CreateButton("Scenes", DumpLevelAreaScenes_Click));
                    dumpers.Children.Add(CreateButton("Toggle MapUI", ToggleRadarUI_Click, default(Thickness), new SolidColorBrush(Colors.NavajoWhite) { Opacity = 0.2 }));

                    var coroutineHelpers = new StackPanel { Background = Brushes.DimGray, Height = 176, Margin = new Thickness(2, 2, 0, 2) };
                    coroutineHelpers.Children.Add(CreateTitle("Coroutines"));
                    coroutineHelpers.Children.Add(CreateButton("Move To Position", MoveToPosition_Click));
                    coroutineHelpers.Children.Add(CreateButton("Move To Map Marker", MoveToMapMarker_Click));
                    coroutineHelpers.Children.Add(CreateButton("Move To Actor", MoveToActor_Click));
                    coroutineHelpers.Children.Add(CreateButton("Enter LevelA rea", EnterLevelArea_Click));
                    coroutineHelpers.Children.Add(CreateButton("Clear Level Area", ClearLevelArea_Click));
                    coroutineHelpers.Children.Add(CreateButton("Clear Area For N Seconds", ClearAreaForNSeconds_Click));

                    var coroutineHelpers3 = new StackPanel { Background = Brushes.DimGray, Height = 176, Margin = new Thickness(0, 2, 2, 2) };
                    coroutineHelpers3.Children.Add(CreateTitle("Profile Tags"));
                    coroutineHelpers3.Children.Add(CreateButton("Move To Position", MoveToPositionTag_Click));
                    coroutineHelpers3.Children.Add(CreateButton("MoveToScenePosition", MoveToScenePositionTag_Click));
                    coroutineHelpers3.Children.Add(CreateButton("Interact", InteractTag_Click));
                    //coroutineHelpers3.Children.Add(CreateButton("Move To Actor", MoveToActorTag_Click));
                    //coroutineHelpers3.Children.Add(CreateButton("Enter Level Area", EnterLevelAreaTag_Click));
                    //coroutineHelpers3.Children.Add(CreateButton("Clear Level Area", ClearLevelAreaTag_Click));
                    //coroutineHelpers3.Children.Add(CreateButton("Clear Area For N Seconds", ClearAreaForNSecondsTag_Click));

                    var coroutineHelpers2 = new StackPanel { Background = Brushes.DimGray, Height = 176, Margin = new Thickness(0, 2, 2, 2) };
                    coroutineHelpers2.Children.Add(CreateTitle(" "));
                    coroutineHelpers2.Children.Add(CreateButton("Wait For N Seconds", WaitForNSeconds_Click, new Thickness(0, 2.5, 5, 2.5)));
                    coroutineHelpers2.Children.Add(CreateButton("Interact With Gizmo", InteractWithGizmo_Click, new Thickness(0, 2.5, 5, 2.5)));
                    coroutineHelpers2.Children.Add(CreateButton("Interact With Unit", InteractWithUnit_Click, new Thickness(0, 2.5, 5, 2.5)));
                    coroutineHelpers2.Children.Add(CreateButton("MoveToScene", MoveToScene_Click, new Thickness(0, 2.5, 5, 2.5)));

                    //var tests = new StackPanel { Background = Brushes.DimGray, Height = 176, Margin = new Thickness(0, 2, 2, 2) };
                    //tests.Children.Add(CreateTitle("Tests"));
                    //tests.Children.Add(CreateButton("Dump Experience", DumpExperience_Click));
                    //tests.Children.Add(CreateButton("Dump Me", DumpMe_Click));
                    //tests.Children.Add(CreateButton("Dump Bounty Quests", DumpBountyQuests_Click));
                    //tests.Children.Add(CreateButton("Dump Backpack", DumpBackpack_Click));
                    //tests.Children.Add(CreateButton("Dump Party Members", DumpParty_Click));
                    //tests.Children.Add(CreateButton("Dump Test1", DumpDynamicBounty_Click));                    
                    //tests.Children.Add(CreateButton("Dump Waypoint", DumpWaypoint_Click));



                    //var mapUiContainer = new StackPanel { Background = Brushes.DimGray, Height = 176, Margin = new Thickness(0, 2, 0, 2)};

                    //var mapUiButton = new Button();
                    //mapUiButton.Click += ToggleRadarUI_Click;
                    //mapUiButton.Content = "Toggle\r\nMap UI";
                    //mapUiButton.Margin = new Thickness(5);
                    //mapUiButton.Background = new SolidColorBrush(Colors.NavajoWhite) {Opacity = 0.2};
                    //mapUiButton.Height = 166;
                    //mapUiButton.FontWeight = FontWeights.Bold;
                    //mapUiButton.FontSize = 26;
                    //mapUiButton.Width = 140;
                    //mapUiButton.HorizontalAlignment = HorizontalAlignment.Left;
                    //mapUiButton.VerticalAlignment = VerticalAlignment.Top;
                    //mapUiContainer.Children.Add(mapUiButton);

                    var uniformGrid = new UniformGrid
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        MaxHeight = 180,
                        Columns = 4
                    };

                    //uniformGrid.Children.Add(mapUiContainer);
                    
                    uniformGrid.Children.Add(dumpers);
                    uniformGrid.Children.Add(coroutineHelpers);
                    uniformGrid.Children.Add(coroutineHelpers2);
                    uniformGrid.Children.Add(coroutineHelpers3);



                    _tabItem = new TabItem
                    {
                        Header = "Adventurer",
                        ToolTip = "Developer Tools",
                        Content = uniformGrid,
                    };

                    var tabs = mainWindow.FindName("tabControlMain") as TabControl;
                    if (tabs == null)
                        return;

                    tabs.Items.Add(_tabItem);
                }
            );
        }

        private static void ShowMainAdventurerWindow(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var configWindow = ConfigWindow.Instance;
                configWindow.Show();
            }
            catch (Exception ex)
            {
                Logger.Error("Error showing Configuration from TabUI:" + ex);
            }
        }

        #region UI Elements
        static TextBlock CreateTitle(string title)
        {
            return new TextBlock
            {
                Text = title,
                Width = 140,
                Height = 18,
                Padding = new Thickness(0, 2, 0, 0),
                Margin = new Thickness(2.5),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
        }

        static Button CreateButton(string title, RoutedEventHandler eventFunc, Thickness margin = default(Thickness), Brush backrground = null)
        {
            if (margin == default(Thickness))
            {
                margin = new Thickness(5, 2.5, 5, 2.5);
            }
            var button = new Button
            {
                Width = 140,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = margin,
                Content = title,
                Height = 20
            };
            if (backrground != null)
            {
                button.Background = backrground;
            }
            button.Click += eventFunc;
            return button;
        }
        #endregion

        private static void DumpExperience_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {

                if (!ZetaDia.IsInGame)
                    return;

                using (ZetaDia.Memory.AcquireFrame(true))
                {
                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();

                    Util.Logger.Raw("ExperienceGranted {0}", ZetaDia.Me.ExperienceGranted);
                    Util.Logger.Raw("ExperienceNextHi {0}", ZetaDia.Me.ExperienceNextHi);
                    Util.Logger.Raw("ExperienceNextLevel {0}", ZetaDia.Me.ExperienceNextLevel);
                    Util.Logger.Raw("ExperienceNextLo {0}", ZetaDia.Me.ExperienceNextLo);
                    Util.Logger.Raw("CurrentExperience {0}", ZetaDia.Me.CurrentExperience);
                    Util.Logger.Raw("ParagonCurrentExperience {0}", (long)ZetaDia.Me.ParagonCurrentExperience);
                    Util.Logger.Raw("ParagonExperienceNextLevel {0}", ZetaDia.Me.ParagonExperienceNextLevel);
                    Util.Logger.Raw("RestExperience {0}", ZetaDia.Me.RestExperience);
                    Util.Logger.Raw("RestExperienceBonusPercent {0}", ZetaDia.Me.RestExperienceBonusPercent);
                    Util.Logger.Raw("AltExperienceNextHi {0}", ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.AltExperienceNextHi));
                    Util.Logger.Raw("AltExperienceNextLo {0}", ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.AltExperienceNextLo));

                    var high = ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.AltExperienceNextHi);
                    var low = ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.AltExperienceNextLo);
                    long result = (long)high << 32 + low;
                    ulong result2 = (ulong)high << 32 + low;
                    Util.Logger.Raw("Result {0}", result);
                    Util.Logger.Raw("Result2 {0}", result2);


                    Util.Logger.Raw(" ");
                }

            }
            catch (Exception ex)
            {
                Util.Logger.Error(ex.ToString());
            }
        }

        static void ToggleRadarUI_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(CacheUI.ToggleRadarWindow);
        }

        static void DumpLevelAreaScenes_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {
                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    Util.Logger.Raw("\nCurrent Level Area {0} ({1})", AdvDia.CurrentLevelAreaId,
                        (SNOLevelArea)AdvDia.CurrentLevelAreaId);

                    ScenesStorage.Update();
                    var scenes = ScenesStorage.CurrentWorldScenes.Where(s => s.LevelAreaId == AdvDia.CurrentLevelAreaId);
                    foreach (var adventurerScene in scenes)
                    {
                        Util.Logger.Raw("{0}", adventurerScene.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.Logger.Error(ex.ToString());
            }
        }

        static void DumpActor_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                var mbox = InputBox.Show("Enter ActorSnoId", "Adventurer", string.Empty);
                if (mbox.ReturnCode == DialogResult.Cancel)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(mbox.Text))
                {
                    Util.Logger.Info("Enter an actorId");
                    return;
                }
                int actorId;
                if (!int.TryParse(mbox.Text, out actorId))
                {
                    Util.Logger.Info("Invalid actorId");
                    return;
                }
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var actor =
                        ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                            .Where(a => a.ActorSnoId == actorId)
                            .OrderBy(a => a.Distance)
                            .FirstOrDefault();
                    if (actor == null)
                    {
                        Util.Logger.Info("Actor not found");
                        return;
                    }

                    //foreach (var attribute in Enum.GetValues(typeof(ActorAttributeType)).Cast<ActorAttributeType>())
                    //{
                    //    Logger.Raw("ActorAttributeType.{0}: {1}", attribute.ToString(), actor.CommonData.GetAttribute<int>(attribute));
                    //}
                    Util.Logger.Raw(" ");
                    Util.Logger.Raw("Actor Details for actorId: {0}", actorId);
                    ObjectDumper.Write(actor, 1);
                    Logger.Raw("Untargetable: {0}", actor.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable));

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void DumpMe_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();


                    //foreach (var attribute in Enum.GetValues(typeof(ActorAttributeType)).Cast<ActorAttributeType>())
                    //{
                    //    Logger.Raw("ActorAttributeType.{0}: {1}", attribute.ToString(), actor.CommonData.GetAttribute<int>(attribute));
                    //}
                    Logger.Raw(" ");
                    ObjectDumper.Write(ZetaDia.Me, 1);
                    Logger.Raw("Bnet Hero Id: {0}", ZetaDia.Service.Hero.HeroId);
                    var heroId = ZetaDia.Service.Hero.HeroId;
                    var baseAddress = ZetaDia.Me.BaseAddress;
                    for (int i = 0; i < 10000; i = i + 4)
                    {
                        if (ZetaDia.Memory.Read<int>(IntPtr.Add(baseAddress, i)) == heroId)
                        {
                            Logger.Raw("Offset for BnetHeroId: {0}", i);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void DumpBountyQuests_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                using (ZetaDia.Memory.AcquireFrame(true))
                {
                    if (!ZetaDia.IsInGame)
                        return;
                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();
                    Dictionary<Act, SNOQuest> actBountyFinishingQuests = new Dictionary<Act, SNOQuest>
                                                    {
                                                        {Act.A1,SNOQuest.x1_AdventureMode_BountyTurnin_A1},
                                                        {Act.A2,SNOQuest.x1_AdventureMode_BountyTurnin_A2},
                                                        {Act.A3,SNOQuest.x1_AdventureMode_BountyTurnin_A3},
                                                        {Act.A4,SNOQuest.x1_AdventureMode_BountyTurnin_A4},
                                                        {Act.A5,SNOQuest.x1_AdventureMode_BountyTurnin_A5},
                                                    };

                    var quests =
                        ZetaDia.ActInfo.AllQuests.Where(q => actBountyFinishingQuests.ContainsValue(q.Quest)).ToList();
                    foreach (var quest in quests)
                    {
                        ObjectDumper.Write(quest, 1);
                    }


                    Logger.Raw(" ");
                    ObjectDumper.Write(ZetaDia.Me, 1);

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void DumpObjects_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var minimapActors =
                        ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                            .Where(
                                o =>
                                    o.IsValid && o.CommonData != null && o.CommonData.IsValid &&
                                    o.CommonData.MinimapVisibilityFlags != 0)
                            .OrderBy(o => o.Distance)
                            .ToList();
                    Logger.Raw(" ");
                    Logger.Raw("Minimap Actors (Count: {0})", minimapActors.Count);
                    foreach (var actor in minimapActors)
                    {
                        var gizmo = actor as DiaGizmo;

                        Logger.Raw(
                            "ActorId: {0}, Type: {1}, Name: {2}, Distance2d: {3}, CollisionRadius: {4}, MinimapActive: {5}, MinimapIconOverride: {6}, MinimapDisableArrow: {7} ",
                            actor.ActorSnoId,
                            actor.ActorType,
                            actor.Name,
                            actor.Position.Distance(AdvDia.MyPosition),
                            actor is DiaGizmo ? gizmo.CollisionSphere.Radius : 0,
                            actor.CommonData.GetAttribute<int>(ActorAttributeType.MinimapActive),
                            actor.CommonData.GetAttribute<int>(ActorAttributeType.MinimapIconOverride),
                            actor.CommonData.GetAttribute<int>(ActorAttributeType.MinimapDisableArrow)
                            );
                    }


                    var objects =
                        ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                            .Where(o => o.IsValid && o.CommonData != null && o.CommonData.IsValid)
                            .OrderBy(o => o.Distance)
                            .ToList();
                    Logger.Raw(" ");
                    Logger.Raw("Actors (Count: {0})", objects.Count);

                    foreach (var actor in objects)
                    {
                        var gizmo = actor as DiaGizmo;

                        Logger.Raw(
                            "ActorId: {0}, Type: {1}, Name: {2}, Distance2d: {3}, CollisionRadius: {4}, MinimapActive: {5}, MinimapIconOverride: {6}, MinimapDisableArrow: {7} ",
                            actor.ActorSnoId,
                            actor.ActorType,
                            actor.Name,
                            actor.Position.Distance(AdvDia.MyPosition),
                            actor is DiaGizmo ? gizmo.CollisionSphere.Radius : 0,
                            actor.CommonData.GetAttribute<int>(ActorAttributeType.MinimapActive),
                            actor.CommonData.GetAttribute<int>(ActorAttributeType.MinimapIconOverride),
                            actor.CommonData.GetAttribute<int>(ActorAttributeType.MinimapDisableArrow)
                            );
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void DumpBackpack_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var objects = ZetaDia.Me.Inventory.Backpack.ToList();
                    Logger.Raw(" ");
                    Logger.Raw("Actors (Count: {0})", objects.Count);

                    foreach (var actor in objects)
                    {

                        Logger.Raw(
                            "ActorId: {0}, Type: {1}, Name: {2}",
                            actor.ActorSnoId,
                            actor.ActorType,
                            actor.Name

                            );
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        private static Dictionary<int, int> _partyMembersFirstHit = new Dictionary<int, int>();
        private static Dictionary<int, int> _partyMembersSecondtHit = new Dictionary<int, int>();
        private static bool isFirstHit = true;
        static void DumpParty_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    Logger.Raw("Hooks:");
                    foreach (var hook in TreeHooks.Instance.Hooks)
                    {
                        Logger.Raw("{0}: {1}", hook.Key,string.Join(", " ,hook.Value));
                    }

                    var party = ZetaDia.Players.ToList();

                    Logger.Raw(" ");
                    Logger.Raw("Party Members (Count: {0})", party.Count);

                    foreach (var actor in party)
                    {
                        Logger.Raw("=======================================================");
                        ObjectDumper.Write(actor, 1);
                    }
                    for (var i = 0; i <= 40000; i = i + 4)
                    {
                        if (isFirstHit)
                        {
                            _partyMembersFirstHit.Add(i, ZetaDia.Memory.Read<int>(ZetaDia.Service.Party.BaseAddress + i));
                        }
                        else
                        {
                            _partyMembersSecondtHit.Add(i, ZetaDia.Memory.Read<int>(ZetaDia.Service.Party.BaseAddress + i));
                        }
                    }
                    Logger.Raw("IsFirstHit: {0}", isFirstHit);
                    if (!isFirstHit)
                    {
                        for (var i = 0; i <= 40000; i = i + 4)
                        {
                            if (_partyMembersFirstHit[i] != _partyMembersSecondtHit[i])
                            {
                                Logger.Raw("{0}: {1} - {2}", i, _partyMembersFirstHit[i], _partyMembersSecondtHit[i]);
                            }
                        }
                        _partyMembersFirstHit = new Dictionary<int, int>();
                        _partyMembersSecondtHit = new Dictionary<int, int>();
                    }
                    isFirstHit = !isFirstHit;
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void DumpMapMarkers_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();
                    var myPosition = ZetaDia.Me.Position;

                    Logger.Raw(" ");
                    Logger.Raw("Dumping Minimap Markers");

                    foreach (
                        var mapMarker in
                            ZetaDia.Minimap.Markers.CurrentWorldMarkers.OrderBy(
                                m => Vector3.Distance(myPosition, m.Position)).Take(100))
                    {

                        var locationInfo = string.Format("x=\"{0:0}\" y=\"{1:0}\" z=\"{2:0}\" ", mapMarker.Position.X,
                            mapMarker.Position.Y, mapMarker.Position.Z);
                        Logger.Raw(
                            "Id={0} MinimapTextureSnoId={1} NameHash={2} IsPointOfInterest={3} IsPortalEntrance={4} IsPortalExit={5} IsWaypoint={6} Location={7} Distance={8:N0}",
                            mapMarker.Id,
                            mapMarker.MinimapTextureSnoId,
                            mapMarker.NameHash,
                            mapMarker.IsPointOfInterest,
                            mapMarker.IsPortalEntrance,
                            mapMarker.IsPortalExit,
                            mapMarker.IsWaypoint,
                            locationInfo,
                            Vector3.Distance(myPosition, mapMarker.Position));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void DumpUnsupportedBounties_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    Logger.Raw("Unsupported Bounties:");
                    Logger.Raw(" ");

                    var bounties =
                        ZetaDia.ActInfo.Bounties.Where(
                            b =>
                                BountyDataFactory.GetBountyData((int)b.Quest) == null &&
                                b.State != QuestState.Completed)
                            .ToList();

                    var wp = ZetaDia.Actors.GetActorsOfType<GizmoWaypoint>().OrderBy(g => g.Distance).FirstOrDefault();
                    var wpNr = 0;
                    if (wp != null)
                    {
                        wpNr = wp.WaypointNumber;
                    }

                    foreach (var bountyInfo in bounties)
                    {
                        DumpBountyInfo(bountyInfo, wpNr);
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void DumpWaypoint_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();

                    DumpNearbyWaypoint();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }


        public static void DumpNearbyWaypoint()
        {
            var wp = ZetaDia.Actors.GetActorsOfType<GizmoWaypoint>().OrderBy(g => g.Distance).FirstOrDefault();
            var wpNr = 0;
            if (wp != null)
            {
                if (!UIElements.WaypointMap.IsVisible)
                    UIManager.ToggleWaypointMap();

                var name =
                    Zeta.Game.Internals.UIElement.FromName(
                        "Root.NormalLayer.WaypointMap_main.LayoutRoot.OverlayContainer.POI.entry " + wp.WaypointNumber +
                        ".LayoutRoot.Name").Text;

                Logger.Raw("{{ {0}, new WaypointData({0}, {1}, {2}, \"{3}\") }}",
                    wp.WaypointNumber, ZetaDia.CurrentLevelAreaSnoId, ZetaDia.CurrentWorldSnoId, name);

                UIManager.ToggleWaypointMap();
            }
        }

        static void DumpDynamicBounty_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();

                    CheckForDynamicBounties();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public static void CheckForDynamicBounties()
        {
            var bounties = ZetaDia.ActInfo.Bounties
                .Where(b => GetDynamicBountyTypeFromName(b.Info.DisplayName) != BountyDataFactory.DynamicBountyType.None &&
                            !BountyDataFactory.DynamicBountyDirectory.ContainsKey((int) b.Info.Quest));

            foreach (var bounty in bounties)
            {
                var type = GetDynamicBountyTypeFromName(bounty.Info.DisplayName);
                Logger.Raw("{{ {0}, DynamicBountyType.{1} }}", bounty.Info.QuestSNO, type);
            }
        }

        public static BountyDataFactory.DynamicBountyType GetDynamicBountyTypeFromName(string name)
        {
            // Note: will not work for non-english clients
            if (name.Contains("Black King's Legacy")) return BountyDataFactory.DynamicBountyType.BlackKingsLegacy;
            if (name.Contains("The Cursed Shrines")) return BountyDataFactory.DynamicBountyType.CursedShrines;
            if (name.Contains("The Bound Shaman")) return BountyDataFactory.DynamicBountyType.BoundShaman;
            if (name.Contains("Plague of Burrowers")) return BountyDataFactory.DynamicBountyType.PlagueOfBurrowers;
            return BountyDataFactory.DynamicBountyType.None;
            ;
        }

        static void DumpBountyInfo(BountyInfo bountyInfo, int waypointNumber)
        {
            Logger.Raw("// {0} - {1} ({2})", bountyInfo.Act, bountyInfo.Info.DisplayName, (int)bountyInfo.Quest);
            Logger.Raw("Bounties.Add(new BountyData");
            Logger.Raw("{");
            Logger.Raw("    QuestId = {0},", (int)bountyInfo.Quest);
            Logger.Raw("    Act = Act.{0},", bountyInfo.Act);
            Logger.Raw("    WorldId = 0, // Enter the final worldId here");
            Logger.Raw("    QuestType = BountyQuestType.SpecialEvent,");
            Logger.Raw("    StartingLevelAreaId = {0},", AdvDia.CurrentLevelAreaId);
            //Logger.Raw("    WaypointNumber = {0},", waypointNumber);
            Logger.Raw("    Coroutines = new List<ISubroutine>");
            Logger.Raw("    {");
            Logger.Raw("        // Coroutines goes here");
            Logger.Raw("    }");
            Logger.Raw("});");
            Logger.Raw(" ");

        }

        static void MoveToPosition_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();
                    Logger.Raw(" ");
                    Logger.Raw("new MoveToPositionCoroutine({3}, new Vector3({0}, {1}, {2})),",
                        (int)AdvDia.MyPosition.X, (int)AdvDia.MyPosition.Y, (int)AdvDia.MyPosition.Z,
                        AdvDia.CurrentWorldId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void MoveToScene_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                ScenesStorage.Update();
                SafeFrameLock.ExecuteWithinFrameLock(() =>
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;
                    //AdvDia.Update();
                    Logger.Raw(" ");
                    Logger.Raw("new MoveToSceneCoroutine({0}, {1}, \"{2}\"),", activeBounty,
                        AdvDia.CurrentWorldId, AdvDia.CurrentWorldScene.Name);
                }, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        //static void MoveToScenePosition_Click(object sender, RoutedEventArgs e)
        //{
        //    if (BotEvents.IsBotRunning)
        //    {
        //        BotMain.Stop();
        //        Thread.Sleep(500);
        //    }
        //    try
        //    {
        //        if (!ZetaDia.IsInGame)
        //            return;
        //        ScenesStorage.Update();
        //        SafeFrameLock.ExecuteWithinFrameLock(() =>
        //        {

        //            if (ZetaDia.Me == null)
        //                return;
        //            if (!ZetaDia.Me.IsValid)
        //                return;

        //            var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
        //                ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
        //                : 0;
        //            //AdvDia.Update();
        //            Logger.Raw(" ");
        //            var currentScenePosition = AdvDia.CurrentWorldScene.GetRelativePosition(AdvDia.MyPosition);
        //            Logger.Raw("new MoveToScenePositionCoroutine({0}, {1}, \"{2}\", new Vector3({3}f, {4}f, {5}f)),", activeBounty,
        //                AdvDia.CurrentWorldId, AdvDia.CurrentWorldScene.Name, currentScenePosition.X,
        //                currentScenePosition.Y, currentScenePosition.Z);
        //        }, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex.ToString());
        //    }
        //}

        static void MoveToMapMarker_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();
                    var objectiveMarkers = AdvDia.CurrentWorldMarkers.Where(m => m.Id >= 0 && m.Id <= 200);

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;


                    Logger.Raw(" ");
                    foreach (var objectiveMarker in objectiveMarkers)
                    {
                        Logger.Raw("new MoveToMapMarkerCoroutine({0}, {1}, {2}),", activeBounty, AdvDia.CurrentWorldId,
                            objectiveMarker.NameHash);
                        Logger.Raw(" ");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void MoveToActor_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;
                    var actors =
                        ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                            .Where(
                                a =>
                                    a.IsFullyValid() &&
                                    (a.IsInteractableQuestObject() ||
                                     (a is DiaUnit && (a as DiaUnit).CommonData.IsUnique)))
                            .OrderBy(a => a.Distance)
                            .ToList();

                    if (actors.Count == 0)
                    {
                        Logger.Raw("// Could not detect an active quest actors, you must be out of range.");
                    }
                    foreach (var actor in actors)
                    {
                        Logger.Raw("// {0} ({1}) Distance: {2}", (SNOActor)actor.ActorSnoId, actor.ActorSnoId,
                            actor.Distance);
                        Logger.Raw("new MoveToActorCoroutine({0}, {1}, {2}),", activeBounty, AdvDia.CurrentWorldId,
                            actor.ActorSnoId);
                        Logger.Raw(" ");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void EnterLevelArea_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;

                    var objectiveMarkers = AdvDia.CurrentWorldMarkers.Where(m => m.Id >= 0 && m.Id <= 200).ToList();

                    if (objectiveMarkers.Count == 0)
                    {
                        Logger.Raw(
                            "// Could not detect an active objective marker, you are either out of range or to close to it.");
                    }
                    foreach (var marker in objectiveMarkers)
                    {
                        //new EnterLevelAreaCoroutine(int questId, int sourceWorldId, int destinationWorldId, int portalMarker, int portalActorId)
                        var portal =
                            ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                                .FirstOrDefault(
                                    a => a.IsFullyValid() && a.IsPortal && a.Position.Distance(marker.Position) <= 5);
                        if (portal != null)
                        {
                            Logger.Raw("new EnterLevelAreaCoroutine({0}, {1}, {2}, {3}, {4}),", activeBounty,
                                AdvDia.CurrentWorldId, 0, marker.NameHash, portal.ActorSnoId);
                        }
                        else
                        {
                            Logger.Raw(
                                "// Could not detect the portal near the marker. You need to get a bit closer to the level entrance");
                        }
                    }
                    Logger.Raw(" ");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void ClearLevelArea_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;

                    Logger.Raw("new ClearLevelAreaCoroutine({0}),", activeBounty);
                    Logger.Raw(" ");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void ClearAreaForNSeconds_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    int seconds;
                    var mbox = InputBox.Show("How many seconds?", "Adventurer", "60");
                    if (mbox.ReturnCode == DialogResult.Cancel)
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(mbox.Text))
                    {
                        seconds = 60;
                    }
                    else
                    {
                        if (!int.TryParse(mbox.Text, out seconds))
                        {
                            Logger.Raw("// Invalid number");
                            return;
                        }
                    }

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;

                    //ClearAreaForNSecondsCoroutine(int questId, int seconds, int actorId, int marker, int radius = 30, bool increaseRadius = true)
                    Logger.Raw("new ClearAreaForNSecondsCoroutine({0}, {1}, {2}, {3}, {4}),", activeBounty, seconds, 0,
                        0, 45);
                    Logger.Raw(" ");
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void MoveToPositionTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ZetaDia.IsInGame || ZetaDia.Me == null)
                    return;

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();
                    var quest = ZetaDia.CurrentQuest;
                    var questId = quest?.QuestSnoId ?? 1;
                    var questStep = quest?.StepId ?? 1;
                    var sceneId = ZetaDia.Me.CurrentScene.SceneInfo.SNOId;
                    var sceneName = ZetaDia.Me.CurrentScene.Name;
                    var scenePosition = AdvDia.CurrentWorldScene.GetRelativePosition(ZetaDia.Me.Position);

                    Logger.Raw($@"     <MoveToPosition questId=""{questId}"" stepId=""{questStep}"" x=""{ZetaDia.Me.Position.X:F0}"" y=""{ZetaDia.Me.Position.Y:F0}"" z=""{ZetaDia.Me.Position.Z:F0}"" worldSnoId=""{ZetaDia.CurrentWorldSnoId}"" levelAreaSnoId=""{ZetaDia.CurrentLevelAreaSnoId}"" sceneSnoId=""{sceneId}"" sceneName=""{sceneName}"" sceneX=""{scenePosition.X:F0}"" sceneY=""{scenePosition.Y:F0}"" sceneZ=""{scenePosition.Z:F0}"" />");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void InteractTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ZetaDia.IsInGame || ZetaDia.Me == null)
                    return;

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();

                    var quest = ZetaDia.CurrentQuest;
                    var questId = quest?.QuestSnoId ?? 1;
                    var questStep = quest?.StepId ?? 1;
                    var sceneId = ZetaDia.Me.CurrentScene.SceneInfo.SNOId;
                    var sceneName = ZetaDia.Me.CurrentScene.Name;

                    Logger.Raw("");

                    var actors = new List<DiaObject>();
                    var bestActor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).OrderBy(a => a.Distance).FirstOrDefault(u => u.IsInteractableQuestObject());
                    if (bestActor == null)
                    {
                        Logger.Raw($@"-- Listing all potential interact targets --");
                        actors.AddRange(ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).Where(g => g.Distance < 15f).OrderBy(a => a.Distance));
                        actors.AddRange(ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(u => u.Distance < 15f && u.PetType <= 0).OrderBy(a => a.Distance));     
                    }
                    else
                    {
                        actors.Add(bestActor);
                    }

                    foreach (var actor in actors)
                    {
                        var actorId = actor?.ActorSnoId ?? 0;
                        var actorName = actor?.Name.Split('-').First() ?? string.Empty;
                        var scenePosition = actor != null ? AdvDia.CurrentWorldScene.GetRelativePosition(actor.Position) : Vector3.Zero;
                        Logger.Raw($@"     <Interact questId=""{questId}"" stepId=""{questStep}"" actorId=""{actorId}"" actorName=""{actorName}"" x=""{ZetaDia.Me.Position.X:F0}"" y=""{ZetaDia.Me.Position.Y:F0}"" z=""{ZetaDia.Me.Position.Z:F0}"" worldSnoId=""{ZetaDia.CurrentWorldSnoId}"" levelAreaSnoId=""{ZetaDia.CurrentLevelAreaSnoId}"" sceneSnoId=""{sceneId}"" sceneName=""{sceneName}"" sceneX=""{scenePosition.X:F0}"" sceneY=""{scenePosition.Y:F0}"" sceneZ=""{scenePosition.Z:F0}"" />");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void MoveToScenePositionTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ZetaDia.IsInGame || ZetaDia.Me == null)
                    return;

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();

                    var quest = ZetaDia.CurrentQuest;
                    var questId = quest?.QuestSnoId ?? 1;
                    var questStep = quest?.StepId ?? 1;
                    var sceneId = ZetaDia.Me.CurrentScene.SceneInfo.SNOId;
                    var sceneName = ZetaDia.Me.CurrentScene.Name;

                    Logger.Raw("");

                    var actor = ZetaDia.Me;
                    var actorId = actor?.ActorSnoId ?? 0;
                    var scenePosition = actor != null ? AdvDia.CurrentWorldScene.GetRelativePosition(actor.Position) : Vector3.Zero;
                    Logger.Raw($@"     <MoveToScenePosition questId=""{questId}"" stepId=""{questStep}"" actorId=""{actorId}"" worldSnoId=""{ZetaDia.CurrentWorldSnoId}"" levelAreaSnoId=""{ZetaDia.CurrentLevelAreaSnoId}"" sceneSnoId=""{sceneId}"" sceneName=""{sceneName}"" sceneX=""{scenePosition.X:F0}"" sceneY=""{scenePosition.Y:F0}"" sceneZ=""{scenePosition.Z:F0}"" />");
                   
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void MoveToMapMarkerTag_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();
                    var objectiveMarkers = AdvDia.CurrentWorldMarkers.Where(m => m.Id >= 0 && m.Id <= 200);

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;


                    Logger.Raw(" ");
                    foreach (var objectiveMarker in objectiveMarkers)
                    {
                        Logger.Raw("new MoveToMapMarkerCoroutine({0}, {1}, {2}),", activeBounty, AdvDia.CurrentWorldId,
                            objectiveMarker.NameHash);
                        Logger.Raw(" ");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void MoveToActorTag_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;
                    var actors =
                        ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                            .Where(
                                a =>
                                    a.IsFullyValid() &&
                                    (a.IsInteractableQuestObject() ||
                                     (a is DiaUnit && (a as DiaUnit).CommonData.IsUnique)))
                            .OrderBy(a => a.Distance)
                            .ToList();

                    if (actors.Count == 0)
                    {
                        Logger.Raw("// Could not detect an active quest actors, you must be out of range.");
                    }
                    foreach (var actor in actors)
                    {
                        Logger.Raw("// {0} ({1}) Distance: {2}", (SNOActor)actor.ActorSnoId, actor.ActorSnoId,
                            actor.Distance);
                        Logger.Raw("new MoveToActorCoroutine({0}, {1}, {2}),", activeBounty, AdvDia.CurrentWorldId,
                            actor.ActorSnoId);
                        Logger.Raw(" ");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void EnterLevelAreaTag_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;

                    var objectiveMarkers = AdvDia.CurrentWorldMarkers.Where(m => m.Id >= 0 && m.Id <= 200).ToList();

                    if (objectiveMarkers.Count == 0)
                    {
                        Logger.Raw(
                            "// Could not detect an active objective marker, you are either out of range or to close to it.");
                    }
                    foreach (var marker in objectiveMarkers)
                    {
                        //new EnterLevelAreaCoroutine(int questId, int sourceWorldId, int destinationWorldId, int portalMarker, int portalActorId)
                        var portal =
                            ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                                .FirstOrDefault(
                                    a => a.IsFullyValid() && a.IsPortal && a.Position.Distance(marker.Position) <= 5);
                        if (portal != null)
                        {
                            Logger.Raw("new EnterLevelAreaCoroutine({0}, {1}, {2}, {3}, {4}),", activeBounty,
                                AdvDia.CurrentWorldId, 0, marker.NameHash, portal.ActorSnoId);
                        }
                        else
                        {
                            Logger.Raw(
                                "// Could not detect the portal near the marker. You need to get a bit closer to the level entrance");
                        }
                    }
                    Logger.Raw(" ");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void ClearLevelAreaTag_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;

                    Logger.Raw("new ClearLevelAreaCoroutine({0}),", activeBounty);
                    Logger.Raw(" ");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void ClearAreaForNSecondsTag_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    int seconds;
                    var mbox = InputBox.Show("How many seconds?", "Adventurer", "60");
                    if (mbox.ReturnCode == DialogResult.Cancel)
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(mbox.Text))
                    {
                        seconds = 60;
                    }
                    else
                    {
                        if (!int.TryParse(mbox.Text, out seconds))
                        {
                            Logger.Raw("// Invalid number");
                            return;
                        }
                    }

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();

                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;

                    //ClearAreaForNSecondsCoroutine(int questId, int seconds, int actorId, int marker, int radius = 30, bool increaseRadius = true)
                    Logger.Raw("new ClearAreaForNSecondsCoroutine({0}, {1}, {2}, {3}, {4}),", activeBounty, seconds, 0,
                        0, 45);
                    Logger.Raw(" ");
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void WaitForNSeconds_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    int seconds;
                    var mbox = InputBox.Show("How many seconds?", "Adventurer", "5");
                    if (mbox.ReturnCode == DialogResult.Cancel)
                    {
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(mbox.Text))
                    {
                        seconds = 60;
                    }
                    else
                    {
                        if (!int.TryParse(mbox.Text, out seconds))
                        {
                            Logger.Raw("// Invalid number");
                            return;
                        }
                    }

                    Logger.Raw("new WaitCoroutine({0}),", seconds * 1000);
                    Logger.Raw(" ");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void InteractWithUnit_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {

                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();
                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;
                    var actors =
                        ZetaDia.Actors.GetActorsOfType<DiaUnit>(true)
                            .Where(a => a.IsFullyValid() && a.IsInteractableQuestObject())
                            .OrderBy(a => a.Distance)
                            .ToList();

                    if (actors.Count == 0)
                    {
                        Logger.Raw("// Could not detect an active quest unit, you must be out of range.");
                    }
                    foreach (var actor in actors)
                    {
                        var marker =
                            AdvDia.CurrentWorldMarkers.FirstOrDefault(m => m.Position.Distance(actor.Position) < 30);
                        int markerId = 0;
                        if (marker != null)
                        {
                            markerId = marker.NameHash;
                        }
                        //InteractWithUnitCoroutine(int questId, int worldId, int actorId, int marker, int interactAttemps = 1, int secondsToSleepAfterInteraction = 1, int secondsToTimeout = 4)
                        Logger.Raw("// {0} ({1}) Distance: {2}", (SNOActor)actor.ActorSnoId, actor.ActorSnoId,
                            actor.Distance);
                        Logger.Raw("new InteractWithUnitCoroutine({0}, {1}, {2}, {3}, {4}),", activeBounty,
                            AdvDia.CurrentWorldId, actor.ActorSnoId, markerId, 5);
                        Logger.Raw(" ");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        static void InteractWithGizmo_Click(object sender, RoutedEventArgs e)
        {
            if (BotEvents.IsBotRunning)
            {
                BotMain.Stop();
                Thread.Sleep(500);
            }
            try
            {
                if (!ZetaDia.IsInGame)
                    return;
                using (ZetaDia.Memory.AcquireFrame(true))
                {


                    if (ZetaDia.Me == null)
                        return;
                    if (!ZetaDia.Me.IsValid)
                        return;

                    ZetaDia.Actors.Update();
                    //AdvDia.Update();
                    var activeBounty = ZetaDia.ActInfo.ActiveBounty != null
                        ? (int)ZetaDia.ActInfo.ActiveBounty.Quest
                        : 0;
                    var actors =
                        ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                            .Where(a => a.IsFullyValid() && a.IsInteractableQuestObject())
                            .OrderBy(a => a.Distance)
                            .ToList();

                    if (actors.Count == 0)
                    {
                        Logger.Raw("// Could not detect an active quest gizmo, you must be out of range.");
                    }
                    foreach (var actor in actors)
                    {
                        var marker =
                            AdvDia.CurrentWorldMarkers.FirstOrDefault(m => m.Position.Distance(actor.Position) < 30);
                        int markerId = 0;
                        if (marker != null)
                        {
                            markerId = marker.NameHash;
                        }
                        //InteractWithGizmoCoroutine(int questId, int worldId, int actorId, int marker, int interactAttemps = 1, int secondsToSleepAfterInteraction = 1, int secondsToTimeout = 10)
                        Logger.Raw("// {0} ({1}) Distance: {2}", (SNOActor)actor.ActorSnoId, actor.ActorSnoId,
                            actor.Distance);
                        Logger.Raw("new InteractWithGizmoCoroutine({0}, {1}, {2}, {3}, {4}),", activeBounty,
                            AdvDia.CurrentWorldId, actor.ActorSnoId, markerId, 5);
                        Logger.Raw(" ");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

    }
}
