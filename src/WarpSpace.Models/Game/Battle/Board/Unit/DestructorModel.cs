using Lanski.Reactive;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class DestructorModel
    {
        public IStream<UnitDestroyed> Stream_Of_Single_Destroyed_Event => _stream_of_single_destroyed_event;

        public DestructorModel(UnitModel unit)
        {
            _unit = unit;
        }

        public void Destroy()
        {
            //TODO: remove current tile from unit (requires using slot)
            var location = _unit.Current_Tile;
            var debris = StructureDescription.Debris(TileHelper.GetOrientation(location.Position), _unit.Inventory.Content);

            location.Reset_Unit();
            location.Set_Structure(debris);
            
            _stream_of_single_destroyed_event.Next(new UnitDestroyed(_unit, location));
        }
        
        private readonly UnitModel _unit;
        private readonly Stream<UnitDestroyed> _stream_of_single_destroyed_event = new Stream<UnitDestroyed>();
    }
}