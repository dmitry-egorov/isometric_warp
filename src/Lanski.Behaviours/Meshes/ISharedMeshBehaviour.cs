using System;
using Lanski.Reactive;
using UnityEngine;

namespace Lanski.Behaviours.Meshes
{
    public interface ISharedMeshBehaviour
    {
        ICell<MeshContainer> MeshCell { get; }
    }
    
    
        
    public struct MeshContainer: IEquatable<MeshContainer>
    {
        public readonly Mesh s_Mesh;

        public MeshContainer(Mesh mesh)
        {
            s_Mesh = mesh;
        }

        public bool Equals(MeshContainer other) => Equals(s_Mesh, other.s_Mesh);

        public static implicit operator MeshContainer(Mesh the_mesh) => new MeshContainer(the_mesh);
    }
}