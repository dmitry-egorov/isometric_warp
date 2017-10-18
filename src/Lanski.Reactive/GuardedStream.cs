using System;
using System.Collections.Generic;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public class GuardedStream<T> : IStream<T>, IConsumer<T>
    {
        public GuardedStream(SignalGuard the_signal_guard)
        {
            the_guard = the_signal_guard;
            
            the_source = new Stream<T>();
            the_queue_of_delayed_events = new Queue<T>();
        }

        public void Happends_With(T value) => Next(value);
        public void Next(T value)
        {
            if (this.the_guard.is_Letting_Events_Through())
            {
                the_source.Next(value);
            }
            else
            {
                the_queue_of_delayed_events.Enqueue(value);
            }
        }
        
        public Action Subscribe(Action<T> the_action)
        {
            var the_new_subscription = the_source.Subscribe(the_action);
            this.adds_a_Subscription();

            return () =>
            {
                the_new_subscription();
                this.removes_a_subscription();
            };
        }

        private void removes_a_subscription()
        {
            the_subscribers_count--;
            if (the_subscribers_count == 0)
            {
                the_subscription();
            }
        }

        private void adds_a_Subscription()
        {
            the_subscribers_count++;
            if (the_subscribers_count == 1)
            {
                the_subscription = the_guard.s_Releases_Stream().Subscribe(processes_the_guards_change);
            }
        }

        private void processes_the_guards_change(TheVoid _)
        {
            var the_queue = the_queue_of_delayed_events;
            while (the_queue.Count != 0)
            {
                var a_held_event = the_queue.Dequeue();
                the_source.Next(a_held_event);
            }
        }

        private readonly Stream<T> the_source;
        private readonly SignalGuard the_guard;
        
        private readonly Queue<T> the_queue_of_delayed_events;
        private int the_subscribers_count;
        private Action the_subscription;
    }
}