using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using static WarpSpace.Models.Descriptions.Passability;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MLocator
    {
        public MLocator(MUnit the_owner, MTile initial_location, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;
            its_chassis_type = the_owner.s_Chassis_Type();
            var its_total_moves = the_owner.s_Total_Moves();
            its_uses_Limiter = new MUsesLimiter(its_total_moves, the_signal_guard);

            its_locations_cell = new GuardedCell<MTile>(initial_location, the_signal_guard);
            it_moved = its_locations_cell.s_Changes().Select(x => new Movement(x.previous, x.current));
        }

        public bool can_Move => its_uses_Limiter.has_Uses_Left;

        public MTile s_Location => its_location;
        public Index2D s_Position => its_location.s_Position;

        public ICell<int> s_Moves_Left_Cell => its_uses_Limiter.s_Uses_Left_Cell;
        public ICell<bool> s_Can_Move_Cell => its_uses_Limiter.s_Has_Uses_Left_Cell;
        public IStream<Movement> Moved => it_moved;

        public bool is_Adjacent_To(MTile the_tile) => its_location.is_Adjacent_To(the_tile);
        public bool is_Adjacent_To(MLocator the_other_locator) => its_location.is_Adjacent_To(the_other_locator.s_Location);
        public bool is_Adjacent_To(MStructure the_structure) => its_location.is_Adjacent_To(the_structure.s_Location);

        public bool can_Move_To(MTile the_destination) => it_can_move_to(the_destination);

        internal void Moves_To(MTile the_destination)
        {
            it_can_move_to(the_destination).Otherwise_Throw("Can't move the unit to the destination");

            it_changes_location_to(the_destination);
        }

        internal void Finishes_the_Turn() => its_uses_Limiter.Restores_the_Uses();

        private void it_changes_location_to(MTile the_destination)
        {
            var the_old_location = its_location;
            its_location_becomes(the_destination);
            the_old_location.s_Occupant_Becomes_Empty();
            the_destination.s_Occupant_Becomes(its_owner);

            if (the_destination.s_Passability_With(its_chassis_type) == Single_Move)
            {
                its_uses_Limiter.Spends_a_Use();
            }
            else
            {
                its_uses_Limiter.Spends_All_Uses();
            }
        }

        private void its_location_becomes(MTile the_new_location) => its_location = the_new_location;

        private bool it_can_move_to(MTile the_destination) => 
            this.can_Move &&
            the_destination.is_Empty() &&
            the_destination.is_Passable_By(its_chassis_type) &&
            the_destination.is_Adjacent_To(its_location)
        ;

        private MTile its_location
        {
            get => its_locations_cell.s_Value;
            set => its_locations_cell.s_Value = value;
        }

        private readonly MUnit its_owner;
        private readonly MChassisType its_chassis_type;
        private readonly MUsesLimiter its_uses_Limiter;
        private readonly GuardedCell<MTile> its_locations_cell;
        
        private readonly IStream<Movement> it_moved;

    }
}