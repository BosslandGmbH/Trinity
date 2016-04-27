// AutoAxis.cs by Charles Petzold, September 2009
using System;
using System.Windows;

namespace LineChartLib
{
    public class AutoAxis : AxisStrategy
    {
        protected override void CalculateAxisItems(Type propertyType, ref double minValue, ref double maxValue)
        {
            for (int index = 0; index < Parent.ItemsSource.Count; index++)
            {
                object item = Parent.ItemsSource[index];
                double value = GetItemPropertyValue(item, PropertyName);

                AxisItem axisItem = new AxisItem()
                {
                    Item = ConvertFromDouble(value, propertyType),
                    Offset = Length * (value - minValue) / (maxValue - minValue)
                };

                if (IsFlipped)
                    axisItem.Offset = Length - axisItem.Offset;

                AxisItems.Add(axisItem);
            }
        }
    }
}
