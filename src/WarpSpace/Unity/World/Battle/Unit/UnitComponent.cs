using System;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle.Unit
{
    public class UnitComponent : MonoBehaviour
    {
        public OwnSettings Settings;
        private Action _outline_wiring;
        private Action _movement_wiring;

        public Mover Mover { get; private set; }
        public UnitModel Unit { get; private set; }

        public static UnitComponent Create(GameObject prefab, Transform parent, UnitModel unit, TileModel source_tile, Board.Tile.TileComponent[,] tile_components, PlayerModel player)
        {
            var obj = Instantiate(prefab, parent).GetComponent<UnitComponent>();

            obj.Init(unit, source_tile, player, tile_components);

            return obj;
        }

        public void Update()
        {
            Mover.Update();
        }

        public void OnDestroy()
        {
            _movement_wiring();
            _outline_wiring();
        }

        private void Init(UnitModel unit, TileModel source_tile, PlayerModel player, Board.Tile.TileComponent[,] tile_components)
        {
            var unitSettings = SelectSettings(unit.Type);
            var factionSettings = SelectFactionSettings(unit.Faction);

            var outline = GetComponentInChildren<OutlineComponent>();
            var filter = GetComponentInChildren<MeshComponent>();
            Mover = new Mover(unitSettings.Movement, transform);
            Unit = unit;

            filter.Init(unitSettings.Mesh, factionSettings.Material);
            outline.Init(unitSettings.Mesh);
            _outline_wiring = Wire_Selections_to_Outline();
            _movement_wiring = Wire_Movements();
            
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
                        .Cell_of_the_Current_Tile
                        .IncludePrevious()
                        .Subscribe(x => MoveUnitComponent(x.previous, x.current));

                void MoveUnitComponent(Slot<TileModel> previousSlot, TileModel current)
                {
                    if (previousSlot.Has_a_Value(out var previous))
                    {
                        var cur_tile_component = tile_components.Get(current.Position);
                        var orientation = previous.GetDirectionTo(current);

                        Mover.ScheduleMovement(cur_tile_component, orientation);
                    }
                    else
                    {
                        transform.localRotation = source_tile.GetDirectionTo(current).ToRotation();
                    }
                    
                }
            }
            
            void Wire_the_Destruction()
            {
                unit
                    .Stream_Of_Single_Destroyed_Event
                    .First()
                    .Subscribe(isAlive => Destroy(gameObject));
            }
        }

        private OwnSettings.FactionsSettings.FactionSettings SelectFactionSettings(Faction unitFaction)
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

        private OwnSettings.UnitsSettings.UnitSettings SelectSettings(UnitType unitType)
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