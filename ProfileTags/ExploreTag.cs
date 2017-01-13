using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.ProfileTags
{
    [XmlElement("Explore")]
    [XmlElement("ExploreLevelArea")]
    public class ExploreTag : TrinityProfileBehavior
    {
        private Stopwatch _stopwatch = new Stopwatch();

        [XmlAttribute("levelAreaId")]
        [DefaultValue(0)]
        public int LevelAreaId { get; set; }

        [XmlAttribute("stopCondition")]
        public string StopCondition { get; set; }

        private bool _isDone;
        public override bool IsDone => _isDone;

        public override void OnStart()
        {
            _stopwatch.Start();
            if (LevelAreaId == 0)
            {
                Pulsator.OnPulse += OnPulse;
                LevelAreaId = AdvDia.CurrentLevelAreaId;
            }
            base.OnStart();
        }

        private void OnPulse(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(StopCondition))
                return;

            if (ScriptManager.GetCondition(StopCondition).Invoke())
            {
                Logger.Info($"[ExploreLevelArea] Stop condition was met: ({StopCondition})");
                _isDone = true;
            }
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Coroutine());
        }

        public async Task<bool> Coroutine()
        {
            if (_isDone)
                return true;

            if (await ExplorationCoroutine.Explore(new HashSet<int> { LevelAreaId }))
            {
                _isDone = true;
                return true;
            }
            return false;
        }        

        public override void OnDone()
        {
            Pulsator.OnPulse -= OnPulse;
            Logger.Info($"[ExploreLevelArea] It took {_stopwatch.Elapsed.TotalMinutes} minutes to explore {(SNOLevelArea)LevelAreaId}");
            base.OnDone();
        }

        public override void ResetCachedDone(bool force = false)
        {
            _isDone = false;
        }
    }
}
