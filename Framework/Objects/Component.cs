using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Trinity.Components.Adventurer.Settings;
using Zeta.Bot;

namespace Trinity.Framework.Objects
{
    public class Component : Module
    {

    }

    public interface IDynamicSetting
    {
        string Name { get; }
        UserControl Control { get; }
        object DataContext { get; }
        string GetCode();
        void ApplyCode(string code);
        void Reset();
        void Save();
    }
}


