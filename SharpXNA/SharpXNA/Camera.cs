using SharpXNA.Input;
using Microsoft.Xna.Framework;
using System;

namespace SharpXNA
{
    public class Camera
    {
        public Vector2 Position { get { return position; } set { position = value; UpdatePositionMatrices(); UpdateViewMatrices(); } }
        public float Angle { get { return angle; } set { angle = value; rotationZ = CreateRotationZ(angle); UpdateViewMatrices(); } }
        public float Zoom { get { return zoom; } set { zoom = value; scale = CreateScale(zoom); UpdateViewMatrices(); } }
        public Matrix ScreenTranslation { get { return screenTranslation; } set { screenTranslation = value; UpdateViewMatrices(); } }

        public float X { get { return Position.X; } set { position.X = value; UpdatePositionMatrices(); UpdateViewMatrices(); } }
        public float Y { get { return Position.Y; } set { position.Y = value; UpdatePositionMatrices(); UpdateViewMatrices(); } }

        private Vector2 position;
        private float angle, zoom;
        private Matrix linearPositionTranslation, pointPositionTranslation, rotationZ, scale, screenTranslation, linearMatrix, pointMatrix, invert;

        public void UpdatePositionMatrices() { linearPositionTranslation = CreateLinearPositionTranslation(position); pointPositionTranslation = CreatePointPositionTranslation(position); }
        public void UpdateViewMatrices() { linearMatrix = CreateCameraMatrix(linearPositionTranslation, rotationZ, scale, ScreenTranslation); pointMatrix = CreateCameraMatrix(pointPositionTranslation, rotationZ, scale, ScreenTranslation); invert = Matrix.Invert(linearMatrix); }

        public static Matrix CreateLinearPositionTranslation(Vector2 position) { return Matrix.CreateTranslation(new Vector3(-position, 0)); }
        public static Matrix CreatePointPositionTranslation(Vector2 position) { return Matrix.CreateTranslation(new Vector3(-new Vector2((int)Math.Round(position.X), (int)Math.Round(position.Y)), 0)); }
        public static Matrix CreateRotationZ(float angle) { return Matrix.CreateRotationZ(-angle); }
        public static Matrix CreateScale(float zoom) { return Matrix.CreateScale(new Vector3(zoom, zoom, 1)); }
        public static Matrix CreateScreenTranslation(float width, float height) { return Matrix.CreateTranslation((width / 2), (height / 2), 0); }
        public static Matrix CreateCameraMatrix(Matrix position, Matrix rotationZ, Matrix scale, Matrix screen) { return (position * rotationZ * scale * screen); }

        public Camera(float angle = 0, float zoom = 1)
        {
            position = Vector2.Zero; linearPositionTranslation = CreateLinearPositionTranslation(position);
            this.angle = angle; rotationZ = CreateRotationZ(angle);
            this.zoom = zoom; scale = CreateScale(zoom);
            screenTranslation = CreateScreenTranslation(Screen.BackBufferWidth, Screen.BackBufferHeight);
            UpdateViewMatrices();
        }
        public Camera(Vector2 position, float angle = 0, float zoom = 1)
        {
            this.position = position; linearPositionTranslation = CreateLinearPositionTranslation(position);
            this.angle = angle; rotationZ = CreateRotationZ(angle);
            this.zoom = zoom; scale = CreateScale(zoom);
            screenTranslation = CreateScreenTranslation(Screen.BackBufferWidth, Screen.BackBufferHeight);
            UpdateViewMatrices();
        }

        public enum Samplers { Linear, Point }
        public Matrix View(Samplers sampler = Samplers.Linear)
        {
            Mouse.CameraPosition = Mouse.Position.ToVector2();
            Vector2.Transform(ref Mouse.CameraPosition, ref invert, out Mouse.CameraPosition);
            return ((sampler == Samplers.Linear) ? linearMatrix : pointMatrix);
        }
    }
}