using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Zeta.Bot;
using Zeta.Game;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Application = System.Windows.Application;

namespace Trinity.UI.UIComponents.Input
{
    /// <summary>
    /// Primary Credit to Joel Abrahamsson
    /// http://joelabrahamsson.com/detecting-mouse-and-keyboard-input-with-net/
    /// Additional credit to by Emma Burrows
    /// https://nappybar.googlecode.com/svn/Keyboard.cs
    /// This is a modified version    
    /// </summary>  
    internal class KeyboardInput : IDisposable
    {
        internal event EventHandler<RichKeyEventArgs> KeyDown;
        internal event EventHandler<KeyEventArgs> KeyUp;

        private WindowsHookHelper.HookDelegate keyBoardDelegate;
        private IntPtr keyBoardHandle;

        //Keyboard Messages
        private const int WM_NULL = 0x0;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        //Win32Constants
        private const int MK_LBUTTON = 0x0001;
        private const int MK_RBUTTON = 0x0002;
        private const int MK_SHIFT = 0x0004;
        private const int MK_CONTROL = 0x0004;
        private const int MK_MBUTTON = 0x0010;
        private const int MK_XBUTTON1 = 0x0020;
        private const int MK_XBUTTON2 = 0x0040;
        
        //Modifier key constants
        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int VK_CAPITAL = 0x14;

        private bool _disposed;
        private HookOptions _options;

        internal class HookOptions
        {
            public IntPtr HandleThatMustHaveFocus;
            public bool AllowSpecialKeys = true;
            public bool AllowWindowsKey = true;
            public bool PassAllKeysToNext = true;
            public bool AllowControlEscape = true;
            public HashSet<Key> AllowedKeys = new HashSet<Key>();
        }

        internal KeyboardInput(HookOptions options = default(HookOptions))
        {
            _options = options ?? new HookOptions();
            ApplicationHandle = WindowsHookHelper.GetCurrentHandle();
            keyBoardDelegate = KeyboardHookDelegate;
            keyBoardHandle = WindowsHookHelper.SetWindowsHookEx(WH_KEYBOARD_LL, keyBoardDelegate, IntPtr.Zero, 0);
        }

        /// <summary>
        /// Application.Current Handle
        /// </summary>
        internal IntPtr ApplicationHandle { get; set; }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms644967(v=vs.85).aspx
        /// </summary>
        internal struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private IntPtr KeyboardHookDelegate(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                return WindowsHookHelper.CallNextHookEx(
                    keyBoardHandle, code, wParam, lParam);
            }
  
            var lParamStruct = RichKeyEventArgs.GetlParamStruct(lParam);
            var keys = (Keys)lParamStruct.vkCode;
            var modifiers = new KeyModifierResult();
            var realChar = string.Empty;            
            var isKeyDown = (int)wParam == WM_KEYDOWN || (int)wParam == WM_SYSKEYDOWN;
            var isKeyUp = (int)wParam == WM_KEYUP || (int)wParam == WM_SYSKEYUP;
            var key = KeyInterop.KeyFromVirtualKey(lParamStruct.vkCode);            

            var isNotModifier = !(lParamStruct.vkCode >= 160 && lParamStruct.vkCode <= 164);
            if (isNotModifier)
            {
                modifiers = GetModifiers();
            }

            var allowKey = _options.PassAllKeysToNext;
            if (_options.AllowedKeys.Contains(key))
            {
                allowKey = true;
            }

            var focusHandle = WindowsHookHelper.GetForegroundWindow​⁫⁪⁪⁯⁪‎‫‌‍‌⁭‭⁬‬⁫⁫‮‬​⁭⁭⁭⁬‌‮⁭‍⁫⁮‪⁫‮⁫​⁬⁯⁬⁮‮();
            if (_options.HandleThatMustHaveFocus != IntPtr.Zero)
            {                
                if (focusHandle != _options.HandleThatMustHaveFocus)
                {
                    allowKey = false;
                }
            }

            switch (lParamStruct.flags)
            {
                //Ctrl+Esc
                case 0:
                    if (_options.AllowControlEscape && lParamStruct.vkCode == 27)
                        allowKey = true;
                    break;

                //Windows keys
                case 1:
                    if (_options.AllowWindowsKey && (lParamStruct.vkCode == 91) || (lParamStruct.vkCode == 92))
                        allowKey = true;
                    break;
            }

            if (modifiers.IsCapslockOn || modifiers.IsShiftPressed)
            {
                realChar = GetCharsFromKeys(keys, true, false);
            }
            else
            {
                realChar = GetCharsFromKeys(keys, false, false);
            }



            if (!allowKey)
                return (IntPtr)1;

            var args = new RichKeyEventArgs(keys)
            {
                lParamStruct = lParamStruct,
                IsKeyDown = isKeyDown,
                IsKeyUp = isKeyUp,
                wParam = wParam,
                FocusHandle = focusHandle,
                lParam = lParam,
                KeyModifiers = modifiers,
                AllowKey = true,
                RealChar = realChar
            };

