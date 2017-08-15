using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Lanski.Geometry;
using Lanski.Structures;
using UnityEditor.Expose;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WarpSpace.Planet.Tiles
{
    
    public class TilesGenerator: MonoBehaviour
    {
        [SerializeField] GameObject _tilePrefab;
        [SerializeField] TileModels[] _tileModelPrefabs; 
        
        [SerializeField, Range(1, 64)] int _rows; 
        [SerializeField, Range(1, 64)] int _columns; 
        
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
            var tiles = GenerateTileGridPositions();

            for (var x = 0; x < tiles.GetLength(0); x++)
            for (var z = 0; z < tiles.GetLength(1); z++)
            {
                var t = tiles[x, z];
                var tile = CreateTile(t);

                var modelAnchor = tile.GetComponent<TilePartsHolder>().ModelAnchor;

                var model = _tileModelPrefabs[t.Type];
                var selection = model.Select(
                    CheckForNeighbour(tiles, x - 1, z    , t.Type),
                    CheckForNeighbour(tiles, x    , z + 1, t.Type),
                    CheckForNeighbour(tiles, x + 1, z    , t.Type),
                    CheckForNeighbour(tiles, x,     z - 1, t.Type),
                    CheckForNeighbour(tiles, x - 1, z + 1, t.Type),
                    CheckForNeighbour(tiles, x + 1, z + 1, t.Type),
                    CheckForNeighbour(tiles, x + 1, z - 1, t.Type),
                    CheckForNeighbour(tiles, x - 1, z - 1, t.Type)
                    );
                
                var modelInstance = Instantiate(selection.Prefab, modelAnchor.transform);
                modelInstance.transform.localPosition = Vector3.zero;
                modelInstance.transform.localRotation = Quaternion.Euler(0, selection.Rotation * 90, 0);
                modelInstance.transform.localScale = 
                    selection.Mirror == MirrorAxis.None ? Vector3.one
                  : selection.Mirror == MirrorAxis.X    ? new Vector3(-1, 1, 1)
                  : selection.Mirror == MirrorAxis.Z    ? new Vector3(1, 1, -1)
                  : new Vector3(-1, 1, -1);
            }
        }

        private static bool CheckForNeighbour(Tile[,] tiles, int x, int z, int type)
        {
            return tiles.Is(x, z, s => s.Type == type, true);
        }

        private GameObject CreateTile(Tile t)
        {
            var tile = Instantiate(_tilePrefab, transform);
            var index = t.Index;
            tile.transform.localPosition = t.Position;
            tile.name = $"Tile ({index.X}, {index.Y})";
            return tile;
        }

        private Tile[,] GenerateTileGridPositions()
        {
            var result = new Tile[_columns, _rows];
            for (var x = 0; x < _columns; x++)
            {
                for (var z = 0; z < _rows; z++)
                {
                    var position = new Vector3(x - _columns * 0.5f, 0, z - _rows * 0.5f);
                    var index = new IntVector2(x, z);
                    var type = Random.Range(0, _tileModelPrefabs.Length);
                    result[x, z] = new Tile(position, index, type);
                }
            }

            return result;
        }
        
        private struct Tile
        {
            public readonly Vector3 Position;
            public readonly IntVector2 Index;
            public readonly int Type;

            public Tile(Vector3 position, IntVector2 index, int type)
            {
                Position = position;
                Index = index;
                Type = type;
            }
        }
    }
    
    internal static class TielModelsExtensions
    {
        public static ModelSelection Select(this TileModels models, bool n0, bool n1, bool n2, bool n3, bool n01, bool n12, bool n23, bool n30)
        {
            var neighbours = new[] {n0, n1, n2, n3};
            var corners = new bool[4, 4];
            corners[0, 1] = corners[1, 0] = n01;
            corners[1, 2] = corners[2, 1] = n12;
            corners[2, 3] = corners[3, 2] = n23;
            corners[3, 0] = corners[0, 3] = n30;

            var allCorners = new[] {n01, n12, n23, n30};
            
            var neighboursCount = neighbours.Count(x => x);
            var a0 = models.Adjacent0;
            if (a0 == null)
                throw new InvalidOperationException("At least one tile model is required");
            
            var a1 = models.Adjacent1 != null ? models.Adjacent1 : a0;
            var a2o = models.Adjacent2Opposite != null ? models.Adjacent2Opposite : a0;
            var a2 = models.Adjacent2 != null ? models.Adjacent2 : a0;
            var a3 = models.Adjacent3 != null ? models.Adjacent3 : a0;
            var a4 = models.Adjacent4 != null ? models.Adjacent4 : a0;
            var a2c1 = models.Adjacent2Corner1 != null ? models.Adjacent2Corner1 : a0;
            var a3c1 = models.Adjacent3Corner1 != null ? models.Adjacent3Corner1 : a0;
            var a3c2 = models.Adjacent3Corner2 != null ? models.Adjacent3Corner2 : a0;
            var a4c1 = models.Adjacent4Corner1 != null ? models.Adjacent4Corner1 : a0;
            var a4c2 = models.Adjacent4Corner2 != null ? models.Adjacent4Corner2 : a0;
            var a4c2o = models.Adjacent4Corner2Opposite != null ? models.Adjacent4Corner2Opposite : a0;
            var a4c3 = models.Adjacent4Corner3 != null ? models.Adjacent4Corner3 : a0;
            var a4c4 = models.Adjacent4Corner4 != null ? models.Adjacent4Corner4 : a0;

            if (neighboursCount == 0)
            {
                return new ModelSelection(a0, RandomRotation(), RandomZMirror());
            }

            if (neighboursCount == 1)
            {
                var index = neighbours.Select((n, i) => new {n, i}).First(x => x.n).i;
                return new ModelSelection(a1, index, RandomZMirror());
            }

            if (neighboursCount == 2)
            {
                if (n0 && n2)
                    return new ModelSelection(a2o, Random.Range(0, 2) * 2, RandomZMirror());
                
                if (n1 && n3)
                    return new ModelSelection(a2o, Random.Range(0, 2) * 2 + 1, RandomZMirror());

                var index = Enumerable.Range(0, 4).First(i => neighbours[i] && neighbours[Mod4(i + 1)]);
                
                return new ModelSelection(corners[index, Mod4(index + 1)] ? a2c1 : a2, index, MirrorAxis.None);
            }

            if (neighboursCount == 3)
            {
                var i = Enumerable.Range(0, 4).First(x => !neighbours[Mod4(x - 1)]);
                
                var i1 = Mod4(i + 1);
                var i2 = Mod4(i + 2);

                var c01 = corners[i, i1];
                var c12 = corners[i1, i2];
                        
                if (c01 && c12)
                    return new ModelSelection(a3c2, i, RandomXMirror());
                        
                if (c01)
                    return new ModelSelection(a3c1, i, MirrorAxis.None);
                        
                if (c12)
                    return new ModelSelection(a3c1, i, MirrorAxis.X);
                    
                return new ModelSelection(a3, i, RandomXMirror());
            }
            
            // (neighboursCount == 4)
            {
                var cornerCount = allCorners.Count(c => c);
            
                if (cornerCount == 0)
                    return new ModelSelection(a4, RandomRotation(), RandomZMirror());

                if (cornerCount == 1)
                {
                    var index = allCorners.Select((c, i) => new {c, i}).First(x => x.c).i;
                    return new ModelSelection(a4c1, index, MirrorAxis.None);
                }

                if (cornerCount == 2)
                {
                    if (n01 && n23)
                        return new ModelSelection(a4c2o, 0, RandomXZMirror());
                
                    if (n12 && n30)
                        return new ModelSelection(a4c2o, 1, RandomXZMirror());

                    var index = Enumerable.Range(0, 4).First(i => corners[i, Mod4(i + 1)] && corners[Mod4(i + 1), Mod4(i + 2)]);
                
                    return new ModelSelection(a4c2, index, RandomXMirror());
                }

                if (cornerCount == 3)
                {
                    var index = Enumerable.Range(0, 4).First(i => !corners[Mod4(i - 1), i]);
                
                    return new ModelSelection(a4c3, index, MirrorAxis.None);
                }
            
                // (cornerCount == 4)
                return new ModelSelection(a4c4, RandomRotation(), RandomZMirror()); 
            }
        }

        private static int RandomRotation()
        {
            return Random.Range(0, 4);
        }

        private static int Mod4(int i)
        {
            if (i == -1)
                return 3;
            
            return i % 4;
        }

        private static MirrorAxis RandomXZMirror()
        {
            return Random.Range(0, 2) == 0 ? MirrorAxis.None : MirrorAxis.XZ;
        }
        
        private static MirrorAxis RandomXMirror()
        {
            return Random.Range(0, 2) == 0 ? MirrorAxis.None : MirrorAxis.X;
        }

        private static MirrorAxis RandomZMirror()
        {
            return Random.Range(0, 2) == 0 ? MirrorAxis.None : MirrorAxis.Z;
        }

        internal struct ModelSelection
        {
            public readonly GameObject Prefab;
            public readonly int Rotation;
            public readonly MirrorAxis Mirror;

            public ModelSelection(GameObject prefab, int rotation, MirrorAxis mirror)
            {
                Prefab = prefab;
                Rotation = rotation;
                Mirror = mirror;
            }
        }
    }

    internal enum MirrorAxis
    {
        X,
        Z,
        XZ,
        None
    }

    [Serializable]
    public struct TileModels
    {
        public GameObject Adjacent0;
        public GameObject Adjacent1;
        public GameObject Adjacent2Opposite;
        public GameObject Adjacent2;
        public GameObject Adjacent3;
        public GameObject Adjacent4;
        
        public GameObject Adjacent2Corner1;
        public GameObject Adjacent3Corner1;
        public GameObject Adjacent3Corner2;
        public GameObject Adjacent4Corner1;
        public GameObject Adjacent4Corner2;
        public GameObject Adjacent4Corner2Opposite;
        public GameObject Adjacent4Corner3;
        public GameObject Adjacent4Corner4;
    }

    
}