//using System;
//using Trinity.Config.Combat;
//using Trinity.DbProvider;
//using Trinity.Technicals;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.Actors.Gizmos;
//using Zeta.Game.Internals.SNO;

//namespace Trinity
//{
//    public partial class TrinityPlugin 
//    {
//        private static bool RefreshGizmo()
//        {
//            bool addToCache;

//            c_diaGizmo = c_diaObject as DiaGizmo;
//            if (c_diaGizmo == null)
//                return false;

//            if (!Settings.WorldObject.AllowPlayerResurection && CurrentCacheObject.ActorSNO == DataDictionary.PLAYER_HEADSTONE_SNO)
//            {
//                c_IgnoreSubStep = "IgnoreHeadstones";
//                addToCache = false;
//                return addToCache;
//            }
//            // start as true, then set as false as we go. If nothing matches below, it will return true.
//            addToCache = true;

//            bool openResplendentChest = CurrentCacheObject.InternalNameLowerCase.Contains("chest_rare");

//            // Ignore it if it's not in range yet, except shrines, pools of reflection and resplendent chests if we're opening chests
//            if ((CurrentCacheObject.RadiusDistance > CurrentBotLootRange || CurrentCacheObject.RadiusDistance > 50) && CurrentCacheObject.Type != TrinityObjectType.HealthWell &&
//                CurrentCacheObject.Type != TrinityObjectType.Shrine && CurrentCacheObject.RActorGuid != LastTargetRactorGUID)
//            {
//                addToCache = false;
//                c_IgnoreSubStep = "NotInRange";
//            }

//            // re-add resplendent chests
//            if (openResplendentChest)
//            {
//                addToCache = true;
//                c_IgnoreSubStep = "";
//            }

//            CacheObjectIsBountyObjective();

//            CacheObjectMinimapActive();


//            if (c_diaGizmo != null)
//            {
//                CurrentCacheObject.GizmoType = c_diaGizmo.ActorInfo.GizmoType;
//            }

//            // Anything that's been disabled by a script
//            bool isGizmoDisabledByScript = false;
//            try
//            {
//                isGizmoDisabledByScript = c_diaGizmo.IsGizmoDisabledByScript;
//            }
//            catch
//            {
//                CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                {
//                    ActorSNO = CurrentCacheObject.ActorSNO,
//                    Radius = CurrentCacheObject.Radius,
//                    Position = CurrentCacheObject.Position,
//                    RActorGUID = CurrentCacheObject.RActorGuid,
//                    ObjectType = CurrentCacheObject.Type,
//                });

//                Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
//                    "Safely handled exception getting Gizmo-Disabled-By-Script attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                c_IgnoreSubStep = "isGizmoDisabledByScriptException";
//                addToCache = false;
//            }                       

//            if (isGizmoDisabledByScript)
//            {
//                MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);

//                CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                {
//                    ActorSNO = CurrentCacheObject.ActorSNO,
//                    Radius = CurrentCacheObject.Radius,
//                    Position = CurrentCacheObject.Position,
//                    RActorGUID = CurrentCacheObject.RActorGuid,
//                    ObjectType = CurrentCacheObject.Type,
//                });

//                addToCache = false;
//                c_IgnoreSubStep = "GizmoDisabledByScript";
//                return addToCache;

//            }


//            // Anything that's Untargetable
//            bool untargetable = false;
//            try
//            {
//                untargetable = CacheObjectUntargetable() > 0;
//            }
//            catch
//            {

//            }


//            // Anything that's Invulnerable
//            bool invulnerable = false;
//            try
//            {
//                invulnerable = CacheObjectInvulnerable() > 0;
//            }
//            catch
//            {

//            }

//            bool noDamage = false;
//            try
//            {
//                noDamage = CacheObjectNoDamage() > 0;
//            }
//            catch
//            {
//                CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                {
//                    ActorSNO = CurrentCacheObject.ActorSNO,
//                    Radius = CurrentCacheObject.Radius,
//                    Position = CurrentCacheObject.Position,
//                    RActorGUID = CurrentCacheObject.RActorGuid,
//                    ObjectType = CurrentCacheObject.Type,
//                });

