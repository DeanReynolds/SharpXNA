﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using OggSharp;
using System.Linq;

namespace SharpXNA
{
    public class Sounds
    {
        public static string RootDirectory;

        private static readonly SoundEffectInstance[] Channels;
        private readonly Dictionary<string, SoundEffect> _assets;

        static Sounds() { Channels = new SoundEffectInstance[256]; _kill = new bool[256]; }
        public Sounds() { _assets = new Dictionary<string, SoundEffect>(); }
        /// <param name="rootDirectory">This controls the global Sounds root directory (used for all Sounds classes).</param>
        public Sounds(string rootDirectory) { RootDirectory = rootDirectory; _assets = new Dictionary<string, SoundEffect>(); }

        public SoundEffect this[string path] => _assets[path];
        public SoundEffectInstance this[byte channel] { get { return Channels[channel]; }set { Channels[channel] = value; } }

        public void Add(SoundEffect sound, string path) { _assets.Add(path, sound); }
        public SoundEffect Load(string path)
        {
            using (var fs = new FileStream((@".\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory + "\\" + path), FileMode.Open))
                _assets.Add(path, SoundEffect.FromStream(fs));
            return _assets[path];
        }
        public void LoadAll(string path = null)
        {
            if (path == null) path = Path.GetDirectoryName(Globe.Assembly.Location);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + path.Substring(1));
            var mainPath = (Path.GetDirectoryName(Globe.Assembly.Location) + "\\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory);
            if (!Directory.Exists(path)) return;
            var files = Globe.DirSearch(path, ".wav");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null) continue;
                var name = ((directoryName.Length == mainPath.Length) ? Path.GetFileName(file) : Path.Combine(directoryName.Remove(0, mainPath.Length + 1), Path.GetFileName(file)));
                using (var fs = new FileStream(file, FileMode.Open)) Add(SoundEffect.FromStream(fs), name);
            }
        }
        public void Dispose(string path) { _assets[path].Dispose(); _assets.Remove(path); }

        public void Dispose() { foreach (var t in _assets.Values) t.Dispose(); _assets.Clear(); }
        
        public static void CompileOggs(string path = null)
        {
            if (path == null) path = Path.GetDirectoryName(Globe.Assembly.Location);
            else if (path.StartsWith(".")) path = (Path.GetDirectoryName(Globe.Assembly.Location) + path.Substring(1));
            if (!Directory.Exists(path)) return;
            var files = Globe.DirSearch(path, ".ogg");
            foreach (var file in files)
            {
                var directoryName = Path.GetDirectoryName(file);
                var newFile = (directoryName + "\\" + Path.GetFileNameWithoutExtension(file) + ".wav");
                if (File.Exists(newFile)) continue;
                using (var fs = new FileStream(newFile, FileMode.CreateNew))
                {
                    var decoder = new OggDecoder();
                    decoder.Initialize(new FileStream(file, FileMode.Open));
                    using (var bw = new BinaryWriter(fs)) { WriteWave(bw, (decoder.Stereo ? 2 : 1), decoder.SampleRate, decoder.SelectMany(chunk => chunk.Bytes.Take(chunk.Length)).ToArray()); bw.Close(); }
                    fs.Close();
                }
            }
        }

        private static bool[] _kill;
        public static byte? Play(SoundEffect sound, float volume, bool kill = false) { return Play(sound, false, volume, kill); }
        public static byte? Play(SoundEffect sound, bool loop, bool kill = false) { return Play(sound, loop, 1, kill); }
        public static byte? Play(SoundEffect sound, bool loop, float volume, bool kill = false)
        {
            for (var i = 0; i < Channels.Length; i++)
            {
                if ((Channels[i] != null) || (_kill[i] && (Channels[i].State == SoundState.Stopped))) continue;
                Channels[i] = sound.CreateInstance();
                Channels[i].IsLooped = loop;
                Channels[i].Volume = volume;
                Channels[i].Play();
                _kill[i] = kill;
                return (byte)i;
            }
            return null;
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
    }
}