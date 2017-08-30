using System;
using System.Collections.Generic;

namespace Lanski.Reactive
{
    public static class Stream
    {
        public static IStream<T> Empty<T>() => new EmptyStream<T>();
    }

    public class Stream<T> : IStream<T>, IConsumer<T>
    {
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();
        private readonly Queue<Action<T>> _removeQueue = new Queue<Action<T>>();
        private readonly HashSet<Action<T>> _removeSet = new HashSet<Action<T>>();
        private bool _isSignaling;

        public Action Subscribe(Action<T> action)
        {
            _subscribers.Add(action);
            return () => Unsubscribe(action);
        }

        public void Next(T value)
        {
            _isSignaling = true;
            for (var i = 0; i < _subscribers.Count; i++)
            {
                var action = _subscribers[i];
                if(_removeSet.Contains(action))
                    continue;

                action(value);
            }
            _isSignaling = false;

            while (_removeQueue.Count > 0)
            {
                var actionToRemove = _removeQueue.Dequeue();
                _removeSet.Remove(actionToRemove);
                _subscribers.Remove(actionToRemove);
            }
        }

        private void Unsubscribe(Action<T> action)
        {
            if (!_subscribers.Contains(action))
                return;

            if (_isSignaling)
            {
                _removeQueue.Enqueue(action);
                _removeSet.Add(action);
            }
            else
            {
                _subscribers.Remove(action);
            }
        }
    }
}