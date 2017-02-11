using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Settings;
using Trinity.Settings.ItemList;

namespace Trinity.Settings.Mock
{
    /// <summary>
    /// Usage:
    /// 
    /// Add namespace: 
    /// xmlns:mock="clr-namespace:Core.Settings.Mock"
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
        public static ItemListMockData ItemList { get; set; } = new ItemListMockData();
        public static RoutineMockData DataContext { get; set; } = new RoutineMockData();
        public static AdventurerGems Gems { get; set; } = new AdventurerGems();


    }
}