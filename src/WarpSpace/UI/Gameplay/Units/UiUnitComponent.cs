using System;
using UnityEngine;
using WarpSpace.Descriptions;
using WarpSpace.Unity.World.Battle.Unit;

namespace WarpSpace.UI.Gameplay.Units
{
    public class UiUnitComponent : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _rect_transform;
        private UnitComponent _unit_component;

        public void Init(UnitComponent unitComponent)
        {
            Wire_Moves();
            var wiring = Wire_Health();
            Wire_Destroyed();

            void Wire_Moves()
            {
                _camera = FindObjectOfType<Camera>();
                _rect_transform = GetComponent<RectTransform>();
                _unit_component = unitComponent;
            }

            Action Wire_Health()
            {
                var health_component = GetComponentInChildren<HealthComponent>();

                var health = unitComponent.Unit.Health;

                health_component.Total = health.TotalHitPoints;
                return health
                    .Current_Hit_Points_Cell
                    .Subscribe(current => 
                        health_component.Current = current
                    );
            }

            void Wire_Destroyed()
            {
                unitComponent
                    .Unit.Stream_Of_Destroyed_Events
                    .Subscribe(_ =>
                    {
                        wiring();
                        Destroy(gameObject);
                    });
            }
        }
        
        public void LateUpdate()
        {
            var transformPosition = _unit_component.transform.position;
            var screen_position = _camera.WorldToScreenPoint(transformPosition);

            var anchoredPosition = new Vector2(Mathf.Round(screen_position.x), Mathf.Round(screen_position.y));
            _rect_transform.anchoredPosition = screen_position;
            //_rect_transform.anchoredPosition = anchoredPosition;
            if(_unit_component.Unit.Faction == Faction.Players)
                Debug.Log(_rect_transform.anchoredPosition);
            

            //var x = Mathf.Floor(canvas_position.x) * _scaler.referenceResolution.x / Screen.width;
            //var y = Mathf.Floor(canvas_position.y) * _scaler.referenceResolution.y / Screen.height;
            //var ref_x = _scaler.referenceResolution.x;
            //var ref_y = _scaler.referenceResolution.y;
            //var screen_x = Screen.width;
            //var screen_y = Screen.height;
            //var points_per_pixel = ref_y / screen_y;

            //var x = canvas_position.x * points_per_pixel;
            //var y = canvas_position.y * points_per_pixel;
            //var canvas_position = new Vector2(x, y);
            //_rect_transform.anchoredPosition = canvas_position;
            
            //_rect_transform.position = transformPosition;
            
        }
        
        
    }
}