using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("ProfileSetting")]
    public class ProfileSettingTag : BaseProfileBehavior
    {
        #region XmlAttributes

        [XmlAttribute("name")]
        [Description("A key used to access the value later")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        [Description("A value to be saved")]
        public string Value { get; set; }

        [Description("Clears all current profile settings")]
        public bool ShouldReset { get; set; }

        #endregion

        public static Dictionary<string, string> ProfileSettings = new Dictionary<string, string>();

        public static bool Initialized;
        public static void Initialize()
        {
            BotMain.OnStart += bot => ProfileSettings.Clear();
            Initialized = true;
        }

        public override async Task<bool> StartTask()
        {
            if (!Initialized)
                Initialize();

            if (ShouldReset)
            {
                Core.Logger.Log($"Removed all profile settings");
                ProfileSettings.Clear();
            }

            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Value))
            {
                if (ProfileSettings.ContainsKey(Name))
                    ProfileSettings[Name] = Value;
                else
                    ProfileSettings.Add(Name, Value);

                Core.Logger.Log("Setting Condition={0} to {1}", Name, Value);
            }

            return true;
        }

    }
}



