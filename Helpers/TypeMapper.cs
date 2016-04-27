using System;
using Trinity.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Helpers
{
    public class TypeMapper
    {
        public static TrinityPlugin.Weighting.ShrineTypes GetShrineType(IActor actor)
        {
            switch (actor.ActorSNO)
            {
                case (int)SNOActor.a4_Heaven_Shrine_Global_Fortune:
                case (int)SNOActor.Shrine_Global_Fortune:
                    return TrinityPlugin.Weighting.ShrineTypes.Fortune;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Frenzied:
                case (int)SNOActor.Shrine_Global_Frenzied:
                    return TrinityPlugin.Weighting.ShrineTypes.Frenzied;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Reloaded:
                case (int)SNOActor.Shrine_Global_Reloaded:
                    return TrinityPlugin.Weighting.ShrineTypes.RunSpeed;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Enlightened:
                case (int)SNOActor.Shrine_Global_Enlightened:
                    return TrinityPlugin.Weighting.ShrineTypes.Enlightened;

                case (int)SNOActor.Shrine_Global_Glow:
                    return TrinityPlugin.Weighting.ShrineTypes.Glow;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Hoarder:
                case (int)SNOActor.Shrine_Global_Hoarder:
                    return TrinityPlugin.Weighting.ShrineTypes.Hoarder;

                case (int)SNOActor.x1_LR_Shrine_Infinite_Casting:
                    return TrinityPlugin.Weighting.ShrineTypes.Casting;

                case (int)SNOActor.x1_LR_Shrine_Electrified_TieredRift:
                case (int)SNOActor.x1_LR_Shrine_Electrified:
                    return TrinityPlugin.Weighting.ShrineTypes.Conduit;

                case (int)SNOActor.x1_LR_Shrine_Invulnerable:
                    return TrinityPlugin.Weighting.ShrineTypes.Shield;

                case (int)SNOActor.x1_LR_Shrine_Run_Speed:
                    return TrinityPlugin.Weighting.ShrineTypes.Shield;

                case (int)SNOActor.x1_LR_Shrine_Damage:
                    return TrinityPlugin.Weighting.ShrineTypes.Damage;
                default:
                    return TrinityPlugin.Weighting.ShrineTypes.Unknown;
            }
        }

        public static ObjectType GetObjectType(IActor actor)
        {
            if (actor == null || actor.CommonData == null)
                return ObjectType.Unknown;

            return GetObjectType(actor.DiaObject.CommonData, actor.DiaObject.ActorType, actor.ActorSNO, actor.DiaObject.CommonData.GizmoType, actor.InternalName);
        }

        public static ObjectType GetObjectType(ACD acd, ActorType actorType, int actorSNO, GizmoType gizmoType, string internalName)
        {
            if (actorType != ActorType.ClientEffect && !acd.IsValid)
                return ObjectType.Unknown;

            if (DataDictionary.ObjectTypeOverrides.ContainsKey(actorSNO))
                return DataDictionary.ObjectTypeOverrides[actorSNO];

            if (DataDictionary.CursedChestSNO.Contains(actorSNO))
                return ObjectType.CursedChest;

            if (DataDictionary.CursedShrineSNO.Contains(actorSNO))
                return ObjectType.CursedShrine;

            if (DataDictionary.ShrineSNO.Contains(actorSNO))
                return ObjectType.Shrine;

            if (DataDictionary.HealthGlobeSNO.Contains(actorSNO))
                return ObjectType.HealthGlobe;

            if (DataDictionary.PowerGlobeSNO.Contains(actorSNO))
                return ObjectType.PowerGlobe;

            if (DataDictionary.ProgressionGlobeSNO.Contains(actorSNO))
                return ObjectType.ProgressionGlobe;

            if (DataDictionary.GoldSNO.Contains(actorSNO))
                return ObjectType.Gold;

            if (DataDictionary.BloodShardSNO.Contains(actorSNO))
                return ObjectType.BloodShard;

            if (actorType == ActorType.Item || DataDictionary.ForceToItemOverrideIds.Contains(actorSNO))
                return ObjectType.Item;

            //if (DataDictionary.InteractWhiteListIds.Contains(actorSNO))
            //    return ObjectType.Interactable;

            if (DataDictionary.AvoidanceTypeSNO.ContainsKey(actorSNO) || DataDictionary.AvoidanceSNO.Contains(actorSNO))
                return ObjectType.Avoidance;

            if (DataDictionary.ForceTypeAsBarricade.Contains(actorSNO))
                return ObjectType.Barricade;

            if (actorType == ActorType.Monster)
                return ObjectType.Unit;

            if (actorType == ActorType.Gizmo)
            {
                switch (gizmoType)
                {
                    case GizmoType.HealingWell:
                        return ObjectType.HealthWell;

                    case GizmoType.Door:
                        return ObjectType.Door;

                    case GizmoType.BreakableDoor:
                        return ObjectType.Barricade;

                    case GizmoType.PoolOfReflection:
                    case GizmoType.PowerUp:
                        return ObjectType.Shrine;

                    case GizmoType.Chest:
                        return ObjectType.Container;

                    case GizmoType.DestroyableObject:
                    case GizmoType.BreakableChest:
                        return ObjectType.Destructible;

                    case GizmoType.PlacedLoot:
                    case GizmoType.Switch:
                    case GizmoType.Headstone:
                        return ObjectType.Interactable;

                    case GizmoType.Portal:
                        return ObjectType.Portal;
                }
            }

            if (actorType == ActorType.Environment || actorType == ActorType.Critter || actorType == ActorType.ServerProp)
                return ObjectType.Environment;

            if (actorType == ActorType.Projectile)
                return ObjectType.Projectile;

            if (actorType == ActorType.ClientEffect)
                return ObjectType.Effect;

            if (actorType == ActorType.Player)
                return ObjectType.Player;

            if (DataDictionary.PlayerBannerSNO.Contains(actorSNO))
                return ObjectType.Banner;

            if (internalName != null && internalName.StartsWith("Waypoint-"))
                return ObjectType.Waypoint;

            return ObjectType.Unknown;
        }

        public static TrinityItemType GetItemType(ACDItem item)
        {
            //return GetItemType(item.InternalName, item.ItemType, item.FollowerSpecialType);

            if (item == null)
                return TrinityItemType.Unknown;

            return GetItemType(item.ItemType, item.IsTwoHand);
        }

        public static TrinityItemType GetItemType(ItemType type, bool isTwoHanded)
        {
            if (isTwoHanded)
            {
                switch (type)
                {
                    case ItemType.Crossbow:
                        return TrinityItemType.TwoHandCrossbow;
                    case ItemType.Axe:
                        return TrinityItemType.TwoHandAxe;
                    case ItemType.Bow:
                        return TrinityItemType.TwoHandBow;
                    case ItemType.Flail:
                        return TrinityItemType.TwoHandFlail;
                    case ItemType.Mace:
                        return TrinityItemType.TwoHandMace;
                    case ItemType.MightyWeapon:
                        return TrinityItemType.TwoHandMighty;
                    case ItemType.Polearm:
                        return TrinityItemType.TwoHandPolearm;
                    case ItemType.Staff:
                        return TrinityItemType.TwoHandStaff;
                    case ItemType.Sword:
                        return TrinityItemType.TwoHandSword;
                }
            }

            if (type == ItemType.CeremonialDagger)
                return TrinityItemType.CeremonialKnife;

            if (type == ItemType.Daibo)
                return TrinityItemType.TwoHandDaibo;

            TrinityItemType trinityType;
            if (Enum.TryParse(type.ToString(), out trinityType))
                return trinityType;

            return TrinityItemType.Unknown;
        }

        internal static TrinityItemBaseType GetItemBaseType(TrinityItemType itemType)
        {
            var itemBaseType = TrinityItemBaseType.Misc;

            // One Handed Weapons
            switch (itemType)
            {
                case TrinityItemType.Axe:
                case TrinityItemType.CeremonialKnife:
                case TrinityItemType.Dagger:
                case TrinityItemType.Flail:
                case TrinityItemType.FistWeapon:
                case TrinityItemType.Mace:
                case TrinityItemType.MightyWeapon:
                case TrinityItemType.Spear:
                case TrinityItemType.Sword:
                case TrinityItemType.Wand:
                    {

                        itemBaseType = TrinityItemBaseType.WeaponOneHand;
                        break;
                    }
                // Two Handed Weapons
                case TrinityItemType.TwoHandDaibo:
                case TrinityItemType.TwoHandMace:
                case TrinityItemType.TwoHandFlail:
                case TrinityItemType.TwoHandMighty:
                case TrinityItemType.TwoHandPolearm:
                case TrinityItemType.TwoHandStaff:
                case TrinityItemType.TwoHandSword:
                case TrinityItemType.TwoHandAxe:
                    {
                        itemBaseType = TrinityItemBaseType.WeaponTwoHand;
                        break;
                    }
                // Ranged Weapons
                case TrinityItemType.TwoHandCrossbow:
                case TrinityItemType.HandCrossbow:
                case TrinityItemType.TwoHandBow:
                    {
                        itemBaseType = TrinityItemBaseType.WeaponRange;
                        break;
                    }
                // Off-hands
                case TrinityItemType.Mojo:
                case TrinityItemType.Orb:
                case TrinityItemType.CrusaderShield:
                case TrinityItemType.Quiver:
                case TrinityItemType.Shield:
                    {
                        itemBaseType = TrinityItemBaseType.Offhand;
                        break;
                    }
                // Armors
                case TrinityItemType.Boots:
                case TrinityItemType.Bracer:
                case TrinityItemType.Chest:
                case TrinityItemType.Cloak:
                case TrinityItemType.Gloves:
                case TrinityItemType.Helm:
                case TrinityItemType.Legs:
                case TrinityItemType.Shoulder:
                case TrinityItemType.SpiritStone:
                case TrinityItemType.VoodooMask:
                case TrinityItemType.WizardHat:
                case TrinityItemType.Belt:
                case TrinityItemType.MightyBelt:
                    {
                        itemBaseType = TrinityItemBaseType.Armor;
                        break;
                    }
                // Jewlery
                case TrinityItemType.Amulet:
                case TrinityItemType.Ring:
                    {
                        itemBaseType = TrinityItemBaseType.Jewelry;
                        break;
                    }
                // Follower Items
                case TrinityItemType.FollowerEnchantress:
                case TrinityItemType.FollowerScoundrel:
                case TrinityItemType.FollowerTemplar:
                    {
                        itemBaseType = TrinityItemBaseType.FollowerItem;
                        break;
                    }
                // Misc Items
                case TrinityItemType.CraftingMaterial:
                case TrinityItemType.CraftTome:
                case TrinityItemType.LootRunKey:
                case TrinityItemType.HoradricRelic:
                case TrinityItemType.SpecialItem:
                case TrinityItemType.CraftingPlan:
                case TrinityItemType.HealthPotion:
                case TrinityItemType.HoradricCache:
                case TrinityItemType.Dye:
                case TrinityItemType.StaffOfHerding:
                case TrinityItemType.InfernalKey:
                case TrinityItemType.ConsumableAddSockets:
                case TrinityItemType.TieredLootrunKey:
                    {
                        itemBaseType = TrinityItemBaseType.Misc;
                        break;
                    }
                // Gems
                case TrinityItemType.Ruby:
                case TrinityItemType.Emerald:
                case TrinityItemType.Topaz:
                case TrinityItemType.Amethyst:
                case TrinityItemType.Diamond:
                    {
                        itemBaseType = TrinityItemBaseType.Gem;
                        break;
                    }
                // Globes
                case TrinityItemType.HealthGlobe:
                    {
                        itemBaseType = TrinityItemBaseType.HealthGlobe;
                        break;
                    }
                case TrinityItemType.PowerGlobe:
                    {
                        itemBaseType = TrinityItemBaseType.PowerGlobe;
                        break;
                    }
                case TrinityItemType.ProgressionGlobe:
                    {
                        itemBaseType = TrinityItemBaseType.ProgressionGlobe;
                        break;
                    }
            }
            return itemBaseType;
        }


    }
}
