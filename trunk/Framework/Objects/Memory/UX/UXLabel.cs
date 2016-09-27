namespace Trinity.Framework.Objects.Memory.UX
{
    public class UXLabel : UXControl
    {
        public string Text => ReadStringPointer(0xA20);
        public int FontSnoId => ReadOffset<int>(0xA24);
        public int FontSize => ReadOffset<int>(0xA28);
        public int TextLength => ReadOffset<int>(0xA30);
    }

}
