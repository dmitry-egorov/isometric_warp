using System.Linq;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game.Battle
{
    public static class BattleFactory
    {
        public static BattleModel From(BoardDescription description, GameModel game)
        {
            var board = new BoardModel(description, new InteractorFactory(game));
            var player = new PlayerModel();

            var units = description.Units.EnumerateWithIndex().Select(x => x.element.Select(unit => (unit, x.index))).SkipNull();
            foreach (var x in units)
            {
                var unit = x.Item1;
                var index = x.Item2;
                board.Create_a_Unit(unit.Type, index, Faction.Natives, unit.Inventory_Content); 
            }

            return new BattleModel(board, player);
        }
    }
}