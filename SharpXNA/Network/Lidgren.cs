using SharpXNA.Collision;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SharpXNA
{
    public static class Network
    {
        public static NetPeerConfiguration DefaultConfiguration
        {
            get
            {
                var config = new NetPeerConfiguration("Game");
                //config.MaximumTransmissionUnit = (ushort.MaxValue / 8);
                config.AutoFlushSendQueue = true;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                config.DisableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
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
                return config;
            }
        }

        public static NetPeer Peer;
        
        public static bool IsNullOrServer { get { return ((Peer == null) || (Peer is NetServer)); } }
        public static bool IsServer { get { return ((Peer != null) && (Peer is NetServer)); } }
        public static bool IsClient { get { return ((Peer != null) && (Peer is NetClient)); } }

        public static NetPeerStatus State { get { if (Peer != null) return Peer.Status; else return NetPeerStatus.NotRunning; } }
        public static void StartHosting(NetPeerConfiguration config) { Peer = new NetServer(config); Peer.Start(); }
        public static void StartHosting(int port, int maxConnections) { var Configuration = DefaultConfiguration; Configuration.Port = port; Configuration.MaximumConnections = maxConnections; StartHosting(Configuration); }
        public static bool StopHosting() { if (IsServer && (State == NetPeerStatus.Running)) { Peer.Shutdown(string.Empty); return true; } else return false; }

        public static void Connect(string ip, int port, NetPeerConfiguration config, Packet packet) { Peer = new NetClient(config); Peer.Start(); packet.Identifier = Globe.Version; Peer.Connect(ip, port, packet.Construct()); }
        public static void Connect(string ip, int port, Packet packet) { Connect(ip, port, DefaultConfiguration, packet); }

        public static NetIncomingMessage ReadMessage() { if (State != NetPeerStatus.NotRunning) return Peer.ReadMessage(); else return null; }
        public static void Update()
        {
            NetIncomingMessage message;
            while ((message = ReadMessage()) != null)
            {
                if (message.MessageType == NetIncomingMessageType.Data) { if (OnData != null) OnData(message); }
                else if (message.MessageType == NetIncomingMessageType.ConnectionApproval) { if (OnConnectionApproval != null) OnConnectionApproval(message); }
                else if (message.MessageType == NetIncomingMessageType.StatusChanged) { if (OnStatusChanged != null) OnStatusChanged(message); }
                else if (message.MessageType == NetIncomingMessageType.ConnectionLatencyUpdated) { if (OnLatencyUpdated != null) OnLatencyUpdated(message); }
                else if (message.MessageType == NetIncomingMessageType.DiscoveryRequest) { if (OnDiscoveryRequest != null) OnDiscoveryRequest(message); }
                else if (message.MessageType == NetIncomingMessageType.DiscoveryResponse) { if (OnDiscoveryResponse != null) OnDiscoveryResponse(message); }
                else if (message.MessageType == NetIncomingMessageType.NatIntroductionSuccess) { if (OnNatIntroSuccess != null) OnNatIntroSuccess(message); }
                else if (message.MessageType == NetIncomingMessageType.Receipt) { if (OnReceipt != null) OnReceipt(message); }
                else if (message.MessageType == NetIncomingMessageType.UnconnectedData) { if (OnUnconnectedData != null) OnUnconnectedData(message); }
                else if (message.MessageType == NetIncomingMessageType.VerboseDebugMessage) { if (OnVerboseDebugmessage != null) OnVerboseDebugmessage(message); }
                else if (message.MessageType == NetIncomingMessageType.WarningMessage) { if (OnWarning != null) OnWarning(message); }
                else if (message.MessageType == NetIncomingMessageType.Error) { if (OnError != null) OnError(message); }
                else if (message.MessageType == NetIncomingMessageType.ErrorMessage) { if (OnErrormessage != null) OnErrormessage(message); }
                else if (message.MessageType == NetIncomingMessageType.DebugMessage) { if (OnDebugmessage != null) OnDebugmessage(message); }
            }
        }

        public delegate void messageRecieved(NetIncomingMessage message);
        public static event messageRecieved OnData, OnConnectionApproval, OnStatusChanged, OnLatencyUpdated, OnDiscoveryRequest, OnDiscoveryResponse,
            OnNatIntroSuccess, OnReceipt, OnUnconnectedData, OnVerboseDebugmessage, OnWarning, OnError, OnErrormessage, OnDebugmessage;

        public static void Send(Packet packet, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0) { Send(packet.Construct(), deliveryMethod, channel); }
        public static bool Send(Packet packet, NetConnection except, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0) { return Send(packet.Construct(), except, deliveryMethod, channel); }
        public static bool SendTo(Packet packet, NetConnection recipient, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0) { return SendTo(packet.Construct(), recipient, deliveryMethod, channel); }

        public static void Send(NetOutgoingMessage message, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0)
        { if (IsServer) { if (Peer.ConnectionsCount > 0) Peer.SendMessage(message, Peer.Connections, deliveryMethod, channel); } else if (IsClient) (Peer as NetClient).SendMessage(message, deliveryMethod, channel); }
        public static bool Send(NetOutgoingMessage message, NetConnection except, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0)
        { if (IsServer) { if (Peer.ConnectionsCount > 0) { (Peer as NetServer).SendToAll(message, except, deliveryMethod, channel); return true; } else return false; } else return false; }
        public static bool SendTo(NetOutgoingMessage message, NetConnection recipient, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0)
        { if (IsServer) { if (Peer.ConnectionsCount > 0) { (Peer as NetServer).SendMessage(message, recipient, deliveryMethod, channel); return true; } else return false; } else return false; }

        public static void Flush() { if (Peer != null) Peer.FlushSendQueue(); }
        public static void Shutdown(string reason = null) { if (Peer != null) { Peer.Shutdown(reason ?? string.Empty); Peer = null; } }

        #region Extensions
        public static void Write(this NetBuffer message, object variable)
        {
            if (variable is Array)
                for (var i = 0; i < (variable as Array).Length; i++)
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
                for (var i = 0; i < (variable as List<object>).Count; i++)
                    message.Write((variable as List<object>)[i]);
            else if (variable.GetType() == typeof(bool)) message.Write((bool)variable); // 1 byte, 8 bits
            else if (variable.GetType() == typeof(sbyte)) message.Write((sbyte)variable); // 1 byte, 8 bits
            else if (variable.GetType() == typeof(byte)) message.Write((byte)variable); // 1 byte, 8 bits
            else if (variable.GetType() == typeof(char)) message.Write((char)variable); // 2 bytes, 16 bits
            else if (variable.GetType() == typeof(short)) message.Write((short)variable); // 2 bytes, 16 bits
            else if (variable.GetType() == typeof(ushort)) message.Write((ushort)variable); // 2 bytes, 16 bits
            else if (variable.GetType() == typeof(float)) message.Write((float)variable); // 4 bytes, 32 bits
            else if (variable.GetType() == typeof(int)) message.Write((int)variable); // 4 bytes, 32 bits
            else if (variable.GetType() == typeof(uint)) message.Write((uint)variable); // 4 bytes, 32 bits
            else if (variable.GetType() == typeof(Vector2)) message.Write((Vector2)variable); // 8 bytes, 64 bits
            else if (variable.GetType() == typeof(Point)) message.Write((Point)variable); // 8 bytes, 64 bits
            else if (variable.GetType() == typeof(double)) message.Write((double)variable); // 8 bytes, 64 bits
            else if (variable.GetType() == typeof(long)) message.Write((long)variable); // 8 bytes, 64 bits
            else if (variable.GetType() == typeof(ulong)) message.Write((ulong)variable); // 8 bytes, 64 bits
            else if (variable.GetType() == typeof(Vector3)) message.Write((Vector3)variable); // 12 bytes, 96 bits
            else if (variable.GetType() == typeof(Vector4)) message.Write((Vector4)variable); // 16 bytes, 128 bits
            else if (variable.GetType() == typeof(Line)) message.Write((Line)variable); // 16 bytes, 128 bits
            else if (variable.GetType() == typeof(Rectangle)) message.Write((Rectangle)variable); // 16 bytes, 128 bits
            else if (variable.GetType() == typeof(Quaternion)) message.Write((Quaternion)variable, 24); // 16 bytes, 128 bits
            else if (variable.GetType() == typeof(BoundingSphere)) message.Write((BoundingSphere)variable); // 16 bytes, 128 bits
            else if (variable.GetType() == typeof(Matrix)) message.Write((Matrix)variable); // 28 bytes, 224 bits
            else if (variable.GetType() == typeof(string)) message.Write((string)variable); // ~~
            else if (variable.GetType() == typeof(Polygon)) message.Write((Polygon)variable); // ~~
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

        public static void Write(this NetBuffer message, Quaternion quaternion, int bits) { if (quaternion.X > 1.0f) quaternion.X = 1.0f; if (quaternion.Y > 1.0f) quaternion.Y = 1.0f; if (quaternion.Z > 1.0f) quaternion.Z = 1.0f; if (quaternion.W > 1.0f) quaternion.W = 1.0f; if (quaternion.X < -1.0f) quaternion.X = -1.0f; if (quaternion.Y < -1.0f) quaternion.Y = -1.0f; if (quaternion.Z < -1.0f) quaternion.Z = -1.0f; if (quaternion.W < -1.0f) quaternion.W = -1.0f; message.WriteSignedSingle(quaternion.X, bits); message.WriteSignedSingle(quaternion.Y, bits); message.WriteSignedSingle(quaternion.Z, bits); message.WriteSignedSingle(quaternion.W, bits); }
        public static Quaternion ReadQuaternion(this NetBuffer message, int bits) { return new Quaternion(message.ReadSignedSingle(bits), message.ReadSignedSingle(bits), message.ReadSignedSingle(bits), message.ReadSignedSingle(bits)); }

        public static void Write(this NetBuffer message, Matrix matrix) { var quaternion = Quaternion.CreateFromRotationMatrix(matrix); Write(message, quaternion, 24); message.Write(matrix.M41); message.Write(matrix.M42); message.Write(matrix.M43); }
        public static Matrix ReadMatrix(this NetBuffer message) { var quaternion = ReadQuaternion(message, 24); var Matrix = Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(quaternion); Matrix.M41 = message.ReadFloat(); Matrix.M42 = message.ReadFloat(); Matrix.M43 = message.ReadFloat(); return Matrix; }

        public static void Write(this NetBuffer message, Polygon polygon) { message.Write((byte)polygon.Lines.Length); message.Write(polygon.position); message.Write(polygon.angle); for (int i = 0; i < polygon.Lines.Length; i++) message.Write(polygon.Lines[i]); }
        public static Polygon ReadPolygon(this NetBuffer message) { var polygon = new Polygon(new Line[message.ReadByte()]) { position = message.ReadVector2(), angle = message.ReadFloat() }; for (int i = 0; i < polygon.Lines.Length; i++) polygon.Lines[i] = ReadLine(message); return polygon; }
        #endregion

        public struct Packet
        {
            public object Identifier;
            
            public object[] Data;
            internal List<object> dataList;

            public Packet(object identifier, params object[] data) { Identifier = identifier; Data = data; dataList = null; }

            public NetOutgoingMessage Construct()
            {
                if (Peer != null)
                {
                    NetOutgoingMessage message = Peer.CreateMessage();
                    if (Identifier != null) message.Write(Identifier);
                    if ((Data != null) && (Data.Length > 0)) message.Write(Data);
                    if (dataList != null) message.Write(dataList);
                    return message;
                }
                else return null;
            }
            public Packet Clone() { return Clone(Identifier); }
            public Packet Clone(object identifier)
            {
                Packet packet = new Packet(identifier);
                packet.Data = new object[Data.Length];
                for (int i = 0; i < Data.Length; i++) packet.Data[i] = Data[i];
                if (dataList != null)
                {
                    packet.dataList = new List<object>(dataList.Count);
                    for (int i = 0; i < dataList.Count; i++) packet.dataList.Add(dataList[i]);
                }
                return packet;
            }

            public void Set(params object[] data) { Data = data; dataList = null; }
            public void Add(params object[] data) { if (dataList == null) dataList = new List<object>(data.Length); dataList.AddRange(data); }

            public void Send(NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0) { Network.Send(this, deliveryMethod, channel); }
            public bool Send(NetConnection except, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0) { return Network.Send(this, except, deliveryMethod, channel); }
            public bool SendTo(NetConnection recipient, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered, int channel = 0) { return Network.SendTo(this, recipient, deliveryMethod, channel); }
        }
    }
}