using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarpSpace.Overlay.Units
{
    public class OutlineMeshBuilder
    {
        public static Mesh Builds(Mesh source_mesh) => builds(source_mesh);
        
        private static Mesh builds(Mesh source_mesh)
        {
            var new_vertices = new List<VertexAccumulator>();
            var vertex_map = new Dictionary<Vector3, int>();

            var src_vertices = source_mesh.vertices;
            var src_normals = source_mesh.normals;
            
            for (var i = 0; i < src_vertices.Length; i++)
            {
                var position = src_vertices[i];
                var normal = src_normals[i];
                
                if (!vertex_map.TryGetValue(position, out var newIndex))
                {
                    new_vertices.Add(new VertexAccumulator(position, normal));
                    vertex_map.Add(position, new_vertices.Count - 1);
                }
                else
                {
                    new_vertices[newIndex].AddNormal(normal);
                }                
            }

            return new Mesh
            {
                vertices = new_vertices.Select(x => x.Position).ToArray(),
                normals = new_vertices.Select(x => x.Normal).Select(x => x.normalized).ToArray(),
                triangles = source_mesh.triangles.Select(i => vertex_map[src_vertices[i]]).ToArray()
            };
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