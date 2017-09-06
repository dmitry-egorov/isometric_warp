using System;
using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;

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

        public static Action PublishTo<T>(this IStream<T> stream, IConsumer<T> consumer)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (consumer == null)
                throw new ArgumentNullException(nameof(consumer));
            
            return stream.Subscribe(consumer.Next);
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

        public static ICell<bool> IsInProgress<T>(this IStream<T> events, Func<T, bool> startCondition, Func<T, bool> finishCondition) => 
            new InProgressCell<T>(events, startCondition, finishCondition);

        public static IStream<T> Skip<T>(this IStream<T> stream, int count) => new SkipStream<T>(stream, count);

        public static IStream<T> SkipEmpty<T>(this IStream<Possible<T>> stream) where T : class => 
            stream
                .Where(r => r.Has_a_Value())
                .Select(r => r.Must_Have_a_Value())
        ;

        public static IStream<(Possible<T> previous, Possible<T> current)> IncludePrevious<T>(this IStream<Possible<T>> stream) => new PairsStream<T>(stream);
        public static IStream<(T previous, T current)> s_Changes<T>(this IStream<T> the_stream) => new ChangesStream<T>(the_stream);

        public static Action<T> Invocation<T>(this IConsumer<T> consumer) => x => consumer.Next(x);

        public static IStream<T> AsStream<T>(this IEnumerable<T> enumerable) => new EnumerableStream<T>(enumerable);

        public class ChangesStream<T> : IStream<(T previous, T current)>
        {
            private readonly IStream<T> s_stream;

            public ChangesStream(IStream<T> the_stream)
            {
                this.s_stream = the_stream;
            }

            public Action Subscribe(Action<(T previous, T current)> action)
            {
                var possible_prev = default(Possible<T>);
                return this.s_stream.Subscribe(v =>
                {
                    if (possible_prev.Has_a_Value(out var prev))
                    {
                        action((prev, v));
                        
                    }
                    possible_prev = v;
                });
            }
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

            public bool s_Value => _isInProgress;
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

                var s = _inStream.Subscribe(x =>
                {
                    lastSubscription?.Invoke();

                    lastSubscription = x != null ? _selector(x).Subscribe(action) : null;
                });
                
                return () =>
                {
                    s();
                    lastSubscription?.Invoke();
                };
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
                Action s = () => unsubscribe = true;
                s = _inStream.Subscribe(x =>
                {
                    action(x);
                    s();
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
    
    public class PairsStream<T>: IStream<(Possible<T>,Possible<T>)>
    {
        private readonly IStream<Possible<T>> _stream;

        public PairsStream(IStream<Possible<T>> stream)
        {
            _stream = stream;
        }

        public Action Subscribe(Action<(Possible<T>, Possible<T>)> action)
        {
            var prev = default(Possible<T>);
            return _stream.Subscribe(v =>
            {
                action((prev, v));
                prev = v;
            });
        }
    }

    public class SkipStream<T> : IStream<T>
    {
        private readonly IStream<T> _stream;
        private readonly int _count;

        public SkipStream(IStream<T> stream, int count)
        {
            _stream = stream;
            _count = count;
        }

        public Action Subscribe(Action<T> action)
        {
            var i = 0;

            return _stream.Subscribe(item =>
            {
                if (i < _count)
                {
                    i++;
                }
                else
                {
                    action(item);
                }

            });
        }
    }
    
    public class EnumerableStream<T> : IStream<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public EnumerableStream(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public Action Subscribe(Action<T> action)
        {
            foreach (var item in _enumerable)
            {
                action(item);
            }
            return () => { };
        }
    }
}