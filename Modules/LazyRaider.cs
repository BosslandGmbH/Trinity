using Trinity.Framework;
using System.Windows.Forms;
using Trinity.Framework.Objects;
using Zeta.Bot;


namespace Trinity.Modules
{
    public class LazyRaider : Module
    {
        protected override int UpdateIntervalMs => 250;

        protected override void OnPulse() => PauseWhileMouseDown();

        private void PauseWhileMouseDown()
        {
            if (Core.Settings.Advanced.LazyRaider && !BotMain.IsPaused && MouseLeft())
            {
                BotMain.PauseWhile(MouseLeft);
            }
        }

        private static bool MouseLeft()
        {
            var result = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
            if (result)
            {
                Core.Logger.Log("按下鼠标左键暂停懒人模式");
            }
            return result;
        }
    }
}


