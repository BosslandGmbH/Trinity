// AxisItem.cs by Charles Petzold, September 2009
using System;
using System.ComponentModel;

namespace LineChartLib
{
    public class AxisItem : INotifyPropertyChanged
    {
        object item;
        double offset;

        public event PropertyChangedEventHandler PropertyChanged;

        public object Item
        {
            set
            {
                if (value != item)
                {
                    item = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Item"));
                }
            }
            get
            {
                return item;
            }
        }

        public double Offset
        {
            set
            {
                if (value != offset)
                {
                    offset = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Offset"));
                }
            }
            get
            {
                return offset;
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
    }
}
