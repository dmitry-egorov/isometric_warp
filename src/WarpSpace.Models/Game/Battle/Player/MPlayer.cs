using System;
using JetBrains.Annotations;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Player
{
    public class MPlayer
    {
        public MPlayer(SignalGuard the_signal_guard)
        {
            this.the_signal_guard = the_signal_guard;
            
            s_selections_cell = new GuardedCell<Possible<Selection>>(Possible.Empty<Selection>(), the_signal_guard);
            
            s_selected_units_cell            = it.s_selections_cell.Select(x => x.Select(s => s.s_Unit));
            s_selected_weapons_cell          = it.s_selections_cell.Select(x => x.SelectMany(s => s.s_Sub_Selection.as_a_Weapon()));
            s_selected_bay_units_cell        = it.s_selections_cell.Select(x => x.SelectMany(s => s.s_Sub_Selection.as_a_Bay_Unit()));
            s_selected_unit_changes_stream   = it.s_selected_units_cell.IncludePrevious().Select(p => new SelectedUnitChanged(p.previous, p.current));
            s_selected_unit_movements_stream = it.s_selected_units_cell.SelectMany(pu => pu.Select(u => u.s_Movements_Stream).Value_Or(StreamCache.Empty_Stream_of_Movements));

            it_wires_the_selected_unit_destructions();
            it_wires_the_selected_bay_unit_moves();
            it_wires_the_selected_unit_docks();
            it_wires_the_selected_weapon_fires();
            
            void it_wires_the_selected_unit_destructions()
            {
                it.s_selected_units_cell
                    .SelectMany(pu => pu.Select(the_unit => the_unit.s_Destruction_Signal).Value_Or_Empty())
                    .Subscribe(_ => it.s_selection_becomes_empty());
            }

            void it_wires_the_selected_bay_unit_moves()
            {
                it.s_selected_bay_units_cell
                    .SelectMany(pu => pu.Select(the_unit => the_unit.s_Movements_Stream.First()).Value_Or_Empty())
                    .Subscribe(_ => it.s_sub_selection_becomes_empty());
            }

            void it_wires_the_selected_unit_docks()
            {
                it.s_selected_unit_movements_stream
                    .Where(m => m.Destination.is_a_Bay())
                    .Subscribe(_ => it.s_selection_becomes_empty());
            }

            void it_wires_the_selected_weapon_fires()
            {
                it.s_selected_weapons_cell
                    .SelectMany(pw => pw.Select_Stream_Or_Empty(w => w.s_Fires_Stream.Select(_ => w)))
                    .Where(w => !w.can_Fire())
                    .Subscribe(w => it.s_sub_selection_becomes_empty())
                ;
            }
        }
        
        public ICell<Possible<MUnit>> s_Selected_Units_Cell => it.s_selected_units_cell;
        public ICell<Possible<MWeapon>> s_Selected_Weapons_Cell => it.s_selected_weapons_cell;
        public ICell<Possible<Selection>> s_Selections_Cell => it.s_selections_cell;//Value can be null

        public IStream<SelectedUnitChanged> s_Selected_Unit_Changes_Stream => it.s_selected_unit_changes_stream;
        public IStream<Movement> s_Selected_Unit_Movements_Stream => it.s_selected_unit_movements_stream;
        public Possible<MUnit> s_Possible_Selected_Unit => it.s_possible_selected_unit;

        public bool has_a_Unit_Selected(out MUnit the_selected_unit) => it.has_a_unit_selected(out the_selected_unit);
        public MUnit must_have_a_Unit_Selected() => it.must_have_a_unit_selected();

        public bool has_a_Command_At(MTile tile, out Command command) => it.might_have_a_command_at(tile).has_a_Value(out command);

        public void Resets_the_Selection() => it.s_selection_becomes_empty();

        public void Executes_a_Command_At(MTile tile)
        {
            if (!it.has_a_Command_At(tile, out var the_command)) 
                return;

            the_command.Executes_With(the_signal_guard);
        }
        
        public void Toggles_the_Weapon_Selection()
        {
            var selection = must_have_a_selection();

            if (selection.has_a_Weapon_Selected())
            {
                it.s_sub_selection_becomes_empty();
            }
            else
            {
                it.selects_current_units_weapon();
            }
        }

        public void Toggles_the_Bay_Unit_Selection(MUnit bay_unit)
        {
            var unit = must_have_a_Unit_Selected();
            var bay = unit.must_have_a_Bay();

            bay.must_Contain(bay_unit);
            
            if (has_a_bay_unit_selected(out var selected_bay_unit) && bay_unit == selected_bay_unit)
            {
                it.s_sub_selection_becomes_empty();
            }
            else
            {
                it.s_sub_selection_becomes(bay_unit);
            }
        }
        
        public void Toggles_the_Dock_Selection()
        {
            var selection = must_have_a_selection();
            if (selection.has_the_Dock_Action_Selected())
            {
                it.s_sub_selection_becomes_empty();
            }
            else
            {
                it.s_sub_selection_becomes(DockAction.Instance);
            }
        }

        internal void Selects_a_Unit(MUnit unit) => it.s_selections_cell.s_Value = new Selection(unit, TheVoid.Instance);

        
        private Possible<Command> might_have_a_command_at(MTile the_tile)
        {
            if (it.has_a_weapon_selected(out var the_selected_weapon))
            {
                if (the_selected_weapon.can_Fire_At_a_Unit_At(the_tile, out var the_target_unit))
                    return Command.Create.Fire(the_selected_weapon, the_target_unit);
            }
            else if (it.has_a_bay_unit_selected(out var the_selected_bay_unit))
            {
                if (the_selected_bay_unit.can_Undock_At(the_tile, out var the_target_location))
                    return Command.Create.Move(the_selected_bay_unit, the_target_location);
            }
            else if (it.has_a_dock_action_selected(out var the_selected_unit))
            {
                if (the_selected_unit.can_Dock_At(the_tile, out var the_target_location))
                    return Command.Create.Move(the_selected_unit, the_target_location);
            }
            else
            {
                if (it.can_select_a_unit_at(the_tile, out var the_target_unit))
                    return Command.Create.Select_Unit(it, the_target_unit);

                if (it.has_a_Unit_Selected(out the_selected_unit))
                {
                    if (the_selected_unit.can_Move_To(the_tile, out var the_tiles_location))
                        return Command.Create.Move(the_selected_unit, the_tiles_location);
                    
                    if (the_selected_unit.can_Interact_With_a_Structure_At(the_tile, out var the_target_structure))
                        return Command.Create.Interact(the_selected_unit, the_target_structure);
                }
            }

            return Possible.Empty<Command>();
        }


        private void selects_current_units_weapon()
        {
            var the_selected_unit = must_have_a_Unit_Selected();
            it.s_sub_selection_becomes(the_selected_unit.s_Weapon);
        }
        
        private Possible<MUnit> s_possible_selected_unit => s_selected_units_cell.s_Value;
        private Possible<MWeapon> s_selected_weapon => s_selected_weapons_cell.s_Value;
        private Possible<MUnit> s_selected_bay_unit => s_selected_bay_units_cell.s_Value;

        private MUnit must_have_a_unit_selected() => s_possible_selected_unit.must_have_a_Value();
        private Selection must_have_a_selection() => it.s_selections_cell.must_have_a_Value();
        
        private bool has_a_dock_action_selected(out MUnit the_selected_unit) => 
            it.has_a_unit_selected(out the_selected_unit) 
            && it.must_have_a_selection().has_the_Dock_Action_Selected()
        ;

        private bool has_a_unit_selected(out MUnit the_selected_unit) => s_possible_selected_unit.has_a_Value(out the_selected_unit);
        private bool has_a_weapon_selected(out MWeapon the_selected_weapon) => s_selected_weapon.has_a_Value(out the_selected_weapon); 
        private bool has_a_bay_unit_selected(out MUnit the_selected_bay_unit) => s_selected_bay_unit.has_a_Value(out the_selected_bay_unit); 

        private bool can_select_a_unit_at(MTile the_tile, out MUnit the_target_unit) => the_tile.has_a_Unit(out the_target_unit) && it.can_select(the_target_unit);
        private bool can_select(MUnit the_unit) => the_unit.s_Faction_is(Faction.Player);

        private void s_sub_selection_becomes_empty() => it.s_sub_selection_becomes(TheVoid.Instance);
        private void s_sub_selection_becomes(Selection.Sub the_sub_selection) => it.s_selection_becomes(new Selection(must_have_a_Unit_Selected(), the_sub_selection));
        private void s_selection_becomes_empty() => it.s_selection_becomes(Possible.Empty<Selection>());
        private void s_selection_becomes(Possible<Selection> the_new_selection) => it.s_selections_cell.s_Value = the_new_selection;

        private MPlayer it => this;

        private readonly GuardedCell<Possible<Selection>> s_selections_cell;
        private readonly SignalGuard the_signal_guard;
        
        private readonly ICell<Possible<MUnit>> s_selected_bay_units_cell;
        private readonly ICell<Possible<MWeapon>> s_selected_weapons_cell;
        private readonly ICell<Possible<MUnit>> s_selected_units_cell;
        private readonly IStream<SelectedUnitChanged> s_selected_unit_changes_stream;
        private readonly IStream<Movement> s_selected_unit_movements_stream;

        public struct Selection: IEquatable<Selection> //Maybe turn into a class, move some mutation methods there?
        {
            public readonly MUnit s_Unit;
            public readonly Sub s_Sub_Selection;

            public Selection(MUnit unit, Sub sub_selection)
            {
                s_Unit = unit;
                s_Sub_Selection = sub_selection;
            }

            public bool has_a_Bay_Unit_Selected(out MUnit bay_unit) => s_Sub_Selection.is_a_Bay_Unit(out bay_unit);
            public bool has_a_Weapon_Selected(out MWeapon weapon) => s_Sub_Selection.is_a_Weapon(out weapon);
            public bool has_a_Weapon_Selected() => s_Sub_Selection.is_a_Weapon();
            public bool has_the_Dock_Action_Selected() => s_Sub_Selection.is_a_Dock_Action();

            public struct Sub: IEquatable<Sub>
            {
                public Or<MWeapon, MUnit, DockAction, TheVoid> Variant;
                
                public static Sub Empty() => new Sub(TheVoid.Instance);

                public Sub(Or<MWeapon, MUnit, DockAction, TheVoid> variant)
                {
                    Variant = variant;
                }

                [Pure] public bool is_a_Weapon() => Variant.is_a_T1();
                [Pure] public bool is_a_Weapon(out MWeapon weapon) => Variant.is_a_T1(out weapon);
                [Pure] public bool is_a_Bay_Unit(out MUnit bay_unit) => Variant.is_a_T2(out bay_unit);
                [Pure] public bool is_a_Dock_Action() => Variant.is_a_T3();
                [Pure] public bool is_Empty() => Variant.is_a_T4();

                [Pure] public Possible<MWeapon> as_a_Weapon() => Variant.as_a_T1();
                [Pure] public Possible<MUnit> as_a_Bay_Unit() => Variant.as_a_T2();
                
                public static implicit operator Sub(MWeapon weapon) => new Sub(weapon);
                public static implicit operator Sub(MUnit unit) => new Sub(unit);
                public static implicit operator Sub(DockAction dock) => new Sub(dock);
                public static implicit operator Sub(TheVoid the_void) => new Sub(the_void);

                public bool Equals(Sub other) => Variant.Equals(other.Variant);
            }

            public bool Equals(Selection other) => Equals(s_Unit, other.s_Unit) && s_Sub_Selection.Equals(other.s_Sub_Selection);
        }

        public struct SelectedUnitChanged
        {
            public readonly Possible<MUnit> Current;
            public readonly Possible<MUnit> Previous;

            public SelectedUnitChanged(Possible<MUnit> previous, Possible<MUnit> current)
            {
                Current = current;
                Previous = previous;
            }
        }
        
        public struct DockAction: IEquatable<DockAction>
        {
            public static readonly DockAction Instance = new DockAction();
            public bool Equals(DockAction other) => true;
        }
    }

    
}