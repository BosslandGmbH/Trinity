using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Trinity.Coroutines.Resources;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Debug;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;
using Trinity.Items;
using Trinity.Reference;
using Trinity.UI.UIComponents;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Trinity.Framework.Actors;
using Trinity.Framework.Actors.ActorTypes;
using Logger = Trinity.Framework.Helpers.Logger;
using MemoryHelper = Trinity.Framework.Helpers.MemoryHelper;
using TrinityItemQuality = Trinity.Settings.TrinityItemQuality;
using UIElement = Zeta.Game.Internals.UIElement;

// For Debug Watch Panel Namespace.
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.UX;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Attributes;
using Trinity.Framework.Objects.Memory.Sno;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Items.Sorting;
using Trinity.Settings;
using Trinity.UI.Visualizer;
using ScenesStorage = Trinity.Components.Adventurer.Game.Exploration.ScenesStorage;

namespace Trinity.UI
{
    internal class TabUi
    {
        private static UniformGrid _tabGrid;
        private static TabItem _tabItem;
        private static DateTime LastStartedConvert = DateTime.UtcNow;

        internal static void InstallTab()
        {
            // Debugging YAR causing IsEnabled from another thread.

            var hasAccessToApp = Application.Current.CheckAccess();

            if (!hasAccessToApp)
                Logger.Log("Current Thread Id={0} Name='{1}' cannot access the current application. CanAccessDispatcher={2}",
                    Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name, Application.Current.Dispatcher.CheckAccess());

            //Logger.Log($"ApplicationDispatcherThreadId={Application.Current.Dispatcher.Thread.ManagedThreadId} " +
            //           $"CurrenThreadId={Thread.CurrentThread.ManagedThreadId}" +
            //           $"CanAccessMainWindow={Application.Current.MainWindow.CheckAccess()}");

            if (!DemonBuddyUI.MainWindow.CheckAccess())
            {
                Logger.Log("Current Thread {0} '{1}' cannot access MainWindow Dispatcher",
                    Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);

                return;
            }

            DemonBuddyUI.MainWindow.Dispatcher.Invoke(() =>
            {

                var mainWindow = DemonBuddyUI.MainWindow;

                _tabGrid = new UniformGrid
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Columns = 4,
                    MaxHeight = 180,
                    Margin = new Thickness(0, 0, 16, 0)
                };

                CreateStretchyGroup(string.Empty, new List<Control>
                        {
                            CreateMajorButton("Configure", ShowMainTrinityUIEventHandler),
                            CreateMajorButton("Open Visualizer", OpenRadarButtonHandler)
                        });

                CreateGroup("Items", new List<Control>
                        {
                            CreateButton("Sort Backpack", SortBackEventHandler),
                            CreateButton("Sort Stash", SortStashEventHandler),
                            //CreateButton("Clean Stash", CleanStashEventHandler),
                            CreateButton("Convert to Magic", btnClick_ConvertToBlue),
                            CreateButton("Convert to Common", btnClick_ConvertToCommon),
                            CreateButton("Convert to Rare", btnClick_ConvertToRare),
                        });

                CreateGroup("Debugging", new List<Control>
                        {
                            CreateButton("Find New ActorIds", GetNewActorSNOsEventHandler),
                            CreateButton("Log Invalid Items", LogInvalidHandler),
                            CreateButton("> Gizmo Attribtues", StartGizmoTestHandler),
                            CreateButton("> Unit Attribtues", StartUnitTestHandler),
                            CreateButton("> Player Attribtues", StartPlayerTestHandler),
                            //CreateButton("Dump Item Powers", StartDataTestHandler),
                            //CreateButton("> Buff Test", StartBuffTestHandler),
                            //CreateButton("> Stop Tests", StopTestHandler),

                });

                CreateGroup("Tools", new List<Control>
                        {
                            CreateButton("Dump My Build", DumpBuildEventHandler),
                            CreateButton("Drop Legendaries", DropLegendariesEventHandler),
                            //CreateButton("Log Run Time", btnClick_LogRunTime),
                            CreateButton("Open Log File", OpenLogFileHandler),
                            CreateButton("Open Settings File", OpenSettingsFileHandler),
                            //CreateButton("Log Town Actor", LogTownActor),
                            //CreateButton("Test UIElement", TestUIElement),                                                       
                            //CreateButton("Test SNOReader", TestSNOReader),
                            //CreateButton("Dump Offsets", DumpOffsets),
                            //CreateButton("Start Internals", StartInternals),
                            //CreateButton("Stop Internals", StopInternals),
                            //CreateButton("Clsoe Vendor", CloseVendorWindowTest),

                      

                            //CreateButton("Test", TestUIElement),

                            CreateButton("Upgrade Rares", UpgradeBackpackRares),
                            //CreateButton("Extract Powers", ExtractBackpackPowers),
                            CreateButton("ItemList Check", btnClick_TestItemList),
                            

                            //CreateButton("Stash Test", StashItems),
                            //CreateButton("Test Internals", TestInternals),
                        });

                //CreateButton("Scan UIElement", btnClick_ScanUIElement)
                //CreateButton("Reload Item Rules", ReloadItemRulesEventHandler);                    
                //CreateButton("Show Cache", ShowCacheWindowEventHandler);
                //CreateButton("Open Radar", OpenRadarButtonHandler);
                //CreateButton("Start Progression", StartProgressionTestHandler);             
                //CreateButton("Stop Progression", StopProgressionTestHandler);                                    
                //CreateButton("Cache Test", CacheTestCacheEventHandler);                    
                //CreateButton("Special Test", btnClick_SpecialTestHandler);
                //CreateButton("1000 Rare => Magic", btnClick_MassConvertRareToMagic);
                //CreateButton("Move to Stash", btnClick_MoveToStash);
                //CreateButton("Move to Cube", btnClick_MoveToCube);
                //CreateButton("Rares => Legendary", btnClick_UpgradeRares);
                //CreateButton("Test1", btnClick_Test1);
                //CreateButton("Test2", btnClick_HijackTest);

                _tabItem = new TabItem
                {
                    Header = "Trinity",
                    Content = _tabGrid
                };

                var tabs = mainWindow.FindName("tabControlMain") as TabControl;
                if (tabs == null)
                    return;

                tabs.Items.Add(_tabItem);

            });
        }

