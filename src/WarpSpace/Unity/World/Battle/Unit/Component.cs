using System;
using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Unity.World.Battle.Unit
{
    public class Component: MonoBehaviour
    {
        public GameObject Outline;
        public MovementSettings Movement;
        
        private Mover _mover;

        public static void Create(GameObject prefab, Transform parent, UnitModel unit, TileModel source_tile, Board.Tile.Component[,] tile_components, Dictionary<UnitModel, Component> components_map)
        {
            var obj = Instantiate(prefab, parent).GetComponent<Component>();

            obj.Init(unit, source_tile, tile_components, components_map);
        }

        public void Enable_the_Outline()
        {
            Outline.SetActive(true);
        }

        public void Disable_the_Outline()
        {
            Outline.SetActive(false);
        }

        public void MoveTo(Board.Tile.Component tile, Direction2D newRotation)
        {
            _mover.ScheduleMovement(tile, newRotation);
        }

        void Update()
        {
            _mover.Update();
        }

        private void Init(UnitModel unit, TileModel source_tile, Board.Tile.Component[,] tile_components, Dictionary<UnitModel, Component> components_map)
        {
            _mover = new Mover(Movement, transform);
            var movement_wiring = Wire_Movements();

            Add_the_Component_To_the_Map();
            Wire_the_Destruction();
            
            Action Wire_Movements()
            {
                return
                    unit
                        .Current_Tile_Cell
                        .IncludePrevious()
                        .Subscribe(x => MoveUnitComponent(x.previous, x.current));

                void MoveUnitComponent(Slot<TileModel> previousSlot, TileModel current)
                {
                    if (previousSlot.Has_a_Value(out var previous))
                    {
                        var cur_tile_component = tile_components.Get(current.Position);
                        var orientation = previous.GetDirectionTo(current);

                        MoveTo(cur_tile_component, orientation);
                    }
                    else
                    {
                        transform.localRotation = source_tile.GetDirectionTo(current).ToRotation();
                    }
                    
                }
            }
            
            void Add_the_Component_To_the_Map() => components_map[unit] = this;

            void Wire_the_Destruction()
            {
                unit
                    .Stream_Of_Destroyed_Events
                    .First()
                    .Subscribe(isAlive => Destory());

                void Destory()
                {
                    Unwire();
                    Remove_the_Component_From_the_Map();
                    Remove_the_Component_From_the_Scene();

                    void Unwire() => movement_wiring();
                    void Remove_the_Component_From_the_Map() => components_map.Remove(unit);
                    void Remove_the_Component_From_the_Scene() => Destroy(gameObject);
                }
            }
        }
        
        [Serializable]
        public struct MovementSettings
        {
            public float MaxAngularSpeed;
            public float MinAngularSpeed;
            public float AnglularAccelerationDistance;
            public float MaxSpeed;
            public float MinSpeed;
            public float AccelerationDistance;
        }
    }
}