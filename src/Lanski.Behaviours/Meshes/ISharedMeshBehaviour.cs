using Lanski.Reactive;
using UnityEngine;

namespace Lanski.Behaviours.Meshes
{
    public interface ISharedMeshBehaviour
    {
        ICell<Mesh> MeshCell { get; }
    }
}