using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Settings.Mock
{
    /// <summary>
    /// Usage:
    /// 
    /// Add namespace: 
    /// xmlns:mock="clr-namespace:TrinityPlugin.Settings.Mock"
    /// 
    /// Ensure you have d (designtime) namesapace    
    /// xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    /// mc:Ignorable="d" 
    /// 
    /// Add Resource: 
    /// <mock:MockData x:Key="DesignViewModel"/>
    /// 
    /// Redirect DataContext where required
    /// d:DataContext="{Binding Source={StaticResource DesignViewModel}}"
    /// 
    /// </summary>
    public class MockData
    {
        static MockData()
        {
            ItemList = new ItemListMockData();
            SkillSettings = new SkillSettingsMockData();
        }

        public static SkillSettingsMockData SkillSettings { get; set; }

        public static ItemListMockData ItemList { get; set; }
    }
}