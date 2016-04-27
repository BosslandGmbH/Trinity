using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Buddy.Overlay;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.UI.Overlays
{
    public class OverlayLoader
    {
        public OverlayLoader()
        {
            Application.Current.MainWindow.Closing += (sender, args) =>
            {
                _internalTimer.Stop();
                _internalTimer.Dispatcher.InvokeShutdown();
                RemoveAll();
            };
        }

        public static List<OverlayBase> ActiveComponents = new List<OverlayBase>();
        private static DispatcherTimer _internalTimer;        

        public static void Enable()
        {
            //if (OverlaySettings.Instance.ShowToolbar)
            //{
            AddComponent(ToolbarOverlayComponent2.CreateNew());
            //AddComponent(ToolbarOverlayComponent.CreateNew());                
            //}

            //OverlaySettings.Instance.PropertyChanged += SettingChanged;

            if (_internalTimer == null)
            {
                _internalTimer = new DispatcherTimer();
                _internalTimer.Tick += InternalTimerTick;
                _internalTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                _internalTimer.Start();
            }
        }

        private static void SettingChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            ZetaDia.Overlay.Dispatcher.Invoke(() =>
            {
                try
                {
                    switch (propertyChangedEventArgs.PropertyName)
                    {
                        case "ShowToolbar":
                            if (!OverlaySettings.Instance.ShowToolbar)
                                HideComponentsOfType<ToolbarOverlayComponent2>();
                            else
                                ShowComponentsOfType<ToolbarOverlayComponent2>();
                            break;

                        case "ShowBorderEffect":
                            if (!OverlaySettings.Instance.ShowBorderEffect)
                                HideBorder();
                            else
                                ShowBorder();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("OverlayLoader.SettingChanged Exception - {0}", ex);
                }
            });
        }

        public static ScaleTransform OriginalTransform;
        internal static Grid UnscaledGrid;
        private static Grid _parentGrid;
        private static Canvas _parentCanvas;
        private static Border _border2;
        private static Border _border1;

        private static void InternalTimerTick(object sender, EventArgs e)
        {
            try
            {

                if (ZetaDia.Overlay.Dispatcher == null)
                    return;

                //if (!ActiveComponents.Any())
                //    return; 

                CreateUnscaledGrid();

                _internalTimer.Stop();

                Init();
            }
            catch (Exception ex)
            {
                Logger.Log("OverlayLoader.InternalTimerTick Exception - {0}", ex);
            }
        }

        private static void Init()
        {
            ZetaDia.Overlay.Dispatcher.Invoke(() =>
            //Application.Current.Dispatcher.Invoke(() =>
            {
                //ShowComponentsOfType<ToolbarOverlayComponent2>();
                //ShowBorder();
            });
        }

        public static void HideBorder()
        {
            _border1.BorderThickness = new Thickness(0);
            _border2.BorderThickness = new Thickness(0);
        }

        public static void ShowBorder()
        {
            _border1.BorderThickness = new Thickness(2);
            _border2.BorderThickness = new Thickness(2);
        }

        public static ScaleTransform CreateUnscaledGrid()
        {
            ZetaDia.Overlay.Dispatcher.Invoke(() =>
            {
                try
                {
                    Logger.Log("Creating Unscaled Grid");

                    var element = ActiveComponents.ElementAt(0).GuiElement;

                    _parentCanvas = element.Parent as Canvas;

                    if (_parentCanvas != null)
                    {
                        Logger.Log("Overlay Parent Canvas Found");

                        _parentGrid = _parentCanvas.Parent as Grid;

                        if (_parentGrid != null)
                        {

                            Logger.Log("Overlay Parent Grid Found");

                            UnscaledGrid = new Grid
                            {                      
                                Background = Brushes.Transparent,
                                LayoutTransform = (Transform)_parentGrid.LayoutTransform.Inverse
                            };

                            var viewport = new Viewport3DComponent();

                            UnscaledGrid.Children.Add(viewport.Control);

                            _parentGrid.Children.Add(UnscaledGrid);

                            var unscaledCanvas = new Canvas
                            {
                                IsHitTestVisible = true,
                            };

                            return _parentGrid.LayoutTransform;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("OverlayLoader.CreateUnscaledGrid Exception - {0}", ex);
                }
                return null;
            });
            return null;
        }


        public static void Disable()
        {
            RemoveAll();
        }

        public static void AddComponentOfType<T>() where T : new()
        {
            var component = new T() as OverlayBase;
            ActiveComponents.Add(component);
            ZetaDia.Overlay.AddUIComponent(component);
        }

        public static void AddComponent<T>(T component) where T : OverlayBase
        {
            ActiveComponents.Add(component);
            ZetaDia.Overlay.AddUIComponent(component);
        }

        public static void RemoveComponentsOfType<T>()
        {
            var ofType = ActiveComponents.OfType<T>().Cast<OverlayUIComponent>().ToList();
            ofType.ForEach(c => ZetaDia.Overlay.RemoveUIComponent(c));
            ActiveComponents.RemoveAll(c => ofType.Contains(c));
        }

        public static void RemoveComponent<T>(T component) where T : OverlayBase
        {
            ZetaDia.Overlay.RemoveUIComponent(component);
            ActiveComponents.Remove(component);
        }

        public static void ShowComponentsOfType<T>() where T : OverlayBase
        {            
            var ofType = ActiveComponents.OfType<T>();
            ofType.ForEach(c => c.Control.Visibility = Visibility.Visible);
        }

        public static void HideComponentsOfType<T>() where T : OverlayBase
        {
            var ofType = ActiveComponents.OfType<T>();
            ofType.ForEach(c => c.Control.Visibility = Visibility.Collapsed);
        }

        private static void RemoveAll()
        {
            ActiveComponents.ForEach(c => ZetaDia.Overlay.RemoveUIComponent(c));
            ActiveComponents.Clear();
        }


    }
}
