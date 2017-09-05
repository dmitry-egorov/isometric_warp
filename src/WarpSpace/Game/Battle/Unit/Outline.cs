using UnityEngine;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(OutlineMeshBuilder))]
    public class Outline : MonoBehaviour
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