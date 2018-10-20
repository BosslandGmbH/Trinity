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
        protected readonly DiaObject Actor;
        private readonly ACD _fixedACD;

        public ActorBase(ACD acd, ActorType type)
        {
            _fixedACD = acd;
            Actor = null;

            InternalName = CommonData?.Name ?? string.Empty;
            ActorType = type;
        }

        public ActorBase(DiaObject actor)
        {
            Actor = actor;

            // TODO: Verify this was never set in Trinity, except for ItemProperties.
            InternalName = Actor.Name ?? string.Empty;
            if (CommonData != null)
            {
                InternalName = Actor.CommonData?.Name;
            }

            // A property that is accessed very often from other properties. In Zeta this one
            // is hold per frame (which is ok there). But here in Trinity we should keep it
            // persistent per object.
            ActorType = Actor.ActorType;
        }

        public ActorType ActorType { get; private set; }


        public bool IsAcdBased => Actor == null;
        public bool IsRActorBased => Actor != null;
        public Vector3 Position => _fixedACD?.Position ?? Actor?.Position ?? Vector3.Zero;
        public int AcdId => CommonData?.ACDId ?? 0;
        public int AnnId => CommonData?.AnnId ?? 0;
        public int RActorId => Actor.RActorId;
        public string InternalName { get; internal set; }
        public int ActorSnoId => CommonData?.ActorSnoId ?? 0;
        public ACD CommonData => _fixedACD ?? Actor.CommonData;
        public DiaObject RActor => Actor;

        public SNORecordActor ActorInfo => CommonData?.ActorInfo ?? default(SNORecordActor);
        public SNORecordMonster MonsterInfo => CommonData?.MonsterInfo ?? default(SNORecordMonster);

        public bool IsValid => (!IsRActorBased || RActor.IsValid) && (!IsAcdBased || IsAcdValid) && (ActorInfo?.IsValid ?? true);
        public bool IsRActorValid => RActor != null && RActor.IsValid && RActor.RActorId != -1 && !IsRActorDisposed;
        public bool IsAcdValid => CommonData != null && CommonData.IsValid && !CommonData.IsDisposed;
        public bool IsRActorDisposed => Actor.BaseAddress == IntPtr.Zero || AnnId != -1 && IsAcdBased && (!IsAcdValid || AnnId != CommonData.AnnId);
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