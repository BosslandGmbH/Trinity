using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Attributes;
using Trinity.Framework.Objects.Memory.Sno.Types;

namespace Trinity.UI.UIComponents
{
    /// <summary>
    /// Allows you to bind a flag value to a checkbox within a DataTemplate and then
    /// have it automatically add/remove itself from a 'Source' BindingMember.
    /// </summary>
    public class PropertyValueFlagBindingItem : INotifyPropertyChanged
    {
        /*
        <ItemsControl Grid.Column="1" Tag="{Binding Value}" ItemsSource="{Binding Converter={StaticResource PropertyValueToCollectionConverter}}">
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <StackPanel>
                        <CheckBox x:Name="CheckBoxFieldCheckbox" Command="{Binding Path=FlagCheckboxSetCommand}" CommandParameter="{Binding Tag, RelativeSource={RelativeSource Mode=FindAncestor, AncestorType={x:Type ItemsControl}}}" IsEnabled="{Binding IsEnabled}" Margin="0,0,6,0" IsChecked="{Binding Value}" HorizontalAlignment="Left" VerticalAlignment="Center" />
                        <CheckBox Command="{Binding Path=FlagCheckboxSetCommand}" CommandParameter="{Binding Tag, RelativeSource={RelativeSource Mode=FindAncestor, AncestorType={x:Type ItemsControl}}}" IsEnabled="{Binding IsEnabled}" ToolTip="{Binding Description}" Style="{DynamicResource TextBlockCheckBox}" FontWeight="Regular" IsChecked="{Binding IsChecked, ElementName=CheckBoxFieldCheckbox}" Content="{Binding Name}" Margin="0,0,0,0" />
                    </StackPanel>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl> 
        */

        public static event SourceChangedEvent OnSourceChanged;

        public delegate void SourceChangedEvent(string sourceName);

        private bool _value;

        public object Name { get; set; }

        public string Description { get; set; }

        public object Flag { get; set; }

        public BindingMember Source
        {
            get { return _source; }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    if (_source != null)
                    {
                        _source.PropertyChanged += SourcePropertyChanged;
                    }                    
                    OnPropertyChanged(nameof(Source));
                }                
            }
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(Source.Value))
            {
                OnPropertyChanged(nameof(Value));
            }
        }

        public Type Type { get; set; }

        private readonly RelayCommand _instancedCommand;
        private readonly bool _isAllFlag;

        public PropertyValueFlagBindingItem()
        {
            _instancedCommand = new RelayCommand(UIExecute, param => true);

            OnSourceChanged += name =>
            {
                if (name == Source.Name)
                    OnPropertyChanged(nameof(Value));
            };
        }

        public void UIExecute(object o)
        {

        }

        private static bool IsSignedTypeCode(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return false;
                default:
                    return true;
            }
        }

        private long _allValuesSum;
        private Enum _allValues;
        private BindingMember _source;

        public Enum AllValues
        {
            get
            {
                if (_allValues != null) return _allValues;
                _allValuesSum = GetAllEnumValues()
                    .Where(value => value == 1 || value % 2 == 0)
                    .Aggregate<long, long>(0, (current, value) => current | value);

                _allValuesSum &= ~Source.ExcludeMask;

                _allValues = (Enum)Enum.Parse(Type, _allValuesSum.ToString());
                return _allValues;
            }
        }

        //private long? _excludeMask;
        //public long ExcludeMask
        //{
        //    get
        //    {
        //        if (_excludeMask.HasValue)
        //            return _excludeMask.Value;

        //        var excludeFlagsAttr = (FlagExclusionAttribute)Source.PropertyInfo.GetCustomAttribute(typeof(FlagExclusionAttribute));
        //        _excludeMask = excludeFlagsAttr?.Mask ?? 0;
        //        return _excludeMask.Value;
        //    }
        //    set { _excludeMask = value; }
        //}

        private IEnumerable<long> GetAllEnumValues()
        {
            return Enum.GetValues(Type).Cast<Enum>().Select(Convert.ToInt64);
        }

        public void ToggleFlag()
        {
            if (Source == null)
                return;

            var targetValue = Convert.ToInt64(Source.Value);
            targetValue ^= Convert.ToInt64(Enum.Parse(Type, (string)Name));
            SetSourceValue(targetValue);
        }

        private void SetSourceValue(long value)
        {
            value &= ~Source.ExcludeMask;
            Source.Value = (Enum)Enum.Parse(Type, value.ToString());
            OnSourceChanged?.Invoke(Source.Name);
        }

        public ICommand FlagCheckboxSetCommand => _instancedCommand;

        public bool Value
        {
            get
            {
                _value = ((Enum)Source.Value).HasFlag((Enum)Flag);
                return _value;
            }
            set
            {
                try
                {
                    if (_value != value)
                    {
                        _value = value;
                        OnPropertyChanged();

                        // Check if this flag is an 'All/None' toggle in enum e.g
                        // All = ~(1 << 24),  

                        if (_allValuesSum + Convert.ToInt64(Flag) < 0)
                        {
                            if (Equals(Source.Value, AllValues))
                            {
                                // Set to nothing selected
                                SetSourceValue(0);
                            }
                            else
                            {
                                // Set to everything selected
                                SetSourceValue(_allValuesSum);
                            }
                        }
                        else
                        {
                            ToggleFlag();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Exception in PropertyValueFlagBindingItem: {ex}");
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}


