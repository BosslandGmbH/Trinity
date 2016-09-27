namespace Trinity.Framework.Objects.Memory.UX
{
    public class UXListItem : UXLabel
    {
        public new const int SizeOf = 0xE08; //3592
        public new const int VTable = 0x01829E98;

        public int xC88 => ReadOffset<int>(0xC88);
        public int _xC8C => ReadOffset<int>(0xC8C);
        public UXReference xC90_UIRef => ReadObject<UXReference>(0xC90);
        public int xE98_Neg1 => ReadOffset<int>(0xE98);
        public int xE9C => ReadOffset<int>(0xE9C);
        public int xEA0 => ReadOffset<int>(0xEA0);
        public int xEA4 => ReadOffset<int>(0xEA4);
        public int xEA8_Neg1 => ReadOffset<int>(0xEA8);
        public int xEAC_Neg1 => ReadOffset<int>(0xEAC);
        public int xEB0_StructStart_Min16Bytes => ReadOffset<int>(0xEB0);
        public int _xEB4 => ReadOffset<int>(0xEB4);
        public int _xEB8 => ReadOffset<int>(0xEB8);
        public int _xEBC => ReadOffset<int>(0xEBC);
        public int xEC0_Neg1 => ReadOffset<int>(0xEC0);
        public int xEC4_Neg1 => ReadOffset<int>(0xEC4);
        public int xEC8_Neg1 => ReadOffset<int>(0xEC8);
        public int xECC_StructStart_Min16Bytes => ReadOffset<int>(0xECC);
        public int _xED0 => ReadOffset<int>(0xED0);
        public int _xED4 => ReadOffset<int>(0xED4);
        public int _xED8 => ReadOffset<int>(0xED8);
        public int xEDC_Neg1 => ReadOffset<int>(0xEDC);
        public int xEE0_Neg1 => ReadOffset<int>(0xEE0);
        public int xEE4_Neg1 => ReadOffset<int>(0xEE4);
        public int xEE8_StructStart_Min16Bytes => ReadOffset<int>(0xEE8);
        public int _xEEC => ReadOffset<int>(0xEEC);
        public int _xEF0 => ReadOffset<int>(0xEF0);
        public int _xEF4 => ReadOffset<int>(0xEF4);
        public int xEF8_Neg1 => ReadOffset<int>(0xEF8);
        public int xEFC_Neg1 => ReadOffset<int>(0xEFC);
        public int xF00 => ReadOffset<int>(0xF00);
        public int xF04 => ReadOffset<int>(0xF04);
        public int xF08 => ReadOffset<int>(0xF08);
        public int xF0C => ReadOffset<int>(0xF0C);
        public int xF10 => ReadOffset<int>(0xF10);
        public int _xF14 => ReadOffset<int>(0xF14);
    }

}
