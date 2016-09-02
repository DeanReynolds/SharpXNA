using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA.Collision
{
    public class Line
    {
        public Vector2 Start, End;

        public float StartX { get { return Start.X; } set { Start.X = value; } }
        public float StartY { get { return Start.Y; } set { Start.Y = value; } }
        public float EndX { get { return End.X; } set { End.X = value; } }
        public float EndY { get { return End.Y; } set { End.Y = value; } }

        public Line(Vector2 start, Vector2 end) { Start = start; End = end; }
        public Line(Vector2 start, float endX, float endY) : this(start, new Vector2(endX, endY)) { }
        public Line(float startX, float startY, Vector2 end) : this(new Vector2(startX, startY), end) { }
        public Line(float startX, float startY, float endX, float endY) : this(new Vector2(startX, startY), new Vector2(endX, endY)) { }

        public void Draw() { Screen.DrawLine(Start, End, Color.White, 1, SpriteEffects.None, 0); }
        public void Draw(SpriteEffects effect) { Screen.DrawLine(Start, End, Color.White, 1, effect, 0); }
        public void Draw(SpriteEffects effect, float layer) { Screen.DrawLine(Start, End, Color.White, 1, effect, layer); }
        public void Draw(float thickness) { Screen.DrawLine(Start, End, Color.White, thickness, SpriteEffects.None, 0); }
        public void Draw(float thickness, float layer) { Screen.DrawLine(Start, End, Color.White, thickness, SpriteEffects.None, layer); }
        public void Draw(float thickness, SpriteEffects effect) { Screen.DrawLine(Start, End, Color.White, thickness, effect, 0); }
        public void Draw(float thickness, SpriteEffects effect, float layer) { Screen.DrawLine(Start, End, Color.White, thickness, effect, layer); }
        public void Draw(Color color) { Screen.DrawLine(Start, End, color, 1, SpriteEffects.None, 0); }
        public void Draw(Color color, SpriteEffects effect) { Screen.DrawLine(Start, End, color, 1, effect, 0); }
        public void Draw(Color color, SpriteEffects effect, float layer) { Screen.DrawLine(Start, End, color, 1, effect, layer); }
        public void Draw(Color color, float thickness) { Screen.DrawLine(Start, End, color, thickness, SpriteEffects.None, 0); }
        public void Draw(Color color, float thickness, float layer) { Screen.DrawLine(Start, End, color, thickness, SpriteEffects.None, layer); }
        public void Draw(Color color, float thickness, SpriteEffects effect) { Screen.DrawLine(Start, End, color, thickness, effect, 0); }
        public void Draw(Color color, float thickness, SpriteEffects effect, float layer) { Screen.DrawLine(Start, End, color, thickness, effect, layer); }
        public void Draw(Batch batch) { batch.DrawLine(Start, End, Color.White, 1, SpriteEffects.None, 0); }
        public void Draw(Batch batch, SpriteEffects effect) { batch.DrawLine(Start, End, Color.White, 1, effect, 0); }
        public void Draw(Batch batch, SpriteEffects effect, float layer) { batch.DrawLine(Start, End, Color.White, 1, effect, layer); }
        public void Draw(Batch batch, float thickness) { batch.DrawLine(Start, End, Color.White, thickness, SpriteEffects.None, 0); }
        public void Draw(Batch batch, float thickness, float layer) { batch.DrawLine(Start, End, Color.White, thickness, SpriteEffects.None, layer); }
        public void Draw(Batch batch, float thickness, SpriteEffects effect) { batch.DrawLine(Start, End, Color.White, thickness, effect, 0); }
        public void Draw(Batch batch, float thickness, SpriteEffects effect, float layer) { batch.DrawLine(Start, End, Color.White, thickness, effect, layer); }
        public void Draw(Batch batch, Color color) { batch.DrawLine(Start, End, color, 1, SpriteEffects.None, 0); }
        public void Draw(Batch batch, Color color, SpriteEffects effect) { batch.DrawLine(Start, End, color, 1, effect, 0); }
        public void Draw(Batch batch, Color color, SpriteEffects effect, float layer) { batch.DrawLine(Start, End, color, 1, effect, layer); }
        public void Draw(Batch batch, Color color, float thickness) { batch.DrawLine(Start, End, color, thickness, SpriteEffects.None, 0); }
        public void Draw(Batch batch, Color color, float thickness, float layer) { batch.DrawLine(Start, End, color, thickness, SpriteEffects.None, layer); }
        public void Draw(Batch batch, Color color, float thickness, SpriteEffects effect) { batch.DrawLine(Start, End, color, thickness, effect, 0); }
        public void Draw(Batch batch, Color color, float thickness, SpriteEffects effect, float layer) { batch.DrawLine(Start, End, color, thickness, effect, layer); }

        public bool Intersects(Line line)
        {
            Vector2 a = (End - Start), b = (line.End - line.Start);
            var cP = a.X * b.Y - a.Y * b.X;
            if (cP == 0) return false;
            var c = (line.Start - Start);
            var t = (c.X * b.Y - c.Y * b.X) / cP;
            if (t < 0 || t > 1) return false;
            var u = (c.X * a.Y - c.Y * a.X) / cP;
            if (u < 0 || u > 1) return false;
            return true;
        }
        public bool Intersects(Line line, ref Vector2 intersection)
        {
            Vector2 a = (End - Start), b = (line.End - line.Start);
            var cP = a.X * b.Y - a.Y * b.X;
            if (cP == 0) return false;
            var c = (line.Start - Start);
            var t = (c.X * b.Y - c.Y * b.X) / cP;
            if (t < 0 || t > 1) return false;
            var u = (c.X * a.Y - c.Y * a.X) / cP;
            if (u < 0 || u > 1) return false;
            intersection = (Start + t * a);
            return true;
        }
        public bool Intersects(Polygon polygon) { return polygon.Intersects(this); }
        public bool Intersects(Polygon polygon, ref Vector2 intersection) { return polygon.Intersects(this, ref intersection); }

        public void Add(Vector2 position) { Start += position; End += position; }
        public void Subtract(Vector2 position) { Start -= position; End -= position; }
    }
}