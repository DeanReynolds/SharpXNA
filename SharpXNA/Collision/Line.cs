using Microsoft.Xna.Framework;

namespace SharpXNA.Collision
{
    public class Line
    {
        public Vector2 Start, End;

        public Line(Vector2 start, Vector2 end) { Start = start; End = end; }
        public Line(Vector2 start, float endX, float endY) : this(start, new Vector2(endX, endY)) { }
        public Line(float startX, float startY, Vector2 end) : this(new Vector2(startX, startY), end) { }
        public Line(float startX, float startY, float endX, float endY) : this(new Vector2(startX, startY), new Vector2(endX, endY)) { }

        public void Draw() { Screen.DrawLine(Start, End); }
        public void Draw(float thickness) { Screen.DrawLine(Start, End, thickness); }
        public void Draw(float thickness, float layer) { Screen.DrawLine(Start, End, thickness, layer); }
        public void Draw(Color color) { Screen.DrawLine(Start, End, color); }
        public void Draw(Color color, float thickness) { Screen.DrawLine(Start, End, color, thickness); }
        public void Draw(Color color, float thickness, float layer) { Screen.DrawLine(Start, End, color, thickness, layer); }

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

        public override int GetHashCode() { return (Start.X.GetHashCode() * 17 + Start.Y.GetHashCode() * 17 + End.X.GetHashCode() * 17 + End.Y.GetHashCode() * 17); }

        public void Add(Vector2 position) { Start += position; End += position; }
        public void Subtract(Vector2 position) { Start -= position; End -= position; }
        public void Add(float x, float y) { AddX(x); AddY(y); }
        public void Subtract(float x, float y) { SubtractX(x); SubtractY(y); }
        public void AddX(float value) { Start.X += value; End.X += value; }
        public void SubtractX(float value) { Start.X -= value; End.X -= value; }
        public void AddY(float value) { Start.Y += value; End.Y += value; }
        public void SubtractY(float value) { Start.Y -= value; End.Y -= value; }
    }
}