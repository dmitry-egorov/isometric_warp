using System;
using Lanski.Structures;
using WarpSpace.Common.Behaviours;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Game.Battle.Unit
{
    public class WOutliner
    {
        public WOutliner(WGame the_game, MUnit the_unit, Outline the_outline)
        {
            if (the_game.s_Player.Owns(the_unit))
            {
                its_wire = it_wires_the_selected_unit();
            }
            else
            {
                UnityEngine.Object.Destroy(the_outline.gameObject);
            }
            
            //Note: every unit's outliner is wired to the player, wich is not the best in terms of performance.
            Action it_wires_the_selected_unit() => 
                the_game
                    .s_Players_Selected_Units_Cell
                    .Subscribe(it_handles_the_selected_unit_change)
            ;
            
            void it_handles_the_selected_unit_change(Possible<MUnit> possibly_selected_unit)
            {
                if (possibly_selected_unit.has_a_Value(out var the_selected_unit) && the_selected_unit == the_unit)
                {
                    the_outline.Shows();
                }
                else
                {
                    the_outline.Hides();
                }
            }
        }
        
        public void Destructs() => its_wire?.Invoke();
        
        private readonly Action its_wire;
    }
}