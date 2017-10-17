using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace SharpXNA.Collision
{
    public class RotatedRectangle
    {
        Rectangle _rectangle;
        float _angle;

        public float Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                UpdateCorners();
            }
        }
        public Vector2 Origin { get; private set; }

        public int X
        {
            get { return _rectangle.X; }
            set
            {
                _rectangle.X = value;
                UpdateCorners();
            }
        }
        public int Y
        {
            get { return _rectangle.Y; }
            set
            {
                _rectangle.Y = value;
                UpdateCorners();
            }
        }
        public int Width
        {
            get { return _rectangle.Width; }
            set
            {
                _rectangle.Width = value;
                UpdateCorners();
            }
        }
        public int Height
        {
            get { return _rectangle.Height; }
            set
            {
                _rectangle.Height = value;
                UpdateCorners();
            }
        }

        public Vector2 TopLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 BottomRight { get; private set; }

        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public RotatedRectangle(Rectangle rectangle, float angle)
        {
            int widthOver2 = (rectangle.Width / 2), heightOver2 = (rectangle.Height / 2);
            _rectangle = new Rectangle((rectangle.X - widthOver2), (rectangle.Y - heightOver2), rectangle.Width, rectangle.Height);
            Angle = angle;
            Origin = new Vector2(widthOver2, heightOver2);
            UpdateCorners();
        }

        public bool Intersects(Rectangle rectangle) => Intersects(new RotatedRectangle(rectangle, 0));
        public bool Intersects(RotatedRectangle rectangle)
        {
            var rAxis = new Vector2[4]
            {
                (TopRight - TopLeft),
                (TopRight - BottomRight),
                (rectangle.TopLeft - rectangle.BottomLeft),
                (rectangle.TopLeft - rectangle.TopRight)
            };
            foreach (var axis in rAxis)
                if (!IsAxisCollision(rectangle, axis))
                    return false;
            return true;
        }

        bool IsAxisCollision(RotatedRectangle rectangle, Vector2 axis)
        {
            var scalars = Scalars(axis);
            var rectangleScalars = rectangle.Scalars(axis);
            if ((scalars[0] <= rectangleScalars[1]) && (scalars[1] >= rectangleScalars[1]))
                return true;
            return ((rectangleScalars[0] <= scalars[1]) && (rectangleScalars[1] >= scalars[1]));
        }
        int Scalar(Vector2 corner, Vector2 axis)
        {
            var divisor = (((corner.X * axis.X) + (corner.Y * axis.Y)) / ((axis.X * axis.X) + (axis.Y * axis.Y)));
            var projectedCorner = new Vector2(divisor * axis.X, divisor * axis.Y);
            return (int)((axis.X * projectedCorner.X) + (axis.Y * projectedCorner.Y));
        }
        int[] Scalars(Vector2 axis)
        {
            int[] array;
            return new int[2]
            {
                ((array = new int[4]
                {
                    Scalar(TopLeft, axis),
                    Scalar(TopRight, axis),
                    Scalar(BottomLeft, axis),
                    Scalar(BottomRight, axis)
                }).Min()),
                array.Max()
            };
        }

        Vector2 Rotate(Vector2 position, float angle, Vector2 origin) => (Vector2.Transform((position - origin), Matrix.CreateRotationZ(angle)) + origin);
        void UpdateCorners()
        {
            Vector2 topLeft = new Vector2(_rectangle.Left, _rectangle.Top),
                topRight = new Vector2(_rectangle.Right, _rectangle.Top),
                bottomLeft = new Vector2(_rectangle.Left, _rectangle.Bottom),
                bottomRight = new Vector2(_rectangle.Right, _rectangle.Bottom);
            var corners = new Vector2[4]
            {
                TopLeft = Rotate(topLeft, Angle, (topLeft + Origin)),
                TopRight = Rotate(topRight, Angle, (topRight + new Vector2(-Origin.X, Origin.Y))),
                BottomLeft = Rotate(bottomLeft, Angle, (bottomLeft + new Vector2(Origin.X, -Origin.Y))),
                BottomRight = Rotate(bottomRight, Angle, (bottomRight + new Vector2(-Origin.X, -Origin.Y)))
            };
            Left = Top = int.MaxValue;
            Right = Bottom = int.MinValue;
            foreach (var c in corners)
            {
                Left = Math.Min(Left, (int)c.X);
                Right = Math.Max(Right, (int)c.X);
                Top = Math.Min(Top, (int)c.Y);
                Bottom = Math.Max(Bottom, (int)c.Y);
            }
        }
    }
}