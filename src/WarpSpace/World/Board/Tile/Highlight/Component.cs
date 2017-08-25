using UnityEngine;

namespace WarpSpace.World.Board.Tile.Highlight
{
    public class Component: MonoBehaviour
    {
        private Landscape.Component _landscape;

        public void Init(Landscape.Component landscape)
        {
            _landscape = landscape;
        }

        public void SetMove()
        {
            _landscape.SetMoveHighlightMaterial();
        }

        public void Reset()
        {
            _landscape.SetNormalMaterial();
        }
    }
}