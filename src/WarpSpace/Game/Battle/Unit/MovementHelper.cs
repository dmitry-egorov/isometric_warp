using UnityEngine;

namespace WarpSpace.Game.Battle.Unit
{
    public static class MovementHelper
    {
        public static float the_acceleration_from(float max_speed, float min_speed, float acceleration_distance)
        {
            var v0 = max_speed;
            var v1 = min_speed;
            var d = acceleration_distance;

            return Mathf.Abs((v1 * v1 - v0 * v0) / (2f * d));
        }
    }
}