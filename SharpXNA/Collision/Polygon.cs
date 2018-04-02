using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SharpXNA.Collision
{
    public class Polygon
    {
        public const float PxOffset = .000001f;

        List<Line> _actualLines;
        public Line[] Lines;
        internal Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                foreach (var t in Lines)
                {
                    t.Subtract(position);
                    t.Add(value);
                }
                position = value;
            }
        }
        public float X
        {
            get { return position.X; }
            set
            {
                foreach (var t in Lines)
                {
                    t.SubtractX(position.X);
                    t.AddX(value);
                }
                position.X = value;
            }
        }
        public float Y
        {
            get { return position.Y; }
            set
            {
                foreach (var t in Lines)
                {
                    t.SubtractY(position.Y);
                    t.AddY(value);
                }
                position.Y = value;
            }
        }

        internal float angle;
        public float Angle
        {
            get { return angle; }
            set
            {
                foreach (var t in Lines)
                {
                    t.Start = Mathf.Rotate(t.Start, -angle, position);
                    t.End = Mathf.Rotate(t.End, -angle, position);
                    t.Start = Mathf.Rotate(t.Start, value, position);
                    t.End = Mathf.Rotate(t.End, value, position);
                }
                angle = value;
            }
        }

        public Polygon(params Line[] lines) { Lines = lines; FixedWidth = Width; FixedHeight = Height; }
        public Polygon(List<Line> lines) { Lines = lines.ToArray(); FixedWidth = Width; FixedHeight = Height; }

        public void Draw()
        {
            foreach (var line in Lines)
                line?.Draw();
        }
        public void Draw(float thickness)
        {
            foreach (var line in Lines)
                line?.Draw(thickness);
        }
        public void Draw(float thickness, float layer)
        {
            foreach (var line in Lines)
                line?.Draw(thickness, layer);
        }
        public void Draw(Color color)
        {
            foreach (var line in Lines)
                line?.Draw(color);
        }
        public void Draw(Color color, float thickness)
        {
            foreach (var line in Lines)
                line?.Draw(color, thickness);
        }
        public void Draw(Color color, float thickness, float layer)
        {
            foreach (var line in Lines)
                line?.Draw(color, thickness, layer);
        }

        public bool Intersects(Line line)
        {
            foreach (var t in Lines)
                if (t?.Intersects(line) ?? false)
                    return true;
            return false;
        }
        public bool Intersects(Line line, ref Vector2 intersection)
        {
            var originalEnd = line.End;
            var intersects = false;
            foreach (var t in Lines)
                if (t?.Intersects(line, ref intersection) ?? false)
                {
                    line.End = intersection;
                    intersects = true;
                }
            line.End = originalEnd;
            return intersects;
        }
        public bool Intersects(Polygon polygon)
        {
            foreach (var a in Lines)
                foreach (var t in polygon.Lines)
                    if (a?.Intersects(t) ?? false)
                        return true;
            return false;
        }
        public bool Intersects(Polygon polygon, ref Vector2 intersection)
        {
            foreach (var t in Lines)
                foreach (var b in polygon.Lines)
                    if (t?.Intersects(b, ref intersection) ?? false)
                        return true;
            return false;
        }

        public float PerpendicularAngle(Line line) { return PerpendicularAngle(line, position, angle); }
        public float PerpendicularAngle(Line line, Vector2 position, float angle) { return line.PerpendicularAngle(Mathf.Angle(position, Mathf.Move(position, angle, -1))); }
        public float PerpendicularAngle(Polygon polygon) { return PerpendicularAngle(polygon, position, angle); }
        public float PerpendicularAngle(Polygon polygon, Vector2 position, float angle)
        {
            foreach (var line in polygon.Lines)
                if (Intersects(line))
                    return line.PerpendicularAngle(Mathf.Angle(position, Mathf.Move(position, angle, -1)));
            return 0;
        }
        public float ReflectionAngle(float angle, float perpendicularAngle)
        {
            float reflectionAngle = perpendicularAngle,
                angleDif = Mathf.AngleDifference(angle, MathHelper.WrapAngle(perpendicularAngle + Mathf.Pi));
            float b;
            if (Mathf.AngleDifference((b = (perpendicularAngle + angleDif)), MathHelper.WrapAngle(angle + Mathf.Pi)) <= Mathf.PiOver1024)
                reflectionAngle -= angleDif;
            else
                reflectionAngle = b;
            return reflectionAngle;
        }
        public float ReflectionAngle(Line line) { return ReflectionAngle(angle, PerpendicularAngle(line)); }
        public float ReflectionAngle(Line line, Vector2 position, float angle) { return ReflectionAngle(angle, PerpendicularAngle(line, position, angle)); }
        public float ReflectionAngle(Polygon polygon, Vector2 position, float angle) { return ReflectionAngle(angle, PerpendicularAngle(polygon, position, angle)); }

        public float MinX { get { var minX = float.MaxValue; foreach (var t in Lines) minX = MathHelper.Min(minX, MathHelper.Min(t.Start.X, t.End.X)); return minX; } }
        public float MinY { get { var minY = float.MaxValue; foreach (var t in Lines) minY = MathHelper.Min(minY, MathHelper.Min(t.Start.Y, t.End.Y)); return minY; } }
        public float MaxX { get { var maxX = float.MinValue; foreach (var t in Lines) maxX = MathHelper.Max(maxX, MathHelper.Max(t.Start.X, t.End.X)); return maxX; } }
        public float MaxY { get { var maxY = float.MinValue; foreach (var t in Lines) maxY = MathHelper.Max(maxY, MathHelper.Max(t.Start.Y, t.End.Y)); return maxY; } }
        public float Width => (MaxX - MinX);
        public float Height => (MaxY - MinY);
        public float FixedWidth { get; private set; }
        public float FixedHeight { get; private set; }

        public Polygon Clone()
        {
            var lines = new Line[Lines.Length];
            for (var i = 0; i < Lines.Length; i++) lines[i] = new Line((Mathf.Rotate(Lines[i].Start, -angle, position) - position), (Mathf.Rotate(Lines[i].End, -angle, position) - position));
            return new Polygon(lines);
        }

        public static Polygon CreateSquare(float radius) { return CreateRectangle(new Vector2(radius), Vector2.Zero); }
        public static Polygon CreateSquare(float radius, Vector2 origin) { return CreateRectangle(new Vector2(radius), origin); }
        public static Polygon CreateSquareWithCross(float radius) { return CreateRectangleWithCross(new Vector2(radius), Vector2.Zero); }
        public static Polygon CreateSquareWithCross(float radius, Vector2 origin) { return CreateRectangleWithCross(new Vector2(radius), origin); }
        public static Polygon CreateRectangle(Vector2 size) { return CreateRectangle(size, Vector2.Zero); }
        public static Polygon CreateRectangle(Vector2 size, Vector2 origin)
        {
            size.X += 1;
            size.Y += 1;
            float x = (size.X / 2), y = (size.Y / 2);
            return new Polygon(new[]
            {
                new Line((new Vector2(-x, -y) - origin), (new Vector2((x - PxOffset), -y) - origin)),
                new Line((new Vector2(x, -y) - origin), (new Vector2(x, (y - PxOffset)) - origin)),
                new Line((new Vector2(x, y) - origin), (new Vector2((-x + PxOffset), y) - origin)),
                new Line((new Vector2(-x, y) - origin), (new Vector2(-x, (-y + PxOffset)) - origin))
            });
        }
        public static Polygon CreateRectangleWithCross(Vector2 size) { return CreateRectangleWithCross(size, Vector2.Zero); }
        public static Polygon CreateRectangleWithCross(Vector2 size, Vector2 origin)
        {
            float x = (size.X / 2), y = (size.Y / 2);
            return new Polygon(new[]
            {
                new Line((new Vector2(-x, -y) - origin), (new Vector2((x - PxOffset), -y) - origin)),
                new Line((new Vector2(x, -y) - origin), (new Vector2(x, (y - PxOffset)) - origin)),
                new Line((new Vector2(x, y) - origin), (new Vector2((-x + PxOffset), y) - origin)),
                new Line((new Vector2(-x, y) - origin), (new Vector2(-x, (-y + PxOffset)) - origin)),
                new Line((new Vector2((-x + PxOffset), (-y + PxOffset)) - origin), (new Vector2((x - PxOffset), (y - PxOffset)) - origin)),
                new Line((new Vector2((x - PxOffset), (-y + PxOffset)) - origin), (new Vector2((-x + PxOffset), (y - PxOffset)) - origin))
            });
        }

        public static Polygon CreateCircle(float radius) { return CreateEllipse(new Vector2(radius), Vector2.Zero, 8); }
        public static Polygon CreateCircle(float radius, byte sides) { return CreateEllipse(new Vector2(radius), Vector2.Zero, sides); }
        public static Polygon CreateCircle(float radius, Vector2 origin) { return CreateEllipse(new Vector2(radius), origin, 8); }
        public static Polygon CreateCircle(float radius, Vector2 origin, byte sides) { return CreateEllipse(new Vector2(radius), origin, sides); }
        public static Polygon CreateCircleWithCross(float radius) { return CreateEllipseWithCross(new Vector2(radius), Vector2.Zero, 8); }
        public static Polygon CreateCircleWithCross(float radius, byte sides) { return CreateEllipseWithCross(new Vector2(radius), Vector2.Zero, sides); }
        public static Polygon CreateCircleWithCross(float radius, Vector2 origin) { return CreateEllipseWithCross(new Vector2(radius), origin, 8); }
        public static Polygon CreateCircleWithCross(float radius, Vector2 origin, byte sides) { return CreateEllipseWithCross(new Vector2(radius), origin, sides); }
        public static Polygon CreateEllipse(Vector2 radius) { return CreateEllipse(radius, Vector2.Zero, 8); }
        public static Polygon CreateEllipse(Vector2 radius, byte sides) { return CreateEllipse(radius, Vector2.Zero, sides); }
        public static Polygon CreateEllipse(Vector2 radius, Vector2 origin) { return CreateEllipse(radius, origin, 8); }
        public static Polygon CreateEllipse(Vector2 radius, Vector2 origin, byte sides)
        {
            float sideLengthX = (radius.X / sides * MathHelper.Pi), sideLengthY = (radius.Y / sides * MathHelper.Pi);
            var start = new Vector2(origin.X, (-(radius.Y / 2) + origin.Y));
            var lines = new List<Line>(sides);
            for (var side = 0; side < sides; side++)
            {
                var angle = MathHelper.ToRadians(((side + .5f) / sides) * 360);
                var end = new Vector2((start.X + ((float)(Math.Cos(angle) * sideLengthX))), (start.Y + ((float)(Math.Sin(angle) * sideLengthY))));
                lines.Add(new Line(start, end));
                if (side > 0)
                    start = Mathf.Move(end, angle, PxOffset);
                else
                    start = end;
            }
            return new Polygon(lines.ToArray());
        }
        public static Polygon CreateEllipseWithCross(Vector2 radius) { return CreateEllipseWithCross(radius, Vector2.Zero, 8); }
        public static Polygon CreateEllipseWithCross(Vector2 radius, byte sides) { return CreateEllipseWithCross(radius, Vector2.Zero, sides); }
        public static Polygon CreateEllipseWithCross(Vector2 radius, Vector2 origin) { return CreateEllipseWithCross(radius, origin, 8); }
        public static Polygon CreateEllipseWithCross(Vector2 radius, Vector2 origin, byte sides)
        {
            var lines = new List<Line>(2 + sides)
            {
                new Line((new Vector2(-(radius.X/2.5f), -(radius.Y/2.5f)) - origin), (new Vector2((radius.X/2.5f), (radius.Y/2.5f)) - origin)),
                new Line((new Vector2((radius.X/2.5f), -(radius.Y/2.5f)) - origin), (new Vector2(-(radius.X/2.5f), (radius.Y/2.5f)) - origin))
            };
            float sideLengthX = (radius.X / sides * MathHelper.Pi), sideLengthY = (radius.Y / sides * MathHelper.Pi);
            var start = new Vector2(origin.X, (-(radius.Y / 2) + origin.Y));
            for (var side = 0; side < sides; side++)
            {
                var angle = MathHelper.ToRadians(((side + .5f) / sides) * 360);
                var end = new Vector2((start.X + ((float)(Math.Cos(angle) * sideLengthX))), (start.Y + ((float)(Math.Sin(angle) * sideLengthY))));
                lines.Add(new Line(start, end));
                start = end;
            }
            return new Polygon(lines.ToArray());
        }

        public static Polygon CreateCross(float radius) { return CreateCross(radius, Vector2.Zero); }
        public static Polygon CreateCross(Vector2 radius) { return CreateCross(radius, Vector2.Zero); }
        public static Polygon CreateCross(float radius, Vector2 origin)
        {
            var lines = new[]
            {
                new Line((new Vector2(-(radius/2f), -(radius/2f)) - origin), (new Vector2((radius/2f), (radius/2f)) - origin)),
                new Line((new Vector2((radius/2f), -(radius/2f)) - origin), (new Vector2(-(radius/2f), (radius/2f)) - origin))
            };
            return new Polygon(lines);
        }
        public static Polygon CreateCross(Vector2 radius, Vector2 origin)
        {
            var lines = new[]
            {
                new Line((new Vector2(-(radius.X/2f), -(radius.X/2f)) - origin), (new Vector2((radius.Y/2f), (radius.Y/2f)) - origin)),
                new Line((new Vector2((radius.X/2f), -(radius.X/2f)) - origin), (new Vector2(-(radius.Y/2f), (radius.Y/2f)) - origin))
            };
            return new Polygon(lines);
        }
    }
}