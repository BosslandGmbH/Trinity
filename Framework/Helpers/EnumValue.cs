using System;
using Trinity.Framework.Objects;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// Conversion of enum value into an object for the purpose of easier UI binding
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct EnumValue<T> : IUnique
    {
        private Type _type;
        private T _value;
        private string _valueString;
        private string _name;
        private int _id;

        public EnumValue(T e)
        {
            _type = typeof(T);
            _value = e;
            _valueString = e.ToString();
            _name = _valueString.AddSpacesToSentence();
            _id = Convert.ToInt32(Enum.Parse(typeof(T), _name) as Enum);
        }

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string ValueString
        {
            get { return _valueString; }
            set { _valueString = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public static implicit operator T(EnumValue<T> x)
        {
            return x.Value;
        }

        public static implicit operator EnumValue<T>(T x)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            return new EnumValue<T>(x);
        }
    }
}