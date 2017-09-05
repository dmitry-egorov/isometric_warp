using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class BayModel
    {
        public readonly UnitModel Owner;
        public int Size => _slots.Count;
        public LocationModel this[int i] => _slots[i];

        public BayModel(int size, UnitModel owner)
        {
            Owner = owner;
            _slots = Create_Slots(size);
        }

        private List<LocationModel> Create_Slots(int size)
        {
            return Enumerable.Range(0, size).Select(x => new LocationModel(this, Slot.Empty<UnitModel>())).ToList();
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
        
        private readonly IReadOnlyList<LocationModel> _slots;

    }
}