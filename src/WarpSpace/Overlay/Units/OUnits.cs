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
            battle.s_Battles_Cell.SelectMany(it_selects_the_unit_creation_stream).Subscribe(it_create_an_overlay_unit);
        }

        private void it_create_an_overlay_unit(WUnit unit_component)
        {
            Debug.Log("New Unit!");

            Instantiate(UnitPrefab, transform).Init(unit_component);
        }

        private IStream<WUnit> it_selects_the_unit_creation_stream(Possible<MBattle> possible_battle)
        {
            Debug.Log("New Battle!");
            gameObject.DestroyChildren();
            
            return possible_battle.has_a_Value() 
                ? FindObjectOfType<WBoard>().s_Created_Units_Stream 
                : Stream.Empty<WUnit>();
        }
    }
}