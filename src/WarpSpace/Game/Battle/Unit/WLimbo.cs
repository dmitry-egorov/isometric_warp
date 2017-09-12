using Lanski.UnityExtensions;
using UnityEngine;

namespace WarpSpace.Game.Battle.Unit
{
    public class WLimbo: MonoBehaviour
    {
        private Transform its_transform;
        
        public Transform s_Transform => its_transform ?? (its_transform = transform);

        public void Destroys_All_Children()
        {
            gameObject.DestroyChildren();
        }
    }
}