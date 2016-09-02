using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA.Collision
{
    public class Polygon
    {
        public Line[] Lines;
        internal Vector2 position;
        public Vector2 Position { get { return position; } set { for (int i = 0; i < Lines.Length; i++) { Lines[i].Subtract(position); Lines[i].Add(value); } position = value; } }

        internal float angle;
        public float Angle
        {
            get { return angle; }
            set
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    Lines[i].Start = Globe.Rotate(Lines[i].Start, -angle, position);
                    Lines[i].End = Globe.Rotate(Lines[i].End, -angle, position);
                    Lines[i].Start = Globe.Rotate(Lines[i].Start, value, position);
                    Lines[i].End = Globe.Rotate(Lines[i].End, value, position);
                }
                angle = value;
            }
        }

        public Polygon(params Line[] lines) { Lines = lines; }
        public Polygon(List<Line> lines) { Lines = lines.ToArray(); }

        public void Draw() { Draw(Color.White, 1, SpriteEffects.None, 0); }
        public void Draw(SpriteEffects effect) { Draw(Color.White, 1, effect, 0); }
        public void Draw(SpriteEffects effect, float layer) { Draw(Color.White, 1, effect, layer); }
        public void Draw(float thickness) { Draw(Color.White, thickness, SpriteEffects.None, 0); }
        public void Draw(float thickness, float layer) { Draw(Color.White, thickness, SpriteEffects.None, layer); }
        public void Draw(float thickness, SpriteEffects effect) { Draw(Color.White, thickness, effect, 0); }
        public void Draw(float thickness, SpriteEffects effect, float layer) { Draw(Color.White, thickness, effect, layer); }
        public void Draw(Color color) { Draw(color, 1, SpriteEffects.None, 0); }
        public void Draw(Color color, SpriteEffects effect) { Draw(color, 1, effect, 0); }
        public void Draw(Color color, SpriteEffects effect, float layer) { Draw(color, 1, effect, layer); }
        public void Draw(Color color, float thickness) { Draw(color, thickness, SpriteEffects.None, 0); }
        public void Draw(Color color, float thickness, float layer) { Draw(color, thickness, SpriteEffects.None, layer); }
        public void Draw(Color color, float thickness, SpriteEffects effect) { Draw(color, thickness, effect, 0); }
        public void Draw(Color color, float thickness, SpriteEffects effect, float layer) { foreach (Line line in Lines) line.Draw(color, thickness, effect, layer); }
        public void Draw(Batch batch) { Draw(batch, Color.White, 1, SpriteEffects.None, 0); }
        public void Draw(Batch batch, SpriteEffects effect) { Draw(batch, Color.White, 1, effect, 0); }
        public void Draw(Batch batch, SpriteEffects effect, float layer) { Draw(batch, Color.White, 1, effect, layer); }
        public void Draw(Batch batch, float thickness) { Draw(batch, Color.White, thickness, SpriteEffects.None, 0); }
        public void Draw(Batch batch, float thickness, float layer) { Draw(batch, Color.White, thickness, SpriteEffects.None, layer); }
        public void Draw(Batch batch, float thickness, SpriteEffects effect) { Draw(batch, Color.White, thickness, effect, 0); }
        public void Draw(Batch batch, float thickness, SpriteEffects effect, float layer) { Draw(batch, Color.White, thickness, effect, layer); }
        public void Draw(Batch batch, Color color) { Draw(batch, color, 1, SpriteEffects.None, 0); }
        public void Draw(Batch batch, Color color, SpriteEffects effect) { Draw(batch, color, 1, effect, 0); }
        public void Draw(Batch batch, Color color, SpriteEffects effect, float layer) { Draw(batch, color, 1, effect, layer); }
        public void Draw(Batch batch, Color color, float thickness) { Draw(batch, color, thickness, SpriteEffects.None, 0); }
        public void Draw(Batch batch, Color color, float thickness, float layer) { Draw(batch, color, thickness, SpriteEffects.None, layer); }
        public void Draw(Batch batch, Color color, float thickness, SpriteEffects effect) { Draw(batch, color, thickness, effect, 0); }
        public void Draw(Batch batch, Color color, float thickness, SpriteEffects effect, float layer) { foreach (Line line in Lines) line.Draw(batch, color, thickness, effect, layer); }

        public bool Intersects(Line line) { for (int i = 0; i < Lines.Length; i++) if (Lines[i].Intersects(line)) return true; return false; }
        public bool Intersects(Line line, ref Vector2 intersection) { var intersects = false; for (uint i = 0; i < Lines.Length; i++) if (Lines[i].Intersects(line, ref intersection)) intersects = true; return intersects; }
        public bool Intersects(Polygon polygon) { foreach (Line a in Lines) foreach (Line b in polygon.Lines) if (a.Intersects(b)) return true; return false; }
        public bool Intersects(Polygon polygon, ref Vector2 intersection) { foreach (Line a in Lines) foreach (Line b in polygon.Lines) if (a.Intersects(b, ref intersection)) return true; return false; }

        public Polygon Clone()
        {
            var polygon = new Polygon(new Line[Lines.Length]);
            for (int i = 0; i < Lines.Length; i++) polygon.Lines[i] = new Line((Globe.Rotate(Lines[i].Start, -angle, position) - position), (Globe.Rotate(Lines[i].End, -angle, position) - position));
            return polygon;
        }

        public static Polygon CreateSquare(float radius) { return CreateRectangle(new Vector2(radius), Vector2.Zero); }
        public static Polygon CreateSquare(float radius, Vector2 origin) { return CreateRectangle(new Vector2(radius), origin); }
        public static Polygon CreateSquareWithCross(float radius) { return CreateRectangleWithCross(new Vector2(radius), Vector2.Zero); }
        public static Polygon CreateSquareWithCross(float radius, Vector2 origin) { return CreateRectangleWithCross(new Vector2(radius), origin); }
        public static Polygon CreateRectangle(Vector2 size) { return CreateRectangle(size, Vector2.Zero); }
        public static Polygon CreateRectangle(Vector2 size, Vector2 origin)
        {
            Line[] lines = new Line[4]
            {
                new Line((new Vector2(-(size.X/2), -(size.Y/2)) - origin), (new Vector2((size.X/2), -(size.Y/2)) - origin)),
                new Line((new Vector2((size.X/2), -(size.Y/2)) - origin), (new Vector2((size.X/2), (size.Y/2)) - origin)),
                new Line((new Vector2(-(size.X/2), -(size.Y/2)) - origin), (new Vector2(-(size.X/2), (size.Y/2)) - origin)),
                new Line((new Vector2(-(size.X/2), (size.Y/2)) - origin), (new Vector2((size.X/2), (size.Y/2)) - origin))
            };
            return new Polygon(lines);
        }
        public static Polygon CreateRectangleWithCross(Vector2 size) { return CreateRectangleWithCross(size, Vector2.Zero); }
        public static Polygon CreateRectangleWithCross(Vector2 size, Vector2 origin)
        {
            Line[] lines = new Line[6]
            {
                new Line((new Vector2(-(size.X/2), -(size.Y/2)) - origin), (new Vector2((size.X/2), -(size.Y/2)) - origin)),
                new Line((new Vector2((size.X/2), -(size.Y/2)) - origin), (new Vector2((size.X/2), (size.Y/2)) - origin)),
                new Line((new Vector2(-(size.X/2), -(size.Y/2)) - origin), (new Vector2(-(size.X/2), (size.Y/2)) - origin)),
                new Line((new Vector2(-(size.X/2), (size.Y/2)) - origin), (new Vector2((size.X/2), (size.Y/2)) - origin)),
                new Line((new Vector2(-(size.X/2), -(size.Y/2)) - origin), (new Vector2((size.X/2), (size.Y/2)) - origin)),
                new Line((new Vector2((size.X/2), -(size.Y/2)) - origin), (new Vector2(-(size.X/2), (size.Y/2)) - origin))
            };
            return new Polygon(lines);
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
            List<Line> lines = new List<Line>(sides);
            for (int side = 0; side < sides; side++)
            {
                var angle = MathHelper.ToRadians(((side + .5f) / sides) * 360);
                var end = new Vector2((start.X + ((float)(Math.Cos(angle) * sideLengthX))), (start.Y + ((float)(Math.Sin(angle) * sideLengthY))));
                lines.Add(new Line(start, end));
                start = end;
            }
            return new Polygon(lines.ToArray());
        }
        public static Polygon CreateEllipseWithCross(Vector2 radius) { return CreateEllipseWithCross(radius, Vector2.Zero, 8); }
        public static Polygon CreateEllipseWithCross(Vector2 radius, byte sides) { return CreateEllipseWithCross(radius, Vector2.Zero, sides); }
        public static Polygon CreateEllipseWithCross(Vector2 radius, Vector2 origin) { return CreateEllipseWithCross(radius, origin, 8); }
        public static Polygon CreateEllipseWithCross(Vector2 radius, Vector2 origin, byte sides)
        {
            List<Line> lines = new List<Line>(2 + sides)
            {
                new Line((new Vector2(-(radius.X/2.5f), -(radius.Y/2.5f)) - origin), (new Vector2((radius.X/2.5f), (radius.Y/2.5f)) - origin)),
                new Line((new Vector2((radius.X/2.5f), -(radius.Y/2.5f)) - origin), (new Vector2(-(radius.X/2.5f), (radius.Y/2.5f)) - origin))
            };
            float sideLengthX = (radius.X / sides * MathHelper.Pi), sideLengthY = (radius.Y / sides * MathHelper.Pi);
            var start = new Vector2(origin.X, (-(radius.Y / 2) + origin.Y));
            for (int side = 0; side < sides; side++)
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
            Line[] lines = new Line[2]
            {
                new Line((new Vector2(-(radius/2f), -(radius/2f)) - origin), (new Vector2((radius/2f), (radius/2f)) - origin)),
                new Line((new Vector2((radius/2f), -(radius/2f)) - origin), (new Vector2(-(radius/2f), (radius/2f)) - origin))
            };
            return new Polygon(lines);
        }
        public static Polygon CreateCross(Vector2 radius, Vector2 origin)
        {
            Line[] lines = new Line[2]
            {
                new Line((new Vector2(-(radius.X/2f), -(radius.X/2f)) - origin), (new Vector2((radius.Y/2f), (radius.Y/2f)) - origin)),
                new Line((new Vector2((radius.X/2f), -(radius.X/2f)) - origin), (new Vector2(-(radius.Y/2f), (radius.Y/2f)) - origin))
            };
            return new Polygon(lines);
        }
    }
}