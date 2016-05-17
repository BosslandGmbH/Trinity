using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Trinity.DbProvider;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity
{
    public partial class TrinityPlugin
    {
        private static bool RefreshUnit()
        {
            bool addToCache = true;

            var unit = CurrentCacheObject.Object as DiaUnit;

            if (CurrentCacheObject.ActorType != ActorType.Monster || c_diaObject.ACDId == -1 || unit == null)
                return false;


            // Always set this, otherwise we divide by zero later
            CurrentCacheObject.KillRange = CurrentBotKillRange;

            var quality = CurrentCacheObject.CommonData.MonsterQualityLevel;

            // See if this is a boss
            CurrentCacheObject.IsBoss = DataDictionary.BossIds.Contains(CurrentCacheObject.ActorSNO) || CurrentCacheObject.InternalNameLowerCase.Contains("boss") || quality == Zeta.Game.Internals.Actors.MonsterQuality.Boss;

            CurrentCacheObject.IsChampion = quality == Zeta.Game.Internals.Actors.MonsterQuality.Champion;

            if (CurrentCacheObject.IsBoss)
                CurrentCacheObject.KillRange = CurrentCacheObject.RadiusDistance + 10f;

            // hax for Diablo_shadowClone
            c_unit_IsAttackable = true;

            CurrentCacheObject.IsHostile = unit.IsHostile;
            CurrentCacheObject.IsSummoned = CurrentCacheObject.Object.CommonData.SummonedByACDId > 0;
            CurrentCacheObject.AnimationState = CurrentCacheObject.Object.CommonData.AnimationState;

            try
            {
                if (CurrentCacheObject.Unit.Movement.IsValid)
                {
                    //c_IsFacingPlayer = CurrentCacheObject.Unit.IsFacingPlayer;
                    CurrentCacheObject.Rotation = CurrentCacheObject.Unit.Movement.Rotation;
                    CurrentCacheObject.DirectionVector = CurrentCacheObject.Unit.Movement.DirectionVector;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug(LogCategory.CacheManagement, "Error while reading Rotation/Facing: {0}", ex.ToString());
            }


            //string teamIdHash = "teamId.RActorId=" + CurrentCacheObject.RActorGuid + ".ActorSnoId=" + CurrentCacheObject.ActorSNO + ".WorldId=" + Player.WorldID;

            //int teamId;
            //if (!CurrentCacheObject.IsBoss && GenericCache.ContainsKey(teamIdHash))
            //{
            //    teamId = (int)GenericCache.GetObject(teamIdHash).Value;
            //}
            //else
            //{
            //    teamId = CurrentCacheObject.Unit.TeamId;

            //    GenericCache.AddToCache(new GenericCacheObject
            //    {
            //        Key = teamIdHash,
            //        Value = teamId,
            //        Expires = DateTime.UtcNow.AddMinutes(60)
            //    });
            //}

            CacheObjectIsBountyObjective();



            try
            {
                CurrentCacheObject.IsNPC = CurrentCacheObject.Unit.IsNPC;
            }
            catch (Exception)
            {
                Logger.LogDebug("Error refreshing IsNPC");
            }

            try
            {
                CurrentCacheObject.IsSpawning = CurrentCacheObject.IsBoss && !CurrentCacheObject.Unit.IsAttackable;
            }
            catch (Exception)
            {
                Logger.LogDebug("Error refreshing IsSpawning");
            }

            CacheUnitNPCIsOperatable();

            CacheObjectMinimapActive();

            try
            {
                CurrentCacheObject.IsQuestMonster = CurrentCacheObject.Unit.IsQuestMonster;
                if (CurrentCacheObject.IsQuestMonster)
                    CurrentCacheObject.KillRange = CurrentCacheObject.RadiusDistance + 10f;
            }
            catch (Exception ex)
            {
                Logger.LogDebug(LogCategory.CacheManagement, "Error reading IsQuestMonster for Unit sno:{0} raGuid:{1} name:{2} ex:{3}",
                    CurrentCacheObject.ActorSNO, CurrentCacheObject.RActorGuid, CurrentCacheObject.InternalName, ex.Message);
            }

            try
            {
                CurrentCacheObject.IsQuestGiver = CurrentCacheObject.Unit.IsQuestGiver;

                // Interact with quest givers, except when doing town-runs
                if (ZetaDia.CurrentAct == Act.OpenWorld && CurrentCacheObject.IsQuestGiver && !(WantToTownRun || ForceVendorRunASAP || BrainBehavior.IsVendoring))
                {
                    CurrentCacheObject.Type = TrinityObjectType.Interactable;
                    CurrentCacheObject.Type = TrinityObjectType.Interactable;
                    CurrentCacheObject.Radius = c_diaObject.CollisionSphere.Radius;
                    return true;
                }
            }
            catch (Exception)
            {
                Logger.LogDebug("Error refreshing IsQuestGiver");
            }

            var teamId = CurrentCacheObject.TeamId;
            if ((teamId == 1 || teamId == 2 || teamId == 17))
            {
                addToCache = false;
                c_IgnoreSubStep += "IsTeam" + teamId;
                return addToCache;
            }

            /* Always refresh monster type */
            if (CurrentCacheObject.Type != TrinityObjectType.Player && !CurrentCacheObject.IsBoss)
            {
                switch (CurrentCacheObject.MonsterType)
                {
                    case MonsterType.Ally:
                    case MonsterType.Scenery:
                    case MonsterType.Helper:
                    case MonsterType.Team:
                        {
                            addToCache = false;
                            c_IgnoreSubStep = "AllySceneryHelperTeam";
                            return addToCache;
                        }
                }
            }

            // Only set treasure goblins to true *IF* they haven't disabled goblins! Then check the SNO in the goblin hash list!
            c_unit_IsTreasureGoblin = false;
            // Flag this as a treasure goblin *OR* ignore this object altogether if treasure goblins are set to ignore
            if (DataDictionary.GoblinIds.Contains(CurrentCacheObject.ActorSNO) || CurrentCacheObject.InternalNameLowerCase.StartsWith("treasureGoblin") || unit.MonsterInfo.MonsterRace == MonsterRace.TreasureGoblin)
            {
                if (Settings.Combat.Misc.GoblinPriority != 0)
                {
                    c_unit_IsTreasureGoblin = true;
                }
                else
                {
                    addToCache = false;
                    c_IgnoreSubStep = "IgnoreTreasureGoblins";
                    return addToCache;
                }
            }

            // Pull up the Monster Affix cached data
            RefreshAffixes();
            if (c_MonsterAffixes.HasFlag(MonsterAffixes.Shielding))
                c_unit_HasShieldAffix = true;

            //[TrinityPlugin 2.14.34] Unit FallenGrunt_A-68856 has MonsterAffix_IllusionistCast (PowerBuff0VisualEffectNone)
            if (CurrentCacheObject.Affixes.Any() && CurrentCacheObject.Affixes.Contains(TrinityMonsterAffix.Illusionist))
            {
                var isIllusion = CurrentCacheObject.CommonData.GetAttribute<int>(((int)SNOPower.MonsterAffix_IllusionistCast << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1;
                if (isIllusion)
                {
                    //Logger.Log("Actor {0} is an illusion! dont be fooled", CurrentCacheObject.InternalName);
                    CurrentCacheObject.IsIllusion = true;
                }
            }



            // Only if at full health, else don't bother checking each loop
            // See if we already have this monster's size stored, if not get it and cache it
            if (!CacheData.MonsterSizes.TryGetValue(CurrentCacheObject.ActorSNO, out c_unit_MonsterSize))
            {
                try
                {
                    RefreshMonsterSize();
                }
                catch
                {
                    Logger.LogDebug("Error refreshing MonsterSize");
                }
            }

            RefreshMonsterHealth();
            
            DebugUtil.LogAnimation(CurrentCacheObject);

            // Unit is already dead
            if ((c_HitPoints <= 0d || CurrentCacheObject.HitPointsPct > 1) && !CurrentCacheObject.IsBoss)
            {
                addToCache = false;
                c_IgnoreSubStep = "0HitPoints";
                return addToCache;
            }

            if (CurrentCacheObject.Unit.IsDead && !DataDictionary.FakeDeathMonsters.Contains(CurrentCacheObject.ActorSNO))
            {
                addToCache = false;
                c_IgnoreSubStep = "IsDead";
                return addToCache;
            }

            if (CurrentCacheObject.IsQuestMonster || CurrentCacheObject.IsBountyObjective)
                return true;

            addToCache = RefreshUnitAttributes(addToCache, CurrentCacheObject.Unit);

            if (!addToCache)
                return addToCache;

            // Set Kill range
            CurrentCacheObject.KillRange = SetKillRange();

            if (CurrentCacheObject.RadiusDistance <= CurrentCacheObject.KillRange)
                AnyMobsInRange = true;

            return addToCache;
        }

        private static void CacheUnitNPCIsOperatable()
        {
            try
            {
                CurrentCacheObject.NPCIsOperable = (c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.NPCIsOperatable) > 0);
            }
            catch (Exception)
            {
                Logger.LogDebug("Error refreshing NPCIsOperable");
            }
        }

        internal static Transform SetVector(Grid ctl)
        {
            return ctl.RenderTransform = new RotateTransform(180, ctl.RenderSize.Width / 2, ctl.RenderSize.Height / 2);
        }

        private static void RefreshMonsterSize()
        {
            SNORecordMonster monsterInfo = CurrentCacheObject.Unit.MonsterInfo;
            if (monsterInfo != null)
            {
                c_unit_MonsterSize = monsterInfo.MonsterSize;
                CacheData.MonsterSizes.Add(CurrentCacheObject.ActorSNO, c_unit_MonsterSize);
            }
            else
            {
                c_unit_MonsterSize = MonsterSize.Unknown;
            }
        }

        private static void RefreshMonsterHealth()
        {
            if (!CurrentCacheObject.Unit.IsValid)
                return;

            // health calculations
            double maxHealth;
            // Get the max health of this unit, a cached version if available, if not cache it
            if (!CacheData.UnitMaxHealth.TryGetValue(CurrentCacheObject.RActorGuid, out maxHealth))
            {
                maxHealth = CurrentCacheObject.Unit.HitpointsMax;
                CacheData.UnitMaxHealth.Add(CurrentCacheObject.RActorGuid, maxHealth);
            }

            // Health calculations            
            c_HitPoints = CurrentCacheObject.Unit.HitpointsCurrent;

            // And finally put the two together for a current health percentage
            c_HitPointsPct = c_HitPoints / maxHealth;
        }

        private static bool RefreshUnitAttributes(bool AddToCache = true, DiaUnit unit = null)
        {


            if (unit.IsUntargetable && !DataDictionary.IgnoreUntargettableAttribute.Contains(CurrentCacheObject.ActorSNO))
            {
                AddToCache = false;
                c_IgnoreSubStep = "IsUntargetable";
                return AddToCache;
            }

            // don't check for invulnerability on shielded and boss units, they are treated seperately
            if (!c_unit_HasShieldAffix && unit.IsInvulnerable)
            {
                AddToCache = false;
                c_IgnoreSubStep = "IsInvulnerable";
                return AddToCache;
            }

            bool isBurrowed;
            if (!CacheData.UnitIsBurrowed.TryGetValue(CurrentCacheObject.RActorGuid, out isBurrowed))
            {
                isBurrowed = unit.IsBurrowed;
                // if the unit is NOT burrowed - we can attack them, add to cache (as IsAttackable)
                if (!isBurrowed)
                {
                    CacheData.UnitIsBurrowed.Add(CurrentCacheObject.RActorGuid, isBurrowed);
                }
            }

            if (isBurrowed)
            {
                AddToCache = false;
                c_IgnoreSubStep = "IsBurrowed";
                return AddToCache;
            }

            // only check for DotDPS/Bleeding in certain conditions to save CPU for everyone else
            // barbs with rend
            // All WD's
            // Monks with Way of the Hundred Fists + Fists of Fury
            if (AddToCache &&
                ((Player.ActorClass == ActorClass.Barbarian && Hotbar.Contains(SNOPower.Barbarian_Rend)) ||
                Player.ActorClass == ActorClass.Witchdoctor ||
                (Player.ActorClass == ActorClass.Monk && CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Monk_WayOfTheHundredFists && s.RuneIndex == 0)))
                )
            {
                bool hasdotDPS = CacheObjectHasDOTDPS();
                c_HasDotDPS = hasdotDPS;
            }
            return AddToCache;

        }

        private static bool CacheObjectHasDOTDPS()
        {
            return c_diaObject.CommonData.GetAttribute<int>(ActorAttributeType.DOTDPS) != 0;
        }
        private static double SetKillRange()
        {
            // Always within kill range if in the NoCheckKillRange list!
            if (DataDictionary.NoCheckKillRange.Contains(CurrentCacheObject.ActorSNO))
                return CurrentCacheObject.RadiusDistance + 100f;

            double killRange;

            // Bosses, always kill
            if (CurrentCacheObject.IsBoss)
            {
                return CurrentCacheObject.RadiusDistance + 100f;
            }

            // Elitey type mobs and things
            if ((c_unit_IsElite || c_unit_IsRare || c_unit_IsUnique))
            {
                // using new GUI slider for elite kill range
                killRange = Settings.Combat.Misc.EliteRange;
            }
            else
            {
                killRange = CurrentBotKillRange;
            }

            if (!Player.IsCastingPortal)
                return killRange;

            // Safety for TownRuns
            if (killRange <= 60) killRange = 60;
            return killRange;
        }
        private static void RefreshAffixes()
        {
            CurrentCacheObject.Affixes = TrinityCacheObject.GetMonsterAffixes(CurrentCacheObject.CommonData.Affixes);

            MonsterAffixes affixFlags;
            if (!CacheData.UnitMonsterAffix.TryGetValue(CurrentCacheObject.RActorGuid, out affixFlags))
            {
                try
                {
                    affixFlags = c_diaObject.CommonData.MonsterAffixes;
                    CacheData.UnitMonsterAffix.Add(CurrentCacheObject.RActorGuid, affixFlags);
                }
                catch (Exception ex)
                {
                    affixFlags = MonsterAffixes.None;
                    Logger.Log(LogCategory.CacheManagement, "Handled Exception getting affixes for Monster SNO={0} Name={1} RAGuid={2}", CurrentCacheObject.ActorSNO, CurrentCacheObject.InternalName, CurrentCacheObject.RActorGuid);
                    Logger.Log(LogCategory.CacheManagement, ex.ToString());
                }
            }

            c_unit_IsElite = affixFlags.HasFlag(MonsterAffixes.Elite);
            c_unit_IsRare = affixFlags.HasFlag(MonsterAffixes.Rare);
            c_unit_IsUnique = affixFlags.HasFlag(MonsterAffixes.Unique);
            c_unit_IsMinion = affixFlags.HasFlag(MonsterAffixes.Minion);
            // All-in-one flag for quicker if checks throughout
            c_IsEliteRareUnique = (c_unit_IsElite || c_unit_IsRare || c_unit_IsUnique || c_unit_IsMinion);

            c_MonsterAffixes = affixFlags;
        }

        private static MonsterType RefreshMonsterType(bool addToDictionary)
        {
            SNORecordMonster monsterInfo = c_diaObject.CommonData.MonsterInfo;
            MonsterType monsterType;
            if (monsterInfo != null)
            {
                // Force Jondar as an undead, since Diablo 3 sticks him as a permanent ally
                if (CurrentCacheObject.ActorSNO == 86624)
                {
                    monsterType = MonsterType.Undead;
                }
                else
                {
                    monsterType = monsterInfo.MonsterType;
                }
                // Is this going to be a new dictionary entry, or updating one already existing?
                if (addToDictionary)
                    CacheData.MonsterTypes.Add(CurrentCacheObject.ActorSNO, monsterType);
                else
                    CacheData.MonsterTypes[CurrentCacheObject.ActorSNO] = monsterType;
            }
            else
            {
                monsterType = MonsterType.Undead;
            }
            return monsterType;
        }

        private static bool RefreshStepCachedSummons(DiaUnit unit)
        {
            var sno = CurrentCacheObject.ActorSNO;
            var actorClass = Player.ActorClass;

            if (!CurrentCacheObject.IsUnit || CurrentCacheObject.IsMe)
                return true;

            var isHostile = CurrentCacheObject.TeamId != Player.TeamId && CurrentCacheObject.IsHostile;

            try
            {
                if (!isHostile) // Phalanx Avatars are ActorType.ClientEffect
                {
                    // 0.3ms very slow
                    CurrentCacheObject.SummonedByACDId = unit.SummonedByACDId;
                }
            }
            catch
            {
                // Only part of a ReadProcessMemory or WriteProcessMemory request was completed
            }

            try
            {
                if (isHostile)
                {
                    CurrentCacheObject.IsSummoner = unit.SummonerId > 0;
                }
            }
            catch
            {
                // Only part of a ReadProcessMemory or WriteProcessMemory request was completed 
            }

            if (CurrentCacheObject.SummonedByACDId <= 0)
                return true;

            // SummonedByACDId is not ACDId, it's DynamicID
            if (CurrentCacheObject.SummonedByACDId == Player.MyDynamicID)
            {
                CurrentCacheObject.IsSummonedByPlayer = true;
            }

            // Count up Mystic Allys, gargantuans, and zombies - if the player has those skills
            if (actorClass == ActorClass.Monk)
            {
                if (DataDictionary.MysticAllyIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer)
                    {
                        PlayerOwnedMysticAllyCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }
            }

            // Count up Demon Hunter pets
            else if (actorClass == ActorClass.DemonHunter)
            {
                if (DataDictionary.DemonHunterPetIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer)
                    {
                        PlayerOwnedDHPetsCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }
            }

            // Count up Demon Hunter sentries
            else if (actorClass == ActorClass.DemonHunter)
            {
                if (DataDictionary.DemonHunterSentryIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer && CurrentCacheObject.Distance < 60f)
                    {
                        PlayerOwnedDHSentryCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }
            }
            // Count up Wiz hydras
            else if (actorClass == ActorClass.Wizard)
            {
                if (DataDictionary.WizardHydraIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer && CurrentCacheObject.Distance < 60f)
                    {
                        PlayerOwnedHydraCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }
            }

            else if (actorClass == ActorClass.Witchdoctor)
            {
                if (DataDictionary.SpiderPetIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer && CurrentCacheObject.Distance < 100f)
                    {
                        PlayerOwnedSpiderPetsCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }
                if (DataDictionary.GargantuanIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer)
                    {
                        PlayerOwnedGargantuanCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }

                if (DataDictionary.ZombieDogIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer)
                    {
                        PlayerOwnedZombieDogCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }

                if (DataDictionary.FetishArmyIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer)
                    {
                        PlayerOwnedFetishArmyCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }
            }

            // Count up Barb ancients
            else if (actorClass == ActorClass.Barbarian)
            {
                if (DataDictionary.AncientIds.Contains(sno))
                {
                    if (CurrentCacheObject.IsSummonedByPlayer)
                    {
                        PlayerOwnedAncientCount++;
                        c_IgnoreSubStep = "IsPlayerSummoned";
                    }
                    return false;
                }
            }

            return true;
        }


    }
}
