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
        public GameObject SlotPrefab;
        
        private IList<BaySlotPresenter> _slot_presenters;

        public void Start()
        {
            _slot_presenters = new List<BaySlotPresenter>();
            
            var battle = FindObjectOfType<BattleComponent>();
            Wire_Player_Slot_Variable();

            void Wire_Player_Slot_Variable() =>
                battle
                    .Player_Cell
                    .SelectMany(pp => pp.Select(p => p.Selected_Unit_Cell).Cell_Or_Single_Default())
                    .Subscribe(Handle_Unit_Selection)
            ;
        }

        private void Handle_Unit_Selection(Possible<MUnit> possible_unit)
        {
            if (possible_unit.Has_a_Value(out var unit) && unit.Has_a_Bay(out var bay))
            {
                var size = bay.Size;
                
                Recreate_Presenters_If_Needed(size);

                for (var i = 0; i < _slot_presenters.Count; i++)
                {
                    var presenter = _slot_presenters[i];
                    if (i < bay.Size)
                    {
                        presenter.Present(bay[i]);
                    }
                    else
                    {
                        presenter.Hide();
                    }
                }

                Show();
            }
            else
            {
                foreach (var slot in _slot_presenters)
                {
                    slot.Hide();
                }

                Hide();
            }
        }

        private void Recreate_Presenters_If_Needed(int target_count)
        {
            var current_count = _slot_presenters.Count;
            if (target_count <= current_count)
                return;

            gameObject.DestroyChildren();
            _slot_presenters.Clear();

            foreach (var _ in Enumerable.Repeat(0, target_count - current_count))
            {
                var slot_presenter = Instantiate(SlotPrefab, transform).GetComponent<BaySlotPresenter>();
                
                _slot_presenters.Add(slot_presenter);
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