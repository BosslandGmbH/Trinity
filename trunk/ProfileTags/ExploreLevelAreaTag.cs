using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("ExploreLevelArea")]
    public class ExploreLevelAreaTag : ProfileBehavior
    {
        private Stopwatch _stopwatch = new Stopwatch();

        [XmlAttribute("levelAreaId")]
        [DefaultValue(0)]
        public int LevelAreaId { get; set; }

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
            _stopwatch.Start();
            //AdvDia.Update(true);
            if (LevelAreaId == 0)
            {
                LevelAreaId = AdvDia.CurrentLevelAreaId;
            }
            Logger.Info("[ExploreLevelArea] Starting to explore {0} ({1})", (SNOLevelArea)LevelAreaId, LevelAreaId);
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Coroutine());
        }

        public async Task<bool> Coroutine()
        {
            if (await ExplorationCoroutine.Explore(new HashSet<int> { LevelAreaId }))
            {
                _isDone = true;
                return true;
            }
            return false;
        }

        public override void OnDone()
        {
            Logger.Info("[ExploreLevelArea] It took {0} ms to explore {1}", _stopwatch.ElapsedMilliseconds, (SNOLevelArea)LevelAreaId);
            base.OnDone();
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }
    }
}
