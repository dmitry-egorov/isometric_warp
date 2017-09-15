using System.Collections.Generic;

namespace WarpSpace.Models.Descriptions
{
    public class MChassisType
    {
        public MChassisType(IReadOnlyDictionary<MLandscapeType, Passability> the_its_passability_map) => its_passability_map = the_its_passability_map;

        public Passability s_Passability_Of(MLandscapeType the_landscape_type) => its_passability_map[the_landscape_type];
        public bool can_Pass(MLandscapeType the_landscape_type) => this.s_Passability_Of(the_landscape_type) != Passability.None;

        private readonly IReadOnlyDictionary<MLandscapeType, Passability> its_passability_map;
    }
}