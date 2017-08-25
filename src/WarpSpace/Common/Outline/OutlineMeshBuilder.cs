using System.Collections.Generic;
using System.Linq;
using UnityEditor.Expose;
using UnityEngine;

namespace WarpSpace.Common.Outline
{
    [RequireComponent(typeof(MeshFilter))]
    public class OutlineMeshBuilder: MonoBehaviour
    {
        public MeshFilter Source;

        [ExposeMethodInEditor]
        public void Build()
        {
            var dst = GetComponent<MeshFilter>();
            var src = Source;
            
            var newVertices = new List<VertexAccumulator>();
            var vertexMap = new Dictionary<Vector3, int>();

            var srcMesh = src.sharedMesh;
            var srcVertices = srcMesh.vertices;
            var srcNormals = srcMesh.normals;
            
            for (var i = 0; i < srcVertices.Length; i++)
            {
                var position = srcVertices[i];
                var normal = srcNormals[i];
                
                if (!vertexMap.TryGetValue(position, out var newIndex))
                {
                    newVertices.Add(new VertexAccumulator(position, normal));
                    vertexMap.Add(position, newVertices.Count - 1);
                }
                else
                {
                    newVertices[newIndex].AddNormal(normal);
                }                
            }

            var newMesh = new Mesh
            {
                vertices = newVertices.Select(x => x.Position).ToArray(),
                normals = newVertices.Select(x => x.Normal).Select(x => x.normalized).ToArray(),
                triangles = srcMesh.triangles.Select(i => vertexMap[srcVertices[i]]).ToArray()
            };
            
            dst.sharedMesh = newMesh;
        }
        
        private class VertexAccumulator
        {
            public readonly Vector3 Position;
            public Vector3 Normal { get; private set; }

            private int _count;

            public VertexAccumulator(Vector3 position, Vector3 normal)
            {
                Position = position;
                Normal = normal;
                
                _count = 1;
            }

            public void AddNormal(Vector3 normal)
            {
                Normal = (Normal * _count + normal) / (_count + 1);
                _count++;
            }
        }
    }
}