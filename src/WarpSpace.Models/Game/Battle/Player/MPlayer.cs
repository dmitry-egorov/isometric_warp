using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;
using static WarpSpace.Models.Descriptions.Faction;

namespace WarpSpace.Models.Game.Battle.Player
{
    public class MPlayer
    {
        public MPlayer(SignalGuard the_signal_guard)
        {
            this.the_signal_guard = the_signal_guard;
            
            its_selections_cell = new GuardedCell<Possible<Selection>>(Possible.Empty<Selection>(), the_signal_guard);
            
            its_selected_units_cell            = it_generates_its_selected_units_cell();
            its_selected_unit_movements_stream = it_generates_its_selected_unit_movements_stream();
            its_selected_unit_changes_stream   = it_generates_its_selected_unit_changes_stream();

            it_wires_the_selected_unit_destructions();
            it_wires_the_selected_unit_docks();
            it_wires_the_selected_action_becomes_unavailable();

            ICell<Possible<MUnit>>      it_generates_its_selected_units_cell()            => its_selections_cell.Select(x => x.Select(s => s.s_Unit));
            IStream<SelectedUnitChange> it_generates_its_selected_unit_changes_stream()   => its_selected_units_cell.IncludePrevious().Select(p => new SelectedUnitChange { s_Previous = p.previous, s_Current = p.current });
            IStream<Movement>           it_generates_its_selected_unit_movements_stream() => its_selected_units_cell.SelectMany(pu => pu.Select_Stream_Or_Empty(u => u.s_Movements_Stream));
            
            void it_wires_the_selected_unit_destructions() => 
                its_selected_units_cell
                .SelectMany(pu => pu.Select(the_unit => the_unit.s_Destruction_Signal).Value_Or_Empty())
                .Subscribe(_ => its_selection_becomes_empty())
            ;

            void it_wires_the_selected_unit_docks() => 
                its_selected_unit_movements_stream
                .Select(m => m.Destination.as_a_possible_Bay().Select(the_bay => the_bay.s_Owner))
                .SkipEmpty()
                .Subscribe(the_bays_owner => its_selected_unit_becomes(the_bays_owner))
            ;

            void it_wires_the_selected_action_becomes_unavailable() => 
                its_selections_cell
                .SelectMany(ps => ps.SelectMany(the_selection => the_selection.s_Possible_Action).Select_Stream_Or_Empty(action => action.s_Availability_Cell))
                .Where(the_selected_action_is_available => !the_selected_action_is_available)
                .Subscribe(w => its_selected_action_becomes_empty())
            ;
        }
        
        public ICell<Possible<MUnit>> s_Selected_Units_Cell => its_selected_units_cell;
        public ICell<Possible<Selection>> s_Selections_Cell => its_selections_cell;

        public IStream<SelectedUnitChange> s_Selected_Unit_Changes_Stream => its_selected_unit_changes_stream;
        public IStream<Movement> s_Selected_Unit_Movements_Stream => its_selected_unit_movements_stream;
        public Possible<MUnit> s_Possible_Selected_Unit => its_possible_selected_unit;

        public bool s_Selected_Unit_is_At(MTile the_tile) => it_has_a_unit_selected(out var the_selected_unit) && the_selected_unit.is_At(the_tile);
        public bool has_a_Unit_Selected(out MUnit the_selected_unit) => it_has_a_unit_selected(out the_selected_unit);
        public MUnit must_have_a_Unit_Selected() => it_must_have_a_unit_selected();

        public bool has_a_Command_At(MTile the_tile, out UnitCommand the_unit_command) => its_possible_command_at(the_tile).has_a_Value(out the_unit_command);

        public void Resets_the_Selection() => its_selection_becomes_empty();

        public void Executes_a_Command_At(MTile the_tile)
        {
            if (this.has_a_Command_At(the_tile, out var the_command))
            {
                the_command.Executes_With(the_signal_guard);
            }
            else if (it_can_select_a_unit_at(the_tile, out var the_target_unit))
            {
                its_selected_unit_becomes(the_target_unit);
            }
        }

