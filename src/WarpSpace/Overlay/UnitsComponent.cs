using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle;

namespace WarpSpace.Overlay
{
    public class UnitsComponent: MonoBehaviour
    {
        public GameObject UnitPrefab;
        
        public void Start()
        {
            var battle = FindObjectOfType<BattleComponent>();
            battle.s_Battles_Cell().SelectMany(SelectWorldUnitsStream).Subscribe(CreateUiUnitComponent);
        }

        private void CreateUiUnitComponent(UnitComponent unit_component)
        {
            var unit = Instantiate(UnitPrefab, transform).GetComponent<OUnit>();
            unit.Init(unit_component);
        }

        private IStream<UnitComponent> SelectWorldUnitsStream(Possible<MBattle> possible_battle)
        {
            gameObject.DestroyChildren();
            
            return possible_battle.Has_a_Value() 
                ? FindObjectOfType<BoardComponent>().Stream_Of_Created_Units 
                : Stream.Empty<UnitComponent>();
        }
    }
}