using System;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public static class Factory
    {
        private static readonly Empty Empty = new Empty();

        public static IInteractor From(StructureType type, Game.GameModel game)
        {
            switch (type)
            {
                case StructureType.Entrance:
                    return Empty;
                case StructureType.Exit:
                    return new Exiter(game);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}