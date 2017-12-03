using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common.Behaviours;
using WarpSpace.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;
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
            
            var the_mesh_renderer = GetComponentInChildren<MeshRenderer>();
            var mesh_presenter = new UnitMeshPresenter(the_mesh_renderer);
            var the_battle_component = FindObjectOfType<WGame>();

            the_battle_component.s_Players_Selections_Cell.Subscribe(updates_the_mesh);
            
            void updates_the_mesh(Possible<MPlayer.Selection> the_possible_selection)
            {
                if
                (
                    the_possible_selection.has_a_Value(out var the_selection) &&
                    the_selection.s_Unit.has_a_Docked_Unit_at(index, out var the_docked_unit)
                )
                {
                    mesh_presenter.Presents(the_docked_unit);
                }
                else
                {
                    mesh_presenter.Hides();
                }
            }
        }
    }
}
