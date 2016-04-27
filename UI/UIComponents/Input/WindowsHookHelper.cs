using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Application = System.Windows.Application;

namespace Trinity.UI.UIComponents.Input
{
    /// <summary>
    /// Credit to Joel Abrahamsson
    /// http://joelabrahamsson.com/detecting-mouse-and-keyboard-input-with-net/
    /// This is a modified version
    /// </summary>  
    internal class WindowsHookHelper
    {
        internal delegate IntPtr HookDelegate(
            Int32 Code, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "CallNextHookEx")]
        internal static extern IntPtr CallNextHookEx(IntPtr hHook, Int32 nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "UnhookWindowsHookEx")]
        internal static extern IntPtr UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("User32.dll", EntryPoint = "SetWindowsHookEx")]
        internal static extern IntPtr SetWindowsHookEx(Int32 idHook, HookDelegate lpfn, IntPtr hmod, Int32 dwThreadId);

        [DllImport("user32.dll")]
        internal static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

        //      public const int ‫‭⁮‫‍‪⁪⁭⁯‌‍‮‫⁫⁭‭‍⁯​‌‫⁯‫‌⁪⁬‫⁭​​⁮‪⁭⁮‍‎⁫‭​⁮‮ = 32;

        //      public const int ‮​‌‫‍‬⁮⁭‍‭‎‭‬‎‭‍‪⁪⁮‭‌‬‏⁪⁭‬‌‪⁭⁫‍⁬‫⁭‭‪‮‭⁫‬‮ = 128;

        //      public const int ‌‭‌‭‪‏‎⁪‏⁮⁬⁪⁪⁯‫⁪‪‎‌​⁬‭‍⁭⁬‎⁪​⁯⁫⁪⁬‏‍‪‏‏⁭⁯‪‮ = 134217728;

        //      public const int ⁬‪⁫⁭‭⁫‫‫⁯‏‎‏⁯⁮‭‬⁮‭‮‏‬⁭‬‎⁪⁫‮‌‭⁮⁯‮‍​⁮‮‏​‏⁮‮ = -2147483648;

        //      public const int ⁫⁯‫‎‍⁮‮‎‎‬‮⁭‭⁪⁮‭⁫​‭⁫⁯‍‮⁭‬‎‮‏‌⁪⁬‍‭​⁯⁫‭‬‌‭‮ = 1073741824;

        //public const int ⁮‎‍‬‪‎‭‪⁮‍‌‍‍⁯‫⁬⁪⁪⁭⁫⁫​⁯‎⁪‭⁮‮‍‫⁮⁭‭‎‪‮‮⁮‌⁫‮ = -20;

        //[DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool ‏‏‫‪‬⁮‍‭‌⁯⁪‌⁪​⁬‬‭‍‮‮‪⁪‏‎‌⁮‭‪‌‫‫⁯‫‌⁮⁭‪⁬⁮‍‮(IntPtr);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetForegroundWindow​⁫⁪⁪⁯⁪‎‫‌‍‌⁭‭⁬‬⁫⁫‮‬​⁭⁭⁭⁬‌‮⁭‍⁫⁮‪⁫‮⁫​⁬⁯⁬⁮‮();

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        internal static extern bool GetCursorPos(ref Point lpPoint);

        //[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool ⁫‌‌‍‭‪⁭‎⁯‭‬​⁫‎‬‪⁬⁮⁮‬‎‎⁫‮‏‌‭⁬‍‬⁯‫‭⁫⁪⁯‮​⁪⁫‮(IntPtr, IntPtr, int, int, int, int, ⁯‬⁬⁮‌⁮‮‫⁯‪‭‪⁭‫⁭‭‭​‮‬⁫‮‎⁮‮‌⁫⁬⁪‭‪⁭‬⁭⁬⁬⁭‬⁫‭‮);

        //[DllImport("user32.dll", EntryPoint = "GetWindowInfo", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool ‬⁪‮⁪‍⁮⁮‫‪⁪⁯​‍‭‫⁯⁯‎​‫⁪‮⁯‏⁫‫‬‬⁭‮​‪​‍‏⁭‎⁭‭‪‮(IntPtr, ref ‬‫‮⁬‭⁬‫‌⁫⁮‫⁫‌⁯‮‫‏‬‭‮⁬‏‪‬‬⁬‬⁮‪‎‎‍​‭‬‏‭‮‬⁯‮);

        //[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        //public static extern int ‬⁯‌⁯⁫‮‌⁯‏⁮‭​⁯​‭‮⁪‍⁮‭‬‮⁮‪‮‭​‌⁬⁭⁯‏​‏⁪⁬‏⁪‫‭‮(IntPtr, int);

        //[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        //public static extern int ‭​‍⁪‌‌‮⁯‍⁭⁭⁯⁬⁯‭⁮⁬​⁪‫⁭‌‪‮⁫⁮‏‫⁮‮⁫‏‭⁬‌‮⁯‫‫⁮‮(IntPtr, int, int);

        //[DllImport("user32.dll", EntryPoint = "GetWindow", SetLastError = true)]
        //public static extern IntPtr ⁫⁫‍‬⁪⁭⁪‭‪‌‬‬‌‮‮⁮⁭‪‮‮‪‮‍‪​‭‫‌‎⁮⁭‬‮⁮⁭‪⁫⁬‍‫‮(IntPtr, ⁭⁮‌⁯‪‮‮⁯‍‬‌‬⁪⁫⁬‍⁯⁯‎‭‪‏‏‌‌⁭‌‌‪​‮‍⁯⁬‬⁭⁪‫⁯‍‮);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowsHookEx", SetLastError = true)]
        //public static extern IntPtr ⁪‌‭‎‮‪‏⁬‎‬⁫‏‏⁮⁭⁯⁫‍‪‫‎‭‎‫‍‌‪​⁮⁭‭‏‬‭‍‏​⁬⁮⁪‮(‮⁭‮⁬⁪‏‏⁮‍‫‬‎⁬‮⁭⁯‌‮‍‭⁫‌⁫⁯⁮⁮⁭⁮‏‬‏⁬⁭‭‫​‪‫‭⁯‮, ‎⁭⁭‏‫‍⁫⁭⁬‮‪‍‌⁪‪​‫‍‏⁫‎​‍⁮‌‪⁬⁫⁪⁬‪‎‮‌⁪‪‫⁫⁬‏‮, IntPtr, uint);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "UnhookWindowsHookEx", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool ‏‌‫⁬‮⁮‫‬‌‫⁫⁯⁯‪‍‫‭‌‫‏‮‎⁪‏‎⁯⁫‍⁬‪‎‍‭⁫‬⁪‎⁮‏‪‮(IntPtr);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "CallNextHookEx", SetLastError = true)]
        //public static extern IntPtr ‬‍⁭‎‎‍⁫‫‪⁪‬⁪⁬⁯‭‪‌​‭‍⁯‬‭‍⁫‫‌⁯​​⁯‬⁯‏‪‍⁪⁭‪‬‮(IntPtr, int, IntPtr, IntPtr);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint GetWindowThreadProcessId⁪⁫⁭‍⁮⁬‌⁪‮⁭‏⁪‪​‪‬‭‎‍‎⁯⁬⁯⁫‬‬‍⁮‍⁫⁯‪‮‫‌‌⁮‏‌‎‮(IntPtr arg1, out uint arg2);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        //[DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
        //public static extern IntPtr ⁪‬‮⁭‭‏⁭‎⁭⁯‪⁭‫​⁪‍⁭‎‮⁮⁯⁫⁮⁭⁭‎‏⁯⁬⁪‏⁬‎‬‏⁮‮⁫⁭⁭‮(IntPtr);

        //[DllImport("kernel32.dll", EntryPoint = "FormatMessage", SetLastError = true)]
        //public static extern int ⁭⁪⁭⁬‫‌‮‎‮‪‍⁪‎⁪​‮⁭⁯⁬⁯‮‌⁪⁫⁭⁮‮⁮‭⁫⁫⁯‮⁬⁬‬⁯‮‎⁭‮(‌‫​⁯​‍⁭⁪‭​‏⁯​‌‮‫​‪‍‭⁫⁮‭⁮‌‏‭‪​‫‫‌⁪‏‬‫⁯‭‬⁬‮, IntPtr, uint, uint, ref IntPtr, uint, IntPtr);

        //[DllImport("user32.dll", EntryPoint = "MapVirtualKey")]
        //public static extern uint ⁪‮⁮‏‎​‏‎⁭‍⁮‬⁬⁯‬⁬‎‫⁭‪‬⁪⁫⁭‫‏⁬‭‎‬⁭‭⁪⁪​‍‬⁫‮‮(uint, uint);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", CharSet = CharSet.Auto)]
        internal static extern uint GetCurrentThreadId‏⁬⁯‍‏⁬‫‌‌⁬‏⁮‭⁪⁫‮‎⁫‭‏‪⁭⁫⁫‭⁭‪‌⁪⁪‮⁮⁬⁪​‎‮⁫⁭⁭‮();

        //[DllImport("user32.dll", EntryPoint = "ToAscii")]
        //public static extern int ⁯‌‪‎‌‍⁪⁭⁮⁭⁫‍‮⁬​⁪⁬⁮‍⁯‎‬‫‫​‏‍⁪‮⁭‍​‫⁫‏‌‍​⁬⁪‮(uint, uint, byte[], byte[], uint);

        //[DllImport("user32.dll", EntryPoint = "GetKeyboardState", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool ‌‎⁯⁮⁯⁪⁪‏‭‬⁭‬‍‎‌⁭‪‭‭‏⁮‏⁫⁬‌⁫​⁫​‭‌​⁬⁭‎⁭‏⁭‏‍‮(byte[]);

        //[DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled")]
        //public static extern int ⁭​⁯‍‬⁫‍‎‮‏⁯‫​​⁫‫‪‮⁫‭‍‍‮⁪‫‏‮⁯‌⁯⁫⁪⁭‌‬‌⁫⁯‎‏‮(out bool);

        [DllImport("user32.dll", EntryPoint = "GetKeyState")]
        internal static extern short GetKeyState‪​⁮⁬‏⁪⁭‪‍‮‬‭⁫⁭⁪‭⁫‏⁯‎‎‬⁭⁭⁫‍‭⁫‍‏‭⁯⁭‍‌‫‪⁬‬‌‮(int nVirtKey);

        //[DllImport("user32.dll", EntryPoint = "GetClientRect")]
        //public static extern bool ‪​⁮⁬‏⁪⁭‪‍‮‬‭⁫⁭⁪‭⁫‏⁯‎‎‬⁭⁭⁫‍‭⁫‍‏‭⁯⁭‍‌‫‪⁬‬‌‮(IntPtr, out ‬⁫⁭⁭​‮‫⁯‮⁬‭‭‪‪‪‎‮‫⁫‏⁫⁬⁯‏‬⁯‪‎⁫⁮‎⁪​‌⁬​‫⁪‮‮);

        //[DllImport("user32.dll", EntryPoint = "ClientToScreen")]
        //public static extern bool ⁮‮‭⁪⁯⁭​​⁪⁯‌‮‎‮⁫⁮⁪‏‭‫‍‬⁫⁯‬‌⁪​‏⁬‪⁯⁯⁬‭‌‍‌‫‮(IntPtr, out Point);

        //public static uint ‎‪⁬‫‬‌‫⁭⁮‎‎‭⁮‏‌⁭‫‬⁯‌‮‭‬​‪⁮‫‍‬‬⁭⁪‌‮‪‏⁭⁪‍‍‮(IntPtr intPtr)
        //{
        //	uint num;
        //	return ‭‏​‏‭‬⁫‬‪⁬‪‪⁬⁯‭​‍‪‪⁯‭⁯⁫‬⁬‫⁬‌‏⁯‮⁪⁯⁬‏‫⁪⁯⁮⁪‮.⁪⁫⁭‍⁮⁬‌⁪‮⁭‏⁪‪​‪‬‭‎‍‎⁯⁬⁯⁫‬‬‍⁮‍⁫⁯‪‮‫‌‌⁮‏‌‎‮(intPtr, out num);
        //}

        //public static uint ⁬‬⁫‎‪⁭‮‍⁯‪⁭⁮‪‭‮⁪⁬‎⁭‫⁫⁪‪‭‪‬‍‌‪‭​‮‏‏‍‏⁪‬‮⁫‮(IntPtr intPtr)
        //{
        //	uint result;
        //	‭‏​‏‭‬⁫‬‪⁬‪‪⁬⁯‭​‍‪‪⁯‭⁯⁫‬⁬‫⁬‌‏⁯‮⁪⁯⁬‏‫⁪⁯⁮⁪‮.⁪⁫⁭‍⁮⁬‌⁪‮⁭‏⁪‪​‪‬‭‎‍‎⁯⁬⁯⁫‬‬‍⁮‍⁫⁯‪‮‫‌‌⁮‏‌‎‮(intPtr, out result);
        //	return result;
        //}

        //public static ‬‫‮⁬‭⁬‫‌⁫⁮‫⁫‌⁯‮‫‏‬‭‮⁬‏‪‬‬⁬‬⁮‪‎‎‍​‭‬‏‭‮‬⁯‮ ‬⁪‮⁪‍⁮⁮‫‪⁪⁯​‍‭‫⁯⁯‎​‫⁪‮⁯‏⁫‫‬‬⁭‮​‪​‍‏⁭‎⁭‭‪‮(IntPtr intPtr)
        //{
        //	‬‫‮⁬‭⁬‫‌⁫⁮‫⁫‌⁯‮‫‏‬‭‮⁬‏‪‬‬⁬‬⁮‪‎‎‍​‭‬‏‭‮‬⁯‮ result = new ‬‫‮⁬‭⁬‫‌⁫⁮‫⁫‌⁯‮‫‏‬‭‮⁬‏‪‬‬⁬‬⁮‪‎‎‍​‭‬‏‭‮‬⁯‮(new bool?(true));
        //	‭‏​‏‭‬⁫‬‪⁬‪‪⁬⁯‭​‍‪‪⁯‭⁯⁫‬⁬‫⁬‌‏⁯‮⁪⁯⁬‏‫⁪⁯⁮⁪‮.‬⁪‮⁪‍⁮⁮‫‪⁪⁯​‍‭‫⁯⁯‎​‫⁪‮⁯‏⁫‫‬‬⁭‮​‪​‍‏⁭‎⁭‭‪‮(intPtr, ref result);
        //	return result;
        //}

        //public static string ⁭‪‮‭⁪​‏‫‪‭⁮⁫⁯‏⁭⁮‬⁬‬⁪‬‏‎‌⁭‍⁯⁫‌⁮‌‭​⁬‎⁭‪⁬⁯⁭‮(int num)
        //{
        //	string result;
        //	try
        //	{
        //		IntPtr intPtr = IntPtr.Zero;
        //		if (‭‏​‏‭‬⁫‬‪⁬‪‪⁬⁯‭​‍‪‪⁯‭⁯⁫‬⁬‫⁬‌‏⁯‮⁪⁯⁬‏‫⁪⁯⁮⁪‮.⁭⁪⁭⁬‫‌‮‎‮‪‍⁪‎⁪​‮⁭⁯⁬⁯‮‌⁪⁫⁭⁮‮⁮‭⁫⁫⁯‮⁬⁬‬⁯‮‎⁭‮(‌‫​⁯​‍⁭⁪‭​‏⁯​‌‮‫​‪‍‭⁫⁮‭⁮‌‏‭‪​‫‫‌⁪‏‬‫⁯‭‬⁬‮.FORMAT_MESSAGE_ALLOCATE_BUFFER | ‌‫​⁯​‍⁭⁪‭​‏⁯​‌‮‫​‪‍‭⁫⁮‭⁮‌‏‭‪​‫‫‌⁪‏‬‫⁯‭‬⁬‮.FORMAT_MESSAGE_IGNORE_INSERTS | ‌‫​⁯​‍⁭⁪‭​‏⁯​‌‮‫​‪‍‭⁫⁮‭⁮‌‏‭‪​‫‫‌⁪‏‬‫⁯‭‬⁬‮.FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, (uint)num, 0u, ref intPtr, 0u, IntPtr.Zero) == 0)
        //		{
        //			result = "Unable to get error code string from System - Error " + Marshal.GetLastWin32Error().ToString();
        //		}
        //		else
        //		{
        //			string arg_48_0 = Marshal.PtrToStringAnsi(intPtr);
        //			intPtr = ‭‏​‏‭‬⁫‬‪⁬‪‪⁬⁯‭​‍‪‪⁯‭⁯⁫‬⁬‫⁬‌‏⁯‮⁪⁯⁬‏‫⁪⁯⁮⁪‮.⁪‬‮⁭‭‏⁭‎⁭⁯‪⁭‫​⁪‍⁭‎‮⁮⁯⁫⁮⁭⁭‎‏⁯⁬⁪‏⁬‎‬‏⁮‮⁫⁭⁭‮(intPtr);
        //			result = arg_48_0;
        //		}
        //	}
        //	catch (Exception ex)
        //	{
        //		result = "Unable to get error code string from System -> " + ex.ToString();
        //	}
        //	return result;
        //}

        internal static IntPtr GetCurrentHandle()
        {
            IntPtr handle = IntPtr.Zero;            
            Application.Current.Dispatcher.Invoke(() =>
            {
                handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            });
            return handle;
        }

        //public static bool ApplicationIsActivated(IntPtr handle)
        //{
        //    if (handle == IntPtr.Zero) return false;            
        //    var procId = Process.GetCurrentProcess().Id;
        //    int activeProcId;
        //    GetWindowThreadProcessId(handle, out activeProcId);
        //    return activeProcId == procId;
        //}

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);


    }
}