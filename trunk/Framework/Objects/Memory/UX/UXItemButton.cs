namespace Trinity.Framework.Objects.Memory.UX
{
    public class UXItemButton : UXButton
    {
        public new const int SizeOf = 0xE60; //3680
        public new const int VTable = 0x01826A28;

        public int xF48_Neg1 => ReadOffset<int>(0xF48);
        public int xF4C_Flags => ReadOffset<int>(0xF4C);
        public int xF50 => ReadOffset<int>(0xF50);
        public int xF54_Neg1 => ReadOffset<int>(0xF54);
        public int xF58_Neg1 => ReadOffset<int>(0xF58);
        public int _xF5C => ReadOffset<int>(0xF5C);
        public int xF60 => ReadOffset<int>(0xF60);
        public int _xF64 => ReadOffset<int>(0xF64);
    }
}
