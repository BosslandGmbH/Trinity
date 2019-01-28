using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public static class GridStore
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        // Contains a list of grids that have been created without blocking their garbage disposal.
        internal static List<WeakReference<IGrid>> Grids = new List<WeakReference<IGrid>>();

        public static List<IGrid> GetCurrentGrids()
        {
            var worldId = ZetaDia.Globals.WorldId;
            var result = new List<IGrid>();
            foreach (var gridRef in Grids.ToList())
            {
                if (!gridRef.TryGetTarget(out var grid))
                {
                    Grids.Remove(gridRef);
                    continue;
                }

                if (grid.WorldDynamicId == worldId)
                    result.Add(grid);
            }

            s_logger.Debug($"[{nameof(GetCurrentGrids)}] contains {Grids.Count} grid instances, {result.Count} for Current world.");

            return result;
        }
    }
}