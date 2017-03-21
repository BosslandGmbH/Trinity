using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
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
            var seed = GetActorSeed(commonData);
            return (T)CreateActor(seed);
        }

        public static TrinityActor CreateActor(ACD commonData)
        {
            var seed = GetActorSeed(commonData);
            return CreateActor(seed);
        }

        public static T CreateActor<T>(DiaObject diaObject, int id = 0) where T : TrinityActor
        {
            var seed = GetActorSeed(diaObject, id);
            return (T)CreateActor(seed);
        }

        public static TrinityActor CreateActor(DiaObject diaObject, int id = 0)
        {
            var seed = GetActorSeed(diaObject, id);
            return CreateActor(seed);
        }

        public static ActorSeed GetActorSeed(ACD commonData)
        {
            if (commonData == null || !commonData.IsValid)
                return null;

            var actorSnoId = commonData.ActorSnoId;
            var acdId = commonData.ACDId;
            var annId = commonData.AnnId;
            var actorType = commonData.ActorType;
            var internalName = commonData.Name;
            var position = commonData.Position;
            var fagId = commonData.FastAttribGroupId;

            return new ActorSeed
            {
                IsAcdBased = true,
                IsRActorBased = false,
                ActorSnoId = actorSnoId,
                CommonData = commonData,
                AcdId = acdId,
                AnnId = annId,
                ActorType = actorType,
                InternalName = internalName,
                Position = position,
                FastAttributeGroupId = fagId
            };
        }

        public static ActorSeed GetActorSeed(DiaObject rActor, int id)
        {
            if (rActor == null || !rActor.IsValid)
                return null;
            
            var actorSnoId = rActor.ActorSnoId;
            if (GameData.ExcludedActorIds.Contains(actorSnoId))
                return null;

            var actorInfo = SnoHelper.GetActorInfo(actorSnoId);
            if (actorInfo == null)
                return null;

            var actorType = actorInfo.Type;
            if (GameData.ExcludedActorTypes.Contains(actorType))
                return null;

            var acdId = rActor.ACDId;
            var commonData = rActor.CommonData;
            var isAcdBased = acdId != -1 && commonData != null;
            var rActorId = id > 0 ? id : rActor.RActorId;            
            var annId = isAcdBased ? commonData.AnnId : -1;
            var fagId = isAcdBased ? commonData.FastAttribGroupId : -1;
            var monsterSnoId = actorInfo.MonsterSnoId;
            var monsterInfo = SnoHelper.GetMonsterInfo(monsterSnoId);
            var position = rActor.Position;
            var internalName = rActor.Name;

            return new ActorSeed
            {
                RActor = rActor,
                RActorId = rActorId,
                ActorSnoId = actorSnoId,
                ActorType = actorType,
                AcdId = acdId,
                AnnId = annId,
                ActorInfo = actorInfo,
                IsAcdBased = isAcdBased,
                IsRActorBased = true,
                InternalName = internalName,
                Position = position,
                CommonData = commonData,
                FastAttributeGroupId = fagId,
                MonsterInfo = monsterInfo,
                MonsterSnoId = monsterSnoId
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

        public static T CreateActor<T>(ActorSeed actorSeed) where T : TrinityActor, new()
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
