using System;
using System.Collections.Generic;
using Trinity.Framework.Objects.Enums;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Objects.Memory
{
    public class GameBalanceHelper
    {
        public static Dictionary<int, object> Cache = new Dictionary<int, object>();

        public static HashSet<SnoGameBalanceType> ValidGameBalanceTypes { get; set; }

        public static IntPtr GetRecordPtr(SnoGameBalanceType gbType, int gbId)
        {
            if ((int)gbType != -1)
            {
                return SNORecordGameBalance.GetGameBalanceRecord(gbId, (GameBalanceType)(int)gbType);
            }
            return IntPtr.Zero;
        }

        public static T GetRecord<T>(SnoGameBalanceType gbType, int gbId) where T : struct
        {
            if ((int)gbType != -1)
            {
                if (Cache.ContainsKey(gbId))
                    return (T)Cache[gbId];

                var record = SNORecordGameBalance.GetGameBalanceRecord<T>(gbId, (GameBalanceType)(int)gbType);
                if (record.HasValue)
                {
                    Cache.Add(gbId, record);
                    return (T)record;
                }
                else
                {
                    Core.Logger.Log($"SnoRecord not found GbId={gbId} GbType={gbType}");
                }
            }
            return default(T);
        }

    }
}