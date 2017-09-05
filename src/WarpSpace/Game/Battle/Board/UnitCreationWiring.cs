using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Board
{
    internal static class UnitCreationWiring
    {
        public static void Wire(MBoard board, Tile.TileComponent[,] tile_components, GameObject prefab, GameObject limbo, MPlayer player, IConsumer<UnitComponent> stream_of_created_units)
        {
            foreach (var unit in board.Units)
            {
                Create_and_Wire_a_Component_For_the_Unit(unit);
            }
            
            board
                .Stream_Of_Unit_Creations
                .Subscribe(Create_and_Wire_a_Component_For_the_Unit);
                
            void Create_and_Wire_a_Component_For_the_Unit(MUnit unit)
            {
                var parent = unit.Is_At_a_Tile(out var tile) ? tile_components.Get(tile.Position).UnitSlot.transform : limbo.transform;

                var unit_component = UnitComponent.Create(prefab, parent, unit, tile_components, player);
                
                stream_of_created_units.Next(unit_component);
            }
        }
    }
}