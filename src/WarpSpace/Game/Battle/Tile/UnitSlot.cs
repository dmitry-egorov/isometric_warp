using UnityEngine;

namespace WarpSpace.Game.Battle.Tile
{
    public class UnitSlot: MonoBehaviour
    {
        //Only used as a placeholder for now
        public Transform Transform { get; private set; }

        public void Awake()
        {
            Transform = transform;
        }
    }
}