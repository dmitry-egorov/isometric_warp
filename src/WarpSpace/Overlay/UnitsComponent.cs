using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle;

namespace WarpSpace.Overlay.Units
{
    public class UnitsComponent: MonoBehaviour
    {
        public GameObject UnitPrefab;
        
        public void Start()
        {
            var battle = FindObjectOfType<BattleComponent>();
            battle.Battle_Cell.SelectMany(SelectWorldUnitsStream).Subscribe(CreateUiUnitComponent);
        }

        private void CreateUiUnitComponent(UnitComponent obj)
        {
            var unit = Instantiate(UnitPrefab, transform).GetComponent<Unit>();
            unit.Init(obj);
        }

        private IStream<UnitComponent> SelectWorldUnitsStream(Possible<MBattle> arg)
        {
            gameObject.DestroyChildren();
            
            return arg.Has_a_Value() 
                ? FindObjectOfType<BoardComponent>().Stream_Of_Created_Units 
                : Stream.Empty<UnitComponent>();
        }
    }
}