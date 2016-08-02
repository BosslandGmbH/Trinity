//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using Adventurer.Util;
//using Buddy.Overlay;
//using Buddy.Overlay.Controls;
//using Zeta.Game;

//namespace Adventurer.UI
//{
//    class OverlayUI
//    {

//        private static bool _added;
//        public static bool Visible { get; private set; }
//        private static TextBlock _logBox;
//        private static LabelOverlayUIComponent _damageOverlay;

//        internal static void InstallOverlayComponents()
//        {
//            _damageOverlay = new LabelOverlayUIComponent();
//            ZetaDia.Overlay.AddUIComponent(_damageOverlay);
//            _added = true;
//            Visible = true;
//        }


//        internal static void AddLineToLog(string message)
//        {
//            //ZetaDia.Overlay.Dispatcher.Invoke(
//            //    () =>
//            //    {
//            //        _logBox.Text = message;
//            //    });
//        }


//        internal static void RemoveOverlayComponents()
//        {
//            ZetaDia.Overlay.RemoveUIComponent(_damageOverlay);
//            _added = false;
//            Visible = false;
//        }

//        internal static void HideOverlayComponents()
//        {
//            if (!_added) return;
//            if (!Visible) return;
//            ZetaDia.Overlay.Dispatcher.Invoke(
//            () =>
//            {
//                _damageOverlay.GuiElement.Visibility = Visibility.Hidden;
//            });
//            Visible = false;
//            Logger.Info("Hiding Overlay Components");
//        }

//        internal static void ShowOverlayComponents()
//        {
//            if (!_added) return;
//            if (Visible) return;
//            ZetaDia.Overlay.Dispatcher.Invoke(
//            () =>
//            {
//                _damageOverlay.GuiElement.Visibility = Visibility.Visible;
//            });
//            Visible = true;
//            Logger.Info("Showing Overlay Components");
//        }

//        // Shows a button that can clicked and a label on top that can be used to drag the button around on screen.
//        private class LabelOverlayUIComponent : OverlayUIComponent
//        {
//            public LabelOverlayUIComponent()
//                : base(false /*isHitTestable, determines whether UI component can handle mouse clicks */) { }

//            private OverlayControl _control;

//            public override OverlayControl Control
//            {
//                get
//                {
//                    if (_control == null)
//                    {
//                        _logBox = new TextBlock
//                                  {
//                                      Height = 20,
//                                      Background = Brushes.Transparent,
//                                      FontFamily = new FontFamily("Arial"),
//                                      FontSize = 16,
//                                      Foreground = Brushes.Tan,
//                                      TextAlignment = TextAlignment.Right
//                                  };
//                        var stackPanel = new StackPanel();
//                        //stackPanel.Children.Add(new TextBlock { Text = "Current DPS", Background = Brushes.DarkGreen, FontSize = 20, TextAlignment = TextAlignment.Center, Foreground = Brushes.White });
//                        stackPanel.Children.Add(_logBox);
//                        stackPanel.Width = 500;
//                        stackPanel.Height = 20;
//                        //stackPanel.op
//                        _control = new OverlayControl
//                                   {
//                                       X = ZetaDia.Overlay.UnscaledOverlayWidth - stackPanel.Width - 45,
//                                       Y =  340,
//                                       Content = stackPanel,
//                                       AllowMoving = false,
//                                       Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
//                                   };
//                    }
//                    return _control;
//                }
//            }
//        }

//    }


//}
