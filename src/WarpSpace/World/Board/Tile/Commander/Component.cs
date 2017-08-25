using UnityEngine;

namespace WarpSpace.World.Board.Tile.Commander
{
    public class Component: MonoBehaviour
    {
        private Player.Component _board;
        private Tile.Component _tile;

        public void Init(Player.Component player, Tile.Component tile)
        {
            _board = player;
            _tile = tile;
        }

        void OnMouseDown()
        {
            _board.ExecuteActionAt(_tile);
        }
    }
}