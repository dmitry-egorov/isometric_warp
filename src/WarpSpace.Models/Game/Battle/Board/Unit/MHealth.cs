using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MHealth
    {
        public ICell<HealthState> s_States_Cell() => this.the_states_cell;
        public IStream<TheVoid> s_Destruction_Signal() => this.the_destruction_signal;
        public bool is_Alive() => the_state().is_Alive();

        public MHealth(MUnit the_owner, EventsGuard the_events_guard)
        {
            this.the_owner = the_owner;

            var the_initial_health_state = HealthState.s_Initial_For(the_owner.s_Type);
            the_states_cell = new GuardedCell<HealthState>(the_initial_health_state, the_events_guard);
            the_destruction_signal = this.the_states_cell.First(x => x.is_Dead()).Select(_ => TheVoid.Instance);
        }

        public void Takes(Damage the_damage)
        {
            var the_new_state = the_state().After_Applying(the_damage);
            
            if (the_new_state.is_Dead())
                the_owner.Destructs();
            
            this.s_State_Becomes(the_new_state);
        }

        private void s_State_Becomes(HealthState value) => this.the_states_cell.s_Value = value;
        
        private HealthState the_state() => this.the_states_cell.s_Value;

        private readonly MUnit the_owner;
        private readonly GuardedCell<HealthState> the_states_cell;
        private readonly IStream<TheVoid> the_destruction_signal;

    }
}