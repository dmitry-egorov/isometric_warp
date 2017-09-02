using System.Linq;
using Lanski.Structures;

namespace WarpSpace.Common.MapParsing
{
    public static class MapParsingExtensions
    {
        public static char[,] ToArray2D(this string data)
        {
            return data.Split('\n')
                .Select(row => row.Split(' ')
                    .Select(s => s[0]))
                .To2DArray();
        }
    }
}