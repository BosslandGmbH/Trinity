using System;
using Trinity.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Zeta.Bot;

namespace Trinity.UI.UIComponents.Converters
{
    public class PropertyGridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return GetProperties(value);
        }

        private static IEnumerable<BindingMember> GetProperties(object value, BindingMember parent = null)
        {
            var previousExecutionState = BotMain.IsPausedForStateExecution;

            BotMain.IsPausedForStateExecution = false;

            var results = new ConcurrentBag<BindingMember>();

            var props = value.GetType().GetProperties();

            //var interfacePropsNames = new HashSet<string>(value.GetType().GetInterfaces().SelectMany(i => i.GetProperties()).Select(p => p.Name));

            Parallel.ForEach(props, prop =>
            {
                BindingMember bm = null;
                try
                {
                    //if (!interfacePropsNames.Contains(prop.Name))
                    //    return;

                    bm = new BindingMember(prop, value, null, true);
                }
                catch (Exception ex)
                {
                    Core.Logger.Error("PropertyGridConverter Exception: {0}", ex);
                }

                if (bm != null)
                    results.Add(bm);         
                    
            });

            BotMain.IsPausedForStateExecution = previousExecutionState;

            return results.OrderBy(p => p.PropertyName);
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

