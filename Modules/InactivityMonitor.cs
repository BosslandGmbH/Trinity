using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Components.Combat;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Bot;

namespace Trinity.Modules
{
    public class InactivityMonitor : Module
    {
        private long _lastGoldAmount;
        private long _lastXpAmount;
        private DateTime _lastFoundGold = DateTime.MinValue;
        private DateTime _lastFoundXp = DateTime.MinValue;

        protected override void OnPulse()
        {
            if (IsGoldInactive())
            {
                Core.Logger.Warn($"金币闲置时间已到 跳闸");
                LeaveGame();
            }
            if (IsXPInactive())
            {
                Core.Logger.Warn($"经验闲置时间已到 跳闸");
                LeaveGame();
            }
        }

        protected override void OnBotStart()
        {
            ResetGold();
            ResetXp();
        }

        protected override void OnGameJoined()
        {
            ResetGold();
            ResetXp();
        }

        protected override void OnWorldChanged(ChangeEventArgs<int> args)
        {
            ResetGold();
            ResetXp();
        }


        protected override void OnBossEncounterChanged(ChangeEventArgs<bool> args)
        {
            if (args.NewValue)
                ResetXp();
        }

        public void ResetGold()
        {
            _lastGoldAmount = 0;
            _lastFoundGold = DateTime.UtcNow;
        }

        private void ResetXp()
        {
            _lastXpAmount = 0;
            _lastFoundXp = DateTime.UtcNow;
        }

        public bool IsGoldInactive()
        {
            if (Core.Player.ParticipatingInTieredLootRun)
                return false;

            if (Core.Settings.Advanced.DisableAllMovement)
                return false;

            if (!Core.Settings.Advanced.GoldInactivityEnabled)
                return false;

            if (ZetaDia.Me == null || !BotMain.IsRunning || BotMain.IsPausedForStateExecution)
            {
                ResetGold();
                return false;
            }

            try
            {
                if (!ZetaDia.IsInGame)
                {
                    ResetGold();
                    return false;
                }

                if (ZetaDia.Globals.IsLoadingWorld)
                    return false;

                // sometimes bosses take a LONG time
                if (TrinityCombat.Targeting.CurrentTarget != null && TrinityCombat.Targeting.CurrentTarget.IsBoss)
                {
                    Core.Logger.Verbose("当前目标为 Boss, 重置金币统计器");
                    ResetGold();
                    return false;
                }

                if (Core.Player.Coinage != _lastGoldAmount && Core.Player.Coinage != 0)
                {
                    Core.Logger.Verbose(LogCategory.GlobalHandler, "金币变动从 {0} 到 {1}", _lastGoldAmount, Core.Player.Coinage);
                    _lastFoundGold = DateTime.UtcNow;
                    _lastGoldAmount = Core.Player.Coinage;
                }

                int goldUnchangedSeconds = Convert.ToInt32(DateTime.UtcNow.Subtract(_lastFoundGold).TotalSeconds);
                if (goldUnchangedSeconds >= Core.Settings.Advanced.InactivityTimer)
                {
                    Core.Logger.Log("发送金币记录 {0}秒后. (设置={1}) 发送中止.", goldUnchangedSeconds, Core.Settings.Advanced.InactivityTimer);
                    _lastFoundGold = DateTime.UtcNow;
                    _lastGoldAmount = Core.Player.Coinage;
                    return true;
                }
                if (goldUnchangedSeconds > 0)
                {
                    Core.Logger.Log(LogCategory.GlobalHandler, "金币在 {0}秒 没有改变 ", goldUnchangedSeconds);
                }
            }
            catch (Exception e)
            {
                Core.Logger.Log(LogCategory.GlobalHandler, "错误 金币记录: " + e.Message);
            }

            return false;
        }

        public bool IsXPInactive()
        {
            if (!Core.Settings.Advanced.XpInactivityEnabled)
                return false;

            if (Core.Settings.Advanced.DisableAllMovement)
                return false;

            if (ZetaDia.Me == null || !BotMain.IsRunning || BotMain.IsPausedForStateExecution)
            {
                ResetXp();
                return false;
            }

            try
            {
                if (!ZetaDia.IsInGame)
                {
                    ResetXp();
                    return false;
                }
                if (ZetaDia.Globals.IsLoadingWorld)
                {
                    Core.Logger.Log("加载世界, 重置经验统计器");
                    return false;
                }

                Int64 exp;
                if (Core.Player.Level < 70)
                    exp = ZetaDia.Me.CurrentExperience;
                else
                    exp = ZetaDia.Me.ParagonCurrentExperience;

                if (exp != _lastXpAmount && exp != 0)
                {
                    Core.Logger.Verbose(LogCategory.GlobalHandler, "经验变动从 {0} 到 {1}", _lastXpAmount, exp);
                    _lastFoundXp = DateTime.UtcNow;
                    _lastXpAmount = exp;
                }

                int xpUnchangedSeconds = Convert.ToInt32(DateTime.UtcNow.Subtract(_lastFoundXp).TotalSeconds);
                if (xpUnchangedSeconds >= Core.Settings.Advanced.InactivityTimer)
                {
                    Core.Logger.Log("经验记录 {0}秒后. 发送中止.", xpUnchangedSeconds);
                    _lastFoundXp = DateTime.UtcNow;
                    _lastXpAmount = Core.Player.Coinage;
                    return true;
                }
                if (xpUnchangedSeconds > 0)
                {
                    Core.Logger.Log(LogCategory.GlobalHandler, "在 {0}秒内经验没有变动 ", xpUnchangedSeconds);
                }
            }
            catch (Exception e)
            {
                Core.Logger.Log(LogCategory.GlobalHandler, "错误经验记录: " + e.Message);
            }

            return false;
        }

        private void LeaveGame()
        {
            ResetGold();
            ResetXp();
            ZetaDia.Service.Party.LeaveGame();
            BotMain.PauseWhile(() => ZetaDia.IsInGame);
        }
    }
}
