using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using UnityEngine.EventSystems;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.UI.Gameplay
{
    
    public class WeaponButton: MonoBehaviour, IPointerClickHandler
    {
        private Slot<PlayerModel> _playerSlot;

        public void Start()
        {
            var battle = FindObjectOfType<World.Battle.Component>();
            battle
                .GameCell
                .SelectMany(GetPlayersStream)
                .Subscribe(player => _playerSlot = player.value_or(null));
        }

        private static IStream<Slot<PlayerModel>> GetPlayersStream(Slot<GameModel> gr) => 
            gr
                .map(g => (IStream<Slot<PlayerModel>>)g.CurrentBattle.Select(br => br.map(b => b.Player)))
                .value_or(Stream.Empty<Slot<PlayerModel>>());

        public void OnPointerClick(PointerEventData eventData)
        {
            var player = _playerSlot.must_have_something().otherwise(new InvalidOperationException("Player not found"));
            player.SelectWeapon();
        }
    }
}