using UnityEngine;

namespace Lanski.Maths
{
    public static class FloatEqualityExtensions
    {
        public static bool is_Approximately(this float the_value, float the_other, float epsilon = 0.000001f) => Mathf.Abs(the_value - the_other) < epsilon;
        public static bool is_Exactly(this float the_value, float the_other)
        {
            return the_value == the_other;
        }
        
    }
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