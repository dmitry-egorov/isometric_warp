using UnityEngine;

namespace WarpSpace.Game.Battle.Board
{
    public class WGameTime
    {
        public WGameTime(float the_boost_multiplier)
        {
            its_boost_multiplier = the_boost_multiplier;
        }
        
        public bool is_Fast_Forwarding => it_is_fast_forwarding;
        public float s_Elapsed_Seconds => (it_is_fast_forwarding ? its_boost_multiplier : 1f) * Time.deltaTime;

        public void Starts_to_Flow_at_Fast_Forwards_Speed() => it_is_fast_forwarding = true;
        public void Resumes_to_Normal() => it_is_fast_forwarding = false;

        private readonly float its_boost_multiplier;
        private bool it_is_fast_forwarding;
    }
}