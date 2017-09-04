using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class LandscapeModel
    {
        public readonly LandscapeType Type;

        public LandscapeModel(LandscapeType type)
        {
            Type = type; //Note: can check value here
        }

        public bool Is_Passable_By(ChassisType chassisType) => Type.IsPassableWith(chassisType);
    }
}