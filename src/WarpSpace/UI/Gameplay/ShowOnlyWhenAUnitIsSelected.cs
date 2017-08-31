using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay
{
    public class ShowOnlyWhenAUnitIsSelected : MonoBehaviour
    {
        public void Start()
        {
            var battle = FindObjectOfType<Unity.World.Battle.Component>();
            
            Wire_Player_Selection();
            
            void Wire_Player_Selection() => 
                battle
                    .Player_Cell
                    .SelectMany(ps => ps.Select(p => p.SelectedUnit).Cell_Or_Single_Default())
                    .Subscribe(Set_Is_Active)
            ;

            void Set_Is_Active(Slot<UnitModel> selected_unit) => 
                gameObject.SetActive(selected_unit.Has_a_Value())
            ;
        }
    }
}