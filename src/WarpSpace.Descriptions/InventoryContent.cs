using System;
using Lanski.Structures;

namespace WarpSpace.Descriptions
{
    public struct InventoryContent
    {
        public readonly int Matter;

        public InventoryContent(int matter)
        {
            Matter = matter;
        }

        public static InventoryContent operator +(InventoryContent i1, InventoryContent i2) => new InventoryContent(i1.Matter + i2.Matter);
        public static InventoryContent? operator +(InventoryContent? pi1, InventoryContent? pi2)
        {
            var pi1_has_value = pi1.Has_a_Value(out var i1);
            var pi2_has_value = pi2.Has_a_Value(out var i2);

            if (pi1_has_value && pi2_has_value)
                return i1 + i2;

            if (pi1_has_value)
                return i1;

            if (pi2_has_value)
                return i2;

            return null;
        }

        public static InventoryContent? InitialFor(UnitType type)
        {
            switch (type)
            {
                case UnitType.Mothership:
                    return null;
                case UnitType.Tank:
                    return new InventoryContent(10);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}