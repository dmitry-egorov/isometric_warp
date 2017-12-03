using UnityEngine;
using WarpSpace.Common.Behaviours;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Game.Battle.Unit
{
    public class CUnit : MonoBehaviour
    {
        public MeshRenderer MainMeshRenderer;
        
        public WUnit s_World_Unit => its_world_unit;

        public static CUnit Is_Created_From(CUnit the_prefab, MUnit the_unit)
        {
            var board = FindObjectOfType<WBoard>();
            var parent = board[the_unit.s_Location()].s_Unit_Slot.s_Transform;

            var obj = Instantiate(the_prefab, parent);

            obj.inits(the_unit);

            return obj;
        }

        public void Update() => its_world_unit.Updates();
        public void OnDestroy() => its_world_unit.Destructs();

        private void inits(MUnit the_unit)
        {
            var the_game = FindObjectOfType<WGame>();
            var the_limbo = FindObjectOfType<WLimbo>().s_Transform;
            var the_board = FindObjectOfType<WBoard>();

            var the_outline = gameObject.GetComponentInChildren<Outline>();
            its_world_unit = new WUnit(the_unit, the_game, the_limbo, the_board, transform, MainMeshRenderer, gameObject, the_outline); 
        }
        
        private WUnit its_world_unit;
    }
}