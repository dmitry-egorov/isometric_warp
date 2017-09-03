﻿using System;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Unity.World.Battle.Unit;

namespace WarpSpace.UI.Gameplay.Units
{
    public class UiUnitComponent : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _rect_transform;
        private UnitComponent _unit_component;
        
        private Action _scale_wiring;
        private Action _health_wiring;

        public void Init(UnitComponent unitComponent)
        {
            _camera = FindObjectOfType<Camera>();
            _rect_transform = GetComponent<RectTransform>();
            _unit_component = unitComponent;
            
            _scale_wiring = Wire_Scale();
            _health_wiring = Wire_Health();
            Wire_Destroyed();

            Action Wire_Scale() => 
                FindObjectOfType<ReferencePixels>()
                .PixelPixelPerfectScaleCell
                .Subscribe(scale => _rect_transform.localScale = new Vector3(scale, scale, 1))
            ;

            Action Wire_Health()
            {
                var health_component = GetComponentInChildren<HealthComponent>();

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
                    .Stream_Of_Single_Destroyed_Event
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
            var screen_position = _camera.WorldToScreenPoint(transformPosition);

            _rect_transform.anchoredPosition = screen_position;
        }
    }
}