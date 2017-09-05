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
            public static Command Fire(MWeapon weapon, MUnit target_unit) => new Command { _variant = new Fire(weapon, target_unit)};
            public static Command Select_Unit(MUnit target_unit) => new Command { _variant = new SelectUnit(target_unit) };
            public static Command Move(MUnit unit, MTile destination) => new Command { _variant = new Move(unit, destination) };
            public static Command Interact(MUnit unit, MStructure target_structure) => new Command { _variant = new Interact(unit, target_structure) };
        }

        public bool Is_a_Fire(out Fire fire) => _variant.Is_a_T1(out fire);
        public bool Is_a_Select_Unit(out SelectUnit select_unit) => _variant.Is_a_T2(out select_unit);
        public bool Is_a_Move(out Move move) => _variant.Is_a_T3(out move);
        public bool Is_an_Interact(out Interact interact) => _variant.Is_a_T4(out interact);

        public bool Is_a_Fire() => _variant.Is_a_T1();
        public bool Is_a_Select_Unit() => _variant.Is_a_T2();
        public bool Is_a_Move() => _variant.Is_a_T3();
        public bool Is_a_Interact() => _variant.Is_a_T4();

        public struct Fire
        {
            public readonly MWeapon Weapon;
            public readonly MUnit Target_Unit;

            public Fire(MWeapon weapon, MUnit target_unit)
            {
                Weapon = weapon;
                Target_Unit = target_unit;
            }
        }

        public struct SelectUnit
        {
            public readonly MUnit Target_Unit;

            public SelectUnit(MUnit target_unit) { Target_Unit = target_unit; }
        }

        public struct Move
        {
            public readonly MUnit Unit;
            public readonly MTile Destination;

            public Move(MUnit unit, MTile destination) { Unit = unit; Destination = destination; }
        }

        public struct Interact
        {
            public readonly MUnit Unit;
            public readonly MStructure Target_Structure;

            public Interact(MUnit unit, MStructure target_structure) { Unit = unit; Target_Structure = target_structure; }
        }

        private Or<Fire, SelectUnit, Move, Interact> _variant;
    }
}