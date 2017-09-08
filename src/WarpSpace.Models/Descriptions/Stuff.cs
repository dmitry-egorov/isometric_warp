using System;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public static class InventoryContentExyensions
    {
        public static Possible<Stuff> and(this Possible<Stuff> pi1, Possible<Stuff> pi2) => 
            pi1.has_a_Value(out var i1) 
                ? i1 + pi2 
                : pi2
        ;
    }
    
    public struct Stuff: IEquatable<Stuff>
    {
        public readonly int Matter;

        public Stuff(int matter)
        {
            Matter = matter;
        }

        public static Stuff operator &(Stuff i1, Stuff i2) => i1 + i2;
        public static Stuff operator +(Stuff i1, Stuff i2) => new Stuff(i1.Matter + i2.Matter);
        public static Stuff operator &(Stuff i1, Possible<Stuff> pi2) => i1 + pi2;
        public static Stuff operator +(Stuff i1, Possible<Stuff> pi2) => pi2.has_a_Value(out var i2) ? i1 + i2 : i1;

        public static Possible<Stuff> Initial_For(UnitType type)
        {
            switch (type)
            {
                case UnitType.a_Mothership:
                    return Possible.Empty<Stuff>();
                case UnitType.a_Tank:
                    return new Stuff(10);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public bool Equals(Stuff other) => Matter == other.Matter;
    }
}