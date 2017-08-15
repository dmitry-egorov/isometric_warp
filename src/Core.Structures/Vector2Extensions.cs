using UnityEngine;

namespace Core.Structures
{
    public static class Vector2Extensions
    {
        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }
    }
}