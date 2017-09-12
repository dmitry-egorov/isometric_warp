using System;
using System.Linq;
using Lanski.Structures;

namespace WarpSpace.Common.MapParsing
{
    public static class MapParsingExtensions
    {
        public static char[,] ToArray2D(this string data)
        {
            return new String(data.Where(x => x != '\r').ToArray())
                .Split('\n')
                .Select(row => row.Split(' ')
                    .Select(s => s[0]))
                .To2DArray();
        }
    }
}