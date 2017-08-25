using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.Water
{
    public class Component : MonoBehaviour
    {
        [SerializeField] MeshFilter _meshFilter;

        public void Init(ComponentSpec? spec)
        {
            spec.Do(s =>
            {
                _meshFilter.sharedMesh = s.Mesh;
                transform.localRotation = s.Rotation;
                transform.localScale = s.Scale;
            });
        }
    }
}