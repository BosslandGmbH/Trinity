using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags.Deprecated
{    
    public class DeprecatedTag : BaseProfileBehavior
    {
        public DeprecatedTag(string message)
        {
            var attrNames = GetType().GetCustomAttributes<XmlElementAttribute>();
            var xmlNames = attrNames.Aggregate("", (s, item) => s + $"{item.Name}, ");
            Core.Logger.Warn($"Invalid Profile Tag Detected: {GetType().Name} ({xmlNames}). {message}");
        }

        public override async Task<bool> StartTask() => true;
    }

    [XmlElement("TrinityLoadOnce")]
    [XmlElement("LoadOnce")]
    [XmlElement("ConfirmOK")]
    [XmlElement("Command")]
    [XmlElement("IncrementCounter")]
    [XmlElement("TrinityMoveToSNO")]
    [XmlElement("TrinityOffsetMove")]
    public class DeprecatedNotSupportedTag : DeprecatedTag
    {
        public DeprecatedNotSupportedTag() : base("This tag is no longer supported") { }
    }

    [XmlElement("TrinityLoadOnce")]
    [XmlElement("LoadOnce")]
    [XmlElement("UseOnce")]
    [XmlElement("TrinityUseOnce")]
    public class DeprecatedConditionalUseTag : DeprecatedTag
    {
        public DeprecatedConditionalUseTag() : base("Use ProfileSetting tag/conditions instead") { }
    }

    [XmlElement("TrinityLog")]
    [XmlElement("TrinityIf")]
    [XmlElement("TrinityIfRandom")]
    [XmlElement("IfRandom")]
    [XmlElement("TrinityLoadProfile")]
    public class DeprecatedUseDefaultVersionTag : DeprecatedTag
    {
        public DeprecatedUseDefaultVersionTag() : base("Use the DB provided default version instead.") { }
    }


}

