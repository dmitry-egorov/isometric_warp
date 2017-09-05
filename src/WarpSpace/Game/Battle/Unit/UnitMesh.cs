using UnityEngine;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class UnitMesh : MonoBehaviour
    {
        public void Init(Mesh mesh, Material material)
        {
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshRenderer>().sharedMaterial = material;
        }
    }
}