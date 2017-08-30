using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Landscape
{
    public class LandscapeModel
    {
        public readonly LandscapeType Type;

        public LandscapeModel(LandscapeType type)
        {
            Type = type;//Note: can check value here
        }

        public bool IsPassableBy(ChassisType chassisType)
        {
            return Type.IsPassableWith(chassisType);
        }
    }
}