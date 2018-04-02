using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpXNA.Plugins;

namespace SharpXNA
{
    public static class Screen
    {
        internal static Batch _batch;
        public static Batch Batch => _batch;

        public static Rectangle BackBufferBounds { get { try { return new Rectangle(0, 0, BackBufferWidth, BackBufferHeight); } catch { return Rectangle.Empty; } } }
        public static int BackBufferWidth
        {
            get { return Engine.GraphicsDeviceManager.PreferredBackBufferWidth; }
            set { Engine.GraphicsDeviceManager.PreferredBackBufferWidth = value; Engine.GraphicsDeviceManager.ApplyChanges(); }
        }
        public static int BackBufferHeight
        {
            get { return Engine.GraphicsDeviceManager.PreferredBackBufferHeight; }
            set { Engine.GraphicsDeviceManager.PreferredBackBufferHeight = value; Engine.GraphicsDeviceManager.ApplyChanges(); }
        }
        public static bool Fullscreen { get { return Engine.GraphicsDeviceManager.IsFullScreen; } set { Engine.GraphicsDeviceManager.IsFullScreen = value; Engine.GraphicsDeviceManager.ApplyChanges(); } }
        public static bool VSync { get { return Engine.GraphicsDeviceManager.SynchronizeWithVerticalRetrace; } set { Engine.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = value; Engine.GraphicsDeviceManager.ApplyChanges(); } }

        public static void Set(int width, int height, bool fullscreen, bool vsync = false)
        {
            Engine.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            Engine.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            Engine.GraphicsDeviceManager.IsFullScreen = fullscreen;
            Engine.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vsync;
            Engine.GraphicsDeviceManager.ApplyChanges();
        }

        public static Rectangle ViewportBounds { get { try { return new Rectangle(0, 0, Engine.Viewport.Width, Engine.Viewport.Height); } catch { return Rectangle.Empty; } } }
        public static int ViewportWidth { get { try { return Engine.Viewport.Width; } catch { return 0; } } }
        public static int ViewportHeight { get { try { return Engine.Viewport.Height; } catch { return 0; } } }

        public static Rectangle WindowBounds { get { try { return Engine._gameWindow.ClientBounds; } catch { return Rectangle.Empty; } } }
        public static int WindowWidth { get { try { return Engine._gameWindow.ClientBounds.Width; } catch { return 0; } } }
        public static int WindowHeight { get { try { return Engine._gameWindow.ClientBounds.Height; } catch { return 0; } } }

        public static int VirtualWidth { get; internal set; }
        public static int VirtualHeight { get; internal set; }

        public static void SetupVirtualResolution(int width, int height)
        {
            VirtualWidth = width;
            VirtualHeight = height;
            var targetAspectRatio = (width / (float)height);
            var width2 = BackBufferWidth;
            var height2 = (int)(width2 / targetAspectRatio + .5f);
            if (height2 > BackBufferHeight)
            {
                height2 = BackBufferHeight;
                width2 = (int)(height2 * targetAspectRatio + .5f);
            }
            Engine._graphicsDevice.Viewport = new Viewport()
            {
                X = ((BackBufferWidth / 2) - (width2 / 2)),
                Y = ((BackBufferHeight / 2) - (height2 / 2)),
                Width = width2,
                Height = height2
            };
        }
        public static void SetupVirtualResolution(int width, int height, out float zoom)
        {
            VirtualWidth = width;
            VirtualHeight = height;
            float xZoom = (BackBufferWidth / (float)width),
                yZoom = (BackBufferHeight / (float)height);
            var targetAspectRatio = (width / (float)height);
            var width2 = BackBufferWidth;
            var height2 = (int)(width2 / targetAspectRatio + .5f);
            if (height2 > BackBufferHeight)
            {
                height2 = BackBufferHeight;
                width2 = (int)(height2 * targetAspectRatio + .5f);
            }
            Engine._graphicsDevice.Viewport = new Viewport()
            {
                X = ((BackBufferWidth / 2) - (width2 / 2)),
                Y = ((BackBufferHeight / 2) - (height2 / 2)),
                Width = width2,
                Height = height2
            };
            zoom = MathHelper.Min(xZoom, yZoom);
        }
        public static void Expand(bool borderless = true, bool changeScreen = true)
        {
            var screen = (changeScreen ? System.Windows.Forms.Screen.FromPoint(Cursor.Position) : System.Windows.Forms.Screen.FromPoint(Engine.Form.Location));
            try { Engine.Window.IsBorderless = borderless; }
            catch { }
            Engine.Form.Location = screen.Bounds.Location;
            Engine.Form.Size = screen.Bounds.Size;
        }
        public static void Center()
        {
            var screen = System.Windows.Forms.Screen.FromRectangle(Engine.Form.Bounds);
            Engine.Form.Location = new System.Drawing.Point(((screen.Bounds.X + (screen.Bounds.Width / 2)) - (int)System.Math.Round(Engine.Form.Width / 2f)),
                ((screen.Bounds.Y + (screen.Bounds.Height / 2)) - (int)System.Math.Round(Engine.Form.Height / 2f)));
        }