//                Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
//                    "Safely handled exception getting NoDamage attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                c_IgnoreSubStep = "NoDamage";
//                addToCache = false;
//            }

//            double minDistance;
//            bool gizmoUsed = false;
//            switch (CurrentCacheObject.Type)
//            {
//                case TrinityObjectType.Door:
//                    {
//                        addToCache = true;

//                        var gizmoDoor = c_diaObject as GizmoDoor;
//                        try
//                        {
//                            if (gizmoDoor != null && gizmoDoor.IsLocked)
//                            {
//                                MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                                CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                                {
//                                    ActorSNO = CurrentCacheObject.ActorSNO,
//                                    Radius = CurrentCacheObject.Radius,
//                                    Position = CurrentCacheObject.Position,
//                                    RActorGUID = CurrentCacheObject.RActorGuid,
//                                    ObjectType = CurrentCacheObject.Type,
//                                });

//                                c_IgnoreSubStep = "IsLocked";
//                                return false;
//                            }
//                        }
//                        catch
//                        {
//                            Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception getting gizmoDoor.IsLocked attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                        }

//                        if (c_diaObject is DiaGizmo && ((DiaGizmo)c_diaObject).HasBeenOperated)
//                        {
//                            if(CurrentCacheObject.ActorSNO == 108466) // trOut_OldTristram_Exit_Gate-1866 (108466) 
//                                AddGizmoToNavigationObstacleCache();
                                         
//                            c_IgnoreSubStep = "Door has been operated";
//                            return false;
//                        }

//                        try
//                        {
//                            string currentAnimation = c_diaObject.CommonData.CurrentAnimation.ToString().ToLower();
//                            gizmoUsed = currentAnimation.EndsWith("open") || currentAnimation.EndsWith("opening");

//                            // special hax for A3 Iron Gates
//                            if (currentAnimation.Contains("irongate") && currentAnimation.Contains("open"))
//                                gizmoUsed = false;
//                            if (currentAnimation.Contains("irongate") && currentAnimation.Contains("idle"))
//                                gizmoUsed = true;
//                        }
//                        catch
//                        {
//                            Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception getting gizmoDoor.CurrentAnimation attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                        }

//                        if (gizmoUsed)
//                        {
//                            Blacklist3Seconds.Add(CurrentCacheObject.AnnId);
//                            c_IgnoreSubStep = "Door is Open or Opening";
//                            return false;
//                        }

//                        try
//                        {
//                            int gizmoState = CacheObjectGizmoState();
//                            if (gizmoState == 1)
//                            {
//                                c_IgnoreSubStep = "GizmoState=1";
//                                return false;
//                            }
//                        }
//                        catch
//                        {
//                            c_IgnoreSubStep = "GizmoStateException";
//                            return false;
//                        }

//                        if (untargetable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            c_IgnoreSubStep = "Untargetable";
//                            return false;
//                        }


//                        if (addToCache)
//                        {
//                            try
//                            {
//                                if (c_diaObject is DiaGizmo)
//                                {
//                                    DiaGizmo door = (DiaGizmo)c_diaObject;

//                                    if (door != null && door.IsGizmoDisabledByScript)
//                                    {
//                                        CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                                        {
//                                            ActorSNO = CurrentCacheObject.ActorSNO,
//                                            Radius = CurrentCacheObject.Radius,
//                                            Position = CurrentCacheObject.Position,
//                                            RActorGUID = CurrentCacheObject.RActorGuid,
//                                            ObjectType = CurrentCacheObject.Type,
//                                        });

//                                        Blacklist3Seconds.Add(CurrentCacheObject.AnnId);
//                                        c_IgnoreSubStep = "DoorDisabledbyScript";
//                                        return false;
//                                    }
//                                }
//                                else
//                                {
//                                    c_IgnoreSubStep = "InvalidCastToDoor";
//                                    return false;
//                                }
//                            }

//                            catch
//                            {
//                                Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception getting gizmoDoor.IsGizmoDisabledByScript attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                            }
//                        }
//                    }
//                    break;
//                case TrinityObjectType.Interactable:
//                    {
//                        addToCache = true;

//                        if (untargetable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            c_IgnoreSubStep = "Untargetable";
//                            return false;
//                        }


