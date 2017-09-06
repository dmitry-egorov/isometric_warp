using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;
using static WarpSpace.Game.Battle.Tile.HighlightType;

namespace WarpSpace.Game.Battle.Tile
{
    public class HighlightElement
    {
        private readonly MPlayer _player;
        private readonly MTile _tile;
        private readonly Landscape _landscape;

        public HighlightElement(MPlayer player, MTile tile, Landscape landscape)
        {
            _player = player;
            _tile = tile;
            _landscape = landscape;

            tile.Site_Cell.Subscribe(_ => Update_the_Highlight());
        }

        public void Update_the_Highlight()
        {
            var highlight_type = Get_the_Highlight_Type();

            _landscape.Set_the_Highlight_To(highlight_type);
            
            HighlightType Get_the_Highlight_Type() =>
                  Selected_Unit_Is_At_the_Tile()                              ? Placeholder
                : !Has_A_Command_At_the_Tile(out var command)                 ? None  
                : command.Is_a_Fire()                                         ? Attack
                : command.Is_a_Move(out var move) && move.Unit.is_At_a_Tile() ? Move 
                : command.Is_a_Move(out     move) && move.Unit.is_At_a_Bay()  ? Interact 
                : command.Is_a_Interact()                                     ? Interact 
                                                                              : None
            ;
        }

        private bool Has_A_Command_At_the_Tile(out Command command) => 
            _player.Possible_Command_At(_tile).Has_a_Value(out command)
        ;

        private bool Selected_Unit_Is_At_the_Tile() => 
            _player.Has_A_Selected_Unit(out var the_selected_unit) && 
            the_selected_unit.is_At(_tile)
        ;
    }
}