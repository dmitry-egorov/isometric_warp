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
    public class OUnits: MonoBehaviour
    {
        public OUnit UnitPrefab;
        
        public void Start()
        {
            var battle = FindObjectOfType<BattleComponent>();
            battle.s_Battles_Cell.SelectMany(SelectWorldUnitsStream).Subscribe(CreateUiUnitComponent);
        }

        private void CreateUiUnitComponent(WUnit unit_component)
        {
            var unit = Instantiate(UnitPrefab, transform).GetComponent<OUnit>();
            unit.Init(unit_component);
        }

        private IStream<WUnit> SelectWorldUnitsStream(Possible<MBattle> possible_battle)
        {
            gameObject.DestroyChildren();
            
            return possible_battle.has_a_Value() 
                ? FindObjectOfType<BoardComponent>().s_Created_Units_Stream 
                : Stream.Empty<WUnit>();
        }
    }
}