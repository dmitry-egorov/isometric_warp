using System.Collections.Generic;
using UnityEngine;

namespace Lanski.UnityExtensions
{
    public static class GameObjectExtensions
    {
        public static void Shows(this GameObject game_object) => game_object.SetActive(true);
        public static void Hides(this GameObject game_object) => game_object.SetActive(false);
        
        public static IEnumerable<GameObject> GetChildren(this GameObject obj)
        {
            var the_transform = obj.transform;
            var count = the_transform.childCount;
            for (var i = 0; i < count; i++)
            {
                yield return the_transform.GetChild(i).gameObject;
            }
        }
    }
}