//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Linq; using Trinity.Framework;
//using System.Threading.Tasks;
//using Buddy.Coroutines;
//using Zeta.Bot;
//using Zeta.Bot.Coroutines;
//using Zeta.Bot.Profile.Common;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.Actors.Gizmos;
//using Zeta.Game.Internals.SNO;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//
//using UIElement = Zeta.Game.Internals.UIElement;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("QTOpenRiftWrapper")]
//    public class QTOpenRiftWrapperTag : OpenRiftTag, IEnhancedProfileBehavior
//    {
//        public const int RiftPortalSno = 345935;
//        public const int GreaterRiftPortalSNO = 396751;

//        private bool _isDone;
//        private int _level;

//        public override bool IsDone
//        {
//            get { return _isDone || base.IsDone; }
//        }

//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            base.ResetCachedDone();
//        }

//        [XmlAttribute("level")]
//        public int Level { get; set; }

//        public override void OnStart()
//        {
//            try
//            {
//                Pulsator.OnPulse += Pulsator_OnPulse;
//                if (!ZetaDia.IsInTown)
//                {
//                    _isDone = true;
//                    Core.Logger.Log("Cannot open rift outside of town");
//                    return;
//                }

//                var keyPriorityList = QuestToolsSettings.Instance.RiftKeyPriority;

//                if (keyPriorityList.Count != 2)
//                    throw new ArgumentOutOfRangeException("RiftKeyPriority", "Expected 3 Rift keys, settings are broken?");

//                if (ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Any(i => i.ACDId != 0 && i.IsValid && (i.ActorSnoId == RiftPortalSno || i.ActorSnoId == GreaterRiftPortalSNO)))
//                {
//                    Core.Logger.Log("Rift Portal already open!");
//                    _isDone = true;
//                }

//                Core.Logger.Log("Key Priority list is: {0}", keyPriorityList.Aggregate("", (current, key) => current + key.ToString() + ", "));

//                var maxLevel = ZetaDia.Me.HighestUnlockedRiftLevel;  

//                if (Level > 0)
//                {
                      
//                    _level = Level > maxLevel ? maxLevel : Level;
//                    Core.Logger.Log("Opening Greater Rift ({0}) (Profile Tag Setting)", _level);
//                    return;
//                }

//                if (Level < 0)
//                {
//                    _level = -1;
//                    Core.Logger.Log("Opening Normal Rift (Profile Tag Setting)");
//                    return;
//                }
                    
//                foreach (var keyType in keyPriorityList)
//                {
//                    if (keyType == RiftKeyUsePriority.Greater && HasGreaterRiftKeys && ZetaDia.Me.Level >= 70)
//                    {
//                        var settingsLevel = QuestToolsSettings.Instance.LimitRiftLevel;
//                        _level = settingsLevel > maxLevel ? maxLevel : settingsLevel; 
//                        Core.Logger.Log("Opening Greater Rift ({0}) (QuestTools Setting)", _level);
//                        return;
//                    }

//                    if (keyType == RiftKeyUsePriority.Normal)
//                    {   
//                        Core.Logger.Log("Opening Normal Rift (QuestTools Setting)");
//                        _level = -1;                      
//                        return;
//                    }
//                }

//                Core.Logger.Log("Unable to open rift");
//                _isDone = true;
//            }
//            catch (Exception ex)
//            {
//                Core.Logger.Error("Error in QTOpenRiftWrapper: " + ex);
//            }
//        }


//        void Pulsator_OnPulse(object sender, EventArgs e)
//        {
//            CheckForRiftPortal();
//        }
//        public override void OnDone()
//        {
//            Pulsator.OnPulse -= Pulsator_OnPulse;
//            base.OnDone();
//        }
//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ret => MainCoroutine());
//        }

//        private async Task<bool> MainCoroutine()
//        {
//            if(ZetaDia.Globals.IsLoadingWorld || !ZetaDia.IsInTown)
//            {
//                Core.Logger.Verbose("We're not in town!");
//                _isDone = true;
//                return false;
//            }  

