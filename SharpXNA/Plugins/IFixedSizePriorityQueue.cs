using System;
using System.Collections.Generic;
using System.Text;

namespace SharpXNA.Plugins
{
    internal interface IFixedSizePriorityQueue<TItem, in TPriority> : IPriorityQueue<TItem, TPriority>
        where TPriority : IComparable<TPriority>
    {
        void Resize(int maxNodes);
        int MaxSize { get; }
    }
}