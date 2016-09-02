using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA
{
    public static class Fonts
    {
        public static string RootDirectory;

        public static SpriteFont Load(string path) { if (!Globe.ContentManager.Loaded<SpriteFont>(path)) Globe.ContentManager.Save<SpriteFont>(path, Globe.ContentManager.Load<SpriteFont>(
            (!string.IsNullOrEmpty(RootDirectory) ? (RootDirectory + (!RootDirectory.EndsWith("\\") ? "\\" : string.Empty)) : string.Empty) + path)); return Globe.ContentManager.Load<SpriteFont>(path); }
        public static bool Loaded(string path) { return Globe.ContentManager.Loaded<SpriteFont>(path); }
        public static bool Unload(string path) { return Globe.ContentManager.Unload<SpriteFont>(path); }
    }
}