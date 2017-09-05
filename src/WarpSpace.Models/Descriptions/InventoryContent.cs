using System;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public static class InventoryContentExyensions
    {
        public static Possible<InventoryContent> And(this Possible<InventoryContent> pi1, Possible<InventoryContent> pi2) => 
            pi1.Has_a_Value(out var i1) 
                ? i1 + pi2 
                : pi2
        ;
    }
    
    public struct InventoryContent
    {
        public readonly int Matter;

        public InventoryContent(int matter)
        {
            Matter = matter;
        }

        public static InventoryContent operator &(InventoryContent i1, InventoryContent i2) => i1 + i2;
        public static InventoryContent operator +(InventoryContent i1, InventoryContent i2) => new InventoryContent(i1.Matter + i2.Matter);
        public static InventoryContent operator &(InventoryContent i1, Possible<InventoryContent> pi2) => i1 + pi2;
        public static InventoryContent operator +(InventoryContent i1, Possible<InventoryContent> pi2) => pi2.Has_a_Value(out var i2) ? i1 + i2 : i1;

        public static Possible<InventoryContent> Initial_For(UnitType type)
        {
            switch (type)
            {
                case UnitType.Mothership:
                    return Possible.Empty<InventoryContent>();
                case UnitType.Tank:
                    return new InventoryContent(10);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}