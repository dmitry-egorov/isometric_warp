using UnityEngine;

namespace Lanski.Geometry
{
    public static class MatrixEx
    {
        public static Matrix4x4 Rotate(float x, float y, float z)
        {
            return Matrix4x4.Rotate(QuaternionEx.Euler(x, y, z));
        }
    }
}