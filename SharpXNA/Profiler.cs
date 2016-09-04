using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpXNA.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpXNA.Plugins;
using static SharpXNA.Textures;

namespace SharpXNA
{
    public static class Profiler
    {
        private static bool enabled;
        public static bool Enabled { get { return enabled; } set { enabled = value; if (!value) foreach (var key in Profiles.Keys) if (Profiles[key].Stopwatch.IsRunning) Stop(key); } }

        public static Dictionary<string, Profile> Profiles = new Dictionary<string, Profile>();

        public static void Start(string name, int max = 20)
        {
            if (!Enabled) return;
            if (!Profiles.ContainsKey(name)) Profiles.Add(name, new Profile(max));
            Profiles[name].Stopwatch.Start();
        }
        public static void Stop(string name)
        {
            if (!Profiles.ContainsKey(name)) return;
            if (Profiles[name].Stopwatch.IsRunning)
            {
                Profiles[name].Stopwatch.Stop();
                Profiles[name].Ticks.Record(Profiles[name].Stopwatch.ElapsedTicks);
                Profiles[name].Milliseconds.Record(Profiles[name].Stopwatch.Elapsed.TotalMilliseconds);
                Profiles[name].Seconds.Record(Profiles[name].Stopwatch.Elapsed.TotalSeconds);
                Profiles[name].Stopwatch.Reset();
            }
        }

