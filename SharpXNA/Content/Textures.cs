using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using SharpXNA.Plugins;

namespace SharpXNA
{
    public static class Textures
    {
        public static string RootDirectory;

        internal static TextureMemoir memoir;
        static Textures() { memoir = new TextureMemoir(); }

        public static Texture2D Load(string path) { return memoir.Load(path); }
        public static bool Save(string path, Texture2D asset) { return memoir.Save(path, asset); }
        public static bool Loaded(string path) { return memoir.Loaded(path); }
        public static void UnloadAll() { memoir.UnloadAll(); }
        public static bool Unload(string path) { return memoir.Unload(path); }

        public static Color ColorFromName(string name)
        {
            var prop = typeof(Color).GetProperty(name);
            if (prop != null) return (Color)prop.GetValue(null, null);
            return default(Color);
        }
        public static Texture2D FillWithColor(this Texture2D texture, Color color, string storeName = null)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int i = 0; i < colors.Length; i++) if (colors[i].A != 0) { colors[i].R = color.R; colors[i].G = color.G; colors[i].B = color.B; }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData(colors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D BrightenTexture(this Texture2D texture, float amount, string storeName = null)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colors);
            for (int i = 0; i < colors.Length; i++)
                if (colors[i].A != 0)
                {
                    if (amount < 0) colors[i] = Color.Lerp(colors[i], Color.Black, -amount);
                    else colors[i] = Color.Lerp(colors[i], Color.White, amount);
                }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData<Color>(colors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D ReplaceColorWithoutAlpha(this Texture2D texture, Color source, Color destination, string storeName = null)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int i = 0; i < colors.Length; i++)
                if ((colors[i].R == source.R) && (colors[i].G == source.G) && (colors[i].B == source.B))
                { colors[i].R = destination.R; colors[i].G = destination.G; colors[i].B = destination.B; }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData(colors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D ReplaceColorWithAlpha(this Texture2D texture, Color source, Color destination, string storeName = null)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colors);
            for (int i = 0; i < colors.Length; i++)
                if ((colors[i].R == source.R) && (colors[i].G == source.G) && (colors[i].B == source.B) && (colors[i].A == source.A))
                { colors[i].R = destination.R; colors[i].G = destination.G; colors[i].B = destination.B; colors[i].A = destination.A; }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData<Color>(colors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D RemoveColorsWithoutAlpha(this Texture2D texture, string storeName = null, params Color[] colors)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] texColors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(texColors);
            for (int i = 0; i < texColors.Length; i++)
                for (int j = 0; j < colors.Length; j++)
                    if ((texColors[i].R == colors[j].R) && (texColors[i].G == colors[j].G) && (texColors[i].B == colors[j].B) && (texColors[i].A == colors[j].A))
                    { texColors[i].R = 0; texColors[i].G = 0; texColors[i].B = 0; texColors[i].A = 0; }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData<Color>(texColors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D RemoveColorsWithAlpha(this Texture2D texture, string storeName = null, params Color[] colors)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] texColors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(texColors);
            for (int i = 0; i < texColors.Length; i++)
                for (int j = 0; j < colors.Length; j++)
                    if ((texColors[i].R == colors[j].R) && (texColors[i].G == colors[j].G) && (texColors[i].B == colors[j].B))
                    { texColors[i].R = 0; texColors[i].G = 0; texColors[i].B = 0; texColors[i].A = 0; }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData<Color>(texColors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D RemoveColorsWithoutAlpha(this Texture2D texture, Color color, double tolerance, string storeName = null)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] texColors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(texColors);
            for (int i = 0; i < texColors.Length; i++)
            {
                var texColor = new Rgb() { R = texColors[i].R, G = texColors[i].G, B = texColors[i].B };
                var rgbColor = new Rgb() { R = color.R, G = color.G, B = color.B };
                double deltaE = texColor.Compare(rgbColor, new Cie1976Comparison());
                if (deltaE <= tolerance) { texColors[i].R = 0; texColors[i].G = 0; texColors[i].B = 0; texColors[i].A = 0; }
            }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData<Color>(texColors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D RemoveColorsWithAlpha(this Texture2D texture, Color color, byte tolerance, string storeName = null)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] texColors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(texColors);
            for (int i = 0; i < texColors.Length; i++)
            {
                var texColor = new Rgb() { R = texColors[i].R, G = texColors[i].G, B = texColors[i].B };
                var rgbColor = new Rgb() { R = color.R, G = color.G, B = color.B };
                double deltaE = texColor.Compare(rgbColor, new Cie1976Comparison());
                if (deltaE <= tolerance) { texColors[i].R = 0; texColors[i].G = 0; texColors[i].B = 0; texColors[i].A = 0; }
            }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData<Color>(texColors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Texture2D ToWireframe(this Texture2D texture, Color color, string storeName = null)
        {
            if ((storeName != null) && Loaded(storeName)) return Globe.ContentManager.Load<Texture2D>(storeName);
            Color[] colors = new Color[texture.Width * texture.Height], newColors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colors);
            for (int i = 0; i < colors.Length; i++)
            {
                int x = (i % texture.Width), y = (i / texture.Width);
                Color? above = colors.GetPixel(texture, x, (y - 1)), below = colors.GetPixel(texture, x, (y + 1)),
                    left = colors.GetPixel(texture, (x - 1), y), right = colors.GetPixel(texture, (x + 1), y);
                bool nextToTransparentPixel = (!above.HasValue || (above.Value.A == 0));
                if (!nextToTransparentPixel) nextToTransparentPixel = (!below.HasValue || (below.Value.A == 0));
                if (!nextToTransparentPixel) nextToTransparentPixel = (!left.HasValue || (left.Value.A == 0));
                if (!nextToTransparentPixel) nextToTransparentPixel = (!right.HasValue || (right.Value.A == 0));
                if (nextToTransparentPixel && (colors[i].A != 0)) { newColors[i].R = color.R; newColors[i].G = color.G; newColors[i].B = color.B; newColors[i].A = 255; }
                else { newColors[i].A = 0; }
            }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData<Color>(newColors);
            if (storeName != null) Save(storeName, filledTexture);
            return filledTexture;
        }
        public static Color GetPixel(this Color[] colors, Texture2D texture, int x, int y) { int index = (x + (y * texture.Width)); return colors[index]; }

