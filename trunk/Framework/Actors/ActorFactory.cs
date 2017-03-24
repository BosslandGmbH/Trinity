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

            var seed = new ActorSeed();
            seed.IsAcdBased = true;
            seed.IsRActorBased = false;
            seed.ActorSnoId = commonData.ActorSnoId;
            seed.CommonData = commonData;
            seed.AcdId = commonData.ACDId;
            seed.AnnId = commonData.AnnId;
            seed.ActorType = commonData.ActorType;
            seed.InternalName = commonData.Name;
            seed.Position = commonData.Position;
            seed.FastAttributeGroupId = commonData.FastAttribGroupId;
            return seed;
        }

        public static ActorSeed GetActorSeed(DiaObject rActor)
        {
            if (rActor == null || !rActor.IsValid)
                return null;

            var commonData = rActor.CommonData;
            var isAcdBased = commonData != null;
            if (isAcdBased && (!commonData.IsValid || commonData.IsDisposed))
                return null;

            var actorInfo = rActor.ActorInfo;
            if (actorInfo == null || !actorInfo.IsValid)
                return null;

            var seed = new ActorSeed();
            seed.RActor = rActor;
            seed.RActorId = rActor.RActorId;
            seed.ActorSnoId = rActor.ActorSnoId;
            seed.ActorType = actorInfo.Type;
            seed.AcdId = rActor.ACDId;
            seed.AnnId = isAcdBased ? rActor.CommonData.AnnId : -1;
            seed.ActorInfo = actorInfo;
            seed.IsAcdBased = isAcdBased;
            seed.IsRActorBased = true;
            seed.InternalName = rActor.Name;
            seed.Position = rActor.Position;
            seed.CommonData = commonData;
            seed.FastAttributeGroupId = isAcdBased ? commonData.FastAttribGroupId : -1;
            seed.MonsterInfo = commonData?.MonsterInfo;
            seed.MonsterSnoId = actorInfo.MonsterSnoId;
            return seed;
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
