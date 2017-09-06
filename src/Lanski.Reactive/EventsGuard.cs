using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public class EventsGuard
    {
        public EventsGuard()
        {
            the_hold_requests_counts_cell = new Cell<int>(0);
            the_releaser = new Releaser(the_hold_requests_counts_cell);
            
            the_stream_of_releases = 
                the_hold_requests_counts_cell
                .Where(count => count == 0)
                .Select(_ => TheVoid.Instance)
            ;
        }

        public IStream<TheVoid> s_Releases_Stream() => the_stream_of_releases;
        public bool is_Holding_Events() => the_hold_requests_counts_cell.s_Value > 0;
        public bool is_Letting_Events_Through() => !this.is_Holding_Events();

        public IDisposable Holds_All_Events()
        {
            the_hold_requests_counts_cell.s_Value++;
            return the_releaser;
        }

        private readonly Cell<int> the_hold_requests_counts_cell;
        private readonly IStream<TheVoid> the_stream_of_releases;
        private readonly Releaser the_releaser;

        
        private class Releaser: IDisposable
        {
            private readonly Cell<int> s_guards_cell_of_hold_requests_count;

            public Releaser(Cell<int> the_guards_cell_of_hold_requests_count)
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