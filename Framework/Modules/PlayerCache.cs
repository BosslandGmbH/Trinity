using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Modules
{
    public class PlayerCache : Module
    {
        public SummonInfo Summons = new SummonInfo();
        public TrinityActor Actor { get; set; } = new TrinityActor();
        public int AcdId { get; private set; }
        public int RActorGuid { get; private set; }
        public bool IsIncapacitated { get; private set; }
        public bool IsRooted { get; private set; }
        public bool IsInRift { get; private set; }
        public double CurrentHealthPct { get; private set; }
        public double PrimaryResource { get; private set; }
        public double PrimaryResourcePct { get; private set; }
        public double PrimaryResourceMax { get; private set; }
        public double PrimaryResourceMissing { get; private set; }
        public double SecondaryResource { get; private set; }
        public double SecondaryResourcePct { get; private set; }
        public double SecondaryResourceMax { get; private set; }
        public double SecondaryResourceMissing { get; private set; }
        public float CooldownReductionPct { get; private set; }
        public float ResourceCostReductionPct { get; private set; }
        public Vector3 Position { get; private set; }
        public int MyDynamicID { get; private set; }
        public int Level { get; private set; }
        public ActorClass ActorClass { get; private set; }
        public string BattleTag { get; private set; }
        public int SceneId { get; private set; }
        public int LevelAreaId { get; private set; }
        public double PlayerDamagePerSecond { get; private set; }
        public SceneInfo Scene { get; private set; }
        public int WorldDynamicID { get; private set; }
        public int WorldSnoId { get; private set; }
        public bool IsInGame { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsLoadingWorld { get; private set; }
        public long Coinage { get; private set; }
        public float GoldPickupRadius { get; private set; }
        public bool IsHidden { get; private set; }
        public long CurrentExperience { get; private set; }
        public long ExperienceNextLevel { get; private set; }
        public long ParagonCurrentExperience { get; private set; }
        public long ParagonExperienceNextLevel { get; private set; }
        public float Rotation { get; private set; }
        public Vector2 DirectionVector { get; private set; }
        public float MovementSpeed { get; private set; }
        public bool IsMoving { get; private set; }
        public bool IsGhosted { get; private set; }
        public bool IsInPandemoniumFortress { get; private set; }
        public GameDifficulty GameDifficulty { get; private set; }
        public bool InActiveEvent { get; private set; }
        public bool HasEventInspectionTask { get; private set; }
        public bool ParticipatingInTieredLootRun { get; private set; }
        public bool IsInTown { get; private set; }
        public bool IsInCombat { get; private set; }
        public int BloodShards { get; private set; }
        public bool IsRanged { get; private set; }
        public bool IsValid { get; private set; }
        public int TieredLootRunlevel { get; private set; }
        public int CurrentQuestSNO { get; private set; }
        public int CurrentQuestStep { get; private set; }
        public Act WorldType { get; private set; }
        public int MaxBloodShards { get; private set; }
        public bool IsMaxCriticalChance { get; set; }
        public bool IsTakingDamage { get; set; }
        public float CurrentHealth { get; set; }
        public SNOAnim CurrentAnimation { get; set; }
        public bool IsJailed { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsCasting { get; set; }
        public bool IsCastingPortal { get; set; }
        public bool IsInParty { get; set; }

        public bool IsInventoryLockedForGreaterRift { get; set; }

        public List<float> HealthHistory { get; set; } = new List<float>();

        public class SceneInfo
        {
            public DateTime LastUpdate { get; set; }
            public int SceneId { get; set; }
        }

        internal static DateTime LastSlowUpdate = DateTime.MinValue;
        internal static DateTime LastVerySlowUpdate = DateTime.MinValue;
        internal static DiaActivePlayer _me;

        protected override void OnPulse()
        {
            Update();
        }

        internal void Update()
        {
            using (new PerformanceLogger("UpdateCachedPlayerData"))
            {
                if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds <= 100)
                    return;

                if (!ZetaDia.IsInGame)
                {
                    IsInGame = false;
                    IsValid = false;
                    return;
                }

                if (ZetaDia.IsLoadingWorld)
                {
                    IsLoadingWorld = true;
                    IsValid = false;
                    return;
                }

                _me = ZetaDia.Me;
                if (_me == null || !_me.IsFullyValid())
                {
                    IsValid = false;
                    return;
                }

                try
                {
                    var levelAreaId = ZetaDia.CurrentLevelAreaSnoId;
                    if (levelAreaId != LevelAreaId)
                    {
                        LastChangedLevelAreaId = DateTime.UtcNow;
                        LevelAreaId = levelAreaId;
                    }

                    IsValid = true;
                    IsInGame = true;
                    IsLoadingWorld = false;


                    WorldDynamicID = ZetaDia.WorldId;
                    WorldSnoId = ZetaDia.CurrentWorldSnoId;

                    TrinityPlugin.CurrentWorldDynamicId = WorldDynamicID;
                    TrinityPlugin.CurrentWorldId = WorldSnoId;


                    if (DateTime.UtcNow.Subtract(LastVerySlowUpdate).TotalMilliseconds > 5000)
                        UpdateVerySlowChangingData();

                    if (DateTime.UtcNow.Subtract(LastSlowUpdate).TotalMilliseconds > 1000)
                        UpdateSlowChangingData();

                    UpdateFastChangingData();


                    UpdateActor();


                }
                catch (Exception ex)
                {
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement, "Safely handled exception for grabbing player data.{0}{1}", Environment.NewLine, ex);
                }
            }
        }

        public DateTime LastChangedLevelAreaId { get; set; }

        private void UpdateActor()
        {
            Actor = Core.Actors.Me;
            //Actor = new TrinityActor
            //{
            //    Object = _me,
            //    CommonData = _me.CommonData,
            //    AcdId = this.AcdId,
            //    ActorType = ActorType.Player,
            //    IsHostile = false,
            //    HitPoints = this.CurrentHealth,
            //    HitPointsPct = this.CurrentHealthPct,
            //    Rotation = this.Rotation,
            //    AnnId = this.MyDynamicID,
            //    AnimationState = _me.CommonData.AnimationState,
            //    Animation = _me.CommonData.CurrentAnimation,
            //    InternalName = _me.Name,
            //    Position = Position,
            //};
        }

        internal void UpdateFastChangingData()
        {
            IsInParty = ZetaDia.Service.Party.NumPartyMembers > 1;
            AcdId = _me.ACDId;
            RActorGuid = _me.RActorId;
            LastUpdated = DateTime.UtcNow;
            IsInTown = DataDictionary.TownLevelAreaIds.Contains(LevelAreaId);
            IsInRift = DataDictionary.RiftWorldIds.Contains(WorldSnoId);
            IsDead = _me.IsDead;
            IsIncapacitated = (_me.IsFeared || _me.IsStunned || _me.IsFrozen || _me.IsBlind);
            IsRooted = _me.IsRooted;
            CurrentHealthPct = _me.HitpointsCurrentPct;
            PrimaryResource = _me.CurrentPrimaryResource;
            PrimaryResourcePct = PrimaryResource / PrimaryResourceMax;
            PrimaryResourceMissing = PrimaryResourceMax - PrimaryResource;
            SecondaryResource = _me.CurrentSecondaryResource;
            SecondaryResourcePct = SecondaryResource / SecondaryResourceMax;
            SecondaryResourceMissing = SecondaryResourceMax - SecondaryResource;
            Position = _me.Position;
            Rotation = _me.Movement.Rotation;
            DirectionVector = _me.Movement.DirectionVector;
            MovementSpeed = (float)PlayerMover.GetMovementSpeed(); //_me.Movement.SpeedXY;
            IsMoving = _me.Movement.IsMoving;
            IsInCombat = _me.IsInCombat;
            MaxBloodShards = 500 + ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.HighestSoloRiftLevel) * 10;
            IsMaxCriticalChance = _me.CritPercentBonusUncapped > 0;
            IsJailed = _me.HasDebuff(SNOPower.MonsterAffix_JailerCast);
            IsFrozen = _me.IsFrozen;
            ParticipatingInTieredLootRun = _me.IsParticipatingInTieredLootRun;
            TieredLootRunlevel = _me.InTieredLootRunLevel;
            IsCasting = _me.LoopingAnimationEndTime > 0;
            CurrentAnimation = _me.CommonData.CurrentAnimation;
            IsInventoryLockedForGreaterRift = ZetaDia.CurrentRift.IsStarted && ZetaDia.CurrentRift.Type == RiftType.Greater && !ZetaDia.CurrentRift.IsCompleted;

            Summons = GetPlayerSummonCounts();

            //var direction = ZetaDia.Me.Movement.DirectionVector;
            //         var directionRadians = Math.Atan2(direction.X, direction.Y);
            //var directionDegrees = directionRadians * 180/Math.PI;

            //Logger.LogNormal("Player DirectionVector={0}{1} Radians={2} (DB: {3}) Degrees={4} (DB: {5})",
            //             DirectionVector.X, 
            //             DirectionVector.Y,
            //             directionRadians,
            //             ZetaDia.Me.Movement.Rotation,
            //             directionDegrees,
            //             ZetaDia.Me.Movement.RotationDegrees
            //             );

            var wasCastingPortal = IsCastingPortal;
            IsCastingPortal = IsCasting && wasCastingPortal || IsCastingTownPortalOrTeleport();

            CurrentHealth = _me.HitpointsCurrent;

            HealthHistory.Add(CurrentHealth);
            while (HealthHistory.Count > 5)
                HealthHistory.RemoveAt(0);

            var averageHealth = HealthHistory.Average();
            IsTakingDamage = averageHealth > CurrentHealth;
            if (IsTakingDamage)
                Logger.LogVerbose(LogCategory.Avoidance, "Taking Damage 5TickAvg={0} Current={1}", averageHealth, CurrentHealth);

            // For WD Angry Chicken
            IsHidden = _me.IsHidden;
        }

        private SummonInfo GetPlayerSummonCounts()
        {
            var info = new SummonInfo();

            // todo, create a units only collection in ActorsCache so we dont have to iterate all RActors.

            foreach (var actor in Core.Actors.AllRActors)
            {
                if (!actor.IsSummonedByPlayer)
                    continue;

                var actorSnoId = actor.ActorSnoId;
                var distance = actor.Distance;

                switch (ActorClass)
                {
                    case ActorClass.Monk:
                        if (DataDictionary.MysticAllyIds.Contains(actorSnoId))
                            info.MysticAllyCount++;
                        break;
                    case ActorClass.DemonHunter:
                        if (DataDictionary.DemonHunterPetIds.Contains(actorSnoId))
                            info.DHPetsCount++;
                        if (DataDictionary.DemonHunterSentryIds.Contains(actorSnoId) && distance < 60f)
                            info.DHSentryCount++;
                        break;
                    case ActorClass.Wizard:
                        if (DataDictionary.WizardHydraIds.Contains(actorSnoId) && distance < 60f)
                            info.HydraCount++;
                        break;
                    case ActorClass.Witchdoctor:
                        if (DataDictionary.SpiderPetIds.Contains(actorSnoId) && distance < 100f)
                            info.SpiderPetCount++;
                        if (DataDictionary.GargantuanIds.Contains(actorSnoId))
                            info.GargantuanCount++;
                        if (DataDictionary.ZombieDogIds.Contains(actorSnoId))
                            info.ZombieDogCount++;
                        if (DataDictionary.FetishArmyIds.Contains(actorSnoId))
                            info.FetishArmyCount++;
                        break;
                    case ActorClass.Barbarian:
                        if (DataDictionary.AncientIds.Contains(actorSnoId))
                            info.AncientCount++;
                        break;
                }
            }

            return info;
        }

        internal void UpdateSlowChangingData()
        {
            BloodShards = ZetaDia.PlayerData.BloodshardCount;
            MyDynamicID = _me.CommonData.AnnId;
            CurrentSceneSnoId = ZetaDia.Me.CurrentScene.SceneInfo.SNOId;

            //Zeta.Game.ZetaDia.Me.CommonData.GetAttribute<int>(Zeta.Game.Internals.Actors.ActorAttributeType.TieredLootRunRewardChoiceState) > 0;

            Coinage = ZetaDia.PlayerData.Coinage;
            CurrentExperience = (long)ZetaDia.Me.CurrentExperience;

            IsInPandemoniumFortress = DataDictionary.PandemoniumFortressWorlds.Contains(WorldSnoId) ||
                    DataDictionary.PandemoniumFortressLevelAreaIds.Contains(LevelAreaId);

            if (CurrentHealthPct > 0)
                IsGhosted = _me.CommonData.GetAttribute<int>(ActorAttributeType.Ghosted) > 0;

            if (Core.Settings.Combat.Misc.UseNavMeshTargeting)
                SceneId = _me.SceneId;

            // Step 13 is used when the player needs to go "Inspect the cursed shrine"
            // Step 1 is event in progress, kill stuff
            // Step 2 is event completed
            // Step -1 is not started
            InActiveEvent = ZetaDia.ActInfo.ActiveQuests.Any(q => DataDictionary.EventQuests.Contains(q.QuestSNO) && q.QuestStep != 13);
            HasEventInspectionTask = ZetaDia.ActInfo.ActiveQuests.Any(q => DataDictionary.EventQuests.Contains(q.QuestSNO) && q.QuestStep == 13);

            FreeBackpackSlots = _me.Inventory.NumFreeBackpackSlots;

            WorldType = ZetaDia.WorldType;
            if (WorldType != Act.OpenWorld)
            {
                // Update these only with campaign
                CurrentQuestSNO = ZetaDia.CurrentQuest.QuestSnoId;
                CurrentQuestStep = ZetaDia.CurrentQuest.StepId;
            }

            LastSlowUpdate = DateTime.UtcNow;
        }

        internal void UpdateVerySlowChangingData()
        {
            Level = _me.Level;
            ActorClass = _me.ActorClass;
            BattleTag = FileManager.BattleTagName;
            CooldownReductionPct = ZetaDia.Me.CommonData.GetAttribute<float>(ActorAttributeType.PowerCooldownReductionPercentAll);
            ResourceCostReductionPct = ZetaDia.Me.CommonData.GetAttribute<float>(ActorAttributeType.ResourceCostReductionPercentAll);
            GoldPickupRadius = _me.GoldPickupRadius;
            ExperienceNextLevel = (long)ZetaDia.Me.ExperienceNextLevel;
            //ParagonLevel = ZetaDia.Me.ParagonLevel;
            ParagonCurrentExperience = (long)ZetaDia.Me.ParagonCurrentExperience;
            ParagonExperienceNextLevel = (long)ZetaDia.Me.ParagonExperienceNextLevel;
            //GameDifficulty = ZetaDia.Service.Hero.CurrentDifficulty;
            SecondaryResourceMax = GetMaxSecondaryResource(_me);
            PrimaryResourceMax = _me.MaxPrimaryResource; //GetMaxPrimaryResource(_me);
            TeamId = _me.CommonData.TeamId;
            Radius = _me.CollisionSphere.Radius;
            IsRanged = ActorClass == ActorClass.Witchdoctor || ActorClass == ActorClass.Wizard || ActorClass == ActorClass.DemonHunter;
            ElementImmunity = GetElementImmunity();
            LastVerySlowUpdate = DateTime.UtcNow;
        }

        public HashSet<SNOAnim> ChannelAnimations = new HashSet<SNOAnim>
        {
            SNOAnim.WitchDoctor_Female_HTH_spell_channel, // = 11001,
            SNOAnim.WitchDoctor_Male_HTH_Spell_Channel, // = 11111,
            SNOAnim.Wizard_Female_1HS_Orb_SpellCast_Channel, // = 11196,
            SNOAnim.Wizard_Female_1HS_SpellCast_Channel, // = 11208,
            SNOAnim.Wizard_Female_HTH_Orb_SpellCast_Channel, // = 11254,
            SNOAnim.Wizard_Female_HTH_SpellCast_Channel, // = 11266,
            SNOAnim.Wizard_Female_STF_SpellCast_Channel, // = 11293,
            SNOAnim.Wizard_Male_HTH_SpellCast_Channel_01, // = 11353,
            SNOAnim.Wizard_Female_Archon_cast_Channel_01, // = 108813,
            SNOAnim.WitchDoctor_Female_2HT_spell_channel, // = 144685,
            SNOAnim.WitchDoctor_Female_1HT_spell_channel, // = 144747,
            SNOAnim.Wizard_Female_HTH_SpellCast_OmniChannel_01, // = 148433,
            SNOAnim.Wizard_Male_HTH_SpellCast_OmniChannel_01, // = 159244,
            SNOAnim.Wizard_Male_Archon_cast_Channel_01, // = 169173,
            SNOAnim.Monk_Female_recall_channel, // = 198326,
            SNOAnim.Monk_Male_recall_channel, // = 198329,
            SNOAnim.Barbarian_Female_HTH_Recall_Channel_01, // = 198435,
            SNOAnim.barbarian_male_HTH_Recall_Channel_01, // = 198479,
            SNOAnim.WitchDoctor_Male_recall_channel, // = 198593,
            SNOAnim.WitchDoctor_Female_recall_channel, // = 198661,
            SNOAnim.Demonhunter_Male_HTH_recall_channel, // = 198860,
            SNOAnim.Demonhunter_Female_HTH_recall_channel, // = 198861,
            SNOAnim.Wizard_Male_HTH_recall_channel, // = 198862,
            SNOAnim.Wizard_Female_HTH_recall_channel, // = 198863,
        };

        public bool IsChannelling => ChannelAnimations.Contains(CurrentAnimation);

        //private float GetMaxPrimaryResource(DiaActivePlayer player)
        //{
        //    return player.MaxPrimaryResource;
        //}

        private float GetMaxPrimaryResource(DiaActivePlayer player)
        {
            switch (ActorClass)
            {
                case ActorClass.Wizard:
                    return player.CommonData.GetAttribute<float>(149 | (int)ResourceType.Arcanum << 12) + player.CommonData.GetAttribute<float>(ActorAttributeType.ResourceEffectiveMaxArcanum);
                case ActorClass.Barbarian:
                    return player.MaxPrimaryResource;
                case ActorClass.Monk:
                    return player.CommonData.GetAttribute<float>(149 | (int)ResourceType.Spirit << 12) + player.CommonData.GetAttribute<float>(ActorAttributeType.ResourceMaxBonusSpirit);
                case ActorClass.Crusader:
                    return player.CommonData.GetAttribute<float>(149 | (int)ResourceType.Faith << 12) + player.CommonData.GetAttribute<float>(ActorAttributeType.ResourceMaxBonusFaith);
                case ActorClass.DemonHunter:
                    return player.CommonData.GetAttribute<float>(149 | (int)ResourceType.Hatred << 12) + player.CommonData.GetAttribute<float>(ActorAttributeType.ResourceMaxBonusHatred);
                case ActorClass.Witchdoctor:
                    return player.CommonData.GetAttribute<float>(149 | (int)ResourceType.Mana << 12) + player.CommonData.GetAttribute<float>(ActorAttributeType.ResourceMaxBonusMana);
            }
            return -1;
        }

        public ACDItem EquippedHealthPotion
        {
            get
            {
                //var potionElement = UIElement.FromHash(9033406906766196825);
                //var potionGameBalanceId = ZetaDia.Memory.Read<int>(potionElement.BaseAddress - 0x24);
                try
                {
                    var potionElement = UIElement.FromHash(16768550267251786851);
                    if (potionElement != null && potionElement.IsValid)
                    {
                        var potionAnnId = ZetaDia.Memory.Read<int>(potionElement.BaseAddress - 0x18);
                        return ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(a => a.AnnId == potionAnnId);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Exception finding EquippedHealthPotion {ex}");
                }
                return ZetaDia.Me.Inventory.BaseHealthPotion;
            }
        }

        public HashSet<Element> ElementImmunity = new HashSet<Element>();

        public HashSet<Element> GetElementImmunity()
        {
            var elements = new HashSet<Element>();

            if (Legendary.MarasKaleidoscope.IsEquipped)
                elements.Add(Element.Poison);

            if (Legendary.TheStarOfAzkaranth.IsEquipped)
                elements.Add(Element.Fire);

            if (Legendary.TalismanOfAranoch.IsEquipped)
                elements.Add(Element.Cold);

            if (Legendary.XephirianAmulet.IsEquipped)
                elements.Add(Element.Lightning);

            if (Legendary.CountessJuliasCameo.IsEquipped)
                elements.Add(Element.Arcane);

            if (Sets.BlackthornesBattlegear.IsMaxBonusActive)
            {
                elements.Add(Element.Poison);
                elements.Add(Element.Fire);
                elements.Add(Element.Physical);
            }
            return elements;
        }

        public bool IsCastingTownPortalOrTeleport()
        {
            try
            {
                var commonData = ZetaDia.Me.CommonData;

                if (CheckVisualEffectNoneForPower(commonData, SNOPower.UseStoneOfRecall))
                {
                    Logger.LogVerbose("Player is casting 'UseStoneOfRecall'");
                    return true;
                }

                //if (CheckVisualEffectNoneForPower(commonData, SNOPower.TeleportToPlayer_Cast))
                //{
                //    Logger.LogVerbose("Player is casting 'TeleportToPlayer_Cast'");
                //    return true;
                //}

                if (CheckVisualEffectNoneForPower(commonData, SNOPower.TeleportToWaypoint_Cast))
                {
                    Logger.LogVerbose("Player is casting 'TeleportToWaypoint_Cast'");
                    return true;
                }

                return false;
            }
            catch (Exception) { }
            return false;
        }

        internal bool CheckVisualEffectNoneForPower(ACD commonData, SNOPower power)
        {
            if (commonData.GetAttribute<int>(((int)power << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1)
                return true;

            return false;
        }

        public bool IsCastingOrLoading
        {
            get
            {
                return
                    ZetaDia.Me != null &&
                    ZetaDia.Me.IsValid &&
                    ZetaDia.Me.CommonData != null &&
                    ZetaDia.Me.CommonData.IsValid &&
                    !ZetaDia.Me.IsDead &&
                    (
                        ZetaDia.IsLoadingWorld ||
                        ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting ||
                        ZetaDia.Me.CommonData.AnimationState == AnimationState.Channeling ||
                        ZetaDia.Me.CommonData.AnimationState == AnimationState.Transform ||
                        ZetaDia.Me.CommonData.AnimationState.ToString() == "13"
                    );
            }
        }

        public object FacingAngle { get; set; }
        public int FreeBackpackSlots { get; set; }
        public int TeamId { get; set; }
        public float Radius { get; set; }
        public int CurrentSceneSnoId { get; set; }
        public PlayerAction CurrentAction { get; set; }

        private float GetMaxSecondaryResource(DiaActivePlayer player)
        {
            switch (ActorClass)
            {
                case ActorClass.DemonHunter:
                    return ZetaDia.Me.CommonData.GetAttribute<float>(149 | (int)ResourceType.Discipline << 12) + player.CommonData.GetAttribute<float>(ActorAttributeType.ResourceMaxBonusDiscipline);
            }
            return -1;
        }

        public void Clear()
        {
            LastUpdated = DateTime.MinValue;
            LastSlowUpdate = DateTime.MinValue;
            LastVerySlowUpdate = DateTime.MinValue;
            IsIncapacitated = false;
            IsRooted = false;
            IsInTown = false;
            CurrentHealthPct = 0;
            PrimaryResource = 0;
            PrimaryResourcePct = 0;
            SecondaryResource = 0;
            SecondaryResourcePct = 0;
            Position = Vector3.Zero;
            MyDynamicID = -1;
            Level = -1;
            ActorClass = ActorClass.Invalid;
            BattleTag = String.Empty;
            SceneId = -1;
            LevelAreaId = -1;
            Scene = new SceneInfo()
            {
                SceneId = -1,
                LastUpdate = DateTime.UtcNow
            };

            Summons = new SummonInfo();
        }

        public void ForceUpdate()
        {
            LastUpdated = DateTime.MinValue;
            LastSlowUpdate = DateTime.MinValue;
            LastVerySlowUpdate = DateTime.MinValue;
        }

        public bool IsFacing(Vector3 targetPosition, float arcDegrees = 70f)
        {
            if (DirectionVector != Vector2.Zero)
            {
                Vector3 u = targetPosition - this.Position;
                u.Z = 0f;
                Vector3 v = new Vector3(DirectionVector.X, DirectionVector.Y, 0f);
                bool result = ((MathEx.ToDegrees(Vector3.AngleBetween(u, v)) <= arcDegrees) ? 1 : 0) != 0;
                return result;
            }
            return false;
        }

        public class SummonInfo
        {
            public int MysticAllyCount = 0;
            public int GargantuanCount = 0;
            public int ZombieDogCount = 0;
            public int FetishArmyCount = 0;
            public int DHPetsCount = 0;
            public int DHSentryCount = 0;
            public int HydraCount = 0;
            public int AncientCount = 0;
            public int SpiderPetCount = 0;
        }
    }
}
