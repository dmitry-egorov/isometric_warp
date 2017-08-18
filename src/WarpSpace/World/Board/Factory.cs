using System;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEditor.Expose;
using UnityEngine;

namespace WarpSpace.World.Board
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] GameObject _tilePrefab;
        [SerializeField] Landscape.Data _landscapeSpecs;
        
        [SerializeField] bool _usePredefinedMap;
        [SerializeField] int _predefinedMapIndex;
        [SerializeField] MapData[] _predefinedMaps;

        [NonSerialized] private string _lastMap;//For editor
        
        [ExposeMethodInEditor]
        public void Create()
        {
            var map = CreateMap();

            _lastMap = map.ToString();
            
            RecreateTiles(map.Tiles);
            
            MapSpec CreateMap()
            {
                return _usePredefinedMap 
                    ? GetPredefinedMap() 
                    : RandomMapGenerator.GenerateRandomMap();
            }
        }

        private void RecreateTiles(TileSpec[,] mapTiles)
        {
            this.DestroyChildren();
            CreateTiles(mapTiles);
        }
        
        private void CreateTiles(TileSpec[,] tileSpecs)
        {
            var d = tileSpecs.GetDimensions();
            var tiles = Array2D.Create<Tile>(d);

            foreach (var (spec, index) in tileSpecs.EnumerateWithIndex())
            {
                var tileObject = TileCreation.CreateTile(_tilePrefab, index, d, transform);
                var tile = tileObject.GetComponent<Tile>();
                
                tile.Init(index, spec);

                var neighbors = tileSpecs.GetFitNeighbours(index); 
                Landscape.Initiator.InitElement(tile, neighbors, _landscapeSpecs);
                Water.Initiator.InitElement(tile);
                
                tiles.Set(index, tile);
            }

            GetComponent<Map>().SetTiles(tiles);
        }

        private MapSpec GetPredefinedMap()
        {
            return MapParser.ParseMap(_predefinedMaps[_predefinedMapIndex]);
        }

        [Serializable]
        public struct MapData
        {
            [TextArea(8,8)] public string Tiles;
            public Index2DData EntrancePosition;
            public Direction2D EntranceDirection;
        }
    
        [Serializable]
        public struct Index2DData
        {
            public int Row;
            public int Column;
        
            public Index2D ToIndex2D()
            {
                return new Index2D(Row, Column);
            }
        }
    }
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