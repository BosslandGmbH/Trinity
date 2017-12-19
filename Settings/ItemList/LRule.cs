using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;

namespace Trinity.Settings.ItemList
{
    /// <summary>
    /// Rule for an item
    /// </summary>
    [DataContract(Namespace = "")]
    public class LRule : NotifyBase
    {
        public LRule()
        {

        }

        public LRule(ItemProperty prop, Item itemReference = null)
        {
            Id = (int)prop;
            Value = GetDefaultValue(prop);
            ItemReference = ItemReference;
        }

        private double _value;
        private int _variant;
        private List<object> _variants = new List<object>();
        private RuleType _type;
        private RuleType _ruleType;
        private string _attributeValue;
        private string _attributeModifier;
        private string _attributeKey;

        public string Name => ItemProperty.ToString();

        [DataMember]
        public int Id { get; set; }

        public ItemProperty ItemProperty => (ItemProperty)Id;

        [DataMember]
        public double Value
        {
            get
            {
                if (ItemStatRange != null)
                    _value = CoerceValue(_value);

                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public static double GetDefaultValue(ItemProperty prop)
        {
            switch (prop)
            {
                case ItemProperty.远古:
                    return 1;
            }
            return 0;
        }

        private double CoerceValue(double value)
        {
            if (value < Min)
                value = Min;
            else if (value > Max)
                value = Max;
            return value;
        }

        [DataMember(EmitDefaultValue = false)]
        public int Variant
        {
            get { return _variant; }
            set
            {
                if (_variant != value)
                {
                    _variant = value;
                    UpdateStatRange();
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateStatRange()
        {
            // This is messed up,

            if (_defaultStatRange == null && (TrinityItemType != TrinityItemType.Unknown || ItemReference != null))
            {
                _defaultStatRange = ItemReference == null
                    ? ItemDataUtils.GetItemStatRange(TrinityItemType, ItemProperty)
                    : ItemDataUtils.GetItemStatRange(ItemReference, ItemProperty);
            }

            if (ItemReference != null && ItemProperty == ItemProperty.技能伤害)
            {
                var customRange = ItemDataUtils.GetStatRangeForItemAndSkill(ItemReference, Variant);
                if (customRange != null)
                {
                    ItemStatRange = customRange;
                }
            }
            else
            {
                ItemStatRange = _defaultStatRange;
            }

            if (ItemStatRange == null && _defaultStatRange != null)
                ItemStatRange = _defaultStatRange;

            if (ItemStatRange == null)
            {
                OnPropertyChanged(nameof(Min));
                OnPropertyChanged(nameof(Max));
                OnPropertyChanged(nameof(Step));
            }
        }

        [DataMember(EmitDefaultValue = false, Name = "AttKey")]
        public string AttributeKey
        {
            get { return _attributeKey; }
            set { SetField(ref _attributeKey, value); }
        }

        [DataMember(EmitDefaultValue = false, Name = "AttMod")]
        public string AttributeModifier
        {
            get { return _attributeModifier; }
            set { SetField(ref _attributeModifier, value); }
        }

        [DataMember(EmitDefaultValue = false, Name = "AttVal")]
        public string AttributeValue
        {
            get { return _attributeValue; }
            set { SetField(ref _attributeValue, value); }
        }

        public List<object> Variants
        {
            get
            {
                if (_variants == null || !_variants.Any())
                {
                    if (ItemReference != null)
                    {
                        _variants = ItemDataUtils.GetItemPropertyVariants(ItemProperty, ItemReference);
                    }
                    else
                    {
                        _variants = ItemDataUtils.GetItemPropertyVariants(ItemProperty, TrinityItemType);
                    }

                    if (_variant == 0)
                    {
                        var firstVariant = (_variants.FirstOrDefault() as IUnique);
                        if (firstVariant != null)
                            Variant = firstVariant.Id;
                    }
                }
                return _variants;
            }
            set
            {
                if (_variants != value)
                {
                    _variants = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember]
        public int TypeId { get; set; }
        public RuleType RuleType
        {
            get { return (RuleType)TypeId; }
            set { TypeId = (int)value; }
        }

        public double Min => ItemStatRange.AbsMin;

        public double Max => ItemStatRange.AbsMax;

        public double Step => ItemStatRange.AbsStep;

        private ItemStatRange _defaultStatRange;
        public ItemStatRange ItemStatRange { get; set; }

        public TrinityItemType TrinityItemType { get; set; }

        public Item ItemReference { get; set; }

        public override int GetHashCode()
        {
            return TypeId.GetHashCode() * 17 ^ Id * 13 ^ Variant * 31 ^ (int)Value;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }
    }
}
