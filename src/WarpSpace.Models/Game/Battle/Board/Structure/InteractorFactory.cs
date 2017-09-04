using System;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public class InteractorFactory
    {
        private readonly GameModel _game;

        public InteractorFactory(GameModel game)
        {
            _game = game;
        }

        public Interactor Create(StructureDescription desc, StructureModel structure)
        {
            if (desc.Is_An_Entrance())
                return Interactor.Create.Empty();
            if (desc.Is_An_Exit())
                return Interactor.Create.Exiter(structure, _game);
            if (desc.Is_A_Debris(out var debrie))
                return Interactor.Create.Debris(structure, debrie.Loot);

            throw new ArgumentOutOfRangeException(nameof(desc), desc, null);
        }
    }
}