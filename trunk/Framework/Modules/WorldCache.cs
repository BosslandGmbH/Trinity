using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Framework.Modules
{
    public class WorldCache : Module
    {
        protected override int UpdateIntervalMs => 1000;

        protected override void OnPulse()
        {
            if (!ZetaDia.IsInGame)
                return;

            var worldInfo = ZetaDia.WorldInfo;
            if (worldInfo != null && worldInfo.IsValid && !worldInfo.IsDisposed)
            {
                //EnvironmentType = ZetaDia.Memory.Read<WorldEnvironmentType>(worldInfo.BaseAddress + 0xAC);
                Name = worldInfo.Name;
                IsGenerated = worldInfo.IsGenerated;
            }
        }

        public bool IsGenerated { get; set; }

        public string Name { get; set; }

        //public WorldEnvironmentType EnvironmentType { get; set; }

        //public enum WorldEnvironmentType
        //{
        //    None = 0,
        //    KulePrisonWorld = 1065353216,
        //}
    }

}

