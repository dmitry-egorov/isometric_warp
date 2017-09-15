using System.Collections.Generic;
using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Game.Battle.BoardGenerators
{
    public abstract class BoardSettings : ScriptableObject
    {
        public abstract DBoard s_Description_With(IReadOnlyList<MUnitType> the_unit_types, IReadOnlyList<MLandscapeType> the_landscape_types, MFaction the_native_faction, MChassisType the_mothership_chassis_type);
    }
}