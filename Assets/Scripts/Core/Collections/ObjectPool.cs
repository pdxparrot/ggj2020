using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace pdxpartyparrot.Core.Collections
{
    public interface IPooledItem
    {
        void ResetPooledItem();
    }

    public interface IReadOnlyObjectPool<out T> : IEnumerable<T>, IEnumerable
    {
        int Size { get; }

        int Used { get; }

        int Free { get; }
    }

    public sealed class ObjectPool<T> : IEnumerable<T>, IEnumerable, IReadOnlyObjectPool<T> where T: class, IPooledItem, new()
    {
        public int Size => Used + Free;

        public int Used => _used.Count;

        public int Free => _unused.Count;

        private readonly List<T> _unused;

        private readonly List<T> _used;

        public ObjectPool(int size)
        {
            _unused = new List<T>(size);
            _used = new List<T>(size);

            for(int i=0; i<size; ++i) {
                _unused.Add(new T());
            }
        }

        [CanBeNull]
        public T Acquire()
        {
            if(Free == 0) {
                return null;
            }

            T item = _unused.RemoveFront();
            item.ResetPooledItem();
            _used.Add(item);
            return item;
        }

        public bool Release(T item)
        {
            if(_used.Remove(item)) {
                _unused.Add(item);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _used.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
