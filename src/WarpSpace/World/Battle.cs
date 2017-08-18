using UnityEditor.Expose;
using UnityEngine;

namespace WarpSpace.World
{
    public class Battle: MonoBehaviour
    {
        private bool _initialized;
        private Board.Factory _boardFactory;

        private void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            _boardFactory = FindObjectOfType<Board.Factory>();
        }
        
        [ExposeMethodInEditor]
        public void Begin()
        {
            Initialize();
            
            _boardFactory.Create();
        }
    }
}