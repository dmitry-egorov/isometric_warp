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
        }

        public void Update_the_Highlight()
        {
            var highlight_type = Get_the_Highlight_Type();

            _landscape.Set_the_Highlight_To(highlight_type);
            
            HighlightType Get_the_Highlight_Type()
            {
                if (A_Unit_Is_Selected(out var selected_unit) && selected_unit.Is_At(_tile))
                    return Unit_Placeholder;
                
                var command_slot = _player.Try_Get_Command_At(_tile);

                if (!command_slot.Has_a_Value(out var command))
                    return None;

                switch (command.TheType)
                {
                    case PlayerCommand.Type.Fire:
                        return Fire_Weapon;
                    case PlayerCommand.Type.Move:
                        return Move;
                    case PlayerCommand.Type.Interact:
                        return Interaction;
                    default:
                        return None;
                }
            }

            bool A_Unit_Is_Selected(out UnitModel selected_unit) => 
                _player.SelectedUnit.Has_a_Value(out selected_unit)
            ;
        }
    }
}