        public static void LoadTextures(string path)
        {
            if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + path.Substring(1));
            string mainPath = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory);
            if (Directory.Exists(path))
            {
                var files = DirSearch(path, ".png", ".jpeg", ".jpg", ".dds");
                foreach (var file in files)
                {
                    var directoryName = Path.GetDirectoryName(file);
                    if (directoryName != null)
                    {
                        var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileNameWithoutExtension(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileNameWithoutExtension(file))).Replace("\\", "/");
                        Save(name, Globe.TextureLoader.FromFile(file));
                    }
                }
            }
            else System.Console.WriteLine($"Directory {path} does not exist.");
        }
        private static IEnumerable<string> DirSearch(string directory, params string[] extensions)
        {
            var dir = new DirectoryInfo(directory);
            var files = dir.GetFiles().Where(f => extensions.Contains(f.Extension)).Select(f => f.FullName).ToList();
            foreach (var d in dir.GetDirectories()) files.AddRange(DirSearch(d.FullName, extensions));
            return files;
        }

        private static List<Texture2D> disposeList;
        public static void AutoDispose(Texture2D Texture) { if (disposeList == null) disposeList = new List<Texture2D>(); disposeList.Add(Texture); }
        public static void ApplyDispose() { if (disposeList != null) { for (int i = 0; i < disposeList.Count; i++) { disposeList[i].Dispose(); disposeList.RemoveAt(i); i--; } if (disposeList.Count <= 0) disposeList = null; } }

        public static Texture2D Pixel(Color color, bool store = false)
        {
            string name = (color.R + "." + color.G + "." + color.B);
            if (store && Loaded(name)) return Textures.Load(name);
            var pixel = new Texture2D(Globe.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { color });
            if (store) Save(name, pixel);
            else AutoDispose(pixel);
            return pixel;
        }
        public static Texture2D Backup(this Texture2D texture)
        {
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            Texture2D backup = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            backup.SetData(colors);
            return backup;
        }

        public class Origin
        {
            public static Origin None { get { return Zero; } }
            public static Origin Zero { get { return new Origin(0); } }
            public static Origin Middle { get { return Center; } }
            public static Origin Center { get { return new Origin(.5f, true); } }

            public Origin(float value, bool scaled = false) { Value = new Vector2(value); Scaled = scaled; }
            public Origin(float x, float y, bool scaled = false) { Value = new Vector2(x, y); Scaled = scaled; }
            public Origin(Vector2 value, bool scaled = false) { Value = value; Scaled = scaled; }

            public bool Scaled;
            public Vector2 Value;

            public void ApplyScale(Rectangle bounds)
            {
                if (Scaled)
                {
                    Value = new Vector2((bounds.Width * Value.X), (bounds.Height * Value.Y));
                    Scaled = false;
                }
            }
            public void ApplyScale(Texture2D texture, Rectangle? source = null)
            {
                if (Scaled)
                {
                    if (source.HasValue) Value = new Vector2((source.Value.Width * Value.X), (source.Value.Height * Value.Y));
                    else Value = new Vector2((texture.Width * Value.X), (texture.Height * Value.Y));
                    Scaled = false;
                }
            }
            public Vector2 Peek(Texture2D texture) { if (Scaled) return new Vector2((texture.Width * Value.X), (texture.Height * Value.Y)); else return new Vector2(Value.X, Value.Y); }
            public Vector2 Peek(SpriteFont font, string @string) { if (string.IsNullOrEmpty(@string)) return Vector2.Zero; if (Scaled) { Vector2 size = font.MeasureString(@string); return new Vector2((size.X * Value.X), (size.Y * Value.Y)); } else return new Vector2(Value.X, Value.Y); }

            public Origin Clone() { return new Origin(Value.X, Value.Y, Scaled); }
        }
    }

    public class TextureLoader : IDisposable
    {
        static TextureLoader()
        {
            blendColorBlendState = new BlendState
            {
                ColorDestinationBlend = Blend.Zero,
                ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha
            };
            blendAlphaBlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                ColorDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One
            };
        }

        public TextureLoader(GraphicsDevice graphicsDevice) { this.graphicsDevice = graphicsDevice; spriteBatch = new SpriteBatch(graphicsDevice); }
        public Texture2D FromFile(string path, bool preMultiplyAlpha = true) { using (Stream fileStream = File.OpenRead(path)) return FromStream(fileStream, preMultiplyAlpha); }

        public Texture2D FromStream(Stream stream, bool preMultiplyAlpha = true)
        {
            var texture = Texture2D.FromStream(graphicsDevice, stream);
            if (preMultiplyAlpha)
            {
                using (var renderTarget = new RenderTarget2D(graphicsDevice, texture.Width, texture.Height))
                {
                    var viewportBackup = graphicsDevice.Viewport;
                    graphicsDevice.SetRenderTarget(renderTarget);
                    graphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.Immediate, blendColorBlendState);
                    spriteBatch.Draw(texture, texture.Bounds, Color.White);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, blendAlphaBlendState);
                    spriteBatch.Draw(texture, texture.Bounds, Color.White);
                    spriteBatch.End();
                    graphicsDevice.SetRenderTarget(null);
                    graphicsDevice.Viewport = viewportBackup;
                    var data = new Color[texture.Width * texture.Height];
                    renderTarget.GetData(data);
                    graphicsDevice.Textures[0] = null;
                    texture.SetData(data);
                }
            }
            return texture;
        }

        private static readonly BlendState blendColorBlendState;
        private static readonly BlendState blendAlphaBlendState;

        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;

        public void Dispose() { spriteBatch.Dispose(); }
    }
}