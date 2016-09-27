using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Trinity.Config
{
    [CollectionDataContract(Namespace = "", ItemName = "Node")]
    public class DynamicNodeCollection : ObservableCollection<DynamicSettingNode> { }
}