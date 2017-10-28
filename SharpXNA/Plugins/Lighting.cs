using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpXNA.Collision;
using System;
using System.Collections.Generic;

namespace SharpXNA.Plugins
{
    public class Lighting
    {
        public const float PxOffset = .001f;
        internal static VertexPositionTexture[] vertices;
        internal static short[] indices;

        public RenderTarget2D ColorMap, LightMap, BlurMap;
        internal Effect CombineEffect;

        public List<PointLight> Lights { get; private set; }

        static Lighting()
        {
            vertices = new VertexPositionTexture[] { new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)), new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)), new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)), new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)) };
            indices = new short[] { 0, 1, 2, 2, 3, 0 };
        }
        public Lighting(Effect combineEffect)
        {
            ChangeResolution();
            CombineEffect = combineEffect;
            Lights = new List<PointLight>();
        }

        public void Draw()
        {
            CombineEffect.Parameters["colorMap"].SetValue(ColorMap);
            CombineEffect.Parameters["lightMap"].SetValue(LightMap);
            CombineEffect.Techniques[0].Passes[0].Apply();
            Engine.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
        }

        public void ChangeResolution()
        {
            ColorMap = new RenderTarget2D(Engine.GraphicsDevice, Screen.ViewportWidth, Screen.ViewportHeight, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
            LightMap = new RenderTarget2D(Engine.GraphicsDevice, Screen.ViewportWidth, Screen.ViewportHeight, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
            BlurMap = new RenderTarget2D(Engine.GraphicsDevice, Screen.ViewportWidth, Screen.ViewportHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }
        public void Blur(float strength, Effect blurEffect, RenderTarget2D target)
        {
            blurEffect.Parameters["renderTargetSize"].SetValue(new Vector2(target.Width, target.Height));
            blurEffect.Parameters["blur"].SetValue(strength);
            Engine.GraphicsDevice.SetRenderTarget(BlurMap);
            Engine.GraphicsDevice.Clear(Color.Transparent);
            blurEffect.Parameters["InputTexture"].SetValue(target);
            blurEffect.Techniques[0].Passes[0].Apply();
            Engine.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
            Engine.GraphicsDevice.SetRenderTarget(target);
            Engine.GraphicsDevice.Clear(Color.Transparent);
            blurEffect.Parameters["InputTexture"].SetValue(BlurMap);
            blurEffect.Techniques[0].Passes[1].Apply();
            Engine.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
        }
    }

    public class PointLight
    {
        private Vector2 _oldPosition;
        private Color _color;
        private Vector3 _poweredColor;
        private float _radius, _power;
        private HashSet<Line> _occluders;
        private VertexPositionTexture[] _vertices;
        private short[] _indeces;
        internal List<EndPoint> _endpoints;
        internal List<Segment> _segments;

        private float _lowFOV, _highFOV, _angle, _fov;
        public float Angle
        {
            get { return _angle; }
            set
            {
                if ((_fov != 0) && (_angle != value))
                {
                    _angle = value;
                    var fov = (_fov / 2);
                    _lowFOV = MathHelper.WrapAngle(_angle - fov);
                    _highFOV = MathHelper.WrapAngle(_angle + fov);
                    ClearOccluders(false);
                    AddOccluders(_occluders);
                }
            }
        }
        public float FOV
        {
            get { return _fov; }
            set
            {
                _fov = value;
                if (_fov != 0)
                {
                    var fov = (_fov / 2);
                    _lowFOV = MathHelper.WrapAngle(_angle - fov);
                    _highFOV = MathHelper.WrapAngle(_angle + fov);
                    ClearOccluders(false);
                    AddOccluders(_occluders);
                }
            }
        }

        public Vector2 Position;
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                _poweredColor = (_color.ToVector3() * _power);
            }
        }
        public float Power
        {
            get { return _power; }
            set
            {
                _power = value;
                _poweredColor = (_color.ToVector3() * _power);
            }
        }

        public PointLight(Vector2 position, float radius) : this(position, radius, Color.White, 1) { }
        public PointLight(Vector2 position, float radius, Color color) : this(position, radius, color, 1) { }
        public PointLight(Vector2 position, float radius, Color color, float power)
        {
            Position = position;
            Radius = radius;
            _color = color;
            Power = power;
            _occluders = new HashSet<Line>();
            _segments = new List<Segment>();
            _endpoints = new List<EndPoint>();
            ClearOccluders();
        }

        public void AddOccluder(Line line)
        {
            _occluders.Add(line);
            if (!(((Vector2.Distance(line.Start, Position) <= Radius) || (Vector2.Distance(line.End, Position) <= Radius)) || line.Intersects(_segments[0].Line) || line.Intersects(_segments[1].Line) || line.Intersects(_segments[2].Line) || line.Intersects(_segments[3].Line)))
                return;
            if (_fov != 0)
            {
                var fov = (_fov / 2);
                bool iS4b = false, iS5b = false;
                Vector2 iS4i = Vector2.Zero, iS5i = Vector2.Zero;
                if (iS4b = _segments[4].Line.Intersects(line, ref iS4i))
                {
                    Mathf.Move(ref iS4i, Angle, -Lighting.PxOffset);
                    _segments[4].Line.End = iS4i;
                }
                if (iS5b = _segments[5].Line.Intersects(line, ref iS5i))
                {
                    Mathf.Move(ref iS5i, Angle, -Lighting.PxOffset);
                    _segments[5].Line.End = iS5i;
                }
                if (!(((Mathf.AngleDifference(Mathf.Angle(Position, line.Start), Angle) <= fov) || (Mathf.AngleDifference(Mathf.Angle(Position, line.End), Angle) <= fov)) ||
                    iS4b || iS5b))
                    return;
            }
            var segment = new Segment(line);
            segment.P1 = new EndPoint(segment);
            segment.P2 = new EndPoint(segment);
            _segments.Add(segment);
            _endpoints.Add(segment.P1);
            _endpoints.Add(segment.P2);
        }
        private void AddOccluderNoFOVCheck(Line line)
        {
            var segment = new Segment(line);
            segment.P1 = new EndPoint(segment);
            segment.P2 = new EndPoint(segment);
            _segments.Add(segment);
            _endpoints.Add(segment.P1);
            _endpoints.Add(segment.P2);
        }
        private void InsertOccluderNoFOVCheck(int index, Line line)
        {
            var segment = new Segment(line);
            segment.P1 = new EndPoint(segment);
            segment.P2 = new EndPoint(segment);
            _segments.Insert(index, segment);
            _endpoints.Add(segment.P1);
            _endpoints.Add(segment.P2);
        }
        public void AddOccluders(IEnumerable<Line> lines)
        {
            foreach (var line in lines)
                AddOccluder(line);
        }
        public void AddOccluders(IEnumerable<Polygon> obstacles)
        {
            foreach (var obstacle in obstacles)
                AddOccluders(obstacle.Lines);
        }
        public void RemoveOccluder(Line line)
        {
            _occluders.RemoveWhere(x => (x == line));
            ClearOccluders(false);
            AddOccluders(_occluders);
        }
        public void ClearOccluders(bool clearStored = true)
        {
            if (clearStored)
                _occluders.Clear();
            _segments.Clear();
            _endpoints.Clear();
            AddOccluderNoFOVCheck(new Line(new Vector2(Position.X - _radius, Position.Y - _radius), new Vector2(Position.X + _radius, Position.Y - _radius)));
            AddOccluderNoFOVCheck(new Line(new Vector2(Position.X - _radius, Position.Y + _radius), new Vector2(Position.X + _radius, Position.Y + _radius)));
            AddOccluderNoFOVCheck(new Line(new Vector2(Position.X - _radius, Position.Y - _radius), new Vector2(Position.X - _radius, Position.Y + _radius)));
            AddOccluderNoFOVCheck(new Line(new Vector2(Position.X + _radius, Position.Y - _radius), new Vector2(Position.X + _radius, Position.Y + _radius)));
            if (_fov != 0)
            {
                var fov = (_fov / 2);
                Line start = new Line(Mathf.Move(Position, Angle, -Lighting.PxOffset), Mathf.Move(Position, _lowFOV, Radius)),
                    end = new Line(Mathf.Move(Position, Angle, -Lighting.PxOffset), Mathf.Move(Position, _highFOV, Radius));
                AddOccluderNoFOVCheck(start);
                AddOccluderNoFOVCheck(end);
            }
        }

        public void Render(Effect lightEffect) { Render(lightEffect, null, Engine.Viewport); }
        public void Render(Effect lightEffect, Camera camera, Viewport gameViewport)
        {
            if (_power > 0)
            {
                if (_oldPosition != Position)
                {
                    _oldPosition = Position;
                    ClearOccluders(false);
                    AddOccluders(_occluders);
                }
                var position = Position;
                var encounters = new Vector2[_endpoints.Count * 2];
                var open = new LinkedList<Segment>();
                if (camera != null)
                {
                    var cam = new Vector2(((camera.X * camera.Zoom) - (gameViewport.Width / 2f)), ((camera.Y * camera.Zoom) - (gameViewport.Height / 2f)));
                    position = new Vector2(((Position.X * camera.Zoom) - cam.X), ((Position.Y * camera.Zoom) - cam.Y));
                    foreach (var segment in _segments)
                    {
                        segment.P1.Position = new Vector2(((segment.Line.Start.X * camera.Zoom) - cam.X), ((segment.Line.Start.Y * camera.Zoom) - cam.Y));
                        segment.P2.Position = new Vector2(((segment.Line.End.X * camera.Zoom) - cam.X), ((segment.Line.End.Y * camera.Zoom) - cam.Y));
                        segment.P1.Angle = (float)Math.Atan2(segment.P1.Position.Y - position.Y, segment.P1.Position.X - position.X);
                        segment.P2.Angle = (float)Math.Atan2(segment.P2.Position.Y - position.Y, segment.P2.Position.X - position.X);
                        var dAngle = (segment.P2.Angle - segment.P1.Angle);
                        if (dAngle <= -MathHelper.Pi)
                            dAngle += MathHelper.TwoPi;
                        else if (dAngle > MathHelper.Pi)
                            dAngle -= MathHelper.TwoPi;
                        segment.P2.Begin = !(segment.P1.Begin = (dAngle > 0));
                    }
                }
                else
                    foreach (var segment in _segments)
                    { // NOTE: future optimization: we could record the quadrant and the y/x or x/y ratio, and sort by (quadrant, ratio), instead of calling atan2. See <https://github.com/mikolalysenko/compare-slope> for a library that does this.
                        segment.P1.Position = segment.Line.Start;
                        segment.P2.Position = segment.Line.End;
                        segment.P1.Angle = (float)Math.Atan2(segment.P1.Position.Y - position.Y, segment.P1.Position.X - position.X);
                        segment.P2.Angle = (float)Math.Atan2(segment.P2.Position.Y - position.Y, segment.P2.Position.X - position.X);
                        var dAngle = (segment.P2.Angle - segment.P1.Angle);
                        if (dAngle <= -MathHelper.Pi)
                            dAngle += MathHelper.TwoPi;
                        else if (dAngle > MathHelper.Pi)
                            dAngle -= MathHelper.TwoPi;
                        segment.P2.Begin = !(segment.P1.Begin = (dAngle > 0));
                    }
                _endpoints.Sort(new EndPointComparer());
                var currentAngle = 0f;
                for (var pass = 0; pass < 2; pass++)
                    for (int i = 0, j = 0; i < _endpoints.Count; i++)
                    {
                        var p = _endpoints[i];
                        var oldSegment = ((open.Count > 0) ? open.First.Value : null);
                        if (p.Begin)
                        {
                            var node = open.First;
                            while ((node != null) && SegmentInFrontOf(p.Segment, node.Value, position))
                                node = node.Next;
                            if (node == null)
                                open.AddLast(p.Segment);
                            else
                                open.AddBefore(node, p.Segment);
                        }
                        else
                            open.Remove(p.Segment);
                        var newSegment = ((open.Count > 0) ? open.First.Value : null);
                        if (oldSegment != newSegment)
                        {
                            if (pass == 1)
                            {
                                var p2 = new Vector2(position.X + (float)Math.Cos(currentAngle), position.Y + (float)Math.Sin(currentAngle));
                                if (oldSegment != null)
                                {
                                    encounters[j++] = VectorMath.LineLineIntersection(oldSegment.P1.Position, oldSegment.P2.Position, position, p2);
                                    p2.X = position.X + (float)Math.Cos(p.Angle);
                                    p2.Y = position.Y + (float)Math.Sin(p.Angle);
                                    encounters[j++] = VectorMath.LineLineIntersection(oldSegment.P1.Position, oldSegment.P2.Position, position, p2);
                                }
                                else
                                {
                                    Vector2 p3 = new Vector2((position.X + (float)Math.Cos(currentAngle) * _radius * 2), (position.Y + (float)Math.Sin(currentAngle) * _radius * 2));
                                    float pAngleCos = (float)Math.Cos(p.Angle), pAngleSin = (float)Math.Sin(p.Angle);
                                    Vector2 p4 = new Vector2((position.X + pAngleCos * _radius * 2), (position.Y + pAngleSin * _radius * 2));
                                    encounters[j++] = VectorMath.LineLineIntersection(p3, p4, position, p2);
                                    p2.X = (position.X + pAngleCos);
                                    p2.Y = (position.Y + pAngleSin);
                                    encounters[j++] = VectorMath.LineLineIntersection(p3, p4, position, p2);
                                }
                            }
                            currentAngle = p.Angle;
                        }
                    }
                _vertices = new VertexPositionTexture[encounters.Length + 1];
                _indeces = new short[(int)(encounters.Length * 1.5)];
                _vertices[0] = new VertexPositionTexture(new Vector3(position.X, position.Y, 0), position);
                for (int i = 1, k = 0; i < encounters.Length; i += 2, k += 3)
                {
                    Vector2 v1 = encounters[i - 1], v2 = encounters[i];
                    _vertices[i] = new VertexPositionTexture(new Vector3(v1.X, v1.Y, 0), v1);
                    _vertices[i + 1] = new VertexPositionTexture(new Vector3(v2.X, v2.Y, 0), v2);
                    _indeces[k] = 0;
                    _indeces[k + 1] = (short)(i + 1);
                    _indeces[k + 2] = (short)i;
                }
                ProjectVertices(_vertices, Screen.BackBufferWidth, Screen.BackBufferHeight);
                lightEffect.Parameters["lightSource"].SetValue(position);
                lightEffect.Parameters["lightColor"].SetValue(_poweredColor);
                lightEffect.Parameters["lightRadius"].SetValue(_radius);
                lightEffect.Techniques[0].Passes[0].Apply();
                Engine.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indeces, 0, _indeces.Length / 3);
            }
        }

        private bool SegmentInFrontOf(Segment a, Segment b, Vector2 relativeTo)
        {
            bool a1 = VectorMath.LeftOf(a.P2.Position, a.P1.Position, VectorMath.Interpolate(b.P1.Position, b.P2.Position));
            bool a2 = VectorMath.LeftOf(a.P2.Position, a.P1.Position, VectorMath.Interpolate(b.P2.Position, b.P1.Position));
            bool a3 = VectorMath.LeftOf(a.P2.Position, a.P1.Position, relativeTo);
            if (a1 == a2)
                return (a2 == a3);
            bool b1 = VectorMath.LeftOf(b.P2.Position, b.P1.Position, VectorMath.Interpolate(a.P1.Position, a.P2.Position));
            bool b2 = VectorMath.LeftOf(b.P2.Position, b.P1.Position, VectorMath.Interpolate(a.P2.Position, a.P1.Position));
            bool b3 = VectorMath.LeftOf(b.P2.Position, b.P1.Position, relativeTo);
            if (b1 == b2)
                return (b2 != b3);
            return false;
        }
        private void ProjectVertices(VertexPositionTexture[] vertices, float screenWidth, float screenHeight)
        {
            float halfScreenWidth = (screenWidth / 2), halfScreenHeight = (screenHeight / 2);
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position.X = (vertices[i].Position.X / halfScreenWidth) - 1;
                vertices[i].Position.Y = ((screenHeight - vertices[i].Position.Y) / halfScreenHeight) - 1;
            }
        }
    }

    public static class VectorMath
    {
        public static Vector2 LineLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            var s = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
            return new Vector2(p1.X + s * (p2.X - p1.X), p1.Y + s * (p2.Y - p1.Y));
        }
        public static bool LeftOf(Vector2 p1, Vector2 p2, Vector2 point) { return (((p2.X - p1.X) * (point.Y - p1.Y) - (p2.Y - p1.Y) * (point.X - p1.X)) < 0); }
        public static Vector2 Interpolate(Vector2 p, Vector2 q) { return new Vector2(p.X * .99f + q.X * .01f, p.Y * .99f + q.Y * .01f); }
    }

    internal class Segment
    {
        internal Line Line;
        internal EndPoint P1, P2;

        internal Segment(Line line) { Line = line; }
    }
    internal class EndPoint
    {
        internal Vector2 Position;
        internal bool Begin;
        internal Segment Segment;
        internal float Angle;

        internal EndPoint(Segment segment) { Segment = segment; }
    }
    internal class EndPointComparer : IComparer<EndPoint>
    {
        public int Compare(EndPoint a, EndPoint b)
        {
            if (a.Angle > b.Angle) return 1;
            if (a.Angle < b.Angle) return -1;
            if (a.Begin)
            {
                if (!b.Begin)
                    return -1;
            }
            else if (b.Begin)
                return 1;
            return 0;
        }
    }
}