using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MChassis
    {
        public MChassis(MUnit the_owner, MLocation initial_location, SignalGuard the_signal_guard)
        {
            s_owner = the_owner;
            s_type = the_owner.s_Type.s_Chassis_Type();
            s_max_moves = the_owner.s_Type.s_Max_Moves();
            s_moves_left = it.s_max_moves;

            s_cell_of_locations = new GuardedCell<MLocation>(initial_location, the_signal_guard);
            s_stream_of_movements = it.s_cell_of_locations.s_Changes().Select(x => new Movement(x.previous, x.current));
            s_stream_of_dock_states = it.s_cell_of_locations.Select(x => x.is_a_Bay());
        }

        public bool is_Docked => it.s_location.is_a_Bay();
        public MLocation s_Location => it.s_location;
        public MTile s_Tile => it.s_location.s_Tile;

        public IStream<Movement> s_Movements_Stream => it.s_stream_of_movements;
        public IStream<bool> s_Dock_States_Stream => it.s_stream_of_dock_states;

        public Possible<MTile> s_Location_As_a_Tile() => it.s_location.as_a_Tile();
        public bool is_At(MTile the_tile) => it.s_location.@is(the_tile);
        public bool is_Adjacent_To(MUnit the_unit) => it.s_location.is_Adjacent_To(the_unit.s_Chassis.s_Location);
        public MTile must_be_At_a_Tile() => it.s_location.must_be_a_Tile();
        public bool is_Adjacent_To(MStructure the_structure) => it.s_location.is_Adjacent_To(the_structure);
        public bool is_At_a_Tile() => it.s_location.is_a_Tile();
        public bool is_At_a_Tile(out MTile the_tile) => it.s_location.is_a_Tile(out the_tile);
        public bool is_At_a_Bay() => it.s_location.is_a_Bay();
        public bool is_At_a_Bay(out MBay the_tile) => it.s_location.is_a_Bay(out the_tile);

        public bool can_Move() => it.has_moves_left();

        public bool can_Move_To(MLocation the_destination) =>
            it.can_Move() &&
            the_destination.is_Passable_By(it.s_type) &&
            the_destination.is_Empty() &&
            it.s_location.is_Accessible_From(the_destination)
        ;
        
        public bool can_Move_To(MTile the_tile, out MLocation the_tiles_location) => 
            the_tile.has_a_Location(out the_tiles_location) && 
            it.can_Move_To(the_tiles_location)
        ;
        
        public bool can_Dock_At(MTile the_tile, out MLocation the_target_location) =>
            semantic_resets(out the_target_location) &&
            the_tile.has_a_Unit(out var the_unit) && 
            the_unit.has_a_Bay(out var the_bay) && 
            the_bay.has_an_Empty_Slot(out the_target_location) && 
            it.can_Move_To(the_target_location)
        ;
        
        public bool can_Undock_At(MTile the_target_tile, out MLocation the_target_location) => 
            the_target_tile.has_a_Location(out the_target_location) && 
            it.can_Move_To(the_target_location)
        ;

        internal void Moves_To(MLocation the_destination)
        {
            it.can_Move_To(the_destination).Otherwise_Throw("Can't move the unit to the destination");

            the_destination.must_be_Empty();
            
            var the_old_location = it.s_location;
            it.s_location_becomes(the_destination);
            the_old_location.Becomes_Empty();
            the_destination.s_Occupant_Becomes(it.s_owner);
            it.spends_a_move();
        }

        internal void Finishes_the_Turn() => it.restores_the_moves();

        private bool has_moves_left() => it.s_moves_left > 0;
        private void s_location_becomes(MLocation the_new_location) => it.s_cell_of_locations.s_Value = the_new_location;
        private void restores_the_moves() => it.s_moves_left = it.s_max_moves;
        private void spends_a_move() => it.s_moves_left--;
        
        private MChassis it => this;
        
        private MLocation s_location => it.s_cell_of_locations.s_Value;

        private readonly MUnit s_owner;
        private readonly ChassisType s_type;
        private readonly GuardedCell<MLocation> s_cell_of_locations;
        private readonly IStream<Movement> s_stream_of_movements;
        private readonly IStream<bool> s_stream_of_dock_states;
        private readonly int s_max_moves;
        
        private int s_moves_left;
    }
}