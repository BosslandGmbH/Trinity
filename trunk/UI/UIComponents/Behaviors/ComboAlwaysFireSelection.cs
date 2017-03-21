using System;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Trinity.UI.UIComponents.Behaviors
{
    public class ComboAlwaysFireSelection : DependencyObject
    {
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(ComboAlwaysFireSelection),
            new PropertyMetadata(false, ActivePropertyChanged));

        private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as ComboBox;
            if (element == null) 
                return;

            if ((e.NewValue as bool?).GetValueOrDefault(false))
            {
                element.DropDownClosed += ElementOnDropDownClosed;
                element.DropDownOpened += ElementOnDropDownOpened;
            }
            else
            {
                element.DropDownClosed -= ElementOnDropDownClosed;
                element.DropDownOpened -= ElementOnDropDownOpened;
            }
        }

        private static void ElementOnDropDownOpened(object sender, EventArgs eventArgs)
        {
            _selectedIndex = ((ComboBox) sender).SelectedIndex;
        }

        private static int _selectedIndex;

        private static void ElementOnDropDownClosed(object sender, EventArgs eventArgs)
        {
            var comboBox = ((ComboBox) sender);
            if (comboBox.SelectedIndex == _selectedIndex)
            {
                comboBox.RaiseEvent(new SelectionChangedEventArgs(Selector.SelectionChangedEvent, new ListItemCollection(), new ListItemCollection()));
            }
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
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
