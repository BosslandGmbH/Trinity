//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.Linq; using Trinity.Framework;
//using System.Threading.Tasks;
//using Buddy.Coroutines;
//using QuestTools.Helpers;
//using QuestTools.ProfileTags.Complex;
//using Zeta.Bot;
//using Zeta.Bot.Coroutines;
//using Zeta.Bot.Profile;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.Game.Internals.Actors;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace QuestTools.ProfileTags
//{
//    [XmlElement("CompleteGreaterRift")]
//    public class CompleteGreaterRiftTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        private bool _isDone;
//        private bool _isGemsOnly;
//        public override bool IsDone
//        {
//            get { return _isDone || !IsActiveQuestStep; }
//        }

//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ret => CompleteGreaterRiftRoutine());
//        }

//        public static UIElement VendorDialog { get { return UIElement.FromHash(0x244BD04C84DF92F1); } }
//        public static UIElement UpgradeKeystoneButton { get { return UIElement.FromHash(0x4BDE2D63B5C36134); } }
//        public static UIElement UpgradeGemButton { get { return UIElement.FromHash(0x826E5716E8D4DD05); } }
//        public static UIElement ContinueButton { get { return UIElement.FromHash(0x1A089FAFF3CB6576); } }
//        public static UIElement UpgradeButton { get { return UIElement.FromHash(0xD365EA84F587D2FE); } }
//        public static UIElement VendorCloseButton { get { return UIElement.FromHash(0xF98A8466DE237BD5); } }

//        public async Task<bool> CompleteGreaterRiftRoutine()
//        {
//            if (!GameUI.IsElementVisible(VendorDialog))
//            {
//                Core.Logger.Log("Rift Vendor Dialog is not visible");
//                _isDone = true;
//                return true;
//            }

//            if (GameUI.IsElementVisible(ContinueButton) && ContinueButton.IsVisible && ContinueButton.IsEnabled)
//            {
//                GameUI.SafeClickElement(ContinueButton, "Continue Button");
//                GameUI.SafeClickElement(VendorCloseButton, "Vendor Window Close Button");
//                await Coroutine.Sleep(250);
//                await Coroutine.Yield();
//            }

//            // Upgrade keystones first, if selected
//            if (QuestToolsSettings.Instance.UpgradeKeyStones && await UpgradeKeyStoneTask())
//                return true;

//            // Attempt to upgrade gems with user settings
//            if (!await UpgradeGemsTask(false)) 
//            {
//                // No gems found? Try and upgrade a keystone so we don't get stuck
//                if (!await UpgradeKeyStoneTask())
//                {
//                    // If there are no keystones to upgrade and no user-priority gems available, try and upgrade ANY gem
//                    await UpgradeGemsTask(true);
//                }
//            }

//            return true;
//        }

//        private async Task<bool> UpgradeKeyStoneTask()
//        {
//            if (GameUI.IsElementVisible(UpgradeKeystoneButton) && UpgradeKeystoneButton.IsEnabled && ZetaDia.Me.AttemptUpgradeKeystone())
//            {
//                Core.Logger.Log("Keystone Upgraded");
//                GameUI.SafeClickElement(VendorCloseButton, "Vendor Window Close Button");
//                await Coroutine.Sleep(250);
//                await Coroutine.Yield();
//                return true;
//            }
//            return false;
//        }

//        private async Task<bool> UpgradeGemsTask(bool force = false)
//        {
//            if (VendorDialog.IsVisible && (GameUI.IsElementVisible(UpgradeGemButton) && UpgradeGemButton.IsEnabled) || GameUI.IsElementVisible(UpgradeButton))
//            {
//                bool hasUpgradeableGems = ZetaDia.Actors.GetActorsOfType<ACDItem>()
//                    .Where(item => item.ItemType == ItemType.LegendaryGem).Any(item => GetUpgradeChance(item) > 0.00f);

//                if (!hasUpgradeableGems)
//                {
//                    Core.Logger.Error("No valid gems found to upgrade! Leaving game...");
//                    await CommonCoroutines.UseTownPortal("Unable to upgrade gem - leaving game");
//                    await Coroutine.Sleep(100);
//                    await Coroutine.Yield();
//                    await CommonCoroutines.LeaveGame("Unable to upgrade gem");
//                    _isDone = true;
//                    await Coroutine.Sleep(100);
//                    await Coroutine.Yield();
//                    return true;
//                }
//                float minimumGemChance = force ? 0f : QuestToolsSettings.Instance.MinimumGemChance;

//                List<ACDItem> gems = ZetaDia.Actors.GetActorsOfType<ACDItem>()
//                    .Where(item => item.ItemType == ItemType.LegendaryGem && GetUpgradeChance(item) > 0.00f && GetUpgradeChance(item) >= minimumGemChance)
//                    .OrderByDescending(item => GetUpgradeChance(item))
//                    .ThenByDescending(item => item.JewelRank).ToList();

//                if (gems.Count == 0 && !_isGemsOnly) //No gems that can be upgraded - upgrade keystone
//                {
//                    return false;
//                }

//                if (gems.Count == 0 && _isGemsOnly)
//                {
//                    gems = ZetaDia.Actors.GetActorsOfType<ACDItem>()
//                    .Where(item => item.ItemType == ItemType.LegendaryGem)
//                    .Where(item => !IsMaxLevelGem(item) && GetUpgradeChance(item) > 0.00f)
//                    .OrderByDescending(item => GetUpgradeChance(item))
//                    .ThenByDescending(item => item.JewelRank).ToList();
//                }

                

