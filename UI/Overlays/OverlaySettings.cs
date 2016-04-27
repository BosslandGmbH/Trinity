using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Trinity.Config;
using Trinity.Configuration;
using Trinity.Helpers;
using Trinity.UI.UIComponents;
using Zeta.Game.Internals.Actors;
using Zeta.XmlEngine;

namespace Trinity.UI.Overlays
{
    public class OverlaySettings : NotifyBase, ITrinitySetting<OverlaySettings>
    {
        private static OverlaySettings _instance;        
        public static OverlaySettings Instance
        {
            get { return _instance ?? (_instance = new OverlaySettings()); }
            set { _instance = value; }
        }

        private bool _showBorderEffect;
        private bool _showToolbar;

        [Setting, XmlElement("ShowBorderEffect")]
        [DisplayName("Show Border Effect")]
        [Description("Shows a colorful border around the edges of the game window")]
        [DefaultValue(true)]
        [Category("Quality")]
        public bool ShowBorderEffect
        {
            get { return _showBorderEffect; }
            set { SetField(ref _showBorderEffect, value); }
        }

        [Setting, XmlElement("ShowToolbar")]
        [DisplayName("Show Toolbar")]
        [DefaultValue(true)]
        [Category("Quality")]
        public bool ShowToolbar
        {
            get { return _showToolbar; }
            set { SetField(ref _showToolbar, value); }
        }

        public void Reset()
        {
            LoadDefaults();
        }

        public void CopyTo(OverlaySettings setting)
        {
            setting.ShowToolbar = ShowToolbar;
            setting.ShowBorderEffect = ShowBorderEffect;
        }

        public OverlaySettings Clone()
        {
            var clone = new OverlaySettings();
            CopyTo(clone);
            return clone;
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            LoadDefaults();
        }
    }
}
