using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SharpXNA
{
    public class Batch
    {
        internal readonly SpriteBatch SpriteBatch;
        internal SpriteSortMode SortMode;
        public GraphicsDevice GraphicsDevice => SpriteBatch.GraphicsDevice;

        internal static Texture2D WhitePixel;

        public bool Begun;

        public Batch(GraphicsDevice graphicsDevice)
        {
            WhitePixel = new Texture2D(graphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });
            SpriteBatch = new SpriteBatch(graphicsDevice);
        }
        
        public void Begin(Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, Matrix? matrix = null) { Begin(sortMode, null, null, null, null, null, matrix); }
        public void Begin(BlendState blendState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, null, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, Matrix? matrix = null) { Begin(sortMode, blendState, null, null, null, null, matrix); }
        public void Begin(SamplerState samplerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, null, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, Matrix? matrix = null) { Begin(sortMode, null, samplerState, null, null, null, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, null, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, Matrix? matrix = null) { Begin(sortMode, blendState, samplerState, null, null, null, matrix); }
        public void Begin(DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, depthStencilState, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(sortMode, null, null, depthStencilState, null, null, matrix); }
        public void Begin(BlendState blendState, DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(sortMode, blendState, null, depthStencilState, null, null, matrix); }
        public void Begin(SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(sortMode, null, samplerState, depthStencilState, null, null, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, null, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Matrix? matrix = null) { Begin(sortMode, blendState, samplerState, depthStencilState, null, null, matrix); }
        public void Begin(RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, null, null, null, rasterizerState, null, matrix); }
        public void Begin(BlendState blendState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, null, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, blendState, null, null, rasterizerState, null, matrix); }
        public void Begin(SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, null, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, null, samplerState, null, rasterizerState, null, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, null, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, blendState, samplerState, null, rasterizerState, null, matrix); }
        public void Begin(DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, null, null, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, blendState, null, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, null, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, matrix); }
        public void Begin(Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, null, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, null, null, null, effect, matrix); }
        public void Begin(BlendState blendState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, null, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, Effect effect, Matrix? matrix = null) { Begin(sortMode, blendState, null, null, null, effect, matrix); }
        public void Begin(SamplerState samplerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, null, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, samplerState, null, null, effect, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, null, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, blendState, samplerState, null, null, effect, matrix); }
        public void Begin(DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, depthStencilState, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, null, depthStencilState, null, effect, matrix); }
        public void Begin(BlendState blendState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(sortMode, blendState, null, depthStencilState, null, effect, matrix); }
        public void Begin(SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, samplerState, depthStencilState, null, effect, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, null, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, Effect effect, Matrix? matrix = null) { Begin(sortMode, blendState, samplerState, depthStencilState, null, effect, matrix); }
        public void Begin(RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, null, null, rasterizerState, effect, matrix); }
        public void Begin(BlendState blendState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, null, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, blendState, null, null, rasterizerState, effect, matrix); }
        public void Begin(SamplerState samplerState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, null, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, samplerState, null, rasterizerState, effect, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, null, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, Effect effect, RasterizerState rasterizerState, Matrix? matrix = null) { Begin(sortMode, blendState, samplerState, null, rasterizerState, effect, matrix); }
        public void Begin(DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, null, depthStencilState, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, null, depthStencilState, rasterizerState, effect, matrix); }
        public void Begin(BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, null, depthStencilState, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, blendState, null, depthStencilState, rasterizerState, effect, matrix); }
        public void Begin(SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, null, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(sortMode, null, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? matrix = null) { SortMode = sortMode; Begun = true; SpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix); }
        public void End() { Begun = false; SpriteBatch.End(); }
        
        public void Draw(Texture2D texture, Vector2 position) { Draw(texture, position, null, Color.White, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, 0, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source) { Draw(texture, position, source, Color.White, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, 0, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint) { Draw(texture, position, null, tint, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, 0, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint) { Draw(texture, position, source, tint, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, SpriteEffects effect, float layer) { Draw(texture, position, source, tint, 0, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds) { Draw(texture, bounds, null, Color.White, 0, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, SpriteEffects effect, float layer) { Draw(texture, bounds, null, Color.White, 0, Origin.None, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source) { Draw(texture, bounds, source, Color.White, 0, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, SpriteEffects effect, float layer) { Draw(texture, bounds, source, Color.White, 0, Origin.None, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Vector2 scale) { Draw(texture, position, null, Color.White, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Vector2 scale, float layer) { Draw(texture, position, null, Color.White, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, 0, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Vector2 scale) { Draw(texture, position, source, Color.White, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Vector2 scale, float layer) { Draw(texture, position, source, Color.White, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, 0, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint) { Draw(texture, bounds, null, tint, 0, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, SpriteEffects effect, float layer) { Draw(texture, bounds, null, tint, 0, Origin.None, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint) { Draw(texture, bounds, source, tint, 0, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, SpriteEffects effect, float layer) { SpriteBatch.Draw(texture, bounds, source, tint, 0, Vector2.Zero, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Vector2 scale) { Draw(texture, position, null, tint, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Vector2 scale, float layer) { Draw(texture, position, null, tint, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, 0, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Vector2 scale) { Draw(texture, position, source, tint, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Vector2 scale, float layer) { Draw(texture, position, source, tint, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, source, tint, 0, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Origin origin) { Draw(texture, position, null, Color.White, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Origin origin, float layer) { Draw(texture, position, null, Color.White, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, 0, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin) { Draw(texture, position, source, Color.White, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, float layer) { Draw(texture, position, source, Color.White, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, 0, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin) { Draw(texture, position, null, tint, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, float layer) { Draw(texture, position, null, tint, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, 0, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin) { Draw(texture, position, source, tint, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, float layer) { Draw(texture, position, source, tint, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, source, tint, 0, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Origin origin) { Draw(texture, bounds, null, Color.White, 0, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Origin origin, float layer) { Draw(texture, bounds, null, Color.White, 0, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Origin origin, SpriteEffects effect, float layer) { Draw(texture, bounds, null, Color.White, 0, origin, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Origin origin) { Draw(texture, bounds, source, Color.White, 0, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Origin origin, float layer) { Draw(texture, bounds, source, Color.White, 0, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Origin origin, SpriteEffects effect, float layer) { Draw(texture, bounds, source, Color.White, 0, origin, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Origin origin, Vector2 scale) { Draw(texture, position, null, Color.White, 0, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Origin origin, Vector2 scale, float layer) { Draw(texture, position, null, Color.White, 0, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, 0, origin, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, Vector2 scale) { Draw(texture, position, source, Color.White, 0, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, Vector2 scale, float layer) { Draw(texture, position, source, Color.White, 0, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, 0, origin, scale, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, Origin origin) { Draw(texture, bounds, null, tint, 0, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, Origin origin, float layer) { Draw(texture, bounds, null, tint, 0, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, Origin origin, SpriteEffects effect, float layer) { Draw(texture, bounds, null, tint, 0, origin, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, Origin origin) { Draw(texture, bounds, source, tint, 0, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, Origin origin, float layer) { Draw(texture, bounds, source, tint, 0, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, Origin origin, SpriteEffects effect, float layer) { Draw(texture, bounds, source, tint, 0, origin, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, Vector2 scale) { Draw(texture, position, null, tint, 0, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, Vector2 scale, float layer) { Draw(texture, position, null, tint, 0, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, 0, origin, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, Vector2 scale) { Draw(texture, position, source, tint, 0, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, Vector2 scale, float layer) { Draw(texture, position, source, tint, 0, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, source, tint, 0, origin, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle) { Draw(texture, position, null, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, float angle, float layer) { Draw(texture, position, null, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, angle, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle) { Draw(texture, position, source, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, float layer) { Draw(texture, position, source, Color.White, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, angle, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle) { Draw(texture, position, null, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, float layer) { Draw(texture, position, null, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, angle, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle) { Draw(texture, position, source, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, float layer) { Draw(texture, position, source, tint, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, SpriteEffects effect, float layer) { Draw(texture, position, source, tint, angle, Origin.None, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, float angle) { Draw(texture, bounds, null, Color.White, angle, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, float angle, float layer) { Draw(texture, bounds, null, Color.White, angle, Origin.None, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, float angle, SpriteEffects effect, float layer) { Draw(texture, bounds, null, Color.White, angle, Origin.None, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle) { Draw(texture, bounds, source, Color.White, angle, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, float layer) { Draw(texture, bounds, source, Color.White, angle, Origin.None, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, SpriteEffects effect, float layer) { Draw(texture, bounds, source, Color.White, angle, Origin.None, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Vector2 scale) { Draw(texture, position, null, Color.White, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Vector2 scale, float layer) { Draw(texture, position, null, Color.White, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, angle, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Vector2 scale) { Draw(texture, position, source, Color.White, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Vector2 scale, float layer) { Draw(texture, position, source, Color.White, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, angle, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle) { Draw(texture, bounds, null, tint, angle, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, float layer) { Draw(texture, bounds, null, tint, angle, Origin.None, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, SpriteEffects effect, float layer) { Draw(texture, bounds, null, tint, angle, Origin.None, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle) { Draw(texture, bounds, source, tint, angle, Origin.None, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, float layer) { Draw(texture, bounds, source, tint, angle, Origin.None, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, SpriteEffects effect, float layer) { SpriteBatch.Draw(texture, bounds, source, tint, angle, Vector2.Zero, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Vector2 scale) { Draw(texture, position, null, tint, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Vector2 scale, float layer) { Draw(texture, position, null, tint, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, angle, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Vector2 scale) { Draw(texture, position, source, tint, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Vector2 scale, float layer) { Draw(texture, position, source, tint, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, source, tint, angle, Origin.None, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Origin origin) { Draw(texture, position, null, Color.White, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, float layer) { Draw(texture, position, null, Color.White, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, angle, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin) { Draw(texture, position, source, Color.White, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, float layer) { Draw(texture, position, source, Color.White, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, angle, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin) { Draw(texture, position, null, tint, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, float layer) { Draw(texture, position, null, tint, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, angle, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin) { Draw(texture, position, source, tint, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, float layer) { Draw(texture, position, source, tint, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { Draw(texture, position, source, tint, angle, origin, Vector2.One, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, float angle, Origin origin) { Draw(texture, bounds, null, Color.White, angle, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, float angle, Origin origin, float layer) { Draw(texture, bounds, null, Color.White, angle, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, float angle, Origin origin, SpriteEffects effect, float layer) { Draw(texture, bounds, null, Color.White, angle, origin, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, Origin origin) { Draw(texture, bounds, source, Color.White, angle, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, Origin origin, float layer) { Draw(texture, bounds, source, Color.White, angle, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, float angle, Origin origin, SpriteEffects effect, float layer) { Draw(texture, bounds, source, Color.White, angle, origin, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, Vector2 scale) { Draw(texture, position, null, Color.White, angle, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, Vector2 scale, float layer) { Draw(texture, position, null, Color.White, angle, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, Color.White, angle, origin, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, Vector2 scale) { Draw(texture, position, source, Color.White, angle, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, Vector2 scale, float layer) { Draw(texture, position, source, Color.White, angle, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, source, Color.White, angle, origin, scale, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, Origin origin) { Draw(texture, bounds, null, tint, angle, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, Origin origin, float layer) { Draw(texture, bounds, null, tint, angle, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { Draw(texture, bounds, null, tint, angle, origin, effect, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, Origin origin) { Draw(texture, bounds, source, tint, angle, origin, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, Origin origin, float layer) { Draw(texture, bounds, source, tint, angle, origin, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Rectangle bounds, Rectangle? source, Color tint, float angle, Origin origin, SpriteEffects effect, float layer) { origin.Apply(texture, source); SpriteBatch.Draw(texture, bounds, source, tint, angle, origin.Value, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, Vector2 scale) { Draw(texture, position, null, tint, angle, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, Vector2 scale, float layer) { Draw(texture, position, null, tint, angle, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Color tint, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { Draw(texture, position, null, tint, angle, origin, scale, effect, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, Vector2 scale) { Draw(texture, position, source, tint, angle, origin, scale, SpriteEffects.None, 0); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, Vector2 scale, float layer) { Draw(texture, position, source, tint, angle, origin, scale, SpriteEffects.None, layer); }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color tint, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { origin.Apply(texture, source); SpriteBatch.Draw(texture, position, source, tint, angle, origin.Value, scale, effect, layer); }

        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore) { DrawString(str, font, position, fore, null, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, 0, Origin.None, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle) { DrawString(str, font, position, fore, null, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, float layer) { DrawString(str, font, position, fore, null, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, angle, Origin.None, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back) { DrawString(str, font, position, fore, back, 0, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, back, 0, Origin.None, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle) { DrawString(str, font, position, fore, back, angle, Origin.None, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, float layer) { DrawString(str, font, position, fore, back, angle, Origin.None, Vector2.One, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, back, angle, Origin.None, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin) { DrawString(str, font, position, fore, null, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, float layer) { DrawString(str, font, position, fore, null, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, 0, origin, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin) { DrawString(str, font, position, fore, null, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, float layer) { DrawString(str, font, position, fore, null, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, angle, origin, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin) { DrawString(str, font, position, fore, back, 0, origin, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, float layer) { DrawString(str, font, position, fore, back, 0, origin, Vector2.One, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, back, 0, origin, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin) { DrawString(str, font, position, fore, back, angle, origin, Vector2.One, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, float layer) { DrawString(str, font, position, fore, back, angle, origin, Vector2.One, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, back, angle, origin, Vector2.One, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Vector2 scale) { DrawString(str, font, position, fore, null, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Vector2 scale, float layer) { DrawString(str, font, position, fore, null, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Vector2 scale, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, 0, Origin.None, scale, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Vector2 scale) { DrawString(str, font, position, fore, null, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Vector2 scale, float layer) { DrawString(str, font, position, fore, null, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Vector2 scale, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, angle, Origin.None, scale, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Vector2 scale) { DrawString(str, font, position, fore, back, 0, Origin.None, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Vector2 scale, float layer) { DrawString(str, font, position, fore, back, 0, Origin.None, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Vector2 scale, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, back, 0, Origin.None, scale, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Vector2 scale) { DrawString(str, font, position, fore, back, angle, Origin.None, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Vector2 scale, float layer) { DrawString(str, font, position, fore, back, angle, Origin.None, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Vector2 scale, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, back, angle, Origin.None, scale, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, Vector2 scale) { DrawString(str, font, position, fore, null, 0, origin, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, Vector2 scale, float layer) { DrawString(str, font, position, fore, null, 0, origin, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, 0, origin, scale, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, Vector2 scale) { DrawString(str, font, position, fore, null, angle, origin, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, Vector2 scale, float layer) { DrawString(str, font, position, fore, null, angle, origin, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, null, angle, origin, scale, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, Vector2 scale) { DrawString(str, font, position, fore, back, 0, origin, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, Vector2 scale, float layer) { DrawString(str, font, position, fore, back, 0, origin, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, Origin origin, Vector2 scale, SpriteEffects effect, float layer) { DrawString(str, font, position, fore, back, 0, origin, scale, effect, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, Vector2 scale) { DrawString(str, font, position, fore, back, angle, origin, scale, SpriteEffects.None, 0); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, Vector2 scale, float layer) { DrawString(str, font, position, fore, back, angle, origin, scale, SpriteEffects.None, layer); }
        public void DrawString(string str, SpriteFont font, Vector2 position, Color fore, Color? back, float angle, Origin origin, Vector2 scale, SpriteEffects effect, float layer)
        {
            if (origin.Scaled) { var size = font.MeasureString(str); origin.Value = new Vector2((size.X * origin.Value.X), (size.Y * origin.Value.Y)); origin.Scaled = false; }
            if (back.HasValue) SpriteBatch.DrawString(font, str, (position + Vector2.One), back.Value, angle, origin.Value, scale, effect, (layer + ((SortMode == SpriteSortMode.BackToFront) ? .000001f : (SortMode == SpriteSortMode.FrontToBack) ? -.000001f : 0)));
            SpriteBatch.DrawString(font, str, position, fore, angle, origin.Value, scale, effect, layer);
        }

        public void FillRectangle(Rectangle bounds, Color fill) { Draw(WhitePixel, bounds, fill, 0, Origin.None, 0); }
        public void FillRectangle(Rectangle bounds, Color fill, float layer) { Draw(WhitePixel, bounds, fill, 0, Origin.None, layer); }
        public void DrawRectangle(Rectangle bounds, Color stroke, float thickness) { DrawRectangle(bounds, stroke, thickness, 0); }
        public void DrawRectangle(Rectangle bounds, Color stroke, float thickness, float layer)
        {
            Draw(WhitePixel, new Vector2((bounds.X + bounds.Width), bounds.Y), stroke, 0, new Origin(.5f, 0, true), new Vector2(thickness, bounds.Height), layer);
            Draw(WhitePixel, new Vector2((bounds.X + bounds.Width), (bounds.Y + bounds.Height)), stroke, MathHelper.ToRadians(180), new Origin(0, .5f, true), new Vector2(bounds.Width, thickness), layer);
            Draw(WhitePixel, new Vector2(bounds.X, (bounds.Y + bounds.Height)), stroke, MathHelper.ToRadians(180), new Origin(.5f, 0, true), new Vector2(thickness, bounds.Height), layer);
            Draw(WhitePixel, new Vector2(bounds.X, bounds.Y), stroke, 0, new Origin(0, .5f, true), new Vector2(bounds.Width, thickness), layer);
        }
        public void DrawRectangle(Rectangle bounds, Color fill, Color stroke, float thickness) { DrawRectangle(bounds, fill, stroke, thickness, 0); }
        public void DrawRectangle(Rectangle bounds, Color fill, Color stroke, float thickness, float layer)
        {
            Draw(WhitePixel, bounds, fill, 0, Origin.None, layer);
            Draw(WhitePixel, new Vector2((bounds.X + bounds.Width), bounds.Y), stroke, 0, new Origin(.5f, 0, true), new Vector2(thickness, bounds.Height), layer);
            Draw(WhitePixel, new Vector2((bounds.X + bounds.Width), (bounds.Y + bounds.Height)), stroke, MathHelper.ToRadians(180), new Origin(0, .5f, true), new Vector2(bounds.Width, thickness), layer);
            Draw(WhitePixel, new Vector2(bounds.X, (bounds.Y + bounds.Height)), stroke, MathHelper.ToRadians(180), new Origin(.5f, 0, true), new Vector2(thickness, bounds.Height), layer);
            Draw(WhitePixel, new Vector2(bounds.X, bounds.Y), stroke, 0, new Origin(0, .5f, true), new Vector2(bounds.Width, thickness), layer);
        }

        public void DrawLine(Vector2 start, Vector2 end) { DrawLine(start, end, Color.White, 1, SpriteEffects.None, 0); }
        public void DrawLine(Vector2 start, Vector2 end, float thickness) { DrawLine(start, end, Color.White, thickness, SpriteEffects.None, 0); }
        public void DrawLine(Vector2 start, Vector2 end, float thickness, float layer) { DrawLine(start, end, Color.White, thickness, SpriteEffects.None, layer); }
        public void DrawLine(Vector2 start, Vector2 end, SpriteEffects effect) { DrawLine(start, end, Color.White, 1, effect, 0); }
        public void DrawLine(Vector2 start, Vector2 end, SpriteEffects effect, float layer) { DrawLine(start, end, Color.White, 1, effect, layer); }
        public void DrawLine(Vector2 start, Vector2 end, Color color) { DrawLine(start, end, color, 1, SpriteEffects.None, 0); }
        public void DrawLine(Vector2 start, Vector2 end, Color color, SpriteEffects effect) { DrawLine(start, end, color, 1, effect, 0); }
        public void DrawLine(Vector2 start, Vector2 end, Color color, SpriteEffects effect, float layer) { DrawLine(start, end, color, 1, effect, layer); }
        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness) { DrawLine(start, end, color, thickness, SpriteEffects.None, 0); }
        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness, float layer) { DrawLine(start, end, color, thickness, SpriteEffects.None, layer); }
        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness, SpriteEffects effect) { DrawLine(start, end, color, thickness, effect, 0); }
        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness, SpriteEffects effect, float layer) { Draw(WhitePixel, start, null, color, Globe.Angle(start, end), new Origin(0, .5f), new Vector2(MathHelper.Max(1, Vector2.Distance(start, end)), thickness), effect, layer); }
    }
}