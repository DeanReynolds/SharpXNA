using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SharpXNA
{
    public class Fonts
    {
        public static string RootDirectory;
        
        private readonly Dictionary<string, SpriteFont> _assets;
        
        public Fonts() { _assets = new Dictionary<string, SpriteFont>(); }
        /// <param name="rootDirectory">This controls the global Fonts root directory (used for all Fonts classes).</param>
        public Fonts(string rootDirectory) { RootDirectory = rootDirectory; _assets = new Dictionary<string, SpriteFont>(); }

        public SpriteFont this[string path] => _assets[path];

        public void Add(SpriteFont sound, string path) { _assets.Add(path, sound); }
        public SpriteFont Load(string path) { _assets.Add(path, Globe.ContentManager.Load<SpriteFont>(RootDirectory + "\\" + path)); return _assets[path]; }
        public void Dispose(string path) { _assets[path].Texture.Dispose(); _assets.Remove(path); }

        public void Dispose() { foreach (var t in _assets.Values) t.Texture.Dispose(); _assets.Clear(); }
    }
}