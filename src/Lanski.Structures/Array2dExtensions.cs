using System;
using System.ComponentModel;

namespace Lanski.Structures
{
    public static class Array2DExtensions
    {
        public static bool Is<T>(this T[,] array, int column, int row, Func<T, bool> condition, bool defaultValue = false) 
            where T : struct
        {
            return array.TryGet(column, row).Select(condition).GetValueOrDefault(defaultValue);
        }
        
        public static T? TryGet<T>(this T[,] array, int column, int row)
            where T: struct
        {
            var columns = array.GetLength(0);
            var rows = array.GetLength(1);

            if (column < 0 || column >= columns || row < 0 || row >= rows)
                return default(T?);

            return array[column, row];
        }
    }
}