using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using Trinity.Cache;
using Trinity.Framework.Objects.Memory.Debug;
using Trinity.Helpers;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public class Globals : MemoryWrapper
    {
        public Globals(IntPtr ptr) : base(ptr) { }
        public int WorldId => ReadOffset<int>(0x30);
        public int GameTime => ReadOffset<int>(0xC);
        public float RiftSouls => Math.Min(ReadOffset<float>(0xF4), 650);
        public float RiftProgressionPct => RiftSouls / 650 * 100;

        //0x0 0: 0 => 4116 FLAG?
        //0x0 0: 0 => 4244 
        //0x4 4: 521482 => 521786 INT >> related to game time
        //0x8 8: 521483 => 521786 INT >> related to game time       
        //0xC 12: 550540 => 550841 INT >> game time 
        //0x20 32: 550535 => 550836 INT >> game time - 1
        //0x1B 27: 0 => 63 INT 
        //0x30 48: 332336 => 288454 INT
        //0x31 49: 1298 => 83887206 INT
        //0x32 50: 5 => 327684 INT
        //0x30 48: 332336 => 288454 INT >> World Id
        //0xD 13: 2013 => 2014 INT
        //0xD 13: 2243 => 2245 INT
        //0x3C // Bool
        //0x2A 42: 6774 => 9681  INT    
        //0x2A 42: 2719 => 767 INT
        //0x2B 43: 26 => 37  INT
        //0x2B 43: 24 => 69  INT
        // 0x2B 43: 10 => 2 INT
        //0xCE 206: 50236 => 50238  INT
        //0x5C 92: 28 => 15  INT
        //0x5B 91: 7209 => 3881  INT
        //0x54 84: 2993 => 1537  INT
        //x5A 90: 1845629 => 993662  INT
        //0x54 84: 2993 => 1537  INT
        //0x46 70: 40 => 41  INT
        //0x45 69: 10460 => 10496  INT
        //0x3A 58: 96046 => 96092 

        //Floats

        //0x10 16: 0 => 6794 
        //0x10 16: 0 => 6794 
        //0x10 16: 0 => 7043 
        //0x14 20: 7166.041 => 7165.957 
        //0x18 24: 3.39 => 3.27 
        //0x18 24: 1.86 => 1.71 
        //0x18 24: 1.65 => 1.71 
        //0x33 51: 0 => 2048.25 
        //0x8C 140: 661.9476 => 604.4709 
        //0x8C 140: 1157.615 => 1025.011 >> count down timer?
        //0x90 144: 0.4959366 => 0.4959361 
        //0x91 145: -190.9919 => -48893.92 
        //0x94 148: -0.8683588 => -0.868359 
        //0x9C 156: -102.8438 => -212.415 
        //0x9C 156: 167.5908 => 149.7418 
        //0x9D 157: -0.02377972 => -24.41622 
        //0xA0 160: -0.8683584 => -0.8683587 
        //0xA4 164: 0.4959372 => 0.4959366 
        //0xAC 172: 542.8399 => 614.3811 
        //0xAC 172: 604.5238 => 606.7512 
        //0xB0 176: 0.2421083 => 0.2421078 
        //0xB1 177: 3.938795E-23 => 2.976069 
        //0xBC 188: -281.3531 => -256.6293 
        //0xC0 192: 0 => 0.7071061 
        //0xC4 196: 0 => 0.7071075 
        //0xCC 204: -911.991 => -839.7819
        //0xF4 244: 243.3501 => 246.0501 >> Rift Souls
        //0xF5 245: 0 => 0.001953125 

        //public static MemoryScan Scan = Create<MemoryScan>(Instance.BaseAddress).Init(0x124);
    }





}




