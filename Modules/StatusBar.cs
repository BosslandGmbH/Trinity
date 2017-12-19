using System.Globalization;
using System.Text;
using Trinity.Components.Combat;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;

namespace Trinity.Modules
{
    public class StatusBar : Module
    {
        protected override int UpdateIntervalMs => 250;
        protected override void OnPulse() => Update();

        private void Update()
        {
            if (TrinityCombat.Targeting.CurrentTarget == null)
                return;

            // todo: moved from handle target due to 5% of cpu usage, refactor

            var statusText = new StringBuilder();
            var currentTarget = TrinityCombat.Targeting.CurrentTarget;
            if (currentTarget == null)
                return;

            var player = Core.Player;

            //if(!CombatBase.IsInCombat)
            //    BotMain.StatusText = "No more targets";

            statusText.Append(" Target=");
            statusText.Append(currentTarget.InternalName);

            if (currentTarget.IsUnit && TrinityCombat.Targeting.CurrentPower != null && TrinityCombat.Targeting.CurrentPower.SNOPower != SNOPower.None)
            {
                statusText.Append(" Power=");
                statusText.Append(TrinityCombat.Targeting.CurrentPower.SNOPower);
            }

            //statusText.Append(" Speed=");
            //statusText.Append(ZetaDia.Me.Movement.SpeedXY.ToString("0.00"));
            statusText.Append(" SNO=");
            statusText.Append(currentTarget.ActorSnoId.ToString(CultureInfo.InvariantCulture));
            statusText.Append(" Elite=");
            statusText.Append(currentTarget.IsElite.ToString());
            statusText.Append(" Weight=");
            statusText.Append(currentTarget.Weight.ToString("0"));
            statusText.Append(" Type=");
            statusText.Append(currentTarget.Type.ToString());
            statusText.Append(" C-Dist=");
            statusText.Append(currentTarget.Distance.ToString("0.0"));
            statusText.Append(" R-Dist=");
            statusText.Append(currentTarget.RadiusDistance.ToString("0.0"));
            statusText.Append(" RangeReq'd=");
            statusText.Append(currentTarget.RequiredRadiusDistance.ToString("0.0"));
            statusText.Append(" DistfromTrgt=");
            statusText.Append(currentTarget.Distance.ToString("0"));
            statusText.Append(" tHP=");
            statusText.Append((currentTarget.HitPointsPct * 100).ToString("0"));
            statusText.Append(" MyHP=");
            statusText.Append((player.CurrentHealthPct * 100).ToString("0"));
            statusText.Append(" MyMana=");
            statusText.Append((player.PrimaryResource).ToString("0"));
            statusText.Append(" InLoS=");
            statusText.Append(currentTarget.IsInLineOfSight.ToString());

            //statusText.Append($" Duration={DateTime.UtcNow.Subtract(TargetHandler.LastPickedTargetTime).TotalSeconds:0}");

            BotMain.StatusText = statusText.ToString();

        }
    }
}