        private static void OpenSettingsFileHandler(object sender, RoutedEventArgs e)
        {
            string logFile = "";

            try
            {
                logFile = TrinityStorage.GetSettingsFilePath();

                if (File.Exists(logFile))
                {
                    Process.Start(logFile);
                }
                else
                {
                    Logger.LogError("Unable to open settings file {0} - file does not exist", logFile);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error opening settings file: {0} {1}", logFile, ex.Message);
            }
        }


        /**************
         * 
         * WARNING
         * 
         * ALWAYS surround your RoutedEventHandlers in try/catch. Failure to do so will result in Demonbuddy CRASHING if an exception is thrown.
         * 
         * WARNING
         *  
         *************/

        private static void DumpOffsets(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var dir = Path.Combine(FileManager.DemonBuddyPath, "Dumps/");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();
                    Core.Actors.Update();
                    //var obj = ZetaDia.Actors.GetActorsOfType<DiaObject>().FirstOrDefault();
                    //if (obj != null)
                    //    File.WriteAllText(Path.Combine(FileManager.DemonBuddyPath, $"Dumps/{obj?.GetType().Name}.txt"), obj.DumpOffsets());

                    //var wall = Core.Actors.GetActorsOfType<TrinityActor>().FirstOrDefault(a => a.ActorSnoId == (int) SNOActor.a2dun_Zolt_Sand_Wall);

                    //if (wall == null)
                    //    return;

                    //var scan = new MemoryScan(wall.RActor.BaseAddress, 4000);

                    //Logger.LogRaw(scan.Floats.ToString());

                    //Logger.Log($"WorldEnv == {Core.World.EnvironmentType}");

                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void OpenRadarButtonHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataContext = VisualizerViewModel.Instance;

                VisualizerViewModel.Window = UILoader.CreateNonModalWindow(
                    "Visualizer\\Visualizer.xaml",
                    "Radar Visualizer",
                    dataContext,
                    dataContext.WindowWidth,
                    dataContext.WindowHeight
                    );

                VisualizerViewModel.Window.ContentRendered += (o, args) => VisualizerViewModel.IsWindowOpen = true;
                VisualizerViewModel.Window.Closed += (o, args) => VisualizerViewModel.IsWindowOpen = false;
                VisualizerViewModel.Window.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error Opening Radar Window:" + ex);
            }
        }

