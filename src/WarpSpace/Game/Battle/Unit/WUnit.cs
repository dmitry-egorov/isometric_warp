using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common.Behaviours;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class WUnit : MonoBehaviour
    {
        public float BoostSpeedMultiplier;
        public MeshRenderer UnitMeshRenderer;

        public MUnit s_Unit => its_unit;
        public WAgenda s_Agenda => its_agenda;
        public Transform s_Tarnsform => its_transform;

        public bool is_Moving => its_agenda.has_a_Task;

        public Possible<Direction2D> s_Orientation => its_spacial.s_Orientation;
        public Possible<Index2D> s_Position => its_spacial.s_Position;
        public IStream<WAgenda.Change> s_Agenda_Changed => its_agenda.Changed;
        public IStream<TheVoid> s_Movements => its_executor.s_Movements;
        public IStream<TheVoid> s_Destruction_Signal => its_destruction_signal;

        public static WUnit Is_Created_From(WUnit the_prefab, MUnit the_unit) => it_is_created_from(the_prefab, the_unit);
        public void Fast_Forwards_the_Movement() => its_executor.Fast_Forwards();
        public void Resumes_the_Movement_To_Normal_Speed() => its_executor.Resumes_Normal_Speed();

        public void Update() => its_executor.Updates();
        public void OnDestroy() => it_destructs();
        
        private static WUnit it_is_created_from(WUnit prefab, MUnit unit)
        {
            var limbo = FindObjectOfType<WLimbo>().s_Transform;
            var board = FindObjectOfType<WBoard>();
            var parent = unit.is_At_a_Tile(out var tile) ? board[tile].s_Unit_Slot.s_Transform : limbo;

            var obj = Instantiate(prefab, parent);

            obj.inits(unit);

            return obj;
        }

        private void inits(MUnit the_unit)
        {
            the_game = FindObjectOfType<WGame>();
            the_limbo = FindObjectOfType<WLimbo>().s_Transform;
            the_board = FindObjectOfType<WBoard>();
            
            its_unit = the_unit;
            its_transform = transform;
            its_outliner = new WOutliner(this, the_game);
            its_spacial = new WSpacial(the_unit.s_Position, its_transform, the_limbo, the_board);
            its_agenda = new WAgenda(this, the_board);
            its_scheduler = new WScheduler(this, its_agenda);
            its_executor = new WExecutor(its_agenda, UnitTypeSettings.Of(the_unit.s_Type).Movement, BoostSpeedMultiplier, its_spacial);
            its_mesh_presenter = new UnitMeshPresenter(UnitMeshRenderer);
            
            its_destruction_signal = new Stream<TheVoid>();

            it_inits_the_mesh();
            it_destroys_itself_on_destruction_of_the_unit();
            it_hides_rendering_upon_hiding_and_resumes_it_upon_showing();
        }

        void it_inits_the_mesh()
        {
            if (its_unit.is_Docked)
            {
                it_hides();
            }
            else
            {
                it_shows();
            }
        }

        private void it_shows()
        {
            its_mesh_presenter.Presents(its_unit);
        }

        private void it_hides()
        {
            its_mesh_presenter.Hides();
        }

        void it_hides_rendering_upon_hiding_and_resumes_it_upon_showing()
        {
            its_agenda.Changed
                .Subscribe(the_change =>
                {
                    if (!the_change.is_a_Task_Completion(out var the_task)) 
                        return;
                    
                    if (the_task.is_to_Hide())
                    {
                        its_mesh_presenter.Hides();
                    }
                    else if (the_task.is_to_Show_Up())
                    {
                        its_mesh_presenter.Presents(its_unit);
                    }
                });
        }

        void it_destroys_itself_on_destruction_of_the_unit() => 
            its_unit.Destructed
                .First()
                .Subscribe(isAlive => Destroy(gameObject))
        ;

        private void it_destructs()
        {
            its_scheduler.Destructs();
            its_outliner.Destructs();
            its_destruction_signal.Next();
        }

        private WSpacial its_spacial;
        private MUnit its_unit;
        private WScheduler its_scheduler;
        private WAgenda its_agenda;
        private WExecutor its_executor;
        private WOutliner its_outliner;
        private UnitMeshPresenter its_mesh_presenter;
        private Stream<TheVoid> its_destruction_signal;
        private Transform its_transform;
        private WGame the_game;
        private WBoard the_board;
        private Transform the_limbo;
    }
}