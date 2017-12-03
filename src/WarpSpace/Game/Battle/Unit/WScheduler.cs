using System;
using Lanski.Structures;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Game.Battle.Unit.Tasks;
using WarpSpace.Game.Tasks;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Game.Battle.Unit
{
    public class WScheduler
    {
        public WScheduler(WUnit the_owner, WAgenda the_agenda, WBoard the_board)
        {
            this.its_owner = the_owner;
            this.the_agenda = the_agenda;
            this.the_board = the_board;
            its_wiring = it_wires_units_movements();
        }
        
        public void Destructs() => it_destructs();
        
        private Action it_wires_units_movements() => its_owner.s_Unit.Moved().Subscribe(x => it_schedules_a_move(x.s_Source, x.s_Destination));
        private Action it_wires_weapon_firings() => its_owner.s_Unit.Fired().Subscribe(x => it_schedules_a_weapon_fire(x.s_Weapon, x.s_Target_Tile));
        private void it_destructs() => its_wiring();

        private void it_schedules_a_weapon_fire(MWeapon the_weapon, MTile the_target)
        {
            
        }

        private void it_schedules_a_move(MTile the_source_tile, MTile the_target_tile)
        {
            var the_orientation = the_source_tile.s_Direction_To(the_target_tile);
            var the_target_position = the_target_tile.s_Position;

            it_rotates_to(the_orientation);
            it_moves_to(the_target_position);
        }

        private Possible<ITask> it_moves_to(Index2D the_target_position)
        {
            var the_task_at_the_target = the_board[the_target_position].s_Tasks_Holder.s_Last_Task;
            it_waits_for(the_task_at_the_target);
            return it_schedules_a_task(new MovementTo(the_target_position)).as_a_Possible();
        }

        private Possible<ITask> it_rotates_to(Direction2D the_target_orientation) => 
            its_expected_orientation != the_target_orientation ? 
                it_schedules_a_task(new RotationTo(the_target_orientation)).as_a_Possible() : 
                the_agenda.s_Possible_Last_Task
        ;

        private Possible<ITask> it_waits_for(Possible<ITask> the_possible_task) => 
            the_possible_task.has_a_Value(out var the_task) ? 
                it_schedules_a_task(new WaitingForTask<WUnit>(the_task)).as_a_Possible() : 
                Possible.Empty<ITask>()
        ;
        private ITask it_schedules_a_task(ITaskVariant<WUnit> the_variant)
        {
            var the_possible_tile_tasks_holder = 
                its_possible_expected_position.has_a_Value(out var the_expected_position) 
                ? the_board[the_expected_position].s_Tasks_Holder 
                : Possible.Empty<WTasksHolder>()
            ;

            var the_new_task = new Task<WUnit>(the_variant, its_owner, the_agenda, the_possible_tile_tasks_holder);
            if (the_possible_tile_tasks_holder.has_a_Value(out var the_tasks_holder))
            {
                the_tasks_holder.Adds(the_new_task);
            }
            
            the_agenda.Schedules_a_Task(the_new_task);

            return the_new_task;
        }
        
        private Direction2D its_expected_orientation => 
            the_agenda.s_Last_Task_Where(the_task =>
                the_task.is_a(out RotationTo the_rotation) && the_rotation.s_Target_Orientation.@as(out var the_orientation) ||
                the_task.is_a(out ShowingUpAt the_showing) && the_showing.s_Target_Orientation.@as(out the_orientation) 
                    ? the_orientation 
                    : Possible.Empty<Direction2D>()
            )
            .has_a_Value(out var the_scheduled_orientation) 
                ? the_scheduled_orientation 
                : its_owner.s_Orientation.s_Value_Or(Direction2D.Left)
        ;

        private Possible<Index2D> its_possible_expected_position =>
            the_agenda.s_Last_Task_Where(the_task =>
                    the_task.is_a(out MovementTo the_movement) && the_movement.s_Target_Position.@as(out var the_position) ||
                    the_task.is_a(out ShowingUpAt the_showing_up) && the_showing_up.s_Target_Position.@as(out the_position)
                        ? the_position
                        : Possible.Empty<Index2D>()
            )
            .has_a_Value(out var the_scheduled_position)
                ? the_scheduled_position
                : its_owner.s_Position
        ;

        private readonly WUnit its_owner;
        private readonly WBoard the_board;
        private readonly WAgenda the_agenda;
        private readonly Action its_wiring;
    }
}