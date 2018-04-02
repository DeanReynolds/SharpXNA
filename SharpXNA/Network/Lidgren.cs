using SharpXNA.Collision;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            var hailMessage = Peer.CreateMessage();
            foreach (var o in data)
                hailMessage.Write(o);
            Peer.Connect(ip, port, hailMessage);
        }
        public static void Connect(string ip, int port, params object[] data)
        {
            Peer = new NetClient(DefaultConfiguration);
            Peer.Start();
            var hailMessage = Peer.CreateMessage();
            foreach (var o in data)
                hailMessage.Write(o);
            Peer.Connect(ip, port, hailMessage);
        }

        public static NetPeerConfiguration DefaultConfiguration
        {
            get
            {
                var config = new NetPeerConfiguration("Game") { AutoFlushSendQueue = true };
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                config.DisableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
                config.DisableMessageType(NetIncomingMessageType.DebugMessage);
                config.DisableMessageType(NetIncomingMessageType.DiscoveryRequest);
                config.DisableMessageType(NetIncomingMessageType.DiscoveryResponse);
                config.DisableMessageType(NetIncomingMessageType.Error);
                config.DisableMessageType(NetIncomingMessageType.ErrorMessage);
                config.DisableMessageType(NetIncomingMessageType.Receipt);
                config.DisableMessageType(NetIncomingMessageType.WarningMessage);
                config.EnableUPnP = true;
                return config;
            }
        }

        public static NetIncomingMessage ReadMessage() => ((State != NetPeerStatus.NotRunning) ? Peer.ReadMessage() : null);
        public static void Update(GameTime time)
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
            Statistics.trafficTimer += time.ElapsedGameTime.TotalSeconds;
            if (Statistics.trafficTimer >= 1)
            {
                Statistics.UploadBytesPerSec = Statistics.uploadBytesPerSecLast;
                Statistics.uploadBytesPerSecLast = 0;
                Statistics.DownloadBytesPerSec = Statistics.downloadBytesPerSecLast;
                Statistics.downloadBytesPerSecLast = 0;
                Statistics.trafficTimer -= 1;
            }
        }

        public delegate void MessageRecieved(NetIncomingMessage message);
        public static event MessageRecieved OnData, OnConnectionApproval, OnStatusChanged, OnLatencyUpdated, OnDiscoveryRequest, OnDiscoveryResponse, OnNatIntroSuccess, OnReceipt, OnUnconnectedData, OnVerboseDebugmessage, OnWarning, OnError, OnErrormessage, OnDebugmessage;

        public static void FlushSendQueue() => Peer?.FlushSendQueue();
        public static void Shutdown(string reason = null)
        {
            if (Peer == null)
                return;
            Peer.Shutdown(reason ?? string.Empty);
            Peer = null;
        }

        public static class Statistics
        {
            public static uint UploadedBytes { get; internal set; }
            public static uint DownloadedBytes { get; internal set; }

            internal static double trafficTimer;
            internal static uint uploadBytesPerSecLast, downloadBytesPerSecLast;
            public static uint UploadBytesPerSec { get; internal set; }
            public static uint DownloadBytesPerSec { get; internal set; }
        }
    }
}