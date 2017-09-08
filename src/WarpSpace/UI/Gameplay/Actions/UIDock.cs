using Lanski.Behaviours;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Settings;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.Actions
{
    [RequireComponent(typeof(UIButton))]
    public class UIDock : MonoBehaviour
    {
        public void Start()
        {
            var settings = FindObjectOfType<UISettingsHolder>();

            var the_battle_component = FindObjectOfType<BattleComponent>();
            var button = GetComponent<UIButton>();
            
            the_battle_component.s_Selections_Cell
                .Subscribe(_ => Updates_the_button())
            ;

            the_battle_component
                .s_Selected_Units_Cell
                .SelectMany(pu => pu.Select_Stream_Or_Empty(u => u.s_Movements_Stream))
                .Subscribe(_ => Updates_the_button())
            ;

            button.s_Presses_Stream.Subscribe(Handles_Click);

            void Handles_Click(TheVoid _)
            {
                if(!the_battle_component.has_a_Player(out var the_player))
                    return;
                
                the_player.Toggles_the_Dock_Selection();
            }
            
            void Updates_the_button()
            {
                if (the_battle_component.s_Selections_Cell.has_a_Value(out var the_selection) && the_selection.s_Unit.can_Dock_in_general())
                {
                    if (!the_selection.s_Unit.can_Move())
                    {
                        button.Becomes_Disabled();
                    }
                    else if (the_selection.has_the_Dock_Action_Selected())
                    {
                        button.Becomes_Active_With(settings.NormalHighlightedButtonMaterial, settings.PressedHighlightedButtonMaterial);
                    }
                    else
                    {
                        button.Becomes_Normal();
                    }

                    Show();
                }
                else
                {
                    Hide();
                }
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