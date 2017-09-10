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
        public static Possible<MBay> From(MUnit the_owner) => 
            the_owner.s_Type.s_bay_size().as_a_possible_positive()
                .Select(the_bay_size => new MBay(the_bay_size, the_owner))
        ;

        public MBay(int size, MUnit owner)
        {
            (size > 0).Otherwise_Throw("Bay size must be greater than 0");
            
            its_owner = owner;
            its_slots = it_creates_its_slots(size);
        }
        
        public MUnit s_Owner => its_owner;
        public int Size => its_slots.Count;
        public Possible<MLocation> this[int i] => i < Size ? its_slots[i] : Possible.Empty<MLocation>();
        
        public void must_Contain(MUnit the_unit)
        {
            var iterator = its_slots.SIterate();
            while (iterator.has_a_Value(out var the_slot))
            {
                if (the_slot.has_a_Unit(out var the_slot_unit) && the_slot_unit == the_unit)
                    return;
            }
            
            throw new InvalidOperationException("Doesn't contain the unit");
        }

        public bool has_an_Empty_Slot(out MLocation the_empty_slot) => its_slots.SFirst(slot => slot.is_Empty()).has_a_Value(out the_empty_slot);

        public Possible<MUnit> s_possible_unit_at(int the_bay_slot_index) => its_slots[the_bay_slot_index].s_Possible_Unit;
        
        private List<MLocation> it_creates_its_slots(int size) => Enumerable.Range(0, size).Select(x => new MLocation(this)).ToList();

        private readonly MUnit its_owner;
        private readonly IReadOnlyList<MLocation> its_slots;

    }
}