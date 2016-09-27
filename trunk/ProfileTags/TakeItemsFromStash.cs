using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Coroutines.Resources;
using Trinity.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Helpers;
using Trinity.ProfileTags.EmbedTags;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;

namespace Trinity.ProfileTags
{
    [XmlElement("TakeFromStash")]
    public class TakeFromStashTag : TrinityProfileBehavior
    {
        [XmlElement("Items")]
        public List<ItemTag> Items { get; set; }

        private bool _isDone;
        public override bool IsDone => _isDone;
        protected override Composite CreateBehavior() => new ActionRunCoroutine(ret => Task());

        public async Task<bool> Task()
        {
            if(Items == null || !Items.Any())
            {
                Logger.LogError("[TakeFromStash] No items were specified. Use: <TakeFromStash><Items><Item id=\"0\" quantity =\"0\" /></Items></TakeFromStash>");
                _isDone = true;
                return false;
            }

            foreach (var item in Items)
            {
                var stashItem = Inventory.Stash.ByActorSNO(item.Id);
                if (stashItem == null)
                {
                    Logger.LogError("[TakeFromStash] Item Id={0} was not found in stash", item.Id);
                    _isDone = true;
                    return false;
                }

                if (!await TakeItemsFromStash.Execute(new List<int> { item.Id }, Math.Max(1,item.Quantity)))
                {
                    Logger.LogError("[TakeFromStash] TakeItemsFromStash coroutine failed.");
                    _isDone = true;
                    return false;
                }
            }

            _isDone = true;
            return true;
        }
    }
}

