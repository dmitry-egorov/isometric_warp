using System;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public static class Factory
    {
        public static IInteractor From(StructureType type, GameModel game, TileModel tile)
        {
            switch (type)
            {
                case StructureType.Entrance:
                    return new Empty();
                case StructureType.Exit:
                    return new Exiter(game, tile);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}