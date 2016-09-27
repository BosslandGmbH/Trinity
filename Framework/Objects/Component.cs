using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static Component Instance { get; protected set; }

        protected Component()
        {
            Instance = this;
        }
    }

    public interface IDynamicSetting
    {
        string GetName();
        UserControl GetControl();
        object GetDataContext();
        string GetCode();
        void ApplyCode(string code);
        void Reset();
        void Save();
    }
}


