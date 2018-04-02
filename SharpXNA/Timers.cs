using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SharpXNA
{
    public static class Timers
    {
        private static readonly Dictionary<string, Timer> array;

        static Timers() { array = new Dictionary<string, Timer>(); }

        public static Timer Add(string name, double interval)
        {
            if (!array.ContainsKey(name))
                array.Add(name, new Timer(interval));
            else
            {
                array[name].Time = 0;
                array[name].Interval = interval;
            }
            return array[name];
        }
        public static void Remove(string name)
        {
            if (array.ContainsKey(name))
                array.Remove(name);
        }
        public static void Clear() { array.Clear(); }

        public static void Update(GameTime time)
        {
            foreach (var key in array.Keys)
                array[key].Time += time.ElapsedGameTime.TotalSeconds;
        }

        public static bool Tick(string name)
        {
            if (!array.ContainsKey(name))
                return false;
            var timer = array[name];
            if (timer.Time >= timer.Interval)
            {
                timer.Time -= timer.Interval;
                return true;
            }
            return false;
        }

        public class Timer
        {
            public double Time, Interval;

            public Timer(double interval) { Interval = interval; }
        }
    }
}