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
        public static void Wire(MBoard board, Tile.TileComponent[,] tile_components, GameObject prefab, MPlayer player, IConsumer<UnitComponent> stream_of_created_units)
        {
            foreach (var unit in board.Units)
            {
                Create_and_Wire_a_Component_For_the_Unit(unit);
            }
            
            board
                .Stream_Of_Unit_Creations
                .Where(u => u.Initial_Location.Is_a_Tile())
                .Subscribe(added => Create_and_Wire_a_Component_For_the_Unit(added.Unit));
                
            void Create_and_Wire_a_Component_For_the_Unit(MUnit unit)
            {
                var tile = unit.Location.Must_Be_a_Tile();
                var tile_component = tile_components.Get(tile.Position);

                var unit_component = UnitComponent.Create(prefab, tile_component.UnitSlot.transform, unit, tile_components, player);
                
                stream_of_created_units.Next(unit_component);
            }
        }
    }
}