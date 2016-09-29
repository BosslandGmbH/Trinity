using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Trinity.UI.UIComponents.Behaviors
{
    public class CancelMouseBubbling : DependencyObject
    {
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(CancelMouseBubbling),
            new PropertyMetadata(false, ActivePropertyChanged));

        /// <summary>
        /// Subscribe to the events we need.
        /// </summary>
        private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null)
            {
                if ((e.NewValue as bool?).GetValueOrDefault(false))
                {
                    element.PreviewMouseLeftButtonDown += ElementOnPreviewMouseLeftButtonDown;
                    element.MouseLeftButtonDown += ElementOnMouseLeftButtonDown;
                }
                else
                {
                    element.PreviewMouseLeftButtonDown -= ElementOnPreviewMouseLeftButtonDown;
                    element.MouseLeftButtonDown -= ElementOnMouseLeftButtonDown;
                }
            }
        }

        /// <summary>
        /// Block some events from bubbling at the OriginalSource.
        /// </summary>
        private static void ElementOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.Source is Panel)
            {
                mouseButtonEventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Block all clicks from going past the element CancelMouseBubbling is set on
        /// </summary>
        private static void ElementOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            mouseButtonEventArgs.Handled = true;
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetActive(DependencyObject @object)
        {
            return (bool)@object.GetValue(ActiveProperty);
        }

        public static void SetActive(DependencyObject @object, bool value)
        {
            @object.SetValue(ActiveProperty, value);
        }
    }
}
