using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MBay
    {
        public readonly MUnit Owner;
        public int Size => _slots.Count;
        public Possible<MLocation> this[int i] => i < Size ? _slots[i] : Possible.Empty<MLocation>();

        public MBay(int size, MUnit owner)
        {
            Owner = owner;
            _slots = Create_Slots(size);
        }
        
        public void Must_Have_a_Slot(MLocation bay_slot) => _slots.Contains(bay_slot).Must_Be_True();

        public void Must_Contain(MUnit bay_unit) => _slots.Any(x => x.Has_a_Unit(out var slot_unit) && slot_unit == bay_unit).Must_Be_True(); 

        public bool Can_Accept(MUnit unit)
        {
            foreach (var slot in _slots) //Any is empty
            {
                if (slot.Is_Empty())
                    return true;
            }
            return false;
        }
        
        private List<MLocation> Create_Slots(int size) => Enumerable.Range(0, size).Select(x => MLocation.Create.From(this)).ToList();

        private readonly IReadOnlyList<MLocation> _slots;

    }
}