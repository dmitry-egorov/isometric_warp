using System.Linq;
using UnityEngine;

namespace Lanski.UnityExtensions
{
    public static class MonoBehaviourChildrenExtensions
    {
        public static void DestroyChildren(this GameObject gameObject)
        {
            var children = gameObject
                .transform
                .Cast<Transform>()
                .Select(x => x.gameObject)
                .ToArray();
            
            foreach (var child in children)
            {
                Object.DestroyImmediate(child);
            }
        }
    }
}