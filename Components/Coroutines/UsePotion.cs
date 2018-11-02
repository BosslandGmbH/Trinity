using log4net;
using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Helpers;
using Zeta.Bot.Coroutines;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines
{
    public class UsePotion
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        public static ACDItem ActivePotion =>
            InventoryManager.Backpack
                .FirstOrDefault(i => i.ItemType == ItemType.Potion && i.IsEquipped) ??
            InventoryManager.BaseHealthPotion;

        public static async Task<CoroutineResult> DrinkPotion()
        {
            if (!ZetaDia.IsInGame ||
                ZetaDia.Globals.IsLoadingWorld ||
                ZetaDia.Globals.IsPlayingCutscene ||
                ZetaDia.IsInTown ||
                SpellHistory.TimeSinceUse(SNOPower.DrinkHealthPotion) <= TimeSpan.FromSeconds(30) ||
                ZetaDia.Me == null ||
                !ZetaDia.Me.IsFullyValid() ||
                ZetaDia.Me.IsDead ||
                Combat.TrinityCombat.Routines.Current == null)
            {
                return CoroutineResult.NoAction;
            }

            if (ZetaDia.Me.HitpointsCurrentPct > Combat.TrinityCombat.Routines.Current.PotionHealthPct)
                return CoroutineResult.NoAction;

            if (ZetaDia.Me.IsFeared ||
                ZetaDia.Me.IsStunned ||
                ZetaDia.Me.IsFrozen ||
                ZetaDia.Me.IsBlind ||
                ZetaDia.Me.CommonData
                    .GetAttribute<bool>(ActorAttributeType.PowerImmobilize))
            {
                s_logger.Warn($"[{nameof(DrinkPotion)}] Can't use potion while incapacitated!");
                return CoroutineResult.NoAction;
            }

            if (ActivePotion == null)
            {
                s_logger.Warn($"[{nameof(DrinkPotion)}] No Available potions!");
                return CoroutineResult.NoAction;
            }

            s_logger.Info($"[{nameof(DrinkPotion)}] Using Potion {ActivePotion.Name}");
            InventoryManager.UseItem(ActivePotion.AnnId);
            SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
            SnapShot.Record();
            return CoroutineResult.Done;
        }
    }
}