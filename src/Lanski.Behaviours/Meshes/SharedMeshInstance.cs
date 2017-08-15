using System;
using Lanski.Reactive;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lanski.Behaviours.Meshes
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public abstract class SharedMeshInstance<T>: MonoBehaviour
        where T : Object, ISharedMeshBehaviour
    {
        [NonSerialized] private bool _isInitialized;

        private MeshFilter _filter;

        private RefCell<Mesh> _ownMeshCell;
        private Action _subscription;

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            Initialize();

            _ownMeshCell.Value = _filter.sharedMesh;
        }

        void OnDestroy()
        {
            _subscription?.Invoke();
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            _filter = GetComponent<MeshFilter>();

            _ownMeshCell = new RefCell<Mesh>(_filter.sharedMesh);

            _subscription = FindObjectOfType<T>().MeshCell
                .Merge(_ownMeshCell.Where(x => x == null))
                .Subscribe(_ => UpdateMesh());
        }

        private void UpdateMesh()
        {
            _filter.sharedMesh = FindObjectOfType<T>().MeshCell.Value;
        }
    }
}