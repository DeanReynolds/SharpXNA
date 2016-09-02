using SharpXNA;
using Lidgren.Network;

namespace $safeprojectname$
{
    using Frames = Game.Frames;
    using Packet = Network.Packet;

    public static class Multiplayer
    {
        private static Game.Frames Frame { get { return Game.Frame; } set { Game.Frame = value; } }
        private static Player Self { get { return Game.Self; } set { Game.Self = value; } }
        private static Player[] Players { get { return Game.Players; } set { Game.Players = value; } }

        public enum Packets { Connection, Disconnection, Initial, Position }

        public static void CreateLobby(string playerName)
        {
            Players = new Player[10];
            Self = Player.Add(new Player(playerName));
            Network.StartHosting(6121, Players.Length);
            Timers.Add("posSync", (1 / 20d));
        }
        public static void QuitLobby() { Network.Shutdown("Game"); Players = null; Timers.Remove("Positions"); Frame = Frames.Menu; }
        public static void OnConnectionApproval(NetIncomingMessage message)
        {
            ulong clientVersion = message.ReadUInt64();
            if (clientVersion == Globe.Version)
            {
                Player connector = Player.Add(new Player(message.ReadString()) { Connection = message.SenderConnection });
                if (connector != null)
                {
                    Packet data = new Packet((byte)Packets.Initial, (byte)Players.Length, connector.Slot);
                    for (int i = 0; i < Players.Length; i++)
                        if (!Players[i].Matches(null, connector)) data.Add(true, Players[i].Name);
                        else data.Add(false);
                    message.SenderConnection.Approve(data.Construct());
                    new Packet((byte)Packets.Connection, connector.Slot, connector.Name).Send(message.SenderConnection);
                }
                else message.SenderConnection.Deny("full");
            }
            else message.SenderConnection.Deny("different version");
        }
        public static void OnStatusChanged(NetIncomingMessage message)
        {
            NetConnectionStatus state = (Network.IsClient ? (NetConnectionStatus)message.ReadByte() : Network.IsServer ? message.SenderConnection.Status : NetConnectionStatus.None);
            if (state == NetConnectionStatus.Connected) { if (Network.IsClient) ProcessPacket((Packets)message.SenderConnection.RemoteHailMessage.ReadByte(), message.SenderConnection.RemoteHailMessage); }
            else if (state == NetConnectionStatus.Disconnected) { if (Network.IsClient) QuitLobby(); else ProcessPacket(Packets.Disconnection, message); }
        }
        public static void OnData(NetIncomingMessage message) { ProcessPacket((Packets)message.ReadByte(), message); }
        public static void ProcessPacket(Packets packet, NetIncomingMessage message)
        {
            #region Connection & Disconnection
            if (packet == Packets.Connection) Player.Set(message.ReadByte(), new Player(message.ReadString()));
            else if (packet == Packets.Disconnection)
            {
                Player disconnector = (Network.IsServer ? Player.Get(message.SenderConnection) : Network.IsClient ? Players[message.ReadByte()] : null);
                if (disconnector != null) Player.Remove(disconnector);
                if (Network.IsServer) new Packet((byte)packet, disconnector.Slot).Send(message.SenderConnection);
            }
            #endregion
            #region Initial Packet
            else if (packet == Packets.Initial)
            {
                Players = new Player[message.ReadByte()];
                Self = Player.Set(message.ReadByte(), new Player(Game.Name));
                for (int i = 0; i < Players.Length; i++)
                    if (message.ReadBoolean())
                        Players[i] = Player.Set((byte)i, new Player(message.ReadString()));
                Timers.Add("Positions", (1 / 20d));
                Frame = Frames.LoadGame;
            }
            #endregion
            #region Position(s)
            else if (packet == Packets.Position)
            {
                if (Network.IsServer)
                {
                    Player sender = Player.Get(message.SenderConnection);
                    if (sender != null) { sender.Position = message.ReadVector2(); sender.Angle = message.ReadFloat(); }
                }
                else if (Network.IsClient)
                {
                    int count = ((message.LengthBytes - 1) / 13);
                    for (int i = 0; i < count; i++)
                    {
                        Player sender = Players[message.ReadByte()];
                        if (sender != null) { sender.Position = message.ReadVector2(); sender.Angle = message.ReadFloat(); }
                    }
                }
            }
            #endregion
            #region Others
            #endregion
        }
    }
}