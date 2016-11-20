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
        private bool _isRunning;
        private string _lootProviderName;
        private bool _isExternalLootProvider;
        private int _heroId;

        private GameInfo()
        {
            ChangeEvents.IsInGame.Changed += IsInGameOnChanged;
            ChangeEvents.LootProvider.Changed += LootProviderOnChanged;
            ChangeEvents.IsRunning.Changed += IsRunningOnChanged;
            ChangeEvents.HeroId.Changed += HeroIdOnChanged;
            IsInGame = ChangeEvents.IsInGame.Value;
        }

        private void HeroIdOnChanged(ChangeDetectorEventArgs<int> args)
        {
            HeroId = args.NewValue;
        }

        private void LootProviderOnChanged(ChangeDetectorEventArgs<ILootProvider> args)
        {
            var name = args.NewValue.GetType().Name;
            Logger.Warn($"LootProvider changed to {name}");
            LootProviderName = name;
            IsExternalLootProvider = !(args.NewValue is DefaultLootProvider);
        }

        private void IsInGameOnChanged(ChangeDetectorEventArgs<bool> args)
        {
            IsInGame = args.NewValue;
        }

        private void IsRunningOnChanged(ChangeDetectorEventArgs<bool> args)
        {
            IsRunning = args.NewValue;
        }

        public bool IsInGame
        {
            get { return _isInGame; }
            set { SetField(ref _isInGame, value); }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetField(ref _isRunning, value); }
        }

        public bool IsExternalLootProvider
        {
            get { return _isExternalLootProvider; }
            set { SetField(ref _isExternalLootProvider, value); }
        }

        public string LootProviderName
        {
            get { return _lootProviderName; }
            set { SetField(ref _lootProviderName, value); }
        }

        public int HeroId
        {
            get { return _heroId; }
            set { SetField(ref _heroId, value); }
        }
    }

}
