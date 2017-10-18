﻿using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public struct UnitCommand
    {
        public static class Create
        {
            public static UnitCommand Fire(MWeapon weapon, MUnit target_unit) => new UnitCommand { its_variant = new Fire(weapon, target_unit)};
            public static UnitCommand Move(MUnit unit, MTile destination) => new UnitCommand { its_variant = new Move(unit, destination) };
            public static UnitCommand Dock(MUnit unit, MBaySlot destination) => new UnitCommand { its_variant = new Dock(unit, destination) };
            public static UnitCommand Interact(MUnit unit, MStructure target_structure) => new UnitCommand { its_variant = new Interact(unit, target_structure) };
        }
        
        public bool is_a_Fire_Command(out Fire the_fire)                      => its_variant.is_a_T1(out the_fire);
        public bool is_a_Move_Command(out Move the_move_command)              => its_variant.is_a_T2(out the_move_command);
        public bool is_a_Dock_Command(out Dock the_move_command)              => its_variant.is_a_T3(out the_move_command);
        public bool is_an_Interact_Command(out Interact the_interact_command) => its_variant.is_a_T4(out the_interact_command);

        public bool is_a_Fire_Command()        => its_variant.is_a_T1();
        public bool is_a_Move_Command()        => its_variant.is_a_T2();
        public bool is_a_Dock_Command()        => its_variant.is_a_T3();
        public bool is_an_Interact_Command()   => its_variant.is_a_T4();
        
        public bool is_a_Fire_Command(out MWeapon the_weapon, out MUnit the_target)
        {
            if (this.is_a_Fire_Command(out var fire))
            {
                the_weapon = fire.s_Weapon;
                the_target = fire.s_Target;

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
                the_unit = move.s_Unit;
                the_destination = move.s_Destination;
                return true;
            }

            the_unit = null;
            the_destination = default(MUnitLocation);
            return false;
        }
        
        public bool is_an_Interact_Command(out MUnit the_unit, out MStructure the_target)
        {
            if (this.is_an_Interact_Command(out var interact))
            {
                the_unit = interact.s_Unit;
                the_target = interact.s_Target;
                return true;
            }

            the_unit = null;
            the_target = null;
            return false;
        }
        
        public void Executes()
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

        private Or<Fire, Move, Dock, Interact> its_variant;

        public struct Fire
        {
            public readonly MWeapon s_Weapon;
            public readonly MUnit s_Target;

            public Fire(MWeapon weapon, MUnit the_target) { s_Weapon = weapon; s_Target = the_target; }
        }

        public struct Move
        {
            public readonly MUnit s_Unit;
            public readonly MTile s_Destination;

            public Move(MUnit unit, MTile destination) { s_Unit = unit; s_Destination = destination; }
        }

        public struct Dock
        {
            public readonly MUnit s_Unit;
            public readonly MBaySlot s_Destination;

            public Dock(MUnit unit, MBaySlot destination) { s_Unit = unit; s_Destination = destination; }
        }

        public struct Interact
        {
            public readonly MUnit s_Unit;
            public readonly MStructure s_Target;

            public Interact(MUnit unit, MStructure target_structure) { s_Unit = unit; s_Target = target_structure; }
        }

    }
}