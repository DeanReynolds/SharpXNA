using SharpXNA.Collision;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SharpXNA
{
    public static class Network
    {
        public static NetPeer Peer;

        public static bool IsNullOrServer => ((Peer == null) || (Peer is NetServer));
        public static bool IsServer => (Peer is NetServer);
        public static bool IsClient => (Peer is NetClient);
        public static NetUPnP UPnP => Peer.UPnP;

        public static NetPeerStatus State => Peer?.Status ?? NetPeerStatus.NotRunning;
        public static void StartHosting(NetPeerConfiguration config)
        {
            Peer = new NetServer(config);
            Peer.Start();
            try
            {
                if (Peer.UPnP != null)
                    Peer.UPnP.ForwardPort(config.Port, $"{((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false))?.Title ?? "Game"} {((AssemblyVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyVersionAttribute), false))?.Version ?? "1.0.0.0"}");
            }
            catch { }
        }
        public static void StartHosting(int port, int maxConnections)
        {
            var config = DefaultConfiguration;
            config.Port = port;
            config.MaximumConnections = maxConnections;
            StartHosting(config);
        }

        public static void Connect(string ip, int port, NetPeerConfiguration config, params object[] data)
        {
            Peer = new NetClient(config);
            Peer.Start();
            Peer.Connect(ip, port, new Packet(data)._message);
        }
        public static void Connect(string ip, int port, params object[] data)
        {
            Peer = new NetClient(DefaultConfiguration);
            Peer.Start();
            Peer.Connect(ip, port, new Packet(data)._message);
        }

        public static NetPeerConfiguration DefaultConfiguration
        {
            get
            {
                var config = new NetPeerConfiguration("Game") { AutoFlushSendQueue = true };
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
                config.DisableMessageType(NetIncomingMessageType.DebugMessage);
                config.DisableMessageType(NetIncomingMessageType.DiscoveryRequest);
                config.DisableMessageType(NetIncomingMessageType.DiscoveryResponse);
                config.DisableMessageType(NetIncomingMessageType.Error);
                config.DisableMessageType(NetIncomingMessageType.ErrorMessage);
                config.DisableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
                config.DisableMessageType(NetIncomingMessageType.Receipt);
                config.DisableMessageType(NetIncomingMessageType.UnconnectedData);
                config.DisableMessageType(NetIncomingMessageType.VerboseDebugMessage);
                config.DisableMessageType(NetIncomingMessageType.WarningMessage);
                config.EnableUPnP = true;
                return config;
            }
        }

        public static NetIncomingMessage ReadMessage()
        {
            return ((State != NetPeerStatus.NotRunning) ? Peer.ReadMessage() : null);
        }
        public static void Update()
        {
            NetIncomingMessage message;
            while ((message = ReadMessage()) != null)
                if (message.MessageType == NetIncomingMessageType.Data) OnData?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.ConnectionLatencyUpdated) OnLatencyUpdated?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.StatusChanged) OnStatusChanged?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.ConnectionApproval) OnConnectionApproval?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.UnconnectedData) OnUnconnectedData?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.DiscoveryRequest) OnDiscoveryRequest?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.DiscoveryResponse) OnDiscoveryResponse?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.NatIntroductionSuccess) OnNatIntroSuccess?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.Receipt) OnReceipt?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.VerboseDebugMessage) OnVerboseDebugmessage?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.WarningMessage) OnWarning?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.Error) OnError?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.ErrorMessage) OnErrormessage?.Invoke(message);
                else if (message.MessageType == NetIncomingMessageType.DebugMessage) OnDebugmessage?.Invoke(message);
        }

        public delegate void MessageRecieved(NetIncomingMessage message);
        public static event MessageRecieved OnData, OnConnectionApproval, OnStatusChanged, OnLatencyUpdated, OnDiscoveryRequest, OnDiscoveryResponse, OnNatIntroSuccess, OnReceipt, OnUnconnectedData, OnVerboseDebugmessage, OnWarning, OnError, OnErrormessage, OnDebugmessage;

        public static void Flush()
        {
            Peer?.FlushSendQueue();
        }
        public static void Shutdown(string reason = null)
        {
            if (Peer == null)
                return;
            Peer.Shutdown(reason ?? string.Empty);
            Peer = null;
        }

        #region Extensions
        public static void Write(this NetBuffer message, object variable)
        {
            var c = variable as Array;
            if (c != null)
                for (var i = 0; i < c.Length; i++)
                {
                    if (variable is bool[]) message.Write((variable as bool[])[i]);
                    else if (variable is sbyte[]) message.Write((variable as sbyte[])[i]);
                    else if (variable is byte[]) message.Write((variable as byte[])[i]);
                    else if (variable is char[]) message.Write((variable as char[])[i]);
                    else if (variable is short[]) message.Write((variable as short[])[i]);
                    else if (variable is ushort[]) message.Write((variable as ushort[])[i]);
                    else if (variable is float[]) message.Write((variable as float[])[i]);
                    else if (variable is int[]) message.Write((variable as int[])[i]);
                    else if (variable is uint[]) message.Write((variable as uint[])[i]);
                    else if (variable is Vector2[]) message.Write((variable as Vector2[])[i]);
                    else if (variable is Point[]) message.Write((variable as Point[])[i]);
                    else if (variable is double[]) message.Write((variable as double[])[i]);
                    else if (variable is long[]) message.Write((variable as long[])[i]);
                    else if (variable is ulong[]) message.Write((variable as ulong[])[i]);
                    else if (variable is Vector3[]) message.Write((variable as Vector3[])[i]);
                    else if (variable is Vector4[]) message.Write((variable as Vector4[])[i]);
                    else if (variable is Line[]) message.Write((variable as Line[])[i]);
                    else if (variable is Rectangle[]) message.Write((variable as Rectangle[])[i]);
                    else if (variable is Quaternion[]) message.Write((variable as Quaternion[])[i]);
                    else if (variable is BoundingSphere[]) message.Write((variable as BoundingSphere[])[i]);
                    else if (variable is Matrix[]) message.Write((variable as Matrix[])[i]);
                    else if (variable is string[]) message.Write((variable as string[])[i]);
                    else if (variable is Polygon[]) message.Write((variable as Polygon[])[i]);
                    else message.Write((variable as object[])[i]);
                }
            else if (variable is List<object>)
                for (var i = 0; i < ((List<object>)variable).Count; i++)
                    message.Write(((List<object>)variable)[i]);
            else if (variable is bool) message.Write((bool)variable);
            else if (variable is sbyte) message.Write((sbyte)variable);
            else if (variable is byte) message.Write((byte)variable);
            else if (variable is char) message.Write((char)variable);
            else if (variable is short) message.Write((short)variable);
            else if (variable is ushort) message.Write((ushort)variable);
            else if (variable is float) message.Write((float)variable);
            else if (variable is int) message.Write((int)variable);
            else if (variable is uint) message.Write((uint)variable);
            else if (variable is Vector2) message.Write((Vector2)variable);
            else if (variable is Point) message.Write((Point)variable);
            else if (variable is double) message.Write((double)variable);
            else if (variable is long) message.Write((long)variable);
            else if (variable is ulong) message.Write((ulong)variable);
            else if (variable is Vector3) message.Write((Vector3)variable);
            else if (variable is Vector4) message.Write((Vector4)variable);
            else if (variable is Line) message.Write((Line)variable);
            else if (variable is Rectangle) message.Write((Rectangle)variable);
            else if (variable is Quaternion) message.Write((Quaternion)variable, 24);
            else if (variable is BoundingSphere) message.Write((BoundingSphere)variable);
            else if (variable is Matrix) message.Write((Matrix)variable);
            else if (variable is string) message.Write((string)variable);
            else if (variable is Polygon) message.Write((Polygon)variable);
        }

        public static void Write(this NetBuffer message, Point point) { message.Write(point.X); message.Write(point.Y); }
        public static Point ReadPoint(this NetBuffer message) { return new Point(message.ReadInt32(), message.ReadInt32()); }

        public static void Write(this NetBuffer message, Vector2 vector2) { message.Write(vector2.X); message.Write(vector2.Y); }
        public static Vector2 ReadVector2(this NetBuffer message) { return new Vector2(message.ReadFloat(), message.ReadFloat()); }

        public static void Write(this NetBuffer message, Vector3 vector3) { message.Write(vector3.X); message.Write(vector3.Y); message.Write(vector3.Z); }
        public static Vector3 ReadVector3(this NetBuffer message) { return new Vector3(ReadVector2(message), message.ReadFloat()); }

        public static void Write(this NetBuffer message, Vector4 vector4) { message.Write(vector4.X); message.Write(vector4.Y); message.Write(vector4.Z); message.Write(vector4.W); }
        public static Vector4 ReadVector4(this NetBuffer message) { return new Vector4(ReadVector3(message), message.ReadFloat()); }

        public static void Write(this NetBuffer message, Line line) { message.Write(line.Start); message.Write(line.End); }
        public static Line ReadLine(this NetBuffer message) { return new Line(ReadVector2(message), ReadVector2(message)); }

        public static void Write(this NetBuffer message, Rectangle rectangle) { message.Write(rectangle.X); message.Write(rectangle.Y); message.Write(rectangle.Width); message.Write(rectangle.Height); }
        public static Rectangle ReadRectangle(this NetBuffer message) { return new Rectangle(message.ReadInt32(), message.ReadInt32(), message.ReadInt32(), message.ReadInt32()); }

        public static void Write(this NetBuffer message, BoundingSphere boundingSphere) { message.Write(boundingSphere.Center); message.Write(boundingSphere.Radius); }
        public static BoundingSphere ReadBoundingSphere(this NetBuffer message) { return new BoundingSphere(ReadVector3(message), message.ReadFloat()); }

        public static void Write(this NetBuffer message, Quaternion quaternion, int bits)
        {
            if (quaternion.X > 1.0f) quaternion.X = 1.0f;
            if (quaternion.Y > 1.0f) quaternion.Y = 1.0f;
            if (quaternion.Z > 1.0f) quaternion.Z = 1.0f;
            if (quaternion.W > 1.0f) quaternion.W = 1.0f;
            if (quaternion.X < -1.0f) quaternion.X = -1.0f;
            if (quaternion.Y < -1.0f) quaternion.Y = -1.0f;
            if (quaternion.Z < -1.0f) quaternion.Z = -1.0f;
            if (quaternion.W < -1.0f) quaternion.W = -1.0f;
            message.WriteSignedSingle(quaternion.X, bits);
            message.WriteSignedSingle(quaternion.Y, bits);
            message.WriteSignedSingle(quaternion.Z, bits);
            message.WriteSignedSingle(quaternion.W, bits);
        }
        public static Quaternion ReadQuaternion(this NetBuffer message, int bits) { return new Quaternion(message.ReadSignedSingle(bits), message.ReadSignedSingle(bits), message.ReadSignedSingle(bits), message.ReadSignedSingle(bits)); }

        public static void Write(this NetBuffer message, Matrix matrix) { var quaternion = Quaternion.CreateFromRotationMatrix(matrix); Write(message, quaternion, 24); message.Write(matrix.M41); message.Write(matrix.M42); message.Write(matrix.M43); }
        public static Matrix ReadMatrix(this NetBuffer message) { var quaternion = ReadQuaternion(message, 24); var matrix = Matrix.CreateFromQuaternion(quaternion); matrix.M41 = message.ReadFloat(); matrix.M42 = message.ReadFloat(); matrix.M43 = message.ReadFloat(); return matrix; }

        public static void Write(this NetBuffer message, Polygon polygon) { message.Write((byte)polygon.Lines.Length); message.Write(polygon.position); message.Write(polygon.angle); foreach (var t in polygon.Lines) message.Write(t); }
        public static Polygon ReadPolygon(this NetBuffer message) { var polygon = new Polygon(new Line[message.ReadByte()]) { position = message.ReadVector2(), angle = message.ReadFloat() }; for (var i = 0; i < polygon.Lines.Length; i++) polygon.Lines[i] = ReadLine(message); return polygon; }
        #endregion

        public class Packet
        {
            internal NetOutgoingMessage _message;

            public Packet(params object[] data) { Set(data); }

            public void Add(params object[] data)
            {
                foreach (var d in data)
                    _message.Write(d);
            }
            public void Set(params object[] data)
            {
                _message = Peer.CreateMessage();
                foreach (var d in data)
                    _message.Write(d);
            }

            public int LengthBytes => _message.LengthBytes;
            public int LengthBits => _message.LengthBits;

            public void WriteRangedSingle(float value, float min, float max, int numberOfBits) => _message.WriteRangedSingle(value, min, max, numberOfBits);
            public int WriteRangedInteger(int min, int max, int value) => _message.WriteRangedInteger(min, max, value);

            public void Send(NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0)
            {
                if (IsServer)
                    ((NetServer)Peer).SendMessage(_message, Peer.Connections, deliveryMethod, channel);
                else if (IsClient)
                    ((NetClient)Peer).SendMessage(_message, deliveryMethod, channel);
            }
            public void Send(NetConnection except, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0)
            {
                if (IsServer)
                    ((NetServer)Peer).SendToAll(_message, except, deliveryMethod, channel);
            }
            public void SendTo(NetConnection recipient, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0)
            {
                if (IsServer)
                    if (recipient == null)
                        ((NetServer)Peer).SendMessage(_message, Peer.Connections, deliveryMethod, channel);
                    else
                        ((NetServer)Peer).SendMessage(_message, recipient, deliveryMethod, channel);
            }
        }
    }
}