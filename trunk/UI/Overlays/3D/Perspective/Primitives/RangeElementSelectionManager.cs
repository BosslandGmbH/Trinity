//------------------------------------------------------------------
//
//  For licensing information and to get the latest version go to:
//  http://www.codeplex.com/perspective
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY
//  OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//  LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//  FITNESS FOR A PARTICULAR PURPOSE.
//
//------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace Perspective.Core.Primitives
{
    /// <summary>
    /// A class to handle multiselection of range elements (faders, knobs...)
    /// which implement IRangeElement interface
    /// </summary>
    public class RangeElementSelectionManager : List<IRangeElement>
    {
        private RangeElementSelectionManager()
        {
        }

        private static RangeElementSelectionManager _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets a signleton instance of RangeElementSelectionManager.
        /// </summary>
        public static RangeElementSelectionManager Current
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new RangeElementSelectionManager();
                    }
                    return _instance;
                }
            }
        }
        
        /// <summary>
        /// Increments the Value of other IRangeElement controls which have not the focus
        /// (the one which have the focus update his value himself)
        /// </summary>
        /// <param name="rate">Increment rate</param>
        public void ApplyRate(double rate)
        {
            foreach (IRangeElement re in this)
            {
                if ((!re.IsKeyboardFocused)
                    && (!re.IsKeyboardFocusWithin))
                {
                    re.Value += ((re.Maximum - re.Minimum) * rate);
                }
            }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <param name="d">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        public static void IsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IRangeElement re = (IRangeElement)d;
            if ((bool)e.NewValue == true)
            {
                if (!Current.Contains(re))
                {
                    Current.Add(re);
                }
            }
            else
            {
                if (Current.Contains(re))
                {
                    Current.Remove(re);
                }
            }
        }
    }
}
