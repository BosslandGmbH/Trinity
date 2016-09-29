using System;
using System.Windows.Forms;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace Trinity.UI.UIComponents.Input
{
    public static class InputExtensions
    {
        public static void InsertText(this TextBox textbox, string strippedText)
        {
            int start = textbox.SelectionStart;
            string newTxt = textbox.Text;
            newTxt = newTxt.Remove(textbox.SelectionStart, textbox.SelectionLength);
            newTxt = newTxt.Insert(textbox.SelectionStart, strippedText);
            textbox.Text = newTxt;
            textbox.SelectionStart = start + strippedText.Length;
        }

        public static void Delete(this TextBox textbox)
        {
            var startLength = textbox.Text.Length;
            if (textbox.Text.Length == 0) return;
            var isSelection = textbox.SelectionLength > 0;
            var length = Math.Max(!isSelection ? 1 : textbox.SelectionLength, 0);
            int start = textbox.SelectionStart;
            string newTxt = textbox.Text;
            if (length == 0 || start + length > startLength) return;
            newTxt = newTxt.Remove(start, length);
            textbox.Text = newTxt;
            textbox.SelectionStart = start;
        }

        public static void Backspace(this TextBox textbox)
        {
            var startLength = textbox.Text.Length;
            if (startLength == 0) return;
            var isSelection = textbox.SelectionLength > 0;
            var length = Math.Max(!isSelection ? 1 : textbox.SelectionLength, 0);
            int start = Math.Max(textbox.SelectionStart - 1, 0);
            if (length == 0 || start == 0) return;
            string newTxt = textbox.Text;
            newTxt = newTxt.Remove(start, length);
            textbox.Text = newTxt;
            textbox.SelectionStart = start;
        }

        public static void MoveCaretRight(this TextBox textbox)
        {
            textbox.SelectionStart = Math.Min(Math.Max(0, textbox.SelectionStart + 1), textbox.Text.Length);
        }

        public static void MoveCaretLeft(this TextBox textbox)
        {
            textbox.SelectionStart = Math.Min(Math.Max(0, textbox.SelectionStart - 1), textbox.Text.Length);
        }

        public static bool IsModifier(this KeyEventArgs e)
        {
            return e.Control || e.Alt || e.Shift;
        }

        public static bool IsNavigationKey(this KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.PageUp:
                case Keys.PageDown:
                    return true;
            }
            return false;
        }

        public static bool IsNonNumber(this KeyEventArgs e)
        {
            var key = (char)e.KeyCode;
            return
                char.IsLetter(key) ||
                char.IsSymbol(key) ||
                char.IsWhiteSpace(key) ||
                char.IsPunctuation(key);
        }

        public static void Paste(TextBox textbox, Func<char, int, bool> charFilter = null)
        {
            var pasteText = Clipboard.GetText();
            var strippedText = "";
            for (var i = 0; i < pasteText.Length; i++)
            {
                if (charFilter == null || charFilter(pasteText[i], i))
                    strippedText += pasteText[i].ToString();
            }
            InsertText(textbox, strippedText);
        }
    }
}
