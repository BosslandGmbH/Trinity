using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Trinity.Reference;
using Trinity.Settings.Loot;
using Trinity.UI.UIComponents;
using Trinity.UIComponents;
using Zeta.Game.Internals.Actors;

namespace Trinity.Settings.Mock
{
    public class ItemListMockData
    {
        public CollectionViewSource Collection { get; set; }   
        public List<LItem> DisplayItems { get; set; }

        /// <summary>
        /// Mock Data for viewing ItemList controls in DesignTime
        /// </summary>
        public ItemListMockData()
        {
            DisplayItems = new List<LItem>
            {
                new LItem(Legendary.BombardiersRucksack)
                {             
                    IsSelected = true,
                    Rules = new ObservableCollection<LRule>
                    {
                        new LRule
                        {
                            Id = (int)ItemProperty.Ancient, 
                            RuleType = RuleType.Optional
                        },
                        new LRule
                        {
                            Id = (int)ItemProperty.PrimaryStat,
                            Value = 445
                        },
                        new LRule
                        {
                            Id = (int)ItemProperty.SkillDamage, 
                            RuleType = RuleType.Optional
                        },
                        new LRule
                        {
                            Id = (int)ItemProperty.AttackSpeed, 
                            RuleType = RuleType.Required
                        }
                    }
                },
                //new LItem(Legendary.BoardWalkers),
                new LItem(Legendary.LutSocks),
                //new ItemListItem(Legendary.BreastplateOfAkkhan)
                //{
                //    IsSelected = true
                //},
                new LItem(Legendary.Cindercoat),
                new LItem(Legendary.FlyingDragon), 
            };

            Collection = new CollectionViewSource();
            Collection.Source = DisplayItems;
            Collection.GroupDescriptions.Add(new PropertyGroupDescription("ItemType"));
        }
    }
}
