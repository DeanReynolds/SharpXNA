using System;

namespace SharpXNA.Plugins
{
    public class Heap<T> where T : IHeapItem<T>
    {
        public int Count { get; private set; }

        T[] _items;

        public Heap(int maxHeapSize) { _items = new T[maxHeapSize]; }

        public void Enqueue(T item)
        {
            item.HeapIndex = Count;
            _items[Count++] = item;
            SortUp(item);
        }
        public T Dequeue()
        {
            T firstItem = _items[0];
            _items[0] = _items[--Count];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            return firstItem;
        }
        public void UpdateItem(T item) => SortUp(item);
        public bool Contains(T item) => Equals(_items[item.HeapIndex], item);
        public void Clear()
        {
            Array.Clear(_items, 0, _items.Length);
            Count = 0;
        }

        void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = (item.HeapIndex * 2 + 1),
                    childIndexRight = (item.HeapIndex * 2 + 2),
                    swapIndex = 0;
                if (childIndexLeft < Count)
                {
                    swapIndex = childIndexLeft;
                    if ((childIndexRight < Count) && (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0))
                        swapIndex = childIndexRight;
                    if (item.CompareTo(_items[swapIndex]) < 0)
                        Swap(item, _items[swapIndex]);
                    else
                        return;
                }
                else
                    return;
            }
        }
        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parentItem = _items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;
                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }
        void Swap(T itemA, T itemB)
        {
            _items[itemA.HeapIndex] = itemB;
            _items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}