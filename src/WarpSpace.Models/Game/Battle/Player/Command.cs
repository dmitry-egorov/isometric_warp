using Lanski.Reactive;
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
            public static Command Fire(MWeapon weapon, MUnit target_unit) => new Command { _variant = new FireCommand(weapon, target_unit)};
            public static Command Select_Unit(MPlayer the_player, MUnit the_target_unit) => new Command { _variant = new SelectUnit(the_player, the_target_unit) };
            public static Command Move(MUnit unit, MTile destination) => new Command { _variant = new Move(unit, destination) };
            public static Command Interact(MUnit unit, MStructure target_structure) => new Command { _variant = new Interact(unit, target_structure) };
        }
        
        public void Executes_With(EventsGuard the_events_guard)
        {
            using (the_events_guard.Holds_All_Events())
            {
                if (this.is_a_Fire_Command(out var the_weapon, out var the_target_unit))
                {
                    the_weapon.Fires_At(the_target_unit);
                }
                else if (this.is_a_Select_Unit_Command(out var the_player, out the_target_unit))
                {
                    the_player.Selects_a_Unit(the_target_unit);
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
        }

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

        public bool is_a_Select_Unit_Command(out MPlayer the_player, out MUnit the_target)
        {
            if (this.is_a_Select_Unit_Command(out var select_unit))
            {
                the_player = select_unit.Player;
                the_target = select_unit.Target;
                return true;
            }

            the_player = null;
            the_target = null;
            return false;
        }

        public bool is_a_Move_Command(out MUnit the_unit, out MTile the_destination)
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

        public bool is_a_Fire_Command(out FireCommand the_fire_command) => _variant.Is_a_T1(out the_fire_command);
        public bool is_a_Select_Unit_Command(out SelectUnit the_select_unit_command) => _variant.Is_a_T2(out the_select_unit_command);
        public bool is_a_Move_Command(out Move the_move_command) => _variant.Is_a_T3(out the_move_command);
        public bool is_an_Interact_Command(out Interact the_interact_command) => _variant.Is_a_T4(out the_interact_command);

        public bool is_a_Fire_Command() => _variant.Is_a_T1();
        public bool is_a_Select_Unit_Command() => _variant.Is_a_T2();
        public bool is_a_Move_Command() => _variant.Is_a_T3();
        public bool is_an_Interact_Command() => _variant.Is_a_T4();

        public struct FireCommand
        {
            public readonly MWeapon Weapon;
            public readonly MUnit Target;

            public FireCommand(MWeapon weapon, MUnit the_target)
            {
                Weapon = weapon;
                Target = the_target;
            }
        }

        public struct SelectUnit
        {
            public readonly MPlayer Player;
            public readonly MUnit Target;

            public SelectUnit(MPlayer the_player, MUnit the_target) { Player = the_player; Target = the_target; }
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
            public readonly MStructure Target;

            public Interact(MUnit unit, MStructure target_structure) { Unit = unit; Target = target_structure; }
        }

        private Or<FireCommand, SelectUnit, Move, Interact> _variant;
    }
}