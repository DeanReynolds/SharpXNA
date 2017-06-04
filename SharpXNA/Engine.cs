using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SharpXNA
{
    public static class Engine
    {
        public static bool IsActive;
        
        internal static Form _form;
        internal static GameWindow _gameWindow;
        internal static GraphicsDevice _graphicsDevice;
        internal static ContentManager _contentManager;

        public static Assembly Assembly => Assembly.GetExecutingAssembly();
        public static Form Form => _form;
        public static GameWindow Window => _gameWindow;
        public static GraphicsDevice GraphicsDevice => _graphicsDevice;
        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static Viewport Viewport { get { return _graphicsDevice.Viewport; } set { _graphicsDevice.Viewport = value; } }
        public static ContentManager Content => _contentManager;

        public static void Start(GameWindow gameWindow, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            _form = (Form)Control.FromHandle(gameWindow.Handle);
            _gameWindow = gameWindow;
            _graphicsDevice = graphicsDevice;
            Screen._batch = new Batch(_graphicsDevice);
            _contentManager = contentManager;
        }

        public static IEnumerable<string> DirSearch(string path, params string[] exts)
        {
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles().Where(f => exts.Contains(f.Extension)).Select(f => f.FullName).ToList();
            foreach (var d in dir.GetDirectories()) files.AddRange(DirSearch(d.FullName, exts));
            return files;
        }
        public static IEnumerable<string> FilterFiles(string path, params string[] exts) { return Directory.EnumerateFiles(path, "*.*").Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase))); }
    }
}