using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SharpXNA
{
    public class Pathfinder
    {
        protected const float sqrt2 = 1.4142135f;

        internal Node[,] nodes;
        internal HashSet<Node> open, closed;
        internal Dictionary<Point, Dictionary<Point, Path>> memory;

        public int Width => nodes.GetLength(0);
        public int Height => nodes.GetLength(1);
        public bool MemorizePaths;

        public Pathfinder(int width, int height)
        {
            nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    nodes[x, y] = new Node(x, y);
            open = new HashSet<Node>();
            closed = new HashSet<Node>();
            memory = new Dictionary<Point, Dictionary<Point, Path>>();
        }
        public Node this[int x, int y] { get { return nodes[x, y]; } }

        public Path Find(Point source, Point goal) { return Find(source.X, source.Y, goal.X, goal.Y); }
        public Path Find(int sourceX, int sourceY, int goalX, int goalY)
        {
            if ((sourceX == goalX) && (sourceY == goalY)) return null;
            if (MemorizePaths)
            {
                Point sourceP = new Point(sourceX, sourceY), goalP = new Point(goalX, goalY);
                if (memory.ContainsKey(sourceP) && memory[sourceP].ContainsKey(goalP))
                    return memory[sourceP][goalP].Clone();
                if (memory.ContainsKey(goalP) && memory[goalP].ContainsKey(sourceP))
                    return memory[goalP][sourceP].ReverseClone(goalP);
            }
            Node source = nodes[sourceX, sourceY], goal = nodes[goalX, goalY];
            open.Clear();
            open.Add(source);
            closed.Clear();
            while (open.Count > 0)
            {
                var current = (LowestF(open) ?? source);
                if (current == goal) return Build(source, goal);
                open.Remove(current);
                closed.Add(current);
                var neighbours = Neighbours(current.x, current.y);
                foreach (var n in neighbours)
                {
                    var g = (current.g + Cost(n.x, n.y, current.x, current.y));
                    if (!open.Contains(n)) open.Add(n);
                    else if (g >= n.g) continue;
                    n.parent = current;
                    n.g = g;
                    n.f = (g + (2 * (float)Math.Floor(Math.Sqrt(Math.Pow((n.x - goalX), 2) + Math.Pow((n.y - goalY), 2)))));
                }
            }
            return null;
        }

        internal Node LowestF(HashSet<Node> nodes)
        {
            Node node = null;
            var lowestF = float.MaxValue;
            foreach (var n in nodes) if (n.f < lowestF) { lowestF = n.f; node = n; }
            return node;
        }
        internal HashSet<Node> Neighbours(int x, int y)
        {
            var neighbours = new HashSet<Node>();
            if (((y - 1) >= 0) && nodes[x, (y - 1)].Walkable)
            {
                if (!closed.Contains(nodes[x, (y - 1)])) neighbours.Add(nodes[x, (y - 1)]);
                if (((x - 1) >= 0) && !closed.Contains(nodes[(x - 1), (y - 1)]) && nodes[(x - 1), (y - 1)].Walkable &&
                    nodes[(x - 1), y].Walkable && nodes[x, (y - 1)].Walkable)
                    neighbours.Add(nodes[(x - 1), (y - 1)]);
                if (((x + 1) < Width) && !closed.Contains(nodes[(x + 1), (y - 1)]) && nodes[(x + 1), (y - 1)].Walkable &&
                    nodes[x, (y - 1)].Walkable && nodes[(x + 1), y].Walkable)
                    neighbours.Add(nodes[(x + 1), (y - 1)]);
            }
            if (((y + 1) < Height) && nodes[x, (y + 1)].Walkable)
            {
                if (!closed.Contains(nodes[x, (y + 1)])) neighbours.Add(nodes[x, (y + 1)]);
                if (((x - 1) >= 0) && !closed.Contains(nodes[(x - 1), (y + 1)]) && nodes[(x - 1), (y + 1)].Walkable &&
                    nodes[(x - 1), y].Walkable && nodes[x, (y + 1)].Walkable)
                    neighbours.Add(nodes[(x - 1), (y + 1)]);
                if (((x + 1) < Width) && !closed.Contains(nodes[(x + 1), (y + 1)]) && nodes[(x + 1), (y + 1)].Walkable &&
                    nodes[x, (y + 1)].Walkable && nodes[(x + 1), y].Walkable)
                    neighbours.Add(nodes[(x + 1), (y + 1)]);
            }
            if (((x - 1) >= 0) && !closed.Contains(nodes[(x - 1), y]) && nodes[(x - 1), y].Walkable) neighbours.Add(nodes[(x - 1), y]);
            if (((x + 1) < Width) && !closed.Contains(nodes[(x + 1), y]) && nodes[(x + 1), y].Walkable) neighbours.Add(nodes[(x + 1), y]);
            return neighbours;
        }
        internal float Cost(int sourceX, int sourceY, int goalX, int goalY) { if ((sourceX == goalX) || (sourceY == goalY)) return nodes[sourceX, sourceY].Cost; else return (nodes[sourceX, sourceY].Cost * sqrt2); }
        protected Path Build(Node source, Node goal)
        {
            var path = new Path(2);
            var goalP = new Point(goal.x, goal.y);
            path.Add(goalP);
            var current = goal.parent;
            while (current != source) { path.Insert(0, new Point(current.x, current.y)); current = current.parent; }
            if (!MemorizePaths) return path;
            var sourceP = new Point(source.x, source.y);
            if (memory.ContainsKey(sourceP)) { if (!memory[sourceP].ContainsKey(goalP)) memory[sourceP].Add(goalP, path.Clone()); }
            else { memory.Add(sourceP, new Dictionary<Point, Path>()); memory[sourceP].Add(goalP, path.Clone()); }
            return path;
        }

        public class Node
        {
            internal int x, y;

            internal Node(int x, int y) { this.x = x; this.y = y; }

            internal Node parent;
            internal float g, f;

            public bool Walkable;
            public float Cost = 1;
        }

        public class Path : List<Point>
        {
            public Path() : base() { }
            public Path(int capacity) : base(capacity) { }

            internal Path Clone() { var path = new Path(Count); path.AddRange(this); return path; }
            internal Path ReverseClone(Point sourceP)
            {
                var path = Clone();
                path.Reverse();
                path.RemoveAt(0);
                path.Add(sourceP);
                return path;
            }
        }
    }
}