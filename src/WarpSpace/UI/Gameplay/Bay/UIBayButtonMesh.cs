using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Player;
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
            var the_battle_component = FindObjectOfType<WGame>();

            the_battle_component.s_Players_Selections_Cell.Subscribe(updates_the_mesh);
            
            void updates_the_mesh(Possible<MPlayer.Selection> the_possible_selection)
            {
                if
                (
                    the_possible_selection.has_a_Value(out var the_selection) &&
                    the_selection.s_Unit.has_a_docked_unit_at(index, out var the_docked_unit)
                )
                {
                    unit_mesh.Present(the_docked_unit.s_Type, the_docked_unit.s_Faction);
                }
                else
                {
                    unit_mesh.Hide();
                }
            }
        }
    }
}
