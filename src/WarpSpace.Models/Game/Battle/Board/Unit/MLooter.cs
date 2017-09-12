using WarpSpace.Models.Game.Battle.Board.Structure;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MLooter
    {
        public MLooter(MUnit the_owner)
        {
            its_owner = the_owner;
        }

        internal void Loots(MStructure the_structure)
        {
            var the_debris = the_structure.must_be_a_Debris();
            its_owner.Takes(the_debris.s_Loot);
            the_structure.Destructs();
        }

        private readonly MUnit its_owner;
    }
}