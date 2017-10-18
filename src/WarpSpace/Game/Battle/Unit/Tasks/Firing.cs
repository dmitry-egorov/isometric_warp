using Lanski.Structures;

namespace WarpSpace.Game.Battle.Unit.Tasks
{
    public class Firing
    {
        public Index2D s_Position; 
        public override string ToString() => $"{nameof(Firing)} at {s_Position}"; 
    }
}