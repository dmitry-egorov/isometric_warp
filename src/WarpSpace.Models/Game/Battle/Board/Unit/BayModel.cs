namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class BayModel
    {
        public readonly UnitModel Owner;
        public int Size => _slots.Length;
        public LocationModel this[int i] => _slots[i];

        public BayModel(int size, UnitModel owner)
        {
            Owner = owner;
            _slots = new LocationModel[size];
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