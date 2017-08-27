using UnityEngine;

namespace Lanski.Maths
{
    public static class Mathe
    {
        public static float Avg(float v1, float v2, float v3, float v4)
        {
            return (v1 + v2 + v3 + v4) / 4f;
        }
        
        public static float Avg(float v1, float v2, float v3)
        {
            return (v1 + v2 + v3) / 3f;
        }
        
        public static float Avg(float v1, float v2)
        {
            return (v1 + v2) * 0.5f;
        }

        public static int Mod(int i, int mod)
        {
            var m = i % mod;
            return m < 0 ? m + mod : m;
        }

        public static float Min(float v1, float v2, float v3)
        {
            return Mathf.Min(Mathf.Min(v1, v2), v3);
        }
        
        public static float Min(float v1, float v2, float v3, float v4)
        {
            return Mathf.Min(Mathf.Min(v1, v2), Mathf.Min(v3, v4));
        }

        public static int Abs(int i)
        {
            return i >= 0 ? i : -i;
        }
    }
}