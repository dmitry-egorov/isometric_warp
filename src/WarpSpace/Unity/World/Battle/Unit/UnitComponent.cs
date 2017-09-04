using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle.Unit
{
    public class UnitComponent : MonoBehaviour
    {
        public OwnSettings Settings;
        private Action _wirings;

        public Mover Mover { get; private set; }
        public UnitModel Unit { get; private set; }

        public static UnitComponent Create(GameObject prefab, Transform parent, UnitModel unit, Board.Tile.TileComponent[,] tile_components, PlayerModel player)
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

        private void Init(UnitModel unit, PlayerModel player, Board.Tile.TileComponent[,] tile_components)
        {
            var unitSettings = Select_Settings(unit.Type);
            var factionSettings = Select_Faction_Settings(unit.Faction);

            var outline = GetComponentInChildren<OutlineComponent>();
            var filter = GetComponentInChildren<MeshComponent>();
            Mover = new Mover(unitSettings.Movement, transform);
            Unit = unit;

            filter.Init(unitSettings.Mesh, factionSettings.Material);
            outline.Init(unitSettings.Mesh);
            
            transform.localRotation = Direction2D.Left.To_Rotation();
            
            var outline_wiring = Wire_Selections_to_Outline();
            var movement_wiring = Wire_Movements();
            _wirings = () => { movement_wiring(); outline_wiring(); };
            
            Wire_the_Destruction();

            Action Wire_Selections_to_Outline()
            {
                return player
                    .Selected_Unit_Cell
                    .IncludePrevious()
                    .Subscribe(x => SetOutline(x.previous, x.current))
                ;

                void SetOutline(Slot<UnitModel> possible_prev_unit, Slot<UnitModel> possible_cur_unit)
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
                    unit
                        .Stream_Of_Movements
                        .Subscribe(x => MoveUnitComponent(x.Source, x.Destination));

                void MoveUnitComponent(LocationModel previous_slot, LocationModel current_slot)
                {
                    if (previous_slot.Is_a_Tile(out var previous_tile) && current_slot.Is_a_Tile(out var current_tile))
                    {
                        var cur_tile_component = tile_components.Get(current_tile.Position);
                        var orientation = previous_tile.Get_Direction_To(current_tile);

                        Mover.ScheduleMovement(cur_tile_component, orientation);
                    }
                    else
                    {
                        //TODO: Handle bays
                    }
                    
                }
            }
            
            void Wire_the_Destruction()
            {
                unit
                    .Signal_Of_the_Destruction
                    .First()
                    .Subscribe(isAlive => Destroy(gameObject));
            }
        }

        private OwnSettings.FactionsSettings.FactionSettings Select_Faction_Settings(Faction unitFaction)
        {
            switch (unitFaction)
            {
                case Faction.Players:
                    return Settings.Factions.Player;
                case Faction.Natives:
                    return Settings.Factions.Natives;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitFaction), unitFaction, null);
            }
        }

        private OwnSettings.UnitsSettings.UnitSettings Select_Settings(UnitType unitType)
        {
            switch (unitType)
            {
                case UnitType.Mothership:
                    return Settings.Units.Mothership;
                case UnitType.Tank:
                    return Settings.Units.Tank;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public FactionsSettings Factions;
            public UnitsSettings Units;

            [Serializable]
            public struct UnitsSettings
            {
                public UnitSettings Mothership;
                public UnitSettings Tank;
            
                [Serializable]
                public struct UnitSettings
                {
                    public Mover.MovementSettings Movement;
                    public Mesh Mesh;
                }
            }

            [Serializable]
            public struct FactionsSettings
            {
                public FactionSettings Player;
                public FactionSettings Natives;
                
                [Serializable]
                public struct FactionSettings
                {
                    public Material Material;
                }
            }
        }
    }
}