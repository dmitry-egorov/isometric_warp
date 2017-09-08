using System;
using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(WMover))]
    public class WUnit : MonoBehaviour
    {
        public MUnit s_Unit { get; private set; }
        public Transform s_Transform { get; private set; }

        public static WUnit Is_Created_From(WUnit prefab, MUnit unit)
        {
            var limbo = FindObjectOfType<WLimbo>().s_Transform;
            var board = FindObjectOfType<BoardComponent>();
            var parent = unit.is_At_a_Tile(out var tile) ? board[tile].UnitSlot.Transform : limbo;

            var obj = Instantiate(prefab, parent);

            obj.inits(unit);

            return obj;
        }

        private void inits(MUnit unit)
        {
            var unit_type = unit.s_Type;
            
            s_Unit = unit;
            s_Transform = transform;

            var its_transform = transform;

            GetComponentInChildren<UnitMesh>().Present(unit_type, unit.s_Faction);
            
            its_transform.localRotation = Direction2D.Left.To_Rotation();
            Wire_the_Destruction();

            void Wire_the_Destruction()
            {
                unit.s_Destruction_Signal
                    .First()
                    .Subscribe(isAlive => Destroy(gameObject));
            }
        }

    }
}