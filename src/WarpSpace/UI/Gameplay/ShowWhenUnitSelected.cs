using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay
{
    public class ShowWhenUnitSelected : MonoBehaviour
    {
        public void Start()
        {
            var battle = FindObjectOfType<WGame>();
            
            Wire_Player_Selection();
            
            void Wire_Player_Selection() => 
                battle.s_Players_Selected_Units_Cell
                    .Subscribe(Set_Is_Active)
            ;

            void Set_Is_Active(Possible<MUnit> selected_unit) => 
                gameObject.SetActive(selected_unit.has_a_Value())
            ;
        }
    }
}