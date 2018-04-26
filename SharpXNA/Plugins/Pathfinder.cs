using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace SharpXNA.Plugins
{
    public class Pathfinder
    {
        public Node[,] Nodes { get; internal set; }
        public int Width => (Nodes?.GetLength(0) ?? 0);
        public int Height => (Nodes?.GetLength(1) ?? 0);
        public bool InBounds(int x, int y) => !((x < 0) || (y < 0) || (x >= Nodes.GetLength(0)) || (y >= Nodes.GetLength(1)));
        public bool MemorizePaths,
            StraightenPaths;

        bool _fourDirectional;
        internal Node _current,
            _source,
            _goal;
        internal Heap<Node> _open;
        internal HashSet<Node> _closed;
        Dictionary<Node, Dictionary<Node, Path>> _memory;
        Func<Node, Node, float> _aStarHeuritic,
            _jumpPointSearchHeuristic,
            _thetAStarHeuristic;

        public Pathfinder(int width, int height, bool fourDirectional = false)
        {
            Nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Nodes[x, y] = new Node(x, y);
            if (fourDirectional)
            {
                _aStarHeuritic = ManhattanHeuristic;
                _jumpPointSearchHeuristic = ManhattanHeuristic;
                _thetAStarHeuristic = EuclideanCrossedHeuristic;
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        Nodes[x, y]._neighbours = Neighbours4Dir(Nodes[x, y]);
            }
            else
            {
                _aStarHeuritic = OctileCrossedHeuristic;
                _jumpPointSearchHeuristic = OctileHeuristic;
                _thetAStarHeuristic = EuclideanCrossedHeuristic;
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        Nodes[x, y]._neighbours = Neighbours8Dir(Nodes[x, y]);
            }
            _fourDirectional = fourDirectional;
            _open = new Heap<Node>(width * height);
            _closed = new HashSet<Node>();
            _memory = new Dictionary<Node, Dictionary<Node, Path>>();
        }
        public Node this[int x, int y] { get { return Nodes[x, y]; } }

        public Path AStar(Point source, Point goal) => AStar(source.X, source.Y, goal.X, goal.Y);
        public Path AStar(int sourceX, int sourceY, int goalX, int goalY)
        {
            if (((sourceX == goalX) && (sourceY == goalY)) || !Nodes[goalX, goalY].Walkable)
                return null;
            _source = Nodes[sourceX, sourceY];
            _goal = Nodes[goalX, goalY];
            if (MemorizePaths)
            {
                if (_memory.ContainsKey(_source) && _memory[_source].ContainsKey(_goal))
                    return _memory[_source][_goal].Clone();
                if (_memory.ContainsKey(_goal) && _memory[_goal].ContainsKey(_source))
                    return _memory[_goal][_source].ReverseClone(_goal);
            }
            _open.Clear();
            _open.Enqueue(_source);
            _closed.Clear();
            while (_open.Count > 0)
            {
                _current = _open.Dequeue();
                if (_current == _goal)
                    return BuildAStarPath();
                _closed.Add(_current);
                foreach (Node n in _current._neighbours)
                {
                    if (_closed.Contains(n))
                        continue;
                    if (!_open.Contains(n))
                    {
                        n._gCost = (_current._gCost + Cost(_current, n));
                        n._hCost = _aStarHeuritic(n, _goal);
                        n._parent = _current;
                        _open.Enqueue(n);
                        continue;
                    }
                    float gCost = (_current._gCost + Cost(_current, n));
                    if (gCost >= n._gCost)
                        continue;
                    n._gCost = gCost;
                    n._parent = _current;
                    _open.UpdateItem(n);
                }
            }
            return null;
        }
        internal Path StepAStar()
        {
            _current = _open.Dequeue();
            if (_current == _goal)
                return BuildAStarPath();
            _closed.Add(_current);
            foreach (Node n in _current._neighbours)
            {
                if (_closed.Contains(n))
                    continue;
                if (!_open.Contains(n))
                {
                    n._gCost = (_current._gCost + Cost(_current, n));
                    n._hCost = _aStarHeuritic(n, _goal);
                    n._parent = _current;
                    _open.Enqueue(n);
                    continue;
                }
                float gCost = (_current._gCost + Cost(_current, n));
                if (gCost >= n._gCost)
                    continue;
                n._gCost = gCost;
                n._parent = _current;
                _open.UpdateItem(n);
            }
            return null;
        }
        public Path JumpPointSearch(Point source, Point goal) => JumpPointSearch(source.X, source.Y, goal.X, goal.Y);
        public Path JumpPointSearch(int sourceX, int sourceY, int goalX, int goalY)
        {
            if (((sourceX == goalX) && (sourceY == goalY)) || !Nodes[goalX, goalY].Walkable)
                return null;
            _source = Nodes[sourceX, sourceY];
            _goal = Nodes[goalX, goalY];
            if (MemorizePaths)
            {
                if (_memory.ContainsKey(_source) && _memory[_source].ContainsKey(_goal))
                    return _memory[_source][_goal].Clone();
                if (_memory.ContainsKey(_goal) && _memory[_goal].ContainsKey(_source))
                    return _memory[_goal][_source].ReverseClone(_goal);
            }
            _open.Clear();
            _open.Enqueue(_source);
            _closed.Clear();
            while (_open.Count > 0)
            {
                _current = _open.Dequeue();
                if (_current == _goal)
                    return BuildJumpPointSearchPath();
                _closed.Add(_current);
                Node[] successors = Successors(_current);
                foreach (Node n in successors)
                {
                    if (_closed.Contains(n))
                        continue;
                    if (!_open.Contains(n))
                    {
                        n._gCost = (_current._gCost + Cost(_current, n));
                        n._hCost = _jumpPointSearchHeuristic(n, _goal);
                        n._parent = _current;
                        _open.Enqueue(n);
                        continue;
                    }
                    float gCost = (_current._gCost + Cost(_current, n));
                    if (gCost >= n._gCost)
                        continue;
                    n._gCost = gCost;
                    n._parent = _current;
                    _open.UpdateItem(n);
                }
            }
            return null;
        }
        internal Path StepJumpPointSearch()
        {
            _current = _open.Dequeue();
            if (_current == _goal)
                return BuildJumpPointSearchPath();
            _closed.Add(_current);
            Node[] successors = Successors(_current);
            foreach (Node n in successors)
            {
                if (_closed.Contains(n))
                    continue;
                if (!_open.Contains(n))
                {
                    n._gCost = (_current._gCost + Cost(_current, n));
                    n._hCost = _jumpPointSearchHeuristic(n, _goal);
                    n._parent = _current;
                    _open.Enqueue(n);
                    continue;
                }
                float gCost = (_current._gCost + Cost(_current, n));
                if (gCost >= n._gCost)
                    continue;
                n._gCost = gCost;
                n._parent = _current;
                _open.UpdateItem(n);
            }
            return null;
        }
        Node[] Successors(Node node)
        {
            if (_fourDirectional)
            {
                List<Node> successors = new List<Node>(node._neighbours.Length);
                foreach (Node n in node._neighbours)
                {
                    int dX = (int)Mathf.Clamp((n.X - node.X), -1, 1),
                        dY = (int)Mathf.Clamp((n.Y - node.Y), -1, 1);
                    Node jumpPoint = null;
                    if (dX != 0)
                        jumpPoint = JumpHorizontally(node.X, node.Y, dX);
                    else
                        jumpPoint = JumpVertically(node.X, node.Y, dY);
                    if (jumpPoint != null)
                        successors.Add(jumpPoint);
                }
                return successors.ToArray();
            }
            else
            {
                Node[] neighbors = Neighbours8DirJumpPointSearch(node);
                List<Node> successors = new List<Node>(neighbors.Length);
                foreach (Node n in neighbors)
                {
                    int dX = (int)Mathf.Clamp((n.X - node.X), -1, 1),
                        dY = (int)Mathf.Clamp((n.Y - node.Y), -1, 1);
                    Node jumpPoint = null;
                    if (dX != 0)
                    {
                        if (dY != 0)
                            jumpPoint = JumpDiagonally(node.X, node.Y, dX, dY);
                        else
                            jumpPoint = JumpHorizontally(node.X, node.Y, dX);
                    }
                    else
                        jumpPoint = JumpVertically(node.X, node.Y, dY);
                    if (jumpPoint != null)
                        successors.Add(jumpPoint);
                }
                return successors.ToArray();
            }
        }
        Node JumpHorizontally(int cX, int cY, int dir)
        {
            while (true)
            {
                int nX = (cX + dir);
                if ((nX < 0) ||
                    (nX >= Nodes.GetLength(0)) ||
                    !Nodes[nX, cY].Walkable)
                    return null;
                if ((nX == _goal.X) && (cY == _goal.Y))
                    return Nodes[nX, cY];
                int nnX = (nX + dir);
                if (!((nnX < 0) || (nnX >= Nodes.GetLength(0))))
                    if (((cY > 0) &&
                        Nodes[nnX, (cY - 1)].Walkable &&
                        !Nodes[nX, (cY - 1)].Walkable) ||
                        (((cY + 1) < Nodes.GetLength(1)) &&
                        Nodes[nnX, (cY + 1)].Walkable &&
                        !Nodes[nX, (cY + 1)].Walkable))
                        return Nodes[nX, cY];
                cX = nX;
            }
        }
        Node JumpVertically(int cX, int cY, int dir)
        {
            while (true)
            {
                int nY = (cY + dir);
                if ((nY < 0) ||
                    (nY >= Nodes.GetLength(1)) ||
                    !Nodes[cX, nY].Walkable)
                    return null;
                if ((cX == _goal.X) && (nY == _goal.Y))
                    return Nodes[cX, nY];
                int nnY = (nY + dir);
                if (!((nnY < 0) || (nnY >= Nodes.GetLength(1))))
                    if (((cX > 0) &&
                        Nodes[(cX - 1), nnY].Walkable &&
                        !Nodes[(cX - 1), nY].Walkable) ||
                        (((cX + 1) < Nodes.GetLength(0)) &&
                        Nodes[(cX + 1), nnY].Walkable &&
                        !Nodes[(cX + 1), nY].Walkable))
                        return Nodes[cX, nY];
                cY = nY;
            }
        }
        Node JumpDiagonally(int cX, int cY, int dX, int dY)
        {
            while (true)
            {
                int nX = (cX + dX),
                    nY = (cY + dY);
                if (!InBounds(nX, nY) ||
                    !Nodes[nX, nY].Walkable)
                    return null;
                if (!Nodes[nX, cY].Walkable &&
                    !Nodes[cX, nY].Walkable)
                    return null;
                if ((nX == _goal.X) && (nY == _goal.Y))
                    return Nodes[nX, nY];
                int nnX = (nX + dX),
                    nnY = (nY + dY);
                if (!((nnX < 0) || (nnX >= Nodes.GetLength(0))))
                    if (Nodes[nnX, cY].Walkable &&
                        !Nodes[nX, cY].Walkable)
                        return Nodes[nX, nY];
                if (!((nnY < 0) || (nnY >= Nodes.GetLength(1))))
                    if (Nodes[cX, nnY].Walkable &&
                        !Nodes[cX, nY].Walkable)
                        return Nodes[nX, nY];
                if ((JumpHorizontally(nX, nY, dX) != null) ||
                    (JumpVertically(nX, nY, dY) != null))
                    return Nodes[nX, nY];
                cX = nX;
                cY = nY;
            }
        }
        public Path ThetAStar(Point source, Point goal) => ThetAStar(source.X, source.Y, goal.X, goal.Y);
        public Path ThetAStar(int sourceX, int sourceY, int goalX, int goalY)
        {
            if (((sourceX == goalX) && (sourceY == goalY)) || !Nodes[goalX, goalY].Walkable)
                return null;
            _source = Nodes[sourceX, sourceY];
            _goal = Nodes[goalX, goalY];
            if (MemorizePaths)
            {
                if (_memory.ContainsKey(_source) && _memory[_source].ContainsKey(_goal))
                    return _memory[_source][_goal].Clone();
                if (_memory.ContainsKey(_goal) && _memory[_goal].ContainsKey(_source))
                    return _memory[_goal][_source].ReverseClone(_goal);
            }
            _source._parent = null;
            _open.Clear();
            _open.Enqueue(_source);
            _closed.Clear();
            while (_open.Count > 0)
            {
                _current = _open.Dequeue();
                if (_current == _goal)
                    return BuildThetAStarPath();
                _closed.Add(_current);
                foreach (Node n in _current._neighbours)
                {
                    if (_closed.Contains(n))
                        continue;
                    if (!_open.Contains(n))
                    {
                        n._gCost = (_current._gCost + EuclideanHeuristic(_current, n));
                        n._hCost = _thetAStarHeuristic(n, _goal);
                        n._parent = _current;
                        _open.Enqueue(n);
                    }
                    if (_current._parent != null)
                    {
                        if (_current._parent == n._parent)
                            continue;
                        if (LineOfSight(_current._parent.X, _current._parent.Y, n.X, n.Y))
                        {
                            float gCost = (_current._parent._gCost + EuclideanHeuristic(_current._parent, n));
                            if (gCost >= n._gCost)
                                continue;
                            n._gCost = gCost;
                            n._parent = _current._parent;
                            _open.UpdateItem(n);
                            continue;
                        }
                    }
                    float gCost2 = (_current._gCost + EuclideanHeuristic(_current, n));
                    if (gCost2 >= n._gCost)
                        continue;
                    n._gCost = gCost2;
                    n._parent = _current;
                    _open.UpdateItem(n);
                }
            }
            return null;
        }
        internal Path StepThetAStar()
        {
            _current = _open.Dequeue();
            if (_current == _goal)
                return BuildAStarPath();
            _closed.Add(_current);
            foreach (Node n in _current._neighbours)
            {
                if (_closed.Contains(n))
                    continue;
                if (!_open.Contains(n))
                {
                    n._gCost = (_current._gCost + EuclideanHeuristic(_current, n));
                    n._hCost = _thetAStarHeuristic(n, _goal);
                    n._parent = _current;
                    _open.Enqueue(n);
                }
                if (_current._parent != null)
                {
                    if (_current._parent == n._parent)
                        continue;
                    if (LineOfSight(_current._parent.X, _current._parent.Y, n.X, n.Y))
                    {
                        float gCost = (_current._parent._gCost + EuclideanHeuristic(_current._parent, n));
                        if (gCost >= n._gCost)
                            continue;
                        n._gCost = gCost;
                        n._parent = _current._parent;
                        _open.UpdateItem(n);
                        continue;
                    }
                }
                float gCost2 = (_current._gCost + EuclideanHeuristic(_current, n));
                if (gCost2 >= n._gCost)
                    continue;
                n._gCost = gCost2;
                n._parent = _current;
                _open.UpdateItem(n);
            }
            return null;
        }
        //public Path LazyThetAStar(Point source, Point goal) => LazyThetAStar(source.X, source.Y, goal.X, goal.Y);
        //public Path LazyThetAStar(int sourceX, int sourceY, int goalX, int goalY)
        //{
        //    if (((sourceX == goalX) && (sourceY == goalY)) || !Nodes[goalX, goalY].Walkable)
        //        return null;
        //    _source = Nodes[sourceX, sourceY];
        //    _goal = Nodes[goalX, goalY];
        //    if (MemorizePaths)
        //    {
        //        if (_memory.ContainsKey(_source) && _memory[_source].ContainsKey(_goal))
        //            return _memory[_source][_goal].Clone();
        //        if (_memory.ContainsKey(_goal) && _memory[_goal].ContainsKey(_source))
        //            return _memory[_goal][_source].ReverseClone(_goal);
        //    }
        //    _source._parent = null;
        //    _open.Clear();
        //    _open.Enqueue(_source);
        //    _closed.Clear();
        //    while (_open.Count > 0)
        //    {
        //        _current = _open.Dequeue();
        //        if (_current == _goal)
        //            return BuildThetAStarPath();
        //        _closed.Add(_current);
        //        if (_current._parent != null)
        //        {
        //            if (!LineOfSight(_current._parent.X, _current._parent.Y, _current.X, _current.Y))
        //                foreach (Node n in _current._neighbours)
        //                {
        //                    if (!_closed.Contains(n))
        //                        continue;
        //                    float gCost = (n._gCost + EuclideanHeuristic(_current, n));
        //                    if (gCost >= _current._gCost)
        //                        continue;
        //                    _current._gCost = gCost;
        //                    //_current._hCost = _thetAStarHeuristic(n, _goal);
        //                    _current._parent = n;
        //                }
        //        }
        //        foreach (Node n in _current._neighbours)
        //        {
        //            if (_closed.Contains(n))
        //                continue;
        //            if (!_open.Contains(n))
        //            {
        //                n._gCost = (_current._gCost + EuclideanHeuristic(_current, n));
        //                n._hCost = _thetAStarHeuristic(n, _goal);
        //                n._parent = _current;
        //                _open.Enqueue(n);
        //            }
        //            if (_current._parent != null)
        //            {
        //                float gCost = (_current._parent._gCost + EuclideanHeuristic(_current._parent, n));
        //                if (gCost >= n._gCost)
        //                    continue;
        //                n._gCost = gCost;
        //                n._parent = _current._parent;
        //            }
        //            else
        //            {
        //                float gCost = (_current._gCost + EuclideanHeuristic(_current, n));
        //                if (gCost >= n._gCost)
        //                    continue;
        //                n._gCost = gCost;
        //                n._parent = _current;
        //            }
        //            _open.UpdateItem(n);
        //        }
        //    }
        //    return null;
        //}
        Node[] Neighbours4Dir(Node node)
        {
            List<Node> neighbours = new List<Node>(4);
            int xM1 = (node.X - 1),
                xA1 = (node.X + 1),
                yM1 = (node.Y - 1),
                yA1 = (node.Y + 1);
            if ((yM1 >= 0) && Nodes[node.X, yM1].Walkable)
                neighbours.Add(Nodes[node.X, yM1]);
            if ((yA1 < Nodes.GetLength(1)) && Nodes[node.X, yA1].Walkable)
                neighbours.Add(Nodes[node.X, yA1]);
            if ((xM1 >= 0) && Nodes[xM1, node.Y].Walkable)
                neighbours.Add(Nodes[xM1, node.Y]);
            if ((xA1 < Nodes.GetLength(0)) && Nodes[xA1, node.Y].Walkable)
                neighbours.Add(Nodes[xA1, node.Y]);
            return neighbours.ToArray();
        }
        Node[] Neighbours8Dir(Node node)
        {
            List<Node> neighbours = new List<Node>(8);
            int xM1 = (node.X - 1),
                xA1 = (node.X + 1),
                yM1 = (node.Y - 1),
                yA1 = (node.Y + 1);
            if ((yM1 >= 0) && Nodes[node.X, yM1].Walkable)
            {
                neighbours.Add(Nodes[node.X, yM1]);
                if ((xM1 >= 0) &&
                    Nodes[xM1, yM1].Walkable &&
                    Nodes[xM1, node.Y].Walkable)
                    neighbours.Add(Nodes[xM1, yM1]);
                if ((xA1 < Nodes.GetLength(0)) &&
                    Nodes[xA1, yM1].Walkable &&
                    Nodes[xA1, node.Y].Walkable)
                    neighbours.Add(Nodes[xA1, yM1]);
            }
            if ((yA1 < Nodes.GetLength(1)) && Nodes[node.X, yA1].Walkable)
            {
                neighbours.Add(Nodes[node.X, yA1]);
                if ((xM1 >= 0) &&
                    Nodes[xM1, yA1].Walkable &&
                    Nodes[xM1, node.Y].Walkable)
                    neighbours.Add(Nodes[xM1, yA1]);
                if ((xA1 < Nodes.GetLength(0)) &&
                    Nodes[xA1, yA1].Walkable &&
                    Nodes[xA1, node.Y].Walkable)
                    neighbours.Add(Nodes[xA1, yA1]);
            }
            if ((xM1 >= 0) && Nodes[xM1, node.Y].Walkable)
                neighbours.Add(Nodes[xM1, node.Y]);
            if ((xA1 < Nodes.GetLength(0)) && Nodes[xA1, node.Y].Walkable)
                neighbours.Add(Nodes[xA1, node.Y]);
            return neighbours.ToArray();
        }
        Node[] Neighbours8DirJumpPointSearch(Node node)
        {
            HashSet<Node> neighbours = new HashSet<Node>();
            int xM1 = (node.X - 1),
                xA1 = (node.X + 1),
                yM1 = (node.Y - 1),
                yA1 = (node.Y + 1);
            if ((yM1 >= 0) && Nodes[node.X, yM1].Walkable)
            {
                neighbours.Add(Nodes[node.X, yM1]);
                if ((xM1 >= 0) &&
                    Nodes[xM1, yM1].Walkable)
                    neighbours.Add(Nodes[xM1, yM1]);
                if ((xA1 < Nodes.GetLength(1)) &&
                    Nodes[xA1, yM1].Walkable)
                    neighbours.Add(Nodes[xA1, yM1]);
            }
            if ((yA1 < Height) && Nodes[node.X, yA1].Walkable)
            {
                neighbours.Add(Nodes[node.X, yA1]);
                if ((xM1 >= 0) &&
                    Nodes[xM1, yA1].Walkable)
                    neighbours.Add(Nodes[xM1, yA1]);
                if ((xA1 < Nodes.GetLength(1)) &&
                    Nodes[xA1, yA1].Walkable)
                    neighbours.Add(Nodes[xA1, yA1]);
            }
            if ((xM1 >= 0) && Nodes[xM1, node.Y].Walkable)
            {
                neighbours.Add(Nodes[xM1, node.Y]);
                if ((yM1 >= 0) &&
                    Nodes[xM1, yM1].Walkable)
                    neighbours.Add(Nodes[xM1, yM1]);
                if ((yA1 < Nodes.GetLength(1)) &&
                    Nodes[xM1, yA1].Walkable)
                    neighbours.Add(Nodes[xM1, yA1]);
            }
            if ((xA1 < Width) && Nodes[xA1, node.Y].Walkable)
            {
                neighbours.Add(Nodes[xA1, node.Y]);
                if ((yM1 >= 0) &&
                    Nodes[xA1, yM1].Walkable)
                    neighbours.Add(Nodes[xA1, yM1]);
                if ((yA1 < Nodes.GetLength(1)) &&
                    Nodes[xA1, yA1].Walkable)
                    neighbours.Add(Nodes[xA1, yA1]);
            }
            return neighbours.ToArray();
        }
        int Cost(Node a, Node b)
        {
            if ((a.X == b.X) || (a.Y == b.Y))
                return (10 + Nodes[a.X, a.Y].Cost);
            else
                return (14 + Nodes[a.X, a.Y].Cost);
        }
        float ManhattanHeuristic(Node a, Node b)
        {
            int dX = Math.Abs(a.X - b.X),
                dY = Math.Abs(a.Y - b.Y);
            return (10 * (dX + dY));
        }
        float OctileHeuristic(Node a, Node b)
        {
            int dX = Math.Abs(a.X - b.X),
                dY = Math.Abs(a.Y - b.Y);
            int straight = Math.Abs(dX - dY);
            return ((10 * straight) + (14 * (Math.Max(dX, dY) - straight)));
        }
        float OctileCrossedHeuristic(Node a, Node b)
        {
            int dX = Math.Abs(a.X - b.X),
                dY = Math.Abs(a.Y - b.Y),
                dX1 = (a.X - b.X),
                dY1 = (a.Y - b.Y),
                dX2 = (_source.X - b.X),
                dY2 = (_source.Y - b.Y),
                cross = Math.Abs((dX1 * dY2) - (dX2 * dY1));
            int straight = Math.Abs(dX - dY);
            return ((10 * straight) + (14 * (Math.Max(dX, dY) - straight)) + (cross * .001f));
        }
        float EuclideanHeuristic(Node a, Node b)
        {
            int dX = Math.Abs(a.X - b.X),
                dY = Math.Abs(a.Y - b.Y);
            return (10 * (float)Math.Sqrt((dX * dX) + (dY * dY)));
        }
        float EuclideanCrossedHeuristic(Node a, Node b)
        {
            int dX = Math.Abs(a.X - b.X),
                dY = Math.Abs(a.Y - b.Y),
                dX1 = (a.X - b.X),
                dY1 = (a.Y - b.Y),
                dX2 = (_source.X - b.X),
                dY2 = (_source.Y - b.Y),
                cross = Math.Abs((dX1 * dY2) - (dX2 * dY1));
            return (10 * (float)Math.Sqrt((dX * dX) + (dY * dY)) + (cross * .001f));
        }
        Path BuildAStarPath()
        {
            Path path = new Path(2);
            if (StraightenPaths)
            {
                Node current = _source,
                    furthest = _goal;
                while (true)
                {
                    if (!LineOfSight(current.X, current.Y, furthest.X, furthest.Y))
                    {
                        furthest = furthest._parent;
                        continue;
                    }
                    foreach (Node n in NodesAlongRay(current.X, current.Y, furthest.X, furthest.Y))
                        if (current != n)
                        {
                            current = n;
                            path.Add(new Point(current.X, current.Y));
                        }
                    if (current == _goal)
                        break;
                    furthest = _goal;
                }
                if (!_fourDirectional)
                {
                    if (path.Count > 1)
                    {
                        int yDir = -(_source.Y - path[1].Y);
                        if (yDir == -1)
                        {
                            int xDir = -(_source.X - path[1].X);
                            if (xDir == 1)
                            {
                                if (Nodes[(_source.X + 1), _source.Y].Walkable && Nodes[_source.X, (_source.Y - 1)].Walkable)
                                    path.RemoveAt(0);
                            }
                            else if (xDir == -1)
                            {
                                if (Nodes[(_source.X - 1), _source.Y].Walkable && Nodes[_source.X, (_source.Y - 1)].Walkable)
                                    path.RemoveAt(0);
                            }
                        }
                        else if (yDir == 1)
                        {
                            int xDir = -(_source.X - path[1].X);
                            if (xDir == 1)
                            {
                                if (Nodes[(_source.X + 1), _source.Y].Walkable && Nodes[_source.X, (_source.Y + 1)].Walkable)
                                    path.RemoveAt(0);
                            }
                            else if (xDir == -1)
                            {
                                if (Nodes[(_source.X - 1), _source.Y].Walkable && Nodes[_source.X, (_source.Y + 1)].Walkable)
                                    path.RemoveAt(0);
                            }
                        }
                    }
                    for (int i = 0; i < (path.Count - 2); i++)
                    {
                        int dir = path.Direction(i),
                            dir1 = path.Direction(i + 1);
                        if (dir == 0)
                        {
                            if (dir1 == 2)
                            {
                                if (Nodes[(path[i].X + 1), path[i].Y].Walkable)
                                    path.RemoveAt(i + 1);
                            }
                            else if (dir1 == 6)
                                if (Nodes[(path[i].X - 1), path[i].Y].Walkable)
                                    path.RemoveAt(i + 1);
                        }
                        else if (dir == 2)
                        {
                            if (dir1 == 0)
                            {
                                if (Nodes[path[i].X, (path[i].Y - 1)].Walkable)
                                    path.RemoveAt(i + 1);
                            }
                            else if (dir1 == 4)
                                if (Nodes[path[i].X, (path[i].Y + 1)].Walkable)
                                    path.RemoveAt(i + 1);
                        }
                        else if (dir == 4)
                        {
                            if (dir1 == 2)
                            {
                                if (Nodes[(path[i].X + 1), path[i].Y].Walkable)
                                    path.RemoveAt(i + 1);
                            }
                            else if (dir1 == 6)
                                if (Nodes[(path[i].X - 1), path[i].Y].Walkable)
                                    path.RemoveAt(i + 1);
                        }
                        else if (dir == 6)
                        {
                            if (dir1 == 0)
                            {
                                if (Nodes[path[i].X, (path[i].Y - 1)].Walkable)
                                    path.RemoveAt(i + 1);
                            }
                            else if (dir1 == 4)
                                if (Nodes[path[i].X, (path[i].Y + 1)].Walkable)
                                    path.RemoveAt(i + 1);
                        }
                    }
                }
            }
            else
            {
                Node current = _goal._parent;
                path.Add(new Point(_goal.X, _goal.Y));
                while (current != _source)
                {
                    path.Insert(0, new Point(current.X, current.Y));
                    current = current._parent;
                }
            }
            Path path2 = path.Clone();
            for (int i = 1, j = 1; i < path2.Count; i++, j++)
            {
                int dir = path2.Direction(i),
                    dir1 = path2.Direction(i - 1);
                if (dir == dir1)
                    path.RemoveAt(j--);
            }
            if (!MemorizePaths)
                return path;
            if (_memory.ContainsKey(_source))
            {
                if (!_memory[_source].ContainsKey(_goal))
                    _memory[_source].Add(_goal, path.Clone());
            }
            else
            {
                _memory.Add(_source, new Dictionary<Node, Path>());
                _memory[_source].Add(_goal, path.Clone());
            }
            return path;
        }
        bool LineOfSight(int x1, int y1, int x2, int y2)
        {
            int xD = (x2 - x1),
                yD = (y2 - y1);
            Point rayStart = new Point(x1, y1);
            Point curVoxel = rayStart;
            int stepX = Math.Sign(xD),
                nvbX = (curVoxel.X + stepX);
            double tMaxX = ((xD != 0) ? ((nvbX - x1) / (double)xD) : 1),
                tDeltaX = ((xD != 0) ? ((1 / (double)xD) * stepX) : 1);
            int stepY = Math.Sign(yD),
                nvbY = (curVoxel.Y + stepY);
            double tMaxY = ((yD != 0) ? ((nvbY - y1) / (double)yD) : 1),
                tDeltaY = ((yD != 0) ? ((1 / (double)yD) * stepY) : 1);
            bool rX = false,
                rY = false;
            while (!(rX && rY))
            {
                if (((tMaxX < tMaxY) && !rX) || rY)
                {
                    curVoxel.X += stepX;
                    tMaxX += tDeltaX;
                }
                else
                {
                    curVoxel.Y += stepY;
                    tMaxY += tDeltaY;
                }
                //for (int x = -radius; x <= radius; x++)
                //    for (int y = -radius; y <= radius; y++)
                //        if ((Math.Abs(x) + Math.Abs(y)) <= radius)
                //        {
                //            int i = (curVoxel.X + x);
                //            if (!((i < 0) || (i >= Nodes.GetLength(0))))
                //            {
                //                int j = (curVoxel.Y + y);
                //                if (!((j < 0) || (j >= Nodes.GetLength(1))))
                //                    if (!Nodes[i, j].Walkable)
                //                        return true;
                //            }
                //        }
                if (!Nodes[curVoxel.X, curVoxel.Y].Walkable)
                    return false;
                rX = ((stepX > 0) ? (curVoxel.X >= x2) : (curVoxel.X <= x2));
                rY = ((stepY > 0) ? (curVoxel.Y >= y2) : (curVoxel.Y <= y2));
            }
            return true;
        }
        IEnumerable<Node> NodesAlongRay(int x1, int y1, int x2, int y2)
        {
            int xD = (x2 - x1),
                yD = (y2 - y1);
            Point rayStart = new Point(x1, y1),
                curVoxel = rayStart;
            int stepX = Math.Sign(xD),
                nvbX = (curVoxel.X + stepX);
            double tMaxX = ((xD != 0) ? ((nvbX - x1) / (double)xD) : 1),
                tDeltaX = ((xD != 0) ? ((1 / (double)xD) * stepX) : 1);
            int stepY = Math.Sign(yD),
                nvbY = (curVoxel.Y + stepY);
            double tMaxY = ((yD != 0) ? ((nvbY - y1) / (double)yD) : 1),
                tDeltaY = ((yD != 0) ? ((1 / (double)yD) * stepY) : 1);
            bool rX = false,
                rY = false;
            while (!(rX && rY))
            {
                if (((tMaxX < tMaxY) && !rX) || rY)
                {
                    curVoxel.X += stepX;
                    tMaxX += tDeltaX;
                }
                else
                {
                    curVoxel.Y += stepY;
                    tMaxY += tDeltaY;
                }
                //for (int x = -radius; x <= radius; x++)
                //    for (int y = -radius; y <= radius; y++)
                //        if ((Math.Abs(x) + Math.Abs(y)) <= radius)
                //        {
                //            int i = (curVoxel.X + x);
                //            if (!((i < 0) || (i >= Nodes.GetLength(0))))
                //            {
                //                int j = (curVoxel.Y + y);
                //                if (!((j < 0) || (j >= Nodes.GetLength(1))))
                //                    if (!Nodes[i, j].Walkable)
                //                        return true;
                //            }
                //        }
                yield return Nodes[curVoxel.X, curVoxel.Y];
                rX = ((stepX > 0) ? (curVoxel.X >= x2) : (curVoxel.X <= x2));
                rY = ((stepY > 0) ? (curVoxel.Y >= y2) : (curVoxel.Y <= y2));
            }
        }
        Path BuildJumpPointSearchPath()
        {
            Path path = new Path(2);
            Node current = _goal._parent;
            path.Add(new Point(_goal.X, _goal.Y));
            while (current != _source)
            {
                path.Insert(0, new Point(current.X, current.Y));
                current = current._parent;
            }
            if (!_fourDirectional)
            {
                int dir = path.Direction(0),
                    xDir = -(_source.X - path[0].X),
                    yDir = -(_source.Y - path[0].Y);
                if ((dir == 0) && (yDir > 0))
                {
                    path.Insert(1, new Point(path[0].X, (path[0].Y - 1)));
                    path.RemoveAt(0);
                }
                else if ((dir == 2) && (xDir < 0))
                {
                    path.Insert(1, new Point((path[0].X + 1), path[0].Y));
                    path.RemoveAt(0);
                }
                else if ((dir == 4) && (yDir < 0))
                {
                    path.Insert(1, new Point(path[0].X, (path[0].Y + 1)));
                    path.RemoveAt(0);
                }
                else if ((dir == 6) && (xDir > 0))
                {
                    path.Insert(1, new Point((path[0].X - 1), path[0].Y));
                    path.RemoveAt(0);
                }
            }
            //for (int i = 1; i < path.Count; i++)
            //{
            //    int xD = (path[i].X - path[i - 1].X);
            //    if (Math.Abs(xD) > 1)
            //        continue;
            //    int yD = (path[i].Y - path[i - 1].Y);
            //    if (Math.Abs(yD) > 1)
            //        continue;
            //    int dir = path.Direction(i),
            //        dir1 = path.Direction(i - 1);
            //    if (dir == dir1)
            //        path.RemoveAt(i--);
            //    else if (dir == 0)
            //    {
            //        if ((dir1 == 3) || (dir1 == 5))
            //        {
            //            path.Insert((i + 1), new Point(path[i].X, (path[i].Y - 1)));
            //            path.RemoveAt(i);
            //        }
            //        else if ((dir1 == 1) || (dir1 == 7))
            //        {
            //            path.Insert((i + 1), new Point(path[i].X, (path[i].Y + 1)));
            //            path.RemoveAt(i);
            //        }
            //    }
            //    else if (dir == 1)
            //    {
            //        if (dir1 == 3)
            //        {
            //            path.Insert((i + 1), new Point(path[i].X, (path[i].Y - 1)));
            //            path.RemoveAt(i);
            //        }
            //        else if (dir1 == 7)
            //        {
            //            path.Insert((i + 1), new Point((path[i].X + 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //    }
            //    else if (dir == 2)
            //    {
            //        if ((dir1 == 1) || (dir1 == 3))
            //        {
            //            path.Insert((i + 1), new Point((path[i].X - 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //        else if ((dir1 == 5) || (dir1 == 7))
            //        {
            //            path.Insert((i + 1), new Point((path[i].X + 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //    }
            //    else if (dir == 3)
            //    {
            //        if (dir1 == 5)
            //        {
            //            path.Insert((i + 1), new Point((path[i].X + 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //    }
            //    else if (dir == 4)
            //    {
            //        if ((dir1 == 1) || (dir1 == 7))
            //        {
            //            path.Insert((i + 1), new Point(path[i].X, (path[i].Y + 1)));
            //            path.RemoveAt(i);
            //        }
            //        else if ((dir1 == 3) || (dir1 == 5))
            //        {
            //            path.Insert((i + 1), new Point(path[i].X, (path[i].Y - 1)));
            //            path.RemoveAt(i);
            //        }
            //    }
            //    else if (dir == 5)
            //    {
            //        if (dir1 == 3)
            //        {
            //            path.Insert((i + 1), new Point((path[i].X - 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //    }
            //    else if (dir == 6)
            //    {
            //        if ((dir1 == 1) || (dir1 == 3))
            //        {
            //            path.Insert((i + 1), new Point((path[i].X - 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //        else if ((dir1 == 5) || (dir1 == 7))
            //        {
            //            path.Insert((i + 1), new Point((path[i].X + 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //    }
            //    else if (dir == 7)
            //    {
            //        if (dir1 == 1)
            //        {
            //            path.Insert((i + 1), new Point((path[i].X - 1), path[i].Y));
            //            path.RemoveAt(i);
            //        }
            //    }
            //}
            if (!MemorizePaths)
                return path;
            if (_memory.ContainsKey(_source))
            {
                if (!_memory[_source].ContainsKey(_goal))
                    _memory[_source].Add(_goal, path.Clone());
            }
            else
            {
                _memory.Add(_source, new Dictionary<Node, Path>());
                _memory[_source].Add(_goal, path.Clone());
            }
            return path;
        }
        Path BuildThetAStarPath()
        {
            Path path = new Path(2);
            Node current = _goal._parent;
            path.Add(new Point(_goal.X, _goal.Y));
            while (current != _source)
            {
                path.Insert(0, new Point(current.X, current.Y));
                current = current._parent;
            }
            if (!MemorizePaths)
                return path;
            if (_memory.ContainsKey(_source))
            {
                if (!_memory[_source].ContainsKey(_goal))
                    _memory[_source].Add(_goal, path.Clone());
            }
            else
            {
                _memory.Add(_source, new Dictionary<Node, Path>());
                _memory[_source].Add(_goal, path.Clone());
            }
            return path;
        }

        public bool IsOpen(Node node) => _open.Contains(node);
        public bool IsOpen(int x, int y) => _open.Contains(Nodes[x, y]);
        public bool IsClosed(Node node) => _closed.Contains(node);
        public bool IsClosed(int x, int y) => _closed.Contains(Nodes[x, y]);

        public void SetWalkable(int x, int y, bool walkable)
        {
            Nodes[x, y].Walkable = walkable;
            if (_fourDirectional)
            {
                for (int j = (x - 1); j <= x; j++)
                    if ((j >= 0) && (j < Nodes.GetLength(0)))
                        for (int k = (y - 1); k <= y; k++)
                            if ((k >= 0) && (k < Nodes.GetLength(1)))
                                Nodes[j, k]._neighbours = Neighbours4Dir(Nodes[j, k]);
            }
            else
                for (int j = (x - 1); j <= x; j++)
                    if ((j >= 0) && (j < Nodes.GetLength(0)))
                        for (int k = (y - 1); k <= y; k++)
                            if ((k >= 0) && (k < Nodes.GetLength(1)))
                                Nodes[j, k]._neighbours = Neighbours8Dir(Nodes[j, k]);
        }
        public void BlurCosts(int size)
        {
            int width = Nodes.GetLength(0),
                height = Nodes.GetLength(1),
                kernelSize = (size * 2 + 1),
                kernelExtents = ((kernelSize - 1) / 2);
            float[,] penaltiesHorizontalPass = new float[width, height],
                penaltiesVerticalPass = new float[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                    penaltiesHorizontalPass[0, y] += Nodes[(int)Mathf.Clamp(x, 0, kernelExtents), y].Cost;
                for (int x = 1; x < width; x++)
                    penaltiesHorizontalPass[x, y] = (penaltiesHorizontalPass[x - 1, y] - Nodes[(int)Mathf.Clamp((x - kernelExtents - 1), 0, width), y].Cost + Nodes[(int)Mathf.Clamp((x + kernelExtents), 0, (width - 1)), y].Cost);
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, (int)Mathf.Clamp(y, 0, kernelExtents)];
                if (Nodes[x, 0].Walkable)
                    Nodes[x, 0].Cost = (int)((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                for (int y = 1; y < height; y++)
                {
                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, (int)Mathf.Clamp((y - kernelExtents - 1), 0, height)] + penaltiesHorizontalPass[x, (int)Mathf.Clamp((y + kernelExtents), 0, (height - 1))];
                    if (Nodes[x, y].Walkable)
                        Nodes[x, y].Cost = (int)((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                }
            }
        }

        public class Node : IHeapItem<Node>
        {
            public int X { get; internal set; }
            public int Y { get; internal set; }

            internal Node _parent;
            internal float _gCost;
            internal float _hCost;
            internal float _fCost => (_gCost + _hCost);
            internal Node[] _neighbours;

            int heapIndex;

            public int HeapIndex
            {
                get { return heapIndex; }
                set { heapIndex = value; }
            }
            public bool Walkable { get; internal set; }
            public int Cost;

            internal Node(int x, int y)
            {
                X = x;
                Y = y;
                Walkable = true;
            }

            public int CompareTo(Node n)
            {
                int compare = _fCost.CompareTo(n._fCost);
                if (compare == 0)
                    compare = _hCost.CompareTo(n._hCost);
                return -compare;
            }
        }
        public class Path : List<Point>
        {
            public Path() { }
            public Path(int capacity) : base(capacity) { }

            public int Direction(int index)
            {
                if (Count <= (index + 1))
                    return -1;
                if (this[index + 1].X == this[index].X)
                {
                    if (this[index + 1].Y > this[index].Y)
                        return 4;
                    if (this[index + 1].Y < this[index].Y)
                        return 0;
                }
                else
                {
                    if (this[index + 1].X > this[index].X)
                    {
                        if (this[index + 1].Y == this[index].Y)
                            return 2;
                        if (this[index + 1].Y > this[index].Y)
                            return 3;
                        if (this[index + 1].Y < this[index].Y)
                            return 1;
                    }
                    if (this[index + 1].X < this[index].X)
                    {
                        if (this[index + 1].Y == this[index].Y)
                            return 6;
                        if (this[index + 1].Y > this[index].Y)
                            return 5;
                        if (this[index + 1].Y < this[index].Y)
                            return 7;
                    }
                }
                return -1;
            }

            internal Path Clone()
            {
                var path = new Path(Count);
                path.AddRange(this);
                return path;
            }
            internal Path ReverseClone(Node source)
            {
                var path = Clone();
                path.Reverse();
                path.RemoveAt(0);
                path.Add(new Point(source.X, source.Y));
                return path;
            }
        }
    }
}