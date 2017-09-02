using Lanski.Structures;

namespace WarpSpace.Common
{
    public static class Direction2DEx
    {
        public static Direction2D Random()
        {
            return (Direction2D) UnityEngine.Random.Range(0, 4);
        }
    }
}