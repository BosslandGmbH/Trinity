using System;
using Trinity.Components.Combat;
using Trinity.Framework;
using Trinity.Framework.Helpers;

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

        private GameInfo()
        {
            ChangeEvents.IsInGame.Changed += IsInGameOnChanged;
            ChangeEvents.LootProvider.Changed += LootProviderOnChanged;
            IsInGame = ChangeEvents.IsInGame.Value;
        }

        private void LootProviderOnChanged(ChangeDetectorEventArgs<ILootProvider> args)
        {
            var name = args.NewValue.GetType().Name;
            Logger.Warn($"LootProvider changed to {name}");
            LootProviderName = name;
            IsExternalLootProvider = !(args.NewValue is DefaultLootProvider);
        }

        public bool IsExternalLootProvider { get; set; }
        public string LootProviderName { get; set; }

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
