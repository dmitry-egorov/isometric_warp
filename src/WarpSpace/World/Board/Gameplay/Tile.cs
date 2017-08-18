using Lanski.Structures;
using UnityEngine;
using WarpSpace.Planet.Tiles;

namespace WarpSpace.World.Board.Gameplay
{
    public class Tile : MonoBehaviour
    {
        LandscapeType _landscapeType;
        Index2D _index;

        public void Init(LandscapeType landscapeType, Index2D index)
        {
            _landscapeType = landscapeType;
            _index = index;
        }
    }
}