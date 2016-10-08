using System;
using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Objects.Memory.Sno.Helpers
{
    public class GameBalanceHelper
    {
        public Dictionary<int, object> Cache = new Dictionary<int, object>();

        public HashSet<SnoGameBalanceType> ValidGameBalanceTypes { get; set; }

        public IntPtr GetRecordPtr(SnoGameBalanceType gbType, int gbId)
        {
            if ((int)gbType != -1)
            {
                return SNORecordGameBalance.GetGameBalanceRecord(gbId, (Zeta.Game.Internals.SNO.GameBalanceType)(int)gbType);
            }
            return IntPtr.Zero;
        }

        public T GetRecord<T>(SnoGameBalanceType gbType, int gbId) where T : struct
        {
            if ((int)gbType != -1)
            {
                if (Cache.ContainsKey(gbId))
                    return (T)Cache[gbId];

                var record = SNORecordGameBalance.GetGameBalanceRecord<T>(gbId, (Zeta.Game.Internals.SNO.GameBalanceType)(int)gbType);
                if (record.HasValue)
                {
                    Cache.Add(gbId, record);
                    return (T)record;
                }
                else
                {
                    Logger.Log($"SnoRecord not found GbId={gbId} GbType={gbType}");
                }

            }
            return default(T);
        }

        
    }
}