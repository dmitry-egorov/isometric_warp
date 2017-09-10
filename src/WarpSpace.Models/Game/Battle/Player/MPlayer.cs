using System;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Weapon;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Player
{
    public class MPlayer
    {
        public MPlayer(SignalGuard the_signal_guard)
        {
            this.the_signal_guard = the_signal_guard;
            
            its_selections_cell = new GuardedCell<Possible<Selection>>(Possible.Empty<Selection>(), the_signal_guard);
            
            its_selected_units_cell            = it_generates_its_selected_units_cell();
            its_selected_weapons_cell          = it_generates_its_selected_weapons_cell();
            its_selected_bay_units_cell        = it_generates_its_selected_bay_units_cell();
            its_selected_unit_changes_stream   = it_generates_its_selected_unit_changes_stream();
            its_selected_unit_movements_stream = it_generates_its_selected_unit_movements_stream();

            it_wires_the_selected_unit_destructions();
            it_wires_the_selected_bay_unit_moves();
            it_wires_the_selected_unit_docks();
            it_wires_the_selected_weapon_fires();

            ICell<Possible<MUnit>> it_generates_its_selected_units_cell()                => its_selections_cell.Select(x => x.Select(s => s.s_Unit));
            ICell<Possible<MWeapon>> it_generates_its_selected_weapons_cell()            => its_selections_cell.Select(x => x.SelectMany(s => s.s_Sub_Selection.as_a_Weapon()));
            ICell<Possible<MUnit>> it_generates_its_selected_bay_units_cell()            => its_selections_cell.Select(x => x.SelectMany(s => s.s_Sub_Selection.as_a_Bay_Unit()));
            IStream<SelectedUnitChange> it_generates_its_selected_unit_changes_stream() => its_selected_units_cell.IncludePrevious().Select(p => new SelectedUnitChange { s_Previous = p.previous, s_Current = p.current });
            IStream<Movement> it_generates_its_selected_unit_movements_stream()          => its_selected_units_cell.SelectMany(pu => pu.Select_Stream_Or_Empty(u => u.s_Movements_Stream));
            
            void it_wires_the_selected_unit_destructions() => 
                its_selected_units_cell
                .SelectMany(pu => pu.Select(the_unit => the_unit.s_Destruction_Signal).Value_Or_Empty())
                .Subscribe(_ => its_selection_becomes_empty())
            ;

            void it_wires_the_selected_bay_unit_moves() => 
                its_selected_bay_units_cell
                .SelectMany(pu => pu.Select(the_unit => the_unit.s_Movements_Stream.First()).Value_Or_Empty())
                .Subscribe(_ => its_selected_action_becomes_empty())
            ;

            void it_wires_the_selected_unit_docks() => 
                its_selected_unit_movements_stream
                .Where(m => m.Destination.is_a_Bay())
                .Subscribe(_ => its_selection_becomes_empty())
            ;

            void it_wires_the_selected_weapon_fires() => 
                its_selected_weapons_cell
                .SelectMany(pw => pw.Select_Stream_Or_Empty(the_weapon => the_weapon.s_Fires_Stream.Select(_ => the_weapon)))
                .Where(w => !w.can_Fire())
                .Subscribe(w => its_selected_action_becomes_empty())
            ;
        }
        
        public ICell<Possible<MUnit>> s_Selected_Units_Cell => its_selected_units_cell;
        public ICell<Possible<MWeapon>> s_Selected_Weapons_Cell => its_selected_weapons_cell;
        public ICell<Possible<Selection>> s_Selections_Cell => its_selections_cell;//Value can be null

        public IStream<SelectedUnitChange> s_Selected_Unit_Changes_Stream => its_selected_unit_changes_stream;
        public IStream<Movement> s_Selected_Unit_Movements_Stream => its_selected_unit_movements_stream;
        public Possible<MUnit> s_Possible_Selected_Unit => its_possible_selected_unit;

        public bool has_a_Unit_Selected(out MUnit the_selected_unit) => it_has_a_unit_selected(out the_selected_unit);
        public MUnit must_have_a_Unit_Selected() => it_must_have_a_unit_selected();

        public bool has_a_Command_At(MTile tile, out UnitCommand the_unit_command) => its_possible_command_at(tile).has_a_Value(out the_unit_command);

        public void Resets_the_Selection() => its_selection_becomes_empty();

        public void Executes_a_Command_At(MTile the_tile)
        {
            if (this.has_a_Command_At(the_tile, out var the_command))
            {
                the_command.Executes_With(the_signal_guard);
            }
            else if (it_can_select_a_unit_at(the_tile, out var the_target_unit))
            {
                it_selects_a_unit(the_target_unit);
            }
        }

        public void toggles_the_selected_action_with(DUnitAction the_action_desc)
        {
            var the_selection = its_possible_selection.must_have_a_Value();
            
            if (the_selection.s_action_is(the_action_desc))
            {
                its_selected_action_becomes_empty();
            }
            else
            {
                var the_selected_unit = the_selection.s_Unit;
                var the_action = the_selected_unit.s_possible_action_for(the_action_desc).must_have_a_Value();
                its_selected_action_becomes(the_action);
            }
        }
        
//        public void Toggles_the_Weapon_Selection()
//        {
//            var selection = it_must_have_a_selection();
//
//            if (selection.has_a_Weapon_Selected())
//            {
//                its_sub_selection_becomes_empty();
//            }
//            else
//            {
//                it_selects_current_units_weapon();
//            }
//        }
//
//        public void Toggles_the_Bay_Unit_Selection(MUnit bay_unit)
//        {
//            var unit = must_have_a_Unit_Selected();
//            var bay = unit.must_have_a_Bay();
//
//            bay.must_Contain(bay_unit);
//            
//            if (it_has_a_bay_unit_selected(out var selected_bay_unit) && bay_unit == selected_bay_unit)
//            {
//                its_sub_selection_becomes_empty();
//            }
//            else
//            {
//                its_sub_selection_becomes(bay_unit);
//            }
//        }
//        
//        public void Toggles_the_Dock_Selection()
//        {
//            var selection = it_must_have_a_selection();
//            if (selection.has_the_Dock_Action_Selected())
//            {
//                its_sub_selection_becomes_empty();
//            }
//            else
//            {
//                its_sub_selection_becomes(DockAction.Instance);
//            }
//        }

        internal void it_selects_a_unit(MUnit unit) => its_selections_cell.s_Value = new Selection(unit, Possible.Empty<MUnitAction>());

        private Possible<UnitCommand> its_possible_command_at(MTile the_tile) => 
            it_has_a_selection(out var the_selection) 
            ? the_selection.s_possible_command_at(the_tile) 
            : Possible.Empty<UnitCommand>()
        ;
        
        private Possible<Selection> its_possible_selection => its_selections_cell.s_Value;
        private Possible<MUnit> its_possible_selected_unit => its_selected_units_cell.s_Value;
        private Possible<MUnit> its_selected_bay_unit => its_selected_bay_units_cell.s_Value;

        private MUnit it_must_have_a_unit_selected() => its_possible_selected_unit.must_have_a_Value();
        private Selection it_must_have_a_selection() => its_selections_cell.must_have_a_Value();
        
        private bool it_has_a_selection(out Selection the_selection) => its_possible_selection.has_a_Value(out the_selection);
        private bool it_has_a_unit_selected(out MUnit the_selected_unit) => its_possible_selected_unit.has_a_Value(out the_selected_unit);
        private bool it_has_a_bay_unit_selected(out MUnit the_selected_bay_unit) => its_selected_bay_unit.has_a_Value(out the_selected_bay_unit); 

        private bool it_can_select_a_unit_at(MTile the_tile, out MUnit the_target_unit) => the_tile.has_a_Unit(out the_target_unit) && it_can_select(the_target_unit);
        private bool it_can_select(MUnit the_unit) => the_unit.s_Faction_is(Faction.Player);

        private void its_selected_action_becomes_empty() => its_selected_action_becomes(Possible.Empty<MUnitAction>());
        private void its_selected_action_becomes(Possible<MUnitAction> the_action) => its_selection_becomes(new Selection(must_have_a_Unit_Selected(), the_action));
        private void its_selection_becomes_empty() => its_selection_becomes(Possible.Empty<Selection>());
        private void its_selection_becomes(Possible<Selection> the_new_selection) => its_selections_cell.s_Value = the_new_selection;

        private readonly GuardedCell<Possible<Selection>> its_selections_cell;
        private readonly SignalGuard the_signal_guard;
        
        private readonly ICell<Possible<MUnit>> its_selected_bay_units_cell;
        private readonly ICell<Possible<MWeapon>> its_selected_weapons_cell;
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

            public bool has_an_action(out MUnitAction the_action) => its_possible_action.has_a_Value(out the_action);
            public bool s_action_is(DUnitAction the_requested_action_desc) => 
                this.has_an_action(out var the_selected_action) && 
                the_selected_action.@is(the_requested_action_desc);

            public Possible<UnitCommand> s_possible_command_at(MTile the_tile) => 
                has_a_special_command_At(the_tile, out var the_command) 
                    ? the_command 
                    : its_possible_regular_action(the_tile)
            ;

            public bool Equals(Selection other) => Equals(s_Unit, other.s_Unit) && its_possible_action.Equals(other.its_possible_action);

            private bool has_a_special_command_At(MTile the_tile, out UnitCommand the_command) =>
                semantic_resets(out the_command) &&
                has_an_action(out var the_action) && 
                the_action.s_possible_Command_at(the_tile).has_a_Value(out the_command)
            ;

            private Possible<UnitCommand> its_possible_regular_action(MTile the_tile)
            {
                var iterator = s_Unit.s_Regular_Actions.SIterate();
                while (iterator.has_a_Value(out var the_regular_action))
                {
                    if (the_regular_action.s_possible_Command_at(the_tile).has_a_Value(out var the_command))
                        return the_command;
                }

                return Possible.Empty<UnitCommand>();
            }

            private readonly MUnit its_unit;
            private readonly Possible<MUnitAction> its_possible_action;
        }

        public struct SelectedUnitChange { public Possible<MUnit> s_Current; public Possible<MUnit> s_Previous; }
    }
}