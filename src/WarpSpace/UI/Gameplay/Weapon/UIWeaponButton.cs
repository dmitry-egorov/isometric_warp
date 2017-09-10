using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Settings;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.Weapon
{
    [RequireComponent(typeof(UIButton))]
    public class UIWeaponButton : MonoBehaviour
    {
        public void Start()
        {
            var settings = FindObjectOfType<UISettingsHolder>();
            var the_button = GetComponent<UIButton>();
            var the_battle_component = FindObjectOfType<BattleComponent>();

            it_wires_clicks();
            it_wires_weapon_selection();
            it_wires_weapon_fires();
            it_wires_turn_ends();

            void it_wires_clicks() =>
                the_button.s_Presses_Stream
                    .Subscribe(it_handles_a_click)
            ;
            
            void it_handles_a_click(TheVoid the_the_void)
            {
                if (!the_battle_component.has_a_Player(out var the_player))
                    return;

                the_player.Toggles_the_Weapon_Selection();
            }

            void it_wires_weapon_fires() =>
                the_battle_component.s_Players_Selected_Units_Cell
                    .SelectMany(pu => pu.Select_Stream_Or_Empty(the_slected_unit => the_slected_unit.s_Weapon.s_Fires_Stream))
                    .Subscribe(_ => it_updates_the_button())
            ;
            
            void it_wires_weapon_selection() =>
                the_battle_component.s_Players_Cell
                    .SelectMany(pp => pp.Select(the_player => the_player.s_Selected_Weapons_Cell).Cell_Or_Single_Default())
                    .Subscribe(_ => it_updates_the_button())
            ;

            void it_wires_turn_ends() =>
                the_battle_component.s_Battles_Cell
                    .SelectMany(pb => pb.Select_Stream_Or_Empty(the_battle => the_battle.s_Board.s_Turn_Ends_Stream))
                    .Subscribe(_ => it_updates_the_button())
            ;
            
            void it_updates_the_button()
            {
                if (the_battle_component.s_Players_Selections_Cell.s_Value.has_a_Value(out var the_selection))
                {
                    if (!the_selection.s_Unit.s_Weapon.can_Fire())
                    {
                        the_button.Becomes_Disabled();
                    }
                    else if (the_selection.has_a_Weapon_Selected())
                    {
                        the_button.Becomes_Active_With(settings.NormalWarningButtonMaterial, settings.PressedWarningButtonMaterial);
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