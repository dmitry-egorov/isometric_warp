using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.EventSystems;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.UI.Common;

namespace WarpSpace.UI.Gameplay.Weapon
{
    [RequireComponent(typeof(ClickSource))]
    public class WeaponClicksHandler: MonoBehaviour
    {
        private Slot<PlayerModel> _playerSlot;

        public void Start()
        {
            var clicks = GetComponent<ClickSource>();
            var battle = FindObjectOfType<Unity.World.Battle.Component>();
            Wire_Player_Slot_Variable();

            void Wire_Player_Slot_Variable() =>
                battle
                .Player_Cell
                .SelectMany(player => clicks.Clicks.Select(c => player))
                .Subscribe(HandleClick);
            ;
            
            void HandleClick(Slot<PlayerModel> player) => 
                player
                .Must_Have_a_Value()
                .Otherwise(new InvalidOperationException("Player not found"))
                .Toggle_Weapon_Selection()
            ;
        }

        
    }
}