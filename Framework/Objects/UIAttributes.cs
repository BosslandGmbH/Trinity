using System;
using Trinity.Framework.Objects.Enums;
using Trinity.UI.UIComponents;

namespace Trinity.Framework.Objects
{
    /// <summary>
    ///     Attribute to set author name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IsSetting : Attribute
    {
    }

    /// <summary>
    /// If this setting should enable/disable other settings in the category
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsGroupController : Attribute
    {
    }

    /// <summary>
    /// If this setting should enable/disable other settings in the category
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GroupAttribute : Attribute
    {
        public GroupAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    /// <summary>
    /// If this setting should enable/disable other settings in the category
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AdvancedSetting : Attribute
    {
    }

    /// <summary>
    /// If this setting should enable/disable other settings in the category
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ShowDescription : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RoutineBehaviorAttribute : Attribute
    {
    }

    /// <summary>
    ///     Attribute to set author name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class Author : Attribute
    {
        public Author(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    /// <summary>
    ///     Attribute to set author name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SourceAttribute : Attribute
    {
        public SourceAttribute(string sourcePropertyName)
        {
            SourcePropertyName = sourcePropertyName;
        }

        public string SourcePropertyName { get; set; }
    }

    /// <summary>
    ///     Attribute to set author name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ComparisonOperatorAttribute : Attribute
    {
        public ComparisonOperatorAttribute(ComparisonOperator op)
        {
            Operator = op;
        }

        public ComparisonOperatorAttribute(string sourcePropertyName)
        {
            SourcePropertyName = sourcePropertyName;
        }

        public ComparisonOperator Operator { get; set; }
        public string SourcePropertyName { get; set; }
    }

    /// <summary>
    ///     Attribute to set limits to decimal property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LimitAttribute : Attribute
    {
        public LimitAttribute(double low, double high)
        {
            Low = low;
            High = high;
        }

        public double High { get; set; }
        public double Low { get; set; }
    }

    [Flags]
    public enum UIControlOptions
    {
        None = 0,
        NoLabel = 1 << 0,
        Unused = 1 << 1,
        Inline = 1 << 2,
    }

    /// <summary>
    ///     Attribute to set limits to decimal property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UIControlAttribute : Attribute
    {
        //public UIControlAttribute(UIControlType type,
        //    string sourceEnumerable = "",
        //    string displayProperty = "",
        //    string storageProperty = "",
        //    UIControlOptions options = UIControlOptions.None)
        //{
        //    SourceEnumerable = sourceEnumerable;
        //    DisplayProperty = displayProperty;
        //    StorageProperty = storageProperty;
        //    Type = type;
        //    Options = options;
        //}

        public UIControlAttribute(UIControlType type, UIControlOptions options = UIControlOptions.None)
        {
            Type = type;
            Options = options;
        }

        public UIControlType Type { get; set; }

        public string SourceEnumerable { get; set; }

        public string DisplayProperty { get; set; }

        public string StorageProperty { get; set; }

        public UIControlOptions Options { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FlagExclusionAttribute : Attribute
    {
        public FlagExclusionAttribute(object flags)
        {
            var type = flags.GetType();
            if (type.IsEnum)
            {
                Flags = (Enum)flags;
                Mask = Convert.ToInt64(flags);
            }
            else if (type == typeof(int))
            {
                Mask = Convert.ToInt64(flags);
            }
            else if (type == typeof(long))
            {
                Mask = (long)flags;
            }
        }

        public FlagExclusionAttribute(int flags)
        {
            Mask = Convert.ToInt64(flags);
        }

        public FlagExclusionAttribute(long flags)
        {
            Mask = flags;
        }

        public Enum Flags { get; set; }
        public long Mask { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class BindingAttribute : Attribute
    {
        public string Source { get; set; }

        public string DisplayProperty { get; set; }

        public string StorageProperty { get; set; }

        public int Order { get; set; }
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class SymbolAttribute : Attribute
    {
        public string Value { get; set; }

        public SymbolAttribute(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    ///     Attribute to set limits to decimal property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LimitBindingAttribute : Attribute
    {
        public LimitBindingAttribute(double low, string highSource)
        {
            Low = low;
            HighSource = highSource;
        }

        public LimitBindingAttribute(string lowSource, string highSource)
        {
            HighSource = highSource;
            LowSource = lowSource;
        }

        public LimitBindingAttribute(string lowSource, double high)
        {
            High = high;
            LowSource = lowSource;
        }

        public string HighSource { get; set; }
        public string LowSource { get; set; }

        public double High { get; set; }
        public double Low { get; set; }
    }

    /// <summary>
    ///     Attribute to set limits to decimal property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TickFrequencyAttribute : Attribute
    {
        public TickFrequencyAttribute(double frequency)
        {
            Frequency = frequency;
        }

        public double Frequency { get; set; }
    }

    /// <summary>
    ///     Attribute to set at debug infos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DebugInfoAttribute : Attribute
    {
    }

    /// <summary>
    ///     Attribute to set at debug infos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RepositoryAttribute : Attribute
    {
        public RepositoryAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    /// <summary>
    ///     Attribute to set at list view an properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ListViewAttribute : Attribute
    {
        public ListViewAttribute(string listName)
        {
            ListName = listName;
        }

        public string ListName { get; set; }
    }

    /// <summary>
    ///     Attribute to set at list view an properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ListViewItemAttribute : Attribute
    {
        public ListViewItemAttribute(string listName, int index, int colWidth)
        {
            ListName = listName;
            Index = index;
            MinColumnWidth = colWidth;
        }

        public string ListName { get; set; }
        public int Index { get; set; }
        public int MinColumnWidth { get; set; }
    }

    /// <summary>
    ///     Attribute to set at list view an properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ListViewItemDescriptionAttribute : Attribute
    {
        public ListViewItemDescriptionAttribute(string listName)
        {
            ListName = listName;
        }

        public string ListName { get; set; }
    }
}