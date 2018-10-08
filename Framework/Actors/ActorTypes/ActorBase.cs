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
        protected readonly DiaObject _actor;

        public ActorBase(DiaObject actor)
        {
            _actor = actor;

            // TODO: Verify this was never set in Trinity, except for ItemProperties.
            InternalName = _actor.Name ?? string.Empty;
            if (CommonData != null)
            {
                InternalName = _actor.CommonData?.Name;
            }
        }

        public bool IsAcdBased => _actor == null;
        public bool IsRActorBased => _actor != null;
        public Vector3 Position => _actor.Position;
        public int AcdId => CommonData?.ACDId ?? 0;
        public int AnnId => CommonData?.AnnId ?? 0;
        public ActorType ActorType => _actor.ActorType;
        public int RActorId => _actor.RActorId;
        public string InternalName { get; internal set; }
        public int ActorSnoId => CommonData?.ActorSnoId ?? 0;
        public ACD CommonData => _actor.CommonData;
        public DiaObject RActor => _actor;

        public SNORecordActor ActorInfo => CommonData?.ActorInfo ?? default(SNORecordActor);
        public SNORecordMonster MonsterInfo => CommonData?.MonsterInfo ?? default(SNORecordMonster);

        public bool IsValid => (!IsRActorBased || RActor.IsValid) && (!IsAcdBased || IsAcdValid) && (ActorInfo?.IsValid ?? true);
        public bool IsRActorValid => RActor != null && RActor.IsValid && RActor.RActorId != -1 && !IsRActorDisposed;
        public bool IsAcdValid => CommonData != null && CommonData.IsValid && !CommonData.IsDisposed;
        public bool IsRActorDisposed => _actor.BaseAddress == IntPtr.Zero || AnnId != -1 && IsAcdBased && (!IsAcdValid || AnnId != CommonData.AnnId);
        public int FastAttributeGroupId => CommonData?.FastAttribGroupId ?? 0;

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

        public override string ToString() => $"{GetType().Name}: RActorId={RActorId}, {ActorType}, {InternalName}";
    }
}