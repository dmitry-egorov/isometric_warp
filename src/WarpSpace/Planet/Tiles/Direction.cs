using System;
using Lanski.Structures;

namespace WarpSpace.Planet.Tiles
{
    public static class Direction2DExtensions
    {
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