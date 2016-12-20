using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SharpXNA
{
    public static class Globe
    {
        public static bool IsActive;
        private static GameServiceContainer _container;
        private static Random _systemRandom;
        public static string ExecutableDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

        static Globe() { _container = new GameServiceContainer(); _systemRandom = new Random(); }

        public static Form Form { get { return Get<Form>(); } set { Add(value); } }
        public static Viewport Viewport { get { return Get<Viewport>(); } set { Add(value); } }
        public static GameWindow GameWindow { get { return Get<GameWindow>(); } set { Add(value); } }
        public static ContentManager ContentManager { get { return Get<ContentManager>(); } set { Add(value); } }
        public static GraphicsDevice GraphicsDevice { get { return Get<GraphicsDevice>(); } set { Add(value); } }
        public static GraphicsAdapter GraphicsAdapter { get { return Get<GraphicsAdapter>(); } set { Add(value); } }
        public static GraphicsDeviceManager GraphicsDeviceManager { get { return Get<GraphicsDeviceManager>(); } set { Add(value); } }

        public static T Get<T>() { return (T)_container.GetService(typeof(T)); }
        public static void Add<T>(T service) { _container.AddService(typeof(T), service); }
        public static void Remove<T>() { _container.RemoveService(typeof(T)); }

        public static Assembly Assembly { get { return Assembly.GetExecutingAssembly(); } }

        public static string[] StringsBetween(string text, char both) { return StringsBetween(text, both, both); }
        public static string[] StringsBetween(string text, string both) { return StringsBetween(text, both, both); }
        public static string[] StringsBetween(string text, char start, char end) { return StringsBetween(text, start.ToString(), end.ToString()); }
        public static string[] StringsBetween(string text, string start, string end)
        {
            var matches = Regex.Matches(text, ("(?<=" + Regex.Escape(start) + ").*?(?=" + Regex.Escape(end) + ")"));
            if (matches.Count > 0)
            {
                var values = new string[matches.Count];
                for (var i = 0; i < matches.Count; i++) values[i] = matches[i].Value;
                return values;
            }
            return null;
        }
        public static string[] Between(this string text, char both) { return StringsBetween(text, both); }
        public static string[] Between(this string text, string both) { return StringsBetween(text, both); }
        public static string[] Between(this string text, char start, char end) { return StringsBetween(text, start, end); }
        public static string[] Between(this string text, string start, string end) { return StringsBetween(text, start, end); }
        public static bool IsNuberic(this string text) { int temp; return int.TryParse(text, out temp); }

        public static Vector2 Rotate(Vector2 position, float angle) { return Vector2.Transform(position, Matrix.CreateRotationZ(angle)); }
        public static Vector2 Rotate(Vector2 position, float angle, Vector2 origin) { return (Vector2.Transform((position - origin), Matrix.CreateRotationZ(angle)) + origin); }
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
        public static float Lerp(this float start, float end, float amount) { float difference = end - start; float adjusted = difference * amount; return start + adjusted; }
        public static float Angle(Vector2 source, Vector2 destination) { return (float)Math.Atan2((destination.Y - source.Y), (destination.X - source.X)); }
        public static double Wrap(double value, double max, double min = 0)
        {
            value -= min;
            max -= min;
            if (max == 0) return min;
            value = value % max;
            value += min;
            while (value < min) value += max;
            return value;
        }

        public static Vector2 Move(ref Vector2 position, float angle, float velocity) { return (position += new Vector2(((float)Math.Cos(angle) * velocity), ((float)Math.Sin(angle) * velocity))); }
        public static Vector2 Move(ref Vector2 position, Vector2 other, float velocity) { return Move(ref position, Angle(position, other), velocity); }
        public static Vector2 Move(Vector2 position, float angle, float velocity) { return (position + new Vector2(((float)Math.Cos(angle) * velocity), ((float)Math.Sin(angle) * velocity))); }
        public static Vector2 Move(Vector2 position, Vector2 other, float velocity) { return Move(position, Angle(position, other), velocity); }

        public static float VolumeFromDistance(this Vector2 from, Vector2 to, float fade, float max) { return MathHelper.Clamp(((fade - (Vector2.Distance(from, to) - max)) / fade), 0, 1); }
        public static float AngleDifference(float a, float b)
        {
            float aD = MathHelper.ToDegrees(a), bD = MathHelper.ToDegrees(b), difference = Math.Abs(aD - bD) % 360;
            if (difference > 180) difference = 360 - difference;
            return MathHelper.ToRadians(difference);
        }

        public static double RandomDouble() { return _systemRandom.NextDouble(); }
        public static int Random(int min, int max) { return _systemRandom.Next(min, (max + 1)); }
        public static float Random(float min, float max) { return (float)Random(min, (double)max); }
        public static double Random(double min, double max) { return (min + (_systemRandom.NextDouble() * Math.Abs(max - min))); }
        public static long Random(long min, long max) { return (long)(min + (_systemRandom.NextDouble() * Math.Abs(max - min))); }
        public static int Random(int max) { return Random(0, max); }
        public static float Random(float max) { return (float)Random((double)max); }
        public static double Random(double max) { return Random(0, max); }
        public static long Random(long max) { return Random(0, max); }

        public static byte Min(params byte[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static decimal Min(params decimal[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static double Min(params double[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static float Min(params float[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static int Min(params int[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static long Min(params long[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static sbyte Min(params sbyte[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static short Min(params short[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static uint Min(params uint[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static ulong Min(params ulong[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }
        public static ushort Min(params ushort[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Min(value, values[i]); return value; }

        public static byte Max(params byte[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static decimal Max(params decimal[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static double Max(params double[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static float Max(params float[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static int Max(params int[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static long Max(params long[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static sbyte Max(params sbyte[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static short Max(params short[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static uint Max(params uint[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static ulong Max(params ulong[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }
        public static ushort Max(params ushort[] values) { var value = values[0]; for (var i = 1; i < values.Length; i++) value = System.Math.Max(value, values[i]); return value; }

        public static double Avg(params byte[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static decimal Avg(params decimal[] values) { decimal value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params double[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params float[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params int[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params long[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params sbyte[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params short[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params uint[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params ulong[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }
        public static double Avg(params ushort[] values) { double value = values[0]; for (int i = 1; i < values.Length; i++) value += values[i]; return (value / values.Length); }

        public static T Pick<T>(params T[] values) { return (T)values[Random(values.Length - 1)]; }
        public static bool Chance(float value) { return (Random(100f) <= value); }

        public static bool Matches<T>(this T obj, params T[] args) { return args.Contains(obj); }
        public static bool Matches<T>(this List<T> original, List<T> other)
        {
            if ((original == null) && (other == null)) return true;
            else if (((original == null) && (other != null)) || ((other == null) && (original != null))) return false;
            else if (original.Count == other.Count) { foreach (T obj in original) if (!other.Contains(obj)) return false; return true; }
            else return false;
        }
        public static List<T> Merge<T>(this List<T> original, List<T> other, bool ignoreDuplicates)
        {
            int capacity = (((original != null) ? original.Count : 0) + ((other != null) ? other.Count : 0));
            if (capacity == 0) return null;
            else
            {
                List<T> newList = new List<T>(capacity);
                if (original != null) newList.AddRange(original);
                if (other != null) foreach (T obj in other) if (!ignoreDuplicates || !newList.Contains(obj)) newList.Add(obj);
                return newList;
            }
        }
        public static List<T> RemoveRange<T>(this List<T> original, List<T> other)
        {
            if (original == null) return null;
            else
            {
                List<T> newList = new List<T>(original.Count);
                newList.AddRange(original);
                if (other != null) foreach (T obj in other) newList.Remove(obj);
                return newList;
            }
        }
        public static System.Drawing.Point ToSystemPoint(this Point point) { return new System.Drawing.Point(point.X, point.Y); }
        
        public static Rectangle BoxMask(Texture2D texture, Vector2 position, float angle)
        {
            Vector2 topRight, bottomRight, bottomLeft, topLeft;
            float widthHalved = (texture.Width / 2f), heightHalved = (texture.Height / 2f);
            topRight = Rotate(new Vector2(widthHalved, -heightHalved), angle);
            bottomRight = Rotate(new Vector2(widthHalved, heightHalved), angle);
            bottomLeft = Rotate(new Vector2(-widthHalved, heightHalved), angle);
            topLeft = Rotate(new Vector2(-widthHalved, -heightHalved), angle);
            float minX = Min(topRight.X, bottomRight.X, bottomLeft.X, topLeft.X), minY = Min(topRight.Y, bottomRight.Y, bottomLeft.Y, topLeft.Y),
                maxX = Max(topRight.X, bottomRight.X, bottomLeft.X, topLeft.X), maxY = Max(topRight.Y, bottomRight.Y, bottomLeft.Y, topLeft.Y);
            return new Rectangle((int)(position.X + minX), (int)(position.Y + minY), (int)Math.Ceiling(maxX - minX), (int)Math.Ceiling(maxY - minY));
        }
        public static Rectangle OpaqueBoxMask(Texture2D texture, Vector2 position, float angle)
        {
            Vector2 topRight, bottomRight, bottomLeft, topLeft;
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                {
                    if (colors[(x + (y * texture.Width))].A == 255)
                    {
                        minX = Math.Min(minX, x); minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
                    }
                }
            float width = (maxX - minX), height = (maxY - minY), widthHalved = (width / 2f), heightHalved = (height / 2f);
            topRight = Rotate(new Vector2(widthHalved, -heightHalved), angle);
            bottomRight = Rotate(new Vector2(widthHalved, heightHalved), angle);
            bottomLeft = Rotate(new Vector2(-widthHalved, heightHalved), angle);
            topLeft = Rotate(new Vector2(-widthHalved, -heightHalved), angle);
            minX = Min(topRight.X, bottomRight.X, bottomLeft.X, topLeft.X); minY = Min(topRight.Y, bottomRight.Y, bottomLeft.Y, topLeft.Y);
            maxX = Max(topRight.X, bottomRight.X, bottomLeft.X, topLeft.X); maxY = Max(topRight.Y, bottomRight.Y, bottomLeft.Y, topLeft.Y);
            return new Rectangle((int)(position.X + minX), (int)(position.Y + minY), (int)Math.Ceiling(maxX - minX), (int)Math.Ceiling(maxY - minY));
        }

        public static void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v)
        {
            float min = Min(r, g, b), max = Max(r, g, b), delta = (max - min);
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

        public static IEnumerable<string> DirSearch(string path, params string[] extensions)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles().Where(f => extensions.Contains(f.Extension)).Select(f => f.FullName).ToList();
            foreach (var d in dir.GetDirectories()) files.AddRange(DirSearch(d.FullName, extensions));
            return files;
        }
        public static IEnumerable<string> FilterFiles(string path, params string[] exts)
        {
            return
                Directory
                .EnumerateFiles(path, "*.*")
                .Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)));
        }
    }
}