using Lanski.Reactive;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MDestructor
    {
        public MDestructor(MUnit owner, SignalGuard the_signal_guard)
        {
            its_owner = owner;
            its_destruction_signal = new GuardedStream<TheVoid>(the_signal_guard);
        }

        public IStream<TheVoid> s_Destruction_Signal => its_destruction_signal;

        internal void Destructs()
        {
            var the_loot = its_owner.s_Inventory_Content;

            if (its_owner.is_At_a_Tile(out var the_tile))
                the_tile.Creates_a_Debris_with(the_loot);

            if (its_owner.is_At_a_Bay(out var the_bay))
                the_bay.s_Owner.Takes(the_loot);

            this.sends_the_destruction_signal();
        }

        private void sends_the_destruction_signal() => its_destruction_signal.Next();

        private readonly MUnit its_owner;
        private readonly GuardedStream<TheVoid> its_destruction_signal;
    }
}