using System.Collections.Generic;
using System.Linq;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.UI.Gameplay.Bay
{
    public class BayPresenter: MonoBehaviour
    {
        public void Start()
        {
            var battle = FindObjectOfType<BattleComponent>();
            Wire_Player_Slot_Variable();

            void Wire_Player_Slot_Variable() =>
                battle.s_Players_Cell()
                    .SelectMany(pp => pp.Select(p => p.s_Selected_Units_Cell).Cell_Or_Single_Default())
                    .Subscribe(Handle_Unit_Selection)
            ;
        }

        private void Handle_Unit_Selection(Possible<MUnit> possible_unit)
        {
            if (possible_unit.Has_a_Value(out var unit) && unit.Has_a_Bay(out var bay))
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