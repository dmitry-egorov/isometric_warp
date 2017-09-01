using UnityEngine;
using WarpSpace.Common.Outline;

namespace WarpSpace.Unity.World.Battle.Unit
{
    [RequireComponent(typeof(OutlineMeshBuilder))]
    public class OutlineComponent : MonoBehaviour
    {
        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);

        public void Init(Mesh mesh)
        {
            GetComponent<OutlineMeshBuilder>().Build_From(mesh);
            Disable();
        }
    }
}