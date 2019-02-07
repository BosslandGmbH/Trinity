using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Actors
{
    public static class EntryPortals
    {
        private static readonly HashSet<int> _entryPortalWhiteList = new HashSet<int> { 705396550, 1825723588, 1037011047 };
        public static Dictionary<SNOWorld, int> EntryPortalHashNames = new Dictionary<SNOWorld, int>();

        public static void AddEntryPortal()
        {
            var currentWorldId = AdvDia.CurrentWorldId;
            if (EntryPortalHashNames.ContainsKey(currentWorldId))
            {
                return;
            }

            if (ZetaDia.Globals.IsLoadingWorld || Core.IsOutOfGame)
                return;

            var entryPortal = ZetaDia.Minimap.Markers.CurrentWorldMarkers.FirstOrDefault(m => (m.IsPortalEntrance || m.IsPortalExit) && m.Position.Distance(ZetaDia.Me.Position) < 12);
            if (entryPortal != null)
            {
                if (_entryPortalWhiteList.Contains(entryPortal.NameHash))
                {
                    return;
                }
                Core.Logger.Debug("[BountyData] Added entry portal {0}", entryPortal.NameHash);
                EntryPortalHashNames.Add(currentWorldId, entryPortal.NameHash);
            }
        }

        public static bool IsEntryPortal(SNOWorld currentWorldId, int markerNameHash)
        {
            return EntryPortalHashNames.ContainsKey(currentWorldId) && EntryPortalHashNames[currentWorldId] == markerNameHash;
        }
    }
}
