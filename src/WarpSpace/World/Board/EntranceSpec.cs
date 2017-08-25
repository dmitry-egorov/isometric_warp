using UnityEngine;
using WarpSpace.World.Board.Tile.Descriptions;

namespace WarpSpace.World.Board
{
    public struct EntranceSpec
    {
        public readonly Spacial2D Spacial;
        public readonly GameObject MothershipPrefab;

        public EntranceSpec(GameObject mothershipPrefab, Spacial2D spacial)
        {
            Spacial = spacial;
            MothershipPrefab = mothershipPrefab;
        }
    }
}