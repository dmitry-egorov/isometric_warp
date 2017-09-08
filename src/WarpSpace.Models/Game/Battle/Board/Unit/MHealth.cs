using Lanski.Reactive;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MHealth
    {
        public MHealth(MUnit the_owner, SignalGuard the_signal_guard)
        {
            s_owner = the_owner;

            var the_initial_state = HealthState.s_Initial_For(the_owner.s_Type);
            s_states_cell = new GuardedCell<HealthState>(the_initial_state, the_signal_guard);
        }

        public ICell<HealthState> s_States_Cell => it.s_states_cell;
        public bool is_Normal => it.s_state.is_Normal;

        public void Takes(Damage the_damage)
        {
            var the_new_state = it.s_state.After_Applying(the_damage);
            
            if (the_new_state.is_Lethal)
                it.s_owner.Destructs();
            
            it.s_state_becomes(the_new_state);
        }

        private void s_state_becomes(HealthState value) => this.it.s_states_cell.s_Value = value;

        private MHealth it => this;
        private HealthState s_state => this.it.s_states_cell.s_Value;
        
        private readonly MUnit s_owner;
        private readonly GuardedCell<HealthState> s_states_cell;

    }
}