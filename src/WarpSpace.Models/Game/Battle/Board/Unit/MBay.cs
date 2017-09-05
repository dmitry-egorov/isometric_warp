using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MBay
    {
        public readonly MUnit Owner;
        public int Size => _slots.Count;
        public MLocation this[int i] => _slots[i];

        public MBay(int size, MUnit owner)
        {
            Owner = owner;
            _slots = Create_Slots(size);
        }

        private List<MLocation> Create_Slots(int size)
        {
            return Enumerable.Range(0, size).Select(x => new MLocation(this, Possible.Empty<MUnit>())).ToList();
        }

        public bool Can_Accept(MUnit unit)
        {
            foreach (var slot in _slots) //Any is empty
            {
                if (slot.Is_Empty())
                    return true;
            }
            return false;
        }
        
        private readonly IReadOnlyList<MLocation> _slots;

    }
}