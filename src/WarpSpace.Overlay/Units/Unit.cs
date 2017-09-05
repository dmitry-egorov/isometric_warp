using System;
using Lanski.Geometry;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Overlay.Units
{
    public class Unit : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _rect_transform;
        private UnitComponent _unit_component;
        private ReferencePixels _reference_pixels;

        private Action _scale_wiring;
        private Action _health_wiring;

        public void Init(UnitComponent unitComponent)
        {
            _camera = FindObjectOfType<Camera>();
            _reference_pixels = FindObjectOfType<ReferencePixels>();
            _rect_transform = GetComponent<RectTransform>();
            _unit_component = unitComponent;
            
            _scale_wiring = Wire_Scale();
            _health_wiring = Wire_Health();
            Wire_Destroyed();

            Action Wire_Scale() => 
                FindObjectOfType<ReferencePixels>()
                .PixelPerfectScaleCell
                .Subscribe(scale => _rect_transform.localScale = new Vector3(scale, scale, 1))
            ;

            Action Wire_Health()
            {
                var health_component = GetComponentInChildren<Health>();

                var health = unitComponent.Unit.Health;

                health_component.Total = health.TotalHitPoints;
                
                return 
                    health
                    .Current_Hit_Points_Cell
                    .Subscribe(current => 
                        health_component.Current = current
                    );
            }

            void Wire_Destroyed()
            {
                unitComponent
                    .Unit
                    .Signal_Of_the_Destruction
                    .Subscribe(_ => Destroy(gameObject));
            }
        }

        public void OnDestroy()
        {
            _health_wiring();
            _scale_wiring();
        }
        
        public void LateUpdate()
        {
            var transformPosition = _unit_component.transform.position;
            var screen_position = _camera.WorldToScreenPoint(transformPosition).XY();

            var scale = _reference_pixels.PixelPerfectScale;

            _rect_transform.anchoredPosition = screen_position.Floor(); //(screen_position * scale).Floor() / scale;
        }
    }
}