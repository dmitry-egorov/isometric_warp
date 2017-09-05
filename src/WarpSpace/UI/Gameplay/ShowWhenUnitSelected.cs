using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay
{
    public class ShowWhenUnitSelected : MonoBehaviour
    {
        public void Start()
        {
            var battle = FindObjectOfType<BattleComponent>();
            
            Wire_Player_Selection();
            
            void Wire_Player_Selection() => 
                battle
                    .Player_Cell
                    .SelectMany(ps => ps.Select(p => p.Selected_Unit_Cell).Cell_Or_Single_Default())
                    .Subscribe(Set_Is_Active)
            ;

            void Set_Is_Active(Possible<MUnit> selected_unit) => 
                gameObject.SetActive(selected_unit.Has_a_Value())
            ;
        }
    }
}