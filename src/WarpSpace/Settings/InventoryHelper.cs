using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    public static class InventoryHelper
    {
        public static Possible<DStuff> Possible_Stuff_From(int the_matter) => the_matter == 0 ? Possible.Empty<DStuff>() : new DStuff(the_matter);
    }
}