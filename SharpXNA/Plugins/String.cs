using SharpXNA.Input;

namespace SharpXNA.Plugins
{
    public static class String
    {
        [System.Flags]
        public enum InputFlags
        {
            None, NoSpecalCharacters = 1, NoLetters = 2, NoNumbers = 4, NoLeadingPeriods = 8, NoSpaces = 16, AllowPeriods = 32, NoLeadingColons = 64, AllowColons = 128, NoRepeatingColons = 256,
            NoRepeatingPeriods = 512, NoRepeatingSpaces = 1024, NoLeadingSpaces = 2048
        }
        
        public static string AcceptInput(this string @string, int maxLength = int.MaxValue) { return @string.AcceptInput(InputFlags.None, maxLength); }
        public static string AcceptInput(this string @string, InputFlags flags = InputFlags.None, int maxLength = int.MaxValue)
        {
            var newString = (@string ?? string.Empty);
            var keys = Keyboard.GetPressedKeys();
            foreach (var key in keys)
            {
                if ((key == Keyboard.Keys.CapsLock) || (key == Keyboard.Keys.LeftShift) || (key == Keyboard.Keys.RightShift) ||
           (key == Keyboard.Keys.LeftControl) || (key == Keyboard.Keys.RightControl) || (key == Keyboard.Keys.Escape) || (key == Keyboard.Keys.Insert) ||
           (key == Keyboard.Keys.Delete) || (key == Keyboard.Keys.Home) || (key == Keyboard.Keys.End) || (key == Keyboard.Keys.PageDown) ||
           (key == Keyboard.Keys.PageUp) || (key == Keyboard.Keys.Tab) || (key == Keyboard.Keys.NumLock) || (key == Keyboard.Keys.Enter) ||
           (key == Keyboard.Keys.F1) || (key == Keyboard.Keys.F2) || (key == Keyboard.Keys.F3) || (key == Keyboard.Keys.F4) || (key == Keyboard.Keys.F5) ||
           (key == Keyboard.Keys.F6) || (key == Keyboard.Keys.F7) || (key == Keyboard.Keys.F8) || (key == Keyboard.Keys.F9) || (key == Keyboard.Keys.F10) ||
           (key == Keyboard.Keys.F11) || (key == Keyboard.Keys.F12) || (key == Keyboard.Keys.F13) || (key == Keyboard.Keys.F14) || (key == Keyboard.Keys.F15) ||
           (key == Keyboard.Keys.F16) || (key == Keyboard.Keys.F17) || (key == Keyboard.Keys.F18) || (key == Keyboard.Keys.F19) || (key == Keyboard.Keys.F20) ||
           (key == Keyboard.Keys.F21) || (key == Keyboard.Keys.F22) || (key == Keyboard.Keys.F23) || (key == Keyboard.Keys.F24) || (key == Keyboard.Keys.LeftAlt) ||
           (key == Keyboard.Keys.RightAlt) || (key == Keyboard.Keys.LeftWindows) || (key == Keyboard.Keys.RightWindows) || (key == Keyboard.Keys.Up) || (key == Keyboard.Keys.Down) ||
           (key == Keyboard.Keys.Left) || (key == Keyboard.Keys.Right) || (key == Keyboard.Keys.Add) || (key == Keyboard.Keys.Subtract) || (key == Keyboard.Keys.Divide) ||
           (key == Keyboard.Keys.Apps) || (key == Keyboard.Keys.PrintScreen) || (key == Keyboard.Keys.Print) || (key == Keyboard.Keys.Scroll) || (key == Keyboard.Keys.Pause))
                    continue;
                bool caps = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock), shift = (Keyboard.Holding(Keyboard.Keys.LeftShift) || Keyboard.Holding(Keyboard.Keys.RightShift));
                if (key == Keyboard.Keys.Back) { if (!string.IsNullOrEmpty(newString)) newString = newString.Substring(0, (newString.Length - 1)); }
                else if (key == Keyboard.Keys.OemPeriod)
                {
                    if (!flags.HasFlag(InputFlags.NoSpecalCharacters) || (!shift && (flags.HasFlag(InputFlags.AllowPeriods) && (!flags.HasFlag(InputFlags.NoLeadingPeriods) ||
                        !string.IsNullOrEmpty(newString)) && (!flags.HasFlag(InputFlags.NoRepeatingPeriods) || !newString.EndsWith(".")))))
                    { if (shift) newString += ">"; else newString += "."; }
                }
                else if (key == Keyboard.Keys.Decimal)
                {
                    if (!flags.HasFlag(InputFlags.NoSpecalCharacters) || (flags.HasFlag(InputFlags.AllowPeriods) && (!flags.HasFlag(InputFlags.NoLeadingPeriods) ||
                        !string.IsNullOrEmpty(newString)) && (!flags.HasFlag(InputFlags.NoRepeatingPeriods) || !newString.EndsWith("."))))
                        newString += ".";
                }
                else if (key == Keyboard.Keys.OemComma) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "<"; else newString += ","; } }
                else if (key == Keyboard.Keys.OemSemicolon)
                {
                    if (!flags.HasFlag(InputFlags.NoSpecalCharacters) || (shift && (flags.HasFlag(InputFlags.AllowColons) && (!flags.HasFlag(InputFlags.NoLeadingColons) ||
                        !string.IsNullOrEmpty(newString)) && (!flags.HasFlag(InputFlags.NoRepeatingColons) || !newString.EndsWith(":")))))
                    { if (shift) newString += ":"; else newString += ";"; }
                }
                else if (key == Keyboard.Keys.OemTilde) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "@"; else newString += "'"; } }
                else if (key == Keyboard.Keys.OemOpenBrackets) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "{"; else newString += "["; } }
                else if (key == Keyboard.Keys.OemCloseBrackets) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "}"; else newString += "]"; } }
                else if (key == Keyboard.Keys.OemPlus) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "+"; else newString += "="; } }
                else if (key == Keyboard.Keys.OemMinus) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "_"; else newString += "-"; } }
                else if (key == Keyboard.Keys.Oem8) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "¬"; else newString += "`"; } }
                else if (key == Keyboard.Keys.OemPipe) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "|"; else newString += "\\"; } }
                else if (key == Keyboard.Keys.OemQuotes) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "~"; else newString += "#"; } }
                else if (key == Keyboard.Keys.OemQuestion) { if (!flags.HasFlag(InputFlags.NoSpecalCharacters)) { if (shift) newString += "?"; else newString += "/"; } }
                else if (key == Keyboard.Keys.D1) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "!"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "1"; }
                else if (key == Keyboard.Keys.D2) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "\""; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "2"; }
                else if (key == Keyboard.Keys.D3) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "£"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "3"; }
                else if (key == Keyboard.Keys.D4) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "$"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "4"; }
                else if (key == Keyboard.Keys.D5) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "%"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "5"; }
                else if (key == Keyboard.Keys.D6) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "^"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "6"; }
                else if (key == Keyboard.Keys.D7) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "&"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "7"; }
                else if (key == Keyboard.Keys.D8) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "*"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "8"; }
                else if (key == Keyboard.Keys.D9) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += "("; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "9"; }
                else if (key == Keyboard.Keys.D0) { if (shift && !flags.HasFlag(InputFlags.NoSpecalCharacters)) newString += ")"; else if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "0"; }
                else if (key == Keyboard.Keys.NumPad0) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "0"; }
                else if (key == Keyboard.Keys.NumPad1) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "1"; }
                else if (key == Keyboard.Keys.NumPad2) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "2"; }
                else if (key == Keyboard.Keys.NumPad3) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "3"; }
                else if (key == Keyboard.Keys.NumPad4) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "4"; }
                else if (key == Keyboard.Keys.NumPad5) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "5"; }
                else if (key == Keyboard.Keys.NumPad6) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "6"; }
                else if (key == Keyboard.Keys.NumPad7) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "7"; }
                else if (key == Keyboard.Keys.NumPad8) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "8"; }
                else if (key == Keyboard.Keys.NumPad9) { if (!flags.HasFlag(InputFlags.NoNumbers)) newString += "9"; }
                else if (key == Keyboard.Keys.Space)
                {
                    if (!flags.HasFlag(InputFlags.NoSpaces) && ((!flags.HasFlag(InputFlags.NoLeadingSpaces) ||
                        !string.IsNullOrEmpty(newString)) && (!flags.HasFlag(InputFlags.NoRepeatingSpaces) || !newString.EndsWith(" "))))
                        newString += " ";
                }
                else if (!flags.HasFlag(InputFlags.NoLetters)) newString += (((caps && !shift) || (!caps && shift)) ? key.ToString().ToUpper() : key.ToString().ToLower());
            }
            if (newString.Length > maxLength) newString = newString.Substring(0, maxLength);
            return newString;
        }
    }
}