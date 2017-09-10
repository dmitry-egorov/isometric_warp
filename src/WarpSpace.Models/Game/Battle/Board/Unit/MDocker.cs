using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Tile;
using static WarpSpace.Models.Descriptions.UnitType;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MDocker
    {
        public static Possible<MDocker> From(MUnit the_unit) =>
            the_unit.is_not(a_Mothership) 
            ? new MDocker(the_unit) 
            : Possible.Empty<MDocker>()
        ;

        private MDocker(MUnit the_owner)
        {
            its_owner = the_owner;
        }

        public bool can_Dock_At(MTile the_tile, out MLocation the_target_location) =>
            Semantics.semantic_resets(out the_target_location) &&
            the_tile.has_a_unit_with_an_empty_bay_slot(out the_target_location) && 
            its_owner.can_Move_To(the_target_location)
        ;
        
        private readonly MUnit its_owner;
    }
}