using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class UnitFactory
    {
        public IStream<UnitCreated> Stream_Of_Unit_Creations => _stream_of_unit_creations;

        public bool Can_Create_a_Unit_At(LocationModel location) => location.Is_Empty();
        
        public void Create_a_Unit(UnitDescription desc, LocationModel initial_location)
        {
            Can_Create_a_Unit_At(initial_location).Otherwise_Throw("Can't create a unit at the location");
            
            var unit = new UnitModel(desc, initial_location);

            initial_location.Set_the_Occupant_To(unit);
            
            Signal_the_Creation();
    
            void Signal_the_Creation()
            {
                var unit_created = new UnitCreated(unit, unit.Location);
                _stream_of_unit_creations.Next(unit_created);                    
            }            
        }
        
        private readonly Stream<UnitCreated> _stream_of_unit_creations = new Stream<UnitCreated>();
    }
}