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
        public bool is_Moving => its_movement_queue.has_a_Target;
        public IStream<TheVoid> s_Destruction_Signal => its_destruction_signal;

        public static WUnit Is_Created_From(WUnit prefab, MUnit unit) => it_is_created(prefab, unit);
        public void Fast_Forwards_the_Movement() => its_mover.Fast_Forwards();
        public void Resumes_the_Movement_To_Normal_Speed() => its_mover.Resumes_Normal_Speed();

        public void Update() => its_mover.Updates();
        public void OnDestroy() => it_destructs();
        
        private static WUnit it_is_created(WUnit prefab, MUnit unit)
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
            its_movement_queue = new WMovementQueue(this, the_limbo, the_board);
            its_mover = new WMover(its_movement_queue, UnitTypeSettings.Of(the_unit.s_Type).Movement, BoostSpeedMultiplier, its_transform);
            its_mesh_presenter = new UnitMeshPresenter(UnitMeshRenderer);
            
            its_destruction_signal = new Stream<TheVoid>();

            it_inits_the_mesh();
            it_destroys_itself_on_destruction_of_the_unit();
            it_deactivates_in_limbo();
            it_deactivates_upon_reaching_the_limbo();
        }

        void it_inits_the_mesh() => its_mesh_presenter.Presents(its_unit);

        void it_deactivates_upon_reaching_the_limbo()
        {
            its_movement_queue.s_Movement_Events_Stream
                .Subscribe(e =>
                {
                    if (e.is_Teleported_To(out var _))
                        it_deactivates_in_limbo();
                });
        }

        void it_destroys_itself_on_destruction_of_the_unit() => 
            its_unit.s_Destruction_Signal
                .First()
                .Subscribe(isAlive => Destroy(gameObject))
        ;

        private void it_deactivates_in_limbo() => gameObject.SetActive(its_transform.parent != the_limbo);

        private void it_destructs()
        {
            its_movement_queue.Destructs();
            its_outliner.Destructs();
            its_destruction_signal.Next();
        }

        private Transform the_limbo;
        private MUnit its_unit;
        private WMovementQueue its_movement_queue;
        private WMover its_mover;
        private WOutliner its_outliner;
        private UnitMeshPresenter its_mesh_presenter;
        private Stream<TheVoid> its_destruction_signal;
        private Transform its_transform;
        private WGame the_game;
        private WBoard the_board;
    }
}