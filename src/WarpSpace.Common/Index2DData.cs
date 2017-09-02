using System;
using Lanski.Structures;

namespace WarpSpace.Common
{
    [Serializable]
    public struct Index2DData
    {
        public int Row;
        public int Column;
            
        public Index2D ToIndex2D() => new Index2D(Row, Column);
    
        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}