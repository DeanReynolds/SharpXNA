using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SharpXNA.Input
{
    public static class XboxPad
    {
        private static readonly GamePadState[] S = new GamePadState[4];
        private static readonly GamePadState[] LastS = new GamePadState[4];

        public static void Update(GameTime Time)
        {
            for (var i = 0; i < 4; i++)
            {
                LastS[i] = S[i];
                S[i] = GamePad.GetState((PlayerIndex)i);
            }
        }

        /// <summary>
        /// Check if a buttton on a pad has been pressed
        /// </summary>
        /// <param name="Button">The xbox pad button, to check.</param>
        /// <param name="PlayerIndex">The pad (index) to check, use null for ANY pad.</param>
        /// <returns>A True/False statement.</returns>
        public static bool Pressed(Buttons Button, PlayerIndex? PlayerIndex = null)
        {
            if (PlayerIndex.HasValue)
                return (S[(int)PlayerIndex].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)Button) &&
                        ((LastS == null) ||
                         LastS[(int)PlayerIndex].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)Button)));
            var Pressing = false;
            for (var i = 0; i < 4; i++)
                if (S[i].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)Button) &&
                    ((LastS == null) || LastS[i].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)Button)))
                {
                    Pressing = true;
                    break;
                }
            return Pressing;
        }

        /// <summary>
        /// Check if a buttton on a pad has been released
        /// </summary>
        /// <param name="Button">The xbox pad button, to check.</param>
        /// <param name="PlayerIndex">The pad (index) to check, use null for ANY pad.</param>
        /// <returns>A True/False statement.</returns>
        public static bool Released(Buttons Button, PlayerIndex? PlayerIndex = null)
        {
            if (PlayerIndex.HasValue)
                return (S[(int)PlayerIndex].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)Button) &&
                        ((LastS == null) ||
                         LastS[(int)PlayerIndex].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)Button)));
            var Pressing = false;
            for (var i = 0; i < 4; i++)
                if (S[i].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)Button) &&
                    ((LastS == null) || LastS[i].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)Button)))
                {
                    Pressing = true;
                    break;
                }
            return Pressing;
        }

        /// <summary>
        /// Check if a buttton on a pad is being held
        /// </summary>
        /// <param name="Button">The xbox pad button, to check.</param>
        /// <param name="PlayerIndex">The pad (index) to check, use null for ANY pad.</param>
        /// <returns>A True/False statement.</returns>
        public static bool Holding(Buttons Button, PlayerIndex? PlayerIndex = null)
        {
            if (PlayerIndex.HasValue)
                return S[(int)PlayerIndex].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)Button);
            var Pressing = false;
            for (var i = 0; i < 4; i++)
                if (S[i].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)Button))
                {
                    Pressing = true;
                    break;
                }
            return Pressing;
        }

        public enum Buttons
        {
            DPadUp = 1,
            DPadDown = 2,
            DPadLeft = 4,
            DPadRight = 8,
            Start = 16,
            Back = 32,
            LeftStick = 64,
            RightStick = 128,
            LeftShoulder = 256,
            RightShoulder = 512,
            BigButton = 2048,
            A = 4096,
            B = 8192,
            X = 16384,
            Y = 32768,
            LeftThumbstickLeft = 2097152,
            RightTrigger = 4194304,
            LeftTrigger = 8388608,
            RightThumbstickUp = 16777216,
            RightThumbstickDown = 33554432,
            RightThumbstickRight = 67108864,
            RightThumbstickLeft = 134217728,
            LeftThumbstickUp = 268435456,
            LeftThumbstickDown = 536870912,
            LeftThumbstickRight = 1073741824
        }
    }
}