using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.Common
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
}