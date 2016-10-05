using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;

namespace SharpXNA.Input
{
    public static class Keyboard
    {
        private static Microsoft.Xna.Framework.Input.Keys[] LastKs, LastKsConstrains;
        private static double KsTimer, KsConstraint;
        public static KeyboardState KeyState, LastKeyState;

        public static void Update(GameTime time)
        {
            LastKeyState = KeyState;
            KeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var Ks = KeyState.GetPressedKeys();
            foreach (var Key in Ks)
                if (Key != Microsoft.Xna.Framework.Input.Keys.None)
                {
                    if ((LastKs != null) && LastKs.Contains(Key))
                    {
                        if (((time.TotalGameTime.TotalMilliseconds - KsTimer) > KsConstraint) && (InputRecieved != null))
                        {
                            InputRecieved((Keys)Key);
                            KsTimer = time.TotalGameTime.TotalMilliseconds;
                            if (LastKsConstrains == null)
                            {
                                KsConstraint = 50;
                                LastKsConstrains = Ks;
                            }
                        }
                    }
                    else if (InputRecieved != null)
                    {
                        InputRecieved((Keys)Key);
                        KsTimer = time.TotalGameTime.TotalMilliseconds;
                    }
                    if ((LastKsConstrains != null) && !LastKsConstrains.Contains(Key))
                    {
                        KsConstraint = 425;
                        LastKsConstrains = null;
                    }
                }
            if (Ks.Length == 0)
            {
                KsConstraint = 425;
                LastKsConstrains = null;
            }
            LastKs = Ks;
            foreach (Keys key in KeyStatesH.Keys)
            {
                if (((KeyStateH)KeyStatesH[key]).Timer > 0) ((KeyStateH)KeyStatesH[key]).Timer -= time.ElapsedGameTime.TotalSeconds;
                else
                {
                    ((KeyStateH)KeyStatesH[key]).Timer += ((KeyStateH)KeyStatesH[key]).ResetTime;
                    ((KeyStateH)KeyStatesH[key]).ResetTime = System.Math.Max(.025, (((KeyStateH)KeyStatesH[key]).ResetTime / 2d));
                }
            }
        }

        public delegate void InputRecievedEvent(Keys Key);
        public static event InputRecievedEvent InputRecieved;

        public static OrderedDictionary KeyStatesH = new OrderedDictionary(System.Enum.GetValues(typeof(Keys)).Length);
        public static object KeyFromIndex(this OrderedDictionary dictionary, int index) { if (index == -1) return null; return ((dictionary.Count > index) ? dictionary.Cast<DictionaryEntry>().ElementAt(index).Key : null); }
        public static Keys[] GetPressedKeys()
        {
            var origPressedKeys = KeyState.GetPressedKeys();
            var pressedKeys = new List<Keys>(origPressedKeys.Length);
            for (var i = 0; i < KeyStatesH.Count; i++) if (!origPressedKeys.Contains((Microsoft.Xna.Framework.Input.Keys)KeyStatesH.KeyFromIndex(i))) { KeyStatesH.RemoveAt(i); i--; continue; }
            for (var i = 0; i < origPressedKeys.Length; i++)
            {
                var key = (Keys)origPressedKeys[i];
                if (KeyStatesH.Contains(key)) { if (((KeyStateH)KeyStatesH[key]).Timer <= 0) pressedKeys.Add(key); }
                else { KeyStatesH.Add(key, new KeyStateH(.6)); pressedKeys.Add(key); }
            }
            return pressedKeys.ToArray();
        }
        public class KeyStateH { public double Timer, ResetTime; public KeyStateH(double resetTime) { Timer = 0; ResetTime = resetTime; } }

