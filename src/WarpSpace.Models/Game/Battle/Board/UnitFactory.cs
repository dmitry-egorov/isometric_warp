using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class UnitFactory
    {
        public IStream<UnitCreated> Stream_Of_Unit_Creations => _stream_of_unit_creations;

        public bool Can_Create_a_Unit_At(LocationModel location) => location.Is_Empty();
        
        public void Create_a_Unit(UnitDescription desc, LocationModel initial_location)
        {
            Debug.Log($"Creating {desc.Type} at {ToString(initial_location)}");
            Can_Create_a_Unit_At(initial_location).Otherwise_Throw("Can't create a unit at the location");
            
            var unit = new UnitModel(desc.Type, desc.Faction, desc.Inventory_Content, initial_location);

            initial_location.Set_the_Occupant_To(unit);
            
            Signal_the_Creation();

            Create_Units_In_The_Bay(desc, unit);

            void Signal_the_Creation()
            {
                var unit_created = new UnitCreated(unit, unit.Location);
                _stream_of_unit_creations.Next(unit_created);                    
            }            
        }

        private static string ToString(LocationModel initial_location)
        {
            Debug.Log($"Not null loc: {initial_location != null}");
            Debug.Log(initial_location.ToString());
            if (initial_location.Is_a_Tile(out var tile)) return tile.Position.ToString();


            var must_be_a_bay = initial_location.Must_Be_a_Bay();
            Debug.Log((must_be_a_bay != null).ToString());
            Debug.Log(must_be_a_bay.ToString());
            return must_be_a_bay.Owner.Type.ToString();
        }

        private void Create_Units_In_The_Bay(UnitDescription desc, UnitModel unit)
        {
            if (!unit.Has_a_Bay(out var bay)) 
                return;
            
            var content = desc.Bay_Content.Must_Have_a_Value();
            (bay.Size >= content.Count).Otherwise_Throw("Actual unit's bay size is smaller then the described content size");

            for (var i = 0; i < content.Count; i++)
            {
                Debug.Log($"Creating unit {i}");
                var possible_unit_in_the_bay = content[i];
                if (possible_unit_in_the_bay.Has_a_Value(out var unit_in_the_bay))
                    Create_a_Unit(unit_in_the_bay, bay[i]);
            }
        }

        private readonly Stream<UnitCreated> _stream_of_unit_creations = new Stream<UnitCreated>();
    }
}