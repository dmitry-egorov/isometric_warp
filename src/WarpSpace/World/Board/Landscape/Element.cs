using UnityEngine;

namespace WarpSpace.World.Board.Landscape
{
    public class Element: MonoBehaviour
    {
        [SerializeField] MeshFilter _landscapeMeshFilter;
        
        public void Init(Mesh mesh)
        {
            _landscapeMeshFilter.sharedMesh = mesh;
        }
    }
}