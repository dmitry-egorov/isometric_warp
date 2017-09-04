using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Weapon
{
    public class WeaponModel
    {
        public WeaponModel(WeaponType type, UnitModel mountingUnit)
        {
            _mountingUnit = mountingUnit;
            _damage = type.Get_Damage_Description();
        }

        public void Fire_At(UnitModel unit)
        {
            Can_Fire_At(unit).Otherwise_Throw("Can't fire at the unit");

            unit.Take(_damage);
        }

        public bool Can_Fire_At(UnitModel unit)
        {
            var target = The(unit);
            
            return unit.Is_Alive 
                && target.Is_Within_Range()
                && target.s_Faction_Is_Hostile()
            ;
        }

        private Target The(UnitModel unit) => new Target(unit, _mountingUnit);
        
        private struct Target
        {
            private readonly UnitModel _target_unit;
            private readonly UnitModel _mounting_unit;

            public Target(UnitModel targetUnit, UnitModel mountingUnit)
            {
                _target_unit = targetUnit;
                _mounting_unit = mountingUnit;
            }

            public bool s_Faction_Is_Hostile() => _target_unit.Faction.Is_Hostile_Towards(_mounting_unit.Faction);

            public bool Is_Within_Range()
            {
                var targets_tile = _target_unit.Location;
                var source_tile = _mounting_unit.Location;
                
                return source_tile.Is_Adjacent_To(targets_tile);
            }
        }
        
        private readonly UnitModel _mountingUnit;
        private readonly DamageDescription _damage;
    }
}