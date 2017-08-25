using UnityEngine;

namespace WarpSpace.World.Unit
{
    public struct ComponentSpec
    {
        public readonly Quaternion Rotation;
        public readonly ChassisType Chassis;
        public readonly Board.Tile.Component Tile;

        public ComponentSpec(Quaternion rotation, ChassisType chassis, Board.Tile.Component tile)
        {
            Rotation = rotation;
            Chassis = chassis;
            Tile = tile;
        }
    }
}