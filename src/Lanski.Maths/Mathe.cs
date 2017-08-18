﻿namespace Lanski.Maths
{
    public static class Mathe
    {
        public static float Avg(float v1, float v2)
        {
            return (v1 + v2) * 0.5f;
        }

        public static int Mod(int i, int mod)
        {
            var m = i % mod;
            return m < 0 ? m + mod : m;
        }
    }
}