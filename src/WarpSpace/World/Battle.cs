using UnityEditor.Expose;
using UnityEngine;

namespace WarpSpace.World
{
    public class Battle: MonoBehaviour
    {
        private bool _initialized;
        private Board.Factory _boardFactory;
        private Board.Gameplay.Layer _gameplay;

        private void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            _boardFactory = FindObjectOfType<Board.Factory>();
            _gameplay = FindObjectOfType<Board.Gameplay.Layer>();
        }
        
        [ExposeMethodInEditor]
        public void Begin()
        {
            _boardFactory.Create();
            _gameplay.SpawnPlayer();
        }
    }
}