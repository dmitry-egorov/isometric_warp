using Lanski.Reactive;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MHealth
    {
        public MHealth(int the_total_hit_points, MDestructor the_destructor, SignalGuard the_signal_guard)
        {
            its_destructor = the_destructor;
            its_cell_of_current_hitpoints = new GuardedCell<int>(the_total_hit_points, the_signal_guard);
        }

        public ICell<int> s_Current_Hitpoints_Cell => its_cell_of_current_hitpoints;

        internal void Takes(DDamage the_damage)
        {
            its_cell_of_current_hitpoints.s_Value -= the_damage.Amount;

            if (this.is_Dead())
            {
                its_destructor.Destructs();
            }
        }

        private readonly MDestructor its_destructor;
        private readonly GuardedCell<int> its_cell_of_current_hitpoints;
    }

    public static class MHealthReadExtensions
    {
        public static int s_Current_Hit_Points(this MHealth the_health) => the_health.s_Current_Hitpoints_Cell.s_Value;
        public static bool is_Dead(this MHealth the_health) => !the_health.is_Normal();
        public static bool is_Normal(this MHealth the_health) => the_health.s_Current_Hit_Points() > 0;
    }
}