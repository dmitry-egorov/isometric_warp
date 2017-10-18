using Lanski.Structures;
using WarpSpace.Game.Tasks;

namespace WarpSpace.Game.Battle.Unit.Tasks
{
    public class MovementTo: ITaskVariant<WUnit>
    {
        public Index2D s_Target_Position => its_target_position;
        public MovementTo(Index2D the_target_position) => its_target_position = the_target_position;

        public bool Performs_a_Step_On(WUnit the_executor) => the_executor.Moves_To(its_target_position);
        public override string ToString() => $"{nameof(MovementTo)} to {its_target_position}";
            
        private readonly Index2D its_target_position;
    }
}