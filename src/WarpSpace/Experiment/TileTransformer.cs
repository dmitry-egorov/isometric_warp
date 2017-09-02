using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common.MeshTransformation;

namespace WarpSpace.Experiment
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class TileTransformer: MonoBehaviour
    {
        public Direction2D Rotation;
        public float Falloff;
        public Elevations Elevation;
        public Mesh Mesh;

        private bool _initialized;
        
        private ChangeStream<Direction2D, float, Elevations, Mesh> _change;
        private MeshFilter _meshFilter;

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            Initialize();

            _change.Update(Rotation, Falloff, Elevation, Mesh);
        }

        private void Initialize()
        {
            if(_initialized)
                return;

            _initialized = true;

            _meshFilter = GetComponent<MeshFilter>();
            _change = new ChangeStream<Direction2D, float, Elevations, Mesh>();
            _change.Subscribe(x => UpdateMesh());
        }

        private void UpdateMesh()
        {
            _meshFilter.sharedMesh = MeshTransformer.Transform(Mesh, Rotation, Elevation.ToNeighbourhood(), Falloff);
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

            internal FullNeighbourhood2D<float> ToNeighbourhood()
            {
                var a = new float[3,3];
                
                a[0, 0] = LU;
                a[1, 0] = U;
                a[2, 0] = RU;
                a[0, 1] = L;
                a[1, 1] = 0f;
                a[2, 1] = R;
                a[0, 2] = LD;
                a[1, 2] = D;
                a[2, 2] = RD;
                
                return new FullNeighbourhood2D<float>(a);
            }

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