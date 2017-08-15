using System.Collections.Generic;

namespace Core.Structures
{
    public class HashCounter<T>
    {
        private readonly IDictionary<T, int> _map = new Dictionary<T, int>();

        public void Increment(T key)
        {
            int current;
            if (_map.TryGetValue(key, out current))
            {
                _map[key] = current + 1;
            }
            else
            {
                _map[key] = 1;
            }
        }

        public int Current(T key)
        {
            int current;
            return _map.TryGetValue(key, out current) ? current : 0;
        }
    }
}