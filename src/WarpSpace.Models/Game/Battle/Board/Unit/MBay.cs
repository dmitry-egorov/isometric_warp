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
            its_slots = it_creates_its_slots(size, the_signal_guard);
            its_can_deploy_cells = it_creates_its_can_deploy_cells();
            its_docked_unit_changed = it_creates_docked_unit_changed_stream();
        }

        public MUnit s_Owner => its_owner;
        public int Size => its_slots.Count;
        public MUnitLocation this[int i] => its_slots[i];
        public IStream<BaysDockedUnitChanged> s_Docked_Unit_Changed => its_docked_unit_changed;

        public ICell<bool> s_can_Deploy_Cell(int the_bay_slot_index) => its_can_deploy_cells[the_bay_slot_index];
        public bool has_an_Empty_Slot(out MUnitLocation the_empty_slot) => its_slots.SFirst(slot => slot.is_Empty()).has_a_Value(out the_empty_slot);
        public Possible<MUnit> s_possible_unit_at(int the_bay_slot_index) => its_slots[the_bay_slot_index].s_Possible_Unit;
        
        private IReadOnlyList<MUnitLocation> it_creates_its_slots(int size, SignalGuard the_signal_guard) => Enumerable.Range(0, size).Select(x => new MUnitLocation(this, the_signal_guard)).ToList();
        private ICell<bool>[] it_creates_its_can_deploy_cells() => 
            its_slots
                .Select(location => location.s_Possible_Units_Cell)
                .Select(x => x.SelectMany(pu => pu.Select(u => u.s_can_Move_Cell).Cell_Or_Single_Default()))
                .ToArray()
        ;

        private IStream<BaysDockedUnitChanged> it_creates_docked_unit_changed_stream() => 
            its_slots
                .Select((slot, index) => slot.s_Possible_Units_Cell.Select(pu => new BaysDockedUnitChanged(index, pu)))
                .Merge()
        ;

        private readonly MUnit its_owner;
        private readonly IReadOnlyList<MUnitLocation> its_slots;
        private readonly IReadOnlyList<ICell<bool>> its_can_deploy_cells;
        private readonly IStream<BaysDockedUnitChanged> its_docked_unit_changed;
    }
    
    public struct BaysDockedUnitChanged
    {
        public readonly int Slot_Index;
        public readonly Possible<MUnit> New_Unit;

        public BaysDockedUnitChanged(int the_slot_index, Possible<MUnit> the_new_unit)
        {
            Slot_Index = the_slot_index;
            New_Unit = the_new_unit;
        }
    }
}