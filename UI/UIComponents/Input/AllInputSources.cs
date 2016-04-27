using System;
using System.Runtime.InteropServices;

namespace Trinity.UI.UIComponents.Input
{
    /// <summary>
    /// Credit to Joel Abrahamsson
    /// http://joelabrahamsson.com/detecting-mouse-and-keyboard-input-with-net/
    /// </summary>   
    internal class AllInputSources
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        internal DateTime GetLastInputTime()
        {
            var lastInputInfo = new LASTINPUTINFO();            
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            
            GetLastInputInfo(ref lastInputInfo);

            return DateTime.Now.AddMilliseconds(-(Environment.TickCount - lastInputInfo.dwTime));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }
}
