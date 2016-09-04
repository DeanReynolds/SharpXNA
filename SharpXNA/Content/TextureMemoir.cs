using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SharpXNA.Plugins
{
    public class TextureMemoir
    {
        internal Dictionary<string, Texture2D> assets;
        public TextureMemoir() { assets = new Dictionary<string, Texture2D>(); }
        public TextureMemoir(int capacity) { assets = new Dictionary<string, Texture2D>(capacity); }

        public Texture2D Load(string path) { if (!assets.ContainsKey(path)) assets.Add(path, Globe.TextureLoader.FromFile(@".\" + Globe.ContentManager.RootDirectory + "\\" + Textures.RootDirectory + "\\" + path)); return assets[path]; }
        public bool Save(string path, Texture2D asset) { if (!assets.ContainsKey(path)) { assets.Add(path, asset); return true; } else return false; }
        public bool Loaded(string path) { return assets.ContainsKey(path); }
        public void UnloadAll() { foreach (Texture2D a in assets.Values) a.Dispose(); assets.Clear(); }
        public bool Unload(string path) { if (assets.ContainsKey(path)) { assets[path].Dispose(); assets.Remove(path); return true; } else return false; }
    }
}