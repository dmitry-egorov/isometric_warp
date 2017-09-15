using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Randomness;

namespace WarpSpace.Models.Game.Battle
{
    public class RandomBoardGenerator
    {
        public RandomBoardGenerator(IRandom random, MChassisType the_mothership_chassis_type, IReadOnlyList<MLandscapeType> the_landscape_types)
        {
            _random = random;
            this.the_motherships_chassis_type = the_mothership_chassis_type;
            this.the_landscape_types = the_landscape_types;
        }

        public DBoard Generates_a_Board()
        {
            var types = GenerateTiles();
            
            var entranceSpacial = Get_Structure_Spacial();
            var exitSpacial = Get_Structure_Spacial();

            var entranceStructure = DStructure.Create.Entrance(entranceSpacial.Orientation);
            var exitStructure = DStructure.Create.Exit(exitSpacial.Orientation);

            var tiles = types.Map((t, i) => new DTile(t, SelectContent(i)));
            
            return new DBoard(tiles, entranceSpacial);
            
            MLandscapeType[,] GenerateTiles() => 
                Array2D.Create(new Dimensions2D(Rows, Columns), p => _random.Random_Element_Of(the_landscape_types));

            Spacial2D Get_Structure_Spacial() => 
                _random.Random_Element_Of(Get_Possible_Structure_Spacials())
            ;
            
            Spacial2D[] Get_Possible_Structure_Spacials() => 
                types
                .EnumerateWithIndex()
                .Where(e => the_motherships_chassis_type.can_Pass(e.element))
                .Select(p => the_orientation_to_random_passable_neighbour_from(types.GetAdjacentNeighbours(p.index)).Select(d => new Spacial2D(p.index, d)))
                .SkipNull()
                .ToArray()
            ;

            DTileSite SelectContent(Index2D i)
            {
                if (i == entranceSpacial.Position) 
                    return entranceStructure;
                if (i == exitSpacial.Position)
                    return exitStructure;
                
                return TheVoid.Instance;
            }
        }

        private Direction2D? the_orientation_to_random_passable_neighbour_from(AdjacentNeighbourhood2D<MLandscapeType> n)
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
            
            bool IsPassable(Possible<MLandscapeType> landscape) => landscape.@is(x => the_motherships_chassis_type.can_Pass(x));
        }

        private readonly IRandom _random;
        private readonly MChassisType the_motherships_chassis_type;
        private readonly IReadOnlyList<MLandscapeType> the_landscape_types;
        private const int Rows = 8;
        private const int Columns = 8;
    }
}