using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpXNA.Content
{
    public class TextureMemoir
    {
        internal Dictionary<string, Texture2D> assets;
        public TextureMemoir() { assets = new Dictionary<string, Texture2D>(); }
        public TextureMemoir(int capacity) { assets = new Dictionary<string, Texture2D>(capacity); }

        //public Texture2D Load(string path) { if (!assets.ContainsKey(path)) assets.Add(path, Globe.TextureLoader.FromFile(@".\" + Globe.ContentManager.RootDirectory + "\\" + Textures.RootDirectory + "\\" + path)); return assets[path]; }
        public Texture2D Load(string path) { if (!assets.ContainsKey(path)) using (FileStream fs = new FileStream((@".\" + Globe.ContentManager.RootDirectory + "\\" + Textures.RootDirectory + "\\" + path), FileMode.Open)) assets.Add(path, Texture2D.FromStream(Globe.GraphicsDevice, fs)); return assets[path]; }
        public void LoadAll(string path)
        {
            if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + path.Substring(1));
            string mainPath = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + Textures.RootDirectory);
            if (Directory.Exists(path))
            {
                var files = DirSearch(path, ".png", ".jpeg", ".jpg", ".dds");
                foreach (var file in files)
                {
                    var directoryName = Path.GetDirectoryName(file);
                    if (directoryName != null)
                    {
                        var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileNameWithoutExtension(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileNameWithoutExtension(file)));
                        //Save(name, Globe.TextureLoader.FromFile(file));
                        using (FileStream fs = new FileStream(file, FileMode.Open)) Save(name, Texture2D.FromStream(Globe.GraphicsDevice, fs));
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
        public bool Save(string path, Texture2D asset) { if (!assets.ContainsKey(path)) { assets.Add(path, asset); return true; } else return false; }
        public bool Loaded(string path) { return assets.ContainsKey(path); }
        public void UnloadAll() { foreach (Texture2D a in assets.Values) a.Dispose(); assets.Clear(); }
        public bool Unload(string path) { if (assets.ContainsKey(path)) { assets[path].Dispose(); assets.Remove(path); return true; } else return false; }
    }
}