using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public struct HealthState
    {
        public int s_Total_Hit_Points() => s_total_hit_points;
        public int s_Current_Hit_Points() => s_current_hit_points;

        public bool is_Alive() => this.s_current_hit_points > 0;
        public bool is_Dead() => !is_Alive();

        public static HealthState s_Initial_For(UnitType the_unit_type) => new HealthState(the_unit_type.s_Hit_Points());

        public HealthState(int the_total_hit_points): this(the_total_hit_points, the_total_hit_points) {}
        public HealthState(int the_total_hit_points, int the_current_hit_points)
        {
            this.s_total_hit_points = the_total_hit_points;
            this.s_current_hit_points = the_current_hit_points;
        }
        
        public HealthState After_Applying(Damage damage) => new HealthState(this.s_total_hit_points, this.s_current_hit_points - damage.Amount);
        
        private readonly int s_current_hit_points;
        private readonly int s_total_hit_points;
    }
}