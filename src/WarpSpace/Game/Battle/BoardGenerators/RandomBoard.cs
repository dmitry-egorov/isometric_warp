using System.Collections.Generic;
using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Services;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.BoardGenerators
{
    [CreateAssetMenu(fileName = "Random Board", menuName = "Custom/Boards/Random", order = 1)]
    public class RandomBoard : BoardSettings
    {
        public override DBoard s_Description_With(IReadOnlyList<MUnitType> the_unit_types, IReadOnlyList<MLandscapeType> the_landscape_types, MFaction the_native_faction, MChassisType the_mothership_chassis_type) => new RandomBoardGenerator(new UnityRandom(), the_mothership_chassis_type, LandscapeTypeSettings.s_All_Models).Generates_a_Board();
    }
}