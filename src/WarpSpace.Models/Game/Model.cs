using Lanski.Reactive;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game
{
    public class Model
    {
        private readonly BoardDescription _boardDescription;

        private readonly RefCell<Battle.Model> _currentBattle;
        public ICell<Battle.Model> CurrentBattle => _currentBattle;

        public Model(BoardDescription boardDescription)
        {
            _boardDescription = boardDescription;
            _currentBattle = new RefCell<Battle.Model>(null);
        }

        public void Start()
        {
            Restart();
        }
        
        public void Restart()
        {
            var battle = Battle.Factory.From(_boardDescription);

            _currentBattle.Value = battle;
            
            battle.Start();
        }
    }
}