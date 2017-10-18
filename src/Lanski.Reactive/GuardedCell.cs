using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class GuardedCell
    {
        public static GuardedCell<Possible<T>> Empty<T>(SignalGuard the_signal_guard) => new GuardedCell<Possible<T>>(Possible.Empty<T>(), the_signal_guard);
    }
    
    public class GuardedCell<T> : ICell<T>, IConsumer<T>
        where T: IEquatable<T>
    {
        public GuardedCell(T initial_value, SignalGuard the_signal_guard)
        {
            the_guard = the_signal_guard;
            
            the_source = new Cell<T>(initial_value);
        }

        public T s_Value
        {
            get => the_source.s_Value;
            set => Next(value);
        }

        public void Next(T value)
        {
            the_source.s_Value = value;
        }
        
        public Action Subscribe(Action<T> the_action)
        {
            var signal_the_value_on_release = false;
            var subs = the_source.Subscribe(v =>
            {
                if (the_guard.is_Letting_Events_Through())
                {
                    the_action(v);
                }
                else
                {
                    signal_the_value_on_release = true;
                }
            });
            
            var releases_subs = the_guard.s_Releases_Stream()
                .Subscribe(value =>
                {
                    if (!signal_the_value_on_release) 
                        return;
                    
                    the_action(the_source.s_Value);
                    signal_the_value_on_release = false;
                });

            return () =>
            {
                subs();
                releases_subs();
            };
        }

        private readonly Cell<T> the_source;
        private readonly SignalGuard the_guard;
    }
}