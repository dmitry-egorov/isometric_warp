using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public class MLandscape
    {
        public readonly LandscapeType Type;

        public MLandscape(LandscapeType type)
        {
            Type = type; //Note: can check value here
        }

        public bool Is_Passable_By(ChassisType chassisType) => Type.is_passable_with(chassisType);
    }
}