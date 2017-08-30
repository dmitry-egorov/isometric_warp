using Lanski.Reactive;
using WarpSpace.Descriptions;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit.Weapon
{
    public class WeaponModel
    {
        private readonly UnitModel _mountingUnit;
        private readonly DamageDescription _damage;

        public WeaponModel(WeaponType type, UnitModel mountingUnit)
        {
            _mountingUnit = mountingUnit;
            _damage = type.GetDamageDescription();
        }

        public bool try_to_fire_at_the(UnitModel unit) => 
               Can_Fire_At_the(unit) 
            && unit.take_the(_damage)
        ;

        public bool Can_Fire_At_the(UnitModel unit) =>
               the(unit).aka(out var target)
            && target.is_within_range()
            && target.s_faction_is_hostile()
        ;

        private TheUnit the(UnitModel unit) => new TheUnit(unit, _mountingUnit);
        
        private struct TheUnit
        {
            private readonly UnitModel _target_unit;
            private readonly UnitModel _mounting_unit;

            public TheUnit(UnitModel targetUnit, UnitModel mountingUnit)
            {
                _target_unit = targetUnit;
                _mounting_unit = mountingUnit;
            }

            public bool s_faction_is_hostile() => _target_unit.is_owned_by_the_player != _mounting_unit.is_owned_by_the_player;

            public bool is_within_range() => 
                   _target_unit.CurrentTileCell.aka(out var targets_tile_cell)
                && _mounting_unit.CurrentTileCell.aka(out var own_tile_cell)
                && targets_tile_cell.s_value_is_the(out var targets_tile)
                && own_tile_cell.s_value_is_the(out var own_tile)
                && own_tile.is_adjacent_to_the(targets_tile)
            ;
        }
    }
}