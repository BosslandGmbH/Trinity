using System;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors
{
    /// <summary>
    /// Creates actors
    /// </summary>
    public static class ActorFactory
    {
        /// <summary>
        /// Object to hold the minimum requirements for creating an actor object, 
        /// to be able to switch based on ActorType without reading memory multiple times.
        /// Centralizes varying source data - ACD only, RActor only and ACD + RActor
        /// </summary>
        public class ActorSeed
        {
            public SNORecordActor ActorInfo;
            public int ActorSnoId;
            public int RActorId;
            public ActorType ActorType;
            public RActor RActor;
            public int AcdId;
            public string InternalName;
            public Vector3 Position;
            public ActorCommonData CommonData;
            public bool IsAcdBased;
            public bool IsRActorBased;
            public int FastAttributeGroupId;
            public int MonsterSnoId;
            public SNORecordMonster MonsterInfo;
            public int AnnId;
        }

        /// <summary>
        /// Create a cached actor of the proper derived type.
        /// </summary>
        public static ActorBase CreateActor(RActor rActor)
        {
            var seed = GetActorSeed(rActor);
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

        public static ActorSeed GetActorSeed(ActorCommonData commonData)
        {
            if (commonData == null || !commonData.IsValid)
                return null;

            return new ActorSeed
            {
                IsAcdBased = true,
                IsRActorBased = false,
                ActorSnoId = commonData.ActorSnoId,
                CommonData = commonData,
                AcdId = commonData.AcdId,
                AnnId = commonData.AnnId,
                ActorType = commonData.ActorType,
                InternalName = commonData.Name,
                Position = commonData.Position,
                FastAttributeGroupId = commonData.FastAttributeGroupId,
            };
        }

        public static ActorSeed GetActorSeed(RActor rActor)
        {           
            if (rActor == null || !rActor.IsValid)
                return null;

            var acdId = rActor.AcdId;            
            var isAcdBased = acdId != -1;
            var commonData = isAcdBased ? rActor.CommonData : null;
            var annId = isAcdBased ? rActor.CommonData.AnnId : -1;
            var fastAttributeGroupId = isAcdBased ? commonData.FastAttributeGroupId : -1;
            var actorSnoId = rActor.ActorSnoId;
            var actorInfo = ZetaDia.SNO[Zeta.Game.Internals.ClientSNOTable.Actor].GetRecord<SNORecordActor>(actorSnoId);
            var type = actorInfo.Type;

            var actorBones = new ActorSeed
            {
                RActor = rActor,
                RActorId = rActor.RActorId,
                ActorSnoId = actorSnoId,
                ActorType = type,
                AcdId = acdId,
                AnnId = annId,
                ActorInfo = actorInfo,
                IsAcdBased = isAcdBased,
                IsRActorBased = true,
                InternalName = rActor.Name,
                Position = rActor.Position,
                CommonData = commonData,
                FastAttributeGroupId = fastAttributeGroupId,
            };

            if (type == ActorType.Monster)
            {
                actorBones.MonsterSnoId = rActor.MonsterSnoId;
                actorBones.MonsterInfo = ZetaDia.SNO[Zeta.Game.Internals.ClientSNOTable.Monster].GetRecord<SNORecordMonster>(actorBones.MonsterSnoId);
            }

            return actorBones;
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

        public static bool IsAcdBased { get; set; }

        public static RActor CreateRActor(IntPtr ptr)
        {
            return MemoryWrapper.Create<RActor>(ptr);
        }

        public static ActorCommonData CreateCommonData(IntPtr ptr)
        {
            return MemoryWrapper.Create<ActorCommonData>(ptr);
        }

        public static T CreateFromRActorPtr<T>(IntPtr ptr) where T : ActorBase, new()
        {
            return CreateFromRActor<T>(CreateRActor(ptr));
        }

        public static T CreateFromRActor<T>(RActor rActor) where T : ActorBase, new()
        {
            return CreateActor<T>(GetActorSeed(rActor));
        }

        public static T CreateFromAcdPtr<T>(IntPtr ptr) where T : ActorBase, new()
        {
            return CreateFromAcd<T>(CreateCommonData(ptr));
        }

        public static T CreateFromAcd<T>(ActorCommonData commonData) where T : ActorBase, new()
        {
            return CreateActor<T>(GetActorSeed(commonData));
        }

        public static TrinityItem CreateActor(ACDItem item)
        {
            return CreateFromRActorPtr<TrinityItem>(item.BaseAddress);
        }

        public static TrinityActor CreateActor(DiaObject diaObject)
        {
            return CreateFromRActorPtr<TrinityActor>(diaObject.BaseAddress);
        }

    }
}
