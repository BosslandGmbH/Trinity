using System;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;

namespace Trinity.Framework.Objects.Api
{
    public class ApiBuildBuilder
    {
        public static ApiBuild CreateBuild()
        {
            var cubeItems = Reference.Legendary.Equipped.Where(i => i.IsEquippedInCube);
            var equippedItems = Core.Inventory.Equipped.Where(i => !i.IsGem && !Core.Inventory.KanaisCubeIds.Contains(i.ActorSnoId));
            var socketedItems = Core.Inventory.Equipped.Where(i => i.IsGem);

            var passives = Reference.PassiveUtils.Active.Select(p => new ApiPassive
            {
                Id = p.Id,
                Name = p.Name,
                IconSlug = p.IconSlug,
            });

            var skills = Reference.SkillUtils.Active.Select(p => new ApiSkill
            {
                Id = p.Id,
                Name = p.Name,
                RuneIndex = p.CurrentRune.Index,
                IconSlug = p.IconSlug,
                RuneName = p.CurrentRune.Name,
            });

            var sets = Reference.Sets.Equipped.Select(p => new ApiSet
            {
                Name = p.Name,
                EquippedCount = p.CurrentBonuses,
                MaxCount = p.MaxBonuses
            });

            var build = new ApiBuild
            {
                EquippedItems = equippedItems.Select(CreateItem).ToList(),
                CubedItems = cubeItems.Select(CreateItem).ToList(),
                SocketedItems = socketedItems.Select(CreateItem).ToList(),
                Passives = passives.ToList(),
                Skills = skills.ToList(),
                Sets = sets.ToList(),
            };

            return build;
        }

        public static ApiItem CreateItem(TrinityItem i)
        {
            var itemRef = i.Reference;
            var iconSlug = !string.IsNullOrEmpty(itemRef?.IconUrl) ? TrimEnd(i.Reference.IconUrl.Split('/').LastOrDefault(), ".png") : string.Empty;

            return new ApiItem
            {
                Id = i.ActorSnoId,
                Slot = i.InventorySlot,
                Name = i.Name,
                IsAncient = i.IsAncient,
                IconSlug = iconSlug
            };
        }

        public static ApiItem CreateItem(Item i)
        {
            var iconSlug = !string.IsNullOrEmpty(i.IconUrl) ? TrimEnd(i.IconUrl.Split('/').LastOrDefault(), ".png") : string.Empty;

            return new ApiItem
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
