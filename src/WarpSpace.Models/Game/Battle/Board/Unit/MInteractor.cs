using System;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    internal class MInteractor
    {
        public MInteractor(MUnit the_owner, MGame the_game)
        {
            its_owner = the_owner;
            this.the_game = the_game;
        }

        public bool can_Interact_With_a_Structure_At(MTile the_tile, out MStructure the_target_structure) => 
            the_tile.has_a_Structure(out the_target_structure) && 
            this.it_can_interact_with(the_target_structure)
        ;

        public void Interacts_With(MStructure the_structure)
        {
            this.it_can_interact_with(the_structure).Otherwise_Throw("Can't interact with the structure");

            if (the_structure.is_an_Exit())
            {
                the_game.Starts_a_New_Battle();
            }
            else if (the_structure.is_a_Debris())
            {
                its_owner.Loots(the_structure);
            }
            else
            {
                throw new InvalidOperationException("Can't interact");
            }
        }
        
        private bool it_can_interact_with(MStructure the_structure) => 
            its_owner.is_Adjacent_To(the_structure) && 
            (
                the_structure.is_an_Exit() && its_owner.can_Exit && its_owner.can_Move || 
                the_structure.is_a_Debris() 
            )
        ;
        
        private readonly MUnit its_owner;
        private readonly MGame the_game;
    }
}