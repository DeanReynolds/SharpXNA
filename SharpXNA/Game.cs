using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SharpXNA;
using SharpXNA.Input;
using SharpXNA.Plugins;

namespace $safeprojectname$
{
    using Packet = Network.Packet;
    using Packets = Multiplayer.Packets;
    using InputFlags = SharpXNA.Plugins.String.InputFlags;

    public class Game : Microsoft.Xna.Framework.Game
    {
        public static ulong Version { get { return Globe.Version; } set { Globe.Version = value; } }
        public static float Speed { get { return Globe.Speed; } set { Globe.Speed = value; } }
    
        public Game() { Globe.GraphicsDeviceManager = new GraphicsDeviceManager(this); Content.RootDirectory = "Content"; }
    
        public enum Frames { Menu, Connecting, LoadGame, Game }
        public static Frames Frame = Frames.Menu;

        public static Player Self;
        public static Player[] Players;
        public static bool Quit = false;
        public static Vector2 Scale { get { return new Vector2((Screen.BackBufferWidth / 1920f), (Screen.BackBufferHeight / 1080f)); } }

        #region Menu/Connecting Variables
        public static INI Settings;
        public static string Name, IP;
        public static byte MenuState = 0;
        public static double BlinkTimer;
        #endregion
        #region Game Variables
        #endregion

