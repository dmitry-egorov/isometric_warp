using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Unity.World.Battle.Unit;

namespace WarpSpace.Unity.World.Battle.Board
{
    internal static class UnitCreationWiring
    {
        public static void Wire(BoardModel board, Tile.TileComponent[,] tile_components, GameObject prefab, PlayerModel player, IConsumer<UnitComponent> stream_of_created_units)
        {
            board
                .Stream_Of_Added_Units
                .Subscribe(Create_and_Wire_a_Component_For_the_Unit);
                
            void Create_and_Wire_a_Component_For_the_Unit(UnitAdded unit_added)
            {
                var unit = unit_added.Unit;
                var tile = unit.Cell_of_the_Current_Tile.Value;
                var source_tile = unit_added.SourceTile;
                var tile_component = tile_components.Get(tile.Position);

                var unit_component = UnitComponent.Create(prefab, tile_component.UnitSlot.transform, unit, source_tile, tile_components, player);
                
                stream_of_created_units.Next(unit_component);
            }
        }
    }
}