using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA
{
    public class Origin
    {
        public static Origin None => Zero;
        public static Origin Zero => new Origin(0);
        public static Origin Middle => Center;
        public static Origin Center => new Origin(.5f, true);

        public Origin(float value, bool scaled = false) { Value = new Vector2(value); Scaled = scaled; }
        public Origin(float x, float y, bool scaled = false) { Value = new Vector2(x, y); Scaled = scaled; }
        public Origin(Vector2 value, bool scaled = false) { Value = value; Scaled = scaled; }

        public bool Scaled;
        public Vector2 Value;

        public void Apply(Rectangle bounds)
        {
            if (!Scaled)
                return;
            Value = new Vector2((bounds.Width * Value.X), (bounds.Height * Value.Y));
            Scaled = false;
        }
        public void Apply(Texture2D texture, Rectangle? source = null)
        {
            if (!Scaled)
                return;
            Value = source.HasValue ? new Vector2((source.Value.Width * Value.X), (source.Value.Height * Value.Y)) : new Vector2((texture.Width * Value.X), (texture.Height * Value.Y));
            Scaled = false;
        }

        public float X
        {
            get { return Value.X; }
            set { Value.X = value; }
        }
        public float Y
        {
            get { return Value.Y; }
            set { Value.Y = value; }
        }

        public Origin Clone() { return new Origin(Value.X, Value.Y, Scaled); }
    }
}