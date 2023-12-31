﻿using System.Collections;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game;
using WarpSpace.Game.Battle.Board;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.EndTurn
{
    [RequireComponent(typeof(UIButton))]
    public class EndTurnButton : MonoBehaviour
    {
        void Start()
        {
            the_world_board = FindObjectOfType<WBoard>();
            its_button = GetComponent<UIButton>();
            its_button.s_Presses_Stream.Subscribe(Handle_Button_Press);
        }

        private void Handle_Button_Press(TheVoid _)
        {
            StartCoroutine(Ends_The_Turn());
        }

        private IEnumerator Ends_The_Turn()
        {
            its_button.Becomes_Disabled();

            yield return StartCoroutine(the_world_board.Ends_the_Turn());

            its_button.Becomes_Normal();
        }

        private WBoard the_world_board;
        private UIButton its_button;
    }
}