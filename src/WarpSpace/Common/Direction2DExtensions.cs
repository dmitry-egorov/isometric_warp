using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.Common
{
    public static class Direction2DExtensions
    {
        private static readonly Quaternion Rotation_Left = Quaternion.Euler(0,   0, 0);
        private static readonly Quaternion Rotation_Up = Quaternion.Euler(0,  90, 0);
        private static readonly Quaternion Rotation_Right = Quaternion.Euler(0, 180, 0);

        public static Direction2D s_Direction(this Quaternion rotation)
        {
            return Quaternion.Angle(rotation, Rotation_Left)  < 45 ? Direction2D.Left 
                 : Quaternion.Angle(rotation, Rotation_Up)    < 45 ? Direction2D.Up 
                 : Quaternion.Angle(rotation, Rotation_Right) < 45 ? Direction2D.Right 
                                                                   : Direction2D.Down;
        }
        
        public static Quaternion s_Rotation(this Direction2D direction)
        {
            return Quaternion.Euler(0, direction.ToAngle(), 0);
        }
        
        public static float ToAngle(this Direction2D direction)
        {
            return (float) direction * 90;
        }
    }
}