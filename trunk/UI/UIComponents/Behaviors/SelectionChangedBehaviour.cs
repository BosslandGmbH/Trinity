using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Trinity.UI.UIComponents.Behaviors
{
    public class SelectionChangedBehaviour
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand),
        typeof(SelectionChangedBehaviour), new PropertyMetadata(PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            Selector selector = (Selector)depObj;
            if (selector != null)
            {
                selector.SelectionChanged += SelectionChanged;
            }
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static void SetCommand(UIElement element, ICommand command)
        {
            element.SetValue(CommandProperty, command);
        }

        private static DateTime LastEventHandled = DateTime.UtcNow;

        private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = (Selector)sender;

            if (selector != null)
            {
                //// Temp Debounce Bad Event Binding
                //if (DateTime.UtcNow.Subtract(LastEventHandled).TotalSeconds < 1 && selector.SelectedItem != null)
                //{
                //    Log.Info("Debounce {0}", DateTime.UtcNow.Subtract(LastEventHandled).TotalSeconds);
                //    e.Handled = true;
                //    return;
                //}
                //Core.Logger.Log("SelectedIndex={0} SelectedItem={1} SelectedValue={2}",
                //    selector.SelectedIndex, selector.SelectedItem, selector.SelectedValue);

                ICommand command = selector.GetValue(CommandProperty) as ICommand;
                if (command != null)
                {
                    command.Execute(selector.SelectedItem);
                    selector.SelectedIndex = -1;
                    LastEventHandled = DateTime.UtcNow;
                    e.Handled = true;
                }
            }
        }
    }

    //public class SelectionChangedBehaviour
    //{
    //    public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand),
    //    typeof(SelectionChangedBehaviour), new PropertyMetadata(PropertyChangedCallback));

    //    public static void PropertyChangedCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
    //    {
    //        Selector selector = (Selector)depObj;
    //        if (selector != null)
    //        {
    //            selector.SelectionChanged += SelectionChanged;
    //        }
    //    }

    //    public static ICommand GetCommand(UIElement element)
    //    {
    //        return (ICommand)element.GetValue(CommandProperty);
    //    }

    //    public static void SetCommand(UIElement element, ICommand command)
    //    {
    //        element.SetValue(CommandProperty, command);
    //    }

    //    private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
    //        Selector selector = (Selector)sender;
    //        if (selector != null)
    //        {
    //            ICommand command = selector.GetValue(CommandProperty) as ICommand;
    //            if (command != null)
    //            {
    //                command.Execute(selector.SelectedItem);
    //            }
    //        }
    //    }
    //}

}