//                _isGemsOnly = true;

//                int selectedGemId = int.MaxValue;
//                string selectedGemPreference = "";
//                foreach (string gemName in QuestToolsSettings.Instance.GemPriority)
//                {
//                    selectedGemId = GameData.LegendaryGems.FirstOrDefault(kv => kv.Value == gemName).Key;

//                    // Map to known gem type or dynamic priority
//                    if (selectedGemId == int.MaxValue)
//                    {
//                        Core.Logger.Error("Invalid Gem Name: {0}", gemName);
//                        continue;
//                    }

//                    // Equipped Gems
//                    if (selectedGemId == 0)
//                    {
//                        selectedGemPreference = gemName;
//                        if (gems.Any(IsGemEquipped))
//                        {
//                            gems = gems.Where(item => item.InventorySlot == InventorySlot.Socket).ToList();
//                            break;
//                        }
//                    }

//                    // Lowest Rank
//                    if (selectedGemId == 1)
//                    {
//                        selectedGemPreference = gemName;
//                        gems = gems.OrderBy(item => item.JewelRank).ToList();
//                        break;
//                    }

//                    // Highest Rank
//                    if (selectedGemId == 2)
//                    {
//                        selectedGemPreference = gemName;
//                        gems = gems.OrderByDescending(item => item.JewelRank).ToList();
//                        break;
//                    }

//                    // Selected gem
//                    if (gems.Any(i => i.ActorSnoId == selectedGemId))
//                    {
//                        selectedGemPreference = gemName;
//                        if (gems.Any(i => i.ActorSnoId == selectedGemId))
//                        {
//                            gems = gems.Where(i => i.ActorSnoId == selectedGemId).Take(1).ToList();
//                            break;
//                        }

//                    }

//                    // No gem found... skip!
//                }

//                if (selectedGemId < 10)
//                {
//                    Core.Logger.Log("Using gem priority of {0}", selectedGemPreference);
//                }

//                var bestGem = gems.FirstOrDefault();

//                if (bestGem != null && await CommonCoroutines.AttemptUpgradeGem(bestGem))
//                {
//                    await Coroutine.Sleep(250);
//                    GameUI.SafeClickElement(VendorCloseButton, "Vendor Window Close Button");
//                    await Coroutine.Yield();
//                    return true;
//                }
//                else
//                {
//                    /*
//                     * Demonbuddy MAY randomly fail to upgrade the selected gem. This is a workaround, in case we get stuck...
//                     */

//                    var randomGems = ZetaDia.Actors.GetActorsOfType<ACDItem>()
//                                        .Where(item => item.ItemType == ItemType.LegendaryGem)
//                                        .OrderBy(item => item.JewelRank).ToList();

//                    Random random = new Random(DateTime.UtcNow.Millisecond);
//                    int i = random.Next(0, randomGems.Count - 1);

//                    var randomGem = randomGems.ElementAtOrDefault(i);
//                    if (randomGem == null)
//                    {
//                        Core.Logger.Error("Error: No gems found");
//                        return false;
//                    }

//                    Core.Logger.Error("Gem Upgrade failed! Upgrading random Gem {0} ({1}) - {2:##.##}% {3} ", randomGem.Name, randomGem.JewelRank, GetUpgradeChance(randomGem) * 100, IsGemEquipped(randomGem) ? "Equipped" : string.Empty);

//                    if (!await CommonCoroutines.AttemptUpgradeGem(randomGem))
//                    {
//                        Core.Logger.Error("Random gem upgrade also failed. Something... seriously... wrong... ");
//                    }

//                    return true;
//                }
//            }

//            return false;
//        }

//        private bool IsMaxLevelGem(ACDItem item)
//        {
//            if (item.JewelRank != 50)
//                return false;

//            var sno = item.ActorSnoId;
//            return sno == 428355 || sno == 405796 || sno == 405797 || sno == 405803;
//        }

//        public static Func<ACDItem, bool> IsGemEquipped = gem => (gem.InventorySlot == InventorySlot.Socket);
//        public static Func<ACDItem, float> GetUpgradeChance = gem =>
//        {
//            var lootRunLevel = ZetaDia.Actors.Me.InTieredLootRunLevel;
//            var delta = lootRunLevel - gem.JewelRank;

//            if (delta >= 10) return 1f;
//            if (delta <= -15) return 0f; //Diablo3 disables upgrades for -15 levels difference
//            if (delta >= -14 && delta < -7) return 0.01f; // I don't know if this is right :( All the D3 gem calculators are for pre-2.1.2. Please Help!

//            switch (delta)
//            {
//                case 9: return 0.9f;
//                case 8: return 0.8f;
//                case 7: return 0.7f;
//                case 6: return 0.6f;
//                case 5: return 0.6f;
//                case 4: return 0.6f;
//                case 3: return 0.6f;
//                case 2: return 0.6f;
//                case 1: return 0.6f;
//                case 0: return 0.6f;
//                case -1: return 0.3f;
//                case -2: return 0.15f;
//                case -3: return 0.08f;
//                case -4: return 0.04f;
//                case -5: return 0.02f;
//                case -6: return 0.01f;
//                default: return 0f;
//            }
//        };


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
