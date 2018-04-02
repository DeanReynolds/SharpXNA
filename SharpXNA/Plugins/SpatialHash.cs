using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpXNA.Plugins
{
    public class SpatialHash
    {
        private Hashtable _hash;
        private int _size;

        public SpatialHash(int size)
        {
            _hash = new Hashtable();
            _size = size;
        }

        public ICollection Cells => _hash.Keys;
        public int Count => _hash.Count;

        public ArrayList Query(Vector2 position, bool clone = false)
        {
            var key = new Point((int)Math.Round(position.X / _size), (int)Math.Round(position.Y / _size));
            if (_hash.Contains(key))
            {
                if (clone)
                    return (ArrayList)((ArrayList)_hash[key]).Clone();
                return (ArrayList)_hash[key];
            }
            return null;
        }
        public void Add(object obj, Vector2 position)
        {
            foreach (var key in Keys(position))
                if (_hash.Contains(key))
                {
                    var cell = (ArrayList)_hash[key];
                    if (!cell.Contains(obj))
                        cell.Add(obj);
                }
                else
                    _hash.Add(key, new ArrayList() { obj });
        }
        public void Change(object obj, Vector2 oldPosition, Vector2 newPosition)
        {
            HashSet<Point> oldKeys = Keys(oldPosition), newKeys = Keys(newPosition);
            foreach (var key in oldKeys)
                if (!newKeys.Contains(key))
                {
                    if (_hash.Contains(key))
                        ((ArrayList)_hash[key]).Remove(obj);
                }
                else
                    newKeys.Remove(key);
            foreach (var key in newKeys)
                if (_hash.Contains(key))
                {
                    var cell = (ArrayList)_hash[key];
                    if (!cell.Contains(obj))
                        cell.Add(obj);
                }
                else
                    _hash.Add(key, new ArrayList() { obj });
        }
        public void Remove(object obj, Vector2 position)
        {
            foreach (var key in Keys(position))
                if (_hash.Contains(key))
                    ((ArrayList)_hash[key]).Remove(obj);
        }
        public void Clear() { _hash.Clear(); }

        private HashSet<Point> Keys(Vector2 position)
        {
            int px = (int)Math.Round(position.X / _size), pxmh = (px - 1), pxph = (px + 1),
                py = (int)Math.Round(position.Y / _size), pymh = (py - 1), pyph = (py + 1);
            return new HashSet<Point>()
            {
                new Point(px, py), new Point(px, pymh), new Point(px, pyph),
                new Point(pxmh, py), new Point(pxmh, pymh), new Point(pxmh, pyph),
                new Point(pxph, py), new Point(pxph, pymh), new Point(pxph, pyph)
            };
        }
    }
}