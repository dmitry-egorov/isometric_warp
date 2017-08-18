using System;
using System.Collections.Generic;
using System.Linq;

namespace Lanski.Structures
{
    public static class Array2D
    {
        public static T[,] Create<T>(Dimensions2D dimensions)
        {
            return new T[dimensions.Rows, dimensions.Columns];
        }
        
        public static T[,] Create<T>(Dimensions2D dimensions, Func<Index2D, T> valueSelector)
        {
            var result = Create<T>(dimensions);
            
            foreach (var i in result.EnumerateIndex())
            {
                result.Set(i, valueSelector(i));
            }
            
            return result;
        }
    }
    
    public static class Array2DExtensions
    {
        public static T[,] To2DArray<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            return enumerable.Select(x => x.ToArray()).ToArray().To2DArray();
        }
        
        public static T[,] To2DArray<T>(this T[][] array)
        {
            //TODO: assertions
            return Array2D.Create(new Dimensions2D(array.Length, array[0].Length), i => array[i.Row][i.Column]);
        }
        
        public static T Get<T>(this T[,] array, Index2D index)
        {
            return array[index.Row, index.Column];
        }

        public static void Set<T>(this T[,] array, Index2D index, T value)
        {
            array[index.Row, index.Column] = value;
        }

        public static Dimensions2D GetDimensions<T>(this T[,] array)
        {
            return new Dimensions2D(array.GetRowsCount(), array.GetColumnsCount());
        }
        
        public static int GetRowsCount<T>(this T[,] array)
        {
            return array.GetLength(0);
        }

        public static int GetColumnsCount<T>(this T[,] array)
        {
            return array.GetLength(1);
        }

        public static TResult[,] Map<T, TResult>(this T[,] array, Func<T, Index2D, TResult> selector)
        {
            return Array2D.Create(array.GetDimensions(), i => selector(array.Get(i), i));
        }

        public static IEnumerable<T> Enumerate<T>(this T[,] array)
        {
            return array.EnumerateIndex().Select(array.Get);
        }
        
        public static IEnumerable<(T element, Index2D index)> EnumerateWithIndex<T>(this T[,] array)
        {
            return array.EnumerateIndex().Select(i => (array.Get(i), i));
        }
        
        public static IEnumerable<Index2D> EnumerateIndex<T>(this T[,] array)
        {
            var d = array.GetDimensions();
            
            for (var row = 0; row < d.Rows; row++)
            for (var column = 0; column < d.Columns; column++)
            {
                yield return new Index2D(row, column);
            }
        }
        
        public static IEnumerable<IEnumerable<T>> EnumerateRows<T>(this T[,] array)
        {
            for (var row = 0; row < array.GetDimensions().Rows; row++)
            {
                yield return EnumerateRow(row);
            }

            IEnumerable<T> EnumerateRow(int row)
            {
                for (var column = 0; column < array.GetDimensions().Columns; column++)
                {
                    yield return array[row, column];
                }
            }
        }

        public static Neighbourhood2d<T> GetNeighbours<T>(this T[,] array, Index2D i)
            where T: struct
        {
            return new Neighbourhood2d<T>(Neighbours().ToArray());
            
            IEnumerable<T?> Neighbours()
            {
                yield return array.TryGet(i.Left());
                yield return array.TryGet(i.Up());
                yield return array.TryGet(i.Right());
                yield return array.TryGet(i.Down());
            }
        }

        public static bool Is<T>(this T[,] array, Index2D i, Func<T, bool> condition, bool defaultValue = false) 
            where T : struct
        {
            return array.TryGet(i).Select(condition).GetValueOrDefault(defaultValue);
        }
        
        public static T? TryGet<T>(this T[,] array, Index2D i)
            where T: struct
        {
            if (!i.Fits(array))
                return default(T?);

            return array.Get(i);
        }
    }

    public struct Neighbourhood2d<T>
        where T: struct
    {
        public T? Left  => Neighbours[0]; 
        public T? Up    => Neighbours[1]; 
        public T? Right => Neighbours[2]; 
        public T? Down  => Neighbours[3]; 
        public readonly T?[] Neighbours;//TODO: use read-only interface

        public Neighbourhood2d(T?[] neighbours)
        {
            Neighbours = neighbours;
        }
    }

    public struct ElementAndIndex2D<T>
    {
        public readonly T Element;
        public readonly Index2D Index;

        public ElementAndIndex2D(T element, Index2D index)
        {
            Element = element;
            Index = index;
        }
    }

    [Serializable]
    public struct Index2D: IEquatable<Index2D>
    {
        public readonly int Row;
        public readonly int Column;

        public Index2D(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Index2D Up()
        {
            return new Index2D(Row - 1, Column);
        }

        public Index2D Down()
        {
            return new Index2D(Row + 1, Column);
        }

        public Index2D Left()
        {
            return new Index2D(Row, Column - 1);
        }

        public Index2D Right()
        {
            return new Index2D(Row, Column + 1);
        }


        public Index2D FitTo<T>(T[,] array)
        {
            var d = array.GetDimensions();
            
            var r = Row < 0       ? 0
                  : Row >= d.Rows ? d.Rows - 1
                  :                 Row;
            
            var c = Column < 0          ? 0
                  : Column >= d.Columns ? d.Columns - 1
                  :                       Column;
            
            return new Index2D(r, c);
        }

        public bool Fits<T>(T[,] array)
        {
            var d = array.GetDimensions();
            
            return Column >= 0 
                && Column < d.Columns 
                && Row >= 0 
                && Row < d.Rows;
        }
        
        public static bool operator ==(Index2D i1, Index2D i2) => i1.Equals(i2);
        public static bool operator !=(Index2D i1, Index2D i2) => !(i1 == i2);
        public bool Equals(Index2D other) => Row == other.Row && Column == other.Column;
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (obj is Index2D && Equals((Index2D) obj));
        public override int GetHashCode()
        {
            unchecked
            {
                return (Row * 397) ^ Column;
            }
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
        
        public static implicit operator Index2D(ValueTuple<int, int> v)
        {
            return new Index2D(v.Item1, v.Item2);
        }
    }

    public struct Dimensions2D: IEquatable<Dimensions2D>
    {
        public readonly int Rows;
        public readonly int Columns;

        public Dimensions2D(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        public bool Equals(Dimensions2D other) => Rows == other.Rows && Columns == other.Columns;
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (obj is Dimensions2D && Equals((Dimensions2D) obj));
        public override int GetHashCode()
        {
            unchecked
            {
                return (Rows * 397) ^ Columns;
            }
        }
    }
    
    public enum Direction2D
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3,
    }
}