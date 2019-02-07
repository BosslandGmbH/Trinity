using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("Waypoint")]
    [XmlElement("UseWaypoint")]
    [XmlElement("TakeWaypoint")]
    public class TakeWaypointTag : BaseProfileBehavior
    {
        [XmlAttribute("number")]
        [XmlAttribute("waypointNumber")]
        [Description("Number of waypoint to arrive at")]
        [DefaultValue(-1)]
        public int WaypointNumber { get; set; }

        [XmlAttribute("levelAreaId")]
        [XmlAttribute("levelAreaSnoId")]
        [XmlAttribute("destinationLevelAreaSnoId")]
        [Description("Id of level area to arrive at")]
        public SNOLevelArea DestinationLevelAreaSnoId { get; set; }

        public override async Task<bool> MainTask()
        {
            if (WaypointNumber == -1 && DestinationLevelAreaSnoId != 0)
                WaypointNumber = WaypointCoroutine.GetWaypointNumber(DestinationLevelAreaSnoId);

            if (!await WaypointCoroutine.UseWaypoint(WaypointNumber))
                return false;

            Done();
            await Coroutine.Yield();
            return true;
        }

    }

    [XmlElement("DumpBounties")]
    public class DumpUnsupportedBountiesTag : BaseProfileBehavior
    {
        public override async Task<bool> MainTask()
        {
            var bountiesNoCoroutines = ZetaDia.Storage.Quests.Bounties.Where(
                        b =>
                        {
                            BountyData bd = BountyDataFactory.GetBountyData(b.Quest);
                            if (bd != null && bd.Coroutines.Count == 0)
                                return true;

                            return false;
                        }).ToList();

            var bounties = ZetaDia.Storage.Quests.Bounties.Where(
                            b =>
                                BountyDataFactory.GetBountyData(b.Quest) == null &&
                                b.State != QuestState.Completed)
                            .ToList();

            if (bountiesNoCoroutines.Count != 0)
            {
                Core.Logger.Debug("No coroutines:");
                foreach (var bounty in bountiesNoCoroutines)
                {
                    DumpBountyInfo(bounty);
                }
            }

            if (bounties.Count != 0)
            {
                Core.Logger.Debug("Not supported:");
                foreach (var bounty in bounties)
                {
                    DumpBountyInfo(bounty);
                }
            }

            if (bounties.Count != 0 || bountiesNoCoroutines.Count != 0)
                BotMain.Stop(reason: "Unsupported bounty found!");

            Done();
            await Coroutine.Yield();
            return true;
        }

        private static void DumpBountyInfo(BountyInfo bountyInfo)
        {
            Core.Logger.Raw("// {0} - {1} ({2})", bountyInfo.Act, bountyInfo.Info.DisplayName, (int)bountyInfo.Quest);
            Core.Logger.Raw("Bounties.Add(new BountyData");
            Core.Logger.Raw("{");
            Core.Logger.Raw("    QuestId = {0},", (int)bountyInfo.Quest);
            Core.Logger.Raw("    Act = Act.{0},", bountyInfo.Act);
            Core.Logger.Raw("    WorldId = 0, // Enter the final worldId here");
            Core.Logger.Raw("    QuestType = BountyQuestType.SpecialEvent,");
            Core.Logger.Raw("    WaypointLevelAreaId = {0},", (int)bountyInfo.StartingLevelArea);
            Core.Logger.Raw("    Coroutines = new List<ISubroutine>");
            Core.Logger.Raw("    {");
            Core.Logger.Raw("        // Coroutines goes here");
            Core.Logger.Raw("    }");
            Core.Logger.Raw("});");
            Core.Logger.Raw(" ");
        }
    }
}

