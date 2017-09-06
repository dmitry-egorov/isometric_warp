using System;
using Lanski.Behaviours.Overlay;
using Lanski.Geometry;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Overlay
{
    public class Unit : MonoBehaviour
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
            var health_component = GetComponentInChildren<Health>();

            var scale_wiring = Wire_Scale();
            var health_wiring = Wire_Health();
            var docked_wiring = Wire_Docked();
            _wirings = () => { scale_wiring(); health_wiring(); docked_wiring(); };
            
            Wire_Destroyed();

            Action Wire_Scale() => 
                FindObjectOfType<ReferencePixels>()
                .PixelPerfectScaleCell
                .Subscribe(scale => _rect_transform.localScale = new Vector3(scale, scale, 1))
            ;

            Action Wire_Health() => 
                unit.s_Cell_of_Health_Status()
                .Subscribe(the_status =>
                {
                    health_component.Total = the_status.s_Total_Hit_Points();
                    health_component.Current = the_status.s_Current_Hit_Points();
                })
            ;

            Action Wire_Docked() => 
                unit
                .s_Stream_of_Dock_States
                .Subscribe(is_docked => gameObject.SetActive(!is_docked))
            ;

            void Wire_Destroyed()
            {
                unit
                    .s_Signal_of_the_Destruction
                    .Subscribe(_ => Destroy(gameObject));
            }
        }

        public void OnDestroy()
        {
            _wirings();
        }
        
        public void LateUpdate()
        {
            if (_unit_component.Unit.Is_Docked)
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