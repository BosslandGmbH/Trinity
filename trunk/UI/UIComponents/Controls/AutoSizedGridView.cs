﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zeta.Common;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.UIComponents
{
    /// <summary>
    /// GridViewColumns set to "Auto" will be resized when underlying data is updated.
    /// </summary>
    public class AutoSizedGridView : GridView
    {
        HashSet<int> _autoWidthColumns;

        protected override void PrepareItem(ListViewItem item)
        {
            if (_autoWidthColumns == null)
            {
                _autoWidthColumns = new HashSet<int>();

                foreach (var column in Columns)
                {
                    if(double.IsNaN(column.Width))
                        _autoWidthColumns.Add(column.GetHashCode());
                }                
            }

            foreach (var column in Columns)
            {
                if (_autoWidthColumns.Contains(column.GetHashCode()))
                {
                    if (double.IsNaN(column.Width))
                        column.Width = column.ActualWidth;

                    column.Width = double.NaN;                    
                }          
            }

            base.PrepareItem(item);
        }        
    }
}
