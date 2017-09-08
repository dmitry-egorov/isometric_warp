using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using static WarpSpace.Models.Descriptions.UnitType;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    internal class MInteractor
    {
        public MInteractor(MUnit the_owner, SignalGuard the_signal_guard)
        {
            s_owner = the_owner;

            s_exit_signal = new GuardedStream<TheVoid>(the_signal_guard);
        }

        public IStream<TheVoid> s_Exit_Signal => it.s_exit_signal;

        public bool can_Interact_With(MStructure the_structure) => 
            it.s_owner.is_Adjacent_To(the_structure) && 
            (
                the_structure.is_an_Exit() && it.s_owner.@is(a_Mothership) || 
                the_structure.is_a_Debris()
            )
        ;
        
        public bool can_Interact_With_a_Structure_At(MTile the_tile, out MStructure the_target_structure) => 
            the_tile.has_a_Structure(out the_target_structure) && 
            it.can_Interact_With(the_target_structure)
        ;

        public void Interacts_With(MStructure the_structure)
        {
            it.can_Interact_With(the_structure).Otherwise_Throw("Can't interact with the structure");

            if (the_structure.is_an_Exit())
            {
                it.sends_the_exit_signal();
            }
            else if (the_structure.is_a_Debris())
            {
                it.s_owner.Loots(the_structure);
            }
            else
            {
                throw new InvalidOperationException("Can't interact");
            }
        }

        private void sends_the_exit_signal() => it.s_exit_signal.Next();

        private MInteractor it => this;
        private readonly MUnit s_owner;
        private readonly GuardedStream<TheVoid> s_exit_signal;
    }
}