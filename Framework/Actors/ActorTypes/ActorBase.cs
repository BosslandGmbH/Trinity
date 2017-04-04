#region

using System;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

#endregion

namespace Trinity.Framework.Actors.ActorTypes
{
    /// <summary>
    /// Minimum set of properties shared by all actors.
    /// </summary>
    public class ActorBase
    {
        private SNORecordActor _actorInfo;
        private SNORecordMonster _monsterInfo;
        public bool IsAcdBased { get; set; }
        public bool IsRActorBased { get; set; }
        public Vector3 Position { get; set; }
        public int AcdId { get; set; }
        public int AnnId { get; set; }
        public ActorType ActorType { get; set; } = ActorType.Invalid;
        public int RActorId { get; set; }
        public string InternalName { get; set; }
        public int ActorSnoId { get; set; }
        public int MonsterSnoId { get; set; }
        public ACD CommonData { get; set; }
        public DiaObject RActor { get; set; }

        public SNORecordActor ActorInfo
        {
            // Map on these records is causing d3 to crash.
            get { return CommonData != null && CommonData.IsValid && _actorInfo != null && _actorInfo.IsValid ? _actorInfo : default(SNORecordActor); }
            set { _actorInfo = value; }
        }

        public SNORecordMonster MonsterInfo
        {
            // Map on these records is causing d3 to crash.
            get { return CommonData != null && CommonData.IsValid && _monsterInfo != null && _monsterInfo.IsValid ? _monsterInfo : default(SNORecordMonster); }
            set { _monsterInfo = value; }
        }

        public bool IsValid => (!IsRActorBased || RActor.IsValid) && (!IsAcdBased || IsAcdValid) && (ActorInfo?.IsValid ?? true);
        public bool IsRActorValid => RActor != null && RActor.IsValid && RActor.RActorId != -1 && !IsRActorDisposed;
        public bool IsAcdValid => CommonData != null && CommonData.IsValid && !CommonData.IsDisposed;
        public bool IsRActorDisposed => AnnId != -1 && IsAcdBased && (!IsAcdValid || AnnId != CommonData.AnnId);
        public int FastAttributeGroupId { get; set; }
        public double CreateTime { get; set; }
        public double UpdateTime { get; set; }
        public DateTime Created { get; set; }

        /// <summary>
        /// Occurs when the actor data is newly existing;
        /// Usually when it first comes into range of the player.
        /// </summary>
        public virtual void OnCreated() { }

        /// <summary>
        /// Occurs every time the actor is updated by ActorCache
        /// How often depends the UpdateInterval set in the ActorCache.
        /// </summary>
        public virtual void OnUpdated() { }

        /// <summary>
        /// Occurs whenever the actor's data is no longer valid;
        /// Which doesn't mean the actor is dead, it may just be out of range.
        /// Actors may also be dead but OnDestroyed has not yet been called.
        /// </summary>
        public virtual void OnDestroyed() { }

        public override string ToString() => $"{GetType().Name}: RActorId={RActorId}, {ActorType}, {InternalName}";
    }
}