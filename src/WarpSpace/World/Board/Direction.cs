using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board
{
    public static class Direction2DExtensions
    {
        public static Quaternion ToRotation(this Direction2D direction)
        {
            return Quaternion.Euler(0, direction.ToAngle(), 0);
        }
        
        public static float ToAngle(this Direction2D direction)
        {
            return (float) direction * 90;
        }
    }

    public static class Direction2DEx
    {
        public static Direction2D Random()
        {
            return (Direction2D) UnityEngine.Random.Range(0, 4);
        }
    }
}