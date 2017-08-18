using System.Collections.Generic;
using System.Linq;
using Lanski.Linq;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Planet.Tiles;
using static Lanski.Structures.Direction2D;

namespace WarpSpace.World.Board
{
    public class RandomMapGenerator
    {
        private const int Rows = 8;
        private const int Columns = 8;

        public Map GenerateRandomMap()
        {
            var tiles = GenerateTiles();
            
            var found = tiles.EnumerateWithIndex()
                .Where(e => e.element.Type == LandscapeType.Flatland || e.element.Type == LandscapeType.Hill)
                .Select(p => GetRotationToRandomPassableNeighbour(tiles.GetNeighbours(p.index)).Select(d => (p.index, d)))
                .WhereNotNull()
                .ToArray()
                .RandomElement();

            var entrance = new MapObject(found.Item1, found.Item2, MapObjectType.Entrance);
            
            return new Map(tiles, new[] {entrance});
        }

        private Direction2D? GetRotationToRandomPassableNeighbour(Neighbourhood2d<Tile> n)
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
            
            bool IsPassable(Tile? t)
            {
                return t.Is(x => x.Type.IsPassable());
            }
        }

        

        private Tile[,] GenerateTiles()
        {
            return Array2D.Create(new Dimensions2D(Rows, Columns), p => new Tile(LandscapeTypeEx.Random()));
        }
        
        /*
        1373467341
        
M M M H H H H M
M M H L H H L H
H H W W E L L H
L H W W M H H M
L W H L H H M M
L L L L H M M H
M H H W W W L L
M H W W W W W W

M M M M H W M M
M W W M H E M M
M W W L L L H M
H H L L M W H L
L L H M W W W W
W W H M W W M H
H L L H H H L H
L L M M M L L W
        */
    }
}