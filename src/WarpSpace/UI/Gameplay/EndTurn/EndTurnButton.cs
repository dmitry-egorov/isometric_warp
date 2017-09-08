using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.EndTurn
{
    [RequireComponent(typeof(UIButton))]
    public class EndTurnButton : MonoBehaviour
    {
        void Start()
        {
            the_battle_component = FindObjectOfType<BattleComponent>();
            GetComponent<UIButton>().s_Presses_Stream.Subscribe(Handle_Button_Press);
        }

        private void Handle_Button_Press(TheVoid _)
        {
            if (the_battle_component.has_a_Battle(out var the_battle))
            {
                the_battle.Ends_the_Turn();
            }
        }

        private BattleComponent the_battle_component;
    }
}