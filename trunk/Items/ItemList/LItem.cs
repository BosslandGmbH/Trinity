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
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Technicals;
using Trinity.UIComponents;

namespace Trinity.UI.UIComponents
{
    /// <summary>
    /// Item Object wrapped for use in SettingsUI
    /// </summary>
    [DataContract(Namespace = "")]
    public class LItem : Item, INotifyPropertyChanged, ICloneable
    {
        private bool _isSelected;
        private readonly Item _item;
        private List<ItemProperty> _itemProperties;
        private ObservableCollection<LRule> _rules = new ObservableCollection<LRule>();
        private int _ops;

        public LItem()
        {


        }

        public LItem(Item item)
        {
            LoadCommands();

            Type = ILType.Item;

            _item = item;

            Name = item.Name;
            Id = item.Id;
            BaseType = item.BaseType;
            DataUrl = item.DataUrl;
            InternalName = item.InternalName;
            IsCrafted = item.IsCrafted;
            ItemType = item.ItemType;
            LegendaryAffix = item.LegendaryAffix;
            Quality = item.Quality;
            RelativeUrl = item.RelativeUrl;
            IsCrafted = item.IsCrafted;
            Slug = item.Slug;
            Url = item.Url;
            IconUrl = item.IconUrl;
            IsTwoHanded = item.IsTwoHanded;
            TrinityItemType = item.TrinityItemType;
            IsSetItem = item.IsSetItem;
            SetName = item.IsSetItem ? item.Set.Name : "None";
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
        /// Properties that are valid for this particular item
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

                return _itemProperties = ItemDataUtils.GetPropertiesForItem(_item);
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
        [DataMember]
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
            return new LItem(_item)
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

            return ItemDataUtils.GetItemStatRange(_item, property);
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
                var statRange = GetItemStatRange(property);
                if (statRange != null)
                {
                    Logger.LogVerbose($"Stats Min = {statRange.AbsMin.ToString()} Max = {statRange.AbsMax.ToString()} Step = {statRange.AbsStep.ToString()}");
                }

                Rules.Add(new LRule
                {
                    Id = (int)property,
                    ItemStatRange = statRange,
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
