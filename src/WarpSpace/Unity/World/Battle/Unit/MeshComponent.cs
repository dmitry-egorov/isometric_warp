using UnityEngine;

namespace WarpSpace.Unity.World.Battle.Unit
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshComponent : MonoBehaviour
    {
        public void Init(Mesh mesh, Material material)
        {
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshRenderer>().sharedMaterial = material;
        }
    }
}