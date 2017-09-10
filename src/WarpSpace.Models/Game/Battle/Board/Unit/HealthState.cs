using System;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public struct HealthState: IEquatable<HealthState>
    {
        public static HealthState s_Initial_For(UnitType the_unit_type) => new HealthState(the_unit_type.s_Hit_Points());

        public HealthState(int the_total_hit_points): this(the_total_hit_points, the_total_hit_points) {}
        public HealthState(int the_total_hit_points, int the_current_hit_points)
        {
            its_total_hit_points = the_total_hit_points;
            its_current_hit_points = the_current_hit_points;
        }
        
        public int s_Total_Hit_Points => its_total_hit_points;
        public int s_Current_Hit_Points => its_current_hit_points;

        public bool is_Lethal => !is_Normal;
        public bool is_Normal => its_current_hit_points > 0;
        
        public HealthState After_Applying(Damage damage) => new HealthState(its_total_hit_points, its_current_hit_points - damage.Amount);
        public bool Equals(HealthState other) => its_current_hit_points == other.its_current_hit_points && its_total_hit_points == other.its_total_hit_points;
        
        private readonly int its_current_hit_points;
        private readonly int its_total_hit_points;

    }
}