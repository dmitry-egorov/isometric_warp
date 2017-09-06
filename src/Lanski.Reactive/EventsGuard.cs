using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public class EventsGuard
    {
        public EventsGuard()
        {
            the_cell_of_hold_requests_count = new ValueCell<int>(0);
            the_releaser = new Releaser(the_cell_of_hold_requests_count);
            
            the_stream_of_releases = 
                the_cell_of_hold_requests_count
                .Where(the_hold_requests_count => the_hold_requests_count == 0)
                .Select(_ => TheVoid.Instance)
            ;
        }

        public IStream<TheVoid> s_Stream_Of_Releases() => the_stream_of_releases;
        public bool is_Holding_Events() => the_cell_of_hold_requests_count.s_Value > 0;
        public bool is_Letting_Events_Through() => !this.is_Holding_Events();

        public IDisposable Holds_All_Events()
        {
            the_cell_of_hold_requests_count.s_Value++;
            return the_releaser;
        }

        private readonly ValueCell<int> the_cell_of_hold_requests_count;
        private readonly IStream<TheVoid> the_stream_of_releases;
        private readonly Releaser the_releaser;

        
        private class Releaser: IDisposable
        {
            private readonly ValueCell<int> s_guards_cell_of_hold_requests_count;

            public Releaser(ValueCell<int> the_guards_cell_of_hold_requests_count)
            {
                s_guards_cell_of_hold_requests_count = the_guards_cell_of_hold_requests_count;
            }

            public void Dispose()
            {
                s_guards_cell_of_hold_requests_count.s_Value--;
            }
        }
    }
}