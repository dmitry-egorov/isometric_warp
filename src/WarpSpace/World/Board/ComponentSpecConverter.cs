using Lanski.Structures;
using UnityEngine;
using WarpSpace.World.Board.Tile.Descriptions;
using WarpSpace.World.Board.Tile.Landscape;
using WarpSpace.World.Board.Tile.Landscape.Extensions;
using WarpSpace.World.Board.Tile.StructureSlot.Extensions;
using WarpSpace.World.Board.Tile.Water;
using WarpSpace.World.Board.Tile.Water.Extensions;

namespace WarpSpace.World.Board
{
    public class ComponentSpecConverter
    {
        private readonly GameObject _tilePrefab;
        private readonly GameObject _mothershipPrefab;
        
        private readonly Initiator _landscape;
        private readonly SpecGenerator _water;
        private readonly Tile.StructureSlot.Initiator _structure;

        public ComponentSpecConverter(Settings settings)
        {
            _tilePrefab = settings.TilePrefab;
            _mothershipPrefab = settings.MothershipPrefab;
            
            _water     = settings.Water.ToInitiator();
            _landscape = settings.Landscape.ToInitiator();
            _structure = settings.Structure.ToInitiator();
        }

        public ComponentSpec GenerateComponentSpec(BoardDescription board, Player.Component player)
        {
            var tiles = board.Tiles;
            var dimensions = tiles.GetDimensions();

            var tileSpecs = tiles.Map((spec, index) =>
            {
                var position = GetPosition(index, dimensions);
                var name = $"Tile ({index.Column}, {index.Row})";

                var neighbors = tiles.GetFitNeighbours(index).Map(x => x.Type);
                var landscapeSpec = _landscape.Init(index, neighbors);
                var waterSpec = _water.Init(index, neighbors);
                var structureSpec = _structure.Init(spec.Structure);

                return new Tile.ComponentSpec(position, name, index, player, landscapeSpec, waterSpec, structureSpec);
            });
            
            return new ComponentSpec(_tilePrefab, tileSpecs, new EntranceSpec(_mothershipPrefab, board.EntranceSpacial));
        }

        private static Vector3 GetPosition(Index2D i, Dimensions2D dimensions)
        {
            return new Vector3(i.Column - dimensions.Columns * 0.5f, 0, dimensions.Rows * 0.5f - i.Row);
        }
    }
}