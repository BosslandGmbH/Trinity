using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.KeywardenCoroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.QuestTools;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("Keywardens")]
    public class KeywardensTag : BaseProfileBehavior
    {
        private KeywardenCoroutine _keywardenCoroutine;

        public override async Task<bool> StartTask()
        {
            PluginEvents.CurrentProfileType = ProfileType.Keywarden;

            var keywardenData = GetNext();
            if (keywardenData == null)
            {
                Core.Logger.Log("[Keywardens] Uhm. No eligible keywardens to cook, remaking the game.");
                return true;
            }

            _keywardenCoroutine = new KeywardenCoroutine(keywardenData);
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!await _keywardenCoroutine.GetCoroutine())
                return false;

            _keywardenCoroutine.Dispose();
            _keywardenCoroutine = null;

            var keywardenData = GetNext();
            if (keywardenData != null)
            {
                _keywardenCoroutine = new KeywardenCoroutine(keywardenData);
                return false;
            }

            return true;
        }

        private KeywardenData GetNext()
        {
            Core.Logger.Log("[Keywarden] Current Machine Counts");
            Core.Logger.Log("[Keywarden] Act 1: {0}", KeywardenDataFactory.Items[Act.A1].MachinesCount);
            Core.Logger.Log("[Keywarden] Act 2: {0}", KeywardenDataFactory.Items[Act.A2].MachinesCount);
            Core.Logger.Log("[Keywarden] Act 3: {0}", KeywardenDataFactory.Items[Act.A3].MachinesCount);
            Core.Logger.Log("[Keywarden] Act 4: {0}", KeywardenDataFactory.Items[Act.A4].MachinesCount);
            var averageMachinesCount = KeywardenDataFactory.Items.Values.Average(kwd => kwd.MachinesCount);
            return KeywardenDataFactory.Items.Values.OrderBy(kwd => kwd.MachinesCount).FirstOrDefault(kwd => kwd.IsAlive && kwd.MachinesCount <= averageMachinesCount + 1);
        }
    }
}