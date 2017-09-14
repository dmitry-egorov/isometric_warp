using System.Collections.Generic;

namespace WarpSpace.Models.Descriptions
{
    public class MChassisType
    {
        public MChassisType(IReadOnlyDictionary<LandscapeType, Passability> the_its_passability_map) => its_passability_map = the_its_passability_map;

        public Passability s_Passability_Of(LandscapeType the_landscape_type) => its_passability_map[the_landscape_type];
        public bool can_Pass(LandscapeType the_landscape_type) => this.s_Passability_Of(the_landscape_type) != Passability.None;

        private readonly IReadOnlyDictionary<LandscapeType, Passability> its_passability_map;
    }
}