using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using OggSharp;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SharpXNA
{
    public class Sounds
    {
        public static string RootDirectory;

        private static readonly Instance[] Channels;
        private static bool[] _kill;
        private readonly Dictionary<string, SoundEffect> _assets;

        static Sounds()
        {
            Channels = new Instance[256];
            _kill = new bool[256];
        }
        public Sounds()
        {
            _assets = new Dictionary<string, SoundEffect>();
        }
        public Sounds(string rootDirectory)
        {
            _assets = new Dictionary<string, SoundEffect>();
            RootDirectory = rootDirectory;
        }

        public SoundEffect this[string path] => _assets[path];
        public Instance this[byte channel]
        {
            get { return Channels[channel]; }
            set { Channels[channel] = value; }
        }

        public void Add(SoundEffect sound, string path) { _assets.Add(path, sound); }
        public SoundEffect Load(string path)
        {
            if (Path.GetExtension(path) == ".xnb")
                try
                {
                    var contentFile = (RootDirectory + "\\" + path);
                    contentFile = contentFile.Substring(0, (contentFile.Length - 4));
                    var asset = Engine.Content.Load<SoundEffect>(contentFile);
                    _assets.Add(path, asset);
                }
                catch { }
            else
                using (var fs = new FileStream((@".\" + Engine._contentManager.RootDirectory + "\\" + RootDirectory + "\\" + path), FileMode.Open, FileAccess.Read, FileShare.Read))
                    _assets.Add(path, SoundEffect.FromStream(fs));
            return _assets[path];
        }
        public void LoadAll(string path = null)
        {
            if (path == null)
                path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            else if (path.StartsWith("."))
                path = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory + "\\" + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Engine.Assembly.Location) + "\\" + (!string.IsNullOrEmpty(Engine._contentManager?.RootDirectory) ? (Engine._contentManager.RootDirectory + "\\") : null) + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Engine.DirSearch(path, ".wav");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file)));
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    _assets.Add(name, SoundEffect.FromStream(fs));
            }
            files = Engine.DirSearch(path, ".xnb");
            foreach (var file in files)
            {
                try
                {
                    var directoryName = Path.GetDirectoryName(file);
                    if (directoryName == null)
                        continue;
                    var contentFile = (RootDirectory + "\\" + ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file))));
                    contentFile = contentFile.Substring(0, (contentFile.Length - 4));
                    var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file)));
                    var asset = Engine.Content.Load<SoundEffect>(contentFile);
                    _assets.Add(name, asset);
                }
                catch { }
            }
        }
        public void Dispose() { foreach (var t in _assets.Values) t.Dispose(); _assets.Clear(); }
        public void Dispose(string path) { _assets[path].Dispose(); _assets.Remove(path); }
        
        public static void CompileOggs(string path = null)
        {
            if (path == null)
                path = Path.GetDirectoryName(Engine.Assembly.Location);
            else if (path.StartsWith("."))
                path = (Path.GetDirectoryName(Engine.Assembly.Location) + path.Substring(1));
            if (!Directory.Exists(path)) return;
            var files = Engine.DirSearch(path, ".ogg");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                var newFile = (directoryName + "\\" + Path.GetFileNameWithoutExtension(file) + ".wav");
                if (File.Exists(newFile)) continue;
                using (var fs = new FileStream(newFile, FileMode.CreateNew))
                {
                    var decoder = new OggDecoder();
                    decoder.Initialize(new FileStream(file, FileMode.Open));
                    using (var bw = new BinaryWriter(fs))
                    {
                        WriteWave(bw, (decoder.Stereo ? 2 : 1), decoder.SampleRate, decoder.SelectMany(chunk => chunk.Bytes.Take(chunk.Length)).ToArray());
                        bw.Close();
                    }
                    fs.Close();
                }
            }
        }

        public static byte? Play(SoundEffect sound, float volume, bool kill = false)
        {
            return Play(sound, false, volume, kill);
        }
        public static byte? Play(SoundEffect sound, bool loop, bool kill = false)
        {
            return Play(sound, loop, 1, kill);
        }
        public static byte? Play(SoundEffect sound, bool loop, float volume, bool kill = false)
        {
            if (volume <= 0) return null;
            for (var i = 0; i < Channels.Length; i++)
            {
                if (Channels[i] != null)
                {
                    if (_kill[i])
                    {
                        if (Channels[i]._instance.State == SoundState.Stopped)
                            Channels[i]._instance.Dispose();
                        else continue;
                    }
                    else continue;
                }
                Channels[i] = new Instance(sound.CreateInstance());
                Channels[i]._instance.IsLooped = loop;
                Channels[i]._instance.Volume = volume;
                Channels[i]._instance.Play();
                _kill[i] = kill;
                return (byte)i;
            }
            return null;
        }
        public static byte? Play(SoundEffect sound, float volume, Vector2 position, Vector2 listener, float amplify = 1, bool kill = false)
        {
            return Play(sound, false, volume, position, listener, amplify, kill);
        }
        public static byte? Play(SoundEffect sound, bool loop, Vector2 position, Vector2 listener, float amplify = 1, bool kill = false)
        {
            return Play(sound, loop, 1, position, listener, amplify, kill);
        }
        public static byte? Play(SoundEffect sound, bool loop, float volume, Vector2 position, Vector2 listener, float amplify = 1, bool kill = false)
        {
            if (volume <= 0) return null;
            for (var i = 0; i < Channels.Length; i++)
            {
                if (Channels[i] != null)
                {
                    if (_kill[i])
                    {
                        if (Channels[i]._instance.State == SoundState.Stopped)
                            Channels[i]._instance.Dispose();
                        else continue;
                    }
                    else continue;
                }
                var rAmplify = (1 / amplify);
                Channels[i] = new Instance(sound.CreateInstance(), (position * rAmplify), (listener * rAmplify));
                Channels[i]._instance.IsLooped = loop;
                Channels[i]._instance.Volume = volume;
                Channels[i]._instance.Play();
                _kill[i] = kill;
                return (byte)i;
            }
            return null;
        }

        public static void Kill(byte channel)
        {
            if (Channels[channel] != null)
            {
                Channels[channel]._instance.Stop();
                Channels[channel]._instance.Dispose();
                Channels[channel] = null;
            }
        }
        private static void WriteWave(BinaryWriter writer, int channels, int rate, byte[] data)
        {
            writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write((int)(36 + data.Length));
            writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[4] { 'f', 'm', 't', ' ' });
            writer.Write((int)16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write((int)rate);
            writer.Write((int)(rate * ((16 * channels) / 8)));
            writer.Write((short)((16 * channels) / 8));
            writer.Write((short)16);
            writer.Write(new char[4] { 'd', 'a', 't', 'a' });
            writer.Write((int)data.Length);
            writer.Write(data);
        }

        public class Instance
        {
            internal SoundEffectInstance _instance;

            internal AudioListener _listener;
            internal AudioEmitter _emitter;

            public Instance(SoundEffectInstance instance)
            {
                _instance = instance;
            }
            public Instance(SoundEffectInstance instance, Vector2 position, Vector2 listener)
            {
                _instance = instance;
                _listener = new AudioListener();
                _listener.Position = new Vector3(listener.X, 0, listener.Y);
                _emitter = new AudioEmitter();
                _emitter.Position = new Vector3(position.X, 0, position.Y);
                _instance.Apply3D(_listener, _emitter);
            }

            public SoundState State
            {
                get { return _instance.State; }
            }
            public bool IsLooped
            {
                get { return _instance.IsLooped; }
                set { _instance.IsLooped = value; }
            }
            public float Volume
            {
                get { return _instance.Volume; }
                set { _instance.Volume = value; }
            }
        }
    }
}