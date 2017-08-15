using UnityEngine;

namespace Lanski.Geometry
{
    public static class QuaternionEx
    {
        public static Quaternion Euler(float x, float y, float z)
        {
            return Quaternion.Euler(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);
        }
    }
}