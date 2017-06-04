using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SharpXNA.Plugins
{
    public static class TexturePacker
    {
        public static Sheet LoadFile(Texture2D texture, string dataFilePath) { return Load(texture, File.ReadAllText(Engine._contentManager.RootDirectory + "\\" + dataFilePath)); }
        public static Sheet Load(Texture2D texture, string data)
        {
            var sheet = new Sheet(texture);
            using (var reader = new StringReader(data))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#", true, CultureInfo.InvariantCulture) || string.IsNullOrEmpty(line.Trim()))
                        continue;
                    var s = line.Split(';');
                    var name = s[0];
                    var source = new Rectangle(int.Parse(s[2], CultureInfo.InvariantCulture), int.Parse(s[3], CultureInfo.InvariantCulture), int.Parse(s[4], CultureInfo.InvariantCulture), int.Parse(s[5], CultureInfo.InvariantCulture));
                    var origin = new Origin(float.Parse(s[8], CultureInfo.InvariantCulture), float.Parse(s[9], CultureInfo.InvariantCulture));
                    var isRotated = (int.Parse(s[1], CultureInfo.InvariantCulture) == 1);
                    sheet.Add(name, new Sprite(sheet, source, origin, isRotated));
                }
            }
            return sheet;
        }
    }

    public class Sheet
    {
        public Texture2D Texture { get; protected set; }
        public Dictionary<string, Sprite> Sprites { get; protected set; }

        public Sheet(Texture2D texture)
        {
            Texture = texture;
            Sprites = new Dictionary<string, Sprite>();
        }

        public Sprite this[string name] { get { return Sprites[name]; } }

        internal void Add(string name, Sprite sprite) { Sprites.Add(name, sprite); }
    }

    public class Sprite
    {
        public Sheet Sheet { get; private set; }

        public Sprite(Sheet sheet, Rectangle source, Origin origin, bool isRotated)
        {
            Sheet = sheet;
            Source = source;
            Origin = isRotated ? new Origin((source.Width * (1 - origin.Y)), (source.Height * origin.X)) : new Origin((source.Width * origin.X), (source.Height * origin.Y));
            IsRotated = isRotated;
        }

        public Rectangle Source { get; private set; }
        public Origin Origin { get; private set; }
        public bool IsRotated { get; private set; }
    }
}