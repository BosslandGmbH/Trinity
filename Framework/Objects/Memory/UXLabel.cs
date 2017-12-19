using System;
using Trinity.Framework.Helpers;
using System.Collections;
using System.Collections.Generic;

namespace Trinity.Framework.Objects.Memory
{
    public class UXLabel : UXControl
    {
        public string Text => ReadStringPointer(0xA20);
        public int FontSnoId => ReadOffset<int>(0xA24);
        public int FontSize => ReadOffset<int>(0xA28);
        public int TextLength => ReadOffset<int>(0xA30);
    }

    public class UXBlinker : UXLabel
    {
        public new const int SizeOf = 0xB90; //2960
        public new const int VTable = 0x01826888;

        public int xC88 => ReadOffset<int>(0xC88);
        public int xC8C => ReadOffset<int>(0xC8C);
        public int xC90 => ReadOffset<int>(0xC90);
        public int xC94 => ReadOffset<int>(0xC94);
        public int xC98 => ReadOffset<int>(0xC98);
        public int _xC9C => ReadOffset<int>(0xC9C);
    }

    internal class UXText : UXItemsControl
    {
        public new const int SizeOf = 0xC48; //3144
        public new const int VTable = 0x01826EA8;

        public int xA48 => ReadOffset<int>(0xA48);
        public int xA4C => ReadOffset<int>(0xA4C);
        public int xA50 => ReadOffset<int>(0xA50);
        public int xA54 => ReadOffset<int>(0xA54);
        public int xA58 => ReadOffset<int>(0xA58);
        public float xA5C => ReadOffset<float>(0xA5C);
        public int xA60 => ReadOffset<int>(0xA60);
        public int xA64 => ReadOffset<int>(0xA64);
        public UXReference xA68_UIRef => ReadObject<UXReference>(0xA68);
    }

    public class UXStackPanel : UXItemsControl
    {
        public new const int SizeOf = 0xA48; //2632
        public new const int VTable = 0x01826500;

        public int xA48 => ReadOffset<int>(0xA48);
        public int xA4C => ReadOffset<int>(0xA4C);
        public float xA50 => ReadOffset<float>(0xA50);
        public float xA54 => ReadOffset<float>(0xA54);
        public float xA58 => ReadOffset<float>(0xA58);
        public float xA5C => ReadOffset<float>(0xA5C);
        public float xA60 => ReadOffset<float>(0xA60);
        public int xA64_Base_x500 => ReadOffset<int>(0xA64);
        public int xA68_Base_x504 => ReadOffset<int>(0xA68);
        public int xA6C => ReadOffset<int>(0xA6C);
    }

    public class UXHotbarButton : UXButton
    {
        public new const int SizeOf = 0xE48; //3656
        public new const int VTable = 0x01826648;

        public int xF48_Neg1 => ReadOffset<int>(0xF48);
        public int xF4C => ReadOffset<int>(0xF4C);
        //public int x1678 => ReadOffset<int>(0x1678);

        public int x167c_ItemAnnId => ReadOffset<int>(0x167c);

    }

    public class UXButton : UXLabel
    {
        public new const int SizeOf = 0xE40; //3648
        public new const int VTable = 0x018265A0;

