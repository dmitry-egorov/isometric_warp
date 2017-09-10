using Lanski.Reactive;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MHealth
    {
        public MHealth(MUnit the_owner, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;

            var the_initial_state = HealthState.s_Initial_For(the_owner.s_Type);
            its_states_cell = new GuardedCell<HealthState>(the_initial_state, the_signal_guard);
        }

        public ICell<HealthState> s_States_Cell => its_states_cell;
        public bool is_Normal => its_state.is_Normal;

        public void Takes(Damage the_damage)
        {
            var the_new_state = its_state.After_Applying(the_damage);
            
            if (the_new_state.is_Lethal)
                its_owner.Destructs();
            
            its_state_becomes(the_new_state);
        }

        private void its_state_becomes(HealthState value) => its_states_cell.s_Value = value;

        private HealthState its_state => its_states_cell.s_Value;
        
        private readonly MUnit its_owner;
        private readonly GuardedCell<HealthState> its_states_cell;

    }
}