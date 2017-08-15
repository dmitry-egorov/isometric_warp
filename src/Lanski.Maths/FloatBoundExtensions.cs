namespace Lanski.Maths
{
    public static class FloatBoundExtensions
    {
        public static float Bound(this float v, float min, float max)
        {
            if (v <= min)
                return min;
            if (v >= max) 
                return max;
            
            return v;
        }
    }
}