        public int _xB78 => ReadOffset<int>(0xB78);
        public int xB80_Neg1 => ReadOffset<int>(0xB80);
        public int xB84_Neg1 => ReadOffset<int>(0xB84);
        public int xB88_Neg1 => ReadOffset<int>(0xB88);
        public int xB8C_Neg1 => ReadOffset<int>(0xB8C);
        public int xB90_Neg1 => ReadOffset<int>(0xB90);
        public int xB94_Neg1 => ReadOffset<int>(0xB94);
        public int xB98_Neg1 => ReadOffset<int>(0xB98);
        public int xB9C_Neg1 => ReadOffset<int>(0xB9C);
        public int xBA0_Neg1 => ReadOffset<int>(0xBA0);
        public int xBA4_Neg1 => ReadOffset<int>(0xBA4);
        public int xBA8_Neg1 => ReadOffset<int>(0xBA8);
        public int xBAC_Neg1 => ReadOffset<int>(0xBAC);
        public int _xBB0 => ReadOffset<int>(0xBB0);
        public int xBB4_Neg1 => ReadOffset<int>(0xBB4);
        public int xBB8_Neg1 => ReadOffset<int>(0xBB8);
        public int xBBC_Neg1 => ReadOffset<int>(0xBBC);
        public int xBC0_Neg1 => ReadOffset<int>(0xBC0);
        public int _xBC4 => ReadOffset<int>(0xBC4);
        public int _xBC8 => ReadOffset<int>(0xBC8);
        public int _xBCC => ReadOffset<int>(0xBCC);
        public int _xBD0 => ReadOffset<int>(0xBD0);
        public int _xBD4 => ReadOffset<int>(0xBD4);
        public int _xBD8 => ReadOffset<int>(0xBD8);
        public int _xBDC => ReadOffset<int>(0xBDC);
        public int _xBE0 => ReadOffset<int>(0xBE0);
        public int _xBE4 => ReadOffset<int>(0xBE4);
        public int _xBE8 => ReadOffset<int>(0xBE8);
        public int _xBEC => ReadOffset<int>(0xBEC);
        public int _xBF0 => ReadOffset<int>(0xBF0);
        public int _xBF4 => ReadOffset<int>(0xBF4);
        public int _xBF8 => ReadOffset<int>(0xBF8);
        public int _xBFC => ReadOffset<int>(0xBFC);
        public int _xC00 => ReadOffset<int>(0xC00);
        public int _xC04 => ReadOffset<int>(0xC04);
        public int _xC08 => ReadOffset<int>(0xC08);
        public int _xC0C => ReadOffset<int>(0xC0C);
        public int _xC10 => ReadOffset<int>(0xC10);
        public int _xC14 => ReadOffset<int>(0xC14);
        public int _xC18 => ReadOffset<int>(0xC18);
        public int _xC1C => ReadOffset<int>(0xC1C);
        public int _xC20 => ReadOffset<int>(0xC20);
        public int _xC24 => ReadOffset<int>(0xC24);
        public int _xC28 => ReadOffset<int>(0xC28);
        public int _xC2C => ReadOffset<int>(0xC2C);
        public int _xC30 => ReadOffset<int>(0xC30);
        public int _xC34 => ReadOffset<int>(0xC34);
        public int _xC38 => ReadOffset<int>(0xC38);
        public int _xC3C => ReadOffset<int>(0xC3C);
        public int _xC40 => ReadOffset<int>(0xC40);
        public int _xC44 => ReadOffset<int>(0xC44);
        public int _xC48 => ReadOffset<int>(0xC48);
        public int _xC4C => ReadOffset<int>(0xC4C);
        public int _xC50 => ReadOffset<int>(0xC50);
        public int _xC54 => ReadOffset<int>(0xC54);
        public int _xC58 => ReadOffset<int>(0xC58);
        public int _xC5C => ReadOffset<int>(0xC5C);
        public int _xC60 => ReadOffset<int>(0xC60);
        public int _xC64 => ReadOffset<int>(0xC64);
        public int _xC68 => ReadOffset<int>(0xC68);
        public int _xC6C => ReadOffset<int>(0xC6C);
        public int _xC70 => ReadOffset<int>(0xC70);
        public int _xC74 => ReadOffset<int>(0xC74);
        public int _xC78 => ReadOffset<int>(0xC78);
        public int _xC7C => ReadOffset<int>(0xC7C);
        public int _xC80 => ReadOffset<int>(0xC80);
        public int _xC84 => ReadOffset<int>(0xC84);
        public int _xC88 => ReadOffset<int>(0xC88);
        public int _xC8C => ReadOffset<int>(0xC8C);
        public int _xC90 => ReadOffset<int>(0xC90);
        public int _xC94 => ReadOffset<int>(0xC94);
        public int _xC98 => ReadOffset<int>(0xC98);
        public int _xC9C => ReadOffset<int>(0xC9C);
        public int _xCA0 => ReadOffset<int>(0xCA0);
        public int _xCA4 => ReadOffset<int>(0xCA4);
        public int _xCA8 => ReadOffset<int>(0xCA8);
        public int _xCAC => ReadOffset<int>(0xCAC);
        public int _xCB0 => ReadOffset<int>(0xCB0);
        public int _xCB4 => ReadOffset<int>(0xCB4);
        public int _xCB8 => ReadOffset<int>(0xCB8);
        public int _xCBC => ReadOffset<int>(0xCBC);
        public int _xCC0 => ReadOffset<int>(0xCC0);
        public int _xCC4 => ReadOffset<int>(0xCC4);
        public int _xCC8 => ReadOffset<int>(0xCC8);
        public int _xCCC => ReadOffset<int>(0xCCC);
        public int _xCD0 => ReadOffset<int>(0xCD0);
        public int _xCD4 => ReadOffset<int>(0xCD4);
        public int _xCD8 => ReadOffset<int>(0xCD8);
        public int _xCDC => ReadOffset<int>(0xCDC);
        public int _xCE0 => ReadOffset<int>(0xCE0);
        public int _xCE4 => ReadOffset<int>(0xCE4);
        public int _xCE8 => ReadOffset<int>(0xCE8);
        public int _xCEC => ReadOffset<int>(0xCEC);
        public int _xCF0 => ReadOffset<int>(0xCF0);
        public int _xCF4 => ReadOffset<int>(0xCF4);
        public int _xCF8 => ReadOffset<int>(0xCF8);
        public int _xCFC => ReadOffset<int>(0xCFC);
        public int _xD00 => ReadOffset<int>(0xD00);
        public int _xD04 => ReadOffset<int>(0xD04);
        public int _xD08 => ReadOffset<int>(0xD08);
        public int _xD0C => ReadOffset<int>(0xD0C);
        public int _xD10 => ReadOffset<int>(0xD10);
        public int _xD14 => ReadOffset<int>(0xD14);
        public int _xD18 => ReadOffset<int>(0xD18);
        public int _xD1C => ReadOffset<int>(0xD1C);
        public int _xD20 => ReadOffset<int>(0xD20);
        public int _xD24 => ReadOffset<int>(0xD24);
        public int _xD28 => ReadOffset<int>(0xD28);
        public int _xD2C => ReadOffset<int>(0xD2C);
        public int _xD30 => ReadOffset<int>(0xD30);
        public int _xD34 => ReadOffset<int>(0xD34);
        public int _xD38 => ReadOffset<int>(0xD38);
        public int _xD3C => ReadOffset<int>(0xD3C);
        public int _xD40 => ReadOffset<int>(0xD40);
        public int _xD44 => ReadOffset<int>(0xD44);
        public int _xD48 => ReadOffset<int>(0xD48);
        public int _xD4C => ReadOffset<int>(0xD4C);
        public int _xD50 => ReadOffset<int>(0xD50);
        public int _xD54 => ReadOffset<int>(0xD54);
        public int _xD58 => ReadOffset<int>(0xD58);
        public int _xD5C => ReadOffset<int>(0xD5C);
        public int _xD60 => ReadOffset<int>(0xD60);
        public int _xD64 => ReadOffset<int>(0xD64);
        public int _xD68 => ReadOffset<int>(0xD68);
        public int _xD6C => ReadOffset<int>(0xD6C);
        public int _xD70 => ReadOffset<int>(0xD70);
        public int _xD74 => ReadOffset<int>(0xD74);
        public int _xD78 => ReadOffset<int>(0xD78);
        public int _xD7C => ReadOffset<int>(0xD7C);
        public int _xD80 => ReadOffset<int>(0xD80);
        public int _xD84 => ReadOffset<int>(0xD84);
        public int _xD88 => ReadOffset<int>(0xD88);
        public int _xD8C => ReadOffset<int>(0xD8C);
        public int _xD90 => ReadOffset<int>(0xD90);
        public int _xD94 => ReadOffset<int>(0xD94);
        public int _xD98 => ReadOffset<int>(0xD98);
        public int _xD9C => ReadOffset<int>(0xD9C);
        public int _xDA0 => ReadOffset<int>(0xDA0);
        public int _xDA4 => ReadOffset<int>(0xDA4);
        public int _xDA8 => ReadOffset<int>(0xDA8);
        public int _xDAC => ReadOffset<int>(0xDAC);
        public int _xDB0 => ReadOffset<int>(0xDB0);
        public int _xDB4 => ReadOffset<int>(0xDB4);
        public int _xDB8 => ReadOffset<int>(0xDB8);
        public int _xDBC => ReadOffset<int>(0xDBC);
        public int _xDC0 => ReadOffset<int>(0xDC0);
        public int _xDC4 => ReadOffset<int>(0xDC4);
        public int _xDC8 => ReadOffset<int>(0xDC8);
        public int _xDCC => ReadOffset<int>(0xDCC);
        public int _xDD0 => ReadOffset<int>(0xDD0);
        public int _xDD4 => ReadOffset<int>(0xDD4);
        public int _xDD8 => ReadOffset<int>(0xDD8);
        public int _xDDC => ReadOffset<int>(0xDDC);
        public int _xDE0 => ReadOffset<int>(0xDE0);
        public int _xDE4 => ReadOffset<int>(0xDE4);
        public int _xDE8 => ReadOffset<int>(0xDE8);
        public int _xDEC => ReadOffset<int>(0xDEC);
        public int _xDF0 => ReadOffset<int>(0xDF0);
        public int _xDF4 => ReadOffset<int>(0xDF4);
        public int _xDF8 => ReadOffset<int>(0xDF8);
        public int _xDFC => ReadOffset<int>(0xDFC);
        public int _xE00 => ReadOffset<int>(0xE00);
        public int _xE04 => ReadOffset<int>(0xE04);
        public int _xE08 => ReadOffset<int>(0xE08);
        public int _xE0C => ReadOffset<int>(0xE0C);
        public int _xE10 => ReadOffset<int>(0xE10);
        public int _xE14 => ReadOffset<int>(0xE14);
        public int _xE18 => ReadOffset<int>(0xE18);
        public int _xE1C => ReadOffset<int>(0xE1C);
        public int _xE20 => ReadOffset<int>(0xE20);
        public int _xE24 => ReadOffset<int>(0xE24);
        public int _xE28 => ReadOffset<int>(0xE28);
        public int _xE2C => ReadOffset<int>(0xE2C);
        public int _xE30 => ReadOffset<int>(0xE30);
        public int _xE34 => ReadOffset<int>(0xE34);
        public int _xE38 => ReadOffset<int>(0xE38);
        public int _xE3C => ReadOffset<int>(0xE3C);
    }

