//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.ComponentModel;
//using System.Linq; using Trinity.Framework;
//using System.Threading.Tasks;
//using System.Windows.Data;
//using Buddy.Coroutines;
//using Zeta.Bot;
//using Zeta.Bot.Coroutines;
//using Zeta.Bot.Profile;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.Actors.Gizmos;
//using Zeta.Game.Internals.SNO;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("GetItemFromStash")]
//    public class GetItemFromStashTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        private const int SharedStashSNO = 130400;

//        [XmlAttribute("gameBalanceId")]
//        public int GameBalanceId { get; set; }

//        [XmlAttribute("actorId")]
//        public int ActorId { get; set; }

//        [XmlAttribute("stackCount")]
//        [DefaultValue(1)]
//        public int StackCount { get; set; }

//        [XmlAttribute("greaterRiftKey")]
//        public bool GreaterRiftKey { get; set; }

//        [XmlAttribute("maxItems")]
//        public int MaxItems { get; set; }

//        private bool _isDone;
//        public override bool IsDone
//        {
//            get { return _isDone || !IsActiveQuestStep; }
//        }

//        private static Vector3 StashLocation
//        {
//            get
//            {
//                switch (ZetaDia.CurrentLevelAreaSnoId)
//                {
//                    case 19947: // Campaign A1 Hub
//                        return new Vector3(2968.16f, 2789.63f, 23.94531f);
//                    case 332339: // OpenWorld A1 Hub
//                        return new Vector3(388.16f, 509.63f, 23.94531f);
//                    case 168314: // A2 Hub
//                        return new Vector3(323.0558f, 222.7048f, 0f);
//                    case 92945: // A3/A4 Hub
//                        return new Vector3(387.6834f, 382.0295f, 0f);
//                    case 270011: // A5 Hub
//                        return new Vector3(502.8296f, 739.7472f, 2.598635f);
//                    default:
//                        throw new ValueUnavailableException("Unknown LevelArea Id " + ZetaDia.CurrentLevelAreaSnoId);
//                }
//            }
//        }

//        // [202472F0] GizmoType: SharedStash Name: Player_Shared_Stash-2767 ActorSnoId: 130400 Distance: 3.39224 Position: <502.8296, 739.7472, 2.598635> Barracade: False Radius: 7.982353
//        private static DiaGizmo SharedStash
//        {
//            get
//            {
//                return ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>()
//                    .FirstOrDefault(o => o.IsValid && o.ActorInfo.IsValid && o.ActorInfo.GizmoType == GizmoType.SharedStash);
//            }
//        }

//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ret => GetItemFromStashRoutine());
//        }

//        private Func<ACDItem, bool> ItemMatcherFunc
//        {
//            get
//            {
//                return i => i.IsValid && (
//                    i.GameBalanceId == GameBalanceId ||
//                    i.ActorSnoId == ActorId ||
//                    (GreaterRiftKey && i.TieredLootRunKeyLevel > 0 &&
//                     (!QuestToolsSettings.Instance.EnableLimitRiftLevel || i.TieredLootRunKeyLevel <= QuestToolsSettings.Instance.LimitRiftLevel))
//                    );
//            }
//        }

//        private async Task<bool> GetItemFromStashRoutine()
//        {
//            if (!ZetaDia.IsInGame) return false;
//            if (ZetaDia.Globals.IsLoadingWorld) return false;
//            if (!ZetaDia.IsInTown) { _isDone = true; return false; }
//            if (ZetaDia.Me == null) return false;
//            if (!ZetaDia.Me.IsValid) return false;

//            // Validate parameters
//            if (GameBalanceId == 0 && ActorId == 0 && !GreaterRiftKey)
//            {
//                Core.Logger.Error("GetItemFromStash: invalid parameters. Please specify at least gameBalanceId=\"\" or actorId=\"\" with valid ID numbers or set greaterRiftKey=\"True\"");
//                _isDone = true;
//                return true;
//            }

//            var backPackCount = InventoryManager.Backpack.Where(ItemMatcherFunc).Sum(i => i.ItemStackQuantity);

//            // Check to see if we already have the stack in our backpack
//            if (StackCount != 0 && backPackCount >= StackCount)
//            {
//                Core.Logger.Log("Already have {0} items in our backpack (GameBalanceId={1} ActorSnoId={2} GreaterRiftKey={3})", backPackCount, GameBalanceId, ActorId, GreaterRiftKey);
//                _isDone = true;
//                return true;
//            }

//            // Go to Town
//            if (!ZetaDia.IsInTown)
//                await CommonCoroutines.UseTownPortal("Returning to Town to get Item");

