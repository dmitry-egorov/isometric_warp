using System.Linq;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game.Battle
{
    public static class BattleFactory
    {
        public static BattleModel From(BoardDescription description, GameModel game)
        {
            var structure_factory = new StructureModelFactory(game);
            var tiles = description.Tiles.Map((tile_desc, position) => CreateTile(position, tile_desc));
            foreach (var i in tiles.EnumerateIndex())
            {
                var adjacent = tiles.GetAdjacent(i);
                tiles.Get(i).Init(adjacent);
            }
            
            var board = new BoardModel(tiles, description.EntranceSpacial);
            var player = new PlayerModel();

            var units = description.Units.EnumerateWithIndex().Select(x => x.element.Select(unit => (unit, x.index))).SkipNull();
            foreach (var x in units)
            {
                var unit = x.Item1;
                var index = x.Item2;
                board.Add_a_Unit(unit.Type, index, index, Faction.Natives, unit.Inventory_Content);
            }

            return new BattleModel(board, player);

            TileModel CreateTile(Index2D position, TileDescription desc)
            {
                var tile = new TileModel(position, desc.Type, structure_factory);
                tile.Set_Structure(desc.Structure);
                return tile;
            }
        }
    }
}