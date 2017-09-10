using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.Actions
{
    [RequireComponent(typeof(UIButton))]
    public class UIUnitActionButton : MonoBehaviour
    {
        public UIActionButtonType Type;
        
        public void Start()
        {
            var the_action_desc = Type.as_a_description();
        
            var the_battle_component = FindObjectOfType<BattleComponent>();
            var the_button = GetComponent<UIButton>();
            ???
//            the_battle_component.s_Players_Selections_Cell
//                .Select(ps => ps.SelectMany(the_selection => 
//                                            the_selection.s_Unit.s_possible_action_for(the_action_desc)
//                                            .Select(the_action => (the_selection, the_action))
//                                )
//                                .Select_Stream_Or_Empty(t => t.Item2.s_Availability_Cell.Select(is_avalable => (t.Item1, )
//                                )
//                .Subscribe(the_selection => it_updates_the_button(the_selection))
//            ;
//
//            the_battle_component.s_Players_Selected_Units_Cell
//                .SelectMany(pu => pu.Select_Stream_Or_Empty(u => u.s_Movements_Stream))
//                .Subscribe(_ => it_updates_the_button())
//            ;

            the_button.s_Presses_Stream.Subscribe(handles_a_click);

            void handles_a_click(TheVoid _)
            {
                if (!the_battle_component.has_a_Player(out var the_player))
                    return;
                
                the_player.toggles_the_selected_action_with(the_action_desc);
            }
            
            void it_updates_the_button(Possible<MPlayer.Selection> the_possible_selection)
            {
                if (the_battle_component.has_a_Selection(out var the_selection) && 
                    the_selection.s_Unit.s_possible_action_for(the_action_desc).has_a_Value(out var the_action))
                {
                    if (!the_action.is_available())
                    {
                        the_button.Becomes_Disabled();
                    }
                    else if (the_selection.s_action_is(the_action_desc))
                    {
                        the_button.Becomes_Toggled();
                    }
                    else
                    {
                        the_button.Becomes_Normal();
                    }

                    gameObject.Show();
                }
                else
                {
                    gameObject.Hide();
                }
            }
        }
    }
}