        private static void btnClick_LogRunTime(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Bot {0} has been running for {1} hours {2} minutes and {3} seconds", ZetaDia.PlayerData.HeroName, GameStats.Instance.RunTime.Hours, GameStats.Instance.RunTime.Minutes, GameStats.Instance.RunTime.Seconds);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception {0}", ex);
            }
        }

        private static void btnClick_TestItemList(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();
                    Core.Player.Update();
                    ScenesStorage.Update();
                    Core.Update(true);
                    DebugUtil.ItemListTest();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception {0}", ex);
            }
        }

        public static void StashItems(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                if (!BotMain.IsRunning)
                {
                    //ActorManager.Start();
                    TaskDispatcher.Start(ret => Coroutines.Town.StashItems.Execute(), o => o == null || (RunStatus)o != RunStatus.Running);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error running StashItems: " + ex);
            }
        }

        private static void LogInvalidHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                DebugUtil.LogInvalidItems();
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception {0}", ex);
            }
        }

        private static void CacheTestCacheEventHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Finished Cache Test");
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception {0}", ex);
            }
        }

        private static void StartProgressionTestHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                //RiftProgression.ThreadStart();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error Starting Rift Progression : " + ex);
            }
        }

        private static void StartUnitTestHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var unitAtts = new Dictionary<int, HashSet<ActorAttributeType>>();
                var unitLastDamage = new Dictionary<int, float>();

                if (!ZetaDia.IsInGame)
                    return;

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();

                    Func<DiaObject, bool> isValid = u => u != null && u.IsValid && u.CommonData != null && u.CommonData.IsValid && !u.CommonData.IsDisposed;
                    var testunits = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(u => isValid(u) && u.RActorId != ZetaDia.Me.RActorId && ZetaDia.Me.TeamId != u.TeamId).ToList();
                    var testunit = testunits.OrderBy(u => u.Distance).FirstOrDefault();
                    if (testunit == null || testunit.CommonData == null)
                    {
                        testunit = ZetaDia.Me;
                    }

                    var acd = MemoryWrapper.Create<ActorCommonData>(testunit.CommonData.BaseAddress);
                    var atts = new Trinity.Framework.Objects.Memory.Attributes.Attributes(acd.FastAttributeGroupId);
                    Logger.Log($"-- Dumping Attribtues for {acd.Name} (Sno={acd.ActorSnoId} Ann={acd.AnnId}) at {acd.Position} ----");
                    Logger.Log(atts + "\r\n");
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error Starting LazyCache: " + ex);
            }
        }

        private static void StartPlayerTestHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var unitAtts = new Dictionary<int, HashSet<ActorAttributeType>>();
                var unitLastDamage = new Dictionary<int, float>();

                if (!ZetaDia.IsInGame)
                    return;

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();
                    //var testunit = ZetaDia.Me;

                    Func<DiaObject, bool> isValid = u => u != null && u.IsValid && u.CommonData != null && u.CommonData.IsValid && !u.CommonData.IsDisposed;
                    var testunits = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where(u => isValid(u) && u.RActorId == ZetaDia.Me.RActorId).ToList();
                    var testunit = testunits.OrderBy(u => u.Distance).FirstOrDefault();

                    var acd = MemoryWrapper.Create<ActorCommonData>(testunit.CommonData.BaseAddress);
                    var atts = new Trinity.Framework.Objects.Memory.Attributes.Attributes(acd.FastAttributeGroupId);
                    Logger.Log($"-- Dumping Attribtues for {acd.Name} (Sno={acd.ActorSnoId} Ann={acd.AnnId}) at {acd.Position} ----");
                    Logger.Log(atts + "\r\n");
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error Starting LazyCache: " + ex);
            }
        }

        private static void StartDataTestHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                if (!ZetaDia.IsInGame)
                    return;

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();
                    Core.Update();


                    //ClassMapper.MapRecursively(SnoManager.Groups.Actor.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Monster.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Hero.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.StringList.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Globals.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.GameBalance.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Actor.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.LevelArea.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Power.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.TreasureClass.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Act.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Account.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Scene.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.SceneGroup.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.Worlds.DataType);
                    //ClassMapper.MapRecursively(SnoManager.Groups.SkillKit.DataType);

                    //var powers = SnoManager.Groups.Power;
                    //var skillkits = SnoManager.Groups.SkillKit;
                    //var actors = SnoManager.Groups.Actor;
                    //var GameBalance = SnoManager.Groups.GameBalance;
                    //var monster = SnoManager.Groups.Monster;
                    //var test = SnoManager.StringList.GetStringList(SnoStringListType.Affixes);

                    //StringList = Core.CreateGroup<SnoStringList>(SnoType.StringList);
                    //Monster = Core.CreateGroup<SnoStringList>(SnoType.Monster);
                    //Hero = Core.CreateGroup<SnoStringList>(SnoType.Hero);
                    //GameBalance = Core.CreateGroup<SnoStringList>(SnoType.GameBalance);
                    //Globals = Core.CreateGroup<SnoStringList>(SnoType.Globals);
                    //Actor = Core.CreateGroup<SnoStringList>(SnoType.Actor);
                    //Account = Core.CreateGroup<SnoStringList>(SnoType.Account);
                    //Globals = Core.CreateGroup<SnoStringList>(SnoType.Globals);
                    //LevelArea = Core.CreateGroup<SnoStringList>(SnoType.LevelArea);
                    //Act = Core.CreateGroup<SnoStringList>(SnoType.Act);
                    //TreasureClass = Core.CreateGroup<SnoStringList>(SnoType.TreasureClass);
                    //Power = Core.CreateGroup<SnoStringList>(SnoType.Power);
                    //TimedEvent = Core.CreateGroup<SnoStringList>(SnoType.TimedEvent);

                    //test = SnoManager.Groups.Globals;
                    //ClassMapper.MapRecursively(test.DataType);
                    //test = SnoManager.Groups.GameBalance;
                    //ClassMapper.MapRecursively(test.DataType);

                    //var map = test.DataType.Map;
                    //var symbols = SymbolManager.Tables;
                    // SymbolManager.GenerateEnums();

                    //var count = 0;
                    //var foundSnoIds = new HashSet<int>();
                    //var regex = new Regex(@"(?<min>[\d.]+)[-–](?<max>[\d.]+)", RegexOptions.Compiled);

                    //foreach (var item in Inventory.AllItems)
                    //{
                    //    if (item == null)
                    //        continue;

                    //    if (foundSnoIds.Contains(item.ActorSnoId))
                    //    {
                    //        continue;
                    //    }

                    //    Internals.Objects.Item itemRef;
                    //    if (!Legendary.TryGetItemByActorSnoId(item.ActorSnoId, out itemRef))
                    //    {
                    //        continue;
                    //    }

                    //    var match = regex.Match(itemRef.LegendaryAffix);

                    //    double max;
                    //    double min;

                    //    if (match.Groups.Count == 3)
                    //    {
                    //        min = (double)Convert.ChangeType(match.Groups[1].Value, typeof(double));
                    //        max = (double)Convert.ChangeType(match.Groups[2].Value, typeof(double));
                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }

                    //    if (Math.Abs(min - max) < double.Epsilon || min <= 0 || max <= 0)
                    //    {
                    //        continue;
                    //    }

                    //    var passive = item.Attributes.GetAttributeItem(ActorAttributeType.ItemPowerPassive);
                    //    if (passive == null)
                    //        continue;

                    //    var index = itemRef.LegendaryAffix.IndexOf(max.ToString()) + max.ToString().Length;
                    //    var isPercent = itemRef.LegendaryAffix[index] == '%';

                    //    if (!ItemDataUtils.ItemPassivePowers.ContainsKey(item.ActorSnoId))
                    //    {
                    //        Logger.LogRaw($"    {{ {item.ActorSnoId}, new ItemPowerDescripter({passive.Key.ModifierId}, {passive.Key.Value}, {min}, {max}, {isPercent.ToString().ToLower()}) }}, // {item.Name} ({passive.Modifer}) {itemRef.LegendaryAffix}");
                    //        foundSnoIds.Add(item.ActorSnoId);
                    //    }
                    //    count++;
                    //}

                    //if (count == 0)
                    //    Logger.Log("No new passive powers were found");

                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }


        private static void OpenLogFileHandler(object sender, RoutedEventArgs e)
        {
            string logFile = "";
            try
            {
                string exePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                int myPid = Process.GetCurrentProcess().Id;
                DateTime startTime = Process.GetCurrentProcess().StartTime;
                if (exePath != null)
                    logFile = Path.Combine(exePath, "Logs", myPid + " " + startTime.ToString("yyyy-MM-dd HH.mm") + ".txt");

                if (File.Exists(logFile))
                    Process.Start(logFile);
                else
                {
                    Logger.LogError("Unable to open log file {0} - file does not exist", logFile);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error opening log file: {0} {1}", logFile, ex.Message);
            }
        }

        internal static Regex NameRegex = new Regex(@"-\d+", RegexOptions.Compiled);

        private static void LogTownActor(object sender, RoutedEventArgs e)
        {
            string logFile = "";
            try
            {
                using (new AquireFrameHelper())
                {
                    var nearestActor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                        .Where(a => a.IsFullyValid() && (a is DiaUnit || a is DiaGizmo) && a.CommonData.SummonedByACDId <= 0 &&
                        a.ACDId != ZetaDia.ActivePlayerACDId).OrderBy(a => a.Distance).FirstOrDefault();

                    if (nearestActor == null)
                        return;

                    var internalName = NameRegex.Replace(nearestActor.Name, "");

                    Components.Adventurer.Util.Logger.Raw($@"
            public static TownActor {internalName} = new TownActor
            {{
                Name = ""{internalName}"",
                InternalName = ""{internalName}"",
                Act = Act.{ZetaDia.CurrentAct},
                ActorId = {nearestActor.ActorSnoId},
                InteractPosition = new Vector3({ZetaDia.Me.Position.X}f,{ZetaDia.Me.Position.Y}f,{ZetaDia.Me.Position.Z}f),
                Position = new Vector3({nearestActor.Position.X}f,{nearestActor.Position.Y}f,{nearestActor.Position.Z}f),
                IsAdventurerMode = {(ZetaDia.WorldType == Act.OpenWorld).ToString().ToLower()},
                ServiceType = ServiceType.{GetMerchantType(nearestActor)},
                IsGizmo = {(nearestActor is DiaGizmo).ToString().ToLower()},
                IsUnit = {(nearestActor is DiaUnit).ToString().ToLower()},
                WorldSnoId = {ZetaDia.CurrentWorldSnoId},
                LevelAreaId = {ZetaDia.CurrentLevelAreaSnoId},
            }};              
                    ");
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error in LogTownActor: {0} {1}", logFile, ex.Message);
            }
        }

        private static void TestUIElement(object sender, RoutedEventArgs e)
        {
            string logFile = "";
            try
            {
                using (new AquireFrameHelper())
                {
                    var uiMapT = UXHelper.UIMapByType;

                    var recipieRadios = uiMapT[ControlType.RecipeListRadioButton].Where(u => u.IsVisible);

                    var text = uiMapT[ControlType.Text].Where(u => u.IsVisible);

                    var ibuttons = uiMapT[ControlType.ItemButton].Where(u => u.IsVisible);

                    var hotbarButtons = uiMapT[ControlType.HotbarButton].Where(u => u.IsVisible);

                    var allVisible = UXHelper.UIMap.Where(u => u.Value.IsVisible);

                    // [23194B50] Mouseover: 0xE062F8B5040F3076, Name: Root.NormalLayer.vendor_dialog_mainPage.tab_3

                    var test = GameUI.GamePotion;

                    foreach (var el in UIElement.UIMap)
                    {
                        if (el.Name.ToLower().Contains("minimap"))
                        {
                            var mapel = el;
                        }
                    }

                    //var el = UIElement.FromHash(0xE062F8B5040F3076);
                    //el.Click();

                    Logger.Warn($"CurrentDifficulty={Core.MemoryModel.Storage.CurrentDifficulty}");

                    //var elName = "Root.NormalLayer.gamemenu_dialog.gamemenu_bkgrnd.GameParams.GameParams.RightButtonStackContainer.Difficulty";
                    //var el = UIElement.FromName(elName);
                    //if (el != null)
                    //{
                    //    el.Log();
                    //    el.GetChildren().Where(c => c != null).ForEach(c => c.Log());
                    //}
                    //else
                    //{
                    //    Logger.Log($"UIElement not found {elName}");
                    //}

                    //var elsA = new List<UIElement>();
                    //var elsB = new List<UIElement>();
                    //foreach (var el in UIElement.UIMap)
                    //{
                    //    if (!el.IsValid) continue;
                    //    try
                    //    {
                    //        elsA.AddRange(el.FindDecedentsWithText("Master"));
                    //        elsB.AddRange(el.FindDecendentsWithName("Difficulty"));
                    //    }
                    //    catch (Exception) { }

                    //}

                    //var elName1 = "Root.NormalLayer.minimap_dialog_backgroundScreen.minimap_dialog_pve.button_map";
                    //var el1 = UIElement.FromName(elName1);

                    //var test = UIElement.FromHash(3401232850578496395);
                    //var ptr = ZetaDia.Memory.Read<IntPtr>(test.BaseAddress - 0x170);
                    //var difficulty = ZetaDia.Memory.ReadString(ptr, Encoding.UTF8);
                    //Logger.Warn(difficulty + " (A)");

                    //var test2 = UIElement.FromHash(17922421684644209059).Text;
                    //Logger.Warn(test2);

                    //if (el != null)
                    //{
                    //    el.Log();
                    //    el.GetChildren().Where(c => c != null).ForEach(c => c.Log());
                    //}
                    //else
                    //{
                    //    Logger.Log($"UIElement not found {elName}");
                    //}

                    //elName = "Root.NormalLayer.gamemenu_dialog.gamemenu_bkgrnd.GameParams.GameParams.RightButtonStackContainer";
                    //el = UIElement.FromName(elName);
                    //if (el != null)
                    //{
                    //    el.Log();
                    //    el.GetChildren().Where(c => c != null).ForEach(c => c.Log());
                    //}
                    //else
                    //{
                    //    Logger.Log($"UIElement not found {elName}");
                    //}

                    //Root.NormalLayer.BattleNetStore_main.LayoutRoot.OverlayContainer.CurrencyPurchase.PurchaseButtonStackPanel1.PurchaseButtonTemplate2.TextDefault.PriceBannerTemplate.DiscountBanner;

                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error in TestUIElement: {0} {1}", logFile, ex.Message);
            }
        }

        private static void TestSNOReader(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ZetaDia.Memory.AcquireFrame(true))
                {
                    //var snoreader = (SNOReader)Activator.CreateInstance(typeof(SNOReader),
                    //   BindingFlags.NonPublic | BindingFlags.Instance,
                    //   null, new object[] { }, null);

                    //string[] names = Enum.GetNames(typeof(ClientSNOTable));
                    //for (int i = 0; i < names.Length; i++)
                    //{
                    //    var table = snoreader[(ClientSNOTable)i];

                    //    Logger.Log($"SNOTable TableType={table.TableType} IsValid={table.IsValid} IsDisposed={table.IsDisposed} IsDisposing={table.IsDisposing}");

                    //    if (table.TableType == ClientSNOTable.Actor)
                    //    {
                    //        //var recordPtr = table.GetRecordPtr((int)SNOActor.Wizard_Female);
                    //        var record = table.GetRecord<SNORecordActor>((int)SNOActor.Wizard_Female);
                    //        Logger.Log($"SNOActor.Wizard_Female MonsterSnoId={record.MonsterSnoId}  Sphere.Center={record.Sphere.Center}");
                    //        record.Dispose();
                    //    }
                    //}
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error in TestSNOReader: {0} {1}");
            }
        }

        //private static void TestInternals(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        using (ZetaDia.Memory.AcquireFrame())
        //        {
        //           // ZetaInternals.Update();

        //            //CachedActorManager.Update();

        //            //var items = CachedActorManager.Items;
        //            //var firstItem = items.FirstOrDefault();
        //            //if (firstItem != null)
        //            //    Logger.Log($"Item: {firstItem.Name}");

        //            foreach (var entry in SnoManager.GameBalance.Cache)
        //            {
        //                var gbId = entry.Key;
        //                var record = entry.Value;

        //                if (record is ItemTypeRecord)
        //                {
        //                    var itemTypeRecord = (ItemTypeRecord)record;
        //                    var name = Encoding.UTF8.GetString(MemoryHelper.GetMinByteArray(itemTypeRecord.InternalName));
        //                    Logger.Log($"{name} = {gbId},");
        //                }
        //            }

        //            //var tItems = CachedActorManager.TrinityItems;
        //            // var test = ZetaInternals.ACDManager;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError("Error in TestInternals: {0} {1}");
        //    }
        //}

        private static void StartInternals(object sender, RoutedEventArgs e)
        {
            try
            {
                //ActorManager.Start();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in StartInternals: {0} {1}");
            }
        }

        private static void StopInternals(object sender, RoutedEventArgs e)
        {
            try
            {
                //ActorManager.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in StopInternals: {0} {1}");
            }
        }

        private static void CloseVendorWindowTest(object sender, RoutedEventArgs e)
        {
            try
            {
                UIElement.FromHash(0x109597E125942DA4).Click();
                UIElement.FromHash(0xF98A8466DE237BD5).Click();

                //Mouseover: 0x109597E125942DA4, Name: Root.NormalLayer.shop_dialog_mainPage.button_exit
                //Mouseover: 0xF98A8466DE237BD5, Name: Root.NormalLayer.vendor_dialog_mainPage.vendor_button_exit
                //Mouseover: 0x617664A4EFBC3AC2, Name: Root.NormalLayer.rift_dialog_mainPage.LayoutRoot.CloseButton
                //Mouseover: 0x368FF8C552241695, Name: Root.NormalLayer.inventory_dialog_mainPage.inventory_button_exit
                //Mouseover: 0xEDE91C4942014134, Name: Root.NormalLayer.SkillPane_main.LayoutRoot.CloseButton
                //Mouseover: 0x368FF8C552241695, Name: Root.NormalLayer.inventory_dialog_mainPage.inventory_button_exit
                //Mouseover: 0xF98A8466DE237BD5, Name: Root.NormalLayer.vendor_dialog_mainPage.vendor_button_exit
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in StopInternals: {0} {1}");
            }
        }

        private static ServiceType GetMerchantType(DiaObject nearestActor)
        {
            var internalNameLower = nearestActor.Name.ToLower();

            if (internalNameLower.Contains("fence"))
                return ServiceType.Fence;

            if (internalNameLower.Contains("miner"))
                return ServiceType.Miner;

            if (internalNameLower.Contains("collector"))
                return ServiceType.Collector;

            if (internalNameLower.Contains("quartermaster"))
                return ServiceType.QuarterMaster;

            if (internalNameLower.Contains("peddler"))
                return ServiceType.Peddler;

            if (internalNameLower.Contains("healer"))
                return ServiceType.Healing;

            return default(ServiceType);
        }

        private static void StartGizmoTestHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var unitAtts = new Dictionary<int, HashSet<ActorAttributeType>>();
                var unitLastDamage = new Dictionary<int, float>();

                if (!ZetaDia.IsInGame)
                    return;

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();
                    Func<DiaObject, bool> isValid = u => u != null && u.IsValid && u.CommonData != null && u.CommonData.IsValid && !u.CommonData.IsDisposed;
                    var testunits = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).Where(u => isValid(u) && u.RActorId != ZetaDia.Me.RActorId).ToList();
                    var testunit = testunits.OrderBy(u => u.Distance).FirstOrDefault();
                    if (testunit == null || testunit.CommonData == null)
                    {
                        return;
                    }

                    var acd = MemoryWrapper.Create<ActorCommonData>(testunit.CommonData.BaseAddress);
                    var atts = new Trinity.Framework.Objects.Memory.Attributes.Attributes(acd.FastAttributeGroupId);
                    Logger.Log($"-- Dumping Attribtues for {acd.Name} (Sno={acd.ActorSnoId} Ann={acd.AnnId}) at {acd.Position} ----");
                    Logger.Log(atts + "\r\n");
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error Starting LazyCache: " + ex);
            }
        }

        private static void MonitorActors(DiaObject testunit, Dictionary<int, HashSet<ActorAttributeType>> unitAtts)
        {
            var existingAtts = unitAtts.GetOrCreateValue(testunit.ACDId, new HashSet<ActorAttributeType>());
            var atts = Enum.GetValues(typeof(ActorAttributeType)).Cast<ActorAttributeType>().ToList();
            var annId = ZetaDia.Me.CommonData.AnnId;
            var acdId = ZetaDia.Me.ACDId;

            //var root = ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.RootTotalTicks);
            //Logger.Log("Root Ticks {0}", root);

            //[TrinityPlugin 2.14.34] Unit FallenGrunt_A-69336 has MonsterAffix_IllusionistCast (PowerBuff0VisualEffectNone)

            foreach (var att in atts)
            {
                try
                {
                    var attiResult = testunit.CommonData.GetAttribute<int>(att);
                    var attfResult = testunit.CommonData.GetAttribute<float>(att);
                    var hasValue = attiResult > 0 || !float.IsNaN(attfResult) && attfResult > 0;
                    if (hasValue)
                    {
                        if (!existingAtts.Contains(att) || att.ToString().ToLower().Contains("time") || att.ToString().ToLower().Contains("tick"))
                        {
                            Logger.Log("Unit {0} ({4}) has gained {1} (i:{2} f:{3:00.00000})", testunit.Name, att.ToString(), attiResult, attfResult, testunit.ActorSnoId);
                            existingAtts.Add(att);
                        }
                    }
                    else
                    {

                        //var annIdResult = testunit.CommonData.GetAttribute<int>((annId << 12) + ((int)att & 0xFFF));
                        //var acdIdResult = testunit.CommonData.GetAttribute<int>((acdId << 12) + ((int)att & 0xFFF));
                        //if (annIdResult > 0 || acdIdResult > 0)
                        //{
                        //    Logger.Log("Unit {0} ({4}) has ACD/ANN ATTR {1} (acd{2} ann:{3})", testunit.Name, att.ToString(), annIdResult, acdIdResult, testunit.ActorSnoId);
                        //}

                        if (existingAtts.Contains(att))
                        {
                            Logger.Log("Unit {0} ({4}) has lost {1} (i:{2} f:{3:00.00000})", testunit.Name, att.ToString(), attiResult, attfResult, testunit.ActorSnoId);
                            existingAtts.Remove(att);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            var allpowers = Enum.GetValues(typeof(SNOPower)).Cast<SNOPower>().ToList();
            var allAttributes = Enum.GetValues(typeof(ActorAttributeType)).Cast<ActorAttributeType>().ToList();
            var allBuffAttributes = allAttributes.Where(a => a.ToString().Contains("Power")).ToList();

            var checkpowers = new HashSet<SNOPower>
            {
                SNOPower.MonsterAffix_ReflectsDamage,
                SNOPower.MonsterAffix_ReflectsDamageCast,
                SNOPower.Monk_ExplodingPalm
            };

            foreach (var power in allpowers)
            {
                foreach (var buffattr in allBuffAttributes)
                {
                    try
                    {
                        if (testunit.CommonData.GetAttribute<int>(((int)power << 12) + ((int)buffattr & 0xFFF)) == 1)
                        {
                            Logger.Log("Unit {0} has {1} ({2})", testunit.Name, power, buffattr);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            var allTimeAttributes = allAttributes.Where(a => a.ToString().ToLower().Contains("time") || a.ToString().ToLower().Contains("tick")).ToList();

            foreach (var power in allpowers)
            {
                foreach (var timeAttr in allTimeAttributes)
                {
                    try
                    {
                        var result = testunit.CommonData.GetAttribute<int>((int)timeAttr & ((1 << 12) - 1) | ((int)power << 12));
                        if (result > 1)
                        {
                            Logger.Log("Unit {0} has {1} ({2}) Value={3}", testunit.Name, power, timeAttr, result);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }


            //var List<SNOPower> testPowers = new List<SNOPower>
            //{
            //    SNOPower.None
            //};

            //foreach (var timeAttr in allTimeAttributes)
            //{
            //    try
            //    {
            //        var result = testunit.CommonData.GetAttribute<int>((int)ActorAttributeType.PowerCooldown & ((1 << 12) - 1) | ((int)power << 12));
            //        if (result > 0)
            //        {
            //            Logger.Log("Unit {0} has {1} ({2}) Value={3}", testunit.Name, power, timeAttr, result);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            //Logger.Log("),"testunit.CommonData.GetAttribute<int>((SNOPower.None << 12) + ((int)ActorAttributeType.None & 0xFFF)));


        }

        private static void StopTestHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Worker.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error Starting LazyCache: " + ex);
            }
        }

        private static void StopProgressionTestHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                //RiftProgression.ThreadStop();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error Starting LazyCache: " + ex);
            }
        }

        //private static void StartProgressionAutomatedHandler(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    try
        //    {
        //        RiftProgression.StartAutomated();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError("Error Starting LazyCache: " + ex);
        //    }
        //}



        private static void btnClick_UpgradeRares(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Starting");

                CoroutineHelper.RunCoroutine(() => CubeRaresToLegendary.Execute());

                Logger.Log("Finished");
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void UpgradeBackpackRares(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                // todo figure out why the first time cache doesnt update properly
                //Core.ForcedUpdate();

                var alltypes = Enum.GetValues(typeof(ItemSelectionType)).Cast<ItemSelectionType>().ToList();
                CoroutineHelper.RunCoroutine(() => CubeRaresToLegendary.Execute(alltypes), result => !CubeRaresToLegendary.CanRun());
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void ExtractBackpackPowers(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                // todo figure out why the first time cache doesnt update properly
                //Core.ForcedUpdate();

                CoroutineHelper.RunCoroutine(ExtractLegendaryPowers.ExtractAllBackpack);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void btnClick_Test1(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Starting");

                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();

                    foreach (var item in ZetaDia.Me.Inventory.Backpack)
                    {
                        var stackHi = item.GetAttribute<int>(ActorAttributeType.ItemStackQuantityHi);
                        var stackLo = item.GetAttribute<int>(ActorAttributeType.ItemStackQuantityLo);

                        Logger.Log("Item: {0} {1} ItemStackQuantity={2} StackHi={3} StackLo={4}",
                            item.Name, item.ACDId, item.ItemStackQuantity, stackHi, stackLo);
                    }
                }

                Logger.Log("Finished");
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void btnClick_ConvertToBlue(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Starting Conversion of Backpack VeiledCrystals to Magic Dust.");

                var result = ConvertMaterials(new[] { InventoryItemType.VeiledCrystal, InventoryItemType.ReusableParts }, InventoryItemType.ArcaneDust);

                Logger.Log("Finished Result={0}", result);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static bool ConvertMaterials(InventoryItemType[] fromMaterials, InventoryItemType to)
        {
            var conversions = 0;
            var working = false;
            var startTime = DateTime.UtcNow;

            foreach (var fromMaterial in fromMaterials)
            {
                var canRun = false;
                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();
                    Core.Update();
                    canRun = Coroutines.Town.ConvertMaterials.CanRun(fromMaterial, to);
                }

                if (UIElements.TransmuteItemsDialog.IsVisible && canRun)
                {
                    LastStartedConvert = DateTime.UtcNow;
                    CoroutineHelper.RunCoroutine(() => Coroutines.Town.ConvertMaterials.Execute(fromMaterial, to), result =>
                    {
                        var r = !Coroutines.Town.ConvertMaterials.CanRun(fromMaterial, to) || CheckConvertTimeout() || !result;
                        working = !r;
                        return r;

                    }, 50, () => working = false);
                    conversions++;
                }
                while (working)
                {
                    if (DateTime.UtcNow.Subtract(startTime).TotalSeconds > 30)
                    {
                        Logger.LogError("ConvertMaterials timed out");
                        break;
                    }
                    Thread.Sleep(500);
                }
            }

            return conversions > 0;
        }

        public static bool CheckConvertTimeout()
        {
            if (DateTime.UtcNow.Subtract(LastStartedConvert).TotalSeconds > 20)
            {
                Logger.LogError("Timeout");
                return true;
            }
            return false;
        }

        private static void btnClick_ConvertToCommon(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Starting Conversion of Backpack VeiledCrystals to ReusableParts");

                var result = ConvertMaterials(new[] { InventoryItemType.ArcaneDust, InventoryItemType.VeiledCrystal }, InventoryItemType.ReusableParts);

                Logger.Log("Finished Result={0}", result);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void btnClick_ConvertToRare(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Starting Conversion of Backpack ReusableParts/ArcaneDust to VeiledCrystals");

                var result = ConvertMaterials(new[] { InventoryItemType.ArcaneDust, InventoryItemType.ReusableParts }, InventoryItemType.VeiledCrystal);

                Logger.Log("Finished Result={0}", result);
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void btnClick_MassConvertRareToMagic(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Logger.Log("Starting Conversion of Backpack VeiledCrystals to ArcaneDust");

                var from = InventoryItemType.VeiledCrystal;
                var to = InventoryItemType.ArcaneDust;

                if (!UIElements.TransmuteItemsDialog.IsVisible || !Coroutines.Town.ConvertMaterials.CanRun(from, to))
                {
                    Logger.LogError("You need to have the cube window open and all the required materials in your backpack.");
                    return;
                }

                LastStartedConvert = DateTime.UtcNow;

                CoroutineHelper.RunCoroutine(() => Coroutines.Town.ConvertMaterials.Execute(from, to), result => !Coroutines.Town.ConvertMaterials.CanRun(from, to) || CheckConvertTimeout());

                Logger.Log("Finished");
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception: " + ex);
            }
        }

        private static void btnClick_ScanUIElement(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                //[1E8E8E20] Last clicked: 0x80E63C97B008F590, Name: Root.NormalLayer.vendor_dialog_mainPage.training_dialog
                //[1E94FCC0] Mouseover: 0x244BD04C84DF92F1, Name: Root.NormalLayer.vendor_dialog_mainPage

                using (new AquireFrameHelper())
                {
                    UIElement.FromHash(0x244BD04C84DF92F1).FindDecedentsWithText("jeweler");
                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error btnClick_ScanUIElement:" + ex);
            }
        }


        private static void ShowMainTrinityUIEventHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var configWindow = UILoader.GetDisplayWindow();
                configWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error showing Configuration from TabUI:" + ex);
            }
        }

        private static void DumpBuildEventHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                DebugUtil.LogBuildAndItems();
            }
            catch (Exception ex)
            {
                Logger.LogError("DumpBuildEventHandler: " + ex);
            }
        }

        private static void GetNewActorSNOsEventHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                DebugUtil.LogNewItems();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error logging new items:" + ex);
            }
        }

        private static void SortBackEventHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemSort.SortBackpack();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error sorting backpack:" + ex);
            }
        }

        private static void DropLegendariesEventHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                using (new AquireFrameHelper())
                {
                    ZetaDia.Me.Inventory.Backpack.Where(i => i.ItemQualityLevel == ItemQuality.Legendary).ForEach(i => i.Drop());

                    if (BotMain.IsRunning && !BotMain.IsPausedForStateExecution)
                        BotMain.PauseFor(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error dropping legendaries:" + ex);
            }
        }

        private static void SortStashEventHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("This must be started with stash open.");
                Logger.Warn("Please wait - this may take up to 30 seconds before starting.");

                ItemSort.SortStash();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error dropping legendaries:" + ex);
            }
        }

        private static void CleanStashEventHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("This feature has been disabled");
                return;

                var result = MessageBox.Show("Are you sure? This may remove and salvage/sell items from your stash! Permanently!", "Clean Stash Confirmation",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {

                    //CleanStash.RunCleanStash();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error Cleaning Stash:" + ex);
            }
        }


        #region TabMethods

        internal static void RemoveTab()
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var mainWindow = Application.Current.MainWindow;
                    var tabs = mainWindow.FindName("tabControlMain") as TabControl;
                    if (tabs == null)
                        return;
                    tabs.Items.Remove(_tabItem);
                }
                );
        }

        private static Button CreateMajorButton(string buttonText, RoutedEventHandler clickHandler)
        {
            var button = new Button
            {
                //Width = 120,                
                Background = Brushes.DarkSlateBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(2),
                Content = buttonText
            };
            button.Click += clickHandler;
            //_tabGrid.Children.Add(button);
            return button;
        }

        private static Button CreateButton(string buttonText, RoutedEventHandler clickHandler)
        {
            var button = new Button
            {
                //Width = 120,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(1),
                Padding = new Thickness(3),
                Content = buttonText
            };
            button.Click += clickHandler;
            //_tabGrid.Children.Add(button);
            return button;
        }

        private static void CreateStretchyGroup(string title, List<Control> items)
        {
            var group = new UniformGrid
            {
                Columns = 1,
                Background = BackgroundBrush,
                Height = 160,
                Margin = new Thickness(10, 10, 10, 10),
            };

            if (!string.IsNullOrEmpty(title))
                group.Children.Add(CreateTitle(title));

            foreach (var item in items)
            {
                if (item != null)
                    group.Children.Add(item);
            }

            _tabGrid.Children.Add(group);
        }

        private static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromRgb(67, 67, 67));

        private static void CreateGroup(string title, List<Control> items)
        {
            var group = new StackPanel
            {
                Background = BackgroundBrush,
                Height = 170,
                Margin = new Thickness(7, -13, 0, 0),
                ClipToBounds = false
            };

            if (!string.IsNullOrEmpty(title))
                group.Children.Add(CreateTitle(title));

            foreach (var item in items)
            {
                if (item != null)
                    group.Children.Add(item);
            }

            _tabGrid.Children.Add(group);
        }

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

        #endregion
    }
}

