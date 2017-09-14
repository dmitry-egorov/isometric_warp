using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class MLandscape
    {
        public LandscapeType s_Type => its_type;

        public MLandscape(LandscapeType type)
        {
            its_type = type; //Note: can check value here
        }

        public Passability s_Passability_With(MChassisType the_chassis_type) => the_chassis_type.s_Passability_Of(its_type);
        public bool is_Passable_With(MChassisType the_chassis_type) => the_chassis_type.can_Pass(its_type);
        
        private readonly LandscapeType its_type;
    }
}