using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using Trinity.Components.Adventurer.Util;

namespace Trinity.Components.Adventurer.UI.UIComponents
{
    public class UILoader
    {

        private static Dictionary<string, string> _xaml = new Dictionary<string, string>();

        /// <summary>Loads the and transform xaml file.</summary>
        /// <param name="filename">The absolute path to the file to be loaded.</param>
        /// <returns><see cref="Stream"/> which contains transformed XAML file.</returns>
        public static T LoadAndTransformXamlFile<T>(string filename)
        {
            try
            {
                if (_xaml.ContainsKey(filename)) return (T)XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(_xaml[filename])));

                Logger.Debug("Load XAML file : {0}", filename);
                var filecontent = File.ReadAllText(filename);

                // Change reference to custom Trinity class
                filecontent = filecontent.Replace(
                    "xmlns:converters=\"clr-namespace:Trinity.Components.Adventurer.UI.UIComponents.Converters\"",
                    "xmlns:converters=\"clr-namespace:Trinity.Components.Adventurer.UI.UIComponents.Converters;assembly=" +
                    Assembly.GetExecutingAssembly().GetName().Name + "\"");
                filecontent = filecontent.Replace(
                    "xmlns:uiComponents=\"clr-namespace:Trinity.Components.Adventurer.UI.UIComponents\"",
                    "xmlns:uiComponents=\"clr-namespace:Trinity.Components.Adventurer.UI.UIComponents;assembly=" +
                    Assembly.GetExecutingAssembly().GetName().Name + "\"");
                filecontent =
                    filecontent.Replace(
                        "xmlns:radarCanvas=\"clr-namespace:Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas\"",
                        "xmlns:radarCanvas=\"clr-namespace:Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas;assembly=" +
                        Assembly.GetExecutingAssembly().GetName().Name + "\"");
                filecontent = filecontent.Replace("xmlns:objects=\"clr-namespace:Adventurer.Objects\"",
                    "xmlns:objects=\"clr-namespace:Trinity.Components.Adventurer.Objects;assembly=" +
                    Assembly.GetExecutingAssembly().GetName().Name + "\"");
                filecontent = filecontent.Replace("xmlns:mock=\"clr-namespace:Trinity.Components.Adventurer.Settings.Mock\"",
                    "xmlns:mock=\"clr-namespace:Trinity.Components.Adventurer.Settings.Mock;assembly=" +
                    Assembly.GetExecutingAssembly().GetName().Name + "\"");
                filecontent = filecontent.Replace("xmlns:charts=\"clr-namespace:LineChartLib\"",
                    "xmlns:charts=\"clr-namespace:LineChartLib;assembly=" +
                    Assembly.GetExecutingAssembly().GetName().Name + "\"");

                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                filecontent = filecontent.Replace("xmlns:radarCanvas=\"clr-namespace:Trinity.UI.Visualizer.RadarCanvas\"", "xmlns:radarCanvas=\"clr-namespace:Trinity.UI.Visualizer.RadarCanvas;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:itemlist=\"clr-namespace:Trinity.Settings.ItemList\"", "xmlns:itemlist=\"clr-namespace:Trinity.Settings.ItemList;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:markupExtensions=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions\"", "xmlns:markupExtensions=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:markup=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions\"", "xmlns:markup=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:input=\"clr-namespace:Trinity.UI.UIComponents.Input\"", "xmlns:input=\"clr-namespace:Trinity.UI.UIComponents.Input;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:behaviors=\"clr-namespace:Trinity.UI.UIComponents.Behaviors\"", "xmlns:behaviors=\"clr-namespace:Trinity.UI.UIComponents.Behaviors;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:controls=\"clr-namespace:Trinity.UI.UIComponents.Controls\"", "xmlns:controls=\"clr-namespace:Trinity.UI.UIComponents.Controls;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents\"", "xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:objects=\"clr-namespace:Trinity.Framework.Objects\"", "xmlns:objects=\"clr-namespace:Trinity.Framework.Objects;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:mock=\"clr-namespace:Trinity.Settings.Mock\"", "xmlns:mock=\"clr-namespace:Trinity.Settings.Mock;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:settings=\"clr-namespace:Trinity.Settings\"", "xmlns:settings=\"clr-namespace:Trinity.Settings;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:charts=\"clr-namespace:LineChartLib\"", "xmlns:charts=\"clr-namespace:LineChartLib;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents\"", "xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:converters=\"clr-namespace:Trinity.UI.UIComponents.Converters\"", "xmlns:converters=\"clr-namespace:Trinity.UI.UIComponents.Converters;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:converters2=\"clr-namespace:Trinity.UI.UIComponents.Converters\"", "xmlns:converters2=\"clr-namespace:Trinity.UI.UIComponents.Converters;assembly=" + assemblyName + "\"");

                filecontent = filecontent.Replace("xmlns:radarCanvas=\"clr-namespace:Trinity.UI.UIComponents.RadarCanvas\"", "xmlns:radarCanvas=\"clr-namespace:Trinity.UI.UIComponents.RadarCanvas;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:ui=\"clr-namespace:Trinity.UI.UIComponents\"", "xmlns:ui=\"clr-namespace:Trinity.UI.UIComponents;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:dd=\"clr-namespace:GongSolutions.Wpf.DragDrop\"", "xmlns:dd=\"clr-namespace:GongSolutions.Wpf.DragDrop;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:enums=\"clr-namespace:Trinity.Framework.Objects.Enums\"", "xmlns:enums=\"clr-namespace:Trinity.Framework.Objects.Enums;assembly=" + assemblyName + "\"");
                filecontent = Regex.Replace(filecontent, "<ResourceDictionary.MergedDictionaries>.*</ResourceDictionary.MergedDictionaries>", string.Empty, RegexOptions.Singleline | RegexOptions.Compiled);



                // Remove Template designer reference
                //filecontent = filecontent.Replace("<ResourceDictionary.MergedDictionaries><ResourceDictionary Source=\"..\\Template.xaml\"/></ResourceDictionary.MergedDictionaries>", string.Empty);
                //filecontent = filecontent.Replace("<ResourceDictionary.MergedDictionaries><ResourceDictionary Source=\"Template.xaml\"/></ResourceDictionary.MergedDictionaries>", string.Empty);

                _xaml.Add(filename, Regex.Replace(filecontent,
                    "<ResourceDictionary.MergedDictionaries>.*</ResourceDictionary.MergedDictionaries>",
                    string.Empty, RegexOptions.Singleline | RegexOptions.Compiled));

                return (T)XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(_xaml[filename])));
            }
            catch (Exception ex)
            {
                Logger.Debug("Error loading/transforming XAML {0}", ex);
                return default(T);
            }
        }





    }
}
