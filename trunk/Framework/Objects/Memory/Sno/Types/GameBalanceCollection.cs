using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory.Sno.Types
{
    public class GameBalanceCollection : NativeGameBalance
    {
        //public const int SizeOf = 552; // 0x228
        public SnoGameBalanceType GameBalanceType => ReadOffset<SnoGameBalanceType>(0xC);
        public GameBalanceTableId SnoType => (GameBalanceTableId)Header.SnoId;
        public int TableIndex => ReadOffset<int>(0xC);
        public List<GameBalanceVector> Tables => ReadObjects<GameBalanceVector>(0x18, 32, v => v.HasData);

        public class GameBalanceVector : MemoryWrapper
        {
            public const int SizeOf = 0x10; // 16
            public bool HasData => ReadOffset<int>(0x0) != 0;
            public NativeSerializeData SerializeData => ReadObject<NativeSerializeData>(0x0);
            public IntPtr TableStart => ReadOffset<IntPtr>(0x8);
            public int Offset => (int)BaseAddress-(int)ParentAddress;
            public List<T> Cast<T>() where T : MemoryWrapper, new() 
                => ReadSerializedObjects<T>(Offset, SerializeData, ParentAddress);
        }        

        public object Value
        {
            get
            {
                switch (SnoType)
                {
                    case GameBalanceTableId.Characters:
                        return Tables.SelectMany(i => i.Cast<NativeHeroData>());
                    case GameBalanceTableId.Hirelings:
                        return Tables.SelectMany(i => i.Cast<NativeHirelingEntry>());
                    case GameBalanceTableId._xx1AffixList:
                    case GameBalanceTableId.x1AffixList:
                    case GameBalanceTableId.AffixList:
                        return Tables.SelectMany(i => i.Cast<NativeAffixTableEntry>());
                    case GameBalanceTableId.ParagonBonuses:
                        return Tables.SelectMany(i => i.Cast<NativeParagonBonus>());
                    case GameBalanceTableId.ItemSalvageLevels:
                        return Tables.SelectMany(i => i.Cast<NativeItemSalvageLevel>());
                    case GameBalanceTableId.HandicapLevels:
                        return Tables.SelectMany(i => i.Cast<NativeHandicapLevel>());
                }
                return null;
            }
        }

        public override string ToString() => $"{GetType().Name}: {(GameBalanceTableId)Header.SnoId}";
    }
}