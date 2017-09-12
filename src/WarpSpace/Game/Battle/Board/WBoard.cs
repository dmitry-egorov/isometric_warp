using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Board
{
    public class WBoard : MonoBehaviour
    {
        public WTile TilePrefab;
        public WUnit UnitPrefab;
        
        public IStream<Possible<MBattle>> s_New_Battles_Stream => its_new_Battle_stream;
        public IStream<WUnit> s_Created_Units_Stream => its_created_units_stream;
        public WTile this[MTile the_tile] => its_tile.Get(the_tile.s_Position);

        void Awake()
        {
            its_created_units_stream = new RepeatAllStream<WUnit>();
            its_new_Battle_stream = new RepeatAllStream<Possible<MBattle>>();
            its_limbo = FindObjectOfType<WLimbo>();
            its_transform = transform;
            its_world_battle = GetComponentInParent<WGame>();
        }

        void Start()
        {
            its_world_battle.s_Battles_Cell
                .Subscribe(battle => it_handles_a_new_battle(battle))
            ;
            
            its_world_battle.s_Battles_Cell
                .SelectMany_Or_Empty(the_battle => the_battle.s_Board.s_Stream_Of_Unit_Creations)
                .Subscribe(the_unit => it_creates_a_unit_from(the_unit))
            ;

            its_world_battle.s_Players_Cell
                .SelectMany_Or_Empty(the_player => the_player.s_Selected_Unit_Movements_Stream)
                .Subscribe(moved => 
                {
                    it_updates_the_neighborhood_of(moved.Source.as_a_Tile());
                    it_updates_the_neighborhood_of(moved.Destination.as_a_Tile());
                })
            ;
            
            its_world_battle.s_Players_Cell
                .SelectMany_Or_Empty(the_player => the_player.s_Selected_Unit_Changes_Stream)
                .Subscribe(the_change =>
                {
                    it_updates_the_neighborhood_of(the_change.s_Previous.Select(u => u.s_Tile));
                    it_updates_the_neighborhood_of(the_change.s_Current.Select(u => u.s_Tile));
                })
            ;
            
            its_world_battle.s_Battles_Cell
                .SelectMany_Or_Empty(board => board.s_Board.s_Turn_Ends_Stream)
                .Subscribe(_ => it_updates_the_neighborhood_of(its_world_battle.s_Player.must_have_a_Value().s_Possible_Selected_Unit.Select(u => u.s_Tile)))
            ;
            
            its_world_battle.s_Battles_Cell
                .SelectMany_Or_Empty(board => board.s_Board.s_Unit_Destructions_Stream)
                .Select(destroyed => destroyed.s_Location_As_a_Tile())
                .Subscribe(it_updates_the_neighborhood_of)
            ;
        }

        private void it_handles_a_new_battle(Possible<MBattle> the_possible_battle)
        {
            gameObject.DestroyChildren();
            its_limbo.Destroys_All_Children();
            
            its_new_Battle_stream.Next(the_possible_battle);

            if (!the_possible_battle.has_a_Value(out var the_battle))
                return;

            var the_board = the_battle.s_Board;

            var the_player = its_world_battle.s_Player.must_have_a_Value();
            its_tile = it_creates_the_tiles(the_board.s_Tiles, the_player);
        }

        private WTile[,] it_creates_the_tiles(MTile[,] the_board_tiles, MPlayer the_player)
        {
            return the_board_tiles.Map((tile, index) => WTile.Create(TilePrefab, its_transform, tile, the_board_tiles.s_Dimensions(), the_player));
        }
        
        private void it_creates_a_unit_from(MUnit the_model_unit)
        {
            var world_unit = WUnit.Is_Created_From(UnitPrefab, the_model_unit);
                
            its_created_units_stream.Next(world_unit);
        }

        private void it_updates_the_neighborhood_of(Possible<MTile> possible_tile)
        {
            if (!possible_tile.has_a_Value(out var the_tile))
                return;
                
            //it_updates_the_highlight_of(the_tile);
                
            //foreach (var adjacent in the_tile.s_Adjacent_Tiles.NotEmpty)
                //it_updates_the_highlight_of(adjacent);
        }
        
        //private void it_updates_the_highlight_of(MTile tile) => true;this[tile].s_Highlight.Updates_the_Highlight();

        private RepeatAllStream<WUnit> its_created_units_stream;
        private RepeatAllStream<Possible<MBattle>> its_new_Battle_stream;
        private WTile[,] its_tile;
        private WLimbo its_limbo;
        private Transform its_transform;
        private WGame its_world_battle;
    }
}