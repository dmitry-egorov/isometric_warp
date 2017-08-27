using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Landscape
{
    public class Model
    {
        public readonly LandscapeType Type;

        public Model(LandscapeType type)
        {
            Type = type;//Note: can check value here
        }

        public bool IsPassableBy(Chassis chassis)
        {
            return Type.IsPassableWith(chassis);
        }
    }
}