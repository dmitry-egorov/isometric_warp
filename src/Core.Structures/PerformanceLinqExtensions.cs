namespace Core.Structures
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class PerformanceLinqExtensions
    {
        public static bool Any<T>(this T[] array, Func<T, bool> condition)
        {
            foreach (var item in array)
            {
                if (condition(item))
                    return true;
            }

            return false;
        }

        public static bool Any<T>(this List<T> array, Func<T, bool> condition)
        {
            foreach (var item in array)
            {
                if (condition(item))
                    return true;
            }

            return false;
        }

        public static void Do<T>(this T[] array, Action<T> action)
        {
            foreach (var item in array)
            {
                action(item);
            }
        }

        public static void DoOnFirst<T>(this T[] array, Action<T> action)
        {
            if (array.Length == 0)
                return;

            action(array[0]);
        }

        public static T First<T>(this T[] array, Func<T, bool> condition)
        {
            foreach (var item in array)
            {
                if (condition(item))
                    return item;
            }

            throw new InvalidOperationException("No item satisfied the condition");
        }

        public static T Last<T>(this T[] array)
        {
            return array[array.Length - 1];
        }

        public static Vector3 Sum(this List<Vector3> vectors)
        {
            return vectors.Sum(Vector3.zero);
        }

        public static Vector3 Sum(this List<Vector3> vectors, Vector3 origin)
        {
            var sum = origin;
            foreach (var vector in vectors)
            {
                sum += vector;
            }

            return sum;
        }
    }
}