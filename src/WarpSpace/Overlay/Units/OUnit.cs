using System;
using Lanski.Geometry;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Overlay.Units
{
    [RequireComponent(typeof(RectTransform))]
    public class OUnit : MonoBehaviour, IUnitReference
    {
        public MUnit s_Unit => its_world_unit.s_Unit;
        
        public void Init(WUnit the_world_unit)
        {
            the_camera = FindObjectOfType<Camera>();
            its_rect_transform = GetComponent<RectTransform>();
            its_world_unit = the_world_unit;

            var the_unit = the_world_unit.s_Unit;

            it_wires_the_destruction_signal();

            void it_wires_the_destruction_signal()
            {
                the_unit.Been_Destroyed()
                    .Subscribe(_ => Destroy(gameObject));
            }
        }

        public void LateUpdate()
        {
            var the_transform_position = its_world_unit.s_Transform.position;
            var the_screen_position = the_camera.WorldToScreenPoint(the_transform_position).XY();

            its_rect_transform.anchoredPosition = the_screen_position.Floor();
        }

        private Camera the_camera;
        private RectTransform its_rect_transform;
        private WUnit its_world_unit;
    }
}