using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Trinity.Settings
{
    [CollectionDataContract(Namespace = "", ItemName = "Node")]
    public class DynamicNodeCollection : ObservableCollection<DynamicSettingNode> { }
}