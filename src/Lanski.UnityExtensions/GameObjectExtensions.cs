using UnityEngine;

namespace Lanski.UnityExtensions
{
    public static class GameObjectExtensions
    {
        public static void Show(this GameObject game_object) => game_object.SetActive(true);
        public static void Hide(this GameObject game_object) => game_object.SetActive(false);
    }
}