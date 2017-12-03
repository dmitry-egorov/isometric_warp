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
        public MLocator(MUnit the_owner, MLocation initial_location, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;
            its_chassis_type = the_owner.s_Chassis_Type();
            var its_total_moves = the_owner.s_Total_Moves();
            its_uses_Limiter = new MUsesLimiter(its_total_moves, the_signal_guard);

            its_locations_cell = new GuardedCell<MLocation>(initial_location, the_signal_guard);
            it_moved = its_locations_cell.s_Changes().Select(x => new Movement(x.previous, x.current));
            its_is_docked_cell = its_locations_cell.Select(x => x.is_a_Bay_Slot());
        }

        public bool s_Is_Docked => its_is_docked_cell.s_Value;
        public bool can_Move => its_uses_Limiter.has_Uses_Left;

        public MLocation s_Location => its_location;
        public Possible<Index2D> s_Possible_Position => its_location.s_Possible_Position;

        public ICell<int> s_Moves_Left_Cell => its_uses_Limiter.s_Uses_Left_Cell;
        public ICell<bool> s_Can_Move_Cell => its_uses_Limiter.s_Has_Uses_Left_Cell;
        public ICell<bool> s_Is_Docked_Cell => its_is_docked_cell;
        public IStream<Movement> Moved => it_moved;

        public bool is_Adjacent_To(MTile the_tile) => its_location.is_Adjacent_To(the_tile);
        public bool is_Adjacent_To(MLocator the_other_locator) => its_location.is_Adjacent_To(the_other_locator.s_Location);
        public MTile must_be_At_a_Tile() => its_location.must_be_a_Tile();
        public MBay must_be_At_a_Bay() => its_location.must_be_a_Bay();
        public bool is_Adjacent_To(MStructure the_structure) => its_location.is_Adjacent_To(the_structure);
        public bool is_At_a_Tile(out MTile the_tile) => its_location.is_a_Tile(out the_tile);
        public bool is_At_a_Bay(out MBay the_bay) => its_location.is_a_Bay(out the_bay);

        public bool can_Move_To(MTile the_destination) => it_can_move_to(the_destination);
        public bool can_Dock_To(MBaySlot the_destination) => it_can_dock_to(the_destination);

        internal void Moves_To(MTile the_destination)
        {
            it_can_move_to(the_destination).Otherwise_Throw("Can't move the unit to the destination");

            it_changes_location_to(the_destination);
        }

        internal void Docks_To(MBaySlot the_destination)
        {
            it_can_dock_to(the_destination).Otherwise_Throw("Can't dock the unit to the destination");

            it_changes_location_to(the_destination);
        }

        internal void Finishes_the_Turn() => its_uses_Limiter.Restores_the_Uses();

        private void it_changes_location_to(MLocation the_destination)
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

        private void its_location_becomes(MLocation the_new_location) => its_location = the_new_location;

        private bool it_can_move_to(MTile the_destination) => 
            this.can_Move &&
            the_destination.is_Empty() &&
            the_destination.is_Passable_By(its_chassis_type) &&
            the_destination.is_Adjacent_To(its_location.s_Tile)
        ;

        private bool it_can_dock_to(MBaySlot the_destination) => 
            this.can_Move &&
            the_destination.is_Empty() &&
            the_destination.is_Accessible_From(its_location.s_Tile)
        ;

        private MLocation its_location
        {
            get => its_locations_cell.s_Value;
            set => its_locations_cell.s_Value = value;
        }

        private readonly MUnit its_owner;
        private readonly MChassisType its_chassis_type;
        private readonly MUsesLimiter its_uses_Limiter;
        private readonly GuardedCell<MLocation> its_locations_cell;
        
        private readonly ICell<bool> its_is_docked_cell;
        private readonly IStream<Movement> it_moved;

    }
}