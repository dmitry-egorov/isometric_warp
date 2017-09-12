using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public struct UnitCommand
    {
        public static class Create
        {
            public static UnitCommand Fire(MWeapon weapon, MUnit target_unit) => new UnitCommand { its_variant = new Fire(weapon, target_unit)};
            public static UnitCommand Move(MUnit unit, MUnitLocation destination) => new UnitCommand { its_variant = new Move(unit, destination) };
            public static UnitCommand Interact(MUnit unit, MStructure target_structure) => new UnitCommand { its_variant = new Interact(unit, target_structure) };
        }
        
        public bool is_a_Fire_Command(out Fire the_fire)                      => its_variant.is_a_T1(out the_fire);
        public bool is_a_Move_Command(out Move the_move_command)              => its_variant.is_a_T2(out the_move_command);
        public bool is_an_Interact_Command(out Interact the_interact_command) => its_variant.is_a_T3(out the_interact_command);

        public bool is_a_Fire_Command()        => its_variant.is_a_T1();
        public bool is_a_Move_Command()        => its_variant.is_a_T2();
        public bool is_an_Interact_Command()   => its_variant.is_a_T3();
        
        public bool is_a_Tile_Move_Command() => is_a_Move_Command(out var the_move_command) && the_move_command.is_a_Tile_Move();
        public bool is_a_Dock_Command()      => is_a_Move_Command(out var the_move_command) && !the_move_command.is_a_Tile_Move();

        public bool is_a_Fire_Command(out MWeapon the_weapon, out MUnit the_target)
        {
            if (this.is_a_Fire_Command(out var fire))
            {
                the_weapon = fire.Weapon;
                the_target = fire.Target;

                return true;
            }

            the_weapon = null;
            the_target = null;
            return false;
        }

        public bool is_a_Move_Command(out MUnit the_unit, out MUnitLocation the_destination)
        {
            if (this.is_a_Move_Command(out var move))
            {
                the_unit = move.Unit;
                the_destination = move.Destination;
                return true;
            }

            the_unit = null;
            the_destination = null;
            return false;
        }
        
        public bool is_an_Interact_Command(out MUnit the_unit, out MStructure the_target)
        {
            if (this.is_an_Interact_Command(out var interact))
            {
                the_unit = interact.Unit;
                the_target = interact.Target;
                return true;
            }

            the_unit = null;
            the_target = null;
            return false;
        }
        
        public void Executes_With()
        {
            if (this.is_a_Fire_Command(out var the_weapon, out var the_target_unit))
            {
                the_weapon.Fires_At(the_target_unit);
            }
            else if (this.is_a_Move_Command(out var the_unit, out var the_destination))
            {
                the_unit.Moves_To(the_destination);
            }
            else if (this.is_an_Interact_Command(out the_unit, out var the_target_structure))
            {
                the_unit.Interacts_With(the_target_structure);
            }
        }

        private Or<Fire, Move, Interact> its_variant;

        public struct Fire
        {
            public readonly MWeapon Weapon;
            public readonly MUnit Target;

            public Fire(MWeapon weapon, MUnit the_target) { Weapon = weapon; Target = the_target; }
        }

        public struct Move
        {
            public readonly MUnit Unit;
            public readonly MUnitLocation Destination;

            public Move(MUnit unit, MUnitLocation destination) { Unit = unit; Destination = destination; }

            public bool is_a_Tile_Move() => Destination.is_a_Tile() && Unit.is_At_a_Tile();
        }

        public struct Interact
        {
            public readonly MUnit Unit;
            public readonly MStructure Target;

            public Interact(MUnit unit, MStructure target_structure) { Unit = unit; Target = target_structure; }
        }

    }
}