//                        int endAnimation;
//                        if (DataDictionary.InteractEndAnimations.TryGetValue(CurrentCacheObject.ActorSNO, out endAnimation))
//                        {
//                            if (endAnimation == (int)c_diaGizmo.CommonData.CurrentAnimation)
//                            {
//                                c_IgnoreSubStep = "EndAnimation";
//                                return false;
//                            }
//                        }

//                        if (c_diaGizmo.GizmoState == 1)
//                        {
//                            c_IgnoreSubStep = "GizmoState1";
//                            return false;
//                        }

//                        if (c_diaGizmo.HasBeenOperated)
//                        {
//                            c_IgnoreSubStep = "GizmoHasBeenOperated";
//                            return false;
//                        }

//                        CurrentCacheObject.Radius = c_diaObject.CollisionSphere.Radius;
//                    }
//                    break;
//                case TrinityObjectType.HealthWell:
//                    {
//                        addToCache = true;

//                        MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);

//                        try
//                        {
//                            gizmoUsed = (CacheObjectGizmoCharges() <= 0 && CacheObjectGizmoCharges() > 0);
//                        }
//                        catch
//                        {
//                            Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception getting shrine-been-operated attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                        }
//                        try
//                        {
//                            int gizmoState = CacheObjectGizmoState();
//                            if (gizmoState == 1)
//                            {
//                                addToCache = false;
//                                c_IgnoreSubStep = "GizmoState=1";
//                                return addToCache;
//                            }
//                        }
//                        catch
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "GizmoStateException";
//                            return addToCache;
//                        }
//                        if (gizmoUsed)
//                        {
//                            c_IgnoreSubStep = "GizmoCharges";
//                            addToCache = false;
//                            return addToCache;
//                        }
//                        if (untargetable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "Untargetable";
//                            return addToCache;
//                        }

//                    }
//                    break;
//                case TrinityObjectType.CursedShrine:
//                case TrinityObjectType.Shrine:
//                    {
//                        addToCache = true;
//                        // Shrines
//                        // Check if either we want to ignore all shrines
//                        if (!Settings.WorldObject.UseShrine)
//                        {
//                            // We're ignoring all shrines, so blacklist this one
//                            c_IgnoreSubStep = "IgnoreAllShrinesSet";
//                            addToCache = false;
//                            return addToCache;
//                        }

//                        try
//                        {
//                            int gizmoState = CacheObjectGizmoState();
//                            if (gizmoState == 1)
//                            {
//                                addToCache = false;
//                                c_IgnoreSubStep = "GizmoState=1";
//                                return addToCache;
//                            }
//                        }
//                        catch
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "GizmoStateException";
//                            return addToCache;
//                        }

//                        MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, 4f);

//                        if (untargetable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "Untargetable";
//                            return addToCache;
//                        }


//                        // Determine what shrine type it is, and blacklist if the user has disabled it
//                        switch (CurrentCacheObject.ActorSNO)
//                        {
//                            case (int)SNOActor.Shrine_Global_Frenzied:  //Frenzy Shrine
//                                if (!Settings.WorldObject.UseFrenzyShrine)
//                                {
//                                    Blacklist60Seconds.Add(CurrentCacheObject.AnnId);
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    addToCache = false;
//                                }
//                                if (Player.ActorClass == ActorClass.Monk && Settings.Combat.Monk.TROption.HasFlag(TempestRushOption.MovementOnly) && Hotbar.Contains(SNOPower.Monk_TempestRush))
//                                {
//                                    // Frenzy shrines are a huge time sink for monks using Tempest Rush to move, we should ignore them.
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.Shrine_Global_Fortune:  //Fortune Shrine 
//                                if (!Settings.WorldObject.UseFortuneShrine)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.Shrine_Global_Blessed: //Protection Shrine
//                                if (!Settings.WorldObject.UseProtectionShrine)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.Shrine_Global_Reloaded: //Empowered Shrine - Shrine_Global_Reloaded
//                                if (!Settings.WorldObject.UseEmpoweredShrine)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.Shrine_Global_Enlightened:  //Enlightened Shrine - Shrine_Global_Enlightened
//                                if (!Settings.WorldObject.UseEnlightenedShrine)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.Shrine_Global_Hoarder:  //Fleeting Shrine
//                                if (!Settings.WorldObject.UseFleetingShrine)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.x1_LR_Shrine_Infinite_Casting:  //Channeling Pylon - x1_LR_Shrine_Infinite_Casting
//                                if (!Settings.WorldObject.UseChannelingPylon)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.x1_LR_Shrine_Electrified:  //Conduit Pylon - x1_LR_Shrine_Electrified
//                                if (!Settings.WorldObject.UseConduitPylon)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.x1_LR_Shrine_Invulnerable:  //Shield Pylon -x1_LR_Shrine_Invulnerable
//                                if (!Settings.WorldObject.UseShieldPylon)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;

