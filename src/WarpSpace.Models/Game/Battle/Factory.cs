using Lanski.Structures;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle
{
    public static class Factory
    {
        public static Model From(BoardDescription description)
        {
            var tiles = description.Tiles.Map((t, i) => new Board.Tile.Model(i, t.Type, t.Structure));
            foreach (var i in tiles.EnumerateIndex())
            {
                var adjacent = tiles.GetAdjacent(i);
                tiles.Get(i).Init(adjacent);
            }
            var board = new Board.Model(tiles, description.EntranceSpacial);
            var player = new Player.Model();
            
            return new Model(board, player);
        }
    }
}