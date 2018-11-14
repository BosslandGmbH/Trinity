using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Settings;

namespace Trinity.Components.Combat.Resources
{
    public static class WeightingUtils
    {
        public static bool ShouldIgnoreGlobe(TrinityItem actor)
        {
            return ShouldIgnoreGlobe(actor, out _);
        }

        public static bool ShouldIgnoreGlobe(TrinityItem actor, out string reason)
        {
            reason = string.Empty;

            if (actor == null)
                return false;

            switch (Core.Settings.Weighting.GlobeWeighting)
            {
                case SettingMode.Disabled:
                    reason = "Ignore(Globes=Disabled)";
                    return true;

                case SettingMode.None:
                    reason = "Keep(Globes=None)";
                    return false;

                case SettingMode.Enabled:
                    reason = "Keep(Globes=Enabled)";
                    return false;
            }

            var item = actor.ToAcdItem();
            if (Core.Settings.Weighting.GlobeTypes.HasFlag(item.GetGlobeType()))
                return false;

            reason = $"Ignore({item.GetGlobeType()}=Disabled)";
            return true;
        }

        public static bool ShouldIgnoreSpecialTarget(TrinityActor actor)
        {
            return ShouldIgnoreSpecialTarget(actor, out _);
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
            return ShouldIgnoreTrash(unit, out _);
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
