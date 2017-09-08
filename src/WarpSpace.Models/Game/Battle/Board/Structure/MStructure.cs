using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public class MStructure
    {
        public readonly StructureDescription Description;
        public readonly MTile Location;

        internal MStructure(StructureDescription description, MTile location)
        {
            Description = description;
            Location = location;
        }

        public void Destructs()
        {
            Location.Reset_Structure();
        }
        
        public bool Is_an_Entrance() => Description.Is_An_Entrance();
        public bool is_an_Exit() => Description.Is_An_Exit();
        public bool is_a_Debris() => Description.Is_A_Debris();
        public bool is_a_Debris(out StructureDescription.Debris debris) => Description.Is_A_Debris(out debris);
        public StructureDescription.Debris must_be_a_Debris() => Description.Must_Be_a_Debris();
    }
}