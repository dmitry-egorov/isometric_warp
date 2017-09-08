using Lanski.Reactive;
using Lanski.Structures;
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
            var highlight_type = Get_the_Highlight_Type();

            the_landscape.Set_the_Highlight_To(highlight_type);
            
            HighlightType Get_the_Highlight_Type() =>
                  Selected_Unit_is_At_the_Tile()                          ? Placeholder
                : !the_player.has_a_Command_At(the_tile, out var command) ? None  
                : command.is_a_Fire_Command()                             ? Attack
                : command.is_a_Tile_Move_Command()                        ? Move 
                : command.is_a_Dock_Command()                             ? Interact 
                : command.is_an_Interact_Command()                        ? Interact 
                                                                          : None
            ;
        }

        private bool Selected_Unit_is_At_the_Tile() => 
            the_player.has_a_Unit_Selected(out var the_selected_unit) && 
            the_selected_unit.is_At(the_tile)
        ;
        
        private readonly MPlayer the_player;
        private readonly MTile the_tile;
        private readonly Landscape the_landscape;
    }
}