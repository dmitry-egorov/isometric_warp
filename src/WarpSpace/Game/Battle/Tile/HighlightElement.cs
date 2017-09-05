using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Tile
{
    public class HighlightElement
    {
        private readonly PlayerModel _player;
        private readonly TileModel _tile;
        private readonly Landscape _landscape;

        public HighlightElement(PlayerModel player, TileModel tile, Landscape landscape)
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
                  Selected_Unit_Is_At_the_Tile()              ? HighlightType.Unit_Placeholder
                : !Has_A_Command_At_the_Tile(out var command) ? HighlightType.None  
                : command.Is_Fire()                           ? HighlightType.Fire_Weapon
                : command.Is_Move()                           ? HighlightType.Move 
                : command.Is_Interact()                       ? HighlightType.Interaction 
                                                              : HighlightType.None
            ;
        }

        private bool Has_A_Command_At_the_Tile(out Command command) => 
            _player.Possible_Command_At(_tile).Has_a_Value(out command)
        ;

        private bool Selected_Unit_Is_At_the_Tile() => 
            _player.Selected_Unit_Cell.Has_a_Value(out var selected_unit) 
            && selected_unit.Is_At(_tile)
        ;
    }
}