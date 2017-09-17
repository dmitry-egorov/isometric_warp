using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Game.Battle.Tile
{
    public class WUnitSlot: MonoBehaviour
    {
        public Vector3 s_Position => its_transform.position;
        public Transform s_Transform => its_transform;

        public void Awake()
        {
            its_transform = transform;
        }
        
        private Transform its_transform;

        
    }
}