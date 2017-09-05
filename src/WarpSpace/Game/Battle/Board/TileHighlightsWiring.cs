using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Models;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Board
{
    internal static class TileHighlightsWiring
    {
        public static void Wire(MPlayer player, MBoard board, TileComponent[,] tile_components)
        {
            player
                .Selected_Unit_Cell
                .SelectMany(pu => pu.Select(u => u.Stream_Of_Movements).Value_Or(StreamCache.Empty_Stream_of_Movements))
                .Subscribe(moved =>
                {
                    Update_Neighborhood_Of(moved.Source.As_a_Tile());
                    Update_Neighborhood_Of(moved.Destination.As_a_Tile());
                })
            ;

            player
                .Selected_Unit_Cell
                .IncludePrevious()
                .Subscribe(p => Handle_New_Selected_Unit(p.previous.SelectMany(u => u.Location_As_a_Tile()), p.current.SelectMany(u => u.Location_As_a_Tile())))
            ;

            board
                .Stream_Of_Unit_Destructions
                .Select(destroyed => destroyed.Location_As_a_Tile())
                .Subscribe(Update_Neighborhood_Of)
            ;

            void Handle_New_Selected_Unit(Possible<MTile> previous, Possible<MTile> current)
            {
                Update_Neighborhood_Of(previous);
                Update_Neighborhood_Of(current);
            }

            void Update_Neighborhood_Of(Possible<MTile> possible_tile)
            {
                if (!possible_tile.Has_a_Value(out var prev_tile))
                    return;

                Update_Highlight_Of(prev_tile);
                
                foreach (var adjacent in prev_tile.Adjacent.NotEmpty)
                    Update_Highlight_Of(adjacent);
            }
            
            void Update_Highlight_Of(MTile tile) => 
                Get_the_Highlight_Element_Of(tile).Update_the_Highlight()
            ;

            HighlightElement Get_the_Highlight_Element_Of(MTile tile) => 
                tile_components.Get(tile.Position).Highlight
            ;
        }
    }
}