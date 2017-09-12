using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;
using static WarpSpace.Game.Battle.Tile.HighlightType;

namespace WarpSpace.Game.Battle.Tile
{
    public class HighlightElement
    {
        public HighlightElement(MPlayer player, MTile tile, Landscape landscape)
        {
            the_player = player;
            the_tile = tile;
            the_landscape = landscape;

            tile.s_Sites_Cell.Subscribe(_ => this.Updates_the_Highlight());
        }

        public void Updates_the_Highlight()
        {
            var the_highlight_type = its_highlight_type();

            the_landscape.s_Highlight_Becomes(the_highlight_type);
            
            HighlightType its_highlight_type() =>
                  the_selected_unit_is_at_the_tile()                      ? Placeholder
                : !the_player.has_a_Command_At(the_tile, out var command) ? None  
                : command.is_a_Fire_Command()                             ? Attack
                : command.is_a_Tile_Move_Command()                        ? Move 
                : command.is_a_Dock_Command()                             ? Interact 
                : command.is_an_Interact_Command()                        ? Interact 
                                                                          : None
            ;
        }

        private bool the_selected_unit_is_at_the_tile() => 
            the_player.has_a_Unit_Selected(out var the_selected_unit) && 
            the_selected_unit.is_At(the_tile)
        ;
        
        private readonly MPlayer the_player;
        private readonly MTile the_tile;
        private readonly Landscape the_landscape;
    }
}