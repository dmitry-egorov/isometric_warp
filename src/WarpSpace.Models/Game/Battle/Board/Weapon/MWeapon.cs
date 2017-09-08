using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Weapon
{
    public class MWeapon
    {
        public MUnit s_Owner => it.s_owner;
        public IStream<MUnit> s_Fires_Stream => it.s_fires_stream;

        public MWeapon(MUnit the_owner, SignalGuard the_signal_guard)
        {
            s_owner = the_owner;
            
            var the_weapon_type = the_owner.s_Type.Get_Weapon_Type();
            s_max_uses = the_weapon_type.s_Max_Uses();
            s_uses_left = it.s_max_uses;
            
            s_damage = the_weapon_type.s_Damage_Description();
            s_fires_stream = new GuardedStream<MUnit>(the_signal_guard);
        }
        
        public bool can_Fire() => it.s_uses_left > 0;
        
        public bool can_Fire_At_a_Unit_At(MTile the_tile, out MUnit the_target_unit) => 
            the_tile.has_a_Unit(out the_target_unit) && 
            it.can_Fire_At(the_target_unit)
        ;

        public bool can_Fire_At(MUnit the_unit) =>
            it.can_Fire() &&
            the_unit.is_Alive &&  
            the_unit.is_Within_the_Range_Of(it) && 
            the_unit.is_Hostile_Towards(it.s_owner)
        ;

        internal void Fires_At(MUnit the_target) => fires_at(the_target);
        internal void Finishes_the_Turn() => it.restores_the_uses();

        private void fires_at(MUnit the_target)
        {
            it.can_Fire_At(the_target).Otherwise_Throw("Can't fire at the unit");

            the_target.Takes(it.s_damage);
            it.spends_a_use();

            it.signals_a_fire(the_target);
        }

        private void restores_the_uses() => it.s_uses_left = it.s_max_uses;
        private void spends_a_use() => it.s_uses_left--;
        private void signals_a_fire(MUnit the_target) => it.s_fires_stream.Next(the_target);

        private MWeapon it => this;

        private readonly MUnit s_owner;
        private readonly int s_max_uses;
        private readonly Damage s_damage;
        private readonly GuardedStream<MUnit> s_fires_stream;
        
        private int s_uses_left;
    }
}