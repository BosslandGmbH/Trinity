using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Trinity.Framework.Objects;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.UI.UIComponents
{
    public class BindingMember : INotifyPropertyChanged
    {
        private string _displayName;
        private readonly PropertyInfo _propertyInfo;
        private readonly object _baseObject;
        private readonly Type _type;
        private readonly DefaultValueAttribute _defaultValueAttribute;
        private readonly LimitAttribute _limitAttribute;
        private readonly DisplayNameAttribute _displayNameAttribute;
        private readonly DescriptionAttribute _descriptionAttribute;
        private readonly CategoryAttribute _categoryAttribute;
        private readonly UIControlAttribute _uiControlAttribute;
        private readonly AdvancedSetting _advancedAttribute;
        private readonly IsGroupController _isGroupControllerAttribute;
        private readonly GroupAttribute _groupAttribute;

        public static event CategoryChangedEvent OnCategoryChanged;
        public delegate void CategoryChangedEvent(string categoryName);
        //public event PropertyChangedEventHandler PropertyChanged;
        private List<object> _source = new List<object>();
        public static Dictionary<string, bool> GroupStatus = new Dictionary<string, bool>();
        private Range _range;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SupressChangeNotifications { get; set; }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            if (!SupressChangeNotifications)
                OnPropertyChanged(propertyName);
            return true;
        }

        public string PropertyName { get; set; }

        public BindingMember(MemberInfo propertyInfo, object baseObject, BindingMember parent = null, bool lean = false)
        {
            BoundSources = new List<BoundSource>();

            _propertyInfo = propertyInfo as PropertyInfo;
            _baseObject = baseObject;
            
            PropertyName = propertyInfo.Name;

            if (lean)
                return;

            _defaultValueAttribute = GetAttribute<DefaultValueAttribute>(propertyInfo);
            _limitAttribute = GetAttribute<LimitAttribute>(propertyInfo);
            _displayNameAttribute = GetAttribute<DisplayNameAttribute>(propertyInfo);
            _descriptionAttribute = GetAttribute<DescriptionAttribute>(propertyInfo);                     
            _uiControlAttribute = GetAttribute<UIControlAttribute>(propertyInfo);
            _categoryAttribute = GetAttribute<CategoryAttribute>(propertyInfo);
            _advancedAttribute = GetAttribute<AdvancedSetting>(propertyInfo);            
            _limitBindingAttribute = GetAttribute<LimitBindingAttribute>(propertyInfo);
            _isGroupControllerAttribute = GetAttribute<IsGroupController>(propertyInfo);
            _groupAttribute = GetAttribute<GroupAttribute>(propertyInfo);

            //Core.Logger.Verbose($"Created Binding Member for {propertyInfo.Name}");

            var notify = baseObject as INotifyPropertyChanged;
            if (notify != null)
            {
                //Core.Logger.Verbose($"Registering Property Changed for {baseObject.GetType().Name} ({propertyInfo.Name})");
                notify.PropertyChanged += NotifyOnPropertyChanged;
            }

            //_bindingAttribute = GetAttribute<BindingAttribute>(propertyInfo);

            _BindingAttributes = propertyInfo.GetCustomAttributes(typeof (BindingAttribute)).OfType<BindingAttribute>().ToList();

            if (_categoryAttribute == null && parent != null && parent.Category != null)
                _categoryAttribute = new CategoryAttribute(parent.Category);

            // The parent property of embedded TrinitySettings objects can define a category
            // When children do not have one explicitly set.
            if (_categoryAttribute == null && parent != null && parent.Category != null)
                _categoryAttribute = new CategoryAttribute(parent.Category);

            // GroupControllers notify other instances that a category has changed.
            // Applicable listners can then refresh their state in the UI.
            OnCategoryChanged += categoryId =>
            {
                if (categoryId == GroupId)
                    OnPropertyChanged(nameof(IsEnabled));
            };

            IsNoLabel = _uiControlAttribute != null && _uiControlAttribute.Options.HasFlag(UIControlOptions.NoLabel);
            
            //IsUseDescription = _uiControlAttribute != null && _uiControlAttribute.Options.HasFlag(UIControlOptions.UseDescription);

            IsInline = _uiControlAttribute != null && _uiControlAttribute.Options.HasFlag(UIControlOptions.Inline);

            if (_limitAttribute != null)
            {
                Range = new Range
                {
                    Min = _limitAttribute.Low, 
                    Max = _limitAttribute.High
                };
            }

            _type = _baseObject.GetType();

            if (_limitBindingAttribute != null)
            {
                var lowSource = _limitBindingAttribute.LowSource;
                var highSource = _limitBindingAttribute.HighSource;

                if (!BotMain.IsRunning)
                {
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        Range = new Range
                        {
                            Min = lowSource != null ? GetMemberValue<double>(lowSource) : _limitBindingAttribute.Low,
                            Max = highSource != null ? GetMemberValue<double>(highSource) : _limitBindingAttribute.High
                        };
                    }
                }
                else
                {
                    Range = new Range
                    {
                        Min = lowSource != null ? GetMemberValue<double>(lowSource) : _limitBindingAttribute.Low,
                        Max = highSource != null ? GetMemberValue<double>(highSource) : _limitBindingAttribute.High
                    };                   
                }
            }

            if (_BindingAttributes != null && _BindingAttributes.Any())
            {
                var attributes = _BindingAttributes.OrderBy(a => a.Order).ToList();            
                foreach (var bindingAttr in attributes)
                {
                    var member = GetMember(bindingAttr);
                    var prop = member as PropertyInfo;
                    if (prop == null) continue;
                    var boundSource = new BoundSource
                    {
                        Member = new BindingMember(prop, baseObject, this),
                        Items = GetBoundItems(prop, bindingAttr)
                    };
                    BoundSources.Add(boundSource);
                }                
            }

            if (IsGroupController)
            {
                ChangeGroup(GroupId, Value);
            }

            Source = new BoundSource
            {
                Member = this,
                Items = GetBoundItems(propertyInfo)
            };

        }

        private void NotifyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (PropertyName == propertyChangedEventArgs.PropertyName)
            {
                OnPropertyChanged(nameof(Value));                
            }
        }

        private long? _excludeMask;
        public long ExcludeMask
        {
            get
            {
                if (_excludeMask.HasValue)
                    return _excludeMask.Value;

                var excludeFlagsAttr = (FlagExclusionAttribute)PropertyInfo.GetCustomAttribute(typeof(FlagExclusionAttribute));
                _excludeMask = excludeFlagsAttr?.Mask ?? 0;
                return _excludeMask.Value;
            }
            set { _excludeMask = value; }
        }

        public bool IsIndented => IsGroupChild && !IsInline;

        public bool IsInline { get; set; }

        public BoundSource Source
        {
            get { return _source1; }
            set { SetField(ref _source1, value); }
        }

        private MemberInfo GetMember(BindingAttribute bindingAttr)
        {
            var source = bindingAttr.Source;
            if (string.IsNullOrEmpty(source))
                return null;

            var field = _type.GetField(source);
            if (field != null)
                return field;
   
            return _type.GetProperty(source);
        }

        private List<PropertyValueBindingItem> GetBoundItems(MemberInfo member, BindingAttribute bindingAttr = null)
        {
            string displayProperty = string.Empty;
            string storageProperty = string.Empty;

            if (bindingAttr != null)
            {
                displayProperty = bindingAttr.DisplayProperty;                
                storageProperty = bindingAttr.StorageProperty;
            }

            var items = new List<PropertyValueBindingItem>();
            var field = member as FieldInfo;
            var prop = member as PropertyInfo;

            object newVal;
            Type type;

            if (field == null && prop == null)
                return items;

            bool isIEnumerable;
            var isEnum = false;

            if (field != null)
            {
                isIEnumerable = ((IList)field.FieldType.GetInterfaces()).Contains(typeof(IEnumerable));
                if(isIEnumerable)
                    isEnum = field.FieldType.IsEnum;
                newVal = field.GetValue(_baseObject);
                type = field.FieldType;
            }
            else
            {
                isIEnumerable = ((IList)prop.PropertyType.GetInterfaces()).Contains(typeof(IEnumerable));
                if (!isIEnumerable)
                    isEnum = prop.PropertyType.IsEnum;
                newVal = prop.GetValue(_baseObject);
                type = prop.PropertyType;
            }

            if (isEnum)
            {
                var values = Enum.GetValues(type);
                var names = Enum.GetNames(type);
                for (var i = 0; i < values.Length; i++)
                {
                    var val = values.GetValue(i);
                    var name = names[i];
                    items.Add(new PropertyValueBindingItem(name, val, GetDescription(type, val)));
                }
            }

            else if (isIEnumerable && newVal != null)
            {
                foreach (var item in (IEnumerable)newVal)
                {
                    var t = item.GetType();

                    var displayProp = t.GetProperty(displayProperty);
                    if (displayProp == null)
                        continue;

                    var displayValue = displayProp.GetValue(item, null);
                    if (displayValue == null)
                        continue;

                    var storageProp = t.GetProperty(storageProperty);
                    if (storageProp == null)
                        continue;

                    var storageValue = storageProp.GetValue(item, null);
                    if (storageValue == null)
                        continue;

                    items.Add(new PropertyValueBindingItem(displayValue, storageValue));
                }
            }

            return items;
        }

        private static string GetDescription(Type type, object enumValue)
        {
            var memInfo = type.GetMember(enumValue.ToString());
            if (!memInfo.Any())
                return null;

            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (!attributes.Any())
                return null;

            return ((DescriptionAttribute)attributes[0]).Description;
        }

        public bool IsNoLabel { get; set; }

        public bool IsUseDescription { get; set; }

        public T GetMemberValue<T>(string name)
        {
            var field = _type.GetField(name);
            if (field != null)
                return (T)Convert.ChangeType(field.GetValue(_baseObject), typeof(T));

            var prop = _type.GetProperty(name);
            if (prop != null)
                return (T)Convert.ChangeType(prop.GetValue(_baseObject), typeof(T)); 

            return default(T);
        }

        private Range Range;

        private LimitBindingAttribute _limitBindingAttribute;

        private List<BindingAttribute> _BindingAttributes;

        public float Min
        {
            get { return Range.AbsMin; }
        }

        public float Max
        {
            get { return Range.AbsMax; }
        }

        public float Step
        {
            get { return Range.AbsStep; }
        }

        private float CoerceValue(float value)
        {
            if (value < Min)
                value = Min;
            else if (value > Max)
                value = Max;
            return value;
        }

        public class BoundSource : NotifyBase
        {
            private List<PropertyValueBindingItem> _items;
            private BindingMember _member;

            public BindingMember Member
            {
                get { return _member; }
                set { SetField(ref _member, value); }
            }

            public List<PropertyValueBindingItem> Items
            {
                get { return _items; }
                set { SetField(ref _items, value); }
            }
        }

        public bool IsSingleSourced
        {
            get { return BoundSources.Count == 1; }
        }

        public bool IsDoubleSourced
        {
            get { return BoundSources.Count == 2; }
        }

        public bool IsTripleSourced
        {
            get { return BoundSources.Count == 3; }
        }

        public List<BoundSource> BoundSources { get; set; }

        public BoundSource BoundSource1 { get { return BoundSources.Any() ? BoundSources.ElementAt(0) : null; } }

        public BoundSource BoundSource2 { get { return BoundSources.Any() ? BoundSources.ElementAt(1) : null; } }

        public BoundSource BoundSource3 { get { return BoundSources.Any() ? BoundSources.ElementAt(2) : null; } }

        public static bool IsGroupEnabled(string groupName)
        {
            bool status;
            return !GroupStatus.TryGetValue(groupName, out status) || status;
        }

        public bool ControllerExists
        {
            get { return Group != null && GroupStatus.ContainsKey(GroupId); }
        }

        public bool IsEnabled
        {
            get { return (IsGroupController || IsGroupEnabled(GroupId)); }
        }

        public bool IsGroupChild
        {
            get { return !IsGroupController && ControllerExists; }
        }

        public string Group
        {
            get { return _groupAttribute != null ? _groupAttribute.Name : null; }
        }

        public string GroupId
        {
            get { return _type.FullName + Group; }
        }

        public bool IsGroupController
        {
            get { return _isGroupControllerAttribute != null;  }
        }

        private string GetDisplayName()
        {
            string displayname;
            if (_displayNameAttribute == null)
            {
                StringBuilder builder = new StringBuilder();
                var theString = _propertyInfo.Name;
                foreach (char c in theString)
                {
                    if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                    builder.Append(c);
                }
                displayname = builder.ToString();
            }
            else
            {
                displayname = _displayNameAttribute.DisplayName;
            }
            return displayname;
        }

        public Type Type
        {
            get { return _propertyInfo.PropertyType; }
        }

        public string Name
        {
            get { return _propertyInfo.Name; }
        }

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }

        public Type PropertyType
        {
            get { return _propertyInfo.PropertyType; }
        }

        public object DefaultValue
        {
            get { return _defaultValueAttribute != null ? _defaultValueAttribute.Value : null; }
        }

        public bool IsAdvanced
        {
            get { return _advancedAttribute != null; }
        }

        public Range Limit
        {
            get { return _limitAttribute != null ? new Range { Min = _limitAttribute.Low, Max = _limitAttribute.High } : new Range(); }
        }

        public string DisplayName
        {
            get { return _displayName ?? (_displayName = GetDisplayName()); }
        }

        public string Category
        {
            get { return _categoryAttribute != null ? _categoryAttribute.Category : null; }
        }

        public string Description
        {
            get { return _descriptionAttribute != null ? _descriptionAttribute.Description : null; }
        }

        private static T GetAttribute<T>(MemberInfo type) where T : class
        {
            return type.GetCustomAttribute(typeof(T), false) as T;
        }

        public UIControlType UIControl
        {
            get
            {
                if (_uiControlAttribute != null) 
                    return _uiControlAttribute.Type;
              
                return GetUIControlByType(PropertyType);
            }
        }

        private UIControlType GetUIControlByType(Type type)
        {
            if(type.IsEnum)
                return UIControlType.ComboBox;

            if (type == typeof(bool))
                return UIControlType.Checkbox;

            if (IsNumericType(type))
                return UIControlType.TextBox;

            return UIControlType.Label;
        }

        public static bool IsNumericType(Type type)
        {
            if (type == null)
            {
                return false;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        public static void ChangeGroup(string groupId, object value)
        {
            if (!(value is bool))
                return;

            Core.Logger.Log(LogCategory.UI, "Setting Group {0} to {1}", groupId, value);  

            AddOrUpdateGroup(groupId, value);

            if (OnCategoryChanged != null)
                OnCategoryChanged(groupId);
        }

        private static void AddOrUpdateGroup(string groupId, object value)
        {
            if (GroupStatus.ContainsKey(groupId))
            {
                GroupStatus[groupId] = (bool) value;
                return;
            }

            GroupStatus.Add(groupId, (bool) value);            
        }

        public object Value
        {
            get
            {
                try
                {
                    object value;
                    //if (!BotMain.IsRunning || BotMain.IsPausedForStateExecution)
                    //{
                        //using (new AquireFrameHelper())
                        //{
                            //value = _propertyInfo.GetValue(_baseObject, null);
                        //}                            
                    //}
                    //else
                    //{
                        value = _propertyInfo.GetValue(_baseObject, null);
                    //}

                    // Slider controls need a value within range or they dont work properly.
                    //if (Range != null && UIControl == UIControlType.Slider && IsNumericType(PropertyType))
                    if (Range != null && IsNumericType(PropertyType))
                    {
                        value = Math.Round(CoerceValue((float)Convert.ChangeType(value, typeof(float))),2,MidpointRounding.AwayFromZero);
                    }
                    
                    // Some of the enums we're dealing with have a -1 default value and no value for 0 (great plan huh).
                    if (Type.IsEnum && !Type.IsDefined(typeof(FlagsAttribute), false) && !Type.IsEnumDefined(value))
                    {
                        return GetFirstEnumValue(Type);
                    }

                    //// Set default from DefaultValueAttribute
                    //if (Equals(value, GetDefault(PropertyType)))
                    //{
                    //    SetValue(DefaultValue);
                    //    return DefaultValue;
                    //}

                    //if (PropertyType.IsEnum && value is string)
                    //{
                    //    value = GetValueFromDescriptionAttribute((string)value, PropertyType);
                    //}                    
                    return value;
                }
                catch (Exception ex)
                {
                    Core.Logger.Error("Exception in PropertyValue_Get {0} {1} {2}", Name, Type, ex);
                    throw;
                }
            }
            set
            {
                SetValue(value);
            }
        }

        private object _actualDefaultValue;
        public object ActualDefaultValue
        {
            get { return _actualDefaultValue ?? (_actualDefaultValue = Type.IsValueType ? Activator.CreateInstance(Type) : null); }           
        }

        public static object GetFirstEnumValue(Type type)
        {
            var results = new ArrayList();
            var underType = Enum.GetUnderlyingType(type);
            foreach (var value in Enum.GetValues(type))
            {
                results.Add(Convert.ChangeType(value, underType));
            }
            results.Sort(EnumComparer);
            return results.Count > 0 ? results[0] : null;
        }
        
        private static readonly IComparer EnumComparer =  new UnknownEnumValueComparer();
        public class UnknownEnumValueComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                int xNum;
                int yNum;
                if (int.TryParse(x.ToString(), out xNum) && int.TryParse(y.ToString(), out yNum))
                {
                    return Comparer<int>.Default.Compare(xNum, yNum);
                };
                return 0;
            }
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private void SetValue(object value)
        {
            try
            {
                Core.Logger.Log("Setting {0} to {1}", Name, value);

                if (IsGroupController)
                {
                    ChangeGroup(GroupId, (bool) value);
                }

                //if (Type.IsEnum && !Type.IsDefined(typeof(FlagsAttribute), false) && Type.IsEnumDefined(value))
                //{
                //    _propertyInfo.SetValue(_baseObject, ActualDefaultValue, null);
                //    return;
                //}

                //if (PropertyType.IsEnum && value is string)
                //{
                //    Core.Logger.Log("Getting value from string: {0} Type={1}", value, value.GetType());                         
                //    val = GetValueFromDescriptionAttribute((string)value, PropertyType);
                //}
                //else
                //{
                var val = Convert.ChangeType(value, PropertyType);
                //}

                // Need to convert type or it won't save numbers properly
                _propertyInfo.SetValue(_baseObject, val, null);
                OnPropertyChanged(nameof(Value));
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Exception in PropertyValue_Set {0} {1} {2}", Name, Type, ex);
                throw;
            }
        }

        //public Dictionary<string, object> _descriptionToValueMapping = new Dictionary<string, object>();
        private BoundSource _source1;

        //public object GetValueFromDescriptionAttribute(string description, Type type)
        //{
        //    object actualValue;

        //    if (_descriptionToValueMapping.TryGetValue(description, out actualValue))
        //    {
        //        return actualValue;
        //    }

        //    if (type.IsEnum)
        //    {
        //        var enumValues = Enum.GetValues(type);
        //        foreach (var enumValue in enumValues)
        //        {
        //            var attr = GetDescriptionAttribute(type, enumValue);
        //            if (attr != null)
        //            {                        
        //                _descriptionToValueMapping.Add(attr, enumValue);

        //                if (attr == description)
        //                    actualValue = enumValue;
        //            }
        //        }
        //    }

        //    return actualValue;
        //}

        //private static string GetDescriptionAttribute(Type type, object enumValue)
        //{
        //    var memInfo = type.GetMember(enumValue.ToString());
        //    if (!memInfo.Any())
        //        return null;

        //    var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        //    if (!attributes.Any())
        //        return null;

        //    return ((DescriptionAttribute)attributes[0]).Description;
        //}


        //public List<BindingMember> Children { get; set; }

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    var handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}

        internal void Refresh()
        {
            OnPropertyChanged("");
        }
    }
}
