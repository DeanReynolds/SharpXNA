using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SharpXNA.Input
{
    public static class Mouse
    {
        private static MouseState state, lastState;

        public static int X { get { return (int)(((state.X / (float)Screen.WindowWidth) * Screen.BackBufferWidth) - Globe.Viewport.X); } }
        public static int Y { get { return (int)(((state.Y / (float)Screen.WindowHeight) * Screen.BackBufferHeight) - Globe.Viewport.Y); } }
        public static Point Position { get { return new Point(X, Y); } set { Microsoft.Xna.Framework.Input.Mouse.SetPosition(value.X, value.Y); } }

        static Mouse() { released = new Dictionary<Buttons, bool>(3); for (int i = 0; i < 3; i++) released.Add((Buttons)i, true); }

        private static Dictionary<Buttons, bool> released;

        public static void Update()
        {
            lastState = state; state = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if ((Pressed != null) || (Released != null))
                for (int i = 0; i < Enum.GetValues(typeof(Buttons)).Length; i++)
                {
                    Buttons key = (Buttons)i;
                    if (Holding(key)) { if (released[key]) { released[key] = false; if (Pressed != null) Pressed(key); } }
                    else if (!released[key]) { released[key] = true; if (Released != null) Released(key); }
                }
        }

        public delegate void ButtonHandler(Buttons key);
        public static event ButtonHandler Pressed, Released;

        public static bool Press(Buttons key)
        {
            if ((key == Buttons.Left) && (state.LeftButton == ButtonState.Pressed) && ((lastState == null) || (lastState.LeftButton == ButtonState.Released))) return true;
            if ((key == Buttons.Middle) && (state.MiddleButton == ButtonState.Pressed) && ((lastState == null) || (lastState.MiddleButton == ButtonState.Released))) return true;
            if ((key == Buttons.Right) && (state.RightButton == ButtonState.Pressed) && ((lastState == null) || (lastState.RightButton == ButtonState.Released))) return true;
            return false;
        }
        public static bool Release(Buttons key)
        {
            if ((key == Buttons.Left) && (state.LeftButton == ButtonState.Released) && ((lastState != null) && (lastState.LeftButton == ButtonState.Pressed))) return true;
            if ((key == Buttons.Middle) && (state.MiddleButton == ButtonState.Released) && ((lastState != null) && (lastState.MiddleButton == ButtonState.Pressed))) return true;
            if ((key == Buttons.Right) && (state.RightButton == ButtonState.Released) && ((lastState != null) && (lastState.RightButton == ButtonState.Pressed))) return true;
            return false;
        }
        public static bool Holding(Buttons key)
        {
            if ((key == Buttons.Left) && (state.LeftButton == ButtonState.Pressed)) return true;
            if ((key == Buttons.Middle) && (state.MiddleButton == ButtonState.Pressed)) return true;
            if ((key == Buttons.Right) && (state.RightButton == ButtonState.Pressed)) return true;
            return false;
        }

        public static bool ScrolledUp() { return (state.ScrollWheelValue > lastState.ScrollWheelValue); }
        public static bool ScrolledDown() { return (state.ScrollWheelValue < lastState.ScrollWheelValue); }

        public enum Buttons { Left, Middle, Right }
    }
}