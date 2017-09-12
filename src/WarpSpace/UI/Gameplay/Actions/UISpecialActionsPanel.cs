using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay.Actions
{
    public class UISpecialActionsPanel: MonoBehaviour
    {
        public void Start()
        {
            var battle = FindObjectOfType<WGame>();
            battle.s_Players_Selected_Units_Cell.Subscribe(Handle_Unit_Selection);
        }

        private void Handle_Unit_Selection(Possible<MUnit> possible_unit)
        {
            if (possible_unit.has_a_Value(out var unit) && unit.can_Perform_Special_Actions())
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}