using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Objects.Memory.Sno.Helpers
{
    public static class SnoSearchProxy
    {
        // The Sno data in memory is a cache and does not have all possible entries in it.
        // Only data that has been required by the game is present. For example, 
        // If you try to find a Power for a different actorClass that you have't used or 
        // seen this game session then it wont exist. Switch to that hero in game and it appears.
        // If you need all the data (maybe to export it or something) then you need to either 
        // do a function call (like nesox does) or read the game files directly (like rosbot does).

        private static SNOTable _actorTable;
        private static SNOTable _powerTable;

        private static readonly Func<IntPtr, SNOTable, SNORecordSkillKit> LeastOverheadRecordFactory = (ptr, table) => ptr.UnsafeCreate<SNORecordSkillKit>(table);

        public static T GetRecord<T>(SnoType type, int snoId) where T : MemoryWrapper, new()
        {
            SNOTable table;
            switch (type)
            {
                case SnoType.Actor:
                    if (_actorTable == null)
                        _actorTable = Internals.Addresses.ActorSnoTable.UnsafeCreate<SNOTable>(ClientSNOTable.Actor, LeastOverheadRecordFactory);
                    table = _actorTable;
                    break;
                case SnoType.Power:
                    if (_powerTable == null)
                        _powerTable = Internals.Addresses.PowerSnoTable.UnsafeCreate<SNOTable>(ClientSNOTable.Power, LeastOverheadRecordFactory);
                    table = _powerTable;
                    break;
                default:
                    throw new ArgumentException($"SnoQueryHelper.GetSnoRecord does not support the '{type}' type");
            }
            var record = table.GetRecord<SNORecordSkillKit>(snoId);
            var ptr = record.BaseAddress;
            record.Dispose();
            return MemoryWrapper.Create<T>(ptr);
        }

        public static string GetName(SnoType type, int snoId)
            => ZetaDia.SNO.LookupSNOName((SNOGroup)type, snoId);
    }
}


