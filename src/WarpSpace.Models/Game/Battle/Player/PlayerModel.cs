using JetBrains.Annotations;
using Lanski.Reactive;
using Lanski.Structures;
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

            wire_Selected_unit_Destruction();
            
            void wire_Selected_unit_Destruction()
            {
                SelectedUnit
                    .SkipEmpty()
                    .SelectMany(u => u.Destroyed)
                    .Subscribe(_ => deselect());
                    
                void deselect() => _selectionCell.Value = null;
            }
        }

        public bool ExecuteActionAt(TileModel tile)
        {
            return try_to_Use_the_Selected_Weapon_at_the(tile)
                || try_to_Deselect_the_Weapon()
                || try_to_Select_a_Unit_at_the(tile)
                || try_to_Move_the_Selected_Unit_To_the(tile)
                || try_to_make_the_Selected_Unit_Interact_with_the_Structure_at_the(tile)
            ;
            
            bool try_to_Use_the_Selected_Weapon_at_the(TileModel target_tile) => 
                   a_Weapon_is_Selected(out var selected_weapon)
                && target_tile.Has_a_Unit(out var target_unit)
                && selected_weapon.try_to_fire_at_the(target_unit)
                && try_to_Deselect_the_Weapon()
            ;

            bool try_to_Select_a_Unit_at_the(TileModel target_tile) =>
                   a_Weapon_is_Not_Selected()
                && target_tile.Has_a_Unit(out var target_unit) 
                && try_to_Select_the_Unit(target_unit)
            ;

            bool try_to_Move_the_Selected_Unit_To_the(TileModel target_tile) => 
                   a_Unit_is_Selected(out var selected_unit)
                && a_Weapon_is_Not_Selected()
                && selected_unit.try_to_Move_To_the(target_tile)
            ;

            bool try_to_make_the_Selected_Unit_Interact_with_the_Structure_at_the(TileModel target_tile) => 
                   a_Unit_is_Selected(out var selected_unit)
                && a_Weapon_is_Not_Selected()
                && target_tile.Has_a_Structure(out var target_structure)
                && selected_unit.try_to_Interact_with_the(target_structure)
            ;

            bool try_to_Deselect_the_Weapon() =>
                   a_Weapon_is_Selected()
                && try_to_Select_the_Weapon(null)
            ;
        }

        public bool SelectWeapon() => 
               a_Unit_is_Selected(out var selected_unit)
            && selected_unit.Weapon.aka(out var selected_units_weapon)
            && try_to_Select_the_Weapon(selected_units_weapon);

        private bool try_to_Select_the_Unit(UnitModel unit) => 
               unit.can_be_Selected() 
            && (_selectionCell.Value = new PlayersSelection(unit, null)).inline();

        private bool try_to_Select_the_Weapon(Slot<WeaponModel> weapon) => 
               theres_a_Selection(out var selection) 
            && (_selectionCell.Value = selection.with(weapon)).inline();

        private bool a_Unit_is_Selected(out UnitModel selected_unit) =>
               default(UnitModel).is_the(out selected_unit)
            && SelectionCell.has_a_value(out var selection) 
            && selection.Unit.is_the(out selected_unit);

        private bool a_Weapon_is_Selected() => 
               theres_a_Selection(out var selection) 
            && selection.has_a_Weapon();
        
        private bool a_Weapon_is_Selected(out WeaponModel selected_weapon) => 
               default(WeaponModel).is_the(out selected_weapon) 
            && theres_a_Selection(out var selection) 
            && selection.has_a_Weapon(out selected_weapon); 

        private bool a_Weapon_is_Not_Selected() => SelectionCell.doesnt_have_a(out var selection) || selection.doesnt_have_a_weapon(); 
        private bool theres_a_Selection(out PlayersSelection selection) => SelectionCell.has_a_value(out selection);

        public struct PlayersSelection
        {
            public readonly UnitModel Unit;
            public readonly Slot<WeaponModel> WeaponSlot;

            public PlayersSelection(UnitModel unit, Slot<WeaponModel> weaponSlot)
            {
                Unit = unit;
                WeaponSlot = weaponSlot;
            }

            [Pure]public PlayersSelection with(Slot<WeaponModel> weapon) => new PlayersSelection(Unit, weapon);

            public bool has_a_Weapon() => WeaponSlot.has_something();
            public bool has_a_Weapon(out WeaponModel weapon) => WeaponSlot.Has_a_Value(out weapon);
            public bool doesnt_have_a_weapon() => WeaponSlot.Has_Nothing();
        }
    }

    internal static class UnitExtensionForPlayer
    {
        public static bool can_be_Selected(this UnitModel unit) => unit.is_owned_by_the_player;
    }
}