using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Logger = Trinity.Framework.Helpers.Logger;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Trinity.UI.UIComponents.Input
{
    /// <summary>
    /// Credit to Joel Abrahamsson
    /// http://joelabrahamsson.com/detecting-mouse-and-keyboard-input-with-net/
    /// This is a modified version
    /// </summary>  
    internal class MouseInput : IDisposable
    {
        internal event EventHandler<EventArgs> MouseMoved;
        internal event EventHandler<MouseButtonEventArgs> MouseLeftDown;

        private WindowsHookHelper.HookDelegate _mouseMoveDelegate;

        private IntPtr _mouseMoveHandle;
        private IntPtr _mouseLeftDownHandle;

        private const int WH_MOUSE_LL = 14;
        private bool disposed;

        internal MouseInput()
        {
            _mouseMoveDelegate = MouseMoveHookDelegate;
            _mouseMoveHandle = WindowsHookHelper.SetWindowsHookEx(WH_MOUSE_LL, _mouseMoveDelegate, IntPtr.Zero, 0);
        }

        private enum MouseMessages
        {
            WM_NULL = 0x0,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CursorPoint
        {
            public int X;
            public int Y;
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms644970(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MSLLHOOKSTRUCT
        {
            public CursorPoint Point;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        internal class RichMouseButtonEventArgs : MouseButtonEventArgs
        {
            public readonly IntPtr LParam;
            public readonly IntPtr WParam;
            public readonly MSLLHOOKSTRUCT MouseInfo;
            public readonly int Code;

            public RichMouseButtonEventArgs(int code, IntPtr lParam, IntPtr wParam) : 
                base(Mouse.PrimaryDevice, 0, GetMouseButton(wParam))
            {
                Code = code;
                MouseInfo = GetMouseInfo(lParam);
                LParam = lParam;
                WParam = wParam;                
            }

            private static MouseButton GetMouseButton(IntPtr wParam)
            {                
                switch ((MouseMessages)wParam)
                {
                    case MouseMessages.WM_LBUTTONDOWN: return MouseButton.Left;
                    case MouseMessages.WM_RBUTTONDOWN: return MouseButton.Right;
                }
                return default(MouseButton);
            }

            internal static MSLLHOOKSTRUCT GetMouseInfo(IntPtr lParam)
            {
                return (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            }

            public override string ToString()
            {
                return $@"RichMouseButtonEventArgs: LParam={LParam} WParam={WParam}, X={MouseInfo.Point.X} Y={MouseInfo.Point.Y}
                    ButtonState={ButtonState}, RawTime={MouseInfo.Time} ExtraInfo={MouseInfo.ExtraInfo} MouseData={MouseInfo.MouseData} 
                    Flags={MouseInfo.Flags} Handled={Handled} LeftButton={LeftButton} RightButton={RightButton}";
            }
        }

        private IntPtr MouseMoveHookDelegate(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return WindowsHookHelper.CallNextHookEx(_mouseMoveHandle, code, wParam, lParam);

            if (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MouseLeftDown?.Invoke(this, new RichMouseButtonEventArgs(code, lParam, wParam));
            }
            else
            {
                MouseMoved?.Invoke(this, new EventArgs());
            }

            return WindowsHookHelper.CallNextHookEx(_mouseMoveHandle, code, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (_mouseMoveHandle != IntPtr.Zero)
                    WindowsHookHelper.UnhookWindowsHookEx(_mouseMoveHandle);

                disposed = true;
            }
        }

        ~MouseInput()
        {
            Dispose(false);
        }
    }
}
