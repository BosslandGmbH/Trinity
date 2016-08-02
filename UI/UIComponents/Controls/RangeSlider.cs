using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CustomControls
{
    public class RangeSlider : UserControl
    {
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, OnLowerValueChanged));

        private static void OnLowerValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var rangeSlider = dependencyObject as RangeSlider;
            if (rangeSlider == null) return;

            rangeSlider.UpperValue = Math.Max(rangeSlider.UpperValue, rangeSlider.LowerValue);
            rangeSlider.UpdateSelectedVisualMargin();
        }

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, OnUpperValueChanged));

        private static void OnUpperValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var rangeSlider = dependencyObject as RangeSlider;
            if (rangeSlider == null) return;

            rangeSlider.LowerValue = Math.Min(rangeSlider.UpperValue, rangeSlider.LowerValue);
            rangeSlider.UpdateSelectedVisualMargin();
        }

        private void UpdateSelectedVisualMargin()
        {
            if (Maximum <= 0) return;
            var upperPct = UpperValue / Maximum;
            var lowerPct = LowerValue / Maximum;
            var adjustedWidth = this.ActualWidth - ThumbWidth;
            var lowerWidth = adjustedWidth * lowerPct;
            var upperWidth = adjustedWidth - (adjustedWidth * upperPct);
            SelectedVisualMargin = new Thickness(lowerWidth, 0, upperWidth, 0);
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1d));

        public double ThumbWidth
        {
            get { return (double)GetValue(ThumbWidthProperty); }
            set { SetValue(ThumbWidthProperty, value); }
        }

        public static readonly DependencyProperty ThumbWidthProperty =
            DependencyProperty.Register("ThumbWidth", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(10d));

        public Thickness SelectedVisualMargin
        {
            get { return (Thickness)GetValue(SelectedVisualMarginProperty); }
            set { SetValue(SelectedVisualMarginProperty, value); }
        }

        public static readonly DependencyProperty SelectedVisualMarginProperty =
            DependencyProperty.Register("SelectedVisualMargin", typeof(Thickness), typeof(RangeSlider), new UIPropertyMetadata(default(Thickness)));

    }

}
