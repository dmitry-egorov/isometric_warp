using System.Collections.Generic;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle;

namespace WarpSpace.Game.Battle.BoardOverlay
{
    public class WAgendasOverlay : MonoBehaviour
    {
        public GameObject UnitPrefab;

        public void Awake()
        {
            its_units = new List<WAgendaPresenter>();
        }
        
        public void Start()
        {
            var the_world_board = FindObjectOfType<WBoard>();

            the_world_board.s_New_Battles_Stream.Subscribe(it_destroys_the_children);
            the_world_board.s_Created_Units_Stream.Subscribe(it_create_an_agend_presenter);
        }

        private void it_destroys_the_children(Possible<MBattle> the_obj)
        {
            its_units.Clear();
            gameObject.DestroyChildren();
        }

        private void it_create_an_agend_presenter(WUnit the_world_unit)
        {
            var obj = Instantiate(UnitPrefab, transform);
            var the_unit = new WAgendaPresenter(obj, the_world_unit);
            its_units.Add(the_unit);
            the_world_unit.s_Destruction_Signal.Subscribe(_ =>
            {
                Destroy(obj);
                its_units.Remove(the_unit);
            });
        }
        
        private List<WAgendaPresenter> its_units;
    }
}