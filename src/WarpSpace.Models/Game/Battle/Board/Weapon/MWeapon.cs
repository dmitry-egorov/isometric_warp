using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Flow;

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

        public bool can_Fire_At(MTile the_tile) =>
            this.can_Fire &&
            let(out var the_owners_tile, this.s_Owner.s_Location()) &&
            the_tile.is_not(the_owners_tile) &&
            the_tile.is_Adjacent_To(the_owners_tile)
        ;

        public override string ToString() => $"{its_type} of the {its_owner}";

        internal void Fires_At(MTile the_target) => it_fires_at(the_target);
        internal void Resets() => its_uses_limiter.Restores_the_Uses();

        private void it_fires_at(MTile the_target)
        {
            this.can_Fire_At(the_target).Otherwise_Throw("Can't fire at the target");

            if (the_target.has_a_Unit(out var the_unit_at_the_target))
            {
                the_unit_at_the_target.Takes(its_damage);                
            }
            
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