using System;
using System.Linq;
using Lanski.Geometry;
using Lanski.UnityExtensions;
using UnityEditor.Expose;
using UnityEngine;
using static Lanski.Maths.Mathe;
using static UnityEngine.Mathf;
using Random = UnityEngine.Random;

namespace WarpSpace.Planet.Tiles
{
    public class TilesGenerator2: MonoBehaviour
    {
        [SerializeField] GameObject _tileCellPrefab;
        [SerializeField] TileSpecification[] _tileSpecifications; 
        
        [SerializeField, Range(1, 64)] int _rows; 
        [SerializeField, Range(1, 64)] int _columns;
        [SerializeField] private float _falloff;
        [SerializeField] private float _crossHeightProportion;

        void Start()
        {
            RecreateTiles();
        }

        [ExposeMethodInEditor]
        private void RecreateTiles()
        {
            DestroyTiles();

            CreateTiles();
        }

        [ExposeMethodInEditor]
        private void DestroyTiles()
        {
            var children = transform
                .Cast<Transform>()
                .Select(x => x.gameObject)
                .ToArray();
            
            foreach (var child in children)
            {
                DestroyImmediate(child);
            }
        }

        private void CreateTiles()
        {
            var tiles = GenerateTileGrid();
            
            for (var x = 0; x < tiles.GetLength(0); x++)
            for (var z = 0; z < tiles.GetLength(1); z++)
            {
                var t = tiles[x, z];
                var spec = t.Specification;
                
                var cell = CreateTileCell(t);

                var parts = cell.GetComponent<TilePartsHolder>();

                var meshPrototype = spec.TerrainMeshes.RandomElement();

                var elevations = CalculateElevations(tiles, x, z);
                var transformedMesh = MeshTransformer.Transform(meshPrototype, RandomRotation(), elevations, spec.Falloff);

                parts.SetMesh(transformedMesh);
                parts.SetWaterRotation(RandomRotation());
            }
        }

        private static RotationsBy90 RandomRotation()
        {
            return (RotationsBy90)Random.Range(0, 4);
        }

        private MeshTransformer.Elevations CalculateElevations(Tile[,] tiles, int x, int y)
        {
            var self = tiles[x, y];

            var l = GetTileAt(tiles, x - 1, y    );
            var u = GetTileAt(tiles, x    , y + 1);
            var r = GetTileAt(tiles, x + 1, y    );
            var d = GetTileAt(tiles, x    , y - 1);
            
            var lu = GetTileAt(tiles, x - 1, y + 1);
            var ru = GetTileAt(tiles, x + 1, y + 1);
            var rd = GetTileAt(tiles, x + 1, y - 1);
            var ld = GetTileAt(tiles, x - 1, y - 1);
            
            return new MeshTransformer.Elevations
            {
                L = GetAdjacentOffset(self, l),
                U = GetAdjacentOffset(self, u),
                R = GetAdjacentOffset(self, r),
                D = GetAdjacentOffset(self, d),
                
                LU = CalculateCrossOffset(l, u, lu, self),
                RU = CalculateCrossOffset(r, u, ru, self),
                RD = CalculateCrossOffset(r, d, rd, self),
                LD = CalculateCrossOffset(l, d, ld, self)
            };
        }

        private static float GetAdjacentOffset(Tile t1, Tile t2)
        {
            if (t1.Type == t2.Type) 
                return t1.Specification.SameTypeHeight;
            
            var s1 = t1.Specification;
            var s2 = t2.Specification;

            return Min(s1.OwnHeight, s2.OwnHeight);
        }

        private float CalculateCrossOffset(Tile ld, Tile ru, Tile lu, Tile rd)
        {
            if (ld.Type == ru.Type && ru.Type == lu.Type && lu.Type == rd.Type)
                return ld.Specification.SameTypeHeight * _crossHeightProportion;
            
            var ldh = ld.Specification.OwnHeight;
            var ruh = ru.Specification.OwnHeight;
            var luh = lu.Specification.OwnHeight;
            var rdh = rd.Specification.OwnHeight;
            
            return Avg(Min(ldh, ruh), Min(luh, rdh)) * _crossHeightProportion;
        }

        private Tile GetTileAt(Tile[,] tiles, int x, int y)
        {
            if (x < 0)
                x = 0;
            if (x >= tiles.GetLength(0))
                x = tiles.GetLength(0) - 1;
            if (y < 0)
                y = 0;
            if (y >= tiles.GetLength(1))
                y = tiles.GetLength(1) - 1;

            return tiles[x, y];
        }

        private GameObject CreateTileCell(Tile t)
        {
            var cell = Instantiate(_tileCellPrefab, transform);
            var index = t.Index;
            cell.transform.localPosition = t.Position;
            cell.name = $"Tile ({index.X}, {index.Y})";
            return cell;
        }

        private Tile[,] GenerateTileGrid()
        {
            var result = new Tile[_columns, _rows];
            for (var x = 0; x < _columns; x++)
            {
                for (var z = 0; z < _rows; z++)
                {
                    var position = new Vector3(x - _columns * 0.5f, 0, z - _rows * 0.5f);
                    var index = new IntVector2(x, z);
                    var type = Random.Range(0, _tileSpecifications.Length);
                    var spec = _tileSpecifications[type];
                    result[x, z] = new Tile(position, index, type, spec);
                }
            }

            return result;
        }
        
        private struct Tile
        {
            public readonly Vector3 Position;
            public readonly IntVector2 Index;
            public readonly int Type;
            public readonly TileSpecification Specification;

            public Tile(Vector3 position, IntVector2 index, int type, TileSpecification specification)
            {
                Position = position;
                Index = index;
                Specification = specification;
                Type = type;
            }
        }
    }

    [Serializable]
    public struct TileSpecification
    {
        public float OwnHeight;
        public float SameTypeHeight;
        public float Falloff;
        public Mesh[] TerrainMeshes;
    }
}