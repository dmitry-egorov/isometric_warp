using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class UnitFactory
    {
        public IStream<MUnit> Stream_Of_Unit_Creations => _stream_of_unit_creations;

        public bool Can_Create_a_Unit_At(MLocation location) => location.Is_Empty();
        
        public void Create_a_Unit(UnitDescription desc, MLocation initial_location)
        {
            Can_Create_a_Unit_At(initial_location).Otherwise_Throw("Can't create a unit at the location");
            
            var unit = new MUnit(desc.Type, desc.Faction, desc.Inventory_Content, initial_location, new EventsGuard());

            initial_location.Sets_the_Occupant_To(unit);
            
            Signal_the_Creation(unit);

            Create_Units_In_The_Bay(desc, unit);
        }

        private void Create_Units_In_The_Bay(UnitDescription desc, MUnit unit)
        {
            if (!unit.Has_a_Bay(out var bay)) 
                return;
            
            var content = desc.Bay_Content.Must_Have_a_Value();
            (bay.Size >= content.Count).Otherwise_Throw("Actual unit's bay size is smaller then the described content size");

            for (var i = 0; i < content.Count; i++)
            {
                var possible_unit_in_the_bay = content[i];
                if (possible_unit_in_the_bay.Has_a_Value(out var unit_in_the_bay))
                    Create_a_Unit(unit_in_the_bay, bay[i].Must_Have_a_Value());
            }
        }
        
        void Signal_the_Creation(MUnit unit)
        {
            _stream_of_unit_creations.Next(unit);                    
        }

        private readonly Stream<MUnit> _stream_of_unit_creations = new Stream<MUnit>();
    }
}