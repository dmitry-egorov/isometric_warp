using System.Collections.Generic;
using System.Linq;
using Lanski.Linq;
using Lanski.Structures;
using Lanski.UnityExtensions;
using WarpSpace.World.Board.Tile.Descriptions;
using WarpSpace.World.Unit;

namespace WarpSpace.World
{
    public static class RandomMapGenerator
    {
        private const int Rows = 8;
        private const int Columns = 8;

        public static BoardDescription GenerateRandomMap()
        {
            var types = GenerateTiles();
            
            var entranceSpacial = GetEntranceSpacial();

            var entranceStructure = new StructureDescription(StructureType.Entrance, entranceSpacial.Orientation);

            var tiles = types.Map((t, i) => new TileDescription(t, i == entranceSpacial.Position ? (StructureDescription?)entranceStructure : null));
            
            return new BoardDescription(tiles, entranceSpacial);
            
            LandscapeType[,] GenerateTiles() => 
                Array2D.Create(new Dimensions2D(Rows, Columns), p => LandscapeTypeEx.Random());

            Spacial2D GetEntranceSpacial() => 
                types
                .EnumerateWithIndex()
                .Where(e => e.element == LandscapeType.Flatland || e.element == LandscapeType.Hill)
                .Select(p => GetRotationToRandomPassableNeighbour(types.GetAdjacentNeighbours(p.index)).Select(d => new Spacial2D(p.index, d)))
                .WhereNotNull()
                .ToArray()
                .RandomElement();
        }

        

        private static Direction2D? GetRotationToRandomPassableNeighbour(AdjacentNeighbourhood2D<LandscapeType> n)
        {
            var passableDirections = PassableDirections().ToArray();

            if (passableDirections.Length == 0)
                return null;

            return passableDirections.RandomElement();
            
            IEnumerable<Direction2D> PassableDirections()
            {
                if (IsPassable(n.Left))
                    yield return Direction2D.Left;
                
                if (IsPassable(n.Up))
                    yield return Direction2D.Up;
                
                if (IsPassable(n.Right))
                    yield return Direction2D.Right;

                if (IsPassable(n.Down))
                    yield return Direction2D.Down;
            }
            
            bool IsPassable(LandscapeType? landscape)
            {
                return landscape.Is(x => x.IsPassableWith(ChassisType.Mothership));
            }
        }

        
    }
}