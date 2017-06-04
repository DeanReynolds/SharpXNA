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
        public Fonts(string rootDirectory) { _assets = new Dictionary<string, SpriteFont>(); RootDirectory = rootDirectory; }

        public SpriteFont this[string path] => _assets[path];

        public void Add(SpriteFont font, string path) { _assets.Add(path, font); }
        public SpriteFont Load(string path) { _assets.Add(path, Engine._contentManager.Load<SpriteFont>(RootDirectory + "\\" + path)); return _assets[path]; }
        public void LoadAll(string path = null)
        {
            if (path == null) path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory + "\\" + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Engine.DirSearch(path, ".xnb");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileNameWithoutExtension(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileNameWithoutExtension(file)));
                Add(Engine._contentManager.Load<SpriteFont>($"{RootDirectory}\\{name}"), name);
            }
        }
        public void Dispose() { foreach (var t in _assets.Values) t.Texture.Dispose(); _assets.Clear(); }
        public void Dispose(string path) { _assets[path].Texture.Dispose(); _assets.Remove(path); }
    }
}