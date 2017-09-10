using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MMover
    {
        public MMover(MUnit the_owner, MLocation initial_location, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;
            its_chassis_type = the_owner.s_Type.s_Chassis_Type();
            var its_max_moves = the_owner.s_Type.s_Max_Moves();
            its_uses_Limiter = new MUsesLimiter(its_max_moves, the_signal_guard);

            its_cell_of_locations = new GuardedCell<MLocation>(initial_location, the_signal_guard);
            its_stream_of_movements = its_cell_of_locations.s_Changes().Select(x => new Movement(x.previous, x.current));
            its_stream_of_dock_states = its_cell_of_locations.Select(x => x.is_a_Bay());
        }

        public bool is_Docked => its_location.is_a_Bay();
        public bool can_Move => its_uses_Limiter.has_Uses_Left;

        public MLocation s_Location => its_location;
        public MTile s_Tile => its_location.s_Tile;
        
        public ICell<bool> s_can_Move_Cell => its_uses_Limiter.s_Has_Uses_Left_Cell;
        public IStream<Movement> s_Movements_Stream => its_stream_of_movements;
        public IStream<bool> s_Dock_States_Stream => its_stream_of_dock_states;

        public Possible<MTile> s_Location_As_a_Tile() => its_location.as_a_Tile();
        public bool is_At(MTile the_tile) => its_location.@is(the_tile);
        public bool is_Adjacent_To(MUnit the_unit) => its_location.is_Adjacent_To(the_unit.s_Mover.s_Location);
        public MTile must_be_At_a_Tile() => its_location.must_be_a_Tile();
        public bool is_Adjacent_To(MStructure the_structure) => its_location.is_Adjacent_To(the_structure);
        public bool is_At_a_Tile() => its_location.is_a_Tile();
        public bool is_At_a_Tile(out MTile the_tile) => its_location.is_a_Tile(out the_tile);
        public bool is_At_a_Bay() => its_location.is_a_Bay();
        public bool is_At_a_Bay(out MBay the_tile) => its_location.is_a_Bay(out the_tile);


        public bool can_Move_To(MLocation the_destination) =>
            this.can_Move &&
            the_destination.is_Passable_By(its_chassis_type) &&
            the_destination.is_Empty() &&
            its_location.is_Accessible_From(the_destination)
        ;
        
        public bool can_Move_To(MTile the_tile, out MLocation the_tiles_location) => 
            the_tile.has_a_Location(out the_tiles_location) && 
            this.can_Move_To(the_tiles_location)
        ;

        internal void Moves_To(MLocation the_destination)
        {
            this.can_Move_To(the_destination).Otherwise_Throw("Can't move the unit to the destination");

            the_destination.must_be_Empty();
            
            var the_old_location = its_location;
            this.s_location_becomes(the_destination);
            the_old_location.Becomes_Empty();
            the_destination.s_Occupant_Becomes(its_owner);
            its_uses_Limiter.Spends_a_Use();
        }

        internal void Finishes_the_Turn() => its_uses_Limiter.Restores_the_Uses();

        private void s_location_becomes(MLocation the_new_location) => its_cell_of_locations.s_Value = the_new_location;

        private MLocation its_location => its_cell_of_locations.s_Value;

        private readonly MUnit its_owner;
        private readonly ChassisType its_chassis_type;
        private readonly MUsesLimiter its_uses_Limiter;
        private readonly GuardedCell<MLocation> its_cell_of_locations;
        private readonly IStream<Movement> its_stream_of_movements;
        private readonly IStream<bool> its_stream_of_dock_states;
    }
}