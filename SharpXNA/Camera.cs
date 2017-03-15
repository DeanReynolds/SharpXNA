using SharpXNA.Input;
using Microsoft.Xna.Framework;
using System;

namespace SharpXNA
{
    public class Camera
    {
        public Vector2 Position { get { return _position; } set { _position = value; UpdatePositionMatrices(); UpdateViewMatrices(); } }
        public float Angle { get { return _angle; } set { _angle = value; _rotationZ = CreateRotationZ(_angle); UpdateViewMatrices(); } }
        public float Zoom { get { return _zoom; } set { _zoom = value; _scale = CreateScale(_zoom); UpdateViewMatrices(); } }
        public Matrix ScreenTranslation { get { return _screenTranslation; } set { _screenTranslation = value; UpdateViewMatrices(); } }

        public float X { get { return Position.X; } set { _position.X = value; UpdatePositionMatrices(); UpdateViewMatrices(); } }
        public float Y { get { return Position.Y; } set { _position.Y = value; UpdatePositionMatrices(); UpdateViewMatrices(); } }

        private Vector2 _position;
        private float _angle, _zoom;
        internal Matrix _linearPositionTranslation, _pointPositionTranslation, _rotationZ, _scale, _screenTranslation, _linearMatrix, _pointMatrix, _invert;

        public void UpdatePositionMatrices() { _linearPositionTranslation = CreateLinearPositionTranslation(_position); _pointPositionTranslation = CreatePointPositionTranslation(_position); }
        public void UpdateViewMatrices()
        {
            _linearMatrix = CreateCameraMatrix(_linearPositionTranslation, _rotationZ, _scale, ScreenTranslation);
            _pointMatrix = CreateCameraMatrix(_pointPositionTranslation, _rotationZ, _scale, ScreenTranslation);
            _invert = Matrix.Invert(_linearMatrix); UpdateMousePosition();
        }

        public static Matrix CreateLinearPositionTranslation(Vector2 position) { return Matrix.CreateTranslation(new Vector3(-position, 0)); }
        public static Matrix CreatePointPositionTranslation(Vector2 position) { return Matrix.CreateTranslation(new Vector3(-new Vector2((int)position.X, (int)position.Y), 0)); }
        public static Matrix CreateRotationZ(float angle) { return Matrix.CreateRotationZ(-angle); }
        public static Matrix CreateScale(float zoom) { return Matrix.CreateScale(new Vector3(zoom, zoom, 1)); }
        public static Matrix CreateScreenTranslation(float width, float height) { return Matrix.CreateTranslation((width / 2), (height / 2), 0); }
        public static Matrix CreateCameraMatrix(Matrix position, Matrix rotationZ, Matrix scale, Matrix screen) { return (position * rotationZ * scale * screen); }

        public Camera(float angle = 0, float zoom = 1)
        {
            _position = Vector2.Zero;
            UpdatePositionMatrices();
            _angle = angle; _rotationZ = CreateRotationZ(angle);
            _zoom = zoom; _scale = CreateScale(zoom);
            _screenTranslation = CreateScreenTranslation(Screen.ViewportWidth, Screen.ViewportHeight);
            UpdateViewMatrices();
        }
        public Camera(Vector2 position, float angle = 0, float zoom = 1)
        {
            _position = position;
            UpdatePositionMatrices();
            _angle = angle; _rotationZ = CreateRotationZ(angle);
            _zoom = zoom; _scale = CreateScale(zoom);
            _screenTranslation = CreateScreenTranslation(Screen.ViewportWidth, Screen.ViewportHeight);
            UpdateViewMatrices();
        }

        public enum Samplers { Linear, Point }
        public Matrix View(Samplers sampler = Samplers.Linear) { return ((sampler == Samplers.Linear) ? _linearMatrix : _pointMatrix); }

        public void UpdateMousePosition()
        {
            var mousePos = Mouse.Position.ToVector2();
            Vector2.Transform(ref mousePos, ref _invert, out mousePos);
            MousePosition = mousePos;
        }
        public Vector2 MousePosition { get; private set; }
    }
}