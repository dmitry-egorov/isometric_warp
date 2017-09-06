using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Weapon
{
    public class MWeapon
    {
        public MUnit s_Owner => s_owner;

        public MWeapon(MUnit the_owner)
        {
            this_weapon = this;
            s_owner = the_owner;
            s_damage = the_owner.s_Type.Get_Weapon_Type().Get_Damage_Description();
        }

        public bool Can_Fire_At(MUnit the_unit) => 
            the_unit.Is_Alive 
            && the_unit.is_Within_Range_Of(this_weapon)
            && the_unit.is_Hostile_Towards(this_weapon.s_Owner)
        ;

        public void Fires_At(MUnit the_unit)
        {
            Can_Fire_At(the_unit).Otherwise_Throw("Can't fire at the unit");
            the_unit.Takes(this_weapon.s_damage);
        }

        private Target The(MUnit unit) => new Target(unit, this_weapon.s_owner);

        private readonly MWeapon this_weapon;

        private readonly MUnit s_owner;
        private readonly Damage s_damage;

        
        private struct Target
        {
            private readonly MUnit _target_unit;
            private readonly MUnit _mounting_unit;

            public Target(MUnit target_unit, MUnit mounting_unit)
            {
                _target_unit = target_unit;
                _mounting_unit = mounting_unit;
            }

            
        }
    }
}