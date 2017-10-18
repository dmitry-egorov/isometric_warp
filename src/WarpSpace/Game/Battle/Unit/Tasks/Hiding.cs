using WarpSpace.Game.Tasks;

namespace WarpSpace.Game.Battle.Unit.Tasks
{
    public class Hiding: ITaskVariant<WUnit>
    {
        public override string ToString() => nameof(Hiding);
        public bool Performs_a_Step_On(WUnit the_executor)
        {
            the_executor.Hides();
            return true;
        }
    }
}