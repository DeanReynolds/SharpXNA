using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpXNA.Collision
{
    public class RotatedRectangle
    {
        public Rectangle CollisionRectangle;
        public float Rotation;
        public Vector2 Origin;

        public RotatedRectangle(Rectangle rectangle, float angle)
        {
            int widthOver2 = (rectangle.Width / 2), heightOver2 = (rectangle.Height / 2);
            CollisionRectangle = new Rectangle((rectangle.X - widthOver2), (rectangle.Y - heightOver2), rectangle.Width, rectangle.Height);
            Rotation = angle;
            Origin = new Vector2(widthOver2, heightOver2);
        }

        public void ChangePosition(int theXPositionAdjustment, int theYPositionAdjustment)
        {
            CollisionRectangle.X += theXPositionAdjustment;
            CollisionRectangle.Y += theYPositionAdjustment;
        }

        public bool Intersects(Rectangle theRectangle) => Intersects(new RotatedRectangle(theRectangle, 0.0f));
        public bool Intersects(RotatedRectangle theRectangle)
        {
            var aRectangleAxis = new List<Vector2>()
            {
                (UpperRightCorner() - UpperLeftCorner()),
                (UpperRightCorner() - LowerRightCorner()),
                (theRectangle.UpperLeftCorner() - theRectangle.LowerLeftCorner()),
                (theRectangle.UpperLeftCorner() - theRectangle.UpperRightCorner())
            };
            foreach (var aAxis in aRectangleAxis)
                if (!IsAxisCollision(theRectangle, aAxis))
                    return false;
            return true;
        }

        private bool IsAxisCollision(RotatedRectangle theRectangle, Vector2 aAxis)
        {
            var aRectangleAScalars = new List<int>()
            {
                GenerateScalar(theRectangle.UpperLeftCorner(), aAxis),
                GenerateScalar(theRectangle.UpperRightCorner(), aAxis),
                GenerateScalar(theRectangle.LowerLeftCorner(), aAxis),
                GenerateScalar(theRectangle.LowerRightCorner(), aAxis)
            };
            var aRectangleBScalars = new List<int>()
            {
                GenerateScalar(UpperLeftCorner(), aAxis),
                GenerateScalar(UpperRightCorner(), aAxis),
                GenerateScalar(LowerLeftCorner(), aAxis),
                GenerateScalar(LowerRightCorner(), aAxis)
            };
            int aRectangleAMinimum = aRectangleAScalars.Min(), aRectangleAMaximum = aRectangleAScalars.Max(), aRectangleBMinimum = aRectangleBScalars.Min(), aRectangleBMaximum = aRectangleBScalars.Max();
            if (aRectangleBMinimum <= aRectangleAMaximum && aRectangleBMaximum >= aRectangleAMaximum)
                return true;
            return aRectangleAMinimum <= aRectangleBMaximum && aRectangleAMaximum >= aRectangleBMaximum;
        }

        private int GenerateScalar(Vector2 theRectangleCorner, Vector2 theAxis)
        {
            float aNumerator = (theRectangleCorner.X * theAxis.X) + (theRectangleCorner.Y * theAxis.Y);
            float aDenominator = (theAxis.X * theAxis.X) + (theAxis.Y * theAxis.Y);
            float aDivisionResult = aNumerator / aDenominator;
            Vector2 aCornerProjected = new Vector2(aDivisionResult * theAxis.X, aDivisionResult * theAxis.Y);
            return (int)((theAxis.X * aCornerProjected.X) + (theAxis.Y * aCornerProjected.Y));
        }

        private Vector2 RotatePoint(Vector2 thePoint, Vector2 theOrigin, float theRotation)
        {
            return new Vector2((float)(theOrigin.X + (thePoint.X - theOrigin.X) * Math.Cos(theRotation) - (thePoint.Y - theOrigin.Y) * Math.Sin(theRotation)), (float)(theOrigin.Y + (thePoint.Y - theOrigin.Y) * Math.Cos(theRotation) + (thePoint.X - theOrigin.X) * Math.Sin(theRotation)));
        }

        public Vector2 UpperLeftCorner()
        {
            Vector2 aUpperLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Top);
            return RotatePoint(aUpperLeft, aUpperLeft + Origin, Rotation);
        }
        public Vector2 UpperRightCorner()
        {
            Vector2 aUpperRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Top);
            return RotatePoint(aUpperRight, aUpperRight + new Vector2(-Origin.X, Origin.Y), Rotation);
        }
        public Vector2 LowerLeftCorner()
        {
            Vector2 aLowerLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Bottom);
            return RotatePoint(aLowerLeft, aLowerLeft + new Vector2(Origin.X, -Origin.Y), Rotation);
        }
        public Vector2 LowerRightCorner()
        {
            Vector2 aLowerRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Bottom);
            return RotatePoint(aLowerRight, aLowerRight + new Vector2(-Origin.X, -Origin.Y), Rotation);
        }

        public int X => CollisionRectangle.X;
        public int Y => CollisionRectangle.Y;
        public int Width => CollisionRectangle.Width;
        public int Height => CollisionRectangle.Height;
    }
}