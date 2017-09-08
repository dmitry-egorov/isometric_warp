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
    [RequireComponent(typeof(ClickSource))]
    public class UIBayButton : MonoBehaviour
    {
        public void Start()
        {
            var index = transform.GetSiblingIndex();
            var settings = FindObjectOfType<UISettingsHolder>();
            var unit_mesh = GetComponentInChildren<UnitMesh>();
            var button = GetComponent<UIButton>();
            var the_battle_component = FindObjectOfType<BattleComponent>();

            button.s_Presses_Stream.Subscribe(handles_button_click);
            the_battle_component.s_Selections_Cell.Subscribe(_ => updates_the_button());
            
            void handles_button_click(TheVoid _)
            {
                if(!the_battle_component.has_a_Player(out var the_player))
                    return;
                
                var selected_unit = the_player.must_have_a_Unit_Selected();
                var selected_units_bay = selected_unit.must_have_a_Bay();
                var bay_slot = selected_units_bay[index].must_have_a_Value();

                if (bay_slot.has_a_Unit(out var bay_slot_unit))
                {
                    the_player.Toggles_the_Bay_Unit_Selection(bay_slot_unit);
                }
            }
            
            void updates_the_button()
            {
                if
                (
                    the_battle_component.s_Selections_Cell.has_a_Value(out var selection) &&
                    selection.s_Unit.has_a_Bay(out var bay) && 
                    bay[index].has_a_Value(out var bay_slot)
                )
                {
                    if (bay_slot.has_a_Unit(out var unit))
                    {
                        var bay_unit_is_selected = selection.has_a_Bay_Unit_Selected(out var selected_bay_unit) && unit == selected_bay_unit;
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

            void Present_a_Unit(MUnit unit, bool the_unit_is_selected)
            {
                unit_mesh.Present(unit.s_Type, unit.s_Faction);
                if (!unit.can_Move())
                {
                    button.Becomes_Disabled();
                }
                else if (the_unit_is_selected)
                {
                    button.Becomes_Active_With(settings.NormalHighlightedButtonMaterial, settings.PressedHighlightedButtonMaterial);
                }
                else
                {
                    button.Becomes_Active_With(settings.NormalButtonMaterial, settings.PressedButtonMaterial);
                }
            }

            void Present_an_Empty_Bay_Slot()
            {
                unit_mesh.Hide();
                button.Becomes_Disabled();
            }
        }
    }
}
