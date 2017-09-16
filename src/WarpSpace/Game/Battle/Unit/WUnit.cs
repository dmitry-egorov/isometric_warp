using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
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
        public Transform s_Tarnsform => its_transform;
        
        public bool is_Moving => its_agenda.has_a_Target;
        public IStream<WAgenda.Change> s_Agenda_Changed => its_agenda.s_Changes_Stream;
        public IStream<TheVoid> s_Movements => its_mover.s_Movements;
        public IStream<TheVoid> s_Destruction_Signal => its_destruction_signal;

        public static WUnit Is_Created_From(WUnit the_prefab, MUnit the_unit) => it_is_created_from(the_prefab, the_unit);
        public void Fast_Forwards_the_Movement() => its_mover.Fast_Forwards();
        public void Resumes_the_Movement_To_Normal_Speed() => its_mover.Resumes_Normal_Speed();

        public void Update() => its_mover.Updates();
        public void OnDestroy() => it_destructs();
        
        private static WUnit it_is_created_from(WUnit prefab, MUnit unit)
        {
            var limbo = FindObjectOfType<WLimbo>().s_Transform;
            var board = FindObjectOfType<WBoard>();
            var parent = unit.is_At_a_Tile(out var tile) ? board[tile].s_UnitSlot.Transform : limbo;

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
            its_agenda = new WAgenda(this, the_limbo, the_board);
            its_mover = new WMover(its_agenda, UnitTypeSettings.Of(the_unit.s_Type).Movement, BoostSpeedMultiplier, its_transform);
            its_mesh_presenter = new UnitMeshPresenter(UnitMeshRenderer);
            
            its_destruction_signal = new Stream<TheVoid>();

            it_inits_the_mesh();
            it_destroys_itself_on_destruction_of_the_unit();
            it_deactivates_in_the_limbo();
            it_deactivates_upon_reaching_the_limbo_and_resumes_when_leaving_it();
        }

        void it_inits_the_mesh() => its_mesh_presenter.Presents(its_unit);

        void it_deactivates_upon_reaching_the_limbo_and_resumes_when_leaving_it()
        {
            its_agenda.s_Changes_Stream
                .Subscribe(e =>
                {
                    if (e.is_Teleported_To(out var the_parent) && the_parent == the_limbo)
                        gameObject.Hides();
                    if (e.is_Enqueued_a_Target(out var the_target) && the_target.s_Parent != the_limbo && its_transform.parent == the_limbo)
                        gameObject.Shows();
                });
        }

        void it_destroys_itself_on_destruction_of_the_unit() => 
            its_unit.s_Destruction_Signal
                .First()
                .Subscribe(isAlive => Destroy(gameObject))
        ;

        private void it_deactivates_in_the_limbo() => gameObject.SetActive(its_transform.parent != the_limbo);

        private void it_destructs()
        {
            its_agenda.Destructs();
            its_outliner.Destructs();
            its_destruction_signal.Next();
        }

        private Transform the_limbo;
        private MUnit its_unit;
        private WAgenda its_agenda;
        private WMover its_mover;
        private WOutliner its_outliner;
        private UnitMeshPresenter its_mesh_presenter;
        private Stream<TheVoid> its_destruction_signal;
        private Transform its_transform;
        private WGame the_game;
        private WBoard the_board;
    }
}