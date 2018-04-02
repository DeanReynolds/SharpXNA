using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpXNA
{
    public static class Profiler
    {
        static readonly Vector2 _textScale, _textScale2;
        static float _ySpacing2;
        static readonly List<Color> _colors;
        static Dictionary<string, Profile> _profiles;
        static bool _enabled;
        public static bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (!value)
                    foreach (var p in _profiles.Values)
                        p.Stopwatch.Reset();
            }
        }
        public static double TotalTime { get; private set; }

        static Profiler()
        {
            _textScale = new Vector2(.25f);
            _textScale2 = new Vector2(.2f);
            _ySpacing2 = (100 * _textScale2.Y);
            _colors = new List<Color>
            {
                Color.Blue,
                Color.Red,
                Color.Yellow,
                Color.Green,
                Color.Pink,
                Color.Purple,
                Color.Brown,
                Color.White,
                Color.Lime,
                Color.Magenta,
                Color.Maroon,
                Color.Navy,
                Color.Olive,
                Color.Orange,
                Color.Orchid,
                Color.PaleGreen,
                Color.Plum,
                Color.Salmon,
                Color.Silver,
                Color.SpringGreen,
                Color.SteelBlue,
                Color.Tan,
                Color.Teal,
                Color.Turquoise,
                Color.Violet
            };
            _profiles = new Dictionary<string, Profile>();
        }

        public static void Start(string name)
        {
            if (!_enabled)
                return;
            if (!_profiles.ContainsKey(name))
            {
                _profiles.Add(name, new Profile());
                if (_colors.Count > 0)
                {
                    var index = Mathf.Random(_colors.Count - 1);
                    _profiles[name].Color = _colors[index];
                    _colors.RemoveAt(index);
                }
                else
                    _profiles[name].Color = new Color(Mathf.Random(255), Mathf.Random(255), Mathf.Random(255));
            }
            _profiles[name].Stopwatch.Start();
        }
        public static void Stop(string name)
        {
            if (!_enabled)
                return;
            _profiles[name].Records[_profiles[name].Index++] = _profiles[name].Stopwatch.Elapsed.TotalMilliseconds;
            if (_profiles[name].Index >= _profiles[name].Records.Length)
                _profiles[name].Index = 0;
            if (_profiles[name].Recorded < _profiles[name].Records.Length)
                _profiles[name].Recorded++;
            _profiles[name].Stopwatch.Reset();
            TotalTime = Math.Max(0, (TotalTime - _profiles[name].Average));
            _profiles[name].Average = 0;
            for (var i = 0; i < _profiles[name].Recorded; i++)
                _profiles[name].Average += _profiles[name].Records[i];
            _profiles[name].Average /= _profiles[name].Recorded;
            _profiles[name].Average = Math.Round(_profiles[name].Average, 3);
            TotalTime += _profiles[name].Average;
        }

        static readonly string[] SizeSuffixes = { "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (value == 0)
                return "0 bytes";
            if (value == 1)
                return "1 byte";
            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }
            if (mag == 0)
                return $"{adjustedSize} bytes";
            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[--mag]);
        }

        private const float curRowPad = 25, avgRowPad = 29;
        private const int nameYOff = -9, scoresYOff = -13;
        public static void Draw(SpriteFont font)
        {
            lock (_profiles)
            {
                Screen.Begin();
                var textPosition = new Vector2((Screen.ViewportWidth / 8f), (Screen.ViewportHeight - 100));
                var barPosition = new Vector2(textPosition.X, (textPosition.Y + 50));
                var barWidth = (int)Math.Ceiling(Screen.ViewportWidth - (barPosition.X * 2));
                Screen.FillRectangle(new Rectangle((int)barPosition.X, (int)(barPosition.Y - 1), barWidth, 17), (Color.Black * .2f));
                Screen.DrawLine(new Vector2((barPosition.X - 1), (barPosition.Y - 2)), new Vector2((barPosition.X - 1), (barPosition.Y + 17)), Color.White);
                barPosition.X += (barWidth + 2);
                Screen.DrawLine(new Vector2((barPosition.X - 1), (barPosition.Y - 2)), new Vector2((barPosition.X - 1), (barPosition.Y + 17)), Color.White);
                barPosition.X -= (barWidth + 3);
                var index = 1;
                foreach (var name in _profiles.Keys)
                {
                    var profile = _profiles[name];
                    var size = (font.MeasureString($"{name} | {profile.Average} ms") * _textScale);
                    var rectangle = new Rectangle((int)textPosition.X, (int)(textPosition.Y - (size.Y / 2)), (int)Math.Ceiling(size.X + 20), Math.Max(14, (int)Math.Ceiling(size.Y + 4)));
                    Screen.FillRectangle(rectangle, (Color.Black * .2f));
                    Screen.DrawRectangle(new Rectangle((rectangle.X + 4), (rectangle.Y + (rectangle.Height / 2) - 5), 10, 10), profile.Color, Color.WhiteSmoke, 1);
                    textPosition.X += 18;
                    textPosition.Y += 2;
                    Screen.DrawString($"{name} | {profile.Average} ms", font, textPosition, Color.White, Color.Black, new Origin(0, .5f, true), _textScale);
                    textPosition.X -= 18;
                    textPosition.Y -= (4 + rectangle.Height);
                    var barRectangle = new Rectangle((int)(barPosition.X + 1), (int)barPosition.Y, ((index++ >= _profiles.Count) ? (int)(barWidth - (barPosition.X - (Screen.ViewportWidth / 8f)) - 2) : (int)Math.Round(((profile.Average / TotalTime) * barWidth) - 2)), 15);
                    Screen.FillRectangle(barRectangle, profile.Color);
                    barPosition.X += (barRectangle.Width + 2);
                }
                Screen.End();
            }
            if (Network.Peer == null)
                return;
            const float yBegin = 30;
            Screen.Begin();
            //Screen.DrawString($"Networking:\n{Network.Peer.Statistics.ToString()}}", font, new Vector2(30, yBegin), Color.White, Color.Black, _textScale2);
            Screen.DrawString("Networking:", font, new Vector2(30, yBegin), Color.White, Color.Black, _textScale2);
            Screen.DrawString($"Upload (total): {SizeSuffix(Network.Statistics.UploadedBytes)}", font, new Vector2(45, (yBegin + (_ySpacing2 * 1))), Color.White, Color.Black, _textScale2);
            Screen.DrawString($"Upload (p/sec): {SizeSuffix(Network.Statistics.UploadBytesPerSec)}", font, new Vector2(45, (yBegin + (_ySpacing2 * 2))), Color.White, Color.Black, _textScale2);
            Screen.DrawString($"Download (total): {SizeSuffix(Network.Statistics.DownloadedBytes)}", font, new Vector2(45, (yBegin + (_ySpacing2 * 3))), Color.White, Color.Black, _textScale2);
            Screen.DrawString($"Download (p/sec): {SizeSuffix(Network.Statistics.DownloadBytesPerSec)}", font, new Vector2(45, (yBegin + (_ySpacing2 * 4))), Color.White, Color.Black, _textScale2);
            Screen.End();
        }

        class Profile
        {
            public byte Index, Recorded;
            public double[] Records;
            public double Average;
            public Stopwatch Stopwatch;
            public Color Color;

            public Profile()
            {
                Records = new double[50];
                Stopwatch = new Stopwatch();
            }
        }
    }
}