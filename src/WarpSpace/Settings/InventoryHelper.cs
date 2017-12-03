using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    public static class InventoryHelper
    {
        public static DStuff Possible_Stuff_From(int the_matter) => the_matter == 0 ? DStuff.Empty() : new DStuff(the_matter);
    }
}