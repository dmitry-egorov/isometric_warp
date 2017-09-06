using System;
using Lanski.Behaviours.Overlay;
using Lanski.Geometry;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Overlay
{
    public class OUnit : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _rect_transform;
        private UnitComponent _unit_component;

        private Action _wirings;

        public void Init(UnitComponent unitComponent)
        {
            _camera = FindObjectOfType<Camera>();
            _rect_transform = GetComponent<RectTransform>();
            _unit_component = unitComponent;

            var unit = unitComponent.Unit;
            var health_component = GetComponentInChildren<OHealth>();

            health_component.Watches(unit.s_Health);
            
            var scale_wiring = Wire_the_Scale();
            var docked_wiring = Wire_the_Docked_Events();
            _wirings = () => { scale_wiring(); docked_wiring(); health_component.Watches(Possible.Empty<MHealth>());};
            
            Wire_Destroyed();

            Action Wire_the_Scale() => 
                FindObjectOfType<ReferencePixels>()
                .PixelPerfectScaleCell
                .Subscribe(scale => _rect_transform.localScale = new Vector3(scale, scale, 1))
            ;

            Action Wire_the_Docked_Events() => 
                unit.s_Dock_States_Stream()
                .Subscribe(is_docked => gameObject.SetActive(!is_docked))
            ;

            void Wire_Destroyed()
            {
                unit.s_Destruction_Signal()
                    .Subscribe(_ => Destroy(gameObject));
            }
        }

        public void OnDestroy()
        {
            _wirings();
        }
        
        public void LateUpdate()
        {
            if (_unit_component.Unit.is_Docked())
            {
                gameObject.Hide();
                return;
            }
                
            var transformPosition = _unit_component.transform.position;
            var screen_position = _camera.WorldToScreenPoint(transformPosition).XY();

            _rect_transform.anchoredPosition = screen_position.Floor();
        }
    }
}