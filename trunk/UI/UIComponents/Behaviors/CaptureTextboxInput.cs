using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Trinity.UI.UIComponents.Input;
using Zeta.Game;
using Logger = Trinity.Framework.Helpers.Logger;
using TextBox = System.Windows.Controls.TextBox;

namespace Trinity.UI.UIComponents.Behaviors
{
    public class CaptureTextboxInput : DependencyObject
    {
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(CaptureTextboxInput),
            new PropertyMetadata(false, ActivePropertyChanged));

        public static readonly DependencyProperty InputCapturerProperty = DependencyProperty.Register(
            "InputCapturer",
            typeof(KeyboardInput),
            typeof(CaptureTextboxInput),
            new PropertyMetadata(null));

        public static readonly DependencyProperty GlobalKeyDownHandlerProperty = DependencyProperty.Register(
            "GlobalKeyDownHandler",
            typeof(RoutedEventHandler),
            typeof(CaptureTextboxInput),
            new PropertyMetadata(null));

        private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            var textbox = d as TextBox;
            if (textbox != null)
            {
                var routedEventHandler = (RoutedEventHandler)textbox.GetValue(GlobalKeyDownHandlerProperty);
                if (routedEventHandler == null)
                {
                    routedEventHandler = HandleHandledKeyDown;
                    textbox.SetValue(GlobalKeyDownHandlerProperty, routedEventHandler);
                }

                var isActive = (e.NewValue as bool?).GetValueOrDefault(false);
                if (isActive)
                {
                    InputEventMonitor.Add(textbox, InputEventMonitor.ActionType.Deselect);
                    textbox.GotKeyboardFocus += TextboxOnGotKeyboardFocus;                    
                    textbox.LostKeyboardFocus += TextboxOnLostKeyboardFocus;
                    textbox.AddHandler(UIElement.KeyDownEvent, routedEventHandler, true);
                }
                else
                {
                    InputEventMonitor.Remove(textbox);
                    textbox.GotKeyboardFocus -= TextboxOnGotKeyboardFocus;
                    textbox.LostKeyboardFocus -= TextboxOnLostKeyboardFocus;
                    textbox.RemoveHandler(UIElement.KeyDownEvent, routedEventHandler);
                }
            }
        }

        public static void HandleHandledKeyDown(object sender, RoutedEventArgs e)
        {
            System.Windows.Input.KeyEventArgs ke = e as System.Windows.Input.KeyEventArgs;
            if (ke != null && !_allowedInputKeys.Contains(ke.Key))
            {
                ke.Handled = false;
            }
        }

        private static void TextboxOnGotKeyboardFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var textbox = sender as TextBox;
            if (textbox != null)
            {
                StartCapture(textbox);
            }
        }

        private static void StartCapture(DependencyObject element)
        {
            var keyboard = (KeyboardInput)element.GetValue(InputCapturerProperty);
            if (keyboard == null)
            {
                keyboard = new KeyboardInput(new KeyboardInput.HookOptions
                {
                    HandleThatMustHaveFocus = ZetaDia.Overlay.AttachedProcess.MainWindowHandle
                });

                element.SetValue(InputCapturerProperty, keyboard);
            }

            keyboard.KeyDown += OnKeyDown;
        }

        private static void TextboxOnLostKeyboardFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var textbox = sender as TextBox;
            if (textbox != null)
            {
                StopCapture(textbox);
            }
        }

        private static void StopCapture(DependencyObject textbox)
        {
            var keyboard = (KeyboardInput)textbox.GetValue(InputCapturerProperty);
            if (keyboard != null)
            {
                textbox.GetValue(InputCapturerProperty);
                keyboard.KeyDown -= OnKeyDown;
                keyboard.Dispose();
                textbox.SetValue(InputCapturerProperty, null);
            }
        }

        private static List<Key> _allowedInputKeys = new List<Key>
        {
            Key.D0,
            Key.D1,
            Key.D2,
            Key.D3,
            Key.D4,
            Key.D5,
            Key.D6,
            Key.D7,
            Key.D8,
            Key.D9,
            Key.NumPad0,
            Key.NumPad1,
            Key.NumPad2,
            Key.NumPad3,
            Key.NumPad4,
            Key.NumPad5,
            Key.NumPad6,
            Key.NumPad7,
            Key.NumPad8,
            Key.NumPad9,
            Key.Decimal,
            Key.OemPeriod
        };

        private static List<Key> _allowedSpecialKeys = new List<Key>
        {
            Key.Left,
            Key.Right,
            Key.Enter,
            Key.Back,  
            Key.Delete,          
        };

        private static void OnKeyDown(object sender, KeyboardInput.RichKeyEventArgs args)
        {
            var textbox = Keyboard.FocusedElement as TextBox;
            if (textbox == null)
            {
                Logger.Log("Key Input with invalid context, expecting TextBox");
                return;
            }

            var inputSource = PresentationSource.FromVisual(textbox);            
            if (inputSource == null)
            {
                Logger.Log("Input source is not a valid presentation source");
                return;
            }

            if (inputSource.IsDisposed)
            {
                Logger.Log("Input source is no longer valid");
                return;
            }

            if (args.Control && args.KeyCode == Keys.V)
            {
                InputExtensions.Paste(textbox, (c,i) => char.IsDigit(c));
                return;
            }

            var isAllowedInput = _allowedInputKeys.Contains(args.Key);
            var isAllowedSpecial = _allowedSpecialKeys.Contains(args.Key);

            if (isAllowedInput && textbox.IsEnabled)
            {
                //Logger.Log("Key {0} was pressed. RealChar={1} Key={2}", args.KeyCode, args.KeyValueChar, args.Key);
                textbox.InsertText(args.RealChar);
            }
            else
            {
                //Logger.Log("Key press {0} was blocked. RealChar={1} Key={2} SpecialAllowed={3}", args.KeyCode, args.RealChar, args.Key, isAllowedSpecial);
            }

            if (!isAllowedSpecial && !isAllowedInput)
                return;

            textbox.RaiseEvent(
              new System.Windows.Input.KeyEventArgs(
                Keyboard.PrimaryDevice,
                inputSource, 0, args.Key)
              { RoutedEvent = Keyboard.PreviewKeyDownEvent }
            );

            textbox.RaiseEvent(
              new System.Windows.Input.KeyEventArgs(
                Keyboard.PrimaryDevice,
                inputSource, 0, args.Key)
              { RoutedEvent = Keyboard.KeyDownEvent }
            );
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetActive(DependencyObject @object)
        {
            return (bool)@object.GetValue(ActiveProperty);
        }

        public static void SetActive(DependencyObject @object, bool value)
        {
            @object.SetValue(ActiveProperty, value);
        }
    }
}


