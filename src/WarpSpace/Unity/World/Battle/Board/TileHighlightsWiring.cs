using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle.Board
{
    internal static class TileHighlightsWiring
    {
        public static void Wire(PlayerModel player, BoardModel board, Tile.TileComponent[,] tile_components)
        {
            player
                .Selected_Unit_Cell
                .SelectMany(Get_Tiles_Stream)
                .DelayByOne()
                .Subscribe(prev_tile_slot =>
                {
                    Update_Neighbourhood_Of(prev_tile_slot);
                    Update_Neighborhood_Of_Current_Tile();
                });

            board
                .Stream_Of_Destroyed_Units
                .Select(destroyed => destroyed.Location)
                .Subscribe(Update_Highlight_Of);

            void Update_Neighborhood_Of_Current_Tile()
            {
                if (!player.Selected_Unit_Cell.Has_a_Value(out var selected_unit)) return;

                var tile = selected_unit.Current_Tile;
                Update_Neighbourhood_Of(tile);
            }
            
            void Update_Neighbourhood_Of(Slot<TileModel> prev_tile_slot)
            {
                if (!prev_tile_slot.Has_a_Value(out var prev_tile))
                    return;

                Update_Highlight_Of(prev_tile);
                foreach (var adjacent in prev_tile.Adjacent.NotEmpty)
                    Update_Highlight_Of(adjacent);
            }
            
            void Update_Highlight_Of(TileModel tile) => 
                Get_the_Highlight_Element_Of(tile).Update_the_Highlight()
            ;

            IStream<Slot<TileModel>> Get_Tiles_Stream(Slot<UnitModel> selected_unit_slot) =>
                selected_unit_slot
                    .Select(u => u.Cell_of_the_Current_Tile.Select(x => x.AsSlot()))
                    .Cell_Or_Single_Default()
            ;

            Tile.HighlightElement Get_the_Highlight_Element_Of(TileModel tile) => 
                tile_components.Get(tile.Position).Highlight
            ;
        }
    }
}