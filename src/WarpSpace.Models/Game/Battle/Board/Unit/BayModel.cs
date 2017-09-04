namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class BayModel
    {
        public readonly UnitModel Owner;

        public BayModel(int slots_count, UnitModel owner)
        {
            Owner = owner;
            _slots = new LocationModel[slots_count];
        }

        public bool Can_Accept(UnitModel unit)
        {
            foreach (var slot in _slots) //Any is empty
            {
                if (slot.Is_Empty())
                    return true;
            }
            return false;
        }
        
        private readonly LocationModel[] _slots;
    }
}