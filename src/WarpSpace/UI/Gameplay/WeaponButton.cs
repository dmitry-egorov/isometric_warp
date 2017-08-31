using System;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.EventSystems;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.UI.Gameplay
{
    public class WeaponButton: MonoBehaviour, IPointerClickHandler
    {
        private Slot<PlayerModel> _playerSlot;

        public void Start()
        {
            var battle = FindObjectOfType<Unity.World.Battle.Component>();
            Wire_Player_Slot_Variable();
            
            void Wire_Player_Slot_Variable() => 
                battle
                .Player_Cell
                .Subscribe(player => _playerSlot = player)
            ;
        }

        public void OnPointerClick(PointerEventData eventData) => 
            _playerSlot
                .Must_Have_a_Value()
                .Otherwise(new InvalidOperationException("Player not found"))
                .Toggle_Weapon_Selection()
        ;
    }
}