using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Components.Combat;
using Trinity.Framework.Events;

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

        private void HeroIdOnChanged(ChangeEventArgs<int> args)
        {
            HeroId = args.NewValue;
        }

        private void LootProviderOnChanged(ChangeEventArgs<ILootProvider> args)
        {
            var name = args.NewValue.GetType().Name;
            Core.Logger.Debug($"LootProvider changed to {name}");
            LootProviderName = name;
            IsExternalLootProvider = !(args.NewValue is DefaultLootProvider);
        }

        private void IsInGameOnChanged(ChangeEventArgs<bool> args)
        {
            IsInGame = args.NewValue;
        }
        private void IsRunningOnChanged(ChangeEventArgs<bool> args)
        {
            IsRunning = args.NewValue;
        }

        public bool IsInGame
        {
            get => _isInGame;
            set => SetField(ref _isInGame, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetField(ref _isRunning, value);
        }

        public bool IsExternalLootProvider
        {
            get => _isExternalLootProvider;
            set => SetField(ref _isExternalLootProvider, value);
        }

        public string LootProviderName
        {
            get => _lootProviderName;
            set => SetField(ref _lootProviderName, value);
        }

        public int HeroId
        {
            get => _heroId;
            set => SetField(ref _heroId, value);
        }
    }

}
