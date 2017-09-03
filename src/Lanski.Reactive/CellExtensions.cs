using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class CellExtensions
    {
        public static ICell<TOut> Select<TIn, TOut>(this ICell<TIn> cell, Func<TIn, TOut> selector) => new SelectCell<TIn, TOut>(cell, selector);

        public static ICell<TOut> SelectMany<TIn, TOut>(this ICell<TIn> cell, Func<TIn, ICell<TOut>> selector) => new SelectManyCell<TIn, TOut>(cell, selector);

        public static ICell<Tuple<T1, T2>> Merge<T1, T2>(this ICell<T1> cell, ICell<T2> other) 
            where T1 : IEquatable<T1> 
            where T2 : IEquatable<T2> => new MergeCell<T1, T2>(cell, other);

        public static ICell<int> Sum(this ICell<int> cell, ICell<int> other) => cell.Merge(other).Select(x => x.Item1 + x.Item2);
        
        private class SelectCell<TIn, TOut> : ICell<TOut>
        {
            private readonly ICell<TIn> _inCell;
            private readonly Func<TIn, TOut> _selector;

            public SelectCell(ICell<TIn> inCell, Func<TIn, TOut> selector)
            {
                _inCell = inCell;
                _selector = selector;
            }

            public Action Subscribe(Action<TOut> action)
            {
                return _inCell.Subscribe(x => action(_selector(x)));
            }

            public TOut Value => _selector(_inCell.Value);
        }

        public class SelectManyCell<TIn, TOut>: ICell<TOut>
        {
            private readonly ICell<TIn> _inCell;
            private readonly Func<TIn, ICell<TOut>> _selector;

            public SelectManyCell(ICell<TIn> inCell, Func<TIn, ICell<TOut>> selector)
            {
                _inCell = inCell;
                _selector = selector;
            }

            public Action Subscribe(Action<TOut> action)
            {
                Action lastSubscription = null;

                return _inCell.Subscribe(x =>
                {
                    lastSubscription?.Invoke();

                    lastSubscription = x != null ? _selector(x).Subscribe(action) : null;
                });
            }

            public TOut Value => _selector(_inCell.Value).Value;
        }


        private class MergeCell<T1, T2> : ICell<Tuple<T1, T2>> 
            where T1 : IEquatable<T1> 
            where T2 : IEquatable<T2>
        {
            private readonly ICell<T1> _cell1;
            private readonly ICell<T2> _cell2;

            public MergeCell(ICell<T1> cell1, ICell<T2> cell2)
            {
                _cell1 = cell1;
                _cell2 = cell2;
            }

            public Action Subscribe(Action<Tuple<T1, T2>> action)
            {
                var sub1 = _cell1.Subscribe(x => action(new Tuple<T1, T2>(x, _cell2.Value)));
                var sub2 = _cell2.Subscribe(x => action(new Tuple<T1, T2>(_cell1.Value, x)));

                return () =>
                {
                    sub1();
                    sub2();
                };
            }

            public Tuple<T1, T2> Value => Tuple.From(_cell1.Value, _cell2.Value);
        }
    }
}