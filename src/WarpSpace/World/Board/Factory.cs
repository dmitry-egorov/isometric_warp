using System;
using System.Linq;
using Lanski.Structures;
using UnityEditor.Expose;
using UnityEngine;
using WarpSpace.Planet.Tiles;

namespace WarpSpace.World.Board
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] bool _usePredefinedMap;
        [SerializeField] int _predefinedMapIndex;

        [NonSerialized] private bool _initialized;

        [NonSerialized] private string _lastMap;
        
        private TilesGenerator2 _landscapeGenerator;
        private Water.Factory _waterFactory;
        private Gameplay.Factory _gameplayFactory;
        private PredefinedMapsHolder _predefinedMapsHolder;
        private RandomMapGenerator _randomMapGenerator;

        public void Awake() => Initialize();

        [ExposeMethodInEditor]
        public void Create()
        {
            Initialize();
            
            var map = CreateMap();

            _lastMap = MapToString(map);

            _landscapeGenerator.RecreateTiles(map);
            _waterFactory.RecreateTiles(map);
            _gameplayFactory.Recreate(map);

            Map CreateMap()
            {
                return _usePredefinedMap 
                    ? _predefinedMapsHolder.GetMap(_predefinedMapIndex) 
                    : _randomMapGenerator.GenerateRandomMap();
            }
        }

        private void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;
            
            _landscapeGenerator = FindObjectOfType<TilesGenerator2>();
            _waterFactory = FindObjectOfType<Water.Factory>();
            _gameplayFactory = FindObjectOfType<Gameplay.Factory>();
            _predefinedMapsHolder = FindObjectOfType<PredefinedMapsHolder>();
            _randomMapGenerator = new RandomMapGenerator();
        }

        private string MapToString(Map map)
        {
            var rows = map
                .Tiles
                .EnumerateRows()
                .Select(row => string.Join(" ", row
                    .Select(t => t.Type.ToChar().ToString())
                    .ToArray()))
                .ToArray();
            
            return string.Join("\n", rows);
        }
    }
}