using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace SharpXNA
{
    public class Fonts
    {
        public static string RootDirectory;
        
        private readonly Dictionary<string, SpriteFont> _assets;
        
        public Fonts() { _assets = new Dictionary<string, SpriteFont>(); }
        /// <param name="rootDirectory">The global font root directory (used for all Fonts classes).</param>
        public Fonts(string rootDirectory) { RootDirectory = rootDirectory; _assets = new Dictionary<string, SpriteFont>(); }

        public SpriteFont this[string path] => _assets[path];

        public void Add(SpriteFont font, string path) { _assets.Add(path, font); }
        public SpriteFont Load(string path) { _assets.Add(path, Globe.ContentManager.Load<SpriteFont>(RootDirectory + "\\" + path)); return _assets[path]; }
        public void LoadAll(string path = null)
        {
            if (path == null) path = Path.GetDirectoryName(Globe.Assembly.Location);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Globe.DirSearch(path, ".xnb");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = Path.GetFileNameWithoutExtension(((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file))));
                Add(Globe.ContentManager.Load<SpriteFont>($"{RootDirectory}\\{name}"), name);
            }
        }
        public void Dispose(string path) { _assets[path].Texture.Dispose(); _assets.Remove(path); }

        public void Dispose() { foreach (var t in _assets.Values) t.Texture.Dispose(); _assets.Clear(); }
    }
}