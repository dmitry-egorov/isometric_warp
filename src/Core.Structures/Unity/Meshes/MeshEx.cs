using UnityEngine;

namespace Core.Structures.Unity.Meshes
{
    public static class MeshEx
    {
        public static Mesh Combine(CombineInstance[] combineInstances)
        {
            for (var i = 0; i < combineInstances.Length; i++)
            {
                var transform = combineInstances[i].transform;

                combineInstances[i].transform = transform == Matrix4x4.zero
                    ? Matrix4x4.identity
                    : transform;
            }

            var result = new Mesh();
            result.CombineMeshes(combineInstances);
            return result;
        }
    }
}