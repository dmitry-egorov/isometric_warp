using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Weapon
{
    public class MWeapon
    {
        public static MWeapon From(UnitType type, MUnit mounting_unit) => new MWeapon(type.Get_Weapon_Type().Get_Damage_Description(), mounting_unit);

        public MWeapon(DamageDescription desc, MUnit mountingUnit)
        {
            _mountingUnit = mountingUnit;
            _damage = desc;
        }

        public void Fire_At(MUnit unit)
        {
            Can_Fire_At(unit).Otherwise_Throw("Can't fire at the unit");

            unit.Take(_damage);
        }

        public bool Can_Fire_At(MUnit unit)
        {
            var target = The(unit);
            
            return unit.Is_Alive 
                && target.Is_Within_Range()
                && target.s_Faction_Is_Hostile()
            ;
        }

        private Target The(MUnit unit) => new Target(unit, _mountingUnit);


        private readonly MUnit _mountingUnit;
        private readonly DamageDescription _damage;


        private struct Target
        {
            private readonly MUnit _target_unit;
            private readonly MUnit _mounting_unit;

            public Target(MUnit targetUnit, MUnit mountingUnit)
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
    }
}