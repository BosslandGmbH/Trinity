using Trinity.Framework;
using Zeta.Bot;

namespace Trinity.Components.Adventurer.Game.Combat
{
    public static class TargetingHelper
    {
        public static void TurnCombatOff()
        {
            if (CombatTargeting.Instance.AllowedToKillMonsters)
            {
                CombatTargeting.Instance.AllowedToKillMonsters = false;
                Core.Logger.Debug("[ZergMode] On");
                //new ToggleTargetingTag
                //{
                //    Combat = false,
                //    Looting = LootTargeting.Instance.AllowedToLoot,
                //    LootRadius = CharacterSettings.Instance.LootRadius,
                //    KillRadius = CharacterSettings.Instance.KillRadius,
                //}.OnStart();
            }
        }

        public static void TurnCombatOn()
        {
            if (!CombatTargeting.Instance.AllowedToKillMonsters)
            {
                CombatTargeting.Instance.AllowedToKillMonsters = true;
                Core.Logger.Debug("[ZergMode] Off");
                //new ToggleTargetingTag
                //{
                //    Combat = true,
                //    Looting = LootTargeting.Instance.AllowedToLoot,
                //    LootRadius = CharacterSettings.Instance.LootRadius,
                //    KillRadius = CharacterSettings.Instance.KillRadius,
                //}.OnStart();
            }
        }

        public static CombatState CombatState
        {
            get { return CombatTargeting.Instance.AllowedToKillMonsters ? CombatState.Enabled : CombatState.Disabled; }
        }
    }

    public enum CombatState
    {
        Enabled,
        Disabled
    }
}