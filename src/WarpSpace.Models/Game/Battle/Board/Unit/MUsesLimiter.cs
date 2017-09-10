using Lanski.Reactive;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUsesLimiter
    {
        public MUsesLimiter(int the_max_uses, SignalGuard the_signal_guard)
        {
            its_max_uses = the_max_uses;
            its_uses_left_cell = new GuardedCell<int>(the_max_uses, the_signal_guard);

            its_has_uses_left_cell = its_uses_left_cell.Select(x => x > 0);
        }

        public ICell<bool> s_Has_Uses_Left_Cell => its_has_uses_left_cell;
        public bool has_Uses_Left => its_has_uses_left_cell.s_Value;

        public void Spends_a_Use() => its_uses_left--;
        public void Restores_the_Uses() => its_uses_left = its_max_uses;

        private int its_uses_left
        {
            get => its_uses_left_cell.s_Value;
            set => its_uses_left_cell.s_Value = value;
        }

        private readonly int its_max_uses;
        private readonly GuardedCell<int> its_uses_left_cell;
        
        private readonly ICell<bool> its_has_uses_left_cell;
    }
}