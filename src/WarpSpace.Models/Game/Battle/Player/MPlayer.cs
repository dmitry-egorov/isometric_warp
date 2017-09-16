using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;
using static WarpSpace.Models.Game.MFaction;

namespace WarpSpace.Models.Game.Battle.Player
{
    public class MPlayer
    {
        public MPlayer(MGame the_game, MFaction the_its_faction, SignalGuard the_signal_guard)
        {
            this.the_game = the_game;
            this.the_signal_guard = the_signal_guard;
            its_faction = the_its_faction;

            its_selections_cell = new GuardedCell<Possible<Selection>>(Possible.Empty<Selection>(), the_signal_guard);
            it_performed_an_action = new GuardedStream<TheVoid>(the_signal_guard);
            
            its_selected_units_cell = its_selections_cell.Select(x => x.Select(s => s.s_Unit));
        }

        public Possible<Selection> s_Selection => its_possible_selection;
        public ICell<Possible<MUnit>> s_Selected_Units_Cell => its_selected_units_cell;
        public ICell<Possible<Selection>> s_Selections_Cell => its_selections_cell;
        public IStream<TheVoid> Performed_an_Action => it_performed_an_action;

        public bool s_Selected_Unit_is_At(MTile the_tile) => it_has_a_unit_selected(out var the_selected_unit) && the_selected_unit.is_At(the_tile);
        public bool has_a_Command_At(MTile the_tile, out UnitCommand the_command) => it_has_a_command_at(the_tile, out the_command);
        public bool Owns(MUnit the_unit) => it_owns(the_unit);

        public void Executes_a_Command_At(MTile the_tile)
        {
            if (it_is_suspended)
                return;
            
            using (the_signal_guard.Holds_All_Events())
            {
                if (it_has_a_command_at(the_tile, out var the_command))
                {
                    the_command.Executes();
                }
                else if (it_owns_a_unit_at(the_tile, out var the_target_unit))
                {
                    its_selected_unit_becomes(the_target_unit);
                }
            }
            
            //TODO: the selected unit is destroyed?

            if (it_has_a_selected_action(out var the_action) && the_action.is_Not_Available())
                its_selected_action_becomes_empty();
            
            if (it_has_a_selected_unit(out var the_unit) && the_unit.s_Location.is_a_Bay(out var the_bay))
                its_selected_unit_becomes(the_bay.s_Owner);

            it_performed_an_action.Next();                
        }

        public void Toggles_the_Selected_Action_With(DUnitAction the_action_desc)
        {
            if (it_is_suspended)
                return;
            
            using (the_signal_guard.Holds_All_Events())
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
            
                it_performed_an_action.Next();
            }
        }

        public void Suspends_Until_the_Turns_End()
        {
            (!it_is_suspended).Must_Be_True();
            
            its_selection_before_suspending = its_possible_selection;
            its_selection_becomes_empty();
            it_is_suspended = true;
            
            it_performed_an_action.Next();                
        }
        
        public void Ends_the_Turn_and_Resumes()
        {
            it_is_suspended.Must_Be_True();
            
            using (the_signal_guard.Holds_All_Events())
            {
                the_game.must_have_a_Battle().Ends_the_Turn();
                
                its_selection_becomes(its_selection_before_suspending);
                it_is_suspended = false;
            
                it_performed_an_action.Next();
            }
        }
        
        public void Resets_the_Selection() => its_selection_becomes_empty();

        private Possible<Selection> its_possible_selection => its_selections_cell.s_Value;
        private Possible<MUnit> its_possible_selected_unit => its_selected_units_cell.s_Value;

        private MUnit it_must_have_a_unit_selected() => its_possible_selected_unit.must_have_a_Value();
        private bool it_has_a_command_at(MTile the_tile, out UnitCommand the_unit_command) => its_possible_command_at(the_tile).has_a_Value(out the_unit_command);

