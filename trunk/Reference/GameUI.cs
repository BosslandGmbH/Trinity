using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Reference
{
    public class GameUI
    {
        private const ulong mercenaryOKHash = 1591817666218338490;
        private const ulong conversationSkipHash = 0x942F41B6B5346714;
        private const ulong talkToInteractButton1Hash = 0x8EB3A93FB1E49EB8;
        private const ulong confirmTimedDungeonOKHash = 0xF9E7B8A635A4F725;
        private const ulong genericOKHash = 0x891D21408238D18E;
        private const ulong partyLeaderBossAcceptHash = 0x69B3F61C0F8490B0;
        private const ulong partyFollowerBossAcceptHash = 0xF495983BA9BE450F;
        private const ulong partyFollowerBossDeclineHash = 0xCC56694B0659218D;
        private const ulong potionButtonHash = 0xE1F43DD874E42728;
        private const ulong bountyRewardDialogHash = 0x278249110947CA00;
        private const ulong gamePotionHash = 0xE1F43DD874E42728;
        private const ulong tieredRiftRewardContinueHash = 0xE9F673BF3A02ECD5;
        private const ulong stashBuyNewTabButtonHash = 0x1B876AD677C9080;
        private const ulong salvageAllNormalsButton = 0xCE31A05539BE5710;
        private const ulong salvageAllMagicsButton = 0xD58A34C0A51E3A60;
        private const ulong salvageAllRaresButton = 0x9AA6E1AD644CF239;
        private const ulong reviveAtCorpseHash = 0xE3CBD66296A39588;
        private const ulong reviveAtCheckpointHash = 0xBFAAF48BA9316742;
        private const ulong reviveInTownHash = 0x7A2AF9C0F3045ADA;
        private const ulong riftCompleteOkButton = 0x6DA3168427892076;
        private const ulong patchOKButton = 0x16C4B9DB83655800;
        private const ulong craftArmorButton = 0x5974C40E80F8BF8A;
        private const ulong craftWeaponButton = 0x527144818529B790;
        private const ulong salvageAllButton = 0x984DDA45D47C5DB4;
        private const ulong repairEquippedButton = 0x1B20255565A7737D;
        private const ulong jewelerCraftButton = 0xFBD49BB1863FCB07;
        private const ulong jewelerGemCombineButton = 0x55FE0C0AB6DE0757;
        private const ulong jewelerRemoveGemButton = 0xDC35124EADB6E267;


        public static bool IsBlackSmithWindowOpen
        {
            get
            {


                //[1E96F9C0] Mouseover: 0x5974C40E80F8BF8A, Name: Root.NormalLayer.vendor_dialog_mainPage.craftarmor_dialog.craft_button
                //[1EB4BB80] Last clicked: 0xE062F7B5040F2EC3, Name: Root.NormalLayer.vendor_dialog_mainPage.tab_0
                //[1E996610] Mouseover: 0x527144818529B790, Name: Root.NormalLayer.vendor_dialog_mainPage.craftweapons_dialog.craft_button
                //[1EB4E860] Last clicked: 0xE062F9B5040F3229, Name: Root.NormalLayer.vendor_dialog_mainPage.tab_2
                //[1E8507B0] Mouseover: 0x984DDA45D47C5DB4, Name: Root.NormalLayer.vendor_dialog_mainPage.salvage_dialog.salvage_all_wrapper.salvage_button
                //[1EB504A0] Last clicked: 0xE062F8B5040F3076, Name: Root.NormalLayer.vendor_dialog_mainPage.tab_3
                //[1E91B200] Mouseover: 0x1B20255565A7737D, Name: Root.NormalLayer.vendor_dialog_mainPage.repair_dialog.RepairEquipped

                if (UIElement.FromHash(craftArmorButton).IsVisible)
                    return true;

                if (UIElement.FromHash(craftWeaponButton).IsVisible)
                    return true;

                if (UIElement.FromHash(salvageAllButton).IsVisible)
                    return true;

                if (UIElement.FromHash(repairEquippedButton).IsVisible)
                    return true;

                //[238C79D0] Mouseover: 0x80E63C97B008F590, Name: Root.NormalLayer.vendor_dialog_mainPage.training_dialog
                if (UIElement.FromHash(0x80E63C97B008F590).IsVisible)
                    return true;

                //[239A55B0] Mouseover: 0x42030DD531BB34DD, Name: Root.NormalLayer.vendor_dialog_mainPage.repair_dialog
                if (UIElement.FromHash(0x42030DD531BB34DD).IsVisible)
                    return true;

                //[20CEB160] Mouseover: 0xCB4F3D80015B16F3, Name: Root.NormalLayer.vendor_dialog_mainPage.craftweapons_dialog
                if (UIElement.FromHash(0xCB4F3D80015B16F3).IsVisible)
                    return true;

                //[196DDAE0] Mouseover: 0xF5888ED474488A21, Name: Root.NormalLayer.vendor_dialog_mainPage.craftarmor_dialog
                if (UIElement.FromHash(0xF5888ED474488A21).IsVisible)
                    return true;

                return false;
            }
        }

        public static bool IsJewelerWindowOpen
        {
            get
            {
                //[1EA8B690] Mouseover: 0xFBD49BB1863FCB07, Name: Root.NormalLayer.vendor_dialog_mainPage.craftjewelry_dialog.craft_button
                //[1EB4BB80] Last clicked: 0xE062F7B5040F2EC3, Name: Root.NormalLayer.vendor_dialog_mainPage.tab_0
                //[1E9BD120] Mouseover: 0x55FE0C0AB6DE0757, Name: Root.NormalLayer.vendor_dialog_mainPage.gemcombine_dialog.craft_button
                //[1EB4E860] Last clicked: 0xE062F9B5040F3229, Name: Root.NormalLayer.vendor_dialog_mainPage.tab_2
                //[1EA6E9C0] Mouseover: 0xDC35124EADB6E267, Name: Root.NormalLayer.vendor_dialog_mainPage.removegem_dialog.SocketItem

                if (UIElement.FromHash(jewelerCraftButton).IsVisible)
                    return true;

                if (UIElement.FromHash(jewelerGemCombineButton).IsVisible)
                    return true;

                if (UIElement.FromHash(salvageAllButton).IsVisible)
                    return true;

                //todo- read text on train page to determine if its the jeweller.
                if (UIElements.VendorWindow.IsVisible)
                    return true;

                return false;
            }
        }

        public static bool IsAnyTownWindowOpen
        {
            get
            {
                if (KanaisCubeWindow.IsVisible)
                    return true;

                if (UIElements.VendorWindow.IsVisible)
                    return true;

                if (UIElements.StashWindow.IsVisible)
                    return true;

                return false;
            }
        }

        public static void CloseVendorWindow()
        {
            //Mouseover: 0x109597E125942DA4, Name: Root.NormalLayer.shop_dialog_mainPage.button_exit
            //Mouseover: 0xF98A8466DE237BD5, Name: Root.NormalLayer.vendor_dialog_mainPage.vendor_button_exit
            //Mouseover: 0x617664A4EFBC3AC2, Name: Root.NormalLayer.rift_dialog_mainPage.LayoutRoot.CloseButton
            //Mouseover: 0x368FF8C552241695, Name: Root.NormalLayer.inventory_dialog_mainPage.inventory_button_exit
            //Mouseover: 0xEDE91C4942014134, Name: Root.NormalLayer.SkillPane_main.LayoutRoot.CloseButton
            //Mouseover: 0x368FF8C552241695, Name: Root.NormalLayer.inventory_dialog_mainPage.inventory_button_exit
               
            var el = UIElement.FromHash(0xF98A8466DE237BD5);
            if (el.IsVisible)
            {
                UIElement.FromHash(0xF98A8466DE237BD5).Click();
            }

            el = UIElement.FromHash(0x109597E125942DA4);
            if (el.IsVisible)
            {
                UIElement.FromHash(0x109597E125942DA4).Click();
            }

            el = UIElement.FromHash(0x617664A4EFBC3AC2);
            if (el.IsVisible)
            {
                UIElement.FromHash(0x617664A4EFBC3AC2).Click();
            }

            el = UIElement.FromHash(0x368FF8C552241695);
            if (el.IsVisible)
            {
                UIElement.FromHash(0x368FF8C552241695).Click();
            }
        }

        public static UIElement StashWindow
        {
            get { return UIElements.StashWindow; }
        }

        public static UIElement KanaisCubeWindow
        {
            get { return UIElement.FromHash(0xCF916D15D32769F9); }
        }

        public static UIElement ChinaStoreCloseButton
        {
            get { return UIElement.FromHash(0xCDD29D7F6A61DAD8); }
        }

        public static UIElement CloseCreditsButton
        {
            get { return UIElement.FromHash(0x981391BBDF64B009); }
        }

        public static UIElement PatchOKButton { get { return UIElement.FromHash(patchOKButton); } }

        public static UIElement RiftCompleteOkButton
        {
            get { return UIElement.FromHash(riftCompleteOkButton); }
        }

        public static UIElement StashDialogMainPage
        {
            get { return UIElement.FromHash(0xB83F0423F7247928); }
        }

        public static UIElement StashDialogMainPageTab1
        {
            get { return UIElement.FromHash(0x276522EDF3238841); }
        }

        public static UIElement JoinRiftButton
        {
            get { return UIElement.FromHash(0x42E152B771A6BCC1); }
        }

        public static UIElement ReviveAtCorpseButton
        {
            get { return UIElement.FromHash(0xE3CBD66296A39588); }
        }

        public static UIElement ReviveAtCheckpointButton
        {
            get { return UIElement.FromHash(0xBFAAF48BA9316742); }
        }

        public static UIElement ReviveInTownButton
        {
            get { return UIElement.FromHash(0x7A2AF9C0F3045ADA); }
        }

        public static UIElement SalvageAllNormalsButton
        {
            get { return UIElement.FromHash(salvageAllNormalsButton); }
        }
        public static UIElement SalvageAllMagicsButton
        {
            get { return UIElement.FromHash(salvageAllMagicsButton); }
        }
        public static UIElement SalvageAllRaresButton
        {
            get { return UIElement.FromHash(salvageAllRaresButton); }
        }

        public static UIElement GamePotion
        {
            get { return UIElement.FromHash(gamePotionHash); }
        }

        public static UIElement BountyRewardDialog
        {
            get { return UIElement.FromHash(bountyRewardDialogHash); }
        }

        public static UIElement PotionButton
        {
            get
            {
                return UIElement.FromHash(potionButtonHash);
            }
        }

        public static UIElement ConfirmTimedDungeonOK
        {
            get
            {
                return UIElement.FromHash(confirmTimedDungeonOKHash);
            }
        }

        public static UIElement MercenaryOKButton
        {
            get
            {
                return UIElement.FromHash(mercenaryOKHash);
            }
        }

        public static UIElement ConversationSkipButton
        {
            get
            {
                return UIElement.FromHash(conversationSkipHash);
            }
        }

        public static UIElement PartyLeaderBossAccept
        {
            get
            {
                return UIElement.FromHash(partyLeaderBossAcceptHash);
            }
        }

        public static UIElement PartyFollowerBossAccept
        {
            get
            {
                return UIElement.FromHash(partyFollowerBossAcceptHash);
            }
        }

        public static UIElement GenericOK
        {
            get
            {
                return UIElement.FromHash(genericOKHash);
            }
        }

        public static UIElement TalktoInteractButton1
        {
            get
            {
                return UIElement.FromHash(talkToInteractButton1Hash);
            }
        }

        public static UIElement StashBuyNewTabButton
        {
            get
            {
                return UIElement.FromHash(stashBuyNewTabButtonHash);
            }
        }

        public static UIElement TieredRiftRewardContinueButton
        {
            get
            {
                return UIElement.FromHash(tieredRiftRewardContinueHash);
            }
        }

        public static UIElement PartyFollowerBossDecline
        {
            get
            {
                return UIElement.FromHash(partyFollowerBossDeclineHash);
            }
        }

        public static bool IsElementVisible(UIElement element)
        {
            if (element == null)
                return false;
            if (!element.IsValid)
                return false;
            if (!element.IsVisible)
                return false;

            return true;
        }

        /// <summary>
        /// Checks to see if ZetaDia.Me.IsValid, element is visible, triggers fireWorldTransferStart if needed and clicks the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="fireWorldTransfer"></param>
        /// <returns></returns>
        public static bool SafeClickElement(UIElement element, string name = "", bool fireWorldTransfer = false)
        {
            if (!Core.Player.IsValid)
                return false;

            if (!IsElementVisible(element))
                return false;

            if (fireWorldTransfer)
                GameEvents.FireWorldTransferStart();

            Logger.Log("Clicking UI element {0} ({1})", name, element.BaseAddress);
            Core.StuckHandler.Reset("Clicked UI Element");
            element.Click();
            return true;
        }

        private static DateTime _lastCheckedUiButtons = DateTime.MinValue;
        public static void SafeClickUIButtons()
        {
            var isInGame = Core.Player.IsInGame;

            // These buttons should be clicked with no delay

            if (SafeClickElement(CloseCreditsButton, "Close Credits Button"))
                return;
            if (SafeClickElement(PatchOKButton, "Patch Update OK Button"))
                return;
            if (isInGame && SafeClickElement(ChinaStoreCloseButton, "Closing China Store Window"))
                return;
            if (isInGame && SafeClickElement(BountyRewardDialog, "Bounty Reward Dialog"))
                return;
            if (isInGame && SafeClickElement(ConversationSkipButton, "Conversation Button"))
                return;
            if (isInGame && SafeClickElement(TalktoInteractButton1, "Conversation Button"))
                return;

            if (DateTime.UtcNow.Subtract(_lastCheckedUiButtons).TotalMilliseconds <= 500)
                return;

   
            //Last clicked: 0x4CC93A73A58BAFFF, Name: Root.TopLayer.BattleNetModalNotifications_main.ModalNotification
            //[1E41D860] Mouseover: 0x4CC93A73A58BAFFF, Name: Root.TopLayer.BattleNetModalNotifications_main.ModalNotification
     
            //Last clicked: 0x4CC93A73A58BAFFF, Name: Root.TopLayer.BattleNetModalNotifications_main.ModalNotification
            //[1E048BB0] Mouseover: 0xB4433DA3F648A992, Name: Root.TopLayer.BattleNetModalNotifications_main.ModalNotification.Buttons.ButtonList.OkButton

            if (SafeClickElement(UIElement.FromHash(0xB4433DA3F648A992), "Network Disconnect Error Dialog OK Button"))
                return;

            if (isInGame && SafeClickElement(JoinRiftButton, "Join Rift Accept Button", true))
                return;

            if (isInGame && SafeClickElement(PartyLeaderBossAccept, "Party Leader Boss Accept", true))
                return;


            if (IsElementVisible(PartyFollowerBossDecline))
            {
                var declineInBounty = Core.Settings.Combat.FollowerBossFightDialogMode == FollowerBossFightMode.DeclineInBounty && ZetaDia.ActInfo.ActiveBounty != null;
                var alwaysDecline = Core.Settings.Combat.FollowerBossFightDialogMode == FollowerBossFightMode.AlwaysDecline;

                if (declineInBounty || alwaysDecline)
                {
                    if (isInGame && SafeClickElement(PartyFollowerBossDecline, "Party Follower Boss Decline", true))
                        return;
                }
            }
            else
            {
                if (isInGame && SafeClickElement(PartyFollowerBossAccept, "Party Follower Boss Accept", true))
                    return;
            }

            _lastCheckedUiButtons = DateTime.UtcNow;

            if (Core.Player.IsCasting)
                return;

            if (isInGame && SafeClickElement(MercenaryOKButton, "Mercenary OK Button"))
                return;
            if (SafeClickElement(RiftCompleteOkButton, "Rift Complete OK Button"))
                return;
            if (SafeClickElement(GenericOK, "GenericOK"))
                return;
            if (SafeClickElement(UIElements.ConfirmationDialogOkButton, "ConfirmationDialogOKButton", true))
                return;
            if (isInGame && SafeClickElement(ConfirmTimedDungeonOK, "Confirm Timed Dungeon OK Button", true))
                return;
            if (isInGame && SafeClickElement(StashBuyNewTabButton, "Buying new Stash Tab"))
                return;
            if (isInGame && SafeClickElement(TieredRiftRewardContinueButton, "Tiered Rift Reward Continue Button"))
                return;

        }

        public static bool IsPartyDialogVisible
        {
            get
            {
                return IsElementVisible(PartyFollowerBossAccept) || IsElementVisible(PartyLeaderBossAccept);
            }
        }
    }
}
