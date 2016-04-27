using System;
using System.Windows;
using System.Windows.Threading;
using Buddy.Overlay;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.UI.Overlays
{
    public abstract class OverlayBase : OverlayUIComponent
    {
        private static DispatcherTimer _internalTimer;

        protected OverlayBase(bool isHitTestable) : base(isHitTestable)
        {
            if (_internalTimer == null)
            {
                _internalTimer = new DispatcherTimer();
                _internalTimer.Tick += InternalTimerTick;
                _internalTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                _internalTimer.Start();
            }
        }

        private void InternalTimerTick(object sender, EventArgs e)
        {
            ZetaDia.Overlay.Dispatcher.Invoke(() =>
            {
                OnInternalTimerTick();

                if (BotMain.IsPausedForStateExecution && !IsPaused)
                    OnPauseStarted();

                if (!BotMain.IsPausedForStateExecution && IsPaused)
                    OnPauseStarted();
            });
        }

        public virtual void OnInternalTimerTick()
        {
            //if (ZetaDia.Me == null || ZetaDia.Me.IsDead || IsDialogVisible || ZetaDia.IsLoadingWorld)
            //{
            //    Hide();
            //}
            //else
            //{
            //    Show();
            //}
        }

        public bool IsDialogVisible
        {
            get
            {
                if (ErrorDialog.IsVisible)
                    return true;

                if (UIElements.ConfirmationDialog.IsVisible)
                    return true;

                if (UIElements.DeathMenuDialogMain.IsVisible)
                    return true;

                if (UIElements.ReviveInTownButton.IsVisible)
                    return true;

                if (UIElements.ShopDialogRepairWindow.IsVisible)
                    return true;

                if (UIElements.RiftDialog.IsVisible)
                    return true;

                if (UIElements.VendorDialog.IsVisible)
                    return true;

                return false;
            }
        }
 
        public virtual void OnPauseStarted()
        {
            Hide();
        }

        public virtual void OnPauseEnded()
        {
            Show();
        }

        public virtual void Hide()
        {
            Control.Visibility = Visibility.Collapsed;     
        }

        public virtual void Show()
        {
            Control.Visibility = Visibility.Visible;
        }


        public void UITestMethod()
        {
            
        }

        public bool IsPaused { get; set; }

        public bool ShouldScale { get; set; }

        public bool IsUnscaled { get; set; }
    }
}