//            // Move to Stash
//            if (StashLocation.Distance2D(ZetaDia.Me.Position) > 10f)
//                await CommonCoroutines.MoveAndStop(StashLocation, 10f, "Stash Location");

//            if (StashLocation.Distance2D(ZetaDia.Me.Position) <= 10f && SharedStash == null)
//            {
//                Core.Logger.Error("Shared Stash actor is null!");
//            }

//            // Open Stash
//            if (StashLocation.Distance2D(ZetaDia.Me.Position) <= 10f && SharedStash != null && !UIElements.StashWindow.IsVisible)
//            {
//                Core.Logger.Log("Opening Stash");
//                SharedStash.Interact();
//                await Coroutine.Sleep(500);
//            }

//            if (UIElements.StashWindow.IsVisible)
//            {
//                Core.Logger.Debug("Stash window is visible");
//                var itemList = InventoryManager.StashItems.Where(ItemMatcherFunc).ToList();
//                var firstItem = itemList.FirstOrDefault();

//                // Check to see if we have the item in the stash
//                bool invalidGameBalanceId = false, invalidActorId = false;
//                if (GameBalanceId != 0 && itemList.All(item => item.GameBalanceId != GameBalanceId))
//                {
//                    Core.Logger.Error("Unable to find item in stash with GameBalanceId {0}", GameBalanceId);
//                    invalidGameBalanceId = true;
//                }
//                if (ActorId != 0 && itemList.All(item => item.ActorSnoId != ActorId))
//                {
//                    Core.Logger.Error("Unable to find item in stash with ActorSnoId {0}", ActorId);
//                    invalidActorId = true;
//                }
//                if (firstItem == null || (invalidGameBalanceId && invalidActorId))
//                {
//                    _isDone = true;
//                    return true;
//                }

//                Vector2 freeItemSlot = Helpers.ItemManager.FindValidBackpackLocation(firstItem.IsTwoSquareItem);

//                if (freeItemSlot.X == -1 && freeItemSlot.Y == -1)
//                {
//                    Core.Logger.Log("No free slots to move items to");
//                    _isDone = true;
//                    return true;
//                }

//                var itemCount = 0;

//                while ((StackCount == 0 || StackCount > backPackCount) && (MaxItems <= 0 || itemCount < MaxItems))
//                {
//                    bool highestFirst = QuestToolsSettings.Instance.UseHighestKeystone;

//                    var itemsList = InventoryManager.StashItems.Where(ItemMatcherFunc).ToList();

//                    ACDItem item;
//                    if (GreaterRiftKey && highestFirst)
//                    {
//                        item = itemsList.OrderByDescending(i => i.TieredLootRunKeyLevel)
//                            .ThenBy(i => i.ItemStackQuantity)
//                            .FirstOrDefault();
//                    }
//                    else if (GreaterRiftKey && !highestFirst)
//                    {
//                        item = itemsList.OrderBy(i => i.TieredLootRunKeyLevel)
//                            .ThenBy(i => i.ItemStackQuantity)
//                            .FirstOrDefault();
//                    }
//                    else
//                    {
//                        item = itemsList.OrderBy(i => i.ItemStackQuantity).FirstOrDefault();
//                    }
//                    if (item == null)
//                        break;
//                    Core.Logger.Debug("Withdrawing item {0} from stash {0}", item.Name);
//                    InventoryManager.QuickWithdraw(item);
//                    itemCount++;
//                    await Coroutine.Yield();
//                    backPackCount = InventoryManager.Backpack.Where(ItemMatcherFunc).Sum(i => i.ItemStackQuantity);
//                }

//                if (MaxItems > 0 && itemCount >= MaxItems)
//                {
//                    _isDone = true;
//                    Core.Logger.Log("Have max items of {0} in backpack", itemCount);
//                    return true;
//                }

//                if (backPackCount >= StackCount)
//                {
//                    _isDone = true;
//                    Core.Logger.Log("Have stack count of {0} items in backpack", backPackCount);
//                    return true;
//                }
//            }

//            Core.Logger.Debug("No Action Taken (StackCount={0} backPackCount={1} GameBalanceId={2} ActorId={3} GreaterRiftKey={4}",
//                StackCount,
//                backPackCount,
//                GameBalanceId,
//                ActorId,
//                GreaterRiftKey);
//            return true;
//        }
//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            base.ResetCachedDone();
//        }

//        #region IEnhancedProfileBehavior

//        public void Update()
//        {
//            UpdateBehavior();
//        }

//        public void Start()
//        {
//            OnStart();
//        }

//        public void Done()
//        {
//            _isDone = true;
//        }

//        #endregion
//    }
//}
