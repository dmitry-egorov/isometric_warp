using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.StructureSlot
{
    public class Component: MonoBehaviour
    {
        public void Init(ComponentSpec? elementSpec)
        {
            elementSpec.Do(x =>
            {
                var structure = Instantiate(x.Prefab, transform);
                structure.transform.localRotation = x.Rotation;
            });
        }
    }
}