using Microsoft.Xna.Framework;

namespace SharpXNA.Collision
{
    public class Line
    {
        public Vector2 Start, End;
        public bool Culled;

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
            if (line == null)
                return false;
            Vector2 a = (End - Start), b = (line.End - line.Start);
            var cP = a.X * b.Y - a.Y * b.X;
            if (cP == 0)
                return false;
            var c = (line.Start - Start);
            var t = (c.X * b.Y - c.Y * b.X) / cP;
            if (t < 0 || t > 1)
                return false;
            var u = (c.X * a.Y - c.Y * a.X) / cP;
            if (u < 0 || u > 1)
                return false;
            return true;
        }
        public bool Intersects(Line line, ref Vector2 intersection)
        {
            if (line == null)
                return false;
            Vector2 a = (End - Start), b = (line.End - line.Start);
            var cP = a.X * b.Y - a.Y * b.X;
            if (cP == 0)
                return false;
            var c = (line.Start - Start);
            var t = (c.X * b.Y - c.Y * b.X) / cP;
            if (t < 0 || t > 1)
                return false;
            var u = (c.X * a.Y - c.Y * a.X) / cP;
            if (u < 0 || u > 1)
                return false;
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

        /// <summary>
        /// Retrieve the perpendicular angle to this line.
        /// </summary>
        /// <param name="angle">The angle you want the perpendicular angle to face.</param>
        /// <returns>The perpendicular angle relative to the angle given.</returns>
        public float PerpendicularAngle(float angle)
        {
            var d = (End - Start);
            var a = Mathf.Angle(new Vector2(-d.Y, d.X), new Vector2(d.Y, -d.X));
            return ((Mathf.AngleDifference(a, angle) <= Mathf.PiOver2) ? a : (a + Mathf.Pi));
        }
        public float ReflectionAngle(float perpendicularAngle)
        {
            var originalAngle = Mathf.Angle(Start, End);
            float reflectionAngle = perpendicularAngle,
                angleDif = Mathf.AngleDifference(originalAngle, MathHelper.WrapAngle(perpendicularAngle + Mathf.Pi));
            float b;
            if (Mathf.AngleDifference((b = (perpendicularAngle + angleDif)), MathHelper.WrapAngle(originalAngle + Mathf.Pi)) <= Mathf.PiOver1024)
                reflectionAngle -= angleDif;
            else
                reflectionAngle = b;
            return reflectionAngle;
        }
        public float ReflectionAngle(Line line) { return ReflectionAngle(line.PerpendicularAngle(Mathf.Angle(End, Start))); }
        public float ReflectionAngle(Line line, Vector2 position, float angle) { return ReflectionAngle(line.PerpendicularAngle(Mathf.Angle(position, Mathf.Move(position, angle, -1)))); }
        public float ReflectionAngle(Polygon polygon) { return polygon.ReflectionAngle(this); }
        public float ReflectionAngle(Polygon polygon, Vector2 position, float angle) { return polygon.ReflectionAngle(this, position, angle); }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return ReferenceEquals(this, null);
            var other = (Line)obj;
            return !((Start != other.Start) || (End != other.End));
        }
        public static bool operator !=(Line line, Line other) { return !(line == other); }
        public static bool operator ==(Line line, Line other) { return (ReferenceEquals(line, other) || (!ReferenceEquals(line, null) && line.Equals(other))); }
    }
}