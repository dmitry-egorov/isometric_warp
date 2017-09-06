using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MHealth
    {
        public ICell<HealthState> s_Cell_of_Status() => this_health.s_cell_of_status;
        public IStream<TheVoid> s_Signal_of_the_Destruction() => this_health.s_signal_of_the_destruction;
        public bool is_Alive() => this_health.s_State().is_Alive();

        public MHealth(MUnit the_owner, EventsGuard the_events_guard)
        {
            s_owner = the_owner;

            var the_initial_health_state = HealthState.s_Initial_For(the_owner.s_Type);
            s_cell_of_status = new GuardedCell<HealthState>(the_initial_health_state, the_events_guard);
            s_signal_of_the_destruction = this_health.s_cell_of_status.First(x => x.is_Dead()).Select(_ => TheVoid.Instance);
        }

        public void Takes(Damage the_damage)
        {
            var the_new_state = this_health.s_State().After_Applying(the_damage);
            
            if (the_new_state.is_Dead())
                this_health.s_owner.Destructs();
            
            this_health.s_State_Becomes(the_new_state);
        }

        private HealthState s_State() => this_health.s_cell_of_status.s_Value;
        private void s_State_Becomes(HealthState value) => this_health.s_cell_of_status.s_Value = value;

        private MHealth this_health => this;

        private readonly MUnit s_owner;
        private readonly GuardedCell<HealthState> s_cell_of_status;
        private readonly IStream<TheVoid> s_signal_of_the_destruction;

    }
}