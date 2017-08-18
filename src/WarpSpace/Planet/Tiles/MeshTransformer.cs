using System;
using System.Collections.Generic;
using System.Linq;
using Lanski.Geometry;
using Lanski.Structures;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WarpSpace.Planet.Tiles
{
    public static class MeshTransformer
    {
        public static Mesh Transform(Mesh original, Direction2D rotation, Elevations elevations, float falloff)
        {
            var mesh = Object.Instantiate(original);

            mesh.vertices = Bend(Rotate(mesh.vertices, rotation), elevations, falloff).ToArray();

            mesh.RecalculateNormals();
            
            return mesh;
        }

        private static IEnumerable<Vector3> Rotate(IEnumerable<Vector3> vs, Direction2D rotation)
        {
            var angle = Quaternion.Euler(0, rotation.ToAngle(), 0);

            return vs.Select(v => angle * v );
        }

        private static IEnumerable<Vector3> Bend(IEnumerable<Vector3> vs, Elevations elevations, float falloff)
        {
            return vs.Select(v => TransformVertex(v, elevations, falloff));
        }

        private static Vector3 TransformVertex(Vector3 v, Elevations elevations, float falloff)
        {
            if (v.y < -0.9f)
                return v;
            return v + new Vector3(0, GetOffset(v.XZ(), elevations, falloff), 0);
        }

        private static float GetOffset(Vector2 p, Elevations elevations, float falloff)
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
            
            var e0 = s0 == 0 ? e.U
                   : s0 == 1 ? e.L
                   : s0 == 2 ? e.D
                   :           e.R;
            
            var s1 = GetSector(p);

            var e1 =  s1 == 0 ? e.RU 
                    : s1 == 1 ? e.LU
                    : s1 == 2 ? e.LD
                    :           e.RD;

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
        
        [Serializable]
        public struct Elevations
        {
            [Range(0,1)] public float L;
            [Range(0,1)] public float LU;
            [Range(0,1)] public float U;
            [Range(0,1)] public float RU;
            [Range(0,1)] public float R;
            [Range(0,1)] public float RD;
            [Range(0,1)] public float D;
            [Range(0,1)] public float LD;

            public bool Equals(Elevations other)
            {
                return L.Equals(other.L) && LU.Equals(other.LU) && U.Equals(other.U) && RU.Equals(other.RU) && R.Equals(other.R) && RD.Equals(other.RD) && D.Equals(other.D) && LD.Equals(other.LD);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Elevations && Equals((Elevations) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = L.GetHashCode();
                    hashCode = (hashCode * 397) ^ LU.GetHashCode();
                    hashCode = (hashCode * 397) ^ U.GetHashCode();
                    hashCode = (hashCode * 397) ^ RU.GetHashCode();
                    hashCode = (hashCode * 397) ^ R.GetHashCode();
                    hashCode = (hashCode * 397) ^ RD.GetHashCode();
                    hashCode = (hashCode * 397) ^ D.GetHashCode();
                    hashCode = (hashCode * 397) ^ LD.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}