using System;
using System.Collections.Generic;
using System.Linq;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MBay
    {
        public static Possible<MBay> From(MUnit the_owner, SignalGuard the_signal_guard) => 
            the_owner.s_Bay_Size.as_a_possible_positive()
                .Select(the_bay_size => new MBay(the_bay_size, the_owner, the_signal_guard))
        ;

        private MBay(int size, MUnit owner, SignalGuard the_signal_guard)
        {
            (size > 0).Otherwise_Throw("Bay size must be greater than 0");
            
            its_owner = owner;
            its_slots = it_creates_its_slots();
            its_can_deploy_cells = it_creates_its_can_deploy_cells();
            
            IReadOnlyList<MUnitLocation> it_creates_its_slots() => Enumerable.Range(0, size).Select(x => new MUnitLocation(this, the_signal_guard)).ToList();
            ICell<bool>[] it_creates_its_can_deploy_cells() => 
                its_slots
                    .Select(location => location.s_Possible_Units_Cell)
                    .Select(x => x.SelectMany(pu => pu.Select(u => u.s_can_Move_Cell).Cell_Or_Single_Default()))
                    .ToArray()
            ;
        }

        public MUnit s_Owner => its_owner;
        public int Size => its_slots.Count;
        public Possible<MUnitLocation> this[int i] => i < Size ? its_slots[i] : Possible.Empty<MUnitLocation>();

        public ICell<bool> s_can_Deploy_Cell(int the_bay_slot_index) => its_can_deploy_cells[the_bay_slot_index];

        public bool has_an_Empty_Slot(out MUnitLocation the_empty_slot) => its_slots.SFirst(slot => slot.is_Empty()).has_a_Value(out the_empty_slot);

        public Possible<MUnit> s_possible_unit_at(int the_bay_slot_index) => its_slots[the_bay_slot_index].s_Possible_Unit;
        

        private readonly MUnit its_owner;
        private readonly IReadOnlyList<MUnitLocation> its_slots;
        private readonly IReadOnlyList<ICell<bool>> its_can_deploy_cells;


    }
}