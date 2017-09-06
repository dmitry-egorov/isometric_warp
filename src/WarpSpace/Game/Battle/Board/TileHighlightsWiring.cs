using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Board
{
    internal static class TileHighlightsWiring
    {
        public static void Wire(MPlayer player, MBoard board, TileComponent[,] tile_components)
        {
            player.s_Selected_Unit_Movements_Stream
                .Subscribe(moved =>
                {
                    Updates_Neighborhood_Of(moved.Source.As_a_Tile());
                    Updates_Neighborhood_Of(moved.Destination.As_a_Tile());
                })
            ;

            player.s_Selected_Unit_Changes_Stream
                .Subscribe(p => Handles_New_Selected_Unit(p.Previous.SelectMany(u => u.s_Location_As_a_Tile()), p.Current.SelectMany(u => u.s_Location_As_a_Tile())))
            ;

            board.s_Unit_Destructions_Stream
                .Select(destroyed => destroyed.s_Location_As_a_Tile())
                .Subscribe(Updates_Neighborhood_Of)
            ;

            void Handles_New_Selected_Unit(Possible<MTile> previous, Possible<MTile> current)
            {
                Updates_Neighborhood_Of(previous);
                Updates_Neighborhood_Of(current);
            }

            void Updates_Neighborhood_Of(Possible<MTile> possible_tile)
            {
                if (!possible_tile.Has_a_Value(out var prev_tile))
                    return;

                Update_Highlight_Of(prev_tile);
                
                foreach (var adjacent in prev_tile.Adjacent.NotEmpty)
                    Update_Highlight_Of(adjacent);
            }
            
            void Update_Highlight_Of(MTile tile) => 
                Get_the_Highlight_Element_Of(tile).Updates_the_Highlight()
            ;

            HighlightElement Get_the_Highlight_Element_Of(MTile tile) => 
                tile_components.Get(tile.Position).Highlight
            ;
        }
    }
}