using System;
using Trinity.Framework.Helpers;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class RActor : MemoryWrapper
    {
        public int RActorId => ReadOffset<int>(0x0);
        public string Name => ReadString(0x4);
        public Vector3 Position => ReadOffset<Vector3>(0xA8);
        public int ActorSnoId => ReadOffset<int>(0x8C);
        public Sphere CollisionSphere => ReadOffset<Sphere>(0xB8);
        public int WorldDynamicId => ReadOffset<int>(0xE0);
        public int SceneId => ReadOffset<int>(0x84);
        public int AcdId => ReadOffset<int>(0x88);
        public int MonsterSnoId => ReadOffset<int>(0x90);
        public int AppearanceSnoId => ReadOffset<int>(0x158);
        public ActorMovement Movement => ReadOffset<IntPtr>(0x1AC).UnsafeCreate<ActorMovement>();
        public SNORecordActor ActorInfo => ZetaDia.SNO[Zeta.Game.Internals.ClientSNOTable.Actor].GetRecord<SNORecordActor>(ActorSnoId);
        public SNORecordMonster MonsterInfo => ZetaDia.SNO[Zeta.Game.Internals.ClientSNOTable.Monster].GetRecord<SNORecordMonster>(MonsterSnoId);
        public ActorCommonData CommonData => Core.Actors.GetCommonDataById(AcdId);

        //public const int SizeOf = 0x368; // 872
        //public int RActorId => ReadOffset<int>(0x0);
        //public ActorType ActorType => ReadOffset<ActorType>(0x010);
        //public int AcdId => ReadOffset<int>(0x84);
        //public int ActorSnoId => ReadOffset<int>(0x8c);
        //public int PhysMeshSnoId => ReadOffset<int>(0x18);
        //public int AppearanceSnoId => ReadOffset<int>(0x158);
        //public int PhysicsSnoId => ReadOffset<int>(0x84);
        //public AxialCylinder AxialCylinder => ReadOffset<AxialCylinder>(0x030);
        //public int AnimSetSnoId => ReadOffset<int>(0x068);
        //public int MonsterSnoId => ReadOffset<int>(0x06C);
        //public Vector3 Position => ReadOffset<Vector3>(0x088);
        //public Sphere x030 => ReadOffset<Sphere>(0x030);
        //public AABB x040 => ReadOffset<AABB>(0x040);
        //public int x2B8 => ReadOffset<int>(0x2B8);
        //public int x2BC => ReadOffset<int>(0x2BC);
        //public float x2C0 => ReadOffset<float>(0x2C0);
        //public float x2C4 => ReadOffset<float>(0x2C4);
        //public float x2C8 => ReadOffset<float>(0x2C8);
        //public ActorCollisionData x2CC_ActorCollisionData => ReadObject<ActorCollisionData>(0x2CC);

        //public int x340 => ReadOffset<int>(0x340);
        //public string x348_Text => ReadSerializedString(0x348, x350_SerializeData);
        //public SerializeData x350_SerializeData => ReadOffset<SerializeData>(0x350);
        //public string x358_Text => ReadSerializedString(0x358, x360_SerializeData);
        //public SerializeData x360_SerializeData => ReadOffset<SerializeData>(0x360);
    }

    //public class ActorCollisionData : MemoryWrapper
    //{
    //    public const int SizeOf = 0x44; // 68
    //    public ActorCollisionFlags x00_ActorCollisionFlags => ReadObject<ActorCollisionFlags>(0x00);
    //    public int x10 { get { return ReadOffset<int>(0x10); } }
    //    public AxialCylinder x14_AxialCylinder { get { return ReadOffset<AxialCylinder>(0x14); } }
    //    public AABB x28 { get { return ReadOffset<AABB>(0x28); } }
    //    public float x40 { get { return ReadOffset<float>(0x40); } }
    //}

    //public class ActorCollisionFlags : MemoryWrapper
    //{
    //    public const int SizeOf = 0x10; // 16
    //    public int x00 => ReadOffset<int>(0x00);
    //    public int x04 => ReadOffset<int>(0x04);
    //    public int x08 => ReadOffset<int>(0x08);
    //    public int x0C => ReadOffset<int>(0x0C);
    //}
}