using System;

namespace WarpSpace.Models.Descriptions
{
    public static class DStuffExtensions
    {
        public static DStuff and(this DStuff pi1, DStuff pi2) => pi1 + pi2;
    }
    
    public struct DStuff: IEquatable<DStuff>
    {
        public readonly int s_Amount_of_Matter;

        public DStuff(int the_amount_of_matter)
        {
            s_Amount_of_Matter = the_amount_of_matter;
        }

        public static DStuff operator &(DStuff i1, DStuff i2) => i1 + i2;
        public static DStuff operator +(DStuff i1, DStuff i2) => new DStuff(i1.s_Amount_of_Matter + i2.s_Amount_of_Matter);
        
        public static DStuff Empty() => new DStuff(0);

        public bool Equals(DStuff other) => s_Amount_of_Matter == other.s_Amount_of_Matter;
    }
}