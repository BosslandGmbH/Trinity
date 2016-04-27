using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using System.Runtime.InteropServices;

namespace Bitwise
{
    //    var x = new Union { Number = 0xDEADBEEF };
    //    Logger.Log(string.Format("{0:X} {1:X} {2:X}", x.Number, x.High, x.Low));

    //    x.High = 0x1234;
    //    Logger.Log(string.Format("{0:X} {1:X} {2:X}", x.Number, x.High, x.Low));

    //    x.Low = 0x5678;
    //    Logger.Log(string.Format("{0:X} {1:X} {2:X}", x.Number, x.High),

    // https://msdn.microsoft.com/en-us/library/acxa5b99(VS.80).aspx

    [StructLayout(LayoutKind.Explicit)]
    struct Union
    {

        [FieldOffset(0)]
        public uint Number;

        [FieldOffset(0)]
        public ushort Low;

        [FieldOffset(2)]
        public ushort High;
    }

    //LOWORD: System.Int16 y = BitConverter.ToInt16(BitConverter.GetBytes(x), 0);
    //HIWORD: System.Int16 y = BitConverter.ToInt16(BitConverter.GetBytes(x), 2);
}