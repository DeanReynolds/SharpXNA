﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SharpXNA
{
    public class Pathfinder
    {
        protected const float sqrt2 = 1.4142135f;

        public Node[,] Nodes;

        protected HashSet<Node> open, closed;
        public HashSet<Node> Open => open;
        public HashSet<Node> Closed => closed;

        public int Width => Nodes.GetLength(0);
        public int Height => Nodes.GetLength(1);

        internal Dictionary<Node, Dictionary<Node, Path>> pathsMemory;

        public Pathfinder(int width, int height)
        {
            Nodes = new Node[width, height];
            for (int x = 0; x < width; x++) for (int y = 0; y < height; y++) Nodes[x, y] = new Node((ushort)x, (ushort)y);
            pathsMemory = new Dictionary<Node, Dictionary<Node, Path>>();
            open = new HashSet<Node>(); closed = new HashSet<Node>();
        }

        public bool InBounds(int x, int y) { return !((x < 0) || (y < 0) || (x >= Width) || (y >= Height)); }
        public bool InBounds(Point point) { return !((point.X < 0) || (point.Y < 0) || (point.X >= Width) || (point.Y >= Height)); }
        public virtual HashSet<Node> Neighbours(Node node, bool cutCorners, ref HashSet<Node> closed)
        {
            if (cutCorners)
            {
                var neighbours = new HashSet<Node>();
                if (((node.Y - 1) >= 0) && Nodes[node.X, (node.Y - 1)].Walkable)
                {
                    if (!closed.Contains(Nodes[node.X, (node.Y - 1)])) neighbours.Add(Nodes[node.X, (node.Y - 1)]);
                    if (((node.X - 1) >= 0) && !closed.Contains(Nodes[(node.X - 1), (node.Y - 1)]) && Nodes[(node.X - 1), (node.Y - 1)].Walkable &&
                        (Nodes[(node.X - 1), node.Y].Walkable || Nodes[node.X, (node.Y - 1)].Walkable))
                        neighbours.Add(Nodes[(node.X - 1), (node.Y - 1)]);
                    if (((node.X + 1) < Width) && !closed.Contains(Nodes[(node.X + 1), (node.Y - 1)]) && Nodes[(node.X + 1), (node.Y - 1)].Walkable &&
                        (Nodes[node.X, (node.Y - 1)].Walkable || Nodes[(node.X + 1), node.Y].Walkable))
                        neighbours.Add(Nodes[(node.X + 1), (node.Y - 1)]);
                }
                if (((node.Y + 1) < Height) && Nodes[node.X, (node.Y + 1)].Walkable)
                {
                    if (!closed.Contains(Nodes[node.X, (node.Y + 1)])) neighbours.Add(Nodes[node.X, (node.Y + 1)]);
                    if (((node.X - 1) >= 0) && !closed.Contains(Nodes[(node.X - 1), (node.Y + 1)]) && Nodes[(node.X - 1), (node.Y + 1)].Walkable &&
                        (Nodes[(node.X - 1), node.Y].Walkable || Nodes[node.X, (node.Y + 1)].Walkable))
                        neighbours.Add(Nodes[(node.X - 1), (node.Y + 1)]);
                    if (((node.X + 1) < Width) && !closed.Contains(Nodes[(node.X + 1), (node.Y + 1)]) && Nodes[(node.X + 1), (node.Y + 1)].Walkable &&
                        (Nodes[node.X, (node.Y + 1)].Walkable || Nodes[(node.X + 1), node.Y].Walkable))
                        neighbours.Add(Nodes[(node.X + 1), (node.Y + 1)]);
                }
                if (((node.X - 1) >= 0) && !closed.Contains(Nodes[(node.X - 1), node.Y]) && Nodes[(node.X - 1), node.Y].Walkable) neighbours.Add(Nodes[(node.X - 1), node.Y]);
                if (((node.X + 1) < Width) && !closed.Contains(Nodes[(node.X + 1), node.Y]) && Nodes[(node.X + 1), node.Y].Walkable) neighbours.Add(Nodes[(node.X + 1), node.Y]);
                return neighbours;
            }
            else
            {
                var neighbours = new HashSet<Node>();
                if (((node.Y - 1) >= 0) && Nodes[node.X, (node.Y - 1)].Walkable)
                {
                    if (!closed.Contains(Nodes[node.X, (node.Y - 1)])) neighbours.Add(Nodes[node.X, (node.Y - 1)]);
                    if (((node.X - 1) >= 0) && !closed.Contains(Nodes[(node.X - 1), (node.Y - 1)]) && Nodes[(node.X - 1), (node.Y - 1)].Walkable &&
                        Nodes[(node.X - 1), node.Y].Walkable && Nodes[node.X, (node.Y - 1)].Walkable)
                        neighbours.Add(Nodes[(node.X - 1), (node.Y - 1)]);
                    if (((node.X + 1) < Width) && !closed.Contains(Nodes[(node.X + 1), (node.Y - 1)]) && Nodes[(node.X + 1), (node.Y - 1)].Walkable &&
                        Nodes[node.X, (node.Y - 1)].Walkable && Nodes[(node.X + 1), node.Y].Walkable)
                        neighbours.Add(Nodes[(node.X + 1), (node.Y - 1)]);
                }
                if (((node.Y + 1) < Height) && Nodes[node.X, (node.Y + 1)].Walkable)
                {
                    if (!closed.Contains(Nodes[node.X, (node.Y + 1)])) neighbours.Add(Nodes[node.X, (node.Y + 1)]);
                    if (((node.X - 1) >= 0) && !closed.Contains(Nodes[(node.X - 1), (node.Y + 1)]) && Nodes[(node.X - 1), (node.Y + 1)].Walkable &&
                        Nodes[(node.X - 1), node.Y].Walkable && Nodes[node.X, (node.Y + 1)].Walkable)
                        neighbours.Add(Nodes[(node.X - 1), (node.Y + 1)]);
                    if (((node.X + 1) < Width) && !closed.Contains(Nodes[(node.X + 1), (node.Y + 1)]) && Nodes[(node.X + 1), (node.Y + 1)].Walkable &&
                        Nodes[node.X, (node.Y + 1)].Walkable && Nodes[(node.X + 1), node.Y].Walkable)
                        neighbours.Add(Nodes[(node.X + 1), (node.Y + 1)]);
                }
                if (((node.X - 1) >= 0) && !closed.Contains(Nodes[(node.X - 1), node.Y]) && Nodes[(node.X - 1), node.Y].Walkable) neighbours.Add(Nodes[(node.X - 1), node.Y]);
                if (((node.X + 1) < Width) && !closed.Contains(Nodes[(node.X + 1), node.Y]) && Nodes[(node.X + 1), node.Y].Walkable) neighbours.Add(Nodes[(node.X + 1), node.Y]);
                return neighbours;
            }
        }

        public virtual Path Find(Point start, Point end, bool cutCorners = false, bool memorize = false)
        {
            if ((start == end) || !InBounds(start) || !InBounds(end) || !Nodes[start.X, start.Y].Walkable || !Nodes[end.X, end.Y].Walkable) return null;
            Node source = Nodes[start.X, start.Y], goal = Nodes[end.X, end.Y];
            if (memorize && pathsMemory.ContainsKey(source) && pathsMemory[source].ContainsKey(goal)) return pathsMemory[source][goal].Clone();
            open.Clear();
            open.Add(Nodes[source.X, source.Y]);
            closed.Clear();
            while (open.Count > 0)
            {
                var current = open.LowestFScore(source);
                if (current == goal) return ConstructPath(source, goal, memorize);
                open.Remove(current);
                closed.Add(current);
                var neighbours = Neighbours(current, cutCorners, ref closed);
                foreach (var neighbour in neighbours)
                {
                    var gScore = (current.GScore + neighbour.CostFrom(current));
                    if (!open.Contains(neighbour)) open.Add(neighbour);
                    else if (gScore >= neighbour.GScore) continue;
                    neighbour.Parent = current;
                    neighbour.GScore = gScore;
                    neighbour.FScore = (gScore + (2*Heuristics.Euclidean(neighbour, goal)));
                }
            }
            return null;
        }

        protected virtual Path ConstructPath(Node source, Node goal, bool memorize)
        {
            var path = new Path(1) {goal};
            var c = goal;
            while (c != source) { path.Insert(0, c); c = c.Parent; }
            if (!memorize) return path;
            if (pathsMemory.ContainsKey(source)) { if (!pathsMemory[source].ContainsKey(goal)) pathsMemory[source].Add(goal, path.Clone()); }
            else { pathsMemory.Add(source, new Dictionary<Node, Path>()); pathsMemory[source].Add(goal, path.Clone()); }
            var reversedPath = path.Clone();
            reversedPath.Reverse();
            reversedPath.RemoveAt(0);
            reversedPath.Add(source);
            if (pathsMemory.ContainsKey(goal)) { if (!pathsMemory[goal].ContainsKey(source)) pathsMemory[goal].Add(source, reversedPath); }
            else { pathsMemory.Add(goal, new Dictionary<Node, Path>()); pathsMemory[goal].Add(source, reversedPath); }
            return path;
        }

        public virtual Point? RandomWalkableNode(bool guesstimate = true)
        {
            var nodes = new List<Point>(Width * Height);
            for (var x = 0; x < Width; x++) for (var y = 0; y < Height; y++) if (Nodes[x, y].Walkable) nodes.Add(new Point(x, y));
            if (nodes.Count > 0) return nodes[(nodes.Count == 1) ? 0 : Globe.Random(nodes.Count - 1)]; else return null;
        }
        public virtual Point? RandomNonWalkableNode(bool guesstimate = true)
        {
            var nodes = new List<Point>(Width * Height);
            for (var x = 0; x < Width; x++) for (var y = 0; y < Height; y++) if (!Nodes[x, y].Walkable) nodes.Add(new Point(x, y));
            if (nodes.Count > 0) return nodes[(nodes.Count == 1) ? 0 : Globe.Random(nodes.Count - 1)]; else return null;
        }

        public class Node
        {
            public ushort X, Y;

            internal Node Parent;
            internal double GScore, FScore;

            public bool Walkable;
            public float Cost;

            public Node(ushort x, ushort y, bool walkable = true, float cost = 1) { X = x; Y = y; Walkable = walkable; Cost = cost; }
            
            public float CostFrom(Node node) { if ((X == node.X) || (Y == node.Y)) return Cost; else return (Cost * sqrt2); }
        }

        public class Path : List<Node>
        {
            public Path() : base() { }
            public Path(int capacity) : base(capacity) { }

            public Path Clone() { Path clone = new Path(Count); clone.AddRange(this); return clone; }
        }

        public static class Heuristics
        {
            public static double Dijkstra(Node source, Node goal) { return 0; }
            public static double Manhattan(Node source, Node goal) { return (Math.Abs(source.X - goal.X) + Math.Abs(source.Y - goal.Y)); }
            public static double Euclidean(Node source, Node goal) { return Math.Floor(Math.Sqrt(Math.Pow((source.X - goal.X), 2) + Math.Pow((source.Y - goal.Y), 2))); }
            public static double EuclideanNoSQR(Node source, Node goal) { return Math.Floor(Math.Pow((source.X - goal.X), 2) + Math.Pow((source.Y - goal.Y), 2)); }
            public static double MaxDXDY(Node source, Node goal) { return (Math.Max(Math.Abs(source.X - goal.X), Math.Abs(source.Y - goal.Y))); }
            public static double Diagonal(Node source, Node goal)
            {
                int dx = Math.Abs(source.X - goal.X), dy = Math.Abs(source.Y - goal.Y);
                if (dx > dy) return (sqrt2 * dy + (dx - dy));
                else return (sqrt2 * dx + (dy - dx));
            }
        }
    }

    public static class PathfinderExtentions
    {
        internal static Pathfinder.Node LowestFScore(this HashSet<Pathfinder.Node> nodes, Pathfinder.Node source)
        {
            var chosenNode = source;
            var lowestFScore = double.MaxValue;
            foreach (var node in nodes) if (node.FScore < lowestFScore) { lowestFScore = node.FScore; chosenNode = node; }
            return chosenNode;
        }
    }
}