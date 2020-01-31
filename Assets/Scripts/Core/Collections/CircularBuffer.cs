using System.Collections;
using System.Collections.Generic;

namespace pdxpartyparrot.Core.Collections
{
    public class CircularBuffer<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        public int Size { get; }

        private int _head;

        public T Head => Count > 0 ? _elements[_head] : default;

        private int _tail;

        public T Tail => Count > 0 ? _elements[_tail] : default;

        private readonly T[] _elements;

        public CircularBuffer(int size)
        {
            Size = size;
            _elements = new T[Size];

            Clear();
        }

        public void RemoveOldest()
        {
            if(Count < 1) {
                return;
            }

            if(Count == 1) {
                Clear();
                return;
            }

            AdvanceIndex(ref _head);
        }

        private void AdvanceIndex(ref int i)
        {
            i = (i + 1) % Size;
        }

#region ICollection
        public int Count => _head == -1
            ? 0
            : _tail >= _head
                ? _tail - _head + 1
                : Size - _head + _tail + 1;

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            if(Count < 1) {
                yield break;
            }

            int idx = _head;
            while(idx <= _tail) {
                yield return _elements[idx];
                AdvanceIndex(ref idx);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if(Count < 1) {
                _head = 0;
            } else {
                AdvanceIndex(ref _tail);
                if(_head == _tail && Size > 1) {
                    AdvanceIndex(ref _head);
                }
            }
            _elements[_tail] = item;
        }

        public void Clear()
        {
            _head = -1;
            _tail = 0;
        }

        public bool Contains(T item)
        {
            if(Count < 1) {
                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            int idx = _head;
            while(idx <= _tail) {
                if(comparer.Equals(_elements[idx], item)) {
                    return true;
                }
                AdvanceIndex(ref idx);
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if(Count < 1) {
                return;
            }

            int idx = _head;
            while(idx <= _tail && arrayIndex < array.Length) {
                array[arrayIndex] = _elements[idx];
                AdvanceIndex(ref idx);
            }
        }

        public bool Remove(T item)
        {
            throw new System.NotImplementedException();
        }
#endregion
    }
}
