using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Trinity.Components.QuestTools;
using Trinity.Framework.Objects.Memory;
using Trinity.ProfileTags;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Trinity.Components.Adventurer.Game.Rift;
using TabControl = System.Windows.Controls.TabControl;
using Trinity.Framework.Objects;
using Zeta.Common.Helpers;
using Trinity.Components.Adventurer.UI;
using Trinity.Framework.Reference;

namespace Trinity.Components.Adventurer.UI
{
    public class ExpStatistics : Module
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

        private static void ShowMainAdventurerWindow(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var configWindow = ConfigWindow.Instance;
                configWindow.Show();
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error showing Configuration from TabUI:" + ex);
            }
        }

        #region UI Elements
        private static TextBlock CreateLabel(string title, HorizontalAlignment haAlignment)
        {
            return new TextBlock
            {
                Text = title,
                Width = 250,
                Height = 18,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = haAlignment,
                TextAlignment = TextAlignment.Left,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            };
        }

        private static TextBlock CreateLabelBlue(string title, HorizontalAlignment haAlignment)
        {
            return new TextBlock
            {
                Text = title,
                Width = 250,
                Height = 18,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = haAlignment,
                TextAlignment = TextAlignment.Left,
                Foreground = Brushes.LightSkyBlue,
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            };
        }
        
        private static TextBlock CreateTitle(string title)
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
        private static CheckBox CreateCheckBox(string title, HorizontalAlignment haAlignment, RoutedEventHandler eventFunc)
        {
            var checkBox = new CheckBox
            {
                Content = title,
                Width = 120,
                Height = 18,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = haAlignment,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))

            };
            checkBox.Click += eventFunc;
            return checkBox;
        }
        private static Button CreateButton(string title, RoutedEventHandler eventFunc, Thickness margin = default(Thickness), Brush backrground = null)
        {
            if (margin == default(Thickness))
            {
                margin = new Thickness(0);
            }
            var button = new Button
            {
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = margin,
                Content = title,
                Height = 18
            };
            if (backrground != null)
            {
                button.Background = backrground;
            }
            button.Click += eventFunc;
            return button;
        }
        static void AddTownRun_Click(object sender, RoutedEventArgs e)
        {
            AddTownRunCheaked = !AddTownRunCheaked;
        }

        static void ResetExpAll_Click(object sender, RoutedEventArgs e)
        {
            if (GameUI.IsAnyTownWindowOpen)
            {
                Core.Logger.Warn("请等待窗口关闭后再清除统计数据操作!");
            }
            else if (!Core.Player.IsInTown)
            {
                Core.Logger.Warn("请回城镇执行清除统计数据操作!");
            }
            else if (Core.Player.IsInRift || (ZetaDia.Storage.RiftStarted && !Core.Rift.RiftComplete))
            {
                Core.Logger.Warn("在秘境中无法执行清除统计数据操作,请完成秘境后返回城镇操作!");
            }
            else
            {
                ResetExpAll();
                Core.Logger.Warn("统计数据已清除!");
            }
        }

        private static TextBlock _TaskAllTimeLabel;
        private static TextBlock _AllExperienceLabel;
        private static TextBlock _AllExperienceHoursLabel;
        private static TextBlock _OneLevelExperienceTimeLabel;
        private static TextBlock _NextLevelExperienceTimeLabel;
        private static TextBlock _RunCountLabel;
        private static TextBlock _AveOneExperienceLabel;
        private static TextBlock _AveOneTimeLabel;
        private static TextBlock _DeathCountLabel;
        private static TextBlock _TaskThreadCountLabel;
        private static TextBlock _ThenRunCountLabel;
        private static TextBlock _ThenStartParagonInfo;
        private static TextBlock _ThenStartParagonInfo2;
        private static TextBlock _ThenLevelUPLabel;
        private static TextBlock _ThenCurrentParagonInfo;
        private static TextBlock _ThenCurrentParagonInfo2;
        private static TextBlock _ThenRunTimeLabel;
        private static TextBlock _ThenExperienceLabel;
        private static TextBlock _ThenExperienceHoursLabel;
        private static TextBlock _ParagonLevel;
        private static CheckBox _AddTownRun;
        private static Button _ResetExpAll;
        public static bool AddTownRunCheaked;
        #endregion UI Elements

        private static void processExp()
        {
            LastUpdate = DateTime.UtcNow;
            if (!Core.Player.IsInTown)
            {
                int currentRiftLevel = ZetaDia.Storage.CurrentRiftLevel;
                // 大秘 ? 小秘 
                s_RiftLevel = currentRiftLevel > 0 ? currentRiftLevel + 1 : (ZetaDia.Storage.CurrentRiftType == RiftType.Nephalem ? -1 : 0);
            }
            UpdateParagonExp();
            CalceExp();
            MyClass = (MyClass != "") ? MyClass : GetClass();
        }

        private static void ResetExpAll()
        {
            // 注销
            EnablePulse = false;

            /// <summary>
            /// 开始经验信息
            /// </summary>
            startInfo = null;
            /// <summary>
            /// 当前秘境经验信息
            /// </summary>
            startCurrentRiftInfo = null;
            /// <summary>
            /// 开始处理包裹时间信息
            /// </summary>
            StartVendor = default(DateTime);
            /// <summary>
            /// 包裹处理结束时间信息
            /// </summary>
            EndVendor = default(DateTime);
            /// <summary>
            /// 包裹处理时间
            /// </summary>
            VedorTime = new TimeSpan(0, 0, 0);
            /// <summary>
            /// 经验信息
            /// </summary>
            ExpInfo = new Dictionary<int, ParagonExp>();
            /// <summary>
            /// 最后更新时间
            /// </summary>
            LastUpdate = default(DateTime);

            /// <summary>
            /// 大秘境次数
            /// </summary>
            s_TaskAllGreaterRunCount = default(int);
            /// <summary>
            /// 小秘境次数
            /// </summary>
            s_TaskAllNephalemRunCount = default(int);
            /// <summary>
            /// 牛关次数
            /// </summary>
            s_TaskAllCowLevelRunCount = default(int);
            /// <summary>
            /// 启动DB后死亡
            /// </summary>
            s_DeathCount = default(int);
            /// <summary>
            /// 房间重建次数
            /// </summary>
            s_TaskThreadCount = 0;
            /// <summary>
            /// 秘境层级
            /// </summary>
            s_RiftLevel = default(int);

            // 注册
            processExp();
            EnablePulse = true;


        }

        /// <summary>
        /// 大秘境次数
        /// </summary>
        private static int s_TaskAllGreaterRunCount;
        /// <summary>
        /// 小秘境次数
        /// </summary>
        private static int s_TaskAllNephalemRunCount;
        /// <summary>
        /// 牛关次数
        /// </summary>
        public static int s_TaskAllCowLevelRunCount;
        /// <summary>
        /// 启动DB后死亡
        /// </summary>
        private static int s_DeathCount;
        /// <summary>
        /// 房间重建次数
        /// </summary>
        private static int s_TaskThreadCount = 0;
        /// <summary>
        /// 秘境层级
        /// </summary>
        public static int s_RiftLevel;

        private bool IsInitUiTab;

        public ExpStatistics()
        {
            if (!IsInitUiTab)
            {
                InitUiTab();
                IsInitUiTab = true;
            }

        }
        /// <summary>
        /// 开始经验信息
        /// </summary>
        public static ParagonExp startInfo = null;
        /// <summary>
        /// 当前秘境经验信息
        /// </summary>
        public static ParagonExp startCurrentRiftInfo = null;
        /// <summary>
        /// 开始处理包裹时间信息
        /// </summary>
        private static DateTime StartVendor = default(DateTime);
        /// <summary>
        /// 包裹处理结束时间信息
        /// </summary>
        private static DateTime EndVendor = default(DateTime);
        /// <summary>
        /// 包裹处理时间
        /// </summary>
        public static TimeSpan VedorTime = new TimeSpan(0, 0, 0);
        /// <summary>
        /// 经验信息
        /// </summary>
        public static Dictionary<int, ParagonExp> ExpInfo = new Dictionary<int, ParagonExp>();
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public static DateTime LastUpdate = default(DateTime);
        /// <summary>
        /// 启用
        /// </summary>
        public static bool EnablePulse = true;

        /// <summary>
        /// 秘境开始时更新信息
        /// </summary>
        public static void UpdateStartRiftInfo()
        {
            EndVendor = DateTime.UtcNow;
            VedorTime += EndVendor != default(DateTime) && StartVendor != default(DateTime) ? (EndVendor - StartVendor) : new TimeSpan(0, 0, 0);
            startCurrentRiftInfo = new ParagonExp(ZetaDia.Me.ParagonLevel, ZetaDia.Me.ParagonCurrentExperience, ZetaDia.Me.ParagonCurrentExperience + ZetaDia.Me.ParagonExperienceNextLevel, DateTime.Now);
        }

        /// <summary>
        /// 结束时更新信息
        /// </summary>
        public static void UpdateEndRiftInfo()
        {
            if (s_RiftLevel == -1)
                s_TaskAllNephalemRunCount++;
            else if (s_RiftLevel > 0)
                s_TaskAllGreaterRunCount++;
            Core.Logger.Debug($"UpdateEndRiftInfo s_RiftLevel = {s_RiftLevel}");
            s_RiftLevel = 0;
            startCurrentRiftInfo = null;
            StartVendor = DateTime.UtcNow;
        }

        private static int diff = 0;
        /// <summary>
        /// 当前 WorldId
        /// </summary>
        public static int NowWorldID;
        /// <summary>
        /// Bot开始时间
        /// </summary>
        private static int BotStartTimes;
        /// <summary>
        /// 当World改变时调用
        /// </summary>
        /// <param name="args"></param>
        protected override void OnWorldChanged(Trinity.Framework.Helpers.ChangeEventArgs<int> args)
        {
            NowWorldID = args.NewValue;
        }
        protected override void OnGameJoined()
        {
            diff = (int)ZetaDia.Storage?.CurrentGameDifficulty - 3;
            s_TaskThreadCount++;
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    _TaskThreadCountLabel.Text = string.Format("房间重建次数: {0}，DB停止开始次数：{1}", s_TaskThreadCount, BotStartTimes);
                }
            );

        }
        /// <summary>
        /// 当死亡时调用
        /// </summary>
        protected override void OnPlayerDied()
        {
            s_DeathCount += 1;
        }
        /// <summary>
        /// 当BOT启动时调用
        /// </summary>
        protected override void OnBotStart()
        {
            BotStartTimes++;
            EndVendor = DateTime.UtcNow;
            EnablePulse = true;
        }
        /// <summary>
        /// 当BOT停止时调用
        /// </summary>
        protected override void OnBotStop()
        {
            StartVendor = DateTime.UtcNow;
            EnablePulse = false;
            MyClass = "";
        }

        protected override void OnPulse()
        {
            if (EnablePulse)
            {
                EnablePulse = false;
                if (DateTime.UtcNow.Subtract(LastUpdate).TotalSeconds > 1.0)
                {
                    processExp();
                }
                EnablePulse = true;
            }

        }

        /// <summary>
        /// 初始化 UI Tab
        /// </summary>
        internal static void InitUiTab()
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var mainWindow = Application.Current.MainWindow;

                    var ExpPanelLeft = new StackPanel { Background = Brushes.DimGray, Width = 250,  Height = 176, Margin = new Thickness(2, 2, 0, 2) };

                    _TaskAllTimeLabel = CreateLabel("共用时: 0 小时 0 分 0 秒", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_TaskAllTimeLabel);

                    _AllExperienceLabel = CreateLabel("共获经验: 0", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_AllExperienceLabel);

                    _AllExperienceHoursLabel = CreateLabelBlue("平均每小时经验: 0", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_AllExperienceHoursLabel);

                    _RunCountLabel = CreateLabel("完成秘境次数: 0", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_RunCountLabel);

                    _AveOneExperienceLabel = CreateLabel("平均每次经验: 0", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_AveOneExperienceLabel);

                    _AveOneTimeLabel = CreateLabel("平均每次时间: 0 小时 0 分 0 秒", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_AveOneTimeLabel);

                    _DeathCountLabel = CreateLabelBlue("DB启动后死亡: 0次  游戏启动后死亡: 0次", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_DeathCountLabel);

                    _TaskThreadCountLabel = CreateLabel("房间重建次数: 0，DB停止开始次数：0", HorizontalAlignment.Left);
                    ExpPanelLeft.Children.Add(_TaskThreadCountLabel);

                    var ExpPanelRight = new StackPanel { Background = Brushes.DimGray, Margin = new Thickness(2, 2, 0, 2) };

                    _OneLevelExperienceTimeLabel = CreateLabel("预计每升一级用时: 0 小时 0 分 0 秒", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_OneLevelExperienceTimeLabel);

                    _NextLevelExperienceTimeLabel = CreateLabel("预计升至下一级需用时: 0 小时 0 分 0 秒", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_NextLevelExperienceTimeLabel);

                    _ThenRunCountLabel = CreateLabel("当前秘境次数: 0    当前密境等级: 0", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenRunCountLabel);

                    _ThenRunTimeLabel = CreateLabel("当前秘境已用时: 0", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenRunTimeLabel);

                    _ThenExperienceLabel = CreateLabel("当前秘境获得经验: 0", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenExperienceLabel);

                    _ThenExperienceHoursLabel = CreateLabel("当前秘境每小时经验效率: 0", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenExperienceHoursLabel);

                    _ParagonLevel = CreateLabel("我的巅峰： 0", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ParagonLevel);

                    // 操作组
                    var OperationPanelLeft = new StackPanel { Background = Brushes.DimGray, Width = 125, Height = 20, Margin = new Thickness(0) };
                    _AddTownRun = CreateCheckBox("算上处理包裹时间", HorizontalAlignment.Left, AddTownRun_Click);
                    OperationPanelLeft.Children.Add(_AddTownRun);

                    var OperationPanelRight = new StackPanel { Background = Brushes.DimGray, Width = 100, Height = 20, Margin = new Thickness(0) };
                    OperationPanelRight.Children.Add(CreateButton("清除统计数据", ResetExpAll_Click));

                    var OperationGrid = new UniformGrid
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Columns = 2,
                    };

                    OperationGrid.Children.Add(OperationPanelLeft);
                    OperationGrid.Children.Add(OperationPanelRight);

                    ExpPanelRight.Children.Add(OperationGrid);

                    ExpPanelRight.Children.Add(CreateLabel("起始数据", HorizontalAlignment.Center));

                    _ThenStartParagonInfo = CreateLabel("起始巅峰：,起始经验：", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenStartParagonInfo);

                    _ThenStartParagonInfo2 = CreateLabel("整级经验：,起始时间：", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenStartParagonInfo2);

                    ExpPanelRight.Children.Add(CreateLabel("当前数据", HorizontalAlignment.Center));

                    _ThenLevelUPLabel = CreateLabel("", HorizontalAlignment.Center);
                    ExpPanelRight.Children.Add(_ThenLevelUPLabel);

                    _ThenCurrentParagonInfo = CreateLabel("当前巅峰：,当前经验：", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenCurrentParagonInfo);

                    _ThenCurrentParagonInfo2 = CreateLabel("整级经验：,当前时间：", HorizontalAlignment.Left);
                    ExpPanelRight.Children.Add(_ThenCurrentParagonInfo2);

                    var ExpPanelCentre = new StackPanel { Background = Brushes.DimGray, Width = 18, Height = 176, Margin = new Thickness(0, 0, 0, 0) };
                    ExpPanelCentre.Children.Add(CreateLabel("老", HorizontalAlignment.Left));
                    ExpPanelCentre.Children.Add(CreateLabel("挂", HorizontalAlignment.Left));
                    ExpPanelCentre.Children.Add(CreateLabel("逼", HorizontalAlignment.Left));
                    ExpPanelCentre.Children.Add(CreateLabel("们", HorizontalAlignment.Left));
                    ExpPanelCentre.Children.Add(CreateLabel("开", HorizontalAlignment.Left));
                    ExpPanelCentre.Children.Add(CreateLabel("心", HorizontalAlignment.Left));
                    ExpPanelCentre.Children.Add(CreateLabel("就", HorizontalAlignment.Left));
                    ExpPanelCentre.Children.Add(CreateLabel("好", HorizontalAlignment.Left));

                    var scrollViewer3 = new ScrollViewer();
                    scrollViewer3.Content = ExpPanelRight;

                    var uniformGrid = new Grid
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        MaxHeight = 180,
                        Width = Double.NaN
                    };

                    ColumnDefinition cd1 = new ColumnDefinition();
                    cd1.Width = new GridLength(250);
                    uniformGrid.ColumnDefinitions.Add(cd1);


                    ColumnDefinition cd2 = new ColumnDefinition();
                    cd2.Width = new GridLength(20);
                    uniformGrid.ColumnDefinitions.Add(cd2);


                    ColumnDefinition cd3 = new ColumnDefinition();
                    cd3.Width = GridLength.Auto;
                    uniformGrid.ColumnDefinitions.Add(cd3);

                    RowDefinition row1 = new RowDefinition();
                    row1.Height = new GridLength(180);
                    uniformGrid.RowDefinitions.Add(row1);

                    uniformGrid.Children.Add(ExpPanelLeft);
                    Grid.SetRow(ExpPanelLeft, 0);
                    Grid.SetColumn(ExpPanelLeft, 0);

                    uniformGrid.Children.Add(ExpPanelCentre);
                    Grid.SetRow(ExpPanelCentre, 0);
                    Grid.SetColumn(ExpPanelCentre, 1);

                    uniformGrid.Children.Add(scrollViewer3);
                    Grid.SetRow(scrollViewer3, 0);
                    Grid.SetColumn(scrollViewer3, 2);

                    _tabItem = new TabItem
                    {
                        Header = "经验统计",
                        ToolTip = "阿SEN自用Trinity",
                        Content = uniformGrid,
                    };

                    var tabs = mainWindow.FindName("tabControlMain") as TabControl;
                    if (tabs == null)
                        return;

                    tabs.Items.Add(_tabItem);
                }
            );
        }

        /// <summary>
        /// 获取对应巅峰等级文字描述
        /// </summary>
        /// <param name="ParagonLevel"></param>
        /// <returns></returns>
        public static string MyWord(int ParagonLevel)
        {

            if (ParagonLevel < 100)
                return "装逼，是一种向往 ";

            if (ParagonLevel < 300)
                return "我是萌新不敢说话 ";

            if (ParagonLevel > 500 && ParagonLevel < 1000)
                return "走向通往大神的路上 ";

            if (ParagonLevel > 1000 && ParagonLevel < 1500)
                return "离大神又近一步了 ";

            if (ParagonLevel > 1500 && ParagonLevel < 2000)
                return "马上就可以装逼了 ";

            if (ParagonLevel > 2000 && ParagonLevel < 2500)
                return "放开我,我要装逼 ";

            if (ParagonLevel > 2500 && ParagonLevel < 3000)
                return "装逼是一种境界 ";

            if (ParagonLevel > 3000 && ParagonLevel < 3500)
                return "栋哥，求你别封我 ";

            if (ParagonLevel > 3500 && ParagonLevel < 4000)
                return "颤抖吧，凡人 ";

            if (ParagonLevel > 4000)
                return "神，不需要装逼  ";

            return "";
        }

        public static string MyClass = "";
        /// <summary>
        /// 获取职业
        /// </summary>
        /// <returns></returns>
        public static string GetClass()
        {
            string Class = ZetaDia.Me?.ActorClass.ToString();
            switch (Class)
            {

                case "Invalid":
                    return "";
                case "Barbarian":
                    return RarandomClassWord(new List<string> { "野蛮人", "蛮子", "蛮爷", "蛮神" });
                case "Crusader":
                    return RarandomClassWord(new List<string> { "圣教军", "逗教军", "豆角", "逗逼军" });
                case "DemonHunter":
                    return RarandomClassWord(new List<string> { "猎魔人", "高跟鞋", "女王", "魂斗罗" });
                case "Monk":
                    return RarandomClassWord(new List<string> { "武僧", "和尚", "大完美" });
                case "Witchdoctor":
                    return RarandomClassWord(new List<string> { "巫医", "老中医", "大隐藏", "大神教" });
                case "Wizard":
                    return RarandomClassWord(new List<string> { "法师", "亲儿子", "法爷", "萌萌哒", "法狗" });
                case "Necromancer":
                    return RarandomClassWord(new List<string> { "死灵" });
            }
            return "";
        }
        /// <summary>
        /// 随机获取职业名称
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        private static string RarandomClassWord(List<string> classname)
        {
            var rand = new Random();
            int index = rand.Next(classname.Count - 1);
            return classname[index];
        }
        /// <summary>
        /// 经验更新
        /// </summary>
        /// <param name="AllExp"></param>
        /// <param name="AllTimeSpan"></param>
        /// <param name="ParagonLevel"></param>
        /// <param name="CurrentRiftExp"></param>
        /// <param name="CurrentRiftTimes"></param>
        /// <param name="StartInfo"></param>
        /// <param name="CurrentInfo"></param>
        public static void UpdateExp(long AllExp, TimeSpan AllTimeSpan, int ParagonLevel, long CurrentRiftExp, TimeSpan CurrentRiftTimes, ParagonExp StartInfo, ParagonExp CurrentInfo)
        {
            var s_TaskAllExperience = AllExp;
            var ThenTaskAllTime = AllTimeSpan;
            var ThenTaskAllRunCount = s_TaskAllGreaterRunCount + s_TaskAllNephalemRunCount;
            int AOneLevelTime = (int)(((float)ZetaDia.Me.ParagonCurrentExperience + (float)ZetaDia.Me.ParagonExperienceNextLevel) / ((float)s_TaskAllExperience / ThenTaskAllTime.TotalHours) * 3600);
            int AOneLevelHours = (int)(AOneLevelTime / 3600);
            int AOneLevelMinutes = (int)((AOneLevelTime - 3600 * AOneLevelHours) / 60);
            int AOneLevelSeconds = AOneLevelTime - 3600 * AOneLevelHours - 60 * AOneLevelMinutes;

            var ThenTaskOneTime = CurrentRiftTimes;
            int ANextLevelTime = (int)((float)ZetaDia.Me.ParagonExperienceNextLevel / ((float)s_TaskAllExperience / ThenTaskAllTime.TotalHours) * 3600);
            int ANextLevelHours = (int)(ANextLevelTime / 3600);
            int ANextLevelMinutes = (int)((ANextLevelTime - 3600 * ANextLevelHours) / 60);
            int ANextLevelSeconds = ANextLevelTime - 3600 * ANextLevelHours - 60 * ANextLevelMinutes;

            int AvgTime = (int)(ThenTaskAllTime.TotalSeconds / (ThenTaskAllRunCount > 0 ? ThenTaskAllRunCount : 1));
            int AvgHours = (int)(AvgTime / 3600);
            int AvgMinutes = (int)((AvgTime - 3600 * AvgHours) / 60);
            int AvgSeconds = AvgTime - 3600 * AvgHours - 60 * AvgMinutes;

            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    _TaskAllTimeLabel.Text = string.Format("共用时: {0} 小时 {1} 分 {2} 秒", ThenTaskAllTime.Hours, ThenTaskAllTime.Minutes, ThenTaskAllTime.Seconds);
                    _AllExperienceLabel.Text = string.Format("共获经验: {0:0,0}", s_TaskAllExperience);
                    _AllExperienceHoursLabel.Text = string.Format("平均每小时经验: {0:0,0}", s_TaskAllExperience / ThenTaskAllTime.TotalHours);
                    _DeathCountLabel.Text = string.Format("启动DB后死亡: {0}次  进入游戏后死亡: {1}次", s_DeathCount, ZetaDia.Me.DeathCount);
                    _RunCountLabel.Text = string.Format("完成次数: {0}  大秘: {1}  小秘: {2}  牛关: {3}", ThenTaskAllRunCount, s_TaskAllGreaterRunCount, s_TaskAllNephalemRunCount, s_TaskAllCowLevelRunCount);
                    _AveOneExperienceLabel.Text = string.Format("平均每次经验: {0:0,0}", AllExp / (ThenTaskAllRunCount > 0 ? ThenTaskAllRunCount : 1));
                    _AveOneTimeLabel.Text = string.Format("平均每次时间: {0} 小时 {1} 分 {2} 秒", AvgHours, AvgMinutes, AvgSeconds);

                    _OneLevelExperienceTimeLabel.Text = string.Format("预计每升一级用时: {0} 小时 {1} 分 {2} 秒", AOneLevelHours, AOneLevelMinutes, AOneLevelSeconds);
                    _NextLevelExperienceTimeLabel.Text = string.Format("预计升至下一级需用时: {0} 小时 {1} 分 {2} 秒", ANextLevelHours, ANextLevelMinutes, ANextLevelSeconds);

                    if (Core.Player.IsInTown)
                    {
                        _ThenRunCountLabel.Text = string.Format("当前秘境次数: {0}     当前密境等级: 城镇中", ThenTaskAllRunCount + 1);
                    }
                    else if (s_RiftLevel > 0)
                    {
                        _ThenRunCountLabel.Text = string.Format("当前秘境次数: {0}     当前密境等级: {1}", ThenTaskAllRunCount + 1, s_RiftLevel);
                    }
                    else if (s_RiftLevel == -1 || Core.Rift.IsNephalemRift && RiftData.RiftWorldIds.Contains(NowWorldID))
                    {
                        _ThenRunCountLabel.Text = string.Format("当前秘境次数: {0}     当前密境等级: {1}小秘境", ThenTaskAllRunCount + 1, diff > 3 ? "T" + diff.ToString() : diff == 0 ? "普通" : diff == 1 ? "困难" : diff == 2 ? "专家" : "大师");
                    }

                    _ThenRunTimeLabel.Text = string.Format("当前秘境已用时: {0} 小时 {1} 分 {2} 秒", ThenTaskOneTime.Hours, ThenTaskOneTime.Minutes, ThenTaskOneTime.Seconds);
                    _ThenExperienceLabel.Text = string.Format("当前秘境获得经验: {0:0,0}", CurrentRiftExp);
                    _ThenExperienceHoursLabel.Text = string.Format("当前秘境每小时经验效率: {0:0,0}", CurrentRiftExp / ThenTaskOneTime.TotalHours);
                    _ParagonLevel.Text = string.Format("{0} 巅峰：{1}，职业：{2}", MyWord(ParagonLevel), ParagonLevel, MyClass);

                    _ThenStartParagonInfo.Text = string.Format("巅峰：{0}, 经验：{1}", StartInfo.Paragon, StartInfo.CurrentExp);
                    _ThenStartParagonInfo2.Text = string.Format("整级经验：{0}, 时间：{1}", StartInfo.AllLevelExp, StartInfo.LastUpdateTime.ToString("HH:mm M/d"));
                    _ThenLevelUPLabel.Text = string.Format("共升 {0} 级, 当前经验：{1}",
                        CurrentInfo.Paragon - StartInfo.Paragon,
                        ((double)CurrentInfo.CurrentExp / CurrentInfo.AllLevelExp).ToString("0.0%"));
                    _ThenCurrentParagonInfo.Text = string.Format("巅峰：{0}, 经验：{1}", CurrentInfo.Paragon, CurrentInfo.CurrentExp);
                    _ThenCurrentParagonInfo2.Text = string.Format("整级经验：{0}, 时间：{1}", CurrentInfo.AllLevelExp, CurrentInfo.LastUpdateTime.ToString("HH:mm M/d"));
                }
             );

        }
        /// <summary>
        /// 更新巅峰经验
        /// </summary>
        private static void UpdateParagonExp()
        {
            long paragonCurrentExperience = ZetaDia.Me.ParagonCurrentExperience;
            long allLevelExp = ZetaDia.Me.ParagonCurrentExperience + ZetaDia.Me.ParagonExperienceNextLevel;
            int paragonLevel = ZetaDia.Me.ParagonLevel;
            DateTime now = DateTime.Now;
            ParagonExp exp = new ParagonExp(paragonLevel, paragonCurrentExperience, allLevelExp, now);
            if (startInfo == null)
            {
                startInfo = exp;
            }
            if (ExpInfo.ContainsKey(exp.Paragon))
            {
                ExpInfo[exp.Paragon] = exp;
            }
            else
            {
                ExpInfo.Add(exp.Paragon, exp);
            }
        }
        /// <summary>
        /// 计算经验
        /// </summary>
        private static void CalceExp()
        {
            DateTime lastUpdateTime = (startInfo != null) ? startInfo.LastUpdateTime : DateTime.Now;
            TimeSpan allTimeSpan = (TimeSpan)(DateTime.Now - lastUpdateTime);
            allTimeSpan = !AddTownRunCheaked ? (allTimeSpan - VedorTime) : allTimeSpan;
            long allExp = 0L;
            if (ExpInfo.Count != 0)
            {
                foreach (KeyValuePair<int, ParagonExp> pair in ExpInfo)
                {
                    if ((ExpInfo.Count == 1) && (pair.Key == startInfo.Paragon))
                    {
                        allExp += pair.Value.CurrentExp - startInfo.CurrentExp;
                    }
                    else if (((ExpInfo.Count > 1) && pair.Equals(ExpInfo.First<KeyValuePair<int, ParagonExp>>())) && (pair.Key == startInfo.Paragon))
                    {
                        allExp += startInfo.AllLevelExp - startInfo.CurrentExp;
                    }
                    else if (pair.Equals(ExpInfo.Last<KeyValuePair<int, ParagonExp>>()))
                    {
                        allExp += pair.Value.CurrentExp;
                    }
                    else
                    {
                        allExp += pair.Value.AllLevelExp;
                    }
                }
                TimeSpan currentRiftTimeSpan = new TimeSpan();
                long currentRiftExp = 0L;
                if (startCurrentRiftInfo != null)
                {
                    lastUpdateTime = startCurrentRiftInfo.LastUpdateTime;
                    currentRiftTimeSpan = (TimeSpan)(DateTime.Now - lastUpdateTime);
                    bool flag = false;
                    foreach (KeyValuePair<int, ParagonExp> pair2 in ExpInfo)
                    {
                        if (pair2.Key == startCurrentRiftInfo.Paragon)
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            if ((ExpInfo.Count == 1) && (pair2.Key == startCurrentRiftInfo.Paragon))
                            {
                                currentRiftExp += pair2.Value.CurrentExp - startCurrentRiftInfo.CurrentExp;
                            }
                            else if (((ExpInfo.Count > 1) && pair2.Equals(ExpInfo.First<KeyValuePair<int, ParagonExp>>())) && (pair2.Key == ExpStatistics.startCurrentRiftInfo.Paragon))
                            {
                                currentRiftExp += startCurrentRiftInfo.AllLevelExp - startCurrentRiftInfo.CurrentExp;
                            }
                            else if (((ExpInfo.Count > 1) && pair2.Equals(ExpInfo.Last<KeyValuePair<int, ParagonExp>>())) && (pair2.Key == ExpStatistics.startCurrentRiftInfo.Paragon))
                            {
                                currentRiftExp += pair2.Value.CurrentExp - startCurrentRiftInfo.CurrentExp;
                            }
                            else if ((ExpInfo.Count > 1) && pair2.Equals(ExpInfo.Last<KeyValuePair<int, ParagonExp>>()))
                            {
                                currentRiftExp += pair2.Value.CurrentExp;
                            }
                            else
                            {
                                currentRiftExp += pair2.Value.AllLevelExp;
                            }
                        }
                    }
                }
                UpdateExp(allExp, allTimeSpan, ExpInfo.Last<KeyValuePair<int, ParagonExp>>().Value.Paragon, currentRiftExp, currentRiftTimeSpan, startInfo, ExpInfo.Last<KeyValuePair<int, ParagonExp>>().Value);
            }
        }
    }

    /// <summary>
    /// 巅峰经验类
    /// </summary>
    public class ParagonExp
    {
        public ParagonExp(int _Paragon = 0, long _CurrentExp = 0, long _AllLevelExp = 0, DateTime _LastUpdateTime = default(DateTime))
        {
            Paragon = _Paragon;
            CurrentExp = _CurrentExp;
            AllLevelExp = _AllLevelExp;
            LastUpdateTime = _LastUpdateTime;
        }

        public int Paragon = 0;

        public long CurrentExp = 0;

        public long AllLevelExp = 0;

        public DateTime LastUpdateTime = DateTime.MinValue;

        public override string ToString()
        {
            return string.Format($"ParagonExp : Paragon = {Paragon}，CurrentExp = {CurrentExp}，AllLevelExp = {AllLevelExp}，LastUpdateTime = {LastUpdateTime}");
        }
    }
}