        protected override void LoadContent()
        {
            #region Globalization
            Screen.Batch = new Batch(GraphicsDevice);
            Globe.Form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            Globe.GameWindow = Window;
            Globe.ContentManager = new CustomContentManager(Content.ServiceProvider, Content.RootDirectory);
            Globe.GraphicsDevice = GraphicsDevice;
            Globe.Viewport = GraphicsDevice.Viewport;
            Globe.GraphicsAdapter = GraphicsDevice.Adapter;
            Globe.TextureLoader = new TextureLoader(Globe.GraphicsDevice);
            #endregion
            #region Initialization
            Sound.Initialize(256);
            Textures.RootDirectory = "Textures";
            Sound.RootDirectory = "Sound";
            Fonts.RootDirectory = "Fonts";
            Performance.UpdateFPS = new SharpXNA.Plugins.Buffer(20);
            Performance.DrawFPS = new SharpXNA.Plugins.Buffer(20);
            Network.OnConnectionApproval += Multiplayer.OnConnectionApproval;
            Network.OnStatusChanged += Multiplayer.OnStatusChanged;
            Network.OnData += Multiplayer.OnData;
            #endregion
            Screen.Set(1920, 1080, false);
            Screen.Expand(true);
            IsMouseVisible = true;
            Settings = INI.ReadFile("settings.ini");
            Name = Settings.Get("Name"); IP = Settings.Get("IP");
            if (!Name.IsNullOrEmpty()) MenuState = 1;
        }
        protected override void Update(GameTime time)
        {
            Performance.UpdateFPS.Record(1 / time.ElapsedGameTime.TotalSeconds);
            Mouse.Update();
            Keyboard.Update(time);
            XboxPad.Update(time);
            Timers.Update(time);
            Globe.IsActive = IsActive;
            if (XboxPad.Pressed(XboxPad.Buttons.Back) || Keyboard.Pressed(Keyboard.Keys.Escape) || Quit) Exit();
            if (Keyboard.Pressed(Keyboard.Keys.F3)) Profiler.Enabled = !Profiler.Enabled;
            Profiler.Start("Frame Update");
            #region Menu/Connecting Frame
            if (Frame == Frames.Menu)
            {
                if (MenuState == 0)
                {
                    if (IsActive)
                    {
                        BlinkTimer -= time.ElapsedGameTime.TotalSeconds; if (BlinkTimer <= 0) BlinkTimer += .6;
                        Name = Name.AcceptInput((InputFlags.NoLeadingSpaces | InputFlags.NoRepeatingSpaces), 20); Settings.Set("Name", Name, true);
                        if (Keyboard.Pressed(Keyboard.Keys.Enter) && !Name.IsNullOrEmpty()) MenuState = 1;
                    }
                }
                else if (MenuState == 1)
                {
                    if (Mouse.Press(Mouse.Buttons.Left))
                    {
                        Rectangle mouse = new Rectangle(Mouse.X, Mouse.Y, 1, 1);
                        string str = "Host"; SpriteFont font = Fonts.Load("calibri 50"); Vector2 scale = (Scale * .75f), size = (font.MeasureString(str) * scale);
                        Rectangle button = new Rectangle((int)((Screen.BackBufferWidth / 2f) - (size.X / 2f)), (int)((Screen.BackBufferHeight / 2f) - size.Y), (int)size.X, (int)size.Y);
                        if (mouse.Intersects(button)) { Multiplayer.CreateLobby(Name); Frame = Frames.LoadGame; }
                        str = "Connect"; scale = (Scale * .75f); size = (font.MeasureString(str) * scale);
                        button = new Rectangle((int)((Screen.BackBufferWidth / 2f) - (size.X / 2f)), (int)((Screen.BackBufferHeight / 2f) + (size.Y * .25f)), (int)size.X, (int)size.Y);
                        if (mouse.Intersects(button)) MenuState = 2;
                    }
                }
                else if (MenuState == 2)
                {
                    if (IsActive)
                    {
                        BlinkTimer -= time.ElapsedGameTime.TotalSeconds; if (BlinkTimer <= 0) BlinkTimer += .6;
                        IP = IP.AcceptInput((InputFlags.NoLeadingPeriods | InputFlags.NoLetters | InputFlags.NoSpecalCharacters | InputFlags.NoSpaces | InputFlags.AllowPeriods |
                            InputFlags.NoRepeatingPeriods | InputFlags.AllowColons | InputFlags.NoRepeatingColons | InputFlags.NoLeadingPeriods), 21);
                        Settings.Set("IP", IP, true);
                        if (Keyboard.Pressed(Keyboard.Keys.Enter) && !IP.IsNullOrEmpty()) { Network.Connect(IP.Split(':')[0], (IP.Contains(":") ? Convert.ToInt32(IP.Split(':')[1]) : 6121), new Packet(null, Name)); Frame = Frames.Connecting; }
                        else if (Keyboard.Pressed(Keyboard.Keys.Tab)) MenuState = 1;
                    }
                }
                Network.Update();
            }
            else if (Frame == Frames.Connecting) { BlinkTimer -= time.ElapsedGameTime.TotalSeconds; if (BlinkTimer <= 0) BlinkTimer += 1; Network.Update(); }
            #endregion
            #region LoadGame/Game Frame
            else if (Frame == Frames.LoadGame)
            {
                BlinkTimer -= time.ElapsedGameTime.TotalSeconds; if (BlinkTimer <= 0) BlinkTimer += 1;
                if (Network.IsNullOrServer) Frame = Frames.Game;
                Network.Update();
            }
            else if (Frame == Frames.Game)
            {
                for (int i = 0; i < Players.Length; i++)
                    if (Players[i] != null)
                        Players[i].Update(time);
                if (Timers.Tick("posSync") && Network.IsServer)
                    foreach (Player player in Players)
                        if (player != null && (player.Connection != null))
                        {
                            Packet packet = new Packet((byte)Packets.Position);
                            foreach (Player other in Players)
                                if (!other.Matches(null, player))
                                if ((other != player) && (other != null))
                                    packet.Add(other.Slot, other.Position, other.Angle);
                            packet.SendTo(player.Connection, NetDeliveryMethod.UnreliableSequenced, 1);
                        }
                Network.Update();
            }
            #endregion
            Profiler.Stop("Frame Update");
            Textures.ApplyDispose();
            Sound.AutoTerminate();
            base.Update(time);
        }
        protected override void Draw(GameTime time)
        {
            Performance.DrawFPS.Record(1 / time.ElapsedGameTime.TotalSeconds);
            GraphicsDevice.Clear(Color.Black);
            Profiler.Start("Frame Draw");
            #region Menu/Connecting Frame
            if (Frame == Frames.Menu)
            {
                GraphicsDevice.Clear(Color.WhiteSmoke);
                Screen.Setup(SpriteSortMode.Deferred, SamplerState.PointClamp);
                Screen.DrawString("Developed by Dcrew", Fonts.Load("calibri 30"), new Vector2((Screen.BackBufferWidth / 2f), (Screen.BackBufferHeight - (Screen.BackBufferHeight / 8f))), (Color.Gray * .5f), Textures.Origin.Center, (Scale * .5f));
                Vector2 position = new Vector2((Screen.BackBufferWidth / 8f), ((Screen.BackBufferHeight / 2f) - 40));
                if (MenuState == 0)
                {
                    Screen.DrawString("Enter your name!", Fonts.Load("calibri 50"), new Vector2((Screen.BackBufferWidth / 2f), ((Screen.BackBufferHeight / 2f) - (35 * Scale.Y))), (Color.Gray * .75f), new Textures.Origin(.5f, 1, true), (Scale * .75f));
                    Screen.DrawString((Name + (((BlinkTimer <= .3f) && IsActive) ? "|" : string.Empty)), Fonts.Load("calibri 50"), new Vector2((Screen.BackBufferWidth / 2f), ((Screen.BackBufferHeight / 2f) - (30 * Scale.Y))), (Color.Black * .75f), new Textures.Origin(.5f, 0, true), (Scale * .75f));
                    Screen.DrawString("Press 'enter' to proceed!", Fonts.Load("calibri 30"), new Vector2((Screen.BackBufferWidth / 2f), ((Screen.BackBufferHeight / 2f) + (35 * Scale.Y))), (Color.DimGray * .5f), new Textures.Origin(.5f, 1, true), (Scale * .5f));
                }
                else if (MenuState == 1)
                {
                    string str = "Welcome, "; SpriteFont font = Fonts.Load("calibri 30"); Vector2 scale = (Scale * .5f); Vector2 size = (font.MeasureString(str) * scale);
                    Screen.DrawString(str, font, new Vector2(((Screen.BackBufferWidth / 2f) - ((font.MeasureString("Welcome, " + Name + "!").X * scale.X) / 2f)), ((Screen.BackBufferHeight / 2f) - (size.Y * 6))), (Color.Gray * .75f), null, 0, new Textures.Origin(0, .5f, true), scale);
                    str = Name; Screen.DrawString(str, font, new Vector2((((Screen.BackBufferWidth / 2f) - ((font.MeasureString("Welcome, " + Name + "!").X * scale.X) / 2f)) + (font.MeasureString("Welcome, ").X * scale.X)), ((Screen.BackBufferHeight / 2f) - (size.Y * 6))), (Color.Green * .75f), new Textures.Origin(0, .5f, true), scale);
                    str = "!"; Screen.DrawString(str, font, new Vector2((((Screen.BackBufferWidth / 2f) - ((font.MeasureString("Welcome, " + Name + "!").X * scale.X) / 2f)) + (font.MeasureString("Welcome, " + Name).X * scale.X)), ((Screen.BackBufferHeight / 2f) - (size.Y * 6))), (Color.Gray * .75f), new Textures.Origin(0, .5f, true), scale);
                    Rectangle mouse = new Rectangle(Mouse.X, Mouse.Y, 1, 1);
                    str = "Host"; font = Fonts.Load("calibri 50"); scale = (Scale * .75f); size = (font.MeasureString(str) * scale);
                    Rectangle button = new Rectangle((int)((Screen.BackBufferWidth / 2f) - (size.X / 2f)), (int)((Screen.BackBufferHeight / 2f) - size.Y), (int)size.X, (int)size.Y);
                    Color color = Color.Silver; if (mouse.Intersects(button)) { scale += new Vector2(.35f); color = Color.White; }
                    Screen.DrawString(str, font, new Vector2((button.X + (button.Width / 2f)), (button.Y + (button.Height / 2f))), color, (Color.Black * .5f), Textures.Origin.Center, scale);
                    str = "Connect"; scale = (Scale * .75f); size = (font.MeasureString(str) * scale);
                    button = new Rectangle((int)((Screen.BackBufferWidth / 2f) - (size.X / 2f)), (int)((Screen.BackBufferHeight / 2f) + (size.Y * .25f)), (int)size.X, (int)size.Y);
                    color = Color.Silver; if (mouse.Intersects(button)) { scale += new Vector2(.35f); color = Color.White; }
                    Screen.DrawString(str, font, new Vector2((button.X + (button.Width / 2f)), (button.Y + (button.Height / 2f))), color, (Color.Black * .5f), Textures.Origin.Center, scale);
                }
                else if (MenuState == 2)
                {
                    Screen.DrawString("Enter the IP:Port!", Fonts.Load("calibri 50"), new Vector2((Screen.BackBufferWidth / 2f), ((Screen.BackBufferHeight / 2f) - (35 * Scale.Y))), (Color.Gray * .75f), new Textures.Origin(.5f, 1, true), (Scale * .75f));
                    Screen.DrawString((IP + (((BlinkTimer <= .3f) && IsActive) ? "|" : string.Empty)), Fonts.Load("calibri 50"), new Vector2((Screen.BackBufferWidth / 2f), ((Screen.BackBufferHeight / 2f) - (30 * Scale.Y))), (Color.Black * .75f), new Textures.Origin(.5f, 0, true), (Scale * .75f));
                    Screen.DrawString("Press 'enter' to proceed!", Fonts.Load("calibri 30"), new Vector2((Screen.BackBufferWidth / 2f), ((Screen.BackBufferHeight / 2f) + (35 * Scale.Y))), (Color.DimGray * .5f), new Textures.Origin(.5f, 1, true), (Scale * .5f));
                    Screen.DrawString("Press 'tab' to go back!", Fonts.Load("calibri 30"), new Vector2((Screen.BackBufferWidth / 2f), ((Screen.BackBufferHeight / 2f) + (50 * Scale.Y))), (Color.DimGray * .5f), new Textures.Origin(.5f, 1, true), (Scale * .5f));
                }
            }
            else if (Frame == Frames.Connecting) { Screen.Setup(); Screen.DrawString(("Connecting to " + IP + new string('.', (4 - (int)Math.Ceiling(BlinkTimer * 4)))), Fonts.Load("calibri 50"), new Vector2((Screen.BackBufferWidth / 2f), (Screen.BackBufferHeight / 2f)), Color.White, Textures.Origin.Center, (Scale * .5f)); Screen.Cease(); }
            #endregion
            #region LoadGame/Game Frame
            else if (Frame == Frames.LoadGame) { Screen.Setup(); Screen.DrawString(("Loading" + new string('.', (4 - (int)Math.Ceiling(BlinkTimer * 4)))), Fonts.Load("calibri 50"), new Vector2((Screen.BackBufferWidth / 2f), (Screen.BackBufferHeight / 2f)), Color.White, Textures.Origin.Center, (Scale * .5f)); Screen.Cease(); }
            else if (Frame == Frames.Game)
            {
				Screen.Setup();
                foreach (Player player in Players)
                    if (player != null)
                        player.Draw();
				Screen.Cease();
            }
            #endregion
            Profiler.Stop("Frame Draw");
            if (Profiler.Enabled) Profiler.Draw(430);
            if (Screen.IsSetup) Screen.Cease();
            base.Draw(time);
        }

        protected override void OnExiting(object sender, EventArgs args) { Multiplayer.QuitLobby(); base.OnExiting(sender, args); }
    }
}