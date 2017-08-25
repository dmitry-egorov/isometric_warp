using UnityEngine;

namespace WarpSpace.World.Board.Tile.Commander
{
    public class Component: MonoBehaviour
    {
        private Player.Component _player;
        private Tile.Component _tile;

        public void Init(Player.Component player, Tile.Component tile)
        {
            _player = player;
            _tile = tile;
        }

        void OnMouseDown()
        {
            _player.ExecuteActionAt(_tile);
        }
    }
}