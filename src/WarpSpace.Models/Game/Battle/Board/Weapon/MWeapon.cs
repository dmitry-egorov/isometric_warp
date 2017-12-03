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

        public IStream<Firing> Fired => it_fired;

        public MWeapon(MUnit the_owner, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;
            
            its_type = the_owner.s_Weapon_Type();
            
            var its_max_uses = its_type.s_Max_Uses;
            its_uses_limiter = new MUsesLimiter(its_max_uses, the_signal_guard);
            
            its_damage = its_type.s_Damage;
            
            it_fired = new GuardedStream<Firing>(the_signal_guard);
        }

        public ICell<bool> s_Can_Fire_Cell => its_uses_limiter.s_Has_Uses_Left_Cell;
        
        public bool can_Fire_At_a_Unit_At(MTile the_tile, out MUnit the_target_unit) => 
            the_tile.has_a_Unit(out the_target_unit) && 
            this.it_can_fire_at(the_target_unit)
        ;

        public override string ToString() => $"{its_type} of the {its_owner}";

        internal void Fires_At(MUnit the_target) => it_fires_at(the_target);
        internal void Resets() => its_uses_limiter.Restores_the_Uses();

        private bool it_can_fire_at(MUnit the_unit) =>
            this.can_Fire &&
            the_unit.is_Alive() &&  
            the_unit.is_Within_the_Range_Of(this) && 
            the_unit.is_Hostile_Towards(its_owner)
        ;

        private void it_fires_at(MUnit the_target)
        {
            this.it_can_fire_at(the_target).Otherwise_Throw("Can't fire at the target");

            the_target.Takes(its_damage);
            its_uses_limiter.Spends_a_Use();
            
            it_fired.Happends_With(new Firing(this, the_target));
        }
        
        private bool can_Fire => its_uses_limiter.has_Uses_Left;

        private readonly MUnit its_owner;
        private readonly MWeaponType its_type;
        private readonly DDamage its_damage;
        private readonly MUsesLimiter its_uses_limiter;
        private readonly GuardedStream<Firing> it_fired;
    }
}