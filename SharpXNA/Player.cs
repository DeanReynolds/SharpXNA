using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpXNA;
using SharpXNA.Input;
using SharpXNA.Collision;
using static SharpXNA.Textures;

namespace $safeprojectname$
{
    using Packet = Network.Packet;
    using Packets = Multiplayer.Packets;

	public class Player
    {
        private static Player Self { get { return Game.Self; } }
        private static Player[] Players { get { return Game.Players; } }

        public byte Slot;
        public NetConnection Connection;
        public string Name;

        public Player(string Name) { this.Name = Name; Load(); }
        public Player(byte Slot, string Name) { this.Slot = Slot; this.Name = Name; Load(); }
        public void Load()
        {
			Mask = Polygon.CreateCircle(24, Vector2.Zero);
        }

		public Vector2 Position;
		public float Angle;
        public Polygon Mask;
		public Vector2 Speed = new Vector2(250, 250);

        public void Update(GameTime time)
        {
            if (this == Self)
            {
                if (Globe.IsActive)
                {
                    Vector2 oldPosition = Position;
                    if (Keyboard.Holding(Keyboard.Keys.W)) Move(new Vector2(0, -(float)(Speed.Y * time.ElapsedGameTime.TotalSeconds)));
                    if (Keyboard.Holding(Keyboard.Keys.S)) Move(new Vector2(0, (float)(Speed.Y * time.ElapsedGameTime.TotalSeconds)));
                    if (Keyboard.Holding(Keyboard.Keys.A)) Move(new Vector2(-(float)(Speed.X * time.ElapsedGameTime.TotalSeconds), 0));
                    if (Keyboard.Holding(Keyboard.Keys.D)) Move(new Vector2((float)(Speed.X * time.ElapsedGameTime.TotalSeconds), 0));
                    if (oldPosition != Position) ;
                }
                if (Timers.Tick("posSync") && Network.IsClient) new Packet((byte)Packets.Position, Position, Angle).Send(NetDeliveryMethod.UnreliableSequenced, 1);
            }
        }
        public void Draw()
        {
            Screen.Draw(Pixel(Color.Blue), Position, Angle, Origin.Center, new Vector2(32));
            Mask.Draw(Color.White, 1);
        }
        
        public void Move(Vector2 offset)
        {
            float specific = 1;
            if ((offset.X != 0) && !Collides)
            {
                Position.X += offset.X;
                if (Collides)
                {
                    Position.X -= offset.X;
                    while (!Collides) Position.X += ((offset.X < 0) ? -specific : specific);
                    while (Collides) Position.X -= ((offset.X < 0) ? -specific : specific);
                }
            }
            if ((offset.Y != 0) && !Collides)
            {
                Position.Y += offset.Y;
                if (Collides)
                {
                    Position.Y -= offset.Y;
                    while (!Collides) Position.Y += ((offset.Y < 0) ? -specific : specific);
                    while (Collides) Position.Y -= ((offset.Y < 0) ? -specific : specific);
                }
            }
        }
        public void UpdateMask() { Mask.Position = Position; }
        public bool Collides
        {
            get
            {
                UpdateMask();
                /*for (int x = (int)(Position.X / Tile.Width - 1); x <= (Position.X / Tile.Width + 1); x++)
                    for (int y = (int)(Position.Y / Tile.Height - 1); y <= (Position.Y / Tile.Height + 1); y++)
                        if (World.InBounds(x, y) && (World.Tiles[x, y].Fore > 0) && Tiles.Fore[World.Tiles[x, y].Fore].Solid)
                        {
                            Polygon Mask = Polygon.CreateRectangleWithCross(new Vector2(Tile.Width, Tile.Height), Vector2.Zero);
                            Mask.Position = new Vector2(((x * Tile.Width) + (Tile.Width / 2f)), ((y * Tile.Height) + (Tile.Height / 2f)));
                            if (this.Mask.Intersects(Mask)) return true;
                        }*/
                return false;
            }
        }
        
        public float VolumeFromDistance(Vector2 position, float fade, float max) { return Position.VolumeFromDistance(position, fade, max); }
        
        public static Player Get(NetConnection connection) { for (int i = 0; i < Players.Length; i++) if ((Players[i] != null) && (Players[i].Connection == connection)) return Players[i]; return null; }
        public static Player Add(Player player) { for (int i = 0; i < Players.Length; i++) if (Players[i] == null) { player.Slot = (byte)i; Players[i] = player; return player; } return null; }
        public static Player Set(byte slot, Player player) { if (slot < Players.Length) { player.Slot = slot; Players[slot] = player; return player; } return null; }
        public static bool Remove(Player player) { for (int i = 0; i < Players.Length; i++) if (Players[i] == player) { Players[i] = null; return true; } return false; }
    }
}