using Microsoft.Xna.Framework;
using SharpXNA.Collision;
using System;

namespace SharpXNA
{
    public static class Mathf
    {
        public const float Sqrt2 = 1.41421356237f, Pi = 3.1415926536f, PiOver2 = 1.5707963268f, PiOver4 = .7853981634f, PiOver8 = .3926990817f, PiOver16 = .19634954085f, PiOver32 = .098174770425f, PiOver64 = .049087385212f, PiOver128 = .024543692606f, PiOver256 = .012271846303f, PiOver512 = .0061359231515f, PiOver1024 = .0030679615758f;

        private static Random _random;

        static Mathf() { _random = new Random(); }

        public static Vector2 Rotate(Vector2 position, float angle) => Vector2.Transform(position, Matrix.CreateRotationZ(angle));
        public static Vector2 Rotate(Vector2 position, float angle, Vector2 origin) => (Vector2.Transform((position - origin), Matrix.CreateRotationZ(angle)) + origin);

        public static float Lerp(this float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }
        public static float LerpAngle(this float sourceAngle, float destinationAngle, float amount)
        {
            float c, d;
            if (destinationAngle < sourceAngle)
            {
                c = destinationAngle + MathHelper.TwoPi;
                d = c - sourceAngle > sourceAngle - destinationAngle
                    ? MathHelper.Lerp(sourceAngle, destinationAngle, amount)
                    : MathHelper.Lerp(sourceAngle, c, amount);
            }
            else if (destinationAngle > sourceAngle)
            {
                c = destinationAngle - MathHelper.TwoPi;
                d = destinationAngle - sourceAngle > sourceAngle - c
                    ? MathHelper.Lerp(sourceAngle, c, amount)
                    : MathHelper.Lerp(sourceAngle, destinationAngle, amount);
            }
            else return sourceAngle;
            return MathHelper.WrapAngle(d);
        }

        public static float Angle(Vector2 source, Vector2 destination) => (float)Math.Atan2((destination.Y - source.Y), (destination.X - source.X));
        public static double Wrap(float value, float max, float min = 0)
        {
            value -= min;
            max -= min;
            if (max == 0) return min;
            value = value % max;
            value += min;
            while (value < min)
                value += max;
            return value;
        }

        public static double RandomDouble() { return _random.NextDouble(); }
        public static int Random(int min, int max) { return _random.Next(min, (max + 1)); }
        public static float Random(float min, float max) { return (float)Random(min, (double)max); }
        public static double Random(double min, double max) { return (min + (_random.NextDouble() * Math.Abs(max - min))); }
        public static long Random(long min, long max) { return (long)(min + (_random.NextDouble() * Math.Abs(max - min))); }
        public static int Random(int max) { return Random(0, max); }
        public static float Random(float max) { return (float)Random((double)max); }
        public static double Random(double max) { return Random(0, max); }
        public static long Random(long max) { return Random(0, max); }

        public static Vector2 Move(ref Vector2 position, float angle, float velocity) { return (position += new Vector2(((float)Math.Cos(angle) * velocity), ((float)Math.Sin(angle) * velocity))); }
        public static Vector2 Move(ref Vector2 position, Vector2 other, float velocity) { return Move(ref position, Angle(position, other), velocity); }
        public static Vector2 Move(Vector2 position, float angle, float velocity) { return (position + new Vector2(((float)Math.Cos(angle) * velocity), ((float)Math.Sin(angle) * velocity))); }
        public static Vector2 Move(Vector2 position, Vector2 other, float velocity) { return Move(position, Angle(position, other), velocity); }

        public static float VolumeFromDistance(this Vector2 from, Vector2 to, float fade, float max) => MathHelper.Clamp(((fade - (Vector2.Distance(from, to) - max)) / fade), 0, 1);
        public static float AngleDifference(float a, float b)
        {
            float aD = MathHelper.ToDegrees(a),
                bD = MathHelper.ToDegrees(b),
                difference = (Math.Abs(aD - bD) % 360);
            if (difference > 180)
                difference = (360 - difference);
            return MathHelper.ToRadians(difference);
        }

        public static T Pick<T>(params T[] values) => values[Random(values.Length - 1)];
        public static bool Chance(float value) => (_random.NextDouble() <= (value / 100));

        public static void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v)
        {
            float min, max;
            if (r < g)
            {
                if (r < b) { min = r; max = ((g > b) ? g : b); }
                else { min = b; max = g; }
            }
            else
            {
                if (r < b) { min = g; max = b; }
                else { min = ((g > b) ? b : g); max = r; }
            }
            var delta = (max - min);
            v = max;
            if (max != 0)
            {
                s = (delta / max);
                if (r == max) h = ((g - b) / delta);
                else if (g == max) h = (2 + (b - r) / delta);
                else h = (4 + (r - g) / delta);
                h *= 60;
                if (h < 0) h += 360;
            }
            else { s = 0; h = -1; }
        }
        public static void HSVtoRGB(float h, float s, float v, out float r, out float g, out float b)
        {
            h = (h - ((int)(h / 360) * 360));
            int i; float f, p, q, t;
            if (s == 0) { r = g = b = v; }
            else
            {
                h /= 60;
                i = (int)h;
                f = (h - i);
                p = (v * (1 - s));
                q = (v * (1 - s * f));
                t = (v * (1 - s * (1 - f)));
                if (i == 0) { r = v; g = t; b = p; }
                else if (i == 1) { r = q; g = v; b = p; }
                else if (i == 2) { r = p; g = v; b = t; }
                else if (i == 3) { r = p; g = q; b = v; }
                else if (i == 4) { r = t; g = p; b = v; }
                else { r = v; g = p; b = q; }
            }
        }

        public static Color ChangeHue(this Color color, float value)
        {
            float h, s, v;
            RGBtoHSV(color.R, color.G, color.B, out h, out s, out v);
            h = value;
            float r, g, b;
            HSVtoRGB(h, s, v, out r, out g, out b);
            return new Color((byte)r, (byte)g, (byte)b, color.A);
        }
        public static Color ChangeSaturation(this Color color, float value)
        {
            float h, s, v;
            RGBtoHSV(color.R, color.G, color.B, out h, out s, out v);
            s = value;
            float r, g, b;
            HSVtoRGB(h, s, v, out r, out g, out b);
            return new Color((byte)r, (byte)g, (byte)b, color.A);
        }
        public static Color ChangeBrightness(this Color color, float value)
        {
            float h, s, v;
            RGBtoHSV(color.R, color.G, color.B, out h, out s, out v);
            v = value;
            float r, g, b;
            HSVtoRGB(h, s, v, out r, out g, out b);
            return new Color((byte)r, (byte)g, (byte)b, color.A);
        }
    }
}