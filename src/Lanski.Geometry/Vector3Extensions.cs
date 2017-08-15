using UnityEngine;

namespace Lanski.Geometry
{
    public static class Vector3Extensions
    {
        public static Vector3 MirrorX(this Vector3 v)
        {
            return new Vector3(-v.x, v.y, v.z);
        }
        
        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        
        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}