//                            case (int)SNOActor.x1_LR_Shrine_Run_Speed:  //Speed Pylon - x1_LR_Shrine_Run_Speed
//                                if (!Settings.WorldObject.UseSpeedPylon)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;
//                            case (int)SNOActor.x1_LR_Shrine_Damage:  //Power Pylon - x1_LR_Shrine_Damage
//                                if (!Settings.WorldObject.UsePowerPylon)
//                                {
//                                    addToCache = false;
//                                    c_IgnoreSubStep = "IgnoreShrineOption";
//                                    return addToCache;
//                                }
//                                break;
//                        }  //end switch

//                        // Bag it!
//                        CurrentCacheObject.Radius = 4f;
//                        break;
//                    }
//                case TrinityObjectType.Barricade:
//                    {
//                        addToCache = true;

//                        var gizmoDestructible = c_diaObject as GizmoDestructible;
//                        if (gizmoDestructible != null && gizmoDestructible.HitpointsCurrentPct <= 0)
//                        {
//                            c_IgnoreSubStep = "HitPoints0";
//                            return false;
//                        }

//                        if (noDamage)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "NoDamage";
//                            return addToCache;
//                        }
//                        if (untargetable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "Untargetable";
//                            return addToCache;
//                        }


//                        if (invulnerable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "Invulnerable";
//                            return addToCache;
//                        }

//                        //float maxRadiusDistance;

//                        //if (DataDictionary.DestructableObjectRadius.TryGetValue(CurrentCacheObject.ActorSNO, out maxRadiusDistance))
//                        //{
//                        //    if (CurrentCacheObject.RadiusDistance < maxRadiusDistance)
//                        //    {
//                        //        addToCache = true;
//                        //        c_IgnoreSubStep = "DestructableObjectRadius";
//                        //    }
//                        //}

//                        if (DateTime.UtcNow.Subtract(PlayerMover.LastGeneratedStuckPosition).TotalSeconds <= 1)
//                        {
//                            addToCache = true;
//                            c_IgnoreSubStep = "RecentStuck";
//                            break;
//                        }

//                        // Set min distance to user-defined setting
//                        minDistance = Settings.WorldObject.DestructibleRange + CurrentCacheObject.Radius;
//                        if (_forceCloseRangeTarget)
//                            minDistance += 6f;

//                        // This object isn't yet in our destructible desire range
//                        if (minDistance <= 0 || CurrentCacheObject.RadiusDistance > minDistance)
//                        {
//                            c_IgnoreSubStep = "NotInBarricadeRange";
//                            addToCache = false;
//                            return addToCache;
//                        }

//                        break;
//                    }
//                case TrinityObjectType.Destructible:
//                    {
//                        addToCache = true;

//                        var gizmoDestructible = c_diaObject as GizmoDestructible;
//                        try
//                        {
//                            if (gizmoDestructible != null && gizmoDestructible.HitpointsCurrentPct <= 0)
//                            {
//                                c_IgnoreSubStep = "HitPoints0";
//                                return false;
//                            }
//                        }
//                        catch (Exception)
//                        {
//                            Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception getting gizmoDestructible.HitpointsCurrentPct attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                        }

//                        if (noDamage)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "NoDamage";
//                            return addToCache;
//                        }

//                        if (invulnerable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "Invulnerable";
//                            return addToCache;
//                        }
//                        if (untargetable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "Untargetable";
//                            return addToCache;
//                        }


