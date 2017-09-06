using Lanski.Behaviours;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using UnityEngine.UI;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Settings;

namespace WarpSpace.UI.Gameplay.Bay
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(ClickSource))]
    public class BaySlotPresenter : MonoBehaviour
    {
        public void Start()
        {
            var index = transform.GetSiblingIndex();
            var settings = FindObjectOfType<BaySlotPresenterSettingsHolder>();
            var unit_mesh = GetComponentInChildren<UnitMesh>();
            var background = GetComponent<Image>();
            var player_cell = FindObjectOfType<BattleComponent>().Player_Cell;
            
            player_cell
                .SelectMany(pp => pp.Select(p => p.Selection_Cell).Cell_Or_Empty())
                .Subscribe(Present_Selection)
            ;

            var click_source = GetComponent<ClickSource>();
            player_cell
                .SkipEmpty()
                .SelectMany(player => click_source.Clicks.Select(click_event => player))
                .Subscribe(Handle_Click)
            ;

            void Handle_Click(MPlayer player)
            {
                var selected_unit = player.Must_Have_A_Selected_Unit();
                var selected_units_bay = selected_unit.Must_Have_a_Bay();
                var bay_slot = selected_units_bay[index].Must_Have_a_Value();

                if (bay_slot.Has_a_Unit(out var bay_slot_unit))
                {
                    player.Toggle_Bay_Unit_Selection(bay_slot_unit);
                }
            }
            
            void Present_Selection(Possible<MPlayer.Selection> possible_selection)
            {
                if 
                (
                    possible_selection.Has_a_Value(out var selection) &&
                    selection.Unit.Has_a_Bay(out var bay) && 
                    bay[index].Has_a_Value(out var bay_slot)
                )
                {
                    if (bay_slot.Has_a_Unit(out var unit))
                    {
                        var bay_unit_is_selected = selection.Has_a_Bay_Unit_Selected(out var selected_bay_unit) && unit == selected_bay_unit;
                        Present_a_Unit(unit, bay_unit_is_selected);
                    }
                    else
                    {
                        Present_an_Empty_Bay_Slot();
                    }

                    gameObject.Show();
                }
                else
                {
                    gameObject.Hide();
                }
            }

            void Present_a_Unit(MUnit unit, bool bay_unit_is_selected)
            {
                unit_mesh.Present(unit.s_Type, unit.s_Faction);
                background.material = bay_unit_is_selected ? settings.SelectedUnitBackgroundMaterial : settings.UnitBackgroundMaterial;
            }

            void Present_an_Empty_Bay_Slot()
            {
                unit_mesh.Hide();
                background.material = settings.EmptyBackgroundMaterial;
            }
        }
    }
}
