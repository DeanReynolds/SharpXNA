using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SharpXNA.Plugins
{
    public class Pathfinder
    {
        internal Node[,] Nodes;
        internal PriorityQueue<Node> Open;
        internal HashSet<Node> Closed;
        internal Dictionary<Point, Dictionary<Point, Path>> Memory;

        public int Width => Nodes.GetLength(0);
        public int Height => Nodes.GetLength(1);
        public bool MemorizePaths;

        public Pathfinder(int width, int height)
        {
            Nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Nodes[x, y] = new Node(x, y);
            Open = new PriorityQueue<Node>();
            Closed = new HashSet<Node>();
            Memory = new Dictionary<Point, Dictionary<Point, Path>>();
        }
        public Node this[int x, int y] { get { return Nodes[x, y]; } }

        public Path Find(Point source, Point goal) { return Find(source.X, source.Y, goal.X, goal.Y); }
        public Path Find(int sourceX, int sourceY, int goalX, int goalY)
        {
            if ((sourceX == goalX) && (sourceY == goalY))
                return null;
            if (MemorizePaths)
            {
                Point sourceP = new Point(sourceX, sourceY),
                    goalP = new Point(goalX, goalY);
                if (Memory.ContainsKey(sourceP) && Memory[sourceP].ContainsKey(goalP))
                    return Memory[sourceP][goalP].Clone();
                if (Memory.ContainsKey(goalP) && Memory[goalP].ContainsKey(sourceP))
                    return Memory[goalP][sourceP].ReverseClone(goalP);
            }
            Node source = Nodes[sourceX, sourceY],
                goal = Nodes[goalX, goalY];
            Open.Clear();
            Open.Enqueue(source, 0);
            Closed.Clear();
            while (Open.Count > 0)
            {
                var current = Open.Dequeue();
                if (current == goal)
                    return Build(source, goal);
                Closed.Add(current);
                var neighbours = Neighbours(current.X, current.Y);
                foreach (var n in neighbours)
                {
                    var g = (current.G + Cost(n.X, n.Y, current.X, current.Y));
                    if (!Open.Contains(n))
                    {
                        n.Parent = current;
                        n.G = g;
                        n.F = (g + (float)Math.Sqrt(Math.Pow((n.X - goalX), 2) + Math.Pow((n.Y - goalY), 2)));
                        Open.Enqueue(n, n.F);
                        continue;
                    }
                    else if (g >= n.G)
                        continue;
                    n.Parent = current;
                    n.G = g;
                    n.F = (g + (float)Math.Sqrt(Math.Pow((n.X - goalX), 2) + Math.Pow((n.Y - goalY), 2)));
                }
            }
            return null;
        }

        internal HashSet<Node> Neighbours(int x, int y)
        {
            var neighbours = new HashSet<Node>();
            if (((y - 1) >= 0) && Nodes[x, (y - 1)].Walkable)
            {
                if (!Closed.Contains(Nodes[x, (y - 1)])) neighbours.Add(Nodes[x, (y - 1)]);
                if (((x - 1) >= 0) && !Closed.Contains(Nodes[(x - 1), (y - 1)]) && Nodes[(x - 1), (y - 1)].Walkable &&
                    Nodes[(x - 1), y].Walkable && Nodes[x, (y - 1)].Walkable)
                    neighbours.Add(Nodes[(x - 1), (y - 1)]);
                if (((x + 1) < Width) && !Closed.Contains(Nodes[(x + 1), (y - 1)]) && Nodes[(x + 1), (y - 1)].Walkable &&
                    Nodes[x, (y - 1)].Walkable && Nodes[(x + 1), y].Walkable)
                    neighbours.Add(Nodes[(x + 1), (y - 1)]);
            }
            if (((y + 1) < Height) && Nodes[x, (y + 1)].Walkable)
            {
                if (!Closed.Contains(Nodes[x, (y + 1)])) neighbours.Add(Nodes[x, (y + 1)]);
                if (((x - 1) >= 0) && !Closed.Contains(Nodes[(x - 1), (y + 1)]) && Nodes[(x - 1), (y + 1)].Walkable &&
                    Nodes[(x - 1), y].Walkable && Nodes[x, (y + 1)].Walkable)
                    neighbours.Add(Nodes[(x - 1), (y + 1)]);
                if (((x + 1) < Width) && !Closed.Contains(Nodes[(x + 1), (y + 1)]) && Nodes[(x + 1), (y + 1)].Walkable &&
                    Nodes[x, (y + 1)].Walkable && Nodes[(x + 1), y].Walkable)
                    neighbours.Add(Nodes[(x + 1), (y + 1)]);
            }
            if (((x - 1) >= 0) && !Closed.Contains(Nodes[(x - 1), y]) && Nodes[(x - 1), y].Walkable)
                neighbours.Add(Nodes[(x - 1), y]);
            if (((x + 1) < Width) && !Closed.Contains(Nodes[(x + 1), y]) && Nodes[(x + 1), y].Walkable)
                neighbours.Add(Nodes[(x + 1), y]);
            return neighbours;
        }
        internal float Cost(int sourceX, int sourceY, int goalX, int goalY)
        {
            if ((sourceX == goalX) || (sourceY == goalY))
                return Nodes[sourceX, sourceY].Cost;
            else
                return (Nodes[sourceX, sourceY].Cost * Mathf.Sqrt2);
        }
        protected Path Build(Node source, Node goal)
        {
            var path = new Path(2);
            var goalP = new Point(goal.X, goal.Y);
            path.Add(goalP);
            var current = goal.Parent;
            while (current != source) { path.Insert(0, new Point(current.X, current.Y)); current = current.Parent; }
            if (!MemorizePaths) return path;
            var sourceP = new Point(source.X, source.Y);
            if (Memory.ContainsKey(sourceP))
            {
                if (!Memory[sourceP].ContainsKey(goalP))
                    Memory[sourceP].Add(goalP, path.Clone());
            }
            else
            {
                Memory.Add(sourceP, new Dictionary<Point, Path>());
                Memory[sourceP].Add(goalP, path.Clone());
            }
            return path;
        }

        public class Node
        {
            internal int X, Y;

            internal Node(int x, int y)
            {
                X = x;
                Y = y;
            }

            internal Node Parent;
            internal float G, F;

            public bool Walkable;
            public float Cost = 1;
        }

        public class Path : List<Point>
        {
            public Path() : base() { }
            public Path(int capacity) : base(capacity) { }

            internal Path Clone()
            {
                var path = new Path(Count);
                path.AddRange(this);
                return path;
            }
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