using Lanski.Behaviours;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.Bay
{
    [RequireComponent(typeof(UIButton))]
    public class UIBayButtonMesh : MonoBehaviour
    {
        public void Start()
        {
            var index = transform.GetSiblingIndex();
            var unit_mesh = GetComponentInChildren<UnitMesh>();
            var the_battle_component = FindObjectOfType<BattleComponent>();

            the_battle_component.s_Players_Selections_Cell.Subscribe(_ => updates_the_mesh());
            
            void updates_the_mesh()
            {
                if
                (
                    the_battle_component.s_Players_Selections_Cell.has_a_Value(out var selection) &&
                    selection.s_Unit.has_a_docked_unit_at(index, out var the_unit)
                )
                {
                    unit_mesh.Present(the_unit.s_Type, the_unit.s_Faction);
                }
                else
                {
                    unit_mesh.Hide();
                }
            }
        }
    }
}
