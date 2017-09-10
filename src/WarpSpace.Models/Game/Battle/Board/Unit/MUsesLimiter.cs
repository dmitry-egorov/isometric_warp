namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MUsesLimiter
    {
        public MUsesLimiter(int the_max_uses)
        {
            its_max_uses = the_max_uses;
            its_uses_left = the_max_uses;
        }

        public bool has_Uses_Left() => its_uses_left > 0;
        public void Spends_a_Use() => its_uses_left--;
        public void Restores_the_Uses() => its_uses_left = its_max_uses;
        
        private readonly int its_max_uses;
        private int its_uses_left;
    }
}