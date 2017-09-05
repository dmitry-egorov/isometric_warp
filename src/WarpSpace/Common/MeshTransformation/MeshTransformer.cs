using System.Collections.Generic;
using System.Linq;
using Lanski.Geometry;
using Lanski.Structures;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WarpSpace.Common.MeshTransformation
{
    public static class MeshTransformer
    {
        public static Mesh Transform(Mesh original, Direction2D rotation, FullNeighbourhood2D<float> elevations, float falloff)
        {
            var mesh = Object.Instantiate(original);

            mesh.vertices = Bend(Rotate(mesh.vertices, rotation), elevations, falloff).ToArray();

            mesh.RecalculateNormals();
            
            return mesh;
        }

        private static IEnumerable<Vector3> Rotate(IEnumerable<Vector3> vs, Direction2D orientation)
        {
            var rotation = orientation.To_Rotation();

            return vs.Select(v => rotation * v );
        }

        private static IEnumerable<Vector3> Bend(IEnumerable<Vector3> vs, FullNeighbourhood2D<float> elevations, float falloff)
        {
            return vs.Select(v => TransformVertex(v, elevations, falloff));
        }

        private static Vector3 TransformVertex(Vector3 v, FullNeighbourhood2D<float> elevations, float falloff)
        {
            if (v.y < -0.9f)
                return v;
            return v + new Vector3(0, GetOffset(v.XZ(), elevations, falloff), 0);
        }

        private static float GetOffset(Vector2 p, FullNeighbourhood2D<float> elevations, float falloff)
        {
            //                       oe
            //  \ * |   /    e1\--0-----|e0
            //   \  |  /        \  \  a |
            //    \ | /          \  *---|
            //     \|/            \  \  |
            //  ----+----  -->     \  \ | b
            //     /|\              \ \ |
            //    / | \              \ \|
            //   /  |  \              \\|
            //  /   |   \              \|
            
            
            if (p.sqrMagnitude < 0.0001f)
                return 0;
            
            var e = elevations;
            var f = falloff;

            var s0 = GetSector(p.Rotate(-45));
            
            var e0 = s0 == 0 ? e.Up
                   : s0 == 1 ? e.Left
                   : s0 == 2 ? e.Down
                   :           e.Right;
            
            var s1 = GetSector(p);

            var e1 =  s1 == 0 ? e.RightUp
                    : s1 == 1 ? e.LeftUp
                    : s1 == 2 ? e.LeftDown
                    :           e.RightDown;

            var pt = p.Rotate(s0 * 90); // coordinates inside a triangle

            var a = Mathf.Abs(pt.x) * 2f;
            var b = Mathf.Abs(pt.y) * 2f;
            
            var oe = a / b; // offset along the edge 
            var edgeHeight = Mathf.SmoothStep(e0, e1, oe);
            var dca = Mathf.Pow(b, f); // proportional distance to the center adjasted for falloff
            var height = Mathf.SmoothStep(0, edgeHeight, dca);

            return height;
        }

        private static int GetSector(Vector2 v)
        {
            if (v.x >= 0 && v.y >= 0)
                return 0;
            if (v.x < 0 && v.y >= 0)
                return 1;
            if (v.x < 0 && v.y < 0)
                return 2;
            // (v.x >= 0 && v.y < 0)
                return 3;
        }
    }
}