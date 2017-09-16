using System.Collections.Generic;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Game.Battle.BoardOverlay
{
    public class WAgendaPresenter
    {
        private readonly WUnit the_world_unit;

        public WAgendaPresenter(GameObject the_game_object, WUnit the_world_unit)
        {
            this.the_world_unit = the_world_unit;
            its_line_renderer = the_game_object.GetComponent<LineRenderer>();
            
            its_points_list = new List<Vector3>(4) { Vector3.zero };
            it_updates_all_points();
            it_updates_the_first_point();

            the_world_unit.s_Agenda_Changed.Subscribe(it_handles_the_agenda_change);

            the_world_unit.s_Movements.Subscribe(_ => it_updates_the_first_point());
        }

        private void it_handles_the_agenda_change(WAgenda.Change change)
        {
            if (change.is_Enqueued_a_Target(out var the_target))
            {
                its_points_list.Add(the_target.s_Parent.position);
                it_updates_all_points();
            }
            else
            {
                its_points_list.RemoveAt(1);
                it_updates_all_points();
            }
        }

        void it_updates_the_first_point()
        {
            var current_position = the_world_unit.s_Tarnsform.position;
            its_points_list[0] = current_position;
            its_line_renderer.SetPosition(0, current_position);
        }

        private void it_updates_all_points()
        {
            its_line_renderer.positionCount = its_points_list.Count;
            its_line_renderer.SetPositions(its_points_list.ToArray());
        }

        private readonly LineRenderer its_line_renderer;
        private readonly List<Vector3> its_points_list;
    }
}