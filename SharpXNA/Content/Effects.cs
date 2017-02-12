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
        /// <param name="rootDirectory">The global texture root directory (used for all Textures classes).</param>
        public Effects(string rootDirectory) { RootDirectory = rootDirectory; _assets = new Dictionary<string, Effect>(); }

        public Effect this[string path] => _assets[path];
        
        public void Add(Effect effect, string path) { _assets.Add(path, effect); }
        public Effect Load(string path)
        {
            //using (var fs = new FileStream((@".\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory + "\\" + path), FileMode.Open, FileAccess.Read, FileShare.Read))
            //    _assets.Add(path, Effect.FromStream(Globe.GraphicsDevice, fs));
            _assets.Add(path, Globe.ContentManager.Load<Effect>(RootDirectory + "\\" + path));
            return _assets[path];
        }
        public void LoadAll(string path = null)
        {
            if (path == null) path = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory + "\\" + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Globe.ContentManager.RootDirectory) ? (Globe.ContentManager.RootDirectory + "\\") : null) + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Globe.DirSearch(path, ".xnb");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileNameWithoutExtension(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileNameWithoutExtension(file)));
                //using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) Add(Effect.FromStream(Globe.GraphicsDevice, fs), name);
                Add(Globe.ContentManager.Load<Effect>(RootDirectory + "\\" + name), name);
            }
        }
        public void Dispose(string path) { _assets[path].Dispose(); _assets.Remove(path); }
        
        public void Dispose() { foreach (var t in _assets.Values) t.Dispose(); _assets.Clear(); }
    }
}