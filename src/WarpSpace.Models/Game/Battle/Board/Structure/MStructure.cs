using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public class MStructure
    {
        internal MStructure(DStructure description, MTile location)
        {
            its_description = description;
            its_location = location;
        }

        public MTile s_Location => its_location;
        public DStructure s_Description => its_description;
        
        public bool is_an_Exit() => its_description.is_an_Exit();
        public bool is_a_Debris() => its_description.is_a_Debris();
        public DStructure.Debris must_be_a_Debris() => its_description.must_be_a_Debris();
        
        public void Destructs() => its_location.Removes_its_Structure();
        
        private readonly MTile its_location;
        private readonly DStructure its_description;
    }
}