//            if (ZetaDia.Me.IsParticipatingInTieredLootRun)
//            {
//                Core.Logger.Debug("Already in Greater Rift!");
//                _isDone = true;
//                return false;
//            }

//            CheckForRiftPortal();

//            //[22559C94] GizmoType: LootRunSwitch Name: x1_OpenWorld_LootRunObelisk_B-91 ActorSnoId: 364715 Distance: 4.497307 Position: <359.9, 262.766, -0.0996094> Barracade: False Radius: 9.874258

//            var destination = Vector3.Zero;
//            DiaObject actor;

//            switch (ZetaDia.CurrentAct)
//            {
//                case Act.A3:
//                case Act.A4:
//                    destination = new Vector3(463.4105f, 387.2089f, 0.4986931f);
//                    break;
//                case Act.A5:
//                    destination = new Vector3(602.5745f, 751.5975f, 2.620764f);
//                    break;
//                case Act.A1:
//                    destination = new Vector3(372.5257f, 591.0864f, 24.04533f);
//                    break;
//                case Act.A2:
//                    destination = new Vector3(353.7471f, 262.6955f, -0.3242264f);
//                    break;
//                default:
                    
//                    actor = GetObeliskActor();
//                    if (actor != null)
//                    {
//                        destination = actor.Position;
//                    }
//                    else
//                    {
//                        Core.Logger.Error("Unable to find Rift Obelisk");
//                        return false;
//                    }                        
//                    break;
//            }

//            if (ZetaDia.Me.Position.Distance(destination) > 15f)
//            {
//                Core.Logger.Verbose("Starting movement to Rift Obelisk");

//                while (ZetaDia.Me.Position.Distance(destination) > 15f)
//                {
//                    await CommonCoroutines.MoveAndStop(destination, 15f);
//                    await Coroutine.Yield();
//                }

//                Core.Logger.Verbose("Finished movement to Rift Obelisk");
//            }

//            actor = GetObeliskActor();

//            if (actor == null || !actor.IsValid)
//            {
//                Core.Logger.Error("Unable to find Rift Obelisk");
//                return false;                
//            }

//            bool readyToStart = false;
//            for (int j = 0; j < 10; j++)
//            {
//                if (UIElement.FromHash(0x3182F223039F15F0).IsVisible)
//                {
//                    readyToStart = true;
//                    break;
//                }                   
//                Core.Logger.Verbose("Interacting with rift obelisk");
//                actor.Interact();
//                await Coroutine.Sleep(500);
//            }

//            if (readyToStart)
//            {
//                ZetaDia.Me.OpenRift(_level);
//                await Coroutine.Sleep(1000);
//            }
                

//            return false;
//        }

//        private DiaObject GetObeliskActor()
//        {
//            return ZetaDia.Actors.GetActorsOfType<DiaGizmo>().FirstOrDefault(o => o.ActorInfo.GizmoType == GizmoType.LootRunSwitch);
//        }

//        private bool CheckForRiftPortal()
//        {
//            var portals = ZetaDia.Actors.GetActorsOfType<GizmoPortal>(true).Where(p => p.IsValid && p.ActorSnoId == RiftPortalSno);
//            if (portals.Any())
//            {
//                Core.Logger.Debug("Rift portal already open!");
//                _isDone = true;
//                return false;
//            }
//            return true;
//        }

//        private Func<ACDItem, bool> ItemMatcherFunc
//        {
//            get
//            {
//                //ActorId: 408416, Type: Item, Name: Greater Rift Keystone
//                return i => i.IsValid && i.ActorSnoId == 408416;
//            }
//        }

//        public bool HasGreaterRiftKeys
//        {
//            get
//            {
//                var backPackCount = InventoryManager.Backpack.Where(ItemMatcherFunc).Sum(i => i.ItemStackQuantity);
//                var stashCount = InventoryManager.StashItems.Where(ItemMatcherFunc).Sum(i => i.ItemStackQuantity);
//                return stashCount + backPackCount > 0;
//            }
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
