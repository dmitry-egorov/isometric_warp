using System;
using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MBay
    {
        public readonly MUnit s_Owner;
        public int Size => _slots.Count;
        public Possible<MLocation> this[int i] => i < Size ? _slots[i] : Possible.Empty<MLocation>();

        public static Possible<MBay> From(MUnit the_owner) => 
            the_owner.s_Type.Requires_a_Bay(out var the_bay_s_size)
            ? new MBay(the_bay_s_size, the_owner)
            : Possible.Empty<MBay>()
        ;

        public MBay(int size, MUnit owner)
        {
            (size > 0).Otherwise_Throw("Bay size must be greater than 0");
            
            s_Owner = owner;
            _slots = Create_Slots(size);
        }
        
        public void must_Contain(MUnit the_unit)
        {
            var iterator = _slots.SIterate();
            while (iterator.has_a_Value(out var the_slot))
            {
                if (the_slot.has_a_Unit(out var the_slot_unit) && the_slot_unit == the_unit)
                    return;
            }
            
            throw new InvalidOperationException("Doesn't contain the unit");
        }

        public bool has_an_Empty_Slot(out MLocation the_empty_slot) => _slots.SFirst(slot => slot.is_Empty()).has_a_Value(out the_empty_slot);

        private List<MLocation> Create_Slots(int size) => Enumerable.Range(0, size).Select(x => new MLocation(this)).ToList();

        private readonly IReadOnlyList<MLocation> _slots;

    }
}