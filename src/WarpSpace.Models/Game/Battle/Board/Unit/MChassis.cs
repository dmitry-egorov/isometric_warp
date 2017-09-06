using Lanski.Reactive;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    internal class MChassis
    {
        public bool is_Docked() => the_location().is_a_Bay();
        public MLocation s_Location() => the_location();

        public IStream<Movement> s_Stream_of_Movements() => the_stream_of_movements;
        public IStream<bool> s_Stream_of_Dock_States() => the_stream_of_dock_states;

        public MChassis(UnitType the_owners_type, MLocation initial_location, EventsGuard the_events_guard)
        {
            the_type = the_owners_type.s_Chassis_Type();
            
            the_cell_of_locations = new GuardedCell<MLocation>(initial_location, the_events_guard);
            the_stream_of_movements = the_cell_of_locations.s_Changes().Select(x => new Movement(x.previous, x.current));
            the_stream_of_dock_states = the_cell_of_locations.Select(x => x.is_a_Bay());
        }

        public bool Can_Move_To(MTile the_destination) => 
            the_destination.is_Passable_By(the_type)
            && the_location().is_Adjacent_To(the_destination)
        ;

        internal void Sets_the_Location_To(MLocation the_new_location)
        {
            the_new_location.Must_Be_Empty();
            
            the_location_becomes(the_new_location);
        }
        
        private void the_location_becomes(MLocation the_new_location) => the_cell_of_locations.s_Value = the_new_location;
        private MLocation the_location() => the_cell_of_locations.s_Value;

        private readonly ChassisType the_type;
        private readonly GuardedCell<MLocation> the_cell_of_locations;
        private readonly IStream<Movement> the_stream_of_movements;
        private readonly IStream<bool> the_stream_of_dock_states;
    }
}