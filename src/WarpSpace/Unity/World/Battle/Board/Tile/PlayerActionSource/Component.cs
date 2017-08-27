using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.Unity.World.Battle.Board.Tile.PlayerActionSource
{
    public class Component: MonoBehaviour
    {
        private Stream<TheVoid> _actions;
        public IStream<TheVoid> Actions => _actions;

        public void Init()
        {
            _actions = new Stream<TheVoid>();
        }

        void OnMouseDown()
        {
            _actions.Next();
        }
    }
}