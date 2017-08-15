using Core.Structures.Unity.Meshes;
using UnityEngine;

namespace WarpSpace.Common.Unity.Meshes
{
    public static class MeshExtensions
    {
        public static Mesh Translate(this Mesh mesh, Vector3 offset)
        {
            return mesh.Transform(Matrix4x4.Translate(offset));
        }
        
        public static Mesh Transform(this Mesh mesh, Matrix4x4 transform)
        {
            return MeshEx.Combine(new[] {new CombineInstance {mesh = mesh, transform = transform}});
        }
    }
}