        public static bool Pressed(Keys key) { return (KeyState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key) && ((LastKeyState == null) || LastKeyState.IsKeyUp((Microsoft.Xna.Framework.Input.Keys)key))); }
        public static bool Released(Keys key) { return (KeyState.IsKeyUp((Microsoft.Xna.Framework.Input.Keys)key) && ((LastKeyState != null) && LastKeyState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key))); }
        public static bool Holding(Keys key) { return KeyState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key); }
        public static bool Pressed(Microsoft.Xna.Framework.Input.Keys key) { return (KeyState.IsKeyDown(key) && ((LastKeyState == null) || LastKeyState.IsKeyUp(key))); }
        public static bool Released(Microsoft.Xna.Framework.Input.Keys key) { return (KeyState.IsKeyUp(key) && ((LastKeyState != null) && LastKeyState.IsKeyDown(key))); }
        public static bool Holding(Microsoft.Xna.Framework.Input.Keys key) { return KeyState.IsKeyDown(key); }
        public static bool PressedShift() { return (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift)); }
        public static bool ReleasedShift() { return (Released(Keys.LeftShift) || Released(Keys.RightShift)); }
        public static bool HoldingShift() { return (Holding(Keys.LeftShift) || Holding(Keys.RightShift)); }
        public static bool PressedControl() { return (Pressed(Keys.LeftControl) || Pressed(Keys.RightControl)); }
        public static bool ReleasedControl() { return (Released(Keys.LeftControl) || Released(Keys.RightControl)); }
        public static bool HoldingControl() { return (Holding(Keys.LeftControl) || Holding(Keys.RightControl)); }

        public enum Keys
        {
            None = 0,
            Back = 8,
            Tab = 9,
            Enter = 13,
            Pause = 19,
            //
            // Summary:
            //     CAPS LOCK key
            CapsLock = 20,
            //
            // Summary:
            //     Kana key on Japanese keyboards
            Kana = 21,
            //
            // Summary:
            //     Kanji key on Japanese keyboards
            Kanji = 25,
            //
            // Summary:
            //     ESC key
            Escape = 27,
            //
            // Summary:
            //     IME Convert key
            ImeConvert = 28,
            //
            // Summary:
            //     IME NoConvert key
            ImeNoConvert = 29,
            //
            // Summary:
            //     SPACEBAR
            Space = 32,
            //
            // Summary:
            //     PAGE UP key
            PageUp = 33,
            //
            // Summary:
            //     PAGE DOWN key
            PageDown = 34,
            //
            // Summary:
            //     END key
            End = 35,
            //
            // Summary:
            //     HOME key
            Home = 36,
            //
            // Summary:
            //     LEFT ARROW key
            Left = 37,
            //
            // Summary:
            //     UP ARROW key
            Up = 38,
            //
            // Summary:
            //     RIGHT ARROW key
            Right = 39,
            //
            // Summary:
            //     DOWN ARROW key
            Down = 40,
            //
            // Summary:
            //     SELECT key
            Select = 41,
            //
            // Summary:
            //     PRINT key
            Print = 42,
            //
            // Summary:
            //     EXECUTE key
            Execute = 43,
            //
            // Summary:
            //     PRINT SCREEN key
            PrintScreen = 44,
            //
            // Summary:
            //     INS key
            Insert = 45,
            //
            // Summary:
            //     DEL key
            Delete = 46,
            //
            // Summary:
            //     HELP key
            Help = 47,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D0 = 48,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D1 = 49,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D2 = 50,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D3 = 51,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D4 = 52,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D5 = 53,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D6 = 54,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D7 = 55,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D8 = 56,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            D9 = 57,
            //
            // Summary:
            //     A key
            A = 65,
            //
            // Summary:
            //     B key
            B = 66,
            //
            // Summary:
            //     C key
            C = 67,
            //
            // Summary:
            //     D key
            D = 68,
            //
            // Summary:
            //     E key
            E = 69,
            //
            // Summary:
            //     F key
            F = 70,
            //
            // Summary:
            //     G key
            G = 71,
            //
            // Summary:
            //     H key
            H = 72,
            //
            // Summary:
            //     I key
            I = 73,
            //
            // Summary:
            //     J key
            J = 74,
            //
            // Summary:
            //     K key
            K = 75,
            //
            // Summary:
            //     L key
            L = 76,
            //
            // Summary:
            //     M key
            M = 77,
            //
            // Summary:
            //     N key
            N = 78,
            //
            // Summary:
            //     O key
            O = 79,
            //
            // Summary:
            //     P key
            P = 80,
            //
            // Summary:
            //     Q key
            Q = 81,
            //
            // Summary:
            //     R key
            R = 82,
            //
            // Summary:
            //     S key
            S = 83,
            //
            // Summary:
            //     T key
            T = 84,
            //
            // Summary:
            //     U key
            U = 85,
            //
            // Summary:
            //     V key
            V = 86,
            //
            // Summary:
            //     W key
            W = 87,
            //
            // Summary:
            //     X key
            X = 88,
            //
            // Summary:
            //     Y key
            Y = 89,
            //
            // Summary:
            //     Z key
            Z = 90,
            //
            // Summary:
            //     Left Windows key
            LeftWindows = 91,
            //
            // Summary:
            //     Right Windows key
            RightWindows = 92,
            //
            // Summary:
            //     Applications key
            Apps = 93,
            //
            // Summary:
            //     Computer Sleep key
            Sleep = 95,
            //
            // Summary:
            //     Numeric keypad 0 key
            NumPad0 = 96,
            //
            // Summary:
            //     Numeric keypad 1 key
            NumPad1 = 97,
            //
            // Summary:
            //     Numeric keypad 2 key
            NumPad2 = 98,
            //
            // Summary:
            //     Numeric keypad 3 key
            NumPad3 = 99,
            //
            // Summary:
            //     Numeric keypad 4 key
            NumPad4 = 100,
            //
            // Summary:
            //     Numeric keypad 5 key
            NumPad5 = 101,
            //
            // Summary:
            //     Numeric keypad 6 key
            NumPad6 = 102,
            //
            // Summary:
            //     Numeric keypad 7 key
            NumPad7 = 103,
            //
            // Summary:
            //     Numeric keypad 8 key
            NumPad8 = 104,
            //
            // Summary:
            //     Numeric keypad 9 key
            NumPad9 = 105,
            //
            // Summary:
            //     Multiply key
            Multiply = 106,
            //
            // Summary:
            //     Add key
            Add = 107,
            //
            // Summary:
            //     Separator key
            Separator = 108,
            //
            // Summary:
            //     Subtract key
            Subtract = 109,
            //
            // Summary:
            //     Decimal key
            Decimal = 110,
            //
            // Summary:
            //     Divide key
            Divide = 111,
            //
            // Summary:
            //     F1 key
            F1 = 112,
            //
            // Summary:
            //     F2 key
            F2 = 113,
            //
            // Summary:
            //     F3 key
            F3 = 114,
            //
            // Summary:
            //     F4 key
            F4 = 115,
            //
            // Summary:
            //     F5 key
            F5 = 116,
            //
            // Summary:
            //     F6 key
            F6 = 117,
            //
            // Summary:
            //     F7 key
            F7 = 118,
            //
            // Summary:
            //     F8 key
            F8 = 119,
            //
            // Summary:
            //     F9 key
            F9 = 120,
            //
            // Summary:
            //     F10 key
            F10 = 121,
            //
            // Summary:
            //     F11 key
            F11 = 122,
            //
            // Summary:
            //     F12 key
            F12 = 123,
            //
            // Summary:
            //     F13 key
            F13 = 124,
            //
            // Summary:
            //     F14 key
            F14 = 125,
            //
            // Summary:
            //     F15 key
            F15 = 126,
            //
            // Summary:
            //     F16 key
            F16 = 127,
            //
            // Summary:
            //     F17 key
            F17 = 128,
            //
            // Summary:
            //     F18 key
            F18 = 129,
            //
            // Summary:
            //     F19 key
            F19 = 130,
            //
            // Summary:
            //     F20 key
            F20 = 131,
            //
            // Summary:
            //     F21 key
            F21 = 132,
            //
            // Summary:
            //     F22 key
            F22 = 133,
            //
            // Summary:
            //     F23 key
            F23 = 134,
            //
            // Summary:
            //     F24 key
            F24 = 135,
            //
            // Summary:
            //     NUM LOCK key
            NumLock = 144,
            //
            // Summary:
            //     SCROLL LOCK key
            Scroll = 145,
            //
            // Summary:
            //     Left SHIFT key
            LeftShift = 160,
            //
            // Summary:
            //     Right SHIFT key
            RightShift = 161,
            //
            // Summary:
            //     Left CONTROL key
            LeftControl = 162,
            //
            // Summary:
            //     Right CONTROL key
            RightControl = 163,
            //
            // Summary:
            //     Left ALT key
            LeftAlt = 164,
            //
            // Summary:
            //     Right ALT key
            RightAlt = 165,
            //
            // Summary:
            //     Windows 2000/XP: Browser Back key
            BrowserBack = 166,
            //
            // Summary:
            //     Windows 2000/XP: Browser Forward key
            BrowserForward = 167,
            //
            // Summary:
            //     Windows 2000/XP: Browser Refresh key
            BrowserRefresh = 168,
            //
            // Summary:
            //     Windows 2000/XP: Browser Stop key
            BrowserStop = 169,
            //
            // Summary:
            //     Windows 2000/XP: Browser Search key
            BrowserSearch = 170,
            //
            // Summary:
            //     Windows 2000/XP: Browser Favorites key
            BrowserFavorites = 171,
            //
            // Summary:
            //     Windows 2000/XP: Browser Start and Home key
            BrowserHome = 172,
            //
            // Summary:
            //     Windows 2000/XP: Volume Mute key
            VolumeMute = 173,
            //
            // Summary:
            //     Windows 2000/XP: Volume Down key
            VolumeDown = 174,
            //
            // Summary:
            //     Windows 2000/XP: Volume Up key
            VolumeUp = 175,
            //
            // Summary:
            //     Windows 2000/XP: Next Track key
            MediaNextTrack = 176,
            //
            // Summary:
            //     Windows 2000/XP: Previous Track key
            MediaPreviousTrack = 177,
            //
            // Summary:
            //     Windows 2000/XP: Stop Media key
            MediaStop = 178,
            //
            // Summary:
            //     Windows 2000/XP: Play/Pause Media key
            MediaPlayPause = 179,
            //
            // Summary:
            //     Windows 2000/XP: Start Mail key
            LaunchMail = 180,
            //
            // Summary:
            //     Windows 2000/XP: Select Media key
            SelectMedia = 181,
            //
            // Summary:
            //     Windows 2000/XP: Start Application 1 key
            LaunchApplication1 = 182,
            //
            // Summary:
            //     Windows 2000/XP: Start Application 2 key
            LaunchApplication2 = 183,
            //
            // Summary:
            //     Windows 2000/XP: The OEM Semicolon key on a US standard keyboard
            OemSemicolon = 186,
            //
            // Summary:
            //     Windows 2000/XP: For any country/region, the '+' key
            OemPlus = 187,
            //
            // Summary:
            //     Windows 2000/XP: For any country/region, the ',' key
            OemComma = 188,
            //
            // Summary:
            //     Windows 2000/XP: For any country/region, the '-' key
            OemMinus = 189,
            //
            // Summary:
            //     Windows 2000/XP: For any country/region, the '.' key
            OemPeriod = 190,
            //
            // Summary:
            //     Windows 2000/XP: The OEM question mark key on a US standard keyboard
            OemQuestion = 191,
            //
            // Summary:
            //     Windows 2000/XP: The OEM tilde key on a US standard keyboard
            OemTilde = 192,
            //
            // Summary:
            //     Green ChatPad key
            ChatPadGreen = 202,
            //
            // Summary:
            //     Orange ChatPad key
            ChatPadOrange = 203,
            //
            // Summary:
            //     Windows 2000/XP: The OEM open bracket key on a US standard keyboard
            OemOpenBrackets = 219,
            //
            // Summary:
            //     Windows 2000/XP: The OEM pipe key on a US standard keyboard
            OemPipe = 220,
            //
            // Summary:
            //     Windows 2000/XP: The OEM close bracket key on a US standard keyboard
            OemCloseBrackets = 221,
            //
            // Summary:
            //     Windows 2000/XP: The OEM singled/double quote key on a US standard keyboard
            OemQuotes = 222,
            //
            // Summary:
            //     Used for miscellaneous characters; it can vary by keyboard.
            Oem8 = 223,
            //
            // Summary:
            //     Windows 2000/XP: The OEM angle bracket or backslash key on the RT 102 key
            //     keyboard
            OemBackslash = 226,
            //
            // Summary:
            //     Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
            ProcessKey = 229,
            //
            // Summary:
            //     OEM Copy key
            OemCopy = 242,
            //
            // Summary:
            //     OEM Auto key
            OemAuto = 243,
            //
            // Summary:
            //     OEM Enlarge Window key
            OemEnlW = 244,
            //
            // Summary:
            //     Attn key
            Attn = 246,
            //
            // Summary:
            //     CrSel key
            Crsel = 247,
            //
            // Summary:
            //     ExSel key
            Exsel = 248,
            //
            // Summary:
            //     Erase EOF key
            EraseEof = 249,
            //
            // Summary:
            //     Play key
            Play = 250,
            //
            // Summary:
            //     Zoom key
            Zoom = 251,
            //
            // Summary:
            //     PA1 key
            Pa1 = 253,
            //
            // Summary:
            //     CLEAR key
            OemClear = 254
        }
    }
}