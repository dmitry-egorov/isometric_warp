using System;
using System.Collections.Generic;
using Lanski.SwiftLinq;

namespace Lanski.Reactive
{
    public static class Stream
    {
        public static IStream<T> Empty<T>() => new EmptyStream<T>();
    }

    public class Stream<T> : IStream<T>, IConsumer<T>
    {
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();
        private readonly Queue<T> _signalQueue = new Queue<T>();
        private readonly Queue<Action<T>> _removeQueue = new Queue<Action<T>>();
        private readonly HashSet<Action<T>> _removeSet = new HashSet<Action<T>>();
        private readonly Queue<Action<T>> _subscribeQueue = new Queue<Action<T>>();
        private bool _isSignaling;

        public Action Subscribe(Action<T> action)
        {
            if (_isSignaling)
            {
                AddToSubscribeQueue(action);
            }
            else
            {
                _subscribers.Add(action);
            }
            return () => Unsubscribe(action);
        }

        public void Next(T value)
        {
            if (!_isSignaling)
            {
                _isSignaling = true;
                var iterator = _subscribers.s_new_iterator();
                while (iterator.has_a_Value(out var action))
                {
                    action(value);
                }
                _isSignaling = false;

                ProcessSignalQueue();
                ProcessSubscribeQueue();
                ProcessRemoveQueue();
            }
            else
            {
                _signalQueue.Enqueue(value);
            }
        }

        private void ProcessSubscribeQueue()
        {
            while (_subscribeQueue.Count > 0)
            {
                var actionToSubscribe = _subscribeQueue.Dequeue();
                _subscribers.Add(actionToSubscribe);
            }
        }

        public void UnsubscribeAll()
        {
            foreach (var subscriber in _subscribers)
            {
                AddToRemoveQueue(subscriber);
            }
            
            ProcessRemoveQueue();
        }

        private void ProcessSignalQueue()
        {
            while (_signalQueue.Count > 0)
            {
                var next_value = _signalQueue.Dequeue();
                Next(next_value);
            }
        }
        private void ProcessRemoveQueue()
        {
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
                AddToRemoveQueue(action);
            }
            else
            {
                _subscribers.Remove(action);
            }
        }

        private void AddToRemoveQueue(Action<T> action)
        {
            _removeQueue.Enqueue(action);
            _removeSet.Add(action);
        }

        private void AddToSubscribeQueue(Action<T> action)
        {
            _subscribeQueue.Enqueue(action);
        }
    }
}