    public class UXItemsControl : UXItemsControlBase
    {
        public new const int SizeOf = 0xA20; //2592
        public new const int VTable = 0x017D2CA8;

        public UXRect x468_UIRect => ReadOffset<UXRect>(0x468);
        public int x478_Neg1 => ReadOffset<int>(0x478);
        public int x47C => ReadOffset<int>(0x47C);
        public int x480 => ReadOffset<int>(0x480);
        public int x484 => ReadOffset<int>(0x484);
        public int x488 => ReadOffset<int>(0x488);
        public float x48C => ReadOffset<float>(0x48C);
        public float x490 => ReadOffset<float>(0x490);
        public int x494_Neg1 => ReadOffset<int>(0x494);
        public int x498_Neg1 => ReadOffset<int>(0x498);
        public int x49C => ReadOffset<int>(0x49C);
        public int x4A0_Flags => ReadOffset<int>(0x4A0);
        public int x4A4 => ReadOffset<int>(0x4A4);
        public int x4A8_Neg1_Anim2DSnoId => ReadOffset<int>(0x4A8);
        public int x4AC_StructStart_Min16Bytes_Anim2DRelated => ReadOffset<int>(0x4AC);
        public int _x4B0 => ReadOffset<int>(0x4B0);
        public int _x4B4 => ReadOffset<int>(0x4B4);
        public int _x4B8 => ReadOffset<int>(0x4B8);
        public UXRect x4BC_UIRect_1600x1200 => ReadOffset<UXRect>(0x4BC);
        public UXRect x4CC_UIRect => ReadOffset<UXRect>(0x4CC);
        public float x4DC_Neg1f => ReadOffset<float>(0x4DC);
        public float x4E0_Neg1f => ReadOffset<float>(0x4E0);
        public float x4E4_Neg1f => ReadOffset<float>(0x4E4);
        public float x4E8_Neg1f => ReadOffset<float>(0x4E8);
        public int x4EC => ReadOffset<int>(0x4EC);
        public int x4F0 => ReadOffset<int>(0x4F0);
        public int x4F4 => ReadOffset<int>(0x4F4);
        public int x4F8 => ReadOffset<int>(0x4F8);
        public int x4FC_Ptr_32Bytes_Methods => ReadOffset<int>(0x4FC);
        public string x500_String128 => ReadString(0x500, 128);
        public int x580 => ReadOffset<int>(0x580);
        public int x584 => ReadOffset<int>(0x584);
        public float x588 => ReadOffset<float>(0x588);
        public float x58C => ReadOffset<float>(0x58C);
        public int x590 => ReadOffset<int>(0x590);
        public int x594 => ReadOffset<int>(0x594);
        public int x598 => ReadOffset<int>(0x598);
        public int x59C => ReadOffset<int>(0x59C);
        public int x5A0 => ReadOffset<int>(0x5A0);
        public int x5A4 => ReadOffset<int>(0x5A4);
        public string x5A8_String512 => ReadString(0x5A8, 512);
        public string x7A8_String512 => ReadString(0x7A8, 512);
        public int x9A8_Neg1 => ReadOffset<int>(0x9A8);
        public int x9AC_Neg1 => ReadOffset<int>(0x9AC);
        public int x9B0 => ReadOffset<int>(0x9B0);
        public UXRect x9B4_UIRect => ReadOffset<UXRect>(0x9B4);
        public UXRect x9C4_UIRect_1600x1200 => ReadOffset<UXRect>(0x9C4);
        public UXRect x9D4_UIRect => ReadOffset<UXRect>(0x9D4);
        public float x9E4 => ReadOffset<float>(0x9E4);
        public float x9E8 => ReadOffset<float>(0x9E8);
        public float x9EC_1f => ReadOffset<float>(0x9EC);
        public float x9F0_1f => ReadOffset<float>(0x9F0);
        public UXRect x9F4_UIRect => ReadOffset<UXRect>(0x9F4);
        public int xA04 => ReadOffset<int>(0xA04);
        public int xA08 => ReadOffset<int>(0xA08);
        public int xA0C => ReadOffset<int>(0xA0C);
        public int xA10 => ReadOffset<int>(0xA10);
        public int xA14 => ReadOffset<int>(0xA14);
        public float xA18_1f => ReadOffset<float>(0xA18);
        public int xA1C => ReadOffset<int>(0xA1C);
    }



