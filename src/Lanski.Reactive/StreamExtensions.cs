using System;

namespace Lanski.Reactive
{
    public static class StreamExtensions
    {
        public static IStream<TOut> Select<TIn, TOut>(this IStream<TIn> inStream, Func<TIn, TOut> selector)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            
            return new SelectStream<TIn, TOut>(inStream, selector);
        }

        public static IStream<TOut> SelectMany<TIn, TOut>(this IStream<TIn> inStream, Func<TIn, IStream<TOut>> selector)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            
            return new SelectManyStream<TIn, TOut>(inStream, selector);
        }

        public static IStream<T> Where<T>(this IStream<T> inStream, Func<T, bool> filter)
        {
            if (inStream == null)
                throw new ArgumentNullException(nameof(inStream));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            
            return new WhereStream<T>(inStream, filter);
        }

        public static IStream<T> Merge<T>(this IStream<T> firstStream, IStream<T> secondStream)
        {
            if (firstStream == null)
                throw new ArgumentNullException(nameof(firstStream));
            if (secondStream == null)
                throw new ArgumentNullException(nameof(secondStream));
            
            return new MergeStream<T>(firstStream, secondStream);
        }

        public static IStream<T> Values<T>(this IStream<T?> stream)
            where T: struct
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            
            return stream.Where(x => x.HasValue).Select(x => x.Value);
        }

        public static IStream<T> Publish<T>(this IStream<T> stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            
            var channel = new Stream<T>();
            stream.PublishTo(channel);
            return channel;
        }

        public static void PublishTo<T>(this IStream<T> stream, IConsumer<T> consumer)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (consumer == null)
                throw new ArgumentNullException(nameof(consumer));
            
            stream.Subscribe(consumer.Next);
        }

        public static IStream<T> First<T>(this IStream<T> stream, Func<T, bool> filter)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            
            return stream.Where(filter).First();
        }

        public static IStream<T> First<T>(this IStream<T> stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            
            return new FirstStream<T>(stream);
        }

        public static ICell<bool> IsInProgress<T>(this IStream<T> events, Func<T, bool> startCondition, Func<T, bool> finishCondition)
        {
            return new InProgressCell<T>(events, startCondition, finishCondition);
        }

        public class InProgressCell<T> : ICell<bool>
        {
            private readonly IStream<T> _events;
            private readonly Func<T, bool> _startCondition;
            private readonly Func<T, bool> _finishCondition;
            
            private bool _isInProgress;

            public InProgressCell(IStream<T> events, Func<T, bool> startCondition, Func<T, bool> finishCondition)
            {
                _events = events;
                _startCondition = startCondition;
                _finishCondition = finishCondition;
            }

            public Action Subscribe(Action<bool> action)
            {
                return _events.Subscribe(e =>
                {
                    if (_startCondition(e))
                    {
                        _isInProgress = true;
                        action(true);
                    }
                    else if (_finishCondition(e))
                    {
                        _isInProgress = false;
                        action(false);
                    }
                });
            }

            public bool Value => _isInProgress;
        }

        public static IStream<T> DistinctSequential<T>(this IStream<T> stream)
            where T: IEquatable<T>
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            
            var firstTime = true;
            var lastValue = default(T);
            return stream.Where(x =>
            {
                if (firstTime)
                {
                    firstTime = false;
                    lastValue = x;
                    return true;
                }

                if (lastValue.Equals(x))
                {
                    return false;
                }

                lastValue = x;
                return true;
            });
        }

        private class SelectManyStream<TIn, TOut> : IStream<TOut>
        {
            private readonly IStream<TIn> _inStream;
            private readonly Func<TIn, IStream<TOut>> _selector;

            public SelectManyStream(IStream<TIn> inStream, Func<TIn, IStream<TOut>> selector)
            {
                _inStream = inStream;
                _selector = selector;
            }

            public Action Subscribe(Action<TOut> action)
            {
                Action lastSubscription = null;

                return _inStream.Subscribe(x =>
                {
                    lastSubscription?.Invoke();

                    lastSubscription = _selector(x).Subscribe(action);
                });
            }
        }

        private class SelectStream<TIn, TOut> : IStream<TOut>
        {
            private readonly IStream<TIn> _inStream;
            private readonly Func<TIn, TOut> _selector;

            public SelectStream(IStream<TIn> inStream, Func<TIn, TOut> selector)
            {
                _inStream = inStream;
                _selector = selector;
            }

            public Action Subscribe(Action<TOut> action)
            {
                return _inStream.Subscribe(x => action(_selector(x)));
            }
        }

        private class WhereStream<T>: IStream<T>
        {
            private readonly IStream<T> _inStream;
            private readonly Func<T, bool> _filter;

            public WhereStream(IStream<T> inStream, Func<T, bool> filter)
            {
                _inStream = inStream;
                _filter = filter;
            }

            public Action Subscribe(Action<T> action)
            {
                return _inStream.Subscribe(x =>
                {
                    if (_filter(x))
                        action(x);
                });
            }
        }

        private class FirstStream<T> : IStream<T>
        {
            private readonly IStream<T> _inStream;

            public FirstStream(IStream<T> inStream)
            {
                _inStream = inStream;
            }

            public Action Subscribe(Action<T> action)
            {
                var unsubscribe = false;
                Action s = null;
                s = _inStream.Subscribe(x =>
                {
                    action(x);

                    if (s != null)
                    {
                        s();
                    }
                    else
                    {
                        unsubscribe = true;
                    }
                });

                if (unsubscribe)
                {
                    s();
                }

                return s;
            }
        }

        private class MergeStream<T>: IStream<T>
        {
            private readonly IStream<T> _firstStream;
            private readonly IStream<T> _secondStream;

            public MergeStream(IStream<T> firstStream, IStream<T> secondStream)
            {
                _firstStream = firstStream;
                _secondStream = secondStream;
            }

            public Action Subscribe(Action<T> action)
            {
                var a1 = _firstStream.Subscribe(action);
                var a2 = _secondStream.Subscribe(action);

                return () =>
                {
                    a1();
                    a2();
                };
            }
        }
    }
}