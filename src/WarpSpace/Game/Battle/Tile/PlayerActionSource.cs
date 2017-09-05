using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.Game.Battle.Tile
{
    public class PlayerActionSource: MonoBehaviour
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