using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SharpXNA
{
    public class Textures
    {
        public static string RootDirectory;

        private readonly Dictionary<string, Texture2D> _assets;

        public Textures() { _assets = new Dictionary<string, Texture2D>(); }
        /// <param name="rootDirectory">The global texture root directory (used for all Textures classes).</param>
        public Textures(string rootDirectory) { RootDirectory = rootDirectory; _assets = new Dictionary<string, Texture2D>(); }

        public Texture2D this[string path] => _assets[path];
        
        public Texture2D Fill(Texture2D texture, Color color, string storeName = null)
        {
            if ((storeName != null) && _assets.ContainsKey(storeName)) return _assets[storeName];
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int i = 0; i < colors.Length; i++) if (colors[i].A != 0) { colors[i].R = color.R; colors[i].G = color.G; colors[i].B = color.B; }
            Texture2D filledTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            filledTexture.SetData(colors);
            if (storeName != null) _assets.Add(storeName, filledTexture);
            return filledTexture;
        }
        public Texture2D ToWireframe(Texture2D texture, Color color, string storeName = null)
        {
            if ((storeName != null) && _assets.ContainsKey(storeName)) return _assets[storeName];
            Color[] colors = new Color[texture.Width * texture.Height], newColors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
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
            filledTexture.SetData(newColors);
            if (storeName != null) _assets.Add(storeName, filledTexture);
            return filledTexture;
        }

        public void Add(Texture2D texture, string path) { _assets.Add(path, texture); }
        public Texture2D Load(string path)
        {
            using (var fs = new FileStream((@".\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory + "\\" + path), FileMode.Open, FileAccess.Read, FileShare.Read))
                _assets.Add(path, Texture2D.FromStream(Globe.GraphicsDevice, fs));
            return _assets[path];
        }
        public void LoadAll(string path = null)
        {
            if (path == null) path = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory + "\\" + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Globe.ContentManager.RootDirectory) ? (Globe.ContentManager.RootDirectory + "\\") : null) + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Globe.DirSearch(path, ".png", ".jpeg", ".jpg", ".dds");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file)));
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) Add(Texture2D.FromStream(Globe.GraphicsDevice, fs), name);
            }
        }
        public void Dispose(string path) { _assets[path].Dispose(); _assets.Remove(path); }
        
        public void Dispose() { foreach (var t in _assets.Values) t.Dispose(); _assets.Clear(); }
    }

    public static class TextureExtensions
    {
        public static Color? GetPixel(this Color[] colors, Texture2D texture, int x, int y)
        {
            if ((x < 0) || (y < 0) || (x >= texture.Width) || (y >= texture.Height)) return null;
            else { int index = (x + (y * texture.Width)); return colors[index]; }
        }
    }
}