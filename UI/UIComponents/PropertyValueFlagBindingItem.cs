using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using Trinity.UIComponents;

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

        public object Flag { get; set; }

        public BindingMember Source { get; set; }

        public Type Type { get; set; }

        private readonly RelayCommand _instancedCommand;
        private readonly bool _isAllFlag;

        public PropertyValueFlagBindingItem()
        {
            _instancedCommand = new RelayCommand(UIExecute, param => true);
                        
            OnSourceChanged += name =>
            {
                if (name == Source.Name)
                    OnPropertyChanged("Value");
            };
        }

        public void UIExecute(object o)
        {
    
        }

        private int? _allValues;
        public int AllValues
        {
            get
            {
                if (_allValues.HasValue)
                    return _allValues.Value;

                var enumValues = Enum.GetValues(Type).Cast<int>().Where(e => e == 1 || e % 2 == 0);
                _allValues = enumValues.Aggregate(0, (current, value) => current | value);

                return _allValues.Value;
            }
        }

        public void ToggleFlag()
        {
            if (Source == null)
                return;

            int targetValue = (int)Source.Value;
            targetValue ^= (int)Enum.Parse(Type, (string)Name);
            var result = (Enum)Enum.Parse(Type, targetValue.ToString());
            SetSourceValue(result);
        }

        private void SetSourceValue(Enum result)
        {
            Source.Value = result;

            if (OnSourceChanged != null)
                OnSourceChanged(Source.Name);
        }

        public ICommand FlagCheckboxSetCommand
        {
            get { return _instancedCommand; }
        }

        public bool Value
        {
            get
            {
                _value = ((Enum)Source.Value).HasFlag((Enum)Flag);
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                    
                    if ((int)Flag == AllValues)
                    {
                        if ((int) Source.Value == AllValues)
                        {
                            // Set to nothing selected
                            SetSourceValue((Enum)Enum.Parse(Type, "0"));
                        }
                        else
                        {
                            // Set to everything selected
                            SetSourceValue((Enum)Enum.Parse(Type, AllValues.ToString()));
                        }
                    }
                    else
                    {
                       ToggleFlag(); 
                    }                    
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}


