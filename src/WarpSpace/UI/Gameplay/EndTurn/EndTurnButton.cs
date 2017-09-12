using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game;
using WarpSpace.Game.Battle;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.EndTurn
{
    [RequireComponent(typeof(UIButton))]
    public class EndTurnButton : MonoBehaviour
    {
        void Start()
        {
            the_battle_component = FindObjectOfType<WGame>();
            GetComponent<UIButton>().s_Presses_Stream.Subscribe(Handle_Button_Press);
        }

        private void Handle_Button_Press(TheVoid _)
        {
            if (the_battle_component.has_a_Battle(out var the_battle))
            {
                the_battle.Ends_the_Turn();
            }
        }

        private WGame the_battle_component;
    }
}