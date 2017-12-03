using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common.Behaviours;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Game.Tasks;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class WUnit
    {
        public MUnit s_Unit => its_unit;
        public WScheduler s_Scheduler => its_scheduler;
        public Transform s_Transform => its_transform;

        public bool is_Moving => its_agenda.has_a_Task;

        public Possible<Direction2D> s_Orientation => its_spacial.s_Orientation;
        public Possible<Index2D> s_Position => its_spacial.s_Position;
        public IStream<WAgenda.Change> s_Agenda_Changed => its_agenda.Changed;
        public IStream<TheVoid> s_Movements => its_mover.s_Movements;
        public IStream<TheVoid> s_Destruction_Signal => its_destruction_signal;

        public void Updates() => its_agenda.Updates();
        public void Destructs() => it_destructs();

        public bool Moves_To(Index2D the_target_position) => its_mover.Moves_To(the_target_position);
        public bool Rotates_To(Direction2D the_orientation) => its_rotator.Rotates_To(the_orientation);
        public void Hides()
        {
            its_teleporter.Teleports_To(Possible.Empty<Index2D>(), Direction2D.Left);
            its_visibility.Hides();
        }

        public void Shows_Up_At(Index2D the_position, Direction2D the_orientation)
        {
            its_teleporter.Teleports_To(the_position, the_orientation);
            its_visibility.Shows();
        }

        public WUnit
        (
            MUnit the_unit, 
            WGame the_game, 
            Transform the_limbo, 
            WBoard the_board, 
            Transform the_transform, 
            MeshRenderer the_unit_mesh_renderer, 
            GameObject the_game_object, 
            Outline the_outline
        )
        {
            var the_time = the_board.s_Time;
            
            its_unit = the_unit;
            its_transform = the_transform;
            its_game_object = the_game_object;
            its_outliner = new WOutliner(the_game, the_unit, the_outline);
            its_spacial = new WSpacial(the_unit.s_Possible_Position(), its_transform, the_limbo, the_board);
            its_agenda = new WAgenda();
            var the_mesh_presenter = new UnitMeshPresenter(the_unit_mesh_renderer);
            its_visibility = new WVisibility(the_unit, the_mesh_presenter);
            
            its_scheduler = new WScheduler(this, its_agenda, the_board);

            var the_movement_settings = UnitTypeSettings.Of(the_unit.s_Type).Movement;
            its_mover = new WMover(the_movement_settings, the_time, its_spacial);
            its_rotator = new WRotator(the_movement_settings, the_time, its_spacial);
            its_teleporter = new WTeleporter(its_spacial, its_mover, its_rotator);

            its_destruction_signal = new Stream<TheVoid>();
            
            its_agenda.Changed.Subscribe(e => 
                Debug.Log($"{the_unit.s_Name} {e}. " + 
                          $"Remaining: {its_agenda.s_Tasks_Count}. " + 
                          $"Next task: {its_agenda.s_Possible_Next_Task}")
            );

            it_inits_the_mesh();
            it_destroys_itself_on_destruction_of_the_unit();
        }

        private void it_inits_the_mesh()
        {
            if (its_unit.is_Docked())
            {
                its_visibility.Hides();
            }
            else
            {
                its_visibility.Shows();
            }
        }

        private void it_destroys_itself_on_destruction_of_the_unit() => 
            its_unit.Been_Destroyed()
                .First()
                .Subscribe(_ => Object.Destroy(its_game_object))
        ;

        private void it_destructs()
        {
            its_scheduler.Destructs();
            its_outliner.Destructs();
            its_destruction_signal.Next();
        }

        private readonly WSpacial its_spacial;
        private readonly MUnit its_unit;
        private readonly WScheduler its_scheduler;
        private readonly WAgenda its_agenda;
        private readonly WOutliner its_outliner;
        private readonly Stream<TheVoid> its_destruction_signal;
        private readonly Transform its_transform;
        private readonly WVisibility its_visibility;
        private readonly WMover its_mover;
        private readonly WRotator its_rotator;
        private readonly WTeleporter its_teleporter;
        private readonly GameObject its_game_object;
    }
}