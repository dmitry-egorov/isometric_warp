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
            the_last_held_value = initial_value;
            
            the_cached_process_lambda = Processes_the_Guard_Release;
        }

        public T s_Value
        {
            get => the_source.s_Value;
            set => Next(value);
        }

        public void Next(T value)
        {
            the_last_held_value = value;

            if (the_guard.is_Letting_Events_Through())
            {
                the_source.s_Value = value;
            }
        }
        
        public Action Subscribe(Action<T> the_action)
        {
            var the_new_subscription = the_source.Subscribe(the_action);
            this.Registers_a_New_Subscription();

            return () =>
            {
                the_new_subscription();
                this.Removes_a_Subscription();
            };
        }

        private void Removes_a_Subscription()
        {
            the_subscribers_count--;
            if (the_subscribers_count == 0)
            {
                the_subscription();
            }
        }

        private void Registers_a_New_Subscription()
        {
            the_subscribers_count++;
            if (the_subscribers_count == 1)
            {
                the_subscription = 
                    the_guard.s_Releases_Stream()
                    .Subscribe(the_cached_process_lambda);
            }
        }

        private void Processes_the_Guard_Release(TheVoid _)
        {
            the_source.s_Value = the_last_held_value;
        }

        private readonly Cell<T> the_source;
        private readonly SignalGuard the_guard;
        private readonly Action<TheVoid> the_cached_process_lambda;
        
        private T the_last_held_value;
        private int the_subscribers_count;
        private Action the_subscription;
    }
}