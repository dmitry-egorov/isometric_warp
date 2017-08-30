using System.Linq;
using Lanski.Linq;
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
            var tiles = description.Tiles.Map((t, i) => CreateTile(i, t, game));
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
                board.AddUnit(unit.Type, index, index, false);
            }

            return new BattleModel(board, player);
        }

        private static TileModel CreateTile(Index2D i, TileDescription t, GameModel game)
        {
            var structure = t.Structure.SelectRef(s => new StructureModel(s, game));
            return new TileModel(i, t.Type, structure);
        }
    }
}