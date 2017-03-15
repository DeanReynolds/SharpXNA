using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA
{
    public class Effects
    {
        public static string RootDirectory;

        private readonly Dictionary<string, Effect> _assets;

        public Effects() { _assets = new Dictionary<string, Effect>(); }
        public Effects(string rootDirectory) { _assets = new Dictionary<string, Effect>(); RootDirectory = rootDirectory; }

        public Effect this[string path] => _assets[path];
        
        public void Add(Effect effect, string path) { _assets.Add(path, effect); }
        public Effect Load(string path) { _assets.Add(path, Engine._contentManager.Load<Effect>(RootDirectory + "\\" + path)); return _assets[path]; }
        public void LoadAll(string path = null)
        {
            if (path == null) path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory + "\\" + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Engine.DirSearch(path, ".xnb");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileNameWithoutExtension(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileNameWithoutExtension(file)));
                Add(Engine._contentManager.Load<Effect>(RootDirectory + "\\" + name), name);
            }
        }
        public void Dispose() { foreach (var t in _assets.Values) t.Dispose(); _assets.Clear(); }
        public void Dispose(string path) { _assets[path].Dispose(); _assets.Remove(path); }
    }
}