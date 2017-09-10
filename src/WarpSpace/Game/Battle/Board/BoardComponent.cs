using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Board
{
    public class BoardComponent : MonoBehaviour
    {
        public TileComponent TilePrefab;
        public WUnit UnitPrefab;
        
        public IStream<WUnit> s_Created_Units_Stream => it.s_created_units_stream;
        public TileComponent this[MTile the_tile] => it.s_tile_components.Get(the_tile.s_Position);

        public void Inits(MBoard board, MPlayer player)
        {
            gameObject.DestroyChildren();
            FindObjectOfType<WLimbo>().gameObject.DestroyChildren();
            
            s_created_units_stream = new RepeatAllStream<WUnit>();

            var this_transform = transform;
            it.s_tile_components = CreateTiles();

            it.wires_unit_creation(board);
            wires_the_highlights(player, board);

            TileComponent[,] CreateTiles() => 
                board.Tiles.Map((tile, index) =>
                {
                    var n = board.Tiles.GetFitNeighbours(index).Map(t => t.s_Landscape_Type());
                    return TileComponent.Create(TilePrefab, this_transform, tile, n, board.Tiles.GetDimensions(), player);
                })
            ;
        }
        
        private void wires_unit_creation(MBoard board)
        {
            foreach (var unit in board.Units)
                creates_the_world_unit_from(unit);

            board
                .s_Stream_Of_Unit_Creations
                .Subscribe(the_unit => it.creates_the_world_unit_from(the_unit));
        }
        
        private void creates_the_world_unit_from(MUnit the_model_unit)
        {
            var world_unit = WUnit.Is_Created_From(UnitPrefab, the_model_unit);
                
            it.s_created_units_stream.Next(world_unit);
        }

        private BoardComponent it => this;
        private RepeatAllStream<WUnit> s_created_units_stream;
        private TileComponent[,] s_tile_components;

        private void wires_the_highlights(MPlayer player, MBoard board)
        {
            player.s_Selected_Unit_Movements_Stream
                .Subscribe(moved =>
                {
                    Updates_Neighborhood_Of(moved.Source.as_a_Tile());
                    Updates_Neighborhood_Of(moved.Destination.as_a_Tile());
                })
            ;

            player.s_Selected_Unit_Changes_Stream
                .Subscribe(p => Handles_New_Selected_Unit(p.s_Previous.Select(u => u.s_Tile), p.s_Current.Select(u => u.s_Tile)))
            ;

            board.s_Turn_Ends_Stream
                .Subscribe(_ => Updates_Neighborhood_Of(player.s_Possible_Selected_Unit.Select(u => u.s_Tile)))
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
                if (!possible_tile.has_a_Value(out var prev_tile))
                    return;

                updates_the_highlight_of(prev_tile);
                
                foreach (var adjacent in prev_tile.s_Adjacent_Tiles.NotEmpty)
                    updates_the_highlight_of(adjacent);
            }
            
            void updates_the_highlight_of(MTile tile) => it[tile].Highlight.Updates_the_Highlight();
        }
    }
}