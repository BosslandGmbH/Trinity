using System;
using System.Windows;
using System.Windows.Controls;
using Trinity.Framework.Helpers;

namespace Trinity.UIComponents
{
    public class GridViewColumnExtended : GridViewColumn
    {
        // Variables
        public static DependencyProperty VisibleProperty;

        // PropertyLoader
        public Boolean Visible
        {
            get { return (Boolean)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        // Constructors
        static GridViewColumnExtended()
        {
            VisibleProperty = DependencyProperty.Register("Visible",
                typeof(Boolean),
                typeof(GridViewColumnExtended),
                new PropertyMetadata(true, OnVisibleChanged));

            WidthProperty.OverrideMetadata(typeof(GridViewColumnExtended),
                new FrameworkPropertyMetadata((Double)0, null,
                    CoerceWidth));

            
            //MinWidthProperty.OverrideMetadata(typeof(ColumnDefinitionExtended),
            //    new FrameworkPropertyMetadata((Double)0, null,
            //        new CoerceValueCallback(CoerceMinWidth)));
        }

        // Get/Set
        public static void SetVisible(DependencyObject obj, Boolean nVisible)
        {
            obj.SetValue(VisibleProperty, nVisible);
        }
        public static Boolean GetVisible(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(VisibleProperty);
        }

        static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //Log.Warn("Visible was changed for {0}",obj.GetValue(HeaderProperty));

            //if ((bool) e.NewValue)
            //{
            //    _previousWidth = obj.GetValue(WidthProperty);
            //    obj.SetValue(WidthProperty, new GridLength(0));
            //}
            //else
            //{
            //    obj.SetValue(WidthProperty, _previousWidth);
            //}

            obj.CoerceValue(WidthProperty);
            //obj.CoerceValue(MinWidthProperty);
        }

        static Object CoerceWidth(DependencyObject obj, Object nValue)
        {
            if ((double)nValue < 0)
                nValue = 0;

            return (((GridViewColumnExtended)obj).Visible) ? nValue : (Double)0;
        }

        //static Object CoerceMinWidth(DependencyObject obj, Object nValue)
        //{
        //    return (((ColumnDefinitionExtended)obj).Visible) ? nValue : (Double)0;
        //}
    }
}

