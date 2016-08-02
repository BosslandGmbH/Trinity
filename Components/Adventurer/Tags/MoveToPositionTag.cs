using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("MoveToPosition")]
    public class MoveToPositionTag : ProfileBehavior
    {

        [XmlAttribute("x")]
        [DefaultValue(0)]
        public int X { get; set; }

        [XmlAttribute("y")]
        [DefaultValue(0)]
        public int Y { get; set; }

        [XmlAttribute("z")]
        [DefaultValue(0)]
        public int Z { get; set; }

        private bool _isDone;
        public override bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }

        public override void OnStart()
        {
            Debug.Print("Tag.OnStart");

        }



        public async Task<bool> Routine()
        {
            if (!await NavigationCoroutine.MoveTo(new Vector3(X,Y,Z), 5)) return true;
            _isDone = true;
            return true;
        }



        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }

    }
}
