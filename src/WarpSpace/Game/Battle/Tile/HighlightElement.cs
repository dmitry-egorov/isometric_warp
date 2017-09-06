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

            tile.Site_Cell.Subscribe(_ => this.Updates_the_Highlight());
        }

        public void Updates_the_Highlight()
        {
            var highlight_type = Get_the_Highlight_Type();

            the_landscape.Set_the_Highlight_To(highlight_type);
            
            HighlightType Get_the_Highlight_Type() =>
                  Selected_Unit_is_At_the_Tile()                                      ? Placeholder
                : !Has_A_Command_At_the_Tile(out var command)                         ? None  
                : command.is_a_Fire_Command()                                         ? Attack
                : command.is_a_Move_Command(out var move) && move.Unit.is_At_a_Tile() ? Move 
                : command.is_a_Move_Command(out     move) && move.Unit.is_At_a_Bay()  ? Interact 
                : command.is_an_Interact_Command()                                    ? Interact 
                                                                                      : None
            ;
        }

        private bool Has_A_Command_At_the_Tile(out Command command) => 
            the_player.s_Possible_Command_At(the_tile).Has_a_Value(out command)
        ;

        private bool Selected_Unit_is_At_the_Tile() => 
            the_player.Has_A_Selected_Unit(out var the_selected_unit) && 
            the_selected_unit.is_At(the_tile)
        ;
        
        private readonly MPlayer the_player;
        private readonly MTile the_tile;
        private readonly Landscape the_landscape;
    }
}