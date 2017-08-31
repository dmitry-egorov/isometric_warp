using System;
using System.Collections.Generic;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Unity.World.Battle.Board
{
    internal static class UnitCreationWiring
    {
        public static void Wire(BoardModel board, Tile.Component[,] tile_components, Dictionary<UnitModel, Unit.Component> components_map, Component.OwnSettings settings)
        {
            board
                .Stream_Of_Added_Units
                .Subscribe(Create_and_Wire_a_Component_For_the_Unit);
                
            void Create_and_Wire_a_Component_For_the_Unit(BoardModel.UnitAdded e)
            {
                var unit = e.Unit;
                var tile = unit.Current_Tile_Cell.Value;
                var source_tile = e.SourceTile;
                var prefab = Select_the_Prefab();
                var tile_component = tile_components.Get(tile.Position);

                Create_the_Units_Component(); 

                void Create_the_Units_Component() => 
                    Unit.Component.Create(prefab, tile_component.UnitSlot.transform, unit, source_tile, tile_components, components_map)
                ;
                
                GameObject Select_the_Prefab()
                {
                    switch (e.Unit.Type)
                    {
                        case UnitType.Mothership: return settings.MothershipPrefab;
                        case UnitType.Tank:       return settings.TankPrefab;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}