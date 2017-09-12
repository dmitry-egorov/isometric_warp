using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Overlay.Units;

namespace WarpSpace.Overlay
{
    public class OBoard: MonoBehaviour
    {
        public OUnit UnitPrefab;
        
        public void Start()
        {
            var the_world_board = FindObjectOfType<WBoard>();

            the_world_board.s_New_Battles_Stream.Subscribe(it_destroys_the_children);
            the_world_board.s_Created_Units_Stream.Subscribe(it_create_an_overlay_unit);
        }

        private void it_destroys_the_children(Possible<MBattle> the_obj)
        {
            gameObject.DestroyChildren();
        }

        private void it_create_an_overlay_unit(WUnit unit_component)
        {
            Instantiate(UnitPrefab, transform).Init(unit_component);
        }
    }
}