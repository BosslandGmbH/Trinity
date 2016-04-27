// ItemPoint.cs by Charles Petzold, September 2009
using System;
using System.Windows;
using System.ComponentModel;

namespace LineChartLib
{
    public class ItemPoint : INotifyPropertyChanged
    {
        object item;
        Point point;

        public event PropertyChangedEventHandler PropertyChanged;

        public ItemPoint(object item)
        {
            Item = item;
        }

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

        public Point Point
        {
            set
            {
                if (value != point)
                {
                    point = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Point"));
                }
            }
            get
            {
                return point;
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
    }
}