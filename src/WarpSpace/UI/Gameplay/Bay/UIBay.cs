using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay.Bay
{
    public class UIBay: MonoBehaviour
    {
        public void Start()
        {
            var battle = FindObjectOfType<BattleComponent>();
            Wire_Player_Slot_Variable();

            void Wire_Player_Slot_Variable() =>
                battle.s_Selected_Units_Cell
                .Subscribe(Handle_Unit_Selection)
            ;
        }

        private void Handle_Unit_Selection(Possible<MUnit> possible_unit)
        {
            if (possible_unit.has_a_Value(out var unit) && unit.has_a_Bay(out var bay))
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