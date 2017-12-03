using System.Collections.Generic;
using System.Linq;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Game.Battle.Board.Tile;
using static Lanski.Structures.Flow;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MBay
    {
        public static Possible<MBay> From(MUnit the_owner, SignalGuard the_signal_guard) => 
            the_owner.s_Bay_Size().as_a_possible_positive().Select(the_bay_size => new MBay(the_bay_size, the_owner, the_signal_guard))
        ;

        private MBay(int size, MUnit the_owner, SignalGuard the_signal_guard)
        {
            (size > 0).Otherwise_Throw("Bay size must be greater than 0");
            
            its_owner = the_owner;
            its_slots = it_creates_its_slots(size, the_signal_guard);
            its_can_deploy_cells = it_creates_its_can_deploy_cells();
        }

        public MUnit s_Owner => its_owner;
        public int Size => its_slots.Count;
        public MBaySlot this[int i] => its_slots[i];

        public ICell<bool> s_can_Deploy_Cell(int the_bay_slot_index) => its_can_deploy_cells[the_bay_slot_index];
        public bool has_an_Empty_Slot(out MBaySlot the_empty_slot) => its_slots.SFirst(slot => slot.is_Empty()).has_a_Value(out the_empty_slot);
        public Possible<MUnit> s_possible_Unit_At(int the_bay_slot_index) => its_slots[the_bay_slot_index].s_Possible_Unit;
        
        private IReadOnlyList<MBaySlot> it_creates_its_slots(int size, SignalGuard the_signal_guard) => Enumerable.Range(0, size).Select(index => new MBaySlot(index, this, the_signal_guard)).ToList();
        private ICell<bool>[] it_creates_its_can_deploy_cells() => 
            its_slots
                .Select(location => location.s_Possible_Units_Cell)
                .Select(x => x.SelectMany(pu => pu.Select(u => MUnitReadExtensions.s_Cell_of_can_Move(u)).Cell_Or_Single_Default()))
                .ToArray()
        ;

        private readonly MUnit its_owner;
        private readonly IReadOnlyList<MBaySlot> its_slots;
        private readonly IReadOnlyList<ICell<bool>> its_can_deploy_cells;
    }

    public static class MBayReadExtensions
    {
        public static bool has_an_Empty_Slot(this Possible<MBay> the_possible_bay, out MBaySlot the_bay_slot) =>
            default_as(out the_bay_slot) && 
            the_possible_bay.has_a_Value(out var the_bay) &&
            the_bay.has_an_Empty_Slot(out the_bay_slot)
        ;

        public static Possible<MUnit> s_possible_Unit_At(this Possible<MBay> the_possible_bay, int the_bay_slot_index) =>
            the_possible_bay.has_a_Value(out var the_bay) ? the_bay.s_possible_Unit_At(the_bay_slot_index) : Possible.Empty<MUnit>()
        ;
        
        public static bool has_a_Docked_Unit_at(this Possible<MBay> the_possible_bay, int the_bay_slot_index, out MUnit the_bay_unit) => the_possible_bay.s_possible_Unit_At(the_bay_slot_index).has_a_Value(out the_bay_unit);
        public static bool can_Deploy_a_Unit_At(this Possible<MBay> the_possible_bay, int the_bay_slot_index, MTile the_tile, out MUnit the_docked_unit) => the_possible_bay.has_a_Docked_Unit_at(the_bay_slot_index, out the_docked_unit) && the_docked_unit.can_Move_To(the_tile);

        public static ICell<bool> s_can_Deploy_Cell(this Possible<MBay> the_possible_bay, int the_bay_slot_index) => the_possible_bay.has_a_Value(out var the_bay) ? the_bay.s_can_Deploy_Cell(the_bay_slot_index) : Cell.From(false);
    }
}