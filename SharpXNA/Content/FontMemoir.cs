using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SharpXNA.Plugins
{
    public class FontMemoir
    {
        internal Dictionary<string, SpriteFont> assets;
        public FontMemoir() { assets = new Dictionary<string, SpriteFont>(); }
        public FontMemoir(int capacity) { assets = new Dictionary<string, SpriteFont>(capacity); }

        public SpriteFont Load(string path) { if (!assets.ContainsKey(path)) assets.Add(path, Globe.ContentManager.Load<SpriteFont>(Font.RootDirectory + "\\" + path)); return assets[path]; }
        public bool Save(string path, SpriteFont asset) { if (!assets.ContainsKey(path)) { assets.Add(path, asset); return true; } else return false; }
        public bool Loaded(string path) { return assets.ContainsKey(path); }
        public void UnloadAll() { foreach (SpriteFont a in assets.Values) a.Texture.Dispose(); assets.Clear(); }
        public bool Unload(string path) { if (assets.ContainsKey(path)) { assets[path].Texture.Dispose(); assets.Remove(path); return true; } else return false; }
    }
}