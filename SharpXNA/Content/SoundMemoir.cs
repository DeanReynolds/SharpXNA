using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.IO;

namespace SharpXNA.Content
{
    public class SoundMemoir
    {
        internal Dictionary<string, SoundEffect> assets;
        public SoundMemoir() { assets = new Dictionary<string, SoundEffect>(); }
        public SoundMemoir(int capacity) { assets = new Dictionary<string, SoundEffect>(capacity); }

        public SoundEffect Load(string path) { if (!assets.ContainsKey(path)) assets.Add(path, Globe.ContentManager.Load<SoundEffect>(Sound.RootDirectory + "\\" + path)); return assets[path]; }
        public SoundEffect LoadRaw(string path)
        {
            if (!assets.ContainsKey(path))
            {
                using (FileStream fs = new FileStream((@".\" + Globe.ContentManager.RootDirectory + "\\" + Sound.RootDirectory + "\\" + path), FileMode.Open))
                    assets.Add(path, SoundEffect.FromStream(fs));
            }
            return assets[path];
        }
        public bool Save(string path, SoundEffect asset) { if (!assets.ContainsKey(path)) { assets.Add(path, asset); return true; } else return false; }
        public bool Loaded(string path) { return assets.ContainsKey(path); }
        public void UnloadAll() { foreach (SoundEffect a in assets.Values) a.Dispose(); assets.Clear(); }
        public bool Unload(string path) { if (assets.ContainsKey(path)) { assets[path].Dispose(); assets.Remove(path); return true; } else return false; }
    }
}