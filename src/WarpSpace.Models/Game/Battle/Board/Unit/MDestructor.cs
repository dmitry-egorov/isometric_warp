using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MDestructor
    {
        public MDestructor(MUnit the_owner, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;
            it_destructed = new GuardedStream<TheVoid>(the_signal_guard);
        }

        public IStream<TheVoid> Destructed => it_destructed;

        internal void Destructs()
        {
            var the_loot = its_owner.s_Loot();

            if (its_owner.is_At_a_Tile(out var the_tile))
            {
                the_tile.Creates_a_Debris_With(the_loot);
            }
            else
            {
                var the_bay = its_owner.must_be_At_a_Bay();
                the_bay.s_Owner.Takes(the_loot);
            }

            it_destructed.Happens();
        }

        private readonly MUnit its_owner;
        private readonly GuardedStream<TheVoid> it_destructed;
    }
}