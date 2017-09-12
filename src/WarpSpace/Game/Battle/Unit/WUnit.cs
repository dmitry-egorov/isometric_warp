using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Overlay.Units;
using static WarpSpace.Models.Descriptions.Faction;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(WMover))]
    public class WUnit : MonoBehaviour
    {
        public MUnit s_Unit => its_unit;

        public static WUnit Is_Created_From(WUnit prefab, MUnit unit)
        {
            var limbo = FindObjectOfType<WLimbo>().s_Transform;
            var board = FindObjectOfType<WBoard>();
            var parent = unit.is_At_a_Tile(out var tile) ? board[tile].UnitSlot.Transform : limbo;

            var obj = Instantiate(prefab, parent);

            obj.inits(unit);

            return obj;
        }

        public void OnDestroy()
        {
            its_subscriptions?.Invoke();
        }

        private void inits(MUnit the_unit)
        {
            var unit_type = the_unit.s_Type;
            
            its_unit = the_unit;
            its_transform = transform;
            its_transform.localRotation = Direction2D.Left.To_Rotation();
            
            it_inits_the_mesh();
            it_inits_the_outliner();
            it_wires_the_destruction();

            void it_inits_the_mesh() => GetComponentInChildren<UnitMesh>().Present(unit_type, the_unit.s_Faction);

            void it_inits_the_outliner()
            {
                var outliner = GetComponentInChildren<OOutliner>();

                if (the_unit.Belongs_To(the_Player_Faction))
                {
                    its_subscriptions = it_wires_the_selected_unit();
                }
                else
                {
                    Destroy(outliner.gameObject);
                }
            
                //Note: every unit's outline wires to the player
                Action it_wires_the_selected_unit() => 
                    FindObjectOfType<BattleComponent>()
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
        private Transform its_transform;
        private Action its_subscriptions;
    }
}