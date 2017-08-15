using UnityEngine;

namespace WarpSpace.Planet.Tiles
{
    public class TilePartsHolder: MonoBehaviour
    {
        [SerializeField] MeshFilter _landscapeMeshFilter;
        [SerializeField] GameObject _modelAnchor;
        [SerializeField] GameObject _water;
        
        public GameObject ModelAnchor => _modelAnchor;

        public void SetMesh(Mesh mesh)
        {
            _landscapeMeshFilter.sharedMesh = mesh;
        }

        public void SetWaterRotation(RotationsBy90 randomRotation)
        {
            _water.transform.localRotation = Quaternion.Euler(0, (float)randomRotation * 90, 0);
        }
    }
}