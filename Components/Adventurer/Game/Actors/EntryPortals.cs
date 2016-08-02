using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Util;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Actors
{
    public static class EntryPortals
    {

        private static HashSet<int> _entryPortalWhiteList = new HashSet<int> { 705396550, 1825723588, 1037011047 };
        public static Dictionary<int, int> EntryPortalHashNames = new Dictionary<int, int>();

        public static void AddEntryPortal()
        {
            var currentWorldId = AdvDia.CurrentWorldId;
            if (EntryPortalHashNames.ContainsKey(currentWorldId))
            {
                return;
            }

            if (ZetaDia.IsLoadingWorld)
                return;

            var entryPortal = ZetaDia.Minimap.Markers.CurrentWorldMarkers.FirstOrDefault(m => (m.IsPortalEntrance || m.IsPortalExit) && m.Position.Distance(ZetaDia.Me.Position) < 12);
            if (entryPortal != null)
            {
                if (_entryPortalWhiteList.Contains(entryPortal.NameHash))
                {
                    return;
                }
                Logger.Debug("[BountyData] Added entry portal {0}", entryPortal.NameHash);
                EntryPortalHashNames.Add(currentWorldId, entryPortal.NameHash);
            }
        }

        public static bool IsEntryPortal(int currentWorldId, int markerNameHash)
        {
            return EntryPortalHashNames.ContainsKey(currentWorldId) && EntryPortalHashNames[currentWorldId] == markerNameHash;
        }

    }

}
