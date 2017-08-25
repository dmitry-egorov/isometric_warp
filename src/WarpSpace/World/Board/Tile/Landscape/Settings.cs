using System;

namespace WarpSpace.World.Board.Tile.Landscape
{
    [Serializable]
    public struct Settings
    {
        public TileSettings Mountain;
        public TileSettings Hill;
        public TileSettings Flatland;
        public TileSettings Water;

        
    }
}