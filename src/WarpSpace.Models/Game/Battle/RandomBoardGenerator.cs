using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Randomness;

namespace WarpSpace.Models.Game.Battle
{
    public class RandomBoardGenerator
    {
        private readonly IRandom _random;
        private const int Rows = 8;
        private const int Columns = 8;

        public RandomBoardGenerator(IRandom random)
        {
            _random = random;
        }

        public BoardDescription GenerateRandomMap()
        {
            var types = GenerateTiles();
            
            var entranceSpacial = Get_Structure_Spacial();
            var exitSpacial = Get_Structure_Spacial();

            var entranceStructure = StructureDescription.Create.Entrance(entranceSpacial.Orientation);
            var exitStructure = StructureDescription.Create.Exit(exitSpacial.Orientation);

            var tiles = types.Map((t, i) => new TileDescription(t, SelectStructure(i)));
            
            return new BoardDescription(tiles, entranceSpacial, new UnitDescription?[Rows, Columns]);
            
            LandscapeType[,] GenerateTiles() => 
                Array2D.Create(new Dimensions2D(Rows, Columns), p => _random.Random_Landscape_Type());

            Spacial2D Get_Structure_Spacial() => 
                _random.Random_Element_Of(Get_Possible_Structure_Spacials())
            ;
            
            Spacial2D[] Get_Possible_Structure_Spacials() => 
                types
                .EnumerateWithIndex()
                .Where(e => e.element == LandscapeType.Flatland || e.element == LandscapeType.Hill)
                .Select(p => Get_Orientation_To_Random_Passable_Neighbour(types.GetAdjacentNeighbours(p.index)).Select(d => new Spacial2D(p.index, d)))
                .SkipNull()
                .ToArray()
            ;

            StructureDescription? SelectStructure(Index2D i)
            {
                if (i == entranceSpacial.Position) 
                    return entranceStructure;
                if (i == exitSpacial.Position)
                    return exitStructure;
                
                return null;
            }
        }

        private Direction2D? Get_Orientation_To_Random_Passable_Neighbour(AdjacentNeighbourhood2D<LandscapeType> n)
        {
            var passableDirections = PassableDirections().ToArray();

            if (passableDirections.Length == 0)
                return null;

            return _random.Random_Element_Of(passableDirections);
            
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