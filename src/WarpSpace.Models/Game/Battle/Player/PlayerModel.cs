using JetBrains.Annotations;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.Models.Game.Battle.Player
{
    public class PlayerModel
    {
        private readonly NullableCell<PlayersSelection> _selectionCell;
        public ICell<PlayersSelection?> SelectionCell => _selectionCell;//Value can be null
        public ICell<Slot<UnitModel>> SelectedUnit { get; }

        public PlayerModel()
        {
            _selectionCell = new NullableCell<PlayersSelection>(null);
            SelectedUnit = _selectionCell.Select(x => x.SelectRef(s => s.Unit));

            Wire_Selected_Unit_Destruction();
            
            void Wire_Selected_Unit_Destruction()
            {
                SelectedUnit
                    .SkipEmpty()
                    .SelectMany(u => u.Stream_Of_Destroyed_Events)
                    .Subscribe(_ => deselect());
                    
                void deselect() => _selectionCell.Value = null;
            }
        }

        public void Execute_Command_At(TileModel tile)
        {
            if (Try_Get_Command_At(tile).Has_a_Value(out var command))
                command.Execute();
        }

        public PlayerCommand? Try_Get_Command_At(TileModel tile)
        {
            if (A_Weapon_Is_Selected(out var weapon))
            {
                if (tile.Has_a_Unit(out var target_unit) && weapon.Can_Fire_At(target_unit))
                    return PlayerCommand.Fire(weapon, target_unit);
            }
            else
            {
                if (tile.Has_a_Unit(out var target_unit) && Can_Select(target_unit))
                    return PlayerCommand.Select_Unit(this, target_unit);

                if (A_Unit_Is_Selected(out var selected_unit))
                {
                    if (selected_unit.Can_Move_To(tile))
                        return PlayerCommand.Move(selected_unit, tile);
                    
                    if (tile.Has_a_Structure(out var structure) && selected_unit.Can_Interact_With(structure))
                        return PlayerCommand.Interact(selected_unit, structure);
                }
            }

            return null;
        }

        public void Select_a_Unit(UnitModel unit) => 
            _selectionCell.Value = new PlayersSelection(unit, null)
        ;

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

        private void Select_Current_Units_Weapon()
        {
            if (There_Is_No_Selection(out var selection))
                return;
            
            var selected_units_weapon = selection.Unit.Weapon;
            _selectionCell.Value = selection.With(selected_units_weapon);
        }

        private void Reset_Weapon_Selection()
        {
            if (There_Is_No_Selection(out var selection))
                return;
            
            _selectionCell.Value = selection.With(default(Slot<WeaponModel>));
        }

        private bool A_Unit_Is_Selected(out UnitModel selected_unit) => 
            (selected_unit = There_Is_a_Selection(out var selection) ? selection.Unit : null) != null
        ;

        private bool A_Weapon_Is_Selected(out WeaponModel selected_weapon) => 
             (selected_weapon = There_Is_a_Selection(out var selection) && selection.Has_a_Weapon(out selected_weapon) ? selected_weapon : null) != null
        ; 

        private bool A_Weapon_Is_Not_Selected() => There_Is_No_Selection(out var selection) || selection.Doesnt_Have_a_Weapon(); 
        private bool There_Is_No_Selection() => SelectionCell.Does_Not_Have_a_Value();
        private bool There_Is_No_Selection(out PlayersSelection selection) => !There_Is_a_Selection(out selection);
        private bool There_Is_a_Selection(out PlayersSelection selection) => SelectionCell.Has_a_Value(out selection);
        private bool Can_Select(UnitModel unit) => unit.Faction == Faction.Players;

        public struct PlayersSelection
        {
            public readonly UnitModel Unit;
            public readonly Slot<WeaponModel> WeaponSlot;

            public PlayersSelection(UnitModel unit, Slot<WeaponModel> weaponSlot)
            {
                Unit = unit;
                WeaponSlot = weaponSlot;
            }

            [Pure]public PlayersSelection With(Slot<WeaponModel> weapon) => new PlayersSelection(Unit, weapon);

            public bool Has_a_Weapon(out WeaponModel weapon) => WeaponSlot.Has_a_Value(out weapon);
            public bool Doesnt_Have_a_Weapon() => WeaponSlot.Has_Nothing();
        }
    }
}