        public static bool Begun => Batch.Begun;
        public static void Begin(Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, null, null, null, matrix); }
        public static void Begin(BlendState blendState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, null, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, null, null, null, matrix); }
        public static void Begin(SamplerState samplerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, null, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, null, null, null, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, null, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, null, null, null, matrix); }
        public static void Begin(DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, depthStencilState, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, depthStencilState, null, null, matrix); }
        public static void Begin(BlendState blendState, DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, depthStencilState, null, null, matrix); }
        public static void Begin(SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, depthStencilState, null, null, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, null, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, depthStencilState, null, null, matrix); }
        public static void Begin(RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, null, rasterizerState, null, matrix); }
        public static void Begin(BlendState blendState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, null, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, null, rasterizerState, null, matrix); }
        public static void Begin(SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, null, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, null, rasterizerState, null, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, null, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, null, rasterizerState, null, matrix); }
        public static void Begin(DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public static void Begin(Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, null, null, effect, matrix); }
        public static void Begin(BlendState blendState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, null, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, null, null, effect, matrix); }
        public static void Begin(SamplerState samplerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, null, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, null, null, effect, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, null, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, null, null, effect, matrix); }
        public static void Begin(DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, depthStencilState, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, depthStencilState, null, effect, matrix); }
        public static void Begin(BlendState blendState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, depthStencilState, null, effect, matrix); }
        public static void Begin(SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, depthStencilState, null, effect, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, null, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, depthStencilState, null, effect, matrix); }
        public static void Begin(RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, null, rasterizerState, effect, matrix); }
        public static void Begin(BlendState blendState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, null, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, null, rasterizerState, effect, matrix); }
        public static void Begin(SamplerState samplerState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, null, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, null, rasterizerState, effect, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, null, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, Effect effect, RasterizerState rasterizerState, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, null, rasterizerState, effect, matrix); }
        public static void Begin(DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, null, depthStencilState, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, null, depthStencilState, rasterizerState, effect, matrix); }
        public static void Begin(BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, null, depthStencilState, rasterizerState, effect, matrix); }
        public static void Begin(SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, null, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public static void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public static void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Batch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public static void End() { Batch.End(); }

        public static void Draw(Texture2D texture, Vector2 position) { Batch.Draw(texture, position, null, Color.White, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, 0, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source) { Batch.Draw(texture, position, source, Color.White, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, 0, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint) { Batch.Draw(texture, position, null, tint, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, 0, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint) { Batch.Draw(texture, position, source, tint, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, 0, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds) { Batch.Draw(texture, bounds, null, Color.White, 0, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, Color.White, 0, Origin.None, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source) { Batch.Draw(texture, bounds, source, Color.White, 0, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, Color.White, 0, Origin.None, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Vector2 scale) { Batch.Draw(texture, position, null, Color.White, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Vector2 scale, float layer) { Batch.Draw(texture, position, null, Color.White, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, 0, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Vector2 scale) { Batch.Draw(texture, position, source, Color.White, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Vector2 scale, float layer) { Batch.Draw(texture, position, source, Color.White, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, 0, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint) { Batch.Draw(texture, bounds, null, tint, 0, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, tint, 0, Origin.None, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint) { Batch.Draw(texture, bounds, source, tint, 0, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, tint, 0, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Vector2 scale) { Batch.Draw(texture, position, null, tint, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Vector2 scale, float layer) { Batch.Draw(texture, position, null, tint, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, 0, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Vector2 scale) { Batch.Draw(texture, position, source, tint, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Vector2 scale, float layer) { Batch.Draw(texture, position, source, tint, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, 0, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Origin origin) { Batch.Draw(texture, position, null, Color.White, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Origin origin, float layer) { Batch.Draw(texture, position, null, Color.White, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, 0, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin) { Batch.Draw(texture, position, source, Color.White, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, float layer) { Batch.Draw(texture, position, source, Color.White, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, 0, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin) { Batch.Draw(texture, position, null, tint, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, float layer) { Batch.Draw(texture, position, null, tint, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, 0, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin) { Batch.Draw(texture, position, source, tint, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, float layer) { Batch.Draw(texture, position, source, tint, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, 0, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Origin origin) { Batch.Draw(texture, bounds, null, Color.White, 0, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Origin origin, float layer) { Batch.Draw(texture, bounds, null, Color.White, 0, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, Color.White, 0, origin, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Origin origin) { Batch.Draw(texture, bounds, source, Color.White, 0, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Origin origin, float layer) { Batch.Draw(texture, bounds, source, Color.White, 0, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, Color.White, 0, origin, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Origin origin, Vector2 scale) { Batch.Draw(texture, position, null, Color.White, 0, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, null, Color.White, 0, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, 0, origin, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, Vector2 scale) { Batch.Draw(texture, position, source, Color.White, 0, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, source, Color.White, 0, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, 0, origin, scale, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, Origin origin) { Batch.Draw(texture, bounds, null, tint, 0, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, Origin origin, float layer) { Batch.Draw(texture, bounds, null, tint, 0, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, tint, 0, origin, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, Origin origin) { Batch.Draw(texture, bounds, source, tint, 0, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, Origin origin, float layer) { Batch.Draw(texture, bounds, source, tint, 0, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, tint, 0, origin, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, Vector2 scale) { Batch.Draw(texture, position, null, tint, 0, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, null, tint, 0, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, 0, origin, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, Vector2 scale) { Batch.Draw(texture, position, source, tint, 0, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, source, tint, 0, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, 0, origin, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle) { Batch.Draw(texture, position, null, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, float layer) { Batch.Draw(texture, position, null, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, angle, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle) { Batch.Draw(texture, position, source, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, float layer) { Batch.Draw(texture, position, source, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, angle, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle) { Batch.Draw(texture, position, null, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, float layer) { Batch.Draw(texture, position, null, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, angle, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle) { Batch.Draw(texture, position, source, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, float layer) { Batch.Draw(texture, position, source, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, angle, Origin.None, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, float angle) { Batch.Draw(texture, bounds, null, Color.White, angle, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, float angle, float layer) { Batch.Draw(texture, bounds, null, Color.White, angle, Origin.None, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, Color.White, angle, Origin.None, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle) { Batch.Draw(texture, bounds, source, Color.White, angle, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, float layer) { Batch.Draw(texture, bounds, source, Color.White, angle, Origin.None, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, Color.White, angle, Origin.None, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Vector2 scale) { Batch.Draw(texture, position, null, Color.White, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Vector2 scale, float layer) { Batch.Draw(texture, position, null, Color.White, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, angle, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Vector2 scale) { Batch.Draw(texture, position, source, Color.White, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Vector2 scale, float layer) { Batch.Draw(texture, position, source, Color.White, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, angle, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle) { Batch.Draw(texture, bounds, null, tint, angle, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, float layer) { Batch.Draw(texture, bounds, null, tint, angle, Origin.None, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, tint, angle, Origin.None, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle) { Batch.Draw(texture, bounds, source, tint, angle, Origin.None, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, float layer) { Batch.Draw(texture, bounds, source, tint, angle, Origin.None, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, tint, angle, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Vector2 scale) { Batch.Draw(texture, position, null, tint, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Vector2 scale, float layer) { Batch.Draw(texture, position, null, tint, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, angle, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Vector2 scale) { Batch.Draw(texture, position, source, tint, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Vector2 scale, float layer) { Batch.Draw(texture, position, source, tint, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, angle, Origin.None, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Origin origin) { Batch.Draw(texture, position, null, Color.White, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, float layer) { Batch.Draw(texture, position, null, Color.White, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, angle, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin) { Batch.Draw(texture, position, source, Color.White, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, float layer) { Batch.Draw(texture, position, source, Color.White, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, angle, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin) { Batch.Draw(texture, position, null, tint, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, float layer) { Batch.Draw(texture, position, null, tint, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, angle, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin) { Batch.Draw(texture, position, source, tint, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, float layer) { Batch.Draw(texture, position, source, tint, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, angle, origin, Vector2.One, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, float angle, Origin origin) { Batch.Draw(texture, bounds, null, Color.White, angle, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, float angle, Origin origin, float layer) { Batch.Draw(texture, bounds, null, Color.White, angle, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, Color.White, angle, origin, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, Origin origin) { Batch.Draw(texture, bounds, source, Color.White, angle, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, Origin origin, float layer) { Batch.Draw(texture, bounds, source, Color.White, angle, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, Color.White, angle, origin, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, Vector2 scale) { Batch.Draw(texture, position, null, Color.White, angle, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, null, Color.White, angle, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, Color.White, angle, origin, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, Vector2 scale) { Batch.Draw(texture, position, source, Color.White, angle, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, source, Color.White, angle, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, Color.White, angle, origin, scale, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, Origin origin) { Batch.Draw(texture, bounds, null, tint, angle, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, Origin origin, float layer) { Batch.Draw(texture, bounds, null, tint, angle, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, null, tint, angle, origin, effect, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, Origin origin) { Batch.Draw(texture, bounds, source, tint, angle, origin, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, Origin origin, float layer) { Batch.Draw(texture, bounds, source, tint, angle, origin, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.Draw(texture, bounds, source, tint, angle, origin, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, Vector2 scale) { Batch.Draw(texture, position, null, tint, angle, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, null, tint, angle, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, null, tint, angle, origin, scale, effect, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, Vector2 scale) { Batch.Draw(texture, position, source, tint, angle, origin, scale, SpriteEffects.None, 0); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, Vector2 scale, float layer) { Batch.Draw(texture, position, source, tint, angle, origin, scale, SpriteEffects.None, layer); }
        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(texture, position, source, tint, angle, origin, scale, effect, layer); }

        public static void Draw(Sprite sprite, Vector2 position) { Batch.Draw(sprite, position, Color.White, 0, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, Color.White, 0, Vector2.One, effect, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint) { Batch.Draw(sprite, position, tint, 0, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, tint, 0, Vector2.One, effect, layer); }
        public static void Draw(Sprite sprite, Rectangle bounds) { Batch.Draw(sprite, bounds, Color.White, 0, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Rectangle bounds, SpriteEffects effect, float layer) { Batch.Draw(sprite, bounds, Color.White, 0, effect, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Vector2 scale) { Batch.Draw(sprite, position, Color.White, 0, scale, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, Vector2 scale, float layer) { Batch.Draw(sprite, position, Color.White, 0, scale, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, Color.White, 0, scale, effect, layer); }
        public static void Draw(Sprite sprite, Rectangle bounds, Color tint) { Batch.Draw(sprite, bounds, tint, 0, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Rectangle bounds, Color tint, SpriteEffects effect, float layer) { Batch.Draw(sprite, bounds, tint, 0, effect, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, Vector2 scale) { Batch.Draw(sprite, position, tint, 0, scale, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, Vector2 scale, float layer) { Batch.Draw(sprite, position, tint, 0, scale, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, tint, 0, scale, effect, layer); }
        public static void Draw(Sprite sprite, Vector2 position, float angle) { Batch.Draw(sprite, position, Color.White, angle, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, float angle, float layer) { Batch.Draw(sprite, position, Color.White, angle, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Vector2 position, float angle, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, Color.White, angle, Vector2.One, effect, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, float angle) { Batch.Draw(sprite, position, tint, angle, Vector2.One, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, float angle, float layer) { Batch.Draw(sprite, position, tint, angle, Vector2.One, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, float angle, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, tint, angle, Vector2.One, effect, layer); }
        public static void Draw(Sprite sprite, Rectangle bounds, float angle) { Batch.Draw(sprite, bounds, Color.White, angle, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Rectangle bounds, float angle, float layer) { Batch.Draw(sprite, bounds, Color.White, angle, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Rectangle bounds, float angle, SpriteEffects effect, float layer) { Batch.Draw(sprite, bounds, Color.White, angle, effect, layer); }
        public static void Draw(Sprite sprite, Vector2 position, float angle, Vector2 scale) { Batch.Draw(sprite, position, Color.White, angle, scale, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, float angle, Vector2 scale, float layer) { Draw(sprite, position, Color.White, angle, scale, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Vector2 position, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, Color.White, angle, scale, effect, layer); }
        public static void Draw(Sprite sprite, Rectangle bounds, Color tint, float angle) { Batch.Draw(sprite, bounds, tint, angle, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Rectangle bounds, Color tint, float angle, float layer) { Batch.Draw(sprite, bounds, tint, angle, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Rectangle bounds, Color tint, float angle, SpriteEffects effect, float layer) { Batch.Draw(sprite, bounds, tint, angle, effect, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, float angle, Vector2 scale) { Batch.Draw(sprite, position, tint, angle, scale, SpriteEffects.None, 0); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, float angle, Vector2 scale, float layer) { Batch.Draw(sprite, position, tint, angle, scale, SpriteEffects.None, layer); }
        public static void Draw(Sprite sprite, Vector2 position, Color tint, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.Draw(sprite, position, tint, angle, scale, effect, layer); }

        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore) { Batch.DrawString(str, font, position, fore, null, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, 0, Origin.None, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle) { Batch.DrawString(str, font, position, fore, null, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, float layer) { Batch.DrawString(str, font, position, fore, null, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, angle, Origin.None, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back) { Batch.DrawString(str, font, position, fore, back, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, 0, Origin.None, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle) { Batch.DrawString(str, font, position, fore, back, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, float layer) { Batch.DrawString(str, font, position, fore, back, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, angle, Origin.None, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin) { Batch.DrawString(str, font, position, fore, null, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, float layer) { Batch.DrawString(str, font, position, fore, null, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, 0, origin, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin) { Batch.DrawString(str, font, position, fore, null, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, float layer) { Batch.DrawString(str, font, position, fore, null, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, angle, origin, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin) { Batch.DrawString(str, font, position, fore, back, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, float layer) { Batch.DrawString(str, font, position, fore, back, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, 0, origin, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin) { Batch.DrawString(str, font, position, fore, back, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, float layer) { Batch.DrawString(str, font, position, fore, back, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, angle, origin, Vector2.One, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Vector2 scale) { Batch.DrawString(str, font, position, fore, null, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, null, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, 0, Origin.None, scale, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Vector2 scale) { Batch.DrawString(str, font, position, fore, null, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, null, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, angle, Origin.None, scale, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Vector2 scale) { Batch.DrawString(str, font, position, fore, back, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, back, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, 0, Origin.None, scale, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Vector2 scale) { Batch.DrawString(str, font, position, fore, back, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, back, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, angle, Origin.None, scale, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, Vector2 scale) { Batch.DrawString(str, font, position, fore, null, 0, origin, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, null, 0, origin, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, 0, origin, scale, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, Vector2 scale) { Batch.DrawString(str, font, position, fore, null, angle, origin, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, null, angle, origin, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, null, angle, origin, scale, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, Vector2 scale) { Batch.DrawString(str, font, position, fore, back, 0, origin, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, back, 0, origin, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, 0, origin, scale, effect, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, Vector2 scale) { Batch.DrawString(str, font, position, fore, back, angle, origin, scale, SpriteEffects.None, 0); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, Vector2 scale, float layer) { Batch.DrawString(str, font, position, fore, back, angle, origin, scale, SpriteEffects.None, layer); }
        public static void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Batch.DrawString(str, font, position, fore, back, angle, origin, scale, effect, layer); }

        public static void FillRectangle(Rectangle bounds, Color fill) { Batch.FillRectangle(bounds, fill); }
        public static void FillRectangle(Rectangle bounds, Color fill, float layer) { Batch.FillRectangle(bounds, fill, layer); }
        public static void DrawRectangle(Rectangle bounds, Color stroke, float thickness) { Batch.DrawRectangle(bounds, stroke, thickness); }
        public static void DrawRectangle(Rectangle bounds, Color stroke, float thickness, float layer) { Batch.DrawRectangle(bounds, stroke, thickness, layer); }
        public static void DrawRectangle(Rectangle bounds, Color fill, Color stroke, float thickness) { Batch.DrawRectangle(bounds, fill, stroke, thickness, 0); }
        public static void DrawRectangle(Rectangle bounds, Color fill, Color stroke, float thickness, float layer) { Batch.DrawRectangle(bounds, fill, stroke, thickness, layer); }

        #region Draw Line
        public static void DrawLine(Vector2 start, Vector2 end) => Batch.SpriteBatch.Draw(Batch.WhitePixel, start, null, Color.White, Mathf.Angle(start, end), new Vector2(0, .5f), new Vector2(MathHelper.Max(1, Vector2.Distance(start, end)), 1), 0, 0);
        public static void DrawLine(Vector2 start, Vector2 end, float thickness) => Batch.SpriteBatch.Draw(Batch.WhitePixel, start, null, Color.White, Mathf.Angle(start, end), new Vector2(0, .5f), new Vector2(MathHelper.Max(1, Vector2.Distance(start, end)), thickness), 0, 0);
        public static void DrawLine(Vector2 start, Vector2 end, float thickness, float layer) => Batch.SpriteBatch.Draw(Batch.WhitePixel, start, null, Color.White, Mathf.Angle(start, end), new Vector2(0, .5f), new Vector2(MathHelper.Max(1, Vector2.Distance(start, end)), thickness), 0, layer);
        public static void DrawLine(Vector2 start, Vector2 end, Color color) => Batch.SpriteBatch.Draw(Batch.WhitePixel, start, null, color, Mathf.Angle(start, end), new Vector2(0, .5f), new Vector2(MathHelper.Max(1, Vector2.Distance(start, end)), 1), 0, 0);
        public static void DrawLine(Vector2 start, Vector2 end, Color color, float thickness) => Batch.SpriteBatch.Draw(Batch.WhitePixel, start, null, color, Mathf.Angle(start, end), new Vector2(0, .5f), new Vector2(MathHelper.Max(1, Vector2.Distance(start, end)), thickness), 0, 0);
        public static void DrawLine(Vector2 start, Vector2 end, Color color, float thickness, float layer) => Batch.SpriteBatch.Draw(Batch.WhitePixel, start, null, color, Mathf.Angle(start, end), new Vector2(0, .5f), new Vector2(MathHelper.Max(1, Vector2.Distance(start, end)), thickness), 0, layer);
        #endregion

        #region Draw Pixel
        public static void DrawPixel(Vector2 position) => Batch.SpriteBatch.Draw(Batch.WhitePixel, position, null, Color.White, 0, new Vector2(.5f), Vector2.One, 0, 0);
        public static void DrawPixel(Vector2 position, float layer) => Batch.SpriteBatch.Draw(Batch.WhitePixel, position, null, Color.White, 0, new Vector2(.5f), Vector2.One, 0, layer);
        public static void DrawPixel(Vector2 position, Color color) => Batch.SpriteBatch.Draw(Batch.WhitePixel, position, null, color, 0, new Vector2(.5f), Vector2.One, 0, 0);
        public static void DrawPixel(Vector2 position, Color color, float layer) => Batch.SpriteBatch.Draw(Batch.WhitePixel, position, null, color, 0, new Vector2(.5f), Vector2.One, 0, layer);
        public static void DrawPixel(Vector2 position, Color color, Vector2 scale) => Batch.SpriteBatch.Draw(Batch.WhitePixel, position, null, color, 0, new Vector2(.5f), scale, 0, 0);
        public static void DrawPixel(Vector2 position, Color color, Vector2 scale, float layer) => Batch.SpriteBatch.Draw(Batch.WhitePixel, position, null, color, 0, new Vector2(.5f), scale, 0, layer);
        #endregion
    }
}