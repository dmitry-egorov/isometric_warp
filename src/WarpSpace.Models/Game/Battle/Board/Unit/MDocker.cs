using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Tile;
using static Lanski.Structures.Flow;

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

        public bool can_Dock_At(MTile the_tile, out MBaySlot the_bay_slot) =>
            default_as(out the_bay_slot) &&
            the_tile.has_a_Unit_With_an_Empty_Bay_Slot(out the_bay_slot) && 
            its_owner.can_Dock_To(the_bay_slot)
        ;
        
        private readonly MUnit its_owner;
    }

    public static class MDockerExtensions
    {
        public static bool can_Dock_At(this Possible<MDocker> the_possible_docker, MTile the_tile, out MBaySlot the_bay_slot) =>
            default_as(out the_bay_slot) &&
            the_possible_docker.has_a_Value(out var the_docker) &&
            the_docker.can_Dock_At(the_tile, out the_bay_slot)
        ;
    }
}