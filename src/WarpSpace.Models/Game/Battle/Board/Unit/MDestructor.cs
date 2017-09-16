﻿using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MDestructor
    {
        public MDestructor(MUnit owner, SignalGuard the_signal_guard)
        {
            its_owner = owner;
            it_destructed = new GuardedStream<TheVoid>(the_signal_guard);
        }

        public IStream<TheVoid> Destructed => it_destructed;

        internal void Destructs()
        {
            var the_loot = its_owner.s_Inventory_Content.and(its_owner.s_Loot);

            if (its_owner.is_At_a_Tile(out var the_tile))
                the_tile.Creates_a_Debris_with(the_loot);

            if (its_owner.is_At_a_Bay(out var the_bay))
                the_bay.s_Owner.Takes(the_loot);

            this.sends_the_destruction_signal();
        }

        private void sends_the_destruction_signal() => it_destructed.Next();

        private readonly MUnit its_owner;
        private readonly GuardedStream<TheVoid> it_destructed;
    }
}