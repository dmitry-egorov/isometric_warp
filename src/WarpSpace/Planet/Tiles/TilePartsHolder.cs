using UnityEngine;

namespace WarpSpace.Planet.Tiles
{
    public class TilePartsHolder: MonoBehaviour
    {
        [SerializeField] MeshFilter _landscapeMeshFilter;
        
        public void Init(Mesh mesh, GameObject objectPrefab)
        {
            _landscapeMeshFilter.sharedMesh = mesh;
            if (objectPrefab != null)
            {
                Instantiate(objectPrefab, transform);
            }
        }
    }
}