    public class UXItemsControlBase : UXControl
    {
        public new const int SizeOf = 0x468; //1128
        public new const int VTable = 0x017D2D80;

        public LightVector<UXReference> Children => ReadObject<LightVector<UXReference>>(0x450);
        public UXControl x45C_Ptr_UIControl => ReadPointer<UXControl>(0x45C);
        public UXControl x460_Ptr_UIControl => ReadPointer<UXControl>(0x460);
        public int _x464 => ReadOffset<int>(0x464);
    }

    public class LightVector<T> : MemoryWrapper, IEnumerable<T> where T : MemoryWrapper, new()
    {
        public const int SizeOf = 12;
        public IntPtr ItemsAddr => ReadOffset<IntPtr>(0x00);
        public int Size => ReadOffset<int>(0x04);
        public int Capacity => ReadOffset<int>(0x08);
        public T this[int index] => Create<T>(ItemsAddr + index * ItemSize);
        public int ItemSize => TypeUtil<T>.SizeOf;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator()
        {
            var ptr = ItemsAddr;
            var size = Size;
            var itemSize = ItemSize;
            for (int i = 0; i < size; i++)
                yield return Create<T>(ptr + i * itemSize);
        }

        public override string ToString() => $"Items: {Size} / {Capacity}";
    }

}
