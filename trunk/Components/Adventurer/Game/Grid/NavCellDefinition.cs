using System.Runtime.Serialization;
using Zeta.Common;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Adventurer.Game.Grid
{
    [DataContract]
    public class NavCellDefinition
    {
        [DataMember]
        public Vector3 Min { get; set; }
        [DataMember]
        public Vector3 Max { get; set; }
        [DataMember]
        public NavCellFlags Flags { get; set; }

        private NavCellDefinition() { }

        public static NavCellDefinition Create(NavCell navCell)
        {
            var def = new NavCellDefinition
            {
                Min = navCell.Min,
                Max = navCell.Max,
                Flags = navCell.Flags
            };
            return def;
        }

        public bool IsInCell(float x, float y)
        {
            return x >= Min.X && x <= Max.X && y >= Min.Y && y <= Max.Y;
        }


    }
}