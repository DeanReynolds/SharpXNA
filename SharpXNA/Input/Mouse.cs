using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SharpXNA.Input
{
    public static class Mouse
    {
        internal static ButtonState _lastLMB, _lastMMB, _lastRMB;
        internal static int _lastSWV;
        internal static MouseState _state;

        public static int X { get { return (int)(((_state.X / (float)Screen.WindowWidth) * Screen.BackBufferWidth) - Engine.Viewport.X); } }
        public static int Y { get { return (int)(((_state.Y / (float)Screen.WindowHeight) * Screen.BackBufferHeight) - Engine.Viewport.Y); } }
        public static Point Position { get { return new Point(X, Y); } set { Microsoft.Xna.Framework.Input.Mouse.SetPosition(value.X, value.Y); } }

        public static void Update()
        {
            _lastLMB = _state.LeftButton;
            _lastMMB = _state.MiddleButton;
            _lastRMB = _state.RightButton;
            _lastSWV = _state.ScrollWheelValue;
            _state = Microsoft.Xna.Framework.Input.Mouse.GetState();
        }

        public static bool Pressed(Buttons button)
        {
            if (button == Buttons.Left) return ((_state.LeftButton == ButtonState.Pressed) && (_lastLMB == ButtonState.Released));
            if (button == Buttons.Middle) return ((_state.MiddleButton == ButtonState.Pressed) && (_lastMMB == ButtonState.Released));
            if (button == Buttons.Right) return ((_state.RightButton == ButtonState.Pressed) && (_lastRMB == ButtonState.Released));
            return false;
        }
        public static bool Released(Buttons button)
        {
            if (button == Buttons.Left) return ((_state.LeftButton == ButtonState.Released) && (_lastLMB == ButtonState.Pressed));
            if (button == Buttons.Middle) return ((_state.MiddleButton == ButtonState.Released) && (_lastMMB == ButtonState.Pressed));
            if (button == Buttons.Right) return ((_state.RightButton == ButtonState.Released) && (_lastRMB == ButtonState.Pressed));
            return false;
        }
        public static bool Holding(Buttons button)
        {
            if (button == Buttons.Left) return (_state.LeftButton == ButtonState.Pressed);
            if (button == Buttons.Middle) return (_state.MiddleButton == ButtonState.Pressed);
            if (button == Buttons.Right) return (_state.RightButton == ButtonState.Pressed);
            return false;
        }

        public static bool ScrolledUp() { return (_state.ScrollWheelValue > _lastSWV); }
        public static bool ScrolledDown() { return (_state.ScrollWheelValue < _lastSWV); }

        public enum Buttons { Left, Middle, Right }
    }
}