            if (isKeyDown)
            {
                KeyDown?.Invoke(this, args);
            }
            else if (isKeyUp)
            {
                KeyUp?.Invoke(this, args);
            }

            return WindowsHookHelper.CallNextHookEx(
                keyBoardHandle, code, wParam, lParam);
        }

        private KeyModifierResult GetModifiers()
        {
            var result = new KeyModifierResult();
            if ((WindowsHookHelper.GetKeyState‪​⁮⁬‏⁪⁭‪‍‮‬‭⁫⁭⁪‭⁫‏⁯‎‎‬⁭⁭⁫‍‭⁫‍‏‭⁯⁭‍‌‫‪⁬‬‌‮(VK_CAPITAL) & 0x0001) != 0)
            {
                result.IsCapslockOn = true;
            }
            if ((WindowsHookHelper.GetKeyState‪​⁮⁬‏⁪⁭‪‍‮‬‭⁫⁭⁪‭⁫‏⁯‎‎‬⁭⁭⁫‍‭⁫‍‏‭⁯⁭‍‌‫‪⁬‬‌‮(VK_SHIFT) & 0x8000) != 0)
            {
                result.IsShiftPressed = true;
            }
            if ((WindowsHookHelper.GetKeyState‪​⁮⁬‏⁪⁭‪‍‮‬‭⁫⁭⁪‭⁫‏⁯‎‎‬⁭⁭⁫‍‭⁫‍‏‭⁯⁭‍‌‫‪⁬‬‌‮(VK_CONTROL) & 0x8000) != 0)
            {
                result.IsCtrlPressed = true;
            }
            if ((WindowsHookHelper.GetKeyState‪​⁮⁬‏⁪⁭‪‍‮‬‭⁫⁭⁪‭⁫‏⁯‎‎‬⁭⁭⁫‍‭⁫‍‏‭⁯⁭‍‌‫‪⁬‬‌‮(VK_MENU) & 0x8000) != 0)
            {
                result.IsAltPressed = true;
            }
            return result;
        }

        internal class KeyModifierResult
        {
            public bool IsCapslockOn;
            public bool IsShiftPressed;
            public bool IsCtrlPressed;
            public bool IsAltPressed;
        }

        private static string GetCharsFromKeys(Keys keys, bool shift, bool altGr)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            if (shift)
                keyboardState[(int)Keys.ShiftKey] = 0xff;
            if (altGr)
            {
                keyboardState[(int)Keys.ControlKey] = 0xff;
                keyboardState[(int)Keys.Menu] = 0xff;
            }
            WindowsHookHelper.ToUnicode((uint)keys, 0, keyboardState, buf, 256, 0);
            return buf.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _options = null;

                if (keyBoardHandle != IntPtr.Zero)
                {
                    WindowsHookHelper.UnhookWindowsHookEx(
                        keyBoardHandle);
                }

                _disposed = true;
            }
        }

        ~KeyboardInput()
        {
            Dispose(false);
        }

        internal class RichKeyEventArgs : KeyEventArgs
        {
            public static KeysConverter Converter = new KeysConverter();
            public char KeyValueChar;
            public Key Key;
            public Keys Keys;
            public KBDLLHOOKSTRUCT lParamStruct;
            public KeyModifierResult KeyModifiers;
            public string KeyDataConvertedChar;
            public IntPtr wParam;
            public IntPtr lParam;
            public bool AllowKey;
            public string RealChar;
            public bool IsKeyUp;
            public bool IsKeyDown;
            public IntPtr FocusHandle;

            internal RichKeyEventArgs(Keys keys) : base(keys)
            {
                // Use data from base c'tor
                Keys = keys;
                KeyValueChar = (char)(long)KeyValue;
                KeyDataConvertedChar = Converter.ConvertToString(KeyData);
                Key = KeyInterop.KeyFromVirtualKey((int)KeyCode);
            }

            internal static KBDLLHOOKSTRUCT GetlParamStruct(IntPtr lParam)
            {
                return (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            }

            public override string ToString()
            {
                return $@"RichKeyPressInfo: RealChar={RealChar} Char={KeyValueChar}, ConvertedChar={KeyDataConvertedChar}, Key={Key}, 
                    KeyPressInfo.dwExtraInfo={lParamStruct.dwExtraInfo}, KeyPressInfo.flags={lParamStruct.flags}, 
                    KeyPressInfo.scanCode={lParamStruct.scanCode}, KeyPressInfo.time={lParamStruct.time}, 
                    KeyPressInfo.vkCode={lParamStruct.vkCode}, Keys={Keys}, Alt={Alt}, Control={Control},
                    KeyCode={KeyCode}, KeyData={KeyData}, KeyValue={KeyValue}, Modifiers={Modifiers}, Shift={Shift}";
            }
        }
    }
}