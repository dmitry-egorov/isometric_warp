using UnityEngine;

namespace Core.Structures.Unity.Components
{
    public struct LazyObject<T> where T: MonoBehaviour
    {
        private T _value;
        public T Value => _value ?? (_value = Object.FindObjectOfType<T>());
    }
}