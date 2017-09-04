using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Player
{
    public struct Command
    {
        public static class Create
        {
            public static Command Fire(WeaponModel weapon, UnitModel target_unit) => new Command { _variant = new Fire(weapon, target_unit)};
            public static Command Select_Unit(UnitModel target_unit) => new Command { _variant = new SelectUnit(target_unit) };
            public static Command Move(UnitModel unit, TileModel destination) => new Command { _variant = new Move(unit, destination) };
            public static Command Interact(UnitModel unit, StructureModel target_structure) => new Command { _variant = new Interact(unit, target_structure) };
        }

        public bool Is(out Fire fire) => _variant.Is_a_T1(out fire);
        public bool Is(out SelectUnit select_unit) => _variant.Is_a_T2(out select_unit);
        public bool Is(out Move move) => _variant.Is_a_T3(out move);
        public bool Is(out Interact interact) => _variant.Is_a_T4(out interact);

        public bool Is_Fire() => _variant.Is_a_T1();
        public bool Is_Select_Unit() => _variant.Is_a_T2();
        public bool Is_Move() => _variant.Is_a_T3();
        public bool Is_Interact() => _variant.Is_a_T4();

        public struct Fire
        {
            public readonly WeaponModel Weapon;
            public readonly UnitModel Target_Unit;

            public Fire(WeaponModel weapon, UnitModel target_unit)
            {
                Weapon = weapon;
                Target_Unit = target_unit;
            }
        }

        public struct SelectUnit
        {
            public readonly UnitModel Target_Unit;

            public SelectUnit(UnitModel target_unit) { Target_Unit = target_unit; }
        }

        public struct Move
        {
            public readonly UnitModel Unit;
            public readonly TileModel Destination;

            public Move(UnitModel unit, TileModel destination) { Unit = unit; Destination = destination; }
        }

        public struct Interact
        {
            public readonly UnitModel Unit;
            public readonly StructureModel Target_Structure;

            public Interact(UnitModel unit, StructureModel target_structure) { Unit = unit; Target_Structure = target_structure; }
        }

        private Or<Fire, SelectUnit, Move, Interact> _variant;
    }
}