        public void Toggles_the_Selected_Action_With(DUnitAction the_action_desc)
        {
            var the_selection = its_possible_selection.must_have_a_Value();
            
            if (the_selection.s_action_is(the_action_desc))
            {
                its_selected_action_becomes_empty();
            }
            else
            {
                var the_selected_unit = the_selection.s_Unit;
                var the_action = the_selected_unit.s_possible_Action_For(the_action_desc).must_have_a_Value();
                its_selected_action_becomes(the_action);
            }
        }
        
        private Possible<Selection> its_possible_selection => its_selections_cell.s_Value;
        private Possible<MUnit> its_possible_selected_unit => its_selected_units_cell.s_Value;

        private MUnit it_must_have_a_unit_selected() => its_possible_selected_unit.must_have_a_Value();
        
        private bool it_has_a_selection(out Selection the_selection) => its_possible_selection.has_a_Value(out the_selection);
        private bool it_has_a_unit_selected(out MUnit the_selected_unit) => its_possible_selected_unit.has_a_Value(out the_selected_unit);

        private bool it_can_select_a_unit_at(MTile the_tile, out MUnit the_target_unit) => the_tile.has_a_Unit(out the_target_unit) && it_can_select(the_target_unit);
        private bool it_can_select(MUnit the_unit) => the_unit.Belongs_To(the_Player_Faction);

        private void its_selected_unit_becomes(MUnit unit) => its_selections_cell.s_Value = new Selection(unit, Possible.Empty<MUnitAction>());
        private void its_selected_action_becomes_empty() => its_selected_action_becomes(Possible.Empty<MUnitAction>());
        private void its_selected_action_becomes(Possible<MUnitAction> the_action) => its_selection_becomes(new Selection(must_have_a_Unit_Selected(), the_action));
        private void its_selection_becomes_empty() => its_selection_becomes(Possible.Empty<Selection>());
        private void its_selection_becomes(Possible<Selection> the_new_selection) => its_selections_cell.s_Value = the_new_selection;
        private Possible<UnitCommand> its_possible_command_at(MTile the_tile) => 
            it_has_a_selection(out var the_selection) 
                ? the_selection.s_possible_command_at(the_tile) 
                : Possible.Empty<UnitCommand>()
        ;

        private readonly GuardedCell<Possible<Selection>> its_selections_cell;
        private readonly SignalGuard the_signal_guard;

        private readonly ICell<Possible<MUnit>> its_selected_units_cell;
        private readonly IStream<SelectedUnitChange> its_selected_unit_changes_stream;
        private readonly IStream<Movement> its_selected_unit_movements_stream;

        public struct Selection: IEquatable<Selection> //Maybe add a class, move some mutation methods there?
        {
            public Selection(MUnit unit) : this(unit, Possible.Empty<MUnitAction>()) {}
            public Selection(MUnit unit, Possible<MUnitAction> the_possible_action): this()
            {
                its_unit = unit;
                its_possible_action = the_possible_action;
            }
            
            public MUnit s_Unit => its_unit;
            public Possible<MUnitAction> s_Possible_Action => its_possible_action;

            public bool s_action_is(DUnitAction the_requested_action_desc) => 
                it_has_an_action(out var the_selected_action) && 
                the_selected_action.@is(the_requested_action_desc);

            public Possible<UnitCommand> s_possible_command_at(MTile the_tile) => 
                has_a_special_command_At(the_tile, out var the_command) 
                    ? the_command 
                    : its_possible_regular_action(the_tile)
            ;

            public bool Equals(Selection other) => Equals(s_Unit, other.s_Unit) && s_Possible_Action.Equals(other.s_Possible_Action);

            private bool has_a_special_command_At(MTile the_tile, out UnitCommand the_command) =>
                semantic_resets(out the_command) &&
                it_has_an_action(out var the_action) && 
                the_action.s_possible_Command_at(the_tile).has_a_Value(out the_command)
            ;

            private bool it_has_an_action(out MUnitAction the_action) => s_Possible_Action.has_a_Value(out the_action);
            private Possible<UnitCommand> its_possible_regular_action(MTile the_tile) => its_unit.s_Regular_Command_At(the_tile);

            private readonly MUnit its_unit;
            private readonly Possible<MUnitAction> its_possible_action;
        }

        public struct SelectedUnitChange { public Possible<MUnit> s_Current; public Possible<MUnit> s_Previous; }
    }
}