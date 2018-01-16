using System.ComponentModel;
using Trinity.Components.QuestTools;
using Zeta.Game;
using Zeta.XmlEngine;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;

namespace Trinity.ProfileTags
{
    [XmlElement("UseDeathGate")]
    public class UseDeathGateTag : BaseProfileBehavior
    {
        private ISubroutine _task;

        [XmlAttribute("count")]
        [Description("Number of gates to enter in sequence")]
        [DefaultValue(1)]
        public int Count { get; set; }

        public override async Task<bool> StartTask()
        {
            _task = new MoveThroughDeathGates(QuestId, ZetaDia.Globals.WorldSnoId, Count);
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!_task.IsDone && !await _task.GetCoroutine())
                return false;

            return true;
        }

    }
}
