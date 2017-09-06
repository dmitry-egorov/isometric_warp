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
        public ICell<Possible<MUnit>> Selected_Unit_Cell { get; }
        public ICell<Possible<MWeapon>> Selected_Weapon_Cell { get; }
        public ICell<Possible<MUnit>> Selected_Bay_Unit_Cell { get; }
        public ICell<Possible<Selection>> Selection_Cell => _selection_cell;//Value can be null

        public Possible<MUnit> Selected_Unit => Selected_Unit_Cell.s_Value;
        public Possible<MWeapon> Selected_Weapon => Selected_Weapon_Cell.s_Value;
        public Possible<MUnit> Selected_Bay_Unit => Selected_Bay_Unit_Cell.s_Value;
        
        public IStream<SelectedUnitChanged> Stream_Of_Selected_Unit_Changes { get; }
        public IStream<Movement> Stream_Of_Selected_Unit_Movements { get; } 

        public MPlayer()
        {
            _selection_cell = new ValueCell<Possible<Selection>>(Possible.Empty<Selection>());
            Selected_Unit_Cell = _selection_cell.Select(x => x.Select(s => s.Unit));
            Selected_Weapon_Cell = _selection_cell.Select(x => x.SelectMany(s => s.Sub_Selection.As_a_Weapon()));
            Selected_Bay_Unit_Cell = _selection_cell.Select(x => x.SelectMany(s => s.Sub_Selection.As_a_Bay_Unit()));
            Stream_Of_Selected_Unit_Changes = Create_Stream_Of_Selected_Unit_Changes();
            Stream_Of_Selected_Unit_Movements = Create_Stream_Of_Selected_Unit_Movements();

            Wire_Selected_Unit_Destruction();
            Wire_Selected_Bay_Unit_Move();
            
            
            void Wire_Selected_Unit_Destruction()
            {
                Selected_Unit_Cell
                    .SelectMany(pu => pu.Select(u => u.s_Signal_of_the_Destruction).Value_Or_Empty())
                    .Subscribe(_ => Deselect());
                    
                void Deselect() => _selection_cell.s_Value = Possible.Empty<Selection>();
            }

            void Wire_Selected_Bay_Unit_Move()
            {
                Selected_Bay_Unit_Cell
                    .SelectMany(pu => pu.Select(the_unit => the_unit.s_Stream_Of_Movements.First()).Value_Or_Empty())
                    .Subscribe(_ => _selection_cell.s_Value = new Selection(Selected_Unit.Must_Have_a_Value(), TheVoid.Instance));
            }

            IStream<Movement> Create_Stream_Of_Selected_Unit_Movements() => 
                Selected_Unit_Cell
                    .SelectMany(pu => pu.Select(u => u.s_Stream_Of_Movements).Value_Or(StreamCache.Empty_Stream_of_Movements))
            ;

            IStream<SelectedUnitChanged> Create_Stream_Of_Selected_Unit_Changes() => Selected_Unit_Cell.IncludePrevious().Select(p => new SelectedUnitChanged(p.previous, p.current));
        }

        public bool Has_A_Selected_Unit(out MUnit the_selected_unit) => Selected_Unit.Has_a_Value(out the_selected_unit);
        public MUnit Must_Have_A_Selected_Unit() => Selected_Unit.Must_Have_a_Value();

        public void Execute_Command_At(MTile tile)
        {
            if (!Possible_Command_At(tile).Has_a_Value(out var command)) 
                return;

            if (command.Is_a_Fire(out var fire))
            {
                fire.Weapon.Fires_At(fire.Target_Unit);
            }
            else if (command.Is_a_Select_Unit(out var select_unit))
            {
                Select_a_Unit(select_unit.Target_Unit);
            }
            else if (command.Is_a_Move(out var move))
            {
                move.Unit.Moves_To(move.Destination);
            }
            else if (command.Is_an_Interact(out var interact))
            {
                interact.Unit.Interacts_With(interact.Target_Structure);
            }
            else throw new InvalidOperationException("Unknown command");
        }

        public Command? Possible_Command_At(MTile tile)
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
                    return Command.Create.Select_Unit(target_unit);

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
            
            _selection_cell.s_Value = new Selection(unit, sub_selection);
        }

        private void Select_a_Unit(MUnit unit) => _selection_cell.s_Value = new Selection(unit, TheVoid.Instance);

        private void Select_Current_Units_Weapon()
        {
            var selected_unit = Must_Have_A_Selected_Unit();
            _selection_cell.s_Value = new Selection(selected_unit, selected_unit.s_Weapon);
        }

        private void Reset_Weapon_Selection()
        {
            var selected_unit = Must_Have_A_Selected_Unit();
            
            _selection_cell.s_Value = new Selection(selected_unit, TheVoid.Instance);
        }

        private Selection Must_Have_A_Selection() => Selection_Cell.Must_Have_a_Value();
        private bool A_Weapon_Is_Selected(out MWeapon the_selected_weapon) => Selected_Weapon.Has_a_Value(out the_selected_weapon); 
        private bool A_Bay_Unit_Is_Selected(out MUnit the_selected_bay_unit) => Selected_Bay_Unit.Has_a_Value(out the_selected_bay_unit); 
        private bool Can_Select(MUnit the_unit) => the_unit.s_Faction_Is(Faction.Player);
        

        private readonly ValueCell<Possible<Selection>> _selection_cell;


        public struct Selection
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
            
            public struct SubSelection
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

            }
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