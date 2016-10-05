using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA
{
    public class Textures
    {
        public static string RootDirectory;

        private readonly Dictionary<string, Texture2D> _assets;

        public Textures() { _assets = new Dictionary<string, Texture2D>(); }
        /// <param name="rootDirectory">This controls the global Textures root directory (used for all Textures classes).</param>
        public Textures(string rootDirectory) { RootDirectory = rootDirectory; _assets = new Dictionary<string, Texture2D>(); }

        public Texture2D this[string path] => _assets[path];

        public void Add(Texture2D texture, string path) { _assets.Add(path, texture); }
        public Texture2D Load(string path)
        {
            using (var fs = new FileStream((@".\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory + "\\" + path), FileMode.Open))
                _assets.Add(path, Texture2D.FromStream(Globe.GraphicsDevice, fs));
            return _assets[path];
        }
        public void LoadAll(string path = null)
        {
            if (path == null) path = Path.GetDirectoryName(Globe.Assembly.Location);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Globe.DirSearch(path, ".png", ".jpeg", ".jpg", ".dds");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file)));
                using (var fs = new FileStream(file, FileMode.Open)) Add(Texture2D.FromStream(Globe.GraphicsDevice, fs), name);
            }
        }
        public void Dispose(string path) { _assets[path].Dispose(); _assets.Remove(path); }
        
        public void Dispose() { foreach (var t in _assets.Values) t.Dispose(); _assets.Clear(); }
    }
}