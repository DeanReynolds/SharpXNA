using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA
{
    public static class Profiler
    {
        internal static bool _enabled;
        public static bool Enabled { get { return _enabled; } set { _enabled = value; if (!value) foreach (var p in _profiles.Values) p._stopwatch.Reset(); } }

        internal static Dictionary<string, Profile> _profiles;

        static Profiler() { _profiles = new Dictionary<string, Profile>(); }

        public static void Start(string name)
        {
            if (!_enabled) return;
            if (!_profiles.ContainsKey(name)) _profiles.Add(name, new Profile());
            _profiles[name]._stopwatch.Start();
        }
        public static void Stop(string name)
        {
            if (!_enabled) return;
            _profiles[name]._milliseconds[_profiles[name]._index++] = _profiles[name]._stopwatch.Elapsed.TotalMilliseconds;
            if (_profiles[name]._index >= _profiles[name]._milliseconds.Length) _profiles[name]._index = 0;
            if (_profiles[name]._recorded < _profiles[name]._milliseconds.Length) _profiles[name]._recorded++;
            _profiles[name]._stopwatch.Reset();
        }

        private const float curRowPad = 25, avgRowPad = 29;
        private const int nameYOff = -9, scoresYOff = -13;
        public static void Draw(SpriteFont font, int width)
        {
            lock (_profiles)
            {
                Screen.Begin();
                int x = (Screen.ViewportWidth - (width + 5)), y = 5;
                Screen.DrawRectangle(new Rectangle(x, y, width, (Screen.ViewportHeight - 10)), (Color.Black * .8f), (Color.White * .8f), 1);
                var fontScale = new Vector2(.125f);
                var nameXPos = (Screen.ViewportWidth - 140);
                Screen.DrawLine(new Vector2(nameXPos, y), new Vector2(nameXPos, (Screen.ViewportHeight - 5)), (Color.White * .8f));
                x += 3; nameXPos += 3; y += 15;
                foreach (var k in _profiles.Keys)
                {
                    var t = _profiles[k];
                    if ((y + 31) >= (Screen.ViewportHeight - 10)) break;
                    Screen.DrawLine(new Vector2((x - 3), (y + 16)), new Vector2(((x - 3) + width), (y + 16)), (Color.Silver * .4f));
                    Screen.DrawString(k, font, new Vector2(x, (y + nameYOff)), (Color.White * .8f), new Vector2(.15f));
                    var curYPos = (y - 14);
                    Screen.DrawString("Cur", font, new Vector2(nameXPos, curYPos), (Color.Gray * .8f), fontScale);
                    Screen.DrawString($"{Math.Round(t._milliseconds[t._index], 8)} ms", font, new Vector2((nameXPos + curRowPad), curYPos), (Color.White * .8f), fontScale);
                    var avgYPos = (y - 1);
                    Screen.DrawString("Avg", font, new Vector2(nameXPos, avgYPos), (Color.Gray * .8f), fontScale);
                    var avg = 0d;
                    for (var i = 0; i < t._milliseconds.Length; i++) avg += t._milliseconds[i];
                    avg /= t._recorded;
                    Screen.DrawString($"{Math.Round(avg, 8)} ms", font, new Vector2((nameXPos + avgRowPad), avgYPos), (Color.White * .8f), fontScale);
                    y += 31;
                }
                Screen.End();
            }
        }

        public class Profile
        {
            internal byte _index, _recorded;
            internal double[] _milliseconds;
            internal Stopwatch _stopwatch;

            public Profile() { _milliseconds = new double[50]; _stopwatch = new Stopwatch(); }
        }
    }
}