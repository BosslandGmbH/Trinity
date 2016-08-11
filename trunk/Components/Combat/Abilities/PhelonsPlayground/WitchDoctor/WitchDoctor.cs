using Trinity.Reference;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    using Framework;

    partial class WitchDoctor : CombatBase
    {
        private static bool IszDPS => Legendary.AquilaCuirass.IsEquipped && Legendary.LastBreath.IsEquipped;
        private static bool IsHelltooth => Sets.HelltoothHarness.IsFullyEquipped;
        private static bool IsJadeHArvester => Sets.RaimentOfTheJadeHarvester.IsFullyEquipped;

        #region Variables

        public static int DogCount
        {
            get
            {
                var totalcount = 3;
                if (Passives.WitchDoctor.MidnightFeast.IsActive)
                    totalcount = totalcount + 1;
                if (Passives.WitchDoctor.FierceLoyalty.IsActive)
                    totalcount = totalcount + 1;
                if (Passives.WitchDoctor.ZombieHandler.IsActive)
                    totalcount = totalcount + 1;
                if (Legendary.TheTallMansFinger.IsEquipped)
                    totalcount = 1;
                return totalcount;
            }
        }

        public static readonly int GargCount = Legendary.TheShortMansFinger.IsEquipped ? 3 : 1;

        private static readonly int SoulHarvestMaxStack = Legendary.SacredHarvester.IsEquipped ? 10 : 5;

        #endregion

        public static TrinityPower GetPower()
        {
            TrinityPower power = Unconditional.PowerSelector();

            if (Player.IsInTown)
                return null;
            if (power == null && CurrentTarget != null)
            {
                if (IszDPS)
                    power = ZDps.PowerSelector();
                if (IsJadeHArvester)
                    power = JadeHarvester.Plaguebearer.PowerSelector();
                if (IsHelltooth)
                {
                    if (Skills.WitchDoctor.Gargantuan.IsActive)
                        power = Helltooth.ColdGarg.PowerSelector();
                    if (Skills.WitchDoctor.AcidCloud.IsActive)
                        power = Helltooth.AcidCloud.PowerSelector();
                    if (Skills.WitchDoctor.Firebats.IsActive)
                        power = Helltooth.Firebats.PowerSelector();
                }
            }
            return power;
        }
    }
}
