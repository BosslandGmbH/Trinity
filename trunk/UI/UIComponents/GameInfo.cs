using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Helpers;
using Trinity.Technicals;

namespace Trinity.UI.UIComponents
{
    /// <summary>
    /// Wrapper for common game states and information to be accessed by the UI
    /// </summary>
    public class GameInfo : NotifyBase
    {
        private static GameInfo _instance;
        public static GameInfo Instance = _instance ?? (_instance = new GameInfo());

        private bool _isInGame;

        public GameInfo()
        {
            ChangeEvents.IsInGame.Changed += IsInGameOnChanged;
            IsInGame = ChangeEvents.IsInGame.Value;
        }

        private void IsInGameOnChanged(ChangeDetectorEventArgs<bool> args)
        {
            Logger.Warn($"IsInGame changed to {args.NewValue}");
            IsInGame = args.NewValue;
        }

        public bool IsInGame
        {
            get { return _isInGame; }
            set { SetField(ref _isInGame, value); }
        }
    }

}
