using UnityEngine;

namespace Lanski.Geometry
{
    public static class Vector2Extension
    {
        public static Vector2 Floor(this Vector2 v)
        {
            return new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
        }
        
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            return Quaternion.Euler(0, 0, degrees) * v;
        }
    }
}