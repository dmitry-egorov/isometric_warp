using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Services;

namespace WarpSpace.Game.Battle.BoardGenerators
{
    [CreateAssetMenu(fileName = "Random Board", menuName = "Custom/Boards/Random", order = 1)]
    public class RandomBoard : BoardSettings
    {
        public override DBoard s_Description => new RandomBoardGenerator(new UnityRandom()).Generates_a_Board();
    }
}