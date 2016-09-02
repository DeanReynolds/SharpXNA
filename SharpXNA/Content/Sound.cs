using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using OggSharp;
using Microsoft.Xna.Framework;
using System.Linq;

namespace SharpXNA
{
    public static class Sound
    {
        public static string RootDirectory;

        public static SoundEffect Load(string path)
        {
            if (!Globe.ContentManager.Loaded<SoundEffect>(path))
                try { Globe.ContentManager.Save<SoundEffect>(path, Globe.ContentManager.Load<SoundEffect>(RootDirectory + "\\" + path)); }
                catch { return null; }
            return Globe.ContentManager.Load<SoundEffect>(path);
        }
        public static SoundEffect LoadRaw(string path, string extension = "wav")
        {
            string path1 = (path + "." + extension);
            if (!Globe.ContentManager.Loaded<SoundEffect>(path1))
                try { FileStream fs = new FileStream((@".\" + Globe.ContentManager.RootDirectory + "\\" + RootDirectory + "\\" + path1), FileMode.Open); Globe.ContentManager.Save<SoundEffect>(path1, SoundEffect.FromStream(fs)); fs.Close(); return Globe.ContentManager.Load<SoundEffect>(path1); }
                catch { return null; }
            else return Globe.ContentManager.Load<SoundEffect>(path1);
        }
        public static bool Loaded(string path) { return Globe.ContentManager.Loaded<SoundEffect>(path); }
        public static bool Unload(string path) { return Globe.ContentManager.Unload<SoundEffect>(path); }

        public static SoundEffectInstance[] Channels;
        private static readonly Dictionary<string, SoundEffect> Library = new Dictionary<string, SoundEffect>();
        private static bool[] autoTerminate;

        public static void Initialize(int Channels)
        {
            Sound.Channels = new SoundEffectInstance[Channels];
            autoTerminate = new bool[Channels];
        }
        public static void CompileOggs(string dir = null)
        {
            string path = (@".\" + Globe.ContentManager.RootDirectory + "\\" + (!string.IsNullOrEmpty(RootDirectory) ? (!string.IsNullOrEmpty(RootDirectory) ? (RootDirectory +
                (!RootDirectory.EndsWith("\\") ? "\\" : string.Empty)) : string.Empty) : string.Empty) + dir);
            if (Directory.Exists(path))
            {
                var files = DirSearch(path, ".ogg");
                foreach (var file in files)
                {
                    var directoryName = Path.GetDirectoryName(file);
                    var newFile = (directoryName + "\\" + Path.GetFileNameWithoutExtension(file) + ".wav");
                    if (File.Exists(newFile)) continue;
                    using (FileStream fs = new FileStream(newFile, FileMode.CreateNew))
                    {
                        var decoder = new OggDecoder();
                        decoder.Initialize(new FileStream(file, FileMode.Open));
                        using (BinaryWriter bw = new BinaryWriter(fs)) { WriteWave(bw, (decoder.Stereo ? 2 : 1), decoder.SampleRate, decoder.SelectMany(chunk => chunk.Bytes.Take(chunk.Length)).ToArray()); bw.Close(); }
                        fs.Close();
                    }
                }
            }
            else Console.WriteLine($"Directory {path} does not exist.");
        }
        private static IEnumerable<string> DirSearch(string directory, params string[] extensions)
        {
            var dir = new DirectoryInfo(directory);
            var files = dir.GetFiles().Where(f => extensions.Contains(f.Extension)).Select(f => f.FullName).ToList();
            foreach (var d in dir.GetDirectories()) files.AddRange(DirSearch(d.FullName, extensions));
            return files;
        }

        public static ushort? Play(string path, bool autoTerminate = true) { return Play(path, false, 1, autoTerminate); }
        public static ushort? Play(string path, float volume, bool autoTerminate = true) { return Play(path, false, volume, autoTerminate); }
        public static ushort? Play(string path, bool loop, bool autoTerminate = true) { return Play(path, loop, 1, autoTerminate); }
        public static ushort? Play(string path, bool loop, float volume, bool autoTerminate = true)
        {
            var sound = Load(path);
            if (sound != null)
                for (int i = 0; i < Channels.Length; i++)
                {
                    if (Channels[i] != null) continue;
                    Channels[i] = sound.CreateInstance();
                    Channels[i].IsLooped = loop;
                    Channels[i].Volume = volume;
                    Channels[i].Play();
                    Sound.autoTerminate[i] = (autoTerminate && !loop);
                    return (ushort)i;
                }
            return null;
        }

        public static ushort? PlayRaw(string path, bool autoTerminate = true) { return PlayRaw(path, false, 1, autoTerminate); }
        public static ushort? PlayRaw(string path, float volume, bool autoTerminate = true) { return PlayRaw(path, false, volume, autoTerminate); }
        public static ushort? PlayRaw(string path, bool loop, bool autoTerminate = true) { return PlayRaw(path, loop, 1, autoTerminate); }
        public static ushort? PlayRaw(string path, bool loop, float volume, bool autoTerminate = true)
        {
            SoundEffect sound = LoadRaw(path);
            if (sound != null)
                for (int i = 0; i < Channels.Length; i++)
                {
                    if (Channels[i] != null) continue;
                    Channels[i] = sound.CreateInstance();
                    Channels[i].IsLooped = loop;
                    Channels[i].Volume = volume;
                    Channels[i].Play();
                    Sound.autoTerminate[i] = (autoTerminate && !loop);
                    return (ushort)i;
                }
            return null;
        }

        public static void AutoTerminate()
        {
            for (int i = 0; i < autoTerminate.Length; i++)
                if (autoTerminate[i])
                {
                    if (Channels[i] == null) autoTerminate[i] = false;
                    else if (Channels[i].State == SoundState.Stopped) { Terminate((ushort)i); autoTerminate[i] = false; }
                }
        }

        public static bool Resume(ushort channel) { if ((Channels[channel] != null) && (Channels[channel].State != SoundState.Playing)) { Channels[channel].Resume(); return true; } else return false; }
        public static bool Pause(ushort channel) { if ((Channels[channel] != null) && (Channels[channel].State != SoundState.Paused)) { Channels[channel].Pause(); return true; } else return false; }
        public static bool IsPaused(ushort channel) { if (Channels[channel] != null) return (Channels[channel].State == SoundState.Paused); else return false; }
        public static bool Stop(ushort channel) { if (Channels[channel] != null) { Channels[channel].Stop(); return true; } else return false; }
        public static bool Terminate(ushort channel) { if (Channels[channel] != null) { Channels[channel].Stop(); Channels[channel] = null; return true; } else return false; }
        public static bool IsStopped(ushort channel) { if (Channels[channel] != null) return (Channels[channel].State == SoundState.Stopped); else return false; }
        public static bool? Looped(ushort channel) { if (Channels[channel] != null) return Channels[channel].IsLooped; else return null; }
        public static bool SetLooped(ushort channel, bool value) { if (Channels[channel] != null) { Channels[channel].IsLooped = value; return true; } else return false; }
        public static float? Volume(ushort channel) { if (Channels[channel] != null) return Channels[channel].Volume; else return null; }
        public static bool SetVolume(ushort channel, float value) { if (Channels[channel] != null) { Channels[channel].Volume = value; return true; } else return false; }

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