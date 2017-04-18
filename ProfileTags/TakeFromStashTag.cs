using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Coroutines.Town;
using Trinity.Components.QuestTools;
using Trinity.ProfileTags.EmbedTags;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("TakeItemsFromStash")]
    [XmlElement("TakeFromStash")]
    public class TakeFromStashTag : BaseProfileBehavior
    {
        [XmlElement("Items")]
        [Description("Items to be taken from stash")]
        public List<ItemTag> Items { get; set; }

        public override async Task<bool> MainTask()
        {
            if (Items == null || !Items.Any())
            {
                Core.Logger.Error("[TakeFromStash] No items were specified. Use: <TakeFromStash><Items><Item id=\"0\" quantity =\"0\" /></Items></TakeFromStash>");
                return true;
            }

            foreach (var item in Items)
            {
                var stashItem = Core.Inventory.Stash.FirstOrDefault(i => i.ActorSnoId == item.Id);
                if (stashItem == null)
                {
                    if (item.Name != null)
                    {
                        stashItem = Core.Inventory.Stash.FirstOrDefault(i => i.Name.ToLowerInvariant().Contains(item.Name.ToLowerInvariant()));
                    }
                    if (stashItem == null)
                    {
                        if (item.Quality != TrinityItemQuality.Invalid && item.Quality != TrinityItemQuality.None)
                        {
                            stashItem = Core.Inventory.Stash.FirstOrDefault(i => i.TrinityItemQuality == item.Quality);
                        }
                        if (stashItem == null)
                        {
                            Core.Logger.Error("[TakeFromStash] Unable to find item in stash", item.Id);
                            return true;
                        }
                    }
                }

                if (!await TakeItemsFromStash.Execute(new List<int> { item.Id }, Math.Max(1, item.Quantity)))
                {
                    Core.Logger.Error("[TakeFromStash] TakeItemsFromStash coroutine failed.");
                    return true;
                }
            }

            return true;
        }
    }
}

