using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SharpXNA
{
    public static class Timers
    {
        static readonly Dictionary<string, Timer> _timers;

        static Timers() { _timers = new Dictionary<string, Timer>(); }

        public static void Update(GameTime time)
        {
            foreach (var key in _timers.Keys)
            {
                Timer timer = _timers[key];
                timer.Tick = false;
                timer.Time += time.ElapsedGameTime.TotalSeconds;
            }
        }

        public static void Add(string name, double interval)
        {
            if (!_timers.ContainsKey(name))
                _timers.Add(name, new Timer(interval));
            else
            {
                _timers[name].Time = 0;
                _timers[name].Interval = interval;
            }
        }
        public static void Remove(string name)
        {
            if (_timers.ContainsKey(name))
                _timers.Remove(name);
        }
        public static void Clear() => _timers.Clear();
        public static bool Tick(string name)
        {
            if (!_timers.ContainsKey(name))
                return false;
            Timer timer = _timers[name];
            if (timer.Tick)
                return true;
            if (timer.Time >= timer.Interval)
            {
                timer.Tick = true;
                timer.Time -= timer.Interval;
                return true;
            }
            return false;
        }

        class Timer
        {
            public double Time, Interval;
            public bool Tick;

            public Timer(double interval) { Interval = interval; }
        }
    }
}