using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Zeta.Common;
using Zeta.Game;
using Vector2 = Zeta.Common.Vector2;

namespace Trinity.Modules
{
    /// <summary>
    /// The icons shown on the minimap
    /// </summary>
    public class MinimapCache : Module
    {
        protected override int UpdateIntervalMs => 1000;
        public List<TrinityMinimapIcon> MinimapIcons { get; set; } = new List<TrinityMinimapIcon>();
        public HashSet<int> MinimapIconAcdIds { get; set; } = new HashSet<int>();

        protected override void OnPulse()
        {
            if (ZetaDia.Me == null)
                return;

            var minimap = Core.MemoryModel.Minimap;
            var minimapIcons = new List<TrinityMinimapIcon>();
            var minimapIconAcdIds = new HashSet<int>();
            var playerScreenPosition = minimap.Bounds.Center;
            var playerPosition = ZetaDia.Me.Position;

            foreach (var icon in minimap.Items)
            {
                var acdId = icon.AcdId;
                var screenPosition = icon.ScreenPosition;
                var screenOffset = screenPosition - playerScreenPosition + new Vector2(0, 18);
                var position = playerPosition - Rotate(FlipX(screenOffset, Vector2.Zero), Vector2.Zero, -135).ToVector3();

                minimapIcons.Add(new TrinityMinimapIcon
                {
                    Name = Regex.Replace(icon.Name, @"\{.*?\}(\s|)+", ""),
                    Id = icon.Id,
                    ScreenPosition = screenPosition,
                    ScreenOffset = screenOffset,
                    Position = position,
                    Distance = position.Distance(playerPosition),
                    AcdId = acdId,
                });
                minimapIconAcdIds.Add(acdId);
            }

            MinimapIcons = minimapIcons.OrderBy(i => i.Distance).ToList();
            MinimapIconAcdIds = minimapIconAcdIds;
        }

        private const double DegToRad = Math.PI / 180;

        public static Vector2 Rotate(Vector2 p, Vector2 origin, double degrees)
        {
            var theta = degrees * DegToRad;
            var x = Math.Cos(theta) * (p.X - origin.X) - Math.Sin(theta) * (p.Y - origin.Y) + origin.X;
            var y = Math.Sin(theta) * (p.X - origin.X) + Math.Cos(theta) * (p.Y - origin.Y) + origin.Y;
            return new Vector2((float)x, (float)y);
        }

        public static Vector2 FlipX(Vector2 p, Vector2 origin)
        {
            return new Vector2(origin.X - (p.X - origin.X), p.Y);
        }
    }

    public class TrinityMinimapIcon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector2 ScreenPosition { get; set; }
        public Vector3 Position { get; set; }
        public float Distance { get; set; }
        public int AcdId { get; set; }
        public Vector2 ScreenOffset { get; set; }
        public TrinityActor Actor => Core.Actors.GetActorByAcdId<TrinityActor>(AcdId);
        public override string ToString() => $"{Name}, ScreenPosition={ScreenPosition} AcdId={AcdId}";
    }



}

