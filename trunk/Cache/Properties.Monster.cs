using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Cache
{
    /// <summary>
    /// A collection of properties that are specific to monsters
    /// </summary>
    public class MonsterProperties : Properties.IPropertyCollection
    {
        public bool IsBoss { get; set; }
        public MonsterType MonsterType { get; set; }

        private DateTime LastUpdated = DateTime.MinValue;

        public void ApplyTo(TrinityCacheObject obj)
        {
            if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds > 5000)
                RefreshFrom(obj);

            obj.IsBoss = this.IsBoss;
            obj.MonsterType = this.MonsterType;
        }

        public void RefreshFrom(TrinityCacheObject obj)
        {
            if (obj.ActorType != ActorType.Monster)
                return;

            this.IsBoss = obj.CommonData.MonsterQualityLevel == Zeta.Game.Internals.Actors.MonsterQuality.Boss;

            // Jondar gets a special forcing to undead so DB doesnt make him an Ally.
            this.MonsterType = obj.ActorSNO == 86624 ? MonsterType.Undead : obj.CommonData.MonsterInfo.MonsterType;

            LastUpdated = DateTime.UtcNow;
        }
    }
}