//                        if (Player.ActorClass == ActorClass.Monk && Hotbar.Contains(SNOPower.Monk_TempestRush) && TimeSinceUse(SNOPower.Monk_TempestRush) <= 150)
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "MonkTR";
//                            break;
//                        }

//                        if (Player.ActorClass == ActorClass.Monk && Hotbar.Contains(SNOPower.Monk_SweepingWind) && GetHasBuff(SNOPower.Monk_SweepingWind))
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "MonkSW";
//                            break;
//                        }

//                        if (CurrentCacheObject.IsQuestMonster || CurrentCacheObject.IsMinimapActive)
//                        {
//                            addToCache = true;
//                            c_IgnoreSubStep = "";
//                            break;
//                        }

//                        if (!DataDictionary.ForceDestructibles.Contains(CurrentCacheObject.ActorSNO) && Settings.WorldObject.DestructibleOption == DestructibleIgnoreOption.ForceIgnore)
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "ForceIgnoreDestructibles";
//                            break;
//                        }

//                        if (DateTime.UtcNow.Subtract(PlayerMover.LastGeneratedStuckPosition).TotalSeconds <= 1)
//                        {
//                            addToCache = true;
//                            c_IgnoreSubStep = "";
//                            break;
//                        }

//                        // Set min distance to user-defined setting
//                        minDistance = Settings.WorldObject.DestructibleRange;
//                        if (_forceCloseRangeTarget)
//                            minDistance += 6f;

//                        // Only break destructables if we're stuck and using IgnoreNonBlocking
//                        if (Settings.WorldObject.DestructibleOption == DestructibleIgnoreOption.DestroyAll)
//                        {
//                            minDistance += 12f;
//                            addToCache = true;
//                            c_IgnoreSubStep = "";
//                        }

//                        float maxRadiusDistance;

//                        if (DataDictionary.DestructableObjectRadius.TryGetValue(CurrentCacheObject.ActorSNO, out maxRadiusDistance))
//                        {
//                            if (CurrentCacheObject.RadiusDistance < maxRadiusDistance)
//                            {
//                                addToCache = true;
//                                c_IgnoreSubStep = "";
//                            }
//                        }
//                        // Always add large destructibles within ultra close range
//                        if (!addToCache && CurrentCacheObject.Radius >= 10f && CurrentCacheObject.RadiusDistance <= 5.9f)
//                        {
//                            addToCache = true;
//                            c_IgnoreSubStep = "";
//                            break;
//                        }

//                        // This object isn't yet in our destructible desire range
//                        if (addToCache && (minDistance <= 1 || CurrentCacheObject.RadiusDistance > minDistance) && TrinityPlugin.Player.MovementSpeed >= 1)
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "NotInDestructableRange";
//                        }
//                        if (addToCache && CurrentCacheObject.RadiusDistance <= 2f && DateTime.UtcNow.Subtract(PlayerMover.LastGeneratedStuckPosition).TotalMilliseconds > 500)
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "NotStuck2";
//                        }

//                        // Add if we're standing still and bumping into it
//                        if (CurrentCacheObject.RadiusDistance <= 2f && TrinityPlugin.Player.MovementSpeed <= 0)
//                        {
//                            addToCache = true;
//                            c_IgnoreSubStep = "";
//                        }

//                        if (CurrentCacheObject.RActorGuid == LastTargetRactorGUID)
//                        {
//                            addToCache = true;
//                            c_IgnoreSubStep = "";
//                        }

//                        break;
//                    }
//                case TrinityObjectType.CursedChest:
//                case TrinityObjectType.Container:
//                    {
//                        addToCache = false;

//                        bool isRareChest = CurrentCacheObject.InternalNameLowerCase.Contains("chest_rare") || DataDictionary.ResplendentChestIds.Contains(CurrentCacheObject.ActorSNO);
//                        bool isChest = (!isRareChest && CurrentCacheObject.InternalNameLowerCase.Contains("chest")) ||
//                            DataDictionary.ContainerWhiteListIds.Contains(CurrentCacheObject.ActorSNO); // We know it's a container but this is not a known rare chest
//                        bool isCorpse = CurrentCacheObject.InternalNameLowerCase.Contains("corpse");
//                        bool isWeaponRack = CurrentCacheObject.InternalNameLowerCase.Contains("rack");
//                        bool isGroundClicky = CurrentCacheObject.InternalNameLowerCase.Contains("ground_clicky");

