using System;
using System.Text.RegularExpressions;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Helpers.Bounties
{
    public static class Extensions
    {
        const int ACT_INDEX = 3;
        const int PLACE_INDEX = 4;
        const int TYPE_INDEX = 5;
        const int UNIQUE_INDEX = 6;

        //X1_Bounty_A1_Cathedral_Kill_Braluk
        private static Regex bountyTypeExtract = new Regex("([^_]+)_([^_]+)_([^_]+)_([^_]+)_([^_]+)_([^_]+)", RegexOptions.Compiled);
        /// <summary>
        /// Extracts BountyType from the QuestSnoId Enum
        /// </summary>
        /// <param name="bountyInfo"></param>
        /// <returns></returns>
        public static BountyType GetBountyType(this BountyInfo bountyInfo)
        {
            string name = bountyInfo.Quest.ToString();
            var nameMatch = bountyTypeExtract.Match(name);
            string type = "";

            if (nameMatch.Groups.Count == 7)
                type = nameMatch.Groups[TYPE_INDEX].Value;

            BountyType result;
            if (Enum.TryParse(type, out result))
                return result;

            return BountyType.Invalid;
        }

        public static Act GetBountyAct(this BountyInfo bountyInfo)
        {
            string name = bountyInfo.Quest.ToString();
            var nameMatch = bountyTypeExtract.Match(name);
            string type = "";

            if (nameMatch.Groups.Count == 7)
                type = nameMatch.Groups[ACT_INDEX].Value;

            Act result;
            if (Enum.TryParse(type, out result))
                return result;

            return Act.Invalid;
        }
    }
}
