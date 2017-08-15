namespace Core.Structures
{
    public static class RingPointer
    {
        public static RingPointer<T> From<T>(T[] array)
        {
            return new RingPointer<T>(array);
        }
    }

    public struct RingPointer<T>
    {
        private readonly T[] _array;
        private readonly int _index;

        public T Value => _array[_index];
        public RingPointer<T> Next => new RingPointer<T>(_array, (_index + 1) % _array.Length);

        public RingPointer(T[] array, int index = 0)
        {
            _array = array;
            _index = index % array.Length;
        }

        public bool HasValue => _array != null;
        public bool HasNoValue => !HasValue;
    }
}