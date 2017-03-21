using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors
{
    public static class ActorFactory
    {
        public class ActorSeed
        {
            public SNORecordActor ActorInfo;
            public int ActorSnoId;
            public int RActorId;
            public ActorType ActorType;
            public DiaObject RActor;
            public int AcdId;
            public string InternalName;
            public Vector3 Position;
            public ACD CommonData;
            public bool IsAcdBased;
            public bool IsRActorBased;
            public int FastAttributeGroupId;
            public int MonsterSnoId;
            public SNORecordMonster MonsterInfo;
            public int AnnId;
        }

        public static T CreateActor<T>(ACD commonData) where T : TrinityActor
        {
            return (T)CreateActor(GetActorSeed(commonData));
        }

        public static TrinityActor CreateActor(ACD commonData)
        {
            return CreateActor(GetActorSeed(commonData));
        }

        public static T CreateActor<T>(DiaObject diaObject) where T : TrinityActor
        {
            return (T)CreateActor(GetActorSeed(diaObject));
        }

        public static TrinityActor CreateActor(DiaObject diaObject)
        {
            return CreateActor(GetActorSeed(diaObject));
        }

        public static ActorSeed GetActorSeed(ACD commonData)
        {
            if (commonData == null || !commonData.IsValid)
                return null;

            return new ActorSeed
            {
                IsAcdBased = true,
                IsRActorBased = false,
                ActorSnoId = commonData.ActorSnoId,
                CommonData = commonData,
                AcdId = commonData.ACDId,
                AnnId = commonData.AnnId,
                ActorType = commonData.ActorType,
                InternalName = commonData.Name,
                Position = commonData.Position,
                FastAttributeGroupId = commonData.FastAttribGroupId,
            };
        }

        public static ActorSeed GetActorSeed(DiaObject rActor)
        {
            if (rActor == null || !rActor.IsValid)
                return null;

            var acdId = rActor.ACDId;
            var commonData = rActor.CommonData;
            var isAcdBased = commonData != null;
            var actorInfo = rActor.ActorInfo;
            if (actorInfo == null)
                return null;

            return new ActorSeed
            {
                RActor = rActor,
                RActorId = rActor.RActorId,
                ActorSnoId = rActor.ActorSnoId,
                ActorType = actorInfo.Type,
                AcdId = acdId,
                AnnId = isAcdBased ? rActor.CommonData.AnnId : -1,
                ActorInfo = actorInfo,
                IsAcdBased = isAcdBased,
                IsRActorBased = true,
                InternalName = rActor.Name,
                Position = rActor.Position,
                CommonData = commonData,
                FastAttributeGroupId = isAcdBased ? commonData.FastAttribGroupId : -1,
                MonsterInfo = commonData?.MonsterInfo,
                MonsterSnoId = actorInfo.MonsterSnoId
            };
        }

        public static TrinityActor CreateActor(ActorSeed seed)
        {
            if (seed == null)
                return null;

            switch (seed.ActorType)
            {
                case ActorType.Item:
                    return CreateActor<TrinityItem>(seed);
                case ActorType.Player:
                    return CreateActor<TrinityPlayer>(seed);
            }

            return CreateActor<TrinityActor>(seed);
        }

        public static T CreateActor<T>(ActorSeed actorSeed) where T : ActorBase, new()
        {
            var actor = new T
            {
                RActor = actorSeed.RActor,
                RActorId = actorSeed.RActorId,
                AcdId = actorSeed.AcdId,
                AnnId = actorSeed.AnnId,
                ActorSnoId = actorSeed.ActorSnoId,
                ActorInfo = actorSeed.ActorInfo,
                ActorType = actorSeed.ActorType,
                InternalName = actorSeed.InternalName,
                Position = actorSeed.Position,
                CommonData = actorSeed.CommonData,
                IsAcdBased = actorSeed.IsAcdBased,
                IsRActorBased = actorSeed.IsRActorBased,
                FastAttributeGroupId = actorSeed.FastAttributeGroupId,
                MonsterInfo = actorSeed.MonsterInfo,
                MonsterSnoId = actorSeed.MonsterSnoId
            };

            actor.OnCreated();
            return actor;
        }

    }
}
