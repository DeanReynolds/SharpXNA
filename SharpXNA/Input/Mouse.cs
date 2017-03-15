using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SharpXNA.Input
{
    public static class Mouse
    {
        internal static MouseState _state, _lastState;

        public static int X { get { return (int)(((_state.X / (float)Screen.WindowWidth) * Screen.BackBufferWidth) - Engine.Viewport.X); } }
        public static int Y { get { return (int)(((_state.Y / (float)Screen.WindowHeight) * Screen.BackBufferHeight) - Engine.Viewport.Y); } }
        public static Point Position { get { return new Point(X, Y); } set { Microsoft.Xna.Framework.Input.Mouse.SetPosition(value.X, value.Y); } }
        
        public static void Update() { _lastState = _state; _state = Microsoft.Xna.Framework.Input.Mouse.GetState(); }

        public static bool Pressed(Buttons button)
        {
            if ((button == Buttons.Left) && (_state.LeftButton == ButtonState.Pressed) && ((_lastState == null) || (_lastState.LeftButton == ButtonState.Released))) return true;
            if ((button == Buttons.Middle) && (_state.MiddleButton == ButtonState.Pressed) && ((_lastState == null) || (_lastState.MiddleButton == ButtonState.Released))) return true;
            if ((button == Buttons.Right) && (_state.RightButton == ButtonState.Pressed) && ((_lastState == null) || (_lastState.RightButton == ButtonState.Released))) return true;
            return false;
        }
        public static bool Released(Buttons button)
        {
            if ((button == Buttons.Left) && (_state.LeftButton == ButtonState.Released) && ((_lastState != null) && (_lastState.LeftButton == ButtonState.Pressed))) return true;
            if ((button == Buttons.Middle) && (_state.MiddleButton == ButtonState.Released) && ((_lastState != null) && (_lastState.MiddleButton == ButtonState.Pressed))) return true;
            if ((button == Buttons.Right) && (_state.RightButton == ButtonState.Released) && ((_lastState != null) && (_lastState.RightButton == ButtonState.Pressed))) return true;
            return false;
        }
        public static bool Holding(Buttons button)
        {
            if ((button == Buttons.Left) && (_state.LeftButton == ButtonState.Pressed)) return true;
            if ((button == Buttons.Middle) && (_state.MiddleButton == ButtonState.Pressed)) return true;
            if ((button == Buttons.Right) && (_state.RightButton == ButtonState.Pressed)) return true;
            return false;
        }

        public static bool ScrolledUp() { return (_state.ScrollWheelValue > _lastState.ScrollWheelValue); }
        public static bool ScrolledDown() { return (_state.ScrollWheelValue < _lastState.ScrollWheelValue); }

        public enum Buttons { Left, Middle, Right }
    }
}