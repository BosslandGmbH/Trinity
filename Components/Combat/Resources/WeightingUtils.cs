using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Settings;

namespace Trinity.Components.Combat.Resources
{
    public static class WeightingUtils
    {
        public static bool ShouldIgnoreGlobe(TrinityItem actor)
        {
            string reason;
            return ShouldIgnoreGlobe(actor, out reason);
        }

        public static bool ShouldIgnoreGlobe(TrinityItem actor, out string reason)
        {
            reason = string.Empty;

            if (actor == null)
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

            if (!Core.Settings.Weighting.GlobeTypes.HasFlag(actor.GlobeType))
            {
                reason = $"Ignore({actor.GlobeType}=Disabled)";
                return true;
            }

            return false;
        }

        public static bool ShouldIgnoreSpecialTarget(TrinityActor actor)
        {
            string reason;
            return ShouldIgnoreSpecialTarget(actor, out reason);
        }

        public static bool ShouldIgnoreSpecialTarget(TrinityActor actor, out string reason)
        {
            reason = string.Empty;

            if (actor == null)
                return true;

            if (!Core.Settings.Weighting.SpecialTypes.HasFlag(actor.SpecialType))
            {
                reason = $"Ignore({actor.SpecialType}=Disabled)";
                return true;
            }

            return false;
        }

        public static bool ShouldIgnoreTrash(TrinityActor unit)
        {
            string reason;
            return ShouldIgnoreTrash(unit, out reason);
        }

        public static bool ShouldIgnoreTrash(TrinityActor unit, out string reason)
        {
            reason = string.Empty;

            if (unit == null)
                return true;

            if (!unit.IsTrashMob)
                return false;

            if (unit.IsTreasureGoblin)
                return false;

            if (Core.Player.IsCastingPortal)
            {
                reason = "Ignore(CastingPortal)";
                return true;
            }

            if (unit.IsMinimapActive)
            {
                reason = "Keep(IsMinimapActive)";
                return false;
            }

            if (Core.Settings.Weighting.TrashWeighting == SettingMode.Enabled)
            {
                reason = "Keep(Trash=Enabled)";
                return false;
            }

            if (Core.Settings.Weighting.TrashWeighting == SettingMode.Disabled)
            {
                reason = "Ignore(Trash=Disabled)";
                return true;
            }

            if (Core.Settings.Weighting.TrashWeighting == SettingMode.Auto)
            {
                if (Core.BlockedCheck.IsBlocked || Core.StuckHandler.IsStuck)
                {
                    reason = "Keep(Blocked/Stuck)";
                    return false;
                }

                reason = "Ignore(Trash=OnlyWhenBlocked)";
                return true;
            }
            return false;
        }
    }
}