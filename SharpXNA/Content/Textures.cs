using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace SharpXNA
{
    public class Textures
    {
        public static string RootDirectory;

        private readonly Dictionary<string, Texture2D> _assets;

        public Textures() { _assets = new Dictionary<string, Texture2D>(); }
        public Textures(string rootDirectory)
        {
            _assets = new Dictionary<string, Texture2D>();
            RootDirectory = rootDirectory;
        }

        public Texture2D this[string path] => _assets[path];
        
        public Texture2D Fill(Texture2D texture, Color color, string storeName = null)
        {
            if ((storeName != null) && _assets.ContainsKey(storeName))
                return _assets[storeName];
            var colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int i = 0; i < colors.Length; i++)
                if (colors[i].A != 0)
                {
                    colors[i].R = color.R;
                    colors[i].G = color.G;
                    colors[i].B = color.B;
                }
            var filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData(colors);
            if (storeName != null)
                _assets.Add(storeName, filledTexture);
            return filledTexture;
        }
        public Texture2D ToWireframe(Texture2D texture, Color color, string storeName = null)
        {
            if ((storeName != null) && _assets.ContainsKey(storeName))
                return _assets[storeName];
            Color[] colors = new Color[texture.Width * texture.Height],
                newColors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int i = 0; i < colors.Length; i++)
            {
                int x = (i % texture.Width), y = (i / texture.Width);
                Color? above = colors.GetPixel(texture, x, (y - 1)), below = colors.GetPixel(texture, x, (y + 1)),
                    left = colors.GetPixel(texture, (x - 1), y), right = colors.GetPixel(texture, (x + 1), y);
                bool nextToTransparentPixel = (!above.HasValue || (above.Value.A == 0));
                if (!nextToTransparentPixel)
                    nextToTransparentPixel = (!below.HasValue || (below.Value.A == 0));
                if (!nextToTransparentPixel)
                    nextToTransparentPixel = (!left.HasValue || (left.Value.A == 0));
                if (!nextToTransparentPixel)
                    nextToTransparentPixel = (!right.HasValue || (right.Value.A == 0));
                if (nextToTransparentPixel && (colors[i].A != 0))
                {
                    newColors[i].R = color.R;
                    newColors[i].G = color.G;
                    newColors[i].B = color.B;
                    newColors[i].A = 255;
                }
                else newColors[i].A = 0;
            }
            var filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData(newColors);
            if (storeName != null)
                _assets.Add(storeName, filledTexture);
            return filledTexture;
        }

        public void Add(Texture2D texture, string path) { _assets.Add(path, texture); }
        public Texture2D Load(string path)
        {
            if (Path.GetExtension(path) == ".xnb")
                try
                {
                    var contentFile = (RootDirectory + "\\" + path);
                    contentFile = contentFile.Substring(0, (contentFile.Length - 4));
                    var asset = Engine.Content.Load<Texture2D>(contentFile);
                    _assets.Add(path, asset);
                }
                catch { }
            else
                using (var fs = new FileStream((@".\" + Engine._contentManager.RootDirectory + "\\" + RootDirectory + "\\" + path), FileMode.Open, FileAccess.Read, FileShare.Read))
                    _assets.Add(path, Texture2D.FromStream(Engine.GraphicsDevice, fs));
            return _assets[path];
        }
        public void LoadAll(string path = null)
        {
            if (path == null)
                path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            else if (path.StartsWith("."))
                path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory + "\\" + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            if (!Directory.Exists(path))
                return;
            var files = Engine.DirSearch(path, ".png", ".jpeg", ".jpg", ".dds");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null)
                    continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file)));
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    _assets.Add(name, Texture2D.FromStream(Engine.GraphicsDevice, fs));
            }
            files = Engine.DirSearch(path, ".xnb");
            foreach (var file in files)
            {
                try
                {
                    var directoryName = Path.GetDirectoryName(file);
                    if (directoryName == null)
                        continue;
                    var contentFile = (RootDirectory + "\\" + ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file))));
                    contentFile = contentFile.Substring(0, (contentFile.Length - 4));
                    var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file)));
                    var texture = Engine.Content.Load<Texture2D>(contentFile);
                    _assets.Add(name, texture);
                }
                catch { }
            }
        }
        public void Dispose()
        {
            foreach (var t in _assets.Values)
                t.Dispose();
            _assets.Clear();
        }
        public void Dispose(string path)
        {
            _assets[path].Dispose();
            _assets.Remove(path);
        }
    }

    public static class TextureExtensions
    {
        public static Color? GetPixel(this Color[] colors, Texture2D texture, int x, int y)
        {
            if ((x < 0) || (y < 0) || (x >= texture.Width) || (y >= texture.Height))
                return null;
            return colors[x + (y * texture.Width)];
        }

        public static bool PerPixelCollision(this Texture2D textureA, Rectangle boundsA, Texture2D textureB, Rectangle boundsB)
        {
            var bitsA = new Color[textureA.Width * textureA.Height];
            textureA.GetData(bitsA);
            var bitsB = new Color[textureB.Width * textureB.Height];
            textureB.GetData(bitsB);
            return bitsA.PerPixelCollision(boundsA, bitsB, boundsB);
        }
        public static bool PerPixelCollision(this Color[] bitsA, Rectangle boundsA, Texture2D textureB, Rectangle boundsB)
        {
            var bitsB = new Color[textureB.Width * textureB.Height];
            textureB.GetData(bitsB);
            return bitsA.PerPixelCollision(boundsA, bitsB, boundsB);
        }
        public static bool PerPixelCollision(this Texture2D textureA, Rectangle boundsA, Color[] bitsB, Rectangle boundsB)
        {
            var bitsA = new Color[textureA.Width * textureA.Height];
            textureA.GetData(bitsA);
            return bitsA.PerPixelCollision(boundsA, bitsB, boundsB);
        }
        public static bool PerPixelCollision(this Color[] bitsA, Rectangle boundsA, Color[] bitsB, Rectangle boundsB)
        {
            int x1 = Math.Max(boundsA.X, boundsB.X),
                x2 = Math.Min(boundsA.X + boundsA.Width, boundsB.X + boundsB.Width),
                y1 = Math.Max(boundsA.Y, boundsB.Y),
                y2 = Math.Min(boundsA.Y + boundsA.Height, boundsB.Y + boundsB.Height);
            for (int y = y1; y < y2; ++y)
                for (int x = x1; x < x2; ++x)
                {
                    Color a = bitsA[(x - boundsA.X) + (y - boundsA.Y) * boundsA.Width],
                        b = bitsB[(x - boundsB.X) + (y - boundsB.Y) * boundsB.Width];
                    if ((a.A != 0) && (b.A != 0))
                        return true;
                }
            return false;
        }
    }
}