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
            var the_tile = its_owner.s_Location();
            the_tile.Creates_a_Debris_With(the_loot);

            it_destructed.Happens();
        }

        private readonly MUnit its_owner;
        private readonly GuardedStream<TheVoid> it_destructed;
    }
}