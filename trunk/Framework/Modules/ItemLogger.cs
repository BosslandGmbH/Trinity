using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;
using ItemEvents = Trinity.Framework.Events.ItemEvents;

namespace Trinity.Framework.Modules
{
    public class ItemLogger : Module
    {
        public enum ItemAction
        {
            None = 0,
            Identified,
            Salvaged,
            Stashed,
            Sold
        }

        public ItemLogger()
        {
            ItemEvents.OnItemSalvaged += item => LogItem(ItemAction.Salvaged, item);
            ItemEvents.OnItemStashed += item => LogItem(ItemAction.Stashed, item);
            ItemEvents.OnItemSold += item => LogItem(ItemAction.Sold, item);
            ItemEvents.OnItemIdentified += item => LogItem(ItemAction.Identified, item);
        }

        private void LogItem(ItemAction action, TrinityItem item)
        {
            if (!IsEnabled || !Core.Settings.Advanced.LogItems)
                return;            

            if (item.TrinityItemQuality < TrinityItemQuality.Legendary)
                return;

            var file = $"{action} - {Core.Player.ActorClass}.log";
            var path = Path.Combine(FileManager.LoggingPath, file);
            var message = LogTemplate(action, item);
            File.AppendAllText(path, message);
        }

        public string LogTemplate(ItemAction action, TrinityItem item)
            => $"{Environment.NewLine}" +
               $"{DateTime.UtcNow.ToLocalTime():f} {Environment.NewLine}" +
               $"{item.Name} ({item.ActorSnoId}), {item.RawItemType} {Environment.NewLine}" +
               $"{item.ItemBaseType}: {item.TrinityItemType}, {item.TrinityItemQuality}" +
               $"{(item.IsAncient ? ", Ancient" : string.Empty)}, {item.Attributes.Summary()}" + 
               $"{Environment.NewLine}";
    }


}
