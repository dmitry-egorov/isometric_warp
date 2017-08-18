using System;

namespace WarpSpace.World.Board.Landscape
{
    [Serializable]
    public struct Data
    {
        public TileData Mountain;
        public TileData Hill;
        public TileData Flatland;
        public TileData Water;

        public TileData GetSpecFor(LandscapeType type)
        {
            switch (type)
            {
                case LandscapeType.Mountain:
                    return Mountain;
                case LandscapeType.Hill:
                    return Hill;
                case LandscapeType.Flatland:
                    return Flatland;
                case LandscapeType.Water:
                    return Water;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}