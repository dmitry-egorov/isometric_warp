namespace Core.Structures
{
    using System;

    public class DistinctValueHandler<T>
        where T: struct, IEquatable<T>
    {
        private readonly Action<T> _action;
        private T? _oldValue;

        public DistinctValueHandler(Action<T> action)
        {
            _action = action;
        }

        public void Apply(T newValue)
        {
            if (_oldValue.HasValue && _oldValue.Value.Equals(newValue))
            {
                return;
            }

            _oldValue = newValue;
            _action(newValue);
        }
    }
}