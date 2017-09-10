using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Weapon
{
    public class MWeapon
    {
        public MUnit s_Owner => its_owner;
        public IStream<MUnit> s_Fires_Stream => its_fires_stream;

        public MWeapon(MUnit the_owner, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;
            
            var the_weapon_type = the_owner.s_Type.s_Weapon_Type();
            
            var its_max_uses = the_weapon_type.s_Max_Uses();
            its_uses_limiter = new MUsesLimiter(its_max_uses, the_signal_guard);
            
            its_damage = the_weapon_type.s_Damage_Description();
            its_fires_stream = new GuardedStream<MUnit>(the_signal_guard);
        }
        
        public bool can_Fire() => its_uses_limiter.has_Uses_Left;
        public ICell<bool> s_Can_Fire_Cell => its_uses_limiter.s_Has_Uses_Left_Cell;
        
        public bool can_Fire_At_a_Unit_At(MTile the_tile, out MUnit the_target_unit) => 
            the_tile.has_a_Unit(out the_target_unit) && 
            this.can_Fire_At(the_target_unit)
        ;

        public bool can_Fire_At(MUnit the_unit) =>
            this.can_Fire() &&
            the_unit.is_Alive &&  
            the_unit.is_Within_the_Range_Of(this) && 
            the_unit.is_Hostile_Towards(its_owner)
        ;

        internal void Fires_At(MUnit the_target) => it_fires_at(the_target);
        internal void Finishes_the_Turn() => its_uses_limiter.Restores_the_Uses();

        private void it_fires_at(MUnit the_target)
        {
            this.can_Fire_At(the_target).Otherwise_Throw("Can't fire at the unit");

            the_target.Takes(its_damage);
            its_uses_limiter.Spends_a_Use();

            it_signals_a_fire(the_target);
        }

        private void it_signals_a_fire(MUnit the_target) => its_fires_stream.Next(the_target);

        private readonly MUnit its_owner;
        private readonly Damage its_damage;
        private readonly MUsesLimiter its_uses_limiter;
        private readonly GuardedStream<MUnit> its_fires_stream;
    }
}