using Lanski.Structures;
using WarpSpace.Game.Tasks;

namespace WarpSpace.Game.Battle.Unit.Tasks
{
    public class RotationTo: ITaskVariant<WUnit>
    {
        public RotationTo(Direction2D the_target_orientation) => its_target_orientation = the_target_orientation;
        public Direction2D s_Target_Orientation => its_target_orientation;

        public bool Performs_a_Step_On(WUnit the_executor) => the_executor.Rotates_To(its_target_orientation);
        public override string ToString() => $"{nameof(RotationTo)} to {its_target_orientation}";
            
        private readonly Direction2D its_target_orientation;
    }
}