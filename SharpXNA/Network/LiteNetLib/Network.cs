namespace LiteNetLib
{
    public static class Network
    {
        public static NetManager Manager { get; internal set; }
        public static EventBasedNetListener Listener { get; internal set; }

        public static bool IsNullOrServer => ((Manager == null) || IsServer);
        public static bool IsServer { get; internal set; }
        public static bool IsClient { get; internal set; }
        
        public static void Host(int port, int maxConnections)
        {
            Listener = new EventBasedNetListener();
            Manager = new NetManager(Listener, maxConnections);
            Manager.Start(port);
        }
        public static void Connect(string ip, int port, string connectionKey, params object[] data)
        {
            Listener = new EventBasedNetListener();
            Manager = new NetManager(Listener);
            Manager.Start();
            Manager.Connect(ip, port, connectionKey);
        }
        
        public static void Update() => Manager.PollEvents();
    }
}