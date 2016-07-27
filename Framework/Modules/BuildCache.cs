using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventurer.Util;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.API;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Modules
{
    public class BuildCache : Module
    {
        protected override int UpdateIntervalMs => 10000;

        public Build Current { get; set; } = new Build();

        protected override void OnPulse()
        {
            var cubeItems = Reference.Legendary.Equipped.Where(i => i.IsEquippedInCube);
            var equippedItems = CacheData.Inventory.Equipped.Where(i => !i.IsGem && !CacheData.Inventory.KanaisCubeIds.Contains(i.ActorSnoId));
            var socketedItems = Core.Inventory.Equipped.Where(i => i.IsGem);

            var passives = Reference.PassiveUtils.Active.Select(p => new Passive
            {
                Id = p.Id,
                Name = p.Name,
                IconSlug = p.IconSlug,
            });

            var skills = Reference.SkillUtils.Active.Select(p => new Skill
            {
                Id = p.Id,
                Name = p.Name,
                RuneIndex = p.CurrentRune.Index,
                IconSlug = p.IconSlug,
                RuneName = p.CurrentRune.Name,
            });

            var sets = Reference.Sets.Equipped.Select(p => new Set
            {
                Name = p.Name,
                EquippedCount = p.CurrentBonuses,
                MaxCount = p.MaxBonuses
            });

         
            var build = new Build
            {
                EquippedItems = equippedItems.Select(CreateItem).ToList(),
                CubedItems = cubeItems.Select(CreateItem).ToList(),
                SocketedItems = socketedItems.Select(CreateItem).ToList(),
                Passives = passives.ToList(),
                Skills = skills.ToList(),
                Sets = sets.ToList(),
            };

            Current = build;                   
        }

        public static Item CreateItem(TrinityItem i)
        {
            var itemRef = i.Reference;
            var iconSlug = !string.IsNullOrEmpty(itemRef?.IconUrl) ? TrimEnd(i.Reference.IconUrl.Split('/').LastOrDefault(), ".png") : string.Empty;

            return new Item
            {
                Id = i.ActorSnoId,
                Slot = i.InventorySlot,
                Name = i.Name,
                IsAncient = i.IsAncient,
                IconSlug = iconSlug
            };
        }

        public static Item CreateItem(Trinity.Objects.Item i)
        {
            var iconSlug = !string.IsNullOrEmpty(i.IconUrl) ? TrimEnd(i.IconUrl.Split('/').LastOrDefault(), ".png") : string.Empty;

            return new Item
            {
                Id = i.Id,
                Name = i.Name,
                Slot = InventorySlot.None,
                IsAncient = false,
                IconSlug = iconSlug
            };
        }

        public static string TrimEnd(string source, string value)
        {
            return !source.EndsWith(value) ? source : source.Remove(source.LastIndexOf(value, StringComparison.Ordinal));
        }
    }
}
