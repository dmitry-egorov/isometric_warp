using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.Planet.Tiles
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class TileTransformer: MonoBehaviour
    {
        [SerializeField] Direction2D _rotation;
        [SerializeField] float _falloff;
        [SerializeField] MeshTransformer.Elevations _elevation;
        [SerializeField] Mesh _mesh;

        private bool _initialized;
        
        private ChangeStream<Direction2D, float, MeshTransformer.Elevations, Mesh> _change;
        private MeshFilter _meshFilter;

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            Initialize();

            _change.Update(_rotation, _falloff, _elevation, _mesh);
        }

        private void Initialize()
        {
            if(_initialized)
                return;

            _initialized = true;

            _meshFilter = GetComponent<MeshFilter>();
            _change = new ChangeStream<Direction2D, float, MeshTransformer.Elevations, Mesh>();
            _change.Subscribe(x => UpdateMesh());
        }

        private void UpdateMesh()
        {
            _meshFilter.sharedMesh = MeshTransformer.Transform(_mesh, _rotation, _elevation, _falloff);
        }
    }
}