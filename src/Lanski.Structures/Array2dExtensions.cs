using System;
using System.Collections.Generic;
using System.Linq;
using Lanski.Maths;

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
        
        /// <summary>
        /// Fit index to the nearest element 
        /// </summary>
        public static T GetFit<T>(this T[,] array, Index2D index)
        {
            return array.Get(index.FitTo(array.GetDimensions()));
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

        public static TResult[,] Map<T, TResult>(this T[,] array, Func<T, TResult> selector)
        {
            return Array2D.Create(array.GetDimensions(), i => selector(array.Get(i)));
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

        /// <summary>
        /// Gets neighbourhod of the element at a given index, replacing out of range elements with the closest non empty ones. 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="i">Must be within the array</param>
        public static FullNeighbourhood2D<T> GetFitNeighbours<T>(this T[,] array, Index2D i)
        {
            return FullNeighbourhood2D.Create(
                leftUp:    array.GetFit(i.Left()  .Up()  ),
                up:        array.GetFit(i         .Up()  ),
                rightUp:   array.GetFit(i.Right() .Up()  ),
                left:      array.GetFit(i.Left()         ),
                center:    array.GetFit(i                ),
                right:     array.GetFit(i.Right()        ),
                leftDown:  array.GetFit(i.Left()  .Down()),
                down:      array.GetFit(i         .Down()),
                rightDown: array.GetFit(i.Right() .Down()));
        }

        public static AdjacentRef<T> GetAdjacent<T>(this T[,] array, Index2D i)
            where T: class
        {
            return new AdjacentRef<T>(Neighbours().ToArray());
            
            IEnumerable<T> Neighbours()
            {
                yield return array.TryGetRef(i.Left());
                yield return array.TryGetRef(i.Up());
                yield return array.TryGetRef(i.Right());
                yield return array.TryGetRef(i.Down());
            }
        }
        
        public static AdjacentNeighbourhood2D<T> GetAdjacentNeighbours<T>(this T[,] array, Index2D i)
            where T: struct
        {
            return new AdjacentNeighbourhood2D<T>(Neighbours().ToArray());
            
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

        public static T TryGetRef<T>(this T[,] array, Index2D i)
            where T: class
        {
            return i.Fits(array) ? array.Get(i) 
                                 : default(T);
        }
        
        public static T? TryGet<T>(this T[,] array, Index2D i)
            where T: struct
        {
            return i.Fits(array) ? array.Get(i) 
                                 : default(T?);
        }
    }

    public struct FullNeighbourhood2D
    {
        public static FullNeighbourhood2D<T> Create<T>(T center, T left, T up, T right, T down, T leftUp, T rightUp, T rightDown, T leftDown)
        {
            var n = new T[3,3];
            
            n[0, 0] = leftUp;
            n[1, 0] = up;
            n[2, 0] = rightUp;
            n[0, 1] = left;
            n[1, 1] = center;
            n[2, 1] = right;
            n[0, 2] = leftDown;
            n[1, 2] = down;
            n[2, 2] = rightDown;
            
            return new FullNeighbourhood2D<T>(n);
        }
    }
    
    public struct FullNeighbourhood2D<T>
    {
        public readonly T[,] Elements;

        public T Center    => Elements[1, 1];
        public T Left      => Elements[0, 1];
        public T LeftUp    => Elements[0, 0];
        public T Up        => Elements[1, 0];
        public T RightUp   => Elements[2, 0];
        public T Right     => Elements[2, 1];
        public T RightDown => Elements[2, 2];
        public T Down      => Elements[1, 2];
        public T LeftDown  => Elements[0, 2];

        public FullNeighbourhood2D(T[,] elements)
        {
            Elements = elements;
        }

        public FullNeighbourhood2D<TResult> Map<TResult>(Func<T, TResult> selector)
        {
            return new FullNeighbourhood2D<TResult>(Elements.Map(selector));
        }
    }

    public struct AdjacentNeighbourhood2D<T>
        where T: struct
    {
        public T? Left  => Neighbours[0]; 
        public T? Up    => Neighbours[1]; 
        public T? Right => Neighbours[2]; 
        public T? Down  => Neighbours[3]; 
        public readonly T?[] Neighbours;//TODO: use read-only interface

        public AdjacentNeighbourhood2D(T?[] neighbours)
        {
            Neighbours = neighbours;
        }
    }
    
    public struct AdjacentRef<T>
    {
        private readonly T[] _all;
        public T[] All => _all;

        public AdjacentRef(T[] all)
        {
            _all = all;
        }

        public T this[Direction2D d]
        {
            get
            {
                switch (d)
                {
                    case Direction2D.Left:
                        return _all[0];
                    case Direction2D.Up:
                        return _all[1];
                    case Direction2D.Right:
                        return _all[2];
                    case Direction2D.Down:
                        return _all[3];
                    default:
                        throw new ArgumentOutOfRangeException(nameof(d), d, null);
                }
            }
        }

        public AdjacentRef<TResult> Map<TResult>(Func<T, TResult> selector)
        {
            return new AdjacentRef<TResult>(All.Select(selector).ToArray());
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


        public Index2D FitTo(Dimensions2D d)
        {
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
        
        public static Index2D operator +(Index2D i, Direction2D d)
        {
            switch (d)
            {
                case Direction2D.Left:
                    return new Index2D(i.Row    , i.Column - 1);
                case Direction2D.Up:
                    return new Index2D(i.Row - 1, i.Column    );
                case Direction2D.Right:
                    return new Index2D(i.Row    , i.Column + 1);
                case Direction2D.Down:
                    return new Index2D(i.Row + 1, i.Column    );
                default:
                    throw new ArgumentOutOfRangeException(nameof(d), d, null);
            }
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

        public bool IsAdjacentTo(Index2D other)
        {
            var dr = Mathe.Abs(Row - other.Row);
            var dc = Mathe.Abs(Column - other.Column);

            return dr == 0 && dc == 1 || dr == 1 && dc == 0;
        }

        public Direction2D DirectionTo(Index2D other)
        {
            var dr = other.Row - Row;
            var dc = other.Column - Column;
            
            if (dr < 0)
                return Direction2D.Up;
            
            if (dr > 0)
                return Direction2D.Down;

            if (dc < 0) 
                return Direction2D.Left;
            
            return Direction2D.Right;
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
    
    public enum RotationDirection2D
    {
        Clockwise,
        Counterclockwise
    }
}