        private bool it_has_a_selected_unit(out MUnit the_unit) => its_possible_selected_unit.has_a_Value(out the_unit); 
        private bool it_has_a_selected_action(out MUnitAction the_action) =>
            semantic_resets(out the_action) &&
            its_possible_selection.has_a_Value(out var the_selection) && 
            the_selection.has_an_action(out the_action)
        ;
        private bool it_has_a_selection(out Selection the_selection) => its_possible_selection.has_a_Value(out the_selection);
        private bool it_has_a_unit_selected(out MUnit the_selected_unit) => its_possible_selected_unit.has_a_Value(out the_selected_unit);

        private bool it_owns_a_unit_at(MTile the_tile, out MUnit the_target_unit) => the_tile.has_a_Unit(out the_target_unit) && it_owns(the_target_unit);
        private bool it_owns(MUnit the_unit) => the_unit.Belongs_To(its_faction);

        private void its_selected_unit_becomes(MUnit unit) => its_selections_cell.s_Value = new Selection(unit, Possible.Empty<MUnitAction>());
        private void its_selected_action_becomes_empty() => its_selected_action_becomes(Possible.Empty<MUnitAction>());
        private void its_selected_action_becomes(Possible<MUnitAction> the_action) => its_selection_becomes(new Selection(it_must_have_a_unit_selected(), the_action));
        private void its_selection_becomes_empty() => its_selection_becomes(Possible.Empty<Selection>());
        private void its_selection_becomes(Possible<Selection> the_new_selection) => its_selections_cell.s_Value = the_new_selection;
        private Possible<UnitCommand> its_possible_command_at(MTile the_tile) => 
            it_has_a_selection(out var the_selection) 
                ? the_selection.s_possible_command_at(the_tile) 
                : Possible.Empty<UnitCommand>()
        ;

        private readonly GuardedCell<Possible<Selection>> its_selections_cell;
        private readonly MGame the_game;
        private readonly SignalGuard the_signal_guard;

        private readonly ICell<Possible<MUnit>> its_selected_units_cell;
        private readonly GuardedStream<TheVoid> it_performed_an_action;
        private readonly MFaction its_faction;
        private bool it_is_suspended;
        private Possible<Selection> its_selection_before_suspending;

        public struct Selection: IEquatable<Selection> //Maybe add a class, move some mutation methods there?
        {
            public Selection(MUnit unit) : this(unit, Possible.Empty<MUnitAction>()) {}
            public Selection(MUnit unit, Possible<MUnitAction> the_possible_action): this()
            {
                its_unit = unit;
                its_possible_action = the_possible_action;
            }
            
            public MUnit s_Unit => its_unit;

            public bool has_an_action(out MUnitAction the_action) => its_possible_action.has_a_Value(out the_action);
            public bool s_action_is(DUnitAction the_requested_action_desc) => 
                it_has_an_action(out var the_selected_action) && 
                the_selected_action.@is(the_requested_action_desc);

            public Possible<UnitCommand> s_possible_command_at(MTile the_tile) => 
                has_a_special_command_At(the_tile, out var the_command) 
                    ? the_command 
                    : its_possible_regular_action(the_tile)
            ;

            public bool Equals(Selection other) => Equals(s_Unit, other.s_Unit) && its_possible_action.Equals(other.its_possible_action);

            private bool has_a_special_command_At(MTile the_tile, out UnitCommand the_command) =>
                semantic_resets(out the_command) &&
                it_has_an_action(out var the_action) && 
                the_action.s_possible_Command_at(the_tile).has_a_Value(out the_command)
            ;

            private bool it_has_an_action(out MUnitAction the_action) => its_possible_action.has_a_Value(out the_action);
            private Possible<UnitCommand> its_possible_regular_action(MTile the_tile) => its_unit.s_Regular_Command_At(the_tile);

            private readonly MUnit its_unit;
            private readonly Possible<MUnitAction> its_possible_action;
        }

    }
}