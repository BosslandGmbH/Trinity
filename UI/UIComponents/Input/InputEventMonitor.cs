using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zeta.Common;

namespace Trinity.UI.UIComponents.Input
{
    /// <summary>
    /// Allows dependency objects to be modified from outside events
    /// such as mouse clicks outside the wpf control/application.
    /// </summary>
    internal static class InputEventMonitor
    {
        private static readonly List<WeakReference<DependencyObject>> EnumerableObjects;
        private static readonly ConditionalWeakTable<DependencyObject, EventData> Objects;
        private static readonly MouseInput MouseListener;
        //private static readonly KeyboardInput KeyboardListener;

        static InputEventMonitor()
        {
            EnumerableObjects = new IndexedList<WeakReference<DependencyObject>>();
            Objects = new ConditionalWeakTable<DependencyObject, EventData>();
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
            MouseListener = new MouseInput();
            MouseListener.MouseLeftDown += OnMouseDown;
            //KeyboardListener = new KeyboardInput();
            //KeyboardListener.KeyDown += OnKeyDown;
        }

        //private static void OnKeyDown(object sender, KeyboardInput.RichKeyEventArgs e)
        //{

        //}

        internal enum ActionType
        {
            None = 0,
            Deselect
        }

        internal class EventData
        {
            public ActionType Action;
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //var target = e.MouseDevice.DirectlyOver as DependencyObject;
            //var parent = target != null ? VisualTreeHelper.GetParent(target).ToString() : string.Empty;

            foreach (var wr in EnumerableObjects.ToList())
            {
                DependencyObject dp;
                if (!wr.TryGetTarget(out dp))
                {
                    EnumerableObjects.Remove(wr);
                    continue;
                }

                EventData data;
                if (Objects.TryGetValue(dp, out data))
                {
                    switch (data.Action)
                    {
                        case ActionType.Deselect:

                            dp.Dispatcher.Invoke(() =>
                            {
                                if (!(bool)dp.GetValue(UIElement.IsFocusedProperty))
                                    return;

                                Keyboard.ClearFocus();  
                            });

                            break;
                    }
                }
            }
        }

        internal static void Add(DependencyObject dp, ActionType action)
        {
            var obj = Objects.GetOrCreateValue(dp);
            obj.Action = action;
            EnumerableObjects.Add(new WeakReference<DependencyObject>(dp));
        }

        internal static bool Remove(DependencyObject dp)
        {
            foreach (var wr in EnumerableObjects.ToList())
            {
                DependencyObject current;

                if (!wr.TryGetTarget(out current))
                {
                    EnumerableObjects.Remove(wr);
                    break;
                }

                if (dp.Equals(current))
                {
                    EnumerableObjects.Remove(wr);
                    break;
                }
            }

            return Objects.Remove(dp);
        }
    }
}