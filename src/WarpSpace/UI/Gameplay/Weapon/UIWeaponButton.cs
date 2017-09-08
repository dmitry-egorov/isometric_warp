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

            wires_clicks();
            wires_weapon_selection();
            wires_weapon_fires();
            wires_turn_ends();

            void wires_clicks() =>
                the_button.s_Presses_Stream
                    .Subscribe(handles_click)
            ;
            
            void handles_click(TheVoid the_the_void)
            {
                if (!the_battle_component.has_a_Player(out var the_player))
                    return;

                the_player.Toggles_the_Weapon_Selection();
            }

            void wires_weapon_fires() =>
                the_battle_component.s_Selected_Units_Cell
                    .SelectMany(pu => pu.Select_Stream_Or_Empty(u => u.s_Weapon.s_Fires_Stream))
                    .Subscribe(_ => updates_the_button())
            ;
            
            void wires_weapon_selection() =>
                the_battle_component.s_Players_Cell
                    .SelectMany(pp => pp.Select(p => p.s_Selected_Weapons_Cell).Cell_Or_Single_Default())
                    .Subscribe(_ => updates_the_button())
            ;

            void wires_turn_ends() =>
                the_battle_component.s_Battles_Cell
                    .SelectMany(pb => pb.Select_Stream_Or_Empty(b => b.Board.s_Turn_Ends_Stream))
                    .Subscribe(_ => updates_the_button())
            ;
            
            void updates_the_button()
            {
                if (the_battle_component.s_Selections_Cell.s_Value.has_a_Value(out var the_selection))
                {
                    if (!the_selection.s_Unit.s_Weapon.can_Fire())
                    {
                        the_button.Becomes_Disabled();
                    }
                    else if (the_selection.has_a_Weapon_Selected())
                    {
                        the_button.Becomes_Active_With(settings.NormalWarningButtonMaterial,settings.PressedWarningButtonMaterial);
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