using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Structures
{
    public class Ring<T>
    {
        private readonly T[] _array;
        private int _index;

        public T Current => _array[_index];

        public Ring(IEnumerable<T> values)
        {
            _array = values.ToArray();
            if(_array.Length == 0)
                throw new ArgumentException("values can't be empty");
        }

        public void MoveNext()
        {
            _index = (_index + 1) % _array.Length;
        }
    }
}