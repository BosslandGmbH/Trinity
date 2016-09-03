using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trinity.Framework.Helpers
{
    public class MemoryAnalysis
    {     
        [Flags]
        public enum TestFlags : uint
        {
            None = 0x00000000,
            X00000001 = 0x00000001,
            X00000002 = 0x00000002,
            X00000004 = 0x00000004,
            X00000008 = 0x00000008,
            X00000010 = 0x00000010,
            X00000020 = 0x00000020,
            X00000040 = 0x00000040,
            X00000080 = 0x00000080,
            X00000100 = 0x00000100,
            X00000200 = 0x00000200,
            X00000400 = 0x00000400,
            X00000800 = 0x00000800,
            X00001000 = 0x00001000,
            X00002000 = 0x00002000,
            X00004000 = 0x00004000,
            X00008000 = 0x00008000,
            X00010000 = 0x00010000,
            X00020000 = 0x00020000,
            X00040000 = 0x00040000,
            X00080000 = 0x00080000,
            X00100000 = 0x00100000,
            X00200000 = 0x00200000,
            X00400000 = 0x00400000,
            X00800000 = 0x00800000,
            X01000000 = 0x01000000,
            X02000000 = 0x02000000,
            X04000000 = 0x04000000,
            X08000000 = 0x08000000,
            X10000000 = 0x10000000,
            X20000000 = 0x20000000,
            X40000000 = 0x40000000,
            X80000000 = 0x80000000,
            //X100000000 = 0x100000000,
            //X200000000 = 0x200000000,
            //X400000000 = 0x400000000,
            //X800000000 = 0x800000000,
            //X1000000000 = 0x1000000000,
            //X2000000000 = 0x2000000000,
            //X4000000000 = 0x4000000000,
            //X8000000000 = 0x8000000000,
            //X10000000000 = 0x10000000000,
            //X20000000000 = 0x20000000000,
            //X40000000000 = 0x40000000000,
            //X80000000000 = 0x80000000000,
            //X100000000000 = 0x100000000000,
            //X200000000000 = 0x200000000000,
            //X400000000000 = 0x400000000000,
            //X800000000000 = 0x800000000000,
            //X1000000000000 = 0x1000000000000,
            //X2000000000000 = 0x2000000000000,
            //X4000000000000 = 0x4000000000000,
            //X8000000000000 = 0x8000000000000,
            //X10000000000000 = 0x10000000000000,
            //X20000000000000 = 0x20000000000000,
            //X40000000000000 = 0x40000000000000,
            //X80000000000000 = 0x80000000000000,
            //X100000000000000 = 0x100000000000000,
            //X200000000000000 = 0x200000000000000,
            //X400000000000000 = 0x400000000000000,
            //X800000000000000 = 0x800000000000000,
            //X1000000000000000 = 0x1000000000000000,
            //X2000000000000000 = 0x2000000000000000,
            //X4000000000000000 = 0x4000000000000000,
            //X8000000000000000 = 0x8000000000000000,
        }

        public class ObjectComparer<T>
        {
            private T _object;
            private Type _type;
            private readonly Func<T> _objProducer;
            readonly Dictionary<string, Member> _members = new Dictionary<string, Member>();

            public ObjectComparer(Func<T> objProducer)
            {
                _objProducer = objProducer;
                _object = _objProducer();
                _type = typeof(T);
                GetMembers();
            }

            public ObjectComparer(T obj)
            {
                _object = obj;
                _type = typeof(T);
                GetMembers();
            }

            private void GetMembers()
            {
                foreach (var prop in _type.GetProperties())
                {
                    _members.Add(prop.Name, new Member(prop));
                }
                foreach (var field in _type.GetFields())
                {
                    _members.Add(field.Name, new Member(field));
                }
            }

            public class Member
            {
                public Type Type;
                public bool IsFlagEnum;
                public string Name;
                public MemberDeclarationType MemDecType;
                public MemberInfo Info;

                public Member(MemberInfo memberInfo)
                {
                    Info = memberInfo;

                    if (memberInfo is FieldInfo)
                    {
                        MemDecType = MemberDeclarationType.Field;
                        Type = ((FieldInfo)memberInfo).FieldType;
                    }
                    else if (memberInfo is PropertyInfo)
                    {
                        MemDecType = MemberDeclarationType.Property;
                        Type = ((PropertyInfo)memberInfo).PropertyType;
                    }

                    IsFlagEnum = Type.GetCustomAttributes<FlagsAttribute>().Any();
                    Name = memberInfo.Name;
                }

                public enum MemberDeclarationType
                {
                    None = 0,
                    Field,
                    Property
                }

                public bool HasChanged => CurrentValue != null && !CurrentValue.Equals(PreviousValue);
                public object PreviousValue { get; set; }
                public object CurrentValue { get; set; }

                public object GetValue(object instance)
                {
                    switch (MemDecType)
                    {
                        case MemberDeclarationType.Property:
                            return ((PropertyInfo)Info).GetValue(instance);
                        case MemberDeclarationType.Field:
                            return ((FieldInfo)Info).GetValue(instance);
                    }
                    return null;
                }

                public Member Clone()
                {
                    return new Member(Info)
                    {
                        MemDecType = MemDecType,
                        Name = Name,
                        PreviousValue = PreviousValue,
                        CurrentValue = CurrentValue,
                    };
                }

                public override string ToString()
                {
                    if (IsFlagEnum)
                    {
                        Enum gained;
                        Enum lost;
                        var typeDefault = (Enum)Activator.CreateInstance(Type);
                        CompareFlags(Type, PreviousValue, CurrentValue, out gained, out lost);
                        var str = $"{Name}";
                        if (!gained.Equals(typeDefault)) str += $" gained: {gained}";
                        if (!lost.Equals(typeDefault)) str += $" lost: {lost}";
                        return str;
                    }

                    return $"{Name} from {PreviousValue} to {CurrentValue}";
                }
            }

            public List<Member> Compare(Predicate<Member> filter = null)
            {
                if (_objProducer != null)
                    _object = _objProducer();

                var result = new List<Member>();
                if (_object == null)
                    return result;

                foreach (var member in _members.Values)
                {
                    member.CurrentValue = member.GetValue(_object);
                    if (!member.HasChanged)
                        continue;

                    var isNonStringEnumerable = (member.CurrentValue is IEnumerable && !(member.CurrentValue is string));

                    if ((filter == null || filter(member)) && member.CurrentValue != null && !isNonStringEnumerable)
                    {
                        result.Add(member.Clone());
                    }

                    member.PreviousValue = member.CurrentValue;
                }
                return result;
            }

            private static readonly List<int> TestFlags = new List<int>
            {
                0x00000001,0x00000002,0x00000004,0x00000008, 0x00000010,0x00000020,0x00000040,0x00000080,
                0x00000100,0x00000200,0x00000400,0x00000800, 0x00001000,0x00002000,0x00004000,0x00008000,
                0x00010000,0x00020000,0x00040000,0x00080000, 0x00100000,0x00200000,0x00400000,0x00800000,
                0x01000000,0x02000000,0x04000000,0x08000000, 0x10000000,0x20000000,0x40000000,//0x80000000,
            };

            public static void CompareFlags(Type type, object flagsA, object flagsB, out Enum gainedFlags, out Enum lostFlags)
            {
                int flagNumA = Convert.ToInt32(flagsA);
                int flagNumB = Convert.ToInt32(flagsB);
                int gained = 0, lost = 0;

                foreach (var testValue in TestFlags)
                {
                    if (testValue > flagNumA && testValue > flagNumB)
                        break;

                    var hasA = (flagNumA & testValue) == testValue;
                    var hasB = (flagNumB & testValue) == testValue;

                    if (!hasA && hasB)
                        gained += testValue;
                    else if (hasA && !hasB)
                        lost += testValue;
                }

                gainedFlags = (Enum)Enum.ToObject(type, gained);
                lostFlags = (Enum)Enum.ToObject(type, lost);
            }

        }


        public class FlagComparison
        {
            public Enum CurrentFlags;
            private readonly Func<Enum> _valueProducer;

            public FlagComparison(Func<Enum> valueProducer)
            {
                _valueProducer = valueProducer;
            }

            public void Compare(Action<Enum> onGained, Action<Enum> onLost)
            {
                if (_valueProducer == null)
                    return;

                var previous = CurrentFlags;
                CurrentFlags = _valueProducer();

                Enum gained, lost;
                CompareFlags(previous, CurrentFlags, out gained, out lost);

                if (!gained.Equals(default(Enum)))
                    onGained(gained);

                if (!lost.Equals(default(Enum)))
                    onLost(lost);
            }

            private static readonly List<int> TestFlagValues = new List<int>
            {
                0x00000001,0x00000002,0x00000004,0x00000008,
                0x00000010,0x00000020,0x00000040,0x00000080,
                0x00000100,0x00000200,0x00000400,0x00000800,
                0x00001000,0x00002000,0x00004000,0x00008000,
                0x00010000,0x00020000,0x00040000,0x00080000,
                0x00100000,0x00200000,0x00400000,0x00800000,
                0x01000000,0x02000000,0x04000000,0x08000000,
                0x10000000,0x20000000,0x40000000,//0x80000000,
            };

            public static void CompareFlags(Enum flagsA, Enum flagsB, out Enum gainedFlags, out Enum lostFlags)
            {
                var flagNumA = Convert.ToInt32(flagsA);
                var flagNumB = Convert.ToInt32(flagsB);
                int gained = 0, lost = 0;

                foreach (var testValue in TestFlagValues)
                {
                    if (testValue > flagNumA && testValue > flagNumB)
                        break;

                    var hasA = (flagNumA & testValue) == testValue;
                    var hasB = (flagNumB & testValue) == testValue;

                    if (!hasA && hasB)
                        gained += testValue;
                    else if (hasA && !hasB)
                        lost += testValue;
                }

                gainedFlags = (Enum)Enum.ToObject(flagsA.GetType(), gained);
                lostFlags = (Enum)Enum.ToObject(flagsB.GetType(), lost);
            }

        }

        public class FlagComparison<T>
        {
            public T PreviousFlags;
            private readonly Func<T> _valueProducer;

            public FlagComparison(Func<T> valueProducer)
            {
                _valueProducer = valueProducer;
            }

            public void Compare(Action<T> onGained, Action<T> onLost)
            {
                if (_valueProducer == null)
                    return;

                var previous = PreviousFlags;
                PreviousFlags = _valueProducer();

                T gained, lost;
                CompareFlags(previous, PreviousFlags, out gained, out lost);

                if (!gained.Equals(default(T)))
                    onGained(gained);
                ;
                if (!lost.Equals(default(T)))
                    onLost(lost);
            }

            private static readonly List<uint> TestFlagValues = new List<uint>
            {
                0x00000001,0x00000002,0x00000004,0x00000008,
                0x00000010,0x00000020,0x00000040,0x00000080,
                0x00000100,0x00000200,0x00000400,0x00000800,
                0x00001000,0x00002000,0x00004000,0x00008000,
                0x00010000,0x00020000,0x00040000,0x00080000,
                0x00100000,0x00200000,0x00400000,0x00800000,
                0x01000000,0x02000000,0x04000000,0x08000000,
                0x10000000,0x20000000,0x40000000,0x80000000,
            };

            public static void CompareFlags(T flagsA, T flagsB, out T gainedFlags, out T lostFlags)
            {
                uint flagNumA = Convert.ToUInt32(flagsA);
                uint flagNumB = Convert.ToUInt32(flagsB);
                uint gained = 0, lost = 0;

                foreach (var testValue in TestFlagValues)
                {
                    if (testValue > flagNumA && testValue > flagNumB)
                        break;

                    var hasA = (flagNumA & testValue) == testValue;
                    var hasB = (flagNumB & testValue) == testValue;

                    if (!hasA && hasB)
                        gained += testValue;
                    else if (hasA && !hasB)
                        lost += testValue;
                }

                gainedFlags = (T)Enum.ToObject(typeof(T), gained);
                lostFlags = (T)Enum.ToObject(typeof(T), lost);
            }

        }


    }
}
