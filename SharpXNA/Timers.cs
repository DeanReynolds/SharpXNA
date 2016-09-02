using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SharpXNA
{
    public static class Timers
    {
        private static Dictionary<string, Timer> array;

        static Timers() { array = new Dictionary<string, Timer>(); }

        public static Timer Add(string name, double interval) { if (!array.ContainsKey(name)) array.Add(name, new Timer(interval)); else array[name].Interval = interval; return array[name]; }
        public static void Remove(string name) { if (array.ContainsKey(name)) array.Remove(name); } 
        public static void Clear() { array.Clear(); }

        public static void Update(GameTime time)
        {
            foreach (var timer in array.Values)
            {
                if (timer.Time >= timer.Interval) timer.Time -= timer.Interval;
                timer.Time += time.ElapsedGameTime.TotalSeconds;
            }
        }

        public static bool Tick(string name) { return (array.ContainsKey(name) && array[name].Tick); }

        public class Timer
        {
            public bool Tick { get { return (Time >= Interval); } }

            public double Interval, Time;

            public Timer(double interval) { this.Interval = interval; }
        }
    }
}