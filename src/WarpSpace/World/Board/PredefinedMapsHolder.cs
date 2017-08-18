using System;
using System.Linq;
using JetBrains.Annotations;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEditor.Expose;
using UnityEngine;
using WarpSpace.Planet.Tiles;
using Random = UnityEngine.Random;

namespace WarpSpace.World.Board
{
    [ExecuteInEditMode]
    public class PredefinedMapsHolder : MonoBehaviour
    {
        [SerializeField] private MapData[] _maps;

        public Map GetMap(int i)
        {
            return Parse(_maps[i]);
        }

        private static Map Parse(MapData map)
        {
            var split = map
                .Tiles
                .Split('\n')
                .Select(row => row.Split(' ')
                                  .Select(s => s[0])
                                  .Select(Parse)
                                  .Select(t => new Tile(t)));

            var tiles = split.To2DArray();

            var objects = new[] { new MapObject(map.EntrancePosition, map.EntranceDirection, MapObjectType.Entrance) };
            return new Map(tiles, objects);
        }

        private static LandscapeType Parse(char c)
        {
            return c.ToLandscapeType();
        }

        /*
        1373467341
        
M M M H H H H M
M M H L H H L H
H H W W L L L H
L H W W M H H M
L W H L H H M M
L L L L H M M H
M H H W W W L L
M H W W W W W W

M M M M H W M M
M W W M H L M M
M W W L L L H M
H H L L M W H L
L L H M W W W W
W W H M W W M H
H L L H H H L H
L L M M M L L W
        */
    }

    [Serializable]
    public struct MapData
    {
        [TextArea(8,8)] public string Tiles;
        public Index2DData EntrancePosition;
        public Direction2D EntranceDirection;
    }

    [Serializable]
    public struct MapObjectData
    {
        public Index2DData Location;
        public Direction2D Direction;
    }

    [Serializable]
    public struct Index2DData
    {
        public int Row;
        public int Column;
        
        public static implicit operator Index2D(Index2DData d)
        {
            return new Index2D(d.Row, d.Column);
        }
    }
}