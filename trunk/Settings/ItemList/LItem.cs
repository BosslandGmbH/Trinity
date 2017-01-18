using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.UI.UIComponents;

namespace Trinity.Settings.ItemList
{
    /// <summary>
    /// Item Object wrapped for use in SettingsUI
    /// </summary>
    [DataContract(Namespace = "")]
    public class LItem : Item, INotifyPropertyChanged, ICloneable
    {
        private bool _isSelected;

        private List<ItemProperty> _itemProperties;
        private ObservableCollection<LRule> _rules = new ObservableCollection<LRule>();
        private int _ops;

        public Item ItemReference { get; set; }

        public LItem()
        {


        }

        public LItem(Item itemReference)
        {
            LoadCommands();

            Type = ILType.Item;

            ItemReference = itemReference;
            Name = itemReference.Name;
            Id = itemReference.Id;
            BaseType = itemReference.BaseType;
            DataUrl = itemReference.DataUrl;
            InternalName = itemReference.InternalName;
            IsCrafted = itemReference.IsCrafted;
            ItemType = itemReference.ItemType;
            LegendaryAffix = itemReference.LegendaryAffix;
            Quality = itemReference.Quality;
            RelativeUrl = itemReference.RelativeUrl;
            IsCrafted = itemReference.IsCrafted;
            Slug = itemReference.Slug;
            Url = itemReference.Url;
            IconUrl = itemReference.IconUrl;
            IsTwoHanded = itemReference.IsTwoHanded;
            TrinityItemType = itemReference.TrinityItemType;
            IsSetItem = itemReference.IsSetItem;
            SetName = itemReference.IsSetItem ? itemReference.Set.Name : "None";
        }

        public new bool IsSetItem { get; set; }

        public string InvalidReason { get; set; }

        public string QualityTypeLabel => $"{Quality} {(IsSetItem ? "Set " : string.Empty)}{ItemType}";

        public bool IsValid
        {
            get
            {
                if (Id == 0)
                {
                    InvalidReason = "Id is 0";
                    return false;
                }

                return true;
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public ILType Type { get; set; } = ILType.Item;

        public enum ILType
        {
            None = 0,
            Item,
            Slot
        }

        /// <summary>
        /// PropertyLoader that are valid for this particular item
        /// </summary>
        public List<ItemProperty> ItemProperties
        {
            get
            {
                if (_itemProperties != null)
                    return _itemProperties;

                if (Type == ILType.Slot)
                {
                    return _itemProperties = ItemDataUtils.GetPropertiesForItemType(TrinityItemType);
                }

                return _itemProperties = ItemDataUtils.GetPropertiesForItem(ItemReference);
            }
            set
            {
                if (_itemProperties != value)
                {
                    _itemProperties = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The number of optional rules that must be True
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int Ops
        {
            get
            {
                if (_ops == 0)
                    _ops = 1;

                return _ops;
            }
            set
            {
                if (_ops != value)
                {
                    _ops = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<int> OpCountNumbers
        {
            get { return new ObservableCollection<int>(new List<int> { 1, 2, 3, 4 }); }
        }

        public void Reset()
        {
            IsSelected = false;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //Logger.Log("Property Changed {0}", propertyName);
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            return new LItem(ItemReference)
            {
                IsSelected = IsSelected,
            };
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public ObservableCollection<LRule> Rules
        {
            get { return _rules; }
            set
            {
                if (_rules != value)
                {
                    _rules = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(OptionalRules));
                    OnPropertyChanged(nameof(RequiredRules));
                }
            }
        }

        public ObservableCollection<LRule> RequiredRules
        {
            get { return new ObservableCollection<LRule>(Rules.Where(r => r.RuleType == RuleType.Required)); }
        }

        public ObservableCollection<LRule> OptionalRules
        {
            get { return new ObservableCollection<LRule>(Rules.Where(r => r.RuleType == RuleType.Optional)); }
        }

        public ItemStatRange GetItemStatRange(ItemProperty property)
        {
            if (Type == ILType.Slot)
            {
                return ItemDataUtils.GetItemStatRange(TrinityItemType, property);
            }
            return ItemDataUtils.GetItemStatRange(ItemReference, property);
        }

        #region Commands

        public ICommand AddRequiredRuleCommand { get; set; }
        public ICommand AddOptionalRuleCommand { get; set; }
        public ICommand RemoveRuleCommand { get; set; }
        public ItemSelectionType ItemSelectionType { get; set; }

        public void LoadCommands()
        {
            AddRequiredRuleCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (parameter == null)
                    {
                        return;
                    }

                    Logger.Log("AddRequiredRuleCommand {0}", parameter.ToString());

                    var item = parameter as ComboBoxItem;
                    var selectedPropertyName = item != null ? item.Content.ToString() : parameter.ToString();

                    AddRule(selectedPropertyName, RuleType.Required);
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception in AddRequiredRuleCommand: {0} {1}", ex.Message, ex.InnerException, ex);
                }

            });

            AddOptionalRuleCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (parameter == null)
                    {
                        return;
                    }

                    Logger.Log("AddOptionalRuleCommand {0}", parameter.ToString());

                    var item = parameter as ComboBoxItem;
                    var selectedPropertyName = item != null ? item.Content.ToString() : parameter.ToString();

                    AddRule(selectedPropertyName, RuleType.Optional);
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception in AddOptionalRuleCommand: {0} {1}", ex.Message, ex.InnerException);
                }

            });

            RemoveRuleCommand = new RelayCommand(parameter =>
            {
                try
                {
                    var lRule = parameter as LRule;
                    if (lRule == null)
                        return;

                    Logger.Log("RemoveRuleCommand: {0}", lRule.Name);

                    Rules.Remove(lRule);

                    OnPropertyChanged(nameof(Rules));
                    OnPropertyChanged(lRule.RuleType + "Rules");
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception in AddRequiredRuleCommand: {0} {1}", ex.Message, ex.InnerException);
                }
            });
        }

        #endregion

        #region Methods

        public void AddRule(string propertyName, RuleType ruleType)
        {
            var propertiesWithDuplicatesAllowed = new HashSet<ItemProperty>
            {
                ItemProperty.ElementalDamage,
                ItemProperty.SkillDamage
            };

            Func<ItemProperty, bool> allowedToAdd = p => Rules.All(r => r.ItemProperty != p) || propertiesWithDuplicatesAllowed.Contains(p);

            ItemProperty property;

            Logger.Log("Attempting to Add {0} Type={1}", propertyName, ruleType);

            if (!Enum.TryParse(propertyName, out property))
                return;

            var allowed = allowedToAdd(property);

            if (property != ItemProperty.Unknown && allowed)
            {
                var id = (int) property;
                var statRange = GetItemStatRange(property);
                if (statRange != null)
                {
                    Logger.LogVerbose($"Stats Min = {statRange.AbsMin} Max = {statRange.AbsMax} Step = {statRange.AbsStep}");
                }

                Rules.Add(new LRule
                {
                    Id = id,
                    ItemStatRange = statRange,
                    ItemReference = ItemReference,
                    TrinityItemType = TrinityItemType,
                    RuleType = ruleType,
                    Value = LRule.GetDefaultValue(property)
                });

                OnPropertyChanged(nameof(Rules));
                OnPropertyChanged(ruleType + "Rules");
            }
            else
            {
                if (!allowed)
                {
                    Logger.Log("{0} has already been added and duplicates are not allowed", propertyName);
                }
            }



        }

        #endregion
    }


}
