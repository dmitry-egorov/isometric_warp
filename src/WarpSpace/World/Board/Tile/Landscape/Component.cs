using UnityEngine;
using WarpSpace.World.Board.Tile.Descriptions;
using WarpSpace.World.Unit;

namespace WarpSpace.World.Board.Tile.Landscape
{
    public class Component: MonoBehaviour
    {
        public MeshFilter LandscapeMeshFilter;
        public MeshRenderer Renderer;
        public Material Normal;
        public Material MoveHighlight;
        
        private LandscapeType _type;

        public void Init(ComponentSpec spec)
        {
            LandscapeMeshFilter.sharedMesh = spec.Mesh;
            _type = spec.Type;
        }

        public bool IsPassableBy(Unit.Component unit)
        {
            return _type.IsPassableWith(unit.Chassis);
        }

        public void SetNormalMaterial()
        {
            Renderer.sharedMaterial = Normal;
        }

        public void SetMoveHighlightMaterial()
        {
            Renderer.sharedMaterial = MoveHighlight;
        }
    }
}