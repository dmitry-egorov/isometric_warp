using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Weapon
{
    public class MWeapon
    {
        public MUnit s_Owner => the_owner;
        public IStream<MUnit> s_Fires_Stream => the_fires_stream;

        public MWeapon(MUnit the_owner)
        {
            this.the_owner = the_owner;
            
            the_damage = the_owner.s_Type.Get_Weapon_Type().Get_Damage_Description();
            the_fires_stream = new Stream<MUnit>();
        }

        public bool Can_Fire_At(MUnit the_unit) => 
            the_unit.is_Alive() 
            && the_unit.is_Within_the_Range_Of(this)
            && the_unit.is_Hostile_Towards(the_owner)
        ;

        public void Fires_At(MUnit the_target)
        {
            Can_Fire_At(the_target).Otherwise_Throw("Can't fire at the unit");
            
            the_target.Takes(the_damage);

            this.Sends_the_Fire_Event(the_target);
        }

        private void Sends_the_Fire_Event(MUnit the_target)
        {
            the_fires_stream.Next(the_target);
        }

        private readonly MUnit the_owner;
        private readonly Damage the_damage;
        private readonly Stream<MUnit> the_fires_stream;
    }
}