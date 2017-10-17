using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SharpXNA
{
    public static class Engine
    {
        internal static Game _game;
        internal static Form _form;
        internal static GameWindow _gameWindow;
        internal static GraphicsDevice _graphicsDevice;
        internal static ContentManager _contentManager;

        public static bool IsActive => _game.IsActive;
        public static bool IsMouseVisible
        {
            get { return _game.IsMouseVisible; }
            set { _game.IsMouseVisible = value; }
        }

        public static Assembly Assembly => Assembly.GetExecutingAssembly();
        public static Form Form => _form;
        public static GameWindow Window => _gameWindow;
        public static GraphicsDevice GraphicsDevice => _graphicsDevice;
        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static Viewport Viewport
        {
            get { return _graphicsDevice.Viewport; }
            set { _graphicsDevice.Viewport = value; }
        }
        public static ContentManager Content => _contentManager;

        public static void Start(Game game)
        {
            _game = game;
            _form = (Form)Control.FromHandle(game.Window.Handle);
            _gameWindow = game.Window;
            _graphicsDevice = game.GraphicsDevice;
            Screen._batch = new Batch(_graphicsDevice);
            _contentManager = game.Content;
        }

        public static IEnumerable<string> DirSearch(string path, params string[] exts)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles().Where(f => exts.Contains(f.Extension)).Select(f => f.FullName).ToList();
            foreach (var d in dir.GetDirectories())
                files.AddRange(DirSearch(d.FullName, exts));
            return files;
        }
        public static IEnumerable<string> FilterFiles(string path, params string[] exts) => Directory.EnumerateFiles(path, "*.*").Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)));
    }
}