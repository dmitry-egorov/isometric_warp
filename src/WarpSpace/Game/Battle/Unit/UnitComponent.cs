using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class UnitComponent : MonoBehaviour
    {
        public Mover Mover { get; private set; }
        public MUnit Unit { get; private set; }

        public static UnitComponent Create(GameObject prefab, Transform parent, MUnit unit, TileComponent[,] tile_components, MPlayer player)
        {
            var obj = Instantiate(prefab, parent).GetComponent<UnitComponent>();

            obj.Init(unit, player, tile_components);

            return obj;
        }

        public void Update()
        {
            Mover.Update();
        }

        public void OnDestroy()
        {
            _wirings();
        }

        private void Init(MUnit unit, MPlayer player, TileComponent[,] tile_components)
        {
            var settings_holder = FindObjectOfType<UnitSettingsHolder>();

            var unit_type = unit.s_Type;
            
            Unit = unit;

            var unitSettings = settings_holder.Get_Settings_For(unit_type);
            Mover = new Mover(unitSettings.Movement, transform);

            var outline = GetComponentInChildren<Outline>();
            outline.Init(unit_type);
            var filter = GetComponentInChildren<UnitMesh>();
            filter.Present(unit_type, unit.s_Faction);
            
            transform.localRotation = Direction2D.Left.To_Rotation();
            
            var outline_wiring = Wire_Selections_to_Outline();
            var movement_wiring = Wire_Movements();
            _wirings = () => { movement_wiring(); outline_wiring(); };
            
            Wire_the_Destruction();

            Action Wire_Selections_to_Outline()
            {
                return player
                    .s_Selected_Units_Cell
                    .IncludePrevious()
                    .Subscribe(x => SetOutline(x.previous, x.current))
                ;

                void SetOutline(Possible<MUnit> possible_prev_unit, Possible<MUnit> possible_cur_unit)
                {
                    if (possible_prev_unit.Has_a_Value(out var prev_unit) && prev_unit == unit)
                        outline.Disable();

                    if (possible_cur_unit.Has_a_Value(out var cur_unit) && cur_unit == unit)
                        outline.Enable();
                }
            }
            
            Action Wire_Movements()
            {
                return
                    unit.s_Movements_Stream()
                        .Subscribe(x => MoveUnitComponent(x.Source, x.Destination));

                void MoveUnitComponent(MLocation previous_location, MLocation current_location)
                {
                    if (!current_location.Is_a_Tile(out var current_tile))
                    {
                        //TODO: handle docking
                        return;
                    }

                    if (previous_location.Is_a_Bay(out var bay))
                    {
                        var owners_tile = bay.s_Owner.Must_Be_At_a_Tile();
                        var orientation = owners_tile.Get_Direction_To(current_tile);
                        Mover.Teleport_To(tile_components.Get(owners_tile.Position), orientation);
                        Mover.ScheduleMovement(tile_components.Get(current_tile.Position), orientation);
                    }
                    else
                    {
                        var previous_tile = previous_location.Must_Be_a_Tile();
                        var cur_tile_component = tile_components.Get(current_tile.Position);
                        var orientation = previous_tile.Get_Direction_To(current_tile);

                        Mover.ScheduleMovement(cur_tile_component, orientation);
                    }
                }
            }
            
            void Wire_the_Destruction()
            {
                unit.s_Destruction_Signal()
                    .First()
                    .Subscribe(isAlive => Destroy(gameObject));
            }
        }
        
        private Action _wirings;
    }
}