using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Reference;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class PowerMap : MemoryWrapper
    {
        public int x00_Mask => ReadOffset<int>(0x00);
        public int x04 => ReadOffset<int>(0x04);
        public int x08_BucketCount => ReadOffset<int>(0x08);
        public int x0C_EntryCount => ReadOffset<int>(0x0C);
        public BasicAllocator x10_DynAllocator => ReadPointer<BasicAllocator>(0x10);
        public IntPtr BucketsStartAddr => ReadOffset<IntPtr>(0x18);
        public PtrTable<PowerMapItem> _0x10_Buckets => ReadObject<PtrTable<PowerMapItem>>(0x18);

        public Dictionary<TagType, PowerMapItem> Entries
        {
            get
            {
                var entries = new Dictionary<TagType, PowerMapItem>();
                foreach (var firstItem in _0x10_Buckets.Items)
                {
                    var pair = firstItem;
                    while (pair != null)
                    {
                        entries.Add(pair.x04_Key, pair);
                        pair = pair.x00_Next;
                    }
                }
                return entries;
            }
        }

        public override string ToString() => $"{GetType().Name}: Count={x0C_EntryCount}";

        private static uint TagHasher(int key)
        {
            return unchecked((uint)(key ^ (key >> 12)));
        }

        public bool TryGetItemByKey(TagType key, out PowerMapItem value)
        {
            var hash = TagHasher((int)key);
            var bucketIndex = unchecked((int)(hash & x00_Mask));
            var bucketEntry = _0x10_Buckets[bucketIndex];
            while (bucketEntry != null)
            {
                if (bucketEntry.x04_Key.Equals(key))
                {
                    value = Create<PowerMapItem>(bucketEntry.BaseAddress);
                    return true;
                }
                bucketEntry = bucketEntry.x00_Next;
            }
            value = default(PowerMapItem);
            return false;
        }

        public class FormulaCode : MemoryWrapper
        {
            public List<Token> Tokens => ReadObjects<Token>(0x00, StopCondition, TokenFactory);

            public class Token : MemoryWrapper
            {
                public const int SizeOf = 4;
                public TokenType Type => ReadOffset<TokenType>(0x00);            
            }

            public class NumberToken : Token
            {
                public float FloatValue => ReadOffset<float>(0x4);
            }

            public class LinkToken : Token
            {
                public int Index => ReadOffset<int>(0x4);
                public int SnoPowerId => ReadOffset<int>(0x8);
                public SNOPower SnoPower => ReadOffset<SNOPower>(0x8);
                public int TagId => ReadOffset<int>(0xC); // ?? rune links have ids ~850
                public TagType TagType => ReadOffset<TagType>(0xC);
            }

            public enum TokenType
            {
                EndMarker = 4,
                Condition = -1,
                Number  = 6,
                Link = 5
            }

            public bool StopCondition(Token token) => token == null || token.Type == TokenType.EndMarker;

            public Token TokenFactory(Token token)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        return token.Cast<NumberToken>();
                    case TokenType.Link:
                        return token.Cast<LinkToken>();
                }
                return token;
            }
        }

        public class TagFormulaValue : MemoryWrapper, IMapValue
        {
            public TagType TagType => ReadOffset<TagType>(-0x04);
            public int Length => ReadOffset<int>(0x14);
            public FormulaCode Data => ReadPointer<FormulaCode>(0x08);
            public string Formula => ReadStringPointer(0x00);
            public float Value
            {
                get
                {
                    var str = Formula;
                    int intResult;
                    if (int.TryParse(str, out intResult))
                        return intResult;

                    float floatResult;
                    if (float.TryParse(str, out floatResult))
                        return floatResult;

                    return 0;
                }
            }

            public bool ContainsLink => Data.Tokens.Any(t => t.Type == FormulaCode.TokenType.Link);
            public bool RequiresEvaluation => Data.Tokens.Count > 3 || ContainsLink;

            public override string ToString() => Formula;
            object IMapValue.Value => Value;
        }

        public class TagValueType<T> : IMapValue
        {
            public TagValueType(T value)
            {
                Value = value;
            }
            public T Value { get; }
            object IMapValue.Value => Value;
            public override string ToString() => $"{Value}";        
        }

        public interface IMapValue
        {
            object Value { get; }
        }

        public class PowerMapItem : MemoryWrapper
        {
            public static int SizeOf = 12;
            public PowerMapItem x00_Next => ReadPointer<PowerMapItem>(0x00);
            public int x04_KeyId => ReadOffset<int>(0x04);
            public TagType x04_Key => ReadOffset<TagType>(0x04);
            public int x08_IntValue => ReadOffset<int>(0x08);
            public float x08_FloatValue => ReadOffset<float>(0x08);
            public TagFormulaValue x08_FomulaValue => ReadPointer<TagFormulaValue>(0x08);
            public TagReference TagRef => Tags.GetTag(x04_KeyId);

            public IMapValue Value
            {
                get
                {
                    if (TagRef.DataType == MapDataType.Formula)
                        return ReadPointer<TagFormulaValue>(0x08);

                    int intResult;
                    if (int.TryParse(x08_IntValue.ToString(), out intResult))
                        return new TagValueType<int>(intResult);

                    float floatResult;
                    if (float.TryParse(x08_FloatValue.ToString(CultureInfo.InvariantCulture), out floatResult))
                        return new TagValueType<float>(intResult);

                    return null;
                }
            }

            public T GetValue<T>() where T : MemoryWrapper, new()
                => ReadObject<MemoryWrapper>(0x08).Cast<T>();
        
            public override string ToString() => $"{Value}";

        }
    }


}
