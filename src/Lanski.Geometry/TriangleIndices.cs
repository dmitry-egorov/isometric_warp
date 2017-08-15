using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lanski.Geometry
{
    public static class TriangleIndicesExtensions
    {
        public static int[] ToIndicesArray(this IEnumerable<TriangleIndices> triangleIndices)
        {
            return triangleIndices.SelectMany(x => x).ToArray();
        }
    }
    
    public struct TriangleIndices: IEnumerable<int>
    {
        public int i0;
        public int i1;
        public int i2;

        public TriangleIndices(int i0, int i1, int i2)
        {
            this.i0 = i0;
            this.i1 = i1;
            this.i2 = i2;
        }


        public IEnumerator<int> GetEnumerator()
        {
            yield return i0;
            yield return i1;
            yield return i2;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}