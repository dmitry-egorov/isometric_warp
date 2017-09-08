using Lanski.Reactive;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MDestructor
    {
        public MDestructor(MUnit owner, SignalGuard the_signal_guard)
        {
            the_owner = owner;
            the_destruction_signal = new GuardedStream<TheVoid>(the_signal_guard);
        }

        public IStream<TheVoid> s_Destruction_Signal => the_destruction_signal;

        internal void Destructs()
        {
            var the_loot = the_owner.s_Inventory_Content;

            the_owner.s_Location.Becomes_Empty();

            if (the_owner.is_At_a_Tile(out var the_tile))
                the_tile.Creates_a_Debris_with(the_loot);

            if (the_owner.is_At_a_Bay(out var the_bay))
                the_bay.s_Owner.Takes(the_loot);

            this.sends_the_destruction_signal();
        }

        private void sends_the_destruction_signal() => the_destruction_signal.Next();

        private readonly MUnit the_owner;
        private readonly GuardedStream<TheVoid> the_destruction_signal;
    }
}