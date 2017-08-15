using System;

namespace Lanski.UnityExtensions
{
    public static class RandomExtensions
    {
        public static T RandomElement<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
    }
}