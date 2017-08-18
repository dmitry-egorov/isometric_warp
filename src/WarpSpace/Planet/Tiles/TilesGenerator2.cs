using System;
using System.Linq;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.World.Board;
using static Lanski.Maths.Mathe;
using static UnityEngine.Mathf;

namespace WarpSpace.Planet.Tiles
{
    public class TilesGenerator2: MonoBehaviour
    {
        [SerializeField] GameObject _tilePrefab;
        [SerializeField] GameObject _entrancePrefab;
        [SerializeField] LandscapeTileSpecification _mountainSpec;
        [SerializeField] LandscapeTileSpecification _hillSpec;
        [SerializeField] LandscapeTileSpecification _flatlandSpec;
        [SerializeField] LandscapeTileSpecification _waterSpec;

        public void RecreateTiles(Map map)
        {
            this.DestroyChildren();
            CreateTiles(map);
        }

        private void CreateTiles(Map map)
        {
            var tiles = map.Tiles;
            var d = tiles.GetDimensions();
            var entrance = map.MapObjects.First(o => o.Type == MapObjectType.Entrance);

            foreach (var t in tiles.EnumerateWithIndex())
            {
                var i = t.index;
                var spec = GetSpec(t.element.Type);
                
                var cell = TileCreation.CreateTile(_tilePrefab, i, d, transform);

                var parts = cell.GetComponent<TilePartsHolder>();

                var meshPrototype = spec.TerrainMeshes[i.Row % spec.TerrainMeshes.Length];

                var elevations = CalculateElevations(tiles, i);
                var transformedMesh = MeshTransformer.Transform(meshPrototype, TileCreation.GetDirection(i), elevations, spec.Falloff);
                
                parts.Init(transformedMesh, i == entrance.Index ? _entrancePrefab : null );
            }
        }

        private MeshTransformer.Elevations CalculateElevations(Tile[,] tiles, Index2D i)
        {
            var self = GetTypeAt(tiles, i);

            var l = GetTypeAt(tiles, i.Left());
            var u = GetTypeAt(tiles, i.Up());
            var r = GetTypeAt(tiles, i.Right());
            var d = GetTypeAt(tiles, i.Down());
            
            var lu = GetTypeAt(tiles, i.Left().Up());
            var ru = GetTypeAt(tiles, i.Right().Up());
            var rd = GetTypeAt(tiles, i.Right().Down());
            var ld = GetTypeAt(tiles, i.Left().Down());
            
            return new MeshTransformer.Elevations
            {
                L = CalculateAdjacentOffset(self, l),
                U = CalculateAdjacentOffset(self, u),
                R = CalculateAdjacentOffset(self, r),
                D = CalculateAdjacentOffset(self, d),
                
                LU = CalculateCrossOffset(l, u, lu, self),
                RU = CalculateCrossOffset(r, u, ru, self),
                RD = CalculateCrossOffset(r, d, rd, self),
                LD = CalculateCrossOffset(l, d, ld, self)
            };
        }

        private float CalculateAdjacentOffset(LandscapeType t1, LandscapeType t2)
        {
            if (t1 == t2) 
                return GetSpec(t1).SameTypeHeight;
            
            var s1 = GetHeight(t1);
            var s2 = GetHeight(t2);

            return Min(s1, s2);
        }

        private float GetHeight(LandscapeType t1)
        {
            return GetSpec(t1).OwnHeight;
        }

        private float CalculateCrossOffset(LandscapeType ld, LandscapeType ru, LandscapeType lu, LandscapeType rd)
        {
            if (ld == ru && ru == lu && lu == rd)
                return GetSpec(ld).SameTypeHeightCross;
            
            var ldh = GetHeight(ld);
            var ruh = GetHeight(ru);
            var luh = GetHeight(lu);
            var rdh = GetHeight(rd);
            
            return Avg(Min(ldh, ruh), Min(luh, rdh));
        }

        private LandscapeTileSpecification GetSpec(LandscapeType type)
        {
            switch (type)
            {
                case LandscapeType.Mountain:
                    return _mountainSpec;
                case LandscapeType.Hill:
                    return _hillSpec;
                case LandscapeType.Flatland:
                    return _flatlandSpec;
                case LandscapeType.Water:
                    return _waterSpec;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static LandscapeType GetTypeAt(Tile[,] tiles, Index2D i)
        {
           return tiles.Get(i.FitTo(tiles)).Type;
        }
        
        [Serializable]
        public struct LandscapeTileSpecification
        {
            public float OwnHeight;
            public float SameTypeHeight;
            public float SameTypeHeightCross;
            public float Falloff;
            public Mesh[] TerrainMeshes;
        }
    }
}