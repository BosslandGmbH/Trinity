using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Modules
{
    public class WorldCache : Module
    {
        public string CurrentLevelAreaName => GetLevelAreaName(Core.Player.LevelAreaId);

        public string GetLevelAreaName(int levelAreaSnoId)
        {
            var snoName = ZetaDia.SNO.LookupSNOName(SNOGroup.LevelArea, levelAreaSnoId);
            var snoHash = MemoryHelper.GameBalanceNormalHash(snoName);
            var name = SnoManager.StringListHelper.GetStringListValue(SnoStringListType.LevelAreaNames, snoHash);
            return name;
        }

        //protected override int UpdateIntervalMs => 1000;

        //protected override void OnPulse()
        //{
        //    if (!ZetaDia.IsInGame)
        //        return;

        //    var worldInfo = ZetaDia.WorldInfo;
        //    if (worldInfo != null && worldInfo.IsValid && !worldInfo.IsDisposed)
        //    {
        //        //EnvironmentType = ZetaDia.Memory.Read<WorldEnvironmentType>(worldInfo.BaseAddress + 0xAC);
        //        Name = worldInfo.Name;
        //        IsGenerated = worldInfo.IsGenerated;
        //    }
        //}

        //public bool IsGenerated { get; set; }

        //public string Name { get; set; }

        //public WorldEnvironmentType EnvironmentType { get; set; }

        //public enum WorldEnvironmentType
        //{
        //    None = 0,
        //    KulePrisonWorld = 1065353216,
        //}
    }

}

