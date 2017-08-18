using System.Collections.Generic;
using System.Linq;
using Lanski.Linq;
using Lanski.Structures;
using Lanski.UnityExtensions;
using static Lanski.Structures.Direction2D;

namespace WarpSpace.World.Board
{
    public static class RandomMapGenerator
    {
        private const int Rows = 8;
        private const int Columns = 8;

        public static MapSpec GenerateRandomMap()
        {
            var tiles = GenerateTiles();
            
            var found = tiles
                .EnumerateWithIndex()
                .Where(e => e.element.Type == LandscapeType.Flatland || e.element.Type == LandscapeType.Hill)
                .Select(p => GetRotationToRandomPassableNeighbour(tiles.GetAdjacentNeighbours(p.index)).Select(d => (p.index, d)))
                .WhereNotNull()
                .ToArray()
                .RandomElement();

            var entrance = new StructureSpec(found.Item1, found.Item2, StructureType.Entrance);
            
            return new MapSpec(tiles, new[] {entrance});
        }

        private static Direction2D? GetRotationToRandomPassableNeighbour(AdjacentNeighbourhood2D<TileSpec> n)
        {
            var passableDirections = PassableDirections().ToArray();

            if (passableDirections.Length == 0)
                return null;

            return passableDirections.RandomElement();
            
            IEnumerable<Direction2D> PassableDirections()
            {
                if (IsPassable(n.Left))
                    yield return Left;
                
                if (IsPassable(n.Up))
                    yield return Up;
                
                if (IsPassable(n.Right))
                    yield return Right;

                if (IsPassable(n.Down))
                    yield return Down;
            }
            
            bool IsPassable(TileSpec? t)
            {
                return t.Is(x => x.Type.IsPassable());
            }
        }

        private static TileSpec[,] GenerateTiles()
        {
            return Array2D.Create(new Dimensions2D(Rows, Columns), p => new TileSpec(LandscapeTypeEx.Random()));
        }
    }
}