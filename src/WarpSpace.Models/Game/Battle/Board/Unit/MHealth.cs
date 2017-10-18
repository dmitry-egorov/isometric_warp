using Lanski.Reactive;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MHealth
    {
        public MHealth(MUnit the_owner, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;

            its_current_hitpoints_cell = new GuardedCell<int>(the_owner.s_Total_Hit_Points, the_signal_guard);
        }
        
        public bool is_Dead => !this.is_Normal;
        public bool is_Normal => its_current_hit_points > 0;

        public ICell<int> s_Current_Hitpoints_Cell => its_current_hitpoints_cell;

        internal void Takes(DDamage the_damage)
        {
            its_current_hit_points -= the_damage.Amount;

            if (this.is_Dead)
            {
                its_owner.Destructs();
            }
        }

        private int its_current_hit_points
        {
            get => its_current_hitpoints_cell.s_Value;
            set => its_current_hitpoints_cell.s_Value = value;
        }

        private readonly MUnit its_owner;
        private readonly GuardedCell<int> its_current_hitpoints_cell;
    }
}