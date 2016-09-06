using Microsoft.Xna.Framework.Graphics;
using SharpXNA.Content;

namespace SharpXNA
{
    public static class Font
    {
        public static string RootDirectory;

        internal static FontMemoir memoir;
        static Font() { memoir = new FontMemoir(); }

        public static SpriteFont Load(string path) { return memoir.Load(path); }
        public static bool Save(string path, SpriteFont asset) { return memoir.Save(path, asset); }
        public static bool Loaded(string path) { return memoir.Loaded(path); }
        public static void UnloadAll() { memoir.UnloadAll(); }
        public static bool Unload(string path) { return memoir.Unload(path); }
    }
}