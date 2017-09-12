using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MDocker
    {
        public static Possible<MDocker> From(MUnit the_unit) =>
            the_unit.can_Dock() 
            ? new MDocker(the_unit) 
            : Possible.Empty<MDocker>()
        ;

        private MDocker(MUnit the_owner)
        {
            its_owner = the_owner;
        }

        public bool can_Dock_At(MTile the_tile, out MUnitLocation the_target_location) =>
            Semantics.semantic_resets(out the_target_location) &&
            the_tile.has_a_unit_with_an_empty_bay_slot(out the_target_location) && 
            its_owner.can_Move_To(the_target_location)
        ;
        
        private readonly MUnit its_owner;
    }
}