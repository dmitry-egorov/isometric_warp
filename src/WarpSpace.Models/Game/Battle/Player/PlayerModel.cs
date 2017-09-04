using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Player
{
    public class PlayerModel
    {
        public ICell<Slot<UnitModel>> Selected_Unit_Cell { get; }
        public ICell<Slot<WeaponModel>> Selected_Weapon_Cell { get; }

        public PlayerModel()
        {
            _selection_cell = new NullableCell<PlayersSelection>(null);
            Selected_Unit_Cell = _selection_cell.Select(x => x.SelectRef(s => s.Unit));
            Selected_Weapon_Cell = _selection_cell.Select(x => x.SelectManyRef(s => s.WeaponSlot));

            Wire_Selected_Unit_Destruction();
            
            void Wire_Selected_Unit_Destruction()
            {
                Selected_Unit_Cell
                    .SkipEmpty()
                    .SelectMany(u => u.Signal_Of_the_Destruction)
                    .Subscribe(_ => Deselect());
                    
                void Deselect() => _selection_cell.Value = null;
            }
        }

        public void Execute_Command_At(TileModel tile)
        {
            if (!Possible_Command_At(tile).Has_a_Value(out var command)) 
                return;

            if (command.Is(out Command.Fire fire))
            {
                fire.Weapon.Fire_At(fire.Target_Unit);
            }
            else if (command.Is(out Command.SelectUnit select_unit))
            {
                Select_a_Unit(select_unit.Target_Unit);
            }
            else if (command.Is(out Command.Move move))
            {
                move.Unit.Move_To(move.Destination);
            }
            else if (command.Is(out Command.Interact interact))
            {
                interact.Unit.Interact_With(interact.Target_Structure);
            }
            else
            {
                throw new InvalidOperationException("Unknown command");
            }
        }

        public Command? Possible_Command_At(TileModel tile)
        {
            if (A_Weapon_Is_Selected(out var weapon))
            {
                if (tile.Has_a_Unit(out var target_unit) && weapon.Can_Fire_At(target_unit))
                    return Command.Create.Fire(weapon, target_unit);
            }
            else
            {
                if (tile.Has_a_Unit(out var target_unit) && Can_Select(target_unit))
                    return Command.Create.Select_Unit(target_unit);

                if (A_Unit_Is_Selected(out var selected_unit))
                {
                    if (selected_unit.Can_Move_To(tile))
                        return Command.Create.Move(selected_unit, tile);
                    
                    if (tile.Has_a_Structure(out var structure) && selected_unit.Can_Interact_With(structure))
                        return Command.Create.Interact(selected_unit, structure);
                }
            }

            return null;
        }

        public void Select_a_Unit(UnitModel unit) => _selection_cell.Value = new PlayersSelection(unit, Slot.Empty<WeaponModel>());

        public void Toggle_Weapon_Selection()
        {
            if (There_Is_No_Selection())
                return;

            if (A_Weapon_Is_Not_Selected())
            {
                Select_Current_Units_Weapon();
            }
            else
            {
                Reset_Weapon_Selection();
            }
        }
        
        private ICell<PlayersSelection?> Selection_Cell => _selection_cell;//Value can be null

        private void Select_Current_Units_Weapon()
        {
            if (!A_Unit_Is_Selected(out var selected_unit))
                return;
            
            var selected_units_weapon = selected_unit.Weapon;
            _selection_cell.Value = new PlayersSelection(selected_unit, selected_units_weapon.As_a_Slot());
        }

        private void Reset_Weapon_Selection()
        {
            if (!A_Unit_Is_Selected(out var selected_unit))
                return;
            
            _selection_cell.Value = new PlayersSelection(selected_unit, Slot.Empty<WeaponModel>());
        }

        private bool A_Unit_Is_Selected(out UnitModel selected_unit) => Selected_Unit_Cell.Has_a_Value(out selected_unit);
        private bool A_Weapon_Is_Selected(out WeaponModel selected_weapon) => Selected_Weapon_Cell.Has_a_Value(out selected_weapon); 
        private bool A_Weapon_Is_Not_Selected() => Selected_Weapon_Cell.Does_Not_Have_a_Value(); 
        private bool There_Is_No_Selection() => Selection_Cell.Does_Not_Have_a_Value();
        private bool Can_Select(UnitModel unit) => unit.Faction == Faction.Players;
        
        private struct PlayersSelection
        {
            public readonly UnitModel Unit;
            public readonly Slot<WeaponModel> WeaponSlot;

            public PlayersSelection(UnitModel unit, Slot<WeaponModel> weaponSlot)
            {
                Unit = unit;
                WeaponSlot = weaponSlot;
            }
        }

        private readonly NullableCell<PlayersSelection> _selection_cell;
    }
}