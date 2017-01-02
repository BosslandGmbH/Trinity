using System;
using System.IO;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Settings;
using ItemEvents = Trinity.Framework.Events.ItemEvents;

namespace Trinity.Modules
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
