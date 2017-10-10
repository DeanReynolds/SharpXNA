using SharpXNA.Input;
using Microsoft.Xna.Framework;
using System;

namespace SharpXNA
{
    public class Camera
    {
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                PositionTranslation = Matrix.CreateTranslation(new Vector3(-_position, 0));
                UpdateViewMatrices();
            }
        }
        public float Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                RotationMatrix = Matrix.CreateRotationZ(-_angle);
                UpdateViewMatrices();
            }
        }
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                ScaleMatrix = Matrix.CreateScale(new Vector3(_zoom, _zoom, 1));
                UpdateViewMatrices();
            }
        }
        public Matrix Projection;
        public Matrix ScreenTranslation
        {
            get { return _screenTranslation; }
            set
            {
                _screenTranslation = value;
                UpdateViewMatrices();
            }
        }

        public float X
        {
            get { return Position.X; }
            set
            {
                _position.X = value;
                PositionTranslation = Matrix.CreateTranslation(new Vector3(-_position, 0));
                UpdateViewMatrices();
            }
        }
        public float Y
        {
            get { return Position.Y; }
            set
            {
                _position.Y = value;
                PositionTranslation = Matrix.CreateTranslation(new Vector3(-_position, 0));
                UpdateViewMatrices();
            }
        }

        private Vector2 _position;
        private float _angle, _zoom;

        public Matrix PositionTranslation
        {
            get;
            private set;
        }
        public Matrix RotationMatrix
        {
            get;
            private set;
        }
        public Matrix ScaleMatrix
        {
            get;
            private set;
        }
        public Matrix ViewMatrix
        {
            get;
            private set;
        }
        internal Matrix _screenTranslation, _invert;

        public void UpdateViewMatrices()
        {
            ViewMatrix = CreateCameraMatrix(PositionTranslation, RotationMatrix, ScaleMatrix, ScreenTranslation);
            _invert = Matrix.Invert(ViewMatrix);
        }

        public static Matrix CreateScreenTranslation(float width, float height) { return Matrix.CreateTranslation((width / 2), (height / 2), 0); }
        public static Matrix CreateProjection(float width, float height) { return Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1); }
        public static Matrix CreateCameraMatrix(Matrix position, Matrix rotationZ, Matrix scale, Matrix screen) { return (position * rotationZ * scale * screen); }

        public Camera(float angle = 0, float zoom = 1)
        {
            _position = Vector2.Zero;
            PositionTranslation = Matrix.CreateTranslation(new Vector3(-_position, 0));
            _angle = angle;
            RotationMatrix = Matrix.CreateRotationZ(-angle);
            _zoom = zoom;
            ScaleMatrix = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
            _screenTranslation = CreateScreenTranslation(Screen.ViewportWidth, Screen.ViewportHeight);
            Projection = CreateProjection(Screen.ViewportWidth, Screen.ViewportHeight);
            UpdateViewMatrices();
        }
        public Camera(Vector2 position, float angle = 0, float zoom = 1)
        {
            _position = position;
            PositionTranslation = Matrix.CreateTranslation(new Vector3(-_position, 0));
            _angle = angle;
            RotationMatrix = Matrix.CreateRotationZ(-angle);
            _zoom = zoom;
            ScaleMatrix = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
            _screenTranslation = CreateScreenTranslation(Screen.ViewportWidth, Screen.ViewportHeight);
            Projection = CreateProjection(Screen.ViewportWidth, Screen.ViewportHeight);
            UpdateViewMatrices();
        }

        public void UpdateMousePosition()
        {
            var mousePos = Mouse.Position.ToVector2();
            Vector2.Transform(ref mousePos, ref _invert, out mousePos);
            MousePosition = mousePos;
        }
        public Vector2 MousePosition { get; private set; }
    }
}