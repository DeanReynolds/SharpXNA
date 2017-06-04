using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpXNA.Plugins
{
    public interface IPriorityQueue<T> : IEnumerable<T>
    {
        void Enqueue(T node, double priority);
        T Dequeue();
        void Clear();
        bool Contains(T node);
        void Remove(T node);
        void UpdatePriority(T node, double priority);

        T First { get; }
        int Count { get; }
    }

    public sealed class PriorityQueue<T> : IPriorityQueue<T>
    {
        private class SimpleNode : FastPriorityQueueNode
        {
            public T Data { get; private set; }

            public SimpleNode(T data)
            {
                Data = data;
            }
        }

        private const int INITIAL_QUEUE_SIZE = 10;
        private readonly FastPriorityQueue<SimpleNode> _queue;

        public PriorityQueue()
        {
            _queue = new FastPriorityQueue<SimpleNode>(INITIAL_QUEUE_SIZE);
        }

        private SimpleNode GetExistingNode(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            foreach (var node in _queue)
                if (comparer.Equals(node.Data, item))
                    return node;
            throw new InvalidOperationException("Item cannot be found in queue: " + item);
        }

        public int Count
        {
            get
            {
                lock (_queue)
                    return _queue.Count;
            }
        }

        public T First
        {
            get
            {
                lock (_queue)
                {
                    if (_queue.Count <= 0)
                        throw new InvalidOperationException("Cannot call .First on an empty queue");
                    var first = _queue.First;
                    return (first != null ? first.Data : default(T));
                }
            }
        }

        public void Clear()
        {
            lock (_queue)
                _queue.Clear();
        }

        public bool Contains(T item)
        {
            lock (_queue)
            {
                var comparer = EqualityComparer<T>.Default;
                foreach (var node in _queue)
                    if (comparer.Equals(node.Data, item))
                        return true;
                return false;
            }
        }

        public T Dequeue()
        {
            lock (_queue)
            {
                if (_queue.Count <= 0)
                    throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
                var node = _queue.Dequeue();
                return node.Data;
            }
        }

        public void Enqueue(T item, double priority)
        {
            lock (_queue)
            {
                SimpleNode node = new SimpleNode(item);
                if (_queue.Count == _queue.MaxSize)
                    _queue.Resize(_queue.MaxSize * 2 + 1);
                _queue.Enqueue(node, priority);
            }
        }

        public void Remove(T item)
        {
            lock (_queue)
                try
                {
                    _queue.Remove(GetExistingNode(item));
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + item, ex);
                }
        }

        public void UpdatePriority(T item, double priority)
        {
            lock (_queue)
                try
                {
                    SimpleNode updateMe = GetExistingNode(item);
                    _queue.UpdatePriority(updateMe, priority);
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + item, ex);
                }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var queueData = new List<T>();
            lock (_queue)
                foreach (var node in _queue)
                    queueData.Add(node.Data);
            return queueData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsValidQueue()
        {
            lock (_queue)
                return _queue.IsValidQueue();
        }
    }

    public class FastPriorityQueueNode
    {
        public double Priority { get; set; }
        public long InsertionIndex { get; set; }
        public int QueueIndex { get; set; }
    }

    public sealed class FastPriorityQueue<T> : IPriorityQueue<T>
        where T : FastPriorityQueueNode
    {
        private int _numNodes;
        private T[] _nodes;
        private long _numNodesEverEnqueued;

        public FastPriorityQueue(int maxNodes)
        {
#if DEBUG
            if (maxNodes <= 0)
                throw new InvalidOperationException("New queue size cannot be smaller than 1");
#endif
            _numNodes = 0;
            _nodes = new T[maxNodes + 1];
            _numNodesEverEnqueued = 0;
        }

        public int Count
        {
            get { return _numNodes; }
        }

        public int MaxSize
        {
            get { return _nodes.Length - 1; }
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            Array.Clear(_nodes, 1, _numNodes);
            _numNodes = 0;
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(T node)
        {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException("node");
            if (node.QueueIndex < 0 || node.QueueIndex >= _nodes.Length)
                throw new InvalidOperationException("node.QueueIndex has been corrupted. Did you change it manually? Or add this node to another queue?");
#endif
            return (_nodes[node.QueueIndex] == node);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Enqueue(T node, double priority)
        {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException("node");
            if (_numNodes >= _nodes.Length - 1)
                throw new InvalidOperationException("Queue is full - node cannot be added: " + node);
            if (Contains(node))
                throw new InvalidOperationException("Node is already enqueued: " + node);
#endif
            node.Priority = priority;
            _numNodes++;
            _nodes[_numNodes] = node;
            node.QueueIndex = _numNodes;
            node.InsertionIndex = _numNodesEverEnqueued++;
            CascadeUp(_nodes[_numNodes]);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void Swap(T node1, T node2)
        {
            _nodes[node1.QueueIndex] = node2;
            _nodes[node2.QueueIndex] = node1;
            int temp = node1.QueueIndex;
            node1.QueueIndex = node2.QueueIndex;
            node2.QueueIndex = temp;
        }

        private void CascadeUp(T node)
        {
            int parent = node.QueueIndex / 2;
            while (parent >= 1)
            {
                T parentNode = _nodes[parent];
                if (HasHigherPriority(parentNode, node))
                    break;
                Swap(node, parentNode);
                parent = node.QueueIndex / 2;
            }
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeDown(T node)
        {
            T newParent;
            int finalQueueIndex = node.QueueIndex;
            while (true)
            {
                newParent = node;
                int childLeftIndex = 2 * finalQueueIndex;
                if (childLeftIndex > _numNodes)
                {
                    node.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }
                T childLeft = _nodes[childLeftIndex];
                if (HasHigherPriority(childLeft, newParent))
                    newParent = childLeft;
                int childRightIndex = childLeftIndex + 1;
                if (childRightIndex <= _numNodes)
                {
                    T childRight = _nodes[childRightIndex];
                    if (HasHigherPriority(childRight, newParent))
                        newParent = childRight;
                }
                if (newParent != node)
                {
                    _nodes[finalQueueIndex] = newParent;
                    int temp = newParent.QueueIndex;
                    newParent.QueueIndex = finalQueueIndex;
                    finalQueueIndex = temp;
                }
                else
                {
                    node.QueueIndex = finalQueueIndex;
                    _nodes[finalQueueIndex] = node;
                    break;
                }
            }
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool HasHigherPriority(T higher, T lower)
        {
            return (higher.Priority < lower.Priority ||
                (higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex));
        }

        public T Dequeue()
        {
#if DEBUG
            if (_numNodes <= 0)
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
            if (!IsValidQueue())
                throw new InvalidOperationException("Queue has been corrupted (Did you update a node priority manually instead of calling UpdatePriority()?" +
                                                    "Or add the same node to two different queues?)");
#endif
            T returnMe = _nodes[1];
            Remove(returnMe);
            return returnMe;
        }

        public void Resize(int maxNodes)
        {
#if DEBUG
            if (maxNodes <= 0)
                throw new InvalidOperationException("Queue size cannot be smaller than 1");
            if (maxNodes < _numNodes)
                throw new InvalidOperationException("Called Resize(" + maxNodes + "), but current queue contains " + _numNodes + " nodes");
#endif
            T[] newArray = new T[maxNodes + 1];
            int highestIndexToCopy = Math.Min(maxNodes, _numNodes);
            for (int i = 1; i <= highestIndexToCopy; i++)
                newArray[i] = _nodes[i];
            _nodes = newArray;
        }

        public T First
        {
            get
            {
#if DEBUG
                if (_numNodes <= 0)
                    throw new InvalidOperationException("Cannot call .First on an empty queue");
#endif
                return _nodes[1];
            }
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void UpdatePriority(T node, double priority)
        {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException("node");
            if (!Contains(node))
                throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + node);
#endif
            node.Priority = priority;
            OnNodeUpdated(node);
        }

        private void OnNodeUpdated(T node)
        {
            int parentIndex = node.QueueIndex / 2;
            T parentNode = _nodes[parentIndex];
            if (parentIndex > 0 && HasHigherPriority(node, parentNode))
                CascadeUp(node);
            else
                CascadeDown(node);
        }

        public void Remove(T node)
        {
#if DEBUG
            if (node == null)
                throw new ArgumentNullException("node");
            if (!Contains(node))
                throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + node);
#endif
            if (node.QueueIndex == _numNodes)
            {
                _nodes[_numNodes] = null;
                _numNodes--;
                return;
            }
            T formerLastNode = _nodes[_numNodes];
            Swap(node, formerLastNode);
            _nodes[_numNodes] = null;
            _numNodes--;
            OnNodeUpdated(formerLastNode);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 1; i <= _numNodes; i++)
                yield return _nodes[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsValidQueue()
        {
            for (int i = 1; i < _nodes.Length; i++)
            {
                if (_nodes[i] != null)
                {
                    int childLeftIndex = 2 * i;
                    if (childLeftIndex < _nodes.Length && _nodes[childLeftIndex] != null && HasHigherPriority(_nodes[childLeftIndex], _nodes[i]))
                        return false;
                    int childRightIndex = childLeftIndex + 1;
                    if (childRightIndex < _nodes.Length && _nodes[childRightIndex] != null && HasHigherPriority(_nodes[childRightIndex], _nodes[i]))
                        return false;
                }
            }
            return true;
        }
    }
}