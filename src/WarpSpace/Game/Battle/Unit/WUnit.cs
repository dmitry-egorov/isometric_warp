using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(WMover))]
    public class WUnit : MonoBehaviour
    {
        public MUnit s_Unit => its_unit;
        public bool is_Moving => its_mover.is_Moving;
        public IStream<TheVoid> s_Destruction_Signal => its_destruction_signal;

        public static WUnit Is_Created_From(WUnit prefab, MUnit unit)
        {
            var limbo = FindObjectOfType<WLimbo>().s_Transform;
            var board = FindObjectOfType<WBoard>();
            var parent = unit.is_At_a_Tile(out var tile) ? board[tile].s_UnitSlot.Transform : limbo;
            var the_player = FindObjectOfType<WGame>().s_Player;

            var obj = Instantiate(prefab, parent);

            obj.inits(unit, the_player);

            return obj;
        }
        
        public void Fast_Forwards_the_Movement() => its_mover.Fast_Forwards();
        public void Resumes_the_Movement_To_Normal_Speed() => its_mover.Resumes_Normal_Speed();

        public void OnDestroy()
        {
            its_subscriptions?.Invoke();
            its_destruction_signal.Next();
        }

        private void inits(MUnit the_unit, MPlayer the_player)
        {
            var unit_type = the_unit.s_Type;
            
            its_unit = the_unit;
            its_mover = GetComponent<WMover>();
            its_destruction_signal = new Stream<TheVoid>();
            
            it_inits_the_mesh();
            it_inits_the_outliner();
            it_wires_the_destruction();

            void it_inits_the_mesh() => GetComponentInChildren<UnitMesh>().Present(unit_type, the_unit.s_Faction);

            void it_inits_the_outliner()
            {
                var outliner = GetComponentInChildren<WOutliner>();

                if (the_player.Owns(the_unit))
                {
                    its_subscriptions = it_wires_the_selected_unit();
                }
                else
                {
                    Destroy(outliner.gameObject);
                }
            
                //Note: every unit's outline wires to the player
                Action it_wires_the_selected_unit() => 
                    FindObjectOfType<WGame>()
                        .s_Players_Selected_Units_Cell
                        .Subscribe(it_handles_the_selected_unit_change)
                ;
            
                void it_handles_the_selected_unit_change(Possible<MUnit> possibly_selected_unit)
                {
                    if (possibly_selected_unit.has_a_Value(out var the_selected_unit) && the_selected_unit == its_unit)
                    {
                        outliner.Shows();
                    }
                    else
                    {
                        outliner.Hides();
                    }
                }
            }

            void it_wires_the_destruction()
            {
                the_unit.s_Destruction_Signal
                    .First()
                    .Subscribe(isAlive => Destroy(gameObject));
            }
        }

        private MUnit its_unit;
        private Action its_subscriptions;
        private WMover its_mover;
        private Stream<TheVoid> its_destruction_signal;
    }
}