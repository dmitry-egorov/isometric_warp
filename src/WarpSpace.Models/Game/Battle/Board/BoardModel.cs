using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class BoardModel
    {
        public readonly TileModel[,] Tiles;
        
        public IStream<UnitAdded> Stream_Of_Added_Units => _stream_of_added_units;
        public IStream<UnitDestroyed> Stream_Of_Destroyed_Units => _stream_of_destoryed_units;

        public BoardModel(TileModel[,] tiles, Spacial2D entranceSpacial)
        {
            _entrance_spacial = entranceSpacial;
            Tiles = tiles;
        }

        public void Warp_In_the_Mothership()
        {
            var position = _entrance_spacial.Position;
            var orientation = _entrance_spacial.Orientation;
            
            var source = Tiles.Get(position);
            var initial_tile = Tiles.Get(position + orientation);
            var mothership = new UnitModel(UnitType.Mothership, initial_tile, Faction.Players, null);//TODO: Rememeber and pass inventory
            
            Add(mothership, source);
        }

        public void Add_a_Unit(UnitType type, Index2D position, Index2D source_position, Faction faction, InventoryContent? initial_inventory_content)
        {
            var initial_tile = Tiles.Get(position);
            var source_tile = Tiles.Get(source_position);
            
            var unit = new UnitModel(type, initial_tile, faction, initial_inventory_content);
            Add(unit, source_tile);
        }

        private void Add(UnitModel unit, TileModel source)
        {
            Wire_the_Destruction();
            _stream_of_added_units.Next(new UnitAdded(unit, source));
        
            void Wire_the_Destruction()
            {
                unit
                    .Stream_Of_Single_Destroyed_Event
                    .Subscribe(destroyed => _stream_of_destoryed_units.Next(destroyed));
            }
        }
        
        private readonly RepeatAllStream<UnitDestroyed> _stream_of_destoryed_units = new RepeatAllStream<UnitDestroyed>();
        private readonly RepeatAllStream<UnitAdded> _stream_of_added_units = new RepeatAllStream<UnitAdded>();
        private readonly Spacial2D _entrance_spacial;
    }
}