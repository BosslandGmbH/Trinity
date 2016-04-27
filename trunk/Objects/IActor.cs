using System;
using System.Collections.Generic;
using Trinity.Helpers;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Objects
{
    public interface IActor : IFindable
    {
        float CollisionRadius { get; set; }
        Vector3 Position { get; set; }
        DiaObject DiaObject { get; set; }
        ACD CommonData { get; set; }
        ACDItem ACDItem { get; set; }
        DiaGizmo DiaGizmo { get; set; }
        DiaItem DiaItem { get; set; }
        DiaUnit DiaUnit { get; set; }
        float Distance { get; set; }
        float RadiusDistance { get; set; }
        bool IsMonster { get; set; }
        bool IsGizmo { get; set; }
        bool IsItem { get; set; }
        double Weight { get; set; }
        string InternalName { get; set; }
        int ACDGuid { get; set; }
        bool IsUsed { get; set; }
        bool IsUsable { get; set; }
        bool IsDestroyed { get; set; }
        bool IsDestroyable { get; set; }
        bool IsPickupNoClick { get; set; }
        int ActorSNO { get; set; }
        int RActorGuid { get; set; }
        int GameBalanceId { get; set; }
        bool IsBoss { get; set; }
        bool IsGoblin { get; set; }
        bool IsSummoner { get; set; }
        bool IsSummoned { get; set; }
        bool IsDead { get; set; }
        bool IsInCombat { get; set; }
        bool IsUnit { get; set; }
        bool IsValid { get; }
        double HitpointsPct { get; set; }
        bool IsBossOrEliteRareUnique { get; set; }
        bool IsTrashMob { get; set; }
        bool IsFacingPlayer { get; set; }
        int UnitsInFront { get; set; }
        bool IsInLineOfSight { get; set; }
        bool IsEliteRareUnique { get; set; }
        float HitpointsCurrent { get; set; }
        float Radius { get; set; }
        string Name { get; set; }
        bool IsInvulnerable { get; set; }
        bool IsHidden { get; set; }
        bool IsMoving { get; set; }
        bool IsFriendly { get; set; }
        bool IsUntargetable { get; set; }
        float ZDiff { get; set; }
        bool IsHostile { get; set; }
        bool IsDoor { get; set; }
        bool IsNPC { get; set; }
        bool IsBlacklisted { get; set; }
        bool IsBountyObjective { get; set; }
        bool IsMinimapActive { get; set; }
        bool IsMe { get; set; }
        bool IsPlayer { get; set; }
        bool IsAvoidance { get; set; }
        bool IsElite { get; set; }
        bool IsTreasureGoblin { get; set; }
        float RotationRadians { get; set; }
        DateTime CreationTime { get; set; }
        float MovementSpeed { get; set; }
        bool IsStunned { get; set; }
        bool InGreaterRift { get; set; }
        long Coinage { get; set; }
        Rotator Rotator { get; set; }
        ICollection<TrinityMonsterAffix> MonsterAffixes { get; set; }
        ObjectType Type { get; set; }
        ActorType ActorType { get; set; }
        GizmoType GizmoType { get; set; }
        AnimationState AnimationState { get; set; }
        SNOAnim CurrentAnimation { get; set; }
        TargetingType TargetingType { get; set; }
        MonsterType MonsterType { get; set; }
        MonsterSize MonsterSize { get; set; }
        MonsterQuality MonsterQualityLevel { get; set; }
        double SecondaryResource { get; set; }
        double PrimaryResource { get; set; }
        bool HasBuff(SNOPower snoPower);
        int BuffStacks(SNOPower snoPower);
        bool HasDebuff(SNOPower snoPower);
        bool Interact();
    }

}









