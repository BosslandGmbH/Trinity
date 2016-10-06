using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Settings;

namespace Trinity.Components.Combat.Resources
{
    public static class WeightingUtils
    {
        public static bool ShouldIgnoreGlobe(TrinityItem cacheObject)
        {
            string reason;
            return ShouldIgnoreGlobe(cacheObject, out reason);
        }

        public static bool ShouldIgnoreGlobe(TrinityItem cacheObject, out string reason)
        {
            reason = string.Empty;

            if (cacheObject == null)
                return false;

            switch (Core.Settings.Weighting.GlobeWeighting)
            {
                case SettingMode.Disabled:
                    reason = $"Ignore(Globes=Disabled)";
                    return true;

                case SettingMode.None:
                    reason = $"Keep(Globes=None)";
                    return false;

                case SettingMode.Enabled:
                    reason = $"Keep(Globes=Enabled)";
                    return false;
            }

            if (!Core.Settings.Weighting.GlobeTypes.HasFlag(cacheObject.GlobeType))
            {
                reason = $"Ignore({cacheObject.GlobeType}=Disabled)";
                return true;
            }

            return false;
        }
    }
}

