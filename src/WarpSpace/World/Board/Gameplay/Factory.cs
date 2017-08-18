using System;
using System.Linq;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Planet.Tiles;

namespace WarpSpace.World.Board.Gameplay
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] GameObject _tilePrefab;

        public void Recreate(Map map)
        {
            this.DestroyChildren();
            var tiles = CreateTiles();
            InitializeLayer();

            Tile[,] CreateTiles()
            {
                var d = map.Tiles.GetDimensions();
            
                return map.Tiles.Map((t, i) =>
                {
                    var tile = TileCreation.CreateTile(_tilePrefab, i, d, transform);

                    var gameplayTile = tile.GetComponent<Tile>();
                    gameplayTile.Init(t.Type, i);

                    return gameplayTile;
                });
            }
            
            void InitializeLayer()
            {
                var entranceIndex = map.MapObjects.First(x => x.Type == MapObjectType.Entrance).Index;
                
                FindObjectOfType<Layer>().Init(tiles, tiles.Get(entranceIndex));
            }
        }
    }

    public enum Passability
    {
        Free,
        Penalty,
        None
    }
}