using System;
using System.Collections.Generic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using GizmoType = Trinity.Framework.Objects.Memory.Symbols.Types.GizmoType;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class ActorCommonData : MemoryWrapper
    {
        public int AcdId => ReadOffset<int>(0x000);
        public string Name => ReadString(0x004, 100);
        public int AnnId => ReadOffset<int>(0x088);
        public int ActorSnoId => ReadOffset<int>(0x090);
        public int MonsterSnoId => ReadOffset<int>(0x098);
        public GameBalanceType GameBalanceType => ReadOffset<GameBalanceType>(0x0B0);
        public int GameBalanceId => ReadOffset<int>(0x0B4);
        public MonsterQuality MonsterQuality => ReadOffset<MonsterQuality>(0x0B8);
        public InventorySlot InventorySlot => ReadOffset<InventorySlot>(0x114);
        public int InventoryRow => ReadOffset<int>(0x11C); // Y
        public int InventoryColumn => ReadOffset<int>(0x118); // X
        public int FastAttributeGroupId => ReadOffset<int>(0x120);
        public Vector3 Position => new Vector3(ReadOffset<float>(0x0D0), ReadOffset<float>(0x0D4), ReadOffset<float>(0x0D8));
        public int Radius => ReadOffset<int>(0x0DC);
        public GizmoType GizmoType => ReadOffset<GizmoType>(0x180);
        public ActorType ActorType => ReadOffset<ActorType>(0x184);
        public float HitPoints => ReadOffset<float>(0x188);
        public Containers.LinkedList<int> ItemAffixes => ReadObject<Containers.LinkedList<int>>(0x148);
        public int MagicPrefixGameBalanceId => ItemAffixes.Last.Value;
        public int MagicSuffixGameBalanceId => ItemAffixes.First.Value; 
        public int RareItemPartAIsPrefix => ReadOffset<int>(0x170);
        public int RareItemPartAStringListIndex => ReadOffset<int>(0x17C); 
        public int RareItemPartBStringListId => ReadOffset<int>(0x174); 
        public int RareItemPartBStringListIndex => ReadOffset<int>(0x178);
        public int GoodFood => ReadOffset<int>(0x2c0);
        public bool IsDisposed => GoodFood == unchecked((int)0xFEEDFACE);
        public List<int> AffixIds => ReadArray<int>(0x1AC, 8); // 0x1a4 (2 affixes previous to this? (-8))
        public IntPtr AnimationInfoAddress => ReadOffset<IntPtr>(0x210);
        public SNOAnim Animation => AnimationInfoAddress != IntPtr.Zero ? Read<SNOAnim>(AnimationInfoAddress + 0x04) : default(SNOAnim);
        public AnimationState AnimationState => AnimationInfoAddress != IntPtr.Zero ? Read<AnimationState>(AnimationInfoAddress + 0xD8) : default(AnimationState);


        //public object ItemType { get; set; }
        //public DBItemBaseType DBItemBaseType { get; set; }
        //public FollowerType FollowerSpecialType { get; set; }

    }
}

