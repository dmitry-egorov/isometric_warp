namespace Core.Structures
{
    using UnityEngine;
    using static UnityEngine.Mathf;

    public static class Mathe
    {
        /// <summary>
        /// Absolute tangents of an angle in degrees
        /// </summary>
        public static float AbsTand(float degrees)
        {
            return Abs(Tand(degrees));
        }

        /// <summary>
        /// Absolute sine of an angle in degrees
        /// </summary>
        public static float AbsSind(float degrees)
        {
            return Abs(Sind(degrees));
        }

        /// <summary>
        /// Tangents of an angle in degrees
        /// </summary>
        public static float Tand(float degrees)
        {
            return Tan(Deg2Radi(degrees));
        }

        /// <summary>
        /// Sine of an angle in degrees
        /// </summary>
        public static float Sind(float degrees)
        {
            return Sin(Deg2Radi(degrees));
        }

        public static float Deg2Radi(float degrees)
        {
            return Deg2Rad * degrees;
        }

        public static Quaternion Rotation(float degrees)
        {
            return Quaternion.Euler(0, 0, degrees);
        }
    }
}