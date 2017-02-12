using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpXNA.Plugins;

namespace SharpXNA
{
    public static class Profiler
    {
        private static bool _enabled;
        public static bool Enabled { get { return _enabled; } set { _enabled = value; if (!value) foreach (var p in Profiles.Values) p.Stop(); } }

        public static Dictionary<string, Profile> Profiles = new Dictionary<string, Profile>();

        public static void Start(string name, int max = 20)
        {
            if (!Enabled) return;
            if (!Profiles.ContainsKey(name)) Profiles.Add(name, new Profile(max));
            Profiles[name].Start();
        }
        public static void Stop(string name)
        {
            if (!Profiles.ContainsKey(name)) return;
            Profiles[name].Stop();
        }

        private const float curRowPad = 25, avgRowPad = 29;
        private const int nameYOff = -9, scoresYOff = -13;
        public static void Draw(SpriteFont font, int width) { Draw(Screen.Batch, font, width); }
        public static void Draw(Batch batch, SpriteFont font, int width)
        {
            lock (Profiles)
            {
                batch.Begin(SpriteSortMode.Deferred, SamplerState.PointClamp);
                int x = (Screen.ViewportWidth - (width + 5)), y = 5;
                batch.FillRectangle(new Rectangle(x, y, width, (Screen.ViewportHeight - 10)), (Color.Black*.8f), 1);
                var str = $"Update FPS: {Math.Round(Performance.UpdateFPS.Current)} (Avg: {Math.Round(Performance.UpdateFPS.Average)}) - Draw FPS: {Math.Round(Performance.DrawFPS.Current)} (Avg: {Math.Round(Math.Round(Performance.DrawFPS.Average))})";
                var fontScale = new Vector2(.15f);
                batch.DrawString(str, font, new Vector2((x + (width/2f)), (y + ((font.MeasureString(str).Y*fontScale.Y)/2))), (Color.White*.8f), (Color.Black*.8f), Origin.Center, fontScale);
                y += (int) (Math.Ceiling(font.MeasureString(str).Y*fontScale.Y) + 1);
                batch.DrawLine(new Vector2(x, y), new Vector2((x + width), y), (Color.White*.8f));
                batch.DrawLine(new Vector2(x, (y + 18)), new Vector2((x + width), (y + 18)), (Color.White*.8f));
                batch.DrawLine(new Vector2((x + ((width/5)*3)), y), new Vector2((x + ((width/5)*3)), (y + (Screen.ViewportHeight - 25))), (Color.White*.8f));
                batch.DrawLine(new Vector2((x + ((width/5)*4)), y), new Vector2((x + ((width/5)*4)), (y + (Screen.ViewportHeight - 25))), (Color.White*.8f));
                x += 3;
                y += 3;
                fontScale = new Vector2(.1f);
                batch.DrawString("Name", font, new Vector2(x, y), (Color.White*.8f), fontScale);
                batch.DrawString("Milliseconds", font, new Vector2((x + ((width / 5) * 3)), y), (Color.White * .8f), fontScale);
                batch.DrawString("Ticks", font, new Vector2((x + ((width/5)*4) - 1), y), (Color.White*.8f), fontScale);
                fontScale = new Vector2(.125f);
                y += 5;
                if (Profiles != null)
                    foreach (var k in Profiles.Keys)
                    {
                        var t = Profiles[k];
                        y += 26;
                        batch.DrawLine(new Vector2((x - 5), (y + 16)), new Vector2((x + width), (y + 16)), (Color.Silver*.4f));
                        batch.DrawString(k, font, new Vector2(x, (y + nameYOff)), (Color.White*.8f), new Vector2(.15f));
                        y += scoresYOff;
                        batch.DrawString("Cur", font, new Vector2((x + ((width/5)*3)), y), (Color.Gray*.8f), fontScale);
                        batch.DrawString("Cur", font, new Vector2((x + ((width/5)*4)), y), (Color.Gray*.8f), fontScale);
                        batch.DrawString($"{Math.Round(t.Milliseconds.Average, 3)} ms", font, new Vector2((x + ((width/5)*3) + curRowPad), y), (Color.White*.8f), fontScale);
                        batch.DrawString(t.Ticks.Current.ToString("#,##"), font, new Vector2((x + ((width/5)*4) + curRowPad), y), (Color.White*.8f), fontScale);
                        y += 12;
                        batch.DrawString("Avg", font, new Vector2((x + ((width/5)*3)), y), (Color.Gray*.8f), fontScale);
                        batch.DrawString("Avg", font, new Vector2((x + ((width/5)*4)), y), (Color.Gray*.8f), fontScale);
                        batch.DrawString($"{Math.Round(t.Milliseconds.Average, 3)} ms", font, new Vector2((x + ((width/5)*3) + avgRowPad), y), (Color.White*.8f), fontScale);
                        batch.DrawString(t.Ticks.Average.ToString("#,##0"), font, new Vector2((x + ((width/5)*4) + avgRowPad), y), (Color.White*.8f), fontScale);
                        y -= scoresYOff;
                        y -= 8;
                    }
                batch.End();
            }
        }

        public class Profile
        {
            public Profile(int max) { Ticks = new Plugins.Buffer(max); Milliseconds = new Plugins.Buffer(max); _stopwatch = new Stopwatch(); }

            public Plugins.Buffer Ticks, Milliseconds;
            private readonly Stopwatch _stopwatch;

            public void Start() { _stopwatch.Start(); }
            public void Stop()
            {
                Ticks.Record(_stopwatch.ElapsedTicks);
                Milliseconds.Record(_stopwatch.Elapsed.TotalMilliseconds);
                _stopwatch.Reset();
            }
        }
    }
}