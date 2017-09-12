using System;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public static class InventoryContentExtensions
    {
        public static Possible<DStuff> and(this Possible<DStuff> pi1, Possible<DStuff> pi2) => 
            pi1.has_a_Value(out var i1) 
                ? i1 + pi2 
                : pi2
        ;
    }
    
    public struct DStuff: IEquatable<DStuff>
    {
        public readonly int Matter;

        public DStuff(int matter)
        {
            Matter = matter;
        }

        public static DStuff operator &(DStuff i1, DStuff i2) => i1 + i2;
        public static DStuff operator +(DStuff i1, DStuff i2) => new DStuff(i1.Matter + i2.Matter);
        public static DStuff operator &(DStuff i1, Possible<DStuff> pi2) => i1 + pi2;
        public static DStuff operator +(DStuff i1, Possible<DStuff> pi2) => pi2.has_a_Value(out var i2) ? i1 + i2 : i1;

        public bool Equals(DStuff other) => Matter == other.Matter;
    }
}