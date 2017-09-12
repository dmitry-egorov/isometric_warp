using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game;
using WarpSpace.Game.Battle;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.Actions
{
    [RequireComponent(typeof(UIButton))]
    public class UIUnitActionButton : MonoBehaviour
    {
        public UIActionButtonType Type;
        
        public void Start()
        {
            var index = transform.GetSiblingIndex();
            var the_action_desc = Type.as_a_description(index);
        
            var the_battle_component = FindObjectOfType<WGame>();
            var the_button = GetComponent<UIButton>();

            the_battle_component.s_Players_Selections_Cell
                .SelectMany(ps => 
                    ps.SelectMany(the_selection => 
                        the_selection.s_Unit.s_possible_Action_For(the_action_desc)
                        .Select(the_action => the_action.s_Availability_Cell.Select(is_available => (is_available, the_selection.s_action_is(the_action_desc)))))
                    .Cell_Or_Single_Empty()
                )
                .Subscribe(it_updates_the_button);

            the_button.s_Presses_Stream.Subscribe(handles_a_click);

            void handles_a_click(TheVoid _)
            {
                if (!the_battle_component.has_a_Player(out var the_player))
                    return;
                
                the_player.Toggles_the_Selected_Action_With(the_action_desc);
            }
            
            void it_updates_the_button(Possible<(bool s_action_is_available, bool s_action_is_selected)> the_possible_selection)
            {
                if (the_possible_selection.has_a_Value(out var the_selection))
                {
                    if (!the_selection.s_action_is_available)
                    {
                        the_button.Becomes_Disabled();
                    }
                    else if (the_selection.s_action_is_selected)
                    {
                        the_button.Becomes_Toggled();
                    }
                    else
                    {
                        the_button.Becomes_Normal();
                    }

                    gameObject.Shows();
                }
                else
                {
                    gameObject.Hides();
                }
            }
        }
    }
}