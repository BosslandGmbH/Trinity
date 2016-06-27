using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Resources.BNetAPI
{
    [DataContract]
    public class ApTrinityItem : ApTrinityItemBase, IApiAttributes
    {
        [DataMember(Name = "requiredLevel")]
        public int RequiredLevel { get; set; }

        [DataMember(Name = "itemLevel")]
        public int ItemLevel { get; set; }

        [DataMember(Name = "bonusAffixes")]
        public int BonusAffixes { get; set; }

        [DataMember(Name = "bonusAffixesMax")]
        public int BonusAffixesMax { get; set; }

        [DataMember(Name = "accountBound")]
        public bool AccountBound { get; set; }

        [DataMember(Name = "flavorText")]
        public string FlavorText { get; set; }

        [DataMember(Name = "typeName")]
        public string TypeName { get; set; }

        [DataMember(Name = "type")]
        public ApTrinityItemType Type { get; set; }

        [DataMember(Name = "armor")]
        public ApiRange Armor { get; set; }

        [DataMember(Name = "slots")]
        public object Slots { get; set; }

        [DataMember(Name = "attributes")]
        public ApiAttributes Attributes { get; set; }

        [DataMember(Name = "attributesRaw")]
        public Dictionary<string, ApiRange> AttributesRaw { get; set; }

        [DataMember(Name = "randomAffixes")]
        public List<string> RandomAffixes { get; set; }

        [DataMember(Name = "gems")]
        public List<string> Gems { get; set; }

        [DataMember(Name = "socketEffects")]
        public object SocketEffects { get; set; }

        [DataMember(Name = "set")]
        public ApiSet Set { get; set; }

        [DataMember(Name = "seasonRequiredToDrop")]
        public int SeasonRequiredToDrop { get; set; }

        [DataMember(Name = "isSeasonRequiredToDrop")]
        public bool IsSeasonRequiredToDrop { get; set; }

        [DataMember(Name = "craftedBy")]
        public List<string> CraftedBy { get; set; }

        public int BonusAffixesPrimary
        {
            get { return BonusAffixesMax - BonusAffixesSecondary; }
        }

        public int BonusAffixesSecondary
        {
            get { return 2 - (Attributes.Secondary.Count + Attributes.Passive.Count); }
        }

        public override string ToString()
        {
            return string.Format(" Name = {0}", Name);
        }

    }

    [DataContract]
    public class ApTrinityItemBase
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "displayColor")]
        public string DisplayColor { get; set; }

        [DataMember(Name = "tooltipParams")]
        public string TooltipParams { get; set; }
    }

    [DataContract]
    public class ApiSet
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "items")]
        public List<ApTrinityItemBase> Items { get; set; }

        [DataMember(Name = "slug")]
        public string Slug { get; set; }

        [DataMember(Name = "ranks")]
        public List<ApiSetRank> Ranks { get; set; }

        public override string ToString()
        {
            return string.Format(" Name = {0}, Items = {1} Ranks = {2} ", Name, Items.Count, Ranks.Count);
        }
    }

    [DataContract]
    public class ApiSetRank : IApiAttributes
    {
        [DataMember(Name = "required")]
        public int Required { get; set; }

        [DataMember(Name = "attributes")]
        public ApiAttributes Attributes { get; set; }

        [DataMember(Name = "attributesRaw")]
        public Dictionary<string, ApiRange> AttributesRaw { get; set; }

        [DataMember(Name = "slug")]
        public string Slug { get; set; }
    }

    public interface IApiAttributes
    {
        [DataMember(Name = "attributes")]
        ApiAttributes Attributes { get; set; }

        [DataMember(Name = "attributesRaw")]
        Dictionary<string, ApiRange> AttributesRaw { get; set; }
    }

    [DataContract]
    public class ApTrinityItemType
    {
        [DataMember(Name = "twoHanded")]
        public bool TwoHanded { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        public override string ToString()
        {
            return string.Format(" Id = {0}, TwoHanded = {1} ", Id, TwoHanded);
        }
    }

    [DataContract]
    public class ApiAttributes
    {
        [DataMember(Name = "primary")]
        public List<ApiAttribute> Primary { get; set; }

        [DataMember(Name = "secondary")]
        public List<ApiAttribute> Secondary { get; set; }

        [DataMember(Name = "passive")]
        public List<ApiAttribute> Passive { get; set; }

        public override string ToString()
        {
            return string.Format(" Primary = {0}, Secondary = {1} Passive = {2} ", Primary.Count, Secondary.Count, Passive.Count);
        }
    }

    [DataContract]
    public class ApiAttribute
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "affixType")]
        public string AffixType { get; set; }

        public override string ToString()
        {
            return string.Format(" Text = {0}, Color = {1} AffixType = {2} ", Text, Color, AffixType);
        }
    }

    [DataContract]
    public class ApiAttrbuteRaw : ApiRange
    {
        public string Name { get; set; }
    }

    [DataContract]
    public class ApiRange
    {
        [DataMember(Name = "min")]
        public double Min { get; set; }

        [DataMember(Name = "max")]
        public double Max { get; set; }

        public override string ToString()
        {
            return string.Format(" Min = {0}, Max = {1} ", Min, Max);
        }
    }
}
