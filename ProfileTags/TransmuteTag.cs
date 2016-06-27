using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Helpers;
using Trinity.ProfileTags.EmbedTags;
using TrinityCoroutines.Resources;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Trinity.Technicals;
using TrinityCoroutines;
using Zeta.Game.Internals.Actors;

namespace Trinity.ProfileTags
{
    [XmlElement("Transmute")]
    public class TransmuteTag : ProfileBehavior
    {
        [XmlElement("Items")]
        public List<ItemTag> Items { get; set; }

        private bool _isDone;
        public override bool IsDone { get { return _isDone; } }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ret => Task());
        }

        public async Task<bool> Task()
        {
            if(Items == null || !Items.Any())
            {
                Logger.LogError("[TransmuteTag] No items were specified. Use: <Transmute><Items><Item id=\"0\" quantity =\"0\" /></Items></Transmute>");
                _isDone = true;
                return false;
            }

            if (!GameUI.KanaisCubeWindow.IsVisible)
            {
                Logger.LogError("[TransmuteTag] Kanai's Cube window must be visible");
                _isDone = true;
                return false;
            }

            var transmuteGroup = new List<TrinityItem>();

            foreach (var item in Items)
            {
                var backpackItem = Inventory.Backpack.ByActorSNO(item.Id);
                if (backpackItem == null)
                {
                    Logger.LogError("[TransmuteTag] Item Id={0} was not found in backpack", item.Id);
                    _isDone = true;
                    return false;
                }

                var stacks = Inventory.GetStacksUpToQuantity(backpackItem, item.Quantity);
                transmuteGroup.AddRange(stacks);
            }

            if (!await Transmute.Execute(transmuteGroup))
            {
                Logger.LogError("[TransmuteTag] Trasmute Failed.");
                _isDone = true;
                return false;
            }

            _isDone = true;
            return true;
        }
    }
}

