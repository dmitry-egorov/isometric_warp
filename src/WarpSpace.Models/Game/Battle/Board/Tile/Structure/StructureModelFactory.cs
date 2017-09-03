using System;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure
{
    public class StructureModelFactory
    {
        private readonly GameModel _game;

        public StructureModelFactory(GameModel game)
        {
            _game = game;
        }

        public StructureModel Create(StructureDescription desc, TileModel tile)
        {
            var interactor = CreateInteractor();
            return new StructureModel(desc, interactor);

            IInteractor CreateInteractor()
            {
                if (desc.Is_An_Entrance())
                    return new Empty();
                if (desc.Is_An_Exit())
                    return new Exiter(_game, tile);
                if (desc.Is_A_Debris(out var debrie))
                    return new DebrieInteractor(tile, debrie.Loot);
            
                throw new ArgumentOutOfRangeException(nameof(desc), desc, null);
            }
        }
    }
}