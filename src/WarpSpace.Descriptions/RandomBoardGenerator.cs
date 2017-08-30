using System.Collections.Generic;
using System.Linq;
using Lanski.Linq;
using Lanski.Structures;
using Lanski.UnityExtensions;

namespace WarpSpace.Descriptions
{
    public static class RandomBoardGenerator
    {
        private const int Rows = 8;
        private const int Columns = 8;

        public static BoardDescription GenerateRandomMap()
        {
            var types = GenerateTiles();
            
            var entranceSpacial = GetStructureSpacial();
            var exitSpacial = GetStructureSpacial();

            var entranceStructure = new StructureDescription(StructureType.Entrance, entranceSpacial.Orientation);
            var exitStructure = new StructureDescription(StructureType.Exit, exitSpacial.Orientation);

            var tiles = types.Map((t, i) => new TileDescription(t, SelectStructure(i)));
            
            return new BoardDescription(tiles, entranceSpacial, new UnitDescription?[Rows, Columns]);
            
            LandscapeType[,] GenerateTiles() => 
                Array2D.Create(new Dimensions2D(Rows, Columns), p => LandscapeTypeEx.Random());

            Spacial2D GetStructureSpacial() => 
                types
                .EnumerateWithIndex()
                .Where(e => e.element == LandscapeType.Flatland || e.element == LandscapeType.Hill)
                .Select(p => GetRotationToRandomPassableNeighbour(types.GetAdjacentNeighbours(p.index)).Select(d => new Spacial2D(p.index, d)))
                .SkipNull()
                .ToArray()
                .RandomElement();

            StructureDescription? SelectStructure(Index2D i)
            {
                if (i == entranceSpacial.Position) 
                    return entranceStructure;
                if (i == exitSpacial.Position)
                    return exitStructure;
                
                return null;
            }
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
                return landscape.Is(x => x.IsPassableWith(ChassisType.Hower));
            }
        }

        
    }
}