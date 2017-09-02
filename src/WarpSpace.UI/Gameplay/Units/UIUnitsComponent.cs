using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Unity.World.Battle.Unit;
using Component = WarpSpace.Unity.World.Battle.Board.Component;

namespace WarpSpace.UI.Gameplay.Units
{
    public class UIUnitsComponent: MonoBehaviour
    {
        public GameObject UnitPrefab;
        
        public void Start()
        {
            var battle = FindObjectOfType<Unity.World.Battle.Component>();
            battle.Battle_Cell.SelectMany(SelectWorldUnitsStream).Subscribe(CreateUiUnitComponent);
        }

        private void CreateUiUnitComponent(UnitComponent obj)
        {
            var unit = Instantiate(UnitPrefab, transform).GetComponent<UiUnitComponent>();
            unit.Init(obj);
        }

        private IStream<UnitComponent> SelectWorldUnitsStream(Slot<BattleModel> arg)
        {
            gameObject.DestroyChildren();
            
            return arg.Has_a_Value() 
                ? FindObjectOfType<Component>().Stream_Of_Created_Units 
                : Stream.Empty<UnitComponent>();
        }
    }
}