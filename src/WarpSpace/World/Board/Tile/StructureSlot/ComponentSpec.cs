using UnityEngine;

namespace WarpSpace.World.Board.Tile.StructureSlot
{
    public struct ComponentSpec
    {
        public readonly GameObject Prefab;
        public readonly Quaternion Rotation;

        public ComponentSpec(GameObject prefab, Quaternion rotation)
        {
            Prefab = prefab;
            Rotation = rotation;
        }
    }
}