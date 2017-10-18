using Lanski.Structures;
using WarpSpace.Game.Tasks;

namespace WarpSpace.Game.Battle.Unit.Tasks
{
    public class ShowingUpAt: ITaskVariant<WUnit>
    {
        public ShowingUpAt(Index2D the_target_position, Direction2D the_target_orientation) {its_target_orientation = the_target_orientation; its_target_position = the_target_position; }

        public Index2D s_Target_Position => its_target_position;
        public Direction2D s_Target_Orientation => its_target_orientation;

        public bool Performs_a_Step_On(WUnit the_executor)
        {
            the_executor.Shows_Up_At(its_target_position, its_target_orientation);
            return true;
        }

        public override string ToString() => $"{nameof(ShowingUpAt)} at {its_target_position}, {its_target_orientation}";

        private readonly Index2D its_target_position;
        private readonly Direction2D its_target_orientation;
    }
}