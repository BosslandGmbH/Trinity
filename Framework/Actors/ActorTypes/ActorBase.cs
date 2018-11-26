using System;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.ActorTypes
{
    /// <summary>
    /// Minimum set of properties shared by all actors.
    /// </summary>
    public class ActorBase
    {
        private readonly DiaObject _fixedActor;
        private readonly ACD _fixedACD;

        public ActorBase(ACD acd, ActorType type)
        {
            _fixedACD = acd;
            _fixedActor = null;
            InternalName = CommonData?.Name ?? string.Empty;
            ActorType = type;
        }

        public ActorBase(DiaObject fixedActor)
        {
            _fixedActor = fixedActor;
            _fixedACD = fixedActor.CommonData;

            // TODO: Verify this was never set in Trinity, except for ItemProperties.
            InternalName = _fixedActor.Name ?? string.Empty;
            if (CommonData != null)
            {
                InternalName = _fixedActor.CommonData?.Name;
            }

            // A property that is accessed very often from other properties. In Zeta this one
            // is hold per frame (which is ok there). But here in Trinity we should keep it
            // persistent per object.
            ActorType = _fixedActor.ActorType;
        }

        public ACD CommonData => _fixedACD;
        public DiaObject Actor => _fixedActor;

        public ActorType ActorType { get; private set; }

        public bool IsAcdBased => _fixedActor == null;
        public bool IsRActorBased => _fixedActor != null;
        public Vector3 Position => _fixedACD?.Position ?? _fixedActor?.Position ?? Vector3.Zero;
        public int AcdId => CommonData?.ACDId ?? -1;
        public int AnnId => CommonData?.AnnId ?? -1;
        public int RActorId => _fixedActor?.RActorId ?? -1;
        public string InternalName { get; internal set; }
        public int ActorSnoId => CommonData?.ActorSnoId ?? -1;

        public SNORecordActor ActorInfo => CommonData?.ActorInfo ?? default(SNORecordActor);
        public SNORecordMonster MonsterInfo => CommonData?.MonsterInfo ?? default(SNORecordMonster);

        public bool IsValid => (!IsRActorBased || Actor.IsValid) && (!IsAcdBased || IsAcdValid) && (ActorInfo?.IsValid ?? true);
        public bool IsRActorValid => Actor != null && Actor.IsValid && Actor.RActorId != -1 && !IsRActorDisposed;
        public bool IsAcdValid => CommonData != null && CommonData.IsValid && !CommonData.IsDisposed;
        public bool IsRActorDisposed => _fixedActor.BaseAddress == IntPtr.Zero || AnnId != -1 && IsAcdBased && (!IsAcdValid || AnnId != CommonData.AnnId);
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
