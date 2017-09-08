using System;
using Lanski.Geometry;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Overlay.Units
{
    [RequireComponent(typeof(RectTransform))]
    public class OUnit : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _rect_transform;
        private WUnit _unit_component;

        private Action _wirings;

        public void Init(WUnit the_world_unit)
        {
            _camera = FindObjectOfType<Camera>();
            _rect_transform = GetComponent<RectTransform>();
            _unit_component = the_world_unit;

            var the_unit = the_world_unit.s_Unit;

            GetComponentInChildren<OHealth>().Inits_With(the_unit);
            GetComponentInChildren<OOutliner>().Inits_With(the_world_unit);

            _wirings = Wire_the_Docked_Events();
            
            it_wires_the_destruction_signal();

            Action Wire_the_Docked_Events() => 
                the_unit.s_Dock_States_Stream
                .Subscribe(is_docked => gameObject.SetActive(!is_docked))
            ;

            void it_wires_the_destruction_signal()
            {
                the_unit.s_Destruction_Signal
                    .Subscribe(_ => Destroy(gameObject));
            }
        }

        public void LateUpdate()
        {
            if (_unit_component.s_Unit.is_Docked)
            {
                gameObject.Hide();
                return;
            }
                
            var transformPosition = _unit_component.transform.position;
            var screen_position = _camera.WorldToScreenPoint(transformPosition).XY();

            _rect_transform.anchoredPosition = screen_position.Floor();
        }

        public void OnDestroy()
        {
            _wirings();
        }
    }
}