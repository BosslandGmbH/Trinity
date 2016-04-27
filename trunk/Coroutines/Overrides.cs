using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Trinity.Technicals;
using Zeta.Bot;

namespace Trinity.Coroutines
{
    public class Overrides
    {        
        public async static Task<bool> Execute()
        {
            //await DestroyUbersDoor();

            return true;
        }

        //private async static Task<bool> DestroyUbersDoor()
        //{            
        //    if (TrinityPlugin.Player.WorldID != 332336 || TrinityPlugin.Player.WorldType != Act.OpenWorld)
        //        return false;

        //    //ActorId: 258064, Type: Gizmo, Name: Uber_BossPortal_Door-548, 
        //    var uberDoor = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(g => g.ActorSnoId == 258064 && g.Distance <= 25f);
        //    if (uberDoor == null)
        //        return false;

        //    Logger.Log("[Override] Destroying Uber Door");

        //    var power = CombatBase.DefaultWeaponPower;
        //    var range = CombatBase.DefaultWeaponDistance;

        //    await TrinityCoroutines.MoveTo.Execute(uberDoor.Position, "Ubers Door", 3);

        //    if (ZetaDia.Me.UsePower(power, uberDoor.Position, TrinityPlugin.CurrentWorldDynamicId, -1))
        //    {
        //        Logger.Log("[Override] Attacked Uber Door with {0}", power);
        //        await Coroutine.Sleep(3000);
        //        return true;
        //    }

        //    Logger.Log("[Override] Attack with {0} Failed on Uber Door", power);
        //    await Coroutine.Sleep(1000);
        //    return false;
        //}
    }
}
