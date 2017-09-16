using System.Collections.Generic;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Game.Battle.BoardOverlay
{
    public class WAgendaPresenter
    {
        private readonly GameObject its_game_object;
        private readonly WUnit the_world_unit;

        public WAgendaPresenter(GameObject the_game_object, WUnit the_world_unit)
        {
            this.its_game_object = the_game_object;
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
            if (change.is_Enqueued_a_Task(out var the_task))
            {
                if (the_task.is_to_Move_To(out var the_parent))
                {
                    its_points_list.Add(the_parent.position);
                    it_updates_all_points();
                }
                else if (the_task.is_to_Show_At(out the_parent, out var _))
                {
                    its_game_object.Shows();
                    it_updates_the_first_point(the_parent.position);
                }
            }

            if (change.is_Completed_a_Task(out the_task))
            {
                if (the_task.is_to_Move())
                {
                    its_points_list.RemoveAt(1);
                    it_updates_all_points();
                }
                else if(the_task.is_to_Hide())
                {
                    its_game_object.Hides();
                }
            }

            if (change.is_Replaced_a_Task(out the_task) && the_task.is_to_Move_To(out var the_new_parent))
            {
                it_updates_the_last_point(the_new_parent.position);
            }
        }

        void it_updates_the_first_point()
        {
            var current_position = the_world_unit.s_Tarnsform.position;
            it_updates_the_first_point(current_position);
        }

        private void it_updates_the_first_point(Vector3 the_position)
        {
            its_points_list[0] = the_position;
            its_line_renderer.SetPosition(0, the_position);
        }

        private void it_updates_the_last_point(Vector3 the_position)
        {
            var last_index = its_points_list.Count - 1;
            its_points_list[last_index] = the_position;
            its_line_renderer.SetPosition(last_index, the_position);
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