using Lanski.Structures;
using UnityEngine;
using WarpSpace.Planet.Tiles;

namespace WarpSpace.World.Board.Water
{
    public class Tile : MonoBehaviour
    {
        public void Init(Direction2D rotation, bool mirror)
        {
            transform.localRotation = Quaternion.Euler(0, rotation.ToAngle(), 0);
            transform.localScale = mirror ? new Vector3(-1, 1, 1) : Vector3.one;
        }
    }
}