//                        // We want to do some vendoring, so don't open anything new yet
//                        if (ForceVendorRunASAP)
//                        {
//                            addToCache = false;
//                            c_IgnoreSubStep = "ForceVendorRunASAP";
//                        }

//                        MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);

//                        // Already open, blacklist it and don't look at it again
//                        bool chestOpen;
//                        try
//                        {
//                            chestOpen = CacheObjectIsChestOpen() > 0;
//                        }
//                        catch
//                        {
//                            Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception getting container-been-opened attribute for object {0} [{1}]", CurrentCacheObject.InternalName, CurrentCacheObject.ActorSNO);
//                            c_IgnoreSubStep = "ChestOpenException";
//                            addToCache = false;
//                            return addToCache;
//                        }

//                        // Check if chest is open
//                        if (chestOpen)
//                        {
//                            // It's already open!
//                            addToCache = false;
//                            c_IgnoreSubStep = "AlreadyOpen";
//                            return addToCache;
//                        }

//                        if (untargetable)
//                        {
//                            MainGridProvider.AddCellWeightingObstacle(CurrentCacheObject.ActorSNO, CurrentCacheObject.Radius);
//                            CacheData.NavigationObstacles.Add(new CacheObstacleObject
//                            {
//                                ActorSNO = CurrentCacheObject.ActorSNO,
//                                Radius = CurrentCacheObject.Radius,
//                                Position = CurrentCacheObject.Position,
//                                RActorGUID = CurrentCacheObject.RActorGuid,
//                                ObjectType = CurrentCacheObject.Type,
//                            });

//                            addToCache = false;
//                            c_IgnoreSubStep = "Untargetable";
//                            return addToCache;
//                        }

//                        // Resplendent chests have no range check
//                        if (isRareChest && Settings.WorldObject.OpenRareChests)
//                        {
//                            addToCache = true;
//                            return addToCache;
//                        }

//                        // Regular container, check range
//                        if (CurrentCacheObject.RadiusDistance <= Settings.WorldObject.ContainerOpenRange)
//                        {
//                            if (isChest && Settings.WorldObject.OpenChests)
//                                return true;

//                            if (isCorpse && Settings.WorldObject.InspectCorpses)
//                                return true;

//                            if (isGroundClicky && Settings.WorldObject.InspectGroundClicky)
//                                return true;

//                            if (isWeaponRack && Settings.WorldObject.InspectWeaponRacks)
//                                return true;
//                        }

//                        if (CurrentCacheObject.IsQuestMonster)
//                        {
//                            addToCache = true;
//                            return addToCache;
//                        }

//                        if (Settings.WorldObject.OpenAnyContainer)
//                        {
//                            addToCache = true;
//                            return addToCache;
//                        }

//                        if (!isChest && !isCorpse && !isRareChest)
//                        {
//                            c_IgnoreSubStep = "InvalidContainer";
//                        }
//                        else
//                        {
//                            c_IgnoreSubStep = "IgnoreContainer";
//                        }
//                        break;
//                    }
//            }
//            return addToCache;
//        }

//        private static int CacheObjectIsChestOpen()
//        {
//            //return c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.ChestOpen);
//            return c_diaObject.CommonData.ChestOpen;
//        }

//        private static int CacheObjectGizmoCharges()
//        {
//            //return c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.GizmoCharges);
//            return c_diaObject.CommonData.GizmoCharges;
//        }

//        private static int CacheObjectGizmoState()
//        {
//            //return c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.GizmoState);
//            return c_diaObject.CommonData.GizmoState;
//        }

//        private static int CacheObjectNoDamage()
//        {
//            //return c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.NoDamage);
//            return c_diaObject.CommonData.NoDamage;
//        }

//        private static int CacheObjectInvulnerable()
//        {
//            //return c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.Invulnerable);
//            return c_diaObject.CommonData.Invulnerable;
//        }

//        private static int CacheObjectUntargetable()
//        {
//            //return c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable);
//            return c_diaObject.CommonData.Untargetable;
//        }
//    }
//}
