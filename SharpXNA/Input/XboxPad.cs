using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SharpXNA.Input
{
    public static class XboxPad
    {
        internal static readonly GamePadState[] _state, _lastState;

        static XboxPad() { _state = new GamePadState[4]; _lastState = new GamePadState[4]; }

        public static void Update()
        {
            for (var i = 0; i < 4; i++)
            {
                _lastState[i] = _state[i];
                _state[i] = GamePad.GetState((PlayerIndex)i);
            }
        }
        
        public static bool Pressed(Buttons button, PlayerIndex? playerIndex = null)
        {
            if (!playerIndex.HasValue)
            {
                for (var i = 0; i < 4; i++)
                    if (_state[i].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)button) && ((_lastState[i] == null) || _lastState[i].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)button)))
                        return true;
                return false;
            }
            return (_state[(int)playerIndex].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)button) && ((_lastState[(int)playerIndex] == null) || _lastState[(int)playerIndex].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)button)));
        }
        public static bool Released(Buttons button, PlayerIndex? playerIndex = null)
        {
            if (!playerIndex.HasValue)
            {
                for (var i = 0; i < 4; i++)
                    if (_state[i].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)button) && ((_lastState[i] != null) && _lastState[i].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)button)))
                        return true;
                return false;
            }
            return (_state[(int)playerIndex].IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)button) && ((_lastState[(int)playerIndex] != null) && _lastState[(int)playerIndex].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)button)));
        }
        public static bool Holding(Buttons button, PlayerIndex? playerIndex = null)
        {
            if (!playerIndex.HasValue) { for (var i = 0; i < 4; i++) if (_state[i].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)button)) return true; }
            return _state[(int)playerIndex].IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)button);
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