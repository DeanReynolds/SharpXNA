using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static SharpXNA.Textures;

namespace SharpXNA
{
    public class Animation
    {
        public Frame[] Frames;
        public uint Index;
        public bool Loop, Paused;
        public Origin Origins = Textures.Origin.None;
        public float Speed;
        private double Timer;

        public bool Finished { get { return ((Index == (Frames.Length - 1)) && (Timer >= (Frames[Index].Speed.HasValue ? Frames[Index].Speed.Value : Speed))); } }

        public Animation(Texture2D texture, Origin origin = null) { Frames = new Frame[1]; Frames[0] = new Frame(texture, origin); }
        public Animation(string path, int frames, bool loop = false, float? speed = null)
        {
            Frames = new Frame[frames];
            for (var i = 0; i < frames; i++)
            {
                var Texture = Load(path + ((path.EndsWith(".") || path.EndsWith("-") || path.EndsWith("_")) ? string.Empty : "\\") + i);
                Frames[i] = new Frame(Texture, Textures.Origin.Center);
            }
            Loop = loop;
            if (speed.HasValue) Speed = speed.Value;
        }
        public Animation(string path, int frames, bool loop = false, float? speed = null, params Origin[] origins)
        {
            Frames = new Frame[frames];
            for (var i = 0; i < frames; i++)
            {
                var texture = Load(path + ((path.EndsWith(".") || path.EndsWith("-") || path.EndsWith("_")) ? string.Empty : "\\") + i);
                var origin = ((origins.Length > i) ? Textures.Origin.Center : origins[i]);
                Frames[i] = new Frame(texture, origin);
            }
            Loop = loop;
            if (speed.HasValue) Speed = speed.Value;
        }

        public void Override(Origin origin) { for (var i = 0; i < Frames.Length; i++) Frames[i].Origin = null; Origins = origin; }
        public void Apply(Origin origin) { for (var i = 0; i < Frames.Length; i++) Frames[i].Origin = origin.Clone(); }

        public void Update(GameTime time)
        {
            if (!Paused)
                if (Frames[Index].Speed.HasValue)
                {
                    if (Timer < Frames[Index].Speed) Timer += (time.ElapsedGameTime.TotalSeconds * Globe.Speed);
                    else
                    {
                        if (Index < (Frames.Length - 1)) { Index++; Timer -= Frames[Index].Speed.Value; }
                        else if (Loop) { Timer -= Frames[Index].Speed.Value; Index = 0; }
                    }
                }
                else
                {
                    if (Timer < Speed) Timer += (time.ElapsedGameTime.TotalSeconds * Globe.Speed);
                    else
                    {
                        if (Index < (Frames.Length - 1)) { Index++; Timer -= Speed; }
                        else if (Loop) { Index = 0; Timer -= Speed; }
                    }
                }
        }

        public void Draw(Vector2 position, Color color, float angle, SpriteEffects effect = SpriteEffects.None, float layer = 0) { Draw(Screen.Batch, position, color, angle, 1, effect, layer); }
        public void Draw(Vector2 position, Color color, float angle, float scale, SpriteEffects effect = SpriteEffects.None, float layer = 0) { Draw(Screen.Batch, position, color, angle, scale, effect, layer); }
        public void Draw(Vector2 position, Color color, float angle, Vector2 scale, SpriteEffects effect = SpriteEffects.None, float layer = 0) { Draw(Screen.Batch, position, color, angle, scale, effect, layer); }
        public void Draw(Batch batch, Vector2 position, Color color, float angle, SpriteEffects effect = SpriteEffects.None, float layer = 0) { Draw(batch, position, color, angle, Vector2.One, effect, layer); }
        public void Draw(Batch batch, Vector2 position, Color color, float angle, float scale, SpriteEffects effect = SpriteEffects.None, float layer = 0) { Draw(batch, position, color, angle, new Vector2(scale), effect, layer); }
        public void Draw(Batch batch, Vector2 position, Color color, float angle, Vector2 scale, SpriteEffects effect = SpriteEffects.None, float layer = 0) { batch.Draw(Texture(), position, null, color, angle, Origin(), scale, effect, layer); }

        public Texture2D Texture(uint? index = null)
        {
            if (Frames == null) return null;
            if (index == null) { if (Frames.Length > Index) return Frames[Index].Texture; }
            else if (Frames.Length > index) return Frames[(uint)index].Texture;
            return null;
        }
        public Origin Origin(uint? index = null)
        {
            if (Frames == null) return Textures.Origin.None;
            if (index == null)
            {
                if (Frames.Length > Index)
                    if (Frames[Index].Origin != null) return new Origin(Frames[Index].Origin.Peek(Frames[Index].Texture));
                    else return (Origins ?? Textures.Origin.Center);
            }
            else if (Frames.Length > index) return (Frames[index.Value].Origin ?? (Origins ?? Textures.Origin.Center));
            return null;
        }

        public class Frame
        {
            public Origin Origin = Origin.None;
            public float? Speed;
            public Texture2D Texture;

            public Frame(Origin origin) { Origin = origin; }
            public Frame(Texture2D texture) { Texture = texture; }
            public Frame(Texture2D texture, float speed) { Texture = texture; Speed = speed; }
            public Frame(float speed, Origin origin = null) { Speed = speed; Origin = (origin ?? Origin.Center); }
            public Frame(Texture2D texture, Origin origin = null) { Texture = texture; Origin = origin; }
            public Frame(Texture2D texture, float speed, Origin origin = null) { Texture = texture; Speed = speed; Origin = origin; }
        }
    }
}