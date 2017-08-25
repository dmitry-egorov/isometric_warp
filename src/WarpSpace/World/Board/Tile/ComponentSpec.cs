using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board.Tile
{
    public struct ComponentSpec
    {
        public readonly Vector3 SpacePosition;
        public readonly string Name;
        
        public readonly Index2D Position;
        public readonly Player.Component Player;

        public readonly Landscape.ComponentSpec Landscape;
        public readonly Water.ComponentSpec? Water;
        public readonly StructureSlot.ComponentSpec? Structure;
        
        public ComponentSpec(Vector3 spacePosition, string name, Index2D position, Player.Component player, Landscape.ComponentSpec landscape, Water.ComponentSpec? water, StructureSlot.ComponentSpec? structure)
        {
            SpacePosition = spacePosition;
            Name = name;
            Landscape = landscape;
            Water = water;
            Structure = structure;
            Player = player;
            Position = position;
        }

    }
}