using JetBrains.Annotations;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public struct Interactor
    {
        public static class Create
        {
            public static Interactor Exiter(StructureModel structure, GameModel game) => new Interactor {_variant = new Exiter(structure, game) };    
            public static Interactor Debris(StructureModel structure, InventoryContent? loot) => new Interactor { _variant = new Debris(structure, loot) };    
            public static Interactor Empty() => new Interactor() { _variant = TheVoid.Instance };    
        }
        
        public void Interact_With(UnitModel unit)
        {
            Can_Interact_With(unit).Otherwise_Throw("Can't interact with the unit");
            
            if (Is_an_Exitor(out var exiter)) exiter.Interact_With(unit);
            if (Is_a_Debris(out var debris))  debris.Interact_With(unit);
        }

        [Pure] public bool Can_Interact_With(UnitModel unit)
        {
            return Is_an_Exitor(out var exiter) && exiter.Can_Interact_With(unit)
                || Is_a_Debris(out var debris) && debris.Can_Interact_With(unit);
        }

        public bool Is_an_Exitor(out Exiter exiter) => _variant.Is_a_T1(out exiter);
        public bool Is_a_Debris(out Debris debris) => _variant.Is_a_T2(out debris); 
        public bool Is_Empty() => _variant.Is_a_T3();

        private Or<Exiter, Debris, TheVoid> _variant;
        
        public struct Exiter
        {
            private readonly GameModel _game;
            private readonly StructureModel _structure;

            public Exiter(StructureModel structure, GameModel game)
            {
                _game = game;
                _structure = structure;
            }

            public bool Can_Interact_With(UnitModel unit) =>
                unit.Type == UnitType.Mothership
                && unit.Is_Adjacent_To(_structure)
            ;
        
            public void Interact_With(UnitModel unit)
            {
                _game.RestartBattle();
            }
        }
        
        public class Debris
        {
            private readonly StructureModel _structure;
            private readonly InventoryContent? _loot;

            public Debris(StructureModel structure, InventoryContent? loot)
            {
                _structure = structure;
                _loot = loot;
            }

            public bool Can_Interact_With(UnitModel unit) => unit.Is_Adjacent_To(_structure);

            public void Interact_With(UnitModel unit)
            {
                unit.Take(_loot);
                _structure.Destroy();
            }
        }
    }
}