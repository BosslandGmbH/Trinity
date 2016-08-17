using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.KeywardenCoroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("Keywardens")]
    public class KeywardensTag : ProfileBehavior
    {
        private KeywardenCoroutine _keywardenCoroutine;

        private bool _isDone;
        public override bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        public override void OnStart()
        {
            if (!Core.Adventurer.IsEnabled)
            {
                Logger.Error("Plugin is not enabled. Please enable Adventurer and try again.");
                _isDone = true;
                return;
            }

            PluginEvents.CurrentProfileType = ProfileType.Keywarden;
            if (_keywardenCoroutine == null)
            {
                var keywardenData = GetNext();
                if (keywardenData != null)
                {
                    _keywardenCoroutine = new KeywardenCoroutine(keywardenData);
                }
                else
                {
                    Logger.Info("[Keywardens] Uhm. No eligible keywardens to cook, remaking the game.");
                    _isDone = true;
                }
            }
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Coroutine());
        }


        public async Task<bool> Coroutine()
        {
            if (_isDone)
            {
                return true;
            }
            PluginEvents.PulseUpdates();
            if (!await _keywardenCoroutine.GetCoroutine()) return true;
            _keywardenCoroutine.Dispose();
            _keywardenCoroutine = null;
            var keywardenData = GetNext();
            if (keywardenData != null)
            {
                _keywardenCoroutine = new KeywardenCoroutine(keywardenData);
                return false;
            }
            _isDone = true;
            return true;
        }

        private KeywardenData GetNext()
        {
            Logger.Info("[Keywarden] Current Machine Counts");
            Logger.Info("[Keywarden] Act 1: {0}", KeywardenDataFactory.Items[Act.A1].MachinesCount);
            Logger.Info("[Keywarden] Act 2: {0}", KeywardenDataFactory.Items[Act.A2].MachinesCount);
            Logger.Info("[Keywarden] Act 3: {0}", KeywardenDataFactory.Items[Act.A3].MachinesCount);
            Logger.Info("[Keywarden] Act 4: {0}", KeywardenDataFactory.Items[Act.A4].MachinesCount);
            var averageMachinesCount = KeywardenDataFactory.Items.Values.Average(kwd => kwd.MachinesCount);
            return KeywardenDataFactory.Items.Values.OrderBy(kwd => kwd.MachinesCount).FirstOrDefault(kwd => kwd.IsAlive && kwd.MachinesCount <= averageMachinesCount + 1);

        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
            _keywardenCoroutine = null;
        }

    }
}
