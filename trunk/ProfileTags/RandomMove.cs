using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = System.Action;

namespace Trinity.ProfileTags
{
    /// <summary>
    /// Move to a random valid location from current position without using navigation or pathfinding.
    /// Uses basic clicking on location to move. Useful for anti-stuck and areas prone to pathfinding issues.
    /// </summary>
    [XmlElement("RandomMove")]
    public class RandomMoveTag : BaseProfileBehavior
    {
        private List<Vector3> _points = new List<Vector3>();
        private DateTime _lastMoving;

        #region XmlAttributes

        [XmlAttribute("distance")]
        [XmlAttribute("radius")]
        [DefaultValue(60)]
        [Description("Distance to move")]
        public int Radius { get; set; }

        [XmlAttribute("points")]
        [DefaultValue(12)]
        [Description("Number of random points to generate")]
        public int Points { get; set; }

        #endregion

        public RandomMoveTag()
        {
            Radius = Radius < 10 ? 60 : Radius;
        }

        public override async Task<bool> StartTask()
        {
            _points = ProfileUtils.GetCirclePoints(Points, Radius, ZetaDia.Me.Position);
            ProfileUtils.RandomShuffle(_points);
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!_points.Any() || ProfileUtils.IsWithinRange(_points.First()))
            {
                Core.Logger.Debug($"Arrived at Destination {_points.First()}");
                return true;
            }

            if (ZetaDia.Me.Movement.IsMoving)
                _lastMoving = DateTime.UtcNow;

            var rayResult = ZetaDia.Physics.Raycast(ZetaDia.Me.Position, _points.First(), NavCellFlags.AllowWalk);
            var stuckResult = DateTime.UtcNow.Subtract(_lastMoving).TotalMilliseconds > 500;

            if (stuckResult || !rayResult)
            {
                Core.Logger.Debug($"Discarded Location {_points.First()} Distance={_points.First().Distance(ZetaDia.Me.Position)} RaycastResult={rayResult} StuckResult={stuckResult} LocationsRemaining={_points.Count - 1}");
                _points.RemoveAt(0);
            }

            if (!_points.Any() || _points.First() == Vector3.Zero)
                return true;

            if (_points.First().Distance(ZetaDia.Me.Position) <= 6f)
            {
                Core.Logger.Debug($"Arrived at Location {_points.First()} Distance={_points.First().Distance(ZetaDia.Me.Position)} RaycastResult={rayResult} StuckResult={stuckResult} LocationsRemaining={_points.Count - 1}");
                _points.RemoveAt(0);
            }

            if (!_points.Any() || _points.First() == Vector3.Zero)
                return true;

            if (ZetaDia.Me.Movement.MoveActor(_points.First()) != 1)
                return true;

            return false;
        }
    }
}