        private const float curRowPad = 25, avgRowPad = 29;
        private const int nameYOff = -9, scoresYOff = -13;
        public static void Draw(int width) { Draw(Screen.Batch, Font.Load("Consolas"), width); }
        public static void Draw(Batch batch, int width) { Draw(batch, Font.Load("Consolas"), width); }
        public static void Draw(SpriteFont font, int width) { Draw(Screen.Batch, font, width); }
        public static void Draw(Batch batch, SpriteFont font, int width)
        {
            Start("Profiler Draw", 5);
            bool begun = true;
            if (!batch.Begun) { begun = false; batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp); }
            int x = (Screen.BackBufferWidth - (width + 5)), y = 5;
            batch.FillRectangle(new Rectangle(x, y, width, (Screen.BackBufferHeight - 10)), (Color.Black * .8f), 1);
            var str = string.Format("Update FPS: {0} (Avg: {1}) - Draw FPS: {2} (Avg: {3})",
                Math.Round(Performance.UpdateFPS.Current), Math.Round(Performance.UpdateFPS.Average),
                Math.Round(Performance.DrawFPS.Current), Math.Round(Math.Round(Performance.DrawFPS.Average)));
            Vector2 fontScale = new Vector2(.15f);
            batch.DrawString(str, font, new Vector2((x + (width / 2f)), (y + ((font.MeasureString(str).Y * fontScale.Y) / 2))), (Color.White * .8f), (Color.Black * .8f), Origin.Center, fontScale);
            y += (int)(Math.Ceiling(font.MeasureString(str).Y * fontScale.Y) + 1);
            new Line(new Vector2(x, y), new Vector2((x + width), y)).Draw(batch, (Color.White * .8f));
            new Line(new Vector2(x, (y + 18)), new Vector2((x + width), (y + 18))).Draw(batch, (Color.White * .8f));
            new Line(new Vector2((x + ((width / 5) * 2)), y), new Vector2((x + ((width / 5) * 2)), (y + (Screen.BackBufferHeight - 25)))).Draw(batch, (Color.White * .8f));
            new Line(new Vector2((x + ((width / 5) * 3)), y), new Vector2((x + ((width / 5) * 3)), (y + (Screen.BackBufferHeight - 25)))).Draw(batch, (Color.White * .8f));
            new Line(new Vector2((x + ((width / 5) * 4)), y), new Vector2((x + ((width / 5) * 4)), (y + (Screen.BackBufferHeight - 25)))).Draw(batch, (Color.White * .8f));
            x += 3; y += 3;
            fontScale = new Vector2(.1f);
            batch.DrawString("Name", font, new Vector2(x, y), (Color.White * .8f), fontScale);
            batch.DrawString("Milliseconds", font, new Vector2((x + ((width / 5) * 2)), y), (Color.White * .8f), fontScale);
            batch.DrawString("Seconds", font, new Vector2((x + ((width / 5) * 3) - 1), y), (Color.White * .8f), fontScale);
            batch.DrawString("Ticks", font, new Vector2((x + ((width / 5) * 4) - 1), y), (Color.White * .8f), fontScale);
            y += 5;
            if (Profiles != null)
                foreach (var name in Profiles.Keys)
                {
                    y += 26;
                    new Line(new Vector2((x - 5), (y + 16)), new Vector2((x + width), (y + 16))).Draw(batch, (Color.Silver * .4f));
                    fontScale = new Vector2(.125f);
                    batch.DrawString(name, font, new Vector2(x, (y + nameYOff)), (Color.White * .8f), new Vector2(.15f));
                    y += scoresYOff;
                    batch.DrawString("Cur", font, new Vector2((x + ((width / 5) * 2)), y), (Color.Gray * .8f), fontScale);
                    batch.DrawString("Cur", font, new Vector2((x + ((width / 5) * 3)), y), (Color.Gray * .8f), fontScale);
                    batch.DrawString("Cur", font, new Vector2((x + ((width / 5) * 4)), y), (Color.Gray * .8f), fontScale);
                    batch.DrawString((Math.Round(Profiles[name].Milliseconds.Current, 3) + " ms"), font, new Vector2((x + ((width / 5) * 2) + curRowPad), y), (Color.White * .8f), fontScale);
                    batch.DrawString((Math.Round(Profiles[name].Seconds.Current, 4) + "s"), font, new Vector2((x + ((width / 5) * 3) + curRowPad), y), (Color.White * .8f), fontScale);
                    batch.DrawString(Profiles[name].Ticks.Current.ToString("#,##"), font, new Vector2((x + ((width / 5) * 4) + curRowPad), y), (Color.White * .8f), fontScale);
                    y += 12;
                    batch.DrawString("Avg", font, new Vector2((x + ((width / 5) * 2)), y), (Color.Gray * .8f), fontScale);
                    batch.DrawString("Avg", font, new Vector2((x + ((width / 5) * 3)), y), (Color.Gray * .8f), fontScale);
                    batch.DrawString("Avg", font, new Vector2((x + ((width / 5) * 4)), y), (Color.Gray * .8f), fontScale);
                    batch.DrawString((Math.Round(Profiles[name].Milliseconds.Average, 3) + " ms"), font, new Vector2((x + ((width / 5) * 2) + avgRowPad), y), (Color.White * .8f), fontScale);
                    batch.DrawString((Math.Round(Profiles[name].Seconds.Average, 4) + "s"), font, new Vector2((x + ((width / 5) * 3) + avgRowPad), y), (Color.White * .8f), fontScale);
                    batch.DrawString(Profiles[name].Ticks.Average.ToString("#,##"), font, new Vector2((x + ((width / 5) * 4) + avgRowPad), y), (Color.White * .8f), fontScale);
                    y -= scoresYOff;
                    y -= 8;
                }
            if (!begun) batch.End();
            Stop("Profiler Draw");
        }

        public class Profile
        {
            public Profile(int max) { Ticks = new Plugins.Buffer(max); Milliseconds = new Plugins.Buffer(max); Seconds = new Plugins.Buffer(max); TimeSpans = new CustomTimeSpan[max]; Stopwatch = new Stopwatch(); }

            public Plugins.Buffer Ticks, Milliseconds, Seconds;
            public Stopwatch Stopwatch;
            public CustomTimeSpan[] TimeSpans;

            public struct CustomTimeSpan
            {
                public CustomTimeSpan(long ticks, double milliseconds, double seconds) { Ticks = ticks; Milliseconds = milliseconds; Seconds = seconds; }

                public long Ticks;
                public double Milliseconds, Seconds;
            }
        }
    }
}