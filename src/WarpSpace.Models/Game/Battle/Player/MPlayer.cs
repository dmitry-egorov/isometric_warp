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
        public ICell<Possible<MUnit>> s_Selected_Units_Cell => the_selected_units_cell;
        public ICell<Possible<MWeapon>> s_Selected_Weapons_Cell => the_selected_weapons_cell;
        public ICell<Possible<MUnit>> s_Selected_Bay_Units_Cell => the_selected_bay_units_cell;
        public ICell<Possible<Selection>> s_Selections_Cell => the_selections_cell;//Value can be null

        public Possible<MUnit> Selected_Unit => s_Selected_Units_Cell.s_Value;
        public Possible<MWeapon> Selected_Weapon => s_Selected_Weapons_Cell.s_Value;
        public Possible<MUnit> Selected_Bay_Unit => s_Selected_Bay_Units_Cell.s_Value;

        public IStream<SelectedUnitChanged> s_Selected_Unit_Changes_Stream => the_selected_unit_changes_stream;
        public IStream<Movement> s_Selected_Unit_Movements_Stream => the_selected_unit_movements_stream;

        public MPlayer(EventsGuard the_events_guard)
        {
            this.the_events_guard = the_events_guard;
            the_selections_cell = new GuardedCell<Possible<Selection>>(Possible.Empty<Selection>(), the_events_guard);
            the_selected_units_cell = the_selections_cell.Select(x => x.Select(s => s.Unit));
            the_selected_weapons_cell = the_selections_cell.Select(x => x.SelectMany(s => s.Sub_Selection.As_a_Weapon()));
            the_selected_bay_units_cell = the_selections_cell.Select(x => x.SelectMany(s => s.Sub_Selection.As_a_Bay_Unit()));
            the_selected_unit_changes_stream = the_selected_units_cell.IncludePrevious().Select(p => new SelectedUnitChanged(p.previous, p.current));
            the_selected_unit_movements_stream = the_selected_units_cell.SelectMany(pu => pu.Select(u => u.s_Movements_Stream()).Value_Or(StreamCache.Empty_Stream_of_Movements));

            Wire_Selected_Unit_Destructions();
            Wire_Selected_Bay_Unit_Moves();
            Wire_Selected_Weapon_Fires();
            
            void Wire_Selected_Unit_Destructions()
            {
                the_selected_units_cell
                    .SelectMany(pu => pu.Select(u => u.s_Destruction_Signal()).Value_Or_Empty())
                    .Subscribe(_ => Deselects());
                    
                void Deselects() => the_selections_cell.s_Value = Possible.Empty<Selection>();
            }

            void Wire_Selected_Bay_Unit_Moves()
            {
                the_selected_bay_units_cell
                    .SelectMany(pu => pu.Select(the_unit => the_unit.s_Movements_Stream().First()).Value_Or_Empty())
                    .Subscribe(_ => the_selections_cell.s_Value = new Selection(Selected_Unit.Must_Have_a_Value(), TheVoid.Instance));
            }

            void Wire_Selected_Weapon_Fires()
            {
                the_selected_weapons_cell
                    .SelectMany(pw => pw.Select_Stream_Or_Empty(w => w.s_Fires_Stream.First().Select(_ => w)))
                    .Subscribe(w => this.Toggle_Weapon_Selection())
                ;
            }

        }

        public bool Has_A_Selected_Unit(out MUnit the_selected_unit) => Selected_Unit.Has_a_Value(out the_selected_unit);
        public MUnit Must_Have_A_Selected_Unit() => Selected_Unit.Must_Have_a_Value();

        public void Execute_Command_At(MTile tile)
        {
            if (!s_Possible_Command_At(tile).Has_a_Value(out var command)) 
                return;

            command.Executes_With(the_events_guard);
        }

        public Command? s_Possible_Command_At(MTile tile)
        {
            if (A_Weapon_Is_Selected(out var weapon))
            {
                if (tile.Has_a_Unit(out var target_unit) && weapon.Can_Fire_At(target_unit))
                    return Command.Create.Fire(weapon, target_unit);
            }
            else if(A_Bay_Unit_Is_Selected(out var bay_unit))
            {
                if (bay_unit.Can_Move_To(tile))
                    return Command.Create.Move(bay_unit, tile);
            }
            else
            {
                if (tile.Has_a_Unit(out var target_unit) && Can_Select(target_unit))
                    return Command.Create.Select_Unit(this, target_unit);

                if (Has_A_Selected_Unit(out var selected_unit))
                {
                    if (selected_unit.Can_Move_To(tile))
                        return Command.Create.Move(selected_unit, tile);
                    
                    if (tile.Has_a_Structure(out var structure) && selected_unit.Can_Interact_With(structure))
                        return Command.Create.Interact(selected_unit, structure);
                }
            }

            return null;
        }

        public void Toggle_Weapon_Selection()
        {
            var selection = Must_Have_A_Selection();

            if (selection.Has_a_Weapon_Selected())
            {
                Reset_Weapon_Selection();
            }
            else
            {
                Select_Current_Units_Weapon();
            }
        }

        public void Toggle_Bay_Unit_Selection(MUnit bay_unit)
        {
            var unit = Must_Have_A_Selected_Unit();
            var bay = unit.Must_Have_a_Bay();

            bay.Must_Contain(bay_unit);
            
            var sub_selection = 
                A_Bay_Unit_Is_Selected(out var selected_bay_unit) && bay_unit == selected_bay_unit
                ? Selection.SubSelection.Empty()
                : bay_unit;
            
            the_selections_cell.s_Value = new Selection(unit, sub_selection);
        }

        internal void Selects_a_Unit(MUnit unit) => the_selections_cell.s_Value = new Selection(unit, TheVoid.Instance);

        private void Select_Current_Units_Weapon()
        {
            var selected_unit = Must_Have_A_Selected_Unit();
            the_selections_cell.s_Value = new Selection(selected_unit, selected_unit.s_Weapon);
        }

        private void Reset_Weapon_Selection()
        {
            var selected_unit = Must_Have_A_Selected_Unit();
            
            the_selections_cell.s_Value = new Selection(selected_unit, TheVoid.Instance);
        }
        

        private Selection Must_Have_A_Selection() => s_Selections_Cell.Must_Have_a_Value();
        private bool A_Weapon_Is_Selected(out MWeapon the_selected_weapon) => Selected_Weapon.Has_a_Value(out the_selected_weapon); 
        private bool A_Bay_Unit_Is_Selected(out MUnit the_selected_bay_unit) => Selected_Bay_Unit.Has_a_Value(out the_selected_bay_unit); 
        private bool Can_Select(MUnit the_unit) => the_unit.s_Faction_Is(Faction.Player);
        
        private readonly GuardedCell<Possible<Selection>> the_selections_cell;
        private readonly EventsGuard the_events_guard;
        private readonly ICell<Possible<MUnit>> the_selected_bay_units_cell;
        private readonly ICell<Possible<MWeapon>> the_selected_weapons_cell;
        private readonly ICell<Possible<MUnit>> the_selected_units_cell;
        private readonly IStream<SelectedUnitChanged> the_selected_unit_changes_stream;
        private readonly IStream<Movement> the_selected_unit_movements_stream;


        public struct Selection: IEquatable<Selection>
        {
            public readonly MUnit Unit;
            public readonly SubSelection Sub_Selection;

            public Selection(MUnit unit, SubSelection sub_selection)
            {
                Unit = unit;
                Sub_Selection = sub_selection;
            }

            public bool Has_a_Bay_Unit_Selected(out MUnit bay_unit) => Sub_Selection.Is_a_Bay_Unit(out bay_unit);
            public bool Has_a_Weapon_Selected(out MWeapon weapon) => Sub_Selection.Is_a_Weapon(out weapon);
            public bool Has_a_Weapon_Selected() => Sub_Selection.Is_a_Weapon();
            
            public struct SubSelection: IEquatable<SubSelection>
            {
                public Or<MWeapon, MUnit, TheVoid> Variant;
                
                public static SubSelection Empty() => new SubSelection(TheVoid.Instance);

                public SubSelection(Or<MWeapon, MUnit, TheVoid> variant)
                {
                    Variant = variant;
                }

                [Pure] public bool Is_a_Weapon() => Variant.Is_a_T1();
                [Pure] public bool Is_a_Weapon(out MWeapon weapon) => Variant.Is_a_T1(out weapon);
                [Pure] public bool Is_a_Bay_Unit(out MUnit bay_unit) => Variant.Is_a_T2(out bay_unit);
                [Pure] public bool Is_Empty() => Variant.Is_a_T3();
                
                [Pure] public Possible<MWeapon> As_a_Weapon() => Variant.As_a_T1();
                [Pure] public Possible<MUnit> As_a_Bay_Unit() => Variant.As_a_T2();
                
                public static implicit operator SubSelection(MWeapon weapon) => new SubSelection(weapon);
                public static implicit operator SubSelection(MUnit unit) => new SubSelection(unit);
                public static implicit operator SubSelection(TheVoid the_void) => new SubSelection(the_void);

                public bool Equals(SubSelection other) => Variant.Equals(other.Variant);
            }

            public bool Equals(Selection other) => Equals(Unit, other.Unit) && Sub_Selection.Equals(other.Sub_Selection);
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

    }
}