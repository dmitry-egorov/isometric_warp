using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Unity.World.Battle.Board.Tile.Landscape;
using static WarpSpace.Unity.World.Battle.Board.Tile.Landscape.HighlightType;

namespace WarpSpace.Unity.World.Battle.Board.Tile
{
    public class HighlightElement
    {
        private readonly PlayerModel _player;
        private readonly TileModel _tile;
        private readonly Landscape.Component _landscape;

        public HighlightElement(PlayerModel player, TileModel tile, Landscape.Component landscape)
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
                  Selected_Unit_Is_At_the_Tile()              ? Unit_Placeholder
                : !Has_A_Command_At_the_Tile(out var command) ? None  
                : command.Is_Fire()                           ? Fire_Weapon
                : command.Is_Move()                           ? Move 
                : command.Is_Interact()                       ? Interaction 
                                                              : None
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