using System.Windows.Data;

namespace Trinity.UI.UIComponents
{
    public class IntegerStringBinding : Binding
    {
        public IntegerStringBinding(string path)
            : base(path)
        {
            StringFormat = "{0:0}";
        }
    }
}
